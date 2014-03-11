
namespace KosmoGraph.Desktop.View.Layout
{
    using KosmoGraph.Desktop.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public sealed class SpringEmbedderLayout
    {
        #region Construction and Initialization

        public SpringEmbedderLayout(int iterationsToRun)
		{
			this.iterations = Math.Max(1,iterationsToRun);
		}
		
        readonly Random random = new Random();
		
        readonly int iterations;
		
        #endregion

        public void Start(ILayoutNode[] nodes, ILayoutEdge[] edges)
        {
            this.LetEdgesPull(edges);
            this.LetNodesPush(nodes);
        }

        private void LetNodesPush(ILayoutNode[] nodes)
        {
            // the nodes push each other
            //  - if nodes overlay each other, they randomly move away
            //  - if nodes are close, they a moved away slightly

            for(int i = 0; i < nodes.Length; i++ )
            {
                var n1 = nodes[i];
                double new_dx = 0;
                double new_dy = 0;
                
                // evaluate n1 distance to all other nodes.

                for (int j = 0; j < nodes.Length; j++)
                {
                    var n2 = nodes[j];

                    if (n1 == n2)
                        continue;

                    double dx = n1.GetHorizontalDistance(n2);
                    double dy = n1.GetVerticalDistance(n2);
                    // Pythagoras: a^2+b^2 = c^2
                    double c_quad = dx * dx + dy * dy;

                    // handle the pathological cases

                    if (c_quad <= double.Epsilon)
                    {
                        // the two nodes origin coordinates (Top/Left) overlap -> Move away randomly
                        new_dx += this.random.NextDouble();
                        new_dy += this.random.NextDouble();
                    }
                    else if (c_quad < 100 * 100)
                    {
                        // the nodes are close (less than 100px )
                        // Push n1 them away, proportional to its distance to n2.
                        new_dx += dx / c_quad;
                        new_dy += dy / c_quad;
                    }
                }

                // after close nodes and overlapping nodes are handled,
                // a resulting delta is calculated.
                // Pythagoras again
                double delta_quad = new_dx * new_dx + new_dy * new_dy;

                if (delta_quad > double.Epsilon)
                {
                    delta_quad = Math.Sqrt(delta_quad) / 2;

                    n1.DX += (new_dx / delta_quad);
                    n1.DY += (new_dy / delta_quad);
                }

                // the positional change is limited to 5 in a single step
                // now apply computed delta to the ccordinates.

                n1.Left += Math.Max(-5, Math.Min(5, n1.DX));
                if (n1.Left < 0)
                    n1.Left = 0;

                n1.Top += Math.Max(-5, Math.Min(5, n1.DY));
                if (n1.Top < 0)
                    n1.Top = 0;

                n1.DX = n1.DX / 2;
                n1.DY = n1.DY / 2;
            }
        }

        private void LetEdgesPull(ILayoutEdge[] edges)
        {
            for( int i = 0; i < edges.Length; i++)
            {
                var e = edges[i];

                // the edge pulls the nodes together
                double edgeChangeFactor = this.GetDistanceChangeFactor(e);
                double dx = e.GetHorizontalDistance();
                double dy = e.GetVerticalDistance();
                double new_dx = edgeChangeFactor * dx;
                double new_dy = edgeChangeFactor * dy;

                e.Destination.DX += new_dx;
                e.Destination.DY += new_dy;
                e.Source.DX -= new_dx;
                e.Source.DY -= new_dy;
            }
        }

        private double GetDistanceChangeFactor(ILayoutEdge edge)
        {
            return 1/ (edge.GetDistance()*3);
        }
    }
}
