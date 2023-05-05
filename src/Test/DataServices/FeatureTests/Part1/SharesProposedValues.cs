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
using System.Windows.Controls.Primitives;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where BindingGroup - support swapping bindings for a single source property
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Binding", "SharesProposedValues", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class SharesProposedValues : XamlTest
    {
        #region Private Data

        private int _index;
        private TextBox _myTextBox;
        private TextBlock _myTextBlock;
        private TextBox _myTextBox2;
        private TextBlock _myTextBlock2;
        private Button _myButton;
        private StackPanel _myStackPanel;
        private BindingBase _binding;
        private BindingBase _binding2;
        private string _proposedName;
        
        #endregion

        #region Constructors

        public SharesProposedValues()
            : base(@"SharesProposedValues.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(ValidSwap);
            RunSteps += new TestStep(InvalidSwap);
            RunSteps += new TestStep(ChangeSource);            
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myTextBox = (TextBox)RootElement.FindName("myTextBox");
            _myTextBox2 = (TextBox)RootElement.FindName("myTextBox2");
            _myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");
            _myButton = (Button)RootElement.FindName("myButton");

            if (_myTextBox == null || _myTextBox2 == null || _myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }
            
            Place place = new Place("Newark", "NJ");
            _myStackPanel.DataContext = place;

            _myStackPanel.BindingGroup.BeginEdit();
            _binding = BindingOperations.GetBindingBase(_myTextBox, TextBox.TextProperty);
            _binding2 = BindingOperations.GetBindingBase(_myTextBox2, TextBox.TextProperty);

            return TestResult.Pass;
        }

        // 1. Replace template after proposing a valid new value
        private TestResult ValidSwap()
        {
            WaitForPriority(DispatcherPriority.Render);            

            // Change Text
            _proposedName = _myTextBox.Text = "Toronto";                      
            
            // Replace with non-editable template.
            _index = _myStackPanel.Children.IndexOf(_myTextBox);
            _myStackPanel.Children.RemoveAt(_index);
            _myTextBlock = new TextBlock();
            _myStackPanel.Children.Insert(_index, _myTextBlock); 
            _myTextBlock.SetBinding(TextBlock.TextProperty, _binding);
            
            // Check if proposed values were kept.            
            if (_myTextBlock.Text != _proposedName)
            {
                LogComment("Proposed values were not kept intact through template change.");
                LogComment("Actual: " + _myTextBlock.Text);
                LogComment("Expected: " + _proposedName);
                return TestResult.Fail;
            }

            // Verify that we can commit good data
            if (_myStackPanel.BindingGroup.CommitEdit() != true)
            {
                LogComment("We were unable to commit good data.");
                return TestResult.Fail;
            }

            // Verify that data was updated correctly
            Place grabPlace = (Place)_myStackPanel.DataContext;

            if (grabPlace.Name != _proposedName)
            {
                LogComment("Source was not updated correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // 2. Replace template after proposing invalid value.
        private TestResult InvalidSwap()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Change Value
            _proposedName = _myTextBox2.Text = RawRule.InvalidValue;

            // Replace with non-editable template.
            _index = _myStackPanel.Children.IndexOf(_myTextBox2);
            _myStackPanel.Children.RemoveAt(_index);
            _myTextBlock2 = new TextBlock();
            _myStackPanel.Children.Insert(_index, _myTextBlock2);
            _myTextBlock2.SetBinding(TextBlock.TextProperty, _binding2);            

            ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(_myTextBlock2);
            if (errors.Count != 1)
            {
                LogComment("Did not receIve The validation error.");
                return TestResult.Fail;
            }           

            // Check if proposed values (if they are invalid) were not kept.            
            if (_myTextBlock2.Text != String.Empty)
            {
                LogComment("Proposed values were not kept intact through template change.");
                LogComment("Actual: " + _myTextBlock2.Text);
                LogComment("Expected: " + _proposedName);
                return TestResult.Fail;            
            }

            _myStackPanel.Children.RemoveAt(_index);            
            _myStackPanel.Children.Insert(_index, _myTextBox2);
            _myTextBox2.SetBinding(TextBox.TextProperty, _binding2);

            // Check if proposed values (even if they are invalid) were kept.            
            if (_myTextBox2.Text != _proposedName)
            {
                LogComment("Proposed values were not kept intact through template change.");
                LogComment("Actual: " + _myTextBox2.Text);
                LogComment("Expected: " + _proposedName);
                return TestResult.Fail;
            }            

            // Verify that we can't commit bad data
            if (_myStackPanel.BindingGroup.CommitEdit() != false)
            {
                LogComment("We were able to commit bad data.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult ChangeSource()
        {
            WaitForPriority(DispatcherPriority.Render);

            _myStackPanel.Children.Remove(_myTextBlock);
            _myStackPanel.Children.Add(_myTextBox);
            _myTextBox.SetBinding(TextBox.TextProperty, _binding);
            _proposedName = _myTextBox.Text = "Winterpeg";

            // Now change the source.
            string newSource = "Mississauga";
            Place place2 = new Place(newSource, "ON");
            _myStackPanel.DataContext = place2;            

            _myStackPanel.Children.Remove(_myTextBox);
            _myTextBlock = new TextBlock();
            _myStackPanel.Children.Add(_myTextBlock);
            _myTextBlock.SetBinding(TextBlock.TextProperty, _binding);            

            // Check if proposed values were thrown out.            
            if (_myTextBlock.Text != newSource)
            {
                LogComment("Proposed values were not overriden by changed source.");
                LogComment("Actual: " + _myTextBlock.Text);
                LogComment("Expected: " + newSource);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class RawRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {            
            string str = (string)value;

            if (str == InvalidValue)
            {
                return new ValidationResult(false, "Invalid Raw.");
            }

            return ValidationResult.ValidResult;
        }

        public static readonly string InvalidValue = "invalid raw";
    }

    public class ConvRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = (string)value;

            if (str == InvalidValue)
            {
                return new ValidationResult(false, "Invalid Converted.");
            }

            return ValidationResult.ValidResult;
        }

        public static readonly string InvalidValue = "invalid conv";
    }
    
    #endregion
}