namespace KosmoGraph.Desktop.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPropertyDefinition
    {
        string Name { get; }
    }

    public interface IEditPropertyDefinition : IPropertyDefinition
    {
        new string Name { get; set; }
    }
}
