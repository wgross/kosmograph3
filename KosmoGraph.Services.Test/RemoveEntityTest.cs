namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using KosmoGraph.Model;
    using KosmoGraph.Test;
    using System.Threading;

    [TestClass]
    public class RemoveEntityTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());
        }

        [TestMethod]
        [TestCategory("RemoveEntity")]
        public void RemoveEntityFromDb()
        {
            // ARRANGE

            var entities = new[]
            {
                Entity.Factory.CreateNew(e => e.Name = "e1")
            };

            var entityRepository = new Mock<IEntityRepository>();

            entityRepository // Expect call of remove
                .Setup(_ => _.Remove(entities.First()));
                //.Returns(true);

            var relationshipRepository = new Mock<IRelationshipRepository>();

            var ersvc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);

            // ACT

            bool result=false;
            ersvc.RemoveEntity(entities.First()).EndWith(r => result = r);

            // ASSERT

            Assert.IsTrue(result);
        }
    }
}
