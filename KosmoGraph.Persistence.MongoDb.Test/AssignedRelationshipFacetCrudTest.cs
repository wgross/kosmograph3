namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using MongoDB.Driver;

    [TestClass]
    public class AssignedRelationshipFacetCrudTest
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
            var af1 = r1.Add(r1.CreateNewAssignedFacet(f1, null));

            var relationshipRepository = new RelationshipRepository(this.databaseName);
            
            // ACT

            relationshipRepository.Insert(r1);
            
            Relationship result = relationshipRepository.FindByIdentity(r1.Identity);

            // ASSERT
            // relationship id and facet id assigned

            Assert.AreEqual(1, result.AssignedFacets.Count());
            Assert.AreEqual(f1.Id, result.AssignedFacets.First().FacetId);
        }

        [TestMethod]
        public void RemoveFacetFromFinalRelationshipAndUpdateInDb()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e2");
            var e2 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var r1 = Relationship.Factory.CreateNew(e1, e2);
            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");
            var af1 = r1.Add(r1.CreateNewAssignedFacet(f1, null));
            var relationshipRepository = new RelationshipRepository(this.databaseName);

            relationshipRepository.Insert(r1);

            // ACT

            r1.Remove(r1.AssignedFacets.First());

            relationshipRepository.Update(r1);

            Relationship result = relationshipRepository.FindByIdentity(r1.Identity);

            // ASSERT
            // facet is gone

            Assert.AreEqual(0, result.AssignedFacets.Count());
        }
    }
}
