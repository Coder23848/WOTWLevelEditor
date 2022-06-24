namespace WOTWLevelEditor
{
    /// <summary>
    /// Represents a Unity script type.
    /// </summary>
    public class ScriptType : ObjectType
    {
        public ScriptType(byte[] pattern)
        {
            if (pattern.Length != 39)
            {
                throw new ArgumentException("Pattern length is 39, not " + pattern.Length);
            }
            Signature = pattern[7..39];
            Type = (ObjectTypes)pattern[0];
            Prefix = pattern[1..7];
        }

        public override byte[] Encode()
        {
            byte[] bytes = new byte[39];
            bytes[0] = (byte)Type;
            Prefix.CopyTo(bytes, 1);
            Signature.CopyTo(bytes, 7);
            return bytes;
        }

        public override string ToString()
        {
            return "[" + BitConverter.ToString(Signature) + "]";
        }

        public override bool Equals(object? obj)
        {
            return obj is ScriptType type &&
                   base.Equals(obj) &&
                   Type == type.Type &&
                   EqualityComparer<byte[]>.Default.Equals(Prefix, type.Prefix) &&
                   EqualityComparer<byte[]>.Default.Equals(Signature, type.Signature);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Type, Prefix, Signature);
        }
    }
}