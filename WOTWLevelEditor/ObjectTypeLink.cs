using System.Diagnostics;

namespace WOTWLevelEditor
{
    public class ObjectTypeLink // Figuring out what this stuff does comes later
    {
        public int ObjectID { get; }
        public int Position { get; }
        public int Length { get; }
        public ObjectType TypeID { get; }

        public ObjectTypeLink(int objectID, int position, int length, ObjectType typeID)
        {
            ObjectID = objectID;
            Position = position;
            Length = length;
            TypeID = typeID;
        }

        public override string ToString()
        {
            return BitConverter.ToString(BitConverter.GetBytes(ObjectID)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Position)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Length)) + ", " + TypeID.ToString();
        }
    }
}