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

    public static class DialogServiceExtensions
    {
        public static bool? ShowDialog(this Grid grid, object viewModel, params DialogAction[] dialogActions)
        {
            return grid.ShowDialog(new[] { viewModel }, dialogActions);
        }

        public static bool? ShowDialog(this Grid grid, object[] viewModels, params DialogAction[] dialogActions)
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
    }
}
