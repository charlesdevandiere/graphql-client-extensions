using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphQL.Client.Extensions.UnitTests.Models;

namespace GraphQL.Client.Extensions.UnitTests
{
    [TestClass]
    public class QueryOfTTests
    {
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
