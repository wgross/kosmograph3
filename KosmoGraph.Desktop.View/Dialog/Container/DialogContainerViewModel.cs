namespace KosmoGraph.Desktop.Dialog
{
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    

    public sealed class DialogContainerViewModel : NotificationObject
    {
        #region Dialog Size

        #endregion 

        #region Dialog is populated with this data element

        public ObservableCollection<object> DialogContent
        {
            get
            {
                return this.dialogContent;
            }
            set
            {
                if (object.ReferenceEquals(this.dialogContent, value))
                    return;
                this.dialogContent = value;
                this.RaisePropertyChanged(() => this.DialogContent);
            }
        }

        private ObservableCollection<object> dialogContent;

        #endregion 

        #region Dialog provides dialog actions

        public IEnumerable<DialogAction> DialogActions
        {
            get
            {
                return this.dialogActions;
            }
            set
            {
                if (object.ReferenceEquals(value, this.dialogActions))
                    return;

                this.dialogActions = value;
                this.RaisePropertyChanged(() => this.DialogActions);
            }
        }

        private IEnumerable<DialogAction> dialogActions;

        #endregion
    }
}
