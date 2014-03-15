namespace KosmoGraph.Model.Test
{
    using System;
    using System.Linq;
    using KosmoGraph.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AssignEntityFacetPropertyValueTest
    {
        [TestMethod]
        public void AssignEntityFacetPropertyValue()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var af1 = e1.Add(e1.CreateNewAssignedFacet(f1));
            
            // ACT

            e1.AssignedFacets.First().Properties.First().Value = "value";

            // ASSERT
            // entity id and facet id assigned

            Assert.AreEqual("value", e1.AssignedFacets.First().Properties.First().Value);
        }
    }
}