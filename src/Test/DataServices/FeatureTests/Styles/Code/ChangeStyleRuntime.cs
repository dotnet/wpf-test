// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test;
using System.Windows.Media;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Changes a style with a Binding at runtime and verifies new style was applied.
	/// </description>
	/// <relatedBugs>


    /// </relatedBugs>
	/// </summary>
    [Test(2, "Styles", "ChangeStyleRuntime")]
	public class ChangeStyleRuntime : XamlTest
	{
		TextBox _tb;

		public ChangeStyleRuntime() : base(@"ChangeStyleRuntime.xaml")
		{
			InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyOriginalStyle);
            RunSteps += new TestStep(VerifyChangedStyle);
		}

		private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

			_tb = LogicalTreeHelper.FindLogicalNode(RootElement, "tb") as TextBox;
			if (_tb == null)
			{
				LogComment("Fail - Unable to reference TextBox tb");
				return TestResult.Fail;
			}

			LogComment("Setup was successful");
			return TestResult.Pass;
		}

		private TestResult VerifyOriginalStyle()
		{
			Status("VerifyOriginalStyle");

			if (_tb.Foreground != Brushes.Green)
			{
				LogComment("Fail - Foreground should be green, instead it is " + _tb.Foreground);
				return TestResult.Fail;
			}
			if (_tb.Text != "old source of binding")
			{
				LogComment("Fail - Text should be 'old source of binding', instead it is " + _tb.Text);
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

		private TestResult VerifyChangedStyle()
		{
			Status("VerifyChangedStyle");

			// change style
			_tb.Style = RootElement.Resources["myStyle1"] as Style;
			WaitForPriority(DispatcherPriority.Render);

			// verify it was applied
			if (_tb.Foreground != Brushes.Red)
			{
				LogComment("Fail - Foreground should be red, instead it is " + _tb.Foreground);
				return TestResult.Fail;
			}
			if (_tb.Text != "new source of binding")
			{
				LogComment("Fail - Text should be 'new source of binding', instead it is " + _tb.Text);
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}
	}
}

