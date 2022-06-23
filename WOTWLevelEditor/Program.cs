﻿namespace WOTWLevelEditor
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
            Level level = new(bytes);

            Console.WriteLine("File Length: " + level.FileLength + " bytes");

            /*Console.WriteLine("Object types (" + level.ObjectTypeList.Length + "):");
            foreach (ObjectType i in level.ObjectTypeList)
            {
                Console.WriteLine("    " + i.ToString());
            }*/

            Console.WriteLine("Object type links (" + level.ObjectTypeLinkList.Length + "):");
            foreach (ObjectTypeLink i in level.ObjectTypeLinkList)
            {
                Console.WriteLine("    " + i.ToString());
            }

            /*Console.WriteLine("Data list 3 (" + level.Data3List.Length + "):");
            foreach (Data3 i in level.Data3List)
            {
                Console.WriteLine("    " + i.ToString());
            }*/

            /*Console.WriteLine("File References (" + level.FileReferenceList.Length + "):");
            foreach (FileReference i in level.FileReferenceList)
            {
                Console.WriteLine("    " + i.ToString());
            }*/

            /*Console.WriteLine("Objects (" + level.ObjectList.Length + "):");
            foreach (UnityObject i in level.ObjectList)
            {
                if (i == null)
                {
                    Console.WriteLine("    NULL");
                }
                else
                {
                    Console.WriteLine("    " + i.ToString());
                }
            }*/
        }
    }
}