// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Bindinggroups dont work on a binding attached to a freezable
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "BindingGroupFreezable")]
    public class BindingGroupFreezable : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private UniformGrid _myGrid;
        
        #endregion

        #region Constructors

        public BindingGroupFreezable()
            : base(@"BindingGroupFreezable.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myGrid = (UniformGrid)RootElement.FindName("myGrid");
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");

            if (_myStackPanel == null || _myGrid == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            ValidateFreezable validateFreezable = new ValidateFreezable();
            _myGrid.BindingGroup.ValidationRules.Add(validateFreezable);
            _myGrid.BindingGroup.BeginEdit();

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // If we get are able to commit an edit then this bug is no longer present
            if (_myGrid.BindingGroup.CommitEdit())
            {   
                LogComment("Success. We are able to commit and edit.");
                return TestResult.Pass;
            }

            LogComment("Unable to commit and edit.");
            return TestResult.Fail;
        }

        // This event occurs when a ValidationRule in the BindingGroup
        // or in a Binding fails.
        private void ItemError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
               throw new TestValidationException("Failed to commit edit. " + e.Error.ErrorContent.ToString());
            }
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class ValidateFreezable : ValidationRule
    {
        // Ensure that an item over $100 is available for at least 7 days.
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            BindingGroup bg = value as BindingGroup;

            if (bg.Items.Count == 0)
                return new ValidationResult(false, "Did not find Freezable binding in binding group");

            return ValidationResult.ValidResult;
        }
    }
    
    #endregion
}
