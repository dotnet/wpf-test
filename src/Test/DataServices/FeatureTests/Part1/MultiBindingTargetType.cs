// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Threading;
using System.Windows.Data;
using System.Globalization;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression coverage for bug where Binding with Converter Inside MultiBinding Gets Incorrect Target Type.
    /// </description>

    /// </summary>
    [Test(1, "Regressions.Part1", "MultiBindingTargetType")]
    public class MultiBindingTargetType : XamlTest
    {
        #region Constructors

        public MultiBindingTargetType() :
            base(@"MultiBindingTargetType.xaml")
        {
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.DataBind);         
            return TestResult.Pass;
        }
        #endregion

    }

    #region Helper Classes

    public class Wrapper
    {
        public readonly object Wrapped;

        public Wrapper(object wrapped)
        {
            this.Wrapped = wrapped;
        }
    }

    public class WrapperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Wrapper(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class UnwrapperConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {            
            // Check if the value recieved is of type wrapper or not
            if (values[0] is Wrapper == false)
            {
                throw new TestValidationException("value received is not of type Wrapper"); 
            }

            return ((Wrapper)values[0]).Wrapped;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    #endregion

}