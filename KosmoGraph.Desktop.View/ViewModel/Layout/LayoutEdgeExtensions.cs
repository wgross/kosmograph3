namespace KosmoGraph.Desktop.View.Layout
{
    using KosmoGraph.Desktop.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class LayoutEdgeExtensions
    {
        public static double GetDistance(this ILayoutEdge e)
        {
            double dx = e.GetHorizontalDistance();
            double dy = e.GetVerticalDistance();

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double GetHorizontalDistance(this ILayoutEdge thisEdge)
        {
            return thisEdge.Source.Left - thisEdge.Destination.Left;
        }

        public static double GetVerticalDistance(this ILayoutEdge thisEdge)
        {
            return thisEdge.Source.Top - thisEdge.Destination.Top;
        }
    }
}
