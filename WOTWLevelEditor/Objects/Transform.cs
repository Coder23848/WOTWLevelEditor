using System.Diagnostics;
using System.Numerics;

namespace WOTWLevelEditor.Objects
{
    public class Transform : UnityObject
    {
        public override ObjectTypes Type => ObjectTypes.Transform;
        public int GameObjectID { get; }
        public Quaternion Rotation { get; }
        public Vector3 Position { get; }
        public Vector3 Scale { get; }
        public int[] ChildrenIDs { get; }
        public int ParentID { get; }
        public GameObject ThisGameObject => (GameObject)ParentLevel.FindObjectByID(GameObjectID);
        public Transform Parent => (Transform)ParentLevel.FindObjectByID(ParentID);

        public Transform(Level level, int id, int gameObjectID, Quaternion rotation, Vector3 position, Vector3 scale, int[] childrenIDs, int parentID) : base(level, id)
        {
            GameObjectID = gameObjectID;
            Rotation = rotation;
            Position = position;
            Scale = scale;
            ChildrenIDs = childrenIDs;
            ParentID = parentID;
        }

        public static Transform Parse(Level level, int id, byte[] bytes)
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
            return new Transform(level, id, gameObjectID, rotation, position, scale, childrenIDs, parentID);
        }

        public Transform GetChild(int id)
        {
            return (Transform)ParentLevel.FindObjectByID(ChildrenIDs[id]);
        }

        public override string ToString()
        {
            return string.Join(", ", GameObjectID, Rotation, Position, Scale, string.Join("-", ChildrenIDs), ParentID);
        }
    }
}