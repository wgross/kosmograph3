namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using MongoDB.Driver;

    [TestClass]
    public class AssignedRelationshipFacetPropertyValueCrudTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("relationship");
        }

        private readonly string databaseName = "kosmograph_test";

        [TestMethod]
        public void AssignFacetToFinalRelationshipAndInsertInDb()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e2");
            var e2 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var r1 = Relationship.Factory.CreateNew(e1, e2);
            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var af1 = r1.Add(r1.CreateNewAssignedFacet(f1, null));

            var relationshipRepository = new RelationshipRepository(this.databaseName);
            
            // ACT

            r1.AssignedFacets.First().Properties.First().Value = "value";

            relationshipRepository.Insert(r1);
            
            Relationship result = relationshipRepository.FindByIdentity(r1.Identity);

            // ASSERT
            // relationship id and facet id assigned

            Assert.AreEqual("value", result.AssignedFacets.First().Properties.First().Value);
        }

        [TestMethod]
        public void AssignFacetToFinalRelationshipAndUpdateInDb()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e2");
            var e2 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var r1 = Relationship.Factory.CreateNew(e1, e2);
            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var af1 = r1.Add(r1.CreateNewAssignedFacet(f1, null));
            var relationshipRepository = new RelationshipRepository(this.databaseName);

            r1.AssignedFacets.First().Properties.First().Value = "value";

            relationshipRepository.Insert(r1);

            // ACT

            af1.Properties.First().Value = "changed value";

            relationshipRepository.Update(r1);

            Relationship result = relationshipRepository.FindByIdentity(r1.Identity);

            // ASSERT
            // relationship id and facet id assigned

            Assert.AreEqual("changed value", result.AssignedFacets.First().Properties.First().Value);
        }
    }
}
