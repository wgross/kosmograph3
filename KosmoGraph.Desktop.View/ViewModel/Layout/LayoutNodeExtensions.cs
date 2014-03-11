namespace KosmoGraph.Desktop.View.Layout
{
    using KosmoGraph.Desktop.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class LayoutNodeExtensions
    {
        public static double GetDistance(this ILayoutNode node, ILayoutNode other)
        {
            double dx = node.GetHorizontalDistance(other);
            double dy = node.GetVerticalDistance(other);

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double GetHorizontalDistance(this ILayoutNode node, ILayoutNode other)
        {
            return node.Left - other.Left;
        }

        public static double GetVerticalDistance(this ILayoutNode node, ILayoutNode other)
        {
            return node.Top - other.Top;
        }
    }
}
