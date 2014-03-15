namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using MongoDB.Driver;

    [TestClass]
    public class GetAllRelationshipTest
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            var db = new MongoClient().GetServer().GetDatabase(this.databaseName);
            db.DropCollection("entity");
            db.DropCollection("relationship");
        }

        private readonly string databaseName = "kosmograph_test";

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void EnumerateAllRelationshipsFromDb()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var e2 = Entity.Factory.CreateNew(e => e.Name = "e2");
            var e3 = Entity.Factory.CreateNew(e => e.Name = "e3");
            var entityRepository = new EntityRepository(this.databaseName);
            
            entityRepository.Insert(e1);
            entityRepository.Insert(e2);
            
            var relatinshipRepository = new RelationshipRepository(this.databaseName);
            var r1 = Relationship.Factory.CreateNew(r => 
            {
                r.FromId= e1.Id;
                r.ToId = e2.Id;
            });

            var r2 = Relationship.Factory.CreateNew(r =>
            {
                r.FromId = e2.Id;
                r.ToId = e3.Id;
            });

            relatinshipRepository.Insert(r1);
            relatinshipRepository.Insert(r2);
            
            // ACT

            Relationship[] result = relatinshipRepository.GetAll().ToArray();

            // ASSERT

            CollectionAssert.AreEqual(new[] { r1, r2 }.Select(r => r.Identity).ToArray(), result.Select(r => r.Identity).ToArray());
        }

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void EnumerateAllEntitysFromEmptyDb()
        {
            // ARRANGE

            var relationshipRepository = new RelationshipRepository(this.databaseName);

            // ACT

            Relationship[] result = relationshipRepository.GetAll().ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }
    }
}
