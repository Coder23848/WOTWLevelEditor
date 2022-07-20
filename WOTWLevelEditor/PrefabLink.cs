using System.Diagnostics;

namespace WOTWLevelEditor
{
    public struct PrefabLink
    {
        string name;
        int data1;
        int id;
        int data3;

        public PrefabLink(string name, int data1, int id, int data3)
        {
            Debug.Assert(data3 == 0); // Always 0 for some reason
            this.name = name;
            this.data1 = data1;
            this.id = id;
            this.data3 = data3;
        }

        public override string ToString()
        {
            return name + " contains " + id + ", Unknown: " + data1;
        }
    }
}
