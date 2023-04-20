// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /******************************************************************************
    * CLASS:          BamlReferenceTest
    ******************************************************************************/

    /// <summary>
    /// Class for comparing Xaml and Baml Infosets.
    /// </summary>
    public class BamlReferenceTest : BamlTestType
    {
        #region Private Data

        /// <summary>Sets the location of the .exe</summary>
        private string _exeFilePath = Environment.CurrentDirectory + "\\bin\\Release\\XamlToBaml.exe";

        #endregion

        #region Public and Protected Members

        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Runs a BamlReferenceTest.
        /// Verifies compilation with the specified target and checks that the assemblies loaded do not contain
        /// references to the "UnexpectedVersion".
        /// </summary>
        public override void Run()
        {
            try
            {
                // The base class will take care of compiling the Xaml file.
                base.Run();

                GlobalLog.LogStatus("----------- Starting BamlReferenceTest -----------");

                // Ensures that the test starts on a clean slate
                LocalTestResult = TestResult.Unknown;

                // This causes the WPF assemblies to load, this is a hack that will be removed later
                FrameworkElement frameworkElement = new FrameworkElement();
                frameworkElement = null;

                // Retrieve test parameters from an .xtc file.
                string unexpectedVersion = DriverState.DriverParameters["UnexpectedVersion"];
                string target = DriverState.DriverParameters["Target"];

                if (string.IsNullOrEmpty(unexpectedVersion))
                {
                    throw new TestSetupException("UnexpectedVersion cannot be null");
                }

                GlobalLog.LogStatus("Checking for version: " + unexpectedVersion);

                if (string.IsNullOrEmpty(target))
                {
                    throw new TestSetupException("Target cannot be null");
                }

                GlobalLog.LogStatus("Compiling target version: " + target);

                // Compile.
                if (target == "v3.5")
                {
                    if (NetFxDetector.IsNet35Installed())
                    {
                        string projectPath = Environment.CurrentDirectory + "\\obj\\Release\\";
                        string projectFileName = "XamlToBaml.csproj";
                        FileInfo projectFileInfo = new FileInfo(projectPath + projectFileName);

                        BamlFileName = BamlFactory.GenerateBamlFromProject(target, projectFileInfo, null);

                        // Verify references in assemblies.
                        bool testPassed = BamlReferenceVerifier.CheckReferenceVersion(BamlFileName, _exeFilePath, unexpectedVersion);

                        if (testPassed)
                        {
                            GlobalLog.LogEvidence("PASS: Test sucessful");
                            LocalTestResult = TestResult.Pass;
                        }
                        else
                        {
                            // Throwing an exception so that any subclasses that carry out additional testing will not be executed.
                            throw new TestValidationException("FAIL: BamlReferenceTest failed.");
                        }
                    }
                    else
                    {
                        GlobalLog.LogEvidence("IGNORE: .Net 3.5 is not installed on this machine.");
                        LocalTestResult = TestResult.Ignore;
                    }
                }
                else
                {
                    throw new TestSetupException("ERROR: currently only 3.5 compilation is supported in this test framework.");
                }
            }
            finally
            {
                if (this.GetType().Equals(typeof(BamlReferenceTest)))
                {
                    CleanUp();
                }
            }
        }

        #endregion
    }
}
