using System.Diagnostics;

namespace WOTWLevelEditor
{
    public class Data2 // Figuring out what this stuff does comes later
    {
        public int ID { get; }
        public int Data3 { get; }
        public int Data4 { get; }
        public int Data5 { get; }

        public Data2(byte[] pattern)
        {
            ID = BitConverter.ToInt32(pattern, 0);
            Debug.Assert(BitConverter.ToInt32(pattern, 4) == 0); // Always 0 for some reason
            Data3 = BitConverter.ToInt32(pattern, 8);
            Data4 = BitConverter.ToInt32(pattern, 12);
            Data5 = BitConverter.ToInt32(pattern, 16);
        }

        public override string ToString()
        {
            return BitConverter.ToString(BitConverter.GetBytes(ID)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Data3)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Data4)) + ", " + BitConverter.ToString(BitConverter.GetBytes(Data5));
        }
    }
}
