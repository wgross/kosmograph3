namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using MongoDB.Driver;

    [TestClass]
    public class AssignedEntítyFacetCrudTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("entity");
        }

        private readonly string databaseName = "kosmograph_test";
        
        [TestMethod]
        public void AssignEmptyFacetToEntityAndInsertInDb()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");
            var af1 = e1.Add(e1.CreateNewAssignedFacet(f1));
            var entityRepository = new EntityRepository(this.databaseName);

            // ACT

            entityRepository.Insert(e1);
            Entity result = entityRepository.FindByIdentity(e1.Id);

            // ASSERT
            // facet is still there

            Assert.IsNotNull(result);
            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreEqual(f1.Id, e1.AssignedFacets.First().FacetId);
        }

        [TestMethod]
        public void AssignFacetWithPropertyToEntityAndInsertInDb()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var af1 = e1.Add(e1.CreateNewAssignedFacet(f1));
            var entityRepository = new EntityRepository(this.databaseName);

            // ACT

            entityRepository.Insert(e1);

            Entity result = entityRepository.FindByIdentity(e1.Id);

            // ASSERT
            // facet is still there

            Assert.IsNotNull(result);
            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreEqual(f1.Id, e1.AssignedFacets.First().FacetId);
            Assert.AreEqual(1, e1.AssignedFacets.First().Properties.Count());
            Assert.AreEqual(pd1.Id, e1.AssignedFacets.First().Properties.First().DefinitionId);
        }

        [TestMethod]
        public void RemoveFacetWithPropertyFromEntityAndUpdateInDb()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var af1 = e1.Add(e1.CreateNewAssignedFacet(f1));
            var entityRepository = new EntityRepository(this.databaseName);
            entityRepository.Insert(e1);
            
            // ACT

            e1.Remove(e1.AssignedFacets.First());
            
            entityRepository.Update(e1);

            Entity result = entityRepository.FindByIdentity(e1.Id);

            // ASSERT
            // facet is gone

            Assert.IsNotNull(result);
            Assert.AreEqual(0, e1.AssignedFacets.Count());
        }
    }
}
