// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Resources;
using System.IO;
using System.Diagnostics;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;

namespace Microsoft.Test.Deployment
{
	/// <summary>
	/// Verifies that the machine is in the correct state after deployment
	/// </summary>
	internal sealed class MachineStateVerifier
	{
		private string _appName;

		public MachineStateVerifier(string appName)
		{
			this._appName = appName;
		}

		public bool StartMenuShortcutsExist()
		{
            // The dir after programs needs to be the same as the apps "PublisherName" property or it wont pass
            string startMenuFolder = ApplicationDeploymentHelper.GetAppShortcutDirectory(_appName);

			bool foundShortCut = false;
			bool foundSupportLink = false;

            // This type doesn't exist in .NET Core, replacing with a hard coded value from .NET Framework, but this may cause running in localized machines to fail.
            //ResourceManager resMan = new ResourceManager("System.Deployment", typeof(System.Deployment.Application.ApplicationDeployment).Assembly);
            string supportLinkFileName = startMenuFolder + "\\" + String.Format("{0} online support", _appName) + ".url";
			string shortCutFileName = startMenuFolder + @"\" + _appName + ".appref-ms";


			GlobalLog.LogEvidence("Looking for files:\n " + shortCutFileName + "\n" + supportLinkFileName);

			if (foundSupportLink = File.Exists(supportLinkFileName))
			{
				GlobalLog.LogEvidence("Found support link");
			}
			else
			{
				GlobalLog.LogEvidence("Could not find support link");
			}

			if (foundShortCut = File.Exists(shortCutFileName))
			{
				GlobalLog.LogEvidence("Found shortcut");
			}
			else
			{
				GlobalLog.LogEvidence("Could not find shortcut");
			}

			return foundShortCut && foundSupportLink;
		}

		public bool IsProcessRunning(string processName, int numProcesses)
		{
            // processes don't always appear or disappear immediately
            // this gives them 10 seconds to appear
            int tries = 0;
            Process[] processes = Process.GetProcessesByName(processName);

            while ((processes == null || processes.Length != numProcesses) && tries <= 10)
            {
                tries++;
                Thread.Sleep(1000);
                processes = Process.GetProcessesByName(processName);
            }

            if (processes != null && processes.Length == numProcesses)
			{
				GlobalLog.LogEvidence("Found " + numProcesses + " " + processName + " process(es)");
				return true;
			}
			else
			{
				GlobalLog.LogEvidence("Found " + processes.Length + " " + processName + " process(es) where there should have been " + numProcesses);
				return false;
			}
		}
	}
}
