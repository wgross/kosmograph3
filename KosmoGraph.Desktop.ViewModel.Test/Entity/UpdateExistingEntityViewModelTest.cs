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

    [TestClass]
    public sealed class UpdateExistingEntityViewModelTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        #region UpdateExistingEntity

        [TestMethod]
        [TestCategory("EditEntity")]
        public void UpdateExistingEmptyEntityViewModel()
        {
            // ARRANGE

            var entities = new[]
            {
                EntityFactory.CreateNew(e => e.Name ="e1")
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            
            // ACT

            EditExistingEntityViewModel e1edit = vm.EditEntity(vm.Entities.Single());
            
            // ASSERT
            // edit is initialized but cant commit because it is not changed

            Assert.AreEqual("e1", e1edit.Name);
            Assert.AreSame(entities.Single(), e1edit.Edited.ModelItem);
            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.AreEqual(1, vm.Entities.Count());
            Assert.AreEqual(1, vm.Items.Count);
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
        }

        #endregion 

        #region UpdateExistingEntity > Modify

        [TestMethod]
        [TestCategory("EditEntity")]
        public void ModifyNameAllowsEditExistingEntityViewModelCommit()
        {
            // ARRANGE

            var entities = new[]
            {
                EntityFactory.CreateNew(e => e.Name ="e1")
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.Single());

            // ACT

            e1edit.Name = "e1-changed";

            // ASSERT
            // new name makes edit committable

            Assert.AreEqual("e1-changed", e1edit.Name);
            Assert.AreSame(entities.Single(), e1edit.Edited.ModelItem);
            Assert.AreEqual("e1", entities.Single().Name);
            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());
            Assert.AreEqual(1, vm.Entities.Count());
            Assert.AreEqual(1, vm.Items.Count);
            
            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        [TestMethod]
        [TestCategory("EditEntity")]
        public void ModifyPropertyValueAllowsEditExistigEntityViewModelCommit()
        {
            // ARRANGE

            var facets = new []
            {
                FacetFactory.CreateNew(f =>
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            var fsvc = new Mock<IManageFacets>();
            
            fsvc // expects retrieval of all Facets
                .Setup(_=>_.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name ="e1";
                    e.Add(e.CreateNewAssignedFacet(facets.Single(), f=>f.Properties.Single().Value = "pv1"));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.Single());

            // ACT

            e1edit.Properties.Single().Value = "pv1-changed";

            // ASSERT
            // new name makes edit committable

            Assert.AreSame(entities.Single(), e1edit.Edited.ModelItem);
            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            Assert.AreEqual("pv1-changed", e1edit.Properties.Single().Value);
            Assert.AreEqual(0, e1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, vm.Entities.Count());
            Assert.AreEqual(1, vm.Entities.Single().AssignedFacets.Count());
            Assert.AreEqual(1, vm.Entities.Single().AssignedFacets.Single().Properties.Count());
            Assert.AreEqual("pv1", vm.Entities.Single().AssignedFacets.Single().Properties.Single().Value);
            Assert.AreEqual(1, vm.Items.Count);

            Assert.AreEqual("pv1", entities.Single().AssignedFacets.Single().Properties.Single().Value);
            
            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        #endregion 

        #region UpdateExistingEntity > Modify > Commit

        [TestMethod]
        [TestCategory("EditEntity")]
        public void CommitEditExistingEntityViewModelUpdatesEntity()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name="f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name ="pd1"));
                })
            };

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriaval of all facets
                .Setup(_=>_.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name ="e1";
                    e.Add(e.CreateNewAssignedFacet(facets.Single(), af => af.Properties.Single().Value = "pv1"));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expects update of e1
                .Setup(_ => _.UpdateEntity(entities.Single()))
                .Returns((Entity e) => Task.FromResult(e));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.Single());
            
            e1edit.Name = "e1-changed";
            e1edit.Properties.Single().Value = "pv1-changed";
            
            // ACT

            e1edit.Commit.Execute();
            
            // ASSERT
            // commit added entity to model and call entity service

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual("e1-changed", e1edit.Name);
            Assert.AreSame(entities.Single(), e1edit.Edited.ModelItem);
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            
            Assert.AreEqual(1, vm.Entities.Count());
            Assert.AreEqual(1, vm.Items.Count());
            Assert.AreEqual("e1-changed", vm.Entities.Single().Name);

            Assert.AreEqual("e1-changed", entities.Single().Name);
            Assert.AreEqual("pv1-changed", entities.Single().AssignedFacets.Single().Properties.Single().Value);
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.UpdateEntity(It.IsAny<Entity>()), Times.Once);
            fsvc.VerifyAll();
        }

        #endregion

        #region UpdateExistingEntity > Modify > Rollback

        [TestMethod]
        [TestCategory("EditEntity")]
        public void RollbackEditExistingEntityViewModelInitializesAgain()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name="f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name ="pd1"));
                })
            };

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriaval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name ="e1";
                    e.Add(e.CreateNewAssignedFacet(facets.Single(), af => af.Properties.Single().Value = "pv1"));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.Single());
            
            e1edit.Name = "e1-changed";
            e1edit.Properties.Single().Value = "pv1-changed";

            // ACT

            e1edit.Rollback.Execute();

            // ASSERT
            // rollback sets name back

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual("e1", e1edit.Name);
            Assert.AreSame(entities.Single(), e1edit.Edited.ModelItem);
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());

            Assert.AreEqual(1, vm.Entities.Count());
            Assert.AreEqual(1, vm.Items.Count());
            Assert.AreEqual("e1", vm.Entities.Single().Name);

            Assert.AreEqual("e1", entities.Single().Name);
            Assert.AreEqual("pv1", entities.Single().AssignedFacets.Single().Properties.Single().Value);
            
            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        #endregion

        #region UpdateExistingEntity > Modify > Rollback > Commit

        [TestMethod]
        [TestCategory("EditEntity")]
        public void RollbackEditExistingEntityViewModelAllowsEditingAgainTillCommit()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name="f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name ="pd1"));
                })
            };

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriaval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name ="e1";
                    e.Add(e.CreateNewAssignedFacet(facets.Single(), af => af.Properties.Single().Value = "pv1"));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expects update of e1
                .Setup(_ => _.UpdateEntity(entities.Single()))
                .Returns((Entity e) => Task.FromResult(e));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.Single());

            e1edit.Name = "e1-changed";
            e1edit.Properties.Single().Value = "pv1-changed";
            e1edit.Rollback.Execute();

            // ACT

            e1edit.Name = "e1-changed";
            e1edit.Properties.Single().Value = "pv1-changed";
            e1edit.Commit.Execute();

            // ASSERT
            // commit added entity to model and call entity service

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.AreEqual("e1-changed", e1edit.Name);
            Assert.AreSame(entities.Single(), e1edit.Edited.ModelItem);
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());

            Assert.AreEqual(1, vm.Entities.Count());
            Assert.AreEqual(1, vm.Items.Count());
            Assert.AreEqual("e1-changed", vm.Entities.Single().Name);

            Assert.AreEqual("e1-changed", entities.Single().Name);
            Assert.AreEqual("pv1-changed", entities.Single().AssignedFacets.Single().Properties.Single().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.UpdateEntity(It.IsAny<Entity>()), Times.Once);
            fsvc.VerifyAll();
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
        //    Assert.AreEqual(f1, e1.AssignedTags.Single().Tag);
        //    Assert.AreEqual(1, e1.ModelItem.AssignedFacets.Count());
        //    Assert.AreEqual(f1.ModelItem.Id, e1.ModelItem.AssignedFacets.Single().FacetId);

        //    ersvc.VerifyAll();
        //    fsvc.VerifyAll();
        //}
    }
}
