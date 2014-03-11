namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using MongoDB.Driver;

    [TestClass]
    public class AssignedEntityFacetPropertyValueCrudTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("entity");
        }

        private readonly string databaseName = "kosmograph_test";
        
        [TestMethod]
        public void AssignEntityFacetPropertyValueAndInsertInDb()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var af1 = e1.Add(e1.CreateNewAssignedFacet(f1));
            var entityRepository = new EntityRepository(this.databaseName);

            // ACT

            e1.AssignedFacets.First().Properties.First().Value = "value";
            
            entityRepository.Insert(e1);

            Entity result = entityRepository.FindByIdentity(e1.Id);

            // ASSERT
            // facet is still there

            Assert.AreEqual("value", result.AssignedFacets.First().Properties.First().Value);
        }

        [TestMethod]
        public void AssignEntityFacetPropertyValueAndUpdateInDb()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var af1 = e1.Add(e1.CreateNewAssignedFacet(f1));
            var entityRepository = new EntityRepository(this.databaseName);
            
            e1.AssignedFacets.First().Properties.First().Value = "value";
            entityRepository.Insert(e1);
            
            // ACT

            e1.AssignedFacets.First().Properties.First().Value = "changed value";

            entityRepository.Update(e1);

            Entity result = entityRepository.FindByIdentity(e1.Id);

            // ASSERT
            // facet is still there

            Assert.AreEqual("changed value", result.AssignedFacets.First().Properties.First().Value);
        }
    }
}
