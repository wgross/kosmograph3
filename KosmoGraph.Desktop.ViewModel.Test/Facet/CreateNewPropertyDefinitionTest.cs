﻿namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using Moq;
    using KosmoGraph.Services;
    using KosmoGraph.Model;
    using KosmoGraph.Test;
    using System.Threading.Tasks;

    [TestClass]
    public class CreateNewPropertyDefinitionAtNewFacetTest
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
            this.facets = Enumerable.Empty<Facet>();

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

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);

        }

        #region CreateNewFacet > CreateNewPropertyDefinition

        [TestMethod]
        [TestCategory("CreateNewFacet"),TestCategory("CreateNewPropertyDefinition"),]
        public void CreateNewPropertyAtNewFacetViewModel()
        {
            // ARRANGE

            var f1edit = this.vm.CreateNewFacet();

            // ACT

            f1edit.Name = "f1";
            f1edit.AddPropertyDefinition.Execute();
            
            // ASSERT
            // Facet has new property definition, this allows commit

            Assert.AreEqual("f1", f1edit.Name);
            Assert.AreEqual(1, f1edit.Properties.Count());
            Assert.AreEqual(string.Format(KosmoGraph.Desktop.ViewModel.Properties.Resources.EditNewFacetNewPropertyNameDefault, 1), f1edit.Properties.Single().Name);
            
            Assert.IsTrue(f1edit.Commit.CanExecute());
            Assert.IsTrue(f1edit.Rollback.CanExecute());
            Assert.IsTrue(f1edit.AddPropertyDefinition.CanExecute());
            Assert.IsTrue(f1edit.RemovePropertyDefinition.CanExecute(f1edit.Properties.Single()));
            
            Assert.AreEqual(1, f1edit.Properties.Count());
            Assert.AreEqual(0, this.vm.Facets.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ =>_.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ =>_.GetAllEntities(), Times.Once);
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
        }

        #endregion 

        #region CreateNewFacet > CreateNewPropertyDefinition > Commit

        [TestMethod]
        [TestCategory("CreateNewFacet"),TestCategory("CreateNewPropertyDefinition")]
        public void CommitEditNewFacetViewModelWithPropertyCreatesNewFacet()
        {
            // ARRANGE

            this.fsvc // expect facet creation
                .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
                .Returns((Action<Facet> a) => Task.FromResult(FacetFactory.CreateNew(a)));

            var f1edit = vm.CreateNewFacet();
            
            f1edit.Name = "f1";
            f1edit.AddPropertyDefinition.Execute();
            f1edit.Properties.Single().Name = "pd1";

            // ACT

            f1edit.Commit.Execute();

            // ASSERT
            // commit create a new facet and adds it to the main view model

            Assert.AreEqual("f1", f1edit.Name);
            Assert.AreEqual(1, f1edit.Properties.Count());
            Assert.AreEqual("pd1", f1edit.Properties.Single().Name);
            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.IsFalse(f1edit.AddPropertyDefinition.CanExecute());
            Assert.IsFalse(f1edit.RemovePropertyDefinition.CanExecute(f1edit.Properties.Single()));

            Assert.AreEqual(1, this.vm.Facets.Count());
            Assert.AreEqual(1, this.vm.Items.Count);

            Assert.AreSame(this.vm.Items.Single(), vm.Facets.Single());
            Assert.AreEqual("f1", this.vm.Facets.Single().Name);
            Assert.AreEqual(1, this.vm.Facets.Single().Properties.Count());
            Assert.AreEqual("pd1", this.vm.Facets.Single().Properties.Single().Name);
            
            this.ersvc.VerifyAll();
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()), Times.Once);
        }

        #endregion 

        #region CreateNewFacet > CreateNewPropertyDefinition > Rollback

        [TestMethod]
        [TestCategory("CreateNewFacet"),TestCategory("CreateNewPropertyDefinition")]
        public void RollbackEditNewFacetViewModelWithPropertyInitializesAgain()
        {
            // ARRANGE

            var f1edit = vm.CreateNewFacet();

            f1edit.AddPropertyDefinition.Execute();
            f1edit.Properties.Single().Name = "pd1";

            // ACT

            f1edit.Rollback.Execute();

            // ASSERT

            Assert.AreEqual(KosmoGraph.Desktop.ViewModel.Properties.Resources.EditNewFacetViewModelNameDefault, f1edit.Name);
            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.AreEqual(0, f1edit.Properties.Count());
            Assert.AreEqual(0, this.vm.Facets.Count());
            Assert.AreEqual(0, this.vm.Items.Count);

            ersvc.VerifyAll();
            fsvc.VerifyAll();
        }

        #endregion 

        #region CreateNewFacet > CreateNewPropertyDefinition > Rollback > Commit

        [TestMethod]
        [TestCategory("CreateNewFacet"),TestCategory("CreateNewPropertyDefinition")]
        public void RollbackEditNewFacetViewModelWithPropertyAllowsEditingAgainTillCommit()
        {
            // ARRANGE

            this.fsvc // expect facet creation
                .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
                .Returns((Action<Facet> a) => Task.FromResult(FacetFactory.CreateNew(a)));

            var f1edit = vm.CreateNewFacet();

            f1edit.Name = "f1";
            f1edit.AddPropertyDefinition.Execute();
            f1edit.Properties.Single().Name = "pd1";
            f1edit.Rollback.Execute();

            // ACT

            f1edit.Name = "f1";
            f1edit.AddPropertyDefinition.Execute();
            f1edit.Properties.Single().Name = "pd1";
            f1edit.Commit.Execute();

            // ASSERT

            Assert.AreEqual("f1", f1edit.Name);
            Assert.IsFalse(f1edit.Commit.CanExecute());
            Assert.AreEqual(1, f1edit.Properties.Count());
            Assert.AreEqual("pd1", f1edit.Properties.Single().Name);

            Assert.AreEqual(1, this.vm.Facets.Count());
            Assert.AreEqual(1, this.vm.Items.Count);

            Assert.AreSame(this.vm.Items.Single(), this.vm.Facets.Single());
            Assert.AreEqual("f1", this.vm.Facets.Single().Name);
            Assert.AreEqual(1, this.vm.Facets.Single().Properties.Count());
            Assert.AreEqual("pd1", this.vm.Facets.Single().Properties.Single().Name);

            ersvc.VerifyAll();
            fsvc.VerifyAll();
            fsvc.Verify(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()), Times.Once);
        }

        #endregion 
    }
}
