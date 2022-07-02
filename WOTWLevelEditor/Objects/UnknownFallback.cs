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

        public override string ToString()
        {
            return "Unknown";
        }
    }
}
