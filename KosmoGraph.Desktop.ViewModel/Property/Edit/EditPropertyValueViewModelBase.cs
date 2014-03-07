namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditPropertyValueViewModelBase : NotificationObject
    {
        #region Construction and initialization of this instance 

        public EditPropertyValueViewModelBase(PropertyDefinitionViewModel fromDefinition)
        {
            this.definition = fromDefinition;
        }

        private readonly PropertyDefinitionViewModel definition;

        #endregion 

        #region Property definition data

        public string Name 
        {
            get
            {
                return this.definition.Name;
            }
        }

        public Guid DefinitionId
        {
            get
            {
                return this.definition.ModelItem.Id;
            }
        }

        #endregion 

        #region Edit the property value

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

        #endregion 

        public abstract bool IsDirty { get; }
    }
}
