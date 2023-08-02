using System.Windows;
using System.Windows.Controls;

namespace WpfControlToolkit
{
    public class MonthCalendarWeekNumber : Control
    {
        /// <summary>
        /// Static Constructor
        /// </summary>
        static MonthCalendarWeekNumber()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendarWeekNumber), new FrameworkPropertyMetadata(typeof(MonthCalendarWeekNumber)));
        }
    }
}
