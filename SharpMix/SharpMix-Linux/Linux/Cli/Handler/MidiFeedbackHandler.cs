using System;
using System.Diagnostics;

using SharpMix.Linux.Config;

namespace SharpMix.Linux.Cli.Handler
{
    public class MidiFeedbackHandler
    {
        public MidiFeedbackHandler()
        {
        }

        public static bool SendMidiFeedback(int Channel, int CC, int CCValue)
        {
            return false;
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "";
                process.StartInfo.Arguments = "";
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                return true;
            }
            catch (Exception ea)
            {
                return false;
            }
        }

    }
}
