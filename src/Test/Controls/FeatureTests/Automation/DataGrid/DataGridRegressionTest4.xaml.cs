using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class DataGridRegressionTest4 : Window
    {
        public DataGridRegressionTest4()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new DataGridRegressionTest4TestDialog();
            dlg.ShowDialog();              
        }
    }
}
