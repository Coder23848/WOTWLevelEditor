namespace WOTWLevelEditor
{
    public struct ObjectID
    {
        int data1;
        public int ID { get; }
        int data2;

        public ObjectID(int id) : this(0, id, 0)
        {
        }
        public ObjectID(int data1, int id, int data2)
        {
            this.data1 = data1;
            ID = id;
            this.data2 = data2;
            System.Diagnostics.Debug.Assert(this.data1 == 0 && this.data2 == 0);
        }

        public override string ToString()
        {
            return "#" + ID;
        }
    }
}
