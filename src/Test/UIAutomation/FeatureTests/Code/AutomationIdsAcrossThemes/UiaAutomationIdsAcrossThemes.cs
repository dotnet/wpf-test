// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Collections;
using System.Threading;
using Microsoft.Test.UIAutomaion;
using Microsoft.Test.RenderingVerification;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing Controls below
    /// Button
    /// CheckBox
    /// ComboBox
    /// Expander
    /// Label
    /// Listbox
    /// ListboxItem
    /// ListView
    /// Menu
    /// MenuItem
    /// ProgressBar
    /// RadioButton
    /// RepeatButton
    /// ScrollBar
    /// Separator
    /// Slider
    /// StatusBar
    /// TabControl
    /// TabItem
    /// Thumb
    /// ToggleButton
    /// ToolBar
    /// TreeView
    /// TreeViewItem
    /// </summary>    
    [Serializable]
    public class UiaAutomationIdsAcrossThemes : UiaSimpleTestcase
    {
        public override void Init(object target)
        {
        }

        public override void DoTest(AutomationElement target)
        {
            string defaultTheme = DisplayConfiguration.GetTheme();
            string[] availableThemes = DisplayConfiguration.GetAvailableThemes();
            SharedState["defaultTheme"] = defaultTheme;
            SharedState["availableThemes"] = availableThemes;

            Hashtable[] controlsAutomationIdsTable = new Hashtable[availableThemes.Length];

            for (int i = 0; i < availableThemes.Length; i++)
            {
                DisplayConfiguration.SetTheme(availableThemes[i]);
                QueueHelper.WaitTillQueueItemsProcessed();
                controlsAutomationIdsTable[i] = new Hashtable();
                controlsAutomationIdsTable[i].Add(target.Current.AutomationId, null);
                AutomationElement ae = TreeWalker.ControlViewWalker.GetFirstChild(target);
                AutomationElement nextae = null;
                AutomationElement temp = ae;
                while (temp != null)
                {
                    nextae = TreeWalker.ControlViewWalker.GetNextSibling(temp);
                    if (!string.IsNullOrEmpty(temp.Current.AutomationId))
                    {
                        controlsAutomationIdsTable[i].Add(temp.Current.AutomationId, null);
                    }
                    temp = nextae;
                }
            }
            SharedState["controlsAutomationIds"] = controlsAutomationIdsTable;
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            string defaultTheme = (string)SharedState["defaultTheme"];
            string[] availableThemes = (string[])SharedState["availableThemes"];
            Hashtable[] controlsAutomationIdsTable = (Hashtable[])SharedState["controlsAutomationIds"];

            for (int i = 0; i < availableThemes.Length; i++)
            {
                if (controlsAutomationIdsTable[0].Count != controlsAutomationIdsTable[i].Count)
                {
                    DisplayConfiguration.SetTheme(defaultTheme);
                    TestLog.Current.LogStatus("Fail: Controls Automation Ids count is not the same across themes.");
                    TestLog.Current.Result = TestResult.Fail;
                }
                else
                {
                    foreach (object key in controlsAutomationIdsTable[0].Keys)
                    {
                        if (!controlsAutomationIdsTable[i].ContainsKey(key))
                        {
                            DisplayConfiguration.SetTheme(defaultTheme);
                            TestLog.Current.LogStatus("Fail: Controls Automation Ids are not matched across themes");
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                }
            }
            DisplayConfiguration.SetTheme(defaultTheme);
        }
    }
}
