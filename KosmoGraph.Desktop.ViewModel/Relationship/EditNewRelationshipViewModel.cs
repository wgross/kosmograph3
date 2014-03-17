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
    using System.Windows;

    public class EditNewRelationshipViewModel : EditRelationshipViewModelBase
    {
        #region Construction and initialization

        public EditNewRelationshipViewModel(EntityViewModel from, EntityRelationshipViewModel viewModel, IManageEntitiesAndRelationships withRelationships)
            : base(Resources.EditNewRelationshipViewModelTitle, from)
        {
            this.model = viewModel;
            this.relationships = withRelationships;
            this.SetDestination = new DelegateCommand<EntityViewModel>(this.SetDestinationExecuted, this.SetDestinationCanExecute);
            this.ExecuteRollback();
        }

        public EditNewRelationshipViewModel(EntityViewModel from, EntityViewModel to, EntityRelationshipViewModel viewModel, IManageEntitiesAndRelationships withRelationships)
            : base(Resources.EditNewRelationshipViewModelTitle, from)
        {
            this.model = viewModel;
            this.toEntity = to;
            this.relationships = withRelationships;
            this.ExecuteRollback();   
        }

        private readonly EntityRelationshipViewModel model;

        private readonly IManageEntitiesAndRelationships relationships;

        #endregion 

        #region Set Destination Entity

        #region Destination point

        public Point ToPoint
        {
            get
            {
                return this.toPoint;
            }
            set
            {
                if (this.toPoint == value)
                    return;

                var tmp = this.toPoint;
                this.toPoint = value;
                this.RaisePropertyChanged(() => this.ToPoint);
                //this.UpdateArea();

                //double deltaWidth = 0.0;
                //double deltaHeight = 0.0;

                //if (this.Area.Width - this.MinSize.Width < 0.0)
                //    deltaWidth = this.toPoint.X - tmp.X;

                //if (this.Area.Height - this.MinSize.Height < 0.0)
                //    deltaHeight = this.toPoint.Y - tmp.Y;

                //this.From.MoveConnectionPoint(deltaHeight, deltaWidth);
            }
        }

        private Point toPoint;

        #endregion

        /// <summary>
        /// Changing the destinatin is needed as while creating a new relationship
        /// </summary>
        public override EntityViewModel To
        {
            get
            {
                return this.toEntity;
            }
        }

        private EntityViewModel toEntity;

        public DelegateCommand<EntityViewModel> SetDestination { get; private set; }

        private bool SetDestinationCanExecute(EntityViewModel toEntity)
        {
            if (this.hasAlreadyCommitted)
                return false;

            if (this.To != null && this.To.ModelItem.Id == toEntity.ModelItem.Id)
                return false; // already assigned

            return (this.From.ModelItem.Id != toEntity.ModelItem.Id);
        }

        private void SetDestinationExecuted(EntityViewModel toEntity)
        {
            this.toEntity = toEntity;
        }
        
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
            if (this.To == null || this.From == null)
                return false;

            if (this.hasAlreadyCommitted)
                return false;

            return true;
        }

        #endregion

        #region Rollback Editor

        protected override void ExecuteRollback()
        {
            this.toEntity = null;
            this.RollbackFacets(this.model.Facets);
        }

        protected override bool CanExecuteRollback()
        {
            return !this.hasAlreadyCommitted;
        }

        #endregion 
    }
}
