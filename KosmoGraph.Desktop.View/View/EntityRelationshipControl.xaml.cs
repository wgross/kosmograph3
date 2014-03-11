using KosmoGraph.Desktop.ViewModel;
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

namespace KosmoGraph.Desktop.View
{
    /// <summary>
    /// Interaction logic for EntityRelationshipControl.xaml
    /// </summary>
    public partial class EntityRelationshipControl : UserControl
    {
        public EntityRelationshipControl()
        {
            this.InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.scrollViewer.InvalidateMeasure();
        }       
    }
}
