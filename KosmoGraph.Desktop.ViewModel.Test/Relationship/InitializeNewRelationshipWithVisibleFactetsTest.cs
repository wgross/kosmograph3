using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using KosmoGraph.Model;
using KosmoGraph.Services;
using Moq;
using KosmoGraph.Test;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace KosmoGraph.Desktop.ViewModel.Test
{
    [TestClass]
    public class InitializeNewRelationshipWithVisibleFacetsTest
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

            this.facets = new[]
            {
                Facet.Factory.CreateNew(f => f.Name = "f1")
            };


            this.fsvc = new Mock<IManageFacets>();

            this.fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(this.facets));

            this.entities = new[] 
            {
                Entity.Factory.CreateNew(e=>e.Name = "e1"),
                Entity.Factory.CreateNew(e=>e.Name = "e2"),
            };

            this.relationships = Enumerable.Empty<Relationship>();

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();

            this.ersvc // expects retrueval aof all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));

            this.ersvc // expect retrieval of existig relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        [TestMethod]
        public void CreateNewRelationshipInitializedWithVisibleFacet()
        {
            // ARRANGE

            this.vm.Facets.Single().IsVisible = true;

            // ACT

            EditNewRelationshipViewModel result = this.vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

            // ASSERT

            Assert.AreEqual(1, result.AssignedFacets.Count());
            Assert.AreEqual(0, result.UnassignedFacets.Count());
            Assert.IsTrue(result.PrepareCommit.CanExecute());
            Assert.IsFalse(result.Commit.CanExecute());
            Assert.IsTrue(result.Rollback.CanExecute());

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }
    }
}
