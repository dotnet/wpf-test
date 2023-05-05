// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
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
    /// Sets Binding.Mode to OneTime, OneWay, OneWayToSource, and TwoWay, changes the source and target and makes
	/// sure that the correct data is in the source and target.
	/// </description>
	/// </summary>
    [Test(0, "Binding", "BindTypeBvt")]
    public class BindTypeBvt : WindowTest
    {
        HappyMan _happy;
        Dwarf _dwarf;

        public BindTypeBvt()
        {
            InitializeSteps += new TestStep(CreateTree);

            //OneTime
            RunSteps += new TestStep(OneTime_SetBinding);
            RunSteps += new TestStep(OneTime_ChangeSource);
            RunSteps += new TestStep(OneTime_ChangeTarget);

            //OneWay
            RunSteps += new TestStep(OneWay_SetBinding);
            RunSteps += new TestStep(OneWay_ChangeSource);
            RunSteps += new TestStep(OneWay_ChangeTarget);

            //TwoWay
            RunSteps += new TestStep(TwoWay_SetBinding);
            RunSteps += new TestStep(TwoWay_ChangeSource);
            RunSteps += new TestStep(TwoWay_ChangeTarget);

            //OneWayToSource
            RunSteps += new TestStep(OneWayToSource_SetBinding);
            RunSteps += new TestStep(OneWayToSource_ChangeSource);
            RunSteps += new TestStep(OneWayToSource_ChangeTarget);

		}

		#region CreateTree
		private TestResult CreateTree()
        {
			Status("CreateTree");

			_happy = new HappyMan();

            _happy.Name = "George";
            _happy.Position = new Point(100,100);
            _happy.Width = 200;
            _happy.Height = 200;

            Window.Content = _happy;

            _dwarf = new Dwarf("Sleepy", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true);

            _happy.DataContext = _dwarf;

			LogComment("CreateTree was successful");
			return TestResult.Pass;
		}
		#endregion

		#region OneTime
		// Set OneTime binding.
		private TestResult OneTime_SetBinding()
        {
			Status("OneTime_SetBinding");

			Binding bind = new Binding("Name");
            bind.Mode = BindingMode.OneTime;
            _happy.SetBinding(HappyMan.HappyNameProperty, bind);

			return ValidateBinding("Sleepy", "Sleepy");
        }

        // Validate OneTime binding behavior at source change.
        private TestResult OneTime_ChangeSource()
        {
			Status("OneTime_ChangeSource");
			_dwarf.Name = "new Source";

			return ValidateBinding("new Source", "Sleepy");
        }

        // Validate OneTime binding behavior at target element change.
        private TestResult OneTime_ChangeTarget()
        {
			Status("OneTime_ChangeTarget");
			_happy.HappyName = "new Target";

            return ValidateBinding("new Source", "new Target");
		}
		#endregion

		#region OneWay
		// Set OneWay binding.
		private TestResult OneWay_SetBinding()
        {
			Status("OneWay_SetBinding");

			ResetBinding("Sleepy", "George");

            Binding bind = new Binding("Name");
            bind.Mode = BindingMode.OneWay;
            _happy.SetBinding(HappyMan.HappyNameProperty, bind);

            return ValidateBinding("Sleepy", "Sleepy");
        }

        // Validate OneWay behavior when changing the source value.
        private TestResult OneWay_ChangeSource()
        {
			Status("OneWay_ChangeSource");
			_dwarf.Name = "new Source";

            return ValidateBinding("new Source", "new Source");
        }

        // Validate OneWay behavior when changing the target element value.
        private TestResult OneWay_ChangeTarget()
        {
			Status("OneWay_ChangeTarget");
			_happy.HappyName = "new Target";

            return ValidateBinding("new Source", "new Target");
		}
		#endregion

		#region TwoWay
		// Set TwoWay binding.
		private TestResult TwoWay_SetBinding()
        {
			Status("TwoWay_SetBinding");

			ResetBinding("Sleepy", "George");

            Binding bind = new Binding("Name");
            bind.Mode = BindingMode.TwoWay;
            _happy.SetBinding(HappyMan.HappyNameProperty, bind);

            return ValidateBinding("Sleepy","Sleepy");
        }

        // Validate behavior on TwoWay binding when source value is changed.
        private TestResult TwoWay_ChangeSource()
        {
			Status("TwoWay_ChangeSource");
			_dwarf.Name = "new Source";

            return ValidateBinding("new Source", "new Source");
        }

        // Validate behavior on TwoWay binding when target element is changed.
        private TestResult TwoWay_ChangeTarget()
        {
			Status("TwoWay_ChangeTarget");
			_happy.HappyName = "new Target";

            return ValidateBinding("new Target", "new Target");
		}
		#endregion


        #region OneWayToSource
        // Set OneWayToSource binding.
        private TestResult OneWayToSource_SetBinding()
        {
            Status("OneWay_SetBinding");

            ResetBinding("Sleepy", "George");

            Binding bind = new Binding("Name");
            bind.Mode = BindingMode.OneWayToSource;
            bind.FallbackValue = "George";
            _happy.SetBinding(HappyMan.HappyNameProperty, bind);

            return ValidateBinding("George", "George");
        }

        // Validate OneWay behavior when changing the source value.
        private TestResult OneWayToSource_ChangeSource()
        {
            Status("OneWay_ChangeSource");
            _dwarf.Name = "new Source";

            return ValidateBinding("new Source", "George");
        }

        // Validate OneWay behavior when changing the target element value.
        private TestResult OneWayToSource_ChangeTarget()
        {
            Status("OneWay_ChangeTarget");
            _happy.HappyName = "new Target";

            return ValidateBinding("new Target", "new Target");
        }
        #endregion


		#region AuxMethods
		// Validates that the source and target are the expected values
		private TestResult ValidateBinding(string expectedSource, string expectedTarget)
		{
			Status("ValidateBinding");
			if (expectedSource != _dwarf.Name)
            {
                LogComment("Source Dwarf name has an unexpected value");
                LogComment("Expected: " + expectedSource);
                LogComment("Actual: " + _dwarf.Name);
                return TestResult.Fail;
            }

            if (expectedTarget != _happy.HappyName)
            {
                LogComment("Target HappyMan name has an unexpected value");
                LogComment("Expected: " + expectedTarget);
                LogComment("Actual: " + _happy.HappyName);
                return TestResult.Fail;
            }

			return TestResult.Pass;
		}

		// Resets the binding and values to a specific state
		private void ResetBinding(string source, string target)
		{
			Status("ResetBinding");
			BindingOperations.ClearBinding(_happy, HappyMan.HappyNameProperty);
			_dwarf.Name = source;
			_happy.HappyName = target;
		}
		#endregion
    }
}
