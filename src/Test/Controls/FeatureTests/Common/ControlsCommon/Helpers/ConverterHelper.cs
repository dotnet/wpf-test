using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using System.Globalization;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls.Helpers
{
    [ValueConversion(typeof(Visibility), typeof(bool))]
    public class VisibilityToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GlobalLog.LogStatus(string.Format("VisibilityToBoolConverter called. value: {0}, targetType: {1}", value, targetType));

            Visibility visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
