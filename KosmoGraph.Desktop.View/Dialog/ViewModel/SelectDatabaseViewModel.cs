

namespace KosmoGraph.Desktop.Dialog.ViewModel
{
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SelectModelDatabaseViewModel : NotificationObject
    {
        public string Title { get; set;}

        public string DatabaseName { get; set; }


    }
}
