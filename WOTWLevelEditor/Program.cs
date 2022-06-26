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

            level.PrintObjectList();

            Console.ReadLine();

            byte[] output = level.Encode();

            File.WriteAllBytes(args[0], output);
        }
    }
}