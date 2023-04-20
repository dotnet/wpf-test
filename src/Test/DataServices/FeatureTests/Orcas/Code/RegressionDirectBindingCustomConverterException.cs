// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Data;
using System.Windows.Data;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test - Direct binding with user converter yields NullReferenceException
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>            
    [Test(1, "Binding", "RegressionDirectBindingCustomConverterException")]
    public class RegressionDirectBindingCustomConverterException : XamlTest
    {
        #region Public Members

        public RegressionDirectBindingCustomConverterException()
            : base(@"Markup\RegressionDirectBindingCustomConverterException.xaml")
        {
            RunSteps += new TestStep(Verify);
        }

        public TestResult Verify()
        {
            TextBlock textBlock = (TextBlock)RootElement.FindName("boundTextBlock");

            if (textBlock.Text == "0")
                return TestResult.Pass;
            else
                return TestResult.Fail;
        }

        #endregion
    }

    public class Conv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
