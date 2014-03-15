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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;

    [TestClass]
    public class CreateNewRelationshipViewModelTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());
        }

        #region CreateNewRelationship

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void CreateEditNewRelationshipViewModel()
        {
            // ARRANGE

            var entities = new[] 
            {
                Entity.Factory.CreateNew(e=>e.Name = "e1"),
                Entity.Factory.CreateNew(e=>e.Name = "e2"),
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrueval aof all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);

            // ACT
            // create relationship with currently visible tags

            EditNewRelationshipViewModel r1edit = vm.CreateNewRelationship(e1, e2);

            // ASSERT
            // relatinship is initialized and can be committed

            Assert.AreEqual(string.Format(KosmoGraph.Desktop.ViewModel.Properties.Resources.EditNewRelationshipViewModelTitle, e1.Name, e2.Name), r1edit.Title);
            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreSame(e1, r1edit.From);
            Assert.AreSame(e2, r1edit.To);
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(0, vm.Relationships.Count());
            Assert.AreEqual(0, vm.Items.OfType<RelationshipViewModel>().Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        #endregion

        #region CreateNewRelationship > Modify : Empty

        #endregion

        #region CreateNewRelationship > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void CommitEditNewRelationshipViewModelCreatesNewRelationship()
        {
            // ARRANGE

            var entities = new[] 
            {
                Entity.Factory.CreateNew(e=>e.Name = "e1"),
                Entity.Factory.CreateNew(e=>e.Name = "e2"),
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrueval aof all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);

            ersvc // expect creation of partial relationship with source entity
                .Setup(_ => _.CreatePartialRelationship(e1.ModelItem, It.IsAny<Action<Relationship>>()))
                .Returns<Entity,Action<Relationship>>((e, a) => Relationship.Factory.CreateNewPartial(e));

            ersvc
                .Setup(_ => _.CompletePartialRelationship(It.Is<Relationship>(r => r.FromId == e1.ModelItem.Id), e2.ModelItem))
                .Returns<Relationship,Entity>((r,e) => Task.FromResult(new CompletePartialRelationshipResult(r, e1.ModelItem, e)));

            var r1edit = vm.CreateNewRelationship(e1, e2);

            // ACT

            r1edit.Commit.Execute();

            // ASSERT
            // commit new relationship to database

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreSame(e1, r1edit.From);
            Assert.AreSame(e2, r1edit.To);
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(1, vm.Relationships.Count());
            Assert.AreEqual(1, vm.Items.OfType<RelationshipViewModel>().Count());
            Assert.AreSame(vm.Relationships.First(), vm.Items.OfType<RelationshipViewModel>().First());
            Assert.AreSame(e1, vm.Relationships.First().From.Entity);
            Assert.AreSame(e2, vm.Relationships.First().To.Entity);

            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void CommitEditNewRelationshipViewModelTwiceCreatesOneNewRelationship()
        {
            // ARRANGE

            var entities = new[] 
            {
                Entity.Factory.CreateNew(e=>e.Name = "e1"),
                Entity.Factory.CreateNew(e=>e.Name = "e2"),
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrueval aof all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);

            ersvc // expect creation of partial relationship with source entity
                .Setup(_ => _.CreatePartialRelationship(e1.ModelItem, It.IsAny<Action<Relationship>>()))
                .Returns<Entity, Action<Relationship>>((e, a) => Relationship.Factory.CreateNewPartial(e));

            ersvc
                .Setup(_ => _.CompletePartialRelationship(It.Is<Relationship>(r => r.FromId == e1.ModelItem.Id), e2.ModelItem))
                .Returns<Relationship,Entity>((r,e) => Task.FromResult(new CompletePartialRelationshipResult(r,e1.ModelItem,e)));

            var r1edit = vm.CreateNewRelationship(e1, e2);

            r1edit.Commit.Execute();

            // ACT

            r1edit.Commit.Execute();

            // ASSERT
            // commit new relationship to database


            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreSame(e1, r1edit.From);
            Assert.AreSame(e2, r1edit.To);
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(1, vm.Relationships.Count());
            Assert.AreEqual(1, vm.Items.OfType<RelationshipViewModel>().Count());
            Assert.AreSame(vm.Relationships.First(), vm.Items.OfType<RelationshipViewModel>().First());
            Assert.AreSame(e1, vm.Relationships.First().From.Entity);
            Assert.AreSame(e2, vm.Relationships.First().To.Entity);

            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        #endregion

        #region CreateNewRelationship > Rollback

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void RollbackEditNewRelationshipViewModelInitializesAgain()
        {
            // ARRANGE

            var entities = new[] 
            {
                Entity.Factory.CreateNew(e=>e.Name = "e1"),
                Entity.Factory.CreateNew(e=>e.Name = "e2"),
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrueval aof all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);
            var r1edit = vm.CreateNewRelationship(e1, e2);

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT
            // rollback does basicall nothing, bu doesn't throw either

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreSame(e1, r1edit.From);
            Assert.AreSame(e2, r1edit.To);
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(0, vm.Relationships.Count());
            Assert.AreEqual(0, vm.Items.OfType<RelationshipViewModel>().Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        #endregion

        #region CreateNewRelationship > Rollback > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void RollbackEditNewRelationshipViewModelAllowsEditingAgainTillCommit()
        {
            // ARRANGE

            var entities = new[] 
            {
                Entity.Factory.CreateNew(e=>e.Name = "e1"),
                Entity.Factory.CreateNew(e=>e.Name = "e2"),
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval aof all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);

            ersvc // expect creation of partial relationship with source entity
                .Setup(_ => _.CreatePartialRelationship(e1.ModelItem, It.IsAny<Action<Relationship>>()))
                .Returns<Entity, Action<Relationship>>((e, a) => Relationship.Factory.CreateNewPartial(e));

            ersvc
                .Setup(_ => _.CompletePartialRelationship(It.Is<Relationship>(r => r.FromId == e1.ModelItem.Id), e2.ModelItem))
                .Returns<Relationship,Entity>((r,e)=>Task.FromResult(new CompletePartialRelationshipResult(r, e1.ModelItem, e))); 

            var r1edit = vm.CreateNewRelationship(e1, e2);

            // ACT

            r1edit.Rollback.Execute();
            r1edit.Commit.Execute();

            // ASSERT
            // commit new relationship to database

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreSame(e1, r1edit.From);
            Assert.AreSame(e2, r1edit.To);
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(1, vm.Relationships.Count());
            Assert.AreEqual(1, vm.Items.OfType<RelationshipViewModel>().Count());
            Assert.AreSame(vm.Relationships.First(), vm.Items.OfType<RelationshipViewModel>().First());
            Assert.AreSame(e1, vm.Relationships.First().From.Entity);
            Assert.AreSame(e2, vm.Relationships.First().To.Entity);

            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        #endregion

        //[TestMethod]
        //public void AddRelationshipForAddedEntities()
        //{
        //    // ARRANGE

        //    var vm = EntityRelationshipViewModel.CreateNew();
        //    var e1 = vm.Add(vm.CreateNewEntity("e1"));
        //    var e2 = vm.Add(vm.CreateNewEntity("e2"));

        //    // ACT
        //    // add relationship to connect tow entities

        //    RelationshipViewModel r1 = vm.Add(vm.CreateNewRelationship(e1, e2));

        //    // ASSERT

        //    Assert.IsFalse(r1.IsVisible);
        //    Assert.AreEqual(3, vm.Items.Count);
        //    Assert.AreEqual(1, vm.Items.OfType<RelationshipViewModel>().Count());
        //    Assert.AreEqual(1, vm.Relationships.Count());
        //    //Assert.AreEqual(1,vm.ModelStore.Relationship.Query().Count());
        //    //Assert.AreSame(r1.ModelItem,vm.ModelStore.Relationship.Query().First());
        //    Assert.AreNotEqual(Guid.Empty, r1.ModelItem.Identity);
        //    Assert.AreEqual(e1, r1.From.Entity);
        //    Assert.AreEqual(e2, r1.To.Entity);
        //    Assert.AreEqual(e1.ModelItem.Id, r1.ModelItem.FromId);
        //    Assert.AreEqual(e2.ModelItem.Id, r1.ModelItem.ToId);
        //}

        //[TestMethod]
        //public void AddRelationshipAndItsEntities()
        //{
        //    // ARRANGE

        //    var vm = EntityRelationshipViewModel.CreateNew();
        //    var e1 = vm.CreateNewEntity("e1");
        //    var e2 = vm.CreateNewEntity("e2");

        //    // ACT
        //    // add relationship to connect tow entities

        //    RelationshipViewModel r1 = vm.Add(vm.CreateNewRelationship(e1, e2));

        //    // ASSERT

        //    Assert.IsFalse(r1.IsVisible);
        //    Assert.AreEqual(3, vm.Items.Count);
        //    Assert.AreEqual(2, vm.Items.OfType<EntityViewModel>().Count());
        //    Assert.AreEqual(1, vm.Items.OfType<RelationshipViewModel>().Count());
        //    Assert.AreEqual(1, vm.Relationships.Count());
        //    Assert.AreEqual(2, vm.Entities.Count());
        //    Assert.AreEqual(e1, r1.From.Entity);
        //    Assert.AreEqual(e2, r1.To.Entity);
        //    Assert.AreNotEqual(Guid.Empty, r1.ModelItem.Identity);
        //    Assert.AreEqual(e1, r1.From.Entity);
        //    Assert.AreEqual(e2, r1.To.Entity);
        //    Assert.AreEqual(e1.ModelItem.Id, r1.ModelItem.FromId);
        //    Assert.AreEqual(e2.ModelItem.Id, r1.ModelItem.ToId);
        //}

        //[TestMethod]
        //public void DontAddWithoutEntities()
        //{
        //    // ARRANGE

        //    var vm = EntityRelationshipViewModel.CreateNew();
        //    var a = vm.CreateNewEntity("e1");

        //    // ACT & ASSERT
        //    // e1 null reference as source or destination causes e1 NullReferenceException

        //    ExceptionAssert.Throws<NullReferenceException>(delegate { vm.CreateNewRelationship(null, null); });
        //    ExceptionAssert.Throws<NullReferenceException>(delegate { vm.CreateNewRelationship(a, null); });
        //    ExceptionAssert.Throws<NullReferenceException>(delegate { vm.CreateNewRelationship(null, a); });
        //}

        //[TestMethod]
        //public void DontAddSameRelationshipTwice()
        //{
        //    // ARRANGE

        //    var vm = EntityRelationshipViewModel.CreateNew();
        //    var e1 = vm.CreateNewEntity("e1");
        //    var e2 = vm.CreateNewEntity("e2");
        //    var r = vm.CreateNewRelationship(e1, e2);

        //    // ACT
        //    // add the same relationship twice doesnt add two relationships to the model

        //    vm.Add(r);
        //    var added = vm.Add(r);

        //    // ASSERT
        //    Assert.AreSame(r, added);
        //    Assert.AreEqual(3, vm.Items.Count);
        //}

        //[TestMethod]
        //public void DontAddEqualRelationshipTwice()
        //{
        //    // ARRANGE

        //    var vm = EntityRelationshipViewModel.CreateNew();
        //    var a = vm.CreateNewEntity("e1");
        //    var b = vm.CreateNewEntity("e2");
        //    var r1 = vm.CreateNewRelationship(a, b);
        //    var r2 = vm.CreateNewRelationship(a, b);

        //    // ACT
        //    // adding e1 second relationship with same entities to the model doens add e1 second relationship

        //    vm.Add(r1);
        //    var added = vm.Add(r2);

        //    // ASSERT

        //    Assert.AreSame(r1, added);
        //    Assert.AreEqual(3, vm.Items.Count);
        //    Assert.AreEqual(a, vm.Items[0]);
        //    Assert.AreEqual(b, vm.Items[1]);
        //    Assert.AreEqual(r1, vm.Items[2]);
        //}

        //[TestMethod]
        //public void DontAddInvertedRelationship()
        //{
        //    // ARRANGE

        //    var vm = EntityRelationshipViewModel.CreateNew();
        //    var a = vm.CreateNewEntity("e1");
        //    var b = vm.CreateNewEntity("e2");
        //    var r1 = vm.CreateNewRelationship(a, b);
        //    var r2 = vm.CreateNewRelationship(b, a);

        //    // ACT
        //    // adding e1 second relationship with same entities to the model doens add e1 second relationship

        //    vm.Add(r1);
        //    var added = vm.Add(r2);

        //    // ASSERT

        //    Assert.AreSame(r1, added);
        //    Assert.AreEqual(3, vm.Items.Count);
        //    Assert.AreEqual(a, vm.Items[0]);
        //    Assert.AreEqual(b, vm.Items[1]);
        //    Assert.AreEqual(r1, vm.Items[2]);
        //}

        //[TestMethod]
        //public void DontAddRelationshipWithEqualSourceAndDestination()
        //{
        //    // ARRANGE

        //    var vm = EntityRelationshipViewModel.CreateNew();
        //    var a = vm.CreateNewEntity("e1");

        //    // ACT & ASSERT
        //    // creating e1 relationship with teh same e1 as source and destination causes an InvalidOperationException

        //    ExceptionAssert.Throws<InvalidOperationException>(delegate { vm.CreateNewRelationship(a, a); });
        //}
    }
}
