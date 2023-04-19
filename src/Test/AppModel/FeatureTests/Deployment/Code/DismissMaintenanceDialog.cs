// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using MTI = Microsoft.Test.Input;


/*****************************************************
 * The logic in this file is maintained by the AppModel team
 * contact: Microsoft
 *****************************************************/

namespace Microsoft.Test.Deployment
{


    /// <summary>
    /// Specifies the action to take on the Deployment Maintenance dialog
    /// </summary>
    public enum MaintenanceDialogAction
    {
        /// <summary>
        /// Attach to a testLog, check to see if Restore button is enabled,
        /// then click the "Remove the Application" on the Maintenance dialog
        /// </summary>
        CheckRestoreAndRemove,
        /// <summary>
        /// Select "Remove the application" on the Maintenance dialog
        /// </summary>
        Remove,
        /// <summary>
        /// Select "Restore the application" on the Maintenance dialog
        /// </summary>
        Restore,
        /// <summary>
        /// Click the Cancel button on the Maintenance dialog
        /// </summary>
        Cancel,
        /// <summary>
        /// Click the Help button on the Maintenance dialog
        /// </summary>
        Help,
        /// <summary>
        /// Click the OK button on the "Application restored" dialog.
        /// </summary>
        RestoreOK
    }


    /// <summary>
    /// Handles the Update dialog
    /// </summary>
    public class DismissUpdateDialog : UIHandler
    {
        /// <summary>
        /// Uses UI Automation to click on the "OK" button of an update dialog
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            //Sleep a second to make sure that the dialog is ready
            Thread.Sleep(2000);

            // Find the run button and click it
            AutomationElement theWindow = AutomationElement.FromHandle(topLevelhWnd);
            PropertyCondition OKCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, "btnOk");
            AutomationElement OKButton = theWindow.FindFirst(TreeScope.Descendants, OKCondition);

            InvokePattern ip = OKButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            ip.Invoke();

            return UIHandlerAction.Handled;
        }
    }


    /// <summary>
    /// Handles the Maintenance/Uninstall dialog
    /// </summary>
    public class DismissMaintenanceDialog : UIHandler
    {
        // Automation IDs of the important buttons.  
        private static string s_restoreString = "radioRestore";
        private static string s_removeString = "radioRemove";
        private static string s_okString = "btnOk";

        private MaintenanceDialogAction _action;

        /// <summary>
        /// Create a new DismissMaintenanceDialog handler to perform the specified action
        /// </summary>
        /// <param name="myAction">action to be performed on the dialog</param>
        public DismissMaintenanceDialog(MaintenanceDialogAction myAction)
        {
            _action = myAction;
        }

        /// <summary>
        /// Create a new DismissMaintenanceDialog handler to Remove the application
        /// </summary>
        public DismissMaintenanceDialog()
        {
            _action = MaintenanceDialogAction.Remove;
        }

        /// <summary>
        /// Performs the specifed action on the Maintenance window
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement maintenanceDialogWindow = AutomationElement.FromHandle(topLevelhWnd);

            switch (_action)
            {
                case MaintenanceDialogAction.RestoreOK:
                    {
                        GlobalLog.LogDebug("Handling 'Application Restored' dialog...");
                        AutomationElement theDialog = AutomationElement.FromHandle(topLevelhWnd);
                        AutomationElement RestoredButton = IEAutomationHelper.WaitForElementWithAutomationId(AutomationElement.RootElement, "2", 10);
                        InvokeElement(RestoredButton);

                        break;
                    }
                case MaintenanceDialogAction.Help:
                    {
                        GlobalLog.LogDebug("Tabbing to Help button");
                        // tab to Help button
                        for (int i = 0; i < 2; i++)
                        {
                            MTI.Input.SendKeyboardInput(Key.Tab, true);
                            MTI.Input.SendKeyboardInput(Key.Tab, false);
                        }
                        break;
                    }
                case MaintenanceDialogAction.Restore:
                    {
                        GlobalLog.LogDebug("Selecting Restore option");

                        PropertyCondition restoreBtnCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, s_restoreString);
                        AutomationElement restoreButton = maintenanceDialogWindow.FindFirst(TreeScope.Descendants, restoreBtnCondition);
                        if (restoreButton != null)
                        {
                            if ((bool)restoreButton.GetCurrentPropertyValue(AutomationElement.IsEnabledProperty))
                            {
                                GlobalLog.LogEvidence("Restore Button was enabled.");
                            }
                        }
                        else
                        {
                            GlobalLog.LogEvidence("Restore Button was NOT enabled.");
                        }
                        // Find the OK button and click it 
                        PropertyCondition OKCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, s_okString);
                        AutomationElement OKButton = maintenanceDialogWindow.FindFirst(TreeScope.Descendants, OKCondition);
                        GlobalLog.LogEvidence("Clicking OK Button");
                        InvokeElement(OKButton);

                        break;
                    }
                case MaintenanceDialogAction.CheckRestoreAndRemove:
                    {
                        TestLog testLog = TestLog.Current;
                        if (testLog == null)
                        {
                            throw new InvalidOperationException("CheckRestoreAndRemove must be used in the context of a test log");
                        }
                        GlobalLog.LogDebug("Check Restore and Remove is selected");

                        PropertyCondition restoreBtnCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, s_restoreString);
                        AutomationElement restoreButton = maintenanceDialogWindow.FindFirst(TreeScope.Descendants, restoreBtnCondition);
                        if (restoreButton != null)
                        {
                            if ((bool)restoreButton.GetCurrentPropertyValue(AutomationElement.IsEnabledProperty))
                            {
                                GlobalLog.LogEvidence("Restore Button was enabled.");
                                testLog.Result = TestResult.Pass;
                            }
                        }
                        else
                        {
                            GlobalLog.LogEvidence("Restore Button was NOT enabled.");
                            testLog.Result = TestResult.Fail;
                        }

                        PropertyCondition removeBtnCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, s_removeString);
                        AutomationElement removeButton = maintenanceDialogWindow.FindFirst(TreeScope.Descendants, removeBtnCondition);
                        GlobalLog.LogEvidence("Selecting Remove Radio Button");
                        SelectElement(removeButton);

                        // Find the OK button and click it 
                        PropertyCondition OKCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, s_okString);
                        AutomationElement OKButton = maintenanceDialogWindow.FindFirst(TreeScope.Descendants, OKCondition);
                        GlobalLog.LogEvidence("Clicking OK Button");
                        InvokeElement(OKButton);

                        break;
                    }
                case MaintenanceDialogAction.Remove:
                    {
                        GlobalLog.LogDebug("Remove is selected");
                        // Find the remove button and click it
                        PropertyCondition removeBtnCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, s_removeString);
                        AutomationElement removeButton = maintenanceDialogWindow.FindFirst(TreeScope.Descendants, removeBtnCondition);
                        SelectElement(removeButton);

                        // Find the OK button and click it 
                        PropertyCondition OKCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, s_okString);
                        AutomationElement OKButton = maintenanceDialogWindow.FindFirst(TreeScope.Descendants, OKCondition);
                        InvokeElement(OKButton);

                        break;
                    }
                default:
                case MaintenanceDialogAction.Cancel:
                    {
                        GlobalLog.LogDebug("Tabbing to Cancel button");

                        MTI.Input.SendKeyboardInput(Key.Tab, true);
                        MTI.Input.SendKeyboardInput(Key.Tab, false);
                        break;
                    }
            }

            return UIHandlerAction.Handled;
        }

        private static void InvokeElement(AutomationElement element)
        {
            try
            {
                InvokePattern ip = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                ip.Invoke();
            }
            catch (Exception exc)
            {
                GlobalLog.LogDebug("Hit exception invoking ClickOnce app maintenance dialog buttons: " + exc.Message + "\n" + exc.StackTrace);
            }
        }

        private static void SelectElement(AutomationElement element)
        {
            try
            {
                SelectionItemPattern sip = element.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                sip.Select();
            }
            catch (Exception exc)
            {
                GlobalLog.LogDebug("Hit exception selecting ClickOnce app maintenance dialog radio buttons: " + exc.Message + "\n" + exc.StackTrace);
            }
        }
    }
}
