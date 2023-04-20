// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Test.CompilerServices;
using Microsoft.Test.CompilerServices.Logging;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// XamlTestType for MarkupCompilerErrorTest
    /// </summary>
    public class MarkupCompilerErrorTest : XamlTestType
    {
        /// <summary>
        /// Runs a Markup compiler error test which consists of:
        ///     Loading a project file specifed in DriverState.DriverParameters["Project"]
        ///     Compiling the project and evaluating the result
        ///     Expecting errors specified in XML file DriverState.DriverParameters["ErrorsFile"]
        ///     Return Fail result if expected error doesn't occur or unexpected error occurs
        /// </summary>
        public override void Run()
        {
            Variation log = Log.Current.CreateVariation(DriverState.TestName);
            string projectFile = DriverState.DriverParameters["Project"];
            string projectDir = String.Empty;
            if (projectFile.Contains("\\"))
            {
                projectDir = projectFile.Substring(0, projectFile.LastIndexOf('\\'));
                string dstPath = Path.Combine(Environment.CurrentDirectory, projectDir);

                List<FileInfo> files = new List<FileInfo>();
                files.Add(new FileInfo(Path.Combine(Environment.CurrentDirectory, "TestCommon.Target")));

                FileUtils.CopyFilesToDirectory(files, dstPath);
            }

            if (string.IsNullOrEmpty(projectFile))
            {
                GlobalLog.LogEvidence("Project cannot be null in XTC file");
                log.LogResult(Result.Fail);
                log.Close();
                return;
            }

            bool checkWarnings = false;
            string checkWarningsString = DriverState.DriverParameters["CheckWarnings"];
            if (!string.IsNullOrEmpty(checkWarningsString))
            {
                checkWarnings = bool.Parse(checkWarningsString);
            }

            string errorsFile = DriverState.DriverParameters["ErrorsFile"];
            if (string.IsNullOrEmpty(errorsFile))
            {
                GlobalLog.LogEvidence("ErrorsFile cannot be null in XTC file");
                log.LogResult(Result.Fail);
                log.Close();
                return; 
            }

            errorsFile = Path.Combine(projectDir, errorsFile);

            List<BuildError> errorCodes = new List<BuildError>();
            XmlDocument errorsXmlDocument = new XmlDocument();
            errorsXmlDocument.Load(errorsFile);
            foreach (XmlNode node in errorsXmlDocument["Errors"].ChildNodes)
            {
                if (node.Name.Equals("Error"))
                {
                    if (node.Attributes["Code"] != null && !string.IsNullOrEmpty(node.Attributes["Code"].InnerText))
                    {
                        BuildError buildError = new BuildError(node.Attributes["Code"].InnerText);
                        errorCodes.Add(buildError);
                    }
                }
            }

            if (errorCodes.Count == 0)
            {
                GlobalLog.LogEvidence("Cannot find a valid Error element in XML file");
                log.LogResult(Result.Fail);
                log.Close();
                return;
            }

            GlobalLog.LogStatus("Compiling project file: " + projectFile);
            CompilationHelper helper = new CompilationHelper();
            CompilationHelper.AddSystemXamlReference(projectFile);
            if (!helper.CompileAndEvaluateResult(projectFile, errorCodes, null, checkWarnings))
            {
                GlobalLog.LogEvidence("Unexpected compilation result");
                LogErrorsAndWarnings(helper);
                log.LogResult(Result.Fail);
                log.Close();
                return;
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
