using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTWLevelEditor
{
    public struct ObjectType
    {
        public byte Prefix { get; }
        public string Name { get; }

        public ObjectType(byte[] pattern)
        {
            if (pattern.Length != 23)
            {
                throw new ArgumentException("Pattern length is 23, not " + pattern.Length);
            }
            byte[] name = pattern[7..23];
            Name = TypeFromBytePattern(name);
            Prefix = pattern[0];
        }

        public static string TypeFromBytePattern(byte[] pattern)
        {
            return BitConverter.ToString(pattern) switch
            {
                "9D-16-30-B1-D7-E3-BC-24-AB-E4-6E-58-60-A2-A1-79" => "Mesh Filter",
                "5B-7F-3F-79-84-59-EA-61-9E-49-27-10-6F-D1-FE-4B" => "Mesh Collider",
                "43-9E-66-A8-9C-7F-DE-F6-F0-92-7C-92-56-C0-77-3A" => "Box Collider",
                "8C-86-F4-AC-6B-7F-47-C0-45-56-0D-43-83-AA-DC-8D" => "Sphere Collider",
                "FB-F6-B0-31-C3-2E-46-1E-E6-1B-E4-29-E5-FE-E7-EB" => "Capsule Collider",
                _ => BitConverter.ToString(pattern)
            };
        }
    }
}
