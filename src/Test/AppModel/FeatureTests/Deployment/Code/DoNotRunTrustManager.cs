// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading; 
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Automation;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using MTI = Microsoft.Test.Input;

/*****************************************************
 * The logic in this file is maintained by the AppModel team
 * contact: Microsoft
 *****************************************************/

namespace Microsoft.Test.Deployment {
	/// <summary>
	/// Handles the Trustmanager dialog by denying it.
	/// </summary>
	public class DoNotRunTrustManager : UIHandler
	{
        /// <summary>
        /// Testlog instance
        /// </summary>
        protected TestLog testLog;

        /// <summary>
        /// Name of app to deny running, for process verification
        /// </summary>
        public string AppName = null;

        /// <summary>
        /// If the application is browser hosted, do different verification to find that the application didn't run
        /// This is because PresentationHost.exe will still be alive after trust is not granted.
        /// </summary>
        public bool IsBrowserHostedApp = false;

        /// <summary>
		/// Answers "Do Not Run" to the Trustmanager prompt.
		/// </summary>
		/// <param name="topLevelhWnd"></param>
		/// <param name="hwnd"></param>
		/// <param name="process"></param>
		/// <param name="title"></param>
		/// <param name="notification"></param>
		/// <returns></returns>
		public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
		{
            this.testLog = TestLog.Current;
            if (this.testLog == null)
            {
                throw new InvalidOperationException("Must be run in the context of a test log");
            }
            //Sleep a second to make sure that the dialog is ready
            Thread.Sleep(2000);
            
            //Make sure that the Security Dialog has focus
            GlobalLog.LogDebug("Switching to TrustManager dialog");

            // Find the cancel button and click it
            PropertyCondition cancelCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, "btnCancel");
            AutomationElement thisWindow = AutomationElement.FromHandle(topLevelhWnd);
            AutomationElement cancelButton = thisWindow.FindFirst(TreeScope.Descendants, cancelCondition);
            if (cancelButton == null)
            {
                GlobalLog.LogEvidence("Failed to find the 'Cancel' Button... test failed.");
                this.testLog.Result = TestResult.Fail;
            }
            MTI.Input.MoveToAndClick(cancelButton);

            // Sleep another 4 seconds to let the app start if its going to
            Thread.Sleep(4000);

            if (IsBrowserHostedApp)
            {
                // FindFirst() from Root element is semi-naughty, but since we need to find a child element from a completely separate window,
                // this is easier than getting all IE / FireFox processes then searching their children.
                AutomationElement logFileButton = IEAutomationHelper.WaitForElementWithAutomationId(AutomationElement.RootElement, "LogFileButton", 20);

                if (logFileButton == null)
                {
                    GlobalLog.LogEvidence("Failure: Denying trust to prompting Xbap appears to have not led to the Trust Not Granted Page");
                    this.testLog.Result = TestResult.Fail;
                }
                else
                {
                    GlobalLog.LogEvidence("Success: Denying trust to app ended up at the Trust Not Granted Page" );
                    this.testLog.Result = TestResult.Pass;
                }               
            }
            else
            {

                System.Diagnostics.Process[] appInstances = System.Diagnostics.Process.GetProcessesByName(this.AppName);
                GlobalLog.LogEvidence("Checking for any running instances of " + this.AppName);
                if (appInstances.GetLength(0) > 0)
                {

                    foreach (System.Diagnostics.Process proc in appInstances)
                    {
                        GlobalLog.LogEvidence("Closing an instance of process " + this.AppName);
                        proc.CloseMainWindow();
                    }
                }
                else
                {
                    GlobalLog.LogEvidence("Did not find any running instances of " + this.AppName);
                    this.testLog.Result = TestResult.Pass;
                }
            }
            // Need to Abort here because Activation step is waiting for the process to be created...
            // (Which hopefully it wasn't)
            return UIHandlerAction.Abort;
        }
	}
}
