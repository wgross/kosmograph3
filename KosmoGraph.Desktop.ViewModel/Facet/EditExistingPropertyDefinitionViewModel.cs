namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class EditExistingPropertyDefinitionViewModel : EditPropertyDefinitionViewModelBase, IDataErrorInfo, IEditPropertyDefinition
    {
        #region Construction and initialization of this instance

        public EditExistingPropertyDefinitionViewModel(PropertyDefinitionViewModel edited)
        {
            if (edited == null)
                throw new ArgumentNullException("edited");

            this.Edited = edited;
            this.RollbackExecuted();
        }

        public PropertyDefinitionViewModel Edited { get; private set;}
        
        #endregion 

        #region IDataErrorInfo Members

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                this.HasError = false;

                if (!StringComparer.InvariantCultureIgnoreCase.Equals(this.Name, this.Edited.Name))
                    if (this.Edited.Facet.Properties.Any(pd => StringComparer.InvariantCultureIgnoreCase.Equals(pd.Name, this.Name)))
                    {
                        this.HasError = true;
                        return Resources.ErrorPropertyDefinitionNameIsNotUnique;
                    }

                return string.Empty;
            }
        }

        public bool HasError
        {
            get;
            private set;
        }

        #endregion

        #region Commit editor

        private void CommitExecuted()
        {
            this.Edited.Name = this.Name;
        }

        private bool CommitCanExecute()
        {
            return !this.HasError && this.Edited.Name != this.Name;
        }

        #endregion 

        #region Rollback editor

        private void RollbackExecuted()
        {
            this.Name = this.Edited.Name;
        }

        private bool RollbackCanExecute()
        {
            return true;
        }

        #endregion 
    }
}
