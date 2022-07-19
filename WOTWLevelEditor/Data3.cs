namespace WOTWLevelEditor
{
    public class Data3
    {
        public int Data1 { get; }
        public int Data2 { get; }

        public Data3(int data1, int data2)
        {
            Data1 = data1;
            Data2 = data2;
        }

        public byte[] Encode()
        {
            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes(Data1));
            bytes.AddRange(BitConverter.GetBytes(Data2));
            bytes.AddRange(BitConverter.GetBytes(0));
            return bytes.ToArray();
        }

        public override string ToString()
        {
            return Data1.ToString() + ", " + Data2.ToString();
        }
    }
}