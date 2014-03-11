namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using System.Collections.Generic;
    using MongoDB.Driver;

    [TestClass]
    public class RemoveRelationshipTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            var db = new MongoClient().GetServer().GetDatabase(this.databaseName);
            db.DropCollection("relationship");
            db.DropCollection("entity");
        }

        private readonly string databaseName = "kosmograph_test";

        #region Remove relationship by From or To entity

        [TestMethod]
        public void RemoveSingleRelationshipWithFromEntityId()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e2");
            var r1 = RelationshipFactory.CreateNew(e1, e2);

            var relationshipRepository = new RelationshipRepository(this.databaseName);
            relationshipRepository.Insert(r1);

            // ACT

            relationshipRepository.RemoveByEntityIdentity(e1.Id);

            // ASSERT

            Assert.IsNull(relationshipRepository.FindByIdentity(r1.Identity));
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

            relationshipRepository.RemoveByEntityIdentity(e2.Id);

            // ASSERT

            Assert.IsNull(relationshipRepository.FindByIdentity(r1.Identity));
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

            relationshipRepository.RemoveByEntityIdentity(e2.Id);

            // ASSERT

            Assert.IsNull(relationshipRepository.FindByIdentity(r1.Identity));
            Assert.IsNull(relationshipRepository.FindByIdentity(r2.Identity));
        }

        [TestMethod]
        public void KeepRelationshipsWithOtherEntities()
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

            relationshipRepository.RemoveByEntityIdentity(e1.Id);

            // ASSERT

            Assert.IsNull(relationshipRepository.FindByIdentity(r1.Identity));
            Assert.IsNotNull(relationshipRepository.FindByIdentity(r2.Identity));
        }
        #endregion
    }
}
