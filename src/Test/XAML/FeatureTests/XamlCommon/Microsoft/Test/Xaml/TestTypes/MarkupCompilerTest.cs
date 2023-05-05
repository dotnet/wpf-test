// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Test.CompilerServices;
using Microsoft.Test.CompilerServices.Logging;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// XamlTestType for MarkupCompilerTest
    /// </summary>
    public class MarkupCompilerTest : XamlTestType
    {
        /// <summary>
        /// Runs a Markup compiler test which consists of:
        ///     Loading a project file specifed in DriverState.DriverParameters["Project"]
        ///     Compiling the project and evaluating the result
        ///     Running the compiled application (if applicable)
        /// </summary>
        public override void Run()
        {
            Variation log = Log.Current.CreateVariation(DriverState.TestName);
            string projectFile = DriverState.DriverParameters["Project"];
            string projectDir = String.Empty;
            if (projectFile.Contains("\\"))
            {
                projectDir = projectFile.Substring(0, projectFile.LastIndexOf("\\"));
                string dstPath = Path.Combine(Environment.CurrentDirectory, projectDir);

                List<FileInfo> files = new List<FileInfo>();
                files.Add(new FileInfo(Path.Combine(Environment.CurrentDirectory, "TestCommon.Target")));

                FileUtils.CopyFilesToDirectory(files, dstPath);
            }

            bool checkWarnings = false;
            string checkWarningsString = DriverState.DriverParameters["CheckWarnings"];
            if (!string.IsNullOrEmpty(checkWarningsString))
            {
                checkWarnings = bool.Parse(checkWarningsString);
            }

            bool launchExe = true;
            string launchExeString = DriverState.DriverParameters["LaunchExe"];
            if (!string.IsNullOrEmpty(launchExeString))
            {
                launchExe = bool.Parse(launchExeString);
            }

            if (string.IsNullOrEmpty(projectFile))
            {
                GlobalLog.LogEvidence("Project cannot be null");
                log.LogResult(Result.Fail);
                log.Close();
                return;
            }

            GlobalLog.LogStatus("Compiling project file: " + projectFile);
            Compiler.EnsureMSBuildPath();
            CompilationHelper.AddSystemXamlReference(projectFile);
            CompilationHelper helper = new CompilationHelper();
            if (!helper.CompileAndEvaluateResult(projectFile, null, null, checkWarnings))
            {
                GlobalLog.LogEvidence("Compilation Failed");
                LogErrorsAndWarnings(helper);
                log.LogResult(Result.Fail);
                log.Close();
                return;
            }

            if (launchExe == false)
            {
                GlobalLog.LogStatus("Skipped executing compiled program");
                log.LogResult(Result.Pass);
                log.Close();
                return;
            }

            string binPath = DriverState.DriverParameters["BinaryPath"];
            if (string.IsNullOrEmpty(binPath))
            {
                binPath = helper.BinaryPath;
                binPath = Path.Combine(projectDir, binPath);
            }

            // If the compiled assembly is an exe we will attempt to run it
            if (binPath.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
            {
                GlobalLog.LogStatus("Compilation succeeded, will start compiled app: " + binPath);
                TimeSpan timeOutSpan = new TimeSpan(0, 0, 180);
                Process process = new Process();
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = binPath;
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
                process.StartInfo = info;
                process.Start();
                LogManager.LogProcessDangerously(process.Id);
                string output = process.StandardOutput.ReadToEnd();

                bool exited = process.WaitForExit((int)timeOutSpan.TotalMilliseconds);
                if (!exited)
                {
                    while (!process.HasExited)
                    {
                        process.Kill();
                    }
                }

                if (process.ExitCode != 0)
                {
                    GlobalLog.LogEvidence(output);
                    GlobalLog.LogEvidence("Process exited with code: " + process.ExitCode.ToString());
                    log.LogResult(Result.Fail);
                }
                else
                {
                    log.LogResult(Result.Pass);
                }

                log.Close();
                return;
            }
            else
            {
                GlobalLog.LogDebug("BinPath does not end with .exe.. BinPath=" + binPath);
            }

            log.LogResult(Result.Pass);
            log.Close();
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
