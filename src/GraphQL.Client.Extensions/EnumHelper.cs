namespace GraphQL.Client.Extensions
{
    /// <summary>
    /// Helper class that will allow us to pass a string but
    /// keep the idea that it's really a non-quoted ENUM in the
    /// gQL realm. So the idea would be to pass this as a param
    /// value not a string and it will be rendered without being
    /// quoted
    /// </summary>
    /// <example>
    /// Creating Instance -
    ///     EnumHelper GqlEnumEnabled = new EnumHelper().Enum("ENABLED");
    ///     EnumHelper GqlEnumDisabled = new EnumHelper("DISABLED");
    ///     GqlEnumDisabled.Enum("SOMETHING_ENUM");
    ///
    /// In use -
    ///     Creating a Dictionary for a Select (gQL Parameters)
    ///     Dictionary &lt;string, object&gt; mySubDict = new Dictionary&lt;string, object&gt;
    ///     {
    ///         {"subMake", "aston martin"},
    ///         {"subState", "ca"},
    ///         {"_debug", GqlEnumDisabled},
    ///         {"SuperQuerySpeed", GqlEnumEnabled }
    ///     };
    /// </example>
    public class EnumHelper
    {
        private string _str;

        /// <summary>
        /// Create the Helper with a string, default empty.
        /// </summary>
        /// <param name="enumStr">String to represent as an ENUM to GraphQL, default is the empty string</param>
        public EnumHelper(string enumStr = "")
        {
            _str = enumStr;
        }

        /// <summary>
        /// Set the Enum value
        /// </summary>
        /// <param name="enumStr">The string we want to represent as a GraphQL enum</param>
        /// <returns>this</returns>
        public EnumHelper Enum(string enumStr)
        {
            _str = enumStr;
            return this;
        }

        /// <summary>
        /// Gets the Enum as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _str;
        }
    }
}
