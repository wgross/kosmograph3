namespace Kosmograph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Desktop.ViewModel;
    using KosmoGraph.Test;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Services;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;

    [TestClass]
    public sealed class UpdateExistingEntityViewModelTest
    {
        private Mock<IManageEntitiesAndRelationships> ersvc;
        private Mock<IManageFacets> fsvc;
        private EntityRelationshipViewModel vm;
        private IEnumerable<Entity> entities;
        private IEnumerable<Facet> facets;

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
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // Db contains no facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(this.facets));

            this.entities = new[]
            {
                Entity.Factory.CreateNew(e => 
                {
                    e.Name ="e1";
                    e.Add(e.CreateNewAssignedFacet(facets.Single(), f=>f.Properties.Single().Value = "pv1"));
                })
            };

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();
            
            this.ersvc // Db contains no this.entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(Enumerable.Empty<Entity>()));
            
            this.ersvc // Db Contains no relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(Enumerable.Empty<Relationship>()));

            this.ersvc
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(Enumerable.Empty<Relationship>()));

            this.ersvc
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        #region UpdateExistingEntity > PrepareCommit

        [TestMethod]
        [TestCategory("EditEntity"),TestCategory("ValidateEntity")]
        public void UpdateExistingEmptyEntityViewModel()
        {
            // ARRANGE

            // ACT

            EditExistingEntityViewModel e1edit = this.vm.EditEntity(this.vm.Entities.Single());
            e1edit.PrepareCommit.Execute();

            // ASSERT
            // edit is initialized but cant commit because it is not changed

            Assert.AreEqual("e1", e1edit.Name);
            Assert.AreSame(this.entities.Single(), e1edit.Edited.ModelItem);
            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.PrepareCommit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            
            Assert.AreEqual(1, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Items.Count);
            
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.ValidateEntity(It.IsAny<string>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region UpdateExistingEntity > Modify > PrepareCommit

        [TestMethod]
        [TestCategory("EditEntity"),TestCategory("ValidateEntity")]
        public void ModifyNameAllowsEditExistingEntityViewModelCommit()
        {
            // ARRANGE

            this.ersvc // validate changed tests name
                .Setup(_ => _.ValidateEntity("e1-changed"))
                .Returns(Task.FromResult(new ValidateEntityResult { NameIsNotUnique = false, NameIsNullOrEmpty = false }));

            var e1edit = this.vm.EditEntity(this.vm.Entities.Single());

            // ACT

            e1edit.Name = "e1-changed";
            e1edit.PrepareCommit.Execute();

            // ASSERT
            // new name makes edit committable

            Assert.AreEqual("e1-changed", e1edit.Name);
            Assert.AreSame(entities.Single(), e1edit.Edited.ModelItem);
            Assert.AreEqual("e1", this.entities.Single().Name);
            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.PrepareCommit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());

            Assert.AreEqual(1, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Items.Count());

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.ValidateEntity(It.IsAny<string>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("EditEntity"), TestCategory("ValidateEntity")]
        public void ModifyPropertyValueAllowsEditExistigEntityViewModelCommit()
        {
            // ARRANGE

            var e1edit = this.vm.EditEntity(this.vm.Entities.Single());

            // ACT

            e1edit.Properties.Single().Value = "pv1-changed";
            e1edit.PrepareCommit.Execute();

            // ASSERT
            // new name makes edit committable

            Assert.AreSame(this.entities.Single(), e1edit.Edited.ModelItem);
            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.PrepareCommit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            Assert.AreEqual("pv1-changed", e1edit.Properties.Single().Value);
            Assert.AreEqual(0, e1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Entities.Single().AssignedFacets.Count());
            Assert.AreEqual(1, this.vm.Entities.Single().AssignedFacets.Single().Properties.Count());
            Assert.AreEqual("pv1", this.vm.Entities.Single().AssignedFacets.Single().Properties.Single().Value);
            Assert.AreEqual(1, this.vm.Items.Count);

            Assert.AreEqual("pv1", this.entities.Single().AssignedFacets.Single().Properties.Single().Value);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.ValidateEntity(It.IsAny<string>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region UpdateExistingEntity > Modify > PrepareCommit > Commit

        [TestMethod]
        [TestCategory("EditEntity"),TestCategory("ValidateEntity")]
        public void CommitEditExistingEntityViewModelUpdatesEntity()
        {
            // ARRANGE

            this.ersvc // validate changed tests name
                .Setup(_ => _.ValidateEntity("e1-changed"))
                .Returns(Task.FromResult(new ValidateEntityResult { NameIsNotUnique = false, NameIsNullOrEmpty = false }));

            this.ersvc // expects update of e1
                .Setup(_ => _.UpdateEntity(this.entities.Single()))
                .Returns((Entity e) => Task.FromResult(e));

            var e1edit = this.vm.EditEntity(this.vm.Entities.Single());
            
            e1edit.Name = "e1-changed";
            e1edit.Properties.Single().Value = "pv1-changed";
            e1edit.PrepareCommit.Execute();
            
            // ACT

            e1edit.Commit.Execute();
            
            // ASSERT
            // commit added entity to model and call entity service

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.PrepareCommit.CanExecute());
            Assert.IsFalse(e1edit.Rollback.CanExecute());
            Assert.AreEqual("e1-changed", e1edit.Name);
            Assert.AreSame(this.entities.Single(), e1edit.Edited.ModelItem);
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            
            Assert.AreEqual(1, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Items.Count());
            Assert.AreEqual("e1-changed", this.vm.Entities.Single().Name);

            Assert.AreEqual("e1-changed", this.entities.Single().Name);
            Assert.AreEqual("pv1-changed", this.entities.Single().AssignedFacets.Single().Properties.Single().Value);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.ValidateEntity(It.IsAny<string>()), Times.Once);
            this.ersvc.Verify(_ => _.UpdateEntity(It.IsAny<Entity>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingEntity > Modify > Rollback

        [TestMethod]
        [TestCategory("EditEntity"),TestCategory("ValidateEntity")]
        public void RollbackEditExistingEntityViewModelInitializesAgain()
        {
            // ARRANGE

            var e1edit = this.vm.EditEntity(vm.Entities.Single());
            
            e1edit.Name = "e1-changed";
            e1edit.Properties.Single().Value = "pv1-changed";

            // ACT

            e1edit.Rollback.Execute();

            // ASSERT
            // rollback sets name back

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual("e1", e1edit.Name);
            Assert.AreSame(this.entities.Single(), e1edit.Edited.ModelItem);
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());

            Assert.AreEqual(1, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Items.Count());
            Assert.AreEqual("e1", this.vm.Entities.Single().Name);

            Assert.AreEqual("e1", this.entities.Single().Name);
            Assert.AreEqual("pv1", this.entities.Single().AssignedFacets.Single().Properties.Single().Value);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingEntity > Modify > Rollback > PrepareCommit > Commit

        [TestMethod]
        [TestCategory("EditEntity"),TestCategory("ValidateEntity")]
        public void RollbackEditExistingEntityViewModelAllowsEditingAgainTillCommit()
        {
            // ARRANGE

            var e1edit = this.vm.EditEntity(this.vm.Entities.Single());

            e1edit.Name = "e1-changed";
            e1edit.Properties.Single().Value = "pv1-changed";
            e1edit.Rollback.Execute();

            // ACT

            e1edit.Name = "e1-changed";
            e1edit.Properties.Single().Value = "pv1-changed";
            e1edit.PrepareCommit.Execute();
            e1edit.Commit.Execute();

            // ASSERT
            // commit added entity to model and call entity service

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual("e1-changed", e1edit.Name);
            Assert.AreSame(this.entities.Single(), e1edit.Edited.ModelItem);
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());

            Assert.AreEqual(1, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Items.Count());
            Assert.AreEqual("e1-changed", this.vm.Entities.Single().Name);

            Assert.AreEqual("e1-changed", this.entities.Single().Name);
            Assert.AreEqual("pv1-changed", this.entities.Single().AssignedFacets.Single().Properties.Single().Value);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.ValidateEntity(It.IsAny<string>()), Times.Once);
            this.ersvc.Verify(_ => _.UpdateEntity(It.IsAny<Entity>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        //[TestMethod]
        //[TestCategory("EditEntity")]
        //public void InitializeEntityViewModelWithVisibleTagsFromModel()
        //{
        //    // ARRANGE

        //    var ersvc = new Mock<IManageEntitiesAndRelationships>();
        //    ersvc // expects entity creation
        //        .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
        //        .Returns((Action<Entity> a) => Entity.Factory.CreateNew(a));


        //    var fsvc = new Mock<IManageFacets>();
        //    fsvc // expects Facet creation
        //        .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
        //        .Returns((Action<Facet> a) => Facet.Factory.CreateNew(a));

        //    var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
        //    var f1 = this.vm.Add(vm.CreateNewFacet("f1"));

        //    f1.IsVisible = true;

        //    // ACT

        //    EntityViewModel e1 = this.vm.CreateNewEntity("e1");

        //    // ASSERT
        //    // adding an e1 to the model lets it show up in Items list and this.entities list
        //    // it is initialize with the first available Tag

        //    Assert.IsTrue(e1.IsVisible);
        //    Assert.AreEqual(1, e1.AssignedTags.Count());
        //    Assert.AreEqual(f1, e1.AssignedTags.Single().Tag);
        //    Assert.AreEqual(1, e1.ModelItem.AssignedFacets.Count());
        //    Assert.AreEqual(f1.ModelItem.Id, e1.ModelItem.AssignedFacets.Single().FacetId);

        //    ersvc.VerifyAll();
        //    fsvc.VerifyAll();
        //}
    }
}
