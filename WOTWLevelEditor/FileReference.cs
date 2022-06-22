namespace WOTWLevelEditor
{
    public class FileReference
    {
        public string Name { get; }
        
        public FileReference(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
