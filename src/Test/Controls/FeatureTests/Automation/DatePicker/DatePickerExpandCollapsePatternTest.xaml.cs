using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class DatePickerExpandCollapsePatternTest : Page
    {
        public DatePickerExpandCollapsePatternTest()
        {
            InitializeComponent();
        }

        private void datePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            result.Text = "Opened";
        }

        private void datePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            result.Text = "Closed";
        }
    }
}
