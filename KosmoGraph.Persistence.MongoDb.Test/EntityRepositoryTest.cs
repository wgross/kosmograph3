
namespace KosmoGraph.Persistence.MongoDb.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;

    [TestClass]
    public class EntityRepositoryTest
    {
        [TestMethod]
        public void CreateEntityRepositoryForNullDatabaseFails()
        {
            // ACT & ASSERT

            ExceptionAssert.Throws<ArgumentNullException>(delegate { new EntityRepository(null); });
        }
    }
}
