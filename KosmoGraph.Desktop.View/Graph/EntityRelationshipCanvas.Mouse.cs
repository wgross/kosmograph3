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
                return this.sourceConnectorControl; 
            }
            set
            {
                if (this.sourceConnectorControl != value)
                {
                    this.sourceConnectorControl = value;
                    
                    Rect rectangleBounds = sourceConnectorControl.TransformToVisual(this).TransformBounds(new Rect(this.sourceConnectorControl.RenderSize));
                    Point point = new Point(rectangleBounds.Left + (rectangleBounds.Width / 2), rectangleBounds.Bottom + (rectangleBounds.Height / 2));

                    var sourceEntityViewModel = this.sourceConnectorControl.DataContext as EntityViewModel;
                    this.entitiesHit.Add(sourceEntityViewModel);

                    // the relationship is temporariy fake-added to the Items (not to the model itself)
                    // to be drawn on the canvas
                    this.pendingRelationship = this.Model.CreatePendingRelationship(sourceEntityViewModel);
                    this.pendingRelationship.ToPoint = point;
                }
            }
        }

        private ConnectorControl sourceConnectorControl = null;

        private EditNewRelationshipViewModel pendingRelationship;

        #endregion 

        #region Override mouse events

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.Source == this)
                {
                    // if this canvas is the event source, we are rubberband selecting
                    // this click is the start for a drag operation og ehich remember the start point
                    this.rubberbandSelectionStartPoint = e.GetPosition(this);

                    if (this.Model != null)
                    {
                        // on pressend Ctrl key, th seection is added to an existig one. 
                        // of the key isn't pressed, the current selection is removed
                        if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                            this.Model.ClearSelectedItems();
                    }
                    
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
                // editing of entity is triggered by the Drag thumb now. Hit test in canvas is obsolete
                //if (senderAsFrameworkElement.DataContext is EntityViewModel)
                //{
                //    EntityRelationshipModelCommands.EditEntity.Execute(senderAsFrameworkElement.DataContext as EntityViewModel, this);
                //}
                //else 
                {
                    // a relatinship was doubel clicked. 
                    // sent edit command with hit relatinship
                    var relationshipHit = this.Model.Relationships.FirstOrDefault(r=>r.IsHit(e.GetPosition(this))); 
                    if (relationshipHit!=null)
                        EntityRelationshipModelCommands.EditRelationship.Execute(relationshipHit, this);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.SourceConnector != null && this.pendingRelationship!=null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    // The mouse is moved while the left button is pressend, a source connection is set
                    // and a pending relationship exists:
                    // this is a relationship creation. The penden relationships destination point is updated
                    // during the move. 
                    // foreach point during move a hit test is performed to lookup entoties under the cursor
                    Point currentPoint = e.GetPosition(this);
                    this.pendingRelationship.ToPoint = currentPoint;
                    this.HitTestingWhilePendingConnection(currentPoint);
                }
            }
            else
            {
                // if mouse button is not pressed we have no drag operation, ...
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    // a mouse button is not pressed. 
                    // cleanup: rubber band selection start point, pending relationship?
                    // 
                    rubberbandSelectionStartPoint = null;
                }
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

            if (sourceConnectorControl != null && this.pendingRelationship != null)
            {
                // there is a source connector control AND a pending relationship.
                // This has to be the finalization of a relatiosnhip creation.
                // in any cases the oending relationship is removed from the 
                // diagram items drawn for now:
                
                if (this.entitiesHit.Count == 2)
                {
                    this.pendingRelationship.SetDestination.Execute(this.entitiesHit.Last());
                    EntityRelationshipModelCommands.CreateRelationshipWithEntities.Execute(this.pendingRelationship,this);
                }
                //else if (this.entitiesHit.Count == 1)
                //{
                //    //this.Model.CreateNewRelationship()
                //    //TODO:
                //    var pendingEntity = this.Model.CreateNewEntity(); ///string.Format("Entity {0}", this.Model.Entities.Count()));
                //    //pendingEntity.Top=this.pendingRelationship.ToPoint.Y;
                //    //pendingEntity.Left=this.pendingRelationship.ToPoint.X;
                //    //this.pendingRelationship.To=pendingEntity.CentralConnector;

                //    //EntityRelationshipModelCommands.CreateRelationshipWithEntity.Execute(this.pendingRelationship, this);
                //}
                
            }

            // clean temporary data from diagrm canvas 
            this.entitiesHit = new List<EntityViewModel>();
            this.sourceConnectorControl = null;
            this.pendingRelationship = null;
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
