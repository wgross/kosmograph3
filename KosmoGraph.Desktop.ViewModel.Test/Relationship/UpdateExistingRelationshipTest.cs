namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Services;
    using System.Threading.Tasks;
    using KosmoGraph.Test;
    using System.Threading;
    using System.Collections.Generic;

    [TestClass]
    public class UpdateExistingRelationshipViewModelTest
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
                    r.Add(r.CreateNewAssignedFacet(this.facets.First(), f =>
                    {
                        f.Properties.Single().Value = "pv1";
                    }));
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

        #region UpdateExistingRelationship

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void UpdateExistingRelationshipViewModel()
        {
            // ARRANGE

            // ACT

            EditExistingRelationshipViewModel r1edit = this.vm.EditRelationship(this.vm.Relationships.Single());

            // ASSERT

            Assert.AreSame(this.relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(this.vm.Entities.ElementAt(0), r1edit.Edited.From.Entity);
            Assert.AreEqual(this.vm.Entities.ElementAt(1), r1edit.Edited.To.Entity);
            Assert.AreEqual(this.vm.Entities.ElementAt(0), r1edit.From);
            Assert.AreEqual(this.vm.Entities.ElementAt(1), r1edit.To);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreSame(this.facets.First(), r1edit.AssignedFacets.First().Facet.ModelItem);
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(this.facets.First().Properties.First().Id, r1edit.Edited.Properties.First().Definition.ModelItem.Id);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, this.vm.Facets.Count());
            Assert.AreEqual(2, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Relationships.Count());
            Assert.AreEqual(3, this.vm.Items.Count);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingRelationship > Modify

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void ModifyPropertyValueAllowsEditExistigRelationshipViewModelCommit()
        {
            // ARRANGE

            var r1edit = this.vm.EditRelationship(this.vm.Relationships.Single());

            // ACT

            r1edit.Properties.First().Value = "pv1-changed";

            // ASSERT

            Assert.AreSame(this.relationships.Single(), r1edit.Edited.ModelItem);
            Assert.AreEqual(this.vm.Entities.ElementAt(0), r1edit.Edited.From.Entity);
            Assert.AreEqual(this.vm.Entities.ElementAt(1), r1edit.Edited.To.Entity);
            Assert.AreEqual(this.vm.Entities.ElementAt(0), r1edit.From);
            Assert.AreEqual(this.vm.Entities.ElementAt(1), r1edit.To);
            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreSame(this.facets.First(), r1edit.AssignedFacets.First().Facet.ModelItem);
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(this.facets.First().Properties.First().Id, r1edit.Edited.Properties.First().Definition.ModelItem.Id);
            Assert.AreEqual("pv1-changed", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, this.vm.Facets.Count());
            Assert.AreEqual(2, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Relationships.Count());
            Assert.AreEqual("pv1", this.vm.Relationships.Single().Properties.First().Value);
            Assert.AreEqual(3, this.vm.Items.Count);

            Assert.AreEqual("pv1", this.relationships.Single().AssignedFacets.First().Properties.First().Value);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingRelationship > Modify > Commit

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void CommitEditExistingRelationshipViewModelUpdateRelationship()
        {
            // ARRANGE

            ersvc // expect update of relationship
               .Setup(_ => _.UpdateRelationship(this.relationships.Single()))
               .Returns<Relationship>(r => Task.FromResult(r));

            var r1edit = this.vm.EditRelationship(this.vm.Relationships.Single());

            r1edit.Properties.First().Value = "pv1-changed";

            // ACT

            r1edit.Commit.Execute();

            // ASSERT

            Assert.AreSame(this.relationships.Single(), r1edit.Edited.ModelItem);
            Assert.AreEqual(this.vm.Entities.ElementAt(0), r1edit.Edited.From.Entity);
            Assert.AreEqual(this.vm.Entities.ElementAt(1), r1edit.Edited.To.Entity);
            Assert.AreEqual(this.vm.Entities.ElementAt(0), r1edit.From);
            Assert.AreEqual(this.vm.Entities.ElementAt(1), r1edit.To);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreSame(this.facets.First(), r1edit.AssignedFacets.First().Facet.ModelItem);
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(this.facets.First().Properties.First().Id, r1edit.Edited.Properties.First().Definition.ModelItem.Id);
            Assert.AreEqual("pv1-changed", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, this.vm.Facets.Count());
            Assert.AreEqual(2, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Relationships.Count());
            Assert.AreEqual("pv1-changed", this.vm.Relationships.Single().Properties.First().Value);
            Assert.AreEqual(3, this.vm.Items.Count);

            Assert.AreEqual("pv1-changed", this.relationships.Single().AssignedFacets.First().Properties.First().Value);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingRelationship > Modify > Rollback

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void RollbackEditExistingEntityViewModelInitializesAgain()
        {
            // ARRANGE
            
            var r1edit = this.vm.EditRelationship(this.vm.Relationships.Single());

            r1edit.Properties.First().Value = "pv1-changed";

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.AreSame(this.relationships.Single(), r1edit.Edited.ModelItem);
            Assert.AreEqual(this.vm.Entities.ElementAt(0), r1edit.Edited.From.Entity);
            Assert.AreEqual(this.vm.Entities.ElementAt(1), r1edit.Edited.To.Entity);
            Assert.AreEqual(this.vm.Entities.ElementAt(0), r1edit.From);
            Assert.AreEqual(this.vm.Entities.ElementAt(1), r1edit.To);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreSame(this.facets.First(), r1edit.AssignedFacets.First().Facet.ModelItem);
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(this.facets.First().Properties.First().Id, r1edit.Edited.Properties.First().Definition.ModelItem.Id);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, this.vm.Facets.Count());
            Assert.AreEqual(2, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Relationships.Count());
            Assert.AreEqual("pv1", this.vm.Relationships.Single().Properties.First().Value);
            Assert.AreEqual(3, this.vm.Items.Count);

            Assert.AreEqual("pv1", this.relationships.Single().AssignedFacets.First().Properties.First().Value);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingRelationship > Modify > Rolback > Modify > Commit

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void RollbackEditExistingRelationshipViewModelAllowsEditingAgainTillCommit()
        {
            // ARRANGE

            ersvc // expect update of relationship
                .Setup(_ => _.UpdateRelationship(this.relationships.Single()))
                .Returns<Relationship>(r => Task.FromResult(r));

            var r1edit = this.vm.EditRelationship(this.vm.Relationships.Single());

            r1edit.Properties.First().Value = "pv1-changed";
            r1edit.Rollback.Execute();

            // ACT

            r1edit.Properties.First().Value = "pv1-changed";
            r1edit.Commit.Execute();

            // ASSERT

            Assert.AreSame(this.relationships.Single(), r1edit.Edited.ModelItem);
            Assert.AreEqual(this.vm.Entities.ElementAt(0), r1edit.Edited.From.Entity);
            Assert.AreEqual(this.vm.Entities.ElementAt(1), r1edit.Edited.To.Entity);
            Assert.AreEqual(this.vm.Entities.ElementAt(0), r1edit.From);
            Assert.AreEqual(this.vm.Entities.ElementAt(1), r1edit.To);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreSame(this.facets.First(), r1edit.AssignedFacets.First().Facet.ModelItem);
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(this.facets.First().Properties.First().Id, r1edit.Edited.Properties.First().Definition.ModelItem.Id);
            Assert.AreEqual("pv1-changed", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, this.vm.Facets.Count());
            Assert.AreEqual(2, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Relationships.Count());
            Assert.AreEqual("pv1-changed", this.vm.Relationships.Single().Properties.First().Value);
            Assert.AreEqual(3, this.vm.Items.Count);

            Assert.AreEqual("pv1-changed", this.relationships.Single().AssignedFacets.First().Properties.First().Value);

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