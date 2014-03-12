namespace KosmoGraph.Desktop.Dialog
{
    using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

    public static class PanelDialogExtensions
    {
        public static bool? ShowDialog(this Panel grid, object viewModel, params DialogAction[] dialogActions)
        {
            return grid.ShowDialog(new[] { viewModel }, dialogActions);
        }

        public static bool? ShowDialog(this Panel grid, object[] viewModels, params DialogAction[] dialogActions)
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
    }
}
