namespace KosmoGraph.Desktop.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class EditExistingAssignedFacetViewModel : EditAssignedFacetViewModelBase
    {
        public EditExistingAssignedFacetViewModel(AssignedFacetViewModel assignedFacet)
            : base(assignedFacet.Facet)
        {
            this.Edited = assignedFacet;
        }

        public AssignedFacetViewModel Edited { get; private set; }
    }
}
