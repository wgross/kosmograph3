namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class EditExistingPropertyValueViewModel : EditPropertyValueViewModelBase
    {
        #region Construction and initialization of this instance

        public EditExistingPropertyValueViewModel(PropertyValueViewModel value)
            : base(value.Definition)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.Edited = value;
            this.Rollback = new DelegateCommand(this.RollbackExecuted, this.RollbackCanExecute);
            this.Commit = new DelegateCommand(this.CommitExecuted, this.CommitCanExecute);
            this.RollbackExecuted();
        }

        public PropertyValueViewModel Edited { get; private set; }

        #endregion 

        public override bool IsDirty
        {
            // is dirty if new vale is different from old value
            get { return this.Value != this.Edited.Value; }
        }

        #region Commit editor

        public DelegateCommand Commit
        {
            get; private set;
        }

        private void CommitExecuted()
        {
            this.Edited.Value = this.Value;
            this.Commit.RaiseCanExecuteChanged();
        }

        private bool CommitCanExecute()
        {
            return this.Value!=this.Edited.Value;
        }

        #endregion 

        #region Rollback editor

        public DelegateCommand Rollback
        {
            get; private set;
        }

        private void RollbackExecuted()
        {
            this.Value = this.Edited.Value;
            this.Commit.RaiseCanExecuteChanged();
        }

        private bool RollbackCanExecute()
        {
            return true;
        }

        #endregion 
    }
}
