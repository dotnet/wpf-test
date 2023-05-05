// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Scenario 1: Makes sure that getting the container from item in a grouped TreeView does not throw .
    /// Scenario 2: Produces the same visual result as when using HierarchicalDataTemplate, just by using
    /// DataTemplate and Style.
	/// </description>
    /// </summary>

    [Test(1, "Controls", "TreeViewScenarios")]
	public class TreeViewScenarios : XamlTest
	{
        private GroupingVerifier _groupingVerifier;
        private Page _page;

        public TreeViewScenarios()
            : base(@"TreeViewScenarios.xaml")
		{
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(GroupingTreeView);
            RunSteps += new TestStep(SimulateHierarchicalDataTemplate);
        }

        TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.SystemIdle);

            _groupingVerifier = new GroupingVerifier();
            _page = (Page)(this.Window.Content);

            return TestResult.Pass;
        }

        #region GroupingTreeView
        // Scenario 1: Makes sure that getting the container from item in a grouped TreeView does not throw 
        
        TestResult GroupingTreeView()
        {
            Status("GroupingTreeView");

            Places places = (Places)(_page.Resources["places"]);
            TreeView tv = (TreeView)(LogicalTreeHelper.FindLogicalNode(_page, "tv"));
            CollectionViewSource cvs = (CollectionViewSource)(_page.Resources["cvs"]);

            
            TreeViewItem tvi = (TreeViewItem)(tv.ItemContainerGenerator.ContainerFromItem(places[1]));

            if (!VerifyGroupsState(places, cvs)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        // Verifies the groups when we're grouping by state
        private bool VerifyGroupsState(IList list, CollectionViewSource cvs)
        {
            ExpectedGroup group0 = new ExpectedGroup("WA", new object[] { list[0], list[1], list[2], list[3], list[10] });
            ExpectedGroup group1 = new ExpectedGroup("OR", new object[] { list[4] });
            ExpectedGroup group2 = new ExpectedGroup("CA", new object[] { list[5], list[6], list[7], list[8], list[9] });

            ExpectedGroup[] expectedGroups = new ExpectedGroup[] { group0, group1, group2 };

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(expectedGroups, cvs.View.Groups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return false;
            }

            return true;
        }
        #endregion

        #region SimulateHierarchicalDataTemplate
        TestResult SimulateHierarchicalDataTemplate()
        {
            Status("SimulateHierarchicalDataTemplate");

            Mountains mountains = (Mountains)(_page.Resources["mountains"]);
            TreeView tv1 = (TreeView)(LogicalTreeHelper.FindLogicalNode(_page, "tv1"));
            TreeViewItem tvi1 = (TreeViewItem)(tv1.ItemContainerGenerator.ContainerFromItem(mountains[0]));
            TreeViewItem tvi2 = (TreeViewItem)(tvi1.ItemContainerGenerator.ContainerFromItem(mountains[0].Lifts[0]));
            TreeViewItem tvi3 = (TreeViewItem)(tvi2.ItemContainerGenerator.ContainerFromItem(mountains[0].Lifts[0].Runs[0]));

            TextBlock tb = (TextBlock)(Util.FindDataVisual(tvi3, mountains[0].Lifts[0].Runs[0]));

            if ((tb == null) || (tb.Text != "Headwall"))
            {
                LogComment("Fail - Could not find visual for data in the leaf node");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion
    }
}
