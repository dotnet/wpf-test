// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage - WPF - Validations just not working as expected.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ValidationOnMultipleInvalidSets", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class RegressionValidationOnMultipleConversions : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;

        #endregion

        #region Constructors

        public RegressionValidationOnMultipleConversions() : base(@"RegressionValidationOnMultipleConversions.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");

            if (_myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            _myStackPanel.DataContext = new DateDataContextClass();

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            VisibleValidationRule myValidationRule = (VisibleValidationRule) RootElement.FindName("myTextBoxValidationRule");
            TextBox myValidatedTextbox = (TextBox) RootElement.FindName("MultiValidationTextBox");
            WaitForPriority(DispatcherPriority.Render);

            if (!myValidationRule.InInvalidState)
            {
                LogComment("Validation rule passing in default state");
            }
            else
            {
                LogComment("Error, validation rule was showing FAIL before FAIL occurred.");
                return TestResult.Fail;
            }

            myValidatedTextbox.Text = "1/10001 12:00:00 AM";
            WaitForPriority(DispatcherPriority.Render);

            if (myValidationRule.InInvalidState)
            {
                LogComment("Good, Initial set to invalid value caused FAIL");
            }
            else
            {
                LogComment("Error, Initial set to invalid value caused ValidationRule to not be called");
                return TestResult.Fail;
            }

            myValidatedTextbox.Text = "1/10001 12:00:00 AM";
            WaitForPriority(DispatcherPriority.Render);

            if (myValidationRule.InInvalidState)
            {
                LogComment("Good, second set to invalid value caused FAIL");
            }
            else
            {
                LogComment("Error, second set to invalid value caused ValidationRule to not be called");
                return TestResult.Fail;
            }

            myValidatedTextbox.Text = "1/10001 12:00:00 AM";
            WaitForPriority(DispatcherPriority.Render);

            if (myValidationRule.InInvalidState)
            {
                LogComment("Good, third set to invalid value caused FAIL");
            }
            else
            {
                LogComment("Error, third set to invalid value caused ValidationRule to not be called");
                return TestResult.Fail;
            }
            // It evaluated correctly three times, now set it to a valid value for one last check.
            myValidatedTextbox.Text = "1/1/0001 12:00:00 AM";
            WaitForPriority(DispatcherPriority.Render);

            if (!myValidationRule.InInvalidState)
            {
                LogComment("Good, final set to a valid value caused ValidationRule's evaluation to be WIN");
            }
            else
            {
                LogComment("Error, final set to invalid value caused ValidationRule's evaluation to be fail");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
        #endregion
    }

    #region Helper Classes

    class VisibleValidationRule : ValidationRule
    {
        public bool InInvalidState = false;

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var theDateTime = value as String;

            DateTime theTime;
            if (DateTime.TryParse(theDateTime, out theTime))
            {
                InInvalidState = false;
                return ValidationResult.ValidResult;
            }
            else
            {
                InInvalidState = true;
                return new ValidationResult(false, "This is not valid");
            }
        }
    }

    class DateDataContextClass
    {
        private DateTime _testDate;

        public DateTime TestDate
        {
            get
            {
                return _testDate;
            }
            set
            {
                _testDate = value;
            }
        }
    }

    #endregion
}