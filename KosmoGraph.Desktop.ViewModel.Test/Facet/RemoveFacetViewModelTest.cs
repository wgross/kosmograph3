namespace KosmoGraph.Desktop.ViewModel.Test.Facet
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Services;
    using System.Threading.Tasks;
    using KosmoGraph.Test;

    [TestClass]
    public class RemoveFacetViewModelTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        [TestMethod]
        [TestCategory("RemoveFacet")]
        public void RemoveUnusedEmptyFacetViewModelFromViewModel()
        {
            // ARRANGE

            var facets = new []
            {
                FacetFactory.CreateNew(f => f.Name ="f1")
            };

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect the retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            fsvc // expect deletion of the facet
                .Setup(_ => _.RemoveFacet(facets.First()))
                .Returns(Task.FromResult(true));

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);

            // ACT

            vm.Remove(vm.Facets.Single());

            // ASSERT

            Assert.AreEqual(0, vm.Facets.Count());
            Assert.AreEqual(0, vm.Items.Count());
        }

        [TestMethod]
        [TestCategory("RemoveFacet")]
        public void RemoveUsedEmptyFacetViewModelFromEntityAndRelationshipViewModel()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name ="f1")
            };

            
            var fsvc = new Mock<IManageFacets>();

            fsvc // expect the retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            fsvc // expect deletion of the facet
                .Setup(_ => _.RemoveFacet(facets.First()))
                .Returns(Task.FromResult(true));

            var entities = new[]
            {
                EntityFactory.CreateNew(e =>
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                }),
                EntityFactory.CreateNew(e =>
                {
                    e.Name = "e2";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                })
            };

            var relationships = new []
            {
                RelationshipFactory.CreateNew(r => 
                {
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));
            
            ersvc // expect retrieval of all relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));
            
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);

            // ACT

            vm.Remove(vm.Facets.Single());

            // ASSERT

            Assert.AreEqual(0, vm.Facets.Count());
            Assert.AreEqual(3, vm.Items.Count());
            Assert.AreEqual(2, vm.Entities.Count());
            Assert.AreEqual(1, vm.Relationships.Count());

            Assert.AreEqual(0, vm.Entities.ElementAt(0).AssignedFacets.Count());
            Assert.AreEqual(0, vm.Entities.ElementAt(1).AssignedFacets.Count());
            Assert.AreEqual(0, vm.Relationships.Single().AssignedFacets.Count());
        }

        [TestMethod]
        [TestCategory("RemoveFacet")]
        public void RemoveUsedFacetViewModelWithPropertyFromEntityAndRelationshipViewModel()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name ="f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name ="pd1"));
                })
            };


            var fsvc = new Mock<IManageFacets>();

            fsvc // expect the retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            fsvc // expect deletion of the facet
                .Setup(_ => _.RemoveFacet(facets.First()))
                .Returns(Task.FromResult(true));

            var entities = new[]
            {
                EntityFactory.CreateNew(e =>
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                    e.AssignedFacets.Single().Properties.Single().Value = "pv1";
                }),
                EntityFactory.CreateNew(e =>
                {
                    e.Name = "e2";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                    e.AssignedFacets.Single().Properties.Single().Value = "pv2";
                })
            };

            var relationships = new[]
            {
                RelationshipFactory.CreateNew(r => 
                {
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                    r.AssignedFacets.Single().Properties.Single().Value = "pv3";
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of all relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);

            // ACT

            vm.Remove(vm.Facets.Single());

            // ASSERT

            Assert.AreEqual(0, vm.Facets.Count());
            Assert.AreEqual(3, vm.Items.Count());
            Assert.AreEqual(2, vm.Entities.Count());
            Assert.AreEqual(1, vm.Relationships.Count());

            Assert.AreEqual(0, vm.Entities.ElementAt(0).AssignedFacets.Count());
            Assert.AreEqual(0, vm.Entities.ElementAt(0).Properties.Count());
            Assert.AreEqual(0, vm.Entities.ElementAt(1).AssignedFacets.Count());
            Assert.AreEqual(0, vm.Entities.ElementAt(1).Properties.Count());
            Assert.AreEqual(0, vm.Relationships.Single().AssignedFacets.Count());
            Assert.AreEqual(0, vm.Relationships.Single().Properties.Count());
        }
    }
}
