
namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public sealed class PropertyValueFactory
    {
        #region Create Property values
        
        public static PropertyValue CreateNew(AssignedFacet assignedFacet, PropertyDefinition propertyDefinition)
        {
            if (assignedFacet == null)
                throw new ArgumentNullException("assignedFacet");
            
            if (propertyDefinition == null)
                throw new ArgumentNullException("propertyDefintion");

            return new PropertyValue() 
            {                
                DefinitionId = propertyDefinition.Id,
            };
        }

        public static PropertyValue CreateNew(AssignedFacet assignedFacet, PropertyDefinition propertyDefinition, Action<PropertyValue> initialize)
        {
            var tmp = CreateNew(assignedFacet,propertyDefinition);
            (initialize ?? delegate { })(tmp);
            return tmp;
        }

        #endregion
    }
}
