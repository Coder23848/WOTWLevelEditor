using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTWLevelEditor
{
    public class ByteHelper
    {
        public static int FindBytes(byte[] array, byte[] search, SearchLocation order, SearchLocation returnIndex)
        {
            // This is kinda gross, there's probably a better way to do it.
            string arrayString = BitConverter.ToString(array);
            string searchString = BitConverter.ToString(search);
            var index = order switch
            {
                SearchLocation.First => arrayString.IndexOf(searchString) / 3,
                SearchLocation.Last => arrayString.LastIndexOf(searchString) / 3,
                _ => throw new NotImplementedException()
            };
            return returnIndex switch
            {
                SearchLocation.First => index,
                SearchLocation.Last => index + search.Length - 1,
                _ => throw new NotImplementedException()
            };
        }
        public static byte[] GetAtIndex(byte[] array, int index, int length)
        {
            return array[index..(index + length)];
        }
    }
}
