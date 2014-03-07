namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
    using KosmoGraph.Model;
    using KosmoGraph.Services;
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditNewRelationshipViewModel : EditRelationshipViewModelBase
    {
        #region Construction and initialization

        public EditNewRelationshipViewModel(EntityViewModel from, EntityViewModel to, EntityRelationshipViewModel viewModel, IManageEntitiesAndRelationships withRelationships)
            : base(Resources.EditNewRelationshipViewModelTitle, from, to)
        {
            this.model = viewModel;
            this.relationships = withRelationships;
            this.ExecuteRollback();   
        }

        private readonly EntityRelationshipViewModel model;

        private readonly IManageEntitiesAndRelationships relationships;

        #endregion 

        #region Commit Editor

        private bool hasAlreadyCommitted = false;

        protected override void ExecuteCommit()
        {
            if (this.hasAlreadyCommitted)
                return; // commit is executed only once

            this.hasAlreadyCommitted = true;

            var partial = this.relationships.CreatePartialRelationship(this.From.ModelItem, r =>
            {
                foreach (var pendingAssignedFacet in this.AssignedFacets.OfType<EditNewAssignedFacetViewModel>())
                {
                    r.Add(r.CreateNewAssignedFacet(pendingAssignedFacet.Facet.ModelItem, af=>
                    {
                        af.Properties.ForEach(pv =>
                        {
                            pv.Value = pv.Value = this.Properties.First(pvvm => pvvm.DefinitionId == pv.DefinitionId).Value;
                        });
                    }));
                }
            });

            this.relationships
                .CompletePartialRelationship( partial, this.To.ModelItem)
                .EndWith(succeeded:result =>  // TODO: Handle eror and cancel
                {
                    this.model.Add(result.Relationship, result.From, result.To);
                });
        }

        override protected bool CanExecuteCommit()
        {
            if (this.hasAlreadyCommitted)
                return false;

            return true;
        }

        #endregion

        #region Rollback Editor

        protected override void ExecuteRollback()
        {
            base.RollbackFacets(this.model.Facets);
        }

        protected override bool CanExecuteRollback()
        {
            return !this.hasAlreadyCommitted;
        }

        #endregion 
    }
}
