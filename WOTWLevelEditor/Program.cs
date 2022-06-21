namespace WOTWLevelEditor
{
    class Program
    {
        public ObjectType[] ObjectTypeList { get; private set; } = Array.Empty<ObjectType>();

        static void Main(string[] args)
        {
            if (args.Length == 0) // No file selected
            {
                throw new FileNotFoundException();
            }

            Console.WriteLine("Opening file: " + args[0]);
            byte[] bytes = System.IO.File.ReadAllBytes(args[0]);

            Level level = new(bytes);

            Console.WriteLine("Object types (" + level.ObjectTypeList.Length + "):");
            foreach (ObjectType i in level.ObjectTypeList)
            {
                Console.WriteLine("    " + i.ToString());
            }

            Console.WriteLine("Data2 (" + level.Data2List.Length + "):");
            foreach (Data2 i in level.Data2List)
            {
                Console.WriteLine("    " + i.ToString());
            }
        }
    }
}