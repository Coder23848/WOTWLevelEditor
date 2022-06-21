﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTWLevelEditor
{
    public class ScriptType : ObjectType
    {
        public List<Type> Parameters { get; }

        public ScriptType(byte[] pattern)
        {
            if (pattern.Length != 39)
            {
                throw new ArgumentException("Pattern length is 39, not " + pattern.Length);
            }
            byte[] name = pattern[7..23];
            Name = TypeFromBytePattern(name);
            byte[] parameters = pattern[17..32];
            Parameters = ParametersFromBytePattern(parameters);
            Prefix = pattern[32];
        }

        private static string TypeFromBytePattern(byte[] pattern)
        {
            return BitConverter.ToString(pattern) switch
            {
                _ => BitConverter.ToString(pattern)
            };
        }

        private static List<Type> ParametersFromBytePattern(byte[] pattern)
        {
            return BitConverter.ToString(pattern) switch
            {
                "71 BB 6A 6B 6C 8F 05 2F 94 8D B6 4C 7D D3 CA 4F" => new(),
                _ => new()
            };
        }
    }
}
