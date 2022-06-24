namespace WOTWLevelEditor
{
    public class ObjectTypeLink
    {
        public int ObjectID { get; }
        public int Position { get; }
        public int Length { get; }
        public ObjectType ThisType { get; }

        public ObjectTypeLink(int objectID, int position, int length, ObjectType typeID)
        {
            ObjectID = objectID;
            Position = position;
            Length = length;
            ThisType = typeID;
        }

        public override string ToString()
        {
            return BitConverter.ToString(BitConverter.GetBytes(ObjectID)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Position)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Length)) + ", " + ThisType.ToString();
        }
    }
}