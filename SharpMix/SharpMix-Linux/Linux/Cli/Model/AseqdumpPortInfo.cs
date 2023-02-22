using System;
namespace SharpMix.Linux.Cli.Model
{
    public class AseqdumpPortInfo
    {
        public string ClientName { get; set; }
        public string PortName { get; set; }
        public int MajorPort { get; set; }
        public int SubPort { get; set; }

        public AseqdumpPortInfo(int MajorPort, int SubPort, string ClientName, string PortName)
        {
            this.ClientName = ClientName;
            this.PortName = PortName;
            this.MajorPort = MajorPort;
            this.SubPort = SubPort;
        }

        public AseqdumpPortInfo(string MajorPort, string SubPort, string ClientName, string PortName)
        {
            int majPort = 0;
            int subPort = 0;

            int.TryParse(MajorPort, out majPort);
            int.TryParse(SubPort, out subPort);

            this.ClientName = ClientName;
            this.PortName = PortName;
            this.MajorPort = majPort;
            this.SubPort = subPort;
        }

        override public String ToString()
        {
            return $"[{MajorPort}:{SubPort}] {ClientName} ({PortName})";
        }
    }
}
