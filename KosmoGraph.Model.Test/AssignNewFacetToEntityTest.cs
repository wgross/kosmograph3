namespace KosmoGraph.Model.Test
{
    using System;
    using System.Linq;
    using KosmoGraph.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AssignNewFacetToEntityTest
    {
        [TestMethod]
        public void ThrowsOnMissingArgument()
        {
            // ASSERT
            // throws Argument null exceptin if a parameter is missing

            ExceptionAssert.Throws<ArgumentNullException>(delegate { AssignedFacetFactory.CreateNew((Entity)null, null); });
            ExceptionAssert.Throws<ArgumentNullException>(delegate { AssignedFacetFactory.CreateNew(Entity.Factory.CreateNew(delegate { }), null); });
            ExceptionAssert.Throws<ArgumentNullException>(delegate { AssignedFacetFactory.CreateNew((Entity)null, Facet.Factory.CreateNew(delegate{})); });
        }

        [TestMethod]
        public void AssignEmptyFacetToEntity()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e=>e.Name = "e1");
            var f1 = Facet.Factory.CreateNew(f=>f.Name="f1");

            // ACT

            AssignedFacet result = e1.Add(e1.CreateNewAssignedFacet(f1));
            
            // ASSERT
            // entity id and facet id assigned

            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreEqual(f1.Id, e1.AssignedFacets.First().FacetId);
        }

        [TestMethod]
        public void AssignFacetWithPropertyToEntity()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var f1 = Facet.Factory.CreateNew(f => f.Name = "f1");
            var pd1 = f1.Add(f1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));

            // ACT

            AssignedFacet result = e1.Add(e1.CreateNewAssignedFacet(f1));

            // ASSERT
            // entity id and facet id assigned

            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreEqual(f1.Id, e1.AssignedFacets.First().FacetId);
            Assert.AreEqual(1, e1.AssignedFacets.First().Properties.Count());
            Assert.AreEqual(pd1.Id, e1.AssignedFacets.First().Properties.First().DefinitionId);
        }
    }
}