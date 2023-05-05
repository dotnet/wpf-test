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
	/// Binds 3 buttons: the first to a valid property of a data source, the second to an invalid property
	/// and the third to an invalid index. They all have fallback values. It verifies that the first button
	/// contains the value in the data source and the other two contain the fallback value.
	/// </description>
	/// </summary>
    [Test(0, "Binding", "FallbackValue")]
	public class FallbackValue: XamlTest
	{
		Button _btn1,_btn2,_btn3;

		public FallbackValue() : base(@"FallbackValue.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			
			RunSteps += new TestStep(ValidProperty);
			RunSteps += new TestStep(InvalidProperty);
			RunSteps += new TestStep(InvalidIndex);
		}

		#region Setup
		private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

			_btn1 = LogicalTreeHelper.FindLogicalNode(((DockPanel)RootElement), "btn1") as Button;
			if (_btn1 == null)
			{
				LogComment("Fail - Unable to reference btn1");
				return TestResult.Fail;
			}
			_btn2 = LogicalTreeHelper.FindLogicalNode(((DockPanel)RootElement), "btn2") as Button;
			if (_btn2 == null)
			{
				LogComment("Fail - Unable to reference btn2");
				return TestResult.Fail;
			}
			_btn3 = LogicalTreeHelper.FindLogicalNode(((DockPanel)RootElement), "btn3") as Button;
			if (_btn3 == null)
			{
				LogComment("Fail - Unable to reference btn3");
				return TestResult.Fail;
			}

			LogComment("Setup was successful");
			return TestResult.Pass;
		}
		#endregion

		#region RunSteps
		// Verifying value when binding to valid property
		private TestResult ValidProperty()
		{
			Status("ValidProperty");
			if (!VerifyBoundValue("Sleepy", _btn1.Content.ToString()))
				return TestResult.Fail;

			return TestResult.Pass;
		}

		// Verifying value when binding to invalid property
        private TestResult InvalidProperty()
		{
			Status("InvalidProperty");
			if (!VerifyBoundValue("UseFallbackValue", _btn2.Content.ToString()))
                return TestResult.Fail;

            return TestResult.Pass;
		}

		// Verifying value when binding to invalid index
        private TestResult InvalidIndex()
		{
			Status("InvalidIndex");
			if(!VerifyBoundValue("UseFallbackValue", _btn3.Content.ToString()))
                return TestResult.Fail;

            return TestResult.Pass;
		}
		#endregion

		#region AuxMethods
		// Verifies value is bound correctly.
		private bool VerifyBoundValue(string Expected, string Actual)
		{
			if (Expected != Actual)
			{
				LogComment("Bound value is incorrect.  Expected:  " + Expected + "  Actual:  " + Actual);
				return false;
			}

			return true;
		}
		#endregion
	}

	#region DataSource
    public class Dwarfs : ObservableCollection<Dwarf>
    {
		public Dwarfs()
		{
			Add(new Dwarf("Sleepy", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true));
			Add(new Dwarf("Dopey", "Green", 40, 300, Colors.Salmon, new Point(3, 7), false));
			Add(new Dwarf("Happy", "Red", 30, 400, Colors.Purple, new Point(5, 1), true));
			Add(new Dwarf("Grumpy", "Orange", 40, 275, Colors.Brown, new Point(7, 3), false));
			Add(new Dwarf("Bashful", "Purple", 30, 600, Colors.Gray, new Point(1, 5), true));
			Add(new Dwarf("Sneezy", "Black", 40, 800, Colors.DeepPink, new Point(5, 2), false));
			Add(new Dwarf("Doc", "Pink", 40, 800, Colors.DarkMagenta, new Point(2, 1), false));
		}
	}
	#endregion
}
