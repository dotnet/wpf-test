// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// This tests the scenario where there is a MultiBinding
	/// that has several Binds with ValidationErrors.
	/// </description>
	/// <relatedBugs>



    /// </relatedBugs>
	/// </summary>
    [Test(1, "Validation", "MultiBindingSeveralValRules")]
	public class MultiBindingSeveralValRules : XamlTest
	{
		private TextBox _tb;
		private MultiBindingExpression _multiBindingExpression;
		private ValidationStaticVerifier _staticVerifier;
		private ValidationNonStaticVerifier _nonStaticVerifier;

		public MultiBindingSeveralValRules() : base(@"MultiBindingSeveralValRules.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			// MultiBindingExpression not in error, all Bindings not in error
			RunSteps += new TestStep(MultiBindingNoErrorBindingNoError);
            // MultiBindingExpression not in error, at least one BindingExpression in error
            RunSteps += new TestStep(MultiBindingNoErrorBindingError);
            // MultiBindingExpression in error, all Bindings not in error
            RunSteps += new TestStep(MultiBindingErrorBindingNoError);
            // MultiBindingExpression in error, at least one BindingExpression in error
            RunSteps += new TestStep(MultiBindingErrorBindingError);
		}

		private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);
			// tb
			_tb = Util.FindElement(RootElement, "tb") as TextBox;
			if (_tb == null)
			{
				LogComment("Fail - Unable to reference the TextBox tb");
				return TestResult.Fail;
			}
			_multiBindingExpression = BindingOperations.GetMultiBindingExpression(_tb, TextBox.TextProperty);
			if (_multiBindingExpression == null)
			{
				LogComment("Fail - MultiBindingExpression is null (tb)");
				return TestResult.Fail;
			}

			_staticVerifier = new ValidationStaticVerifier(_tb);
			_nonStaticVerifier = new ValidationNonStaticVerifier();

			return TestResult.Pass;
		}

		// MultiBindingExpression not in error, all Bindings not in error
		private TestResult MultiBindingNoErrorBindingNoError()
		{
            Status("MultiBindingNoErrorBindingNoError");

			_tb.Text = "aaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaa 4"; // no validation errors
			_multiBindingExpression.UpdateSource();

			// element
			_staticVerifier.Step = "MultiBindingNoErrorBindNoError - element";
			if (!_staticVerifier.CheckHasError(false)) { return TestResult.Fail; }
			if (!_staticVerifier.CheckNumErrors(0)) { return TestResult.Fail; }

			// property
			_nonStaticVerifier.Step = "MultiBindingNoErrorBindNoError - property";
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, false)) { return TestResult.Fail; }

			// inner bindings
			_nonStaticVerifier.Step = "MultiBindingNoErrorBindNoError - binding";
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], false)) { return TestResult.Fail; }
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], false)) { return TestResult.Fail; }
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], false)) { return TestResult.Fail; }

			return TestResult.Pass;
		}

		// MultiBindingExpression not in error, at least one BindingExpression in error
		private TestResult MultiBindingNoErrorBindingError()
		{
            Status("MultiBindingNoErrorBindingError");

			// the third bind is in error (due to both rules), all other binds and multibind are not
			_tb.Text = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa abcdef 11";
			_multiBindingExpression.UpdateSource();

			// element
			// notice that although both rules of the third bind are in error, only the first one makes it to the error collection
			_staticVerifier.Step = "MultiBindingNoErrorBindError - element";
			if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
#if TESTBUILD_CLR40
            Status("Expecting two errors since the fix for regression issue is present");
			if (!staticVerifier.CheckNumErrors(2)) { return TestResult.Fail; }
#endif
#if TESTBUILD_CLR20
            Status("Expecting only one error since the fix for regression issue is NOT present (pre-Dev10 build)");
			if (!staticVerifier.CheckNumErrors(1)) { return TestResult.Fail; }
#endif
            BindingExpression binding2 = (BindingExpression)_multiBindingExpression.BindingExpressions[2];
			Binding bind2 = binding2.ParentBinding;
			int count = bind2.ValidationRules.Count;			
			RangeRule rr = bind2.ValidationRules[0] as RangeRule;
			ValidationError ve1 = new ValidationError(rr, binding2, rr.ErrorContent, null);
            if (!_staticVerifier.CheckValidationError(0, ve1)) { return TestResult.Fail; }

			// property
			_nonStaticVerifier.Step = "MultiBindingNoErrorBindError - property";
			// 
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, true)) { return TestResult.Fail; }            

            // inner bindings
			_nonStaticVerifier.Step = "MultiBindingNoErrorBindError - binding";
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], false)) { return TestResult.Fail; }
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], false)) { return TestResult.Fail; }
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], true)) { return TestResult.Fail; }
			if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[2], ve1)) { return TestResult.Fail; }

			ResetValidation();

			return TestResult.Pass;
		}

		// MultiBindingExpression in error, all Bindings not in error
		private TestResult MultiBindingErrorBindingNoError()
		{
            Status("MultiBindingErrorBindingNoError");

			_tb.Text = "abcdef abcdef 5"; // all bindings are valid, multibinding is invalid
			_multiBindingExpression.UpdateSource();

			// element
			_staticVerifier.Step = "MultiBindingErrorBindNoError - element";
			if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
			if (!_staticVerifier.CheckNumErrors(1)) { return TestResult.Fail; }
			MultiBinding multiBinding = _multiBindingExpression.ParentMultiBinding;
			MinCharsRule multiBindingRule = multiBinding.ValidationRules[0] as MinCharsRule;
			ValidationError expectedError = new ValidationError(multiBindingRule, _multiBindingExpression, multiBindingRule.ErrorContent, null);
			if (!_staticVerifier.CheckValidationError(0, expectedError)) { return TestResult.Fail; }

			// property
            _nonStaticVerifier.Step = "MultiBindingErrorBindNoError - property";
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, true)) { return TestResult.Fail; }
			if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression, expectedError)) { return TestResult.Fail; }

			// inner binding
            _nonStaticVerifier.Step = "MultiBindingErrorBindNoError - binding";
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], false)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], false)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], false)) { return TestResult.Fail; }

			ResetValidation();

			return TestResult.Pass;
		}

		// MultiBindingExpression in error, at least one BindingExpression in error
		private TestResult MultiBindingErrorBindingError()
		{
            Status("MultiBindingErrorBindingError");

			_tb.Text = "B C 23"; // invalid for 5 reasons - 4 rules in the binds, 1 in the multibind
			_multiBindingExpression.UpdateSource();

			// only the error in the multibind makes it to the error collection because if we can't pass the
			// conversion of the multibind, there's no point on checking the binds

			// element
            _staticVerifier.Step = "MultiBindingErrorBindingError - element";
			if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
			if (!_staticVerifier.CheckNumErrors(1)) { return TestResult.Fail; }
			MultiBinding multiBinding = _multiBindingExpression.ParentMultiBinding;
			MinCharsRule multiBindingRule = multiBinding.ValidationRules[0] as MinCharsRule;
			ValidationError expectedError = new ValidationError(multiBindingRule, _multiBindingExpression, multiBindingRule.ErrorContent, null);
			if (!_staticVerifier.CheckValidationError(0, expectedError)) { return TestResult.Fail; }

			// property
			_nonStaticVerifier = new ValidationNonStaticVerifier();
            _nonStaticVerifier.Step = "MultiBindingErrorBindingError - property";
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, true)) { return TestResult.Fail; }
			if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression, expectedError)) { return TestResult.Fail; }

			// inner binding
            _nonStaticVerifier.Step = "MultiBindingErrorBindingError - binding";
			if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], false)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], false)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], false)) { return TestResult.Fail; }

			ResetValidation();

			return TestResult.Pass;
		}

		private void ResetValidation()
		{
			_tb.Text = "aaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaa 4";
			_multiBindingExpression.UpdateSource();
		}
	}
}

