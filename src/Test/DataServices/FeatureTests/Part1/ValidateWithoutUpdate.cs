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
using System.Collections.Generic;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Test coverage for BindingExpressionBase.ValidateWithoutUpdate()
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Validation", "ValidateWithoutUpdate", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
    public class ValidateWithoutUpdate : XamlTest
    {
        #region Private Data
        
        StackPanel _stackPanel = new StackPanel();
        TextBox _textBox = new TextBox();
        private StackPanel _myStackPanel;        
        private TextBox _multiTextBox;
        private TextBox _priorityTextBox;

        GenericValidationRule _rawProposedValueStep = new GenericValidationRule(ValidationStep.RawProposedValue, true);
        GenericValidationRule _convertedProposedValueStep = new GenericValidationRule(ValidationStep.ConvertedProposedValue, true);
        
        #endregion

        #region Constructors

        public ValidateWithoutUpdate()
            : base(@"ValidateWithoutUpdate.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(BindingScenario);
            RunSteps += new TestStep(MultiBindingScenario);
            RunSteps += new TestStep(PriorityBindingScenario);                   
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            Place redmond = new Place("Redmond", "WA");
            _stackPanel.DataContext = redmond;

            // Add textbox to the tree            
            _stackPanel.Children.Add(_textBox);

            _myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");
            _multiTextBox = (TextBox)RootElement.FindName("multiTextBox");
            _priorityTextBox = (TextBox)RootElement.FindName("priorityTextBox");


            if (_myStackPanel == null || _multiTextBox == null || _priorityTextBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult BindingScenario()
        {
			WaitForPriority(DispatcherPriority.Render);

            BindingExpressionBase bindingBase;   
         
            // Changing the pass/fail states of the validation steps: RawProposedValue, ConvertedProposedValue
            
            bindingBase = InitValidationBinding(_textBox, true, true);            
            if (!VerifyBinding(bindingBase, true, 2, "NewValue")) return TestResult.Fail;

            bindingBase = InitValidationBinding(_textBox, false, true);
            if (!VerifyBinding(bindingBase, false, 1, "NewValue")) return TestResult.Fail;
            
            bindingBase = InitValidationBinding(_textBox, false, false);
            if (!VerifyBinding(bindingBase, false, 1, "NewValue")) return TestResult.Fail;

            bindingBase = InitValidationBinding(_textBox, true, false);
            if (!VerifyBinding(bindingBase, false, 2, "NewValue")) return TestResult.Fail;

            

            return TestResult.Pass;
        }

        private TestResult MultiBindingScenario()
        {
            WaitForPriority(DispatcherPriority.Render);


            BindingExpressionBase beb = BindingOperations.GetBindingExpressionBase(_multiTextBox, TextBox.TextProperty);

            // First Valid, Second Invalid
            _multiTextBox.Text = "Redmond WART";
            if (beb.ValidateWithoutUpdate() != false)
            {
                LogComment("ValidateWithoutUpdate expected to return false, actual " + beb.ValidateWithoutUpdate());
                return TestResult.Fail;
            }

            // First Invalid, Second valid
            _multiTextBox.Text = "R WA";
            if (beb.ValidateWithoutUpdate() != false)
            {
                LogComment("ValidateWithoutUpdate expected to return false, actual " + beb.ValidateWithoutUpdate());
                return TestResult.Fail;
            }

            // Both Valid
            _multiTextBox.Text = "Redmond WA";            
            if (beb.ValidateWithoutUpdate() != true)
            {                
                LogComment("ValidateWithoutUpdate expected to return true, actual " + beb.ValidateWithoutUpdate());
                return TestResult.Fail;
            }

            // Both Invalid
            _multiTextBox.Text = "R WART";
            if (beb.ValidateWithoutUpdate() != false)
            {
                LogComment("ValidateWithoutUpdate expected to return false, actual " + beb.ValidateWithoutUpdate());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult PriorityBindingScenario()
        {
            WaitForPriority(DispatcherPriority.Render);

            BindingExpressionBase beb = BindingOperations.GetBindingExpressionBase(_priorityTextBox, TextBox.TextProperty);
            
            // First Valid, Second Invalid
            _priorityTextBox.Text = "123456789";
            if (beb.ValidateWithoutUpdate() != true)
            {
                LogComment("ValidateWithoutUpdate expected to return true, actual " + beb.ValidateWithoutUpdate());
                return TestResult.Fail;
            }

            // First Invalid, Second valid
            _priorityTextBox.Text = "12";
            if (beb.ValidateWithoutUpdate() != false)
            {
                LogComment("ValidateWithoutUpdate expected to return true, actual " + beb.ValidateWithoutUpdate());
                return TestResult.Fail;
            }

            // Both Valid
            _priorityTextBox.Text = "12345";
            if (beb.ValidateWithoutUpdate() != true)
            {                
                LogComment("ValidateWithoutUpdate expected to return true, actual " + beb.ValidateWithoutUpdate());
                return TestResult.Fail;
            }

            // Both Invalid
            _priorityTextBox.Text = "";
            if (beb.ValidateWithoutUpdate() != false)
            {
                LogComment("ValidateWithoutUpdate expected to return false, actual " + beb.ValidateWithoutUpdate());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }        

        private bool VerifyBinding(BindingExpressionBase bindingBase, bool expectedVal, int expectedCount, string expectedText)
        {
            if (bindingBase.ValidateWithoutUpdate() != expectedVal || 
                GenericValidationRule.Results.Count != expectedCount || 
                _textBox.Text != expectedText)
            {                
                LogComment("Expected ValidateWithoutUpdate() = " + expectedVal + " Actual: " + bindingBase.ValidateWithoutUpdate().ToString());
                LogComment("Expected GenericValidationRule.Results.Count = " + expectedCount + " Actual: " + GenericValidationRule.Results.Count.ToString());
                LogComment("Expected textBox.Text = " + expectedText + " Actual: " + _textBox.Text);

                return false;
            }

            GenericValidationRule.Results.Clear();
            return true;
        }


        private BindingExpressionBase InitValidationBinding(TextBox textBox, bool rawPasses, bool convertedPasses)
        {
            Binding binding = new Binding("Name");

            binding.ValidationRules.Add(_rawProposedValueStep);
            binding.ValidationRules.Add(_convertedProposedValueStep);
            
            _rawProposedValueStep.Passes = rawPasses;
            _convertedProposedValueStep.Passes = convertedPasses;

            BindingExpressionBase bindingBase = textBox.SetBinding(TextBox.TextProperty, binding);
            textBox.Text = "NewValue";

            return bindingBase;
        }        

        #endregion

        #region Helper Classes

        

        /// <summary>
        /// A GenericValidationRule to easily define the ValidationStep and presents a 'flight recorder' of if it was called and with what params
        /// </summary>
        private class GenericValidationRule : ValidationRule
        {
            public struct GenericValidationResult
            {
                public ValidationStep ValidationStep;
                public bool Passes;
                public object Value;

                public GenericValidationResult(ValidationStep validationStep, bool passes, object value)
                {
                    ValidationStep = validationStep;
                    Passes = passes;
                    Value = value;
                }
            }

            public static List<GenericValidationRule> Results = new List<GenericValidationRule>();

            private ValidationStep _step;
            private bool _passes;
            private object _passedValue;

            public GenericValidationRule()
            {
                Step = this.ValidationStep;
                Passes = true;
            }

            public GenericValidationRule(ValidationStep step, bool passes)
            {
                Step = step;
                Passes = passes;

                ValidationStep = step;
            }

            public ValidationStep Step
            {
                get
                {
                    return _step;
                }
                set
                {
                    _step = value;
                }
            }

            public bool Passes
            {
                get
                {
                    return _passes;
                }
                set
                {
                    _passes = value;
                }
            }

            public object Value
            {
                get
                {
                    return _passedValue;
                }
                set
                {
                    _passedValue = value;
                }
            }

            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                Value = value;
                Results.Add(this);
                return new ValidationResult(Passes, null);
            }
        }

        #endregion
		
    }

    public class NamesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return DependencyProperty.UnsetValue;

            if (targetType != typeof(String))
                return DependencyProperty.UnsetValue;

            return (values[0] as String) + " " + (values[1] as String);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string str = value as string;
            if (str == null)
                return null;

            if (str == "FailConvert")
            {
                return new object[] { DependencyProperty.UnsetValue, DependencyProperty.UnsetValue };
            }

            String[] values = str.Split(null);

            object[] returnValues = new object[2];
            returnValues[0] = values[0];
            returnValues[1] = values[1];

            return returnValues;            
        }
    }

}