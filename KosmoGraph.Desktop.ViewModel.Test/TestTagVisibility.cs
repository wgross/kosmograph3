//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using KosmoGraph.Desktop.ViewModel;

//namespace Kosmograph.Desktop.Test.ViewModel
//{
//    [TestClass]
//    public class TestTagVisibility
//    {
//        [TestMethod]
//        public void TagTogglesRelationshipsVisibleOn()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            var r1 = vm.Add(vm.CreateNewRelationship(e1,e2));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var at1 = r1.Add(r1.CreateNewAssignedRelationshipTag(t1));

//            // ACT
//            // toggling the visibility of e1 t1 makes the relationship and its entities remain invisible

//            t1.IsVisible = true;
            
//            // ASSERT

//            Assert.IsTrue(t1.IsVisible);
//            Assert.IsFalse(e1.IsVisible);
//            Assert.IsFalse(e2.IsVisible);
//            Assert.IsTrue(r1.IsVisible);
//        }

//        [TestMethod]
//        public void TagTogglesRelationshipsVisibleOff()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            var r1 = vm.Add(vm.CreateNewRelationship(e1,e2));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var at1 = r1.Add(r1.CreateNewAssignedRelationshipTag(t1));
//            t1.IsVisible = true;

//            // ACT
//            // toggling the visibility of e1 t1 makes the relationship invisible

//            t1.IsVisible = false;

//            // ASSERT

//            Assert.IsFalse(t1.IsVisible);
//            Assert.IsFalse(e1.IsVisible);
//            Assert.IsFalse(e2.IsVisible);
//            Assert.IsFalse(r1.IsVisible);
//        }

//        [TestMethod]
//        public void TagTogglesRelationshipsVisibleOnAfterAssign()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1") );
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            var r1 = vm.Add(vm.CreateNewRelationship(e1,e2));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var pd1 = t1.Add(t1.CreateNewPropertyDefinition("pd1"));
//            t1.IsVisible = true;           
            
//            // ACT
//            // adding the t1 to the relationship toggles the visiblilty on

//            var at1 = r1.Add(r1.CreateNewAssignedRelationshipTag(t1));

//            // ASSERT

//            Assert.IsTrue(t1.IsVisible);
//            Assert.IsFalse(e1.IsVisible);
//            Assert.IsFalse(e2.IsVisible);
//            Assert.IsTrue(r1.IsVisible);
//            Assert.AreEqual(1, r1.VisibleProperties.Count);
//            Assert.IsTrue(r1.HasVisibleProperties);
//        }

//        [TestMethod]
//        public void TagTogglesRelationshipsVisibleOffAfterUnassign()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            var r1 = vm.Add(vm.CreateNewRelationship(e1,e2));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var pd1 = t1.Add(t1.CreateNewPropertyDefinition("pd1"));
//            var at1 = r1.Add(r1.CreateNewAssignedRelationshipTag(t1));
            
//            t1.IsVisible = true;

//            // ACT
//            // remove the visible t1 makes relationship invisible

//            r1.Remove(at1);

//            // ASSERT

//            Assert.IsTrue(t1.IsVisible);
//            Assert.IsFalse(e1.IsVisible);
//            Assert.IsFalse(e2.IsVisible);
//            Assert.IsFalse(r1.IsVisible);
//            Assert.AreEqual(0,r1.VisibleProperties.Count);
//            Assert.IsFalse(r1.HasVisibleProperties);
//        }

//        [TestMethod]
//        public void TagTogglesEntityVisibleOn()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var pd1 = t1.Add(t1.CreateNewPropertyDefinition("pd1"));
//            var at1 = e1.Add(e1.CreateNewAssignedTag(t1));

//            // ACT
//            // toggling the visibility of e1 t1 makes the relationship and its entities remain invisible
//            // t1 property is added to e1 visible property list

//            t1.IsVisible = true;

//            // ASSERT

//            Assert.IsTrue(t1.IsVisible);
//            Assert.IsTrue(e1.IsVisible);
//            Assert.AreEqual(1, e1.VisibleProperties.Count);
//        }

//        [TestMethod]
//        public void TagTogglesEntityVisibleOff()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var pd1 = t1.Add(t1.CreateNewPropertyDefinition("pd1"));
//            var at1 = e1.Add(e1.CreateNewAssignedTag(t1));
//            t1.IsVisible = true;

//            // ACT
//            // toggling the visibility of e1 t1 makes the relationship and its entities remain invisible
//            // t1 property is removes from e1 visible properties list

//            t1.IsVisible = false;

//            // ASSERT

//            Assert.IsFalse(t1.IsVisible);
//            Assert.IsFalse(e1.IsVisible);
//            Assert.AreEqual(0, e1.VisibleProperties.Count);
//        }

//        [TestMethod]
//        public void TagTogglesEntityVisibleOnAfterAssign()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var pd1 = t1.Add(t1.CreateNewPropertyDefinition("pd1"));
//            t1.IsVisible = true;
            
//            // ACT
//            // assign the visible t1 makes the entty visible and adds its 
//            // property to the visible properties lost

//            var at1 = e1.Add(e1.CreateNewAssignedTag( t1));

//            // ASSERT

//            Assert.IsTrue(t1.IsVisible);
//            Assert.IsTrue(e1.IsVisible);
//            Assert.AreEqual(1, e1.VisibleProperties.Count);
//        }

//        [TestMethod]
//        public void TagTogglesEntityVisibleOffAfterUnassign()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var pd1 = t1.Add(t1.CreateNewPropertyDefinition("pd1"));
//            var at1 = e1.Add(e1.CreateNewAssignedTag( t1));
//            t1.IsVisible = true;

//            // ACT
//            // unsassign the tags make the e1 invisible and removes t1 properties 
//            // from visible propertty list of e1

//            e1.Remove(at1);

//            // ASSERT

//            Assert.IsTrue(t1.IsVisible);
//            Assert.IsFalse(e1.IsVisible);
//            Assert.AreEqual(0, e1.VisibleProperties.Count);
//        }
//    }
//}
