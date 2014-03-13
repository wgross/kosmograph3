namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Model;
    using Moq;
    using KosmoGraph.Test;
    using System.Threading;

    [TestClass]
    public class GetFacetTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());
        }

        [TestMethod]
        public void GetAllFacetsFromDb()
        {
            // ARRANGE

            var facets = new []
            { 
                FacetFactory.CreateNew(f=>f.Name = "f1"), 
                FacetFactory.CreateNew(f => f.Name="f2")
            };

            var facetRepository = new Mock<IFacetRepository>();

            facetRepository // returns a facet collection
                .Setup(_ => _.GetAll())
                .Returns(facets);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new FacetService(facetRepository.Object);
            
            // ACT

            Facet[] result = null;
            
            svc.GetAllFacets().EndWith(f => result = f.ToArray());

            // ASSERT

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(facets.Select(f => f.Id).ToArray(), result.Select(f => f.Id).ToArray());
        }
    }
}
