// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Documents;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Misc coverage for bug where Validation.HasError sequencing causes an ArgumentOutOfRangeException
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ValidationHasErrorException")]
    public class ValidationHasErrorException : XamlTest
    {
        #region Private Data

        private TextBox _myTextBox;
        private StackPanel _myStackPanel;        

        #endregion

        #region Constructors

        public ValidationHasErrorException()
            : base(@"ValidationHasErrorException.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members


        private TestResult Setup()
        {
            WaitForPriority(DispatcherPriority.Background);

            _myTextBox = (TextBox)RootElement.FindName("myTextBox");
            _myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");

            if (_myStackPanel == null || _myTextBox == null)
            {
                LogComment("Unable to load XAML elements.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }


        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Background);

            // Enter Invalid Data to cause an error.
            _myTextBox.Focus();
            _myTextBox.Text = "Invalid";
            _myStackPanel.Focus();
            
            // Wait till render is complete.
            WaitForPriority(DispatcherPriority.Render);

            // Now enter valid Text
            _myTextBox.Focus();
            _myTextBox.Text = "valid";
            _myStackPanel.Focus();

            try
            {
                _myStackPanel.Focus();
            }
            catch (ArgumentOutOfRangeException e)
            {
                LogComment("Unexpected ArgumentOutOfRangeException thrown.");
                LogComment(e.Message);
                return TestResult.Fail;
            }

            LogComment("No exceptions were thrown.");
            return TestResult.Pass;
        }

        #endregion
    }

    public class SamplePerson
    {
        public SamplePerson()
        {
            _name = "Anonymous";
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        string _name;
    }

    public class NameRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;
            string name = (String)value;
            if (name == "Invalid")
                result = new ValidationResult(false, "Name is invalid");
            return result;
        }
    }
}
