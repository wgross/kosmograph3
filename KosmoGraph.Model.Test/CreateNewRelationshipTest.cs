namespace KosmoGraph.Model.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
using KosmoGraph.Test;

    [TestClass]
    public class CreateNewRelationshipTest
    {
        [TestMethod]
        public void InitializationFailsOnNullEntity()
        {
            ExceptionAssert.Throws<ArgumentNullException>( delegate { Relationship.Factory.CreateNewPartial(null);});
            ExceptionAssert.Throws<ArgumentNullException>(delegate { Relationship.Factory.CreateNew(null,null); });
        }

        [TestMethod]
        public void CreateNewTemporaryRelationshipWithoutDestination()
        {
            // ACT

            Entity e1 = Entity.Factory.CreateNew(delegate {});
            Relationship r1 = Relationship.Factory.CreateNewPartial(e1);

            //ASSERT
            // create a relatoinship set an id and a from entity

            Assert.IsNotNull(r1);
            Assert.AreNotEqual(Guid.Empty, r1.Identity);
            Assert.AreEqual(e1.Id, r1.FromId);
            Assert.AreSame(e1, r1.From);

        }

        [TestMethod]
        public void CompleteTemporaryRelationshipWithDestination()
        {
            // ACT

            Entity e1 = Entity.Factory.CreateNew(delegate {});
            Entity e2 = Entity.Factory.CreateNew(delegate {});
            Relationship r1 = Relationship.Factory.CreateNewPartial(e1);

            // ACT

            r1.SetDestination(e2);

            // ASSERT
            // detiniation is set now

            Assert.IsNotNull(r1);
            Assert.AreNotEqual(Guid.Empty, r1.Identity);
            Assert.AreEqual(e1.Id, r1.FromId);
            Assert.AreSame(e1, r1.From);
            Assert.AreEqual(e2.Id, r1.ToId);
            Assert.AreSame(e2, r1.To);
        }

        [TestMethod]
        public void VerifyInitializationWithTowEntities()
        {
            // ACT

            Entity e1 = Entity.Factory.CreateNew(delegate {});
            Entity e2 = Entity.Factory.CreateNew(delegate {});
            Relationship r1 = Relationship.Factory.CreateNew(e1,e2);

            //ASSERT
            // create a relatoinship set an id and a from entity

            Assert.IsNotNull(r1);
            Assert.AreNotEqual(Guid.Empty, r1.Identity);
            Assert.AreEqual(e1.Id, r1.FromId);
            Assert.AreSame(e1, r1.From);
            Assert.AreEqual(e2.Id, r1.ToId);
            Assert.AreSame(e2, r1.To);
        }
    }
}