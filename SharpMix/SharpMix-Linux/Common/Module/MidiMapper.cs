using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SharpMix.Common.Module
{
    public class MidiMapper
    {
        private MidiMap.Root _midiMap;

        public MidiMap.Root MidiMap { get => _midiMap; }

        public bool LoadMap(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                string filecontent = File.ReadAllText(fileName);
                _midiMap = JsonConvert.DeserializeObject<MidiMap.Root>(filecontent);
                return true;
            }
            catch (Exception ea)
            {

            }
            return false;
        }





    }
}
