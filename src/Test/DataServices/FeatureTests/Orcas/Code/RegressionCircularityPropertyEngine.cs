// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test : We put a converter on the binding and catch the excpetion inside the converter.   
    /// </description>
    /// </summary>            
    [Test(1, "Binding", "RegressionCircularityPropertyEngine")]
    public class RegressionCircularityPropertyEngine : XamlTest
    {
        #region Private Members

        private ValueConverter _converter;

        #endregion

        #region Public Members

        public RegressionCircularityPropertyEngine()
            : base(@"Markup\RegressionCircularityPropertyEngine.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Verify);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);

            _converter = RootElement.FindResource("ValueConverter") as ValueConverter;
            if (_converter == null)
            {
                LogComment("Unable to reference the converter.");
                return TestResult.Fail;
            }

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        // Check the value of the converter to see if it ever grabs the wrong DataContext.
        public TestResult Verify()
        {
            Status("RuningTest");

            if (_converter.Failed == true)
            {
                LogComment("Unexpected Exception was recieved: " + _converter.Message);
                return TestResult.Fail;
            }

            LogComment("No unexpected exceptions received.");            
            return TestResult.Pass;
        }

        #endregion
    }

    #region ValueConverter

    // This converter is used to check for instantaneous value change in the content due to datacontext switches
    public class ValueConverter : IValueConverter
    {
        #region Private Members

        //All private helper stuff and private subtypes
        private bool _failed = false;
        private string _message = "";

        #endregion

        #region Public Members

        public bool Failed
        {
            get { return _failed; }
            set { _failed = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String stringValue = String.Empty;

            // Looking for and catching the buggy exception:
            try
            {
                stringValue = (String)value;
            }
            catch (InvalidCastException e)
            {
                Message = e.Message;
                Failed = true;
            }

            return culture.TextInfo.ToTitleCase(stringValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not required;
            return null;
        }

        #endregion
    }

    #endregion
}
