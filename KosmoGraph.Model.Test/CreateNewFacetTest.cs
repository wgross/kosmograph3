namespace KosmoGraph.Model.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CreateNewFacetTest
    {
        [TestMethod]
        public void VerifyInitialization()
        {
            // ACT

            Facet t1 = FacetFactory.CreateNew();

            //ASSERT
            // create an entity must have an id

            Assert.IsNotNull(t1);
            Assert.AreNotEqual(Guid.Empty, t1.Id);
        }
    }
}