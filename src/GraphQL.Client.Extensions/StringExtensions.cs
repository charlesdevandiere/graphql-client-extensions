namespace System
{
    /// <summary>
    /// Helper extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the string to camelCase.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The string with first letter lowercased.</returns>
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
            {
                return str;
            }

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}
