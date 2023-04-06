// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.CompilerServices;
using Microsoft.Test.CompilerServices.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Test.WPF.AppModel.Compilation
{
    public class MultiTargeting 
    {
        private static TestLog s_log;

        public static void EntryPoint()
        {
            InitializeLog();

            /// Read the name of the proj file to be built from Compilation.xtc
            string projectFileName = DriverState.DriverParameters["Project"];

            /// Build the Project
            s_log.LogStatus("Compiling Project: {0}", projectFileName);
            CompilationHelper helper = new CompilationHelper();
            if (false == helper.CompileAndEvaluateResult(projectFileName, null, null, false))
            {
                s_log.LogStatus("Compilation Failed");
                s_log.Result = TestResult.Fail;
                s_log.Close();
                ApplicationMonitor.NotifyStopMonitoring();
                return;
            }

            /// Verify that the expected binary was built 
            /// Note1: If the compilers are able to build and binplace the binary without errors, we trust that the compiler 
            /// has built the binary correctly. 
            /// Note 2: For XBaps and Click-once application, it would be good to verify the manifests. I may add this in the future 
            string binPath = DriverState.DriverParameters["BinaryPath"];
            s_log.LogStatus("Binary Path: (expected:observed) : ({0}:{1})", binPath, helper.BinaryPath);

            s_log.Result = binPath.Equals(helper.BinaryPath) ? TestResult.Pass : TestResult.Fail;
            if (s_log.Result == TestResult.Fail)
            {
                s_log.LogStatus("Run LatestReports.bat -> Click on \"Files\" -> analyse \"_buildLog.txt\""); 
            }
            s_log.Close();
            ApplicationMonitor.NotifyStopMonitoring();
        }

        private static void InitializeLog()
        {
            if (TestLog.Current == null)
            {
                s_log = new TestLog("MultiTargetingTest");
            }
            else
            {
                s_log = TestLog.Current;
            }
        }

        /// <summary>
        /// Logs the errors and warnings.
        /// </summary>
        /// <param name="helper">The helper.</param>
        private void LogErrorsAndWarnings(CompilationHelper helper)
        {
            if (helper.Unexpected.Errors.Count != 0)
            {
                GlobalLog.LogEvidence("Unexpected Build Errors:");
                foreach (BuildStatus error in helper.Unexpected.Errors)
                {
                    GlobalLog.LogEvidence(error.File + "  Line:" + error.LineNumber + " " + error.Code + "  " + error.Message);
                }
            }

            if (helper.Unencountered.Errors.Count != 0)
            {
                GlobalLog.LogEvidence("Unencountered expected Build Errors:");
                foreach (BuildStatus error in helper.Unencountered.Errors)
                {
                    GlobalLog.LogEvidence(error.File + "  Line:" + error.LineNumber + " " + error.Code + "  " + error.Message);
                }
            }

            if (helper.Unexpected.Warnings.Count != 0)
            {
                GlobalLog.LogEvidence("Unexpected Build Warnings:");
                foreach (BuildStatus warning in helper.Unexpected.Warnings)
                {
                    GlobalLog.LogEvidence(warning.File + "  Line:" + warning.LineNumber + " " + warning.Code + "  " + warning.Message);
                }
            }

            if (helper.Unencountered.Warnings.Count != 0)
            {
                GlobalLog.LogEvidence("Unencountered expected Build Warnings:");
                foreach (BuildStatus warning in helper.Unencountered.Warnings)
                {
                    GlobalLog.LogEvidence(warning.File + "  Line:" + warning.LineNumber + " " + warning.Code + "  " + warning.Message);
                }
            }
        }
    }
}
