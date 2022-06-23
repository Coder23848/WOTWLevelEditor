namespace WOTWLevelEditor
{
    public class UnityObject
    {
        public virtual ObjectTypes Type => throw new NotImplementedException();
        public Level ParentLevel { get; }
        public int ID { get; }
        protected UnityObject(Level level, int id)
        {
            ParentLevel = level;
            ID = id;
        }
        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
