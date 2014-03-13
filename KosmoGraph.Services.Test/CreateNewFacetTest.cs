using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KosmoGraph.Model;
using Moq;
using KosmoGraph.Test;
using System.Threading;

namespace KosmoGraph.Services.Test
{
    [TestClass]
    public class CreateNewFacetTest
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            // install sync Task Scheduler
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            SynchronizationContext.SetSynchronizationContext(new ImmediateExecutionSynchronizationContext());
        }

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void CreateNewFacetFromNameAndInsertInDb()
        {
            // ARRANGE

            var facetRepository = new Mock<IFacetRepository>();

            facetRepository // expects a new entity with name 'e1' and returns it
                .Setup(_ => _.Insert(It.Is<Facet>(f => f.Name == "f1")))
                .Returns((Facet f) => f);

            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new FacetService(facetRepository.Object);

            // ACT

            Facet f1 = null;
            svc.CreateNewFacet(f => f.Name = "f1").EndWith(f => f1 = f);

            // ASSERT
            // creation insert facet in DB

            Assert.IsNotNull(f1);
            Assert.AreEqual("f1", f1.Name);

            facetRepository.VerifyAll();
        }

        [TestMethod]
        [TestCategory("CreateNewFacet")]
        public void ThrowOnEmptyFacetName()
        {
            // ARRANGE

            var facetRepository = new Mock<IFacetRepository>();
            var relationshipRepository = new Mock<IRelationshipRepository>();
            var svc = new FacetService(facetRepository.Object);

            // ACT

            ExceptionAssert.Throws<ArgumentNullException>(delegate { Facet e1 = svc.CreateNewFacet(f => f.Name = string.Empty).Result; });

            // ASSERT
            // creation insert facet in DB

            facetRepository.VerifyAll();
        }
    }
}
