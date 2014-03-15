namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
using KosmoGraph.Model;
using KosmoGraph.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class EditExistingFacetViewModel : EditFacetViewModelBase
    {
        #region Construction and Initalization of this instance

        public EditExistingFacetViewModel(FacetViewModel edited, IManageFacets withFacets)
            : base(Resources.EditExistingTagViewModelTitle)
        {
            this.Edited = edited;
            this.facets = withFacets;
            this.ExecuteRollback();
        }

        private readonly IManageFacets facets;

        public FacetViewModel Edited { get; private set; }

        #endregion 

        #region Add Property Definition 

        protected override bool CanExecuteAddPropertyDefinition()
        {
            return (!this.hasAlreadyCommitted);
        }

        override protected void AddPropertyDefinitionExecuted()
        {
            this.Properties.Add(new EditNewPropertyDefinitionViewModel
            {
                Name = string.Format(Resources.EditNewFacetNewPropertyNameDefault, this.Properties.Count() + 1)
            });
        }

        #endregion 

        #region Commit Editor

        private bool hasAlreadyCommitted = false;

        override protected void ExecuteCommit()
        {
            if (this.hasAlreadyCommitted)
                return;

            this.Edited.Name = this.Name;

            // removed missing properties

            this.Edited.Properties.ToList().ForEach(pd=>
            {
                if (!this.Properties.Any(edit_pd => edit_pd.Name == pd.Name))
                    this.Edited.Remove(pd);
            });

            // add new properties

            this.Properties.OfType<EditNewPropertyDefinitionViewModel>().ForEach(pd =>
            {
                this.Edited.Add(this.Edited.CreateNewPropertyDefinition(pd.Name));
            });
            
            // send edited facet to application layer

            this.facets
                .UpdateFacet(this.Edited.ModelItem)
                .EndWith(succeeded:f => this.Edited.Model.UpdateAssignedFacets(this.Edited));

            this.hasAlreadyCommitted = true;
            this.isPropertiesChanged = false;
        }

        override protected bool CanExecuteCommit()
        {
            return (!this.HasError) 
                && (
                    (this.Edited.Name != this.Name) 
                    || 
                    this.isPropertiesChanged 
                    //|| 
                    //this.Properties.Any( epd => epd.Commit.CanExecute())
                );
        }

        #endregion 

        #region Rollback Editor

        override protected void ExecuteRollback()
        {
            this.Name = this.Edited.Name;
            this.Properties = new ObservableCollection<IEditPropertyDefinition>(this.Edited.Properties.Select(pd => new EditExistingPropertyDefinitionViewModel(pd)));
            this.Properties.CollectionChanged += Properties_CollectionChanged;
            this.isPropertiesChanged = false;
        }

        override protected bool CanExecuteRollback()
        {
            return (!this.hasAlreadyCommitted);
        }

        #endregion 
    
        
        protected override bool CanExecuteRemovePropertyDefinition(IEditPropertyDefinition propertyDefinitionToRemove)
        {
            return this.Properties.Contains(propertyDefinitionToRemove) && (!this.hasAlreadyCommitted);
        }
    }
}
