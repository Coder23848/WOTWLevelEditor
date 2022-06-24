namespace WOTWLevelEditor
{
    public class ObjectTypeLink
    {
        public int ObjectID { get; }
        public int Position { get; }
        public int Length { get; }
        public int ThisTypeID { get; }
        public ObjectType ThisType { get; }

        public ObjectTypeLink(int objectID, int position, int length, int typeID, ObjectType type)
        {
            ObjectID = objectID;
            Position = position;
            Length = length;
            ThisTypeID = typeID;
            ThisType = type;
        }

        public byte[] Encode()
        {
            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes(ObjectID));
            bytes.AddRange(BitConverter.GetBytes(0));
            bytes.AddRange(BitConverter.GetBytes(Position));
            bytes.AddRange(BitConverter.GetBytes(Length));
            bytes.AddRange(BitConverter.GetBytes(ThisTypeID));
            return bytes.ToArray();
        }
        public override string ToString()
        {
            return BitConverter.ToString(BitConverter.GetBytes(ObjectID)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Position)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Length)) + ", " + ThisType.ToString();
        }
    }
}