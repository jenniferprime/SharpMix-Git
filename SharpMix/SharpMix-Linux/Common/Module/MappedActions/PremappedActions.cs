using System;
using System.Collections.Generic;
using SharpMix.Linux.Cli.Model.PulseAudio;

namespace SharpMix.Common.Module.MappedActions
{
    public class PremappedActions
    {
        public static Dictionary<String, MidiMappedAction> preMappedActions;
        public static void MapPremappedActions()
        {
            preMappedActions = new Dictionary<string, MidiMappedAction>();
            PulseDevice pulseDevice = PulseDevice.PulseDefaultOut;

            preMappedActions.Add("A:Audio:Pulse:Output:MasterVolume", new MappedFader(0, pulseDevice, MappedTypeFader.PulseVolume));
            preMappedActions.Add("A:Audio:Pulse:Output:MasterMute", new MappedToggle(0, pulseDevice, MappedTypeToggle.PulseMute));

            preMappedActions.Add("A:Media:PlayPause", new MidiMappedAction(MappedActions.MappedTypeAction.MediaPlayPause));
            preMappedActions.Add("A:Media:Play", new MidiMappedAction(MappedActions.MappedTypeAction.MediaPlay));
            preMappedActions.Add("A:Media:Pause", new MidiMappedAction(MappedActions.MappedTypeAction.MediaPause));
            preMappedActions.Add("A:Media:Next", new MidiMappedAction(MappedActions.MappedTypeAction.MediaNext));
            preMappedActions.Add("A:Media:Previous", new MidiMappedAction(MappedActions.MappedTypeAction.MediaPrevious));

            //preMappedActions.Add("A:Audio:Pulse:Output:MasterVolume", new MidiMappedAction());
        }
    }
}
