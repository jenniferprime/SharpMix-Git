//Env Includes
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

//Lib Includes

//Project Includes
using SharpMix.Linux.Config;


namespace SharpMix.Linux.Cli.Model.PulseAudio
{
    public class PulseSink
    {
        //locals
        private PulseSinkType _sinkType;
        private int _sinkID;
        private string _driver;
        private string _protoVolume;
        private bool _sinkMuted = false; //TODO: implement this
        //TODO: sink index?

        private string _nodeName;
        private string _applicationName;
        private string _applicationXDisplay;
        private string _applicationUser;
        private string _applicationHost;
        private string _applicationPID;

        //properties
        public int SinkID { get => _sinkID; }

        public PulseSink()
        {
            
        }

        public PulseSink(PulseSinkType sinkType, int sinkID, string driver, string protoVolume, string nodeName, string applicationName, string applicationXDisplay, string applicationUser, string applicationHost, string applicationPID)
        {
            _sinkType = sinkType;
            _sinkID = sinkID;
            _driver = driver;
            _protoVolume = protoVolume;
            _nodeName = nodeName;
            _applicationName = applicationName;
            _applicationXDisplay = applicationXDisplay;
            _applicationUser = applicationUser;
            _applicationHost = applicationHost;
            _applicationPID = applicationPID;
        }

        //this one does nullchecks????
        /*public PulseSink(PulseSinkType sinkType, string sinkID, string driver, string protoVolume, string nodeName, string applicationName, string applicationXDisplay, string applicationUser, string applicationHost, string applicationPID)
        {
            _sinkType = sinkType;
            _sinkID = sinkID ?? throw new ArgumentNullException(nameof(sinkID));
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _protoVolume = protoVolume ?? throw new ArgumentNullException(nameof(protoVolume));
            _nodeName = nodeName ?? throw new ArgumentNullException(nameof(nodeName));
            _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
            _applicationXDisplay = applicationXDisplay ?? throw new ArgumentNullException(nameof(applicationXDisplay));
            _applicationUser = applicationUser ?? throw new ArgumentNullException(nameof(applicationUser));
            _applicationHost = applicationHost ?? throw new ArgumentNullException(nameof(applicationHost));
            _applicationPID = applicationPID ?? throw new ArgumentNullException(nameof(applicationPID));
        }*/

        public static List<PulseSink> ListAllSinks()
        {
            List<PulseSink> sinks = new List<PulseSink>();

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = SMConfig.PULSEAUDIOCTL;
                process.StartInfo.Arguments = SMConfig.PULSEAUDIOCTL_LISTSINKINPUT;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.RedirectStandardOutput = true;

                process.Start();

                int sid = -1;
                PulseSinkType sinkType = PulseSinkType.DefaultSink;
                string driver = ""; // 
                string protoVolume = "";// 
                string nodeName = "";// 
                string applicationName = ""; // 
                string applicationXDisplay = "";  //
                string applicationUser = ""; //
                string applicationHost = "";
                string applicationPID = ""; //

                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    if (line.Contains("Sink Input #"))
                    {
                        if (sid != -1)
                        {
                            sinks.Add(new PulseSink(sinkType, sid, driver, protoVolume, nodeName, applicationName, applicationXDisplay, applicationUser, applicationHost, applicationPID));
                        }

                        driver = ""; protoVolume = ""; nodeName = ""; applicationName = ""; applicationXDisplay = ""; applicationUser = ""; applicationHost = ""; applicationPID = "";
                        sid = -1;
                        string sidRegex = Regex.Replace(line, "\\D", "");
                        int.TryParse(sidRegex, out sid);
                    }
                    else if (line.Contains("Driver: "))
                    {
                        driver = Regex.Replace(line, "\\s*Driver: ", ""); 
                    }
                    else if (line.Contains("Volume: "))
                    {
                       protoVolume = Regex.Replace(line, "\\s*Volume: ", "");
                    }
                    else if (line.Contains("node.name = "))
                    {
                        nodeName = StripPluseListWithEqualsSign(line, "node.name");
                    }
                    else if (line.Contains("application.name"))
                    {
                        applicationName = Regex.Replace(line, "\\s*application.name = ", "");
                        applicationName = applicationName.Replace("\"", "");
                    }
                    else if (line.Contains("window.x11.display"))
                    {
                        applicationXDisplay = StripPluseListWithEqualsSign(line, "window.x11.display");
                    }
                    else if (line.Contains("application.process.user"))
                    {
                        applicationUser = StripPluseListWithEqualsSign(line, "application.process.user");
                    }
                    else if (line.Contains("application.process.host"))
                    {
                        applicationHost = StripPluseListWithEqualsSign(line, "application.process.host");
                    }

                    else if (line.Contains("application.process.id = "))
                    {
                        applicationPID = Regex.Replace(line, "\\D", "");
                        // int.TryParse(sidRegex, out applicationPID);
                        //TODO: make the thing an int lol
                    }

                    /*else if (line.Contains("Volume: "))
                    {
                        protoVolume = Regex.Replace(line, "\\s*Volume: ", "");
                    }*/
                }
                if (sid != -1 && sinks.Count == 0)
                {
                    sinks.Add(new PulseSink(sinkType, sid, driver, protoVolume, nodeName, applicationName, applicationXDisplay, applicationUser, applicationHost, applicationPID));
                }
            }
            catch (Exception ea)
            {

                return null;
            }

            return sinks;
        }

        private static readonly char[] spaceChars = new char[] { '\t', ' ' };

        public static string StripPluseListWithEqualsSign(string line, string value)
        {
            string extractedValue = line.TrimStart(spaceChars).
                Replace(value, "");
            //extractedValue = Regex.Replace(line, "\\s*" + line + " = ", "");
            extractedValue = extractedValue.Replace("\"", "");
            extractedValue = extractedValue.Replace(" = ", "");


            return extractedValue;
        }
    }

    public enum PulseSinkType
    {
        SinkInput,
        SinkOutput,
        DefaultSink
    }
}
