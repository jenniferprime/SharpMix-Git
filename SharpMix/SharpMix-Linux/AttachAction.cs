using System;
using System.Collections.Generic;
using Gtk;

//project includes
using SharpMix.Linux.Cli.Model.PulseAudio;

namespace SharpMixLinux
{
    public partial class AttachAction : Gtk.Dialog
    {
        private bool _deviceListValid = false;

        private PulseSink _selectedSink = null;

        List<PulseSink> sinks;
        List<PulseSink> sinksOut;

        public AttachAction()
        {
            this.Build();

            FillList();
        }

        void FillList()
        {
            sinks = PulseSink.ListAllSinkInputs();
            ((ListStore)c_appSink.Model).Clear();

            if (sinks.Count > 0)
            {
                foreach (PulseSink sink in sinks)
                {
                    ((ListStore)c_appSink.Model).AppendValues(sink.ToString());
                }
                _deviceListValid = true;
            }
            else
            {
                ((ListStore)c_appSink.Model).AppendValues("None");
                _deviceListValid = false;
            }
            c_appSink.Active = 0;



            sinksOut = PulseSink.ListAllSinkOutputs();
            ((ListStore)c_devSink.Model).Clear();

            if (sinks.Count > 0)
            {
                foreach (PulseSink sink in sinksOut)
                {
                    ((ListStore)c_devSink.Model).AppendValues(sink.ToString());
                }
                _deviceListValid = true;
            }
            else
            {
                ((ListStore)c_devSink.Model).AppendValues("None");
                _deviceListValid = false;
            }
            c_devSink.Active = 0;
        }
    }
}
