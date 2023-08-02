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


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshWithStyleSetBindingTest
{
    /******************************************************************************
    * CLASS:          WithStyleSetBinding
    ******************************************************************************/
    [Test(0, "PropertyEngine.Style", TestCaseSecurityLevel.FullTrust, "WithStyleSetBinding")]
    public class WithStyleSetBinding : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("NegativeTestStyleSetBinding")]
        [Variation("PositiveOneBindStyleSetBinding")]
        [Variation("PositiveTwoBindStyleSetBinding")]
        [Variation("NegativeTestFEFSetBinding")]
        [Variation("PositiveTestFEFSetBinding")]

        /******************************************************************************
        * Function:          WithStyleSetBinding Constructor
        ******************************************************************************/
        public WithStyleSetBinding(string arg)
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
            TestWithStyleSetBinding test = new TestWithStyleSetBinding();

            Utilities.StartRunAllTests("WithStyleSetBinding");

            switch (_testName)
            {
                case "NegativeTestStyleSetBinding":
                    test.NegativeTestStyleSetBinding();
                    break;
                case "PositiveOneBindStyleSetBinding":
                    test.PositiveOneBindStyleSetBinding();
                    break;
                case "PositiveTwoBindStyleSetBinding":
                    test.PositiveTwoBindStyleSetBinding();
                    break;
                case "NegativeTestFEFSetBinding":
                    test.NegativeTestFEFSetBinding();
                    break;
                case "PositiveTestFEFSetBinding":
                    test.PositiveTestFEFSetBinding();
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
    * CLASS:          TestWithStyleSetBinding
    ******************************************************************************/
    ///<summary>
    ///styles - SetBinding on a Style
    ///Dev  (Covers other task list items beyond the coverage of this test)
    ///Important API: Style.SetBinding and FrameworkElementFactory.SetBinding
    ///</summary>
    public class TestWithStyleSetBinding
    {
        /******************************************************************************
        * Function:          NegativeTestStyleSetBinding
        ******************************************************************************/
        /// <summary>
        /// Negative Test Cases
        /// </summary>
        public void NegativeTestStyleSetBinding()
        {
            Style anotherStyle = new Style(typeof(Button));
            Utilities.PrintTitle("Negative test cases with Style.SetBinding");
            Utilities.PrintStatus("When dp is null");
            try
            {
                anotherStyle.Setters.Add(new Setter(null, new System.Windows.Data.Binding()));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("When bind is null");
        }

        /******************************************************************************
        * Function:          PositiveOneBindStyleSetBinding
        ******************************************************************************/
        /// <summary>
        /// Positive Test Case. use Style.SetBinding
        /// </summary>
        public void PositiveOneBindStyleSetBinding()
        {
            Utilities.PrintTitle("Positive Test Case with Style.SetBinding");

            Style testStyle = new Style(typeof(Button));

            testStyle.Setters.Add(new Setter(Button.ContentProperty, new System.Windows.Data.Binding(@"TotalIncome")));

            Button testButton = new Button();
            TaxInfo data = new TaxInfo();

            // This is necessary after due 
            Panel panel = new DockPanel();
            panel.Children.Add(testButton);

            testButton.DataContext = data;
            Utilities.Assert(testButton.Content == null, "With no testStyle, binding resove to null");
            testButton.Style = testStyle;
            testButton.ApplyTemplate();

            decimal info = (decimal)testButton.Content;

            Utilities.Assert(info == 100, "With testStyle, binding resove to 100");
        }

        /******************************************************************************
        * Function:          PositiveTwoBindStyleSetBinding
        ******************************************************************************/
        /// <summary>
        /// Positive Test Case. Use Style.SetBinding twice on same property
        /// </summary>
        public void PositiveTwoBindStyleSetBinding()
        {
            Utilities.PrintTitle("Positive Test Case. Use Style.SetBinding twice on same property");

            Style testStyle = new Style(typeof(Button));

            testStyle.Setters.Add(new Setter(Button.ContentProperty, new System.Windows.Data.Binding("TotalIncome")));
            testStyle.Setters.Add(new Setter(Button.ContentProperty, new System.Windows.Data.Binding("TaxDueOrRefund")));

            Button testButton = new Button();
            TaxInfo data = new TaxInfo();

            // This is necessary after due 
            Panel panel = new DockPanel();
            panel.Children.Add(testButton);

            testButton.DataContext = data;
            Utilities.Assert(testButton.Content == null, "With no testStyle, binding resove to null");
            testButton.Style = testStyle;
            testButton.ApplyTemplate();

            decimal info = (decimal)testButton.Content;

            Utilities.Assert(info == 10, "With testStyle, binding resove to 10");

        }

        /******************************************************************************
        * Function:          NegativeTestFEFSetBinding
        ******************************************************************************/
        /// <summary>
        /// Negative Test Case with FrameworkElementFactory.SetBinding
        /// </summary>
        public void NegativeTestFEFSetBinding()
        {

            Utilities.PrintTitle("Negative Test Case with FrameworkElementFactory.SetBinding()");

            FrameworkElementFactory chrome = new FrameworkElementFactory();

            Utilities.PrintStatus("When DependencyProperty is null");
            try
            {
                chrome.SetBinding(null, new System.Windows.Data.Binding());
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("When Binding is null");
            //        try
            //        {
            //          chrome.SetBinding(TextBox.TextProperty, null).
            //          ZBHelper.ExpectedExceptionNotReceived().
            //        }
            //        catch (ArgumentNullException ex)
            //        {
            //          ZBHelper.ExpectedExceptionReceived(ex).
            //        }

        }

        /******************************************************************************
        * Function:          PositiveTestFEFSetBinding
        ******************************************************************************/
        /// <summary>
        /// Positive test case with FrameworkElementFactory.SetBinding
        /// </summary>
        public void PositiveTestFEFSetBinding()
        {
            Utilities.PrintTitle("Positive test case with FrameworkElementFactory.SetBinding()");

            FrameworkElementFactory chrome = new FrameworkElementFactory(typeof(DockPanel));
            FrameworkElementFactory content = new FrameworkElementFactory(typeof(TextBox));
            content.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding("Information"));
            chrome.AppendChild(content);

            Style testStyle = new Style();
            ControlTemplate template = new ControlTemplate(typeof(Button));
            template.VisualTree = chrome;
            testStyle.Setters.Add(new Setter(Button.TemplateProperty, template));

            Button testButton = new Button();

            // This is necessary after due 
            Panel panel = new DockPanel();
            panel.Children.Add(testButton);

            testButton.Style = testStyle;

            TaxInfo myTaxInfo = new TaxInfo();

            testButton.DataContext = myTaxInfo;

            testButton.ApplyTemplate();

            DockPanel dp = (DockPanel)VisualTreeHelper.GetChild(testButton,0);

            TextBox tb = (TextBox)VisualTreeHelper.GetChild(dp,0);

            Utilities.Assert(tb.Text == myTaxInfo.ToString(), "Text is data bound correctly");
        }
    }

    /******************************************************************************
    * CLASS:          TaxInfo
    ******************************************************************************/
    /// <summary>
    /// Summary description for TaxInfo.
    /// </summary>
    public class TaxInfo
    {
        private decimal _totalIncome;

        private decimal _taxPaid;

        private decimal _taxDueOrRefund;

        /// <summary>
        /// Ctor
        /// </summary>
        public TaxInfo()
        {
            _totalIncome = 100;
            _taxPaid = 40;
            CalculateTaxOrRefund();
        }

        private void CalculateTaxOrRefund()
        {
            decimal expectedTax = _totalIncome / 2;

            _taxDueOrRefund = expectedTax - _taxPaid;
        }

        /// <summary>
        /// Total Incomde
        /// </summary>
        /// <value>total income</value>
        public decimal TotalIncome
        {
            get
            {
                return _totalIncome;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Total Income Cannot be Negative");
                }

                if (_totalIncome != value)
                {
                    _totalIncome = value;
                    CalculateTaxOrRefund();
                }
            }
        }

        /// <summary>
        /// Tax Paid
        /// </summary>
        /// <value></value>
        public decimal TaxPaid
        {
            get
            {
                return _taxPaid;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Total Tax Paid Cannot be Negative");
                }

                if (_taxPaid != value)
                {
                    _taxPaid = value;
                    CalculateTaxOrRefund();
                }
            }
        }

        /// <summary>
        /// Tax Due or Refund
        /// </summary>
        /// <value>Positive value for tax due. Negative value for refund</value>
        public decimal TaxDueOrRefund
        {
            get
            {
                return _taxDueOrRefund;
            }
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder(50);

            sb1.Append("TotalIncome:");
            sb1.Append(TotalIncome);
            sb1.Append(" TaxPaid:");
            sb1.Append(TaxPaid);
            if (TaxDueOrRefund >= 0)
            {
                sb1.Append(" TaxDue:");
            }
            else
            {
                sb1.Append(" Refund:");
            }

            sb1.Append(TaxDueOrRefund);
            return sb1.ToString();
        }

        /// <summary>
        /// Used to bind to string
        /// </summary>
        /// <value></value>
        public string Information
        {
            get
            {
                return ToString();
            }
            set
            {
            }
        }
    }
}
