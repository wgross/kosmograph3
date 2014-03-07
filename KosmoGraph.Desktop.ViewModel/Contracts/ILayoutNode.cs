using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KosmoGraph.Desktop.ViewModel
{
    public interface ILayoutNode
    {
        double DX { get; set; }
        double DY { get; set; }
        double Left { get; set; }
        double Top{ get; set; }
    }
}
