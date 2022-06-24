namespace WOTWLevelEditor.Objects
{
    /// <summary>
    /// Stores the raw data of unparsed objects.
    /// </summary>
    public class UnknownFallback : UnityObject
    {
        public byte[] Data { get; }
        public UnknownFallback(Level level, ObjectType type, int id, byte[] data) : base(level, type, id)
        {
            Data = data;
        }

        public static UnknownFallback Parse(Level level, ObjectType type, int id, byte[] bytes)
        {
            return new UnknownFallback(level, type, id, bytes);
        }

        public override string ToString()
        {
            return "Unknown";
        }
    }
}
