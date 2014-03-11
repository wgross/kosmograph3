namespace KosmoGraph.Desktop.Dialog
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;
    

    public class DialogService
    {
        public bool? ShowDialog( Grid grid, object viewModel, params DialogAction[] dialogActions)
        {
            return grid.ShowDialog(new[] { viewModel }, dialogActions);
        }

        public bool? ShowDialog( Grid grid, object[] viewModels, params DialogAction[] dialogActions)
        {
            var dlg = new DialogControl
            {
                ViewModel = new DialogViewModel
                {
                    DialogContent = new ObservableCollection<object>(viewModels),
                    DialogActions = dialogActions
                }
            };

            try
            {
                grid.Children.Add(dlg);

                return dlg.WaitModal();
            }
            finally
            {
                grid.Children.Remove(dlg);
            }
        }

        public bool? GetModelFileNameToOpen( out string fileName )
        {
            fileName = string.Empty;

            var dlg = new OpenFileDialog()
            {
                CheckPathExists = true,
                DefaultExt = "kgml",
                DereferenceLinks = true,
                Filter = "Kosmograph files (*.kgml)|*.kgml|All files (*.*)|*.*",
                Title = "Open model file",
            };

            var result = dlg.ShowDialog();
            if( result!= null && result.Value)
                fileName =dlg.FileName;
            
            return result;
        }

        public bool? GetModelFileNameToSave(string proposedFileName, out string fileName)
        {
            fileName = string.Empty;

            var dlg = new SaveFileDialog()
            {
                CheckPathExists = true,
                DefaultExt = "kgml",
                DereferenceLinks = true,
                FileName = proposedFileName,
                Filter = "Kosmograph files (*.kgml)|*.kgml|All files (*.*)|*.*",
                //InitialDirectory = ".",
                Title = "CopyTo model as file",
                //ValidateNames = true,
                OverwritePrompt=true,
            };

            if(string.IsNullOrWhiteSpace(Path.GetExtension(fileName)))
                fileName = fileName+".kgml";
            
            var result = dlg.ShowDialog();
            if (result != null && result.Value)
                fileName = dlg.FileName;

            return result;
        }
    }
}
