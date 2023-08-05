// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using Microsoft.Test.UIAutomaion;
using System.Threading;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing SelectionItemPattern RemoveFromSelection for Controls Below
    /// ListBoxItem
    /// TabItem
    /// TreeViewItem
    /// ComboBoxItem
    /// </summary>
    [Serializable]
    public class UiaSelectionItemRemoveFromSelectionTest : UiaSimpleTestcase
    {
        bool _initialIsSelected = false;

        public bool InitialIsSelected
        {
            get { return _initialIsSelected; }
            set { _initialIsSelected = value; }
        }

        public override void Init(object target)
        {
            if (target.GetType() == typeof(ListBoxItem))
            {
                ListBoxItem listboxitem = target as ListBoxItem;
                if (listboxitem != null)
                {
                    //make sure that the ListBoxItem is in the initial state
                    if (listboxitem.IsSelected != _initialIsSelected)
                    {
                        listboxitem.IsSelected = _initialIsSelected;
                        TestLog.Current.LogEvidence("listboxitem.Parent = " + listboxitem.Parent.ToString());
                        TestLog.Current.LogEvidence("listboxitem.Parent.SelectedItem = " + ((ListBox)listboxitem.Parent).SelectedItem.ToString());
                        TestLog.Current.LogEvidence("listboxitem.IsSelected = " + listboxitem.IsSelected.ToString());
                        TestLog.Current.LogEvidence("initialIsSelected = " + _initialIsSelected.ToString());
                    }
                }
            }
            else if (target.GetType() == typeof(TabItem))
            {
                TabItem tabitem = target as TabItem;
                if (tabitem != null)
                {
                    //make sure that the TabControl is in the initial state
                    if (tabitem.IsSelected != _initialIsSelected)
                        tabitem.IsSelected = _initialIsSelected;
                }
            }
            else if (target.GetType() == typeof(TreeViewItem))
            {
                TreeViewItem treeviewitem = target as TreeViewItem;
                if (treeviewitem != null)
                {
                    //make sure that the TreeViewItem is in the initial state
                    if (treeviewitem.IsSelected != _initialIsSelected)
                        treeviewitem.IsSelected = _initialIsSelected;
                }
            }
            else if (target.GetType() == typeof(ComboBoxItem))
            {
                ComboBoxItem comboboxitem = target as ComboBoxItem;
                if (comboboxitem != null)
                {
                    //make sure the ComboBoxItem is in the initial state
                    if (comboboxitem.IsSelected != _initialIsSelected)
                        comboboxitem.IsSelected = _initialIsSelected;
                }
            }
        }

        public override void DoTest(AutomationElement target)
        {
            SelectionItemPattern sip = target.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
            sip.RemoveFromSelection();
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            if (target.GetType() == typeof(ListBoxItem))
            {
                ListBoxItem listboxitem = target as ListBoxItem;
                if (listboxitem != null)
                {
                    if (listboxitem.IsSelected)
                    {
                        TestLog.Current.LogEvidence("RemoveFromSelection doesn't work on ListBoxItem. The item is still selected.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(TabItem))
            {
                TabItem tabitem = target as TabItem;
                if (tabitem != null)
                {
                    if (tabitem.IsSelected)
                    {
                        TestLog.Current.LogEvidence("RemoveFromSelection doesn't work on TabItem. The item is still selected.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(TreeViewItem))
            {
                TreeViewItem treeviewitem = target as TreeViewItem;
                if (treeviewitem != null)
                {
                    if (treeviewitem.IsSelected)
                    {
                        TestLog.Current.LogEvidence("RemoveFromSelection doesn't work on TreeViewItem. The item is still selected.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(ComboBoxItem))
            {
                ComboBoxItem comboboxitem = target as ComboBoxItem;
                if (comboboxitem != null)
                {
                    if (comboboxitem.IsSelected)
                    {
                        TestLog.Current.LogEvidence("RemoveFromSelection doesn't work on ComboBoxItem. The item is still selected.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else
            {
                TestLog.Current.LogEvidence("Unknown test object.");
                TestLog.Current.Result = TestResult.Fail;
            }
            Thread.Sleep(3000);
        }
    }

}
