﻿using System.Diagnostics;

namespace WOTWLevelEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) // No file selected
            {
                throw new FileNotFoundException();
            }

            Console.WriteLine("Opening file: " + args[0]);
            byte[] bytes = File.ReadAllBytes(args[0]);

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
            ObjectType[] objectTypes = new ObjectType[BitConverter.ToInt32(bytes, parserLocation)];
            parserLocation += 4;
            
            for (int i = 0; i < objectTypes.Length; i++)
            {
                if (bytes[parserLocation] == (byte)ObjectTypes.MonoBehaviour)
                {
                    objectTypes[i] = new ScriptType(ByteHelper.GetAtIndex(bytes, parserLocation, 39));
                    parserLocation += 39;
                }
                else
                {
                    objectTypes[i] = new ObjectType(ByteHelper.GetAtIndex(bytes, parserLocation, 23));
                    parserLocation += 23;
                }
            }

            Data2[] data2s = new Data2[BitConverter.ToInt32(bytes, parserLocation)];
            parserLocation += 4;

            // Return to multiple of 4
            while (parserLocation % 4 != 0)
            {
                parserLocation++;
            }

            for (int i = 0; i < data2s.Length; i++)
            {
                data2s[i] = new Data2(ByteHelper.GetAtIndex(bytes, parserLocation, 20));
                parserLocation += 20;
            }

            Console.WriteLine("Object types (" + objectTypes.Length + "):");
            foreach(ObjectType i in objectTypes)
            {
                Console.WriteLine("    " + i.ToString());
            }

            Console.WriteLine("Data2 (" + data2s.Length + "):");
            foreach (Data2 i in data2s)
            {
                Console.WriteLine("    " + i.ToString());
            }
        }
    }
}