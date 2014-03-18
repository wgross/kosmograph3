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
            this.relationships = withRelationships;
            this.SetDestination = new DelegateCommand<EntityViewModel>(this.SetDestinationExecuted, this.SetDestinationCanExecute);
            this.ExecuteRollback();
            this.Model.Items.Add(this);
        }

        //public EditNewRelationshipViewModel(EntityViewModel from, EntityViewModel to, EntityRelationshipViewModel viewModel, IManageEntitiesAndRelationships withRelationships)
        //    : base(Resources.EditNewRelationshipViewModelTitle, from)
        //{
        //    this.model = viewModel;
        //    this.toEntity = to;
        //    this.relationships = withRelationships;
        //    this.ExecuteRollback();   
        //}

        private readonly IManageEntitiesAndRelationships relationships;

        #endregion 

        #region Collection of points to draw for connection path

        public Rect Area
        {
            get
            {
                return this.area;
            }
            private set
            {
                if (this.area == value)
                    return;

                this.area = value;
                this.RaisePropertyChanged(() => this.Area);
                this.UpdateConnectionPoints();
            }
        }

        private Rect area;

        private void UpdateArea()
        {
            this.Area = new Rect(this.FromPoint, this.ToPoint);
        }

        public List<Point> ConnectionPoints
        {
            get
            {
                return connectionPoints;
            }
            private set
            {
                if (object.ReferenceEquals(this.connectionPoints, value))
                    return;

                this.connectionPoints = value;
                this.RaisePropertyChanged(() => this.ConnectionPoints);
            }
        }

        private List<Point> connectionPoints;
        
        private void UpdateConnectionPoints()
        {
            this.ConnectionPoints = new List<Point>()
            {                       
                new Point( this.FromPoint.X  <  this.ToPoint.X ? 0d : this.Area.Width, this.FromPoint.Y  <  this.ToPoint.Y ? 0d : this.Area.Height ), 
                new Point( this.FromPoint.X  >  this.ToPoint.X ? 0d : this.Area.Width, this.FromPoint.Y  >  this.ToPoint.Y ? 0d : this.Area.Height)
            };
        }

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
                this.UpdateArea();
                this.UpdateConnectionPoints();

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
            this.RaisePropertyChanged(() => this.To);
            this.ToPoint = this.toEntity.CentralConnector.GetConnectionPoint();
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
                    this.Model.Items
                        .OfType<EditNewRelationshipViewModel>()
                        .ToList()
                        .ForEach(i => this.Model.Items.Remove(i));

                    this.Model.Add(result.Relationship, result.From, result.To);
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
            this.RollbackFacets(this.Model.Facets);
            this.Model.Items
                        .OfType<EditNewRelationshipViewModel>()
                        .ToList()
                        .ForEach(i => this.Model.Items.Remove(i));
            // might not be correct: visual representation is removed from diagram to early
        }

        protected override bool CanExecuteRollback()
        {
            return !this.hasAlreadyCommitted;
        }

        #endregion 
    }
}
