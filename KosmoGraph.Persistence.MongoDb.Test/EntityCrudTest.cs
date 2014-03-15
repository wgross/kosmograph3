namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using MongoDB.Driver;
    using KosmoGraph.Test;

    [TestClass]
    public class EntityCrudTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("entity");
        }

        private readonly string databaseName = "kosmograph_test";

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void CreateNewEntityAndSaveToDb()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var entityRepository = new EntityRepository(this.databaseName);

            // ACT

            entityRepository.Insert(e1);

            // ASSERT

            Assert.IsNotNull(entityRepository.FindByIdentity(e1.Id));
        }

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void RetrieveSavedEntityFromDb()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var entityRepository = new EntityRepository(this.databaseName);

            entityRepository.Insert(e1);
            
            // ACT

            Entity result = entityRepository.FindByIdentity(e1.Id);
            
            // ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual(e1.Id, result.Id);
            Assert.AreEqual("e1", result.Name);
        }

        [TestMethod]
        [TestCategory("EditEntity")]
        public void UpdateEntityNameInDb()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var entityRepository = new EntityRepository(this.databaseName);

            entityRepository.Insert(e1);
            e1.Name = "e1-changed";

            // ACT

            entityRepository.Update(e1);
            
            Entity result = entityRepository.FindByIdentity(e1.Id);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(e1.Id, result.Id);
            Assert.AreEqual("e1-changed", result.Name);
        }

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void RemoveEntityFromDb()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var entityRepository = new EntityRepository(this.databaseName);

            entityRepository.Insert(e1);
            
            // ACT

            entityRepository.Remove(e1);

            // ASSERT
            Assert.IsNull(entityRepository.FindByIdentity(e1.Id));
        }

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void DontInsertSameEntityTwice()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var entityRepository = new EntityRepository(this.databaseName);

            entityRepository.Insert(e1);
            
            // ACT

            ExceptionAssert.Throws<InvalidOperationException>(delegate { entityRepository.Insert(e1); });

            Entity result = entityRepository.FindByIdentity(e1.Id);

            // ASSERT

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void DontInsertEntityWithDuplicateName()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var entityRepository = new EntityRepository(this.databaseName);
            entityRepository.Insert(e1);

            // ACT & ASSERT

            ExceptionAssert.Throws<InvalidOperationException>(delegate { entityRepository.Insert(Entity.Factory.CreateNew(e=> e.Name ="e1")); });
        }
    }
}
