using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GraphQL.Client.Extensions.UnitTests
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void Select_StringList_AddsToQuery()
        {
            // Arrange
            var query = new Query<object>("something");

            List<string> selectList = new List<string>()
            {
                "id",
                "name"
            };

            // Act
            foreach( string field in selectList)
            {
                query.AddField(field);
            }

            // Assert
            CollectionAssert.AreEqual(selectList, query.SelectList);
        }

        [TestMethod]
        public void From_String_AddsToQuery()
        {
            // Arrange
            const string name = "user";

            var query = new Query<object>(name);

            // Assert
            Assert.AreEqual(name, query.Name);
        }

        [TestMethod]
        public void Select_String_AddsToQuery()
        {
            // Arrange
            var query = new Query<object>("something");

            const string select = "id";

            // Act
            query.AddField(select);

            // Assert
            Assert.AreEqual(select, query.SelectList.First());
        }

        [TestMethod]
        public void Select_DynamicArguments_AddsToQuery()
        {
            // Arrange
            var query = new Query<object>("something");

            // Act
            query.AddField("some").AddField("thing").AddField("else");

            // Assert
            List<string> shouldEqual = new List<string>()
            {
                "some",
                "thing",
                "else"
            };
            CollectionAssert.AreEqual(shouldEqual, query.SelectList);
        }

        [TestMethod]
        public void Select_ArrayOfString_AddsToQuery()
        {
            // Arrange
            var query = new Query<object>("something");

            string[] selects =
            {
                "id",
                "name"
            };

            // Act
            foreach (string field in selects)
            {
                query.AddField(field);
            }

            // Assert
            List<string> shouldEqual = new List<string>()
            {
                "id",
                "name"
            };
            CollectionAssert.AreEqual(shouldEqual, query.SelectList);
        }

        [TestMethod]
        public void Select_ChainCombinationOfStringAndList_AddsToQuery()
        {
            // Arrange
            var query = new Query<object>("something");

            const string select = "id";
            List<string> selectList = new List<string>()
            {
                "name",
                "email"
            };
            string[] selectStrings =
            {
                "array",
                "cool"
            };

            // Act
            query.AddField(select);
            foreach (string field in selectList)
            {
                query.AddField(field);
            }
            query.AddField("some").AddField("thing").AddField("else");
            foreach (string field in selectStrings)
            {
                query.AddField(field);
            }

            // Assert
            List<string> shouldEqual = new List<string>()
            {
                "id",
                "name",
                "email",
                "some",
                "thing",
                "else",
                "array",
                "cool"
            };
            CollectionAssert.AreEqual(shouldEqual, query.SelectList);
        }

        [TestMethod]
        public void Where_IntegerArgumentWhere_AddsToWhere()
        {
            // Arrange
            var query = new Query<object>("something");

            // Act
            query.AddArgument("id", 1);

            // Assert
            Assert.AreEqual(1, query.ArgumentsMap["id"]);
        }

        [TestMethod]
        public void Where_StringArgumentWhere_AddsToWhere()
        {
            // Arrange
            var query = new Query<object>("something");

            // Act
            query.AddArgument("name", "danny");

            // Assert
            Assert.AreEqual("danny", query.ArgumentsMap["name"]);
        }

        [TestMethod]
        public void Where_DictionaryArgumentWhere_AddsToWhere()
        {
            // Arrange
            var query = new Query<object>("something");

            Dictionary<string, int> dict = new Dictionary<string, int>()
            {
                {"from", 1},
                {"to", 100}
            };

            // Act
            query.AddArgument("price", dict);

            // Assert
            Dictionary<string, int> queryWhere = (Dictionary<string, int>) query.ArgumentsMap["price"];
            Assert.AreEqual(1, queryWhere["from"]);
            Assert.AreEqual(100, queryWhere["to"]);
            CollectionAssert.AreEqual(dict, (ICollection) query.ArgumentsMap["price"]);
        }

        [TestMethod]
        public void Where_ChainedWhere_AddsToWhere()
        {
            // Arrange
            var query = new Query<object>("something");

            Dictionary<string, int> dict = new Dictionary<string, int>()
            {
                {"from", 1},
                {"to", 100}
            };

            // Act
            query
                .AddArgument("id", 123)
                .AddArgument("name", "danny")
                .AddArgument("price", dict);

            // Assert
            Dictionary<string, object> shouldPass = new Dictionary<string, object>()
            {
                {"id", 123},
                {"name", "danny"},
                {"price", dict}
            };
            CollectionAssert.AreEqual(shouldPass, query.ArgumentsMap);
        }

        [TestMethod]
        public void Check_Required_Select()
        {
            // Arrange
            var query = new Query<object>(null)
                .AddField("something");

            // Assert
            Assert.ThrowsException<ArgumentException>(() => query.Build());
        }

        [TestMethod]
        public void Check_Required_Name()
        {
            // Arrange
            var query = new Query<object>("something");

            // Assert
            Assert.ThrowsException<ArgumentException>(() => query.Build());
        }
    }
}
