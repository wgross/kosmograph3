
namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Desktop.ViewModel;
    using KosmoGraph.Test;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;


    [TestClass]
    public class RemoveEntityFromViewModelTest
    {
        private IEnumerable<Facet> facets;
        private Mock<IManageFacets> fsvc;
        private IEnumerable<Entity> entities;
        private IEnumerable<Relationship> relationships;
        private Mock<IManageEntitiesAndRelationships> ersvc;
        private EntityRelationshipViewModel vm;

        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());

            // provide a facet with a property definition
            this.facets = Enumerable.Empty<Facet>();

            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            this.entities = new []
            {
                Entity.Factory.CreateNew(e=>e.Name ="e1")
            };

            this.relationships = Enumerable.Empty<Relationship>();

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();
            this.ersvc // expect retrueval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));
            this.ersvc
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        [TestMethod]
        [TestCategory("RemoveEntity")]
        public void RemoveUnusedEntityFromViewModel()
        {
            // ARRANGE

            this.ersvc // expect deletion of entity
                .Setup(_ => _.RemoveEntity(this.entities.Single()))
                .Returns(Task.FromResult(true));

            // ACT

            this.vm.Remove(this.vm.Entities.Single());

            // ASSERT

            Assert.AreEqual(0, this.vm.Entities.Count());
            Assert.AreEqual(0, this.vm.Items.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.RemoveEntity(It.IsAny<Entity>()), Times.Once);
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }
    }
}
