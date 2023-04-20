// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using Microsoft.Test;
using Microsoft.Test.Data;
using System.Globalization;
using System.Security;
using System.Reflection;

namespace Microsoft.Test.DataServices
{
	// Boolean to Visible converter.
	// True = Hidden
	// False = Visible
	public class BooleanToVisibleConverter : IValueConverter
	{
		public BooleanToVisibleConverter()
		{
		}

		public object Convert(object o, Type type, object parameter, CultureInfo culture)
		{
			if ((bool)o)
			{
				return Visibility.Hidden;
			}
			else
			{
				return Visibility.Visible;
			}
		}

		public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
