using System;
using SharpMix.Linux.Actions;
using SharpMix.Linux.Cli.Model.PulseAudio;

namespace SharpMix.Common.Module.MappedActions
{
    public class MappedToggle : MidiMappedAction
    {
        private MappedTypeToggle mappedTypeToggle;
        private PulseDevice pulseDevice;

        private bool feedback = true; //TODO: fix overwriting and shit

        private int currentValue = 0;

        public MappedToggle(MappedTypeAction mappedType, PulseDevice pulseDevice, MappedTypeToggle mappedTypeToggle) : base(mappedType)
        {
            this.pulseDevice = pulseDevice;
            this.mappedTypeToggle = mappedTypeToggle;
        }

        public override bool ExecuteAction(int CCValue, int CC)
        {
            if (!(SharpMix.Linux.Config.SMConfig.ONLYTOGGLEON127 && CCValue == 127))
            { return false; }
            switch (mappedTypeToggle)
            {
                case MappedTypeToggle.PulseDeviceMute:
                    currentValue = 127 - currentValue;

                    return PulseAudio.MuteDevice(pulseDevice, currentValue);


                default: return false;
            }
        }

        private void SendFeedback(int CC, int CCFeedbackValue)
        {
        }
    }
}
