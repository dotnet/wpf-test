// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices.AsyncTest
{

    [Test(0, "Binding", "AsyncPropTest")]
    public class AsyncPropTest : XamlTest
    {
        TextBlock _slowText;
        TextBlock _fastText;
        TextBlock _codeBindText;
        TextBlock _priTestText;
        ObjectDataProvider _dso;
        SlowDataItem _item;
        string _fallback = "fallback";

        public AsyncPropTest(): base(@"AsyncPropTest.xaml")
        {
            InitializeSteps += new TestStep(SetUp);
            RunSteps += new TestStep(Verify);
            RunSteps += new TestStep(BindVerify);
        }


        TestResult SetUp()
        {
            _slowText = (TextBlock)Util.FindElement(RootElement, "Slow");
            _fastText = (TextBlock)Util.FindElement(RootElement, "Fast");
            _codeBindText = (TextBlock)Util.FindElement(RootElement, "CodeBind");
            _priTestText = (TextBlock)Util.FindElement(RootElement, "PriTest");
            _dso = RootElement.Resources["DSO"] as ObjectDataProvider;

            if (_slowText == null)
            {
                LogComment("Could not reference slowText");
                return TestResult.Fail;
            }
            if (_fastText == null)
            {
                LogComment("Could not reference fastText");
                return TestResult.Fail;
            }
            if (_codeBindText == null)
            {
                LogComment("Could not reference codeBindText");
                return TestResult.Fail;
            }
            if (_priTestText == null)
            {
                LogComment("Could not reference PriTest");
                return TestResult.Fail;
            }
            if (_dso == null)
            {
                LogComment("Unable to reference ObjectDataSource");
                return TestResult.Fail;
            }

            _item = (SlowDataItem)_dso.Data;
            if (_item == null)
            {
                LogComment("Could not reference the SlowDataItem");
                return TestResult.Fail;
            }
            LogComment("Referenced all objects successfully");
            return TestResult.Pass;
        }

        TestResult Verify()
        {
            Binding b = new Binding("SlowValue");
            b.NotifyOnTargetUpdated = true;
            b.IsAsync = true;
            b.FallbackValue = "fallback";
            b.Source = _dso;

            _codeBindText.TargetUpdated += new EventHandler<DataTransferEventArgs>(my_datatransfer);
            _codeBindText.SetBinding(TextBlock.TextProperty, b);

            if (_slowText.Text != _fallback)
            {
                LogComment("slowText was not '" + _fallback + "', it contained '" + _slowText.Text + "'");
                return TestResult.Fail;
            }

            if (_fastText.Text != _item.FastValue)
            {
                LogComment("fastText was not 'The end is in sight', it contained '" + _fastText.Text + "'");
                return TestResult.Fail;
            }
            if (_priTestText.Text != _item.FastValue)
            {
                LogComment("priTestText was not '', it contained '" + _priTestText.Text + "'");
                return TestResult.Fail;
            }

            TestResult result = WaitForSignal(10000);
            if (result != TestResult.Pass)
            {
                LogComment("Signal timed out");
                return TestResult.Fail;
            }

            if (_codeBindText.Text != _fallback)
            {
                LogComment("otherText was not '" + _fallback + "', it contained '" + _codeBindText.Text + "'");
                return TestResult.Fail;
            }

            LogComment("All texts had the expected values");
            return TestResult.Pass;
        }

        TestResult BindVerify()
        {
            TestResult result = WaitForSignal(10000);
            if (result != TestResult.Pass)
            {
                LogComment("Signal timed out");
                return TestResult.Fail;
            }

            if (_codeBindText.Text != _item.SlowValueEndResult)
            {
                LogComment("codeBindText was not '" + _item.SlowValueEndResult + "', it contained '" + _codeBindText.Text + "'");
                return TestResult.Fail;
            }


            if (_priTestText.Text != _item.SlowValueEndResult)
            {
                LogComment("priTestText was not '" + _item.SlowValueEndResult + "', it contained '" + _priTestText.Text + "'");
                return TestResult.Fail;
            }


            LogComment("The texts bound to slow data were the expected value after the get returned.");
            return TestResult.Pass;
        }

        private void my_datatransfer(object sender, DataTransferEventArgs args)
        {
            LogComment("Datatransfer");
            Signal(TestResult.Pass);
        }

    }
}
