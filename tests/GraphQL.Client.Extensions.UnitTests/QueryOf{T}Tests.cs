using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphQL.Client.Extensions.UnitTests.Models;
using System.Linq;

namespace GraphQL.Client.Extensions.UnitTests
{
    [TestClass]
    public class QueryOfTTests
    {
        [TestMethod]
        public void TestSelect()
        {
            var query = new Query<Car>("car");
            query.AddField(c => c.Name);

            CollectionAssert.AreEqual(new List<string> { nameof(Car.Name) }, query.SelectList);
        }

        [TestMethod]
        public void TestSubSelect()
        {
            var query = new Query<Car>("car");
            query.AddField(c => c.Color, sq => sq);

            Assert.AreEqual(nameof(Car.Color), (query.SelectList[0] as IQuery<Color>).Name);
        }

        [TestMethod]
        public void TestSelectWithCustomName()
        {
            var query = new Query<Truck>("truck");
            query.AddField(t => t.Name);

            CollectionAssert.AreEqual(new List<string> { "name" }, query.SelectList);
        }

        [TestMethod]
        public void TestSubSelectWithCustomName()
        {
            var query = new Query<Truck>("truck");
            query.AddField(t => t.Load, sq => sq);

            Assert.AreEqual("load", (query.SelectList[0] as IQuery<Load>).Name);
        }

        [TestMethod]
        public void TestSelectWithCustomFormater()
        {
            var query = new Query<Car>("car", options: new QueryOptions
            {
                Formater = QueryFormaters.CamelCaseFormater
            });
            query.AddField(c => c.Name);

            CollectionAssert.AreEqual(new List<string> { "name" }, query.SelectList);
        }

        [TestMethod]
        public void TestSubSelectWithCustomFormater()
        {
            var query = new Query<Car>("car", options: new QueryOptions
            {
                Formater = QueryFormaters.CamelCaseFormater
            });
            query.AddField(c => c.Color, sq => sq);

            Assert.AreEqual("color", (query.SelectList[0] as IQuery<Color>).Name);
        }

        [TestMethod]
        public void TestQuery()
        {
            var query = new Query<Car>(nameof(Car))
                .AddField(car => car.Name)
                .AddField(car => car.Price)
                .AddField(
                    car => car.Color,
                    sq => sq
                        .AddField(color => color.Red)
                        .AddField(color => color.Green)
                        .AddField(color => color.Blue));

            Assert.AreEqual(nameof(Car), query.Name);
            Assert.AreEqual(3, query.SelectList.Count);
            Assert.AreEqual(nameof(Car.Name), query.SelectList[0]);
            Assert.AreEqual(nameof(Car.Price), query.SelectList[1]);

            Assert.AreEqual(nameof(Car.Color), (query.SelectList[2] as IQuery<Color>).Name);
            var expectedSubSelectList = new List<string>
            {
                nameof(Color.Red),
                nameof(Color.Green),
                nameof(Color.Blue)
            };
            CollectionAssert.AreEqual(expectedSubSelectList, (query.SelectList[2] as IQuery<Color>).SelectList);
        }

        [TestMethod]
        public void TestQueryWithCustomName()
        {
            var query = new Query<Truck>("truck")
                .AddField(truck => truck.Name)
                .AddField(truck => truck.WeelsNumber)
                .AddField(
                    truck => truck.Load,
                    sq => sq
                        .AddField(load => load.Weight));

            Assert.AreEqual("truck", query.Name);
            Assert.AreEqual(3, query.SelectList.Count);
            Assert.AreEqual("name", query.SelectList[0]);
            Assert.AreEqual("weelsNumber", query.SelectList[1]);

            Assert.AreEqual("load", (query.SelectList[2] as IQuery<Load>).Name);
            var expectedSubSelectList = new List<string>
            {
                "weight"
            };
            CollectionAssert.AreEqual(expectedSubSelectList, (query.SelectList[2] as IQuery<Load>).SelectList);
        }

        [TestMethod]
        public void TestQueryBuild()
        {
            var query = new Query<Truck>("truck")
                .AddArguments(new { id = "yk8h4vn0", km = 2100 })
                .AddField(truck => truck.Name)
                .AddField(truck => truck.WeelsNumber)
                .AddField(
                    truck => truck.Load,
                    sq => sq
                        .AddField(load => load.Weight));

            string result = query.Build();

            Assert.AreEqual("truck(id:\"yk8h4vn0\",km:2100){name weelsNumber load{weight}}", result);
        }

        [TestMethod]
        public void TestSubSelectWithList()
        {
            var query = new Query<ObjectWithList>("object")
                .AddField<SubObject>(c => c.IEnumerable, sq => sq)
                .AddField<SubObject>(c => c.List, sq => sq)
                .AddField<SubObject>(c => c.IQueryable, sq => sq)
                .AddField<SubObject>(c => c.Array, sq => sq);

            Assert.AreEqual(typeof(Query<SubObject>), query.SelectList[0].GetType());
            Assert.AreEqual(typeof(Query<SubObject>), query.SelectList[1].GetType());
            Assert.AreEqual(typeof(Query<SubObject>), query.SelectList[2].GetType());
            Assert.AreEqual(typeof(Query<SubObject>), query.SelectList[3].GetType());
        }

        class ObjectWithList
        {
            public IEnumerable<SubObject> IEnumerable { get; set; }
            public List<SubObject> List { get; set; }
            public IQueryable<SubObject> IQueryable { get; set; }
            public SubObject[] Array { get; set; }
        }
        class SubObject
        {
            public byte Id { get; set; }
        }
    }
}
