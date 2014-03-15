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
    using System.Windows;

    /// <summary>
    /// Implemnts a finalized connection between to Entities/Connectors 
    /// </summary>
    public sealed class RelationshipViewModel : FacetedModelItemViewModelBase, ILayoutEdge
    {
        #region Construction and initialization of this instance

        /// <summary>
        /// Use this ctor to construct a complete relationship
        /// </summary>
        /// <param name="fromConnector"></param>
        /// <param name="toConnector"></param>
        /// <param name="modelItem"></param>
        internal RelationshipViewModel(EntityConnectorViewModel fromConnector,EntityConnectorViewModel toConnector, Relationship modelItem)
            : this(fromConnector,modelItem)
        {
            if (toConnector == null)
                throw new ArgumentNullException("toConnector");
            if (toConnector.Entity.Equals(this.From.Entity))
                throw new InvalidOperationException("from == to");
            this.To = toConnector;
            this.ToPoint = this.To.GetConnectionPoint();
        }

        /// <summary>
        /// This ctor is used during constraction of relationships in the editor where 
        /// the destination is temorary yet to a point instead of an Entity 
        /// </summary>
        /// <param name="fromConnector"></param>
        /// <param name="toPoint"></param>
        internal RelationshipViewModel(EntityConnectorViewModel fromConnector, Point toPoint)
            : this(fromConnector, Relationship.Factory.CreateNewPartial(fromConnector.Entity.ModelItem))
        {
            this.ToPoint = toPoint;
        }

        private RelationshipViewModel(EntityConnectorViewModel fromConnector, Relationship modelItem)
            : base(fromConnector.Entity.Model, modelItem)
        {
            if (fromConnector == null)
                throw new ArgumentNullException("fromConnector");
            if (modelItem == null)
                throw new ArgumentNullException("modelItem");
            
            this.ModelItem = modelItem;
            this.From = fromConnector;
            this.FromPoint = this.From.GetConnectionPoint();
        }

        public Relationship ModelItem
        {
            get;
            private set;
        }

        #endregion

        #region Destination connector

        public EntityConnectorViewModel To
        {
            get
            {
                return this.to;
            }
            set
            {
                if (object.ReferenceEquals(this.to, value))
                    return;

                this.to = value;
                this.to.Entity.PropertyChanged += new WeakPropertyChangedEventHandler(this.ToPropertyChanged).Handler;
                this.ModelItem.ToId = value.Entity.ModelItem.Id;
                this.RaisePropertyChanged(() => this.To);
            }
        }

        private void ToPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.ToPoint = this.To.GetConnectionPoint();
        }

        private EntityConnectorViewModel to;

        #endregion

        #region Source connector

        public EntityConnectorViewModel From
        {
            get
            {
                return this.from;
            }
            set
            {
                if (object.ReferenceEquals(this.from, value))
                    return;

                this.from = value;
                this.from.Entity.PropertyChanged += new WeakPropertyChangedEventHandler(this.FromPropertyChanged).Handler;
                this.ModelItem.FromId = value.Entity.ModelItem.Id;
                this.RaisePropertyChanged(() => this.From);
            }
        }

        private EntityConnectorViewModel from;

        private void FromPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.FromPoint = this.From.GetConnectionPoint();
        }

        #endregion

        #region Source point

        public Point FromPoint
        {
            get
            {
                return this.fromPoint;
            }
            set
            {
                if (this.fromPoint == value)
                    return;
                var tmp = this.fromPoint;
                this.fromPoint = value;
                this.RaisePropertyChanged(() => this.FromPoint);
                this.UpdateArea();

                //double deltaWidth = 0.0;
                //double deltaHeight = 0.0;
                
                //if(this.Area.Width-this.MinSize.Width < 0.0)
                //    deltaWidth = this.fromPoint.X - tmp.X;

                //if (this.Area.Height - this.MinSize.Height < 0.0) 
                //    deltaHeight = this.fromPoint.Y - tmp.Y;

                //if(this.To!=null)
                //    this.To.MoveConnectionPoint(deltaHeight,deltaWidth);
            }
        }

        private Point fromPoint;

        #endregion

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

        private void UpdateAreaFromMinSize()
        {
            var area = new Rect(this.FromPoint, this.ToPoint);

            if (this.MinSize.Height > area.Height)
            {
                // push from and to awy to get some space
                
                var deltaHalf = ((this.MinSize.Height - area.Height)*0.5);
                if (this.FromPoint.X > this.ToPoint.X)
                {
                    this.FromPoint = new Point(this.FromPoint.X + deltaHalf, this.FromPoint.Y);
                    this.ToPoint = new Point(this.ToPoint.X - deltaHalf, this.ToPoint.Y);
                }
                else
                {
                    this.FromPoint = new Point(this.FromPoint.X - deltaHalf, this.FromPoint.Y);
                    this.ToPoint = new Point(this.ToPoint.X + deltaHalf, this.ToPoint.Y);
                }
            }

            if (this.MinSize.Width > area.Width)
            {
                // push from and to awy to get some space

                var deltaHalf = ((this.MinSize.Width - area.Width) * 0.5);
                if (this.FromPoint.Y > this.ToPoint.Y)
                {
                    this.FromPoint = new Point(this.FromPoint.X, this.FromPoint.Y+deltaHalf);
                    this.ToPoint = new Point(this.ToPoint.X, this.ToPoint.Y-deltaHalf);
                }
                else
                {
                    this.FromPoint = new Point(this.FromPoint.X, this.FromPoint.Y-deltaHalf);
                    this.ToPoint = new Point(this.ToPoint.X, this.ToPoint.Y+deltaHalf);
                }
            }
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
        private EntityRelationshipViewModel entityRelationshipViewModel;
        private EntityConnectorViewModel entityConnectorViewModel1;
        private EntityConnectorViewModel entityConnectorViewModel2;
        private Relationship relationship;

        private void UpdateConnectionPoints()
        {
            this.ConnectionPoints = new List<Point>()
            {                       
                new Point( this.FromPoint.X  <  this.ToPoint.X ? 0d : this.Area.Width, this.FromPoint.Y  <  this.ToPoint.Y ? 0d : this.Area.Height ), 
                new Point( this.FromPoint.X  >  this.ToPoint.X ? 0d : this.Area.Width, this.FromPoint.Y  >  this.ToPoint.Y ? 0d : this.Area.Height)
            };
        }

        #endregion 

        #region ListBox publishes its desired size to the view model

        public Size MinSize
        {
            get
            {
                return this.minSize;
            }
            set
            {
                if (this.minSize == value)
                    return;
                this.minSize = value;
                this.RaisePropertyChanged(() => this.MinSize);
                this.UpdateAreaFromMinSize();
            }
        }

        private Size minSize;

        #endregion

        #region Decide if a specfied point lies 'within' the relatioship

        public bool IsHit(Point pt)
        {
            Point closest;
            return this.CalulateDistanceOfPoint(pt, this.FromPoint, this.ToPoint, out closest) < 10.0;
        }

        private double CalulateDistanceOfPoint(Point pt, Point p1, Point p2, out Point closest)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.        
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.    
            double t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's    
            // end points or a point in the middle.    
            if (t < 0)
            {
                closest = new Point(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Point(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Point(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion
        
        #region Override RefreshIsVisible behaviour

        public override void RefreshIsVisible()
        {
            base.RefreshIsVisible();
            this.IsVisible = this.AssignedFacets.Any(at => at.Facet.IsVisible);
          
            this.From.Entity.RefreshIsVisible();
            if(this.To != null)
                this.To.Entity.RefreshIsVisible();
        }

        #endregion 

        #region Override base class behaviour on changed selection state

        protected override void OnIsSelectedChanged(bool newValue)
        {
            base.OnIsSelectedChanged(newValue);

            // the selection is prpagetd to the assigned tags
            foreach (var at in this.AssignedFacets)
                at.Facet.IsItemSelected = true;
        }

        #endregion 
    
        #region ILayoutEdge Members

        ILayoutNode ILayoutEdge.Source
        {
            get { return this.From.Entity as ILayoutNode; }
        }

        ILayoutNode ILayoutEdge.Destination
        {
            get { return this.To.Entity as ILayoutNode; }
        }

        #endregion

        #region ListBox publishes its actual Size zo the view model

        public double ListBoxActualHeight
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
                this.RaisePropertyChanged(() => this.ListBoxActualHeight);
            }
        }

        private double actualHeight;

        public double ListBoxActualWidth
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
                this.RaisePropertyChanged(() => this.ListBoxActualWidth);
            }
        }

        private double actualWidth;

        #endregion

    }
}
