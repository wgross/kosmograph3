namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using KosmoGraph.Model;
    using Moq;
    using System.Threading.Tasks;
    using KosmoGraph.Services;

    [TestClass]
    public class RemoveAssignedEntitFacetTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        #region RemoveAssignedFacet

        [TestMethod]
        [TestCategory("RemoveEntityFacet"),TestCategory("UpdateExistingEntity")]
        public void RemoveEmptyAssignedFacetFromEntityViewModel()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            var entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            
            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriavl of all facets
                .Setup(_=>_.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());
            
            // ACT

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual(1, vm.Entities.First().AssignedFacets.Count());
            Assert.AreEqual(0, vm.Entities.First().AssignedFacets.First().Properties.Count());
            Assert.AreSame(vm.Facets.First().ModelItem, vm.Entities.First().AssignedFacets.First().Facet.ModelItem);

            Assert.AreEqual(1, entities.First().AssignedFacets.Count());
            Assert.AreEqual(0, entities.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual(facets.First().Id, entities.First().AssignedFacets.First().FacetId);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("RemoveEntityFacet"), TestCategory("UpdateExistingEntity")]
        public void RemoveAssignedFacetWothPropertyFromEntityViewModel()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };

            var entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriavl of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            // ACT

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());

            // ASSERT
            
            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual(1, vm.Entities.First().AssignedFacets.Count());
            Assert.AreEqual(1, vm.Entities.First().AssignedFacets.First().Properties.Count());
            Assert.AreSame(vm.Facets.First().ModelItem, vm.Entities.First().AssignedFacets.First().Facet.ModelItem);

            Assert.AreEqual(1, entities.First().AssignedFacets.Count());
            Assert.AreEqual(1, entities.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual(facets.First().Id, entities.First().AssignedFacets.First().FacetId);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region RemoveAssignedFacet > Commit

        [TestMethod]
        [TestCategory("RemoveEntityFacet"), TestCategory("UpdateExistingEntity")]
        public void CommitRemovedEmptyAssignedFacetFromEntityViewModel()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            var entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect update of e1
                .Setup(_ => _.UpdateEntity(entities.First()))
                .Returns<Entity>(e => Task.FromResult(e));

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriavl of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());
            
            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());
            
            // ACT

            e1edit.Commit.Execute();

            // ASSERT

            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual(0, vm.Entities.First().AssignedFacets.Count());
            
            Assert.AreEqual(0, entities.First().AssignedFacets.Count());
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.UpdateEntity(entities.First()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);

        }

        [TestMethod]
        [TestCategory("RemoveEntityFacet"), TestCategory("UpdateExistingEntity")]
        public void CommitRemovedAssignedFacetWothPropertyFromEntityViewModel()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };

            var entities = new[]
            {
                EntityFactory.CreateNew(e =>
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect update of e1
                .Setup(_ => _.UpdateEntity(entities.First()))
                .Returns<Entity>(e => Task.FromResult(e));

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriavl of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());
            
            // ACT

            e1edit.Commit.Execute();

            // ASSERT

            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual(0, vm.Entities.First().AssignedFacets.Count());
            
            Assert.AreEqual(0, entities.First().AssignedFacets.Count());
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.UpdateEntity(entities.First()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region RemoveAssignedFacet > Rollback

        [TestMethod]
        [TestCategory("RemoveEntityFacet"), TestCategory("UpdateExistingEntity")]
        public void RollbackRemovedEmptyAssignedFacetFromEntityViewModel()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                })
            };

            var entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriavl of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());

            // ACT

            e1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual(1, vm.Entities.First().AssignedFacets.Count());
            Assert.AreEqual(0, vm.Entities.First().AssignedFacets.First().Properties.Count());
            Assert.AreSame(vm.Facets.First().ModelItem, vm.Entities.First().AssignedFacets.First().Facet.ModelItem);

            Assert.AreEqual(1, entities.First().AssignedFacets.Count());
            Assert.AreEqual(0, entities.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual(facets.First().Id, entities.First().AssignedFacets.First().FacetId);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("RemoveEntityFacet"), TestCategory("UpdateExistingEntity")]
        public void RollbackRemovedAssignedFacetWithPropertyFromEntityViewModel()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };
            var entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriavl of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());

            // ACT

            e1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());

            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.UnassignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());

            Assert.AreEqual(1, vm.Entities.First().AssignedFacets.Count());
            Assert.AreEqual(1, vm.Entities.First().AssignedFacets.First().Properties.Count());
            Assert.AreSame(vm.Facets.First().ModelItem, vm.Entities.First().AssignedFacets.First().Facet.ModelItem);

            Assert.AreEqual(1, entities.First().AssignedFacets.Count());
            Assert.AreEqual(1, entities.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual(facets.First().Id, entities.First().AssignedFacets.First().FacetId);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region RemoveAssignedFacet > Rollback > UnassignFacet > Commit

        [TestMethod]
        [TestCategory("RemoveEntityFacet"), TestCategory("UpdateExistingEntity")]
        public void CommitRemovedEmptyAssignedFacetFromEntityViewModelAfterRollback()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            var entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect update of e1
                .Setup(_ => _.UpdateEntity(entities.First()))
                .Returns<Entity>(e => Task.FromResult(e));

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriavl of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());
            e1edit.Rollback.Execute();

            // ACT

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());
            e1edit.Commit.Execute();

            // ASSERT

            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual(0, vm.Entities.First().AssignedFacets.Count());

            Assert.AreEqual(0, entities.First().AssignedFacets.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.UpdateEntity(entities.First()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);

        }

        [TestMethod]
        [TestCategory("RemoveEntityFacet"), TestCategory("UpdateExistingEntity")]
        public void CommitRemovedAssignedFacetWothPropertyFromEntityViewModelAfterRollback()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };

            var entities = new[]
            {
                EntityFactory.CreateNew(e =>
                {
                    e.Name = "e1";
                    e.Add(e.CreateNewAssignedFacet(facets.First()));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect retrieval of all entites
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect update of e1
                .Setup(_ => _.UpdateEntity(entities.First()))
                .Returns<Entity>(e => Task.FromResult(e));

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retriavl of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());
            e1edit.Rollback.Execute();

            // ACT

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());
            e1edit.Commit.Execute();

            // ASSERT

            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual(0, vm.Entities.First().AssignedFacets.Count());

            Assert.AreEqual(0, entities.First().AssignedFacets.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.UpdateEntity(entities.First()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 
    }
}
