namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class PendingAssignedFacetPropertyValueViewModel : NotificationObject
    {
        #region Construction and initialization of this instance 

        public PendingAssignedFacetPropertyValueViewModel (PendingAssignedFacetPropertyDefinitionViewModel assigendFacetPropertyDefinition)
	    {
            this.propertyDefinition = assigendFacetPropertyDefinition;
	    }

        private readonly PendingAssignedFacetPropertyDefinitionViewModel propertyDefinition;

        #endregion 

        public string Name
        {
            get
            {
                return this.propertyDefinition.Name;
            }
        }

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.value == value)
                    return;
                this.value = value;
                this.RaisePropertyChanged(() => this.Value);
            }
        }

        private string value;
    }
}
