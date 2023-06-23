using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Provides access to parts defined in datepicker control template
    /// </summary>
    public class TemplatedDatePicker
    {
        private const string PartRoot = "PART_Root";
        private const string PartTextBox = "PART_TextBox";
        private const string PartButton = "PART_Button";
        private const string PartPopup = "PART_Popup";
        private const string PartWatermark = "PART_Watermark";

        private readonly DatePicker datepicker = null;

        public TemplatedDatePicker(DatePicker datepicker)
        {
            this.datepicker = datepicker;
        }

        public static T FindPartByName<T>(FrameworkElement fe, string partName)
        {
            object part = VisualTreeUtils.FindPartByName(fe, partName);
            if (part == null)
            {
                throw new TestFailedException(string.Format("Template part '{0}' not found", partName));
            }

            if (!(part is T))
            {
                throw new TestFailedException(string.Format("Template part '{0}' is not of type {1}", partName, typeof(T)));
            }

            return (T)part;
        }

        public Grid Root
        {
            get
            {
                return FindPartByName<Grid>(this.datepicker, PartRoot);
            }
        }

        public DatePickerTextBox TextBox
        {
            get
            {
                return FindPartByName<DatePickerTextBox>(this.Root, PartTextBox);
            }
        }

        public Button Button
        {
            get
            {
                return FindPartByName<Button>(this.Root, PartButton);
            }
        }

        public Popup Popup
        {
            get
            {
                return FindPartByName<Popup>(this.Root, PartPopup);
            }
        }

        public Calendar Calendar
        {
            get
            {
                return Popup.Child as Calendar;
            }
        }

        public ContentControl Watermark
        {
            get
            {
                return FindPartByName<ContentControl>(this.TextBox, PartWatermark);
            }
        }
    }
}
