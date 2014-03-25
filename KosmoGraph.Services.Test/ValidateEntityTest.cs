namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using System.Threading;
    using KosmoGraph.Model;
    using Moq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    [TestClass]
    public class ValidateEntityTest
    {
        private IEnumerable<Entity> entities;
        private Mock<IEntityRepository> entityRepository;
        private Mock<IRelationshipRepository> relationshipRepository;
        private EntityRelationshipService ersvc;

        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());

            this.entityRepository = new Mock<IEntityRepository>();

            this.relationshipRepository = new Mock<IRelationshipRepository>();

            this.ersvc = new EntityRelationshipService(this.entityRepository.Object, this.relationshipRepository.Object);
            
            this.entities = new[]
            {
                Entity.Factory.CreateNew(e => e.Name = "e1")
            };
        }

        [TestMethod]
        [TestCategory("ValidateEntity")]
        public void VerifyEntityFailsWithEmptyName()
        {
            // ARRANGE
            
            this.entityRepository
                .Setup(_ => _.ExistsName(string.Empty))
                .Returns(false);
            
            this.entities.Single().Name = string.Empty;

            // ACT

            ValidateEntityResult result = null;

            this.ersvc.ValidateEntity(this.entities.Single().Name).EndWith(r => result = r);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.IsFalse(result.NameIsNotUnique);
            Assert.IsTrue(result.NameIsNullOrEmpty);

            this.entityRepository.VerifyAll();
            this.entityRepository.Verify(_ => _.ExistsName(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("ValidateEntity")]
        public void VerifyEntityNameSucceedsIfNameIsUnknown()
        {
            // ARRANGE

            this.entityRepository
                .Setup(_ => _.ExistsName("e1"))
                .Returns(false);

            // ACT

            ValidateEntityResult result = null;

            this.ersvc.ValidateEntity(this.entities.Single().Name).EndWith(r => result = r);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.IsFalse(result.NameIsNotUnique);
            Assert.IsFalse(result.NameIsNullOrEmpty);

            this.entityRepository.VerifyAll();
            this.entityRepository.Verify(_ => _.ExistsName(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("ValidateEntity")]
        public void VerifyEntityNameFailsIfNameIsKnown()
        {
            // ARRANGE

            this.entityRepository
                .Setup(_ => _.ExistsName("e1"))
                .Returns(true);

            // ACT

            ValidateEntityResult result = null;

            this.ersvc.ValidateEntity(this.entities.Single().Name).EndWith(r => result = r);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.IsTrue(result.NameIsNotUnique);
            Assert.IsFalse(result.NameIsNullOrEmpty);

            this.entityRepository.VerifyAll();
            this.entityRepository.Verify(_ => _.ExistsName(It.IsAny<string>()), Times.Once);
        }
    }
}
