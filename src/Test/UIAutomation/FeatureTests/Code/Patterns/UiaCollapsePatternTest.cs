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
    /// Testing ExpandCollapsePattern Collapse for Controls Below
    /// ComboBox
    /// MenuItem
    /// Expander
    /// TreeViewItem
    /// </summary>    
    [Serializable]
    public class UiaExpandCollapsePatternCollapseTest : UiaSimpleTestcase
    {

        public override void Init(object target)
        {
        }

        public override void DoTest(AutomationElement target)
        {
            ExpandCollapsePattern ecp = target.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            ecp.Expand();
            ecp.Collapse();
            SharedState["targetExpandCollapseState"] = ecp.Current.ExpandCollapseState;
        }

        public override void Validate(object target)
        {
            if (target is ComboBox || target is MenuItem || target is Expander || target is TreeViewItem)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogEvidence("Unknown test object.");
                TestLog.Current.Result = TestResult.Fail;
            }

            MenuItem menuitem = target as MenuItem;
            if (menuitem != null)
            {
                if (menuitem.IsSubmenuOpen)
                {
                    TestLog.Current.LogEvidence("The menuitem IsSubmenuOpen is true");
                    TestLog.Current.Result = TestResult.Fail;
                }
                if ((ExpandCollapseState)SharedState["targetExpandCollapseState"] != ExpandCollapseState.Collapsed)
                {
                    TestLog.Current.LogEvidence("The menuitem ExpandCollapseState was not Collapsed");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }

            ComboBox combobox = target as ComboBox;
            if (combobox != null)
            {
                if (combobox.IsDropDownOpen)
                {
                    TestLog.Current.LogEvidence("The combobox IsDropDownOpen is true");
                    TestLog.Current.Result = TestResult.Fail;
                }
                if ((ExpandCollapseState)SharedState["targetExpandCollapseState"] != ExpandCollapseState.Collapsed)
                {
                    TestLog.Current.LogEvidence("The combobox ExpandCollapseState was not Collapsed");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }

            Expander expander = target as Expander;
            if (expander != null)
            {
                if (expander.IsExpanded)
                {
                    TestLog.Current.LogEvidence("The expander IsExpanded is true");
                    TestLog.Current.Result = TestResult.Fail;
                }
                if ((ExpandCollapseState)SharedState["targetExpandCollapseState"] != ExpandCollapseState.Collapsed)
                {
                    TestLog.Current.LogEvidence("The expander ExpandCollapseState was not Collapsed");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }

            TreeViewItem treeviewitem = target as TreeViewItem;
            if (treeviewitem != null)
            {
                if (treeviewitem.IsExpanded)
                {
                    TestLog.Current.LogEvidence("The treeviewitem IsExpanded is true");
                    TestLog.Current.Result = TestResult.Fail;
                }
                if ((ExpandCollapseState)SharedState["targetExpandCollapseState"] != ExpandCollapseState.Collapsed)
                {
                    TestLog.Current.LogEvidence("The treeviewitem ExpandCollapseState was not Collapsed");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
        }
    }

}
