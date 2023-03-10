using System;
using SharpMix.Linux.Config;
using SharpMix.Linux.Cli.Model.PulseAudio;
using System.Diagnostics;

namespace SharpMix.Linux.Actions
{
    public class PulseAudio
    {

        private static bool runPulseCMD(string Arguments, int timeout = 100)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = SMConfig.PULSEAUDIOCTL;
                process.StartInfo.Arguments = Arguments;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.WaitForExit(timeout);
                return true;
            }
            catch (Exception ea)
            {
                return false;
            }
        }

        public static bool VolumeDevice(PulseDevice pulseDevice, int CCValue)
        {
            try
            {
                int volume = CCValue << 9;

                Process process = new Process();
                process.StartInfo.FileName = SMConfig.PULSEAUDIOCTL;
                process.StartInfo.Arguments = SMConfig.PULSEAUDIOCTL_SETDEVICEVOLUME + pulseDevice.SinkName + $" {volume}";
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                return true;
            }
            catch (Exception ea)
            {
                return false;
            }
            //TODO: try/catch real inefficient here and also maybe add like pulseaudio feedback
        }

        public static bool MuteDevice(PulseDevice pulseDevice)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = SMConfig.PULSEAUDIOCTL;
                process.StartInfo.Arguments = SMConfig.PULSEAUDIOCTL_SETDEVICEMUTE + pulseDevice.SinkName + $" {SMConfig.PULSEAUDIOCTL_SETDEVICEMUTETOGGLE}";
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                return true;
            }
            catch (Exception ea)
            {
                return false;
            }
            //TODO: try/catch real inefficient here and also maybe add like pulseaudio feedback
        }

        public static bool MuteDevice(PulseDevice pulseDevice, int CCvalue)
        {
            CCvalue = Math.Min(1, CCvalue);
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = SMConfig.PULSEAUDIOCTL;
                process.StartInfo.Arguments = SMConfig.PULSEAUDIOCTL_SETDEVICEMUTE + pulseDevice.SinkName + $" {CCvalue}";
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                return true;
            }
            catch (Exception ea)
            {
                return false;
            }
            //TODO: try/catch real inefficient here and also maybe add like pulseaudio feedback
        }

        public static bool VolumeProcess()
        {
            return false;
        }

        public static bool VolumeInputSink(PulseSink pulseSink, int CCValue)
        {
            int volume = CCValue << 9;
            return runPulseCMD($"{SMConfig.PULSEAUDIOCTL_SETINPUTSINKVOLUME} {pulseSink.SinkID} {volume}");


            //TODO: try/catch real inefficient here and also maybe add like pulseaudio feedback
        }

        public static bool MuteInputSink()
        { return false; }

        public static void idkmanthisjustheresoimember()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "",
                    Arguments = "",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
        }
    }
}
