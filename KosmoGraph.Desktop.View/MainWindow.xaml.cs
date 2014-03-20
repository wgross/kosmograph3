namespace KosmoGraph.Desktop.View
{
    using KosmoGraph.Desktop.ViewModel;
    using KosmoGraph.Desktop.Dialog.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using KosmoGraph.Services;
    using KosmoGraph.Persistence.MongoDb;
    using KosmoGraph.Desktop.Dialog;
    
    [Export]
    public partial class MainWindow : Window
    {
        #region Construction and initialization of this instance

        public MainWindow()
        {
            this.InitializeComponent();
            this.Activated += MainWindow_Activated;
        }

        void MainWindow_Activated(object sender, EventArgs e)
        {
            if (this.Model == null)
            {
                this.CreateNewModelFromStore();
                if (this.Model == null)
                    Application.Current.Shutdown();
            }
        }

        public EntityRelationshipViewModel Model
        {
            get
            {
                return this.DataContext as EntityRelationshipViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        #endregion

        #region Handle CreateEntity command

        private void CreateEntityCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            if (this.Model == null)
                args.CanExecute = false;
            else
                args.CanExecute = true;
        }

        private void CreateEntityExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            var dialogViewModel = this.Model.CreateNewEntity();

            new KosmoGraphDialogService().ShowDialog(this.rootPanel, dialogViewModel,
                DialogActionBuilder.Cancel("cancel", dialogViewModel.Rollback),
                DialogActionBuilder.Ok("create entity", dialogViewModel.Commit));
        }

        #endregion

        #region Handle CreateRelationship command

        private void CreateRelationshipBetweenEntitiesCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            var relationshipDialogViewModel = args.Parameter as EditNewRelationshipViewModel;
            if (relationshipDialogViewModel == null)
            {
                args.CanExecute = false;
                return;
            }

            if (relationshipDialogViewModel.From == null || relationshipDialogViewModel.To == null)
            {
                args.CanExecute = false;
                return;
            }

            args.CanExecute = true;
        }

        private void CreateRelationshipBetweenEntitiesExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            var relationshipDialogViewModel = args.Parameter as EditNewRelationshipViewModel;

            this.rootPanel.ShowDialog(relationshipDialogViewModel,
               DialogActionBuilder.Ok("ok", relationshipDialogViewModel.Commit),
               DialogActionBuilder.Cancel("cancel", relationshipDialogViewModel.Rollback));
        }

        #endregion

        #region Handle DeleteRelationship command

        private void DeleteRelationshipCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void DeleteRelationshipExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            if (args.Parameter is ObservableCollection<object>) // called from within dialog
            {
                var first = ((ObservableCollection<object>)args.Parameter).FirstOrDefault() as EditExistingRelationshipViewModel;
                if (first != null)
                    this.DeleteRelationship(first.Edited);
            }
        }

        private void DeleteRelationship(RelationshipViewModel relationship)
        {
            this.Model.Remove(relationship);
        }

        #endregion 

        #region Handle CreateNewFacet command

        private void CreateNewFacetCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = this.Model != null;
        }

        private void CreateNewFacetExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            var dialogViewModel = this.Model.CreateNewFacet();

            new KosmoGraphDialogService().ShowDialog(this.rootPanel, dialogViewModel,
                DialogActionBuilder.Cancel("cancel", dialogViewModel.Rollback),
                DialogActionBuilder.Ok("create facet", dialogViewModel.Commit));

            //var newTag = this.Model.CreateNewTag("Tag" + (this.Model.Tags.Count()+1));

            //if (this.EditFacet(newTag).GetValueOrDefault(false))
            //    this.Model.Add(newTag);
        }

        #endregion

        #region Handle CreateRelatonshipWithEntity command
        
        private void CreateRelationshipAndEntityCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void CreateRelationshipEndEntityExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            var relationshipDialogViewModel = this.Model.EditRelationship(args.Parameter as RelationshipViewModel);
            relationshipDialogViewModel.EnableCommit = true;

            var entityDialogViewModel = this.Model.EditEntity(args.GetParameter<RelationshipViewModel>().To.Entity);
            entityDialogViewModel.EnableCommit = true;

            var committed = this.rootPanel.ShowDialog(new object[]{entityDialogViewModel, relationshipDialogViewModel},
                DialogActionBuilder.Ok("ok", entityDialogViewModel.Commit, relationshipDialogViewModel.Commit), 
                DialogActionBuilder.Cancel("cancel", entityDialogViewModel.Rollback, relationshipDialogViewModel.Rollback));
           
            //if (committed.HasValue && committed.Value)
            //{
            //    this.Model.Add(args.GetParameter<RelationshipViewModel>().To.Entity);
            //    this.Model.Add(args.GetParameter<RelationshipViewModel>());
            //}
        }

        #endregion 

        #region Handle EditEntity command

        private void EditEntityCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void EditEntityExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            this.EditEntity(args.Parameter as EntityViewModel);
        }

        private bool? EditEntity(EntityViewModel entity)
        {
            var dialogViewModel = this.Model.EditEntity(entity);

            return this.rootPanel.ShowDialog(dialogViewModel,
                DialogActionBuilder.Cancel("delete entity", false, EntityRelationshipModelCommands.DeleteEntity),
                DialogActionBuilder.Ok("ok", dialogViewModel.Commit),
                DialogActionBuilder.Cancel("cancel", dialogViewModel.Rollback));
        }
        #endregion 

        #region Handle EditRelationship command

        private void EditRelationshipCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = this.Model != null && args.Parameter is RelationshipViewModel;
        }

        private void EditRelationshipExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            var dialogViewModel = this.Model.EditRelationship(args.Parameter as RelationshipViewModel);

            this.rootPanel.ShowDialog(dialogViewModel,
                DialogActionBuilder.Cancel("delete relationship", false, EntityRelationshipModelCommands.DeleteRelationship),
                DialogActionBuilder.Ok("ok", dialogViewModel.Commit),
                DialogActionBuilder.Cancel("cancel", dialogViewModel.Rollback));
        }

        #endregion 

        #region Handle EditFacet command

        private void EditFacetCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void EditFacetExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            this.EditFacet(args.Parameter as FacetViewModel);
        }

        private bool? EditFacet(FacetViewModel facetViewModel)
        {
            var dialogViewModel = this.Model.EditFacet(facetViewModel);

            return this.rootPanel.ShowDialog(dialogViewModel,
                DialogActionBuilder.Cancel("delete facet", false, EntityRelationshipModelCommands.DeleteFacet),
                DialogActionBuilder.Ok("ok", dialogViewModel.Commit), 
                DialogActionBuilder.Cancel("cancel", dialogViewModel.Rollback));
        }

        #endregion 

        #region Handle DeleteFacet command

        private void DeleteFacetCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void DeleteFacetExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            if (args.Parameter is IEnumerable<object>) // called from within dialog
            {
                var first = ((IEnumerable<object>)args.Parameter).FirstOrDefault() as EditExistingFacetViewModel;
                if (first != null)
                    this.DeleteFacet(first.Edited);
            }
        }

        private void DeleteFacet(FacetViewModel facet)
        {
            this.Model.Remove(facet);
        }

        #endregion 

        #region Handle DeleteEntity command

        private void DeleteEntityCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void DeleteEntityExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            if (args.Parameter is ObservableCollection<object>) // called from within dialog
            {
                var first = ((ObservableCollection<object>)args.Parameter).FirstOrDefault() as EditExistingEntityViewModel;
                if (first != null)
                    this.DeleteEntity(first.Edited);
            }
        }

        private void DeleteEntity(EntityViewModel entity)
        {
            this.Model.Remove(entity);
        }

        #endregion 

        #region //Handle Create New Model command

        //private void CreateNewModelCanExecute(object sender, CanExecuteRoutedEventArgs args)
        //{
        //    args.CanExecute = true;
        //}

        //private void CreateNewModelExecuted(object sender, ExecutedRoutedEventArgs args)
        //{
        //    this.CreateNewModel(args.Parameter as EntityRelationshipViewModel);
        //}

        //private void CreateNewModel(EntityRelationshipViewModel model)
        //{
        //    //string selectedDatabase;
        //    //var result = new KosmoGraphDialogService()
        //    //    .SelectModelDatabase(this.rootPanel, "kosmograph", out selectedDatabase, null);
            
        //    //if(result.HasValue && result.Value)
        //    //    this.Model = new EntityRelationshipViewModelFactory().CreateNewFromDatabaseName(selectedDatabase);
        //}

        #endregion 

        #region //Handle Save As command

        //private void SaveModelAsCanExecute(object sender, CanExecuteRoutedEventArgs args)
        //{
        //    var model = args.Parameter as EntityRelationshipViewModel;

        //    //args.CanExecute = (model != null && model.ModelStore is XmlModelDatabase);
        //}

        //private void SaveModelAsExecuted(object sender, ExecutedRoutedEventArgs args)
        //{
        //    if (args.Parameter is EntityRelationshipViewModel)
        //    {
        //        this.SaveModelAs(args.Parameter as EntityRelationshipViewModel);
        //    }
        //}

        //private void SaveModelAs(EntityRelationshipViewModel model)
        //{
        //    //string fileName;
        //    //var result = new DialogService().GetModelFileNameToSave(((XmlModelDatabase)(model.ModelStore)).FileName, out fileName);
        //    //if(result.GetValueOrDefault())
        //    //    if(!string.IsNullOrWhiteSpace(fileName))
        //    //        ((XmlModelDatabase)(model.ModelStore)).StoreAs(fileName);
        //}

        #endregion

        #region //Handle Save command

        //private void SaveModelCanExecute(object sender, CanExecuteRoutedEventArgs args)
        //{
        //    if (args.Parameter is EntityRelationshipViewModel)
        //    {
        //        var model = args.Parameter as EntityRelationshipViewModel;

        //        args.CanExecute = false;//(
        //            //model.ModelStore != null 
        //            //&& 
        //            //model.ModelStore is XmlModelDatabase 
        //            //&&
        //            //!string.IsNullOrEmpty(((XmlModelDatabase)(model.ModelStore)).FileName)
        //        //);
        //    }
        //    else args.CanExecute = false;
        //}

        //private void SaveModelExecuted(object sender, ExecutedRoutedEventArgs args)
        //{
        //    if (args.Parameter is EntityRelationshipViewModel)
        //    {
        //        this.SaveModel(args.Parameter as EntityRelationshipViewModel);
        //    }
        //}

        //private void SaveModel(EntityRelationshipViewModel model)
        //{
        //    //model.ModelStore.Store();
        //}
        #endregion

        #region Handle Create New Model From Store command

        private void CreateNewModelFromStoreCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void CreateNewModelFromStoreExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            this.CreateNewModelFromStore();
        }

        private void CreateNewModelFromStore()
        {
            string selectedDatabase;
            var result = new KosmoGraphDialogService()
                .SelectModelDatabase(this.rootPanel, "kosmograph", out selectedDatabase, null);

            if (result.HasValue && result.Value)
                this.Model = new EntityRelationshipViewModelFactory().CreateNewFromDatabaseName(selectedDatabase);
           
            //if(model!=null)
            //    this.Model = model;
            //this.Model.StartLayout();
        }

        #endregion

        #region Handle LayoutModel command

        private void LayoutModelCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void LayoutModelExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            if (args.Parameter is EntityRelationshipViewModel)
            {
                this.LayoutModel(args.Parameter as EntityRelationshipViewModel);
            }
        }

        private void LayoutModel(EntityRelationshipViewModel model)
        {
            //model.StartLayout();
        }

        #endregion

        #region Handle listbox item double click

        private void facetListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if (this.facetListBox.SelectedItems.Count == 0)
                return;

            e.Handled = true;
            EntityRelationshipModelCommands.EditTag.Execute(this.facetListBox.SelectedItems[0] as FacetViewModel, this.facetListBox);
        }

        #endregion 
    }
}
