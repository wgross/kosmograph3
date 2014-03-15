using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KosmoGraph.Test;
using Moq;
using System.Collections.Generic;
using KosmoGraph.Model;
using KosmoGraph.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace KosmoGraph.Desktop.ViewModel.Test
{
    [TestClass]
    public class CreateEntityRelationshipViewModelTest
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
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());

            this.facets = new[]
            {
                Facet.Factory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };

            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // expectes retrieval of all Facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(this.facets));

            this.entities = new[]
            {
                Entity.Factory.CreateNew(e=>
                {
                    e.Name="e1";
                    e.Add(e.CreateNewAssignedFacet(this.facets.Single(), af => af.Properties.Single().Value = "pv1"));
                }),
                Entity.Factory.CreateNew(e=>e.Name = "e2")
            };

            this.relationships = new[]
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
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));
        }

        [TestMethod]
        [TestCategory("CreateNewEntityRelationshipModel")]
        public void CreateViewModelFromExistingModel()
        {
            // ARRANGE

            // ACT

            EntityRelationshipViewModel vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);

            // ASSERT

            Assert.AreEqual(1, vm.Facets.Count());
            Assert.AreEqual("f1", vm.Facets.Single().Name);
            Assert.AreEqual(1, vm.Facets.Single().Properties.Count());
            Assert.AreEqual("pd1", vm.Facets.Single().Properties.Single().Name);

            Assert.AreEqual(2, vm.Entities.Count());
            Assert.AreEqual("e1", vm.Entities.ElementAt(0).Name);
            Assert.AreEqual("e2", vm.Entities.ElementAt(1).Name);

            Assert.AreEqual(1, vm.Relationships.Count());
            Assert.AreSame(vm.Entities.ElementAt(0), vm.Relationships.Single().From.Entity);
            Assert.AreSame(vm.Entities.ElementAt(1), vm.Relationships.Single().To.Entity);
        }
    }
}
