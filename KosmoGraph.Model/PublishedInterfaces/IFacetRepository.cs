namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IFacetRepository
    {
        Facet Insert(Facet e);
        Facet Update(Facet e);
        bool Remove(Facet e);

        Facet FindByIdentity(Guid id);

        IEnumerable<Facet> GetAll();

        bool ExistsName(string name);
    }
}
