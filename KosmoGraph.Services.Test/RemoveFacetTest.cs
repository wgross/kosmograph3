namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using KosmoGraph.Model;
    using KosmoGraph.Test;

    [TestClass]
    public class RemoveFacetTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
        }

        [TestMethod]
        [TestCategory("RemoveFacet")]
        public void RemoveFacetFromDb()
        {
            // ARRANGE

            var facets = new[]
            {
                FacetFactory.CreateNew(f => f.Name = "f1")
            };

            var facetRepository = new Mock<IFacetRepository>();

            facetRepository // Expect call of remove
                .Setup(_ => _.Remove(facets.First()))
                .Returns(true);

            var fsvc = new FacetService(facetRepository.Object);

            // ACT

            bool result=false;
            fsvc.RemoveFacet(facets.First()).EndWith(r => result = r);

            // ASSERT

            Assert.IsTrue(result);
        }
    }
}
