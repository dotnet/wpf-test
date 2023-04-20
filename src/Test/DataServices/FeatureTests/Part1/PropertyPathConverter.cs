// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression coverage for bug: PropertyPathConverter generates unnecessary '.' in PropertyPath.Path before indexer property
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "PropertyPathConverter")]
    public class PropertyPathConverter : XamlTest
    {
        #region Private Data

        private Label _myLabel;
        private string _actualOutput;
        private string _expectedOutput;
        
        #endregion

        #region Constructors

        public PropertyPathConverter()
            : base(@"PropertyPathConverter.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myLabel = (Label)RootElement.FindName("myLabel");

            if (_myLabel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Grab Content string
            _myLabel = (Label)RootElement.FindName("myLabel");
            Border border = (Border)VisualTreeHelper.GetChild(_myLabel, 0);
            ContentPresenter cp = (ContentPresenter)VisualTreeHelper.GetChild(border, 0);
            TextBlock tb = (TextBlock)VisualTreeHelper.GetChild(cp, 0);
            _actualOutput = tb.Text;

            _expectedOutput = "(0).(GradientStops)[0]";

            // Verify 
            if (_actualOutput != _expectedOutput)
            {
                LogComment("Content does not match Expected Value");
                LogComment("Actual: " + _actualOutput);
                LogComment("Expected: " + _expectedOutput);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    
    #endregion
}