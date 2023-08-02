// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
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


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshOnExternalValueValidationTest
{
    /******************************************************************************
    * CLASS:          OnExternalValueValidation
    ******************************************************************************/
    [Test(0, "PropertyEngine.OnExternalValueValidation", TestCaseSecurityLevel.FullTrust, "OnExternalValueValidation")]
    public class OnExternalValueValidation : TestCase
    {
        #region Constructor
        /******************************************************************************
        * Function:          OnExternalValueValidation Constructor
        ******************************************************************************/
        public OnExternalValueValidation()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            TestOnExternalValueValidation test = new TestOnExternalValueValidation();

            Utilities.StartRunAllTests("OnExternalValueValidation");
            test.TestWithCustomClass();
            test.TestWithFrameworkClass();
            Utilities.StopRunAllTests();

            //Any test failures will be caught by Asserts or Exceptions.
            return TestResult.Pass;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestOnExternalValueValidation
    ******************************************************************************/
    /// <summary></summary>
    public class TestOnExternalValueValidation
    {
        /******************************************************************************
        * Function:          TestWithCustomClass
        ******************************************************************************/
        /// <summary>
        /// Custom class AvalonDependencyObject derives directly from 
        /// DependencyObject
        /// </summary>
        public void TestWithCustomClass()
        {
            Utilities.PrintTitle("Test with Custom class called AvalonDependencyObject");
            Utilities.PrintStatus("With MagicNumber property");

            ValidateTestWithCustomClass();
        }

        /******************************************************************************
        * Function:          ValidateTestWithCustomClass
        ******************************************************************************/
        private void ValidateTestWithCustomClass()
        {
            AvalonDependencyObject d1 = new AvalonDependencyObject();

            //Positive case
            d1.SetValue(AvalonDependencyObject.MagicNumberProperty, 1);

            int v = (int)d1.GetValue(AvalonDependencyObject.MagicNumberProperty);

            Utilities.Assert(v == 1, "v == 1");
            d1.SetValue(AvalonDependencyObject.MagicNumberProperty, 99);
            v = (int)d1.GetValue(AvalonDependencyObject.MagicNumberProperty);
            Utilities.Assert(v == 99, "v == 99");

            //Negative case
            try
            {
                d1.SetValue(AvalonDependencyObject.MagicNumberProperty, -1);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            v = (int)d1.GetValue(AvalonDependencyObject.MagicNumberProperty);
            Utilities.Assert(v == 99, "v == 99");
            try
            {
                d1.SetValue(AvalonDependencyObject.MagicNumberProperty, 10000);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            v = (int)d1.GetValue(AvalonDependencyObject.MagicNumberProperty);
            Utilities.Assert(v == 99, "v == 99");
            Utilities.PrintStatus("With Strategy property");

            //Positice case
            d1.SetValue(AvalonDependencyObject.StrategyProperty, Strategy.HurryUp);
            Utilities.Assert(Strategy.HurryUp == (Strategy)d1.GetValue(AvalonDependencyObject.StrategyProperty), "Strategy.HurryUp == (Strategy)d1.GetValue(AvalonDependencyObject.StrategyProperty)");
            d1.SetValue(AvalonDependencyObject.StrategyProperty, Strategy.AnyAction);
            Utilities.Assert(Strategy.AnyAction == (Strategy)d1.GetValue(AvalonDependencyObject.StrategyProperty), "Strategy.AnyAction == (Strategy)d1.GetValue(AvalonDependencyObject.StrategyProperty)");

            //Negative case
            try
            {
                d1.SetValue(AvalonDependencyObject.StrategyProperty, (Strategy)(-1));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            try
            {
                d1.SetValue(AvalonDependencyObject.StrategyProperty, (Strategy)10);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /******************************************************************************
        * Function:          TestWithFrameworkClass
        ******************************************************************************/
        /// <summary>
        /// At the time of test code writing, the following Avalon code has provided ValidateValueCallback
        ///
        /// When property type is an enum
        /// Button.ClickModeProperty, CheckBox.IsCheckedProperty, ListBox.SelectionModeProperty
        /// MenuItem.MenuItemBehaviorProperty, PopUp.PlacementProperty, PopUp.CloseModeProperty
        /// ScrollViewer.HorizontalScrollerVisibilityProperty, ScrollViewer.VerticalScrollerVisibilityProperty
        /// 
        /// When proeprty type is an int.
        /// RepeatButton.DelayProperty, RepeatButton.IntervalProperty
        /// 
        /// Do not like name choice in Popup.cs (Actually in PlacementType.cs, CloseModeType.cs)
        /// See 

        public void TestWithFrameworkClass()
        {
            Utilities.PrintTitle("Test with classes defined within Avalon. CLR Property Provided.");
            ValidateTestWithFrameworkClass();
        }

        /******************************************************************************
        * Function:          ValidateTestWithFrameworkClass
        ******************************************************************************/
        private void ValidateTestWithFrameworkClass()
        {
            Utilities.PrintStatus("ListBox.SelectionModeProperty is of enum type: SelectionMode");

            ListBox listbox = new ListBox();

            listbox.SelectionMode = SelectionMode.Multiple;
            Utilities.Assert(SelectionMode.Multiple == listbox.SelectionMode, "SelectionMode.Multiple == listbox.SelectionMode");
            try
            {
                listbox.SelectionMode = (SelectionMode)5;
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("RepeatButton.DelayProperty is of int type.");

            System.Windows.Controls.Primitives.RepeatButton repeatButton = new System.Windows.Controls.Primitives.RepeatButton();

            repeatButton.Delay = 1000;
            Utilities.Assert(1000 == repeatButton.Delay, "1000 == repeatButton.Delay");
            try
            {
                repeatButton.Delay = -1000;
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }
    }

    /// <summary>
    /// Different strategy enum
    /// </summary>
    public enum Strategy
    {
        /// <summary>
        /// Hurry up. Time is flying away.
        /// </summary>
        HurryUp,
        /// <summary>
        /// Take your time for best result
        /// </summary>
        TakeTime,
        /// <summary>
        /// Do nothing at all.
        /// </summary>
        NoAction,
        /// <summary>
        /// Do whatever
        /// </summary>
        AnyAction
    }


    /******************************************************************************
    * CLASS:          AvalonDependencyObject
    ******************************************************************************/
    /// <summary>
    /// Custon class derives directly from DependencyObject. 
    /// ValidateValueCallback works at core level.
    /// </summary>
    public class AvalonDependencyObject : DependencyObject
    {
        /// <summary>
        /// For MagicNumberProperty, it should be between 0 and 100, inclusive
        /// </summary>
        public static readonly DependencyProperty MagicNumberProperty = DependencyProperty.Register("MagicNumber", typeof(int), typeof(AvalonDependencyObject), new PropertyMetadata(50), new ValidateValueCallback(IsMagicNumberValid));

        /******************************************************************************
        * Function:          IsMagicNumberValid
        ******************************************************************************/
        private static bool IsMagicNumberValid(object value)
        {
            System.Diagnostics.Debug.Assert(value is int, "Error Condition 1001 in RefreshOnExternalValueValidationTest");

            int magic = (int)value;

            if (magic >= 0 && magic <= 100)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// For StrategyProperty, it takes valid enum 'Strategy'
        /// </summary>
        public static readonly DependencyProperty StrategyProperty = DependencyProperty.Register("Strategy", typeof(Strategy), typeof(AvalonDependencyObject), new PropertyMetadata(Strategy.TakeTime), new ValidateValueCallback(IsStrategyValid));

        /******************************************************************************
        * Function:          IsStrategyValid
        ******************************************************************************/
        private static bool IsStrategyValid(object value)
        {
            if ((Strategy)value >= Strategy.HurryUp && (Strategy)value <= Strategy.AnyAction)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
