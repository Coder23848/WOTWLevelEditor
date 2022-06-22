using System.Diagnostics;

namespace WOTWLevelEditor
{
    public class Level
    {
        private readonly ObjectType[] objectTypeList = Array.Empty<ObjectType>();
        public ObjectType[] ObjectTypeList => objectTypeList;

        private readonly ObjectTypeLink[] objectTypeLinkList = Array.Empty<ObjectTypeLink>();
        public ObjectTypeLink[] ObjectTypeLinkList => objectTypeLinkList;

        private readonly Data3[] data3List = Array.Empty<Data3>();
        public Data3[] Data3List => data3List;

        private readonly FileReference[] fileReferenceList = Array.Empty<FileReference>();
        public FileReference[] FileReferenceList => fileReferenceList;

        public Level(byte[] bytes)
        {
            int parserLocation = 0;

            parserLocation += 4; // Unknown bytes

            byte[] fileLengthBytes = ByteHelper.GetAtIndex(bytes, 0x4, 4);
            fileLengthBytes = fileLengthBytes.Reverse().ToArray();
            int FileLength = BitConverter.ToInt32(fileLengthBytes);
            Console.WriteLine("File Length: " + FileLength + " bytes");
            parserLocation += 4;

            Debug.Assert(Enumerable.SequenceEqual(ByteHelper.GetAtIndex(bytes, parserLocation, 4), new byte[] { 0x00, 0x00, 0x00, 0x11 })); // Always equals 00-00-00-11
            parserLocation += 4;

            parserLocation += 4; // Unknown bytes

            Debug.Assert(Enumerable.SequenceEqual(ByteHelper.GetAtIndex(bytes, parserLocation, 16), System.Text.Encoding.ASCII.GetBytes("\0\0\0\02018.4.24f1\0"))); // Always equals "\0\0\0\02018.4.24f1\0"
            parserLocation += 16;

            Debug.Assert(Enumerable.SequenceEqual(ByteHelper.GetAtIndex(bytes, parserLocation, 5), new byte[] { 0x13, 0x00, 0x00, 0x00, 0x00 })); // Always equals 13-00-00-00-00
            parserLocation += 5;

            // Get list of object types
            objectTypeList = new ObjectType[BitConverter.ToInt32(bytes, parserLocation)];
            parserLocation += 4;

            for (int i = 0; i < objectTypeList.Length; i++)
            {
                if (bytes[parserLocation] == (byte)ObjectTypes.MonoBehaviour)
                {
                    objectTypeList[i] = new ScriptType(ByteHelper.GetAtIndex(bytes, parserLocation, 39));
                    parserLocation += 39;
                }
                else
                {
                    objectTypeList[i] = new ObjectType(ByteHelper.GetAtIndex(bytes, parserLocation, 23));
                    parserLocation += 23;
                }
            }

            objectTypeLinkList = new ObjectTypeLink[BitConverter.ToInt32(bytes, parserLocation)];
            parserLocation += 4;

            // Return to multiple of 4
            while (parserLocation % 4 != 0)
            {
                parserLocation++;
            }

            for (int i = 0; i < objectTypeLinkList.Length; i++)
            {
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation + 4) == 0); // Always 0 for some reason
                objectTypeLinkList[i] = new ObjectTypeLink(BitConverter.ToInt32(bytes, parserLocation),
                                         BitConverter.ToInt32(bytes, parserLocation + 8),
                                         BitConverter.ToInt32(bytes, parserLocation + 12),
                                         ObjectTypeList[BitConverter.ToInt32(bytes, parserLocation + 16)]);
                parserLocation += 20;
            }

            data3List = new Data3[BitConverter.ToInt32(bytes, parserLocation)];
            parserLocation += 4;

            for (int i = 0; i< data3List.Length; i++)
            {
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation) == 1); // Always 1 for some reason
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation + 8) == 0); // Always 0 for some reason
                data3List[i] = new Data3(BitConverter.ToInt32(bytes, parserLocation + 4));
                parserLocation += 12;
            }

            fileReferenceList = new FileReference[BitConverter.ToInt32(bytes, parserLocation)];
            parserLocation += 4;

            for(int i = 0; i< fileReferenceList.Length; i++)
            {
                // Strings appear to be null-terminated here
                parserLocation += 21;
                int stringLength = Array.IndexOf(bytes, (byte)0x00, parserLocation) - parserLocation; // Get the length of the string by searching for a null byte; there might be a better way to do it
                fileReferenceList[i] = new FileReference(System.Text.Encoding.ASCII.GetString(bytes, parserLocation, stringLength));
                parserLocation += stringLength + 1;
            }
        }
    }
}