
namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditAssignedFacetViewModelBase : NotificationObject
    {
        public EditAssignedFacetViewModelBase(FacetViewModel assignedFacet)
        {
            this.Facet = assignedFacet;
        }

        public FacetViewModel Facet { get; private set; }

        public string Name
        {
            get
            {
                return this.Facet.Name;
            }
        }
    }
}
