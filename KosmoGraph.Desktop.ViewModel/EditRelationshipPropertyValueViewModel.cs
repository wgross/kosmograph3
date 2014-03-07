namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class EditRelationshipPropertyValueViewModel : NotificationObject
    {
        #region Construction and initialization of this instance

        public EditRelationshipPropertyValueViewModel(PropertyValueViewModel value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.Edited = value;
            this.rollbackCommand = new DelegateCommand(this.RollbackExecuted, this.RollbackCanExecute);
            this.commitCommand = new DelegateCommand(this.CommitExecuted, this.CommitCanExecute);
            this.RollbackExecuted();
        }

        public PropertyValueViewModel Edited { get; private set; }

        #endregion 

        #region Edit the properties value

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.value == value)
                    return;
                this.value = value;
                this.RaisePropertyChanged(() => this.Value);
            }
        }

        private string value;

        #endregion 

        #region Commit editor

        public DelegateCommand Commit
        {
            get
            {
                return this.commitCommand;
            }
        }

        private DelegateCommand commitCommand = null;

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
            get
            {
                return this.rollbackCommand;
            }
        }

        private DelegateCommand rollbackCommand = null;

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
