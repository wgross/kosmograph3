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
    using KosmoGraph.Desktop.ViewModel.Properties;

    [TestClass]
    public class CreateNewEntityViewModelTest
    {
        private Mock<IManageEntitiesAndRelationships> ersvc;
        private Mock<IManageFacets> fsvc;
        private EntityRelationshipViewModel vm;

        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());

            // There is only a view model

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();
            this.ersvc // Db contains no entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(Enumerable.Empty<Entity>()));
            this.ersvc // Db Contains no relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(Enumerable.Empty<Relationship>()));

            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // Db contains no facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(Enumerable.Empty<Facet>()));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        #region CreateNewEntity > PrepareCommit

        [TestMethod]
        [TestCategory("CreateNewEntity"), TestCategory("ValidateEntity")]
        public void CreateEditNewEntityViewModel()
        {
            // ARRANGE

            // ACT

            EditNewEntityViewModel e1edit = this.vm.CreateNewEntity();
            e1edit.PrepareCommit.Execute();

            // ASSERT
            // edit is initialized but cant commit

            Assert.AreEqual(KosmoGraph.Desktop.ViewModel.Properties.Resources.EditNewEntityViewModelNameDefault, e1edit.Name);
            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.IsTrue(e1edit.PrepareCommit.CanExecute());
            Assert.IsTrue(e1edit.HasErrors);
            Assert.AreEqual(Resources.ErrorEntityNameIsNullOrEmpty, e1edit.GetErrors("Name").Cast<string>().Single());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.AreEqual(0, this.vm.Entities.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewEntity > Modify > PrepareCommit

        [TestMethod]
        [TestCategory("CreateNewEntity"), TestCategory("ValidateEntity")]
        public void ModifyNameAllowsEditNewEntityViewModelCommit()
        {
            // ARRANGE

            this.ersvc // validate entity test name
                .Setup(_ => _.ValidateEntity("e1"))
                .Returns(Task.FromResult(new ValidateEntityResult { NameIsNotUnique = false, NameIsNullOrEmpty = false }));

            var e1edit = this.vm.CreateNewEntity();

            // ACT

            e1edit.Name = "e1";
            e1edit.PrepareCommit.Execute();

            // ASSERT
            // valid name make edit committable

            Assert.AreEqual("e1", e1edit.Name);
            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.IsTrue(e1edit.PrepareCommit.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.IsFalse(e1edit.HasErrors);
            Assert.AreEqual(0, e1edit.GetErrors("Name").Cast<string>().Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.AreEqual(0, this.vm.Entities.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.ValidateEntity(It.IsAny<string>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewEntity > Modify > PrepareCommit > Commit

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void CommitEditNewEntityViewModelCreatesNewEntity()
        {
            // ARRANGE

            this.ersvc // expects entity creation
                .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
                .Returns((Action<Entity> a) => Task.FromResult(Entity.Factory.CreateNew(a)));

            this.ersvc // validate entity test name
                .Setup(_ => _.ValidateEntity("e1"))
                .Returns(Task.FromResult(new ValidateEntityResult { NameIsNotUnique = false, NameIsNullOrEmpty = false }));

            var e1edit = this.vm.CreateNewEntity();
            e1edit.Name = "e1";
            e1edit.PrepareCommit.Execute();

            // ACT

            e1edit.Commit.Execute();

            // ASSERT
            // commit added entity to model and call entity service

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual("e1", e1edit.Name);
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.IsFalse(e1edit.HasErrors);
            Assert.AreEqual(0, e1edit.GetErrors("Name").Cast<string>().Count());
            Assert.AreEqual(1, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Items.Count());

            Assert.AreSame(this.vm.Items.First(), this.vm.Entities.First());
            Assert.AreEqual("e1", this.vm.Entities.First().Name);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()), Times.Once);
            this.ersvc.Verify(_ => _.ValidateEntity(It.IsAny<string>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntity"), TestCategory("ValidateEntity")]
        public void CommitEditNewEntityViewModelTwiceCreatesOneNewEntity()
        {
            // ARRANGE

            this.ersvc // expects entity creation
                .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
                .Returns((Action<Entity> a) => Task.FromResult(Entity.Factory.CreateNew(a)));

            this.ersvc // validate entity test name
               .Setup(_ => _.ValidateEntity("e1"))
               .Returns(Task.FromResult(new ValidateEntityResult { NameIsNotUnique = false, NameIsNullOrEmpty = false }));

            var e1edit = this.vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.PrepareCommit.Execute();
            e1edit.Commit.Execute();

            // ACT

            e1edit.Commit.Execute();

            // ASSERT
            // commit adde entity to model and call entity service

            Assert.AreEqual("e1", e1edit.Name);
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.IsFalse(e1edit.HasErrors);
            Assert.AreEqual(0, e1edit.GetErrors("Name").Cast<string>().Count());

            Assert.AreEqual(1, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Items.Count);

            Assert.AreSame(this.vm.Items.First(), this.vm.Entities.First());
            Assert.AreEqual("e1", this.vm.Entities.First().Name);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()), Times.Once);
            this.ersvc.Verify(_ => _.ValidateEntity(It.IsAny<string>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewEntity > Modify > Rollback

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void RollbackEditNewEntityViewModelInitializesAgain()
        {
            // ARRANGE

            var e1edit = this.vm.CreateNewEntity();
            e1edit.Name = "e1";

            // ACT

            e1edit.Rollback.Execute();

            // ASSERT
            // rollback sets name back

            Assert.AreEqual(KosmoGraph.Desktop.ViewModel.Properties.Resources.EditNewEntityViewModelNameDefault, e1edit.Name);
            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.IsFalse(e1edit.HasErrors);
            Assert.AreEqual(0, e1edit.GetErrors("Name").Cast<string>().Count());

            Assert.AreEqual(0, this.vm.Entities.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewEntity > Modify > Rollback > PrepareCommit > Commit

        [TestMethod]
        [TestCategory("CreateNewEntity"), TestCategory("ValidateEntity")]
        public void RollbackEditNewEntityViewModelAllowsEditingAgainTillCommit()
        {
            // ARRANGE

            this.ersvc // expects entity creation
                .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
                .Returns((Action<Entity> a) => Task.FromResult(Entity.Factory.CreateNew(a)));

            this.ersvc // validate entity test name
                .Setup(_ => _.ValidateEntity("e1"))
                .Returns(Task.FromResult(new ValidateEntityResult { NameIsNotUnique = false, NameIsNullOrEmpty = false }));

            var e1edit = this.vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.Rollback.Execute();

            // ACT

            e1edit.Name = "e1";
            e1edit.PrepareCommit.Execute();
            e1edit.Commit.Execute();

            // ASSERT
            // commit adde entity to model and call entity service

            Assert.AreEqual("e1", e1edit.Name);
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.IsFalse(e1edit.HasErrors);
            Assert.AreEqual(0, e1edit.GetErrors("Name").Cast<string>().Count());

            Assert.AreEqual(1, this.vm.Entities.Count());
            Assert.AreEqual(1, this.vm.Items.Count);

            Assert.AreSame(this.vm.Items.First(), this.vm.Entities.First());
            Assert.AreEqual("e1", this.vm.Entities.First().Name);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()), Times.Once);
            this.ersvc.Verify(_ => _.ValidateEntity(It.IsAny<string>()), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            
        }

        #endregion

        //[TestMethod]
        //[TestCategory("CreateNewEntity")]
        //public void InitializeEntityViewModelWithVisibleTagsFromModel()
        //{
        //    // ARRANGE

        //    var ersvc = new Mock<IManageEntitiesAndRelationships>();
        //    ersvc // expects entity creation
        //        .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
        //        .Returns((Action<Entity> a) => Entity.Factory.CreateNew(a));


        //    var this.fsvc. = new Mock<IManageFacets>();
        //    this.fsvc. // expects Facet creation
        //        .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
        //        .Returns((Action<Facet> a) => Facet.Factory.CreateNew(a));

        //    var vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        //    var f1 = this.vm.Add(this.vm.CreateNewFacet("f1"));

        //    f1.IsVisible = true;

        //    // ACT

        //    EntityViewModel e1 = this.vm.CreateNewEntity("e1");

        //    // ASSERT
        //    // adding an e1 to the model lets it show up in Items list and Entities list
        //    // it is initialize with the first available Tag

        //    Assert.IsTrue(e1.IsVisible);
        //    Assert.AreEqual(1, e1.AssignedTags.Count());
        //    Assert.AreEqual(f1, e1.AssignedTags.First().Tag);
        //    Assert.AreEqual(1, e1.ModelItem.AssignedFacets.Count());
        //    Assert.AreEqual(f1.ModelItem.Id, e1.ModelItem.AssignedFacets.First().FacetId);

        //    this.ersvc.VerifyAll();
        //    this.fsvc.VerifyAll();
        //}
    }
}
