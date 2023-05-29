// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
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


//Quick Document:
//This class is to test parsing BasedOn between styles

//Note: 2004 - 07 - 29 
// Three tests are disabled: They are 
// PositiveTestOneBasedOnInReverseOrder
// PositiveTestTwoBasedOnInReverseOrder
// NegativeTestCircularBasedOn
//This is in anticipation of dev code check with the removal of 
//BamlObjectFactories, some resources references within styles when 
//loading from xaml are now order dependent, where they didnt used to be.
//
//
//Add a new test case PositiveTestTwoBasedOn to preserve similar test coverage

namespace Avalon.Test.CoreUI.PropertyEngine.RefreshWithParseBasedOnInStyleTest
{
    /******************************************************************************
    * CLASS:          WithParseBasedOnInStyle
    ******************************************************************************/
    [Test(0, "PropertyEngine.WithParseBasedOnInStyle", TestCaseSecurityLevel.FullTrust, "WithParseBasedOnInStyle")]
    public class WithParseBasedOnInStyle : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("PositiveTestOneBasedOn")]
        [Variation("PositiveTestTwoBasedOn")]
        [Variation("PositiveTestOneBasedOnInReverseOrder", Disabled=true)]
        [Variation("PositiveTestTwoBasedOnInReverseOrder", Disabled=true)]
        [Variation("NegativeTestCircularBasedOn")]
        [Variation("NegativeBasedOnDifferentType")]
        [Variation("NegativeBasedOnItself")]

        /******************************************************************************
        * Function:          WithParseBasedOnInStyle Constructor
        ******************************************************************************/
        public WithParseBasedOnInStyle(string arg)
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
            TestWithParseBasedOnInStyle test = new TestWithParseBasedOnInStyle();

            Utilities.StartRunAllTests("WithParseBasedOnInStyle");

            switch (_testName)
            {
                case "PositiveTestOneBasedOn":
                    test.PositiveTestOneBasedOn();
                    break;
                case "PositiveTestTwoBasedOn":
                    test.PositiveTestTwoBasedOn();
                    break;
                case "PositiveTestOneBasedOnInReverseOrder":
                    test.PositiveTestOneBasedOnInReverseOrder();
                    break;
                case "PositiveTestTwoBasedOnInReverseOrder":
                    test.PositiveTestTwoBasedOnInReverseOrder();
                    break;
                case "NegativeTestCircularBasedOn":
                    test.NegativeTestCircularBasedOn();
                    break;
                case "NegativeBasedOnDifferentType":
                    test.NegativeBasedOnDifferentType();
                    break;
                case "NegativeBasedOnItself":
                    test.NegativeBasedOnItself();
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
    * CLASS:          TestWithParseBasedOnInStyle
    ******************************************************************************/
    /// <summary></summary>
    public class TestWithParseBasedOnInStyle
    {

        /******************************************************************************
        * Function:          PositiveTestOneBasedOn
        ******************************************************************************/
        /// <summary>
        /// One Based-On Parsing
        /// </summary>
        public void PositiveTestOneBasedOn()
        {
            Utilities.PrintTitle("One Based-On Parsing");

            string XamlString = @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x= 'http://schemas.microsoft.com/winfx/2006/xaml'>
  <StackPanel.Resources>
    <Style x:Key='MyStyle' TargetType='{x:Type Button}'>
      <Setter Property='Background' Value='Blue' />   
    </Style>
    <Style x:Key='AnotherStyle' BasedOn='{StaticResource MyStyle}' TargetType='{x:Type Button}'>
    </Style>
  </StackPanel.Resources>
  <Button Style='{DynamicResource AnotherStyle}'>Hello</Button>
</StackPanel>";
            StackPanel sp = (StackPanel)UtilityHelper.Utilities.FromXamlToElement(XamlString);

            sp.ApplyTemplate(); //Is it necessary?

            Button btn = (Button)VisualTreeHelper.GetChild(sp,0);

            Utilities.Assert(btn.Background == Brushes.Blue, "Style has been applied. Background is Blue");
            Utilities.PrintStatus("Keep Verifying Style");

            Style btnStyle = btn.Style;
            Style myStyle = (Style)sp.FindResource("MyStyle");

            Utilities.Assert(btnStyle.BasedOn == myStyle, "Style.BasedOn Verified");

        }

        /******************************************************************************
        * Function:          PositiveTestTwoBasedOn
        ******************************************************************************/
        /// <summary>
        /// Two Based-On Parsing
        /// : This test case is added to preserve code coverage since
        /// Reverse-Order test cases are disabled now 
        /// </summary>
        public void PositiveTestTwoBasedOn()
        {
            Utilities.PrintTitle("Two Based-On Parsing");

            string XamlString = @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x= 'http://schemas.microsoft.com/winfx/2006/xaml'>
  <StackPanel.Resources>
    <Style x:Key='MyStyle' TargetType='{x:Type Button}'>
      <Setter Property='Background' Value='Blue' />   
    </Style>
    <Style x:Key='AnotherStyle' BasedOn='{StaticResource MyStyle}' TargetType='{x:Type Button}'>
      <Setter Property='Width' Value='300' />   
    </Style>
    <Style x:Key='InUseStyle' BasedOn='{StaticResource AnotherStyle}' TargetType='{x:Type Button}'>
    </Style>
  </StackPanel.Resources>
  <Button Style='{DynamicResource InUseStyle}'>Hello</Button>
</StackPanel>";

            StackPanel sp = (StackPanel)UtilityHelper.Utilities.FromXamlToElement(XamlString);

            sp.ApplyTemplate(); //Is it necessary?

            Button btn = (Button)VisualTreeHelper.GetChild(sp,0);

            Utilities.Assert(btn.Background == Brushes.Blue, "Style has been applied. Background is Blue");
            Utilities.PrintStatus("Keep Verifying Style");

            Style btnStyle = btn.Style;
            Style myStyle = (Style)sp.FindResource("MyStyle");

            Utilities.Assert(btnStyle.BasedOn.BasedOn == myStyle, "Style.BasedOn.BasedOn Verified");

        }


        /******************************************************************************
        * Function:          PositiveTestOneBasedOnInReverseOrder
        ******************************************************************************/
        /// <summary>
        /// One Based-On parsing, in reverse order
        /// </summary>
        public void PositiveTestOneBasedOnInReverseOrder()
        {
            Utilities.PrintTitle("One Based-On parsing, in reverse order");

            string XamlString = @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x= 'http://schemas.microsoft.com/winfx/2006/xaml'>
  <StackPanel.Resources>
    <Style x:Key='AnotherStyle' BasedOn='{StaticResource MyStyle}' TargetType='{x:Type Button}'>
    </Style>
    <Style x:Key='MyStyle' TargetType='{x:Type Button}'>
      <Setter Property='Background' Value='Blue' />   
    </Style>
  </StackPanel.Resources>
  <Button Style='{DynamicResource AnotherStyle}'>Hello</Button>
</StackPanel>";

            StackPanel sp = (StackPanel)UtilityHelper.Utilities.FromXamlToElement(XamlString);

            sp.ApplyTemplate(); //Is it necessary

            Button btn = (Button)VisualTreeHelper.GetChild(sp,0);

            Utilities.Assert(btn.Background == Brushes.Blue, "Style has been applied. Background is Blue");
            Utilities.PrintStatus("Keep Verifying Style");

            Style btnStyle = btn.Style;
            Style myStyle = (Style)sp.FindResource("MyStyle");

            Utilities.Assert(btnStyle.BasedOn == myStyle, "Style.BasedOn Verified");

        }

        /******************************************************************************
        * Function:          PositiveTestTwoBasedOnInReverseOrder
        ******************************************************************************/
        /// <summary>
        /// Two Based-On parsing, in reverse order
        /// </summary>
        public void PositiveTestTwoBasedOnInReverseOrder()
        {
            Utilities.PrintTitle("Two Based-On parsing, in reverse order");

            string XamlString = @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x= 'http://schemas.microsoft.com/winfx/2006/xaml'>
  <StackPanel.Resources>
    <Style x:Key='InUseStyle' BasedOn='{StaticResource AnotherStyle}' TargetType='{x:Type Button}'>
    </Style>
    <Style x:Key='AnotherStyle' BasedOn='{StaticResource MyStyle}' TargetType='{x:Type Button}'>
      <Setter Property='Width' Value='300' />   
    </Style>
    <Style x:Key='MyStyle' TargetType='{x:Type Button}'>
      <Setter Property='Background' Value='Blue' />   
    </Style>
  </StackPanel.Resources>
  <Button Style='{DynamicResource InUseStyle}'>Hello</Button>
</StackPanel>";

            StackPanel sp = (StackPanel)UtilityHelper.Utilities.FromXamlToElement(XamlString);

            sp.ApplyTemplate(); //Is it necessary

            Button btn = (Button)VisualTreeHelper.GetChild(sp,0);

            Utilities.Assert(btn.Background == Brushes.Blue, "Style has been applied. Background is Blue");
            Utilities.Assert(btn.Width == 300, "And Length is 300pt");
            Utilities.PrintStatus("Keep Verifying Style");

            Style btnStyle = btn.Style;
            Style myStyle = (Style)sp.FindResource("MyStyle");

            Utilities.Assert(btnStyle.BasedOn.BasedOn == myStyle, "Style.BasedOn.BasedOn Verified");

        }

        /******************************************************************************
        * Function:          NegativeTestCircularBasedOn
        ******************************************************************************/
        /// <summary>
        /// Three Based-On in circular order
        /// 
        /// We should Get System.Windows.Markup.XamlParseException: Line 13 Offset 11 You cannot
        /// have a Resource reference or Style reference refer to itself.
        /// </summary>
        public void NegativeTestCircularBasedOn()
        {
            Utilities.PrintTitle("Three Based-On in circular order");
            string XamlString = @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x= 'http://schemas.microsoft.com/winfx/2006/xaml'>
  <StackPanel.Resources>
    <Style x:Key='InUseStyle' BasedOn='{StaticResource AnotherStyle}' TargetType='{x:Type Button}'>
    </Style>
    <Style x:Key='AnotherStyle' BasedOn='{StaticResource MyStyle}' TargetType='{x:Type Button}'>
      <Setter Property='Width' Value='300' />   
    </Style>
    <Style x:Key='MyStyle' BasedOn='{StaticResource InUseStyle}' TargetType='{x:Type Button}'>
      <Setter Property='Background' Value='Blue' />   
    </Style>
  </StackPanel.Resources>
  <Button Style='{DynamicResource InUseStyle}'>Hello</Button>
</StackPanel>";

            try
            {
                StackPanel sp = (StackPanel)UtilityHelper.Utilities.FromXamlToElement(XamlString);

                Utilities.ExpectedExceptionNotReceived();
            }
            catch (XamlParseException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

        }

        /******************************************************************************
        * Function:          NegativeBasedOnDifferentType
        ******************************************************************************/
        /// <summary>
        /// Negative Test case: BasedOn a Style whose target type is not a base type of current type
        /// </summary>
        public void NegativeBasedOnDifferentType()
        {
            Utilities.PrintTitle("BasedOn Different Type");
            string XamlString = @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x= 'http://schemas.microsoft.com/winfx/2006/xaml'>
  <StackPanel.Resources>
    <Style x:Key='MyStyle' TargetType='{x:Type Button}'>
      <Setter Property='Background' Value='Blue' />   
    </Style>
    <Style x:Key='AnotherStyle' BasedOn='{StaticResource MyStyle}' TargetType='{x:Type MenuItem}'>
    </Style>
  </StackPanel.Resources>
  <MenuItem Style='{DynamicResource AnotherStyle}' />
</StackPanel>";

            try
            {
                StackPanel sp = (StackPanel)UtilityHelper.Utilities.FromXamlToElement(XamlString);

                Utilities.ExpectedExceptionNotReceived();
            }
            catch (XamlParseException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

        }

        /******************************************************************************
        * Function:          NegativeBasedOnItself
        ******************************************************************************/
        /// <summary>
        /// Negative Test case: BasedOn itself
        /// </summary>
        public void NegativeBasedOnItself()
        {
            Utilities.PrintTitle("BasedOn Itself");
            string XamlString = @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x= 'http://schemas.microsoft.com/winfx/2006/xaml'>
  <StackPanel.Resources>
    <Style x:Key='MyStyle' BasedOn='{StaticResource MyStyle}' TargetType='{x:Type Button}'>
      <Setter Property='Background' Value='Blue' />   
    </Style>
  </StackPanel.Resources>
  <Button Style='{DynamicResource MyStyle}'>Hello</Button>  
</StackPanel>";

            try
            {
                StackPanel sp = (StackPanel)UtilityHelper.Utilities.FromXamlToElement(XamlString);

                Utilities.ExpectedExceptionNotReceived();
            }
            catch (XamlParseException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);

                //Check 
                string exceptionMessage = ex.Message;
                if (exceptionMessage.Contains("You") || exceptionMessage.Contains("you"))
                {
                    Utilities.PrintStatus(" Not fixed.");
                    Utilities.PrintStatus("ParserBamlObjectFactoryCircularRef error message: use third-person");
                }
                else
                {
                    Utilities.PrintStatus("Bug  Already Fixed.");
                }
            }

        }
    }
}

