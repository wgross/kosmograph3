using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using KosmoGraph.Services;
using Moq;
using KosmoGraph.Model;
using System.Threading;
using KosmoGraph.Test;
using System.Threading.Tasks;
using System.Linq;

namespace KosmoGraph.Desktop.ViewModel.Test
{
    [TestClass]
    public class CreateNewRelationshipFacetAtExistingRelationshipTest
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

            this.relationships = new[]
            {
                Relationship.Factory.CreateNew(r => 
                {
                    r.FromId = this.entities.ElementAt(0).Id;
                    r.ToId = this.entities.ElementAt(1).Id;
                })
            };

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrueval aof all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));

            ersvc // expect retrieval of existig relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        #region UpdateExistingRelationship > AssignFacet

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("EditRelationship")]
        public void CreateNewRelationshipFacetViewModelWithPropertyAtExistingRelationshipViewModel()
        {
            // ARRANGE

            
            var r1edit = this.vm.EditRelationship(vm.Relationships.First());

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
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 
        
        #region UpdateExistingRelationship > AssignFacet > UnassignFacet

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("EditRelationship")]
        public void CreateNewEmptyRelationshipFacetViewModelAtExistingRelationshipButRemoveFromRelationshipAgain()
        {
            // ARRANGE

            var r1edit = this.vm.EditRelationship(vm.Relationships.First());

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());

            // ACT

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());
            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("EditRelationship")]
        public void CreateNewRelationshipFacetViewModelWithPropertyAtExistingRelationshipViewModelButRemoveFromRelatinshipAgain()
        {
            // ARRANGE

            var r1edit = this.vm.EditRelationship(vm.Relationships.First());

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
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region UpdateExistingRelationship > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("EditRelationship")]
        public void CommitNewRelationshipFacetViewModelWithPropertyAtExistingRelatinshipCreatesNewAssignedRelationshipFacet()
        {
            // ARRANGE

            var r1edit = this.vm.EditRelationship(vm.Relationships.First());

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreSame(this.facets.First(), vm.Relationships.First().AssignedFacets.First().Facet.ModelItem);

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(1, relationships.First().AssignedFacets.Count());
            Assert.AreEqual(1, relationships.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual("pv1", relationships.First().AssignedFacets.First().Properties.First().Value);

            Assert.AreEqual(this.facets.First().Id, relationships.First().AssignedFacets.First().FacetId);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingRelationship > AssignFacet > Rollback

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("EditRelationship")]
        public void RollbackNewEmptyRelationshipFacetViewModelAtExistingRelationshipInitializesAgain()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
            var facets = new[]
            {
                Facet.Factory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(this.facets.AsEnumerable()));

            var entities = new[] 
            {
                Entity.Factory.CreateNew(e=>e.Name = "e1"),
                Entity.Factory.CreateNew(e=>e.Name = "e2"),
            };

            var relationships = new[]
            {
                Relationship.Factory.CreateNew(r => 
                {
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of all relatinships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("EditRelationship")]
        public void RollbackNewRelationshipFacetViewModelWithPropertyAtExistingRelatinshipInitializesAgain()
        {
            // ARRANGE

            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingRelationship > AssignFacet > Rollback > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("EditRelationship")]
        public void RollbackNewRelationshipFacetViewModelWithPropertyAtExistingRelationshipAllowsEditTillCommitAgain()
        {
            // ARRANGE

            this.ersvc // expect update of relatinship
                .Setup(_ => _.UpdateRelationship(relationships.First()))
                .Returns<Relationship>(r => Task.FromResult(r));

            var r1edit = this.vm.EditRelationship(this.vm.Relationships.Single());

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";
            r1edit.Rollback.Execute();

            // ACT

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";
            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreSame(this.facets.First(), this.vm.Relationships.First().AssignedFacets.First().Facet.ModelItem);

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(1, relationships.First().AssignedFacets.Count());
            Assert.AreEqual(1, relationships.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual("pv1", relationships.First().AssignedFacets.First().Properties.First().Value);

            Assert.AreEqual(this.facets.First().Id, relationships.First().AssignedFacets.First().FacetId);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion
    }
}
