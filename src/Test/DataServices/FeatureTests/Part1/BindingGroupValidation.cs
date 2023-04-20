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

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Validation for "required value" scenario is broken
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "BindingGroupValidation", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
    public class BindingGroupValidation : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;

        #endregion

        #region Constructors

        public BindingGroupValidation()
            : base(@"BindingGroupValidation.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");

			if (_myStackPanel == null)
			{
				LogComment("Failed to load Xaml correctly");
				return TestResult.Fail;
			}

            _myStackPanel.DataContext = new Person(String.Empty, "Great");

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
			WaitForPriority(DispatcherPriority.Render);

            // Verify that commitedit returns false since validation is broken.
            if (_myStackPanel.BindingGroup.CommitEdit() == true)
            {
                    LogComment("CommitEdit() returns true, it should return false");
                    return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion

    }

	#region Helper Classes

    public class NotEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Console.WriteLine("Validating that value is not empty.");
            if (value == null || String.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Must not be empty.");
            }
            return new ValidationResult(true, null);
        }
    }

	#endregion
}
