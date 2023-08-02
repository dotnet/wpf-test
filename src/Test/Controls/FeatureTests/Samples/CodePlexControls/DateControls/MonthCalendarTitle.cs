using System.Windows;
using System.Windows.Controls;

namespace WpfControlToolkit
{
    public class MonthCalendarTitle : Control
    {
        /// <summary>
        /// Static Constructor
        /// </summary>
        static MonthCalendarTitle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendarTitle), new FrameworkPropertyMetadata(typeof(MonthCalendarTitle)));
        }
    }
}
