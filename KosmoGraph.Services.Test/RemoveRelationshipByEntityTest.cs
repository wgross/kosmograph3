using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KosmoGraph.Model;
using Moq;

namespace KosmoGraph.Services.Test
{
    [TestClass]
    public class RemoveRelationshipByEntityTest
    {
        [TestMethod]
        public void RemoveEntitywithItsRelationship()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e1");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e2");
            
            var entityRepository = new Mock<IEntityRepository>();
            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);

            entityRepository // expects deletion of e1
                .Setup(_ => _.Remove(e1));

            relationshipRepository // returns a relationship connected with this entity
                .Setup(_ => _.RemoveByEntityIdentity(e1.Id));

            // ACT

            svc.RemoveEntity(e1);

            // ASSERT

            entityRepository.VerifyAll();
            relationshipRepository.VerifyAll();
        }
    }
}
