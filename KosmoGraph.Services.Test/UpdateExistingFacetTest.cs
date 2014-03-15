namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using Moq;
    using KosmoGraph.Model;
    using System.Threading;

    [TestClass]
    public class UpdateExistingFacetTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());
        }

        [TestMethod]
        [TestCategory("EditFacet"),TestCategory("RemovePropertyDefinition")]
        public void UpdateExistingFacetInDb()
        {
            // ARRANGE

            var facets = new []
            {
                Facet.Factory.CreateNew(e => e.Name = "f1")
            };
            
            var facetRepository = new Mock<IFacetRepository>();

            facetRepository // expect retrieval of existing facets
                .Setup(_ => _.GetAll())
                .Returns(facets.AsEnumerable());

            facetRepository // expects a new facet with name 'f1' and returns it
                .Setup(_ => _.Update(facets.First()))
                .Returns((Facet e) => e);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new FacetService(facetRepository.Object);

            Facet f1 = null;
            svc.GetAllFacets().EndWith(e => f1 = e.First());

            // ACT

            f1.Name = "f1-changed";

            Facet result = null;
            svc.UpdateFacet(f1).EndWith(e => result = e);

            // ASSERT
            
            Assert.AreSame(f1, result);
            Assert.AreEqual("f1-changed", result.Name);

            facetRepository.VerifyAll();
        }
    }
}
