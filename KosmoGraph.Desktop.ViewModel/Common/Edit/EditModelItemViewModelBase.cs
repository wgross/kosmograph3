namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using KosmoGraph.Services;

    public abstract class EditModelItemViewModelBase : ModelItemViewModelBase, IDataErrorInfo
    {
        #region Construction and initialization of this instance

        public EditModelItemViewModelBase(EntityRelationshipViewModel model)
            : base(model)
        {
            this.PrepareCommit = new DelegateCommand(this.ExecutePrepareCommit);
            this.Commit = new DelegateCommand(this.ExecuteCommit, this.CanExecuteCommit);
            this.Rollback = new DelegateCommand(this.ExecuteRollback, this.CanExecuteRollback);
        }

        #endregion

        #region IDataErrorInfo Members

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                return string.Empty;
            }
        }

        public bool HasError { get; protected set; }

        #endregion

        #region Notification and Validation Helper

        protected bool SetAndInvalidate<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            var changed = this.Set(propertyExpression, ref field, newValue);
            if(changed)
                this.IsValid = null;
            return changed;
        }

        protected bool? IsValid { get; set; }

        #endregion 

        #region Validate/Prepare Commit Editor

        public DelegateCommand PrepareCommit { get; private set; }

        virtual protected void ExecutePrepareCommit() 
        {
            throw new NotImplementedException();
        }

        protected void ValidatePrepareCommit(Task<bool> validateFrom)
        {
            validateFrom.EndWith(
                succeeded: valid => 
                {
                    this.IsValid  = valid;
                    this.HasError = !valid;
                },
                failed: ex => 
                { 
                    this.HasError = true; 
                    return true; 
                });
        }

        #endregion

        #region Commit Editor

        public DelegateCommand Commit { get; private set; }

        abstract protected void ExecuteCommit();

        virtual protected bool CanExecuteCommit()
        {
            return (this.IsValid.GetValueOrDefault(false) && !this.HasError);
        }

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
