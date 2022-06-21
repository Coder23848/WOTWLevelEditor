using System.Diagnostics;

namespace WOTWLevelEditor
{
    public class ObjectTypeLink // Figuring out what this stuff does comes later
    {
        public int ObjectID { get; }
        public int Data3 { get; }
        public int Data4 { get; }
        public ObjectType TypeID { get; }

        public ObjectTypeLink(int objectID, int data3, int data4, ObjectType typeID)
        {
            ObjectID = objectID;
            Data3 = data3;
            Data4 = data4;
            TypeID = typeID;
        }

        public override string ToString()
        {
            return BitConverter.ToString(BitConverter.GetBytes(ObjectID)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Data3)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Data4)) + ", " + TypeID.ToString();
        }
    }
}
