namespace WOTWLevelEditor.Objects
{
    public class ResourceManager : UnityObject
    {
        public List<KeyValuePair<string, ObjectID>> Links { get => (List<KeyValuePair<string, ObjectID>>)parameters[0]; set => parameters[0] = value; }
        public List<KeyValuePair<ObjectID, List<ObjectID>>> Data1 { get => (List<KeyValuePair<ObjectID, List<ObjectID>>>)parameters[1]; set => parameters[1] = value; }

        public ResourceManager(Level level, ObjectType type, int id, object[] parameters) : base(level, type, id, parameters)
        {
        }

        public override string ToString()
        {
            string str = "[\n" + string.Join(",\n", Links) + ",";
            foreach (KeyValuePair<ObjectID, List<ObjectID>> i in Data1)
            {
                str += "\n[" + i.Key.ToString() + ", [" + string.Join(", ", i.Value) + "]]";
            }
            str += "]";
            return  str;
        }
    }
}
