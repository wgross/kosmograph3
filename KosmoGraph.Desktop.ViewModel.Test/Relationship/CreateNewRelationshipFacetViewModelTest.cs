
namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using KosmoGraph.Services;
    using KosmoGraph.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KosmoGraph.Test;

    [TestClass]
    public class CreateNewRelationshipFacetViewModelTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        #region CreateNewRelationship > AssignFacet

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"),TestCategory("CreateNewRelationship")]
        public void CreateNewEmptyRelationshipFacetViewModelAtNewRelationship()
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

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));
           
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);
            var r1edit = vm.CreateNewRelationship(e1, e2);
            
            // ACT

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            
            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void CreateNewRelationshipFacetViewModelWithPropertyAtNewRelationship()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
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

            var entities = new[] 
            {
                EntityFactory.CreateNew(e=>e.Name = "e1"),
                EntityFactory.CreateNew(e=>e.Name = "e2"),
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);
            var r1edit = vm.CreateNewRelationship(e1, e2);

            // ACT

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, r1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region UpdateExistingRelationship > AssignFacet

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"),TestCategory("UpdateExistingRelationship")]
        public void CreateNewEmptyRelationshipFacetViewModelAtExistingRelationship()
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
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of all relatinships
                .Setup( _=>_.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            // ACT

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void CreateNewRelationshipFacetViewModelWithPropertyAtExistingRelationshipViewModel()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
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

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, r1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region CreateNewRelationship > AssignFacet > UnassignFacet

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void CreateNewEmptyRelationshipFacetAtNewRelatsionshipButRemoveFromRelationshipAgain()
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

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);
            var r1edit = vm.CreateNewRelationship(e1, e2);

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());

            // ACT

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void CreateNewRelationshipFacetViewModelWithPropertytNewRelatinshipButRemoveFromRelationshipAgain ()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
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

            var entities = new[] 
            {
                EntityFactory.CreateNew(e=>e.Name = "e1"),
                EntityFactory.CreateNew(e=>e.Name = "e2"),
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);

            var r1edit = vm.CreateNewRelationship(e1, e2);
            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region UpdateExistingRelationship > AssignFacet > UnassignFacet

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void CreateNewEmptyRelationshipFacetViewModelAtExistingRelationshipButRemoveFromRelationshipAgain()
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
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of all relatinships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());
            
            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            
            // ACT

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());
            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void CreateNewRelationshipFacetViewModelWithPropertyAtExistingRelationshipViewModelButRemoveFromRelatinshipAgain()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
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

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.UnassignFacet.Execute(r1edit.AssignedFacets.First());
            
            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region CreateNewRelationship > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void CommitNewEmptyRelationshipFacetViewModelAtNewRelationshipCreatesNewAssignedRelationshipFacet()
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

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);

            ersvc // expect creation of partial relationship with source entity
                .Setup(_ => _.CreatePartialRelationship(e1.ModelItem, It.IsAny<Action<Relationship>>()))
                .Returns<Entity, Action<Relationship>>((e, a) =>
                {
                    var r = RelationshipFactory.CreateNewPartial(e);
                    a(r);
                    return r;
                });

            ersvc
                .Setup(_ => _.CompletePartialRelationship(
                    It.Is<Relationship>(r => r.FromId == e1.ModelItem.Id && r.AssignedFacets.Count() == 1), 
                    e2.ModelItem))
                    .Returns<Relationship,Entity>((r,e)=>Task.FromResult(new CompletePartialRelationshipResult(r, e1.ModelItem, e)));

            var r1edit = vm.CreateNewRelationship(e1, e2);

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            
            // ACT

            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void CommitNewRelationshipFacetViewModelWithPropertyAtNewRelatinshipCreatesNewAssignedRelationshipFacet()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
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

            var entities = new[] 
            {
                EntityFactory.CreateNew(e=>e.Name = "e1"),
                EntityFactory.CreateNew(e=>e.Name = "e2"),
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            
            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);
            
            Relationship r1 = null;
            ersvc // expect creation of partial relationship with source entity
              .Setup(_ => _.CreatePartialRelationship(e1.ModelItem, It.IsAny<Action<Relationship>>()))
              .Returns<Entity, Action<Relationship>>((e, a) =>
              {
                  a(r1 = RelationshipFactory.CreateNewPartial(e));
                  return r1;
              });

            ersvc
                .Setup(_ => _.CompletePartialRelationship(It.Is<Relationship>(r => r.FromId == e1.ModelItem.Id && r.AssignedFacets.Count() == 1), e2.ModelItem))
                .Returns<Relationship, Entity>((r, e) => Task.FromResult(new CompletePartialRelationshipResult(r, e1.ModelItem, e)));

            var r1edit = vm.CreateNewRelationship(e1, e2);
            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, r1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);

            Assert.AreEqual(1, r1.AssignedFacets.Count());
            Assert.AreEqual(facets.First().Id, r1.AssignedFacets.First().FacetId);
            Assert.AreEqual(facets.First().Properties.First().Id, r1.AssignedFacets.First().Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1.AssignedFacets.First().Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_=>_.CompletePartialRelationship(It.IsAny<Relationship>(), e2.ModelItem), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region UpdateExistingRelationship > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void CommitNewEmptyRelationshipFacetViewModelAtExistingRelatinshipCreatesNewAssignedRelationshipFacet()
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
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of all relatinships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            ersvc // expect update of relationship
                .Setup(_ => _.UpdateRelationship(relationships.First()))
                .Returns<Relationship>(r => Task.FromResult(r));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());

            // ACT

            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreSame(facets.First(), vm.Relationships.First().AssignedFacets.First().Facet.ModelItem);

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(1, relationships.First().AssignedFacets.Count());
            Assert.AreEqual(0, relationships.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual(facets.First().Id, relationships.First().AssignedFacets.First().FacetId);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void CommitNewRelationshipFacetViewModelWithPropertyAtExistingRelatinshipCreatesNewAssignedRelationshipFacet()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
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

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.Commit.Execute();
            
            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreSame(facets.First(), vm.Relationships.First().AssignedFacets.First().Facet.ModelItem);

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(1, relationships.First().AssignedFacets.Count());
            Assert.AreEqual(1, relationships.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual("pv1", relationships.First().AssignedFacets.First().Properties.First().Value);

            Assert.AreEqual(facets.First().Id, relationships.First().AssignedFacets.First().FacetId);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region CreateNewRelationship > AssignFacet > Rollback

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void RollbackNewEmptyRelationshipFacetViewModelAtNewRelationshipInitializesAgain()
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

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);
            var r1edit = vm.CreateNewRelationship(e1, e2);

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void RollbackNewRelationshipFacetViewModelWithPropertyAtNewRelationshipInitializesAgain()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
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

            var entities = new[] 
            {
                EntityFactory.CreateNew(e=>e.Name = "e1"),
                EntityFactory.CreateNew(e=>e.Name = "e2"),
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);
            var r1edit = vm.CreateNewRelationship(e1, e2);

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region UpdateExistingRelationship > AssignFacet > Rollback

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void RollbackNewEmptyRelationshipFacetViewModelAtExistingRelationshipInitializesAgain()
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
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of all relatinships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void RollbackNewRelationshipFacetViewModelWithPropertyAtExistingRelatinshipInitializesAgain()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
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

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";

            // ACT

            r1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.AreEqual(0, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.UnassignedFacets.Count());

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(0, relationships.First().AssignedFacets.Count());
            
            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region CreateNewRelationship > AssignFacet > Rollback > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void RollbackNewEmptyRelationshipFacetViewModelAtNewRelationshipAllowsEditTillCommitAgain()
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

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));
                
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);

            ersvc // expect creation of partial relationship with source entity
                .Setup(_ => _.CreatePartialRelationship(e1.ModelItem, It.IsAny<Action<Relationship>>()))
                 .Returns<Entity, Action<Relationship>>((e, a) =>
                 {
                     var r = RelationshipFactory.CreateNewPartial(e);
                     a(r);
                     return r;
                 });

            ersvc
                .Setup(_ => _.CompletePartialRelationship(
                    It.Is<Relationship>(r => r.FromId == e1.ModelItem.Id && r.AssignedFacets.Count() == 1),
                    e2.ModelItem))
                .Returns<Relationship,Entity>((r,e)=>Task.FromResult(new CompletePartialRelationshipResult(r, e1.ModelItem, e)));
                
            var r1edit = vm.CreateNewRelationship(e1, e2);

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Rollback.Execute();

            // ACT

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("CreateNewRelationship")]
        public void RollbackNewRelationshipFacetViewModelWithPropertyAtNewRelationshipAllowsEditTillCommitAgain()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
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

            var entities = new[] 
            {
                EntityFactory.CreateNew(e=>e.Name = "e1"),
                EntityFactory.CreateNew(e=>e.Name = "e2"),
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var e1 = vm.Entities.First();
            var e2 = vm.Entities.ElementAt(1);

            Relationship r1 = null;
            ersvc // expect creation of partial relationship with source entity
              .Setup(_ => _.CreatePartialRelationship(e1.ModelItem, It.IsAny<Action<Relationship>>()))
              .Returns<Entity, Action<Relationship>>((e, a) =>
              {
                  a(r1 = RelationshipFactory.CreateNewPartial(e));
                  return r1;
              });

            ersvc
                .Setup(_ => _.CompletePartialRelationship(It.Is<Relationship>(r => r.FromId == e1.ModelItem.Id && r.AssignedFacets.Count() == 1), e2.ModelItem))
                .Returns<Relationship, Entity>((r, e) => Task.FromResult(new CompletePartialRelationshipResult(r, e1.ModelItem, e)));

            var r1edit = vm.CreateNewRelationship(e1, e2);
            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";
            r1edit.Rollback.Execute();
    
            // ACT

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";
            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual(facets.First().Properties.First().Id, r1edit.Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);

            Assert.AreEqual(1, r1.AssignedFacets.Count());
            Assert.AreEqual(facets.First().Id, r1.AssignedFacets.First().FacetId);
            Assert.AreEqual(facets.First().Properties.First().Id, r1.AssignedFacets.First().Properties.First().DefinitionId);
            Assert.AreEqual("pv1", r1.AssignedFacets.First().Properties.First().Value);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.CompletePartialRelationship(It.IsAny<Relationship>(), e2.ModelItem), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region UpdateExistingRelationship > AssignFacet > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void RollbackNewEmptyRelationshipFacetViewModelAtExistingRelationshipAllowsEditTillCommitAgain()
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
                    r.FromId = entities.ElementAt(0).Id;
                    r.ToId = entities.ElementAt(1).Id;
                })
            };

            var ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(entities.AsEnumerable()));

            ersvc // expect retrieval of all relatinships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(relationships.AsEnumerable()));

            ersvc // expect update of relationship
                .Setup(_ => _.UpdateRelationship(relationships.First()))
                .Returns<Relationship>(r => Task.FromResult(r));

            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var r1edit = vm.EditRelationship(vm.Relationships.First());

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Rollback.Execute();

            // ACT

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(0, r1edit.Properties.Count());
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreSame(facets.First(), vm.Relationships.First().AssignedFacets.First().Facet.ModelItem);

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(1, relationships.First().AssignedFacets.Count());
            Assert.AreEqual(0, relationships.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual(facets.First().Id, relationships.First().AssignedFacets.First().FacetId);

            ersvc.VerifyAll();
            ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            ersvc.Verify(_ => _.UpdateRelationship(It.IsAny<Relationship>()), Times.Once);
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        [TestMethod]
        [TestCategory("CreateNewRelationshipFacet"), TestCategory("UpdateExistingRelationship")]
        public void RollbackNewRelationshipFacetViewModelWithPropertyAtExistingRelationshipAllowsEditTillCommitAgain()
        {
            // ARRANGE

            var fsvc = new Mock<IManageFacets>();
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

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";
            r1edit.Rollback.Execute();

            // ACT

            r1edit.AssignFacet.Execute(r1edit.UnassignedFacets.First());
            r1edit.Properties.First().Value = "pv1";
            r1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.AreEqual(1, r1edit.AssignedFacets.Count());
            Assert.AreEqual(1, r1edit.Properties.Count());
            Assert.AreEqual("pv1", r1edit.Properties.First().Value);
            Assert.AreEqual(0, r1edit.UnassignedFacets.Count());

            Assert.AreSame(facets.First(), vm.Relationships.First().AssignedFacets.First().Facet.ModelItem);

            Assert.AreSame(relationships.First(), r1edit.Edited.ModelItem);
            Assert.AreEqual(1, relationships.First().AssignedFacets.Count());
            Assert.AreEqual(1, relationships.First().AssignedFacets.First().Properties.Count());
            Assert.AreEqual("pv1", relationships.First().AssignedFacets.First().Properties.First().Value);

            Assert.AreEqual(facets.First().Id, relationships.First().AssignedFacets.First().FacetId);

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
