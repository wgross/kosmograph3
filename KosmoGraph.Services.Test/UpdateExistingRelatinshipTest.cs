﻿namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using KosmoGraph.Model;
    using Moq;
    using System.Threading;

    [TestClass]
    public class UpdateExistingRelatinshipTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());
        }

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void UpdateExistingRelationshipInDb()
        {
            // ARRANGE

            var entities = new[]
            {
                Entity.Factory.CreateNew(e => e.Name = "e1"),
                Entity.Factory.CreateNew(e => e.Name = "e2")
            };

            var entityRepository = new Mock<IEntityRepository>();

            var r1 =Relationship.Factory.CreateNew(r => 
            {
                r.FromId = entities.ElementAt(0).Id;
                r.ToId = entities.ElementAt(1).Id;
            });

            var relationshipRepository = new Mock<IRelationshipRepository>();

            relationshipRepository // expects update of relationship r1
                .Setup(_ => _.Update(r1))
                .Returns<Relationship>(r => r);

            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);

            // ACT

            Relationship result =null;
            svc.UpdateRelationship(r1).EndWith(r => result = r);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreSame(r1, result);

            entityRepository.VerifyAll();
            relationshipRepository.VerifyAll();
        }
    }
}
