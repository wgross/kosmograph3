namespace KosmoGraph.Desktop.View
{
    using KosmoGraph.Desktop.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    public sealed class EntityRelationshipStyleSelector : StyleSelector
    {
        public static readonly EntityRelationshipStyleSelector Singleton = new EntityRelationshipStyleSelector();

        public override Style SelectStyle(object item, DependencyObject container)
        {
            ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
            if (itemsControl == null)
                throw new InvalidOperationException("EntityRelationshipStyleSelector : Could not find ItemsControl");

            if (item is EntityViewModel)
            {
                return (Style)itemsControl.FindResource("entityInDiagramStyle");
            }
            else if (item is RelationshipViewModel)
            {
                return (Style)itemsControl.FindResource("relationshipInDiagramStyle");
            }

            return null;
        }
    }
}
