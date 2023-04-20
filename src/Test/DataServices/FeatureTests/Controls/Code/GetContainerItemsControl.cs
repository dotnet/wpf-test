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

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// This tests the helper method ContainerFromElement of ItemsControl. This method is able to 
    /// find the container of a DependencyObject somewhere in the tree of one of the items.
    /// Scenarios tested:
    /// - The visual tree in the DataTemplate has visual elements only
    /// - The tree contains a Freezable (SolidColorBrush)
    /// - The tree contains a VisualBrush
    /// - The tree contains a ToolTip
    /// - The tree contains a ContextMenu
    /// - The data is grouped
    /// - The data is hierarchical with 3 levels
    /// - The element passed as a parameter is the ListBoxItem itself
    /// - Passed an element as a parameter that does not exist in the tree of that ItemsControl
    /// - Passed null and verified it throws
	/// </description>
    /// <relatedBugs>

    /// </relatedBugs>
	/// </summary>


    [Test(1, "Controls", "GetContainerItemsControl")]
	public class GetContainerItemsControl : XamlTest
	{
        private Page _page;

        public GetContainerItemsControl()
            : base(@"GetContainerItemsControl.xaml")
		{
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(GetContainerVisualsOnly);
            RunSteps += new TestStep(GetContainerNonVisuals);
            RunSteps += new TestStep(GetContainerVisualBrush);
            RunSteps += new TestStep(GetContainerToolTip);
            RunSteps += new TestStep(GetContainerContextMenu);
            RunSteps += new TestStep(GetContainerGrouping);
            RunSteps += new TestStep(GetContainerHierarchy);
            RunSteps += new TestStep(GetContainerListBoxItem);
            RunSteps += new TestStep(GetContainerNull);
            RunSteps += new TestStep(PassNullAsParameter1);
            RunSteps += new TestStep(PassNullAsParameter2);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);
            _page = (Page)(this.Window.Content);
            return TestResult.Pass;
        }

        private TestResult GetContainerVisualsOnly()
        {
            Status("GetContainerVisualsOnly");

            ListBox lb1 = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "lb1"));
            ListBoxItem lbi = (ListBoxItem)(lb1.ItemContainerGenerator.ContainerFromIndex(0));
            TextBlock tb1 = (TextBlock)(Util.FindElement(lbi, "tb1"));

            return Verify(tb1.Text, lb1, tb1);
        }

        private TestResult GetContainerNonVisuals()
        {
            Status("GetContainerNonVisuals");

            ListBox lb2 = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "lb2"));
            ListBoxItem lbi = (ListBoxItem)(lb2.ItemContainerGenerator.ContainerFromIndex(0));
            FrameworkElement[] elements = Util.FindElementsWithType(lbi, typeof(Button));
            Button btn = (Button)(elements[0]);
            SolidColorBrush brush = (SolidColorBrush)(btn.Background);

            return Verify(btn.Content.ToString(), lb2, brush);
        }

        private TestResult GetContainerVisualBrush()
        {
            Status("GetContainerVisualBrush");

            ListBox lb3 = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "lb3"));
            ListBoxItem lbi = (ListBoxItem)(lb3.ItemContainerGenerator.ContainerFromIndex(0));
            FrameworkElement[] elements = Util.FindElementsWithType(lbi, typeof(Button));
            Button btn = (Button)(elements[0]);
            VisualBrush vb = (VisualBrush)(btn.Background);
            TextBlock tb = (TextBlock)(vb.Visual);

            return Verify(tb.Text, lb3, tb);
        }

        private TestResult GetContainerToolTip()
        {
            Status("GetContainerToolTip");

            ListBox lb4 = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "lb4"));
            ListBoxItem lbi = (ListBoxItem)(lb4.ItemContainerGenerator.ContainerFromIndex(0));
            FrameworkElement[] elements = Util.FindElementsWithType(lbi, typeof(Button));
            Button btn = (Button)(elements[0]);
            ToolTip tt = (ToolTip)(btn.ToolTip);
            tt.PlacementTarget = btn;
            tt.IsOpen = true;
            WaitForPriority(DispatcherPriority.SystemIdle);
            TextBlock tb = (TextBlock)(tt.Content);

            return Verify(tb.Text, lb4, tb);
        }

        private TestResult GetContainerContextMenu()
        {
            Status("GetContainerContextMenu");

            ListBox lb5 = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "lb5"));
            ListBoxItem lbi = (ListBoxItem)(lb5.ItemContainerGenerator.ContainerFromIndex(0));
            FrameworkElement[] elements = Util.FindElementsWithType(lbi, typeof(Button));
            Button btn = (Button)(elements[0]);
            ContextMenu cm = (ContextMenu)(btn.ContextMenu);
            cm.PlacementTarget = btn;
            cm.IsOpen = true;
            TextBlock tb = (TextBlock)(cm.Items[0]);

            return Verify(tb.Text, lb5, tb);
        }

        private TestResult GetContainerGrouping()
        {
            Status("GetContainerGrouping");

            ListBox lb6 = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "lb6"));
            ListBoxItem lbi = (ListBoxItem)(lb6.ItemContainerGenerator.ContainerFromIndex(0));
            FrameworkElement[] elements = Util.FindElementsWithType(lbi, typeof(TextBlock));
            TextBlock tb = (TextBlock)(elements[0]);

            return Verify(tb.Text, lb6, tb);
        }

        private TestResult GetContainerHierarchy()
        {
            Status("GetContainerHierarchy");

            TreeView tv1 = (TreeView)(LogicalTreeHelper.FindLogicalNode(_page, "tv1"));
            TreeViewItem tvi1 = (TreeViewItem)(tv1.ItemContainerGenerator.ContainerFromIndex(0));
            TreeViewItem tvi11 = (TreeViewItem)(tvi1.ItemContainerGenerator.ContainerFromIndex(0));
            TreeViewItem tvi111 = (TreeViewItem)(tvi11.ItemContainerGenerator.ContainerFromIndex(0));
            FrameworkElement[] elements = Util.FindElementsWithType(tvi111, typeof(TextBlock));
            TextBlock tb = (TextBlock)(elements[0]);

            TestResult res1 = Verify(tb.Text, tv1, tb);
            TestResult res2 = Verify(tb.Text, tvi1, tb);
            TestResult res3 = Verify(tb.Text, tvi11, tb);

            if ((res1 == TestResult.Fail) || (res2 == TestResult.Fail) || (res3 == TestResult.Fail))
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult GetContainerListBoxItem()
        {
            Status("GetContainerListBoxItem");

            ListBox lb7 = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "lb7"));
            ListBoxItem lbi1 = (ListBoxItem)(lb7.ItemContainerGenerator.ContainerFromIndex(0));
            ListBoxItem lbi2 = (ListBoxItem)(lb7.ContainerFromElement(lbi1)); 
            ListBoxItem lbi3 = (ListBoxItem)(ItemsControl.ContainerFromElement(lb7, lbi1));

            if (!(lbi1.Equals(lbi2)) || !(lbi1.Equals(lbi3)))
            {
                LogComment("Fail - Container returned from ContainerFromElement is not correct.");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult GetContainerNull()
        {
            Status("GetContainerNull");

            ListBox lb7 = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "lb7"));
            ListBoxItem lbi1 = (ListBoxItem)(lb7.ContainerFromElement(new Button()));
            if (lbi1 != null)
            {
                LogComment("Fail - lbi1 should be null");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult PassNullAsParameter1()
        {
            Status("PassNullAsParameter1");

            this.SetExpectedErrorTypeInStep(typeof(ArgumentException));
            ListBox lb7 = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "lb7"));
            ListBoxItem lbi = (ListBoxItem)(lb7.ContainerFromElement(null));

            return TestResult.Fail;
        }

        private TestResult PassNullAsParameter2()
        {
            Status("PassNullAsParameter2");

            this.SetExpectedErrorTypeInStep(typeof(ArgumentException));
            ListBox lb7 = (ListBox)(LogicalTreeHelper.FindLogicalNode(_page, "lb7"));
            ListBoxItem lbi = (ListBoxItem)(ItemsControl.ContainerFromElement(lb7, null));

            return TestResult.Fail;
        }

        #region AuxMethods
        private TestResult Verify(string seattle, ItemsControl itemsControl, DependencyObject element)
        {
            if (seattle != "Seattle")
            {
                LogComment("Fail - Actual: " + seattle + ". Expected: Seattle");
                return TestResult.Fail;
            }

            DependencyObject do1 = itemsControl.ItemContainerGenerator.ContainerFromIndex(0);
            DependencyObject do2 = itemsControl.ContainerFromElement(element);
            DependencyObject do3 = ItemsControl.ContainerFromElement(itemsControl, element);

            if (!(do1.Equals(do2)) || !(do1.Equals(do3)))
            {
                LogComment("Fail - Container returned from ContainerFromElement is not correct.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion
	}

    public class MyBlueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Colors.SteelBlue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}

