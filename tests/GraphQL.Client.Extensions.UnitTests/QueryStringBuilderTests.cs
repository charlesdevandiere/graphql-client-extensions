using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GraphQL.Client.Extensions.UnitTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphQL.Client.Extensions.UnitTests
{
    [TestClass]
    public class QueryStringBuilderTests
    {
        public QueryStringBuilderTests()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-us", false);
        }
        
        private static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        [TestMethod]
        public void BuildQueryParam_IntType_ParseInt()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            string intStr = queryString.BuildQueryParam(123);

            // Assert
            Assert.AreEqual("123", intStr);
        }

        [TestMethod]
        public void BuildQueryParam_QuotedStringType_ParseString()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            string strStr = queryString.BuildQueryParam("Haystack");

            // Assert
            Assert.AreEqual("\"Haystack\"", strStr);
        }

        [TestMethod]
        public void BuildQueryParam_DoubleType_ParseDouble()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            string doubleStr = queryString.BuildQueryParam(1234.5678);

            // Assert
            Assert.AreEqual("1234.5678", doubleStr);
        }

        [TestMethod]
        public void BuildQueryParam_EnumType_ParseEnum()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            EnumHelper enumDisabled = new EnumHelper("DISABLED");

            // Act
            string enumStr = queryString.BuildQueryParam(enumDisabled);

            // Assert
            Assert.AreEqual("DISABLED", enumStr);
        }

        [TestMethod]
        public void BuildQueryParam_CustomType_ParseCustom()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Dictionary<string, object> fromToMap = new Dictionary<string, object>
            {
                {"from", 444.45},
                {"to", 555.45}
            };

            // Act
            string fromToMapStr = queryString.BuildQueryParam(fromToMap);

            // Assert
            Assert.AreEqual("{from:444.45, to:555.45}", fromToMapStr);
        }

        [TestMethod]
        public void BuildQueryParam_ListType_ParseList()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            List<int> intList = new List<int>(new[] { 43783, 43784, 43145 });
            string intListStr = queryString.BuildQueryParam(intList);

            // Assert
            Assert.AreEqual("[43783, 43784, 43145]", intListStr);
        }

        [TestMethod]
        public void BuildQueryParam_StringListType_ParseStringList()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            List<string> strList = new List<string>(new[] { "DB7", "DB9", "Vantage" });
            string strListStr = queryString.BuildQueryParam(strList);

            // Assert
            Assert.AreEqual("[\"DB7\", \"DB9\", \"Vantage\"]", strListStr);
        }

        [TestMethod]
        public void BuildQueryParam_DoubleListType_ParseDoubleList()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            List<double> doubleList = new List<double>(new[] { 123.456, 456, 78.901 });
            string doubleListStr = queryString.BuildQueryParam(doubleList);

            // Assert
            Assert.AreEqual("[123.456, 456, 78.901]", doubleListStr);
        }

        [TestMethod]
        public void BuildQueryParam_EnumListType_ParseEnumList()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            EnumHelper enumDisabled = new EnumHelper("DISABLED");
            EnumHelper enumEnabled = new EnumHelper("ENABLED");
            EnumHelper enumHaystack = new EnumHelper("HAYstack");

            // Act
            List<EnumHelper> enumList = new List<EnumHelper>(new[] { enumEnabled, enumDisabled, enumHaystack });
            string enumListStr = queryString.BuildQueryParam(enumList);

            // Assert
            Assert.AreEqual("[ENABLED, DISABLED, HAYstack]", enumListStr);
        }

        [TestMethod]
        public void BuildQueryParam_NestedListType_ParseNestedList()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            List<object> objList = new List<object>(new object[] { "aa", "bb", "cc" });
            EnumHelper enumHaystack = new EnumHelper("HAYstack");

            Dictionary<string, object> fromToMap = new Dictionary<string, object>
            {
                {"from", 444.45},
                {"to", 555.45},
            };

            Dictionary<string, object> nestedListMap = new Dictionary<string, object>
            {
                {"from", 123},
                {"to", 454},
                {"recurse", objList},
                {"map", fromToMap},
                {"name",  enumHaystack}
            };

            // Act
            string nestedListMapStr= queryString.BuildQueryParam(nestedListMap);

            // Assert
            Assert.AreEqual("{from:123, to:454, recurse:[\"aa\", \"bb\", \"cc\"], map:{from:444.45, to:555.45}, name:HAYstack}", nestedListMapStr);
        }

        [TestMethod]
        public void Where_QueryString_ParseQueryString()
        {
            // Arrange
            Query<Car> query = new Query<Car>("test1");

            List<object> objList = new List<object>(new object[] { "aa", "bb", "cc" });
            EnumHelper enumHaystack = new EnumHelper("HAYstack");

            Dictionary<string, object> fromToMap = new Dictionary<string, object>
            {
                {"from", 444.45},
                {"to", 555.45},
            };

            Dictionary<string, object> nestedListMap = new Dictionary<string, object>
            {
                {"from", 123},
                {"to", 454},
                {"recurse", objList},
                {"map", fromToMap},
                {"name",  enumHaystack}
            };

            query
                .Select("name")
                .SetArguments(nestedListMap);

            IQueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            queryString.AddParams(query);

            string addParamStr = RemoveWhitespace(queryString.QueryString.ToString());

            // Assert
            Assert.AreEqual(RemoveWhitespace("from:123,to:454,recurse:[\"aa\",\"bb\",\"cc\"],map:{from:444.45,to:555.45},name:HAYstack"), addParamStr);
        }

        [TestMethod]
        public void Where_ClearQueryString_EmptyQueryString()
        {
            // Arrange
            var query = new Query<object>("test1");

            List<object> objList = new List<object>(new object[] { "aa", "bb", "cc" });
            EnumHelper enumHaystack = new EnumHelper("HAYstack");

            Dictionary<string, object> fromToMap = new Dictionary<string, object>
            {
                {"from", 444.45},
                {"to", 555.45},
            };

            Dictionary<string, object> nestedListMap = new Dictionary<string, object>
            {
                {"from", 123},
                {"to", 454},
                {"recurse", objList},
                {"map", fromToMap},
                {"name",  enumHaystack}
            };

            query
                .Select("name")
                .SetArguments(nestedListMap);

            IQueryStringBuilder queryString = query.Builder;

            queryString.AddParams(query);

            // Act
            queryString.QueryString.Clear();

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(queryString.QueryString.ToString()));
        }

        [TestMethod]
        public void Select_QueryString_ParseQueryString()
        {
            // Arrange
            var query = new Query<object>("test1");
            var subSelect = new Query<object>("subSelect");

            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");

            List<object> subSelList = new List<object>(new object[] { "subName", "subMake", "subModel" });

            Dictionary<string, object> mySubDict = new Dictionary<string, object>
            {
                {"subMake", "aston martin"},
                {"subState", "ca"},
                {"subLimit", 1},
                {"__debug", gqlEnumDisabled},
                {"SuperQuerySpeed", gqlEnumEnabled }
            };

            subSelect
                .Select(subSelList)
                .SetArguments(mySubDict);

            // create a sub-select too
            List<object> selList = new List<object>(new object[] { "id", subSelect, "name", "make", "model" });

            query.Select("more").Select("things").Select("in_a_select")
                .Select(selList);

            // Act
            query.Builder.AddFields(query);
            string addParamStr = RemoveWhitespace(query.Builder.QueryString.ToString());

            // Assert
            Assert.AreEqual(RemoveWhitespace("morethingsin_a_selectidsubSelect(subMake:\"aston martin\",subState:\"ca\",subLimit:1,__debug:DISABLED,SuperQuerySpeed:ENABLED){subNamesubMakesubModel}namemakemodel"), addParamStr);
        }

        [TestMethod]
        public void Build_AllElements_StringMatch()
        {
            // Arrange
            var query = new Query<object>("test1");
            var subSelect = new Query<object>("subSelect");

            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");

            List<object> subSelList = new List<object>(new object[] { "subName", "subMake", "subModel" });

            Dictionary<string, object> mySubDict = new Dictionary<string, object>
            {
                {"subMake", "aston martin"},
                {"subState", "ca"},
                {"subLimit", 1},
                {"__debug", gqlEnumDisabled},
                {"SuperQuerySpeed", gqlEnumEnabled }
            };

            subSelect
                .Select(subSelList)
                .SetArguments(mySubDict);

            List<object> selList = new List<object>(new object[] { "id", subSelect, "name", "make", "model" });

            query.Alias("test1Alias")
                .Select("more").Select("things").Select("in_a_select")
                .Select(selList)

            IQueryStringBuilder queryString = query.Builder;

            // Act
            string buildStr = RemoveWhitespace(queryString.Build(query));

            // Assert
            Assert.AreEqual(RemoveWhitespace("test1Alias:test1{#AsinglelineCommentmorethingsin_a_selectidsubSelect(subMake:\"aston martin\",subState:\"ca\",subLimit:1,__debug:DISABLED,SuperQuerySpeed:ENABLED){subNamesubMakesubModel}namemakemodel}"), buildStr);
        }
    }
}
