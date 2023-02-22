using System;
using System.Diagnostics;

using SharpMix.Linux.Config;

namespace SharpMix.Linux.Actions
{
    public class PlayerCtl
    {
        private static bool SetupProcess(String Arguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = SMConfig.PLAYERCTL;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            process.WaitForExit(100);

            return true;
        }


        public static bool PlayPause(String player = "")
        {
            string args = SMConfig.PLAYERCTL_PLAYPAUSE_TOGGLE;
            if (player != "") { args += $" {SMConfig.PLAYERCTL_PLAYER}{player}"; }
            //return false;
            return SetupProcess(args);
        }

        public static bool Play(String player = "")
        {
            string args = SMConfig.PLAYERCTL_PLAY;
            if (player != "") { args += $" {SMConfig.PLAYERCTL_PLAYER}{player}"; }
            //return false;
            return SetupProcess(args);
        }

        public static bool Pause(String player = "")
        {
            string args = SMConfig.PLAYERCTL_PAUSE;
            if (player != "") { args += $" {SMConfig.PLAYERCTL_PLAYER}{player}"; }
            //return false;
            return SetupProcess(args);
        }

        public static bool Next(String player = "")
        {
            string args = SMConfig.PLAYERCTL_NEXT;
            if (player != "") { args += $" {SMConfig.PLAYERCTL_PLAYER}{player}"; }
            //return false;
            return SetupProcess(args);
        }

        public static bool Previous(String player = "")
        {
            string args = SMConfig.PLAYERCTL_PREVIOUS;
            if (player != "") { args += $" {SMConfig.PLAYERCTL_PLAYER}{player}"; }
            //return false;
            return SetupProcess(args);
        }
    }
}
