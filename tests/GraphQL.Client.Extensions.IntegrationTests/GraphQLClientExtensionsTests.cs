using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;

namespace GraphQL.Client.Extensions.IntegrationTests
{
    [TestClass]
    [DeploymentItem("TestData/batch-query-response-data.json")]
    [DeploymentItem("TestData/nearest-dealer-response-data.json")]
    public class GraphQLClientExtensionsTests
    {
        public GraphQLClientExtensionsTests()
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
        public void Query_SelectParams_ReturnsCorrect()
        {
            const string select = "zip";

            // Arrange
            var query = new Query<object>("test1").Select(select);

            // Assert
            Assert.AreEqual(select, query.SelectList.First());
        }

        [TestMethod]
        public void Query_Unique_ReturnsCorrect()
        {
            // Arrange
            var query = new Query<object>("test1").Select("zip");
            var query1 = new Query<object>("test1").Select("pitydodah");

            // Assert counts and not the same
            Assert.IsTrue(query.SelectList.Count == 1);
            Assert.IsTrue(query1.SelectList.Count == 1);
            Assert.AreNotEqual(query.SelectList.First(), query1.SelectList.First());
        }

        [TestMethod]
        public void Query_Build_ReturnsCorrect()
        {
            // Arrange
            var query = new Query<object>("test1").Select("id");

            // Assert
            Assert.AreEqual("test1{id}", RemoveWhitespace(query.Build()));
        }

        [TestMethod]
        public void ComplexQuery_Build_Check()
        {
            // Arrange
            
            // set up a couple of ENUMS
            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");

            // set up a subselection parameter (where)
            // has simple string, int, and a couple of ENUMs
            Dictionary<string, object> mySubDict = new Dictionary<string, object>
            {
                {"subMake", "aston martin"},
                {"subState", "ca"},
                {"subLimit", 1},
                {"_debug", gqlEnumDisabled},
                {"SuperQuerySpeed", gqlEnumEnabled}
            };

            // List of int's (IDs)
            List<int> trimList = new List<int>(new[] { 143783, 243784, 343145 });

            // String List
            List<string> modelList = new List<string>(new[] { "DB7", "DB9", "Vantage" });

            // Another List but of Generic Objects that should work as strings
            List<object> recList = new List<object>(new object[] { "aa", "bb", "cc" });

            // try a dict for the typical from to with doubles
            Dictionary<string, object> recMap = new Dictionary<string, object>
            {
                {"from", 444.45},
                {"to", 555.45},
            };

            // try a dict for nested list and dict
            Dictionary<string, object> fromToPrice = new Dictionary<string, object>
            {
                {"from", 123},
                {"to", 454},
                {"recurse", recList},
                {"map", recMap}
            };

            // Even more stuff nested in the params
            Dictionary<string, object> myDict = new Dictionary<string, object>
            {
                {"make", "aston martin"},
                {"state", "ca"},
                {"limit", 2},
                {"trims", trimList},
                {"models", modelList},
                {"price", fromToPrice},
                {"_debug", gqlEnumEnabled},
            };

            var query = new Query<object>("Dealer")
                .Alias("myDealerAlias")
                .Select("id")
                .SubSelect<object>("subDealer", q => q
                    .Select("subName")
                    .Select("subMake")
                    .Select("subModel")
                    .SetArguments(mySubDict)
                    )
                .Select("name")
                .Select("make")
                .Select("model")
                .SetArguments(myDict);

            // Get and pack results
            string packedResults = RemoveWhitespace(query.Build());
            string packedCheck = RemoveWhitespace(@"
                    myDealerAlias: Dealer(make: ""aston martin"", state: ""ca"", limit: 2, trims:[143783, 243784, 343145], models:[""DB7"", ""DB9"", ""Vantage""],
                    price:{ from: 123, to: 454, recurse:[""aa"", ""bb"", ""cc""], map: { from: 444.45, to: 555.45} },
                    _debug: ENABLED){
                    id
                    subDealer(subMake: ""aston martin"", subState: ""ca"", subLimit: 1, _debug: DISABLED, SuperQuerySpeed: ENABLED){
                        subName
                        subMake
                        subModel
                    }
                    name
                    make
                    model
                }");

            // Best be the same!
            Assert.AreEqual(packedResults, packedCheck);
        }
    }
}
