using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharpMix.Linux.Cli.Model;
using SharpMix.Linux.Config;

namespace SharpMix.Linux.Cli.Handler
{
    public class AseqdumpHandler
    {
        private static ProcessStartInfo _ListCommand = new ProcessStartInfo(SMConfig.ASEQDUMP, SMConfig.ASEQDUMP_LIST);

        private List<AseqdumpPortInfo> _portInfo;

        public List<AseqdumpPortInfo> getPortInfo()
        {
            return _portInfo;
        }

        void RefreshDeviceList()
        {
            _portInfo = new List<AseqdumpPortInfo>();
            _ListCommand.RedirectStandardOutput = true;
            _ListCommand.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo.FileName = "aseqdump";
            process.StartInfo.Arguments = "-l";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();

            // regxr (\s*)(\d*):(\d*)\s*(((\w)*\s)+)\s*((\w*\s)+)

            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.Contains("Port") && line.Contains("Client name") && line.Contains("Port name"))
                {
                    continue;
                }

                string trimmed = Regex.Replace(line, "\\s+", ";");
                trimmed = Regex.Replace(trimmed, "^;", "");

                string[] info = trimmed.Split(';');
                string[] ports = info[0].Split(':');
                _portInfo.Add(new AseqdumpPortInfo(ports[0], ports[1], info[1], info[2]));
            }

        }

        public AseqdumpHandler()
        {
            RefreshDeviceList();
        }
    }
}
