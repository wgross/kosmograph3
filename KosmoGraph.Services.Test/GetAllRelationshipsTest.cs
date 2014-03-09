﻿namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using Moq;
    using System.Threading.Tasks;
    using KosmoGraph.Test;

    [TestClass]
    public class GetAllRelationshipsTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        [TestMethod]
        [TestCategory("UpdateExistingRelationship")]
        public void GetAllRelationshipsFromDb()
        {
            // ARRANGE

            var entityRepository = new Mock<IEntityRepository>();

            var entities = new[]
            { 
                EntityFactory.CreateNew(e=>e.Name="e1"), 
                EntityFactory.CreateNew(e=>e.Name="e2")
            };

            var relationships = new[]
            { 
                RelationshipFactory.CreateNew(r=>
                {
                    r.FromId = entities.ElementAt(0).Id; 
                    r.ToId = entities.ElementAt(1).Id;
                })
            };

            var relationshipRepository = new Mock<IRelationshipRepository>();

            relationshipRepository // expect retruevel of all relatinships
                .Setup(_ => _.GetAll())
                .Returns(relationships.AsEnumerable());

            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);

            // ACT

            Relationship[] result = null;
            svc.GetAllRelationships().EndWith(r => result = r.ToArray());

            // ASSERT

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(relationships.Select(r => r.Identity).ToArray(), result.Select(r => r.Identity).ToArray());


        }
    }
}