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
    ///  Regression coverage for bug where BindingGroup Validate/Commit API can incorrectly return true when their is an error due to an ExceptionValidationRule
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "BindingGroupValidateError")]
    public class BindingGroupValidateError : AvalonTest
    {
        #region Constructors

        public BindingGroupValidateError()
        {
            InitializeSteps += new TestStep(Validate);                        
        }

        #endregion

        #region Private Members
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Create a StackPanel with a bindinggroup
            StackPanel stackPanel = new StackPanel();
            stackPanel.BindingGroup = new BindingGroup();

            // Create an object and add it to the DataContext
            MyObject testObject = new MyObject();
            testObject.MyProperty = 5.00;
            stackPanel.DataContext = testObject;

            // Create a textbox and add it to the tree
            TextBox textBox = new TextBox();
            stackPanel.Children.Add(textBox);
            
            // Create a binding and Validation Rule
            Binding binding = new Binding("MyProperty");
            ExceptionValidationRule validationRule = new ExceptionValidationRule();
            validationRule.ValidationStep = ValidationStep.ConvertedProposedValue;
            binding.ValidationRules.Add(validationRule);
            textBox.SetBinding(TextBox.TextProperty, binding);

            // Update the value
            textBox.Text = "D";
            
            // Verify 
            if (stackPanel.BindingGroup.ValidateWithoutUpdate() == true)
            {
                LogComment("Since we have a Validation Exception, this should have returned false instead of true.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class MyObject
    {
        public double MyProperty { get; set; }
    }

    #endregion
}
