using System.Diagnostics;

namespace WOTWLevelEditor
{
    public struct PrefabLink
    {
        public string Name { get; }
        public int Data1 { get; }
        public int ID { get; }
        public int Data3 { get; }

        public PrefabLink(string name, int data1, int id, int data3)
        {
            Debug.Assert(data3 == 0); // Always 0 for some reason
            Name = name;
            Data1 = data1;
            ID = id;
            Data3 = data3;
        }

        public override string ToString()
        {
            return Name + " contains " + ID + ", Unknown: " + Data1;
        }
    }
}
