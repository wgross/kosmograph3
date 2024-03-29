﻿namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using Moq;
    using KosmoGraph.Model;
    using System.Threading;

    [TestClass]
    public class GetAllEntitiesTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());
        }

        [TestMethod]
        public void GetAllEntitiesFromDb()
        {
            var entities = new[]
            { 
                Entity.Factory.CreateNew(e=>e.Name="e1"), 
                Entity.Factory.CreateNew(e=>e.Name="e2")
            };

            var entityRepository = new Mock<IEntityRepository>();

            entityRepository // returns a facet collection
                .Setup(_ => _.GetAll())
                .Returns(entities);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);

            // ACT

            Entity[] result = null;

            svc.GetAllEntities().EndWith(f => result = f.ToArray());

            // ASSERT

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(entities.Select(f => f.Id).ToArray(), result.Select(f => f.Id).ToArray());
        }
    }
}
