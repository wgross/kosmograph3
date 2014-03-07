namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditPropertyDefinitionViewModelBase : NotificationObject, IEditPropertyDefinition
    {
        #region Construction and initialization of this instance

        public EditPropertyDefinitionViewModelBase()
        {
        }

        #endregion

        #region IEditPropertyDefinition Members

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name == value)
                    return;
                this.name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        private string name;

        #endregion
    }
}
