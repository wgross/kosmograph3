
namespace KosmoGraph.Desktop.ViewModel.Test
{
    using KosmoGraph.Model;
    using KosmoGraph.Services;
    using KosmoGraph.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [TestClass]
    public class CreateNewEntityFacetViewModelTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        #region CreateNewEntity > AssignFacet

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("CreateNewEntity")]
        public void CreateNewEmptyEntityFacetViewModelAtNewEntityViewModel()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";

            // ACT

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());

            // ASSERT

            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("CreateNewEntity")]
        public void CreateNewEntityFacetViewModelWithPropertyAtNewEntityViewModel()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";

            // ACT

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";

            // ASSERT

            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, e1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", e1edit.Properties.First().Value);

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingEntity > AssignFacet

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("EditEntity")]
        public void CreateNewEmptyEntityFacetViewModelAtExistingEntityViewModel()
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

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            // ACT

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());

            // ASSERT

            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("EditEntity")]
        public void CreateNewEntityFacetViewModelWithPropertyAtExistingEntityViewModel()
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

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            // ACT

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";

            // ASSERT

            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, e1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", e1edit.Properties.First().Value);

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewEntity > AssignFacet > UnassignFacet

        [TestMethod]
        [TestCategory("CreateNewEntityFacet")]
        public void CreateNewEmptyEntityFacetViewModelAtNewEntityButRemoveFromEntityAgain()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());

            // ACT

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("CreateNewEntity")]
        public void CreateNewEntityFacetViewModelWithPropertyAtNewEntityButRemoveFromEntityAgain()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";

            // ACT

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingEntity > AssignFacet > UnassignFacet

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("EditEntity")]
        public void CreateNewEmptyEntityFacetViewModelAtExistingEntityButRemoveFromEntityAgain()
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

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());

            // ACT

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("EditEntity")]
        public void CreateNewEntityFacetViewModelWithPropertyAtExistingEntityButRemoveFromEntityAgain()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";

            // ACT

            e1edit.UnassignFacet.Execute(e1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewEntity > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("CreateNewEntity")]
        public void CommitNewEmptyEntityFacetViewModelAtNewEntityCreatesNewAssignedEntityFacet()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            Entity e1 = null;
            ersvc // expect creation of entity
                .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
                .Returns<Action<Entity>>(a => Task.FromResult(e1 = EntityFactory.CreateNew(a)));

            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());

            // ACT

            e1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsFalse(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual("e1", e1.Name);
            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreEqual(facets.First().Id, e1.AssignedFacets.First().FacetId);
            Assert.AreEqual(0, e1.AssignedFacets.First().Properties.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("CreateNewEntity")]
        public void CommitNewEntityFacetViewModelWithPropertyAtBewEntityCreatesNewAssignedEntityFacet()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            Entity e1 = null;
            ersvc // expect creation of entity
               .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
               .Returns<Action<Entity>>(a => Task.FromResult(e1 = EntityFactory.CreateNew(a)));

            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";

            // ACT

            e1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsFalse(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, e1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", e1edit.Properties.First().Value);

            Assert.AreEqual("e1", e1.Name);
            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreEqual(facets.First().Id, e1.AssignedFacets.First().FacetId);
            Assert.AreEqual(1, e1.AssignedFacets.First().Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, e1.AssignedFacets.First().Properties.First().DefinitionId);
            Assert.AreEqual("pv1", e1.AssignedFacets.First().Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingEntity > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("EditEntity")]
        public void CommitNewEmptyEntityFacetViewModelAtExistingEntityCreatesNewAssignedEntityFacet()
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

            ersvc // expect creation of entity
                .Setup(_ => _.UpdateEntity(entities.First()))
                .Returns<Entity>(e => Task.FromResult(e));

            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());

            // ACT

            e1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsFalse(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual("e1", entities.First().Name);
            Assert.AreEqual(1, entities.First().AssignedFacets.Count());
            Assert.AreEqual(facets.First().Id, entities.First().AssignedFacets.First().FacetId);
            Assert.AreEqual(0, entities.First().AssignedFacets.First().Properties.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.UpdateEntity(It.IsAny<Entity>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("EditEntity")]
        public void CommitNewEntityFacetViewModelWithPropertyAtExistingEntityCreatesNewAssignedEntityFacet()
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

            ersvc // expect creation of entity
                .Setup(_ => _.UpdateEntity(entities.First()))
                .Returns<Entity>(e => Task.FromResult(e));

            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";

            // ACT

            e1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsFalse(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, e1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", e1edit.Properties.First().Value);

            Assert.AreEqual("e1", entities.First().Name);
            Assert.AreEqual(1, entities.First().AssignedFacets.Count());
            Assert.AreEqual(facets.First().Id, entities.First().AssignedFacets.First().FacetId);
            Assert.AreEqual(1, entities.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, entities.First().AssignedFacets.First().Properties.First().DefinitionId);
            Assert.AreEqual("pv1", entities.First().AssignedFacets.First().Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.UpdateEntity(It.IsAny<Entity>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewEntity > AssignFacet > Rollback

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("CreateNewEntity")]
        public void RollbackNewEmptyEntityFacetViewModelAtNewEntityInitializesAgain()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());

            // ACT

            e1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("CreateNewEntity")]
        public void RollbackNewEntityFacetViewModelWithPropertyAtNewEntityInitializesAgain()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";

            // ACT

            e1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingEntity > AssignFacet > Rollback

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("EditEntity")]
        public void RollbackNewEmptyEntityFacetViewModelAtExistingEntityInitializesAgain()
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

            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());

            // ACT

            e1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("EditEntity")]
        public void RollbackNewEntityFacetViewModelWithPropertyAtExistingEntityInitializesAgain()
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

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";

            // ACT

            e1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsTrue(e1edit.Rollback.CanExecute());
            Assert.AreEqual(0, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region CreateNewEntity > AssignFacet > Rollback > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("CreateNewEntity")]
        public void RollbackNewEmptyEntityFacetViewModelAtNewEntityAllowsEditAgainTillCommit()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expect creation of entity
                .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
                .Returns<Action<Entity>>(a => Task.FromResult(EntityFactory.CreateNew(a)));

            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Rollback.Execute();

            // ACT

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsFalse(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("CreateNewEntity")]
        public void RollbackNewEntityFacetViewModelWithPropertyAtNewEntityAllowsEditAgainTillCommit()
        {
            // ARRANGE

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            Entity e1 = null;
            ersvc // expect creation of entity
               .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
               .Returns<Action<Entity>>(a => Task.FromResult(e1 = EntityFactory.CreateNew(a)));

            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.CreateNewEntity();

            e1edit.Name = "e1";
            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";
            e1edit.Rollback.Execute();

            // ACT

            e1edit.Name = "e1";
            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";
            e1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsFalse(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, e1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", e1edit.Properties.First().Value);

            Assert.AreEqual("e1", e1.Name);
            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreEqual(facets.First().Id, e1.AssignedFacets.First().FacetId);
            Assert.AreEqual(1, e1.AssignedFacets.First().Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, e1.AssignedFacets.First().Properties.First().DefinitionId);
            Assert.AreEqual("pv1", e1.AssignedFacets.First().Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingEntity > AssignFacet > Commit > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("EditEntity")]
        public void RollbackNewEmptyEntityFacetViewModelAtExistingEntityAllowsEditAgainTillCommit()
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

            ersvc // expect creation of entity
                .Setup(_ => _.UpdateEntity(entities.First()))
                .Returns<Entity>(e => Task.FromResult(e));

            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Rollback.Execute();

            // ACT

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsFalse(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(0, e1edit.Properties.Count());

            Assert.AreEqual("e1", entities.First().Name);
            Assert.AreEqual(1, entities.First().AssignedFacets.Count());
            Assert.AreEqual(facets.First().Id, entities.First().AssignedFacets.First().FacetId);
            Assert.AreEqual(0, entities.First().AssignedFacets.First().Properties.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.UpdateEntity(It.IsAny<Entity>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewEntityFacet"), TestCategory("EditEntity")]
        public void RollbackNewEntityFacetViewModelWithPropertyAtExistingEntityAllowsEditAgainTillCommit()
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

            ersvc // expect creation of entity
                .Setup(_ => _.UpdateEntity(entities.First()))
                .Returns<Entity>(e => Task.FromResult(e));

            var fsvc = new Mock<IManageFacets>();

            // create facet
            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name="pd1"));
                })
            };

            fsvc // expect Facet retrieval
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1edit = vm.EditEntity(vm.Entities.First());

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";
            e1edit.Rollback.Execute();

            // ACT

            e1edit.AssignFacet.Execute(e1edit.UnassignedFacets.First());
            e1edit.Properties.First().Value = "pv1";
            e1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(e1edit.Commit.CanExecute());
            Assert.IsFalse(e1edit.Rollback.CanExecute());
            Assert.AreEqual(1, e1edit.AssignedFacets.Count());
            Assert.AreEqual(1, e1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, e1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", e1edit.Properties.First().Value);

            Assert.AreEqual("e1", entities.First().Name);
            Assert.AreEqual(1, entities.First().AssignedFacets.Count());
            Assert.AreEqual(facets.First().Id, entities.First().AssignedFacets.First().FacetId);
            Assert.AreEqual(1, entities.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, entities.First().AssignedFacets.First().Properties.First().DefinitionId);
            Assert.AreEqual("pv1", entities.First().AssignedFacets.First().Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.UpdateEntity(It.IsAny<Entity>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion
    }
}
