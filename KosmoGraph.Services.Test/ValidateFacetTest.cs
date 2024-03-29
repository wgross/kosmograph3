﻿namespace KosmoGraph.Services.Test
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
        public void VerifyFacetNameFailsWithEmptyName()
        {
            // ARRANGE

            this.facetRepository
                .Setup(_ => _.ExistsName(string.Empty))
                .Returns(false);

            this.facets.Single().Name = string.Empty;

            // ACT

            ValidateFacetResult result = null;

            this.fsvc.ValidateFacet(this.facets.Single().Name).EndWith(r => result = r);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.IsFalse(result.NameIsNotUnique);
            Assert.IsTrue(result.NameIsNullOrEmpty);

            this.facetRepository.VerifyAll();
            this.facetRepository.Verify(_ => _.ExistsName(It.IsAny<string>()), Times.Once);
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

            ValidateFacetResult result = null;

            this.fsvc.ValidateFacet(this.facets.Single().Name).EndWith(r => result = r);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.IsFalse(result.NameIsNotUnique);
            Assert.IsFalse(result.NameIsNullOrEmpty);

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

            ValidateFacetResult result = null;

            this.fsvc.ValidateFacet(this.facets.Single().Name).EndWith(r => result = r);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.IsTrue(result.NameIsNotUnique);
            Assert.IsFalse(result.NameIsNullOrEmpty);

            this.facetRepository.VerifyAll();
            this.facetRepository.Verify(_ => _.ExistsName(It.IsAny<string>()), Times.Once);
        }
    }
}
