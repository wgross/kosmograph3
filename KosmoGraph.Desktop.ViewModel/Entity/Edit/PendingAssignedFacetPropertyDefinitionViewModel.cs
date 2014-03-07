namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Model;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public sealed class PendingAssignedFacetPropertyDefinitionViewModel : NotificationObject, IPropertyDefinition
    {
        public PendingAssignedFacetPropertyDefinitionViewModel(PropertyDefinitionViewModel withPropertyDefinition)
        {
            this.Edited = withPropertyDefinition;
        }

        public PropertyDefinitionViewModel Edited { get; private set; }

        public string Name { get; private set; }
    }
}
