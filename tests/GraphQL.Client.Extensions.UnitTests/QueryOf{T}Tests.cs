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
            query.Select(t => t.Name);

            CollectionAssert.AreEqual(new List<string> { "name" }, query.SelectList);
        }

        [TestMethod]
        public void TestSubSelectWithCustomName()
        {
            var query = new Query<Truck>();
            query.SubSelect(t => t.Load, new Query<Load>());

            Assert.AreEqual("load", (query.SelectList[0] as IQuery).QueryName);
        }

        [TestMethod]
        public void TestSelectWithCustomFormater()
        {
            var query = new Query<Car>(options: new QueryOptions
            {
                Formater = QueryFormaters.CamelCaseFormater
            });
            query.Select(c => c.Name);

            CollectionAssert.AreEqual(new List<string> { "name" }, query.SelectList);
        }

        [TestMethod]
        public void TestSubSelectWithCustomFormater()
        {
            var query = new Query<Car>(options: new QueryOptions
            {
                Formater = QueryFormaters.CamelCaseFormater
            });
            query.SubSelect(c => c.Color, new Query<Color>());

            Assert.AreEqual("color", (query.SelectList[0] as IQuery).QueryName);
        }

        [TestMethod]
        public void TestQuery()
        {
            var query = new Query<Car>();
            query.Name(nameof(Car))
                .Select(car => car.Name)
                .Select(car => car.Price)
                .SubSelect(
                    car => car.Color,
                    new Query<Color>()
                        .Select(color => color.Red)
                        .Select(color => color.Green)
                        .Select(color => color.Blue));

            Assert.AreEqual(nameof(Car), query.QueryName);
            Assert.AreEqual(3, query.SelectList.Count);
            Assert.AreEqual(nameof(Car.Name), query.SelectList[0]);
            Assert.AreEqual(nameof(Car.Price), query.SelectList[1]);

            Assert.AreEqual(nameof(Car.Color), (query.SelectList[2] as IQuery).QueryName);
            var expectedSubSelectList = new List<string>
            {
                nameof(Color.Red),
                nameof(Color.Green),
                nameof(Color.Blue)
            };
            CollectionAssert.AreEqual(expectedSubSelectList, (query.SelectList[2] as IQuery).SelectList);
        }

        [TestMethod]
        public void TestQueryWithCustomName()
        {
            var query = new Query<Truck>();
            query.Name("truck")
                .Select(truck => truck.Name)
                .Select(truck => truck.WeelsNumber)
                .SubSelect(
                    truck => truck.Load,
                    new Query<Load>()
                        .Select(load => load.Weight));

            Assert.AreEqual("truck", query.QueryName);
            Assert.AreEqual(3, query.SelectList.Count);
            Assert.AreEqual("name", query.SelectList[0]);
            Assert.AreEqual("weelsNumber", query.SelectList[1]);

            Assert.AreEqual("load", (query.SelectList[2] as IQuery).QueryName);
            var expectedSubSelectList = new List<string>
            {
                "weight"
            };
            CollectionAssert.AreEqual(expectedSubSelectList, (query.SelectList[2] as IQuery).SelectList);
        }
    }
}
