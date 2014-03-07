namespace KosmoGraph.Model.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;

    [TestClass]
    public class TestAssignTagToEntity
    {
        #region Create and add a Tag with PropertyDefinition to an Entity
        
        [TestMethod]
        public void CreateAndInitializeAssignedTagAtEntity()
        {
            // ARRAMGE

            var t1 = FacetFactory.CreateNew();
            var e1 = EntityFactory.CreateNew();

            // ACT 
            // create assign tag for entity but dont add it

            AssignedFacet at1 = e1.CreateNewAssignedFacet(t1, delegate { });

            // ASSERT

            Assert.IsNotNull(at1);
            Assert.AreNotEqual(Guid.Empty, at1.FacetId);
            Assert.AreEqual(t1.Id, at1.FacetId);
            Assert.IsNotNull(at1.Properties);
            Assert.AreEqual(0, at1.Properties.Count());
            Assert.AreEqual(0, e1.AssignedFacets.Count());
        }

        [TestMethod]
        public void CreateAndInitializeAssignedTagWithPropertyAtEntity()
        {
            // ARRAMGE

            var t1 = FacetFactory.CreateNew();
            var e1 = EntityFactory.CreateNew();
            var pd1 = t1.Add(t1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
           
            // ACT 
            // creatze assign tag for entity but dont add it

            AssignedFacet at1 = e1.CreateNewAssignedFacet(t1, delegate { });

            // ASSERT

            Assert.IsNotNull(at1);
            Assert.AreNotEqual(Guid.Empty, at1.FacetId);
            Assert.AreEqual(t1.Id, at1.FacetId);
            Assert.IsNotNull(at1.Properties);
            Assert.AreEqual(1, at1.Properties.Count());
            Assert.AreEqual(0, e1.AssignedFacets.Count());
        }

        [TestMethod]
        public void AddAssignedTagWithPropertyAtEntity()
        {
            // ARRAMGE

            var t1 = FacetFactory.CreateNew();
            var e1 = EntityFactory.CreateNew();
            var pd1 = t1.Add(t1.CreateNewPropertyDefinition(pd => pd.Name = "pd1"));
            var at1 = e1.CreateNewAssignedFacet(t1, delegate { });

            // ACT 
            // add assigned tag to entity

            AssignedFacet result = e1.Add(at1);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreSame(at1, result);
            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreSame(at1, e1.AssignedFacets.First());
            Assert.AreEqual(1, e1.AssignedFacets.First().Properties.Count());
            Assert.AreEqual(pd1.Id, e1.AssignedFacets.First().Properties.First().DefinitionId);
        }

        #endregion 

        [TestMethod]
        public void DontAddTagTwiceToEntity()
        {
            // ARRAMGE

            var t1 = FacetFactory.CreateNew();
            var e1 = EntityFactory.CreateNew();
            var at1 = e1.Add(e1.CreateNewAssignedFacet(t1, delegate { }));

            // ACT 
            // add assigned tag to entity

            AssignedFacet result = e1.Add(at1);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreSame(at1, result);
            Assert.AreEqual(1, e1.AssignedFacets.Count());
            Assert.AreSame(at1, e1.AssignedFacets.First());
        }


        #region Invalid Arguments

        [TestMethod]
        public void ThrowOnAssigningNullTag()
        {
            // ARRAMGE

            var e1 = EntityFactory.CreateNew();

            // ACT & ASSERT
            // get exception on assingning null as Tag

            ExceptionAssert.Throws<ArgumentNullException>(delegate { e1.CreateNewAssignedFacet(null, delegate { }); });
        }

        [TestMethod]
        public void DontThrowOnMissingInitializer()
        {
            // ARRAMGE

            var t1 = FacetFactory.CreateNew();
            var e1 = EntityFactory.CreateNew();

            // ACT 
            // a missing initializer doesn't harm the creation

            AssignedFacet at1 = e1.CreateNewAssignedFacet(t1,null);

            // ASSERT

            Assert.IsNotNull(at1);
            Assert.AreNotEqual(Guid.Empty, at1.FacetId);
            Assert.AreEqual(t1.Id, at1.FacetId);
            Assert.IsNotNull(at1.Properties);
            Assert.AreEqual(0, at1.Properties.Count());
            Assert.AreEqual(0, e1.AssignedFacets.Count());
        }

        #endregion 
    }
}
