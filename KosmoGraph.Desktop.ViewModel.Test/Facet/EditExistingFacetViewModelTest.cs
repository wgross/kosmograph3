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

    [TestClass]
    public class EditExistingFacetViewModelTest
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

            // provide a facet with a property definition
            this.facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            this.fsvc = new Mock<IManageFacets>();
            this.fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

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

        #region EditFacet > Modify

        [TestMethod]
        [TestCategory("EditFacet")]
        public void EditExistingFacetName()
        {
            // ARRANGE

            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());

            // ACT

            f1edit.Name = "f1-changed";

            // ASSERT

            Assert.IsTrue(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.Rollback.CanExecute());
            Assert.AreEqual("f1-changed", f1edit.Name);
            Assert.AreEqual("f1", f1edit.Edited.Name);
            Assert.AreEqual("f1", f1edit.Edited.ModelItem.Name);

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_=>_.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_=>_.GetAllRelationships(), Times.Once);
        }

        #endregion 

        #region EditFacet > Modify > Commit

        [TestMethod]
        [TestCategory("EditFacet")]
        public void CommitEditedExistingFacetName()
        {
            // ARRANGE

            this.fsvc // expect update of facet
                .Setup(_ => _.UpdateFacet(this.facets.Single()))
                .Returns<Facet>(f => Task.FromResult(f));

            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());

            f1edit.Name = "f1-changed";

            // ACT

            f1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.IsFalse(f1edit.Rollback.CanExecute());
            Assert.AreEqual("f1-changed", f1edit.Name);
            Assert.AreEqual("f1-changed", f1edit.Edited.Name);
            Assert.AreEqual("f1-changed", f1edit.Edited.ModelItem.Name);

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.fsvc.Verify(_ => _.UpdateFacet(It.IsAny<Facet>()), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }

        #endregion 

        #region EditFacet > Modify > Rollback

        [TestMethod]
        [TestCategory("EditFacet")]
        public void RollbackEditedExistingFacetName()
        {
            // ARRANGE

            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());

            f1edit.Name = "f1-changed";

            // ACT

            f1edit.Rollback.Execute();

            // ASSERT

            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.Rollback.CanExecute());
            Assert.AreEqual("f1", f1edit.Name);
            Assert.AreEqual("f1", f1edit.Edited.Name);
            Assert.AreEqual("f1", f1edit.Edited.ModelItem.Name);

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }


        #endregion 

        #region EditFacet > Modify > Rollback > Modify > Commit

        [TestMethod]
        [TestCategory("EditFacet")]
        public void RollbackEditedExistingFacetNameAllowsCommitAgain()
        {
            // ARRANGE
            
            this.fsvc // expect update of facet
                .Setup(_ => _.UpdateFacet(this.facets.Single()))
                .Returns<Facet>(f => Task.FromResult(f));

            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());

            f1edit.Name = "f1-changed";
            f1edit.Rollback.Execute();

            // ACT

            f1edit.Name = "f1-changed";
            f1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.IsFalse(f1edit.Rollback.CanExecute());
            Assert.AreEqual("f1-changed", f1edit.Name);
            Assert.AreEqual("f1-changed", f1edit.Edited.Name);
            Assert.AreEqual("f1-changed", f1edit.Edited.ModelItem.Name);

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.fsvc.Verify(_ => _.UpdateFacet(It.IsAny<Facet>()), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }


        #endregion 
    }
}
