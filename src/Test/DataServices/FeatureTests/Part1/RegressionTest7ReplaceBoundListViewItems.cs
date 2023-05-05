// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using LocalClasses = Microsoft.Test.DataServices.RegressionTest2;
using Microsoft.Test.DataServices.RegressionTest3; // Useful datagrid manipulation helpers here
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Threading;
using System.Reflection;

namespace Microsoft.Test.DataServices
{

    /// <summary>
    /// <description>
    ///  Regression Test - Selection bug when replacing items of a WPF ListView bound with ObservableCollection property
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "RegressionTest7ReplaceBoundListViewItems", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions="4.0GDR+,4.0GDRClient+" )]
    public class RegressionTest7ReplaceBoundListViewItems : XamlTest
    {
        ObservableCollection<string> _strings;

        #region Private Data

        private ListBox _myListBox;
        private ListView _myListView;
        private TreeView _myTreeView;

        #endregion

        #region Constructors

        public RegressionTest7ReplaceBoundListViewItems()
            : base(@"RegressionTest7ReplaceBoundListViewItems.xaml")
        {
            _strings = new ObservableCollection<string>();
            _strings.Add("first");
            _strings.Add("second");
            _strings.Add("third");

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myListView = (ListView)RootElement.FindName("myListView");
            _myTreeView = (TreeView)RootElement.FindName("myTreeView");
            _myListBox = (ListBox)RootElement.FindName("myListBox");

            if ((_myListBox == null) || (_myTreeView == null) || (_myListView == null))
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }
            _myListView.ItemsSource = _strings;
            _myTreeView.ItemsSource = _strings;
            _myListBox.ItemsSource = _strings;

            while (_myListBox.Items.Count <= 0)
            {
                Thread.Sleep(500);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }
            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            //1. Select the "second" item in the ListView, ListBox, and TreeView.
            _myListView.SelectedIndex = 1;
            _myListBox.SelectedIndex = 1;
            // Have to use reflection to select items programatically in TreeView
            DependencyObject dObject = _myTreeView.ItemContainerGenerator.ContainerFromItem(_myTreeView.Items[1]);
            MethodInfo selectMethod = typeof(TreeViewItem).GetMethod("Select", BindingFlags.NonPublic | BindingFlags.Instance);
            selectMethod.Invoke(dObject, new object[] { true });
            WaitForPriority(DispatcherPriority.SystemIdle);

            bool success = checkSelectedItems(1);

            // 2. Replace value...
            _strings[1] = ("changed value" );
            WaitForPriority(DispatcherPriority.SystemIdle);

            //3. Select "first" item
            _myListView.SelectedIndex = 0;
            _myListBox.SelectedIndex = 0;
            // (NOTE: Have to use reflection to select items programatically in TreeView)
            dObject = _myTreeView.ItemContainerGenerator.ContainerFromItem(_myTreeView.Items[0]);
            selectMethod = typeof(TreeViewItem).GetMethod("Select", BindingFlags.NonPublic | BindingFlags.Instance);
            selectMethod.Invoke(dObject, new object[] { true });
            WaitForPriority(DispatcherPriority.SystemIdle);

            // 4. Hilarity ensues:
            LogComment("Validating selection has moved from item 1 to 0...");
            success &= checkSelectedItems(0);

            if (success)
            {
                LogComment("Success, changing selection when replacing items of a bound WPF ListView with ObservableCollection property worked as expected  ");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Failure, erroneous behavior seen when changing selection while replacing items of a bound WPF ListView with ObservableCollection property worked as expected  ");
                return TestResult.Fail;
            }
        }

        private bool checkSelectedItems(int itemIndex)
        {
            bool success = true;

            int count = 0;
            LogComment("TreeView Items:");
            foreach (string item in _myTreeView.Items)
            {
                TreeViewItem treeViewItem = (TreeViewItem)_myTreeView.ItemContainerGenerator.ContainerFromItem(item);

                LogComment("Tree View item #" + count + " IsSelected = " + treeViewItem.GetValue(TreeViewItem.IsSelectedProperty).ToString());
                // Ensure that ONLY the first item is selected.
                if (count == itemIndex)
                {
                    success &= (true == (bool)(treeViewItem.GetValue(TreeViewItem.IsSelectedProperty)));
                }
                else
                {
                    success &= (false == (bool)(treeViewItem.GetValue(TreeViewItem.IsSelectedProperty)));
                }
                count++;
            }

            count = 0;
            LogComment("ListView:");
            foreach (string item in _myListView.Items)
            {
                ListViewItem listViewItem = (ListViewItem)_myListView.ItemContainerGenerator.ContainerFromItem(item);
                LogComment("ListViewItem #" + count + " IsSelected = " + listViewItem.GetValue(Selector.IsSelectedProperty).ToString());
                // Ensure that ONLY the first item is selected.
                if (count == itemIndex)
                {
                    success &= (true == (bool)(listViewItem.GetValue(Selector.IsSelectedProperty)));
                }
                else
                {
                    success &= (false == (bool)(listViewItem.GetValue(Selector.IsSelectedProperty)));
                }
                count++;
            }
            count = 0;
            LogComment("ListBox:");
            foreach (string item in _myListBox.Items)
            {
                ListBoxItem listBoxItem = (ListBoxItem)_myListBox.ItemContainerGenerator.ContainerFromItem(item);
                LogComment("ListBoxItem #" + count + "'s IsSelected property = " + listBoxItem.GetValue(Selector.IsSelectedProperty).ToString());
                // Ensure that ONLY the first item is selected.
                if (count == itemIndex)
                {
                    success &= (true == (bool)(listBoxItem.GetValue(Selector.IsSelectedProperty)));
                }
                else
                {
                    success &= (false == (bool)(listBoxItem.GetValue(Selector.IsSelectedProperty)));
                }
                count++;
            }
            return success;
        }
        #endregion
    }
}

