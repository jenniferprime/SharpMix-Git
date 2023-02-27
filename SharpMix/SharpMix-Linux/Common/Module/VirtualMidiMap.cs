using System;
using System.Collections.Generic;
using SharpMix.Common.Module.MappedActions;

namespace SharpMix.Common.Module
{
    public class VirtualMidiMap
    {
        //int: CC, MidiMappedAction the thing it does
        int _selectedLayer = 0;

        private Dictionary<int, Dictionary<int, MidiMappedAction>> _mappedControlChange;
        private Dictionary<int, MidiMappedAction> _mappedControlChange2;

        private Dictionary<int, int> _controlChangeValue;

        private Dictionary<int, int> _mapCCtoValue; //CC:Control

        Dictionary<String, int> mappedSink;
        Dictionary<String, int> mappedDevice;

        private List<String[]> _groups;

        private Dictionary<int, int> _currentToggleValues;

        public VirtualMidiMap()
        {
            _mappedControlChange2 = new Dictionary<int, MidiMappedAction>();
            _controlChangeValue = new Dictionary<int, int>();
            _mapCCtoValue = new Dictionary<int, int>();

            _currentToggleValues = new Dictionary<int, int>();

            mappedSink = new Dictionary<string, int>();
            mappedDevice = new Dictionary<string, int>();
        }

        public bool MapAction(int CC, MidiMappedAction mappedAction)
        {
            _mappedControlChange2[CC] = mappedAction;
            return true;
        }

        public bool MapToggle(int CC, MappedToggle mappedToggle)
        {
            _mappedControlChange2[CC] = mappedToggle;
            return true;
        }

        public bool MapFader(int CC, MappedFader mappedFader)
        {
            _mappedControlChange2[CC] = mappedFader;
            return true;
        }

        public void MapCCtoControl(int CC, int ControlID)
        {
            _mapCCtoValue[CC] = ControlID;
        }

        public int GetCCValue(int CC)
        {
            int CCval = -1;
            _controlChangeValue.TryGetValue(CC, out CCval);
            return CCval;
        }

        public int GetControlIDfromCC(int CC)
        {
            int ControlID = -1;
            _mapCCtoValue.TryGetValue(CC, out ControlID);
            return ControlID;
        }

        public int GetCCfromControlID(int ControlID)
        {
            //TODO: performance hit and shit
            foreach (KeyValuePair<int, int> kvp in _mapCCtoValue)
            {
                if (kvp.Value == ControlID)
                {
                    return kvp.Key;
                }
            }
            return -1;
        }

        public void CCAction(int CC, int CCvalue)
        {
            MidiMappedAction action;
            _mappedControlChange2.TryGetValue(CC, out action);

            _controlChangeValue[CC] = CCvalue;

            if(action != null) action.ExecuteAction(CCvalue, CC);
        }

        public int GetNewToggleValue(int CC)
        {
            if (!_currentToggleValues.ContainsKey(CC))
            {
                _currentToggleValues[CC] = 0;
            }

            return _currentToggleValues[CC];
        }
    }
}
