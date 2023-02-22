using System;
using SharpMix.Linux.Actions;
using SharpMix.Linux.Cli.Model.PulseAudio;

namespace SharpMix.Common.Module.MappedActions
{
    public class MappedFader : MidiMappedAction
    {
        private MappedTypeFader mappedTypeFader;

        private PulseDevice pulseDevice;



        public MappedFader(MappedTypeAction mappedType, PulseDevice pulseDevice, MappedTypeFader mappedTypeFader) : base(MappedTypeAction.MappedFader)
        {
            this.mappedTypeFader = (SharpMix.Common.Module.MappedActions.MappedTypeFader)mappedTypeFader;
            this.pulseDevice = pulseDevice;
            this._virtuallyMapped = false;
        }

        public MappedFader(string VirtualMapID, MappedTypeFader mappedTypeFader) : base(MappedTypeAction.MappedFader)
        {
            this._virtuallyMapped = true;
        }


        public override bool ExecuteAction(int CCValue, int CC)
        {
            switch (mappedTypeFader) {
                case MappedTypeFader.PulseDeviceVolume:
                    return PulseAudio.VolumeDevice(pulseDevice, CCValue);
                    

                default: return false;
            }
        }
    }
}
