namespace WOTWLevelEditor
{
    /// <summary>
    /// Represents a Unity script type.
    /// </summary>
    public class ScriptType : ObjectType
    {
        byte[] Pattern { get; }

        public ScriptType(byte[] pattern)
        {
            if (pattern.Length != 39)
            {
                throw new ArgumentException("Pattern length is 39, not " + pattern.Length);
            }
            Pattern = pattern[7..39];
            Type = (ObjectTypes)pattern[0];
            Prefix = pattern[1..7];
        }

        public override string ToString()
        {
            return "[" + BitConverter.ToString(Pattern) + "]";
        }
    }
}