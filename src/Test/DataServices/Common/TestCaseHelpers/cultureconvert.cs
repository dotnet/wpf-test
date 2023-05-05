// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{
  public class CultureConverter : IValueConverter
 {
     #region IValueConverter Members

     public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
     {

         return (string)value + ": " + culture.DisplayName;
     }

     public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
     {
         throw new Exception("The method or operation is not implemented.");
     }

     #endregion
 }

}
