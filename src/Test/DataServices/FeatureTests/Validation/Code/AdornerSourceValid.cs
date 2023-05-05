// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// When there's a successful data transfer from source to target and there were validation errors
    /// previously, those errors should be cleared.
    /// </description>
    /// </summary>
    [Test(2, "Validation", "AdornerSourceValid")]
    public class AdornerSourceValid : XamlTest
    {
        private TextBox _tb;
        private Player _player;

        public AdornerSourceValid()
            : base(@"AdornerSourceValid.xaml")        {
            InitializeSteps += new TestStep(Setup);
            InitializeSteps += new TestStep(VerifyNoValidationError);
            InitializeSteps += new TestStep(SetInvalidTarget);
            InitializeSteps += new TestStep(VerifyValidationError);
            InitializeSteps += new TestStep(ChangeValidSource);
            InitializeSteps += new TestStep(VerifyNoValidationError);
        }

        private TestResult Setup()
        {
            Status("Setup");

            _tb = LogicalTreeHelper.FindLogicalNode(RootElement, "tb") as TextBox;
            if (_tb == null)
            {
                LogComment("Fail - Not able to reference TextBox tb");
                return TestResult.Fail;
            }

            Binding binding = BindingOperations.GetBinding(_tb, TextBox.TextProperty);
            _player = (Player)(binding.Source);
            return TestResult.Pass;
        }

        private TestResult VerifyNoValidationError()
        {
            Status("VerifyNoValidationError");

            ReadOnlyCollection<ValidationError> errors = Validation.GetErrors(_tb);
            if (errors.Count != 0)
            {
                LogComment("Fail - There should be no validation errors, instead there are " + errors.Count);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult SetInvalidTarget()
        {
            Status("SetInvalidSource");

            _tb.Text = "hello";
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult VerifyValidationError()
        {
            Status("VerifyValidationError");
            

            ReadOnlyCollection<ValidationError> errors = Validation.GetErrors(_tb);
            if (errors.Count != 1)
            {
                LogComment("Fail - There should be 1 validation error, instead there are " + errors.Count);
                return TestResult.Fail;
            }

            ValidationError error = (ValidationError)(errors[0]);
            if (error.Exception.GetType() != typeof(FormatException))
            {
                LogComment("Fail - Exception should be of type FormatException, instead it is of type " + error.Exception.GetType());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult ChangeValidSource()
        {
            Status("SetValidSource");

            _player.Votes = 100;

            return TestResult.Pass;
        }

    }
}
