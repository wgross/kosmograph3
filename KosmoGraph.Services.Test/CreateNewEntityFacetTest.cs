namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Test;

    [TestClass]
    public class CreateNewEntityFacetTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet")]
        public void InitializeUnassignedFacetsWithFacetsFromDb()
        {
            // ARRANGE

            var entityRepository = new Mock<IEntityRepository>();

            entityRepository // expects a new entity with name 'e1' and returns it
                .Setup(_ => _.Insert(It.Is<Entity>(e => e.Name == "e1" && e.AssignedFacets.Count() == 0)))
                .Returns((Entity e) => e);

            entityRepository // expects update is called for changed entity
                .Setup(_ => _.Update(It.Is<Entity>(e => e.Name == "e1" && e.AssignedFacets.Count() == 1)))
                .Returns((Entity e) => e);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);

            var e1 = svc.CreateNewEntity(e => e.Name = "e1").Result;
            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");

            // ACT

            AssignedFacet resultAssignedFacet = null;
            Entity resultEntity = null;

            svc.AddFacetToEntity(e1, f1, af => { resultAssignedFacet = af; }, e => { resultEntity = e; });

            // ASSERT

            Assert.AreSame(e1, resultEntity);
            Assert.AreSame(e1.AssignedFacets.First(), resultAssignedFacet);
            Assert.AreEqual(f1.Id, resultAssignedFacet.FacetId);
            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreSame(e1.AssignedFacets.First(), resultAssignedFacet);

            entityRepository.VerifyAll();
            relationshipRepository.VerifyAll();
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet")]
        public void AssignFacetToEntityAndUpdateEntityInDb()
        {
            // ARRANGE

            var entityRepository = new Mock<IEntityRepository>();

            entityRepository // expects a new entity with name 'e1' and returns it
                .Setup(_ => _.Insert(It.Is<Entity>(e => e.Name == "e1" && e.AssignedFacets.Count() == 0)))
                .Returns((Entity e) => e);

            entityRepository // expects update is called for changed entity
                .Setup(_ => _.Update(It.Is<Entity>(e => e.Name == "e1" && e.AssignedFacets.Count() == 1)))
                .Returns((Entity e) => e);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new EntityRelationshipService(entityRepository.Object, relationshipRepository.Object);

            var e1 = svc.CreateNewEntity(e => e.Name = "e1").Result;
            var f1 = FacetFactory.CreateNew(f => f.Name = "f1");

            // ACT

            AssignedFacet resultAssignedFacet = null;
            Entity resultEntity = null;

            svc.AddFacetToEntity(e1, f1, af => { resultAssignedFacet = af; }, e => { resultEntity = e; });

            // ASSERT

            Assert.AreSame(e1, resultEntity);
            Assert.AreSame(e1.AssignedFacets.First(), resultAssignedFacet);
            Assert.AreEqual(f1.Id, resultAssignedFacet.FacetId);
            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreSame(e1.AssignedFacets.First(), resultAssignedFacet);

            entityRepository.VerifyAll();
            relationshipRepository.VerifyAll();
        }
    }
}
