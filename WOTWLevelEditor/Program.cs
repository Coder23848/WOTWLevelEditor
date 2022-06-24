using WOTWLevelEditor.Objects;

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
            Level level = new(bytes);

            //level.PrintObjectList();

            Console.WriteLine("Reloading...");
            byte[] test = level.Encode();
            Level level2 = new(test);

            Console.WriteLine("Reloading...");
            byte[] test2 = level2.Encode();
            Level level3 = new(test2);

            level3.PrintObjectList();
        }
    }
}