// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{
	public class NameConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
            if (values[0] == System.Windows.DependencyProperty.UnsetValue || values[1] == System.Windows.DependencyProperty.UnsetValue || values[2] == System.Windows.DependencyProperty.UnsetValue)
                return "";
            string name = (string)values[0] + " " + (string)values[1] + " " + values[2].ToString();
			return name;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			object[] res = new object[3];
			string[] splitValues = ((string)value).Split(' ');
			res[0] = splitValues[0];
			res[1] = splitValues[1];
			res[2] = Int32.Parse(splitValues[2]);
			return res;
		}
	}
}
