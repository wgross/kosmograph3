
namespace KosmoGraph.Desktop.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPropertyValue
    {
        IPropertyDefinition Definition { get; }

        string Value { get;  }
    }

    public interface IEditProperytValue
    {
        new string Value { get; set; }
    }
}
