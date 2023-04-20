// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; using System.Windows.Threading;
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
    public class FirstValueConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("ConverBack should not be called");
        }
    }}
