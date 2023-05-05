// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Data;
using System;
using System.Globalization;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where WPF Binding resolves to wrong object - causes VS crashing in WF V2 design experience
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "MultiBindingResolution")]
    public class MultiBindingResolution : XamlTest
    {
        #region Private Data

        private ListBox _myListBox;

        #endregion

        #region Constructors

        public MultiBindingResolution()
            : base(@"MultiBindingResolution.xaml")
        {
            InitializeSteps += new TestStep(Validate);                        
        }

        #endregion

        #region Private Members        
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);

            _myListBox = (ListBox)RootElement.FindName("myListBox");

            if (_myListBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }
            
            // Remove first item if count is greater than 0.
            // If this bug regresses, this block will throw an exception (causing the test to fail)
            if (_myListBox.Items.Count > 0)
            {
                _myListBox.Items.RemoveAt(0);
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    
    public class MultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    #endregion
}