namespace KosmoGraph.Model.Test
{
    using System;
    using System.Linq;
    using KosmoGraph.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AssignRelationshipFacetPropertyValueTest
    {
        [TestMethod]
        public void AssignEntityFacetPropertyValueAtTemporaryRelationship()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var r1 = Relationship.Factory.CreateNewPartial(e1);
            var af1 = r1.Add(r1.CreateNewAssignedFacet(f1,null));
            
            // ACT

            af1.Properties.First().Value = "value";

            // ASSERT
            // relationship property vale has changed

            Assert.AreEqual("value", r1.AssignedFacets.First().Properties.First().Value);
        }

        [TestMethod]
        public void AssignEntityFacetPropertyValueAtFinalRelationship()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var e2 = Entity.Factory.CreateNew(e => e.Name = "e2");
            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var r1 = Relationship.Factory.CreateNew(e1,e2);
            var af1 = r1.Add(r1.CreateNewAssignedFacet(f1, null));

            // ACT

            r1.AssignedFacets.First().Properties.First().Value = "value";

            // ASSERT
            // relationship property vale has changed

            Assert.AreEqual("value", r1.AssignedFacets.First().Properties.First().Value);
        }
    }
}