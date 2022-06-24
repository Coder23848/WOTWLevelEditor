namespace WOTWLevelEditor
{
    /// <summary>
    /// Represents a Unity object type.
    /// </summary>
    public class ObjectType
    {
        public ObjectTypes Type { get; init; }
        public byte[] Prefix { get; init; }
        public byte[] Signature { get; init; }

        public ObjectType(byte[] pattern)
        {
            if (pattern.Length != 23)
            {
                throw new ArgumentException("Pattern length is 23, not " + pattern.Length);
            }
            Type = (ObjectTypes)pattern[0];
            Prefix = pattern[1..7];
            Signature = pattern[7..23];
        }
        protected ObjectType() // I don't like this
        {
            Signature = new byte[16];
            Prefix = new byte[6];
        }

        public virtual byte[] Encode()
        {
            byte[] bytes = new byte[23];
            bytes[0] = (byte)Type;
            Prefix.CopyTo(bytes, 1);
            Signature.CopyTo(bytes, 7);
            return bytes;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is ObjectType type &&
                   Type == type.Type &&
                   EqualityComparer<byte[]>.Default.Equals(Prefix, type.Prefix) &&
                   EqualityComparer<byte[]>.Default.Equals(Signature, type.Signature);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Prefix, Signature);
        }
    }
}
