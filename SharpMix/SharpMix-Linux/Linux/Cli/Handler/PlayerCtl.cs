using System;
using System.Diagnostics;

using SharpMix.Linux.Config;
using SharpMix.Linux.Cli.Model;

namespace SharpMix.Linux.Cli.Handler
{
    public class PlayerCtl
    {
        public PlayerCtl()
        {
        }

        public static PlayerCtlStatus GetCtlStatus()
        {
            Process process = new Process();
            process.StartInfo.FileName = SMConfig.PLAYERCTL;
            process.StartInfo.Arguments = SMConfig.PLAYERCTL_STATUS;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();

            // regxr (\s*)(\d*):(\d*)\s*(((\w)*\s)+)\s*((\w*\s)+)

            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.Contains("Playing"))
                {
                    return PlayerCtlStatus.Playing;
                }
                else if (line.Contains("Paused"))
                {
                    return PlayerCtlStatus.Paused;
                }
            }


            return PlayerCtlStatus.Fail;
        }

        public static string GetMetaArtist()
        {
            //TODO: inefficient

            Process process = new Process();
            process.StartInfo.FileName = SMConfig.PLAYERCTL;
            process.StartInfo.Arguments = "metadata xesam:artist";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();

            // regxr (\s*)(\d*):(\d*)\s*(((\w)*\s)+)\s*((\w*\s)+)

            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.Length > 0)
                {
                    return line;
                }
                
            }


            return "-";
        }

        public static string GetMetaTitle()
        {
            //TODO: inefficient

            Process process = new Process();
            process.StartInfo.FileName = SMConfig.PLAYERCTL;
            process.StartInfo.Arguments = "metadata xesam:title";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();

            // regxr (\s*)(\d*):(\d*)\s*(((\w)*\s)+)\s*((\w*\s)+)

            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.Length > 0)
                {
                    return line;
                }

            }


            return "-";
        }

    }
}
