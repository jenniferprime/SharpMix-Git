using System;
namespace SharpMix.Common.Module.MappedActions
{
    public class MappedLayer : MidiMappedAction
    {
        private MappedTypeLayer mappedTypeLayer;

        public MappedLayer(MappedTypeAction mappedType) : base(mappedType)
        {
        }

        public override bool ExecuteAction(int CCValue, int CC)
        {
            switch (mappedTypeLayer)
            {
                case MappedTypeLayer.LayerUp:
                    return false;
                case MappedTypeLayer.LayerDown:
                    return false;
                case MappedTypeLayer.GotoLayer:
                    return false;


                default: return false;
            }
        }
    }
}
