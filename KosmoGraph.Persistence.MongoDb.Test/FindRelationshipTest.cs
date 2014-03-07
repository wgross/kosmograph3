namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using System.Collections.Generic;
    using MongoDB.Driver;

    [TestClass]
    public class FindRelationshipTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            var db = new MongoClient().GetServer().GetDatabase(this.databaseName);
            db.DropCollection("relationship");
            db.DropCollection("entity");
        }

        private readonly string databaseName = "kosmograph";

        #region Find relatinship by From or To entity

        [TestMethod]
        public void FindSingleRelationshipWithFromEntityId()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e2");
            var r1 = RelationshipFactory.CreateNew(e1, e2);

            var relationshipRepository = new RelationshipRepository(this.databaseName);
            relationshipRepository.Insert(r1);

            // ACT

            IEnumerable<Relationship> result = relationshipRepository.FindByEntityIdentity(e1.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(r1.Identity, result.Single().Identity);
        }

        [TestMethod]
        public void FindSingleRelationshipWithToEntityId()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e2");
            var r1 = RelationshipFactory.CreateNew(e1, e2);

            var relationshipRepository = new RelationshipRepository(this.databaseName);
            relationshipRepository.Insert(r1);

            // ACT

            IEnumerable<Relationship> result = relationshipRepository.FindByEntityIdentity(e2.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(r1.Identity, result.Single().Identity);
        }

        [TestMethod]
        public void FindMultipleRelationshipWithToEntityId()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e2");
            var e3 = EntityFactory.CreateNew(e => e.Name = "e3");
            var r1 = RelationshipFactory.CreateNew(e1, e2);
            var r2 = RelationshipFactory.CreateNew(e2, e3);

            var relationshipRepository = new RelationshipRepository(this.databaseName);
            relationshipRepository.Insert(r1);
            relationshipRepository.Insert(r2);

            // ACT

            IEnumerable<Relationship> result = relationshipRepository.FindByEntityIdentity(e2.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        #endregion 
    }
}
