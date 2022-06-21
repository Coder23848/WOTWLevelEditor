using System.Diagnostics;

namespace WOTWLevelEditor
{
    public class Data2 // Figuring out what this stuff does comes later
    {
        public int ID { get; }

        public Data2(byte[] pattern)
        {
            ID = BitConverter.ToInt32(pattern, 0);
            Debug.Assert(BitConverter.ToInt32(pattern, 4) == 0); // Always 0 for some reason
        }

        public override string ToString()
        {
            return ID.ToString();
        }
    }
}
