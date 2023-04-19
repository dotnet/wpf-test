// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using System.Windows.Automation;
using System.Diagnostics;
using System.Windows.Input;
using MTI = Microsoft.Test.Input;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomUIHandlers
{
    /// <summary>
    /// UI handler
    /// </summary>
    public class LinksExplorerHandler: UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public LinksExplorerHandler()
        {
        }

        /// <summary>
        /// HandleWindow
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            if (ApplicationDeploymentHelper.GetIEVersion() < 7)
            {
                GlobalLog.LogEvidence("PASS: Links explorer only exists in IE7 or greater");
                TestLog.Current.Result = TestResult.Pass;
                return UIHandlerAction.Abort;
            }

            AutomationElement ieWindow = AutomationElement.FromHandle(topHwnd);
            ieWindow.SetFocus();

            bool testPasses = true;

            // Try each scenario...
            testPasses = testPasses && ValidateLinksExplorerPaneShown("History", Key.H, false);
            testPasses = testPasses && ValidateLinksExplorerPaneShown("Favorites", Key.I, false);

            if (ApplicationDeploymentHelper.GetIEVersion() > 8)
            {
                testPasses = testPasses && ValidateLinksExplorerPaneShown("Tree View", Key.C, true);
            }
            else
            {
                testPasses = testPasses && ValidateLinksExplorerPaneShown("Tree View", Key.J, false);
            }
            

            // Close it up when done.
            ToggleLinksExplorerWindow("Favorites", Key.I, false, false);

            if (testPasses)
            {
                GlobalLog.LogEvidence("Pass - All Links-Explorer-related accelerator keys working");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Fail - One or more Links-Explorer-related accelerator keys failed");
                TestLog.Current.Result = TestResult.Fail;
            }

            return UIHandlerAction.Abort;
        }

        private static bool ValidateLinksExplorerPaneShown(string paneAutomationName, Key ctrlKey, bool useAltKeyModifier)
        {
            if (ToggleLinksExplorerWindow(paneAutomationName, ctrlKey, false, useAltKeyModifier))
            {
                GlobalLog.LogDebug("Made Links Explorer not visible...");
                if (ToggleLinksExplorerWindow(paneAutomationName, ctrlKey, true, useAltKeyModifier))
                {
                    GlobalLog.LogEvidence("SUCCESS: " + paneAutomationName + " window made visible when " + (useAltKeyModifier ? "Alt-" : "Ctrl-") + ctrlKey.ToString() + " pressed");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("ERROR: " + paneAutomationName + " window not made visible when " + (useAltKeyModifier ? "Alt-" : "Ctrl-") + ctrlKey.ToString() + " pressed");
                    return false;
                }
            }
            else
            {
                GlobalLog.LogEvidence("Failed trying to ensure that Links Explorer pane not visible... likely an issue with automation code, not a product bug");
                return false;
            }
        }

        private static bool ToggleLinksExplorerWindow(string childElementName, Key ctrlKey, bool show, bool useAltKey)
        {
            PropertyCondition isIEWindow = new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame");
            AutomationElement ieWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children, isIEWindow);

            if (!show)
            {
                MTI.Input.SendKeyboardInput(Key.Escape, true);
                MTI.Input.SendKeyboardInput(Key.Escape, false);
                return true;
            }

            // Check to see if Links Explorer pane is visible
            PropertyCondition paneVisible = new PropertyCondition(AutomationElement.ClassNameProperty, "Links Explorer");
            AutomationElement linksExplorerPane = ieWindow.FindFirst(TreeScope.Descendants, paneVisible);

            if (useAltKey)
            {
                GlobalLog.LogDebug("Pressing Alt-" + ctrlKey.ToString());
            }
            else
            {
                GlobalLog.LogDebug("Pressing Ctrl-" + ctrlKey.ToString());
            }
            // if it isnt, show it and get a reference to it
            int timesTried = 0;
            while ((linksExplorerPane == null) && (timesTried < 60))
            {
                if (useAltKey)
                {
                    MTI.Input.SendKeyboardInput(Key.LeftAlt, true);
                    MTI.Input.SendKeyboardInput(ctrlKey, true);
                    MTI.Input.SendKeyboardInput(Key.LeftAlt, false);
                    MTI.Input.SendKeyboardInput(ctrlKey, false);
                }
                else
                {
                    MTI.Input.SendKeyboardInput(Key.LeftCtrl, true);
                    MTI.Input.SendKeyboardInput(ctrlKey, true);
                    MTI.Input.SendKeyboardInput(Key.LeftCtrl, false);
                    MTI.Input.SendKeyboardInput(ctrlKey, false);
                }
                Thread.Sleep(1000);
                linksExplorerPane = ieWindow.FindFirst(TreeScope.Descendants, paneVisible);
                ieWindow.SetFocus();
                timesTried++;
            }

            if (timesTried >= 60)
            {
                GlobalLog.LogDebug("Could not display the " + childElementName + " pane. ");
                return false;
            }
            return true;
        }
    }
}
