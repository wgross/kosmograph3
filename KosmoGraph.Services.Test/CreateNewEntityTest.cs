namespace KosmoGraph.Services.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Test;

    [TestClass]
    public class CreateNewEntityTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void CreateNewEntityFromNameAndInsertInDb()
        {
            // ARRANGE

            var entityRepository = new Mock<IEntityRepository>();

            entityRepository // expects a new entity with name 'e1' and returns it
                .Setup(_ => _.Insert(It.Is<Entity>(e => e.Name=="e1")))
                .Returns((Entity e) => e);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);
            
            // ACT

            Entity e1 = null;
            svc.CreateNewEntity(e => e.Name = "e1").EndWith(e => e1 = e);

            // ASSERT

            entityRepository.VerifyAll();            
        }

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void ThrowOnEmptyEntityName()
        {
            // ARRANGE

            var entityRepository = new Mock<IEntityRepository>();
            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);

            // ACT

            ExceptionAssert.Throws<ArgumentNullException>(delegate { Entity e1 = svc.CreateNewEntity(e => e.Name = string.Empty).Result; });

            // ASSERT

            entityRepository.VerifyAll();           
        }
    }
}
