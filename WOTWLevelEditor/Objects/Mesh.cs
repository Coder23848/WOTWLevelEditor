namespace WOTWLevelEditor.Objects
{
    public class Mesh : UnityObject
    {
        public string Name { get; }
        public int Data1 { get; }
        public int Data2 { get; }
        public Mesh(string name, int data1, int data2)
        {
            Name = name;
            Data1 = data1;
            Data2 = data2;
        }

        public override string ToString()
        {
            return string.Join(", ", Name, Data1, Data2);
        }
    }
}
