// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading; 
using System.Windows.Threading;
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
	/// This tests using the same bind for 2 bindings (to bind to 2 different dps of a control).
    /// </description>
	/// </summary>
    [Test(1, "Binding", "ShareChangeBindingBvt")]
    public class ShareChangeBindingBvt : WindowTest
    {
        HappyMan _happy;
        Dwarf _dwarf;
        Binding _skincolorBind;

        public ShareChangeBindingBvt()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(ShareBinding);
        }

        private TestResult CreateTree()
        {
			Status("CreateTree");

			_happy = new HappyMan();
            _happy.Name = "Mr_X";
            _happy.Position = new Point(500, 500);
            _happy.Width = 200;
            _happy.Height = 200;
            _happy.SkinColor = Colors.Honeydew;
            Window.Content = _happy;

            _dwarf = new Dwarf("Dopey", "GreenYellow", 40, 300, Colors.Lavender, new Point(300, 700), false);
            _happy.DataContext = _dwarf;

			LogComment("CreateTree was successful");
			return TestResult.Pass;
        }

        // Verifies shared binding functionality.
        private TestResult ShareBinding()
        {
			Status("ShareBinding");

			_skincolorBind = new Binding("SkinColor");
            _skincolorBind.Mode = BindingMode.TwoWay;
			_happy.SetBinding(HappyMan.SkinColorProperty, _skincolorBind);

			_happy.SetBinding(HappyMan.EyeColorProperty, _skincolorBind);
            if (!ValidateBinding(Colors.Lavender))
                return TestResult.Fail;

            Status("Changing source value on skin color.");
            _dwarf.SkinColor = Colors.White;
            if (!ValidateBinding(Colors.White))
                return TestResult.Fail;

            Status("Changing target value on SkinColor.");
            _happy.SkinColor = Colors.DodgerBlue;
            WaitForPriority(DispatcherPriority.DataBind-1); //wait for transfer to source
            if (!ValidateBinding(Colors.DodgerBlue))
                return TestResult.Fail;

			LogComment("ShareBinding was successful");
			return TestResult.Pass;
        }

		// validate that all bound values are the what is expected
		private bool ValidateBinding(Color expectedValue)
		{
			if (_happy.SkinColor != expectedValue || _happy.EyeColor != expectedValue || _dwarf.SkinColor != expectedValue)
			{
				LogComment("Shared binding values are not all the same.");
				LogComment("Expected: '" + expectedValue + "'.");
				LogComment("Actual: happy.SkinColor='" + _happy.SkinColor + "'" + ", happy.EyeColor='" + _happy.EyeColor + "'" + ", dwarf.SkinColor='" + _dwarf.SkinColor + "'");
				return false;
			}

			return true;
		}
	}
}
