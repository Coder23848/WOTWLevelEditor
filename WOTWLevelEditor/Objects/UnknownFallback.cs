using System.Diagnostics;

namespace WOTWLevelEditor.Objects
{
    /// <summary>
    /// Stores the raw data of unparsed objects.
    /// </summary>
    public class UnknownFallback : UnityObject
    {
        public byte[] Data { get; }
        public UnknownFallback(Level level, ObjectType type, int id, byte[] data) : base(level, type, id, Array.Empty<object>())
        {
            Data = data;
        }

        public override List<ObjectID> GetReferences()
        {
            Debug.WriteLine(ThisType.ToString() + " does not support " + nameof(GetReferences));
            return new();
        }

        public override void ConvertReferences(Dictionary<ObjectID, ObjectID> conversionTable)
        {
            Debug.WriteLine(ThisType.ToString() + " does not support " + nameof(ConvertReferences));
        }

        public override string ToString()
        {
            return "Unknown";
        }
    }
}
