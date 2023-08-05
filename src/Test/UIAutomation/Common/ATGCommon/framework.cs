// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Base class that drives the actual tests cases
//
//
using System;
using System.Collections;
using System.IO;
using Microsoft.Test.WindowsUIAutomation.Core;
using Microsoft.Test.WindowsUIAutomation.Interfaces;
using Microsoft.Test.WindowsUIAutomation;
using Microsoft.Test.WindowsUIAutomation.TestManager;
using System.Windows;
using System.Reflection;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;

// This suppresses warnings #'s not recognized by the compiler.
#pragma warning disable 1634, 1691

namespace WUITest
{
    using Microsoft.Test.WindowsUIAutomation.Tests.Scenarios;

    public class FrameworkObject : IDisposable
    {

        [DllImport("User32", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetProcessDPIAware();

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal string _assemblyName = null;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal bool _results = true;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
//todo: not used?
//        internal IApplicationCommands _commands = null;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
//todo: not used?
//        internal IWUIMenuCommands _menuInterface = null;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
//todo: not used?
//        internal TestMenu _menuBar = null;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
//todo: not used?
//        internal TestMenu _systemMenu = null;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
//todo: not used?
//        internal TestMenu _testMenu = null;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Object that represents the dynamic application such as CtrlTest, 
        /// WinformsCtrl, etc.
        /// </summary>
        /// -------------------------------------------------------------------
        internal WrapperApplication _app = null;

        #region Command line arguments
        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument of the controls to test 
        /// </summary>
        /// -------------------------------------------------------------------
        internal ArrayList _IDs = new ArrayList();

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies what type of test to run
        /// </summary>
        /// -------------------------------------------------------------------
        internal TestCaseType _testType = TestCaseType.Generic;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies whether to generate summary
        /// report or not
        /// </summary>
        /// -------------------------------------------------------------------
        internal bool _summaryReport = false;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies whether to test events or not
        /// </summary>
        /// -------------------------------------------------------------------
        internal bool _testEvents = false;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that sets TestObject property indicating if 
        /// UIVerify should preserve existing content for controls that support
        /// TextPattern/ValuePattenrn. 
        /// Set to TRUE if contents should be preserved, i.e. not clobbered
        /// </summary>
        /// -------------------------------------------------------------------
        internal bool _noClobber = false;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies whether to also test childrent 
        /// or not
        /// </summary>
        /// -------------------------------------------------------------------
        internal bool _testChildren = true;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies whether to remove some defined
        /// duplicate elements, such as ListItems, TreeViewItems, so that the 
        /// tests don't perform the same tests
        /// </summary>
        /// -------------------------------------------------------------------
        internal bool _normalizeChildren = true;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies the priority of tests to run
        /// </summary>
        /// -------------------------------------------------------------------
        internal TestPriorities _testPriority = TestPriorities.Pri0;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies the target EXE for tests
        /// </summary>
        /// -------------------------------------------------------------------
        internal string _appExeName;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies which tests to run
        /// </summary>
        /// -------------------------------------------------------------------
        internal ArrayList _tests = new ArrayList();

        /// -------------------------------------------------------------------
        /// <summary>
        /// Default /////// Logging binary for ATG
        /// </summary>
        /// -------------------------------------------------------------------
        internal string _loggerBinary = UIVerifyLogger.PiperLogger;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies any command line arguments to 
        /// pass to the target EXE
        /// </summary>
        /// -------------------------------------------------------------------
        internal string _appExeArgs = null;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies the amount of threads to use
        /// for the stress tests
        /// </summary>
        /// -------------------------------------------------------------------
        internal int _stressThreads = 1;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that specifies the lab where the stress tests
        /// will be run from.
        /// </summary>
        /// -------------------------------------------------------------------
        internal TestLab _stressType = TestLab.PullData;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Flag stating whether run is DPI aware
        /// </summary>
        /// -------------------------------------------------------------------
        internal bool _dpiAware = true;

        #endregion Command line arguments

        #region Command line argument tag specifiers

        /// <summary>
        /// Starts all tags
        /// </summary>
        const string TAG = @"/";

        /// <summary>
        /// Specifies a control
        /// </summary>
        const string TAG_ID = TAG + "ID";

        /// <summary>
        /// Placeholder to do nothing
        /// </summary>
        const string TAG_IDD = TAG + "IDD";

        /// <summary>
        /// Name of the application to start
        /// </summary>
        const string TAG_EXE = TAG + "EXE";

        /// <summary>
        /// Specified the priority
        /// </summary>
        const string TAG_PRI = TAG + "PRI";

        /// <summary>
        /// Specified the test
        /// </summary>
        const string TAG_TEST = TAG + "TEST";

        /// <summary>
        /// Specify the test type
        /// </summary>
        const string TAG_testType = TAG + "TYPE";

        /// <summary>
        /// Any arguments to the EXE
        /// </summary>
        const string TAG_EXEARGS = TAG + "EXEARGS";

        /// <summary>
        /// Flag to do summary report
        /// </summary>
        const string TAG_SUMMARYREPORT = TAG + "SR";

        /// <summary>
        /// Flag to do the event tests
        /// </summary>
        const string TAG_TESTEVENTS = TAG + "EVENTS";

        /// <summary>
        /// Set global flag so we don't clobber existing content for "editiable" controls (i.e. supports Text/ValuePattern)
        /// </summary>
        const string TAG_NOCLOBBER = TAG + "NOCLOBBER";

        /// <summary>
        /// 
        /// </summary>
        const string TAG_NO_RECURSE = TAG + "NORECURSE";

        /// <summary>
        /// Normalize the tree
        /// </summary>
        const string TAG_FULLTREE = TAG + "NORMALIZE";

        /// <summary>
        /// /////// Logging binary to use for results output
        /// </summary>
        const string TAG_LOGGER = TAG + "LOG";

        /// <summary>
        /// Specifies where the stress tests are going to be run from
        /// </summary>
        const string TAG_STRESSTYPE = TAG + "STRESSTYPE";

        /// <summary>
        /// Assert so we can attach a debugger to it.
        /// </summary>
        const string TAG_BREAK = TAG + "BREAK";

        /// <summary>
        /// Flag to control dpi awareness
        /// </summary>
        const string TAG_DPINOTAWARE = TAG + "DPINOTWARE";

        const string DELIMITER = "|";

        #endregion Command line argument tag specifiers

        /// -------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// NOTE: This will start the tests that are defined in the AssemblyName
        /// </summary>
        /// <param name="Logger">/////// Logging object to dump the results to</param>
        /// <param name="AssemblyName">Name of the assembly that has the test cases defined</param>
        /// -------------------------------------------------------------------
        public FrameworkObject(string assemblyName, string[] args)
        {

#pragma warning suppress 6504
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentException();
            else
            {
#pragma warning suppress 6504
                _assemblyName = assemblyName;
            }

            ArrayList startup = ParseCommandLine(args);

            if (_loggerBinary == "CONSOLE")
                UIVerifyLogger.SetLoggerType(UIVerifyLogger.ConsoleLogger);
            else
                UIVerifyLogger.SetLoggerType(_loggerBinary);

            UIVerifyLogger.LogComment("Logger loaded");

            UIVerifyLogger.LogComment("==============================================================");
            foreach (string buffer in startup)
            {
                UIVerifyLogger.LogComment(buffer);
            }
            UIVerifyLogger.LogComment("==============================================================");

            if (_dpiAware)
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    SetProcessDPIAware();
                }

            RunTest(_assemblyName);

        }

        #region IDispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_app != null)
                    _app.Dispose();
            }
        }

        #endregion IDispose

        /// -------------------------------------------------------------------
        /// <summary>Defined the TestSuite to run.  Override this in the actual
        /// driver code (ie. GenericDriver.cs)</summary>
        /// -------------------------------------------------------------------
        internal virtual string TestSuite { get { return string.Empty; } }

        /// -------------------------------------------------------------------------
        /// <summary>
        /// Output the final results
        /// </summary>
        /// -------------------------------------------------------------------------
        public void ReportSummary()
        {
            UIVerifyLogger.ReportResults();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("PassThrough",
            TestStatus.Blocked /* marked blocked so it doesn't run when we want all test to run */,
            "Microsoft",
            "Common passthrough of any scenario test specified on the command line")]
        public void PassThrough(TestCaseAttribute testAttribute)
        {
            // The driver can do any special setup before calling UIV, if we search through 
            // all the methodifo's and cannot find the test, then there is not special 
            // setup required so call through to the PassThrough method and let UIV handle 
            // calling the test case
            RunScenarioTest(testAttribute, TestSuite, testAttribute.TestName, null, false, false);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Run the general tests within UIVerify
        /// </summary>
        /// -------------------------------------------------------------------
        internal bool RunScenarioTest(TestCaseAttribute testAttribute, string testSuite, string testName, object[] arguments, bool startApp, bool closeApp)
        {
            Library.ValidateArgumentNonNull(testAttribute, "testAttribute");
            Library.ValidateArgumentNonNull(testSuite, "testSuite");
            Library.ValidateArgumentNonNull(testName, "testName");

            StartTest(startApp);

            if (_app == null)
                _results |= TestRuns.RunScenarioTest(testSuite, testName, arguments, _testEvents, null);
            else
                _results |= TestRuns.RunScenarioTest(testSuite, testName, arguments, _testEvents, _app.IApplicationCommands);

            EndTest(closeApp);

            return _results;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Do any upfront common code for the tests such as starting the 
        /// applicaiton, initializing any menu variables, etc.
        /// </summary>
        /// -------------------------------------------------------------------
        internal virtual void StartTest(bool startApp)
        {
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Cleanup after running the test that will be common
        /// </summary>
        /// -------------------------------------------------------------------
        internal virtual void EndTest(bool closeApp)
        {
        }

        /// -------------------------------------------------------------------------
        /// <summary>
        /// This will call all the tests that have a TestCaseAttribute custom attribute
        /// associated with it.
        /// </summary>
        /// -------------------------------------------------------------------------
        void RunTest(string TestAssemblyName)
        {
            bool bools;

            // Piper complaines if we don't log a pass or fail.  Sometimes we don't have any tests
            // to run since they may be turend off, there are no Pri2 tests, etc, etc.  Do circomvent
            // this, just log one test with a pass and the Piper gods are happy.
            Microsoft.Test.WindowsUIAutomation.TestManager.TestCaseAttribute tca = new Microsoft.Test.WindowsUIAutomation.TestManager.TestCaseAttribute("Verify UIAutomation", TestPriorities.Pri0, Microsoft.Test.WindowsUIAutomation.TestManager.TestStatus.Works, "Microsoft", new string[] { "This test verifies that we have UIAutomation" });
            UIVerifyLogger.StartTest(tca, true, string.Empty, string.Empty, string.Empty);
            UIVerifyLogger.LogPass();
            UIVerifyLogger.EndTest();

            UIVerifyLogger.LogComment("Looking for tests to run");

            Type type = Type.GetType(_assemblyName);

            if (type == null)
                throw new Exception("Type.GetType(" + _assemblyName + ") failed.");

            // The driver can do any special setup before calling UIV, if we search through 
            // all the methodifo's and cannot find the test, then there is not special 
            // setup required so call through to the PassThrough method and let UIV handle 
            // calling the test case
            bool methodFound = false;

            foreach (MethodInfo method in type.GetMethods())
            {
                foreach (Attribute attr in method.GetCustomAttributes(true))
                {
                    if (attr is TestCaseAttribute)
                    {
                        TestCaseAttribute testAttribute = (TestCaseAttribute)attr;

                        switch (_tests.Count)
                        {
                            // No tests were specified, so run all the tests in the assembly
                            case 0:
                                {
                                    if (testAttribute.TestStatus.Equals(TestStatus.Works)
                                        && testAttribute.TestType.Equals(TestType.Generic))
                                    {
                                        UIVerifyLogger.LogComment("Running test : " + testAttribute.TestName);

                                        //Run this if the priority and status are correct.
                                        try
                                        {
                                            object[] test = new object[] { testAttribute };

                                            bools = (bool)method.Invoke(this, test);
                                        }

                                        catch (Exception exception)
                                        {
                                            if (Library.IsCriticalException(exception))
                                                throw;

                                            UIVerifyLogger.LogError(exception);
                                        }
                                        finally
                                        {
                                        }
                                    }

                                    break;
                                }

                            // Tests were defined on the command line so run the specific tests
                            default:
                                {
                                    // If it's one of the tests defined, then run it
                                    if (_tests.IndexOf(testAttribute.TestName) != -1)
                                    {
                                        methodFound = true;
                                        UIVerifyLogger.LogComment("Running test : " + testAttribute.TestName);

                                        //Run this if the priority and status are correct.
                                        try
                                        {
                                            object[] test = new object[] { testAttribute };

                                            method.Invoke(this, test);
                                        }
                                        catch (Exception exception)
                                        {
                                            if (Library.IsCriticalException(exception))
                                                throw;

                                            UIVerifyLogger.LogError(exception);
                                        }
                                        finally
                                        {
                                        }
                                    }

                                    break;
                                }
                        }
                    }
                }
            }

            // Did not find method, so run the PassThrough common method which will
            // pass the test name onto UIV to be ran.
            if (false == methodFound)
            {
                //Run this if the priority and status are correct.
                try
                {
                    MethodInfo mi = type.GetMethod("PassThrough");
                    if (mi != null && _tests.Count != 0)
                    {
                        TestCaseAttribute testAttribute = new TestCaseAttribute((string)_tests[0], TestStatus.Works, "", "");
                        System.Diagnostics.Debug.Assert(null != testAttribute);
                        object[] attr = mi.GetCustomAttributes(false);
                        System.Diagnostics.Debug.Assert(attr.Length > 0);
                        TestCaseAttribute testCaseAttribute = (TestCaseAttribute)attr[0];
                        testCaseAttribute.TestName = (string)_tests[0];
                        object[] test = new object[] { testAttribute };
                        type.GetMethod("PassThrough").Invoke(this, test);
                    }
                }
                catch (Exception exception)
                {
                    if (Library.IsCriticalException(exception))
                        throw;

                    UIVerifyLogger.LogError(exception);
                }
                finally
                {
                }
            }
        }

        /// -------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// -------------------------------------------------------------------------
        ArrayList ShowUsage()
        {
            ArrayList buffer = new ArrayList();
            buffer.Add(@"Need to specify a command line argument with the following items in order");
            buffer.Add(TAG_ID + ": ID of the control to test on the application");
            buffer.Add(TAG_EXE + ": Exe of the testing application");
            buffer.Add(@"Example: TestBVT " + TAG_EXE + @" c:\CtrlTest.exe " + TAG_ID + " 14");
            buffer.Add(@"Example: TestBVT /SR /ID 1000 /ID 1002 /ID 1003 /ID 1004 /ID 1005 /ID 1006 /ID 1007 /ID 1008 /ID 1009 /ID 1011 /ID 1012 /ID 1013 /ID 1015 /ID 1016 /ID 1020 /ID 1021 /ID 1023 /ID 1025 /ID 1026 /ID 1032 /ID 1033 /ID 1034 /ID 1035 /ID 1036 /ID 1042 /ID 1049 /ID 1058 /ID 1062 /ID 1065 /ID 1090 /ID 1091 /ID 1092 /ID 9001 /ID 9002 /ID 9003 /ID 9004 /ID 9005 /ID 9006 /ID 9007  /ID 59393 /ID 90011 /ID 90012 /ID 90013 /ID 90014 /ID 90015 /ID 90016 /ID 90017 /ID 90018 /EXE CtrlTest.exe");
            return buffer;
        }

        /// -------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// -------------------------------------------------------------------------
        ArrayList ParseCommandLine(string[] args)
        {
            ArrayList buffer = new ArrayList();

            int i = 1;
            StringBuilder line = new StringBuilder();


            foreach (string s in args)
            {
                buffer.Add("Argument[" + i++ + "]=" + s);
                line.Append(new StringBuilder(" " + s));
            }

            buffer.Add("Command line: " + line);
            buffer.Add("Number of argument: " + args.Length);

            for (int index = 0; index < args.Length; index++)
            {
                switch (args[index].ToUpper(CultureInfo.CurrentCulture))
                {

                    case TAG_DPINOTAWARE:
                        {
                            _dpiAware = false;
                            break;
                        }

                    case TAG_BREAK:
                        {
                            System.Diagnostics.Debug.Assert(false);
                            break;
                        }
                    case TAG_STRESSTYPE:
                        {
                            index++;
                            _stressType = (TestLab)Enum.Parse(typeof(TestLab), args[index++]);
                            _stressThreads = Convert.ToInt32(args[index]);
                            buffer.Add("Found STRESS: " + _stressType.ToString() + "...THREADS( " + _stressThreads + ")");
                            break;
                        }

                    case TAG_LOGGER:
                        {
                            index++;
                            buffer.Add("Found LOGGER: " + args[index]);
                            _loggerBinary = args[index];
                            break;
                        }

                    case TAG_testType:
                        {
                            index++;
                            buffer.Add("Found Priority:" + args[index]);
                            _testType = (TestCaseType)Enum.Parse(typeof(TestCaseType), args[index]);
                            break;
                        }

                    case TAG_FULLTREE:
                        {
                            _normalizeChildren = false;
                            break;
                        }

                    case TAG_SUMMARYREPORT:
                        {
                            _summaryReport = true;
                            break;
                        }

                    case TAG_TESTEVENTS:
                        {
                            _testEvents = true;
                            break;
                        }

                    case TAG_NOCLOBBER:
                        {
                            _noClobber = true;
                            TestRuns.NoClobber = true;
                            break;
                        }

                    case TAG_NO_RECURSE:
                        {
                            _testChildren = false;
                            break;
                        }

                    // Place holder, just mark the ID as IDD if you don't want to do anything with it
                    case TAG_IDD:
                        {
                            index++;
                            break;
                        }

                    case TAG_ID:
                        {
                            index++;
                            buffer.Add("Found ID:" + args[index]);
                            _IDs.Add(args[index]);
                            break;
                        }

                    case TAG_TEST:
                        {
                            index++;
                            buffer.Add("Found TEST: " + args[index]);
                            _tests.Add(args[index]);
                            break;
                        }

                    case TAG_EXE:
                        {
                            index++;
                            buffer.Add("Found EXE: " + args[index]);
                            _appExeName = args[index];
                            break;
                        }

                    case TAG_PRI:
                        {
                            index++;
                            buffer.Add("Found Priority: " + args[index]);
                            _testPriority = (TestPriorities)Enum.Parse(typeof(TestPriorities), args[index]);
                            break;
                        }

                    case TAG_EXEARGS:
                        {
                            index++;
                            buffer.Add("Found EXEARGS:" + args[index]);
                            _appExeArgs = args[index];
                            break;
                        }

                    default:
                        {
                            return ShowUsage();
                        }
                }
            }

            buffer.Add("".PadLeft(80, '-'));
            buffer.Add("Complete test arguments");
            buffer.Add("  Logger            : " + _loggerBinary);
            buffer.Add("  Priority          : " + _testPriority);
            buffer.Add("  Test Type         : " + _testType.ToString());
            buffer.Add("  Normalize         : " + _normalizeChildren);
            buffer.Add("  Summary Report    : " + _summaryReport);
            buffer.Add("  Test Events       : " + _testEvents);
            buffer.Add("  No Clobber Content: " + _noClobber);
            buffer.Add("  Recurse Children  : " + _testChildren);
            buffer.Add("  Executable Name   : " + _appExeName);
            buffer.Add("  Executable Args   : " + _appExeArgs);
            buffer.Add("  Stress Lab        : " + _stressType);
            buffer.Add("  Stress Threads    : " + _stressThreads);
            buffer.Add("".PadLeft(80, '-'));

            return buffer;
        }
    }
}
