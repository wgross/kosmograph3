namespace KosmoGraph.Desktop.ViewModel.Test.Facet
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Services;
    using System.Threading.Tasks;

    [TestClass]
    public class RemoveFacetPropertyDefinitionTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        #region RemovePropertyDefinition

        [TestMethod]
        [TestCategory("UpdateExistingFacet"),TestCategory("RemovePropertyDefinition")]
        public void RemovePropertyDefinitionFromExistingUnusedFacet()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name ="pd1"));
                }) 
            };

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

         

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var f1edit = vm.EditFacet(vm.Facets.Single());

            // ACT

            f1edit.RemovePropertyDefinition.Execute(f1edit.Properties.Single());

            // ASSERT

            Assert.IsTrue(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.Rollback.CanExecute());
            Assert.AreEqual(0, f1edit.Properties.Count());
            Assert.AreEqual(1, f1edit.Edited.Properties.Count());
            Assert.AreEqual(1, f1edit.Edited.ModelItem.Properties.Count());

            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);

        }

        #endregion 

        #region RemovePropertyDefinition > Commit

        [TestMethod]
        [TestCategory("UpdateExistingFacet"), TestCategory("RemovePropertyDefinition")]
        public void CommitRemovedPropertyDefinitionFromExistingUnusedFacet()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name ="pd1"));
                }) 
            };

            var fsvc = new Mock<IManageFacets>();

            fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(facets.AsEnumerable()));

            fsvc // expect update of facet
             .Setup(_ => _.UpdateFacet(facets.Single()))
             .Returns<Facet>(f => Task.FromResult(f));

            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
            var f1edit = vm.EditFacet(vm.Facets.Single());

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

            fsvc.VerifyAll();
            fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            fsvc.Verify(_ => _.UpdateFacet(It.IsAny<Facet>()), Times.Once);
        }

        #endregion 
    }
}
