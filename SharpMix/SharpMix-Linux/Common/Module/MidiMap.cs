using System;
using System.Collections.Generic;
namespace SharpMix.Common.Module
{
    public class MidiMap
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class ButtonMap
        {
            public int CC { get; set; }
            public int Control { get; set; }
            public int ON { get; set; }
            public int OFF { get; set; }
            public bool ToggleInHardware { get; set; }
            public bool SimulateToggle { get; set; }
            public string Name { get; set; }
            public string UID { get; set; }
            public bool Feedback { get; set; }
            public int FeedbackOn { get; set; }
            public int FeedbackOff { get; set; }
            public string Type { get; set; }
            public string Function { get; set; }
        }

        public class FaderMap
        {
            public string CC { get; set; }
            public string Range { get; set; }
            public string Name { get; set; }
            public string UID { get; set; }
            public bool Feedback { get; set; }
            public string Type { get; set; }
            public string Function { get; set; }
        }

        public class KnobMap
        {
            public string CC { get; set; }
            public string Range { get; set; }
            public string Name { get; set; }
            public string UID { get; set; }
            public bool Feedback { get; set; }
            public string Type { get; set; }
            public string Function { get; set; }
        }

        public class Root
        {
            public string friendlyName { get; set; }
            public string midiClientName { get; set; }
            public string midiPortName { get; set; }
            public int buttonAmount { get; set; }
            public int faderAmount { get; set; }
            public int knobAmount { get; set; }
            public List<FaderMap> faderMap { get; set; }
            public List<ButtonMap> buttonMap { get; set; }
            public List<KnobMap> knobMap { get; set; }
        }
    }
}
