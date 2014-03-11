namespace KosmoGraph.Desktop.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using KosmoGraph.Services;

    public class EntityRelationshipViewModelFactory
    {
        public EntityRelationshipViewModel CreateNew(IManageEntitiesAndRelationships entitiesAndRelationships, IManageFacets facets)
        {
            return new EntityRelationshipViewModel(entitiesAndRelationships, facets);
        }
    }
}
