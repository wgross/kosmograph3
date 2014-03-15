

namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
    using KosmoGraph.Services;
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditExistingRelationshipViewModel : EditRelationshipViewModelBase
    {
        #region Construction and initialization of this instance 

        public EditExistingRelationshipViewModel(RelationshipViewModel edited, IManageEntitiesAndRelationships withRelationships)
            : base(Resources.EditRelationshipViewModelTitle, edited.From.Entity, edited.To.Entity)
        {
            this.Edited = edited;
            this.relationships = withRelationships;
            this.ExecuteRollback();
        }

        private readonly IManageEntitiesAndRelationships relationships;

        public RelationshipViewModel Edited { get; private set; }

        #endregion 

        #region Commit Editor

        private bool hasAlreadyCommitted = false;

        override protected void ExecuteCommit()
        {
            if (this.hasAlreadyCommitted)
                return;

            // add new facets to edited entity view model

            foreach (var pendingAssignedFacet in this.AssignedFacets.OfType<EditNewAssignedFacetViewModel>())
            {
                var addedFacet = this.Edited.CreateNewAssignedFacet(pendingAssignedFacet.Facet);

                foreach (var propertyDefinition in pendingAssignedFacet.Facet.Properties)
                {
                    addedFacet
                        .Properties
                        .First(pv => pv.Definition.ModelItem.Id == propertyDefinition.ModelItem.Id)
                        .Value = this
                                    .Properties
                                    .First(pv => pv.DefinitionId == propertyDefinition.ModelItem.Id)
                                    .Value;
                }

                this.Edited.Add(addedFacet);
            }

            // removed missing facets

            this.Edited.AssignedFacets.ToList().ForEach(af =>
            {
                if (!this.AssignedFacets.Any(edit_af => edit_af.Facet.ModelItem.Id == af.Facet.ModelItem.Id))
                    this.Edited.Remove(af);
            });

            // change property values of modified facets

            foreach (var modifiedAssignedFacet in this.AssignedFacets.OfType<EditExistingAssignedFacetViewModel>())
            {
                modifiedAssignedFacet
                    .Edited
                    .Properties
                    .ForEach(epvm =>
                    {
                        // get modified valu from this editor
                        epvm.Value = this.Properties.First(pv => pv.DefinitionId == epvm.Definition.ModelItem.Id).Value;
                    });
            }

            this.relationships.UpdateRelationship(this.Edited.ModelItem).Wait();
            this.hasAlreadyCommitted = true;
        }

        override protected bool CanExecuteCommit()
        {
            if (this.hasAlreadyCommitted)
                return false;

            //if (this.HasError)
            //    return false;

            return (
                this.HasChangedAssignedFacets 
                ||
                this.Properties.Aggregate(false, (dirty, pv) => dirty || pv.IsDirty)
                //|| 
                //this.AssignedFacets.Any(at => at.Commit.CanExecute())
            );
        }

        #endregion

        #region Rollback Editor

        override protected void ExecuteRollback()
        {
            this.RollbackFacets(this.Edited.Model.Facets, this.Edited.AssignedFacets);
            //this.UnassignedTags = new ObservableCollection<FacetViewModel>(this.Edited.Model.Facets.Except(this.Edited.AssignedFacets.Select(at => at.Facet)));
            //this.AssignedFacets = new ObservableCollection<EditAssignedRelationshipFacetViewModel>(this.Edited.AssignedFacets.Select(at => new EditAssignedRelationshipFacetViewModel(at)));
            //this.AssignedFacets.CollectionChanged+=AssignedTags_CollectionChanged;
            //this.isAssigedTagsChanged = false;
            ////this.Properties = new ObservableCollection<EditPropertyValueViewModelBase>(this.AssignedFacets.SelectMany(at => at.Properties));
        }

        override protected bool CanExecuteRollback()
        {
            return !this.hasAlreadyCommitted;
        }

        #endregion 
    }
}
