namespace WOTWLevelEditor
{
    public class UnityObject
    {
        public Level ParentLevel { get; }
        public ObjectType ThisType { get; }
        public int ID { get; }
        protected UnityObject(Level level, ObjectType type, int id)
        {
            ParentLevel = level;
            ThisType = type;
            ID = id;
        }

        public virtual byte[] Encode()
        {
            throw new NotImplementedException(ToString() + " cannot be encoded.");
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
