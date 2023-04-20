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
    ///  Test scenario to cover async binding to DLR objects
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Binding", "DLRAsyncBinding", SecurityLevel = TestCaseSecurityLevel.PartialTrust, Versions="4.7.1+")]
    public class DLRAsyncBinding : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private TextBox _asyncBindingTextBox;

        #endregion

        #region Constructors

        public DLRAsyncBinding()
            : base(@"DLRAsyncBinding.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(AsyncBinding);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");
            _asyncBindingTextBox = (TextBox)RootElement.FindName("asyncBindingTextBox");

            return TestResult.Pass;
        }

        // Verify if an async binding works with DLR objects.
        private TestResult AsyncBinding()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Defining property on dynamic object before setting as data context
            dynamic simpleBindingDo = new MyDynamicObject();
            simpleBindingDo.TempProperty = "SimpleBinding Value";
            _myStackPanel.DataContext = simpleBindingDo;

            // create an async binding
            Binding b = new Binding("TempProperty");
            b.NotifyOnTargetUpdated = true;
            b.IsAsync = true;
            b.FallbackValue = "fallback";

            // listen for TargetUpdated, then apply the binding
            _asyncBindingTextBox.TargetUpdated += new EventHandler<DataTransferEventArgs>(my_datatransfer);
            _asyncBindingTextBox.SetBinding(TextBlock.TextProperty, b);

            // wait for the TargetUpdated
            TestResult result = WaitForSignal(10000);
            if (result != TestResult.Pass)
            {
                LogComment("Signal timed out");
                return TestResult.Fail;
            }

            // Verify Binding.
            if (_asyncBindingTextBox.Text != "SimpleBinding Value" || simpleBindingDo.TempProperty != "SimpleBinding Value")
            {
                LogComment("Async Binding Test case failed. Values were not updated correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion

        private void my_datatransfer(object sender, DataTransferEventArgs args)
        {
            LogComment("Datatransfer");
            Signal(TestResult.Pass);
        }

    }
}

