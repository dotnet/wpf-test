// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Scenario 1: This tests that when setting ItemTemplate and the ContentTemplate of the ListBoxItem in 
    /// ItemContainerStyle, ItemTemplate has priority (both ItemTemplate and ItemContainerStyle are keyed)
    /// Scenario 2: This test the scenario when we have a keyed ItemTemplate and a typed ItemContainerStyle with a 
    /// ContentTemplate specified for the ListBoxItem.
    /// The ItemTemplate has priority, but when setting it to null, ItemContainerStyle was not being
    /// picked up.
    /// Scenario 3: 
	/// </description>
	/// <relatedBugs>

    /// </relatedBugs>
	/// </summary>


    [Test(2, "Controls", "ItemTemplateScenarios")]
	public class ItemTemplateScenarios : XamlTest
	{
        private Places _places;

        public ItemTemplateScenarios()
            : base(@"ItemTemplateScenarios.xaml")
		{
            InitializeSteps += new TestStep(Setup);
            // This tests that when setting ItemTemplate and the ContentTemplate of the ListBoxItem in 
            // ItemContainerStyle, ItemTemplate has priority (both ItemTemplate and ItemContainerStyle are keyed)
            RunSteps += new TestStep(ItemTemplateHasPriority);
            // This test the scenario when we have a keyed ItemTemplate and a typed ItemContainerStyle with a 
            // ContentTemplate specified for the ListBoxItem.
            // The ItemTemplate has priority, but when setting it to null, ItemContainerStyle was not being
            // picked up.
            RunSteps += new TestStep(ClearItemTemplate);
            // Scenario c) of the bug is an internal implementation detail which requires no test coverage and
            // it is not done yet. It simply means we should regenerate the items when ItemTemplate changes
            // vs keeping them up to date by have a Binding on the ListBoxItem and a TemplateBinding on the
            // ContentPresenter.
        }

        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);
            _places = (Places)(this.RootElement.Resources["places"]);
            return TestResult.Pass;
        }

        TestResult ItemTemplateHasPriority()
        {
            Status("ItemTemplateHasPriority");

            ListBox lb1 = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb1"));

            if (!CheckForeground(lb1, Colors.Green.ToString()))
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult ClearItemTemplate()
        {
            Status("ClearItemTemplate");

            ListBox lb2 = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb2"));
            lb2.ClearValue(ItemsControl.ItemTemplateProperty);

            if (!CheckForeground(lb2, Colors.Red.ToString()))
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        bool CheckForeground(ListBox lb, string color)
        {
            WaitForPriority(DispatcherPriority.Render);
            ListBoxItem lbi0 = (ListBoxItem)(lb.ItemContainerGenerator.ContainerFromItem(_places[0]));

            TextBlock tb0 = (TextBlock)(Util.FindDataVisual(lbi0, _places[0]));

            if (tb0.Foreground.ToString() != color)
            {
                LogComment("Fail - ListBoxItem's foreground should be green, instead it is " + tb0.Foreground.ToString());
                return false;
            }
            return true;
        }
    }
}

