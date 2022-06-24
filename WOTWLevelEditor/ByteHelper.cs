namespace WOTWLevelEditor
{
    public class ByteHelper
    {
        public static int FindBytes(byte[] array, byte[] search, int startIndex)
        {
            var len = search.Length;
            var limit = array.Length - len;
            for (var i = startIndex; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                {
                    if (search[k] != array[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }

        public static byte[] GetAtIndex(byte[] array, int index, int length)
        {
            return array[index..(index + length)];
        }
    }
}