namespace Kosmograph.Desktop.Test.ViewModel
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

    [TestClass]
    public class CreateNewEntityViewModelTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        #region CreateNewEntity

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void CreateEditNewEntityViewModel()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(Enumerable.Empty<Entity>()));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            
            // ACT

            EditNewEntityViewModel e1edit = vm.CreateNewEntity();
            
            // ASSERT
            // edit is initialized but cant commit

            Assert.AreEqual(KosmoGraph.Desktop.ViewModel.Properties.Resources.EditNewEntityViewModelNameDefault, e1edit.Name);
            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.AreEqual(0, vm.Entities.Count());
            Assert.AreEqual(0, vm.Items.Count);
            
            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        #endregion 

        #region CreateNewEntity > Modify

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void ModifyNameAllowsEditNewEntityViewModelCommit()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(Enumerable.Empty<Entity>()));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            // ACT

            e1edit.Name = "e1";

            // ASSERT
            // valid name make edit committable

            Assert.AreEqual("e1", e1edit.Name);
            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.AreEqual(0, vm.Entities.Count());
            Assert.AreEqual(0, vm.Items.Count);

            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        #endregion 

        #region CreateNewEntity > Modify > Commit

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void CommitEditNewEntityViewModelCreatesNewEntity()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(Enumerable.Empty<Entity>()));

            ersvc // expects entity creation
                .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
                .Returns((Action<Entity> a) => Task.FromResult(EntityFactory.CreateNew(a)));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();
            e1edit.Name = "e1";

            // ACT

            e1edit.Commit.Execute();
            
            // ASSERT
            // commit added entity to model and call entity service

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual("e1", e1edit.Name);
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            
            Assert.AreEqual(1, vm.Entities.Count());
            Assert.AreEqual(1, vm.Items.Count());

            Assert.AreSame(vm.Items.First(), vm.Entities.First());
            Assert.AreEqual("e1", vm.Entities.First().Name);
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()), Times.Once);
            fsvc.VerifyAll();
        }

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void CommitEditNewEntityViewModelTwiceCreatesOneNewEntity()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(Enumerable.Empty<Entity>()));

            ersvc // expects entity creation
                .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
                .Returns((Action<Entity> a) => Task.FromResult(EntityFactory.CreateNew(a)));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.Commit.Execute();
            
            // ACT

            e1edit.Commit.Execute();

            // ASSERT
            // commit adde entity to model and call entity service

            Assert.AreEqual("e1", e1edit.Name);
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual(1, vm.Entities.Count());
            Assert.AreEqual(1, vm.Items.Count);

            Assert.AreSame(vm.Items.First(), vm.Entities.First());
            Assert.AreEqual("e1", vm.Entities.First().Name);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()), Times.Once);
            fsvc.VerifyAll();
        }

        #endregion

        #region CreateNewEntity > Modify > Rollback

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void RollbackEditNewEntityViewModelInitializesAgain()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            
            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(Enumerable.Empty<Entity>()));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();
            e1edit.Name = "e1";

            // ACT

            e1edit.Rollback.Execute();

            // ASSERT
            // rollback sets name back

            Assert.AreEqual(KosmoGraph.Desktop.ViewModel.Properties.Resources.EditNewEntityViewModelNameDefault, e1edit.Name);
            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.AreEqual(0, vm.Entities.Count());
            Assert.AreEqual(0, vm.Items.Count);

            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        #endregion

        #region CreateNewEntity > Modify > Rollback > Commit

        [TestMethod]
        [TestCategory("CreateNewEntity")]
        public void RollbackEditNewEntityViewModelAllowsEditingAgainTillCommit()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(Enumerable.Empty<Entity>()));

            ersvc // expects entity creation
              .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
              .Returns((Action<Entity> a) => Task.FromResult(EntityFactory.CreateNew(a)));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();
            e1edit.Name = "e1";
            e1edit.Rollback.Execute();
            
            // ACT

            e1edit.Name = "e1";
            e1edit.Commit.Execute();

            // ASSERT
            // commit adde entity to model and call entity service

            Assert.AreEqual("e1", e1edit.Name);
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual(1, vm.Entities.Count());
            Assert.AreEqual(1, vm.Items.Count);

            Assert.AreSame(vm.Items.First(), vm.Entities.First());
            Assert.AreEqual("e1", vm.Entities.First().Name);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()), Times.Once);
            fsvc.VerifyAll();
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
        //        .Returns((Action<Entity> a) => EntityFactory.CreateNew(a));


        //    var fsvc = new Mock<IManageFacets>();
        //    fsvc // expects Facet creation
        //        .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
        //        .Returns((Action<Facet> a) => FacetFactory.CreateNew(a));

        //    var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
        //    var f1 = vm.Add(vm.CreateNewFacet("f1"));

        //    f1.IsVisible = true;

        //    // ACT

        //    EntityViewModel e1 = vm.CreateNewEntity("e1");

        //    // ASSERT
        //    // adding an e1 to the model lets it show up in Items list and Entities list
        //    // it is initialize with the first available Tag

        //    Assert.IsTrue(e1.IsVisible);
        //    Assert.AreEqual(1, e1.AssignedTags.Count());
        //    Assert.AreEqual(f1, e1.AssignedTags.First().Tag);
        //    Assert.AreEqual(1, e1.ModelItem.AssignedFacets.Count());
        //    Assert.AreEqual(f1.ModelItem.Id, e1.ModelItem.AssignedFacets.First().FacetId);

        //    ersvc.VerifyAll();
        //    fsvc.VerifyAll();
        //}
    }
}
