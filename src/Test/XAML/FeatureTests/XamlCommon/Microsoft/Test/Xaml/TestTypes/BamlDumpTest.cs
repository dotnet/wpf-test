// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /******************************************************************************
    * CLASS:          BamlDumpTest 
    ******************************************************************************/

    /// <summary>
    /// Class for calling BamlDump on the Baml files.
    /// </summary>
    public class BamlDumpTest : BamlTestType
    {
        #region Public and Protected Members

        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Runs a BamlDumpTest
        /// Calls bamldump on the baml file with the "r" switch
        /// </summary>
        public override void Run()
        {
            try
            {
                // The base class will take care of compiling the Xaml file.
                base.Run();

                GlobalLog.LogStatus("----------- Starting BamlDumpTest -----------");

                // Ensures that the test starts on a clean slate
                LocalTestResult = TestResult.Unknown;

                ExecuteLocBamlPath();

                LocalTestResult = TestResult.Pass;
            }
            finally
            {
                if (this.GetType().Equals(typeof(BamlDumpTest)))
                {
                    CleanUp();
                }
            }
        }

        #endregion

        #region PrivateMembers

        /// <summary>
        /// ExecuteLocBamlPath - Calls bamldump on the baml file with the "r" switch
        /// </summary>
        public void ExecuteLocBamlPath()
        {
            string args = BamlFileName + " r";

            if (File.Exists("BamlDump.exe") == false)
            {
                throw new TestValidationException("------- BamlDump.exe is not present in current folder -------");
            }

            GlobalLog.LogStatus(args + "========");
            ProcessStartInfo startInfo = new ProcessStartInfo("Bamldump.exe", args);
            startInfo.UseShellExecute = false;
            startInfo.ErrorDialog = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            try
            {
                Process bamlDumpProc = Process.Start(startInfo);
                System.IO.StreamReader streamReader = bamlDumpProc.StandardOutput;
                string processOutput = streamReader.ReadToEnd();
                streamReader.Close();

                if (bamlDumpProc.WaitForExit(5000) == false)
                {
                    throw new TestValidationException("BamlDump is taking longer than the allocated time");
                }

                if (processOutput.Length > 0)
                {
                    GlobalLog.LogStatus(processOutput);
                    throw new TestValidationException("------------- FAIL: BamlDumpTest failed. ------------- ");
                }
            }
            catch (Exception exception)
            {
                throw new TestValidationException(exception.Message);
            }
        }

        #endregion
    }
}
