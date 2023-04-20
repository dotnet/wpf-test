// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading; 
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// The xaml for this test contains 2 pairs of listboxes, all bound to the same data source, 
	/// each pair with its own view. This tests that each pair of listboxes syncs independently 
	/// on the other pair.
	/// </description>
	/// </summary>
    [Test(0, "Views", "MultiView")]
	public class MultiView : XamlTest
	{
		ListBox _teamLBa;
		ListBox _divisionLBb;
		ListBox _teamLBb;

		public MultiView() : base(@"Multiview.xaml")
		{
			InitializeSteps += new TestStep (init);
			RunSteps += new TestStep(ValidateMultiView);
		}

		private TestResult init ()
		{
			Status ("Initializations");
			WaitForPriority (DispatcherPriority.Background);
			DockPanel dp = ((DockPanel)((StackPanel)RootElement).Children[0]);

			_teamLBa = LogicalTreeHelper.FindLogicalNode (dp, "teamLB") as ListBox;
			_divisionLBb = LogicalTreeHelper.FindLogicalNode(dp, "divisionLB") as ListBox;
			_teamLBb = LogicalTreeHelper.FindLogicalNode(dp, "teamLB1") as ListBox;

			//Changing view
			_divisionLBb.SelectedIndex = 2;
			WaitForPriority(DispatcherPriority.Background);
			return TestResult.Pass;
		}


		private TestResult ValidateMultiView()
		{
			if (((Team)_teamLBa.SelectedItem).Name != "Anaheim" || ((Team)_teamLBb.SelectedItem).Name != "Baltimore")
			{
				LogComment("The 2 views were not different!!");
				return TestResult.Fail;
			}
			return TestResult.Pass;
		}
	}
}

