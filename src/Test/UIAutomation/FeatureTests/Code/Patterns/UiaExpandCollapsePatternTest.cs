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
    /// Testing ExpandCollapsePattern Expand for Controls Below
    /// ComboBox
    /// MenuItem
    /// Expander
    /// TreeViewItem
    /// </summary>    
    [Serializable]
    public class UiaExpandCollapseTest : UiaSimpleTestcase
    {

        public override void Init(object target)
        {
        }

        public override void DoTest(AutomationElement target)
        {
            ExpandCollapsePattern ecp = target.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            ecp.Expand();
            SharedState["targetExpandCollapseState"] = ecp.Current.ExpandCollapseState;
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            if (target.GetType() == typeof(MenuItem))
            {
                MenuItem menuitem = target as MenuItem;
                if (menuitem != null)
                {
                    if (!menuitem.IsSubmenuOpen)
                    {
                        TestLog.Current.LogEvidence("The menuitem IsSubmenuOpen is false");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if ((ExpandCollapseState)SharedState["targetExpandCollapseState"] != ExpandCollapseState.Expanded)
                    {
                        TestLog.Current.LogEvidence("The menuitem ExpandCollapseState was not expanded");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(ComboBox))
            {
                ComboBox combobox = target as ComboBox;
                if (combobox != null)
                {
                    if (!combobox.IsDropDownOpen)
                    {
                        TestLog.Current.LogEvidence("The combobox IsDropDownOpen is false");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if ((ExpandCollapseState)SharedState["targetExpandCollapseState"] != ExpandCollapseState.Expanded)
                    {
                        TestLog.Current.LogEvidence("The combobox ExpandCollapseState was not expanded");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(Expander))
            {
                Expander expander = target as Expander;
                if (expander != null)
                {
                    if (!expander.IsExpanded)
                    {
                        TestLog.Current.LogEvidence("The expander IsExpanded is false");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if ((ExpandCollapseState)SharedState["targetExpandCollapseState"] != ExpandCollapseState.Expanded)
                    {
                        TestLog.Current.LogEvidence("The expander ExpandCollapseState was not expanded");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(TreeViewItem))
            {
                TreeViewItem treeviewitem = target as TreeViewItem;
                if (treeviewitem != null)
                {
                    if (!treeviewitem.IsExpanded)
                    {
                        TestLog.Current.LogEvidence("The treeviewitem IsExpanded is false");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if ((ExpandCollapseState)SharedState["targetExpandCollapseState"] != ExpandCollapseState.Expanded)
                    {
                        TestLog.Current.LogEvidence("The treeviewitem ExpandCollapseState was not expanded");
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
