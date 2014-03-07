//namespace KosmoGraph.Desktop.ViewModel
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Collections.ObjectModel;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;

//    public class PendingAssignedFacetViewModel : EditAssignedFacetViewModelBase
//    {
//        public PendingAssignedFacetViewModel(FacetViewModel assignedFacet)
//            : base(assignedFact)
//        {
//            this.Facet = assignedFacet;
//            this.Properties = new ObservableCollection<PendingAssignedFacetPropertyDefinitionViewModel>(
//                this.Facet
//                    .Properties
//                    .Select(pd => new PendingAssignedFacetPropertyDefinitionViewModel(pd)));
//        }

//        public FacetViewModel Facet { get; private set; }

//        public string Name
//        {
//            get
//            {
//                return this.Facet.Name;
//            }
//        }

//        public IEnumerable<PendingAssignedFacetPropertyDefinitionViewModel> Properties
//        {
//            get;
//            private set;
//        }
//    }
//}
