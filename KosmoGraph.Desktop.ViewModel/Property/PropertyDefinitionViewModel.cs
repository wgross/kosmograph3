namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class PropertyDefinitionViewModel : NamedModelItemViewModelBase<PropertyDefinition>, IPropertyDefinition
    {
        #region Construction and initialization of this instance
        
        public PropertyDefinitionViewModel(FacetViewModel tag, PropertyDefinition propertyDefinition)
            : base(tag.Model,propertyDefinition)
        {
            this.Facet = tag;
        }

        public FacetViewModel Facet { get; private set; }

        #endregion 

        #region Create property value instances

        public PropertyValueViewModel CreateNewPropertyValue(AssignedFacet assignedTag)  
        {
            return this.CreatePropertyValue(this.ModelItem.CreateNewPropertyValue(assignedTag, delegate{}));
        }

        internal PropertyValueViewModel CreatePropertyValue(PropertyValue modelItem)
        {
            return new PropertyValueViewModel(this, modelItem);
        }

        #endregion 
    }
}
