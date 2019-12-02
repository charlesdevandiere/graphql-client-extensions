using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Shared.Models;
using Xunit;

namespace GraphQL.Client.Extensions.IntegrationTests
{
    public class GraphQLClientExtensionsTests
    {
        const string URL = "https://graphql-pokemon.now.sh";

        enum TestEnum
        {
            ENABLED,
            DISABLED,
            HAYstack
        }

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

        [Fact]
        public void Query_SelectParams_ReturnsCorrect()
        {
            const string select = "zip";

            // Arrange
            var query = new Query<object>("test1").AddField(select);

            // Assert
            Assert.Equal(select, query.SelectList.First());
        }

        [Fact]
        public void Query_Unique_ReturnsCorrect()
        {
            // Arrange
            var query = new Query<object>("test1").AddField("zip");
            var query1 = new Query<object>("test1").AddField("pitydodah");

            // Assert counts and not the same
            Assert.True(query.SelectList.Count == 1);
            Assert.True(query1.SelectList.Count == 1);
            Assert.NotEqual(query.SelectList.First(), query1.SelectList.First());
        }

        [Fact]
        public void Query_Build_ReturnsCorrect()
        {
            // Arrange
            var query = new Query<object>("test1").AddField("id");

            // Assert
            Assert.Equal("test1{id}", RemoveWhitespace(query.Build()));
        }

        [Fact]
        public void ComplexQuery_Build_Check()
        {
            // Arrange

            // set up a subselection parameter (where)
            // has simple string, int, and a couple of ENUMs
            Dictionary<string, object> mySubDict = new Dictionary<string, object>
            {
                {"subMake", "aston martin"},
                {"subState", "ca"},
                {"subLimit", 1},
                {"_debug", TestEnum.DISABLED},
                {"SuperQuerySpeed", TestEnum.ENABLED}
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
                {"_debug", TestEnum.ENABLED},
            };

            var query = new Query<object>("Dealer")
                .Alias("myDealerAlias")
                .AddField("id")
                .AddField<object>("subDealer", q => q
                    .AddField("subName")
                    .AddField("subMake")
                    .AddField("subModel")
                    .AddArguments(mySubDict)
                    )
                .AddField("name")
                .AddField("make")
                .AddField("model")
                .AddArguments(myDict);

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
            Assert.Equal(packedResults, packedCheck);
        }

        [Fact]
        public async Task TestGetBatch()
        {
            Func<string, IQuery<Pokemon>> query = (string name) => new Query<Pokemon>("pokemon")
                .Alias(name)
                .AddArguments(new { name })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

            IReadOnlyDictionary<string, JToken> batch = await client.GetBatch(new IQuery[] { query("Pikachu"), query("Bulbasaur") });

            Pokemon pikachu = batch["Pikachu"].ToObject<Pokemon>();
            Assert.NotNull(pikachu);
            Assert.Equal("Pikachu", pikachu.Name);

            Pokemon bulbasaur = batch["Bulbasaur"].ToObject<Pokemon>();
            Assert.NotNull(bulbasaur);
            Assert.Equal("Bulbasaur", bulbasaur.Name);
        }

        [Fact]
        public async Task TestPostBatch()
        {
            Func<string, IQuery<Pokemon>> query = (string name) => new Query<Pokemon>("pokemon")
                .Alias(name)
                .AddArguments(new { name })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

            IReadOnlyDictionary<string, JToken> batch = await client.PostBatch(new IQuery[] { query("Pikachu"), query("Bulbasaur") });

            Pokemon pikachu = batch["Pikachu"].ToObject<Pokemon>();
            Assert.NotNull(pikachu);
            Assert.Equal("Pikachu", pikachu.Name);

            Pokemon bulbasaur = batch["Bulbasaur"].ToObject<Pokemon>();
            Assert.NotNull(bulbasaur);
            Assert.Equal("Bulbasaur", bulbasaur.Name);
        }

        [Fact]
        public async Task TestStringResult()
        {
            var query = new Query<Pokemon>("pokemon")
                .AddArguments(new { name = "pikachu" })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

            string json = await client.Get<string>(query);

            JToken jToken = JToken.Parse(json);

            Assert.Equal(jToken.Count(), 1);
            Assert.Equal(jToken["name"], "Pikachu");
        }

        [Fact]
        public async Task TestJTokenResult()
        {
            var query = new Query<Pokemon>("pokemon")
                .AddArguments(new { name = "pikachu" })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

            JToken jToken = await client.Get<JToken>(query);

            Assert.Equal(jToken.Count(), 1);
            Assert.Equal(jToken["name"], "Pikachu");
        }
    }
}
