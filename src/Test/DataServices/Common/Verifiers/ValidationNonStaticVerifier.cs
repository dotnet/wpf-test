// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;  // Validation
using System.Windows.Data;
using Microsoft.Test;
using System.Globalization;
using Microsoft.Test.Logging;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <area>UIBinding.Validation</area>

	/// <description>
	/// Used to test bindingExpression/multiBindingExpression.HasError and
    /// bindingExpression/multiBindingExpression.ValidationError
	/// </description>
	/// <spec>
	/// <name>Validation</name>
	/// </spec>
	/// </summary>
	public class ValidationNonStaticVerifier
	{
		private string _step;

		public string Step
		{
			get { return _step; }
			set { _step = value; }
		}

		public bool CheckHasError(BindingExpressionBase bindingExpression, bool expected)
		{
            GlobalLog.LogStatus("CheckHasError (BindingExpression) - " + Step);

			if (bindingExpression.HasError != expected)
			{
                GlobalLog.LogStatus("Fail - binding.HasError not as expected. Expected: " + expected.ToString());
				return false;
			}
			return true;
		}

		public bool CheckValidationError(BindingExpressionBase bindingExpression, ValidationError expectedVe)
		{
            GlobalLog.LogStatus("CheckValidationError (BindingExpression) - " + Step);

			ValidationError actualVe = bindingExpression.ValidationError;

			// validate BindingInError
			if (actualVe.BindingInError != expectedVe.BindingInError)
			{
                GlobalLog.LogStatus("Fail - ValidationError's BindingInError not as expected - " + Step);
				return false;
			}

			// don't validate ErrorContent - causes tests to fail in Japanese

			// validate expectedException - don't compare references because if the exception is thrown in the
			// setter, I have no way to get to it to pass it to this method - it gets wrapped by the
			// ValidationError before I can get to it
			if (actualVe.Exception != null && expectedVe.Exception != null)
			{
				if ((actualVe.Exception).GetType() != (expectedVe.Exception).GetType())
				{
                    GlobalLog.LogStatus("Fail - ValidationError's Exception does not have expected type - " + Step);
					return false;
				}
			}
			else
			{
				if (actualVe.Exception != null)
				{
                    GlobalLog.LogStatus("Fail - Actual Exception is null and expected isn't - " + Step);
					return false;
				}
				if (expectedVe.Exception != null)
				{
                    GlobalLog.LogStatus("Fail - Expected Exception is null and expected isn't - " + Step);
					return false;
				}
			}

            // validate rule in error
            if (actualVe.RuleInError.GetType() != expectedVe.RuleInError.GetType())
            {
                GlobalLog.LogStatus("Fail - ValidationError's RuleInError not as expected - " + Step);
                return false;
            }
            return true;
		}

	}
}
