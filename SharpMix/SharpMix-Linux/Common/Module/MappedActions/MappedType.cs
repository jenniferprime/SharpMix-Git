using System;
namespace SharpMix.Common.Module.MappedActions
{
    public enum MappedType
    {

    }

    public enum MappedTypeFader
    {
        PulseDeviceVolume,
        PulseInputSinkVolume,
        Brightness,
        PulseDeviceBalance,
        PulseInputSinkBalance,
    }

    public enum MappedTypeToggle
    {
        PulseDeviceMute,
        PulseInputSinkMute,
        
    }

    public enum MappedTypeAction
    {
        MappedFader,
        MappedToggle,
        MappedLayer,


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
