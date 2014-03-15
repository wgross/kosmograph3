namespace KosmoGraph.Model.Test
{
    using System;
    using System.Linq;
    using KosmoGraph.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CreateNewPropertyDefinitionTest
    {
        [TestMethod]
        public void ThrowOnMissingTag()
        {
            ExceptionAssert.Throws<ArgumentNullException>(delegate { PropertyDefinitionFactory.CreateNew(null); });
        }

        [TestMethod]
        public void CreateWithFacet()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(delegate{});
            
            // ACT

            PropertyDefinition result = f1.CreateNewPropertyDefinition(pd => pd.Name="pd1");

            // ASSERT
            // pd reference tag

            Assert.AreNotSame(Guid.Empty, result.Id);
        }

        [TestMethod]
        public void AddNewPropertyDefinitionToFacet()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(delegate{});
            var pd1 = f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1");

            // ACT

            PropertyDefinition result = f1.Add(pd1);

            // ASSERT
            // pd reference tag

            Assert.AreNotSame(pd1.Id, result.Id);
            Assert.AreSame(pd1,result);
            Assert.AreEqual(1, f1.Properties.Count());
            Assert.AreSame(pd1, f1.Properties.First());
        }

        [TestMethod]
        public void DontAddPropertyDefintionWithDuplicateName()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(delegate{});
            f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));

            // ACT

            ExceptionAssert.Throws<InvalidOperationException>(delegate { f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1")); });
        }

        [TestMethod]
        public void RemovePropertyDefinitionFromFacet()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(delegate{});
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));

            // ACT

            f1.Remove(pd1);

            // ASSERT

            Assert.AreEqual(0, f1.Properties.Count());
        }

        [TestMethod]
        public void DontThrowOnMissingPropertyDefinitionInitializer()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(delegate{});

            // ACT

            PropertyDefinition pd1 = f1.CreateNewPropertyDefinition(null);

            // ASSERT
            // create PD, dont throw

            Assert.IsNotNull(pd1);
            Assert.IsNull(pd1.Name);
            Assert.AreNotEqual(Guid.Empty, pd1.Id);
            Assert.AreEqual(0, f1.Properties.Count());
        }

        [TestMethod]
        public void ThrowOnAddNullPropertyDefinition()
        {
            // ARRANGE

            var f1 = Facet.Factory.CreateNew(delegate{});

            // ACT & ASSERT
            // adding null as property definition causes ArgumentNullException

            ExceptionAssert.Throws<ArgumentNullException>(delegate { f1.Add(null); });
        }
    }
}