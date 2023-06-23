using System;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Interaction logic for Calendar.xaml
    /// </summary>
    public partial class CalendarTest : Page
    {
        public CalendarTest()
        {
            InitializeComponent();
            calendar.SelectedDates.AddRange(startDate, endDate);
        }

        private DateTime startDate = new DateTime(2003, 2, 10);
        private DateTime endDate = new DateTime(2003, 3, 30);
    }
}
