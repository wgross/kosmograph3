namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using MongoDB.Driver;

    [TestClass]
    public class GetAllEntityTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            new MongoClient().GetServer().GetDatabase(this.databaseName).DropCollection("entity");
        }

        private readonly string databaseName = "kosmograph_test";

        [TestMethod]
        [TestCategory("EditEntity"),TestCategory("CreateNewRelationship")]
        public void EnumerateAllEntitysFromDb()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var e2 = Entity.Factory.CreateNew(e => e.Name = "e2");
            var EntityRepository = new EntityRepository(this.databaseName);

            EntityRepository.Insert(e1);
            EntityRepository.Insert(e2);

            // ACT

            Entity[] result = EntityRepository.GetAll().ToArray();

            // ASSERT

            CollectionAssert.AreEqual(new[] { e1, e2 }.Select(f => f.Id).ToArray(), result.Select(f => f.Id).ToArray());
            CollectionAssert.AreEqual(new[] { e1, e2 }.Select(f => f.Name).ToArray(), result.Select(f => f.Name).ToArray());
        }

        [TestMethod]
        [TestCategory("EditEntity"), TestCategory("CreateNewRelationship")]
        public void EnumerateAllEntitysFromEmptyDb()
        {
            // ARRANGE

            var EntityRepository = new EntityRepository(this.databaseName);

            // ACT

            Entity[] result = EntityRepository.GetAll().ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }
    }
}