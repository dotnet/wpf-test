// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Media;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public class ConcatStringValueConverter : IMultiValueConverter
	{
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new ArgumentException("ConcatStringValueConverter only supports strings");
            }
            string final = "";
            string ending = "";

            if (parameter != null && parameter is string)
            {
                final = parameter.ToString() + " ";
                ending = " " + parameter.ToString();
            }

            if (parameter != null && parameter is int)
            {
                int limit = (int)parameter;
                for (int i = 0; i < limit; i++)
                {
                    final += "*";
                }
                ending = final + " ";
                final += " ";
            }

            foreach (object o in values)
            {
                final += o.ToString();
                final += " ";
            }
            return final + ending;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class IntToBrush : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            int i = Int32.Parse(o.ToString());
            int p = 0;
            if (parameter != null)
            {
                p = Int32.Parse(parameter.ToString());
            }

            i += p;

            switch (i)
            {
                case 0:
                    return Brushes.Blue;
                case 1:
                    return Brushes.Purple;
                case 2:
                    return Brushes.Gold;
                case 3:
                    return Brushes.ForestGreen;
                case 4:
                    return Brushes.Pink;
                case 5:
                    return Brushes.SeaGreen;
                default:
                    return Brushes.Red;
            }
        }
        public object ConvertBack(object o, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0;
        }
    }

}
