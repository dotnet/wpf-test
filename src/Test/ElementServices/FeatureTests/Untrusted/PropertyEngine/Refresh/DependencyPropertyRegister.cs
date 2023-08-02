// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshDependencyProperty
{
    /******************************************************************************
    * CLASS:          DependencyPropertyRegister
    ******************************************************************************/
    [Test(0, "PropertyEngine.DependencyProperty", TestCaseSecurityLevel.FullTrust, "DependencyPropertyRegister")]
    public class DependencyPropertyRegister : RefreshDPBase
    {
        #region Private Data
        private string _testName = "";
        private bool _testPassed = false;
        #endregion

        #region Constructor

        [Variation("TestDependecyProperty")]
        [Variation("RegressionTestBug52")]

        /******************************************************************************
        * Function:          DependencyPropertyRegister Constructor
        ******************************************************************************/
        public DependencyPropertyRegister(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "TestDependecyProperty":
                    TestDependecyProperty();
                    break;
                case "RegressionTestBug52":
                    RegressionTestBug52();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            if (_testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          TestDependecyProperty
        ******************************************************************************/
        public void TestDependecyProperty()
        {
            Utilities.StartRunAllTests("DependencyProperty");

            TestDP("DP1", typeof(double), typeof(DependencyPropertyRegister), 3.14, 3.14m);                                  // builtin value type
            TestDP("dp1", typeof(Vector), typeof(DependencyPropertyRegister), new Vector(0, 1), 0);                          // user value type, name is case sensitive
            TestDP("dp2", typeof(Dock), typeof(DependencyPropertyRegister), Dock.Right, new Vector(0, 1));                   // enum
            TestDP("dp3", typeof(Dock), typeof(DependencyPropertyRegister), (Dock)int.MaxValue, 0);                          // out-of-range enum special case
            TestDP("dp4", typeof(MenuItem), typeof(DependencyPropertyRegister), new MenuItem(), new FrameworkElement());     // reference type
            TestDP("dp7", typeof(A), typeof(DependencyPropertyRegister), new C(), new FrameworkElement());                   // inheritance

            TestBadDP(null, typeof(int), typeof(DependencyPropertyRegister), typeof(ArgumentNullException));
            TestBadDP("dp5", null, typeof(DependencyPropertyRegister), typeof(ArgumentNullException));
            TestBadDP("dp6", typeof(int), null, typeof(ArgumentNullException));
            TestBadDP("dp1", typeof(double), typeof(DependencyPropertyRegister), typeof(ArgumentException));                 // same name

            //Any failures are captured by Asserts.
            _testPassed = true;
        }

        /******************************************************************************
        * Function:          RegressionTestBug52
        ******************************************************************************/
        /// <summary>
        /// 



        public void RegressionTestBug52()
        {
            Utilities.PrintStatus("Regression test for bug 52");
            DependencyProperty Test1Property = DependencyProperty.RegisterAttached(
                "Test1", typeof(int), typeof(DependencyPropertyRegister), new PropertyMetadata());
            Utilities.Assert((int)Test1Property.DefaultMetadata.DefaultValue == 0, "DefaltValue is auto-generated.");

            //Any failures are captured by Asserts.
            _testPassed = true;
        }

        private class A {}
        private class B : A {}
        private class C : B {}

        #endregion
    }
}
