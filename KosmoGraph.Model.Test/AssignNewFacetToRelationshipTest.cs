﻿namespace KosmoGraph.Model.Test
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
            ExceptionAssert.Throws<ArgumentNullException>(delegate { AssignedFacetFactory.CreateNew((Relationship)null, Facet.Factory.CreateNew(delegate{})); });
        }


        [TestMethod]
        public void AssignFacetToTemporaryRelationship()
        {
            // ARRANGE

            var e1 = Entity.Factory.CreateNew(delegate {});
            var r1 = Relationship.Factory.CreateNewPartial(e1);
            var f1 = Facet.Factory.CreateNew(delegate{});

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

            var e1 = Entity.Factory.CreateNew(e => e.Name = "e2");
            var e2 = Entity.Factory.CreateNew(e => e.Name = "e1");
            var r1 = Relationship.Factory.CreateNew(e1, e2);
            var f1 = Facet.Factory.CreateNew(f => f.Name="f1" );

            // ACT

            AssignedFacet result = r1.Add(r1.CreateNewAssignedFacet(f1, null));

            // ASSERT
            // relationship id and facet id assigned

            Assert.AreEqual(1, r1.AssignedFacets.Count());
            Assert.AreEqual(f1.Id, r1.AssignedFacets.First().FacetId);
        }
    }
}
