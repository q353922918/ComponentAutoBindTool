namespace Third_Party.ComponentAutoBindTool.Scripts.Core.Editor
{
    internal static class AutoBindFieldNameUtility
    {
        public static string BuildFieldName(string bindName)
        {
            if (string.IsNullOrWhiteSpace(bindName))
            {
                return string.Empty;
            }

            string fieldName = bindName.Replace("_", string.Empty);
            if (fieldName.Length == 0)
            {
                return string.Empty;
            }

            char firstChar = fieldName[0];
            if (firstChar >= 'A' && firstChar <= 'Z')
            {
                fieldName = char.ToLowerInvariant(firstChar) + fieldName.Substring(1);
            }

            return fieldName;
        }

        public static bool IsValidIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return false;
            }

            if (!IsIdentifierStartCharacter(identifier[0]))
            {
                return false;
            }

            for (int i = 1; i < identifier.Length; i++)
            {
                if (!IsIdentifierPartCharacter(identifier[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsIdentifierStartCharacter(char value)
        {
            return value == '_' || IsAsciiLetter(value);
        }

        private static bool IsIdentifierPartCharacter(char value)
        {
            return value == '_' || IsAsciiLetter(value) || (value >= '0' && value <= '9');
        }

        private static bool IsAsciiLetter(char value)
        {
            return (value >= 'a' && value <= 'z') || (value >= 'A' && value <= 'Z');
        }
    }
}
