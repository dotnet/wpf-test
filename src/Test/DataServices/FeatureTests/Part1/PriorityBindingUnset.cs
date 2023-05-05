// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Windows.Controls;
using Microsoft.Test;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using System.Windows.Threading;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where When a priority binding fails, it sets the target property to 'UnsetValue'
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "PriorityBindingUnset")]
    public class PriorityBindingUnset : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private TextBox _myTextBox;
        private int _eventRaised;
        
        #endregion

        #region Constructors

        public PriorityBindingUnset()
            : base(@"PriorityBindingUnset.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);            
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");

            if (_myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            // Create a textbox, attach targetupdated event handler.
            _myTextBox = new TextBox();
            _myTextBox.TargetUpdated += new EventHandler<DataTransferEventArgs>(myTextBox_TargetUpdated);
            _myStackPanel.Children.Add(_myTextBox);

            _eventRaised = 0;

            return TestResult.Pass;
        }

        void myTextBox_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            _eventRaised++;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // Set the bindings.            
            PriorityBinding priorityBind = new PriorityBinding();            

            Binding bindOne = new Binding("Text");
            bindOne.ElementName = "myTB1";
            bindOne.NotifyOnTargetUpdated = true;

            Binding bindTwo = new Binding("Text");
            bindTwo.ElementName = "myTB2";
            bindTwo.NotifyOnTargetUpdated = true;

            priorityBind.Bindings.Add(bindOne);
            priorityBind.Bindings.Add(bindTwo);

            _myTextBox.SetBinding(TextBox.NameProperty, priorityBind);

            // Wait for DataBindings to complete.
            WaitForPriority(DispatcherPriority.DataBind);
                                    
            // Verify binding activated
            if (_eventRaised != 1)
            {
                LogComment("Target Updated called an unexpected number of times.");
                LogComment("Expected: 1");
                LogComment("Actual: " + _eventRaised.ToString());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }       

        #endregion
        
    }
}