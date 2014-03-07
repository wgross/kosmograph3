namespace KosmoGraph.Services.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using Moq;

    [TestClass]
    public class GetEntitiesByRelationshipTest
    {
        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void FindEntitiesConnectedByRelationship()
        {
            // ARRANGE
            var e1 = EntityFactory.CreateNew(e => e.Name="e1");
            var e2 = EntityFactory.CreateNew(e => e.Name="e2");
            var r1 = RelationshipFactory.CreateNew(r => 
            { 
                r.FromId = e1.Id;
                r.ToId = e2.Id;
            });

            var entityRepository = new Mock<IEntityRepository>();

            entityRepository // retrieve e1 fro DB
                .Setup(_ => _.FindByIdentity(e1.Id))
                .Returns(e1);

            entityRepository // retrieve e2 from DB
                .Setup(_ => _.FindByIdentity(e2.Id))
                .Returns(e2);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);

            // ACT

            Entity r1from =null;
            Entity r1to = null;

            svc.GetEntitiesByRelationship(r1).EndWith(result => { r1from = result.From; r1to = result.To; });

            // ASSERT

            Assert.AreSame(e1, r1from);
            Assert.AreSame(e2, r1to);

            entityRepository.VerifyAll();
            relationshipRepository.VerifyAll();
        }
    }
}
