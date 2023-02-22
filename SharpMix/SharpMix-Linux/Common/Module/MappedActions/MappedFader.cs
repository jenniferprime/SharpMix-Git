using System;
using SharpMix.Linux.Actions;
using SharpMix.Linux.Cli.Model.PulseAudio;

namespace SharpMix.Common.Module.MappedActions
{
    public class MappedFader : MidiMappedAction
    {
        private MappedTypeFader mappedTypeFader;

        private PulseDevice pulseDevice;



        public MappedFader(MappedTypeAction mappedType, PulseDevice pulseDevice, MappedTypeFader mappedTypeFader) : base(mappedType)
        {
            this.mappedTypeFader = (SharpMix.Common.Module.MappedActions.MappedTypeFader)mappedTypeFader;
            this.pulseDevice = pulseDevice;
        }


        public override bool ExecuteAction(int CCValue, int CC)
        {
            switch (mappedTypeFader) {
                case MappedTypeFader.PulseVolume:
                    return PulseAudio.VolumeDevice(pulseDevice, CCValue);
                    

                default: return false;
            }
        }
    }
}
