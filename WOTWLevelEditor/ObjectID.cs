namespace WOTWLevelEditor
{
    public struct ObjectID
    {
        public int FileID;
        public int ID { get; }
        int data2;

        public ObjectID(int id) : this(0, id, 0)
        {
        }
        public ObjectID(int fileID, int id, int data2)
        {
            FileID = fileID;
            ID = id;
            this.data2 = data2;
            System.Diagnostics.Debug.Assert(this.data2 == 0);
        }

        public override string ToString()
        {
            if (FileID == 0)
            {
                return "#" + ID;
            }
            else
            {
                return "File " + FileID + ", #" + ID;
            }
        }
    }
}
