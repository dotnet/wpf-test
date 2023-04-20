// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading; 
using System.Windows.Threading;
using System.Windows;
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
	/// Tests the UpdateSourceTrigger values "LostKeyboardFocus" and "Explicit"
	/// </description>
	/// </summary>
    [Test(0, "Binding", "UpdateBindingBvt")]
    public class UpdateBindingBvt : WindowTest
    {
        HappyMan _happyX,_happyY;
        Dwarf _dwarf;
		DockPanel _dp;

        public UpdateBindingBvt()
        {
            InitializeSteps += new TestStep(CreateTree);

            RunSteps += new TestStep(OnLostKeyboardFocus);
            RunSteps += new TestStep(Explicitly);
		}

		private TestResult CreateTree()
        {
			Status("CreateTree");

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

			//Make sure that the window renders before the test continues
			WaitForPriority(DispatcherPriority.Render);

			LogComment("CreateTree was successful");
			return TestResult.Pass;
		}

		// Verifies that transfer to source occurs OnLostKeyboardFocus.
		private TestResult OnLostKeyboardFocus()
        {
			Status("OnLostKeyboardFocus");
			Binding bind = new Binding("Name");
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
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

			LogComment("OnLostKeyboardFocus was successful");
			return TestResult.Pass;
		}

		// Verifies that transfer to source occurs explicitly.
		private TestResult Explicitly()
        {
			Status("Explicitly");
			_dwarf.Name = "Dopey";

            Status("setting Explicit binding.");
            Binding bind = new Binding("Name");
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
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

			LogComment("Explicitly was successful");
			return TestResult.Pass;
		}
	}
}
