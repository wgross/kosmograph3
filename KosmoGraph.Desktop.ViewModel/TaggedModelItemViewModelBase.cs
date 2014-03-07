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

    public abstract class FacetedModelItemViewModelBase : ModelItemViewModelBase
    {
        #region Construction and initialization of this instance 

        public FacetedModelItemViewModelBase(EntityRelationshipViewModel parentEntityRelationshipModel, IHasAssignedFacets modelItem)
            :base(parentEntityRelationshipModel)
        {
            this.facetedModelItem = modelItem;
            this.AssignedFacets = new ObservableCollection<AssignedFacetViewModel>();
            this.Properties = new ObservableCollection<PropertyValueViewModel>();
            this.VisibleProperties = new ObservableCollection<PropertyValueViewModel>();

            this.facetedModelItem
              .AssignedFacets
              .Select(at => this.CreateAssignedFacetFromModelItem(this.Model.Facets.Single(t => t.ModelItem.Id == at.FacetId), at))
              .ToList()
              .ForEach(at => this.AssignedFacets.Add(at));

            this.AssignedFacets
                .SelectMany(at => at.Properties)
                .ToList()
                .ForEach(pv => this.Properties.Add(pv));

            this.AssignedFacets.CollectionChanged += AssignedTags_CollectionChanged;
            this.VisibleProperties.CollectionChanged += VisibleProperties_CollectionChanged;
        }

        private readonly IHasAssignedFacets facetedModelItem;

        #endregion 

        public virtual void RefreshIsVisible()
        {
            this.VisibleProperties = new ObservableCollection<PropertyValueViewModel>(
              this.Properties.Where(pv => pv.Definition.Facet.IsVisible));
        }

        #region A relationship has tags assigned

        public ObservableCollection<AssignedFacetViewModel> AssignedFacets { get; private set; }

        void AssignedTags_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                e.NewItems
                    .Cast<AssignedFacetViewModel>()
                    .ToList()
                    .ForEach(at =>
                    {
                        // copy property value to this.Properties
                        at.Properties.ToList().ForEach(atp => this.Properties.Add(atp));
                        at.Properties.CollectionChanged += AssignedTagProperties_CollectionChanged;

                        // refresh selected state
                        if (this.IsSelected)
                            at.Facet.IsItemSelected = true;
                    });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                e.OldItems
                    .Cast<AssignedFacetViewModel>()
                    .ToList()
                    .ForEach(at =>
                    {
                        // remove propertes of removed tag from this.Properties
                        at.Properties.ToList().ForEach(atp => this.Properties.Remove(atp));
                    });
            }
        }

        void AssignedTagProperties_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                e.NewItems
                    .Cast<PropertyValueViewModel>()
                    .ToList()
                    .ForEach(pv => this.Properties.Add(pv));
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                e.OldItems
                    .Cast<PropertyValueViewModel>()
                    .ToList()
                    .ForEach(pv => this.Properties.Remove(pv));
            }
        }

        #endregion 

        #region Create an AssigedTag instance for this relationship

        public AssignedFacetViewModel CreateNewAssignedFacet(FacetViewModel assigned)
        {
            return this.CreateAssignedFacetFromModelItem(assigned, this.facetedModelItem.CreateNewAssignedFacet(assigned.ModelItem, delegate { }));
        }

        private AssignedFacetViewModel CreateAssignedFacetFromModelItem(FacetViewModel assigned, AssignedFacet modelItem)
        {
            return new AssignedFacetViewModel(assigned, modelItem);
        }

        #endregion 

        #region Add or Remove assigned Tags

        public AssignedFacetViewModel Add(AssignedFacetViewModel assignedTag)
        {
            if (assignedTag == null)
                throw new ArgumentNullException("assignedTag");

            // return already assigned tag if available.
            var alreadyAssigned = this.AssignedFacets.FirstOrDefault(at => at.Facet.Equals(assignedTag.Facet));
            if (alreadyAssigned != null)
                return alreadyAssigned;

            // create new assigned tag and add to list of assigned tags
            this.facetedModelItem.Add(assignedTag.ModelItem);
            this.AssignedFacets.Add(assignedTag);

            // recalculate visibility
            this.RefreshIsVisible();

            return assignedTag;
        }

        public bool RemoveAssignedFacet(FacetViewModel facetToRemove)
        {
            var assignedFacetToRemove = this.AssignedFacets
                .FirstOrDefault(af => af.Facet.ModelItem.Id == facetToRemove.ModelItem.Id);

            if (assignedFacetToRemove != null)
                return this.Remove(assignedFacetToRemove);
            return false;
        }

        public bool Remove(AssignedFacetViewModel assignedTag)
        {
            if (assignedTag == null)
                throw new ArgumentNullException("assignedTag");

            var result = false;
            if (this.facetedModelItem.Remove(assignedTag.ModelItem))
                result = this.AssignedFacets.Remove(assignedTag);

            // recalculate visibility
            this.RefreshIsVisible();

            return result;
        }

        #endregion 

        #region Aggregate all propert values from all assigned tags

        public ObservableCollection<PropertyValueViewModel> Properties { get; set; }

        public ObservableCollection<PropertyValueViewModel> VisibleProperties
        {
            get
            {
                return this.visibleProperties;
            }
            private set
            {
                if (object.ReferenceEquals(this.visibleProperties, value))
                    return;

                this.visibleProperties = value;
                this.RaisePropertyChanged(() => this.VisibleProperties);
                this.RaisePropertyChanged(() => this.HasVisibleProperties);
            }
        }

        void VisibleProperties_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.HasVisibleProperties);
        }

        private ObservableCollection<PropertyValueViewModel> visibleProperties;

        public bool HasVisibleProperties
        {
            get
            {
                return this.VisibleProperties.Any();
            }
        }

        #endregion 
    }
}
