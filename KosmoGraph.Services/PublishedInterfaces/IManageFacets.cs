
namespace KosmoGraph.Services
{
    using KosmoGraph.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ValidateFacetResult
    {
        public bool NameIsNullOrEmpty { get; set; }
        public bool NameIsNotUnique { get; set; }
    }

    public interface IManageFacets
    {
        Task<Facet> CreateNewFacet(Action<Facet> initializeWith);

        Task<IEnumerable<Facet>> GetAllFacets();

        Task<bool> RemoveFacet(Facet toRemove);

        Task<Facet> UpdateFacet(Facet updatedFacet);

        Task<ValidateFacetResult> ValidateFacet(string facetName);
    }
}
