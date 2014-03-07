namespace KosmoGraph.Model.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;

    [TestClass]
    public class AssignNewFacetToRelationshipTest
    {
        [TestMethod]
        public void ThrowsOnMissingArgument()
        {
            // ASSERT
            // throws Argument null exception if a parameter is missing

            ExceptionAssert.Throws<ArgumentNullException>(delegate { AssignedFacetFactory.CreateNew((Relationship)null, null); });
            ExceptionAssert.Throws<ArgumentNullException>(delegate { AssignedFacetFactory.CreateNew(new Relationship(), null); });
            ExceptionAssert.Throws<ArgumentNullException>(delegate { AssignedFacetFactory.CreateNew((Relationship)null, FacetFactory.CreateNew()); });
        }


        [TestMethod]
        public void AssignFacetToTemporaryRelationship()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew();
            var r1 = RelationshipFactory.CreateNewPartial(e1);
            var f1 = FacetFactory.CreateNew();

            // ACT

            AssignedFacet result = r1.Add(r1.CreateNewAssignedFacet(f1,null));

            // ASSERT
            // relationship id and facet id assigned

            Assert.AreEqual(1, r1.AssignedFacets.Count());
            Assert.AreEqual(f1.Id, r1.AssignedFacets.First().FacetId);
        }

        [TestMethod]
        public void FinalRelationshipIdAndTagIdSet()
        {
            // ARRANGE

            var e1 = EntityFactory.CreateNew(e => e.Name = "e2");
            var e2 = EntityFactory.CreateNew(e => e.Name = "e1");
            var r1 = RelationshipFactory.CreateNew(e1, e2);
            var f1 = FacetFactory.CreateNew(f => f.Name="f1" );

            // ACT

            AssignedFacet result = r1.Add(r1.CreateNewAssignedFacet(f1, null));

            // ASSERT
            // relationship id and facet id assigned

            Assert.AreEqual(1, r1.AssignedFacets.Count());
            Assert.AreEqual(f1.Id, r1.AssignedFacets.First().FacetId);
        }
    }
}
