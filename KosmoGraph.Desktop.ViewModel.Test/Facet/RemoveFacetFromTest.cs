namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Services;
    using System.Threading.Tasks;
    using KosmoGraph.Test;
    using System.Collections.Generic;
    using System.Threading;

    [TestClass]
    public class RemoveFacetViewModelTest
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
            this.facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            this.fsvc // expect deletion of the facet
                .Setup(_ => _.RemoveFacet(facets.First()))
                .Returns(Task.FromResult(true));

            this.entities = Enumerable.Empty<Entity>();

            this.relationships = Enumerable.Empty<Relationship>();
            
            this.ersvc = new Mock<IManageEntitiesAndRelationships>();

            this.ersvc // expect retrueval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));

            this.ersvc // expect retrieval of all relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        [TestMethod]
        [TestCategory("RemoveFacet")]
        public void RemoveUnusedEmptyFacetViewModelFromViewModel()
        {
            // ARRANGE

            // ACT

            this.vm.Remove(this.vm.Facets.Single());

            // ASSERT

            Assert.AreEqual(0, this.vm.Facets.Count());
            Assert.AreEqual(0, this.vm.Items.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }

    }
}
