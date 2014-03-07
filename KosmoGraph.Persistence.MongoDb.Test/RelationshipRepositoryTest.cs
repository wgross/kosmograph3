
namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using MongoDB.Driver;

    [TestClass]
    public class RelationshipRepositoryTest
    {
        [TestMethod]
        public void CreateRelationshipRepositoryForNullDatabaseFails()
        {
            // ACT & ASSERT

            ExceptionAssert.Throws<ArgumentNullException>(delegate { new RelationshipRepository(null); });
        }
    }
}
