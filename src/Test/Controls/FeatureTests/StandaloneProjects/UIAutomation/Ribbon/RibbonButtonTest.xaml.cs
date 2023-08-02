using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    public partial class RibbonButtonTest : Window
    {
        public RibbonButtonTest()
        {
            InitializeComponent();
        }

        private void ribbonButton0_Click(object sender, RoutedEventArgs e)
        {
            result.Text = "Pass";
        }
    }
}
