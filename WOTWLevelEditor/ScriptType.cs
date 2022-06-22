namespace WOTWLevelEditor
{
    /// <summary>
    /// Represents a Unity script type.
    /// </summary>
    public class ScriptType : ObjectType
    {
        public string Name { get; }
        public string Parameters { get; }

        public ScriptType(byte[] pattern)
        {
            if (pattern.Length != 39)
            {
                throw new ArgumentException("Pattern length is 39, not " + pattern.Length);
            }
            byte[] name = pattern[7..23];
            Name = NameFromBytePattern(name);
            byte[] parameters = pattern[23..39];
            Parameters = ParametersFromBytePattern(parameters);
            Type = (ObjectTypes)pattern[0];
            Prefix = pattern[1..7];
        }

        private static string NameFromBytePattern(byte[] pattern)
        {
            return BitConverter.ToString(pattern) switch
            {
                _ => "[" + BitConverter.ToString(pattern) + "]"
            };
        }

        private static string ParametersFromBytePattern(byte[] pattern)
        {
            return BitConverter.ToString(pattern) switch
            {
                "71-BB-6A-6B-6C-8F-05-2F-94-8D-B6-4C-7D-D3-CA-4F" => "()",
                _ => "[" + BitConverter.ToString(pattern) + "]"
            };
        }
        public override string ToString()
        {
            return Name + Parameters;
        }
    }
}