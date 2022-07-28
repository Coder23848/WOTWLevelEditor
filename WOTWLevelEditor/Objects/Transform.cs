using System.Diagnostics;
using System.Numerics;

namespace WOTWLevelEditor.Objects
{
    public class Transform : UnityObject
    {
        public ObjectID GameObjectID { get => (ObjectID)parameters[0]; set => parameters[0] = value; }
        public Quaternion Rotation { get => (Quaternion)parameters[1]; set => parameters[1] = value; }
        public Vector3 Position { get => (Vector3)parameters[2]; set => parameters[2] = value; }
        public Vector3 Scale { get => (Vector3)parameters[3]; set => parameters[3] = value; }
        public List<ObjectID> ChildrenIDs => (List<ObjectID>)parameters[4];
        public ObjectID ParentID { get => (ObjectID)parameters[5]; set => parameters[5] = value; }
        public GameObject ThisGameObject => (GameObject)ParentLevel.FindObjectByID(GameObjectID);
        public Transform Parent => (Transform)ParentLevel.FindObjectByID(ParentID);

        public Transform(Level level, ObjectType type, int id, object[] parameters) : base(level, type, id, parameters)
        {
        }

        public Transform GetChild(int id)
        {
            return (Transform)ParentLevel.FindObjectByID(ChildrenIDs[id]);
        }

        public void RemoveChild(ObjectID id)
        {
            if (!ChildrenIDs.Contains(id))
            {
                throw new ArgumentException(id.ToString() + " is not a child of this transform.");
            }
            else
            {
                ChildrenIDs.Remove(id);
            }
        }

        public override List<ObjectID> GetReferences()
        {
            List<ObjectID> references = new();
            references.Add(GameObjectID);
            references.AddRange(ChildrenIDs);
            if (ParentID.ID != 0) // 0 is the scene root
            {
                references.Add(ParentID);
            }
            return references;
        }

        public override void ConvertReferences(Dictionary<ObjectID, ObjectID> conversionTable)
        {
            GameObjectID = conversionTable[GameObjectID];
            for (int i = 0; i < ChildrenIDs.Count; i++)
            {
                ChildrenIDs[i] = conversionTable[ChildrenIDs[i]];
            }
            if (conversionTable.ContainsKey(ParentID))
            {
                ParentID = conversionTable[ParentID];
            }
        }

        public override string ToString()
        {
            return string.Join(", ", "Component of GameObject " + GameObjectID, "Rotation: " + Rotation, "Position: " + Position, "Scale: " + Scale, "Children: [" + string.Join(", ", ChildrenIDs) + "]", "Parent: " + ParentID);
        }
    }
}