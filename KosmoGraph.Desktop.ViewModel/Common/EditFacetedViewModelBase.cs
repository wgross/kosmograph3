
namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Model;
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditFacetedViewModelBase : EditModelItemViewModelBase
    {
        #region Construction and initializaton of this instance 

        public EditFacetedViewModelBase(EntityRelationshipViewModel model)
            : base(model)
        {
            this.AssignFacet = new DelegateCommand<FacetViewModel>(this.AssignFacetExecuted);
            this.UnassignFacet = new DelegateCommand<EditAssignedFacetViewModelBase>(this.UnassignFacetExecuted);
        }

        #endregion

        #region Facets of this view model

        public ObservableCollection<EditAssignedFacetViewModelBase> AssignedFacets
        {
            get
            {
                return this.assignedFacets;
            }
            private set
            {
                if (object.ReferenceEquals(this.assignedFacets, value))
                    return;
                this.assignedFacets = value;
                this.RaisePropertyChanged(() => this.AssignedFacets);
            }
        }

        private ObservableCollection<EditAssignedFacetViewModelBase> assignedFacets = new ObservableCollection<EditAssignedFacetViewModelBase>();

        public ObservableCollection<FacetViewModel> UnassignedFacets
        {
            get
            {
                return this.unassignedFacets;
            }
            private set
            {
                if (object.ReferenceEquals(this.unassignedFacets, value))
                    return;
                this.unassignedFacets = value;
                this.RaisePropertyChanged(() => this.UnassignedFacets);
            }
        }

        private ObservableCollection<FacetViewModel> unassignedFacets = new ObservableCollection<FacetViewModel>();

        #endregion 

        #region AssignFacet Command

        public DelegateCommand<FacetViewModel> AssignFacet { get; private set; }

        private void AssignFacetExecuted(FacetViewModel unassignedFacet)
        {
            if (this.UnassignedFacets.Remove(unassignedFacet))
            {
                this.AssignedFacets.Add(new EditNewAssignedFacetViewModel(unassignedFacet));
                
                unassignedFacet
                    .Properties
                    .ForEach(pd => this.Properties.Add(new EditNewPropertyValueViewModel(pd)));

                this.HasChangedAssignedFacets = true;
            }
        }

        #endregion

        #region UnassignFacet command

        public DelegateCommand<EditAssignedFacetViewModelBase> UnassignFacet { get; private set; }

        private void UnassignFacetExecuted(EditAssignedFacetViewModelBase pendingAssigendFacet)
        {
            if (this.AssignedFacets.Remove(pendingAssigendFacet))
            {
                this.UnassignedFacets.Add(pendingAssigendFacet.Facet);

                pendingAssigendFacet
                    .Facet
                    .Properties
                    .ForEach(pd =>
                    {
                        var toRemove = this.Properties.FirstOrDefault(p => p.DefinitionId == pd.ModelItem.Id);
                        if (toRemove != null)
                            this.Properties.Remove(toRemove);
                    });

                this.HasChangedAssignedFacets = true;
            }
        }

        #endregion 

        protected bool HasChangedAssignedFacets { get; private set;}

        #region Accumulated property values from assigned Facets

        public ObservableCollection<EditPropertyValueViewModelBase> Properties
        {
            get
            {
                return this.properties;
            }
            private set
            {
                if (this.properties == value)
                    return;
                this.properties = value;
                this.RaisePropertyChanged(() => this.Properties);
            }
        }

        private ObservableCollection<EditPropertyValueViewModelBase> properties;

        #endregion 

        #region Rollback Editor

        protected void RollbackFacets(IEnumerable<FacetViewModel> allFacetsInModel)
        {
            this.UnassignedFacets = new ObservableCollection<FacetViewModel>(allFacetsInModel);
            this.AssignedFacets = new ObservableCollection<EditAssignedFacetViewModelBase>();
            this.Properties = new ObservableCollection<EditPropertyValueViewModelBase>();
            this.HasChangedAssignedFacets = false;
        }

        protected void RollbackFacets(IEnumerable<FacetViewModel> allFacetsInModel, IEnumerable<AssignedFacetViewModel> assignedFacetsFromFacetedItem)
        {
            this.UnassignedFacets = new ObservableCollection<FacetViewModel>(
                allFacetsInModel
                    .Except(assignedFacetsFromFacetedItem
                        .Select(afvm => afvm.Facet)));

            this.AssignedFacets = new ObservableCollection<EditAssignedFacetViewModelBase>(assignedFacetsFromFacetedItem.Select(afvm=>new EditExistingAssignedFacetViewModel(afvm)));
            
            this.Properties = new ObservableCollection<EditPropertyValueViewModelBase>(
                assignedFacetsFromFacetedItem
                    .SelectMany(af => af.Properties)
                    .Select(afpv=> new EditExistingPropertyValueViewModel(afpv)));

            this.HasChangedAssignedFacets = false;

            //this.Properties = new ObservableCollection<EditPropertyValueViewModelBase>(
            //    this.AssignedFacets
            //        .SelectMany(at => at.Properties));
            //        //.Select(pv => new EditExistingPropertyValueViewModel(pv)));

        }
        #endregion

    }
}
