//namespace Kosmograph.Desktop.Test.ViewModel
//{
//    using System;
//    using Microsoft.VisualStudio.TestTools.UnitTesting;
//    using KosmoGraph.Desktop.ViewModel;

//    [TestClass]
//    public class TestSelectionOfItem
//    {
//        [TestMethod]
//        public void SelectEntityBySelectingTag()
//        {
//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var at1 = e1.Add(e1.CreateNewAssignedTag(t1));
//            var t2 = vm.Add(vm.CreateNewFacet("t2"));
            
//            // ACT
//            // selecting an e1 selects indirectly the tag as well

//            e1.IsSelected = true;
            
//            // ASSERT

//            Assert.IsTrue(e1.IsSelected);
//            Assert.IsFalse(t1.IsSelected);
//            Assert.IsTrue(t1.IsItemSelected);
//            Assert.IsFalse(t2.IsSelected);
//            Assert.IsFalse(t2.IsItemSelected);
//        }

//        [TestMethod]
//        public void ClearingEntitySelectionsClearsTagItemSelected()
//        {
//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var at1 = e1.Add(e1.CreateNewAssignedTag(t1));
//            var t2 = vm.Add(vm.CreateNewFacet("t2"));
//            e1.IsSelected = true;

//            // ACT
//            // unselecting an e1 selects indirectly the tag as well

//            vm.ClearSelectedItems();

//            // ASSERT

//            Assert.IsFalse(e1.IsSelected);
//            Assert.IsFalse(t1.IsSelected);
//            Assert.IsFalse(t1.IsItemSelected);
//            Assert.IsFalse(t2.IsSelected);
//            Assert.IsFalse(t2.IsItemSelected);
//        }

//        [TestMethod]
//        public void SelectRelationshipBySelectingTag()
//        {
//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            var r1 = vm.Add(vm.CreateNewRelationship(e1,e2));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var at1 = r1.Add(r1.CreateNewAssignedRelationshipTag(t1));
//            var t2 = vm.Add(vm.CreateNewFacet("t2"));

//            // ACT
//            // selecting an e1 selects indirectly the tag as well       

//            r1.IsSelected = true;

//            // ASSERT

//            Assert.IsTrue(r1.IsSelected);
//            Assert.IsFalse(t1.IsSelected);
//            Assert.IsTrue(t1.IsItemSelected);
//            Assert.IsFalse(t2.IsSelected);
//            Assert.IsFalse(t2.IsItemSelected);
//        }

//        [TestMethod]
//        public void ClearingRelationshipSelectionsClearsTagItemSelected()
//        {
//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            var r1 = vm.Add(vm.CreateNewRelationship(e1,e2));
//            var t1 = vm.Add(vm.CreateNewFacet("t1"));
//            var at1 = r1.Add(r1.CreateNewAssignedRelationshipTag(t1));
//            var t2 = vm.Add(vm.CreateNewFacet("t2"));
//            r1.IsSelected = true;

//            // ACT
//            // crealing the sekections clear tag utem selected as well

//            vm.ClearSelectedItems();

//            // ASSERT

//            Assert.IsFalse(r1.IsSelected);
//            Assert.IsFalse(t1.IsSelected);
//            Assert.IsFalse(t1.IsItemSelected);
//            Assert.IsFalse(t2.IsSelected);
//            Assert.IsFalse(t2.IsItemSelected);
//        }
//    }
//}
