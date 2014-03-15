//namespace Kosmograph.Desktop.Test.ViewModel
//{
//    using System;
//    using System.Linq;
//    using Microsoft.VisualStudio.TestTools.UnitTesting;
//    using KosmoGraph.Desktop.ViewModel;
//    using KosmoGraph.Test;
//    using Moq;
//    using KosmoGraph.Services;
//    using KosmoGraph.Model;

//    [TestClass]
//    public class CreateNewEntityFacetViewModelTest
//    {
//        #region Assign tag without property definition

//        [TestMethod]
//        [TestCategory("CreateNewEntityFacet")]
//        public void CreateNewEmptyEntityFacetViewModel()
//        {
//            // ARRANGE
            
//            var ersvc = new Mock<IManageEntitiesAndRelationships>();
            
//            ersvc // expects entity creation
//                .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
//                .Returns((Action<Entity> a) => Entity.Factory.CreateNew(a));

//            var fsvc = new Mock<IManageFacets>();

//            fsvc // expect facet creation
//                .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
//                .Returns((Action<Facet> a) => Facet.Factory.CreateNew(a));

//            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var f1 = vm.Add(vm.CreateNewFacet("f1"));

//            // ACT
            
//            AssignedEntityTagViewModel af1 = e1.CreateNewAssignedTag(f1);

//            // ASSERT
//            // create an asigned facet instance that represents the connecton from entity to tag
//            // but don't add it to the entity now

//            Assert.AreSame(f1, af1.Tag);
//            Assert.AreEqual("f1", af1.Tag.Name);
//            Assert.AreSame(e1, af1.Entity);
//            Assert.AreEqual("e1", af1.Entity.Name);
//            Assert.AreEqual(f1.ModelItem.Id, af1.ModelItem.FacetId);
//            Assert.AreEqual(0, e1.AssignedTags.Count);
//            Assert.AreEqual(0, e1.ModelItem.AssignedFacets.Count());
//        }

//        [TestMethod]
//        [TestCategory("CreateNewEntityFacet")]
//        public void AddNewEmptyEntityFacetViewModel()
//        {
//            // ARRANGE

//            var ersvc = new Mock<IManageEntitiesAndRelationships>();

//            ersvc // expects entity creation
//                .Setup(_ => _.CreateNewEntity(It.IsAny<Action<Entity>>()))
//                .Returns((Action<Entity> a) => Entity.Factory.CreateNew(a));

//            var fsvc = new Mock<IManageFacets>();

//            fsvc // expect facet creation
//                .Setup(_ => _.CreateNewFacet(It.IsAny<Action<Facet>>()))
//                .Returns((Action<Facet> a) => Facet.Factory.CreateNew(a));

//            var vm = new EntityRelationshipViewModel(ersvc.Object, fsvc.Object);
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var f1 = vm.Add(vm.CreateNewFacet("f1"));
//            var af1 = e1.CreateNewAssignedTag(f1);
            
//            // ACT
            
//            AssignedEntityTagViewModel result = e1.Add(af1);

//            // ASSERT
//            // assigning e1 f1 to an e1 creates an 'AssignedEntityTagViewModel' instance.

//            Assert.AreSame(f1, af1.Tag);
//            Assert.AreEqual("f1", af1.Tag.Name);
//            Assert.AreSame(e1, af1.Entity);
//            Assert.AreSame("e1", af1.Entity.Name);
//            Assert.AreEqual(f1.ModelItem.Id, af1.ModelItem.FacetId);
//            Assert.AreEqual(1, e1.AssignedTags.Count);
//            Assert.AreEqual(1, e1.ModelItem.AssignedFacets.Count());
//        }
        
//        #endregion 

//        #region Error cases

//        [TestMethod]
//        public void DontAddNullReference()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
            
//            // ACT & ASSERT
//            // assigning null throws exception

//            ExceptionAssert.Throws<ArgumentNullException>(delegate { e1.Add(null);});
//        }

//        [TestMethod]
//        public void DontAddTagToEntityTwice()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var f1 = vm.Add(vm.CreateNewFacet("f1"));

//            // ACT
//            // assigning e1 f1 twice returns the same AssignedTagViewModel instance
            
//            AssignedEntityTagViewModel assignedTag1 = e1.Add(e1.CreateNewAssignedTag(f1));
//            AssignedEntityTagViewModel assignedTag2 = e1.Add(e1.CreateNewAssignedTag(f1));

//            // ASSERT

//            Assert.AreSame(assignedTag2, assignedTag1);
//        }

//        #endregion 

//        #region Assign tag with property definiition

//        [TestMethod]
//        public void CreateAssigendTagWithPropertyDefinition()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var f1 = vm.Add(vm.CreateNewFacet("f1"));
//            var pd1 = f1.Add(f1.CreateNewPropertyDefinition("pd1"));

//            // ACT
//            // assigning e1 f1 to an e1 creates an 'AssignedEntityTagViewModel' instance.

//            AssignedEntityTagViewModel af1 = e1.CreateNewAssignedTag(f1);

//            // ASSERT

//            Assert.AreEqual(1, af1.Properties.Count());
//            Assert.AreEqual(1, af1.ModelItem.Properties.Count());
//            Assert.AreSame(af1.Properties.First().ModelItem, af1.ModelItem.Properties.First());
//            Assert.AreNotEqual(Guid.Empty, pd1.ModelItem.Id);
//            Assert.AreEqual(pd1, af1.Properties.First().Definition);
//            Assert.AreEqual(pd1.ModelItem.Id, af1.Properties.First().ModelItem.DefinitionId);
//            Assert.AreEqual(f1, af1.Properties.First().Definition.Tag);
//            Assert.AreEqual(0, e1.Properties.Count);
//        }

//        [TestMethod]
//        public void AddTagWithPropertyDeifniitionToEntity()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var f1 = vm.Add(vm.CreateNewFacet("f1"));
//            var pd1 = f1.Add(f1.CreateNewPropertyDefinition("pd1"));
//            var af1 = e1.CreateNewAssignedTag(f1);

//            // ACT
//            // assigning e1 f1 to an e1 creates an 'AssignedEntityTagViewModel' instance.
            
//            AssignedEntityTagViewModel result = e1.Add(af1);
            
//            // ASSERT

//            Assert.AreSame(result, af1);
//            Assert.AreEqual(1, e1.Properties.Count());
//            Assert.AreEqual(1, af1.Properties.Count());
//            Assert.AreSame(e1.Properties.First(), af1.Properties.First());
//            Assert.AreEqual(1, af1.ModelItem.Properties.Count());
//            Assert.AreSame(af1.Properties.First().ModelItem, af1.ModelItem.Properties.First());
//            Assert.AreEqual(pd1, af1.Properties.First().Definition);
//            Assert.AreNotEqual(Guid.Empty,pd1.ModelItem.Id);
//            Assert.AreEqual(pd1.ModelItem.Id, af1.Properties.First().ModelItem.DefinitionId);
//            Assert.AreEqual(f1, af1.Properties.First().Definition.Tag);
//        }

//        #endregion 

//        #region Propagate changes at Tag to Assigned Tag and Entity

//        [TestMethod]
//        public void AddPropertyDefinitionToTagAddsPropertyValueToAssigedTag()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var f1 = vm.Add(vm.CreateNewFacet("f1"));
//            var pd1 = f1.Add(f1.CreateNewPropertyDefinition("pd1"));
//            var af1 = e1.Add(e1.CreateNewAssignedTag(f1));

//            // ACT
//            // Assign another property definition to the entity, make it visible in the property list

//            PropertyDefinitionViewModel pd2 = f1.Add(f1.CreateNewPropertyDefinition("pd2"));

//            // ASSERT

//            Assert.AreEqual(2, e1.Properties.Count());
//            Assert.AreEqual(2, e1.AssignedTags.First().Properties.Count());
//            Assert.AreEqual(2, e1.ModelItem.AssignedFacets.First().Properties.Count());
//            Assert.AreSame(e1.Properties.ElementAt(0), af1.Properties.ElementAt(0));
//            Assert.AreSame(e1.Properties.ElementAt(1), af1.Properties.ElementAt(1));
//            Assert.AreSame(af1.Properties.ElementAt(0).ModelItem, af1.ModelItem.Properties.ElementAt(0));
//            Assert.AreSame(e1.Properties.ElementAt(1).ModelItem, af1.ModelItem.Properties.ElementAt(1));
//        }

//        [TestMethod]
//        public void RemovePropertyDefinitionFromTagRemovesPropertyValueFromAssigedTag()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var f1 = vm.Add(vm.CreateNewFacet("f1"));
//            var pd1 = f1.Add(f1.CreateNewPropertyDefinition("pd1"));
//            e1.Add(e1.CreateNewAssignedTag(f1));

//            // ACT
//            // Assign another property definition to the entity, make it visible in the property list

//            f1.Remove(f1.Properties.First());

//            // ASSERT

//            Assert.AreEqual(0, e1.Properties.Count());
//            Assert.AreEqual(0, e1.AssignedTags.First().Properties.Count());
//            Assert.AreEqual(0, e1.ModelItem.AssignedFacets.First().Properties.Count());
//        }

//        #endregion 

//        [TestMethod]
//        public void EntityPropagatesSelectedStateToTag()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.IsSelected = true;
//            var f1 = vm.Add(vm.CreateNewFacet("f1"));

//            // ACT
//            // assigning f1 to e1 selected e1 make f1 selectes too.

//            AssignedEntityTagViewModel af1 = e1.Add(e1.CreateNewAssignedTag(f1));

//            // ASSERT

//            Assert.IsTrue(af1.Tag.IsItemSelected);
//        }
//    }
//}
