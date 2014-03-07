namespace KosmoGraph.Services.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Test;

    [TestClass]
    public class CreateNewRelationshipTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void CreateNewPartialRelationship()
        {
            // ARRANGE
    
            var entityRepository = new Mock<IEntityRepository>();

            entityRepository // inserts a new entity
                .Setup(_ => _.Insert(It.IsAny<Entity>())).Returns((Entity e) => e);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);
            var e1 = svc.CreateNewEntity(e=>e.Name="e1").Result;

            // ACT

            Relationship result = svc.CreatePartialRelationship(e1, delegate { });

            // ASSERT
            // a new temporary relationship was created having its origin at e1

            Assert.AreSame(e1, result.From);
            Assert.AreEqual(e1.Id, result.FromId);
            Assert.IsNull(result.To);
            //value type//Assert.IsNull(r1.ToId);

            entityRepository.VerifyAll();
            relationshipRepository.VerifyAll();
        }

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void InsertNewCompleteRelationship()
        {
            // ARRANGE
            var entityRepository = new Mock<IEntityRepository>();

            entityRepository // inserts two new entity
                .Setup(_ => _.Insert(It.IsAny<Entity>())).Returns((Entity e) => e);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            
            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);
            var e1 = svc.CreateNewEntity(e => e.Name = "e1").Result;
            var e2 = svc.CreateNewEntity(e => e.Name = "e2").Result;
            var r1 = svc.CreatePartialRelationship(e1, delegate { });
            
            relationshipRepository // Insert of new relationship is expected
                .Setup(_ => _.Insert(It.Is<Relationship>(r => r.From == e1 && r.To==e2)));

            entityRepository // retrieve e1 
                .Setup(_ => _.FindByIdentity(e1.Id))
                .Returns(e1);

            entityRepository // retrieve e2
                .Setup(_ => _.FindByIdentity(e2.Id))
                .Returns(e2);

            // ACT

            Relationship result = null;
            svc
                .CompletePartialRelationship(r1, e2)
                .EndWith(res =>
                    {
                        result = res.Relationship;
                        Assert.AreSame(r1, res.Relationship);
                        Assert.AreSame(e1, res.From);
                        Assert.AreSame(e2, res.To);
                    },
                    failed: f => { Assert.Fail(f.Message); return true; },
                    cancelled: () => Assert.Fail("Cancelled"));

            // ASSERT
            // the complete relationship is returned

            Assert.AreSame(r1, result);
            Assert.AreEqual(e1.Id, r1.FromId);
            Assert.AreEqual(e2.Id, result.ToId);

            entityRepository.VerifyAll();
            entityRepository.Verify(_ => _.Insert(e1));
            entityRepository.Verify(_ => _.Insert(e2));
            relationshipRepository.VerifyAll();
            relationshipRepository.Verify(_ => _.Insert(r1));
        }
    }
}
