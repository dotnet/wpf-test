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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug: SelectedValue has priority over IsSelected, but SelectedIndex doesn't
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "IsSelectedTrumpsSelectedValue")]
    public class IsSelectedTrumpsSelectedValue : XamlTest
    {
        #region Private Data

        private ComboBox _myComboBox;
        
        #endregion

        #region Constructors

        public IsSelectedTrumpsSelectedValue()
            : base(@"IsSelectedTrumpsSelectedValue.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myComboBox = (ComboBox)RootElement.FindName("myComboBox");

            if (_myComboBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Background);
            
            // Verify that index with isSelected=true (i.e. index 0) is selected.
            if (_myComboBox.SelectedIndex != 0)
            {
                LogComment("Incorrect Index Selected");
                LogComment("Expected SelectedIndex: 0");
                LogComment("Actual SelectedIndex: " + _myComboBox.SelectedIndex);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    
    #endregion
}
