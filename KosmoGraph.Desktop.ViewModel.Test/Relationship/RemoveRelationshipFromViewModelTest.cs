namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Services;
    using System.Threading.Tasks;
    using KosmoGraph.Test;
    using System.Threading;
    using System.Collections.Generic;

    [TestClass]
    public class RemoveRelationshipFromViewModelTest
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

            this.facets = Enumerable.Empty<Facet>();
            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // expectes retrueval of all Facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(this.facets));

            this.entities = new[]
            {
                Entity.Factory.CreateNew(e=>e.Name="e1"),
                Entity.Factory.CreateNew(e=>e.Name = "e2")
            };
            
            this.relationships = new []
            {
                Relationship.Factory.CreateNew(r => 
                {
                    r.FromId = this.entities.ElementAt(0).Id;
                    r.ToId = this.entities.ElementAt(1).Id;
                })
            };

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();
            this.ersvc // expect retrueval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));
            this.ersvc // expect retrieval of all relationships
                .Setup(_=>_.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));
            this.ersvc // expectes removal of relationship
                .Setup(_ => _.RemoveRelationship(this.relationships.Single()))
                .Returns(Task.FromResult(true));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        #region RemoveRelationship

        [TestMethod]
        [TestCategory("RemoveRelationship")]
        public void RemoveRelationshipFromModel()
        {
            // ARRANGE

            // ACT

            this.vm.Remove(this.vm.Relationships.Single());

            // ASSSERT

            Assert.AreEqual(0, this.vm.Relationships.Count());
            Assert.AreEqual(2, this.vm.Items.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.RemoveRelationship(It.IsAny<Relationship>()), Times.Once);
        }

        #endregion 
    }
}
