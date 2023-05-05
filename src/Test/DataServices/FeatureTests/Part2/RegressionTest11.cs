// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test - WPF Async PropertyChange not handled properly with complex paths.
    /// </description>
    /// </summary>
    [Test(0, "Binding", "RegressionTest11", Versions = "4.5.1+")]
    public class RegressionTest11 : XamlTest
    {
        #region Private Data

        private MyBaseData _myDataObj;
        private Button _myButton;
        private int _expectValue = 5;

        #endregion

        #region Constuctor

        public RegressionTest11()
            : base(@"RegressionTest11.xaml")
        {
            InitializeSteps += new TestStep(SetUp);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RegressionTest11Test);
        }

        #endregion

        #region Test Steps

        private TestResult SetUp()
        {
            Status("Setup specific for RegressionTest11");

            _myDataObj = RootElement.FindResource("myDataObject") as MyBaseData;
            _myButton = RootElement.FindName("MyButton") as Button;

            Assert.IsNotNull(_myDataObj, "Can't find resource from page which named mydataobj");
            Assert.IsNotNull(_myButton, "Can't find button from page which named MyButton");

            LogComment("Setup for RegressionTest11 was successful");
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _myDataObj = null;
            _myButton = null;

            return TestResult.Pass;
        }

        private TestResult RegressionTest11Test()
        {
            Status("Start DataServicesBugTest");

            LogComment("Step1:change button content by ThreadPool");
            ThreadPool.QueueUserWorkItem(state => { _myDataObj.SubDataObject.Counter = -1; });
            Thread.Sleep(200);

            LogComment("Step2:change button content by new instance");
            _myDataObj.SubDataObject = new MySubData(_expectValue);
            DispatcherHelper.DoEvents(1000);

            LogComment("Start to verify myButton's content");
            int actualValue = _myButton.Content == null ? 0 : int.Parse(_myButton.Content.ToString());

            Assert.IsTrue(actualValue == _expectValue, "\nFailed, the expected button content value should be: " + _expectValue + "\nAnd, please note that this bug just fixed in \"4.5.1 LDR branch\"\n");

            LogComment("Pass, step2 reset the value from step1 setting, button content value show correctly!");
            return TestResult.Pass;

        }

        #endregion

    }

    #region Helper Classes

    public class MyBaseData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MySubData _mySubDataObject;

        public MySubData SubDataObject
        {
            get
            {
                return _mySubDataObject;
            }
            set
            {
                _mySubDataObject = value;
                var eventChange = PropertyChanged;
                if (eventChange != null)
                {
                    eventChange(this, new PropertyChangedEventArgs("SubDataObject"));
                }
            }
        }

        public MyBaseData()
        {
            SubDataObject = new MySubData();
        }
    }

    public class MySubData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _counter;

        public int Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                var eventChange = PropertyChanged;
                if (eventChange != null)
                {
                    eventChange(this, new PropertyChangedEventArgs("Counter"));
                }
            }
        }

        public MySubData()
        {
            Counter = 0;
        }

        public MySubData(int initNum)
        {
            Counter = initNum;
        }
    }

    #endregion

}
