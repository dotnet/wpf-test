// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Threading; using System.Windows.Threading;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using System.Windows.Media;
using System.Windows.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Scenario: ItemTemplate of ItemsControl contains a Grid with a row and as many
	/// columns as properties in the data source object. Styles and property triggers
	/// were used to make it possible to tab from one cell to the next with visual
	/// feedback similar to Excell.
	/// </description>
	/// </summary>
    [Test(3, "Controls", "TabularGrid")]
	public class TabularGrid : XamlTest
	{
		ItemsControl _ic;
		ObservableCollection<MyStar> _aldc;

        public TabularGrid()
            : base(@"TabularGrid.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(VerifyItems);
			RunSteps += new TestStep(VerifyFocus);
		}

        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

			_ic = Util.FindElement(RootElement, "ic") as ItemsControl;
			if (_ic == null)
			{
				LogComment("Fail - Unable to reference ic element (ItemsControl)");
				return TestResult.Fail;
			}

            _aldc = new ObservableCollection<MyStar>();
            _aldc.Add(new MyStar("Mike", "Piazza", 24));
            _aldc.Add(new MyStar("Mark", "McGwire", 26));
            _aldc.Add(new MyStar("Jay", "Bell", 30));
            _aldc.Add(new MyStar("Matt", "Williams", 21));
            _aldc.Add(new MyStar("Barry", "Larkin", 29));
            _aldc.Add(new MyStar("Sammy", "Sosa", 26));
            _aldc.Add(new MyStar("Larry", "Walker", 28));
            _aldc.Add(new MyStar("Tony", "Gwynn", 32));
            _ic.ItemsSource = _aldc;

            LogComment("Setup was successful");
			return TestResult.Pass;
		}

        private TestResult VerifyItems()
		{
			Status("VerifyItems");
			WaitForPriority(DispatcherPriority.Render);

			StarVerifier sv = new StarVerifier(_ic);
			IVerifyResult res = sv.Verify(_aldc);
			LogComment(res.Message);
			return res.Result;
		}

        private TestResult VerifyFocus()
		{
			Status("VerifyFocus");

			// set focus to the first text box
            ContentPresenter cp = _ic.ItemContainerGenerator.ContainerFromIndex(0) as ContentPresenter;
			TextBox tb = Util.FindElement(cp, "firstName") as TextBox;
			tb.Focus();
			WaitForPriority(DispatcherPriority.Background);

			// verify that the focused text box has a specific background and border brush
			if (!VerifyFocusStyleTextBox(tb)) { return TestResult.Fail; }

			LogComment("VerifyFocus was successful");
			return TestResult.Pass;
		}

		// verify that the focused text box has a specific background and border brush
		private bool VerifyFocusStyleTextBox(TextBox tb)
		{
			Status("VerifyFocusStyleTextBox");
			WaitForPriority(DispatcherPriority.SystemIdle);
			//WaitFor(3000);

			if (tb.Background.ToString() != Brushes.LightGray.ToString())
			{
				LogComment("Fail - Expected background:" + Brushes.LightGray + " Actual:" + tb.Background);
				return false;
			}
			if (tb.BorderBrush.ToString() != Brushes.Black.ToString())
			{
				LogComment("Fail - Expected border:" + Brushes.Black + " Actual:" + tb.BorderBrush);
				return false;
			}
			return true;
		}
	}

}
