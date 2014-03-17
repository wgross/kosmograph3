namespace KosmoGraph.Desktop.View
{
    using KosmoGraph.Desktop.ViewModel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class EntityRelationshipCanvas 
    {
        private Point? rubberbandSelectionStartPoint = null;

        #region State of a pending relationship

        public ConnectorControl SourceConnector
        {
            get 
            { 
                return this.sourceConnector; 
            }
            set
            {
                if (this.sourceConnector != value)
                {
                    this.sourceConnector = value;
                    
                    Rect rectangleBounds = sourceConnector.TransformToVisual(this).TransformBounds(new Rect(this.sourceConnector.RenderSize));
                    Point point = new Point(rectangleBounds.Left + (rectangleBounds.Width / 2), rectangleBounds.Bottom + (rectangleBounds.Height / 2));

                    var entityConnectorViewModel = this.sourceConnector.DataContext as EntityViewModel;
                    this.entitiesHit.Add(entityConnectorViewModel);

                    // the relationship is temporariy fake-added to the Items (not to the model itself)
                    // to be drawn on the canvas
                    //this.pendingRelationship = new RelationshipViewModel(entityConnectorViewModel.CentralConnector, point);
                    //entityConnectorViewModel.Model.Items.Add(this.pendingRelationship);
                }
            }
        }

        private ConnectorControl sourceConnector = null;

        private RelationshipViewModel pendingRelationship;

        #endregion 

        #region Override mouse events

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //if we are source of event, we are rubberband selecting
                if (e.Source == this)
                {
                    // in case that this click is the start for a 
                    // drag operation we cache the start point
                    this.rubberbandSelectionStartPoint = e.GetPosition(this);

                    if(this.Model != null)
                        if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                            this.Model.ClearSelectedItems();
                    
                    e.Handled = true;
                }
            }
            if (e.ClickCount == 2)
            {
                var senderAsFrameworkElement = e.Source as FrameworkElement;

                if (senderAsFrameworkElement == null)
                    return;

                if (this.Model == null)
                    return;
                // editing of entity is triggered by the Drag thumb now
                //if (senderAsFrameworkElement.DataContext is EntityViewModel)
                //{
                //    EntityRelationshipModelCommands.EditEntity.Execute(senderAsFrameworkElement.DataContext as EntityViewModel, this);
                //}
                //else 
                {
                    // no entity hit, try relationships
                    var relationshipHit = this.Model.Relationships.FirstOrDefault(r=>r.IsHit(e.GetPosition(this))); 
                    if (relationshipHit!=null)
                        EntityRelationshipModelCommands.EditRelationship.Execute(relationshipHit, this);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.SourceConnector != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPoint = e.GetPosition(this);
                    this.pendingRelationship.ToPoint = currentPoint;
                    this.HitTestingWhilePendingConnection(currentPoint);
                }
            }
            else
            {
                // if mouse button is not pressed we have no drag operation, ...
                if (e.LeftButton != MouseButtonState.Pressed)
                    rubberbandSelectionStartPoint = null;

                // ... but if mouse button is pressed and start
                // point value is set we do have one
                if (this.rubberbandSelectionStartPoint.HasValue)
                {
                    // create rubberband adorner
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    if (adornerLayer != null)
                    {
                        RubberbandSelectionAdorner adorner = new RubberbandSelectionAdorner(this, this.rubberbandSelectionStartPoint.Value);
                        if (adorner != null)
                            adornerLayer.Add(adorner);
                    }
                }
            }
            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (sourceConnector != null)
            {
                // the relatinship is removed from the items. It is added regularily
                // by the command handler
                this.pendingRelationship.From.Entity.Model.Items.Remove(this.pendingRelationship);

                if (this.entitiesHit.Count == 2)
                {
                    this.pendingRelationship.To = this.entitiesHit.Last().CentralConnector;
                    EntityRelationshipModelCommands.CreateRelationship.Execute(this.pendingRelationship,this);
                }
                else if (this.entitiesHit.Count == 1)
                {
                    //this.Model.CreateNewRelationship()
                    //TODO:
                    var pendingEntity = this.Model.CreateNewEntity(); ///string.Format("Entity {0}", this.Model.Entities.Count()));
                    //pendingEntity.Top=this.pendingRelationship.ToPoint.Y;
                    //pendingEntity.Left=this.pendingRelationship.ToPoint.X;
                    //this.pendingRelationship.To=pendingEntity.CentralConnector;

                    EntityRelationshipModelCommands.CreateRelationshipWithEntity.Execute(this.pendingRelationship, this);
                }
                
            }
            this.entitiesHit = new List<EntityViewModel>();
            this.sourceConnector = null;
        }

        #endregion

        #region Test given position while dragging a pending connection of canvas

        private void HitTestingWhilePendingConnection(Point hitPoint)
        {
            var hitObject = this.InputHitTest(hitPoint) as DependencyObject;
            
            while (hitObject != null && hitObject.GetType() != typeof(EntityRelationshipCanvas))
            {
                var hitObjectAsFrameworkElement = hitObject as FrameworkElement;
                if (hitObjectAsFrameworkElement == null || !(hitObjectAsFrameworkElement.DataContext is EntityViewModel))
                {
                    hitObject = VisualTreeHelper.GetParent(hitObject);
                    continue; // not a candidate to look into
                }

                var hitObjectAsFrameworkElementDataContext = hitObjectAsFrameworkElement.DataContext as EntityViewModel;
                if (this.entitiesHit.Contains(hitObjectAsFrameworkElement.DataContext as EntityViewModel))
                {
                    hitObject = VisualTreeHelper.GetParent(hitObject);
                    continue; // already visited this one
                }
                 
                this.entitiesHit.Add(hitObjectAsFrameworkElementDataContext);
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }
        }

        private List<EntityViewModel> entitiesHit = new List<EntityViewModel>();

        #endregion 
    }
}
