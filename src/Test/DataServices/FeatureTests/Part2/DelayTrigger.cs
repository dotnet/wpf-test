// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests Delay property for binding triggers
    /// The delay should only happen on TwoWay binding
    /// and on PropertyChanged trigger
    /// </description>
    /// </summary>
    [Test(0, "Binding", "DelayTrigger")]
    public class DelayTriggerBvt : WindowTest
    {
        HappyMan _happyX,_happyY;
        Dwarf _dwarf;
		DockPanel _dp;
        int _delay;

        public DelayTriggerBvt()
        {
            InitializeSteps += new TestStep(Setup);

            RunSteps += new TestStep(LostFocusDelay);
            RunSteps += new TestStep(ExplicitDelay);
            RunSteps += new TestStep(PropertyChangedDelay);
		}

		private TestResult Setup()
        {
			Status("Setup");

			_dp = new DockPanel();
            _happyX = new HappyMan();
            _happyX.Name = "Mr_X";
            _happyX.Position = new Point(500, 500);
            _happyX.Width = 200;
            _happyX.Height = 200;
            _happyX.SkinColor = Colors.Honeydew;
            _dp.Children.Add(_happyX);

            _happyY = new HappyMan();
            _happyY.Name = "Mr_Y";
            _happyY.Position = new Point(100, 200);
            _happyY.Width = 300;
            _happyY.Height = 200;
            _happyY.SkinColor = Colors.Maroon;
            _dp.Children.Add(_happyY);

            Window.Content = _dp;

            _dwarf = new Dwarf("Dopey", "Green", 40, 300, Colors.Salmon, new Point(300, 700), false);
            _happyX.DataContext = _dwarf;
            _delay = 300;

			//Make sure that the window renders before the test continues
			WaitForPriority(DispatcherPriority.Render);

			LogComment("Setup was successful");
			return TestResult.Pass;
		}

		// Verifies that transfer to source occurs LostFocus.
		private TestResult LostFocusDelay()
        {
			Status("LostFocusDelay");
			Binding bind = new Binding("Name");
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            bind.Delay = _delay;
            _happyX.SetBinding(HappyMan.NameProperty, bind);

            _happyX.Focus();
            Status("Applying new value to bound property.");
            _happyX.Name = "Moctezuma";
            if (_dwarf.Name != "Dopey")
            {
                LogComment("Source value is incorrect.  Expected:  'Dopey'.  Actual:  '" + _dwarf.Name + "'.");
                return TestResult.Fail;
            }

            Status("Removing focus from element.");
            _happyY.Focus();
            if (_dwarf.Name != "Moctezuma")
            {
                LogComment("Source value is incorrect.  Expected:  'Moctezuma'.  Actual:  '" + _dwarf.Name + "'.");
                return TestResult.Fail;
            }

			LogComment("LostFocusDelay was successful");
			return TestResult.Pass;
		}

        private TestResult PropertyChangedDelay()
        {
            Status("PropertyChangedDelay");
            _dwarf.Name = "Dopey";
            Binding bind = new Binding("Name");
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bind.Delay = _delay;
            _happyX.SetBinding(HappyMan.NameProperty, bind);

            Status("Applying new value to bound property.");
            _happyX.Name = "Doc";

            if (_dwarf.Name == "Doc")
            {
                LogComment("Source value is incorrect.  Expected:  'Dopey'.  Actual:  '" + _dwarf.Name + "'.");
                LogComment("Delay property was not respected");
                return TestResult.Fail;
            }

            WaitFor(_delay);

            if (_dwarf.Name != "Doc")
            {
                LogComment("Source value is incorrect.  Expected:  'Doc'.  Actual:  '" + _dwarf.Name + "'.");
                LogComment("The source did not update after Delay trigger");
                return TestResult.Fail;
            }

            LogComment("PropertyChangedDelay was successful");
            return TestResult.Pass;
        }

		// Verifies that transfer to source occurs explicitly.
		private TestResult ExplicitDelay()
        {
			Status("ExplicitDelay");
			_dwarf.Name = "Dopey";

            Status("setting Explicit binding.");
            Binding bind = new Binding("Name");
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bind.Delay = _delay;
            BindingExpressionBase binding = _happyX.SetBinding(HappyMan.NameProperty, bind);

            Status("Applying new value to bound property.");
            _happyX.Name = "Moctezuma";
            if (_dwarf.Name != "Dopey")
            {
                LogComment("Source value is incorrect.  Expected:  'Dopey'.  Actual:  '" + _dwarf.Name + "'.");
                return TestResult.Fail;
            }

            Status("Updating binding.");
            binding.UpdateSource();
            if (_dwarf.Name != "Moctezuma")
            {
                LogComment("Source value is incorrect.  Expected:  'Moctezuma'.  Actual:  '" + _dwarf.Name + "'.");
                return TestResult.Fail;
            }

			LogComment("ExplicitDelay was successful");
			return TestResult.Pass;
		}
	}
}

