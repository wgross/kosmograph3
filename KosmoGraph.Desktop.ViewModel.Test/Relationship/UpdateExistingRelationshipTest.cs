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

    [TestClass]
    public class UpdateExistingRelationshipViewModelTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        #region UpdateExistingRelationship

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void UpdateExistingRelationshipViewModel()
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

            var fsvc = new Mock<IManageFacets>();
            
            fsvc // expect retrieval of all facets
                .Setup(_=>_.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var entities = new[] 
            {
                EntityFactory.CreateNew(e=>e.Name = "e1"),
                EntityFactory.CreateNew(e=>e.Name = "e2"),
            };

            var relationships = new []
            {
                RelationshipFactory.CreateNew(r =>
                {
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(), f =>
                    {
                        f.Properties.First().Value = "pv1";
                    }));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrueval aof all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of existig relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            
            // ACT

            EditExistingRelationshipViewModel r1edit = vm.EditRelationship(vm.Relationships.Single());

            // ASSERT

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreSame(facets.First(), r1edit.AssignedFacets.First().Facet.ModelItem);
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, r1edit.Edited.Properties.First().Definition.ModelItem.Id);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, vm.Facets.Count());
            Assert.AreEqual(2, vm.Entities.Count());
            Assert.AreEqual(1, vm.Relationships.Count());
            Assert.AreEqual(4, vm.Items.Count);
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region UpdateExistingRelationship > Modify

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void ModifyPropertyValueAllowsEditExistigRelationshipViewModelCommit()
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

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var entities = new[] 
            {
                EntityFactory.CreateNew(e=>e.Name = "e1"),
                EntityFactory.CreateNew(e=>e.Name = "e2"),
            };

            var relationships = new[]
            {
                RelationshipFactory.CreateNew(r =>
                {
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(), f =>
                    {
                        f.Properties.First().Value = "pv1";
                    }));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of existing relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.Single());

            // ACT

            r1edit.Properties.First().Value = "pv1-changed";

            // ASSERT

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreSame(facets.First(), r1edit.AssignedFacets.First().Facet.ModelItem);
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, r1edit.Edited.Properties.First().Definition.ModelItem.Id);
            Assert.AreEqual("pv1-changed", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, vm.Facets.Count());
            Assert.AreEqual(2, vm.Entities.Count());
            Assert.AreEqual(1, vm.Relationships.Count());
            Assert.AreEqual("pv1", vm.Relationships.Single().Properties.First().Value);
            Assert.AreEqual(4, vm.Items.Count);

            Assert.AreEqual("pv1", relationships.First().AssignedFacets.First().Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingRelationship > Modify > Commit

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void CommitEditExistingRelationshipViewModelUpdateRelationship()
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

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var entities = new[] 
            {
                EntityFactory.CreateNew(e=>e.Name = "e1"),
                EntityFactory.CreateNew(e=>e.Name = "e2"),
            };

            var relationships = new[]
            {
                RelationshipFactory.CreateNew(r =>
                {
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(), f =>
                    {
                        f.Properties.First().Value = "pv1";
                    }));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of existing relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            ersvc // expect update of relationship
               .Setup(_ => _.UpdateRelationship(relationships.First()))
               .Returns<Relationship>(r => Task.FromResult(r));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.Single());

            r1edit.Properties.First().Value = "pv1-changed";

            // ACT

            r1edit.Commit.Execute();

            // ASSERT

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreSame(facets.First(), r1edit.AssignedFacets.First().Facet.ModelItem);
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, r1edit.Edited.Properties.First().Definition.ModelItem.Id);
            Assert.AreEqual("pv1-changed", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, vm.Facets.Count());
            Assert.AreEqual(2, vm.Entities.Count());
            Assert.AreEqual(1, vm.Relationships.Count());
            Assert.AreEqual("pv1-changed", vm.Relationships.Single().Properties.First().Value);
            Assert.AreEqual(4, vm.Items.Count);

            Assert.AreEqual("pv1-changed", relationships.First().AssignedFacets.First().Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingRelationship > Modify > Rollback

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void RollbackEditExistingEntityViewModelInitializesAgain()
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

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var entities = new[] 
            {
                EntityFactory.CreateNew(e=>e.Name = "e1"),
                EntityFactory.CreateNew(e=>e.Name = "e2"),
            };

            var relationships = new[]
            {
                RelationshipFactory.CreateNew(r =>
                {
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(), f =>
                    {
                        f.Properties.First().Value = "pv1";
                    }));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of existing relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.Single());

            r1edit.Properties.First().Value = "pv1-changed";

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreSame(facets.First(), r1edit.AssignedFacets.First().Facet.ModelItem);
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, r1edit.Edited.Properties.First().Definition.ModelItem.Id);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, vm.Facets.Count());
            Assert.AreEqual(2, vm.Entities.Count());
            Assert.AreEqual(1, vm.Relationships.Count());
            Assert.AreEqual("pv1", vm.Relationships.Single().Properties.First().Value);
            Assert.AreEqual(4, vm.Items.Count);

            Assert.AreEqual("pv1", relationships.First().AssignedFacets.First().Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region UpdateExistingRelationship > Modify > Rolback > Modify > Commit

        [TestMethod]
        [TestCategory("EditRelationship")]
        public void RollbackEditExistingRelationshipViewModelAllowsEditingAgainTillCommit()
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

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            var entities = new[] 
            {
                EntityFactory.CreateNew(e=>e.Name = "e1"),
                EntityFactory.CreateNew(e=>e.Name = "e2"),
            };

            var relationships = new[]
            {
                RelationshipFactory.CreateNew(r =>
                {
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(), f =>
                    {
                        f.Properties.First().Value = "pv1";
                    }));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of existing relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            ersvc // expect update of relationship
                .Setup(_ => _.UpdateRelationship(relationships.First()))
                .Returns<Relationship>(r => Task.FromResult(r));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.Single());

            r1edit.Properties.First().Value = "pv1-changed";
            r1edit.Rollback.Execute();

            // ACT

            r1edit.Properties.First().Value = "pv1-changed";
            r1edit.Commit.Execute();

            // ASSERT

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreSame(facets.First(), r1edit.AssignedFacets.First().Facet.ModelItem);
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, r1edit.Edited.Properties.First().Definition.ModelItem.Id);
            Assert.AreEqual("pv1-changed", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreEqual(1, vm.Facets.Count());
            Assert.AreEqual(2, vm.Entities.Count());
            Assert.AreEqual(1, vm.Relationships.Count());
            Assert.AreEqual("pv1-changed", vm.Relationships.Single().Properties.First().Value);
            Assert.AreEqual(4, vm.Items.Count);

            Assert.AreEqual("pv1-changed", relationships.First().AssignedFacets.First().Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion
    }
}