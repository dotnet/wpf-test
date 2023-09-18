// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides types for cross-test case coordination.

namespace Test.Uis.Management
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Xml;

    using Threading = System.Threading;

    using Test.Uis.IO;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.TestTypes;
    using Win32 = Test.Uis.Wrappers.Win32;

    #endregion Namespaces.

    /// <summary>
    /// Provides cross-test case coordinator.
    /// </summary>
    public class Coordinator
    {

        #region Public methods.

        /// <summary>
        /// Runs the test case specified through the configuration
        /// settings with all the combinations specified in
        /// an external file.
        /// </summary>
        public void RunTestCombinations(string combinatorialFile)
        {
            if (combinatorialFile == null)
            {
                throw new ArgumentNullException("combinatorialFile");
            }
            if (!TextFileUtils.Exists(combinatorialFile))
            {
                throw new System.IO.FileNotFoundException(
                    "Combinatorial file " + combinatorialFile + " not found.",
                    combinatorialFile);
            }
            if (_settings == null)
            {
                throw new InvalidOperationException(
                    "Test.Uis.TestTypes.Coordinator requires a valid " +
                    "ConfigurationSettings to run combinations");
            }

            string testName = _settings.GetArgument("TestName");

            CombinatorialEngine engine =
                CombinatorialEngine.FromFile(combinatorialFile, testName);

            // Set up the values common to all combinations.
            Hashtable values = _settings.CloneValues();
            RemoveCombinatorialXmlBlock(values);
            values[CombFileArgument] = "";
            values["xml"] = CombinationXmlFileName;
            values["IsStandaloneMode"] = IsStandaloneMode.ToString();

            // Run test cases with different settings.
            _results = new ArrayList();
            _combinationIndex = 1;
            while (engine.Next(values))
            {
                Logger.Current.Log(
                    "Combination {0}. Description:\r\n{1}",
                    _combinationIndex, engine.DescribeState());

                TestRunResult result = CreateResult(values);
                RunTestCase(values, result);
                _results.Add(result);

                _combinationIndex++;
            }
            ReportResults();
            //CleanupFiles();
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>Settings for test cases.</summary>
        public ConfigurationSettings Settings
        {
            get { return this._settings; }
            set { this._settings = value; }
        }

        /// <summary>
        /// Gets a property that determines whether the test case is running
        /// standalone as opposed to under a harness such as Piper.
        /// </summary>
        /// <remarks>
        /// Because AutomationFramework is not available in combinatorial
        /// instances, this property hides away
        /// all details and provides the appropriate behavior.
        /// </remarks>
        public static bool IsStandaloneMode
        {
            get
            {
                throw new NotImplementedException("TestLog.Harness no longer exists. Figure out right thing to do.");
                //return
                    //(Logger.Current.TestLog != null) &&
                    //(Logger.Current.TestLog.Harness!=null);
            }
        }

        /// <summary>Argument name for combinatorial specificationf file.</summary>
        public const string CombFileArgument = "CombFile";

        #endregion Public properties.


        #region Private methods.

        #region Result management.

        /// <summary>Deletes temporary files.</summary>
        private void CleanupFiles()
        {
            if (TextFileUtils.Exists(CombinationOutputFileName))
            {
                TextFileUtils.Delete(CombinationOutputFileName);
            }
            if (TextFileUtils.Exists(CombinationXmlFileName))
            {
                TextFileUtils.Delete(CombinationXmlFileName);
            }
        }

        /// <summary>
        /// Creates and initializes a new result in the currentResult
        /// member field.
        /// </summary>
        private TestRunResult CreateResult(Hashtable values)
        {
            // Set up the test case to run.
            TestRunResult result = new TestRunResult();
            result.CombinationIndex = _combinationIndex;
            values["TestName"] = CombinationTestName + "_" + _combinationIndex;

            // Set up the command line to run this.
            bool firstValue = true;
            StringBuilder sb = new StringBuilder();
            foreach(DictionaryEntry de in values)
            {
                // Skip cleared out values and coordination flags.
                string s = de.Value as string;
                if (s == null || s.Length == 0) continue;
                if (!firstValue)
                    sb.Append(" ");
                sb.Append("/");
                sb.Append(de.Key);
                sb.Append("=");
                sb.Append(de.Value);
                firstValue = false;
            }

            new System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            result.CommandLine = Assembly.GetEntryAssembly().Location +
                " " + sb.ToString();
            CodeAccessPermission.RevertAssert();

            // Set up the XML contents to run the test case.
            sb = new StringBuilder();
            sb.Append("<?xml version='1.0'?>\r\n");
            sb.Append("<TestData>\r\n");
            sb.Append("<" + values["TestName"] + ">\r\n");
            foreach(DictionaryEntry de in values)
            {
                string s = de.Value as string;
                if (s == null || s.Length == 0) continue;
                sb.Append("  <");
                sb.Append(de.Key);
                sb.Append(">\r\n");
                sb.Append("  <![CDATA[");
                sb.Append(de.Value);
                sb.Append("]]>\r\n");
                sb.Append("  </");
                sb.Append(de.Key);
                sb.Append(">\r\n");
            }
            sb.Append("</" + values["TestName"] + ">\r\n");
            sb.Append("</TestData>\r\n");
            result.DataFileContents = sb.ToString();

            return result;
        }

        /// <summary>
        /// Removes the Combinatorial XML block from the values.
        /// </summary>
        private void RemoveCombinatorialXmlBlock(Hashtable values)
        {
            System.Diagnostics.Debug.Assert(values != null);

            //
            // Load the XML block into a document, and remove any
            // Combinations tag in it, then write it back to the hash table.
            //
            string block = values[XmlTestConfiguration.XmlBlockArgumentName] as string;
            if (block == null || block.Length == 0)
            {
                throw new InvalidOperationException(
                    "XML block missing from configuration settings.");
            }

            XmlDocument document = new XmlDocument();
            document.PreserveWhitespace = true;
            document.LoadXml(block);

            XmlNodeList list = document.GetElementsByTagName("Combinations");
            if (list.Count == 0)
            {
                throw new InvalidOperationException(
                    "Combinations tag missing from the XML block: " + block);
            }
            if (list.Count > 1)
            {
                throw new InvalidOperationException(
                    "Multiple Combinatiosn tags in XML block: " + block);
            }
            list[0].ParentNode.RemoveChild(list[0]);

            values[XmlTestConfiguration.XmlBlockArgumentName] = document.OuterXml;
        }

        /// <summary>
        /// Reports results and flags the test case as a failure if any
        /// test case failed.
        /// </summary>
        private void ReportResults()
        {
            int passCount = 0;
            int failCount = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("Test case failures:");
            foreach(TestRunResult result in _results)
            {
                if (result.Passed)
                {
                    passCount++;
                }
                else
                {
                    failCount++;
                    if (failCount != 1) sb.Append("\n");
                    sb.Append(result.ToString());
                    sb.Append("\n\n");
                }
            }
            string finalMessage;
            if (failCount > 0)
            {
                finalMessage = "One or more combinations for test runs failed.";
            }
            else
            {
                sb.Append(" NONE");
                finalMessage = "All combinations succeeded.";
            }
            Logger.Current.Log(sb.ToString());
            _settings.TestLog.LogStatus("PassCount: " + passCount.ToString());
            _settings.TestLog.LogStatus("FailCount: " + failCount.ToString());
            _settings.TestLog.LogStatus("FinalMessage: " + finalMessage);
        }

        #endregion Result management.

        #region Test case communication.

        /// <summary>
        /// Creates a ProcessStartInfo instance initialized for the test case
        /// with the specified values.
        /// </summary>
        /// <param name="values">Values for the test case.</param>
        /// <returns>Initialized ProcessStartInfo instance.</returns>
        private ProcessStartInfo CreateProcessStartInfo(Hashtable values)
        {
            //
            // The command line is formed according to the following rules:
            //
            // [entry-point-assembly] [naked-arguments] [coordination-arguments]
            //
            // entry-point-assembly: this is the entry assembly currently running
            //
            // naked-argument: these are argument that are not in the form
            //   of /argument , and are presumed to be arguments for the 
            //   entry-point-assembly
            //
            // coordination-arguments: these are arguments used by the 
            //   coordination manager; test case arguments are passed in the 
            //   combination xml file.
            //                  
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.FileName = Assembly.GetEntryAssembly().Location;
            startInfo.FileName = "EditingTest.exe";            
            string nakedArguments = "";
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];                
                if (i == 0 && arg.Contains(startInfo.FileName)) continue;
                if (i == 0 && arg.Contains("sti.exe")) continue;
                if (arg == null || arg.Length == 0) continue;
                if (arg[0] == '/') continue;
                nakedArguments += arg + " ";
            }
            startInfo.Arguments =
                nakedArguments +
                "/xml=" + CombinationXmlFileName + " " +
                "/OutputFile=" + CombinationOutputFileName + " " +
                "/TestCaseType=" + values["TestCaseType"] + " " +
                "/TestName=" + CombinationTestName + "_" + _combinationIndex;            
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            return startInfo;
        }

        /// <summary>Runs a single test case.</summary>
        private void RunTestCase(Hashtable values, TestRunResult result)
        {
            // Request permissions to access the filesystem, reflect,
            // and execute processes.
            System.Security.Permissions.PermissionState state =
                System.Security.Permissions.PermissionState.Unrestricted;
            System.Security.PermissionSet perms =
                new System.Security.PermissionSet(state);
            perms.AddPermission(
                new System.Security.Permissions.ReflectionPermission(state));
            perms.AddPermission(
                new System.Security.Permissions.FileIOPermission(state));
            perms.AddPermission(
                new System.Security.Permissions.SecurityPermission(state));
            perms.Assert();

            int timeoutMs = 1000 * 60;
            string timoutMsString = values["Timeout"] as string;
            if (timoutMsString != null && timoutMsString.Length > 0)
            {
                timeoutMs = Int32.Parse(timoutMsString);
            }
            if (System.Diagnostics.Debugger.IsAttached)
            {
                timeoutMs = Threading.Timeout.Infinite;
            }

            //
            // Launch process with modified arguments to run as a single
            // combination instance.
            //
            ProcessStartInfo startInfo = CreateProcessStartInfo(values);
            System.Diagnostics.Debug.Assert(startInfo != null);
            Logger.Current.Log(
                "Running: " + startInfo.FileName + " " + startInfo.Arguments);            
            WriteSettingsToFile(result, CombinationXmlFileName);

            Process process;
            process = Process.Start(startInfo);            
            try
            {
                LogStreamWorkItem item = new LogStreamWorkItem(
                    process.StandardOutput, "Case StdOut Log: ", true);
                process.WaitForExit(timeoutMs);
            }
            finally
            {
                if (!process.HasExited)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch(Exception e)
                    {
                        Logger.Current.Log("Unable to kill test case process.");
                        Logger.Current.Log(e);
                    }
                }
                process.Close();
            }
            
            ReadResultFromFile(result, CombinationOutputFileName);
        }

        /// <summary>
        /// Logs the results from the given file name and sets
        /// the specified TestRunResult instance to the parsed results.
        /// </summary>
        private void ReadResultFromFile(TestRunResult result, string fileName)
        {
            System.Diagnostics.Debug.Assert(result != null);
            System.Diagnostics.Debug.Assert(fileName != null);
            System.Diagnostics.Debug.Assert(fileName.Length > 0);

            Logger.Current.Log("Processing output file " + fileName);
            result.Passed = Logger.Current.ProcessLog(fileName);
        }

        /// <summary>
        /// Writes the settings for the test run object to an XML
        /// file that can be read back by a specific combination instance.
        /// </summary>
        private void WriteSettingsToFile(TestRunResult result,
            string fileName)
        {
            System.Diagnostics.Debug.Assert(_settings != null);
            System.Diagnostics.Debug.Assert(fileName != null);
            System.Diagnostics.Debug.Assert(fileName.Length > 0);

            TextFileUtils.SaveToFile(result.DataFileContents, fileName);
        }

        #endregion Test case communication.

        #endregion Private methods.


        #region Private constants.

        /// <summary>
        /// File name used by test instances to write out log to.
        /// </summary>
        private const string CombinationOutputFileName = "combination_output.txt";

        /// <summary>
        /// Test name used for a specific instance.
        /// </summary>
        /// <remarks>
        /// This is used to name the data in the XML file with
        /// settings, and to direct a combination instance to use
        /// it with the /TestName= switch.
        /// </remarks>
        private const string CombinationTestName = "CombinationInstace";

        /// <summary>
        /// File name used to pass information into a specific
        /// test combination.
        /// </summary>
        private const string CombinationXmlFileName = "combination_instance.xml";

        #endregion Private constants.


        #region Private fields.

        /// <summary>Index of combination being run.</summary>
        private int _combinationIndex;

        /// <summary>Settings for test cases.</summary>
        private ConfigurationSettings _settings;

        /// <summary>Array of test case results.</summary>
        private ArrayList _results;

        #endregion Private fields.
    }

    /// <summary>Abstraction for test case run results.</summary>
    class TestRunResult
    {

        #region Public methods.

        /// <summary>Provides a string representation of the result.</summary>
        /// <returns>A string with the result information.</returns>
        public override string ToString()
        {
            string result = (Passed? "PASS" : "FAIL");
            result += " (index:" + CombinationIndex + ")";
            result += " - " + Message;
            result += "\nSuggested cmdline:\n" + CommandLine;
            result += "\n\nData file for problematic command lines:\n" + DataFileContents;
            return result;
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>Combination index.</summary>
        public int CombinationIndex
        {
            get { return this._combinationIndex; }
            set { this._combinationIndex = value; }
        }

        /// <summary>Suggested command line to repro.</summary>
        public string CommandLine
        {
            get { return this._commandLine; }
            set { this._commandLine = value; }
        }

        /// <summary>Contents for a data file specifying this combination to repro.</summary>
        public string DataFileContents
        {
            get { return this._dataFileContents; }
            set { this._dataFileContents = value; }
        }

        /// <summary>Result message.</summary>
        public string Message
        {
            get { return this._message; }
            set { this._message = value; }
        }

        /// <summary>Whether the test case passed.</summary>
        public bool Passed
        {
            get { return this._passed; }
            set { this._passed = value; }
        }

        #endregion Public properties.


        #region Private fields.

        /// <summary>Combination index.</summary>
        private int _combinationIndex;

        /// <summary>Suggested command line to repro.</summary>
        private string _commandLine;

        /// <summary>Data file contents for repro.</summary>
        private string _dataFileContents;

        /// <summary>Result message.</summary>
        private string _message;

        /// <summary>Whether the test case passed.</summary>
        private bool _passed;

        #endregion Private fields.
    }
}