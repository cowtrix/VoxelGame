using Common;

namespace Generation.Spawning
{
    public class ObjectBin : TrackedObject<ObjectBin> 
    {
        public enum eBinType
        {
            VEHICLE,
            NPC,
        }
        public eBinType BinType;
    }
}