using System.Diagnostics;

namespace WOTWLevelEditor.Objects
{
    public class GameObject : UnityObject
    {
        public List<ObjectID> ComponentIDs { get => (List<ObjectID>)parameters[0]; }
        public int Data2 { get => (int)parameters[1]; }
        public int Data3 { get => (int)parameters[2]; }
        public string Name { get => (string)parameters[3]; }
        public byte Data4 { get => (byte)parameters[4]; }
        public byte Data5 { get => (byte)parameters[5]; }
        public bool Enabled { get => (bool)parameters[6]; }
        public Transform ThisTransform => (Transform)ParentLevel.FindObjectByID(ComponentIDs[0]);

        public GameObject(Level level, ObjectType type, int id, object[] parameters) : base(level, type, id, parameters)
        {
        }

        public UnityObject GetComponent(int id)
        {
            return ParentLevel.FindObjectByID(ComponentIDs[id]);
        }

        public override List<ObjectID> GetReferences()
        {
            List<ObjectID> references = new();
            references.AddRange(ComponentIDs);
            return references;
        }

        public override void ConvertReferences(Dictionary<ObjectID, ObjectID> conversionTable)
        {
            for (int i = 0; i < ComponentIDs.Count; i++)
            {
                ComponentIDs[i] = conversionTable[ComponentIDs[i]];
            }
        }

        public override string ToString()
        {
            return string.Join(", ", Name, "Components: [" + string.Join(", ", ComponentIDs) + "]", Enabled ? "Enabled" : "Disabled", "Unknown: " + string.Join(", ", Data2, Data3));
        }
    }
}