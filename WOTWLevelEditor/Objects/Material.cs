namespace WOTWLevelEditor.Objects
{
    public class Material : UnityObject
    {
        public string Name { get; }
        public int Data2 { get; }
        public int Data3 { get; }
        public string Flags { get; }
        public byte[] Data5 { get; }
        public Material(string name, int data2, int data3, string flags, byte[] data5)
        {
            Name = name;
            Data2 = data2;
            Data3 = data3;
            Flags = flags;
            Data5 = data5;
        }

        public override string ToString()
        {
            return string.Join(", ", Name, Data2, Data3, Flags, Data5.Length);
        }
    }
}
