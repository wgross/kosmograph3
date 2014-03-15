namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Entity : IHasAssignedFacets
    {
        public static readonly IModelItemFactory<Entity> Factory = new EntityFactory();

        #region Construction and initialization of this instance

        public Entity()
        {
            this.Id = Guid.NewGuid();
            this.AssignedFacets = new List<AssignedFacet>();
        }

        #endregion

        #region IHasName Members

        public string Name
        {
            get;
            set;
        }

        #endregion IHasName Members

        #region IHasIdentity<Guid> Members

        public Guid Id
        {
            get;
            set;
        }

        #endregion IHasIdentity<Guid> Members

        #region IHasAssignedFacets Members

        public IEnumerable<AssignedFacet> AssignedFacets
        {
            get
            {
                return this.assignedFacets;
            }
            private set
            {
                this.assignedFacets.Clear();
                this.assignedFacets.AddRange(value ?? Enumerable.Empty<AssignedFacet>());
            }
        }

        private readonly List<AssignedFacet> assignedFacets = new List<AssignedFacet>();

        #endregion 

        #region An Entity manages a collection of assigned facets 

        public AssignedFacet CreateNewAssignedFacet(Facet assignedFacet, Action<AssignedFacet> initialize = null)
        {
            var tmp = AssignedFacetFactory.CreateNew(this, assignedFacet, af =>
            {
                // create property value instance for all property deifnitions of the assigend tag
                af.Properties = assignedFacet.Properties
                    .Select(pd => pd.CreateNewPropertyValue(af, delegate { }))
                    .ToList();
            });

            // apply initializer to new AssignedTag instance
            (initialize ?? delegate { })(tmp);

            return tmp;
        }

        public AssignedFacet Add(AssignedFacet assignedFacet)
        {
            var duplicate = this.AssignedFacets.FirstOrDefault(t => t.Equals(assignedFacet) || t.FacetId.Equals(assignedFacet.FacetId));
            if (duplicate != null)
                return duplicate;

            this.assignedFacets.Add(assignedFacet);
            return assignedFacet;
        }

        public bool Remove(AssignedFacet assignedFacet)
        {
            return this.assignedFacets.Remove(assignedFacet);
        }

        #endregion 
    }
}