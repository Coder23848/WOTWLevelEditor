namespace WOTWLevelEditor
{
    public class Data3
    {
        public int Data2 { get; }

        public Data3(int data2)
        {
            Data2 = data2;
        }

        public byte[] Encode()
        {
            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes(1));
            bytes.AddRange(BitConverter.GetBytes(Data2));
            bytes.AddRange(BitConverter.GetBytes(0));
            return bytes.ToArray();
        }

        public override string ToString()
        {
            return Data2.ToString();
        }
    }
}