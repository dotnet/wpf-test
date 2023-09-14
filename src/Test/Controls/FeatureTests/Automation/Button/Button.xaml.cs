using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class ButtonTest : Page
    {
        public ButtonTest()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            result.Content = "Pass";
        }
    }
}
