namespace KosmoGraph.Desktop.Dialog
{
    using KosmoGraph.Desktop.Dialog.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

    public sealed class KosmoGraphDialogService
    {
        public bool? ShowDialog(Panel grid, object viewModel, params DialogAction[] dialogActions)
        {
            return grid.ShowDialog(new[] { viewModel }, dialogActions);
        }

        public bool? ShowDialog(Panel grid, object[] viewModels, params DialogAction[] dialogActions)
        {
            var dlg = new DialogContainerControl
            {
                ViewModel = new DialogContainerViewModel
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

        public bool? GetModelFileNameToOpen(out string fileName)
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
            if (result != null && result.Value)
                fileName = dlg.FileName;

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
                OverwritePrompt = true,
            };

            if (string.IsNullOrWhiteSpace(Path.GetExtension(fileName)))
                fileName = fileName + ".kgml";

            var result = dlg.ShowDialog();
            if (result != null && result.Value)
                fileName = dlg.FileName;

            return result;
        }

        public bool? SelectModelDatabase(Panel atPanel, string defaultDatabaseName, out string selectedDatabaseName, params DialogAction[] dialogActions)
        {
            var selectModelDatabaseViewModel = new SelectModelDatabaseViewModel
            {
                Title = "Connect to database",
                DatabaseName = "kosmograph"
            };

            selectedDatabaseName = null;

            var result = atPanel.ShowDialog(selectModelDatabaseViewModel,
                DialogActionBuilder.Cancel("cancel", delegate { /*just do nothing*/}),
                DialogActionBuilder.Ok("OPEN", delegate { } ));

            if (result.HasValue && result.Value)
                selectedDatabaseName = selectModelDatabaseViewModel.DatabaseName;

            return result;
        }
    }
}
