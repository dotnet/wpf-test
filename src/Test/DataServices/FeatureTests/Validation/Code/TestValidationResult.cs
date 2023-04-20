// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// This test calls the Validate() method directly making the ValidationResult be
	/// valid and invalid depending on which value is passed to Validate().
	/// In real life, this method probably won't be called directly. When the user tries
	/// to update the data, the Validate() method is called automatically on each expectedRule
	/// until one is in error or all of them pass.
	/// </description>
    /// <relatedBugs>

    /// </relatedBugs>
	/// </summary>
    [Test(3, "Validation", "TestValidationResult")]
	public class TestValidationResult : XamlTest
	{
		private Player _player1;
		private TextBox _tbPrice1;

		public TestValidationResult() : base(@"TestValidationResult.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(TestValidationResultValid);
			RunSteps += new TestStep(TestValidationResultInvalid);
            RunSteps += new TestStep(TestOperators);
        }

		private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);
			_player1 = RootElement.Resources["player1"] as Player;
			_tbPrice1 = (TextBox)Util.FindElement(RootElement, "tbPrice1");

			if (_player1 == null)
			{
				LogComment("Unable to reference the Player data source.");
				return TestResult.Fail;
			}

			if (_tbPrice1 == null)
			{
				LogComment("Unable to reference tbPrice1 element.");
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

		private TestResult TestValidationResultValid()
		{
			Status("TestValidationResultValid");

			BindingExpression bindingExpression = _tbPrice1.GetBindingExpression(TextBox.TextProperty);
			Binding binding = bindingExpression.ParentBinding;
			IList<ValidationRule> ruleCollection = binding.ValidationRules;
			int numRules = 1;
			if (ruleCollection.Count != numRules)
			{
				LogComment("Fail - There should be " + numRules + " rules in the ValidationRuleCollection, instead there are " + ruleCollection.Count);
				return TestResult.Fail;
			}
			RangeRule rangeRule = ruleCollection[0] as RangeRule;
			if (rangeRule == null)
			{
				LogComment("Could not convert first ValidationRule to RangeRule");
				return TestResult.Fail;
			}
			ValidationResult result = rangeRule.Validate("5", CultureInfo.InvariantCulture);
            
            if (result == null)
            {
                LogComment("Fail - ValidationResult is null");
                return TestResult.Fail;
            }
            if (result.IsValid != true)
			{
				LogComment("Fail - ValidationResult should be valid");
				return TestResult.Fail;
			}
			if (result.ErrorContent != null)
			{
				LogComment("Fail - ValidationResult's error content should be null");
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

		private TestResult TestValidationResultInvalid()
		{
			Status("TestValidationResultInvalid");

			BindingExpression bindingExpression = _tbPrice1.GetBindingExpression(TextBox.TextProperty);
			Binding binding = bindingExpression.ParentBinding;
			IList<ValidationRule> ruleCollection = binding.ValidationRules;
			int numRules = 1;
			if (ruleCollection.Count != numRules)
			{
				LogComment("Fail - There should be " + numRules + " rules in the ValidationRuleCollection, instead there are " + ruleCollection.Count);
				return TestResult.Fail;
			}
			RangeRule rangeRule = ruleCollection[0] as RangeRule;
			if (rangeRule == null)
			{
				LogComment("Could not convert first ValidationRule to RangeRule");
				return TestResult.Fail;
			}
			ValidationResult result = rangeRule.Validate("15", CultureInfo.InvariantCulture);
            
            if (result == null)
            {
                LogComment("Fail - ValidationResult is null");
                return TestResult.Fail;
            }
            if (result.IsValid != false)
			{
				LogComment("Fail - ValidationResult should be invalid");
				return TestResult.Fail;
			}
			if ((string)(result.ErrorContent) != rangeRule.ErrorContent)
			{
				LogComment("Fail - ValidationResult's error content should be '" + rangeRule.ErrorContent +
					"', instead it is '" + (string)(result.ErrorContent) + "'");
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

        // provides coverage for the equality and inequality operators of ValidationResult
        private TestResult TestOperators()
        {
            Status("TestOperators");

            ValidationResult vrOriginal = new ValidationResult(true, "Original error content");
            ValidationResult vrByVal = new ValidationResult(vrOriginal.IsValid, vrOriginal.ErrorContent);
            ValidationResult vrByRef = vrOriginal;
            ValidationResult vrDifferent = ValidationResult.ValidResult;
            ValidationResult vrNull = null;

            // == operator
            
            if (vrOriginal == null)
            {
                LogComment("Fail - vrOriginal should not be null");
                return TestResult.Fail; ;
            }
            if (vrNull == null)
            {
                // do nothing
            }
            else
            {
                LogComment("Fail - vrNull should be null");
                return TestResult.Fail;
            }

            if (vrOriginal == vrDifferent)
            {
                LogComment("Fail - vrOriginal and vrDifferent should be different");
                return TestResult.Fail;
            }

            if (vrOriginal == vrByVal)
            {
                // do nothing
            }
            else
            {
                LogComment("Fail - vrOriginal and vrByVal should be the equal");
                return TestResult.Fail;
            }

            // != operator
            
            if (vrOriginal != null)
            {
                // do nothing
            }
            else
            {
                LogComment("Fail - vrOriginal should not be null");
                return TestResult.Fail;
            }

            if (vrNull != null)
            {
                LogComment("Fail - vrNull should be null");
                return TestResult.Fail;
            }

            if (vrOriginal != vrByVal)
            {
                LogComment("Fail - vrOriginal and vrByVal should be equal");
                return TestResult.Fail;
            }

            if (vrOriginal != vrDifferent)
            {
                // do nothing
            }
            else
            {
                LogComment("Fail - vrOriginal and vrDifferent should be different");
                return TestResult.Fail;
            }

            // Equals
            
            if (vrOriginal.Equals(null))
            {
                LogComment("Fail - vrOriginal should not be null");
                return TestResult.Fail;
            }

            if (vrOriginal.Equals(vrByVal))
            {
                // do nothing
            }
            else
            {
                LogComment("Fail - vrOriginal and vrByVal should be the same");
                return TestResult.Fail;
            }

            if (vrOriginal.Equals(vrByRef))
            {
                // do nothing
            }
            else
            {
                LogComment("Fail - vrOriginal and vrByRef should be the same");
                return TestResult.Fail;
            }

            int hashCode = vrOriginal.GetHashCode();

            return TestResult.Pass;
        }
    }
}
