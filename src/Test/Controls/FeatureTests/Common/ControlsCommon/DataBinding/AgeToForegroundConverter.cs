using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// AgeToForegroundConverter
    /// </summary>
    public class AgeToForegroundConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush)) return null;
            int age = int.Parse(value.ToString());
            return (age > 25 ? Brushes.Red : Brushes.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
