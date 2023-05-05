// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

/*
Element = TextBox
Property = Text -> Rule = RangeRule
Property = FontSize -> Rule = EvenRule
*/

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests the situation where an element has 2 properties and each of them has a 
	/// rule. In this situation, if they are both invalid, both errors make it 
	/// to the attached property Validation.ValidationErrorsProperty.
	/// </description>
	/// <relatedBugs>


	/// </relatedBugs>
	/// </summary>
    [Test(0, "Validation", "TwoPropsOneRuleEach")]
	public class TwoPropsOneRuleEach : XamlTest
	{

		private XmlDataProvider _dso;
		private TextBox _tbPrice1;
        private TextBlock _countTB;
		private ValidationError _rangeError;
		private ValidationError _evenError;
		private ValidationStaticVerifier _staticVerifier;
		private ValidationNonStaticVerifier _nonStaticVerifier;
		private BindingExpression _bindingExpressionText;
		private BindingExpression _bindingExpressionMaxLength;

		public TwoPropsOneRuleEach() : base(@"TwoPropsOneRuleEach.xaml")
		{
			InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TextInvalidMaxLengthInvalid);
            RunSteps += new TestStep(TextValidMaxLengthInvalid);
            RunSteps += new TestStep(TextValidMaxLengthValid);
		}

		private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);
			_dso = RootElement.Resources["dso"] as XmlDataProvider;
			_tbPrice1 = (TextBox)Util.FindElement(RootElement, "tbPrice1");
            _countTB = (TextBlock)Util.FindElement(RootElement, "countTB");

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

            if (_countTB == null)
            {
                LogComment("Unable to reference countTB element.");
                return TestResult.Fail;
            }
            
            _bindingExpressionText = _tbPrice1.GetBindingExpression(TextBox.TextProperty);
			_bindingExpressionMaxLength = _tbPrice1.GetBindingExpression(TextBox.MaxLengthProperty);

			_staticVerifier = new ValidationStaticVerifier(_tbPrice1);
			_nonStaticVerifier = new ValidationNonStaticVerifier();

			return TestResult.Pass;
		}

		private TestResult TextInvalidMaxLengthInvalid()
		{
            Status("TextInvalidMaxLengthInvalid");

			if (!MarkTextInvalid()) { return TestResult.Fail; }
			if (!MarkMaxLengthInvalid()) { return TestResult.Fail; }

			// element level 
            _staticVerifier.Step = "TextInvalidMaxLengthInvalid - element";
			if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
			if (!_staticVerifier.CheckNumErrors(2)) { return TestResult.Fail; }
			if (!_staticVerifier.CheckValidationError(0, _rangeError)) { return TestResult.Fail; }
			if (!_staticVerifier.CheckValidationError(1, _evenError)) { return TestResult.Fail; }

			// property level - TextProperty
            _nonStaticVerifier.Step = "TextInvalidMaxLengthInvalid - TextProperty";
			if (!_nonStaticVerifier.CheckHasError(_bindingExpressionText, true)) { return TestResult.Fail; }
			if (!_nonStaticVerifier.CheckValidationError(_bindingExpressionText, _rangeError)) { return TestResult.Fail; }

			// property level - FontSizeProperty
            _nonStaticVerifier.Step = "TextInvalidMaxLengthInvalid - FontSizeProperty";
			if (!_nonStaticVerifier.CheckHasError(_bindingExpressionMaxLength, true)) { return TestResult.Fail; }
			if (!_nonStaticVerifier.CheckValidationError(_bindingExpressionMaxLength, _evenError)) { return TestResult.Fail; }

            if (!VerifyBoundCount(2)) { return TestResult.Fail; }

			return TestResult.Pass;
		}

        private TestResult TextValidMaxLengthInvalid()
		{
            Status("TextValidMaxLengthInvalid");

			if (!ClearInvalidText()) { return TestResult.Fail; }

			// element level 
            _staticVerifier.Step = "TextValidMaxLengthInvalid - element";
			if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
			if (!_staticVerifier.CheckNumErrors(1)) { return TestResult.Fail; }
			if(!_staticVerifier.CheckValidationError(0, _evenError)) { return TestResult.Fail; }

			// property level - TextProperty
            _nonStaticVerifier.Step = "TextValidMaxLengthInvalid - TextProperty";
			if (!_nonStaticVerifier.CheckHasError(_bindingExpressionText, false)) { return TestResult.Fail; }
			// property level - FontSizeProperty
            _nonStaticVerifier.Step = "TextValidMaxLengthInvalid - FontSizeProperty";
			if (!_nonStaticVerifier.CheckHasError(_bindingExpressionMaxLength, true)) { return TestResult.Fail; }
			if (!_nonStaticVerifier.CheckValidationError(_bindingExpressionMaxLength, _evenError)) { return TestResult.Fail; }

            if (!VerifyBoundCount(1)) { return TestResult.Fail; }

			return TestResult.Pass;
		}

        private TestResult TextValidMaxLengthValid()
		{
            Status("TextValidMaxLengthValid");

			if (!ClearInvalidMaxLength()) { return TestResult.Fail; }

			// element level 
            _staticVerifier.Step = "TextValidMaxLengthValid - element";
			if (!_staticVerifier.CheckHasError(false)) { return TestResult.Fail; }
			if (!_staticVerifier.CheckNumErrors(0)) { return TestResult.Fail; }

			// property level - TextProperty
            _nonStaticVerifier.Step = "TextValidMaxLengthValid - TextProperty";
			if (!_nonStaticVerifier.CheckHasError(_bindingExpressionText, false)) { return TestResult.Fail; }
			// property level - FontSizeProperty
            _nonStaticVerifier.Step = "TextValidMaxLengthValid - FontSizeProperty";
			if (!_nonStaticVerifier.CheckHasError(_bindingExpressionMaxLength, false)) { return TestResult.Fail; }

            if (!VerifyBoundCount(0)) { return TestResult.Fail; }

			return TestResult.Pass;
		}

		#region AuxMethods
		// mark the text property as invalid
		private bool MarkTextInvalid()
		{
			Status("MarkTextInvalid");

			Binding bindingText = _bindingExpressionText.ParentBinding;
            RangeRule rangeRuleText = bindingText.ValidationRules[0] as RangeRule;
			Exception exc = new Exception("Message - Range Error");
            _rangeError = new ValidationError(rangeRuleText, _bindingExpressionText, rangeRuleText.ErrorContent, exc);
			Validation.MarkInvalid(_bindingExpressionText, _rangeError);
			return true;
		}

		// mark the max length property as invalid
        private bool MarkMaxLengthInvalid()
		{
            Status("MarkMaxLengthInvalid");

			Binding bindingMaxLength = _bindingExpressionMaxLength.ParentBinding;
            EvenRule evenRuleMaxLength = bindingMaxLength.ValidationRules[0] as EvenRule;
			Exception exc = new NotSupportedException("Message - Even Error");
            _evenError = new ValidationError(evenRuleMaxLength, _bindingExpressionMaxLength, evenRuleMaxLength.ErrorContent, exc);
			Validation.MarkInvalid(_bindingExpressionMaxLength, _evenError);
			return true;
		}

		private bool ClearInvalidText()
		{
			Status("ClearInvalidText");
			BindingExpression bindingExpressionText = _tbPrice1.GetBindingExpression(TextBox.TextProperty);
			Validation.ClearInvalid(bindingExpressionText);
			return true;
		}

		private bool ClearInvalidMaxLength()
		{
            Status("ClearInvalidMaxLength");
			BindingExpression bindingExpressionMaxLength = _tbPrice1.GetBindingExpression(TextBox.MaxLengthProperty);
			Validation.ClearInvalid(bindingExpressionMaxLength);
			return true;
		}

        
        private bool VerifyBoundCount(int expectedCount)
        {
            Status("VerifyBoundCount");
            if (Int32.Parse(_countTB.Text) != expectedCount)
            {
                LogComment("Fail - Bound count has value " + _countTB.Text + ". Expected: " + expectedCount);
                return false;
            }
            return true;
        }
		#endregion
	}
}

