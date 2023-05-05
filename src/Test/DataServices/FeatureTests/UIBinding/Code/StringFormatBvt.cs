// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using System.Diagnostics;

namespace Microsoft.Test.DataServices
{
    /// <summary> StringFormat Tests including following cases 
    /// <description> 
    ///     Scenario 1 - number of parameters to StringFormat, and binding errors, etc.
    ///     Scenario 2 - 3 basic conditions (S2T, string at T, !IsNullOrEmpty(StringFormat))
    ///     Scenario 3 - user converter
    ///     Scenario 4 - update direction, binding trace, etc.
    ///     Scenario 6 - binding culture tests
    /// </description>
    /// <relatedTasks>

    /// </relatedTasks>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(0, "Binding", "StringFormatBvt")]
    public class StringFormatBvt : XamlTest
    {
        #region private fields

        private Page _page;
        private TextBlock _btb1;                 // B: 0 param w/ OneWay
        private TextBlock _btb2;                 // B: 1 param w/ OneTime + converter
        private TextBlock _btb3;                 // B: 2 params w/ TwoWay w/o converter
        private ComboBox _myComboBox;

        private TextBlock _usTB,_trTB;           // B: binding culture
        private Button _button2;                 // B: binding culture

        private TextBlock _mbtb1,_mbtb2,_mbtb3;  // MB: 0 param w/ 2 bindings
        private TextBlock _mbtb4,_mbtb5,_mbtb6;  // MB: 1 param w/ 2 bindings
        private TextBlock _mbtb7;                // MB: 2 params w/ 2 bindings + TwoWay
        private TextBlock _mbtb8;                // MB: 3 params w/ 2 bindings
        private TextBlock _mbtb9;                // MB: composite 1 param

        private ListBox _lb0;                    // S - int  
        private ListBox _lb1;                    // S - obj
        private ListBox _lb2;                    // S - date
        private ListBox _lb3;                    // S - enum

        private TextBlock _tbNull;               // T - binding to null
        private TextBlock _tbT;                  // Foreground binding to the text of another control

        private ListBox _udLB;                   // Update direction lb
        private TextBlock _udTB1,_udTB2;         // Update direction
        private ComboBox _cb10;                  // $10 case
        private TextBox _tBox;                   // $10 case

        #endregion

        public StringFormatBvt()
            : base(@"StringFormatBvt.xaml")
        {
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(CheckParams);
            RunSteps += new TestStep(VerifyBasicConditions);
            RunSteps += new TestStep(TestUpdateDirection);
            RunSteps += new TestStep(TestBindingCulture);
        }

        #region TestSteps

        /// <summary>
        /// Initialize all controls being tested
        /// </summary>
        /// <returns>fail if some control can't be initialized for some reason</returns>
        private TestResult Init()
        {
            Status("Init");
            WaitForPriority(DispatcherPriority.SystemIdle);

            _page = this.Window.Content as Page;
            if (_page == null)
            {
                LogComment("Fail - Page is null");
                return TestResult.Fail;
            }

            _btb1 = (TextBlock)Util.FindElement(RootElement, "Btb1");
            _btb2 = (TextBlock)Util.FindElement(RootElement, "Btb2");
            _btb3 = (TextBlock)Util.FindElement(RootElement, "Btb3");
            _myComboBox = (ComboBox)Util.FindElement(RootElement, "myComboBox");

            _usTB = (TextBlock)Util.FindElement(RootElement, "UStextblock");
            _trTB = (TextBlock)Util.FindElement(RootElement, "TRtextblock");
            _button2 = (Button)Util.FindElement(RootElement, "button2");

            _mbtb1 = (TextBlock)Util.FindElement(RootElement, "MBtb1");
            _mbtb2 = (TextBlock)Util.FindElement(RootElement, "MBtb2");
            _mbtb3 = (TextBlock)Util.FindElement(RootElement, "MBtb3");
            _mbtb4 = (TextBlock)Util.FindElement(RootElement, "MBtb4");
            _mbtb5 = (TextBlock)Util.FindElement(RootElement, "MBtb5");
            _mbtb6 = (TextBlock)Util.FindElement(RootElement, "MBtb6");
            _mbtb7 = (TextBlock)Util.FindElement(RootElement, "MBtb7");
            _mbtb8 = (TextBlock)Util.FindElement(RootElement, "MBtb8");
            _mbtb9 = (TextBlock)Util.FindElement(RootElement, "MBtb9");

            _lb0 = (ListBox)Util.FindElement(RootElement, "lb0");
            _lb1 = (ListBox)Util.FindElement(RootElement, "lb1");
            _lb2 = (ListBox)Util.FindElement(RootElement, "lb2");
            _lb3 = (ListBox)Util.FindElement(RootElement, "lb3");
            _tbNull = (TextBlock)Util.FindElement(RootElement, "tbNull");
            _tbT = (TextBlock)Util.FindElement(RootElement, "tbT");

            _udLB = (ListBox)Util.FindElement(RootElement, "udLB");
            _udTB1 = (TextBlock)Util.FindElement(RootElement, "udTB1");
            _udTB2 = (TextBlock)Util.FindElement(RootElement, "udTB2");
            _cb10 = (ComboBox)Util.FindElement(RootElement, "cb10");
            _tBox = (TextBox)Util.FindElement(RootElement, "tBox");

            if ((_btb1 == null) || (_btb2 == null) || (_btb3 == null))
            {
                LogComment("Unable to Binding TextBlock element(s).");
                return TestResult.Fail;
            }
            if ((_usTB == null) || (_trTB == null) || (_button2 == null))
            {
                LogComment("Unable to locate binding culture element(s).");
                return TestResult.Fail;
            }
            if ((_mbtb1 == null) || (_mbtb2 == null) || (_mbtb3 == null) || (_mbtb4 == null) ||
                (_mbtb5 == null) || (_mbtb6 == null) || (_mbtb7 == null) || (_mbtb8 == null) || (_mbtb9 == null))
            {
                LogComment("Unable to locate MB TextBlock element(s).");
                return TestResult.Fail;
            }
            if ((_lb1 == null) || (_lb2 == null) || (_lb3 == null) || (_lb0 == null) || (_tbNull == null) || (_tbT == null))
            {
                LogComment("Unable to locate basic conditions tests' element(s).");
                return TestResult.Fail;
            }
            if ((_udLB == null) || (_udTB1 == null) || (_udTB2 == null) || (_cb10 == null) || (_tBox == null))
            {
                LogComment("Unable to locate update direction tests' element(s).");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Check the param to StringFormat by 0, 1, 2 (B/MB), and 3 (MB)
        /// where BindingExpression.Status = BindingStatus.UpdateTargetError when
        ///     Binding w/ 2 or more params
        ///     MultiBinding w/ params more than the Bindings
        /// </summary>
        /// <returns></returns>
        private TestResult CheckParams()
        {
            Status("CheckParams");
            string expected;
            CultureInfo en_us = CultureInfo.CreateSpecificCulture("en-us");

            // zero param - B: btb1; MB: mbtb1; --> both should be OK
            Util.AssertEquals(_btb1.Text, "The value is: 168.361", "Error in btb1.Text on index=0");
            _myComboBox.SelectedIndex = 1;
            Util.AssertEquals(_btb1.Text, "The value is: 456.9832", "Error in btb1.Text on index=1");

            expected = String.Format(en_us, "{0:C2}", _mbtb2.Text);
            Util.AssertEquals(_mbtb1.Text, expected, "Error in mbtb1.Text with C2");

            // one param - B: btb2; MB: mbtb4; --> both OK
            Util.AssertEquals(_btb2.Text, "My Name is: Btb2: English (United States)", "Error in btb2.Text");
            Util.AssertEquals(_mbtb4.Text, "12345", "Error in mbtb4.Text");

            // two params - B: btb3 --> BindingExpression.Status = BindingStatus.UpdateTargetError 
            //            MB: mbtb7 --> OK 
            if (!CheckBindingError(_btb3, TextBlock.TextProperty, BindingStatus.UpdateTargetError, false))
            {
                LogComment("Binding in Btb3 did not get expected Binding Error");
                return TestResult.Fail;
            }
            Util.AssertEquals(_mbtb7.Text, "12345 67891!", "Error in mbtb7.Text");

            // three params - MB: mbtb8 --> BindingExpression.Status = BindingStatus.UpdateTargetError
            if (!CheckBindingError(_mbtb8, TextBlock.TextProperty, BindingStatus.UpdateTargetError, true))
            {
                LogComment("Binding in mbtb8 did not get expected Binding Error");
                return TestResult.Fail;
            }

            // one param mb w/ composite format
            Util.AssertEquals(_mbtb9.Text, "First Value is: 67891", "Error in mbtb9.Text");

            return TestResult.Pass;
        }

        /// <summary>
        /// Scenario 2 - 3 Basic Conditions we are looking at: 
        ///     a. S2T binding 
        ///     b. T is string; attempt: int, datetime, object, any can convert to string, and can't convert to string
        ///     c. StringFormat is not null or empty - omit here since other tests are done for it
        /// </summary>
        /// <returns></returns>
        private TestResult VerifyBasicConditions()
        {
            Status("VerifyBasicConditions");
            ListBoxItem lbi;
            TextBlock tb;

            lbi = Util.FindVisualChild<ListBoxItem>(_lb0);
            tb = Util.FindControlByTypeInTemplate<TextBlock>(lbi);
            Util.AssertEquals(tb.Text, "20024867", "Error in lb0 SelectedItem");

            lbi = Util.FindVisualChild<ListBoxItem>(_lb1);
            tb = Util.FindControlByTypeInTemplate<TextBlock>(lbi);
            Util.AssertEquals(tb.Text, "Western Hemisphere", "Error in lb1 SelectedItem");

            lbi = Util.FindVisualChild<ListBoxItem>(_lb2);
            tb = Util.FindControlByTypeInTemplate<TextBlock>(lbi);
            Util.AssertEquals(tb.Text, "05/22/1990", "Error in lb2 SelectedItem");

            lbi = Util.FindVisualChild<ListBoxItem>(_lb3);
            tb = Util.FindControlByTypeInTemplate<TextBlock>(lbi);
            Util.AssertEquals(tb.Text, "Republic", "Error in lb3 SelectedItem");

            if (!string.IsNullOrEmpty(_tbNull.Text)) // Keep this
            {
                LogComment("Error in Binding null - Expected:NullOrEmpty.  Actual:" + _tbNull.Text.ToString());
                return TestResult.Fail;
            }

            BindingExpression b = _tbT.GetBindingExpression(TextBlock.ForegroundProperty);
            while (b.Status == BindingStatus.Unattached)
            { WaitFor(100); }
            Util.AssertEquals(b.Status, BindingStatus.UpdateTargetError, "Error in tbT - binding status");

            return TestResult.Pass;
        }

        /// <summary>
        /// Secnario 4 - Test update direction 
        ///     a. OneWay - S2T: done with other tests
        ///     b. TwoWay - when source updated, the converter/formatting re-applied. 
        ///     c. if no converter, T2S is failed w/ BindingStatus.UpdateSourceError
        /// </summary>
        /// <returns></returns>
        private TestResult TestUpdateDirection()
        {
            Status("TestUpdateDirection");
            // B TwoWay - when source updated, the converter/formatting re-applied. 

            Util.AssertEquals(_udTB1.Text, "Beatriz portuguese", "Error in udTB1.Text - initial");
            Util.AssertEquals(_udTB2.Text, "portuguese, Beatriz", "Error in udTB2.Text - initial");
            _udLB.SelectedIndex = 1;
            Util.AssertEquals(_udTB1.Text, "Radu romanian", "Error in udTB2.Text - selection changed");
            Util.AssertEquals(_udTB2.Text, "romanian, Radu", "Error in udTB2.Text - selection changed");

            // now update the S
            People src = (People)(_page.Resources["peoplelist"]);
            src.Add(new Person("Anne", "Chinese"));
            BindingExpression be = _udLB.GetBindingExpression(ListBox.ItemsSourceProperty);
            be.UpdateSource();
            WaitForPriority(DispatcherPriority.SystemIdle);
            _udLB.SelectedIndex = 10;
            Util.AssertEquals(_udTB1.Text, "Anne Chinese", "Error in udTB1.Text - updated S");
            Util.AssertEquals(_udTB2.Text, "Chinese, Anne", "Error in udTB2.Text - updated S");
            be = null;

            // onto T2S
            _mbtb7.Text = "Good Morning!";
            MultiBindingExpression mbe = BindingOperations.GetMultiBindingExpression(_mbtb7, TextBlock.TextProperty);
            mbe.UpdateSource();
            WaitForPriority(DispatcherPriority.SystemIdle);
            Util.AssertEquals(_mbtb5.Text, "Good", "Error in mbtb5.Text - T2S");
            Util.AssertEquals(_mbtb6.Text, "Morning!", "Error in mbtb6.Text - T2S");
            mbe = null;

            // if no converter, T2S is failed w/ BindingStatus.SourceUpdatedError
            _mbtb4.Text = "Good Morning!";
            mbe = BindingOperations.GetMultiBindingExpression(_mbtb4, TextBlock.TextProperty);
            mbe.UpdateSource();
            WaitForPriority(DispatcherPriority.SystemIdle);
            while (mbe.Status == BindingStatus.Unattached)
            { WaitFor(200); }
            if (!CheckBindingError(_mbtb4, TextBlock.TextProperty, BindingStatus.UpdateSourceError, true))
            {
                LogComment("T2S binding error in mbtb4");
                return TestResult.Fail;
            }
            mbe = null;

            // $10 tests
            Util.AssertEquals(_tBox.Text, "168.36", "Error in tBox.Text - initial");
            _cb10.SelectedIndex = 1;
            WaitForPriority(DispatcherPriority.SystemIdle);
            Util.AssertEquals(_tBox.Text, "$456.98", "Error in tBox.Text - selection updated");

            return TestResult.Pass;
        }

        /// <summary>
        /// Scenario 6 - test w/ Binding Culture specify in either 
        ///     a. binding.ConverterCulture or 
        ///     b. the target element's Lang property
        /// The tests can be combined with other scenarios, separate them for clarity and managability here.
        /// The tests are mainly for verifying the Culture info are still correct after the SF applied.  
        /// </summary>
        /// <returns></returns>
        private TestResult TestBindingCulture()
        {
            Status("TestBindingCulture");

            if (!_usTB.Text.EndsWith(CultureInfo.GetCultureInfoByIetfLanguageTag("en-US").DisplayName))
            {
                LogComment("TextBlock didn't pick up English Culture info.  Actual:  " + _usTB.Text);
                return TestResult.Fail;
            }

            if (!_trTB.Text.EndsWith(CultureInfo.GetCultureInfoByIetfLanguageTag("tr-TR").DisplayName))
            {
                LogComment("TextBlock didn't pick up Turkish Culture info.  Actual:  " + _trTB.Text);
                return TestResult.Fail;
            }

            TextBlock tb = Util.FindVisualByType(typeof(TextBlock), _button2) as TextBlock;
            if (tb == null)
            {
                LogComment("TextBlock in DataTemplate was null!!");
                return TestResult.Fail;
            }
            if (!tb.Text.EndsWith(CultureInfo.GetCultureInfoByIetfLanguageTag("en-US").DisplayName))
            {
                LogComment("TextBlock didn't pick up English Culture info.  Actual:  " + tb.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check the BindingStatus, here we're only interested in: 
        ///     UpdateTargetError and UpdateSourceError
        /// </summary>
        /// <param name="fe">The element to verify the bindingstatus against</param>
        /// <param name="dp">The DP to evaluate for</param>
        /// <param name="expected">The expected BindingStatus: UpdateTargetError and UpdateSourceError</param>
        /// <returns>true if as expected; false not</returns>
        private bool CheckBindingError(FrameworkElement fe, DependencyProperty dp, BindingStatus expected, bool isMulti)
        {
            BindingExpressionBase b;
            if (isMulti)
            {
                b = (MultiBindingExpression)BindingOperations.GetMultiBindingExpression(fe, dp);
            }
            else
            {
                b = (BindingExpression)fe.GetBindingExpression(dp);
            }

            while (b.Status == BindingStatus.Unattached)
            { WaitFor(100); }

            if (b.Status != expected)
            {
                LogComment("Binding Status Error - Expected:" + Enum.GetName(typeof(BindingStatus), expected) + "  Actual:" + b.Status.ToString());
                return false;
            }

            return true;
        }

        #endregion
    }
}