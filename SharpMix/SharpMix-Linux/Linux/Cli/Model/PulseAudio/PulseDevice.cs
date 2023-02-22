using System;
namespace SharpMix.Linux.Cli.Model.PulseAudio
{
    public class PulseDevice
    {
        private string _sinkName;

        public string SinkName { get => _sinkName; }

        public PulseDevice()
        {
        }

        public PulseDevice(String SinkName)
        {
            _sinkName = SinkName;
        }

        public static PulseDevice PulseDefaultOut = new PulseDevice("@DEFAULT_SINK@");
    }
}
