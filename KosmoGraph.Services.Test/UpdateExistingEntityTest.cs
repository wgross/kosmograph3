namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using Moq;
    using KosmoGraph.Model;
    using System.Threading;

    [TestClass]
    public class UpdateExistingEntityTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());
        }

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void UpdateExistingEntityInDb()
        {
            // ARRANGE

            var entities = new []
            {
                Entity.Factory.CreateNew(e => e.Name = "e1")
            };
            
            var entityRepository = new Mock<IEntityRepository>();

            entityRepository // expect retrieval of existing entities
                .Setup(_ => _.GetAll())
                .Returns(entities.AsEnumerable());

            entityRepository // expects a new entity with name 'e1' and returns it
                .Setup(_ => _.Update(entities.First()))
                .Returns((Entity e) => e);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);

            Entity e1 = null;
            svc.GetAllEntities().EndWith(e => e1 = e.First());

            // ACT

            e1.Name = "e1-changed";

            Entity result = null;
            svc.UpdateEntity(e1).EndWith(e => result = e);

            // ASSERT
            
            Assert.AreSame(e1, result);
            Assert.AreEqual("e1-changed", result.Name);

            entityRepository.VerifyAll();
        }
    }
}
