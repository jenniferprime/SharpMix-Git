using System;
using SharpMix.Linux.Config;
using SharpMix.Linux.Cli.Model;
using SharpMix.Common.Module;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SharpMix.Linux.Cli.Handler
{
    public class AseqdumpHandlerCC
    {
        private AseqdumpPortInfo _portInfo;
        private Process _aseqdumpProcess;

        private VirtualMidiMap _virtualMidiMap;

        private int _aseqdumpFrames = 0;
        public int AseqdumpFrames { get => _aseqdumpFrames; }

        public AseqdumpHandlerCC(AseqdumpPortInfo portInfo, VirtualMidiMap virtualMidiMap)
        {
            _portInfo = portInfo;
            _virtualMidiMap = virtualMidiMap;

            _aseqdumpProcess = new Process();
            _aseqdumpProcess.StartInfo = new ProcessStartInfo {
                FileName = SMConfig.ASEQDUMP,
                Arguments = $"{SMConfig.ASEQDUMP_PORT}\"{portInfo.ClientName}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };


        }

        public delegate void CCArrivedEventhandler(object sender, int CC);
        public event CCArrivedEventhandler CCArrived;

        public void StartAseqdump()
        {
            _aseqdumpProcess.Start();
            _aseqdumpProcess.OutputDataReceived += _aseqdumpProcess_OutputDataReceived;
            _aseqdumpProcess.BeginOutputReadLine();
        }

        public void StopAseqdump()
        {
            _aseqdumpProcess.Kill();
        }

        public int Value = 0;

        void _aseqdumpProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _aseqdumpFrames++;

            // " 28:0   Control change          0, controller 0, value 0"

            string line = e.Data;

            if (line.Contains("Control change"))
            {
                string trimmed = Regex.Replace(line, "\\s+", ";");
                trimmed = Regex.Replace(trimmed, "^;", "");
                trimmed = trimmed.Replace(",", "");
                trimmed = trimmed.Replace("Control;change", "CC");

                string[] ccEvent = trimmed.Split(';');
                int cc = 0;
                int value = 0;

                int.TryParse(ccEvent[4], out cc);
                int.TryParse(ccEvent[6], out value);

                _virtualMidiMap.CCAction(cc, value);
                CCArrived?.Invoke(this, cc);
            }
        }

    }
}
