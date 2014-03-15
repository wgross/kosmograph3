
namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using System.Collections.Generic;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Services;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Threading;

    [TestClass]
    public class RemoveFacetAsEntityFacetTest
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
                Facet.Factory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };

            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            this.fsvc // expect deletion of the facet
                .Setup(_ => _.RemoveFacet(facets.First()))
                .Returns(Task.FromResult(true));

            this.entities = new[]
            {
                Entity.Factory.CreateNew(e =>
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                    e.AssignedFacets.Single().Properties.Single().Value = "pv1";
                }),
                Entity.Factory.CreateNew(e =>
                {
                    e.Name = "e2";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                    e.AssignedFacets.Single().Properties.Single().Value = "pv2";
                })
            };

            this.relationships = new[]
            {
                Relationship.Factory.CreateNew(r => 
                {
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                    r.AssignedFacets.Single().Properties.Single().Value = "pv3";
                })
            };

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
        public void RemoveUsedEmptyFacetViewModelFromEntityAndRelationshipViewModel()
        {
            // ARRANGE

            // ACT

            this.vm.Remove(this.vm.Facets.Single());

            // ASSERT

            Assert.AreEqual(0, this.vm.Facets.Count());
            Assert.AreEqual(3, this.vm.Items.Count());
            Assert.AreEqual(2, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Relationships.Count());

            Assert.AreEqual(0, this.vm.Entities.ElementAt(0).AssignedFacets.Count());
            Assert.AreEqual(0, this.vm.Entities.ElementAt(1).AssignedFacets.Count());
            Assert.AreEqual(0, this.vm.Relationships.Single().AssignedFacets.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }

        [TestMethod]
        [TestCategory("RemoveFacet")]
        public void RemoveUsedFacetViewModelWithPropertyFromEntityAndRelationshipViewModel()
        {
            // ARRANGE

            // ACT

            this.vm.Remove(this.vm.Facets.Single());

            // ASSERT

            Assert.AreEqual(0, this.vm.Facets.Count());
            Assert.AreEqual(3, this.vm.Items.Count());
            Assert.AreEqual(2, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Relationships.Count());

            Assert.AreEqual(0, this.vm.Entities.ElementAt(0).AssignedFacets.Count());
            Assert.AreEqual(0, this.vm.Entities.ElementAt(0).Properties.Count());
            Assert.AreEqual(0, this.vm.Entities.ElementAt(1).AssignedFacets.Count());
            Assert.AreEqual(0, this.vm.Entities.ElementAt(1).Properties.Count());
            Assert.AreEqual(0, this.vm.Relationships.Single().AssignedFacets.Count());
            Assert.AreEqual(0, this.vm.Relationships.Single().Properties.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }
    }
}
