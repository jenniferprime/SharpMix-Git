using System;
namespace SharpMix.Linux.Config
{
    public class SMConfig
    {
        //Config for aseqdump
        public const String ASEQDUMP = "aseqdump";
        public const String ASEQDUMP_LIST = "-l";
        public const String ASEQDUMP_PORT = "-p ";
        public const String ASEQDUMP_PATTERN = @"\b(\s*)(\d*):(\d*)\s*(((\w)*\s)+)\s*((\w*\s)+)\b"; //regexr: (\s*)(\d*):(\d*)\s*(((\w)*\s)+)\s*((\w*\s)+)

        //Config for pulse
        public const String PULSEAUDIOCTL = "pactl";
        public const String PULSEAUDIOCTL_SETDEFAULTOUTVOLUME = "set-sink-volume @DEFAULT_SINK@ ";
        public const String PULSEAUDIOCTL_SETDEVICEVOLUME = "set-sink-volume ";
        public const String PULSEAUDIOCTL_SETDEVICEMUTE = "set-sink-mute ";
        public const String PULSEAUDIOCTL_SETDEVICEMUTETOGGLE = "toggle";

        //Config for PlayerCTL
        public const String PLAYERCTL = "playerctl";
        public const String PLAYERCTL_STATUS = "status";
        public const String PLAYERCTL_LIST = "-l";
        public const String PLAYERCTL_PLAYER = "-p "; //use the thing from -l after, also after the command
        public const String PLAYERCTL_PAUSE = "pause";
        public const String PLAYERCTL_PLAY = "play";
        public const String PLAYERCTL_PLAYPAUSE_TOGGLE = "play-pause";
        public const String PLAYERCTL_NEXT = "next";
        public const String PLAYERCTL_PREVIOUS = "previous";

        //Config
        public const String MAPDIRECTORY = "./midimap";
        public const String MAPFILEEXT = ".map.json";


        //Debugging things
        public const bool CREATENEWFOLDERS = true;

        //MidiConfig
        public const bool ONLYTOGGLEON127 = true;
    }
}
