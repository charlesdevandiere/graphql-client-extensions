using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace GraphQL.Client.Extensions.UnitTests
{
    [TestClass]
    public class QueryOfTTests
    {
        #region classes
        class Car
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public Color Color { get; set; }
        }

        class Color
        {
            public byte Red { get; set; }
            public byte Green { get; set; }
            public byte Blue { get; set; }
        }

        class Truck
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("weelsNumber")]
            public int WeelsNumber { get; set; }

            [JsonProperty("load")]
            public Load Load { get; set; }
        }

        class Load
        {
            [JsonProperty("weight")]
            public int Weight { get; set; }
        }
        #endregion

        [TestMethod]
        public void TestSelect()
        {
            var query = new Query<Car>();
            query.Select(c => c.Name);

            CollectionAssert.AreEqual(new List<string> { nameof(Car.Name) }, query.SelectList);
        }

        [TestMethod]
        public void TestSubSelect()
        {
            var query = new Query<Car>();
            query.SubSelect(c => c.Color, new Query<Color>());

            Assert.AreEqual(nameof(Car.Color), (query.SelectList[0] as IQuery).QueryName);
        }

        [TestMethod]
        public void TestSelectWithCustomName()
        {
            var query = new Query<Truck>();
            query.Select(c => c.Name);

            CollectionAssert.AreEqual(new List<string> { "name" }, query.SelectList);
        }

        [TestMethod]
        public void TestSubSelectWithCustomName()
        {
            var query = new Query<Truck>();
            query.SubSelect(c => c.Load, new Query<Load>());

            Assert.AreEqual("load", (query.SelectList[0] as IQuery).QueryName);
        }
    }
}