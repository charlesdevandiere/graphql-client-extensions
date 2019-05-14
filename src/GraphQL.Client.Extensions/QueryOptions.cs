using System;

namespace GraphQL.Client.Extensions
{
    public class QueryOptions
    {
        public Func<string, string> Formater { get; set; }
    }

    public static class QueryFormaters
    {
        public static Func<string, string> CamelCaseFormater = str =>
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
            {
                return str;
            }

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        };
    }
}
