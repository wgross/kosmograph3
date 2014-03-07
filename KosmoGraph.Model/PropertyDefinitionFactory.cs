
namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public sealed class PropertyDefinitionFactory
    {
        public static PropertyDefinition CreateNew(Facet tag)
        {
            if (tag == null)
                throw new ArgumentNullException("tag");

            return new PropertyDefinition() 
            { 
                Id = Guid.NewGuid(),
            };
        }

    }
}
