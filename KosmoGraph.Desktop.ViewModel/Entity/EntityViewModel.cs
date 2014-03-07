namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Model;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class EntityViewModel : FacetedModelItemViewModelBase, ILayoutNode
    {
        #region Construction and Inititialization of this instance

        internal EntityViewModel(EntityRelationshipViewModel parentEntityRelationshipModel, Entity entity)
            : base(parentEntityRelationshipModel, entity) 
        {
            this.centralConnector = new EntityConnectorViewModel(this);
            this.ModelItem = entity;
        }

        public Entity ModelItem
        {
            get;
            private set;
        }

        #endregion

        #region Name of Entity

        public string Name
        {
            get
            {
                return this.ModelItem.Name;
            }
            set
            {
                if (this.ModelItem.Name == value)
                    return;
                this.ModelItem.Name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        #endregion 

        #region Coordinates on Canvas

        public double Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left == value)
                    return;

                this.left = value;
                this.RaisePropertyChanged(() => this.Left);
            }
        }

        private double left;

        public double Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top == value)
                    return;

                this.top = value;
                this.RaisePropertyChanged(() => this.Top);
            }
        }

        private double top;

        #endregion 

        #region Size on Canvas

        public double ActualHeight
        {
            get
            {
                return this.actualHeight;
            }
            set
            {
                if (this.actualHeight == value)
                    return;
                this.actualHeight = value;
                this.RaisePropertyChanged(() => this.ActualHeight);
            }
        }

        private double actualHeight;

        public double ActualWidth
        {
            get
            {
                return this.actualWidth;
            }
            set
            {
                if (this.actualWidth == value)
                    return;
                
                this.actualWidth = value;
                this.RaisePropertyChanged(() => this.ActualWidth);
            }
        }

        private double actualWidth;

        #endregion

        #region Connector

        public EntityConnectorViewModel CentralConnector
        {
            get
            {
                return this.centralConnector;
            }
        }

        private readonly EntityConnectorViewModel centralConnector;

        #endregion

        public override void RefreshIsVisible()
        {
            base.RefreshIsVisible();
            this.IsVisible = this.AssignedFacets.Any(at => at.Facet.IsVisible);
        }

        #region Override base class behaviour on changed selection state

        protected override void OnIsSelectedChanged(bool newValue)
        {
            base.OnIsSelectedChanged(newValue);

            // the selection is prpagetd to the assigned tags
            foreach (var at in this.AssignedFacets)
                at.Facet.IsItemSelected = true;
        }

        #endregion 
    
        #region ILayoutNode Members

        double ILayoutNode.DX
        {
            get;set;
        }

        double ILayoutNode.DY
        {
            get;
            set;
        }

        double ILayoutNode.Left
        {
            get
            {
                return this.Left;
            }
            set
            {
                this.Left = value;
            }
        }

        double ILayoutNode.Top
        {
            get
            {
                return this.Top;
            }
            set
            {
                this.Top = value;
            }
        }

        #endregion
    }
}
