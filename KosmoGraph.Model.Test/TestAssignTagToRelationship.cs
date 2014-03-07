namespace KosmoGraph.Model.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;

    [TestClass]
    public class TestAssignTagToRelationship
    {
        #region Create and add a Tag with PropertyDefinition to an Entity

        [TestMethod]
        public void CreateAndInitilaizeAssignedTagAtRelationship()
        {
            // ARRANGE

            var t1 = new Facet();
            var r1 = new Relationship();

            // ACT
            // create assinged tag but don't add it.

            AssignedFacet at1 = r1.CreateNewAssignedFacet(t1, delegate { });

            // ASSERT

            Assert.IsNotNull(at1);
            Assert.AreNotEqual(Guid.Empty, at1.FacetId);
            Assert.AreEqual(t1.Id, at1.FacetId);
            Assert.IsNotNull(at1.Properties);
            Assert.AreEqual(0, at1.Properties.Count());
            Assert.AreEqual(0, r1.AssignedFacets.Count());
        }

        [TestMethod]
        public void CreateAndInitilaizeAssignedTagWithPropertyAtRelationship()
        {
            // ARRANGE

            var t1 = new Facet();
            var r1 = new Relationship();
            var pd1 = t1.Add(t1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var at1 = r1.CreateNewAssignedFacet(t1, delegate { });

            // ACT
            // add asigned tag assinged tag with property

            AssignedFacet result = r1.Add(at1);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreSame(at1, result);
            Assert.AreNotEqual(Guid.Empty, at1.FacetId);
            Assert.AreEqual(t1.Id, at1.FacetId);
            Assert.IsNotNull(at1.Properties);
            Assert.AreEqual(1, at1.Properties.Count());
            Assert.AreEqual(1, r1.AssignedFacets.Count());
            Assert.AreEqual(pd1.Id, r1.AssignedFacets.First().Properties.First().DefinitionId);
        }

        #endregion 

        [TestMethod]
        public void ThrowOnAssigningNullTag()
        {
            // ARRAMGE

            var r1 = new Relationship();

            // ACT & ASSERT
            // get exception on assingning null as Tag

            ExceptionAssert.Throws<ArgumentNullException>(delegate { r1.CreateNewAssignedFacet(null, delegate { }); });
        }

        [TestMethod]
        public void DontThrowOnMissingInitializer()
        {
            // ARRAMGE

            var t1 = new Facet();
            var r1 = new Relationship();

            // ACT 
            // a missing initializer doesn't harm the creation

            AssignedFacet at1 = r1.CreateNewAssignedFacet(t1, null);

            // ASSERT

            Assert.IsNotNull(at1);
            Assert.AreNotEqual(Guid.Empty, at1.FacetId);
            Assert.AreEqual(t1.Id, at1.FacetId);
            Assert.IsNotNull(at1.Properties);
            Assert.AreEqual(0, at1.Properties.Count());
            Assert.AreEqual(0, r1.AssignedFacets.Count());
        }
    }
}
