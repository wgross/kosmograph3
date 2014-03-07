
namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditAssignedEntityTagViewModel : EditAssignedFacetViewModelBase
    {
        #region Construction ad Initialization of this instance

        public EditAssignedEntityTagViewModel(AssignedFacetViewModel assignedTag)
            : base(assignedTag.Facet)
        {
            if (assignedTag == null)
                throw new ArgumentNullException("assignedTag");

            this.Edited = assignedTag;
            this.rollbackCommand = new DelegateCommand(this.RollbackExecuted, this.RollbackCanExecute);
            this.commitCommand = new DelegateCommand(this.CommitExecuted, this.CommitCanExecute);
            this.RollbackExecuted();
        }

        public AssignedFacetViewModel Edited { get; private set; }

        #endregion 
    
       

        #region Commit Editor

        public DelegateCommand Commit
        {
            get
            {
                return this.commitCommand;
            }
        }

        private readonly DelegateCommand commitCommand;

        private void CommitExecuted()
        {
            //foreach(var property in this.Properties)
            //    if(property.Commit.CanExecute())
            //        property.Commit.Execute();
            
            this.Commit.RaiseCanExecuteChanged();
        }

        private bool CommitCanExecute()
        {
            return false;// return this.Properties.Any(p => p.Commit.CanExecute());
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

        private readonly DelegateCommand rollbackCommand;

        private void RollbackExecuted()
        {
            //this.Properties = new ObservableCollection<EditEntityPropertyValueViewModelBase>(this.Edited.Properties.Select(p => new EditEntityPropertyValueViewModel(p)));
            //this.RaisePropertyChanged(() => this.Properties);
        }

        private bool RollbackCanExecute()
        {
            return true;
        }

        #endregion 
    }
}
