// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using System.Globalization;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests databinding's methods of FrameworkContentElement. No real user scenarios,
    /// done to exercise code.
    /// </description>
    /// </summary>
    [Test(3, "Binding", "TestFrameworkContentElement")]
    public class TestFrameworkContentElement : WindowTest
    {
        private Underline _underline;
        private int _dataContextCount;
        private int _dataTransferCount;

        public TestFrameworkContentElement()
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestDataContext);
            RunSteps += new TestStep(TestDataTransfer);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);
            _underline = new Underline();
            _dataContextCount = 0;
            _dataTransferCount = 0;
            Paragraph paragraph = new Paragraph(new Run("some text "));
            paragraph.Inlines.Add(new Underline(new Run("underlined text")));
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.Document = new FlowDocument(paragraph);
            Window.Content =  fdsv;
            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        #region TestDataContext
        private TestResult TestDataContext()
        {
            Status("TestDataContext");

            _underline.DataContextChanged += new DependencyPropertyChangedEventHandler(btn_DataContextChanged);
            _underline.DataContext = new MyDataClass("Red", "Blue");

            TestResult res1 = WaitForSignal("DataContextChanged");
            if (res1 != TestResult.Pass)
            {
                LogComment("Fail - Timed out waiting for DataContextChanged signal");
                return TestResult.Fail;
            }

            _underline.DataContextChanged -= new DependencyPropertyChangedEventHandler(btn_DataContextChanged);
            _underline.DataContext = new MyDataClass("Blue", "Red");

            WaitFor(1000);
            if (_dataContextCount != 1)
            {
                LogComment("Fail - DataContextChanged event handler was not properly removed");
                return TestResult.Fail;
            }

            LogComment("TestDataContext was successful");
            return TestResult.Pass;
        }

        void btn_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Status("btn_DataContextChanged");
            _dataContextCount++;
            Signal("DataContextChanged", TestResult.Pass);
        }
        #endregion

        #region TestDataTransfer
        private TestResult TestDataTransfer()
        {
            Status("TestDataTransfer");

            _underline.TargetUpdated += new EventHandler<DataTransferEventArgs>(btn_DataTransfer);

            Binding bind = new Binding("MyData1");
            bind.NotifyOnTargetUpdated = true;
            BindingExpressionBase binding = _underline.SetBinding(Underline.BackgroundProperty, bind);

            TestResult res1 = WaitForSignal("DataTransfer");
            if (res1 != TestResult.Pass)
            {
                LogComment("Fail - Timed out waiting for DataTransfer signal");
                return TestResult.Fail;
            }

            _underline.TargetUpdated -= new EventHandler<DataTransferEventArgs>(btn_DataTransfer);
            binding.UpdateTarget();

            WaitFor(1000);
            if (_dataTransferCount != 1)
            {
                LogComment("Fail - DataTransfer event handler was not properly removed");
                return TestResult.Fail;
            }

            LogComment("TestDataTransfer was successful");
            return TestResult.Pass;
        }

        void btn_DataTransfer(object sender, DataTransferEventArgs e)
        {
            Status("btn_DataTransfer");
            _dataTransferCount++;
            Signal("DataTransfer", TestResult.Pass);
        }
        #endregion
    }
}

