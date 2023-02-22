using System;
namespace SharpMix.Common.Module.MappedActions
{
    public enum MappedType
    {

    }

    public enum MappedTypeFader
    {
        PulseVolume,
        PulseVolumeProcess,
        Brightness,
        PulseBalance,
    }

    public enum MappedTypeToggle
    {
        PulseMute,
        PulseMuteProcess,
        
    }

    public enum MappedTypeAction
    {
        PulseMute,
        PulseMuteProcess,
        SetPID,
        MediaPlayPause,
        MediaPlay,
        MediaPause,
        MediaNext,
        MediaPrevious,


    }

    public enum MappedTypeLayer
    {
        GotoLayer,
        LayerUp,
        LayerDown
    }
}
