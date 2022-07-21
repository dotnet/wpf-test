// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DrtDataSrv
{
    public partial class DataTriggerPage : Page
    {
        private void OnFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = MyFilter(e.Item);
        }

        private bool MyFilter(object item)
        {
            Place place = item as Place;

            return place != null && place.State == "WA";
        }
    }

    // A converter that does nothing
    [ValueConversion(typeof(string), typeof(string))]
    public class IdentityConverter : IValueConverter
    {
        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return o;
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return o;
        }

    }
}
