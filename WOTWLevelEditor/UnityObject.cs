using System.Numerics;
using WOTWLevelEditor.Objects;

namespace WOTWLevelEditor
{
    public class UnityObject
    {
        public Level ParentLevel { get; }
        public ObjectType ThisType { get; }
        public int ID { get; }

        protected object[] parameters;

        protected UnityObject(Level level, ObjectType type, int id, object[] parameters)
        {
            ParentLevel = level;
            ThisType = type;
            ID = id;
            this.parameters = parameters;
        }

        public static UnityObject Parse(Level level, ObjectType type, int id, byte[] bytes)
        {
            int parserLocation = 0;
            Type[] signature = type.GetSignature();
            object[] parameters = new object[signature.Length];
            for (int i = 0; i < signature.Length; i++)
            {
                parameters[i] = ParseType(level, signature[i], bytes, ref parserLocation);
            }
            return type.Type switch
            {
                ObjectTypes.GameObject => new GameObject(level, type, id, parameters),
                ObjectTypes.Transform => new Transform(level, type, id, parameters),
                _ => new UnknownFallback(level, type, id, bytes)
            };
        }
        private static object ParseType(Level level, Type type, byte[] bytes, ref int parserLocation)
        {
            object result;
            if (type == typeof(int))
            {
                result = BitConverter.ToInt32(bytes, parserLocation);
                parserLocation += 4;
            }
            else if (type == typeof(byte))
            {
                result = bytes[parserLocation];
                parserLocation ++;
            }
            else if (type == typeof(bool))
            {
                result = bytes[parserLocation] != 0;
                parserLocation++;
            }
            else if (type == typeof(Vector3))
            {
                result = new Vector3(BitConverter.ToSingle(bytes, parserLocation),
                                     BitConverter.ToSingle(bytes, parserLocation + 4),
                                     BitConverter.ToSingle(bytes, parserLocation + 8));
                parserLocation += 12;
            }
            else if (type == typeof(Quaternion))
            {
                result = new Quaternion(BitConverter.ToSingle(bytes, parserLocation),
                                        BitConverter.ToSingle(bytes, parserLocation + 4),
                                        BitConverter.ToSingle(bytes, parserLocation + 8),
                                        BitConverter.ToSingle(bytes, parserLocation + 12));
                parserLocation += 16;
            }
            else if (type == typeof(ObjectID))
            {
                result = new ObjectID(BitConverter.ToInt32(bytes, parserLocation),
                                      BitConverter.ToInt32(bytes, parserLocation + 4),
                                      BitConverter.ToInt32(bytes, parserLocation + 8));
                parserLocation += 12;
            }
            else if (type == typeof(string))
            {
                int length = BitConverter.ToInt32(bytes, parserLocation);
                parserLocation += 4;
                result = System.Text.Encoding.ASCII.GetString(bytes, parserLocation, length);
                parserLocation += length;
                while (parserLocation % 4 != 0)
                {
                    parserLocation++;
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listType = type.GetGenericArguments()[0];
                object list = typeof(List<>).MakeGenericType(listType).GetConstructor(Array.Empty<Type>()).Invoke(Array.Empty<object>());
                int length = BitConverter.ToInt32(bytes, parserLocation);
                parserLocation += 4;
                for (int i = 0; i < length; i++)
                {
                    list.GetType().GetMethod("Add").Invoke(list, new object[] { ParseType(level, listType, bytes, ref parserLocation) });
                }
                result = list;
            }
            else if (type == typeof(Array))
            {
                Type thisType = type.GenericTypeArguments[0];
                Array contents = Array.CreateInstance(thisType, BitConverter.ToInt32(bytes, parserLocation));
                parserLocation += 4;
                for (int i = 0; i < contents.Length; i++)
                {
                    contents.SetValue(ParseType(level, thisType, bytes, ref parserLocation), i);
                }
                result = contents;
            }
            else
            {
                Console.WriteLine("Parsing " + type.FullName + " is not yet supported.");
                result = null;
            }
            return result;
        }

        public byte[] Encode()
        {
            if (this is UnknownFallback unf)
            {
                return unf.Data;
            }
            List<byte> bytes = new();
            for (int i = 0; i < parameters.Length; i++)
            {
                bytes.AddRange(EncodeType(parameters[i]));
            }

            return bytes.ToArray();
        }
        public byte[] EncodeType(object data)
        {
            Type type = data.GetType();
            byte[] result;
            if (type == typeof(int))
            {
                result = BitConverter.GetBytes((int)data);
            }
            else if (type == typeof(byte))
            {
                result = new byte[] { (byte)data };
            }
            else if (type == typeof(bool))
            {
                result = BitConverter.GetBytes((bool)data);
            }
            else if (type == typeof(Vector3))
            {
                result = BitConverter.GetBytes(((Vector3)data).X)
                 .Concat(BitConverter.GetBytes(((Vector3)data).Y))
                 .Concat(BitConverter.GetBytes(((Vector3)data).Z)).ToArray();
            }
            else if (type == typeof(Quaternion))
            {
                result = BitConverter.GetBytes(((Quaternion)data).X)
                 .Concat(BitConverter.GetBytes(((Quaternion)data).Y))
                 .Concat(BitConverter.GetBytes(((Quaternion)data).Z))
                 .Concat(BitConverter.GetBytes(((Quaternion)data).W)).ToArray();
            }
            else if (type == typeof(ObjectID))
            {
                result = BitConverter.GetBytes(0)
                 .Concat(BitConverter.GetBytes(((ObjectID)data).ID))
                 .Concat(BitConverter.GetBytes(0)).ToArray();
            }
            else if (type == typeof(string))
            {
                result = BitConverter.GetBytes(((string)data).Length)
                 .Concat(System.Text.Encoding.ASCII.GetBytes((string)data)).ToArray();
                while (result.Length % 4 != 0)
                {
                    result = result.Append<byte>(0).ToArray();
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Array asArray = (Array)data.GetType().GetMethod("ToArray").Invoke(data, Array.Empty<object>());
                int length = asArray.Length;
                byte[][] listData = new byte[length][];
                for (int i = 0; i < listData.Length; i++)
                {
                    listData[i] = EncodeType(asArray.GetValue(i));
                }
                result = BitConverter.GetBytes(length);
                foreach (byte[] i in listData)
                {
                    result = result.Concat(i).ToArray();
                }
            }
            else
            {
                Console.WriteLine("Encoding " + type.FullName + " is not yet supported.");
                result = null;
            }

            return result;
        }

        public override string ToString()
        {
            return string.Join(", ", parameters);
        }
    }
}