// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Validates the behavior on a two-way binding when the target and the source change
	/// </description>
	/// </summary>
    [Test(0, "Binding", "ChangedPatternBvt")]
    public class ChangedPatternBvt : WindowTest
    {
        HappyMan _happy;
        Human _human;

        public ChangedPatternBvt()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep (SetBinding);
            RunSteps += new TestStep (ChangeSource);
            RunSteps += new TestStep (ChangeTarget);
        }

        private TestResult CreateTree() {
            Status("Creating the Element Tree");
            _happy = new HappyMan();
            _happy.Name = "George";
            Window.Content = _happy;

            _human = new Human("Snow_White");

            _happy.DataContext = _human;
            return TestResult.Pass;
        }


        // Set TwoWay binding.
		private TestResult SetBinding()
		{
            Status("Setting TwoWay binding.");
            Binding bind = new Binding("Name");
            bind.Mode = BindingMode.TwoWay;
            _happy.SetBinding(HappyMan.NameProperty, bind);
            return ValidateBinding("Snow_White");
        }

        // Validate behavior on TwoWay binding when source value is changed.
        private TestResult ChangeSource ()
        {
            Status("Changing source value for TwoWay binding.");
            _human.Name = "new_Source";

            return ValidateBinding("new_Source");
        }

        // Validate behavior on TwoWay binding when target element is changed.
        private TestResult ChangeTarget ()
        {
            Status("Changing target element value for TwoWay binding.");
            _happy.Name = "new_Target";

            return ValidateBinding("new_Target");
        }

        // Validates that the source and target are the expected values
        private TestResult ValidateBinding(string expectedValue) {
            if (expectedValue != _human.Name)
            {
                LogComment("Source Human name has an unexpected value");
                LogComment("Expected: " + expectedValue);
                LogComment("Actual: " + _human.Name);
                return TestResult.Fail;
            }

            if (expectedValue != _happy.Name)
            {
                LogComment("Target HappyMan name has an unexpected value");
                LogComment("Expected: " + expectedValue);
                LogComment("Actual: " + _happy.Name);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
