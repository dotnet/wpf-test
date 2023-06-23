using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class DataGridItemContainerPatternTest : Page
    {
        public DataGridItemContainerPatternTest()
        {
            InitializeComponent();
        }

        // Setup AutomationId for each cell
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UIADataGridHelper.SetupAutomationIdForEachCell(datagrid, ready);
        }
    }
}
