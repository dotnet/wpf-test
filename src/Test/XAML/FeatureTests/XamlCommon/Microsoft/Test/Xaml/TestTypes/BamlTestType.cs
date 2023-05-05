// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Framework;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /******************************************************************************
    * CLASS:          BamlTestType
    ******************************************************************************/

    /// <summary>
    /// Abstract class for the test types used in Baml testing.
    /// </summary>
    public abstract class BamlTestType : XamlTestType
    {
        /// <summary> Path to which the project will be compiled. </summary>
        private readonly string _bamlDirPath = Environment.CurrentDirectory + "\\obj\\Release\\netcoreapp3.0\\";

        /// <summary> The name of the .csproj file to be compiled. </summary>
        private string _projectFileName = "XamlToBaml.csproj";

        /// <summary> Local TestResult value. </summary>
        private TestResult _localTestResult = TestResult.Unknown;

        /// <summary>
        /// Specifies the type of Baml test being called.
        /// </summary>
        public enum BamlTest
        {
            /// <summary>
            /// Tests for the Baml2006Reader.
            /// </summary>
            BAMLREADERTEST,

            /// <summary>
            /// Tests for the BamlWriter.
            /// </summary>
            BAMLWRITERTEST,

            /// <summary>
            /// Tests for the correct wpf version in Baml.
            /// </summary>
            BAMLREFERENCETEST,

            /// <summary>
            /// Tests XamlXmlWriter using a Baml2006Reader.
            /// </summary>
            BAMLTOXAMLTEST,

            /// <summary>
            /// Tests the creation of objects from Baml.
            /// </summary>
            BAMLTOOBJECTTEST
        }

        /// <summary> Gets or sets name of the .csproj file to be compiled. </summary>
        public string ProjectFileName
        {
            set
            {
                _projectFileName = value;
            }

            get
            {
                return _projectFileName;
            }
        }

        /// <summary>
        /// Gets or sets the name of the standard .xaml file that will be compiled.
        /// Every test .xaml file will be copied to the a file with this name, which is specified in the .csproj file.
        /// </summary>
        /// <value>The name of the standard xaml file.</value>
        public string ProjectXamlFileName { get; set; }

        /// <summary> Gets or sets the TestLog value. </summary>
        public TestLog LocalTestLog { get; set; }

        /// <summary>
        /// Gets or sets the local test result
        /// </summary>
        public TestResult LocalTestResult
        {
            set
            {
                _localTestResult = value;
            }

            get
            {
                return _localTestResult;
            }
        }

        /// <summary>
        /// Gets or sets the name of a .xaml file.
        /// </summary>
        /// <value>The name of a xaml file.</value>
        protected string XamlFileName { get; set; }

        /// <summary>
        /// Gets or sets the name of a .baml file.
        /// </summary>
        /// <value>The name of a baml file.</value>
        protected string BamlFileName { get; set; }

        /// <summary> Gets or sets properties to be passed on to the build engine. </summary>
        protected Dictionary<string, string> GlobalProperties { get; set; }

        /// <summary>
        /// Gets the path where the baml file is located.
        /// </summary>
        protected string BamlDirPath
        {
            get
            {
                return _bamlDirPath;
            }
        }

        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Main method of execution for each test type.
        /// </summary>
        public override void Run()
        {
            GlobalLog.LogStatus("Test Name: " + DriverState.TestName);
            LocalTestLog = new TestLog(DriverState.TestName);

            XamlFileName = DriverState.DriverParameters["XamlFile"];
            if (String.IsNullOrEmpty(XamlFileName))
            {
                throw new TestSetupException("XamlFileName cannot be null.");
            }

            if (!File.Exists(XamlFileName))
            {
                throw new TestSetupException("ERROR: the Xaml file specified does not exist.");
            }

            // Load any supporting assemblies
            if (!String.IsNullOrEmpty(DriverState.DriverParameters["SupportingAssemblies"]))
            {
                string assemblies = DriverState.DriverParameters["SupportingAssemblies"];
                GlobalLog.LogStatus("Loading Assemblies: " + assemblies);
                FrameworkHelper.LoadSupportingAssemblies(assemblies);
            }

            ProjectXamlFileName = Path.GetFileNameWithoutExtension(_projectFileName) + ".xaml";

            // Each incoming .xaml file is copied to a standard tempXamlFileName that is included in the project file
            // that will be dynamically compiled.
            File.Copy(XamlFileName, ProjectXamlFileName, true);

            int delayCount = 3; // delay being introduced since IO operation from the previous test may be holding onto the binaries
            if (Directory.Exists(_bamlDirPath) && (delayCount-- > 0))
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, 1));
            }

            FileInfo projectFileInfo = new FileInfo(Environment.CurrentDirectory + "\\" + _projectFileName);

            // GlobalProperties is set by the test case, if needed.
            BamlFileName = BamlFactory.GenerateBamlFromProject(string.Empty, projectFileInfo, GlobalProperties);

            // Copy the .baml file from the project directory to the current directory where it is later referenced.
            File.Copy(_bamlDirPath + "\\" + BamlFileName, BamlFileName, true);
        }

        /******************************************************************************
        * Function:          CleanUp
        ******************************************************************************/

        /// <summary>
        /// Removes the .xaml file and the directory where it was compiled.
        /// </summary>
        protected void CleanUp()
        {
            if (!string.IsNullOrEmpty(this.ProjectXamlFileName) && File.Exists(this.ProjectXamlFileName))
            {
                File.Delete(ProjectXamlFileName);
            }

            if (!string.IsNullOrEmpty(this.BamlFileName) && File.Exists(this.BamlFileName))
            {
                File.Delete(BamlFileName);
            }

            if (Directory.Exists(BamlDirPath))
            {
                try
                {
                    Directory.Delete(BamlDirPath, true);
                }
                catch (IOException)
                {
                    // Ignore IOException that is sometimes thrown even though recursive argument is true.
                    // Happens only when running RunXamlTests.cmd
                }
            }

            //// If the TestResult == Unknown it means an exception was thrown
            //// either because the test failed or the Product threw an exception
            if (LocalTestResult != TestResult.Unknown)
            {
                TestLog.Current.Result = LocalTestResult;
                LocalTestLog.Close();
            }
        }
    }
}
