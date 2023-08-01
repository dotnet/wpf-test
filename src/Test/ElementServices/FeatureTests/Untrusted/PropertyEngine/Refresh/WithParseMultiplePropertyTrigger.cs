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


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshWithParseMultiplePropertyTriggerTest
{
    /******************************************************************************
    * CLASS:          WithParseMultiplePropertyTrigger
    ******************************************************************************/
    [Test(0, "PropertyEngine.WithParseMultiplePropertyTrigger", TestCaseSecurityLevel.FullTrust, "WithParseMultiplePropertyTrigger")]
    public class WithParseMultiplePropertyTrigger : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("TestPositive")]
        [Variation("TestPositiveNoDefName")]

        /******************************************************************************
        * Function:          WithParseMultiplePropertyTrigger Constructor
        ******************************************************************************/
        public WithParseMultiplePropertyTrigger(string arg)
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
            TestWithParseMultiplePropertyTrigger test = new TestWithParseMultiplePropertyTrigger();

            Utilities.StartRunAllTests("WithParseMultiplePropertyTrigger");

            switch (_testName)
            {
                case "TestPositive":
                    test.TestPositive();
                    break;
                case "TestPositiveNoDefName":
                    test.TestPositiveNoDefName();
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
    * CLASS:          TestWithParseMultiplePropertyTrigger
    ******************************************************************************/
    /// <summary>
    /// </summary>
    public class TestWithParseMultiplePropertyTrigger
    {
        /// <summary>
        /// Misc test. Not used in this file
        /// </summary>
        public void TestMisc()
        {
        }

        /// <summary>
        /// Positive Test Case
        /// </summary>
        public void TestPositive()
        {
            string XamlString = @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x= 'http://schemas.microsoft.com/winfx/2006/xaml'>
  <StackPanel.Resources>
    <Style x:Key='MyStyle' TargetType='{x:Type Button}'>
      <Setter Property='Background' Value='Blue' />
      <Style.Triggers>
        <MultiTrigger>
          <MultiTrigger.Conditions>
            <Condition Property='IsPressed' Value='True'/>
            <Condition Property='IsKeyboardFocusWithin' Value='True'/>
            <Condition Property='IsEnabled' Value='True'/>
          </MultiTrigger.Conditions>
          <Setter Property='Background' Value='Red'/>
        </MultiTrigger>
      </Style.Triggers>
    </Style>
  </StackPanel.Resources>
  <Button Style='{DynamicResource MyStyle}'>Hello MultiTrigger</Button>
</StackPanel>";
            Utilities.PrintTitle("Positive Test Case");
            StackPanel sp = (StackPanel)UtilityHelper.Utilities.FromXamlToElement(XamlString);
            sp.ApplyTemplate();
            Button testButton = (Button)sp.Children[0];
            Style testButtonStyle = testButton.Style;
            Utilities.Assert(testButtonStyle.Triggers != null, "testButtonStyle.Triggers != null");
            MultiTrigger testTrigger = (MultiTrigger)testButtonStyle.Triggers[0];
            Utilities.Assert(testTrigger.Conditions.Count == 3, "testTrigger.Conditions.Count == 3");
            //Check the last Condition
            System.Windows.Condition lastCondition = testTrigger.Conditions[2];
            Utilities.Assert(lastCondition.Property == FrameworkElement.IsEnabledProperty,
              "lastCondition.Property == FrameworkElement.IsEnabledProperty");
            Utilities.Assert((bool)lastCondition.Value == true, "(bool)lastCondition.Value == true");

        }

        /******************************************************************************
        * CLASS:          TestPositiveNoDefName
        ******************************************************************************/
        /// <summary>
        /// Positive test case without use of x:Key
        /// </summary>
        public void TestPositiveNoDefName()
        {
            string XamlString = @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x= 'http://schemas.microsoft.com/winfx/2006/xaml'>
  <StackPanel.Resources>
    <Style TargetType='{x:Type Button}'>
      <Setter Property='Background' Value='Blue' />   
      <Style.Triggers>
        <MultiTrigger>
          <MultiTrigger.Conditions>
            <Condition Property='IsPressed' Value='True'/>
            <Condition Property='IsKeyboardFocusWithin' Value='True'/>
            <Condition Property='IsEnabled' Value='True'/>
          </MultiTrigger.Conditions>
          <Setter Property='Background' Value='Red'/>
        </MultiTrigger>
      </Style.Triggers>
    </Style>
  </StackPanel.Resources>
  <Button>Hello MultiTrigger</Button>
</StackPanel>";
            Utilities.PrintTitle("Positive Test Case Without using x:Key");

            StackPanel sp = (StackPanel)UtilityHelper.Utilities.FromXamlToElement(XamlString);
            sp.ApplyTemplate();
            Button testButton = (Button)sp.Children[0];
            Style testButtonStyle = testButton.Style;
            Utilities.Assert(testButtonStyle.Triggers != null, "testButtonStyle.Triggers != null");
            MultiTrigger testTrigger = (MultiTrigger)testButtonStyle.Triggers[0];
            Utilities.Assert(testTrigger.Conditions.Count == 3, "testTrigger.Conditions.Count == 3");
            //Check the last Condition
            System.Windows.Condition lastCondition = testTrigger.Conditions[2];
            Utilities.Assert(lastCondition.Property == FrameworkElement.IsEnabledProperty,
              "lastCondition.Property == FrameworkElement.IsEnabledProperty");
            Utilities.Assert((bool)lastCondition.Value == true, "(bool)lastCondition.Value == true");

        }
    }
}
