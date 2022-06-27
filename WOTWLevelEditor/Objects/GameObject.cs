using System.Diagnostics;

namespace WOTWLevelEditor.Objects
{
    public class GameObject : UnityObject
    {
        public List<int> ComponentIDs { get; set; }
        public int Data2 { get; }
        public int Data3 { get; }
        public string Name { get; }
        public byte Data4 { get; }
        public byte Data5 { get; }
        public bool Enabled { get; }
        public Transform ThisTransform => (Transform)ParentLevel.FindObjectByID(ComponentIDs[0]);

        public GameObject(Level level, ObjectType type, int id, List<int> componentIDs, int data2, int data3, string name, byte data4, byte data5, bool enabled) : base(level, type, id)
        {
            ComponentIDs = componentIDs;
            Data2 = data2;
            Data3 = data3;
            Name = name;
            Data4 = data4;
            Data5 = data5;
            Enabled = enabled;
        }

        public static GameObject Parse(Level level, ObjectType type, int id, byte[] bytes)
        {
            int[] componentIDs = new int[BitConverter.ToInt32(bytes, 0)];
            int parserLocation = 4;
            for (int i = 0; i < componentIDs.Length; i++)
            {
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation) == 0);
                componentIDs[i] = BitConverter.ToInt32(bytes, parserLocation + 4);
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
            byte data4 = bytes[parserLocation];
            parserLocation += 1;
            byte data5 = bytes[parserLocation];
            parserLocation += 1;
            bool enabled = BitConverter.ToBoolean(bytes, parserLocation);
            parserLocation += 1;
            Debug.Assert(bytes.Length == parserLocation);
            return new GameObject(level, type, id, new(componentIDs), data2, data3, name, data4, data5, enabled);
        }

        public UnityObject GetComponent(int id)
        {
            return ParentLevel.FindObjectByID(ComponentIDs[id]);
        }

        public override byte[] Encode()
        {
            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes(ComponentIDs.Count));
            foreach (int i in ComponentIDs)
            {
                bytes.AddRange(BitConverter.GetBytes(0));
                bytes.AddRange(BitConverter.GetBytes(i));
                bytes.AddRange(BitConverter.GetBytes(0));
            }
            bytes.AddRange(BitConverter.GetBytes(Data2));
            bytes.AddRange(BitConverter.GetBytes(Data3));
            bytes.AddRange(BitConverter.GetBytes(Name.Length));
            bytes.AddRange(System.Text.Encoding.ASCII.GetBytes(Name));
            // Return to multiple of 4
            while (bytes.Count % 4 != 0)
            {
                bytes.Add(0);
            }
            bytes.Add(Data4);
            bytes.Add(Data5);
            bytes.AddRange(BitConverter.GetBytes(Enabled));

            return bytes.ToArray();
        }

        public override string ToString()
        {
            return string.Join(", ", string.Join("-", ComponentIDs), Data2, Data3, Name, Enabled);
        }
    }
}
