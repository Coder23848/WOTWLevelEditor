using System.Diagnostics;
using System.Numerics;

namespace WOTWLevelEditor.Objects
{
    public class Transform : UnityObject
    {
        public int GameObjectID { get; }
        public Quaternion Rotation { get; }
        public Vector3 Position { get; }
        public Vector3 Scale { get; }
        public int[] ChildrenIDs { get; }
        public int ParentID { get; }
        public GameObject ThisGameObject => (GameObject)ParentLevel.FindObjectByID(GameObjectID);
        public Transform Parent => (Transform)ParentLevel.FindObjectByID(ParentID);

        public Transform(Level level, ObjectType type, int id, int gameObjectID, Quaternion rotation, Vector3 position, Vector3 scale, int[] childrenIDs, int parentID) : base(level, type, id)
        {
            GameObjectID = gameObjectID;
            Rotation = rotation;
            Position = position;
            Scale = scale;
            ChildrenIDs = childrenIDs;
            ParentID = parentID;
        }

        public static Transform Parse(Level level, ObjectType type, int id, byte[] bytes)
        {
            Debug.Assert(BitConverter.ToInt32(bytes, 0) == 0);
            int gameObjectID = BitConverter.ToInt32(bytes, 4);
            Debug.Assert(BitConverter.ToInt32(bytes, 8) == 0);
            Quaternion rotation = new(BitConverter.ToSingle(bytes, 12),
                                      BitConverter.ToSingle(bytes, 16),
                                      BitConverter.ToSingle(bytes, 20),
                                      BitConverter.ToSingle(bytes, 24));
            Vector3 position = new(BitConverter.ToSingle(bytes, 28),
                                   BitConverter.ToSingle(bytes, 32),
                                   BitConverter.ToSingle(bytes, 36));
            Vector3 scale = new(BitConverter.ToSingle(bytes, 40),
                                BitConverter.ToSingle(bytes, 44),
                                BitConverter.ToSingle(bytes, 48));
            int[] childrenIDs = new int[BitConverter.ToInt32(bytes, 52)];
            int parserLocation = 56;
            for (int i = 0; i < childrenIDs.Length; i++)
            {
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation) == 0);
                childrenIDs[i] = BitConverter.ToInt32(bytes, parserLocation + 4);
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation + 8) == 0);
                parserLocation += 12;
            }
            Debug.Assert(BitConverter.ToInt32(bytes, parserLocation) == 0);
            int parentID = BitConverter.ToInt32(bytes, parserLocation + 4);
            Debug.Assert(BitConverter.ToInt32(bytes, parserLocation + 8) == 0);
            parserLocation += 12;
            Debug.Assert(bytes.Length == parserLocation);
            return new Transform(level, type, id, gameObjectID, rotation, position, scale, childrenIDs, parentID);
        }

        public Transform GetChild(int id)
        {
            return (Transform)ParentLevel.FindObjectByID(ChildrenIDs[id]);
        }

        public override byte[] Encode()
        {
            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes(0));
            bytes.AddRange(BitConverter.GetBytes(GameObjectID));
            bytes.AddRange(BitConverter.GetBytes(0));
            bytes.AddRange(BitConverter.GetBytes(Rotation.W));
            bytes.AddRange(BitConverter.GetBytes(Rotation.X));
            bytes.AddRange(BitConverter.GetBytes(Rotation.Y));
            bytes.AddRange(BitConverter.GetBytes(Rotation.Z));
            bytes.AddRange(BitConverter.GetBytes(Position.X));
            bytes.AddRange(BitConverter.GetBytes(Position.Y));
            bytes.AddRange(BitConverter.GetBytes(Position.Z));
            bytes.AddRange(BitConverter.GetBytes(Scale.X));
            bytes.AddRange(BitConverter.GetBytes(Scale.Y));
            bytes.AddRange(BitConverter.GetBytes(Scale.Z));
            bytes.AddRange(BitConverter.GetBytes(ChildrenIDs.Length));
            foreach (int i in ChildrenIDs)
            {
                bytes.AddRange(BitConverter.GetBytes(0));
                bytes.AddRange(BitConverter.GetBytes(i));
                bytes.AddRange(BitConverter.GetBytes(0));
            }
            bytes.AddRange(BitConverter.GetBytes(0));
            bytes.AddRange(BitConverter.GetBytes(ParentID));
            bytes.AddRange(BitConverter.GetBytes(0));

            return bytes.ToArray();
        }

        public override string ToString()
        {
            return string.Join(", ", GameObjectID, Rotation, Position, Scale, string.Join("-", ChildrenIDs), ParentID);
        }
    }
}