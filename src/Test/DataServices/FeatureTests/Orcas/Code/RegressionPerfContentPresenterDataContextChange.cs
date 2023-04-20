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
    /// Regression Test : We put a converter on the binding of a contentpresenter to verify that it always grabs the inherited DataContext.   
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>            
    [Test(1, "Binding", "RegressionPerfContentPresenterDataContextChange")]
    public class RegressionPerfContentPresenterDataContextChange : XamlTest
    {
        #region Private Members
        private DataContextChangeConverter _converter;
        #endregion       

        #region Public Members
        public RegressionPerfContentPresenterDataContextChange()
            : base(@"Markup\RegressionPerfContentPresenterDataContextChange.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Verify);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);

            ContentPresenter contentPresenter;

            contentPresenter = Util.FindElement(RootElement, "contentPresenter") as ContentPresenter;
            if (contentPresenter == null)
            {
                LogComment("Unable to reference the ContentPresenter.");
                return TestResult.Fail;
            }

            _converter = contentPresenter.FindResource("DataContextChangeConverter") as DataContextChangeConverter;
            if (_converter == null)
            {
                LogComment("Unable to reference the ContentPresenter.");
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
                LogComment("DataContext is not set to inherited datacontext on first try.");
                return TestResult.Fail;
            }

            LogComment("DataContext is set correctly throughout.");
            return TestResult.Pass;
        }
        #endregion
    }

    #region MyObj
    // A class that creates an object hat has a property "Bavkground" on it.
    public class MyObj
    {
        #region Public Members
        public string Background
        {
            get { return "Text"; }
        }
        #endregion
    }
    #endregion

    #region MyConverter
    // This converter is used to check for instantaneous value change in the content due to datacontext switches
    public class DataContextChangeConverter : IValueConverter
    {
        #region Private Members
        //All private helper stuff and private subtypes
        private bool _failed = false;
        #endregion

        #region Public Members
        public bool Failed
        {
            get { return _failed; }
            set { _failed = value; }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && !value.Equals("Text") )
            {                
                this.Failed = true;
                return false;
            }

            return true;
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
