//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Generic;

namespace WpfControlToolkit
{
    public class DateTimeOffsetConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the month and year to a proper string.
        /// </summary>
        /// <param name="value">DateTime</param>
        /// <param name="targetType">string</param>
        /// <param name="parameter">null</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns>title string</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value != null) && (value is DateTime))
            {
                DateTime startDate = (DateTime)value;
                int offset = 0;
                if (parameter != null)
                {
                    if (parameter is string)
                    {
                        Int32.TryParse((string)parameter, out offset);
                    }
                    else if (parameter is int)
                    {
                        offset = (int)parameter;
                    }
                }

                return startDate.AddDays((double)offset);
            }

            return DateTime.Today;
        }

        /// <summary>
        ///     Not Supported
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
