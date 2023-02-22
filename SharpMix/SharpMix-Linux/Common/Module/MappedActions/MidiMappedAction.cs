using System;

using SharpMix.Linux.Actions;

namespace SharpMix.Common.Module.MappedActions
{
    public class MidiMappedAction
    {
        private MappedTypeAction _mappedType;

        protected bool _feedback;

        public MidiMappedAction(MappedTypeAction mappedType)
        {
            _mappedType = mappedType;
        }

        public virtual bool ExecuteAction(int CCValue, int CC)
        {
            if (!(SharpMix.Linux.Config.SMConfig.ONLYTOGGLEON127 && CCValue == 127))
            { return false; }

            switch (_mappedType)
            {
                case MappedTypeAction.SetPID:
                    return false;
                case MappedTypeAction.MediaPlayPause:
                    return PlayerCtl.PlayPause();
                case MappedTypeAction.MediaPlay:
                    return PlayerCtl.Play();
                case MappedTypeAction.MediaPause:
                    return PlayerCtl.Pause();
                case MappedTypeAction.MediaNext:
                    return PlayerCtl.Next();
                case MappedTypeAction.MediaPrevious:
                    return PlayerCtl.Previous();


                default: return false;
            }
        }
    }
}
