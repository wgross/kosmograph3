namespace KosmoGraph.Services.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KosmoGraph.Test;
    using System.Threading;
    using KosmoGraph.Model;
    using Moq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    [TestClass]
    public class ValidateFacetTest
    {
        private IEnumerable<Facet> facets;
        private Mock<IFacetRepository> facetRepository;
        private FacetService fsvc;

        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());

            this.facetRepository = new Mock<IFacetRepository>();
            this.fsvc = new FacetService(this.facetRepository.Object);
            this.facets = new[]
            {
                Facet.Factory.CreateNew(f => f.Name = "f1")
            };
        }

        [TestMethod]
        [TestCategory("ValidateFacet")]
        public void VerifyFacetNameSucceedsIfNameIsUnknown()
        {
            // ARRANGE

            this.facetRepository
                .Setup(_ => _.ExistsName("f1"))
                .Returns(false);

            // ACT

            bool result = false;

            this.fsvc.ValidateFacet(this.facets.Single().Name).EndWith(r => result = r);

            // ASSERT

            Assert.IsTrue(result);

            this.facetRepository.VerifyAll();
            this.facetRepository.Verify(_ => _.ExistsName(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("ValidateFacet")]
        public void VerifyFacetNameFailsIfNameIsKnown()
        {
            // ARRANGE

            this.facetRepository
                .Setup(_ => _.ExistsName("f1"))
                .Returns(true);

            // ACT

            bool result = false;

            this.fsvc.ValidateFacet(this.facets.Single().Name).EndWith(r => result = r);

            // ASSERT

            Assert.IsFalse(result);

            this.facetRepository.VerifyAll();
            this.facetRepository.Verify(_ => _.ExistsName(It.IsAny<string>()), Times.Once);
        }
    }
}
