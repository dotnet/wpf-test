// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.Test.Loaders;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Logging;
using System.Windows.Automation;

namespace Microsoft.Test.Deployment
{
	/// <summary>
	/// Appverifier for StandAlone Applications
	/// </summary>
	public class OnDemandApplicationVerifier : AppVerifier
    {
        #region Public Members

        /// <summary>
        /// Name, including extension, of assembly to look for inside the application store before and after on-demand api is called.
        /// Verifies that it is not there before using automation to invoke a button with ID "OnDemandButton", and that it is there after 
        /// </summary>
        public string OnDemandAssemblyName;

        #endregion

        /// <summary>
        /// Constructor: calls base ctor
        /// </summary>
        public OnDemandApplicationVerifier()
		{
			GlobalLog.LogDebug("Entering the constructor for " + this.ToString());
		}
        
        /// <summary>
        /// First verfies that the on-demand assembly is not in the app store, then clicks UI button 
        /// OnDemandButton, and waits until its name changes.  Once it does change to "Succeeded", 
        /// Checks one more time that assembly gets into the app store.
        /// Finally, calls base HandleWindow() for normal verification stuff.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
  	        this.TestLog = TestLog.Current;
			if (this.TestLog == null)
			{
				throw new InvalidOperationException("Must be run in the context of a test log");
			}

            // Make sure it isnt there...
            if (checkAppStoreForAssembly(this.AppName, OnDemandAssemblyName, false) == false)
            {
                this.TestLog.Result = TestResult.Fail;
                GlobalLog.LogEvidence("Failing - Found " + OnDemandAssemblyName + " before invoking on-demand API...");
                return UIHandlerAction.Abort;
            }
            else
            {
                GlobalLog.LogEvidence("App store did not contain " + OnDemandAssemblyName + " before invoking on-demand API...");
            }

            invokeOnDemandButton();

            // Make sure it showed up...
            if (checkAppStoreForAssembly(this.AppName, OnDemandAssemblyName, true) == false)
            {
                this.TestLog.Result = TestResult.Fail;
                GlobalLog.LogEvidence("Failing - Didn't find " + OnDemandAssemblyName + " after invoking on-demand API...");
                return UIHandlerAction.Abort;
            }
            else
            {
                GlobalLog.LogEvidence("App store contained " + OnDemandAssemblyName + " after invoking on-demand API...");
            }

            base.HandleWindow(topLevelhWnd, hwnd, process, title, notification);
			// Once the window has been handled, we're done: Abort out
            return UIHandlerAction.Abort;
        }

        #region Private Methods

        private void invokeOnDemandButton()
        {
            int timesToWait = 30;

            AutomationElement onDemandBtn = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "OnDemandBtn"));
            while ((onDemandBtn == null) && timesToWait > 0)
            {
                Thread.Sleep(10000);
                timesToWait --;
                GlobalLog.LogDebug("Waiting to see On-Demand-Assembly-invoking test button...");
                onDemandBtn = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "OnDemandBtn"));
            }

            if (onDemandBtn != null)
		    {
                object patternObject;
                onDemandBtn.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
                InvokePattern ip = patternObject as InvokePattern;
                ip.Invoke();
			    Thread.Sleep(5000);
			    PropertyCondition isButtonLabeledRight = new PropertyCondition(AutomationElement.NameProperty, "Succeeded");
			    AutomationElement correctlyLabeledButton = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, isButtonLabeledRight);

                while (correctlyLabeledButton == null)
                {
                    Thread.Sleep(2000);
                    correctlyLabeledButton = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, isButtonLabeledRight);
                }

                // Pass case...
			    if (correctlyLabeledButton != null)
			    {
				    GlobalLog.LogEvidence("On-Demand API invocation successful... ");

			    }
			    else // Fail case... 
			    {
				    GlobalLog.LogEvidence("Unable to invoke On-Demand API (App did not set proper name to button with automation id OnDemandBtn)");
				    this.TestLog.Result = TestResult.Fail;
			    }
		    }
		    else
		    {
			    GlobalLog.LogEvidence("Attempted to activate button with automation id 'OnDemandBtn' and it didn't exist... failing");
			    this.TestLog.Result = TestResult.Fail;
		    }
        }

        // returns true if shouldExist=true, and AssemblyName is the name of a file in the app store with AppName
        private bool checkAppStoreForAssembly(string AppName, string AssemblyName, bool shouldExist)
        {
            string STORE_FOLDER_NAME_XP = @"Apps\2.0";
            string STORE_FOLDER_NAME_VISTA = @"Local\Apps\2.0"; 
            // Get the local store path where this .exe should show up
            string localStorePath = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            string possibleLocalStorePath = Path.Combine(localStorePath, STORE_FOLDER_NAME_XP);
            if (Directory.Exists(possibleLocalStorePath))
            {
                localStorePath = possibleLocalStorePath;
            }
            else
            {
                localStorePath = Path.Combine(localStorePath, STORE_FOLDER_NAME_VISTA);
            }

            if (!Directory.Exists(localStorePath))
            {
                throw new FileNotFoundException("Couldnt find the App store at " + localStorePath + " or " + possibleLocalStorePath);
            }

            bool assemblyExists = false;
            bool foundTheExe = false;

            // Find every dir in the app store...
            string[] directoriesToCheck = Directory.GetDirectories(localStorePath, "*", SearchOption.AllDirectories);

            foreach (string path in directoriesToCheck)
            {
                string exeFile = path + @"\" + AppName + ".exe";
                string assemblyFile = path + @"\" + AssemblyName;

                if (File.Exists(exeFile))
                {
                    foundTheExe = true;
                    GlobalLog.LogDebug("Found " + AppName + ".exe in \n" + path);

                    if (File.Exists(assemblyFile))
                    {
                        GlobalLog.LogDebug("Found " + AssemblyName + " in the app store");
                        assemblyExists = true;
                    }
                    else
                    {
                        GlobalLog.LogDebug("Failed to find " + AssemblyName + " in the app store");
                        assemblyExists = false;
                    }
                }
            }

            if (!foundTheExe)
            {
                GlobalLog.LogDebug("Never found directory with " + AppName + ".exe in it!");
                return false;
            }
            if (assemblyExists)
            {
                if (shouldExist)
                {
                    GlobalLog.LogDebug("Success: Found " + AssemblyName);
                    return true;
                }
                else
                {
                    GlobalLog.LogDebug("Failure: Found " + AssemblyName + " in the app store when we expected not to.");
                    return false;
                }
            }
            else
            {
                if (!shouldExist)
                {
                    GlobalLog.LogDebug("Success: Did not find " + AssemblyName);
                    return true;
                }
                else
                {
                    GlobalLog.LogDebug("Failure: Couldn't find " + AssemblyName + " in the app store when we expected to.");
                    return false;
                }
            }
        }
        #endregion
    }
}
