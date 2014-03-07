//namespace Kosmograph.Desktop.Test.ViewModel
//{
//    using System;
//    using Microsoft.VisualStudio.TestTools.UnitTesting;
//    using KosmoGraph.Desktop.ViewModel;
//    using System.Windows;

//    [TestClass]
//    public class TestRelationshipViewModelLayout
//    {
//        //[TestMethod]
//        public void MoveDestintationEntityToTheLeft()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 10;
//            e1.Top = 10;
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 60;
//            e2.Top = 60;
//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));
//            r1.MinSize = new Size(50, 50);

//            // ACT
//            // Move e1 closer to e2. e2 is pushed to enforce min size.

//            e1.Top = 20;

//            // ASSERT

//            Assert.AreEqual(10, e1.Left);
//            Assert.AreEqual(20, e1.Top);
//            Assert.AreEqual(60, e2.Left);
//            Assert.AreEqual(70, e2.Top);
//        }

//        //[TestMethod]
//        public void MoveDestintationEntityToTheTop()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 10;
//            e1.Top = 10;
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 60;
//            e2.Top = 60;
//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));
//            r1.MinSize = new Size(50, 50);

//            // ACT
//            // Move e1 closer to e2. e2 is pushed to enforce min size.

//            e1.Left = 20;

//            // ASSERT

//            Assert.AreEqual(20, e1.Left);
//            Assert.AreEqual(10, e1.Top);
//            Assert.AreEqual(70, e2.Left);
//            Assert.AreEqual(60, e2.Top);
//        }

//        //[TestMethod]
//        public void MoveSourceEntityToTheLeft()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 10;
//            e1.Top = 10;
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 60;
//            e2.Top = 60;
//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));
//            r1.MinSize = new Size(50, 50);

//            // ACT
//            // Move e1 closer to e2. e2 is pushed to enforce min size.

//            e2.Top = 50;

//            // ASSERT

//            Assert.AreEqual(10, e1.Left);
//            Assert.AreEqual(0, e1.Top);
//            Assert.AreEqual(60, e2.Left);
//            Assert.AreEqual(50, e2.Top);
//        }

//        //[TestMethod]
//        public void MoveSourceEntityToTheTop()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 10;
//            e1.Top = 10;
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 60;
//            e2.Top = 60;
//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));
//            r1.MinSize = new Size(50, 50);

//            // ACT
//            // Move e1 closer to e2. e2 is pushed to enforce min size.

//            e2.Left = 50;

//            // ASSERT

//            Assert.AreEqual(0, e1.Left);
//            Assert.AreEqual(10, e1.Top);
//            Assert.AreEqual(50, e2.Left);
//            Assert.AreEqual(60, e2.Top);
//        }

//        //[TestMethod]
//        public void MoveDestintationEntityToTheRight()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 60;
//            e1.Top = 60;
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 10;
//            e2.Top = 10;
//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));
//            r1.MinSize = new Size(50, 50);

//            // ACT
//            // Move e1 closer to e2. e2 is pushed to enforce min size.

//            e1.Top = 50;

//            // ASSERT

//            Assert.AreEqual(60, e1.Left);
//            Assert.AreEqual(50, e1.Top);
//            Assert.AreEqual(10, e2.Left);
//            Assert.AreEqual(0, e2.Top);
//        }

//        //[TestMethod]
//        public void MoveDestintationEntityToTheBottom()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 60;
//            e1.Top = 60;
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 10;
//            e2.Top = 10;
//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));
//            r1.MinSize = new Size(50, 50);

//            // ACT
//            // Move e1 closer to e2. e2 is pushed to enforce min size.

//            e1.Left = 50;

//            // ASSERT

//            Assert.AreEqual(50, e1.Left);
//            Assert.AreEqual(60, e1.Top);
//            Assert.AreEqual(0, e2.Left);
//            Assert.AreEqual(10, e2.Top);
//        }

//        //[TestMethod]
//        public void MoveSourceEntityToTheRight()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 60;
//            e1.Top = 60;
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 10;
//            e2.Top = 10;
//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));
//            r1.MinSize = new Size(50, 50);

//            // ACT
//            // Move e1 closer to e2. e2 is pushed to enforce min size.

//            e1.Top = 50;

//            // ASSERT

//            Assert.AreEqual(60, e1.Left);
//            Assert.AreEqual(50, e1.Top);
//            Assert.AreEqual(10, e2.Left);
//            Assert.AreEqual(0, e2.Top);
//        }

//        //[TestMethod]
//        public void MoveSourceEntityToTheBottom()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 60;
//            e1.Top = 60;
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 10;
//            e2.Top = 10;
//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));
//            r1.MinSize = new Size(50, 50);

//            // ACT
//            // Move e1 closer to e2. e2 is pushed to enforce min size.

//            e1.Left = 50;

//            // ASSERT

//            Assert.AreEqual(50, e1.Left);
//            Assert.AreEqual(60, e1.Top);
//            Assert.AreEqual(0, e2.Left);
//            Assert.AreEqual(10, e2.Top);
//        }

//        [TestMethod]
//        public void AreaCornersAreLinkedToSourceAndDestination()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 10;
//            e1.Top = 10;
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 50;
//            e2.Top = 50;
            
//            // ACT

//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));
            
//            // ASSERT

//            Assert.AreEqual(e1.Top, r1.Area.Top);
//            Assert.AreEqual(e1.Left, r1.Area.Left);
//            Assert.AreEqual(e2.Top, r1.Area.Bottom);
//            Assert.AreEqual(e2.Left, r1.Area.Right);
//            Assert.AreEqual(40, r1.Area.Height);
//            Assert.AreEqual(40, r1.Area.Width);
//        }

//        [TestMethod]
//        public void AreaHeightIsNotSmallerAsDesiredHeight()
//        {
//            // ARRANGE
//            // Area between entities is is 40x40

//            var vm = EntityRelationshipViewModel.CreateNew();
            
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 10;
//            e1.Top = 10;
            
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 50;
//            e2.Top = 50;

//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));
            
//            // ACT
//            // Area need is 100x100

//            r1.MinSize = new Size(100, 100);
            
//            // ASSERT
//            // area size is 100x100
//            // e1 moved left/up -30
//            // e2 moved right down 30

//            Assert.AreEqual(100, r1.Area.Height);
//            Assert.AreEqual(100, r1.Area.Width);
//            Assert.AreEqual(-20, r1.Area.Top);
//            Assert.AreEqual(-20, r1.Area.Left);
//            Assert.AreEqual(80, r1.Area.Bottom);
//            Assert.AreEqual(80, r1.Area.Right);
//        }

//        [TestMethod]
//        public void IncreasingTheMinSizePushesFromAndToAway()
//        {
//            // ARRANGE

//            var vm = EntityRelationshipViewModel.CreateNew();
//            var e1 = vm.Add(vm.CreateNewEntity("e1"));
//            e1.Left = 50;
//            e1.Top = 50;
//            var e2 = vm.Add(vm.CreateNewEntity("e2"));
//            e2.Left = 100;
//            e2.Top = 100;
//            var r1 = vm.Add(vm.CreateNewRelationship(e1, e2));

//            // ACT

//            r1.MinSize = new Size(100, 100);

//            // ASSERT
//            Assert.AreEqual(25, r1.Area.Top);
//            Assert.AreEqual(25, r1.Area.Left);
//            Assert.AreEqual(125, r1.Area.Right);
//            Assert.AreEqual(125, r1.Area.Bottom);
//            Assert.AreEqual(e1.Top, r1.Area.Top);
//            Assert.AreEqual(e1.Left, r1.Area.Left);
//            Assert.AreEqual(e2.Top, r1.Area.Bottom);
//            Assert.AreEqual(e2.Left, r1.Area.Right);
//        }
//    }
//}
