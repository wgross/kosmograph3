namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using MongoDB.Driver;
    using KosmoGraph.Test;

    [TestClass]
    public class RelationshipCrudTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("relationship");
        }

        private readonly string databaseName = "kosmograph_test";

        [TestMethod]
        public void CreateNewRelationshipAndSaveToDb()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e2");
            var r1 = RelationshipFactory.CreateNew(e1, e2);

            var relationshipRepository = new RelationshipRepository(this.databaseName);

            // ACT

            relationshipRepository.Insert(r1);

            // ASSERT

            Assert.IsNotNull(relationshipRepository.FindByIdentity(r1.Identity));
        }

        [TestMethod]
        public void RetrieveSavedRelationshipFromDb()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e2");
            var r1 = RelationshipFactory.CreateNew(e1, e2);

            var relationshipRepository = new RelationshipRepository(this.databaseName);

            relationshipRepository.Insert(r1);
            
            // ACT

            Relationship result = relationshipRepository.FindByIdentity(r1.Identity);
            
            // ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual(r1.Identity, result.Identity);
            Assert.AreEqual(e1.Id, result.FromId);
            Assert.AreEqual(e2.Id, result.ToId);
        }

        [TestMethod]
        public void UpdateRelationshipInDb()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e2");
            var r1 = RelationshipFactory.CreateNew(e1, e2);

            var relationshipRepository = new RelationshipRepository(this.databaseName);

            relationshipRepository.Insert(r1);
            
            // ACT

            relationshipRepository.Update(r1);
            
            Relationship result = relationshipRepository.FindByIdentity(r1.Identity);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(r1.Identity, result.Identity);
        }

        [TestMethod]
        public void RemoveRelationshipFromDb()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e2");
            var r1 = RelationshipFactory.CreateNew(e1, e2);

            var relationshipRepository = new RelationshipRepository(this.databaseName);

            relationshipRepository.Insert(r1);
            
            // ACT

            relationshipRepository.Remove(r1);

            // ASSERT
            Assert.IsNull(relationshipRepository.FindByIdentity(r1.Identity));
        }

        [TestMethod]
        public void DontInsertSameRelationshipTwice()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e2");
            var r1 = RelationshipFactory.CreateNew(e1, e2);

            var relationshipRepository = new RelationshipRepository(this.databaseName);

            relationshipRepository.Insert(r1);
            
            // ACT

            ExceptionAssert.Throws<WriteConcernException>(delegate { relationshipRepository.Insert(r1); });

            Relationship result = relationshipRepository.FindByIdentity(r1.Identity);

            // ASSERT

            Assert.IsNotNull(result);
        }
    }
}
