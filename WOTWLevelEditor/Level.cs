using System.Diagnostics;
using WOTWLevelEditor.Objects;

namespace WOTWLevelEditor
{
    /// <summary>
    /// Represents a Unity scene.
    /// </summary>
    public class Level
    {
        // These two are only used because I haven't yet figured out how to calculate them from the object list.
        private readonly List<Data3> data3List = new();
        private readonly List<FileReference> fileReferenceList = new();

        private readonly List<UnityObject> objectList = new();

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
            parserLocation += 4;

            byte[] fileLengthBytes = ByteHelper.GetAtIndex(bytes, parserLocation, 4);
            fileLengthBytes = fileLengthBytes.Reverse().ToArray();
            int fileLength = BitConverter.ToInt32(fileLengthBytes);
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
            ObjectType[] objectTypeList = new ObjectType[BitConverter.ToInt32(bytes, parserLocation)];
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

            ObjectTypeLink[] objectTypeLinkList = new ObjectTypeLink[BitConverter.ToInt32(bytes, parserLocation)];
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
                                         BitConverter.ToInt32(bytes, parserLocation + 16),
                                         objectTypeList[BitConverter.ToInt32(bytes, parserLocation + 16)]);
                parserLocation += 20;
            }

            Data3[] data3List = new Data3[BitConverter.ToInt32(bytes, parserLocation)];
            parserLocation += 4;

            for (int i = 0; i < data3List.Length; i++)
            {
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation) == 1); // Always 1 for some reason
                Debug.Assert(BitConverter.ToInt32(bytes, parserLocation + 8) == 0); // Always 0 for some reason
                data3List[i] = new Data3(BitConverter.ToInt32(bytes, parserLocation + 4));
                parserLocation += 12;
            }

            this.data3List = data3List.ToList();

            FileReference[] fileReferenceList = new FileReference[BitConverter.ToInt32(bytes, parserLocation)];
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

            this.fileReferenceList = fileReferenceList.ToList();

            // Return to multiple of 16
            while (parserLocation % 16 != 0)
            {
                parserLocation++;
            }

            objectList = new(objectTypeLinkList.Length);
            for(int i = 0; i < objectTypeLinkList.Length; i++)
            {
                byte[] objectData = ByteHelper.GetAtIndex(bytes, objectStartLocation + objectTypeLinkList[i].Position, objectTypeLinkList[i].Length);
                switch (objectTypeLinkList[i].ThisType.Type)
                {
                    case ObjectTypes.GameObject:
                        objectList.Add(GameObject.Parse(this, objectTypeLinkList[i].ThisType, objectTypeLinkList[i].ObjectID, objectData));
                        break;
                    case ObjectTypes.Transform:
                        objectList.Add(Transform.Parse(this, objectTypeLinkList[i].ThisType, objectTypeLinkList[i].ObjectID, objectData));
                        break;
                    default:
                        objectList.Add(UnknownFallback.Parse(this, objectTypeLinkList[i].ThisType, objectTypeLinkList[i].ObjectID, objectData));
                        break;
                }
            }
        }

        public byte[] Encode()
        {
            List<byte> bytes = new();

            List<ObjectType> newObjectTypeList = new();
            List<ObjectTypeLink> newObjectTypeLinkList = new();
            List<byte[]> encodedObjectList = new();

            bytes.AddRange(new byte[4]); // To replace later
            bytes.AddRange(new byte[4]); // To replace later
            bytes.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x11 });
            bytes.AddRange(new byte[4]); // To replace later
            bytes.AddRange(System.Text.Encoding.ASCII.GetBytes("\0\0\0\02018.4.24f1\0"));
            bytes.AddRange(new byte[] { 0x13, 0x00, 0x00, 0x00, 0x00 });

            int objectLocation = 0;
            foreach (UnityObject i in objectList)
            {
                bool alreadyInList = false;
                int thisTypeID = -1;
                // Set up the corresponding type in the type list, if needed
                for (int j = 0; j < newObjectTypeList.Count; j++)
                {
                    if (i.ThisType.Equals(newObjectTypeList[j]))
                    {
                        alreadyInList = true;
                        thisTypeID = j;
                        break;
                    }
                }
                if (!alreadyInList)
                {
                    newObjectTypeList.Add(i.ThisType);
                    thisTypeID = newObjectTypeList.Count - 1;
                }
                byte[] encoded = i.Encode();
                int objectLength = encoded.Length;
                // Set up the object's type link
                newObjectTypeLinkList.Add(new(i.ID, objectLocation, objectLength, thisTypeID, i.ThisType));
                objectLocation += objectLength;
                // Return to multiple of 4
                while (objectLocation % 4 != 0)
                {
                    objectLocation++;
                }
                // Set up the object data
                encodedObjectList.Add(encoded);
            }

            // Add the type list
            bytes.AddRange(BitConverter.GetBytes(newObjectTypeList.Count));
            foreach (ObjectType i in newObjectTypeList)
            {
                bytes.AddRange(i.Encode());
            }

            // Add the type link list
            bytes.AddRange(BitConverter.GetBytes(newObjectTypeLinkList.Count));
            // Return to multiple of 4
            while (bytes.Count % 4 != 0)
            {
                bytes.Add(0);
            }
            foreach (ObjectTypeLink i in newObjectTypeLinkList)
            {
                bytes.AddRange(i.Encode());
            }

            // Add back the Data3 list
            bytes.AddRange(BitConverter.GetBytes(data3List.Count));
            foreach (Data3 i in data3List)
            {
                bytes.AddRange(i.Encode());
            }

            // Add back the File Reference list
            bytes.AddRange(BitConverter.GetBytes(fileReferenceList.Count));
            foreach (FileReference i in fileReferenceList)
            {
                bytes.AddRange(i.Encode());
            }

            int unityWhy = 19;
            // Return to multiple of 16
            while (bytes.Count % 16 != 0)
            {
                bytes.Add(0);
                unityWhy++;
            }

            int objectStartPosition = bytes.Count;
            // Add the object list
            foreach (byte[] i in encodedObjectList)
            {
                bytes.AddRange(i);
                // Return to multiple of 4
                while (bytes.Count % 4 != 0)
                {
                    bytes.Add(0);
                }
            }

            // Always equals the end of the file reference list minus 19, for some reason
            int unknown = objectStartPosition - unityWhy;
            byte[] unknownBytes = BitConverter.GetBytes(unknown);
            unknownBytes = unknownBytes.Reverse().ToArray();
            bytes.RemoveRange(0, 4);
            bytes.InsertRange(0, unknownBytes);

            int fileLength = bytes.Count;
            byte[] fileLengthBytes = BitConverter.GetBytes(fileLength);
            fileLengthBytes = fileLengthBytes.Reverse().ToArray();
            bytes.RemoveRange(4, 4);
            bytes.InsertRange(4, fileLengthBytes);

            byte[] objectStartPositionBytes = BitConverter.GetBytes(objectStartPosition);
            objectStartPositionBytes = objectStartPositionBytes.Reverse().ToArray();
            bytes.RemoveRange(12, 4);
            bytes.InsertRange(12, objectStartPositionBytes);

            return bytes.ToArray();
        }

        /// <summary>
        /// Gets a <see cref="UnityObject"/> based on its ID.
        /// </summary>
        /// <param name="id">The ID to search for.</param>
        /// <returns>A <see cref="UnityObject"/> with the specified ID.</returns>
        /// <exception cref="IndexOutOfRangeException">A <see cref="UnityObject"/> with the specified ID does not exist in this <see cref="Level"/>.</exception>
        public UnityObject FindObjectByID(int id)
        {
            foreach (UnityObject obj in objectList) // This seems inefficient
            {
                if (obj != null && obj.ID == id)
                {
                    return obj;
                }
            }
            throw new IndexOutOfRangeException("Object with ID " + id + " does not exist.");
        }

        /// <summary>
        /// Gets a list of <see cref="GameObject"/>s based on their name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>A list of all <see cref="GameObject"/>s in this <see cref="Level"/> with the specified name.</returns>
        public List<GameObject> FindGameObjectsByName(string name)
        {
            List<GameObject> result = new();
            foreach (UnityObject obj in objectList) // This seems inefficient
            {
                if (obj != null && obj is GameObject gob && gob.Name == name)
                {
                    result.Add(gob);
                }
            }
            return result;
        }

        /// <summary>
        /// Deletes a <see cref="UnityObject"/> in this <see cref="Level"/>, and everything attatched to it.
        /// </summary>
        /// <param name="id">The ID of the <see cref="UnityObject"/> to delete.</param>
        public void DeleteObject(int id)
        {
            UnityObject obj;
            try
            {
                obj = FindObjectByID(id);
            }
            catch (IndexOutOfRangeException) // Object does not exist, it was probably already deleted
            {
                return;
            }
            if (obj is GameObject gameObject)
            {
                int[] componentIDs = new int[gameObject.ComponentIDs.Count];
                gameObject.ComponentIDs.CopyTo(componentIDs); // This is kinda weird
                foreach (int i in componentIDs)
                {
                    DeleteObject(i);
                }
            }
            if (obj is Transform transform)
            {
                transform.ThisGameObject.ComponentIDs.Remove(id); // This is needed to prevent an infinite loop
                DeleteObject(transform.GameObjectID);
                int[] childrenIDs = new int[transform.ChildrenIDs.Count];
                transform.ChildrenIDs.CopyTo(childrenIDs); // This is kinda weird
                foreach (int i in childrenIDs)
                {
                    DeleteObject(i);
                }
                transform.Parent.ChildrenIDs.Remove(transform.ID);
            }
            objectList.Remove(obj);
        }

        public void PrintObjectList()
        {
            Console.WriteLine("Objects (" + objectList.Count + "):");
            for (int i = 0; i < objectList.Count; i++)
            {
                Console.WriteLine("    " + objectList[i].ThisType.ToString() + " " + objectList[i].ID + ": " + objectList[i].ToString());
            }
        }
    }
}