namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using MongoDB.Driver;
    using KosmoGraph.Test;

    [TestClass]
    public class PropertyDefinitionCrudTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("facet");
        }

        private readonly string databaseName = "kosmograph";

        [TestMethod]
        public void CreateNewFacetWithPropertyDefinitionAndStoreToDb()
        {
            // ARRANGE

            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");
            f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            
            var facetRepository = new FacetRepository(this.databaseName);
            
            // ACT

            facetRepository.Insert(f1);
            Facet result = facetRepository.FindByIdentity(f1.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Properties.Count());
            Assert.AreEqual(f1.Properties.First().Id, result.Properties.First().Id);
            Assert.AreEqual(f1.Properties.First().Name, result.Properties.First().Name);
        }

        [TestMethod]
        public void UpdateFacetsPropertyDefinitionInDb()
        {
            // ARRANGE

            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");
            f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));

            var facetRepository = new FacetRepository(this.databaseName);
            
            facetRepository.Insert(f1);
            
            // ACT

            f1.Properties.First().Name = "pd1-changed";
            facetRepository.Update(f1);
            Facet result = facetRepository.FindByIdentity(f1.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Properties.Count());
            Assert.AreEqual(f1.Properties.First().Id, result.Properties.First().Id);
            Assert.AreEqual(f1.Properties.First().Name, result.Properties.First().Name);
        }

        [TestMethod]
        public void UpdateFacetsPropertyDefinitionWithNewPropertyDefinition()
        {
            // ARRANGE

            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");
            f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));

            var facetRepository = new FacetRepository(this.databaseName);

            facetRepository.Insert(f1);
            
            // ACT
            var pd2 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd2"));
            
            facetRepository.Update(f1);
            
            Facet result = facetRepository.FindByIdentity(f1.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Properties.Count());
            Assert.AreEqual(f1.Properties.ElementAt(0).Id, result.Properties.ElementAt(0).Id);
            Assert.AreEqual(f1.Properties.ElementAt(0).Name, result.Properties.ElementAt(0).Name);
            Assert.AreEqual(f1.Properties.ElementAt(1).Id, result.Properties.ElementAt(1).Id);
            Assert.AreEqual(f1.Properties.ElementAt(1).Name, result.Properties.ElementAt(1).Name);
        }

        [TestMethod]
        public void RemovePropetyDefinitionFromFacet()
        {
            // ARRANGE

            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));

            var facetRepository = new FacetRepository(this.databaseName);

            facetRepository.Insert(f1);

            // ACT
            f1.Remove(pd1);

            facetRepository.Update(f1);

            Facet result = facetRepository.FindByIdentity(f1.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Properties.Count());
        }
    }
}
