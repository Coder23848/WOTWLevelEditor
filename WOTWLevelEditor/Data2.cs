using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTWLevelEditor
{
    public class Data2 // Figuring out what this stuff does comes later
    {
        public int ID { get; }

        public Data2(byte[] pattern)
        {
            ID = BitConverter.ToInt32(pattern, 0);
        }

        public override string ToString()
        {
            return ID.ToString();
        }
    }
}
