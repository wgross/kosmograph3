namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MongoDB.Driver;
    using KosmoGraph.Model;

    [TestClass]
    public class GetAllFacetTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("facet");
        }

        private readonly string databaseName = "kosmograph_test";

        [TestMethod]
        public void EnumerateAllFacetsFromDb()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");
            var f2 = Facet.Factory.CreateNew(f => f.Name = "f2");
            var facetRepository = new FacetRepository(this.databaseName);

            facetRepository.Insert(f1);
            facetRepository.Insert(f2);

            // ACT

            Facet[] result = facetRepository.GetAll().ToArray();

            // ASSERT

            CollectionAssert.AreEqual(new[] { f1, f2 }.Select(f => f.Id).ToArray(), result.Select(f => f.Id).ToArray());
            CollectionAssert.AreEqual(new[] { f1, f2 }.Select(f => f.Name).ToArray(), result.Select(f => f.Name).ToArray());
        }

        [TestMethod]
        public void EnumerateAllFacetsFromEmptyDb()
        {
            // ARRANGE

            var facetRepository = new FacetRepository(this.databaseName);

            // ACT

            Facet[] result = facetRepository.GetAll().ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }
    }
}
