// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests binding a list of items to an items control with a dock panel as the items host.
	/// </description>
	/// </summary>
    [Test(3, "Controls", "DockPanelTest")]
	public class DockPanelTest : PanelBaseTest
	{
		public DockPanelTest() : this(@"dockpanel.xaml")
		{
		}
		public DockPanelTest(string filename) : base(filename)
		{
		}

	}
	/// <summary>
	/// <description>
	/// Tests binding a list of items to an items control with a canvas as the items host.
	/// </description>
	/// </summary>
    [Test(3, "Controls", "CanvasTest")]
	public class CanvasTest : PanelBaseTest
	{
		public CanvasTest() : this(@"canvas.xaml")
		{
		}
		public CanvasTest(string filename) : base(filename)
		{
		}
	}
	/// <summary>
	/// <description>
	/// Tests binding a list of items to an items control with a flow panel as the items host.
	/// </description>
	/// </summary>
    [Test(3, "Controls", "FlowPanelTest")]
	public class FlowPanelTest : PanelBaseTest
	{
		public FlowPanelTest() : this(@"flowpanel.xaml")
		{
		}
		public FlowPanelTest(string filename) : base(filename)
		{
		}
	}
	/// <summary>
	/// <description>
	/// Tests binding a list of items to an items control with a grid as the items host.
	/// </description>
	/// </summary>
    [Test(3, "Controls", "GridTest")]
	public class GridTest : PanelBaseTest
	{
		public GridTest() : this(@"gridpanel.xaml")
		{
		}
		public GridTest(string filename) : base(filename)
		{
		}
	}
	/// <summary>
	/// <area>Controls.Panels</area>

	/// <description>
	/// Base class for panels test cases.
	/// Makes sure that the elements within the panel match with the ones in the data source.
	/// </description>
	/// <spec>
	/// <name>ItemsControl</name>
	/// <section>No specific section</section>
	/// </spec>
	/// </summary>
	public abstract class PanelBaseTest : XamlTest
	{
		SortDataItems _testItems;
		public PanelBaseTest(string filename) : base(filename)
		{
			InitializeSteps += new TestStep(Init);
			RunSteps += new TestStep(ValidatePanel);
		}

        private TestResult Init()
		{
			Status("Init");
			_testItems = new SortDataItems();
			return TestResult.Pass;
		}

        private TestResult ValidatePanel()
		{
			Status("ValidatePanel");
			FrameworkElement[] visuals = Util.FindElements(RootElement, "visualElement");
			if (visuals.Length != _testItems.Count)
			{
				LogComment("Visuals Elements Count doesn't match DataSourceCount");
				return TestResult.Fail;
			}
			for (int i = 0; i < _testItems.Count; i++)
			{
				if (((TextBlock)visuals[i]).Text != ((SortItem)_testItems[i]).Name)
				{
					LogComment("Actual: " + ((TextBlock)visuals[i]).Text + "  Expected: " + ((SortItem)_testItems[i]).Name);
					return TestResult.Fail;
				}

			}
			LogComment("ValidatePanel was successful");
			return TestResult.Pass;
		}
	}

}
