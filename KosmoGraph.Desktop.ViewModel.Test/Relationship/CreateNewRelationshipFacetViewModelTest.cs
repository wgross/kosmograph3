
namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using KosmoGraph.Services;
    using KosmoGraph.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KosmoGraph.Test;
    using System.Threading;

    [TestClass]
    public class CreateNewRelationshipFacetViewModelTest
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
                Facet.Factory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
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

            ersvc // expects retrueval aof all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));

            ersvc // expect retrieval of existig relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        #region CreateNewRelationship > AssignFacet

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void CreateNewRelationshipFacetViewModelWithPropertyAtNewRelationship()
        {
            // ARRANGE

            var r1edit = vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

            r1edit.SetDestination.Execute(this.vm.Entities.ElementAt(1));

            // ACT

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(this.facets.First().Properties.First().Id, r1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewRelationship > AssignFacet > UnassignFacet

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void CreateNewRelationshipFacetViewModelWithPropertytNewRelatinshipButRemoveFromRelationshipAgain()
        {
            // ARRANGE

            var r1edit = this.vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

            r1edit.SetDestination.Execute(this.vm.Entities.ElementAt(1));
            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewRelationship > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void CommitNewRelationshipFacetViewModelWithPropertyAtNewRelatinshipCreatesNewAssignedRelationshipFacet()
        {
            // ARRANGE

            Relationship r1 = null;
            ersvc // expect creation of partial relationship with source entity
              .Setup(_ => _.CreatePartialRelationship(this.entities.ElementAt(0), It.IsAny<Action<Relationship>>()))
              .Returns<Entity, Action<Relationship>>((e, a) =>
              {
                  a(r1 = Relationship.Factory.CreateNewPartial(e));
                  return r1;
              });

            ersvc
                .Setup(_ => _.CompletePartialRelationship(It.Is<Relationship>(r => r.FromId == this.entities.ElementAt(0).Id && r.AssignedFacets.Count() == 1), this.entities.ElementAt(1)))
                .Returns<Relationship, Entity>((r, e) => Task.FromResult(new CompletePartialRelationshipResult(r, this.entities.ElementAt(1), e)));

            var r1edit = vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

            r1edit.SetDestination.Execute(this.vm.Entities.ElementAt(1));
            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(this.facets.First().Properties.First().Id, r1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);

            Assert.AreEqual(1, r1.AssignedFacets.Count());
            Assert.AreEqual(this.facets.First().Id, r1.AssignedFacets.First().FacetId);
            Assert.AreEqual(this.facets.First().Properties.First().Id, r1.AssignedFacets.First().Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1.AssignedFacets.First().Properties.First().Value);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.CompletePartialRelationship(It.IsAny<Relationship>(), this.entities.ElementAt(1)), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewRelationship > AssignFacet > Rollback

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void RollbackNewRelationshipFacetViewModelWithPropertyAtNewRelationshipInitializesAgain()
        {
            // ARRANGE

            var r1edit = vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewRelationship > AssignFacet > Rollback > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void RollbackNewRelationshipFacetViewModelWithPropertyAtNewRelationshipAllowsEditTillCommitAgain()
        {
            // ARRANGE

            Relationship r1 = null;
            this.ersvc // expect creation of partial relationship with source entity
              .Setup(_ => _.CreatePartialRelationship(this.entities.ElementAt(0), It.IsAny<Action<Relationship>>()))
              .Returns<Entity, Action<Relationship>>((e, a) =>
              {
                  a(r1 = Relationship.Factory.CreateNewPartial(e));
                  return r1;
              });

            this.ersvc
                .Setup(_ => _.CompletePartialRelationship(It.Is<Relationship>(r => r.FromId == this.entities.ElementAt(0).Id && r.AssignedFacets.Count() == 1), this.entities.ElementAt(1)))
                .Returns<Relationship, Entity>((r, e) => Task.FromResult(new CompletePartialRelationshipResult(r, this.entities.ElementAt(0), e)));

            var r1edit = this.vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

            r1edit.SetDestination.Execute(this.vm.Entities.ElementAt(1));
            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";
            r1edit.Rollback.Execute();

            // ACT

            r1edit.SetDestination.Execute(this.vm.Entities.ElementAt(1));
            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";
            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(this.facets.First().Properties.First().Id, r1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);

            Assert.AreEqual(1, r1.AssignedFacets.Count());
            Assert.AreEqual(this.facets.First().Id, r1.AssignedFacets.First().FacetId);
            Assert.AreEqual(this.facets.First().Properties.First().Id, r1.AssignedFacets.First().Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1.AssignedFacets.First().Properties.First().Value);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.CompletePartialRelationship(It.IsAny<Relationship>(), this.entities.ElementAt(1)), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion
    }
}
