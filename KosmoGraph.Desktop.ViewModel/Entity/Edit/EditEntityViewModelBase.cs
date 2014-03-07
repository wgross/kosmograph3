
namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditEntityViewModelBase : EditFacetedViewModelBase, IDataErrorInfo
    {
        #region Construction and Initialization of this instance

        public EditEntityViewModelBase(string withTitleFormat)
        {
            this.titleFormat = withTitleFormat;
            //this.assignTagCommand = new DelegateCommand<TagViewModel>(this.AssignTagExecuted);
            //this.unassignTagCommand = new DelegateCommand<EditAssignedEntityTagViewModel>(this.UnassignTagExecuted);
            
        }

        private readonly string titleFormat;

        public bool EnableCommit { get; set; }

        #endregion 

        #region Edit the name of the entity

        public string Title
        {
            get
            {
                return string.Format(this.titleFormat, this.Name);
            }
        }

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
                this.RaisePropertyChanged(() => this.Title);
                this.RefreshCommands();
            }
        }

        private string name;

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

                if (columnName == "Name")
                {
                    //TODO: This has to be checked by the database in the background
                    //if (this.Edited.Name != this.Name)
                    //    if (this.Edited.Model.Entities.Any(e => StringComparer.InvariantCultureIgnoreCase.Equals(e.Name, this.Name)))
                    //    {
                    //        this.HasError = true;
                    //        return Resources.ErrorEntityNameIsNotUnique;
                    //    }
                }
                return string.Empty;
            }
        }

        public bool HasError { get; private set; }

        #endregion
    }
}
