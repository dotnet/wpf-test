// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Helper functions for running a single case and
 *          a routine (w/ overrides) for running new processes.
 * Contributors: Microsoft, Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.Globalization;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;
using System.Drawing.Printing; //for Printing Permission
using System.Diagnostics;
using System.IO;
using System.Text;

using Microsoft.Test.Logging;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Trusted
{

    ///<summary>
    ///</summary>
    public class CoreTestsSingleRunServices
    {
        ///<summary>
        ///</summary>
        static public void RunTestCaseProcess(string exeFile, int timeOut)
        {
            RunTestCaseProcess(exeFile, timeOut, "", true, false);
        }

        ///<summary>
        ///</summary>
        static public void RunTestCaseProcess(string exeFile, int timeOut, string paramsStr, bool addRemotedParam)
        {
            RunTestCaseProcess(exeFile, timeOut, paramsStr, addRemotedParam, true);
        }

        ///<summary>
        ///</summary>
        static public void RunTestCaseProcess(string exeFile, int timeOut, string paramsStr, bool addRemotedParam, bool redirectOutput)
        {
            RunTestCaseProcess(exeFile, timeOut, paramsStr, addRemotedParam, true, true);
        }

        ///<summary>
        ///</summary>
        static public void RunTestCaseProcess(string exeFile, int timeOut, string paramsStr, bool addRemotedParam, bool redirectOutput, bool startApplicationMonitor)
        {

            TimeSpan maxTimeSpan = new TimeSpan(0, 0, 0, 0, timeOut);
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.FileName = exeFile;

            // Set WorkingDirectory to exe's path if possible.
            // Otherwise, use the current working directory.
            string directory = Directory.GetCurrentDirectory();
            if (exeFile.Contains(Path.DirectorySeparatorChar.ToString()))
            {
                directory = Path.GetDirectoryName(exeFile);
            }
            startInfo.WorkingDirectory = directory;

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = redirectOutput;
            if (addRemotedParam)
                paramsStr += " /remoted";
            startInfo.Arguments = paramsStr.Trim(' ');

            Process process = new Process();

            process.StartInfo = startInfo;

            CoreLogger.LogStatus("Starting process: " + startInfo.FileName + " " + startInfo.Arguments);
            process.Start();

            if (startApplicationMonitor)
            {
                CoreLogger.MonitorProcess(process);
            }

            // Read output of process until it exits or timeout is reached.
            DateTime maxTime = (process.StartTime + maxTimeSpan);

            bool isExited = false;
            if (redirectOutput)
            {
                for (string stdout = process.StandardOutput.ReadLine();
                     stdout != null && DateTime.Now < maxTime;
                     stdout = process.StandardOutput.ReadLine())
                {
                    CoreLogger.LogStatus(stdout);
                }

                isExited = process.HasExited;
            }
            else
            {
                isExited = process.WaitForExit(timeOut);
            }

            // Check if process exited properly.
            if (!isExited)
            {
                CoreLogger.LogStatus("Killing process due to timeout...");
                process.Kill();

                // Sleep until the process exits.
                while (!process.HasExited) { Thread.Sleep(100); }

                // Forget the timeout if the process was able to log a result between the 
                // the timeout and the kill.
                if (TestLog.Current.Result == TestResult.Unknown)
                    CoreLogger.LogException(new Microsoft.Test.TestValidationException("The test case timed out..."));
                else
                {
                    CoreLogger.LogStatus("Ignoring timeout, process logged before it was killed...");
                }
            }

            if (process.ExitCode != 0)
                throw new Microsoft.Test.TestValidationException("The test case process didn't shutdown correctly");

        }

    }
}
