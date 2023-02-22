using System;
using Gtk;
using SharpMixLinux;
using SharpMix.Linux.Cli.Handler;
using SharpMix.Linux.Cli.Model;
using SharpMix.Common.Module;
using SharpMix.Common.Module.MappedActions;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Timers;

public partial class MainWindow : Gtk.Window
{
    const int FADEROFFSET  = 0;
    const int KNOBOFFSET = 100;
    const int BUTTONOFFSET = 200;
    const int TOGGLEOFFSET = 300;

    System.Timers.Timer transportTimer;

    VirtualMidiMap virtualMidiMap;

    Dictionary<int, Widget> controlID;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();


        controlID = new Dictionary<int, Widget>();
        virtualMidiMap = new VirtualMidiMap();
        debugVirtualMidiMap();

        setupTimers();
    }

    public void debugVirtualMidiMap()
    {
        PremappedActions.MapPremappedActions();

        SharpMix.Linux.Cli.Model.PulseAudio.PulseDevice device = SharpMix.Linux.Cli.Model.PulseAudio.PulseDevice.PulseDefaultOut;

        //map main out volume
        virtualMidiMap.MapFader(0, new MappedFader(
        MappedTypeAction.SetPID, device, MappedTypeFader.PulseVolume));

        virtualMidiMap.MapCCtoControl(0, 0 + FADEROFFSET);
        controlID[0 + FADEROFFSET] = s_mainout;

        //map Mute for main output
        virtualMidiMap.MapToggle(48, new MappedToggle(
            MappedTypeAction.SetPID, device, MappedTypeToggle.PulseMute));
        //missing CCtoControl

        //map PlayPause
                //virtualMidiMap.MapAction(41, new MidiMappedAction(MappedTypeAction.MediaPlayPause));
        virtualMidiMap.MapAction(41, PremappedActions.preMappedActions["A:Media:Play"]);
        virtualMidiMap.MapCCtoControl(41, 41 + BUTTONOFFSET);
        controlID[41 + BUTTONOFFSET] = btn_transportPlay;

        virtualMidiMap.MapAction(42, PremappedActions.preMappedActions["A:Media:Pause"]);
        virtualMidiMap.MapCCtoControl(42, 42 + BUTTONOFFSET);
        controlID[42 + BUTTONOFFSET] = btn_transportPause;

        virtualMidiMap.MapAction(43, PremappedActions.preMappedActions["A:Media:Previous"]);
        virtualMidiMap.MapCCtoControl(43, 43 + BUTTONOFFSET);
        controlID[43 + BUTTONOFFSET] = btn_transportPrev;

        virtualMidiMap.MapAction(44, PremappedActions.preMappedActions["A:Media:Next"]);
        virtualMidiMap.MapCCtoControl(44, 44 + BUTTONOFFSET);
        controlID[44 + BUTTONOFFSET] = btn_transportNext;

    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    private void setupTimers()
    {
        //transportcontrol timer
        transportTimer = new System.Timers.Timer(5000);
        transportTimer.Elapsed += (sender, e) => { 
        UpdateTransportControls();
         };



        transportTimer.Start();
    }


    private AseqdumpPortInfo portInfo;
    private string midiMapFileName;

    protected void OnBMidiCFGClicked(object sender, EventArgs e)
    {
        MidiDeviceDialog midiDeviceDialog = new MidiDeviceDialog();
        midiDeviceDialog.Run();
        midiDeviceDialog.Destroy();

        if (midiDeviceDialog.DialogResult == "Ok")
        {
            portInfo = midiDeviceDialog.SelectedPort;
            midiMapFileName = midiDeviceDialog.SelectedMapFile;
        }
    }

    AseqdumpHandlerCC aseqdumpHandlerCC;

    protected void OnBMidiLoadClicked(object sender, EventArgs e)
    {
        if (portInfo != null && midiMapFileName != null)
        {
            MidiMapper mapper = new MidiMapper();
            mapper.LoadMap(midiMapFileName);

            //MessageDialog message = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
            //        $"MidiMap Loaded for {mapper.MidiMap.friendlyName}");
            //message.Run();
            //message.Destroy();

            l_stMidiMap.Text = $"Map: {mapper.MidiMap.friendlyName}";
            l_stMidiDevice.Text = $"Device: {portInfo.ClientName}";
            l_stMidiStatus.Text = "MIDI: listening";


            aseqdumpHandlerCC  = new AseqdumpHandlerCC(portInfo, virtualMidiMap);
            aseqdumpHandlerCC.StartAseqdump();
            aseqdumpHandlerCC.CCArrived += AseqdumpHandlerCC_CCArrived;

        }
    }

    bool updateAllowed = true;

    void AseqdumpHandlerCC_CCArrived(object sender, int CC)
    {
        if (aseqdumpHandlerCC != null && updateAllowed && ck_autoUpdateUI.Active)
        {
            l_frameNumber.Text = aseqdumpHandlerCC.AseqdumpFrames.ToString();
            s_mainout.Value = virtualMidiMap.GetCCValue(CC);
            updateAllowed = false;

            /*IAsyncResult result;
            System.Action action = () =>
            { updateAllowed = true; };

            result = action.BeginInvoke(null, null);*/

            var task = Task.Run(() => { Thread.Sleep(250); s_mainout.Value = virtualMidiMap.GetCCValue(CC); updateAllowed = true; }); //TODO: this is real ugly


        }
    }


    protected void OnButton9Clicked(object sender, EventArgs e)
    {
        if (aseqdumpHandlerCC != null)
        {
            aseqdumpHandlerCC.StopAseqdump();
            l_stMidiStatus.Text = "MIDI: idle";
        }
    }

    protected void OnBSetMainOutClicked(object sender, EventArgs e)
    {
        //SharpMix.Linux.Cli.Model.PulseAudio.PulseDevice device = SharpMix.Linux.Cli.Model.PulseAudio.PulseDevice.PulseDefaultOut;

        //SharpMix.Linux.Actions.PulseAudio.VolumeDevice(device, (int)s_mainout.Value);

        virtualMidiMap.CCAction((virtualMidiMap.GetCCfromControlID(0)), (int)s_mainout.Value);
    }

    protected void OnBtnG1MuteClicked(object sender, EventArgs e)
    {
        //SharpMix.Linux.Cli.Model.PulseAudio.PulseDevice device = SharpMix.Linux.Cli.Model.PulseAudio.PulseDevice.PulseDefaultOut;

        //        SharpMix.Linux.Actions.PulseAudio.MuteDevice(device);

        virtualMidiMap.CCAction(48, 127);
    }

    protected void OnBtnUpdateUIClicked(object sender, EventArgs e)
    {

        //updatemixer
        if (aseqdumpHandlerCC != null)
        {
            l_frameNumber.Text = aseqdumpHandlerCC.AseqdumpFrames.ToString();
            s_mainout.Value = virtualMidiMap.GetCCValue(0);
        }

        UpdateTransportControls();
    }

    private void UpdateTransportControls()
    {
         if (!ck_autoUpdateUI.Active) { return; } //it should be fine without
        //update transport controls
        PlayerCtlStatus status = PlayerCtl.GetCtlStatus();

        switch (status)
        {
            case PlayerCtlStatus.Playing:
                btn_transportPlay.Label = "⏵!";
                btn_transportPause.Label = "⏸";
                break;
            case PlayerCtlStatus.Paused:
                btn_transportPlay.Label = "⏵";
                btn_transportPause.Label = "⏸!";
                break;
        }

        l_mediaTrack.Text = "Title: " + PlayerCtl.GetMetaTitle();
        l_mediaArtist.Text = "Artist:" + PlayerCtl.GetMetaArtist();
    }

        protected void OnBtnTransportPlayClicked(object sender, EventArgs e)
    {
        virtualMidiMap.CCAction(41, 127);
    }

    protected void OnBtnTransportPauseClicked(object sender, EventArgs e)
    {
        virtualMidiMap.CCAction(42, 127);
    }

    protected void OnBtnTransportNextClicked(object sender, EventArgs e)
    {
        virtualMidiMap.CCAction(44, 127);
    }

    protected void OnBtnTransportPrevClicked(object sender, EventArgs e)
    {
        //virtualMidiMap.CCAction(virtualMidiMap.GetCCfromControlID(44+BUTTONOFFSET), 127);
        virtualMidiMap.CCAction(virtualMidiMap.GetCCfromControlID(GetControlIDfromControl((Widget)sender)), 127);
    }

    protected void OnBtnTransportClicked(object sender, EventArgs e)
    {
        //virtualMidiMap.CCAction(virtualMidiMap.GetCCfromControlID(44+BUTTONOFFSET), 127);
        virtualMidiMap.CCAction(virtualMidiMap.GetCCfromControlID(GetControlIDfromControl((Widget)sender)), 127);
    }

    public int GetControlIDfromControl(Widget control)
    {
        //TODO: performance hit and shit
        foreach (KeyValuePair<int, Widget> kvp in controlID)
        {
            if (kvp.Value == control)
            {
                return kvp.Key;
            }
        }
        return -1;
    }
}
