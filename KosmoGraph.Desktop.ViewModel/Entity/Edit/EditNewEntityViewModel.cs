
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

    public sealed class EditNewEntityViewModel : EditEntityViewModelBase
    {
        #region Construction andinitialization of this instance 
        
        public EditNewEntityViewModel(EntityRelationshipViewModel viewModel, IManageEntitiesAndRelationships withEntities)
            : base(Resources.EditExistingEntityViewModelTitle)
        {
            this.model = viewModel;
            this.entities = withEntities;
            this.ExecuteRollback();
        }

        private readonly EntityRelationshipViewModel model;

        private readonly IManageEntitiesAndRelationships entities;

        private bool hasAlreadyCommitted = false;

        #endregion 

        #region Commit Editor

        override protected void ExecuteCommit()
        {
            if (this.hasAlreadyCommitted)
                return;

            this.entities
                .CreateNewEntity(e =>
                {
                    e.Name = this.Name;

                    // add all asigned faces to entity and set the property vaules.
                    foreach (var pendingAssignedFacet in this.AssignedFacets.OfType<EditAssignedFacetViewModelBase>())
                    {
                        e.Add(e.CreateNewAssignedFacet(pendingAssignedFacet.Facet.ModelItem, addedFacet =>
                        {
                            foreach (var propertyDefinition in pendingAssignedFacet.Facet.Properties)
                            {
                                addedFacet
                                    .Properties
                                    .First(pv => pv.DefinitionId == propertyDefinition.ModelItem.Id)
                                    .Value = this
                                                .Properties
                                                .First(pv => pv.DefinitionId == propertyDefinition.ModelItem.Id)
                                                .Value;
                            }
                        }));
                    }
                    
                    this.hasAlreadyCommitted = true;
                })
                .EndWith(e =>
                {
                    this.model.Add(e);
                });
        }

        override protected bool CanExecuteCommit()
        {
            if (this.hasAlreadyCommitted)
                return false;

            if (this.HasError)
                return false;
            
            return (
                    // Name has changed 
                    !(StringComparer.CurrentCultureIgnoreCase.Equals(this.Name, Resources.EditNewEntityViewModelNameDefault))
                    ||
                    this.HasChangedAssignedFacets
                );
        }

        #endregion

        #region Rollback Editor

        override protected void ExecuteRollback()
        {
            // reset values to default state
            base.Name = Resources.EditNewEntityViewModelNameDefault;
            base.RollbackFacets(this.model.Facets);
        }

        
        override protected bool CanExecuteRollback()
        {
            return (!this.hasAlreadyCommitted);
        }

        #endregion     
    }
}
