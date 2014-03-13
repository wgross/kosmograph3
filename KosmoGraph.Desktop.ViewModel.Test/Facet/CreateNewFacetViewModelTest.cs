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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;

    [TestClass]
    public class CreateNewFacetViewModelTest
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

            this.facets = Enumerable.Empty<Facet>();

            this.fsvc = new Mock<IManageFacets>();
            
            this.fsvc // expect retrieval of all facets
               .Setup(_ => _.GetAllFacets())
               .Returns(Task.FromResult(this.facets));

            this.entities = Enumerable.Empty<Entity>();
            
            this.relationships = Enumerable.Empty<Relationship>();
            
            this.ersvc = new Mock<IManageEntitiesAndRelationships>();
            
            this.ersvc // expect retrueval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));
            
            this.ersvc
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        #region CreateNewFacet

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void CreateEditNewFacetViewModel()
        {
            // ARRANGE

         
            // ACT
            
            EditNewFacetViewModel f1edit = this.vm.CreateNewFacet();

            // ASSERT
            // new fact is member of Facets list and Items list

            Assert.AreEqual(KosmoGraph.Desktop.ViewModel.Properties.Resources.EditNewFacetViewModelNameDefault, f1edit.Name);
            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.AddPropertyDefinition.CanExecute());
            Assert.IsFalse(f1edit.RemovePropertyDefinition.CanExecute(null));
            Assert.AreEqual(0, f1edit.Properties.Count());
            Assert.AreEqual(0, this.vm.Facets.Count());
            Assert.AreEqual(0, this.vm.Items.Count);
            
            this.ersvc.VerifyAll();
            this.fsvc.VerifyAll();
        }

        #endregion

        #region CreateNewFacet > Modify

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void ModifyNameAllowsEditNewFacetViewModelCommit()
        {
            // ARRANGE

            var f1edit = vm.CreateNewFacet();

            // ACT

            f1edit.Name = "f1";
            
            // ASSERT
            // new fact is member of Facets list and Items list

            Assert.AreEqual("f1", f1edit.Name);
            Assert.IsTrue(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.Rollback.CanExecute());
            Assert.IsTrue(f1edit.AddPropertyDefinition.CanExecute());
            Assert.IsFalse(f1edit.RemovePropertyDefinition.CanExecute(null));
            Assert.AreEqual(0, f1edit.Properties.Count());
            Assert.AreEqual(0, this.vm.Facets.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            this.ersvc.VerifyAll();
            this.fsvc.VerifyAll();
        }

        [TestMethod]
        [TestCategory("CreateNewFacet"),TestCategory("RemovePropertyDefinition")]
        public void RemoveNewPropertyDefinitionFromNewFacet()
        {
            // ARRANGE

            var f1edit = vm.CreateNewFacet();

            f1edit.Name = "f1";
            f1edit.AddPropertyDefinition.Execute();

            // ACT

            f1edit.RemovePropertyDefinition.Execute(f1edit.Properties.First());

            // ASSERT
            // Facet has new property definition, this allows commit

            Assert.AreEqual("f1", f1edit.Name);
            Assert.AreEqual(0, f1edit.Properties.Count());
            
            Assert.IsTrue(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.Rollback.CanExecute());
            Assert.IsTrue(f1edit.AddPropertyDefinition.CanExecute());
            Assert.IsFalse(f1edit.RemovePropertyDefinition.CanExecute(null));

            Assert.AreEqual(0, f1edit.Properties.Count());
            Assert.AreEqual(0, this.vm.Facets.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            this.ersvc.VerifyAll();
            this.fsvc.VerifyAll();
        }

        #endregion 

        #region CreateNewFacet > Modify > Commit

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void CommitEditNewFacetViewModelWithNameCreatesNewFacet()
        {
            // ARRANGE

            this.fsvc // expect facet creation
                .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
                .Returns((Action<Facet> a) => Task.FromResult(FacetFactory.CreateNew(a)));

            var f1edit = vm.CreateNewFacet();

            f1edit.Name = "f1";
           
            // ACT

            f1edit.Commit.Execute();

            TestDispatcher.DoEvents();
            
            // ASSERT
            // commit createx a new facet and adds it to the main view model

            Assert.AreEqual("f1", f1edit.Name);
            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.AreEqual(0, f1edit.Properties.Count());
            
            Assert.AreEqual(1, this.vm.Facets.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            Assert.AreEqual("f1", this.vm.Facets.First().Name);
            
            this.ersvc.VerifyAll();
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()), Times.Once);
        }

       
        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void CommitEditNewFacetViewModelTwiceCreatesOneNewFacet()
        {
            // ARRANGE

            fsvc // expect facet creation
                .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
                .Returns((Action<Facet> a) => Task.FromResult(FacetFactory.CreateNew(a)));

            var f1edit = this.vm.CreateNewFacet();
            f1edit.Name = "f1";
            f1edit.Commit.Execute();

            TestDispatcher.DoEvents();
            
            // ACT

            f1edit.Commit.Execute();

            TestDispatcher.DoEvents();
           
            // ASSERT
            // commit twice creatze no new facet

            Assert.AreEqual("f1", f1edit.Name);
            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.AreEqual(0, f1edit.Properties.Count());
            
            Assert.AreEqual(1, this.vm.Facets.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            Assert.AreEqual("f1", this.vm.Facets.First().Name);
            
            this.ersvc.VerifyAll();
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()), Times.Once);
        }

        #endregion

        #region CreateNewFacet > Modify > Rollback

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void RollbackEditNewFacetViewModelWithNameInitializesAgain()
        {
            // ARRANGE

             var f1edit = vm.CreateNewFacet();
            
            f1edit.Name = "f1";
            
            // ACT

            f1edit.Rollback.Execute();

            // ASSERT

            Assert.AreEqual(KosmoGraph.Desktop.ViewModel.Properties.Resources.EditNewFacetViewModelNameDefault, f1edit.Name);
            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.AreEqual(0, f1edit.Properties.Count());
            Assert.AreEqual(0, this.vm.Facets.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            this.ersvc.VerifyAll();
            this.fsvc.VerifyAll();
        }

        #endregion 

        #region CreateNewEntity > Modify > Rollback > Commit

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void RollbackEditNewFacetViewModelWithNameAllowsEditingAgainTillCommit()
        {
            // ARRANGE

            fsvc // expect facet creation
                .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
                .Returns((Action<Facet> a) => Task.FromResult(FacetFactory.CreateNew(a)));

            var f1edit = vm.CreateNewFacet();

            f1edit.Name = "f1";
            f1edit.Rollback.Execute();

            // ACT

            f1edit.Name = "f1";
            f1edit.Commit.Execute();

            TestDispatcher.DoEvents();

            // ASSERT

            Assert.AreEqual("f1", f1edit.Name);
            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.AreEqual(0, f1edit.Properties.Count());

            Assert.AreEqual(1, this.vm.Facets.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            Assert.AreEqual("f1", this.vm.Facets.First().Name);

            this.ersvc.VerifyAll();
            this.fsvc.VerifyAll();
            fsvc.Verify(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()), Times.Once);
        }

        #endregion 

        //[TestMethod]
        //[TestCategory("CreateNewFacet")]
        //public void DontAddFacetViewModelTwice()
        //{
        //    // ARRANGE

        //    var ersvc = new Mock<IManageEntitiesAndRelationships>();
        //    var fsvc = new Mock<IManageFacets>();

        //    fsvc // expect facet creation
        //        .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
        //        .Returns((Action<Facet> a) => FacetFactory.CreateNew(a));

        //    var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
        //    var f1 = vm.CreateNewFacet("f1");
            
        //    // ACT
        //    // adding the f1 to the model second time doesn't create e1 second item in f1 list
            
        //    TagViewModel added1 = vm.Add(f1);
        //    TagViewModel added2 = vm.Add(f1);

        //    // ASSERT

        //    Assert.AreEqual(1, vm.Tags.Count());
        //    Assert.AreSame(added1, added2);
        //    Assert.AreEqual(f1, added1);
        //    Assert.AreEqual(f1, added2);

        //    this.ersvc.VerifyAll();
        //    this.fsvc.VerifyAll();
        //}

        //[TestMethod]
        //[TestCategory("CreateNewFacet")]
        //public void DontAddFacetVewModelWithDuplicateName()
        //{
        //     // ARRANGE

        //    var ersvc = new Mock<IManageEntitiesAndRelationships>();
        //    var fsvc = new Mock<IManageFacets>();

        //    fsvc // expect facet creation
        //        .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
        //        .Returns((Action<Facet> a) => FacetFactory.CreateNew(a));

        //    var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
        //    var f1 = vm.Add(vm.CreateNewFacet("f1"));
            
        //    // ACT & ASSERT
        //    // add facet with same name throws 

        //    ExceptionAssert.Throws<InvalidOperationException>(delegate { vm.Add(vm.CreateNewFacet("f1")); });

        //    this.ersvc.VerifyAll();
        //    this.fsvc.VerifyAll();
        //    fsvc.Verify(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()), Times.Once());
        //}

        //[TestMethod]
        //[TestCategory("CreateNewFacet")]
        //public void DontAddFacetViewModelFromForeignModel()
        //{
        //    // ARRANGE

        //    var ersvc = new Mock<IManageEntitiesAndRelationships>();
        //    var fsvc = new Mock<IManageFacets>();

        //    fsvc // expects facet creation
        //        .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
        //        .Returns((Action<Facet> a) => FacetFactory.CreateNew(a));

        //    var vm1 = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
        //    var vm2 = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
        //    var f1 = vm1.Add(vm1.CreateNewFacet("f1"));

        //    // ACT & ASSERT
        //    // adding an e1 twice doesn't result to two entities in the model

        //    ExceptionAssert.Throws<InvalidOperationException>(delegate { vm2.Add(f1); });
        //    Assert.AreEqual(0, vm2.Tags.Count());
        //    Assert.AreEqual(0, vm2.Items.Count);

        //    this.ersvc.VerifyAll();
        //    this.fsvc.VerifyAll();
        //}
    }
}
