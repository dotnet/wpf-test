// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Media;
using System.Text;
using System.Globalization;

namespace Microsoft.Test.DataServices
{
    public class PeopleNameConverter : IMultiValueConverter
	{
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string name;

            switch ((string)parameter)
            {
                case "FormatLastFirst":
                    name = values[1] + ", " + values[0];
                    break;
                case "FormatNormal":
                default:
                    name = values[0] + " " + values[1];
                    break;
            }

            return name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string[] splitValues = ((string)value).Split(' ');
            return splitValues;
        }

        #endregion
    }
}
