namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using KosmoGraph.Services;
    using System.Collections.Generic;
    using Moq;
    using KosmoGraph.Test;
    using System.Threading;
    using System.Threading.Tasks;

    [TestClass]
    public class InitializeNewEntityWithVisibleFacetsTest
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

            // create facet
            this.facets = new[]
            {
                Facet.Factory.CreateNew(f => f.Name = "f1")
            };

            this.fsvc = new Mock<IManageFacets>();

            this.fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(this.facets));

            this.entities = Enumerable.Empty<Entity>();

            this.relationships = Enumerable.Empty<Relationship>();  

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();

            this.ersvc // retrieve empty set of entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));

            this.ersvc // retrieve empty set of entities
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));


            this.vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
        }

        #region CreateNewEntity

        [TestMethod]
        public void CreateNewEntityInitializedWithVisibleFacet()
        {
            // ARRANGE

            this.vm.Facets.Single().IsVisible = true;

            // ACT

            EditNewEntityViewModel result = this.vm.CreateNewEntity();

            // ASSERT

            Assert.AreEqual(1, result.AssignedFacets.Count());
            Assert.AreEqual(0, result.UnassignedFacets.Count());
            Assert.IsTrue(result.Commit.CanExecute());
            Assert.IsTrue(result.Rollback.CanExecute());
            Assert.IsTrue(result.PrepareCommit.CanExecute());

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 
    }
}
