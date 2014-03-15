namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using MongoDB.Driver;
    using KosmoGraph.Test;

    [TestClass]
    public class FacetCrudTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("facet");
        }

        private readonly string databaseName = "kosmograph_test";

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void CreateNewFacetFromNameAndInsertInToMongoDb()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");
            var facetRepository = new FacetRepository(this.databaseName);

            // ACT

            facetRepository.Insert(f1);

            // ASSERT

            Assert.IsNotNull(facetRepository.FindByIdentity(f1.Id));
        }

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void CreateNewFacetFromNameAndPropertyAndInsertInToMongoDb()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(f => 
            {
                f.Name = "f1";
                f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            });

            var facetRepository = new FacetRepository(this.databaseName);

            // ACT

            facetRepository.Insert(f1);

            // ASSERT

            Assert.IsNotNull(facetRepository.FindByIdentity(f1.Id));
        }

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void RetrieveSavedFacetWithNameFromMongoDb()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(e => e.Name = "f1");
            var facetRepository = new FacetRepository(this.databaseName);

            facetRepository.Insert(f1);

            // ACT

            Facet result = facetRepository.FindByIdentity(f1.Id);

            // ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual(f1.Id, result.Id);
            Assert.AreEqual("f1", result.Name);
            Assert.AreEqual(0, result.Properties.Count());
        }

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void RetrieveSavedFacetWithNameAndPropertyFromMongoDb()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(f =>
            {
                f.Name = "f1";
                f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            });

            var facetRepository = new FacetRepository(this.databaseName);

            facetRepository.Insert(f1);

            // ACT

            Facet result = facetRepository.FindByIdentity(f1.Id);

            // ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual(f1.Id, result.Id);
            Assert.AreEqual("f1", result.Name);
            Assert.AreEqual(1, result.Properties.Count());
            Assert.AreEqual("pd1", result.Properties.First().Name);
        }

        [TestMethod]
        [TestCategory("EditFacet")]
        public void UpdateFacetNameInMongoDb()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(e => e.Name = "f1");
            var facetRepository = new FacetRepository(this.databaseName);

            facetRepository.Insert(f1);
            f1.Name = "f1-changed";

            // ACT

            facetRepository.Update(f1);

            Facet result = facetRepository.FindByIdentity(f1.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(f1.Id, result.Id);
            Assert.AreEqual("f1-changed", result.Name);
        }

        [TestMethod]
        [TestCategory("EditFacet")]
        public void UpdateFacetPropertyNameInMongoDb()
        {
            // ARRANGE

             var f1 = Facet.Factory.CreateNew(f =>
            {
                f.Name = "f1";
                f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            });

            var facetRepository = new FacetRepository(this.databaseName);

            facetRepository.Insert(f1);
            f1.Name = "f1-changed";
            f1.Properties.First().Name = "pd1-changed";

            // ACT

            facetRepository.Update(f1);

            Facet result = facetRepository.FindByIdentity(f1.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(f1.Id, result.Id);
            Assert.AreEqual("f1-changed", result.Name);
            Assert.AreEqual("pd1-changed", result.Properties.First().Name);
        }

        [TestMethod]
        [TestCategory("EditFacet"),TestCategory("RemovePropertyDefinition")]
        public void UpdateFacetRemovePropertyInMongoDb()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(f =>
            {
                f.Name = "f1";
                f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            });

            var facetRepository = new FacetRepository(this.databaseName);

            facetRepository.Insert(f1);
            f1.Name = "f1-changed";
            f1.Remove(f1.Properties.First());

            // ACT

            facetRepository.Update(f1);

            Facet result = facetRepository.FindByIdentity(f1.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(f1.Id, result.Id);
            Assert.AreEqual("f1-changed", result.Name);
            Assert.AreEqual(0, result.Properties.Count());
        }

        [TestMethod]
        [TestCategory("RemoveFacet")]
        public void RemoveFacetFromMongoDb()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(e => e.Name = "f1");
            var facetRepository = new FacetRepository(this.databaseName);

            facetRepository.Insert(f1);

            // ACT

            facetRepository.Remove(f1);

            // ASSERT
            Assert.IsNull(facetRepository.FindByIdentity(f1.Id));
        }

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void DontInsertSameFacetTwice()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(e => e.Name = "f1");
            var facetRepository = new FacetRepository(this.databaseName);

            facetRepository.Insert(f1);

            // ACT

            ExceptionAssert.Throws<InvalidOperationException>(delegate { facetRepository.Insert(f1); });

            Facet result = facetRepository.FindByIdentity(f1.Id);

            // ASSERT

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory("CreateNewFacet"), TestCategory("EditFacet")]
        public void DontInsertFacetWithDuplicateNameInMongoDb()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");
            var facetRepository = new FacetRepository(this.databaseName);
            facetRepository.Insert(f1);

            // ACT & ASSERT
            // throw excetin if a Facet with existing name is inserted in MongoDb
            
            ExceptionAssert.Throws<InvalidOperationException>(delegate { facetRepository.Insert(Facet.Factory.CreateNew(f => f.Name = "f1")); });

            Assert.IsNotNull(facetRepository.FindByIdentity(f1.Id));
        }
    }
}
