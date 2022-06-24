namespace WOTWLevelEditor.Objects
{
    /// <summary>
    /// Stores the raw data of unparsed objects.
    /// </summary>
    public class UnknownFallback : UnityObject
    {
        public byte[] Data { get; }
        public UnknownFallback(Level level, int id, byte[] data) : base(level, id)
        {
            Data = data;
        }

        public static UnknownFallback Parse(Level level, int id, byte[] bytes)
        {
            return new UnknownFallback(level, id, bytes);
        }

        public override string ToString()
        {
            return "Unknown";
        }
    }
}
