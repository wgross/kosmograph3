
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

    public class EditAssignedRelationshipFacetViewModel : NotificationObject
    {
        #region Construction ad Initialization of this instance

        public EditAssignedRelationshipFacetViewModel(AssignedFacetViewModel assignedFacet)
        {
            if (assignedFacet == null)
                throw new ArgumentNullException("assignedFacet");

            this.Edited = assignedFacet;
            this.rollbackCommand = new DelegateCommand(this.RollbackExecuted, this.RollbackCanExecute);
            this.commitCommand = new DelegateCommand(this.CommitExecuted, this.CommitCanExecute);
            this.RollbackExecuted();
        }

        public AssignedFacetViewModel Edited { get; private set; }

        #endregion 
    
        #region Edit the property values of the assigned Relationship Facet
        
        public IEnumerable<EditRelationshipPropertyValueViewModel> Properties
        {
            get
            {
                return this.properties;
            }
        }

        private ObservableCollection<EditRelationshipPropertyValueViewModel> properties;

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
            foreach(var property in this.Properties)
                if(property.Commit.CanExecute())
                    property.Commit.Execute();
            
            this.Commit.RaiseCanExecuteChanged();
        }

        private bool CommitCanExecute()
        {
            return this.Properties.Any(p => p.Commit.CanExecute());
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
            this.properties = new ObservableCollection<EditRelationshipPropertyValueViewModel>(this.Edited.Properties.Select(p => new EditRelationshipPropertyValueViewModel(p)));
            this.RaisePropertyChanged(() => this.Properties);
        }

        private bool RollbackCanExecute()
        {
            return true;
        }

        #endregion 
    }
}
