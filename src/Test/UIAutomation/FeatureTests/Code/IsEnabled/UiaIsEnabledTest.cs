// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Windows;
using Microsoft.Test.UIAutomaion;
using System.Threading;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing IsEnabled for Controls Below
    /// Button
    /// CheckBox
    /// ComboBox
    /// Expander
    /// Image
    /// Label
    /// Listbox
    /// ListboxItem
    /// Menu
    /// MenuItem
    /// PopupRoot       <-- no test yet
    /// ProgressBar
    /// RadioButton
    /// RepeatButton
    /// ScrollBar
    /// ScrollViewer    <-- bug
    /// Selector        <-- no test yet
    /// Separator
    /// Slider
    /// StatusBar
    /// StatusBarItem   <-- bug 
    /// TabControl
    /// TabItem
    /// Thumb
    /// ToggleButton
    /// ToolBar
    /// TreeView
    /// TreeViewItem
    /// Window          <-- no test yet
    /// MyButton
    /// </summary>
    [Serializable]
    public class UiaIsEnabledTest : UiaSimpleTestcase
    {
        bool _initialIsEnabled;

        public bool InitialIsEnabled
        {
            get { return _initialIsEnabled; }
            set { _initialIsEnabled = value; }
        }

        public override void Init(object target)
        {
            FrameworkElement fe = target as FrameworkElement;
            if (fe != null)
            {
                //make sure control is in the intial state
                if (fe.IsEnabled != _initialIsEnabled)
                    fe.IsEnabled = _initialIsEnabled;
            }
            TestLog.Current.LogEvidence("*** initialIsEnabled = " + _initialIsEnabled.ToString());
            TestLog.Current.LogEvidence("*** IsEnabled = " + fe.IsEnabled.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public override void DoTest(AutomationElement target)
        {
            QueueHelper.WaitTillTimeout(new TimeSpan(0, 0, 1));
            TestLog.Current.LogEvidence("*** " + target.Current.AutomationId.ToString() + " IsEnabled = " + target.Current.IsEnabled.ToString());
            SharedState["targetIsEnabled"] = target.Current.IsEnabled;
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;

            FrameworkElement fe = target as FrameworkElement;
            if (fe != null)
            {
                if ((bool)SharedState["targetIsEnabled"] != fe.IsEnabled)
                {
                    TestLog.Current.LogEvidence("The Control IsEnabled != AutomationElement IsEnabled");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
        }
    }

}
