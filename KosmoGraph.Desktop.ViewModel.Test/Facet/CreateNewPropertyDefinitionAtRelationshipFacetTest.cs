namespace KosmoGraph.Desktop.ViewModel.Test
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
    public class CreateNewPropertyDefinitionAtRelationshipFacetTest
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

            // provide a facet with a property definition
            this.facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            this.entities = new[]
            {
                EntityFactory.CreateNew(e => 
                {
                    e.Name = "e1";
                }),
                EntityFactory.CreateNew(e =>
                {
                    e.Name = "e2";
                })
            };

            this.relationships = new[]
            {
                RelationshipFactory.CreateNew(r => 
                {
                    r.FromId = this.entities.ElementAt(0).Id;
                    r.ToId = this.entities.ElementAt(1).Id;
                    r.Add(r.CreateNewAssignedFacet(this.facets.Single(), delegate { }));
                })
            };

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();
            this.ersvc // expect retrueval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));
            this.ersvc
                .Setup( _=>_.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        #region EditFacet > AddPropertyDefinition

        [TestMethod]
        [TestCategory("EditFacet"), TestCategory("AddPropertyDefinition")]
        public void CreateNewPropertyDefinitionAtEntityFacet()
        {
            // ARRANGE

            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());

            // ACT

            f1edit.AddPropertyDefinition.Execute();

            // ASSERT

            Assert.IsTrue(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.Rollback.CanExecute());
            Assert.AreEqual(1, f1edit.Properties.Count());
            Assert.AreEqual(string.Format(Properties.Resources.EditNewFacetNewPropertyNameDefault, 1), f1edit.Properties.Single().Name);
            Assert.AreEqual(0, f1edit.Edited.Properties.Count());
            Assert.AreEqual(0, f1edit.Edited.ModelItem.Properties.Count());

            Assert.AreEqual(0, this.vm.Relationships.Single().Properties.Count());
            Assert.AreEqual(0, this.vm.Relationships.Single().AssignedFacets.Single().Properties.Count());

            Assert.AreEqual(0, this.relationships.Single().AssignedFacets.Single().Properties.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }

        #endregion 

        #region EditFacet > AddPropertyDefinition > Commit

        [TestMethod]
        [TestCategory("EditFacet"), TestCategory("AddPropertyDefinition")]
        public void CommitCreateNewPropertyDefinitionAtEntityFacet()
        {
            // ARRANGE

            this.fsvc // expect update of facet
                .Setup(_=>_.UpdateFacet(this.facets.Single()))
                .Returns<Facet>(f =>Task.FromResult(f));

            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());
            
            f1edit.AddPropertyDefinition.Execute();

            // ACT

            f1edit.Commit.Execute();
            

            // ASSERT

            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.IsFalse(f1edit.Rollback.CanExecute());
            Assert.AreEqual(1, f1edit.Properties.Count());
            Assert.AreEqual(string.Format(Properties.Resources.EditNewFacetNewPropertyNameDefault, 1), f1edit.Properties.Single().Name);
            Assert.AreEqual(1, f1edit.Edited.Properties.Count());
            Assert.AreEqual(string.Format(Properties.Resources.EditNewFacetNewPropertyNameDefault, 1), f1edit.Edited.Properties.Single().Name);
            Assert.AreEqual(1, f1edit.Edited.ModelItem.Properties.Count());
            Assert.AreEqual(string.Format(Properties.Resources.EditNewFacetNewPropertyNameDefault, 1), f1edit.Edited.ModelItem.Properties.Single().Name);

            Assert.AreEqual(1, this.vm.Relationships.Single().Properties.Count());
            Assert.AreEqual(1, this.vm.Relationships.Single().AssignedFacets.Single().Properties.Count());

            Assert.AreEqual(0, this.relationships.Single().AssignedFacets.Single().Properties.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.fsvc.Verify(_ => _.UpdateFacet(It.IsAny<Facet>()), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }

        #endregion 

        #region EditFacet > AddPropertyDefinition > Rollback

        [TestMethod]
        [TestCategory("EditFacet"), TestCategory("AddPropertyDefinition")]
        public void RollbackCreateNewPropertyDefinitionAtEntityFacet()
        {
            // ARRANGE

            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());

            f1edit.AddPropertyDefinition.Execute();

            // ACT

            f1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.Rollback.CanExecute());
            Assert.AreEqual(0, f1edit.Properties.Count());
            Assert.AreEqual(0, f1edit.Edited.Properties.Count());
            Assert.AreEqual(0, f1edit.Edited.ModelItem.Properties.Count());

            Assert.AreEqual(0, this.vm.Relationships.Single().Properties.Count());
            Assert.AreEqual(0, this.vm.Relationships.Single().AssignedFacets.Single().Properties.Count());

            Assert.AreEqual(0, this.relationships.Single().AssignedFacets.Single().Properties.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }

        #endregion 
    }
}
