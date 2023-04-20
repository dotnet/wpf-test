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
    ///  Regression coverage for bug where NotifyOnTargetUpdated does not work if set on a binding inside a priority binding
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "NotifyOnTargetUpdated")]
    public class NotifyOnTargetUpdated : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private TextBox _myTextBox;
        private TextBox _myTB1;
        private TextBox _myTB2;
        private bool _eventRaised;
        
        #endregion

        #region Constructors

        public NotifyOnTargetUpdated()
            : base(@"NotifyOnTargetUpdated.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myTB1 = (TextBox)RootElement.FindName("myTB1");
            _myTB2 = (TextBox)RootElement.FindName("myTB2");
            

            if (_myStackPanel == null || _myTB1 == null || _myTB2 == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            // Create a textbox, attach targetupdated event handler.
            _myTextBox = new TextBox();
            _myTextBox.TargetUpdated += new EventHandler<DataTransferEventArgs>(myTextBox_TargetUpdated);
            _myStackPanel.Children.Add(_myTextBox);

            _eventRaised = false;

            return TestResult.Pass;
        }

        void myTextBox_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            _eventRaised = true;
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

            _myTextBox.SetBinding(TextBox.TextProperty, priorityBind);

            // Wait for DataBindings to complete.
            WaitForPriority(DispatcherPriority.DataBind);
         
            // Verify binding activated
            if (_myTextBox.Text != _myTB1.Text)
            {
                LogComment("PriorityBinding Failed");
                return TestResult.Fail;
            }

            // Verify TargetUpdated notification raised.
            if (!_eventRaised)
            {
                LogComment("Target Updated Event was not raised.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }       

        #endregion
        
    }
}
