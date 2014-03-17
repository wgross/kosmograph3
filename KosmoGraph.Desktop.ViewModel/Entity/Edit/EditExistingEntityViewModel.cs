
namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
    using KosmoGraph.Services;
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditExistingEntityViewModel : EditEntityViewModelBase
    {
        #region Construction and Initialization of this instance

        public EditExistingEntityViewModel(EntityViewModel edited, IManageEntitiesAndRelationships withEntities)
            : base(edited.Model,Resources.EditExistingEntityViewModelTitle)
        {
            this.Edited = edited;
            this.entities = withEntities;
            this.ExecuteRollback();
        }

        public EntityViewModel Edited { get; private set; }

        private readonly IManageEntitiesAndRelationships entities;

        #endregion 

        #region Commit Editor

        override protected void ExecuteCommit()
        {
            if (this.hasAlreadyCommitted)
                return;

            this.Edited.Name = this.Name;

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

            this.Edited.AssignedFacets.ToList().ForEach(af=>
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
            
            this.entities.UpdateEntity(this.Edited.ModelItem).Wait();
            this.hasAlreadyCommitted = true;
            this.EnableCommit = false;
        }

        override protected bool CanExecuteCommit()
        {
            if (this.hasAlreadyCommitted)
                return false;

            if (this.HasError)
                return false;

            return (
                (this.Edited.Name != this.Name) // name was changed... 
                || 
                this.HasChangedAssignedFacets   // or tags have changed
                ||
                this.Properties.Aggregate(false, (dirty,pv) => dirty || pv.IsDirty)
                //|| 
                //this.AssignedTags.Any(at => at.Commit.CanExecute()) //.. an can commit
            );
        }

        private bool hasAlreadyCommitted = false;

        #endregion 

        #region Rollback Editor

        override protected void ExecuteRollback()
        {
            // restore values from edited EntityViewModel
            this.Name = this.Edited.Name;
            this.RollbackFacets(this.Edited.Model.Facets,this.Edited.AssignedFacets);
        }

        override protected bool CanExecuteRollback()
        {
            return (!this.hasAlreadyCommitted);
        }

        #endregion     
    }
}
