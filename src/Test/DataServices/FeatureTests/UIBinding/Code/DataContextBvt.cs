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
    /// Tests setting and changing DataContext on a control and tests DataContext inheritance.
    /// </description>
	/// </summary>
    [Test(1, "Binding", "DataContextBvt")]
    public class DataContextBvt : WindowTest
    {
        HappyMan _happy1,_happy2;
        Dwarf _dwarf1,_dwarf2,_dwarf3;
		DockPanel _dp;

        public DataContextBvt()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(DCTransition);
            RunSteps += new TestStep(Inheritance);
        }

        private TestResult CreateTree()
        {
			Status("CreateTree");

			_dp = new DockPanel();
			_happy1 = new HappyMan();
			_happy1.Name = "Mr_X";
            _happy1.Position = new Point(500, 500);
            _happy1.Width = 200;
            _happy1.Height = 200;
            _happy1.SkinColor = Colors.Honeydew;
		    _dp.Children.Add(_happy1);
            Window.Content = _dp;

			_happy2 = new HappyMan();
			_happy2.Name = "Smiley";
			_happy2.SkinColor = Colors.AliceBlue;
			_happy2.EyeColor = Colors.Turquoise;

			_dwarf1 = new Dwarf("Dopey", "GreenYellow", 40, 300, Colors.Lavender, new Point(300, 700), false);
			_dwarf2 = new Dwarf("Dopey2", "Green", 40, 300, Colors.Red, new Point(200, 400), false);
			_dwarf3 = new Dwarf("Dopey3", "Blue", 40, 300, Colors.Black, new Point(200, 500), false);

			LogComment("CreateTree was successful");
			return TestResult.Pass;
        }

        // Verifies DataContext transitions
        private TestResult DCTransition()
        {
			Status("DCTransition");

			Status("Setting binding for happy1");
			// there's no DataContext or DataSource, so this binding is invalid
			// the SkinColor value goes back to the default, which is Yellow
			_happy1.SetBinding(HappyMan.SkinColorProperty, "SkinColor");
			WaitForPriority(DispatcherPriority.SystemIdle);
            if (!ValidateTest(Colors.Yellow, _happy1.SkinColor))
                return TestResult.Fail;

            Status("Changing happy1 DataContext to dwarf1");
            _happy1.DataContext = _dwarf1;
            if (!ValidateTest(Colors.Lavender, _happy1.SkinColor))
                return TestResult.Fail;

            Status("Changing happy1 DataContext to dwarf2.");
            _happy1.DataContext = _dwarf2;
            if (!ValidateTest(Colors.Red, _happy1.SkinColor))
                return TestResult.Fail;

            Status("Changing happy1 DataContext to null.");
            _happy1.DataContext = null;
            if (!ValidateTest(Colors.Yellow, _happy1.SkinColor))
                return TestResult.Fail;

			LogComment("DCTransition was successful");
			return TestResult.Pass;
        }

        // Verifies DataContext inheritance
        private TestResult Inheritance()
        {
			Status("Inheritance");

			Window.DataContext = _dwarf3;
			_dp.Children.Add(_happy2);

			// happy2's grandparent has DataContext set (Window -> DockPanel dp -> HappyMan happy2)
			Status("Inheriting DataContext from grandparent");
			_happy2.SetBinding(HappyMan.SkinColorProperty, "SkinColor");
			WaitForPriority(DispatcherPriority.SystemIdle);
            if(!ValidateTest(Colors.Black, _happy2.SkinColor))
                return TestResult.Fail;

			// happy2's parent also has DataContext set (should have priority)
			Status("Inheriting DataContext from parent");
			_dwarf3.SkinColor = Colors.Pink;
			_dp.DataContext = _dwarf3;
			if (!ValidateTest(Colors.Pink, _happy2.SkinColor))
				return TestResult.Fail;

			// happy2 has DataContext set (should have priority)
			Status("Setting DataContext on happy2.");
            _happy2.DataContext = _dwarf2;
            if (!ValidateTest(Colors.Red, _happy2.SkinColor))
                return TestResult.Fail;

			// happy2 no longer has DataContext, should inherit it from parent again
            Status("Clearing the DataContext on happy2.");
            _happy2.ClearValue(FrameworkElement.DataContextProperty);
            if (!ValidateTest(Colors.Pink, _happy2.SkinColor))
                return TestResult.Fail;

			LogComment("Inheritance was successful");
			return TestResult.Pass;
        }

        private bool ValidateTest(Color expectedValue, Color actualValue)
        {
			Status("ValidateTest");
			if (expectedValue != actualValue)
            {
                LogComment("Bound value is incorrect.");
                LogComment("Expected: " + expectedValue);
                LogComment("Actual: " + actualValue);
                return false;
            }

            return true;
        }


    }


}
