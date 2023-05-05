// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test - NullReferenceException from BindingExpression.OnErrorsChanged
    /// </description>
    /// </summary>
    [Test(0, "Binding", "RegressionBindingExpressionNullRef")]
    public class RegressionBindingExpressionNullRef : XamlTest
    {
        private Page _page;

        public RegressionBindingExpressionNullRef()
            : base(@"RegressionBindingExpressionNullRef.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(NullReferenceExceptionFromBindingExpressionOnErrorsChanged);
        }

        TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.SystemIdle);
            _page = this.Window.Content as Page;
            if (_page == null)
            {
                LogComment("Fail - Page is null");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult NullReferenceExceptionFromBindingExpressionOnErrorsChanged()
        {
            Status("NullReferenceExceptionFromBindingExpressionOnErrorsChanged");
            TextBlock tb = new TextBlock();
            MyDataItem item = new MyDataItem() { Text = "hello" };
            tb.DataContext = item;

            // create a binding
            tb.SetBinding(TextBlock.TextProperty, new Binding(String.Empty));

            // replace a binding
            tb.SetBinding(TextBlock.TextProperty, new Binding(String.Empty));
            TestResult result = TestResult.Fail;

            try
            {
                // raise DataChanged event - this crashes with NRE in the original binding
                item.OnErrorsChanged(null);
                result = TestResult.Pass;
            }
            catch (NullReferenceException)
            {
                LogComment("This case failed due to regression issue (NullReferenceException from BindingExpression.OnErrorsChanged) happening again.");
            }
            catch (Exception e)
            {
                LogComment(string.Format("An unexpected Exception has occured. \r\n{0}", e.ToString()));
            }
            return result;
        }

        private class MyDataItem : INotifyDataErrorInfo
        {
            public string Text { get; set; }

            public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

            public void OnErrorsChanged(string name)
            {
                if (ErrorsChanged != null)
                    ErrorsChanged(this, new DataErrorsChangedEventArgs(name));
            }

            public System.Collections.IEnumerable GetErrors(string propertyName)
            {
                return null;
            }

            public bool HasErrors
            {
                get { return false; }
            }
        }
    }
}
