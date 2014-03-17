﻿
namespace KosmoGraph.Desktop.ViewModel
{
    using KosmoGraph.Desktop.ViewModel.Properties;
    using KosmoGraph.Services;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    public class EditNewFacetViewModel : EditFacetViewModelBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Construction and initialization of this instance

        public EditNewFacetViewModel(EntityRelationshipViewModel withViewModel, IManageFacets withFacets)
            : base(withViewModel, Resources.EditNewFacetViewModelTitle)
        {
            this.facets = withFacets;
            this.ExecuteRollback();
        }

        private bool hasAlreadyCommitted = false;

        private readonly IManageFacets facets;

        #endregion

        #region Add PropertyDefinition to facet

        override protected bool CanExecuteAddPropertyDefinition()
        {
            return !this.hasAlreadyCommitted;
        }

        override protected void AddPropertyDefinitionExecuted()
        {
            this.Properties.Add(new EditNewPropertyDefinitionViewModel()
            {
                Name = string.Format(Resources.EditNewFacetNewPropertyNameDefault, this.Properties.Count() + 1)
            });
        }

        #endregion

        #region Remove PropertyDefinition from Facet

        protected override bool CanExecuteRemovePropertyDefinition(IEditPropertyDefinition propertyDefinitionToRemove)
        {
            return propertyDefinitionToRemove != null && !this.hasAlreadyCommitted;
        }

        #endregion

        #region Commit Editor

        override protected void ExecuteCommit()
        {
            if (this.hasAlreadyCommitted)
                return;

            this.hasAlreadyCommitted = true;

            var scheduleAtUiThread = TaskScheduler.FromCurrentSynchronizationContext();

            this.facets
                .CreateNewFacet(f =>
                {
                    f.Name = this.Name;
                    foreach (var pdvm in this.Properties)
                    {
                        f.Add(f.CreateNewPropertyDefinition(pd =>
                        {
                            pd.Name = pdvm.Name;
                        }));
                    }

                    //// add new property definitions to tag
                    //// this can be done without comparison because Add doesnt add twice the same tag
                    //foreach (var addedPropertyDefinition in this.Properties)
                    //{
                    //    addedPropertyDefinition.Commit.Execute();
                    //    this.Edited.Add(addedPropertyDefinition.Edited);
                    //}

                    //// remove removed property definitions from tag
                    //foreach (var commitedPropertyDefinition in this.Edited.Properties.ToArray())
                    //    if (!this.Properties.Any(pd => pd.Edited.Equals(commitedPropertyDefinition)))
                    //        this.Edited.Remove(commitedPropertyDefinition);
                })
                .EndWith(
                    succeeded: f =>
                    {
                        try
                        {
                            this.Model.Add(f);
                        }
                        catch (Exception ex)
                        {
                            log.ErrorException("Cought excption:", ex);
                        }
                    },
                    failed: ex =>
                    {
                        this.hasAlreadyCommitted = false;
                        return true; // handled ?!
                    },
                    scheduleAt: scheduleAtUiThread);
        }

        override protected bool CanExecuteCommit()
        {
            if (this.HasError)
                return false;

            if (this.hasAlreadyCommitted)
                return false;

            return (
                    !(StringComparer.CurrentCultureIgnoreCase.Equals(this.Name, Resources.EditNewFacetViewModelNameDefault))
                    ||
                    this.Properties.Any()
                );
        }

        #endregion

        #region Rollback Editor

        override protected void ExecuteRollback()
        {
            this.Name = Resources.EditNewFacetViewModelNameDefault;
            this.Properties = new ObservableCollection<IEditPropertyDefinition>();
            this.Properties.CollectionChanged += Properties_CollectionChanged;
            this.isPropertiesChanged = false;
            this.hasAlreadyCommitted = false;
        }

        override protected bool CanExecuteRollback()
        {
            return true;
        }

        #endregion
    }
}
