using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KosmoGraph.Desktop.ViewModel
{
    public interface ILayoutEdge
    {
        ILayoutNode Source { get; }
        
        ILayoutNode Destination { get; }
    }
}
