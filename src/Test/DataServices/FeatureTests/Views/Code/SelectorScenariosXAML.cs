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
using System.Collections.Specialized;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Scenario 1: Select an item in the ListBox. 
    /// Set SelectedValue such that there are two items in the collection that satisfy the
    /// condition, and the selected item is the second one. The first one should be selected now, independently
    /// on which one was selected before.
    /// Scenario 2: Adding null to an array of items that is set to an ItemsSource used to throw. Verify it
    /// doesn't throw anymore
    /// </description>
    /// <relatedBugs>


    /// </relatedBugs>
	/// </summary>

    [Test(2, "Views", "SelectorScenariosXAML")]
    public class SelectorScenariosXAML : XamlTest
    {
        Page _page;

        public SelectorScenariosXAML()
            : base(@"SelectorScenariosXAML.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(AmbiguousSelectedValue);
            RunSteps += new TestStep(CrashNullValue);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);
            _page = (Page)(this.Window.Content);
            return TestResult.Pass;
        }

        #region AmbiguousSelectedValue
        private TestResult AmbiguousSelectedValue()
        {
            Status("AmbiguousSelectedValue");

            WaitForPriority(DispatcherPriority.SystemIdle);
            ListBox BugLB = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "BugLB"));

            // Want this to fail if the listbox never populates, so let it time out.
            while (BugLB.Items.Count <= 0)
            {
                TestLog.Current.LogEvidence("Waiting for Data Provider...");
                WaitForPriority(DispatcherPriority.SystemIdle);
                Thread.Sleep(1000);
            }

            BugLB.SelectedIndex = 3;
            BugLB.SelectedValue = "1";            

            // Items with Pri 1 have indicies 0 and 2 with no guarantee which gets selected.
            if ((BugLB.SelectedIndex != 0) && (BugLB.SelectedIndex != 2))
            {
                LogComment("Fail - Actual SelectedIndex: " + BugLB.SelectedIndex + ". Expected: 0 or 2 (indeterminate)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion

        #region CrashNullValue
        private TestResult CrashNullValue()
        {
            Status("CrashNullValue");

            ComboBox cb = (ComboBox)(LogicalTreeHelper.FindLogicalNode(_page, "cb"));
            object[] itemsSource = (object[])(cb.ItemsSource);
            object obj1 = itemsSource[0];
            if (obj1 != null)
            {
                LogComment("Fail - obj1 should be null, instead it is " + obj1);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
        #endregion
    }
}

