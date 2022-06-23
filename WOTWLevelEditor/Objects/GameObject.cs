using System.Diagnostics;

namespace WOTWLevelEditor.Objects
{
    public class GameObject : UnityObject
    {
        public int[] Data1 { get; }
        public int Data2 { get; }
        public int Data3 { get; }
        public string Name { get; }
        public bool Enabled { get; }

        public GameObject(int[] data1, int data2, int data3, string name, bool enabled)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
            Name = name;
            Enabled = enabled;
        }

        public static GameObject Parse(byte[] bytes)
        {
            int[] data1 = new int[BitConverter.ToInt32(bytes, 0)];
            int parserLocation = 4;
            for (int i = 0; i < data1.Length; i++)
            {
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation) == 0);
                data1[i] = BitConverter.ToInt32(bytes, parserLocation + 4);
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation + 8) == 0);
                parserLocation += 12;
            }
            int data2 = BitConverter.ToInt32(bytes, parserLocation);
            parserLocation += 4;
            int data3 = BitConverter.ToInt32(bytes, parserLocation);
            parserLocation += 4;
            int nameLength = BitConverter.ToInt32(bytes, parserLocation);
            parserLocation += 4;
            string name = System.Text.Encoding.ASCII.GetString(bytes, parserLocation, nameLength);
            parserLocation += nameLength;
            while (parserLocation % 4 != 0)
            {
                parserLocation++;
            }
            parserLocation += 2;
            bool enabled = BitConverter.ToBoolean(bytes, parserLocation);
            return new GameObject(data1, data2, data3, name, enabled);
        }

        public override string ToString()
        {
            return string.Join(", ", string.Join("-", Data1), Data2, Data3, Name, Enabled);
        }
    }
}
