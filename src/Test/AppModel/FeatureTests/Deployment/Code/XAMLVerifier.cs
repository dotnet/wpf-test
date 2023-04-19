// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using System.IO;
using System.Windows.Automation;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Appverifier for simple XAML markup 
    /// </summary>
    public class FireFoxXAMLVerifier : AppVerifier
    {
        #region Constructors

        /// <summary>
        /// Constructor : Adds iexplore and presentationhost to list of processes to monitor
        /// </summary>
        public FireFoxXAMLVerifier()
        {
            GlobalLog.LogDebug("Entering constructor for  " + this.ToString());

            // check for these processes
            this.ProcessesToCheck.Add("firefox");
            this.ProcessesToCheck.Add("presentationhost");
            this.AppShouldBeInStore = false;
            this.ShouldNotSeeTM = true;
        }

        #endregion

        #region Public Members

        public string AccessibilityTestElement = "";

        /// <summary>
        /// Calls base HandleWindow()
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            UIHandlerAction result = UIHandlerAction.Unhandled;

            if (AccessibilityTestElement != "")
            {
                if (IEAutomationHelper.WaitForElementWithAutomationId(topLevelhWnd, AccessibilityTestElement, 25) != null)
                {
                    GlobalLog.LogEvidence("Expected element " + AccessibilityTestElement + " found, continuing verification");
                    result = base.HandleWindow(topLevelhWnd, hwnd, process, title, notification);
                }
                else
                {
                    GlobalLog.LogEvidence("Expected element " + AccessibilityTestElement + " not found, aborting verification");
                    result = UIHandlerAction.Abort;
                }
            }
            else
            {
                result = base.HandleWindow(topLevelhWnd, hwnd, process, title, notification);
            }

            return result;
        }

        #endregion
    }

    /// <summary>
    /// Uses databound value in test XAML to ensure that we're running on latest CLR
    /// </summary>
    public class XamlVersionCheckHandler : UIHandler
    {
        /// <summary>
        /// String to check for in running Xaml; indicates CLR this Xaml is running against
        /// </summary>
        public string MajorMinorExpectedVersion = null;

        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement thisWindow = AutomationElement.FromHandle(topLevelhWnd);

            // Assume 3.0 is the minimum version on this box, since this code only runs if a window containing Xaml loads.
            Version highestVersionSeen = new Version(3, 0);
            List<string> versionsSeen = new List<string>(4);

            GlobalLog.LogEvidence("Frameworks detected on machine: ");

            // A for loop is cool and future proof, but is worthless for SxS scenarios... 
            // So, check the registry...
            try // 3.0
            {
                if ((int)(Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\NET Framework Setup\NDP\v3.0", "Install", -1)) == 1)
                {
                    highestVersionSeen = new Version(3, 0);
                    GlobalLog.LogEvidence("  .NET 3.0");
                    versionsSeen.Add("3.0");
                    versionsSeen.Add("2.0");
                }
            }
            catch { /* Do nothing, may see NullRefs when this fwk is not installed */ }
            try // 3.5
            {
                if ((int)(Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\NET Framework Setup\NDP\v3.5", "Install", -1)) == 1)
                {
                    highestVersionSeen = new Version(3, 5);
                    GlobalLog.LogEvidence("  .NET 3.5");
                    versionsSeen.Add("3.5");
                }
            }
            catch { /* Do nothing, may see NullRefs when this fwk is not installed */ }

            try
            {
                // 4.0.  We don't care about Client/Extended here: both must work w/ Loose Xaml.
                if ((int)(Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\NET Framework Setup\NDP\v4\Client", "Install", -1)) == 1)
                {
                    highestVersionSeen = new Version(4, 0);
                    GlobalLog.LogEvidence("  .NET 4.0");
                    versionsSeen.Add("4.0");
                }
            }
            catch { /* Do nothing, may see NullRefs when this fwk is not installed */ }

            try
            {
                // 5.0+, assuming of course that it follows the same scheme as v4.0.  If this works, I'll only need to add new Xaml and add to the config file.
                int versionGreaterThanV4 = 5;
                while ((int)(Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\NET Framework Setup\NDP\v" + versionGreaterThanV4 + @"\Client", "Install", -1)) == 1)
                {
                    highestVersionSeen = new Version(versionGreaterThanV4, 0);
                    GlobalLog.LogEvidence("  .NET " + versionGreaterThanV4 );
                    versionsSeen.Add(versionGreaterThanV4.ToString() + ".0");
                    versionGreaterThanV4++;
                }
            }
            catch { /* Do nothing, may see NullRefs when this fwk is not installed */ }            

            string majorMinorString;

            // If we dont specify a version, OR that version was not found, we mean "latest"
            if ((MajorMinorExpectedVersion == null) || !versionsSeen.Contains(MajorMinorExpectedVersion))
            {
                majorMinorString = highestVersionSeen.Major.ToString() + "." + highestVersionSeen.Minor.ToString();

                // If it's 3.0, 3.5, or 3.5 SP1, the version is actually 2.0...
                if (highestVersionSeen.Major == 3)
                {
                    majorMinorString = "2.0";
                }
            }
            else
            {
                majorMinorString = MajorMinorExpectedVersion;
            }

            AutomationElement frameworkVersionButton = IEAutomationHelper.WaitForElementWithAutomationId(thisWindow, "LoadedFrameworkVersion", 20);

            if (((string)frameworkVersionButton.GetCurrentPropertyValue(AutomationElement.NameProperty)).StartsWith(majorMinorString))
            {
                GlobalLog.LogEvidence("Success! This markup was expected to run against " + majorMinorString + " and this Xaml is running against it!");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failure! This markup was expected to run against " + majorMinorString + " but this Xaml is running against " + ((string)frameworkVersionButton.GetCurrentPropertyValue(AutomationElement.NameProperty)) + "!");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }
    }
}
