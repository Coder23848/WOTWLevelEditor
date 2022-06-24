using System.Diagnostics;
using WOTWLevelEditor.Objects;

namespace WOTWLevelEditor
{
    /// <summary>
    /// Represents a Unity scene.
    /// </summary>
    public class Level
    {
        private readonly int fileLength;
        /// <summary>
        /// The length of the level file in bytes.
        /// </summary>
        public int FileLength => fileLength;

        private readonly ObjectType[] objectTypeList = Array.Empty<ObjectType>();
        /// <summary>
        /// A list of <see cref="ObjectType"/>s that this <see cref="Level"/> uses.
        /// </summary>
        public ObjectType[] ObjectTypeList => objectTypeList;

        private readonly ObjectTypeLink[] objectTypeLinkList = Array.Empty<ObjectTypeLink>();
        /// <summary>
        /// A list of <see cref="ObjectTypeLink"/>s that represent the <see cref="UnityObject"/>s in this <see cref="Level"/>.
        /// </summary>
        public ObjectTypeLink[] ObjectTypeLinkList => objectTypeLinkList;

        private readonly Data3[] data3List = Array.Empty<Data3>();
        public Data3[] Data3List => data3List;

        private readonly FileReference[] fileReferenceList = Array.Empty<FileReference>();
        public FileReference[] FileReferenceList => fileReferenceList;

        private readonly UnityObject[] objectList = Array.Empty<UnityObject>();
        /// <summary>
        /// A list of <see cref="UnityObject"/>s in this <see cref="Level"/>.
        /// </summary>
        public UnityObject[] ObjectList => objectList;

        /// <summary>
        /// Constructs a level from the contents of a level file.
        /// </summary>
        /// <param name="bytes">A byte array representing the contents of a level file.</param>
        public Level(byte[] bytes)
        {
            int parserLocation = 0;

            byte[] unknownBytes = ByteHelper.GetAtIndex(bytes, parserLocation, 4);
            unknownBytes = unknownBytes.Reverse().ToArray();
            int unknown = BitConverter.ToInt32(unknownBytes);
            parserLocation += 4; // Unknown bytes

            byte[] fileLengthBytes = ByteHelper.GetAtIndex(bytes, parserLocation, 4);
            fileLengthBytes = fileLengthBytes.Reverse().ToArray();
            fileLength = BitConverter.ToInt32(fileLengthBytes);
            parserLocation += 4;

            Debug.Assert(Enumerable.SequenceEqual(ByteHelper.GetAtIndex(bytes, parserLocation, 4), new byte[] { 0x00, 0x00, 0x00, 0x11 })); // Always equals 00-00-00-11
            parserLocation += 4;

            byte[] objectStartBytes = ByteHelper.GetAtIndex(bytes, parserLocation, 4);
            objectStartBytes = objectStartBytes.Reverse().ToArray();
            int objectStartLocation = BitConverter.ToInt32(objectStartBytes);
            parserLocation += 4;

            Debug.Assert(Enumerable.SequenceEqual(ByteHelper.GetAtIndex(bytes, parserLocation, 16), System.Text.Encoding.ASCII.GetBytes("\0\0\0\02018.4.24f1\0"))); // Always equals "\0\0\0\02018.4.24f1\0"
            parserLocation += 16;

            Debug.Assert(Enumerable.SequenceEqual(ByteHelper.GetAtIndex(bytes, parserLocation, 5), new byte[] { 0x13, 0x00, 0x00, 0x00, 0x00 })); // Always equals 13-00-00-00-00
            parserLocation += 5;

            // Get list of object types
            objectTypeList = new ObjectType[BitConverter.ToInt32(bytes, parserLocation)];
            parserLocation += 4;

            for (int i = 0; i < ObjectTypeList.Length; i++)
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

            for (int i = 0; i < ObjectTypeLinkList.Length; i++)
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

            for (int i = 0; i < Data3List.Length; i++)
            {
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation) == 1); // Always 1 for some reason
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation + 8) == 0); // Always 0 for some reason
                data3List[i] = new Data3(BitConverter.ToInt32(bytes, parserLocation + 4));
                parserLocation += 12;
            }

            fileReferenceList = new FileReference[BitConverter.ToInt32(bytes, parserLocation)];
            parserLocation += 4;

            for(int i = 0; i < fileReferenceList.Length; i++)
            {
                byte[] fileReferenceData = ByteHelper.GetAtIndex(bytes, parserLocation, 21); // Not sure what this is, but it looks important
                parserLocation += 21; 
                // Strings appear to be null-terminated here
                int stringLength = Array.IndexOf(bytes, (byte)0x00, parserLocation) - parserLocation; // Get the length of the string by searching for a null byte; there might be a better way to do it
                fileReferenceList[i] = new FileReference(fileReferenceData, System.Text.Encoding.ASCII.GetString(bytes, parserLocation, stringLength));
                parserLocation += stringLength + 1;
            }

            // Return to multiple of 16
            while (parserLocation % 16 != 0)
            {
                parserLocation++;
            }

            objectList = new UnityObject[ObjectTypeLinkList.Length];
            for(int i = 0; i < ObjectTypeLinkList.Length; i++)
            {
                byte[] objectData = ByteHelper.GetAtIndex(bytes, objectStartLocation + ObjectTypeLinkList[i].Position, ObjectTypeLinkList[i].Length);
                switch (ObjectTypeLinkList[i].TypeID.Type)
                {
                    case ObjectTypes.GameObject:
                        objectList[i] = GameObject.Parse(this, ObjectTypeLinkList[i].ObjectID, objectData);
                        break;
                    case ObjectTypes.Transform:
                        objectList[i] = Transform.Parse(this, ObjectTypeLinkList[i].ObjectID, objectData);
                        break;
                    default:
                        objectList[i] = UnknownFallback.Parse(this, ObjectTypeLinkList[i].ObjectID, objectData);
                        break;
                }
            }
        }

        public UnityObject FindObjectByID(int id)
        {
            foreach (UnityObject obj in objectList)
            {
                if (obj != null && obj.ID == id)
                {
                    return obj;
                }
            }
            throw new IndexOutOfRangeException("Object with ID " + id + " does not exist.");
        }
    }
}