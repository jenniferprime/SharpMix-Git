using System;
using System.Collections.Generic;
using System.IO;
using Gtk;
using SharpMix.Linux.Cli.Handler;
using SharpMix.Linux.Cli.Model;

namespace SharpMixLinux
{
    public partial class MidiDeviceDialog : Gtk.Dialog
    {
        private AseqdumpPortInfo _selectedPort;
        private string _selectedMapFile;
        private string _dialogResult;

        private bool _deviceListValid = false;
        private bool _mapListValid = false;


        public AseqdumpPortInfo SelectedPort { get => _selectedPort; }
        public string DialogResult { get => _dialogResult; }
        public string SelectedMapFile { get => _selectedMapFile; }

        public MidiDeviceDialog()
        {
            this.Build();

            GenerateDeviceList();
            GenerateMapList();
        }


        List<AseqdumpPortInfo> portInfo;
        void GenerateDeviceList()
        {
            AseqdumpHandler handler = new AseqdumpHandler();
            portInfo = handler.getPortInfo();

            ((ListStore)cb_MidiDevice.Model).Clear();

            if (portInfo.Count > 0)
            {
                foreach (AseqdumpPortInfo port in portInfo)
                {
                    ((ListStore)cb_MidiDevice.Model).AppendValues(port.ToString());
                }
                _deviceListValid = true;
            }
            else
            {
                ((ListStore)cb_MidiDevice.Model).AppendValues("None");
                _deviceListValid = false;
            }
            cb_MidiDevice.Active = 0;
        }


        List<string> files;



        void GenerateMapList()
        {
            ((ListStore)cb_MidiMap.Model).Clear();
            if (!Directory.Exists(SharpMix.Linux.Config.SMConfig.MAPDIRECTORY))
            {
                ((ListStore)cb_MidiMap.Model).AppendValues("Map Directory Missing");
                _mapListValid = false;
                if (SharpMix.Linux.Config.SMConfig.CREATENEWFOLDERS) // the warning is fine since debugging and stuff
                {
                    Directory.CreateDirectory(SharpMix.Linux.Config.SMConfig.MAPDIRECTORY);
                }
                return;
            }

            files = new List<string>();

            foreach (string file in Directory.EnumerateFiles(SharpMix.Linux.Config.SMConfig.MAPDIRECTORY))
            {
                if (file.Contains(SharpMix.Linux.Config.SMConfig.MAPFILEEXT))
                {
                    files.Add(file);
                }
            }
            _mapListValid = files.Count > 0;
            if (_mapListValid)
            {
                //this is extra to read properties (like name and shit) from the things when adding it to the list
                foreach (string file in files)
                {
                    ((ListStore)cb_MidiMap.Model).AppendValues(file);
                }
                cb_MidiDevice.Active = 0;
                return;
            }
            ((ListStore)cb_MidiMap.Model).AppendValues("Map Directory is empty");
            cb_MidiDevice.Active = 0;
        }

        protected void OnButtonOkClicked(object sender, EventArgs e)
        {
            if (_deviceListValid && _mapListValid && portInfo != null && files != null)
            {
                _selectedPort = portInfo[cb_MidiDevice.Active];
                _selectedMapFile = files[cb_MidiMap.Active];
                _dialogResult = "Ok";
            }
            else
            {
                MessageDialog message = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
                    "Error in the selection thingy, sorry for no specifics I'm high on tiredness rn.");
                message.Run();
                message.Destroy();
                _dialogResult = "Error";

                //TODO: apparently the OK button closes the thing now anyways even tho it didn't before?????
                //I truly love GTK
            }
        }

        protected void OnButtonCancelClicked(object sender, EventArgs e)
        {
            _dialogResult = "Cancel";
        }
    }
}
