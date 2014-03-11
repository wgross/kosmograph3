namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    public sealed class AssignedFacetViewModel
    {
        #region Construction and initialization of this instance

        internal AssignedFacetViewModel(FacetViewModel assigned, AssignedFacet modelItem)
        {
            this.ModelItem = modelItem;
            this.Facet = assigned;
            //this.Facet.Properties.CollectionChanged += PropertyDefinitions_CollectionChanged;
            this.Properties = new ObservableCollection<PropertyValueViewModel>();
            this.UpdatePropertyValues();
        }

        public void UpdatePropertyValues()
        {
            this.Properties.Clear();

            // rebuild property values from model item

            this.ModelItem.Properties.ToList().ForEach(pv =>
            {
                var propertyDefinition = this.Facet.Properties.FirstOrDefault(pd => pd.ModelItem.Id == pv.DefinitionId);
                if(propertyDefinition==null)
                {
                    // property definition is doesn't exist anymore
                    this.ModelItem.Properties.Remove(pv);
                }
                else
                {
                    // property definietin exists -> create view model
                    this.Properties.Add(propertyDefinition.CreatePropertyValue(pv));
                }
            });

            // add newly created properties from Facet

            this.Facet.Properties.ForEach(pd =>
            {
                // add property value foreach property definetions if not already added

                if(!this.Properties.Any(pv => pv.Definition.ModelItem.Id == pd.ModelItem.Id))
                {
                    this.Properties.Add(pd.CreateNewPropertyValue(this.ModelItem));
                }
            });
        }

        //void PropertyDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        // property defininition view models are added -> change this in model
        //        e.NewItems
        //            .Cast<PropertyDefinitionViewModel>()
        //            .Select(pdvm => pdvm.CreateNewPropertyValue(this.ModelItem))
        //            .ToList()
        //            .ForEach(pvvm =>
        //            {
        //                this.Properties.Add(pvvm);
        //            });

        //        // Set values in model item
        //        this.ModelItem.Properties = this.Properties.Select(pvvm => pvvm.ModelItem);
        //    }
        //    else if (e.Action == NotifyCollectionChangedAction.Remove)
        //    {
        //        e.OldItems
        //            .Cast<PropertyDefinitionViewModel>()
        //            .ToList()
        //            .ForEach(pd =>
        //            {
        //                var propertyToRemove = this.Properties.First(p => p.Definition.Equals(pd));
        //                this.Properties.Remove(propertyToRemove);
        //            });

        //        // Set values in model item
        //        this.ModelItem.Properties = this.Properties.Select(pvvm => pvvm.ModelItem);
        //    }
        //}

        public AssignedFacet ModelItem
        {
            get;
            private set;
        }

        #endregion

        public FacetViewModel Facet { get; private set; }

        public ObservableCollection<PropertyValueViewModel> Properties { get; private set; }
    }
}
