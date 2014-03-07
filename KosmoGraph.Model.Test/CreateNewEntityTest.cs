namespace KosmoGraph.Model.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CreateNewEntityTest
    {
        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void VerifyInitialization()
        {
            // ACT

            Entity e1 = EntityFactory.CreateNew();

            // ASSERT
            // create an entity must have an id

            Assert.IsNotNull(e1);
            Assert.AreNotEqual(Guid.Empty, e1.Id);
        }

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void VerifyCustomInitialization()
        {
            // ACT

            Entity e1 = EntityFactory.CreateNew(e => e.Name = "e1");

            // ASSERT
            // initializer was applied

            Assert.AreEqual("e1", e1.Name);
        }
    }
}