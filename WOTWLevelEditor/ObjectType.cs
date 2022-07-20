using System.Numerics;

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

        public Type[] GetSignature()
        {
            return BitConverter.ToString(Signature) switch
            {
                "76-1C-A8-1F-78-49-15-42-BA-DC-37-F8-10-AB-34-55" => new Type[] { typeof(ObjectID), typeof(Quaternion), typeof(Vector3), typeof(Vector3), typeof(List<ObjectID>), typeof(ObjectID) }, // Transform
                "2C-F0-38-CC-F1-06-22-04-53-57-07-9A-41-F3-E9-45" => new Type[] { typeof(List<ObjectID>), typeof(int), typeof(int), typeof(string), typeof(byte), typeof(byte), typeof(bool) }, // GameObject
                "0D-9D-88-0E-07-7F-33-C7-6F-C2-C9-9B-D6-29-A5-0A" => new Type[] { typeof(List<PrefabLink>) }, // ResourceManager, incomplete
                _ => Array.Empty<Type>()
            };
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