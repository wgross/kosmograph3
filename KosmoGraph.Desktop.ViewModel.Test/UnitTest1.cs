//namespace KosmoGraph.Desktop.ViewModel.Test
//{
//    using System;
//    using System.Linq;
//    using Microsoft.VisualStudio.TestTools.UnitTesting;
//    using System.Collections.Generic;
//    using KosmoGraph.Model;
//    using KosmoGraph.Services;
//    using Moq;
//    using KosmoGraph.Test;
//    using System.Threading;
//    using System.Threading.Tasks;

//    [TestClass]
//    public class EntityRelationshipDiagramItemsTest
//    {
//        private IEnumerable<Facet> facets;
//        private Mock<IManageFacets> fsvc;
//        private IEnumerable<Entity> entities;
//        private IEnumerable<Relationship> relationships;
//        private Mock<IManageEntitiesAndRelationships> ersvc;
//        private EntityRelationshipViewModel vm;

//        [TestInitialize]
//        public void BeforeEachTest()
//        {
//            // install sync Task Scheduler
//            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());

//            this.facets = new[]
//            {
//                Facet.Factory.CreateNew(f => 
//                {
//                    f.Name = "f1";
//                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
//                })
//            };

//            this.fsvc = new Mock<IManageFacets>();
//            this.fsvc // expectes retrieval of all Facets
//                .Setup(_ => _.GetAllFacets())
//                .Returns(Task.FromResult(this.facets));

//            this.entities = new[]
//            {
//                Entity.Factory.CreateNew(e=>
//                {
//                    e.Name="e1";
//                    e.Add(e.CreateNewAssignedFacet(this.facets.Single(), af => af.Properties.Single().Value = "pv1"));
//                }),
//                Entity.Factory.CreateNew(e=>e.Name = "e2")
//            };

//            this.relationships = Enumerable.Empty<Relationship>();

//            this.ersvc = new Mock<IManageEntitiesAndRelationships>();
//            this.ersvc // expect retrueval of all entities
//                .Setup(_ => _.GetAllEntities())
//                .Returns(Task.FromResult(this.entities));
//            this.ersvc // expect retrieval of all relationships
//                .Setup(_ => _.GetAllRelationships())
//                .Returns(Task.FromResult(this.relationships));
//        }

//        [TestMethod]
//        public void AddPendingRelationshipToViewModel()
//        {
//            // ARRANGE

//            this.vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

//            // ACT


//        }
//    }
//}
