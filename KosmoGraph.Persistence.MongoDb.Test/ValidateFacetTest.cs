
namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MongoDB.Driver;
    using KosmoGraph.Model;

    [TestClass]
    public class ValidateFacetTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("facet");

            this.facetRepository = new FacetRepository(this.databaseName);
        }

        private readonly string databaseName = "kosmograph_test";

        private IFacetRepository facetRepository;

        [TestMethod]
        [TestCategory("ValidateFacet")]
        public void FacetNameDoesntExistInEmptyDatabase()
        {
            // ARRANGE

            // ACT

            bool result = this.facetRepository.ExistsName("f1");

            // ASSERT

            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("ValidateFacet")]
        public void FacetNameDoesExistInEmptyDatabaseAfterInsert()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");

            this.facetRepository.Insert(f1);

            // ACT

            bool result = this.facetRepository.ExistsName("f1");

            // ASSERT

            Assert.IsTrue(result);
        }
    }
}
