namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using KosmoGraph.Services;
    using System.Threading.Tasks;
    using KosmoGraph.Test;
    using KosmoGraph.Model;
    using System.Linq;
    using System.Collections.Generic;

    [TestClass]
    public class ToggleFacetVisiblityTogglesModelItemVisibilityTest
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

            // provide a facet with a property definition
            this.facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            this.entities = new[]
            {
                EntityFactory.CreateNew(e=>
                {
                    e.Name ="e1";
                    e.Add(e.CreateNewAssignedFacet(this.facets.Single(), delegate{}));
                }),
                EntityFactory.CreateNew(e=>e.Name ="e2")
            };

            this.relationships = new[]
            {
                RelationshipFactory.CreateNew(r => 
                {
                    r.FromId = this.entities.ElementAt(0).Id;
                    r.ToId = this.entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(this.facets.Single(), delegate{}));
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
        [TestCategory("ShowFacet")]
        public void MakeEntityAndRelationshipVisible()
        {
            // ARRANGE

            // ACT

            this.vm.Facets.Single().IsVisible = true;

            // ASSERT

            Assert.IsTrue(this.vm.Relationships.Single().IsVisible);
            Assert.IsTrue(this.vm.Entities.ElementAt(0).IsVisible);
        }

        [TestMethod]
        [TestCategory("ShowFacet")]
        public void MakeEntityAndRelationshipInvisible()
        {
            // ARRANGE

            this.vm.Facets.Single().IsVisible = true;

            // ACT

            this.vm.Facets.Single().IsVisible = false;

            // ASSERT

            Assert.IsFalse(this.vm.Relationships.Single().IsVisible);
            Assert.IsFalse(this.vm.Entities.ElementAt(0).IsVisible);
        }
    }
}
