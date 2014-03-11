namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Services;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    [TestClass]
    public class RemoveAssignedEntityFacetsPropertyDefinitionTest
    {
        private IEnumerable<Facet> facets;
        private Mock<IManageFacets> fsvc;
        private IEnumerable<Entity> entities;
        private Mock<IManageEntitiesAndRelationships> ersvc;
        private EntityRelationshipViewModel vm;

        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();

            // provide a facet with a property definition
            this.facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name ="pd1"));
                }) 
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
                    e.Add(e.CreateNewAssignedFacet(this.facets.Single(), af => 
                    {
                        af.Properties.Single().Value = "pv1";
                    }));
                })
            };

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();
            this.ersvc // expect retrueval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);

        }

        #region RemovePropertyDefinition

        [TestMethod]
        [TestCategory("EditFacet"), TestCategory("RemovePropertyDefinition")]
        public void RemovePropertyDefinitionFromExistingAssignedEntityFacet()
        {
            // ARRANGE

            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());

            // ACT

            f1edit.RemovePropertyDefinition.Execute(f1edit.Properties.Single());

            // ASSERT

            Assert.IsTrue(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.Rollback.CanExecute());
            Assert.AreEqual(0, f1edit.Properties.Count());
            Assert.AreEqual(1, f1edit.Edited.Properties.Count());
            Assert.AreEqual(1, f1edit.Edited.ModelItem.Properties.Count());

            Assert.AreEqual(1, this.vm.Entities.Single().AssignedFacets.Single().Properties.Count());
            Assert.AreEqual(1, this.vm.Entities.Single().Properties.Count());
            
            Assert.AreEqual(1, this.entities.Single().AssignedFacets.Single().Properties.Count());
            Assert.AreEqual("pv1", this.entities.Single().AssignedFacets.Single().Properties.Single().Value);

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
        }

        #endregion

        #region RemovePropertyDefinition > Commit

        [TestMethod]
        [TestCategory("EditFacet"), TestCategory("RemovePropertyDefinition")]
        public void CommitRemovedPropertyDefinitionFromExistingAssignedEntityFacet()
        {
            // ARRANGE

            this.fsvc // exect update of facet
               .Setup(_ => _.UpdateFacet(this.facets.Single()))
               .Returns<Facet>(f => Task.FromResult(f));
            
            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());

            f1edit.RemovePropertyDefinition.Execute(f1edit.Properties.Single());

            // ACT

            f1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.IsFalse(f1edit.Rollback.CanExecute());
            Assert.AreEqual(0, f1edit.Properties.Count());
            Assert.AreEqual(0, f1edit.Edited.Properties.Count());
            Assert.AreEqual(0, f1edit.Edited.ModelItem.Properties.Count());

            Assert.AreEqual(0, this.vm.Entities.Single().AssignedFacets.Single().Properties.Count());
            Assert.AreEqual(0, this.vm.Entities.Single().Properties.Count());
            
            Assert.AreEqual(0, this.entities.Single().AssignedFacets.Single().Properties.Count());
            
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.fsvc.Verify(_=>_.UpdateFacet(It.IsAny<Facet>()), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
        }

        #endregion

        #region RemovePropertyDefinition > Rollback

        [TestMethod]
        [TestCategory("EditFacet"), TestCategory("RemovePropertyDefinition")]
        public void RollbackRemovedPropertyDefinitionFromExistingEntityFacet()
        {
            // ARRANGE

            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());

            f1edit.RemovePropertyDefinition.Execute(f1edit.Properties.Single());

            // ACT

            f1edit.Rollback.Execute();
            
            // ASSERT

            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.Rollback.CanExecute());
            Assert.AreEqual(1, f1edit.Properties.Count());
            Assert.AreEqual(1, f1edit.Edited.Properties.Count());
            Assert.AreEqual(1, f1edit.Edited.ModelItem.Properties.Count());

            Assert.AreEqual(1, this.vm.Entities.Single().AssignedFacets.Single().Properties.Count());
            Assert.AreEqual(1, this.vm.Entities.Single().Properties.Count());

            Assert.AreEqual(1, this.entities.Single().AssignedFacets.Single().Properties.Count());
            Assert.AreEqual("pv1", this.entities.Single().AssignedFacets.Single().Properties.Single().Value);

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
        }

        #endregion
    }
}
