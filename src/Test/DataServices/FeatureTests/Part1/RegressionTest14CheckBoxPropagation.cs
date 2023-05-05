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
using System.Collections.Generic;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage  - WPF CheckBox bound to single item in a collection not updating the value in .NET 4 (but works in .NET 3.5)
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "RegressionTest14CheckBoxPropagation", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class RegressionTest14CheckBoxPropagation : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private CheckBox _myCheckBox;

        private List<bool?> _myCollection = new List<bool?> { true, false, true, false };

        public List<bool?> MyCollection
        {
            get { return _myCollection; }
            set { _myCollection = value; }
        }
        #endregion

        #region Constructors

        public RegressionTest14CheckBoxPropagation() : base(@"RegressionTest14CheckBoxPropagation.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");
            _myCheckBox = (CheckBox)RootElement.FindName("myCheckBox");
            if (_myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }
            _myStackPanel.DataContext = this;

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            LogComment("Beginning validation: Ensuring that checkbox IsChecked propagates to boolean array (Regression Prevention:)");
            
            WaitForPriority(DispatcherPriority.Background);

            if ((bool)_myCheckBox.IsChecked)
            {
                LogComment("Check Box was initially checked (expected)");
            }
            else
            {
                LogComment("Check Box was not initially checked (binding failure?)");
                return TestResult.Fail;
            }
            
            _myCheckBox.IsChecked = false;
            WaitForPriority(DispatcherPriority.Background);

            if (MyCollection[0] == false)
            {
                LogComment("Unchecking check box propagated to boolean array (expected)");
            }
            else
            {
                LogComment("Unchecking check box may not have propagated to boolean array (error)");
                return TestResult.Fail;
            }

            _myCheckBox.IsChecked = true;
            WaitForPriority(DispatcherPriority.Background);

            if (MyCollection[0] == true)
            {
                LogComment("Success, two way binding between a boolean array and checkbox succeeds (Regression prevention ).");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Failure, issue encountered with two way binding between a boolean array and checkbox (Regression prevention ).");
                return TestResult.Fail;
            }
        }
        #endregion
    }    
}

