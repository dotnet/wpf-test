// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.UIAutomaion;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing ScrollBar AutomationId
    /// </summary>
    [Serializable]
    public class UiaScrollBarTest : UiaSimpleTestcase
    {
        public override void Init(object target)
        {
        }

        public override void DoTest(AutomationElement target)
        {
            AutomationElementCollection aec = target.FindAll(System.Windows.Automation.TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
            if (target.Current.AutomationId.ToLowerInvariant() == "hscrollbar")
            {
                if (((AutomationElement)aec[0]).Current.AutomationId != "LineLeft")
                {
                    SharedState["targetHScrollBarLineLeft"] = "fail";
                }
                if (((AutomationElement)aec[1]).Current.AutomationId != "PageLeft")
                {
                    SharedState["targetHScrollBarPageLeft"] = "fail";
                }
                if (((AutomationElement)aec[2]).Current.AutomationId != "PageRight")
                {
                    SharedState["targetHScrollBarPageRight"] = "fail";
                }
                if (((AutomationElement)aec[4]).Current.AutomationId != "LineRight")
                {
                    SharedState["targetHScrollBarLineRight"] = "fail";
                }
            }
            if (target.Current.AutomationId.ToLowerInvariant() == "vscrollbar")
            {
                if (((AutomationElement)aec[0]).Current.AutomationId != "LineUp")
                {
                    SharedState["targetVScrollBarLineUp"] = "fail";
                }
                if (((AutomationElement)aec[1]).Current.AutomationId != "PageUp")
                {
                    SharedState["targetVScrollBarPageUp"] = "fail";
                }
                if (((AutomationElement)aec[2]).Current.AutomationId != "PageDown")
                {
                    SharedState["targetVScrollBarPageDown"] = "fail";
                }
                if (((AutomationElement)aec[4]).Current.AutomationId != "LineDown")
                {
                    SharedState["targetVScrollBarLineDown"] = "fail";
                }
            }
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            if (((ScrollBar)target).Name.ToLowerInvariant() == "hscrollbar")
            {
                if ((string)SharedState["targetHScrollBarLineLeft"] == "fail")
                {
                    TestLog.Current.LogEvidence("Horizontal ScrollBar first child AutomationId is not LineLeft");
                    TestLog.Current.Result = TestResult.Fail;
                }
                if ((string)SharedState["targetHScrollBarPageLeft"] == "fail")
                {
                    TestLog.Current.LogEvidence("Horizontal ScrollBar second child AutomationId is not PageLeft");
                    TestLog.Current.Result = TestResult.Fail;
                }
                if ((string)SharedState["targetHScrollBarPageRight"] == "fail")
                {
                    TestLog.Current.LogEvidence("Horizontal ScrollBar third child AutomationId is not PageRight");
                    TestLog.Current.Result = TestResult.Fail;
                }
                if ((string)SharedState["targetHScrollBarLineRight"] == "fail")
                {
                    TestLog.Current.LogEvidence("Horizontal ScrollBar fifth child AutomationId is not LineRight");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
            if (((ScrollBar)target).Name.ToLowerInvariant() == "vscrollbar")
            {
                if ((string)SharedState["targetVScrollBarLineUp"] == "fail")
                {
                    TestLog.Current.LogEvidence("Vertical ScrollBar first child AutomationId is not LineUp");
                    TestLog.Current.Result = TestResult.Fail;
                }
                if ((string)SharedState["targetVScrollBarPageUp"] == "fail")
                {
                    TestLog.Current.LogEvidence("Vertical ScrollBar second child AutomationId is not PageUp");
                    TestLog.Current.Result = TestResult.Fail;
                }
                if ((string)SharedState["targetVScrollBarPageDown"] == "fail")
                {
                    TestLog.Current.LogEvidence("Vertical ScrollBar third child AutomationId is not PageDown");
                    TestLog.Current.Result = TestResult.Fail;
                }
                if ((string)SharedState["targetVScrollBarLineDown"] == "fail")
                {
                    TestLog.Current.LogEvidence("Vertical ScrollBar fifth child AutomationId is not LineDown");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }

        }
    }
}
