// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests the situation where one element+property has 2 rules. In this situation,
    /// if they are both invalid, only one of the errors makes it to the attached property
    /// Validation.ValidationErrorsProperty.
    /// It also verifies that passing null to GetIsValid and GetValidationErrors throws exception.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(0, "Validation", "OnePropTwoRules")]
    public class OnePropTwoRules : XamlTest
    {
        private XmlDataProvider _dso;
        private TextBox _tbPrice1;
        private TextBlock _txtBound;
        private RangeRule _rangeRule;
        private EvenRule _evenRule;
        private ValidationStaticVerifier _staticVerifier;
        private ValidationNonStaticVerifier _nonStaticVerifier;
        private TextBlock _tb1;
        private TextBlock _tb2;
        private string _rangeInvalidEvenInvalid = "11";
        private string _rangeValidEvenInvalid = "5";
        private string _rangeValidEvenValid = "6";

        public OnePropTwoRules()
            : base(@"OnePropTwoRules.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RangeInvalidEvenInvalid);
            RunSteps += new TestStep(RangeValidEvenInvalid);
            RunSteps += new TestStep(RangeValidEvenValid);
            RunSteps += new TestStep(TestGetHasErrorException);
            RunSteps += new TestStep(TestGetErrorsException);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);
            _dso = RootElement.Resources["dso"] as XmlDataProvider;
            _tbPrice1 = (TextBox)Util.FindElement(RootElement, "tbPrice1");
            _txtBound = (TextBlock)Util.FindElement(RootElement, "txtBound");
            _tb1 = (TextBlock)Util.FindElement(RootElement, "tb1");
            _tb2 = (TextBlock)Util.FindElement(RootElement, "tb2");

            if (_dso == null)
            {
                LogComment("Unable to reference the XmlDataSource.");
                return TestResult.Fail;
            }

            if (_tbPrice1 == null)
            {
                LogComment("Unable to reference tbPrice1 element.");
                return TestResult.Fail;
            }

            if (_txtBound == null)
            {
                LogComment("Unable to reference txtBound element.");
                return TestResult.Fail;
            }

            if (_tb1 == null)
            {
                LogComment("Unable to reference tb1 element.");
                return TestResult.Fail;
            }

            if (_tb2 == null)
            {
                LogComment("Unable to reference tb2 element.");
                return TestResult.Fail;
            }

            _staticVerifier = new ValidationStaticVerifier(_tbPrice1);
            _nonStaticVerifier = new ValidationNonStaticVerifier();

            // get validation rules
            BindingExpression bindingExpression = _tbPrice1.GetBindingExpression(TextBox.TextProperty);
            Binding binding = bindingExpression.ParentBinding;
            Collection<ValidationRule> ruleCollection = binding.ValidationRules;
            if (ruleCollection.Count != 2)
            {
                LogComment("Fail - There should be 2 validation rules in the collection, instead there are " +
                    ruleCollection.Count);
                return TestResult.Fail;
            }
            _rangeRule = ruleCollection[0] as RangeRule;
            _evenRule = ruleCollection[1] as EvenRule;

            return TestResult.Pass;
        }

        private TestResult RangeInvalidEvenInvalid()
        {
            Status("RangeInvalidEvenInvalid");
            _staticVerifier.Step = "RangeInvalidEvenInvalid";

            WaitForPriority(DispatcherPriority.SystemIdle);

            _tbPrice1.Text = _rangeInvalidEvenInvalid;

            // element level - only the first rule makes it to the error collection
            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && (_tbPrice1.GetValue(TextBox.TextProperty).ToString() != _rangeInvalidEvenInvalid))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }
            BindingExpression bindingExpressionText = _tbPrice1.GetBindingExpression(TextBox.TextProperty);
            Thread.Sleep(1000);
            WaitForPriority(DispatcherPriority.SystemIdle);

            bindingExpressionText.UpdateSource();
            Thread.Sleep(1000);
            WaitForPriority(DispatcherPriority.SystemIdle);

            // element level - only the first rule makes it to the error collection
            // Needed for slower machines (like VMs with low memory)
            retryCount = 0;
            while (retryCount < 5 && !_staticVerifier.CheckNumErrors(1))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }

            if (!_staticVerifier.CheckNumErrors(1)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            ValidationError veExpected = new ValidationError(_rangeRule, bindingExpressionText, _rangeRule.ErrorContent, null);
            if (!_staticVerifier.CheckValidationError(0, veExpected)) { return TestResult.Fail; }
            // property level
            _nonStaticVerifier.Step = "RangeInvalidEvenInvalid";
            if (!_nonStaticVerifier.CheckHasError(bindingExpressionText, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(bindingExpressionText, veExpected)) { return TestResult.Fail; }

            if (_txtBound.Text == _rangeInvalidEvenInvalid)
            {
                LogComment("Fail - TextBox is invalid and DataSource was updated");
                return TestResult.Fail;
            }

            // Validation.HasError
            if (_tb1.Text != "True")
            {
                LogComment("Fail - tb1 should be true because the text box has error. Actual: " + _tb1.Text);
                return TestResult.Fail;
            }
            // (Validation.Errors)[0].ErrorContent
            if (_tb2.Text != "must lie between 0 and 10")
            {
                LogComment("Fail - tb2 does not have the correct error message. Actual: " + _tb2.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult RangeValidEvenInvalid()
        {
            Status("RangeValidEvenInvalid");

            _tbPrice1.Text = _rangeValidEvenInvalid;

            BindingExpression bindingExpressionText = _tbPrice1.GetBindingExpression(TextBox.TextProperty);
            bindingExpressionText.UpdateSource();

            WaitForPriority(DispatcherPriority.SystemIdle);

            _staticVerifier.Step = "RangeValidEvenInvalid";
            // element level
            if (!_staticVerifier.CheckNumErrors(1)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            ValidationError veExpected = new ValidationError(_evenRule, bindingExpressionText, _evenRule.ErrorContent, null);
            if (!_staticVerifier.CheckValidationError(0, veExpected)) { return TestResult.Fail; }
            // property level
            _nonStaticVerifier.Step = "RangeValidEvenInvalid";
            if (!_nonStaticVerifier.CheckHasError(bindingExpressionText, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(bindingExpressionText, veExpected)) { return TestResult.Fail; }

            if (_txtBound.Text == _rangeValidEvenInvalid)
            {
                LogComment("Fail - TextBox is invalid and DataSource was updated");
                return TestResult.Fail;
            }

            // Validation.HasError
            if (_tb1.Text != "True")
            {
                LogComment("Fail - tb1 should be true because the text box has error. Actual: " + _tb1.Text);
                return TestResult.Fail;
            }
            // (Validation.Errors)[0].ErrorContent
            if (_tb2.Text != "must be even")
            {
                LogComment("Fail - tb2 does not have the correct error message. Actual: " + _tb2.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult RangeValidEvenValid()
        {
            Status("RangeValidEvenValid");

            _tbPrice1.Text = _rangeValidEvenValid;

            BindingExpression bindingExpressionText = _tbPrice1.GetBindingExpression(TextBox.TextProperty);
            bindingExpressionText.UpdateSource();

            WaitForPriority(DispatcherPriority.SystemIdle);

            _staticVerifier.Step = "RangeValidEvenValid";
            // element level
            if (!_staticVerifier.CheckNumErrors(0)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckHasError(false)) { return TestResult.Fail; }
            // property level
            _nonStaticVerifier.Step = "RangeValidEvenValid";
            if (!_nonStaticVerifier.CheckHasError(bindingExpressionText, false)) { return TestResult.Fail; }

            if (_txtBound.Text != _rangeValidEvenValid)
            {
                LogComment("Fail - TextBox is valid but DataSource was not updated");
                return TestResult.Fail;
            }

            // Validation.HasError
            if (_tb1.Text != "False")
            {
                LogComment("Fail - tb1 should be false because the text box has no error. Actual: " + _tb1.Text);
                return TestResult.Fail;
            }
            // (Validation.Errors)[0].ErrorContent
            if (_tb2.Text != "")
            {
                LogComment("Fail - tb2 should have no error message. Actual: " + _tb2.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestGetHasErrorException()
        {
            Status("TestGetHasErrorException");

            // verify ArgumentNullException in GetHasError
            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            bool hasError1 = (bool)Validation.GetHasError(null);

            return TestResult.Pass;
        }

        private TestResult TestGetErrorsException()
        {
            Status("TestGetErrorsException");

            // verify ArgumentNullException in GetErrors
            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            ReadOnlyCollection<ValidationError> vec1 = Validation.GetErrors(null);

            return TestResult.Pass;
        }
    }
}

