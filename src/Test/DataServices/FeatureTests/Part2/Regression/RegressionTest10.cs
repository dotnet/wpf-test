// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test - Empty from SelectedValue Binding Path on 4.5
    /// </description>
    /// </summary>
    [Test(0, "Regression", "RegressionTest10")]
    public class RegressionTest10 : XamlTest
    {
        private ListBox _myList;

        public RegressionTest10()
            : base(@"RegressionTest10.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyBinding);
        }

        private TestResult Setup()
        {
            Status("Setup");

            _myList = (ListBox)LogicalTreeHelper.FindLogicalNode(RootElement, "TestListBox");
            Assert.IsNotNull(_myList, "Can't find ListBox!");

            _myList.DataContext = new TestDataWithStaticProperty();
            WaitForPriority(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        private TestResult VerifyBinding()
        {
            Status("Start to verify binding for seleted item");

            _myList.SelectedIndex = 1;
            WaitForPriority(DispatcherPriority.SystemIdle);

            string getStringFromProperty = TestDataWithStaticProperty.MyInitData.MySelector;
            if (!_myList.SelectedItem.ToString().Equals(getStringFromProperty))
            {
                LogComment("Binding failed, the property value after update should be: " + _myList.SelectedItem.ToString() + ", actual is: " + getStringFromProperty);
                return TestResult.Fail;
            }

            LogComment("Verify binding succeeded");
            return TestResult.Pass;
        }
    }

    #region Help class

    public sealed class TestDataWithStaticProperty
    {
        private static MyData s_myInitData;

        public static MyData MyInitData
        {
            get
            {
                if (s_myInitData == null)
                {
                    s_myInitData = new MyData();
                }
                return s_myInitData;
            }
        }
    }

    public sealed class MyData
    {
        private string _mySelector;

        public string MySelector
        {
            get { return _mySelector; }
            set
            {
                _mySelector = value;
            }
        }
    }    

    #endregion
}
