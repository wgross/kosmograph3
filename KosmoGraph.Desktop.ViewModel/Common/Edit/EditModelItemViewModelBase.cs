namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.Commands;

    public abstract class EditModelItemViewModelBase : ModelItemViewModelBase
    {
        #region Construction and initialization of this instance

        public EditModelItemViewModelBase(EntityRelationshipViewModel model)
            : base(model)
        {
            this.PrepareCommit = new DelegateCommand(this.ExecutePrepareCommit, this.CanExecutePrepareCommit);
            this.Commit = new DelegateCommand(this.ExecuteCommit, this.CanExecuteCommit);
            this.Rollback = new DelegateCommand(this.ExecuteRollback, this.CanExecuteRollback);
        }

        #endregion

        #region Validate/Prepare Commit Editor

        public DelegateCommand PrepareCommit { get; private set; }

        virtual protected void ExecutePrepareCommit() { }

        virtual protected bool CanExecutePrepareCommit() { return false; }

        #endregion

        #region Commit Editor

        public DelegateCommand Commit { get; private set; }

        abstract protected void ExecuteCommit();

        abstract protected bool CanExecuteCommit();

        #endregion

        #region Rollback Editor

        public DelegateCommand Rollback { get; private set; }

        abstract protected void ExecuteRollback();

        abstract protected bool CanExecuteRollback();

        #endregion

        protected void RefreshCommands()
        {
            this.PrepareCommit.RaiseCanExecuteChanged();
            this.Commit.RaiseCanExecuteChanged();
            this.Rollback.RaiseCanExecuteChanged();
        }
    }
}
