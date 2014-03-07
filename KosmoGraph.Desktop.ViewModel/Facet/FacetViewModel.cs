namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class FacetViewModel : NamedModelItemViewModelBase<Facet>
    {
        #region Construction and Initialization of this instance

        public FacetViewModel(EntityRelationshipViewModel parentEntityRelationshipModel, Facet tag)
            : base(parentEntityRelationshipModel,tag)
        {
            this.properties = new ObservableCollection<PropertyDefinitionViewModel>(
                this.ModelItem.Properties.Select(p => this.CreatePropertyDefinition(p)));
        }

        #endregion 

        #region Create new property definitions

        public PropertyDefinitionViewModel CreateNewPropertyDefinition(string name)
        {
            return this.CreatePropertyDefinition(this.ModelItem.CreateNewPropertyDefinition(pd => { pd.Name = name; }));
        }

        private PropertyDefinitionViewModel CreatePropertyDefinition(PropertyDefinition propertyDefinition)
        {
            return new PropertyDefinitionViewModel(this, propertyDefinition);
        }

        #endregion 

        #region The Tags PropertyDefinitions

        public ObservableCollection<PropertyDefinitionViewModel> Properties
        {
            get
            {
                return this.properties;
            }
        }

        private ObservableCollection<PropertyDefinitionViewModel> properties;

        public IPropertyDefinition Add(PropertyDefinitionViewModel propertyDefinition)
        {
            if (propertyDefinition == null)
                throw new ArgumentNullException("propertyDefinition");

            var alreadyAssigned = this.Properties.FirstOrDefault(pd => pd.Equals(propertyDefinition));
            if (alreadyAssigned != null)
                return alreadyAssigned;

            if (this.Properties.Any(pd => StringComparer.InvariantCultureIgnoreCase.Equals(pd.Name, propertyDefinition.Name)))
                throw new InvalidOperationException("Name of property definition must be unique");

            // keep entity and model item consistent
            this.Properties.Add(propertyDefinition);
            this.ModelItem.Add(propertyDefinition.ModelItem);

            return propertyDefinition;
        }

        public bool Remove(PropertyDefinitionViewModel propertyDefinition)
        {
            if(propertyDefinition==null)
                throw new ArgumentNullException("propertyDefinition");

            if(this.ModelItem.Remove(propertyDefinition.ModelItem))
                return this.Properties.Remove(propertyDefinition);
            return false;
        }

        #endregion 

        #region Indicates that an item that has this tag assigned is selected

        public bool IsItemSelected
        {
            get
            {
                return this.isItemSelected;
            }
            set
            {
                if (this.isItemSelected == value)
                    return;
                this.isItemSelected = value;
                this.RaisePropertyChanged(() => this.IsItemSelected);
            }
        }

        private bool isItemSelected = false;

        #endregion 

        protected override void OnIsVisibleChanged(bool newValue)
        {
            foreach (var item in this.Model.Items.OfType<FacetedModelItemViewModelBase>())
                item.RefreshIsVisible();
        }
    }
}
