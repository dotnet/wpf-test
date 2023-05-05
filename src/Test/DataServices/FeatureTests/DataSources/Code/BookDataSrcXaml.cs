// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{


    [Test(2, "DataSources", "bookDataSrcXamlTest")]
    public class bookDataSrcXamlTest: XamlTest
    {
        private bool _dataChanged = false;

        private BookDataSource _dso;

        private TextBlock _testText;

        private object _initialObject;

        public bookDataSrcXamlTest() : base(@"BookDataSource.xaml")
        {
            this.LoadCompleted += new LoadCompletedEventHandler(bookDataSrcXamlTest_LoadCompleted);

            RunSteps += new TestStep(SetUp);
            RunSteps += new TestStep(InitialVerify);
            RunSteps += new TestStep(RefreshCall);
            RunSteps += new TestStep(FinalVerify);
        }

        private void bookDataSrcXamlTest_LoadCompleted(object sender, NavigationEventArgs e)
        {
            LogComment("Adding event handler to the DataSource");
            _dso = RootElement.Resources["DSO"] as BookDataSource;
            if (_dso == null)
            {
                LogComment("DSO is null ");
            }
            _dso.DataChanged += new EventHandler(myDataChangedEventHandler);
        }

        private TestResult SetUp()
        {
            LogComment("Testing Custom Data Source");
              
            _testText = (TextBlock)Util.FindElement(((DockPanel)RootElement), "testText");
            if (_testText == null)
            {
                LogComment("Unable to find TextBlock element");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult InitialVerify()
        {
            Status("Initial verification of values");

            TestResult result;

            result = CheckValues(false, "System.Collections.ObjectModel.ObservableCollection`1[Microsoft.Test.DataServices.BookSource]", "Microsoft C# Language Specifications", "Initial Values");
            _initialObject = _dso.Data;

            LogComment("Initial verification of values");
            return result;
        }


        private TestResult RefreshCall()
        {
            Status("Calling Refresh()");
            _dso.Refresh();
            LogComment("Refresh() called");
            return TestResult.Pass;
        }


        private TestResult FinalVerify()
        {
            Status("Final verification of values after waiting for 2 seconds");
            TestResult result = WaitForSignal();

            if (result != TestResult.Pass)
            {
                LogComment("TIMEOUT");
                return TestResult.Fail;
            }
            WaitForPriority(DispatcherPriority.Background);

            result = CheckValues(true, "System.Collections.ObjectModel.ObservableCollection`1[Microsoft.Test.DataServices.BookSource]", "Microsoft C# Language Specifications", "Final Values");

            if (Util.CompareObjects(_initialObject, _dso.Data))
            {
                LogComment("Initial value of Data and Final value of Data are equal");
                result = TestResult.Fail;
            }

            LogComment("Final verification of values complete");
            return result;
        }


        public void myDataChangedEventHandler(object sender, EventArgs args)
        {
            LogComment("DataChanged Fired");
            _dataChanged = true;
            Signal(TestResult.Pass);
        }


        public void myRefreshEventHandler(object sender, AsyncCompletedEventArgs args)
        {
            LogComment("Refresh Fired");
        }


        private TestResult CheckValues(bool expDataChanged, string expDataString, string expText, string stepName)
        {
            bool pass = true;

            if (_dataChanged != expDataChanged)
            {
                LogComment("FAILED: DataChanged flag value, expected:" + expDataChanged + " actual:" + _dataChanged);
                pass = false;
            }

            if (expDataString == null)
            {
                if (_dso.Data != null)
                {
                    LogComment("FAILED: dso.Data value, expected:null actual:" + _dso.Data.ToString());
                    pass = false;
                }
            }
            else
            {
                if (_dso.Data.ToString() != expDataString)
                {
                    LogComment("FAILED: dso.Data value, expected:" + expDataString + " actual:" + _dso.Data.ToString());
                    pass = false;
                }
            }

            if (_testText.Text != expText)
            {
                LogComment("FAILED: testText.Text value, expected:" + expText + " actual:" + _testText.Text);
                pass = false;
            }

            if (pass)
            {
                LogComment("Properties are as expected for " + stepName);
                return TestResult.Pass;
            }
            else
            {
                LogComment("Properties were not as expected for " + stepName);
                return TestResult.Fail;
            }
        }
    }
}

