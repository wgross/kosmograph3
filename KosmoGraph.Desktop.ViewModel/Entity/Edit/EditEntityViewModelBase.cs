
namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using KosmoGraph.Services;

    public abstract class EditEntityViewModelBase : EditFacetedViewModelBase
    {
        #region Construction and Initialization of this instance

        public EditEntityViewModelBase(EntityRelationshipViewModel model, IManageEntities entities, string withTitleFormat)
            : base(model)
        {
            this.titleFormat = withTitleFormat;
            this.ManageEntities = entities;    
        }
        
        private readonly string titleFormat;

        protected IManageEntities ManageEntities { get; private set; }

        public bool EnableCommit { get; set; }

        #endregion 

        #region Edit the name of the entity

        public string Title
        {
            get
            {
                return string.Format(this.titleFormat, this.Name);
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if(this.SetAndInvalidate(() => this.Name, ref this.name, value))
                    this.RaisePropertyChanged(() => this.Title);
            }
        }

        private string name;

        #endregion 

        #region Validate/Prepare Commit Editor

        protected override void ExecutePrepareCommit()
        {
            this.ValidateEntityEditor().EndWith(
                succeeded: result =>
                {
                    this.ClearErrors();

                    if (result.NameIsNullOrEmpty)
                        this.SetError(() => this.Name, Resources.ErrorEntityNameIsNullOrEmpty);
                    if (result.NameIsNotUnique)
                        this.SetError(() => this.Name, Resources.ErrorEntityNameIsNotUnique);

                    this.IsValid = !(result.NameIsNullOrEmpty || result.NameIsNotUnique);
                });
        }

        private Task<ValidateEntityResult> ValidateEntityEditor()
        {
            if (StringComparer.CurrentCultureIgnoreCase.Equals(this.Name, Resources.EditNewEntityViewModelNameDefault))
                return Task.FromResult(new ValidateEntityResult { NameIsNullOrEmpty = true });

            return this.ManageEntities.ValidateEntity(this.Name);
        }

        #endregion
    }
}
