namespace KosmoGraph.Services
{
    using KosmoGraph.Model;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FacetService : IManageFacets
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Construction and initialization of this instance

        public FacetService(IFacetRepository facets)
        {
            this.facetRepository = facets;

        }

        private readonly IFacetRepository facetRepository;

        #endregion

        #region IManageFacets members

        public Task<Facet> CreateNewFacet(Action<Facet> initializeWith)
        {
            var tmp = FacetFactory.CreateNew(initializeWith ?? delegate { });

            if (string.IsNullOrEmpty(tmp.Name))
                throw new ArgumentNullException("name");

            return Task.Factory.StartNew(() => this.facetRepository.Insert(tmp));
        }

        public Task<IEnumerable<Facet>> GetAllFacets()
        {
            return Task.Factory.StartNew(() => this.facetRepository.GetAll());
        }

        public Task<bool> RemoveFacet(Facet toRemove)
        {
            return Task.Factory.StartNew(() => this.facetRepository.Remove(toRemove));
        }

        public Task<Facet> UpdateFacet(Facet updatedFacet)
        {
            return Task.Factory.StartNew(() => this.facetRepository.Update(updatedFacet));
        }

        #endregion

        #region Default fail/cancel handling

        private static void OnCancel(string message, params object[] parameters)
        {
            log.Warn("Operation canceled:{0}", string.Format(message, parameters));
        }

        private static bool OnFailed(Exception failure, string message, params object[] parameters)
        {
            log.Error("Operation Failed:{0}:Failure:{1}", string.Format(message, parameters), failure);
            return true; // mark exception as 'handled'
        }

        #endregion

      
    }
}
