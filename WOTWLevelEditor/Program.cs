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

            UnityObject selected = level.FindObjectByID(1);
            while (true)
            {
                string? command = Console.ReadLine();
                if (command == null)
                {
                    continue;
                }
                string[] commandArgs = command.Split(' ');
                switch (commandArgs[0]) {
                    case "logall":
                        string path = Directory.GetCurrentDirectory() + "\\object_list.log";
                        Console.WriteLine("this may take a while for large files...");
                        File.WriteAllText(path, level.ObjectListToString());
                        Console.WriteLine("wrote level contents to " + path);
                        break;
                    case "select":
                        List<GameObject> objects = level.FindGameObjectsByName(commandArgs[1]);
                        if (objects.Count > 0)
                        {
                            selected = objects[int.Parse(commandArgs[2])];
                            Console.WriteLine(selected.ID.ToString() + ", " + selected.ThisType.ToString() + ": " + selected.ToString());
                        }
                        else
                        {
                            Console.WriteLine("there are no objects with that name");
                        }
                        break;
                    case "selectid":
                        selected = level.FindObjectByID(int.Parse(commandArgs[1]));
                        Console.WriteLine(selected.ID.ToString() + ", " + selected.ThisType.ToString() + ": " + selected.ToString());
                        break;
                    case "listselected":
                        Console.WriteLine(selected.ID.ToString() + ", " + selected.ThisType.ToString() + ": " + selected.ToString());
                        break;
                    case "setpos":
                        if (selected is Transform tra)
                        {
                            tra.Position = new(float.Parse(commandArgs[1]), float.Parse(commandArgs[2]), float.Parse(commandArgs[3]));
                            Console.WriteLine("done");
                            break;
                        }
                        else if (selected is GameObject gob)
                        {
                            gob.ThisTransform.Position = new(float.Parse(commandArgs[1]), float.Parse(commandArgs[2]), float.Parse(commandArgs[3]));
                            Console.WriteLine("done");
                            break;
                        }
                        Console.WriteLine("this command only works on transforms or gameobjects");
                        break;
                    case "move":
                        if (selected is Transform tra2)
                        {
                            tra2.Position += new System.Numerics.Vector3(float.Parse(commandArgs[1]), float.Parse(commandArgs[2]), float.Parse(commandArgs[3]));
                            Console.WriteLine("done");
                            break;
                        }
                        else if (selected is GameObject gob2)
                        {
                            gob2.ThisTransform.Position += new System.Numerics.Vector3(float.Parse(commandArgs[1]), float.Parse(commandArgs[2]), float.Parse(commandArgs[3]));
                            Console.WriteLine("done");
                            break;
                        }
                        Console.WriteLine("this command only works on transforms or gameobjects");
                        break;
                    case "delete":
                        level.DeleteObject(new(selected.ID));
                        Console.WriteLine("done");
                        break;
                    case "save":
                        byte[] output = level.Encode();
                        File.WriteAllBytes(args[0], output);
                        Console.WriteLine("saved to " + args[0]);
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("unknown command");
                        break;
                }
            }
        }
    }
}