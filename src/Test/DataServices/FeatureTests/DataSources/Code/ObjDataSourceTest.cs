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

	/// <summary>
	/// <description>
	/// Tests the ObjectDataProvider both when it is synchronous and asynchronous
	/// (2 variations). Checks all its parameters, uses the data source in a
	/// bind, does a Refresh(), checks that RefreshCompleted and DataChanged event handlers
	/// were called and verifies that the bound control got the correct values.
	/// </description>
	/// </summary>
    [Test(1, "DataSources", TestCaseSecurityLevel.FullTrust, "ObjDataSourceTest")]
    public class ObjDataSourceTest: XamlTest
    {
        private int _dataChanged = 0;

        private ObjectDataProvider _dso;

        private TextBlock _testText;

        private object _initialObject;

        private bool _asyncTest = false;

        private Type _typeName = typeof(SlowRecord);

        [Variation(true)]
        [Variation(false)]
        public ObjDataSourceTest(bool async) : base(@"Blank.xaml")
        {
            RunSteps += new TestStep(CreateDataSource);
            RunSteps += new TestStep(InitialVerify);
            RunSteps += new TestStep(InitialBind);
            RunSteps += new TestStep(MidVerify);
            if (async)
            {
                _asyncTest = true;
                RunSteps += new TestStep(WaitMidVerify);
            }

            RunSteps += new TestStep(RefreshCall);
            RunSteps += new TestStep(FinalVerify);
        }


        private TestResult CreateDataSource()
        {
            _dso = new ObjectDataProvider();
            ((ISupportInitialize)_dso).BeginInit();
            _dso.DataChanged += new EventHandler(myDataChangedEventHandler);
            _dso.IsAsynchronous = _asyncTest;
            _dso.ObjectType = typeof(SlowRecord);
            _dso.ConstructorParameters.Add("1");
            _dso.IsInitialLoadEnabled = true;
            ((ISupportInitialize)_dso).EndInit();

            LogComment("Testing Object Data Source, Asyncflag = " + _dso.IsAsynchronous);

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

            if (_asyncTest)
            {
                result = CheckValues(true, 0, "1", _typeName, null, "", "Initial Values");
            }
            else
            {
                result = CheckValues(false, 1, "1", _typeName, typeof(SlowRecord), "", "Initial Values");
                _initialObject = _dso.Data;
            }

            LogComment("Initial verification of values");
            return result;
        }

        private TestResult InitialBind()
        {
            Status("Binding to text");

            Binding b = new Binding("StringField");

            b.Source = _dso;
            _testText.SetBinding(TextBlock.TextProperty, b);
            LogComment("Binding to text completed");
            return TestResult.Pass;
        }

        private TestResult MidVerify()
        {
            Status("Mid verification of values");

            TestResult result;
            if (_asyncTest)
            {
                result = CheckValues(true, 0, "1", _typeName, null, "", "Mid Values");
            }
            else 
            {
                WaitForPriority(DispatcherPriority.Background); 
                result = CheckValues(false, 1, "1", _typeName, typeof(SlowRecord), "Record 1", "Mid Values");
            }

            LogComment("Mid verification of values complete");
            return result;
        }


        private TestResult WaitMidVerify()
        {
            Status("Mid verification of values after waiting for 2 seconds");
            TestResult result = WaitForSignal("1");

            if (result != TestResult.Pass)
            {
                LogComment("TIMEOUT");
                return TestResult.Fail;
            }
            WaitForPriority(DispatcherPriority.Background);

            result = CheckValues(true, 1, "1", _typeName, typeof(SlowRecord), "Record 1", "Mid Values After Waiting");

            _initialObject = _dso.Data;
            return result;
        }


        private TestResult RefreshCall()
        {
            Status("Causing implicit Refresh()");
            if (_dso.ConstructorParameters.Count > 0)
            {
                _dso.ConstructorParameters[0] = "2";
            }
            else
            {
                _dso.ConstructorParameters.Add("2");
            }
            LogComment("Implicit Refresh() called");
            return TestResult.Pass;
        }


        private TestResult FinalVerify()
        {
            Status("Final verification of values after waiting for 2 seconds");
            TestResult result = WaitForSignal("2");

            if (result != TestResult.Pass)
            {
                LogComment("TIMEOUT");
                return TestResult.Fail;
            }
            WaitForPriority(DispatcherPriority.Background);

            if (_asyncTest)
            {
                result = CheckValues(true, 2, "2", _typeName, typeof(SlowRecord), "Record 2", "Final Values");
            }
            else
            {
                result = CheckValues(false, 2, "2", _typeName, typeof(SlowRecord), "Record 2", "Final Values");
            }

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
            _dataChanged++;
            Signal(_dataChanged.ToString(), TestResult.Pass);
        }


        private TestResult CheckValues(bool expAsync, int expDataChanged, string expParameters, Type expType, Type expDataString, string expText, string stepName)
        {
            bool pass = true;

            if (_dso.IsAsynchronous != expAsync)
            {
                LogComment("FAILED: dso.IsAsynchronous value, expected:" + expAsync + " actual:" + _dso.IsAsynchronous);
                pass = false;
            }

            if (_dataChanged != expDataChanged)
            {
                LogComment("FAILED: DataChanged flag value, expected:" + expDataChanged + " actual:" + _dataChanged);
                pass = false;
            }

            if (_dso.ConstructorParameters[0].ToString() != expParameters)
            {
                LogComment("FAILED: dso.ConstructorParameters value, expected:" + expParameters + " actual:" + _dso.ConstructorParameters[0].ToString());
                pass = false;
            }

            if (_dso.ObjectType != expType)
            {
                LogComment("FAILED: dso.ObjectType value, expected:" + expType.Name + " actual:" + _dso.ObjectType.Name);
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
                if (_dso.Data == null)
                {
                    LogComment("FAILED: dso.Data value, expected:" + expDataString + " actual: null");
                    pass = false;
                }
                else if (_dso.Data.GetType() != expDataString)
                {
                    LogComment("FAILED: dso.Data value, expected:" + expDataString.ToString() + " actual:" + _dso.Data.GetType().ToString());
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

