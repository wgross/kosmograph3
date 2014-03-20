
namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
using KosmoGraph.Model;
using KosmoGraph.Services;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

    public abstract class EditFacetViewModelBase : EditModelItemViewModelBase, IDataErrorInfo
    {
        #region Construction and initialization of this instance 

        public EditFacetViewModelBase(EntityRelationshipViewModel model, IManageFacets facets, string withTitleFormat)
            :base(model)
        {
            this.ManageFacets = facets;
            this.AddPropertyDefinition = new DelegateCommand(this.AddPropertyDefinitionExecuted, this.CanExecuteAddPropertyDefinition);
            this.RemovePropertyDefinition = new DelegateCommand<IEditPropertyDefinition>(this.RemovePropertyDefinitionExecuted, this.CanExecuteRemovePropertyDefinition);
            this.titleFormat = withTitleFormat;
        }

        private readonly string titleFormat;

        protected IManageFacets ManageFacets { get; private set; }

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
                if(this.SetAndValidate(() => this.Name, ref this.name, value))
                    this.RaisePropertyChanged(() => this.Title);
            }
        }

        private string name;

        #endregion 

        #region Edit the Facets property definitions

        public DelegateCommand AddPropertyDefinition { get; private set; }

        abstract protected void AddPropertyDefinitionExecuted();

        abstract protected bool CanExecuteAddPropertyDefinition();

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
                this.Set(() => this.Properties, ref this.properties, value);
            }
        }

        private ObservableCollection<IEditPropertyDefinition> properties;

        protected void Properties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.isPropertiesChanged = true;
        }

        protected bool isPropertiesChanged = false;

        #endregion 

        #region Validate the facets date state

        protected bool SetAndValidate<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            if (this.Set(propertyExpression, ref field, newValue))
            {
                this.ValidateFacetData(); // spawns thread for validation
                return true;
            }
            return false;
        }
       
        protected bool? IsValid { get; private set;}

        protected void ValidateFacetData()
        {
            this.IsValid = null;
            this.ManageFacets.ValidateFacet(this.Name).EndWith(valid => this.IsValid = valid);
        }

        #endregion
    }
}
