namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditModelItemViewModelBase : NotificationObject
    {
        public EditModelItemViewModelBase()
        {
            this.Commit = new DelegateCommand(this.ExecuteCommit, this.CanExecuteCommit);
            this.Rollback = new DelegateCommand(this.ExecuteRollback, this.CanExecuteRollback);
        }

        #region Commit Editor

        public DelegateCommand Commit
        {
            get;
            private set;
        }

        abstract protected void ExecuteCommit();
        
        abstract protected bool CanExecuteCommit();
        
        #endregion

        #region Rollback Editor

        public DelegateCommand Rollback
        {
            get;
            private set;
        }

        abstract protected void ExecuteRollback();
       
        abstract protected bool CanExecuteRollback();
        
        #endregion

        protected void RefreshCommands()
        {
            this.Commit.RaiseCanExecuteChanged();
            this.Rollback.RaiseCanExecuteChanged();
        }
    }
}
