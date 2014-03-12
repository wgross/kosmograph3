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
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Model = EntityRelationshipViewModel.CreateNewFromStore(new EfModelDatabase());
            //this.Model.AddDummyData();
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

        private EntityRelationshipViewModel model;

        #endregion

        #region Handle CreateEntity command

        private void CreateEntityCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void CreateEntityExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            var newEntity = this.Model.CreateNewEntity(); //"entity" + (model.Entities.Count()+1));
            //if (this.EditEntity(newEntity).GetValueOrDefault(false))
            //    this.Model.Add(newEntity);
        }

        #endregion

        #region Handle CreateRelationship command

        private void CreateRelationshipCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void CreateRelationshipExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            //var relationshipDialogViewModel = new EditRelationshipViewModel(args.Parameter as RelationshipViewModel);
            //var committed = this.rootPanel.ShowDialog(relationshipDialogViewModel,
            //   DialogActionBuilder.Ok("ok", relationshipDialogViewModel.Commit),
            //   DialogActionBuilder.Cancel("cancel", relationshipDialogViewModel.Rollback));

            //if (committed.HasValue && committed.Value)
            //{
            //    var adedd = this.Model.Add(args.GetParameter<RelationshipViewModel>());
            //}
        }

        #endregion

        #region Handle DeleteRelationship command

        private void DeleteRelationshipCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void DeleteRelationshipExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            //if (args.Parameter is ObservableCollection<object>) // called from within dialog
            //{
            //    var first = ((ObservableCollection<object>)args.Parameter).FirstOrDefault() as EditRelationshipViewModel;
            //    if (first != null)
            //        this.DeleteRelationship(first.Edited);
            //}
        }

        private void DeleteRelationship(RelationshipViewModel relationship)
        {
            //relationship.Model.Remove(relationship);
        }

        #endregion 

        #region Handle CreateTag command

        private void CreateTagCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void CreateTagExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            //var newTag = this.Model.CreateNewTag("Tag" + (this.Model.Tags.Count()+1));

            //if (this.EditFacet(newTag).GetValueOrDefault(false))
            //    this.Model.Add(newTag);
        }

        #endregion

        #region Handle CreateRelatonshipWithEntity command
        
        private void CreateRelationshipWithEntityCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void CreateRelationshipWithEntityExecuted(object sender, ExecutedRoutedEventArgs args)
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
            return false;
            //var dialogViewModel = new EditEntityViewModel(entity);

            //return this.rootPanel.ShowDialog(dialogViewModel, 
            //    DialogActionBuilder.Cancel("delete entity", false, EntityRelationshipModelCommands.DeleteEntity),
            //    DialogActionBuilder.Ok("ok", dialogViewModel.Commit), 
            //    DialogActionBuilder.Cancel("cancel", dialogViewModel.Rollback));
        }
        #endregion 

        #region Handle EditRelationship command

        private void EditRelationshipCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void EditRelationshipExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            //var dialogViewModel = new EditRelationshipViewModel(args.Parameter as RelationshipViewModel);
            
            //this.rootPanel.ShowDialog(dialogViewModel, 
            //    DialogActionBuilder.Cancel("delete relationship",false, EntityRelationshipModelCommands.DeleteRelationship),
            //    DialogActionBuilder.Ok("ok", dialogViewModel.Commit), 
            //    DialogActionBuilder.Cancel("cancel", dialogViewModel.Rollback));
        }

        #endregion 

        #region Handle EditTag command

        private void EditFacetCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void EditTagExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            this.EditFacet(args.Parameter as FacetViewModel);
        }

        private bool? EditFacet(FacetViewModel facetViewModel)
        {
            var dialogViewModel = this.model.EditFacet(facetViewModel);

            return this.rootPanel.ShowDialog(dialogViewModel,
                DialogActionBuilder.Cancel("delete tag", false, EntityRelationshipModelCommands.DeleteTag),
                DialogActionBuilder.Ok("ok", dialogViewModel.Commit), 
                DialogActionBuilder.Cancel("cancel", dialogViewModel.Rollback));
        }

        #endregion 

        #region Handle DeleteTag command

        private void DeleteTagCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void DeleteTagExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            //if (args.Parameter is ObservableCollection<object>) // called from within dialog
            //{
            //    var first = ((ObservableCollection<object>)args.Parameter).FirstOrDefault() as EditFacetViewModel;
            //    if (first != null)
            //        this.DeleteTag(first.Edited);
            //}
        }

        private void DeleteTag(FacetViewModel facet)
        {
            facet.Model.Remove(facet);
        }

        #endregion 

        #region Handle DeleteEntity command

        private void DeleteEntityCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void DeleteEntityExecuted(object sender, ExecutedRoutedEventArgs args)
        {   
            //if(args.Parameter is ObservableCollection<object>) // called from within dialog
            //{
            //    var first = ((ObservableCollection<object>)args.Parameter).FirstOrDefault() as EditEntityViewModel;
            //    if(first != null)
            //        this.DeleteEntity(first.Edited);
            //}
        }

        private void DeleteEntity(EntityViewModel entity)
        {
            entity.Model.Remove(entity);
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

        private void tagListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if (this.tagListBox.SelectedItems.Count == 0)
                return;

            e.Handled = true;
            EntityRelationshipModelCommands.EditTag.Execute(this.tagListBox.SelectedItems[0] as FacetViewModel, this.tagListBox);
        }

        #endregion 
    }
}
