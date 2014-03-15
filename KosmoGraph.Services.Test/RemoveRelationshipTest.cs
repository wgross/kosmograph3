namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using KosmoGraph.Model;
    using KosmoGraph.Test;
    using System.Threading;
    using System.Collections.Generic;

    [TestClass]
    public class RemoveRelationshipTest
    {
        private IEnumerable<Entity> entities;
        private IEnumerable<Relationship> relationships;
        private Mock<IRelationshipRepository> relationshipRepository;
        private Mock<IEntityRepository> entityRepository;

        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());

            this.entities = new[]
            {
                EntityFactory.CreateNew(e => e.Name = "e1"),
                EntityFactory.CreateNew(e => e.Name ="e2")
            };

            this.entityRepository = new Mock<IEntityRepository>();
 
            this.relationships = new []
            {
                RelationshipFactory.CreateNew(r =>
                {
                    r.FromId = this.entities.ElementAt(0).Id;
                    r.ToId = this.entities.ElementAt(1).Id;
                })
            };

            this.relationshipRepository = new Mock<IRelationshipRepository>();
        }

        [TestMethod]
        [TestCategory("RemoveRelationship")]
        public void RemoveRelationshipFromDb()
        {
            // ARRANGE

            this.relationshipRepository // Expect call of remove
                .Setup(_ => _.Remove(this.relationships.Single()));
                //.Returns(true);

            var ersvc = new EntityRelationshipService(this.entityRepository.Object, this.relationshipRepository.Object);

            // ACT

            bool result=false;
            ersvc.RemoveRelationship(this.relationships.Single()).EndWith(r => result = r);

            // ASSERT

            Assert.IsTrue(result);
            this.relationshipRepository.VerifyAll();
            this.relationshipRepository.Verify(_ => _.Remove(It.IsAny<Relationship>()), Times.Once);
        }
    }
}
