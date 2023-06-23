using System.Windows;
using System.Windows.Controls;

namespace WpfControlToolkit
{
    public class MonthCalendarDayHeader : Control
    {
        /// <summary>
        /// Static Constructor
        /// </summary>
        static MonthCalendarDayHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendarDayHeader), new FrameworkPropertyMetadata(typeof(MonthCalendarDayHeader)));
        }
    }
}
