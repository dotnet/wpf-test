// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshWithReadOnlyDependencyPropertyTest
{
    /******************************************************************************
    * CLASS:          WithReadOnlyDependencyProperty
    ******************************************************************************/
    [Test(0, "PropertyEngine.WithReadOnlyDependencyProperty", TestCaseSecurityLevel.FullTrust, "WithReadOnlyDependencyProperty")]
    public class WithReadOnlyDependencyProperty : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("TestApiRegisterReadOnly")]
        [Variation("TestApiRegisterAttachedReadOnly")]
        [Variation("TestApiSetValue")]
        [Variation("TestApiClearValue")]
        [Variation("TestApiOverrideMetadata")]

        /******************************************************************************
        * Function:          WithReadOnlyDependencyProperty Constructor
        ******************************************************************************/
        public WithReadOnlyDependencyProperty(string arg)
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
            TestWithReadOnlyDependencyProperty test = new TestWithReadOnlyDependencyProperty();

            Utilities.StartRunAllTests("WithReadOnlyDependencyProperty");

            switch (_testName)
            {
                case "TestApiRegisterReadOnly":
                    test.TestApiRegisterReadOnly();
                    break;
                case "TestApiRegisterAttachedReadOnly":
                    test.TestApiRegisterAttachedReadOnly();
                    break;
                case "TestApiSetValue":
                    test.TestApiSetValue();
                    break;
                case "TestApiClearValue":
                    test.TestApiClearValue();
                    break;
                case "TestApiOverrideMetadata":
                    test.TestApiOverrideMetadata();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            Utilities.StopRunAllTests();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestWithReadOnlyDependencyProperty
    ******************************************************************************/
    /// <summary></summary>
    public class TestWithReadOnlyDependencyProperty
    {
        /// <summary>
        /// API Testing: RegisterReadOnly
        /// Class DependencyPropertyKey is covered as well
        /// </summary>
        public void TestApiRegisterReadOnly()
        {
            ApiTestElement.TestApiRegisterReadOnly();
        }

        /// <summary>
        /// API Testing: RegisterAttachedReadOnly
        /// </summary>
        public void TestApiRegisterAttachedReadOnly()
        {
            ApiTestElement.TestApiRegisterAttachedReadOnly();
        }

        /// <summary>
        /// API testing: SetValue 
        /// </summary>
        public void TestApiSetValue()
        {
            Utilities.PrintTitle("Test Setvalue");

            Utilities.PrintStatus("Positive Case: Set value for RO-DP with right key");
            TestElement element = new TestElement();
            element.SetValue(TestElement.IsKeyLocakedPropertyKey, true);
            Utilities.Assert((bool)element.GetValue(TestElement.IsKeyLockedProperty) == true, "Value correctly set");
            Utilities.PrintStatus("Negative case: Set value for RO-DP with incorrect API overload");
            try
            {
                element.SetValue(TestElement.IsKeyLockedProperty, false);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// API Test: ClearValue
        /// </summary>
        public void TestApiClearValue()
        {
            Utilities.PrintTitle("Test ClearValue");
            Utilities.PrintStatus("Positive Case: Clear Value for RO-DP with right key");
            TestElement element = new TestElement();
            element.ClearValue(TestElement.IsMouseOverPropertyKey);
            Utilities.Assert(element.ReadLocalValue(TestElement.IsMouseOverProperty) == DependencyProperty.UnsetValue, "Value is cleared");

            Utilities.PrintStatus("Negative Case: Clear Value for RO-DP without key");
            try
            {
                element.ClearValue(TestElement.IsMouseOverProperty);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

        }

        /// <summary>
        /// Api Testing: OverrideMetadata
        /// </summary>
        public void TestApiOverrideMetadata()
        {
            MyTestElemet.ApiTestOverrideMetadata();
        }

        /******************************************************************************
        * CLASS:          ApiTestElement
        ******************************************************************************/
        /// <summary>
        /// Used for API Testing
        /// </summary>
        public class ApiTestElement : DependencyObject
        {
            private static int s_countFuncCall;

            private static bool ValidateIt(object value)
            {
                s_countFuncCall++;
                return true;
            }

            /// <summary>
            /// API Testing: RegisterReadOnly
            /// </summary>
            public static void TestApiRegisterReadOnly()
            {
                Utilities.PrintTitle("API Testing: RegisterReadOnly");
                Utilities.PrintStatus("Overload 2-1");
                DependencyPropertyKey TestOnePropertyKey = DependencyProperty.RegisterReadOnly("TestOne", typeof(string), typeof(ApiTestElement), new PropertyMetadata("Default"));
                DependencyProperty TestOne = TestOnePropertyKey.DependencyProperty;
                Utilities.Assert(TestOne.ReadOnly, "Property is ReadOnly");
                Utilities.Assert((string)TestOne.GetMetadata(typeof(ApiTestElement)).DefaultValue == "Default", "Default value verified");
                Utilities.Assert(TestOne.ReadOnly, "Property is ReadOnly");
                Utilities.Assert((string)TestOne.GetMetadata(typeof(TestElement)).DefaultValue == "Default", "Default value verified");
                Utilities.PrintStatus("Overload 2-2");
                DependencyPropertyKey TestTwoPropertyKey = DependencyProperty.RegisterReadOnly("TestTwo", typeof(bool), typeof(ApiTestElement), new PropertyMetadata(true), new ValidateValueCallback(ValidateIt));
                DependencyProperty TestTwoProperty = TestTwoPropertyKey.DependencyProperty;
                Utilities.Assert(TestTwoProperty.ReadOnly, "Property is Readonly");
                Utilities.Assert((bool)TestTwoProperty.GetMetadata(typeof(ApiTestElement)).DefaultValue == true, "Metadata is Readonly");
                int count = s_countFuncCall;

                DependencyObject o = new DependencyObject();
                o.SetValue(TestTwoPropertyKey, false);
                Utilities.Assert(s_countFuncCall == count + 1, "ValidateValueCallback is called");
            }

            /// <summary>
            /// API Testing: RegisterAttachedReadOnly
            /// </summary>
            public static void TestApiRegisterAttachedReadOnly()
            {
                Utilities.PrintTitle("API Testing: RegisterAttachedReadOnly");
                Utilities.PrintStatus("Overload 2-1");
                DependencyPropertyKey TestFourPropertyKey = DependencyProperty.RegisterAttachedReadOnly("TestFour", typeof(string), typeof(ApiTestElement), new PropertyMetadata("Default"));
                DependencyProperty TestFour = TestFourPropertyKey.DependencyProperty;
                Utilities.Assert(TestFour.ReadOnly, "Property is ReadOnly");
                Utilities.Assert((string)TestFour.GetMetadata(typeof(ApiTestElement)).DefaultValue == "Default", "Default value verified");
                Utilities.Assert(TestFour.ReadOnly, "Property is ReadOnly");
                Utilities.Assert((string)TestFour.GetMetadata(typeof(TestElement)).DefaultValue == "Default", "Default value verified");
                Utilities.PrintStatus("Overload 2-2");
                DependencyPropertyKey TestFivePropertyKey = DependencyProperty.RegisterAttachedReadOnly("TestFive", typeof(bool), typeof(ApiTestElement), new PropertyMetadata(true), new ValidateValueCallback(ValidateIt));
                DependencyProperty TestFiveProperty = TestFivePropertyKey.DependencyProperty;
                Utilities.Assert(TestFiveProperty.ReadOnly, "Property is Readonly");
                Utilities.Assert((bool)TestFiveProperty.GetMetadata(typeof(ApiTestElement)).DefaultValue == true, "Metadata is Readonly");
                int count = s_countFuncCall;

                DependencyObject d = new DependencyObject();
                d.SetValue(TestFivePropertyKey, false);
                Utilities.Assert(s_countFuncCall == count + 1, "ValidateValueCallback is called");
            }
        }

        /******************************************************************************
        * CLASS:          TestElement
        ******************************************************************************/
        /// <summary>
        /// Used for ReadOnly DependencyProperty Testing
        /// </summary>
        public class TestElement : DependencyObject
        {
            /// <summary>
            /// DependencyPropertyKey for ReadOnly Dependency Property IsMouseOver
            /// </summary>
            public static readonly DependencyPropertyKey IsMouseOverPropertyKey =
              DependencyProperty.RegisterReadOnly("IsMouseOver", typeof(bool), typeof(TestElement), null);

            /// <summary>
            /// A ReadOnly Dependency Property IsMouseOver
            /// </summary>
            public static readonly DependencyProperty IsMouseOverProperty = IsMouseOverPropertyKey.DependencyProperty;

            /// <summary>
            /// DependencyPropertyKey for ReadOnly DependencyProperty IsKeyLocked
            /// </summary>
            public static readonly DependencyPropertyKey IsKeyLocakedPropertyKey =
              DependencyProperty.RegisterReadOnly("IsKeyLocked", typeof(bool), typeof(TestElement), null);

            /// <summary>
            /// A Read Only DependencyProperty
            /// </summary>
            public static readonly DependencyProperty IsKeyLockedProperty = IsKeyLocakedPropertyKey.DependencyProperty;

            /// <summary>
            /// A Read-Write Dependency Property "Magic"
            /// </summary>
            public static readonly DependencyProperty MagicProperty = DependencyProperty.Register("Magic", typeof(bool), typeof(TestElement));
        }

        /******************************************************************************
        * CLASS:          MyTestElemet
        ******************************************************************************/
        /// <summary>
        /// Used for testing OverrideMetadata
        /// </summary>
        public class MyTestElemet : TestElement
        {
            /// <summary>
            /// Testing OverrideMetadata
            /// </summary>
            public static void ApiTestOverrideMetadata()
            {
                Utilities.PrintTitle("Testing OverrideMetadata");
                Utilities.PrintStatus("Positive: DP is registered as RO, override it with OverrideMetadata and right authorization key");
                IsMouseOverProperty.OverrideMetadata(typeof(MyTestElemet), new PropertyMetadata(true), IsMouseOverPropertyKey);

                Utilities.PrintStatus("Negative: DP is registered as RO, override it with OverrideMetadata and wrong authorization key");
                try
                {
                    IsMouseOverProperty.OverrideMetadata(typeof(MyTestElemet), new PropertyMetadata(true), IsKeyLocakedPropertyKey);
                    Utilities.ExpectedExceptionNotReceived();
                }
                catch (ArgumentException ex)
                {
                    Utilities.ExpectedExceptionReceived(ex);
                }

                Utilities.PrintStatus("Negative: DP is registered as RO, override it with OverrideMetadata but wrong API overload (type, metadata)");
                try
                {
                    IsMouseOverProperty.OverrideMetadata(typeof(MyTestElemet), new PropertyMetadata(true));
                    Utilities.ExpectedExceptionNotReceived();
                }
                catch (InvalidOperationException ex)
                {
                    Utilities.ExpectedExceptionReceived(ex);
                }

                Utilities.PrintStatus("Negative: DP is registered as RO, override it with OverrideMetadataReadOnly");
                try
                {
                    IsMouseOverProperty.OverrideMetadata(typeof(MyTestElemet), new PropertyMetadata(true));
                    Utilities.ExpectedExceptionNotReceived();
                }
                catch (InvalidOperationException ex)
                {
                    Utilities.ExpectedExceptionReceived(ex);
                }
            }
        }

        /******************************************************************************
        * CLASS:          AlphaElement
        ******************************************************************************/
        /// <summary>
        /// Used for ReadOnly DependencyProperty Testing
        /// </summary>
        public class AlphaElement : DependencyObject
        {
        }
    }
}
