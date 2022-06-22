namespace WOTWLevelEditor
{
    public class FileReference
    {
        public byte[] Data { get; }
        public string Name { get; }
        
        public FileReference(byte[] data, string name)
        {
            Data = data;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
