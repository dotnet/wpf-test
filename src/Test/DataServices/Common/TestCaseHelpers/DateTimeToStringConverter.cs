// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.ComponentModel;
using System.Text;

namespace Microsoft.Test.DataServices
{
	public class DateTimeToStringConverter : IValueConverter
	{

        #region IValueConverter Members

        /// <summary>
        /// Converts from a DateTime to a string value
        /// </summary>
        /// <param name="value">DateTime value to convert</param>
        /// <param name="targetType">expected to be typeof string</param>
        /// <param name="parameter">offset by this amount</param>
        /// <param name="culture">culture to format the output to</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dValue = (DateTime)value;
            string output = "";
            string append = "";
            if (parameter != null)
            {
                if (parameter is TimeSpan)
                {
                    output = "** ";
                    append = " **";
                    TimeSpan ts = (TimeSpan)parameter;
                    dValue += ts;
                }
                if (parameter is long)
                {
                    output = "~~ ";
                    append = " ~~";
                    long ticks = (long)parameter;
                    TimeSpan ts = new TimeSpan(ticks);
                    dValue += ts;
                }
            }
            else
            {
                output = "!! ";
                append = " !!";
            }


            if (culture != null)
            {
                output += dValue.ToString(culture.DateTimeFormat);
                output += append;
            }
            else
            {
                output += dValue.ToString();
                output += append;
            }
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string output = (string)value;
            if (parameter != null)
            {
                if (parameter is TimeSpan)
                {
                    output = output.Replace("**", "");
                }
                if (parameter is long)
                {
                    output = output.Replace("~~", "");
                }
            }
            else
            {
                output = output.Replace("!!", "");
            }

            output.Trim();

            DateTime dt;
            if (culture != null)
            {
                dt = DateTime.Parse(output, culture.DateTimeFormat);
            }
            else
            {
                dt = DateTime.Parse(output);
            }
            return dt;

        }

        #endregion
    }
}
