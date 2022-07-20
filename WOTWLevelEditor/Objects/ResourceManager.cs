namespace WOTWLevelEditor.Objects
{
    public class ResourceManager : UnityObject
    {
        public List<PrefabLink> Links { get => (List<PrefabLink>)parameters[0]; set => parameters[0] = value; }

        public ResourceManager(Level level, ObjectType type, int id, object[] parameters) : base(level, type, id, parameters)
        {
        }

        public override string ToString()
        {
            return "[\n" + string.Join(",\n", Links) + "]";
        }
    }
}
