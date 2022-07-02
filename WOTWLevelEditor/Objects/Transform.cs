using System.Diagnostics;
using System.Numerics;

namespace WOTWLevelEditor.Objects
{
    public class Transform : UnityObject
    {
        public ObjectID GameObjectID => (ObjectID)parameters[0];
        public Quaternion Rotation { get => (Quaternion)parameters[1]; set => parameters[1] = value; }
        public Vector3 Position { get => (Vector3)parameters[2]; set => parameters[2] = value; }
        public Vector3 Scale { get => (Vector3)parameters[3]; set => parameters[3] = value; }
        public List<ObjectID> ChildrenIDs => (List<ObjectID>)parameters[4];
        public ObjectID ParentID => (ObjectID)parameters[5];
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

        public override string ToString()
        {
            return string.Join(", ", GameObjectID, Rotation, Position, Scale, string.Join("-", ChildrenIDs), ParentID);
        }
    }
}