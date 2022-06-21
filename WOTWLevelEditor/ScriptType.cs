using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTWLevelEditor
{
    public struct ScriptType
    {
        public string Name { get; }
        public List<Type> Parameters { get; }
        public byte Suffix { get; }

        public ScriptType(byte[] pattern)
        {
            if (pattern.Length != 33)
            {
                throw new ArgumentException("Pattern length is 32 bytes + 1 suffix byte.");
            }
            byte[] name = pattern[0..16];
            Name = TypeFromBytePattern(name);
            byte[] parameters = pattern[17..32];
            Parameters = ParametersFromBytePattern(parameters);
            Suffix = pattern[32];
        }

        public static string TypeFromBytePattern(byte[] pattern)
        {
            return BitConverter.ToString(pattern) switch
            {
                _ => BitConverter.ToString(pattern)
            };
        }

        public static List<Type> ParametersFromBytePattern(byte[] pattern)
        {
            return BitConverter.ToString(pattern) switch
            {
                "71 BB 6A 6B 6C 8F 05 2F 94 8D B6 4C 7D D3 CA 4F" => new(),
                _ => new()
            };
        }
    }
}
