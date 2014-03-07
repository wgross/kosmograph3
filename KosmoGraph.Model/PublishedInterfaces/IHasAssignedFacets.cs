namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IHasAssignedFacets
    {
        IEnumerable<AssignedFacet> AssignedFacets { get; }

        AssignedFacet Add(AssignedFacet assignedFacet);

        bool Remove(AssignedFacet assignedFacet);

        AssignedFacet CreateNewAssignedFacet(Facet assignedFacet, Action<AssignedFacet> initialize = null);
    }
}
