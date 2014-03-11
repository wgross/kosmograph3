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
    public class RemoveUnassignedFacetsPropertyDefinitionTest
    {
        private IEnumerable<Facet> facets;
        private Mock<IManageFacets> fsvc;
        private IEnumerable<Entity> entities = Enumerable.Empty<Entity>();
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

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();
            this.ersvc // expect retrieval of all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        #region RemovePropertyDefinition

        [TestMethod]
        [TestCategory("EditFacet"), TestCategory("RemovePropertyDefinition")]
        public void RemovePropertyDefinitionFromExistingUnusedFacet()
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

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion

        #region RemovePropertyDefinition > Commit

        [TestMethod]
        [TestCategory("EditFacet"), TestCategory("RemovePropertyDefinition")]
        public void CommitRemovedPropertyDefinitionFromExistingUnusedFacet()
        {
            // ARRANGE

            this.fsvc // expect update of facet
                .Setup(_ => _.UpdateFacet(facets.Single()))
                .Returns<Facet>(f => Task.FromResult(f));

            var f1edit = this.vm.EditFacet(this.vm.Facets.Single());

            f1edit.RemovePropertyDefinition.Execute(f1edit.Properties.Single());

            // ACT

            f1edit.Commit.Execute();

            // ASSERT

            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.IsFalse(f1edit.Rollback.CanExecute());
            Assert.AreEqual(0, f1edit.Properties.Count());

            Assert.AreSame(vm.Facets.Single(), f1edit.Edited);
            Assert.AreEqual(0, f1edit.Edited.Properties.Count());

            Assert.AreSame(facets.Single(), f1edit.Edited.ModelItem);
            Assert.AreEqual(0, f1edit.Edited.ModelItem.Properties.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.fsvc.Verify(_ => _.UpdateFacet(It.IsAny<Facet>()), Times.Once);
        }

        #endregion

        #region RemovePropertyDefinition > Rollback

        [TestMethod]
        [TestCategory("EditFacet"), TestCategory("RemovePropertyDefinition")]
        public void RollbackRemovedPropertyDefinitionFromExistingUnusedFacet()
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
            Assert.AreEqual("pd1", f1edit.Properties.First().Name);
            Assert.AreEqual(1, f1edit.Edited.Properties.Count());
            Assert.AreEqual(1, f1edit.Edited.ModelItem.Properties.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion
    }
}
