﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTWLevelEditor
{
    public class ObjectType
    {
        public ObjectTypes Type { get; init; }
        public byte[] Prefix { get; init; }
        public byte[] Parameters { get; init; }

        

        public ObjectType(byte[] pattern)
        {
            if (pattern.Length != 23)
            {
                throw new ArgumentException("Pattern length is 23, not " + pattern.Length);
            }
            Type = (ObjectTypes)pattern[0];
            Prefix = pattern[1..7];
            Parameters = pattern[7..23];
        }
        protected ObjectType() // I don't like this
        {
            Parameters = new byte[16];
            Prefix = new byte[6];
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}
