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
    ///  Regression coverage for bug where TwoWay bindings inside priority binding are buggy
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "TwoWayPriorityBindings")]
    public class TwoWayPriorityBindings : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private TextBox _myTB2;
        private TextBox _myTB3;
        
        #endregion

        #region Constructors

        public TwoWayPriorityBindings()
            : base(@"TwoWayPriorityBindings.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myTB2 = (TextBox)RootElement.FindName("myTB2");
            _myTB3 = (TextBox)RootElement.FindName("myTB3");

            if (_myStackPanel == null || _myTB2 == null || _myTB3 == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }        
                
        private TestResult Validate()
        {            
            // Set the TextProperty on myTB3 to activate the TwoWay binding
            _myTB3.Focus();            
            _myTB3.Text = "Random String";
            
            WaitForPriority(DispatcherPriority.DataBind);
            
            // Need to focus on myTB2 to activate twoway binding.
            _myTB2.Focus();
            _myStackPanel.Focus();

            // Verify 
            if (_myTB2.Text != "Random String")
            {
                LogComment("myTB2 was not updated correctly");
                LogComment("Actual: " + _myTB2.Text);
                LogComment("Expected: Random String");
                return TestResult.Fail;
            }

            if (_myTB3.Text != "Random String")
            {
                LogComment("myTB3 was not updated correctly");
                LogComment("Actual: " + _myTB3.Text);
                LogComment("Expected: Random String");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }       

        #endregion
        
    }
}
