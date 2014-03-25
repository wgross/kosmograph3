
namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MongoDB.Driver;
    using KosmoGraph.Model;

    [TestClass]
    public class ValidateEntityTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("entity");

            this.entityRepository = new EntityRepository(this.databaseName);
        }

        private readonly string databaseName = "kosmograph_test";

        private IEntityRepository entityRepository;

        [TestMethod]
        [TestCategory("ValidateEntity")]
        public void EntityNameDoesntExistInEmptyDatabase()
        {
            // ARRANGE

            // ACT

            bool result = this.entityRepository.ExistsName("e1");

            // ASSERT

            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("ValidateEntity")]
        public void EntityNameDoesExistInEmptyDatabaseAfterInsert()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(f => f.Name = "e1");

            this.entityRepository.Insert(e1);

            // ACT

            bool result = this.entityRepository.ExistsName("e1");

            // ASSERT

            Assert.IsTrue(result);
        }
    }
}
