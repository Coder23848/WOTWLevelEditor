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

        public byte[] Encode()
        {
            List<byte> bytes = new();
            bytes.AddRange(Data);
            bytes.AddRange(System.Text.Encoding.ASCII.GetBytes(Name));
            bytes.Add(0x00);
            return bytes.ToArray();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
