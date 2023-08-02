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


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing SelectionItemPattern IsSelected, SelectionContainer for Controls Below
    /// ListBoxItem
    /// TabItem
    /// TreeViewItem
    /// RadioButton
    /// ComboBoxItem
    /// </summary>
    [Serializable]
    public class UiaSelectionItemTest : UiaSimpleTestcase
    {
        bool _initialIsSelected = false;

        public bool InitialIsSelected
        {
            get { return _initialIsSelected; }
            set { _initialIsSelected = value; }
        }

        public override void Init(object target)
        {
            ListBoxItem listboxitem = target as ListBoxItem;
            if (listboxitem != null)
            {
                //make sure that the ListBoxItem is in the intial state
                if (listboxitem.IsSelected != _initialIsSelected)
                    listboxitem.IsSelected = _initialIsSelected;
            }
            TabItem tabitem = target as TabItem;
            if (tabitem != null)
            {
                //make sure that the TabControl is in the intial state
                if (tabitem.IsSelected != _initialIsSelected)
                    tabitem.IsSelected = _initialIsSelected;
            }
            TreeViewItem treeviewitem = target as TreeViewItem;
            if (treeviewitem != null)
            {
                //make sure that the TreeViewItem is in the intial state
                if (treeviewitem.IsSelected != _initialIsSelected)
                    treeviewitem.IsSelected = _initialIsSelected;
            }
            ComboBoxItem comboboxitem = target as ComboBoxItem;
            if (comboboxitem != null)
            {
                //make sure that the TreeViewItem is in the intial state
                if (comboboxitem.IsSelected != _initialIsSelected)
                    comboboxitem.IsSelected = _initialIsSelected;
            }
            RadioButton radiobutton = target as RadioButton;
            if (radiobutton != null)
            {
                //make sure that the RadioButton is in the intial state
                if (radiobutton.IsChecked != _initialIsSelected)
                    radiobutton.IsChecked = _initialIsSelected;
            }
        }

        public override void DoTest(AutomationElement target)
        {
            SelectionItemPattern sip = target.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
            SharedState["targetIsSelected"] = sip.Current.IsSelected;
            if (target.Current.ControlType != ControlType.RadioButton)
            {
                SharedState["targetSelectionContainer"] = sip.Current.SelectionContainer.Current.AutomationId;
            }
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            if (target.GetType() == typeof(ListBoxItem))
            {
                TestLog.Current.LogEvidence("***Validate ListBoxItem.");
                ListBoxItem listboxitem = target as ListBoxItem;
                if (listboxitem != null)
                {
                    if (listboxitem.IsSelected != (bool)SharedState["targetIsSelected"])
                    {
                        TestLog.Current.LogEvidence("The ListBoxItem IsSelected doesn't work.");
                        TestLog.Current.LogEvidence("listboxitem.IsSelected = " + listboxitem.IsSelected.ToString());
                        TestLog.Current.LogEvidence("(bool)SharedState['targetIsSelected'] = " + SharedState["targetIsSelected"].ToString());
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (string.Compare(((ListBox)listboxitem.Parent).Name, (string)SharedState["targetSelectionContainer"], true, System.Globalization.CultureInfo.InvariantCulture) != 0)
                    {
                        TestLog.Current.LogEvidence("The ListBoxItem SelectionContainer doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(TabItem))
            {
                TabItem tabitem = target as TabItem;
                if (tabitem != null)
                {
                    if (tabitem.IsSelected != (bool)SharedState["targetIsSelected"])
                    {
                        TestLog.Current.LogEvidence("The TabItem IsSelected doesn't work.");
                        TestLog.Current.LogEvidence("tabitem.IsSelected = " + tabitem.IsSelected.ToString());
                        TestLog.Current.LogEvidence("(bool)SharedState['targetIsSelected'] = " + SharedState["targetIsSelected"].ToString());
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (string.Compare(((TabControl)tabitem.Parent).Name, (string)SharedState["targetSelectionContainer"], true, System.Globalization.CultureInfo.InvariantCulture) != 0)
                    {
                        TestLog.Current.LogEvidence("The TabItem SelectionContainer doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(TreeViewItem))
            {
                TreeViewItem treeviewitem = target as TreeViewItem;
                if (treeviewitem != null)
                {
                    if (treeviewitem.IsSelected != (bool)SharedState["targetIsSelected"])
                    {
                        TestLog.Current.LogEvidence("The TreeViewItem IsSelected doesn't work.");
                        TestLog.Current.LogEvidence("treeviewitem.IsSelected = " + treeviewitem.IsSelected.ToString());
                        TestLog.Current.LogEvidence("(bool)SharedState['targetIsSelected'] = " + SharedState["targetIsSelected"].ToString());
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (string.Compare(((TreeView)treeviewitem.Parent).Name, (string)SharedState["targetSelectionContainer"], true, System.Globalization.CultureInfo.InvariantCulture) != 0)
                    {
                        TestLog.Current.LogEvidence("The TreeViewItem SelectionContainer doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(ComboBoxItem))
            {
                TestLog.Current.LogEvidence("***Validate ComboBoxItem.");
                ComboBoxItem comboboxitem = target as ComboBoxItem;
                if (comboboxitem != null)
                {
                    if (comboboxitem.IsSelected != (bool)SharedState["targetIsSelected"])
                    {
                        TestLog.Current.LogEvidence("The ComboBoxItem IsSelected doesn't work.");
                        TestLog.Current.LogEvidence("comboboxitem.IsSelected = " + comboboxitem.IsSelected.ToString());
                        TestLog.Current.LogEvidence("(bool)SharedState['targetIsSelected'] = " + SharedState["targetIsSelected"].ToString());
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    else if (string.Compare(((ComboBox)comboboxitem.Parent).Name, (string)SharedState["targetSelectionContainer"], true, System.Globalization.CultureInfo.InvariantCulture) != 0)
                    {
                        TestLog.Current.LogEvidence("The ComboBoxItem SelectionContainer doesn't work.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(RadioButton))
            {
                RadioButton radiobutton = target as RadioButton;
                if (radiobutton != null)
                {
                    if (radiobutton.IsChecked != (bool)SharedState["targetIsSelected"])
                    {
                        TestLog.Current.LogEvidence("The radiobutton IsSelected doesn't work.");
                        TestLog.Current.LogEvidence("radiobutton.IsSelected = " + radiobutton.IsChecked.ToString());
                        TestLog.Current.LogEvidence("(bool)SharedState['targetIsSelected'] = " + SharedState["targetIsSelected"].ToString());
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else
            {
                TestLog.Current.LogEvidence("Unknown test object.");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }

}
