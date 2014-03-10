namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AssignedFacet
    {
        #region An AssignedFacet relates with a facet by id

        public Guid FacetId { get; set; }

        #endregion

        #region An assigned facet owns a set of property values

        public ICollection<PropertyValue> Properties
        {
            get
            {
                return this.properties;
            }
            set
            {
                this.properties.Clear();
                this.properties.AddRange(value ?? Enumerable.Empty<PropertyValue>());
            }
        }

        private readonly List<PropertyValue> properties = new List<PropertyValue>();

        #endregion    
    }
}