namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using KosmoGraph.Services;
    using System.Collections.Generic;
    using System.Collections;

    public abstract class EditModelItemViewModelBase : ModelItemViewModelBase, INotifyDataErrorInfo
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

        #region Notification and Validation Helper

        protected bool SetAndInvalidate<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            var changed = this.Set(propertyExpression, ref field, newValue);
            if (changed)
            {
                this.IsValid = null;
                this.ClearErrors(propertyExpression);
            }
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

        //protected void ValidatePrepareCommit(Task<bool> validateFrom)
        //{
        //    validateFrom.EndWith(
        //        succeeded: valid => 
        //        {
        //            this.IsValid  = valid;
        //            this.HasError = !valid;
        //        },
        //        failed: ex => 
        //        { 
        //            this.HasError = true; 
        //            return true; 
        //        });
        //}

        #endregion

        #region Commit Editor

        public DelegateCommand Commit { get; private set; }

        abstract protected void ExecuteCommit();

        virtual protected bool CanExecuteCommit()
        {
            return (this.IsValid.GetValueOrDefault(false) && !this.HasErrors);
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

        #region INotifyDataErrorInfo Members
        
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            List<string> errorMessages;
            if (!this.errors.TryGetValue(propertyName, out errorMessages))
                return Enumerable.Empty<string>();
            return errorMessages;
        }

        public bool HasErrors
        {
            get 
            {
                return this.errors.Any();
            }
        }

        #endregion

        #region Internal error handling

        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        private void ClearErrors<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = propertyExpression.GetPropertyName();

            List<string> errorMessages;
            if (this.errors.TryGetValue(propertyName, out errorMessages))
            {
                errorMessages.Clear();
            }
        }

        protected void ClearErrors()
        {
            this.errors.Clear();
        }

        protected void SetError<T>(Expression<Func<T>> propertyExpression, string message)
        {
            string propertyName = propertyExpression.GetPropertyName();

            List<string> errorMessages;
            if (!this.errors.TryGetValue(propertyName, out errorMessages))
            {
                errorMessages = new List<string>();
                this.errors.Add(propertyName, errorMessages);
            }

            errorMessages.Add(message);

            this.RaiseErrorsChanged(propertyName);
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            var tmp = this.ErrorsChanged;
            if(tmp!=null)
                tmp(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion 
    }
}
