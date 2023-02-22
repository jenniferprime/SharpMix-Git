using System;
using Gtk;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Timers;

//Project Includes
using SharpMixLinux;
using SharpMix.Linux.Cli.Handler;
using SharpMix.Linux.Cli.Model;
using SharpMix.Common.Module;
using SharpMix.Common.Module.MappedActions;
using SharpMix.Linux.Cli.Model.PulseAudio;

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
        MappedTypeAction.SetPID, device, MappedTypeFader.PulseDeviceVolume));

        virtualMidiMap.MapCCtoControl(0, 0 + FADEROFFSET);
        controlID[0 + FADEROFFSET] = s_mainout;

        //map Mute for main output
        virtualMidiMap.MapToggle(48, new MappedToggle(
            MappedTypeAction.SetPID, device, MappedTypeToggle.PulseDeviceMute));
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

    protected void OnBtnListSinkInputClicked(object sender, EventArgs e)
    {
        List<PulseSink> sinks = PulseSink.ListAllSinks();

        SharpMix.Linux.Actions.PulseAudio.VolumeInputSink(sinks[0], (int)s_mainout.Value);

    }




    public static string GenerateMixerUIControlName(int group, string type, string prefix = "s_")
    {
        return $"{prefix}{type}-{group}";
    }

    private void AddMixerControls(int amount)
    {
        for (int i = 1; i < amount+1; i++)
        {
            Gtk.VScale fader = GenerateUIMixerFader(GenerateMixerUIControlName(i, "fader", "mixer_s_"));
            Gtk.Button mute = GenerateUIMixerButton(GenerateMixerUIControlName(i, "btnMute", "mixer_s_"), "M");
            Gtk.Button solo = GenerateUIMixerButton(GenerateMixerUIControlName(i, "btnSolo", "mixer_s_"), "S");
            Gtk.Button attach = GenerateUIMixerButton(GenerateMixerUIControlName(i, "btnAttach", "mixer_s_"), "A");
            Gtk.Label label = GenerateUIMixerLabel(GenerateMixerUIControlName(i, "lTrackLabel", "mixer_s_"), "Config...");


            /*l_tableMixer.Add(label);
            Gtk.Table.TableChild tableChild = (Gtk.Table.TableChild)(l_tableMixer[label]);
            tableChild.TopAttach = ((uint)0);
            tableChild.BottomAttach = ((uint)1);

            tableChild.LeftAttach = (uint)1;
            tableChild.RightAttach = (uint)2;

            tableChild.XOptions = AttachOptions.Fill;
            tableChild.YOptions = AttachOptions.Fill;*/

            // left attach, right, top , bottom: left and right attach stay constant per group and top/bottom constant
            l_tableMixer.Attach(attach, (uint)i, (uint)i+1, 4, 5);
            l_tableMixer.Attach(label,  (uint)i, (uint)i+1, 0, 1);
            l_tableMixer.Attach(solo,   (uint)i, (uint)i+1, 3, 4);
            l_tableMixer.Attach(mute,   (uint)i, (uint)i+1, 2, 3);
            l_tableMixer.Attach(fader,  (uint)i, (uint)i+1, 1, 2);

            //(Gtk.Table.TableChild)(l_tableMixer[label])
            Gtk.Table.TableChild lableC = (Gtk.Table.TableChild)(l_tableMixer[label]);
            lableC.XOptions = AttachOptions.Fill;
            lableC.YOptions = AttachOptions.Fill;

            Gtk.Table.TableChild attachC = (Gtk.Table.TableChild)(l_tableMixer[attach]);
            attachC.XOptions = AttachOptions.Fill;
            attachC.YOptions = AttachOptions.Fill;

            Gtk.Table.TableChild faderC = (Gtk.Table.TableChild)(l_tableMixer[fader]);
            faderC.XOptions = AttachOptions.Fill;
            faderC.YOptions = AttachOptions.Fill;

            Gtk.Table.TableChild muteC = (Gtk.Table.TableChild)(l_tableMixer[mute]);
            muteC.XOptions = AttachOptions.Fill;
            muteC.YOptions = AttachOptions.Fill;

            Gtk.Table.TableChild soloC = (Gtk.Table.TableChild)(l_tableMixer[solo]);
            soloC.XOptions = AttachOptions.Fill;
            soloC.YOptions = AttachOptions.Fill;



            label.Show();
            attach.Show();
            solo.Show();
            mute.Show();
            fader.Show();
        }
    }

    private Gtk.VScale GenerateUIMixerFader(string name)
    {
        global::Gtk.VScale fader;
        fader = new global::Gtk.VScale(null);
        fader.HeightRequest = 200;
        fader.CanFocus = true;
        fader.Name = name;
        fader.Inverted = true;
        fader.Adjustment.Upper = 127D;
        fader.Adjustment.PageIncrement = 1D;
        fader.Adjustment.StepIncrement = 1D;
        fader.Adjustment.Value = 99D;
        fader.DrawValue = true;
        fader.Digits = 0;
        fader.ValuePos = ((global::Gtk.PositionType)(2));
        fader.WidthRequest = 50;

        return fader;
    }

    private Gtk.Button GenerateUIMixerButton(string name, string Text)
    {
        global::Gtk.Button btn;
        btn = new global::Gtk.Button();
        btn.CanFocus = true;
        btn.Name = name;
        btn.Label = global::Mono.Unix.Catalog.GetString(Text);
        btn.WidthRequest = 50;
        btn.HeightRequest = 50;

        return btn;
    }

    private Gtk.Label GenerateUIMixerLabel(string name, string Text)
    {
        //TODO: this needs to be actually made like the label on the Master
        Gtk.Label label;
        label = new global::Gtk.Label();
        label.Name = name;
        label.LabelProp = Text;
        label.WidthRequest = 50;
        label.HeightRequest = 50;
        label.Wrap = true;
        label.LineWrapMode = Pango.WrapMode.Char;
        return label;
    }

    bool fadersAdded = false;
    protected void OnBtnAddFadersClicked(object sender, EventArgs e)
    {
        if (fadersAdded) return;
        AddMixerControls(8);
        fadersAdded = true;
    }
}
