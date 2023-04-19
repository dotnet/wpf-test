// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using System.Windows.Automation;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Used to corrupt Deployment Manifest files (.xbap and .application) to exercise 
    /// Deployment error UI.  
    /// </summary>
    public class IEUnknownContentTestHandler : UIHandler
    {

        #region Step Implementation

        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement theWindow = AutomationElement.FromHandle(topLevelhWnd);
            IEAutomationHelper.WaitForElementWithAutomationId(theWindow, "unknownFileLink", 30);
            IEAutomationHelper.ClickCenterOfElementById(theWindow, "unknownFileLink");
            AutomationElement newDialog = null;

            int countDown = 30;

            while ((newDialog == null) && countDown > 0)
            {
                countDown--;
                Thread.Sleep(1000);

                if (ApplicationDeploymentHelper.GetIEVersion() >= 9)
                {
                    GlobalLog.LogEvidence("Looking for IE 9 unknown content dialog...");

                    AutomationElement notifyBar = IEAutomationHelper.WaitForElementByAndCondition(AutomationElement.RootElement, new AndCondition(
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar),
                        new PropertyCondition(AutomationElement.NameProperty, "Notification bar")), 20);

                    newDialog = notifyBar.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Notification bar Text"));

                    ValuePattern notifyBarValuePattern = newDialog.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                    if (notifyBarValuePattern.Current.Value.ToString().Contains("deploy_un.known"))
                    {
                        GlobalLog.LogEvidence("PASS: Saw the \"Unknown content\" dialog on navigation to unknown content.");
                        TestLog.Current.Result = TestResult.Pass;
                    }
                    else
                    {
                        GlobalLog.LogEvidence("FAIL: Did not see the \"Unknown content\" dialog on navigation to unknown content ");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    return UIHandlerAction.Abort;
                }
                else
                {
                    newDialog = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "#32770"));
                }
            }

            if (newDialog != null)
            {
                GlobalLog.LogEvidence("PASS: Saw the \"Unknown content\" dialog on navigation to unknown content.");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("FAIL: Did not see the \"Unknown content\" dialog on navigation to unknown content within 30 seconds.");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }

        #endregion
    }
}
