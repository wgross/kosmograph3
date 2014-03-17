namespace KosmoGraph.Desktop.View
{
    using System.Linq;
    using KosmoGraph.Desktop.ViewModel;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    public class EntityDragThumb : Thumb
    {
        public static readonly RoutedEvent EntityDraggedEvent;

        static EntityDragThumb()
        {
            EntityDraggedEvent = EventManager.RegisterRoutedEvent(
                    "EntityDragged",
                     RoutingStrategy.Bubble,
                     typeof(RoutedEventHandler),
                     typeof(EntityDragThumb));
        }

        public EntityDragThumb()
        {
            base.DragDelta += new DragDeltaEventHandler(EntityDragThumb_DragDelta);
        }

        public event RoutedEventHandler EntityDragged
        {
            add 
            { 
                this.AddHandler(EntityDraggedEvent, value); 
            }
            remove 
            {
                this.RemoveHandler(EntityDraggedEvent, value); 
            }
        }

        void EntityDragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var draggedItem = this.DataContext as EntityViewModel;
            
            if( draggedItem != null )
            {
                draggedItem.Left +=e.HorizontalChange;
                draggedItem.Top += e.VerticalChange;

                e.Handled = true;
            }
        }

        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            EntityRelationshipModelCommands.EditEntity.Execute(this.DataContext as EntityViewModel, this);
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.RaiseEvent(new RoutedEventArgs(EntityDragThumb.EntityDraggedEvent, this));

        }
    }
}
