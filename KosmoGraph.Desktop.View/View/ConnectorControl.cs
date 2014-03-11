namespace KosmoGraph.Desktop.View
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public sealed class ConnectorControl : Control
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            EntityRelationshipCanvas canvas = this.GetEntityRelationshipCanvas(this);
            if (canvas != null)
                canvas.SourceConnector = this;
        }

        private EntityRelationshipCanvas GetEntityRelationshipCanvas(System.Windows.DependencyObject element)
        {
            while (element != null && !(element is EntityRelationshipCanvas))
                element = VisualTreeHelper.GetParent(element);

            return element as EntityRelationshipCanvas;
        }
    }
}
