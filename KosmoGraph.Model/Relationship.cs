namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Relationship : IHasAssignedFacets
    {
        public Relationship()
        {
            this.AssignedFacets = new List<AssignedFacet>();
        }

        #region IHasIdentity<Guid> Members

        public Guid Identity
        {
            get;
            internal set;
        }

        #endregion IHasIdentity<Guid> Members

        #region A Relationship provides a collecion of assigned facets

        public AssignedFacet CreateNewAssignedFacet(Facet assignedTag, Action<AssignedFacet> initialize)
        {
            var tmp = AssignedFacetFactory.CreateNew(this, assignedTag, at =>
            {
                // create property value instances for all property definitiOns
                at.Properties = assignedTag.Properties
                    .Select(pd => pd.CreateNewPropertyValue(at, delegate { }))
                    .ToList();
            });

            // apply initializer to new AssignedTag instance
            (initialize ?? delegate { })(tmp);

            return tmp;
        }

        public AssignedFacet Add(AssignedFacet assignedTag)
        {
            this.assignedFacets.Add(assignedTag);
            return assignedTag;
        }

        public bool Remove(AssignedFacet assignedTag)
        {
            return this.assignedFacets.Remove(assignedTag);
        }

        #endregion

        #region IHasAssignedFacet Members

        public virtual IEnumerable<AssignedFacet> AssignedFacets 
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

        #region Source entity of this relationship

        public virtual Entity From { get; private set; }

        public Guid FromId { get; set; }

        internal void SetSource(Entity fromEntity)
        {
            if (fromEntity == null)
                throw new ArgumentNullException("fromEntity");

            this.From=fromEntity;
            this.FromId = fromEntity.Id;
        }
        
        #endregion 

        #region Destination entity of the relationship

        public virtual Entity To { get; private set; }

        public Guid ToId { get; set; }

        public void SetDestination(Entity toEntity)
        {
            if (toEntity == null)
                throw new ArgumentNullException("toEntity");

            this.To = toEntity;
            this.ToId = toEntity.Id;
        }

        #endregion 
    }
}