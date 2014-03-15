
namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using KosmoGraph.Model;
    using KosmoGraph.Services;
    using KosmoGraph.Test;
    using Moq;
    using System.Threading.Tasks;

    [TestClass]
    public class SelectFacetSelectsAssignedModelItemsTest
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
                Facet.Factory.CreateNew(f => f.Name = "f1")
            };

            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            this.entities = new[]
            {
                Entity.Factory.CreateNew(e=>
                {
                    e.Name ="e1";
                    e.Add(e.CreateNewAssignedFacet(this.facets.Single(), delegate{}));
                }),
                Entity.Factory.CreateNew(e=>e.Name ="e2")
            };

            this.relationships = new[]
            {
                Relationship.Factory.CreateNew(r => 
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

        #region SelectRelationship

        [TestMethod]
        [TestCategory("SelectRelationship")]
        public void SelectRelationshipSelectsItsFacet()
        {
            // ARRANGE

            // ACT

            this.vm.Relationships.Single().IsSelected = true;

            // ASSERT

            Assert.IsTrue(this.vm.Facets.Single().IsItemSelected);
        }

        [TestMethod]
        [TestCategory("SelectRelationship"),TestCategory("ClearSelectedItems")]
        public void UnselectRelationshipUnselectsItsFacet()
        {
            // ARRANGE

            this.vm.Relationships.Single().IsSelected = true;

            // ACT

            this.vm.ClearSelectedItems();

            // ASSERT

            Assert.IsFalse(this.vm.Facets.Single().IsItemSelected);
        }

        #endregion 

        #region SelectEntity

        [TestMethod]
        [TestCategory("SelectEntity")]
        public void SelectEntitySelectsItsFacet()
        {
            // ARRANGE

            // ACT

            this.vm.Entities.First().IsSelected = true;

            // ASSERT

            Assert.IsTrue(this.vm.Facets.Single().IsItemSelected);
        }

        [TestMethod]
        [TestCategory("SelectEntity"),TestCategory("ClearSelectedItems")]
        public void UnselectEntityUnselectsItsFacet()
        {
            // ARRANGE

            this.vm.Entities.First().IsSelected = true;

            // ACT

            this.vm.ClearSelectedItems();

            // ASSERT

            //Assert.IsFalse(this.vm.Facets.Single().IsItemSelected);
        }

        #endregion 
    }
}
