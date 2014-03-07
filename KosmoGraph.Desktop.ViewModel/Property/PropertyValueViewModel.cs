namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Model;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Holds a value of a PropertyDefnietio at the owning Faceted item
    /// </summary>
    public sealed class PropertyValueViewModel : NotificationObject
    {
        #region Construction and Initialization of this instance

        internal PropertyValueViewModel(PropertyDefinitionViewModel definition, PropertyValue modelItem)
        {
            if (definition.ModelItem == null || Guid.Empty == definition.ModelItem.Id)
                throw new InvalidOperationException("PropertyDefinition isn't initialized");

            this.ModelItem = modelItem;
            this.Definition = definition;
            // onect property value to property definition
            this.ModelItem.DefinitionId = this.Definition.ModelItem.Id;
        }

        public PropertyValue ModelItem { get; private set; }

        public PropertyDefinitionViewModel Definition { get; private set; }

        #endregion

        public string Value
        {
            get
            {
                return this.ModelItem.Value;
            }
            set
            {
                if (this.ModelItem.Value == value)
                    return;
                this.ModelItem.Value = value;
                this.RaisePropertyChanged(() => this.Value);
            }
        }
    }
}
