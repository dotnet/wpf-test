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
    /// Testing SelectionPattern for Controls Below
    /// ComboBox
    /// ListBox
    /// TabControl
    /// TreeView
    /// </summary>
    [Serializable]
    public class UiaSelectionTest : UiaSimpleTestcase
    {
        SelectionMode _initialSelectionMode = SelectionMode.Single;

        public SelectionMode InitialSelectionMode
        {
            get { return _initialSelectionMode; }
            set { _initialSelectionMode = value; }
        }

        public override void Init(object target)
        {
            ListBox listbox = target as ListBox;
            if (listbox != null)
            {
                //make sure that the ListBox is in the intial state
                if (listbox.SelectionMode != _initialSelectionMode)
                    listbox.SelectionMode = _initialSelectionMode;
            }
            ListView listview = target as ListView;
            if (listview != null)
            {
                if (listview.SelectionMode != _initialSelectionMode)
                    listview.SelectionMode = _initialSelectionMode;
            }
        }

        public override void DoTest(AutomationElement target)
        {
            SelectionPattern sp = target.GetCurrentPattern(SelectionPattern.Pattern) as SelectionPattern;
            SharedState["targetCanSelectMultiple"] = sp.Current.CanSelectMultiple;
            SharedState["targetIsSelectionRequired"] = sp.Current.IsSelectionRequired;
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            if (target.GetType() == typeof(TabControl))
            {
                TestLog.Current.LogEvidence("*** Testing TabControl ***");
                TabControl tabcontrol = target as TabControl;
                if (tabcontrol != null)
                {
                    if ((bool)SharedState["targetCanSelectMultiple"])
                    {
                        TestLog.Current.LogEvidence("The TabControl CanSelectMultiple shouldn't be true");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (!(bool)SharedState["targetIsSelectionRequired"])
                    {
                        TestLog.Current.LogEvidence("The IsSelectionRequired should always be true for TabControl");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() != typeof(TabControl))
            {
                TestLog.Current.LogEvidence("*** Not Testing TabControl ***");
                if ((bool)SharedState["targetIsSelectionRequired"])
                {
                    TestLog.Current.LogEvidence("The IsSelectionRequired shouldn't be true for ComboBox, ListBox and TreeView");
                    TestLog.Current.Result = TestResult.Fail;
                }
                
                if (target.GetType() == typeof(ComboBox))
                {
                    ComboBox combobox = target as ComboBox;
                    if (combobox != null)
                    {
                        if ((bool)SharedState["targetCanSelectMultiple"])
                        {
                            TestLog.Current.LogEvidence("The ComboBox CanSelectMultiple shouldn't be true");
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                }
                else if (target.GetType() == typeof(TreeView))
                {
                    TreeView treeview = target as TreeView;
                    if (treeview != null)
                    {
                        if ((bool)SharedState["targetCanSelectMultiple"])
                        {
                            TestLog.Current.LogEvidence("The TreeView CanSelectMultiple shouldn't be true");
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                }
                else if (target.GetType() == typeof(ListBox))
                {
                    ListBox listbox = target as ListBox;
                    if (listbox != null)
                    {
                        if ((bool)SharedState["targetCanSelectMultiple"] != (listbox.SelectionMode != SelectionMode.Single))
                        {
                            TestLog.Current.LogEvidence("The ListBox CanSelectMultiple doesn't work. Selection = " + listbox.SelectionMode);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                }
                else if (target.GetType() == typeof(ListView))
                {
                    ListView listview = target as ListView;
                    if (listview != null)
                    {
                        if ((bool)SharedState["targetCanSelectMultiple"] != (listview.SelectionMode != SelectionMode.Single))
                        {
                            TestLog.Current.LogEvidence("The ListView CanSelectMultiple doesn't work. Selection = " + listview.SelectionMode);
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

}
