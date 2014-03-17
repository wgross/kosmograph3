
namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditFacetViewModelBase : EditModelItemViewModelBase, IDataErrorInfo
    {
        #region Construction and initialization of this instance 

        public EditFacetViewModelBase(EntityRelationshipViewModel model, string withTitleFormat)
            :base(model)
        {
            this.AddPropertyDefinition = new DelegateCommand(this.AddPropertyDefinitionExecuted, this.CanExecuteAddPropertyDefinition);
            this.RemovePropertyDefinition = new DelegateCommand<IEditPropertyDefinition>(this.RemovePropertyDefinitionExecuted, this.CanExecuteRemovePropertyDefinition);
            this.titleFormat = withTitleFormat;
        }

        private readonly string titleFormat;

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
                //this.HasError = false;

                //if (columnName == "Name")
                //{
                //    if (this.Edited.Name != this.Name)
                //        if (this.Edited.Model.Tags.Any(t => t.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase)))
                //        {
                //            this.HasError = true;
                //            return Resources.ErrorTagNameIsNotUnique;
                //        }
                //}
                return string.Empty;
            }
        }

        public bool HasError { get; private set; }

        #endregion

        #region Edit the Facets name

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

        #region Edit the Facets property definitions

        public DelegateCommand AddPropertyDefinition { get; private set; }

        abstract protected void AddPropertyDefinitionExecuted();

        abstract protected bool CanExecuteAddPropertyDefinition();

        //{
        //    //this.Properties.Add(new EditPropertyDefinitionViewModel(this.Edited.CreateNewPropertyDefinition(string.Format("Edited {0}", this.Properties.Count + 1))));
        //}

        public DelegateCommand<IEditPropertyDefinition> RemovePropertyDefinition
        {
            get; private set;
        }

        abstract protected bool CanExecuteRemovePropertyDefinition(IEditPropertyDefinition propertyDefinitionToRemove);

        private void RemovePropertyDefinitionExecuted(IEditPropertyDefinition propertyDefinitionToRemove)
        {
            this.Properties.Remove(propertyDefinitionToRemove);
        }

        public ObservableCollection<IEditPropertyDefinition> Properties
        {
            get
            {
                return this.properties;
            }
            protected set
            {
                if (object.ReferenceEquals(this.properties, value))
                    return;
                this.properties = value;
                this.RaisePropertyChanged(() => this.Properties);
            }
        }

        private ObservableCollection<IEditPropertyDefinition> properties;

        protected void Properties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.isPropertiesChanged = true;
        }

        protected bool isPropertiesChanged = false;

        #endregion 
    }
}
