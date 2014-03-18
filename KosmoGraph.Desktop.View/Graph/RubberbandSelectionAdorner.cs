namespace KosmoGraph.Desktop.View
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System;
    using KosmoGraph.Desktop.ViewModel;

    public sealed class RubberbandSelectionAdorner : Adorner
    {
        #region Construction and Initialization of this instance

        public RubberbandSelectionAdorner(EntityRelationshipCanvas canvas, Point dragStartPoint)
            : base(canvas)
        {
            this.entityRelationshipCanvas = canvas;
            this.startPoint = dragStartPoint;
            this.rubberbandPen = new Pen(Brushes.Red, 1);
            this.rubberbandPen.DashStyle = new DashStyle(new double[] { 2 }, 1);
        }

        private readonly EntityRelationshipCanvas entityRelationshipCanvas;
        
        private readonly Point startPoint;
        
        private readonly Pen rubberbandPen;

        #endregion 

        #region Increase the selected area as the mouse moves

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                    this.CaptureMouse();

                this.endPoint = e.GetPosition(this);
                this.UpdateSelection();
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) 
                    this.ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        private Point? endPoint;
        
        #endregion 

        #region Remove this adorner from Adorner layer if mouse buttons gets up

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            // release mouse capture
            if (this.IsMouseCaptured) 
                this.ReleaseMouseCapture();

            // remove this adorner from adorner layer
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.entityRelationshipCanvas);
            if (adornerLayer != null)
                adornerLayer.Remove(this);

            e.Handled = true;
        }

        #endregion

        #region Draw a rectangle as visual representation
 
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // without a background the OnMouseMove event would not be fired !
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(this.RenderSize));

            if (this.endPoint.HasValue)
                dc.DrawRectangle(Brushes.Transparent, rubberbandPen, new Rect(this.startPoint, this.endPoint.Value));
        }

        #endregion 

        #region Mark all entities and relationships as selected that lie within the selection rectangle
 
        private void UpdateSelection()
        {
            EntityRelationshipViewModel vm = (this.entityRelationshipCanvas.DataContext as EntityRelationshipViewModel);
            if (vm == null)
                return;

            Rect rubberBand = new Rect(this.startPoint, this.endPoint.Value);
            
            ItemsControl itemsControl = GetParent<ItemsControl>(typeof(ItemsControl), entityRelationshipCanvas);

            foreach (ModelItemViewModelBase item in vm.Items)
            {
                DependencyObject container = itemsControl.ItemContainerGenerator.ContainerFromItem(item);

                Rect itemRect = VisualTreeHelper.GetDescendantBounds((Visual)container);
                Rect itemBounds = ((Visual)container).TransformToAncestor(this.entityRelationshipCanvas).TransformBounds(itemRect);

                if (rubberBand.Contains(itemBounds))
                {
                    item.IsSelected = true;
                }
                else
                {
                    if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    {
                        item.IsSelected = false;
                    }
                }
            }
        }

        private T GetParent<T>(Type parentType, DependencyObject dependencyObject) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent.GetType() == parentType)
                return (T)parent;

            return GetParent<T>(parentType, parent);
        }

        #endregion 
    }
}
