namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using KosmoGraph.Services;
    using KosmoGraph.Model;
    using System.Threading.Tasks;
    using KosmoGraph.Test;

    [TestClass]
    public class RemoveAssignedRelationshipFacetTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        #region RemoveRelationshipFacet

        [TestMethod]
        [TestCategory("RemoveRelationshipFacet"),TestCategory("UpdateExistingRelationship")]
        public void RemoveEmptyAssignedFacetFromExistingRelationshipViewModel()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();

            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
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
                    r.FromId=entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

             ersvc // expects retrieval of all relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            // ACT

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(1, vm.Relationships.First().AssignedFacets.Count());
            Assert.AreEqual(0, vm.Relationships.First().AssignedFacets.First().Properties.Count());

            Assert.AreEqual(1, relationships.First().AssignedFacets.Count());
            Assert.AreEqual(0, relationships.First().AssignedFacets.First().Properties.Count()); 
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("RemoveRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void RemoveAssignedFacetWithPropertyFromExistingRelationshipViewModel()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };

            fsvc // expect Facet retrieval
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
                    r.FromId=entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expects retrieval of all relationships
               .Setup(_ => _.GetAllRelationships())
               .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            // ACT

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(1, vm.Relationships.First().AssignedFacets.Count());
            Assert.AreEqual(1, vm.Relationships.First().AssignedFacets.First().Properties.Count());

            Assert.AreEqual(1, relationships.First().AssignedFacets.Count());
            Assert.AreEqual(1, relationships.First().AssignedFacets.First().Properties.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }


        #endregion

        #region RemoveRelationshipFacet > Commit

        [TestMethod]
        [TestCategory("RemoveRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void CommitRemovedEmptyAssignedFacetFromExistingRelationshipViewModel()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();

            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
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
                    r.FromId=entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expects retrieval of all relationships
               .Setup(_ => _.GetAllRelationships())
               .Returns(Task.FromResult(relationships.AsEnumerable()));

            ersvc // expect update of relatinship
                .Setup(_ => _.UpdateRelationship(relationships.First()))
                .Returns<Relationship>(r => Task.FromResult(r));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());

            // ACT

            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(0, vm.Relationships.First().AssignedFacets.Count());
            
            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("RemoveRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void CommitRemovedAssignedFacetWithPropertyFromExistingRelationshipViewModel()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };

            fsvc // expect Facet retrieval
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
                    r.FromId=entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expects retrieval of all relationships
               .Setup(_ => _.GetAllRelationships())
               .Returns(Task.FromResult(relationships.AsEnumerable()));

            ersvc // expect update of relatinship
             .Setup(_ => _.UpdateRelationship(relationships.First()))
             .Returns<Relationship>(r => Task.FromResult(r));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());

            // ACT

            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(0, vm.Relationships.First().AssignedFacets.Count());
            
            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region RemoveRelationshipFacet > Rollback

        [TestMethod]
        [TestCategory("RemoveRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void RollbackRemovedEmptyAssignedFacetFromExistingRelationshipViewModel()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();

            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
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
                    r.FromId=entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expects retrieval of all relationships
               .Setup(_ => _.GetAllRelationships())
               .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(1, vm.Relationships.First().AssignedFacets.Count());
            Assert.AreEqual(0, vm.Relationships.First().AssignedFacets.First().Properties.Count());

            Assert.AreEqual(1, relationships.First().AssignedFacets.Count());
            Assert.AreEqual(0, relationships.First().AssignedFacets.First().Properties.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("RemoveRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void RollbackRemovedAssignedFacetWithPropertyFromExistingRelationshipViewModel()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };

            fsvc // expect Facet retrieval
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
                    r.FromId=entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expects retrieval of all relationships
               .Setup(_ => _.GetAllRelationships())
               .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());


            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());

            Assert.AreEqual(1, vm.Relationships.First().AssignedFacets.Count());
            Assert.AreEqual(1, vm.Relationships.First().AssignedFacets.First().Properties.Count());

            Assert.AreEqual(1, relationships.First().AssignedFacets.Count());
            Assert.AreEqual(1, relationships.First().AssignedFacets.First().Properties.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region RemoveRelationshipFacet > Rollback > UnassignFacet > Commit

        [TestMethod]
        [TestCategory("RemoveRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void CommitRemovedEmptyAssignedFacetFromExistingRelationshipViewModelAfterRollback()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();

            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            fsvc // expect Facet retrieval
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
                    r.FromId=entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expects retrieval of all relationships
               .Setup(_ => _.GetAllRelationships())
               .Returns(Task.FromResult(relationships.AsEnumerable()));

            ersvc // expect update of relatinship
                .Setup(_ => _.UpdateRelationship(relationships.First()))
                .Returns<Relationship>(r => Task.FromResult(r));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());
            r1edit.Rollback.Execute();

            // ACT

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());
            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(0, vm.Relationships.First().AssignedFacets.Count());

            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("RemoveRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void CommitRemovedAssignedFacetWithPropertyFromExistingRelationshipViewModelAfterRollback()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };

            fsvc // expect Facet retrieval
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
                    r.FromId=entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(facets.First(),delegate{}));
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expects retrieval of all relationships
               .Setup(_ => _.GetAllRelationships())
               .Returns(Task.FromResult(relationships.AsEnumerable()));

            ersvc // expect update of relatinship
             .Setup(_ => _.UpdateRelationship(relationships.First()))
             .Returns<Relationship>(r => Task.FromResult(r));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());
            r1edit.Rollback.Execute();

            // ACT

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());
            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            Assert.AreEqual(0, vm.Relationships.First().AssignedFacets.Count());

            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion
    }
}
