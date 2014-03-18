namespace KosmoGraph.Desktop.ViewModel.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using KosmoGraph.Model;
    using KosmoGraph.Services;
    using Moq;
    using KosmoGraph.Test;
    using System.Threading;
    using System.Threading.Tasks;

    [TestClass]
    public class CreateNewRelationshipInteractivelyTest
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

            this.facets = new[]
            {
                Facet.Factory.CreateNew(f => 
                {
                    f.Name = "f1";
                    f.Add(f.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
                })
            };


            this.fsvc = new Mock<IManageFacets>();

            this.fsvc // expect retrieval of all facets
                .Setup(_ => _.GetAllFacets())
                .Returns(Task.FromResult(this.facets));

            this.entities = new[] 
            {
                Entity.Factory.CreateNew(e=>e.Name = "e1"),
                Entity.Factory.CreateNew(e=>e.Name = "e2"),
            };

            this.relationships = Enumerable.Empty<Relationship>();

            this.ersvc = new Mock<IManageEntitiesAndRelationships>();

            ersvc // expects retrueval aof all entities
                .Setup(_ => _.GetAllEntities())
                .Returns(Task.FromResult(this.entities));

            ersvc // expect retrieval of existig relationships
                .Setup(_ => _.GetAllRelationships())
                .Returns(Task.FromResult(this.relationships));

            this.vm = new EntityRelationshipViewModel(this.ersvc.Object, this.fsvc.Object);
        }

        #region CreatNewRelationship

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void CreateNewRelationshipViewModelWithStartEntityWithoutDestinationEntity()
        {
            // ARRANGE

            // ACT

            EditNewRelationshipViewModel r1edit = this.vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

            // ASSERT

            Assert.AreSame(this.vm.Entities.ElementAt(0), r1edit.From);
            Assert.IsNull(r1edit.To);
            Assert.AreEqual(string.Format(Properties.Resources.EditNewRelationshipViewModelTitle, this.entities.ElementAt(0).Name, "?"), r1edit.Title);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.IsTrue(r1edit.SetDestination.CanExecute(this.vm.Entities.ElementAt(1)));
            Assert.IsFalse(r1edit.SetDestination.CanExecute(this.vm.Entities.ElementAt(0)));
            Assert.AreEqual(0, this.vm.Relationships.Count());
            Assert.AreEqual(3, this.vm.Items.Count());
            Assert.AreSame(r1edit, this.vm.Items.Last());
            

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }

        #endregion 

        #region CreateNewRelationship > SetDestination

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void SetDestinationOfNewRelationshipViewModel()
        {
            // ARRANGE

            var r1edit = this.vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

            // ACT

            r1edit.SetDestination.Execute(this.vm.Entities.ElementAt(1));

            // ASSERT

            Assert.AreSame(this.vm.Entities.ElementAt(0), r1edit.From);
            Assert.AreSame(this.vm.Entities.ElementAt(1), r1edit.To);
            Assert.AreEqual(string.Format(Properties.Resources.EditNewRelationshipViewModelTitle, this.entities.ElementAt(0).Name, this.entities.ElementAt(1).Name), r1edit.Title);
            Assert.IsTrue(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.IsFalse(r1edit.SetDestination.CanExecute(this.vm.Entities.ElementAt(1)));
            Assert.IsFalse(r1edit.SetDestination.CanExecute(this.vm.Entities.ElementAt(0)));
            Assert.AreEqual(0, this.vm.Relationships.Count());
            Assert.AreEqual(3, this.vm.Items.Count());
            Assert.AreSame(r1edit, this.vm.Items.Last());
            
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }

        #endregion 

        #region CreateNewRelationship > SetDestination > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void CommitNewlyCreatedRelationshipViewModel()
        {
            // ARRANGE

            ersvc // expect creation of partial relationship with source entity
                .Setup(_ => _.CreatePartialRelationship(this.entities.ElementAt(0), It.IsAny<Action<Relationship>>()))
                .Returns<Entity, Action<Relationship>>((e, a) => Relationship.Factory.CreateNewPartial(e));

            ersvc
                .Setup(_ => _.CompletePartialRelationship(It.Is<Relationship>(r => r.FromId == this.entities.ElementAt(0).Id), this.entities.ElementAt(1)))
                .Returns<Relationship, Entity>((r, e) => Task.FromResult(new CompletePartialRelationshipResult(r, this.entities.ElementAt(0), e)));

            var r1edit = this.vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

            r1edit.SetDestination.Execute(this.vm.Entities.ElementAt(1));

            // ACT

            r1edit.Commit.Execute();

            // ASSERT

            Assert.AreSame(this.vm.Entities.ElementAt(0), r1edit.From);
            Assert.AreSame(this.vm.Entities.ElementAt(1), r1edit.To);
            Assert.AreEqual(string.Format(Properties.Resources.EditNewRelationshipViewModelTitle, this.entities.ElementAt(0).Name, this.entities.ElementAt(1).Name), r1edit.Title);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.IsFalse(r1edit.SetDestination.CanExecute(this.vm.Entities.ElementAt(1)));
            Assert.IsFalse(r1edit.SetDestination.CanExecute(this.vm.Entities.ElementAt(0)));
            Assert.AreEqual(1, this.vm.Relationships.Count());
            Assert.AreSame(this.vm.Entities.ElementAt(0), this.vm.Relationships.Single().From.Entity);
            Assert.AreSame(this.vm.Entities.ElementAt(1), this.vm.Relationships.Single().To.Entity);
            Assert.AreEqual(3, this.vm.Items.Count());
            Assert.AreSame(this.vm.Items.ElementAt(2), this.vm.Relationships.Single());
            
            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.CreatePartialRelationship(It.IsAny<Entity>(), It.IsAny<Action<Relationship>>()), Times.Once);
            this.ersvc.Verify(_ => _.CompletePartialRelationship(It.IsAny<Relationship>(), It.IsAny<Entity>()),Times.Once);    
        }

        #endregion 

        #region CreateNewRelationship > SetDestination > Rollback

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void RollbackNewRelationshipViewModelInitializeasAgainWithoutDestination()
        {
            // ARRANGE

            var r1edit = this.vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));
            
            r1edit.SetDestination.Execute(this.vm.Entities.ElementAt(1));

            // ACT

            r1edit.Rollback.Execute();
            
            // ASSERT

            Assert.AreSame(this.vm.Entities.ElementAt(0), r1edit.From);
            Assert.IsNull(r1edit.To);
            Assert.AreEqual(string.Format(Properties.Resources.EditNewRelationshipViewModelTitle, this.entities.ElementAt(0).Name, "?"), r1edit.Title);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsTrue(r1edit.Rollback.CanExecute());
            Assert.IsTrue(r1edit.SetDestination.CanExecute(this.vm.Entities.ElementAt(1)));
            Assert.IsFalse(r1edit.SetDestination.CanExecute(this.vm.Entities.ElementAt(0)));
            Assert.AreEqual(0, this.vm.Relationships.Count());
            Assert.AreEqual(2, this.vm.Items.Count());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
        }

        #endregion 

        #region CreateNewRelationship > SetDestination > Rollback > Commit

        [TestMethod]
        [TestCategory("CreateNewRelationship")]
        public void RollbackNewlyCreatedRelationshipViewModelAllowsAgainCommit()
        {
            // ARRANGE

            ersvc // expect creation of partial relationship with source entity
                .Setup(_ => _.CreatePartialRelationship(this.entities.ElementAt(0), It.IsAny<Action<Relationship>>()))
                .Returns<Entity, Action<Relationship>>((e, a) => Relationship.Factory.CreateNewPartial(e));

            ersvc
                .Setup(_ => _.CompletePartialRelationship(It.Is<Relationship>(r => r.FromId == this.entities.ElementAt(0).Id), this.entities.ElementAt(1)))
                .Returns<Relationship, Entity>((r, e) => Task.FromResult(new CompletePartialRelationshipResult(r, this.entities.ElementAt(0), e)));

            var r1edit = this.vm.CreatePendingRelationship(this.vm.Entities.ElementAt(0));

            r1edit.SetDestination.Execute(this.vm.Entities.ElementAt(1));
            r1edit.Rollback.Execute();

            // ACT

            r1edit.SetDestination.Execute(this.vm.Entities.ElementAt(1));
            r1edit.Commit.Execute();

            // ASSERT

            Assert.AreSame(this.vm.Entities.ElementAt(0), r1edit.From);
            Assert.AreSame(this.vm.Entities.ElementAt(1), r1edit.To);
            Assert.AreEqual(string.Format(Properties.Resources.EditNewRelationshipViewModelTitle, this.entities.ElementAt(0).Name, this.entities.ElementAt(1).Name), r1edit.Title);
            Assert.IsFalse(r1edit.Commit.CanExecute());
            Assert.IsFalse(r1edit.Rollback.CanExecute());
            Assert.IsFalse(r1edit.SetDestination.CanExecute(this.vm.Entities.ElementAt(1)));
            Assert.IsFalse(r1edit.SetDestination.CanExecute(this.vm.Entities.ElementAt(0)));
            Assert.AreEqual(1, this.vm.Relationships.Count());
            Assert.AreSame(this.vm.Entities.ElementAt(0), this.vm.Relationships.Single().From.Entity);
            Assert.AreSame(this.vm.Entities.ElementAt(1), this.vm.Relationships.Single().To.Entity);
            Assert.AreEqual(3, this.vm.Items.Count());
            Assert.AreSame(this.vm.Items.ElementAt(2), this.vm.Relationships.Single());

            this.fsvc.VerifyAll();
            this.fsvc.Verify(_ => _.GetAllFacets(), Times.Once);
            this.ersvc.VerifyAll();
            this.ersvc.Verify(_ => _.GetAllEntities(), Times.Once);
            this.ersvc.Verify(_ => _.GetAllRelationships(), Times.Once);
            this.ersvc.Verify(_ => _.CreatePartialRelationship(It.IsAny<Entity>(), It.IsAny<Action<Relationship>>()), Times.Once);
            this.ersvc.Verify(_ => _.CompletePartialRelationship(It.IsAny<Relationship>(), It.IsAny<Entity>()), Times.Once);
        }

        #endregion 
    }
}
