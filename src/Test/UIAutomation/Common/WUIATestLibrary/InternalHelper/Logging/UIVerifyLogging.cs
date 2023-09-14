// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//*******************************************************************/
using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Text;

namespace Microsoft.Test.WindowsUIAutomation
{
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using Microsoft.Test.WindowsUIAutomation.Logging;
    using Microsoft.Test.WindowsUIAutomation.Core;

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Loggin object for InternalHelper
    /// </summary>
    /// -----------------------------------------------------------------------
    sealed public class UIVerifyLogger
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const int _headerWidth = 90;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static int s_incorrectConfigurations = 0;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static int s_knownIssues = 0;

        ///// -------------------------------------------------------------------
        ///// <summary>Stores the path to the element for later use</summary>
        ///// -------------------------------------------------------------------
        //static string _elementPath;

        ///// -------------------------------------------------------------------
        ///// <summary>Stores the example call to the test for later use</summary>
        ///// -------------------------------------------------------------------
        //static string _exampleCall;

        /// -------------------------------------------------------------------
        /// <summary>Console logger</summary>
        /// -------------------------------------------------------------------
        public static string ConsoleLogger = Microsoft.Test.WindowsUIAutomation.Logging.LogTypes.ConsoleLogger;

        /// -------------------------------------------------------------------
        /// <summary>Piper logger</summary>
        /// -------------------------------------------------------------------
        public static string PiperLogger = Microsoft.Test.WindowsUIAutomation.Logging.LogTypes.PiperLogger;

        /// -------------------------------------------------------------------
        /// <summary>Default logger defined by WUILogger</summary>
        /// -------------------------------------------------------------------
        public static string DefaultLogger = Microsoft.Test.WindowsUIAutomation.Logging.LogTypes.DefaultLogger;

        /// -------------------------------------------------------------------
        /// <summary>Flag to determine if we are running within the test manager</summary>
        /// -------------------------------------------------------------------
        public static bool IsRunningFromWithinUIATestManager
        {
            get
            {
                //This used to be based on if the code was running in these exes: VUIVERIFY.EXE, VUIVERIFYINTERNAL.EXE
                //since this is no longer true we always return false
                return false;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Report that the test had an error
        /// </summary>
        /// -------------------------------------------------------------------
        public static void LogError(Exception exception)
        {

            // Our test framework calls using Invoke, and throw is returned 
            if (exception is TargetInvocationException)
                exception = exception.InnerException;

            // If a test catches the exception, and then rethrows the excpeption later, it uses "RETHROW" 
            // to allow this global exception handler to peel this rethrow and analyze the actual exception.
            if (exception.Message == "RETHROW")
                exception = exception.InnerException;

            if( exception.GetType() == typeof(InternalHelper.Tests.KnownProductIssueException))
            {
                s_knownIssues++;
                LogComment("Known Product Failure. Bug: " + exception.Message);
                Logger.LogPass();
            }
            else if (exception.GetType() == typeof(InternalHelper.Tests.IncorrectElementConfigurationForTestException))
            {
                s_incorrectConfigurations++;
                LogComment("Incorrect configurations. Reason: " + exception.Message);
                Logger.LogPass();
            }
            else if (exception.GetType() == typeof(InternalHelper.Tests.TestErrorException))
            {
                Logger.LogError(exception, true);
            }
            else
            {
                Logger.LogUnexpectedError(exception, true);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Report an excepetion that was not expected
        /// </summary>
        /// -------------------------------------------------------------------
        public static void LogUnexpectedError(Exception exception)
        {
            Logger.LogUnexpectedError(exception, true);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Loads a logger DLL binary that impements WUIALogging interface
        /// </summary>
        /// <param name="filename">Filename with path</param>
        /// -------------------------------------------------------------------
        static public void SetLoggerType(string filename)
        {
            Logger.SetLogger(filename);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static IWUIALogger BackEndLogger
        {
            set
            {
                Logger.BackEndLogger = value;
            }
            get
            {
                return Logger.BackEndLogger;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static void LogComment(object comment)
        {
            Logger.LogComment(comment);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Log comment to the output
        /// </summary>
        /// <param name="format">The format string</param>
        /// <param name="args">An array of objects to write using format</param>
        /// -------------------------------------------------------------------
        public static void LogComment(string format, params object[] args)
        {
            // format may have formating characters '{' & '}', only call
            // String.Format if there is a valid args arrray. Calling it
            // without and arg will throw an exception if you have formating 
            // chars and no args array.
            if (args.Length == 0)
            {
                Logger.LogComment(format);
            }
            else
            {
                Logger.LogComment(String.Format(format, args));
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static void StartTest(TestCaseAttribute testAttribute, bool tbd1, string elementXmlPath, object exampleCodeCall, object uiSpyLook)
        {
            Logger.StartTest(testAttribute.TestName);

            if (IsRunningFromWithinUIATestManager)
            {
                // The Xml logger for UIATestManager knows how to output 
                // TestCaseAttribute in a more robust manner.
                Logger.LogComment(testAttribute);
            }
            else
            {
                int step = 0;

                Logger.LogComment("".PadRight(_headerWidth, '='));
                Logger.LogComment("Test          : " + testAttribute.TestName);
                Logger.LogComment("Summary       : " + testAttribute.TestSummary);
                Logger.LogComment("Test Pri      : " + testAttribute.Priority.ToString());
                Logger.LogComment("TestCaseType  : " + testAttribute.TestCaseType);
                Logger.LogComment("Element       : " + testAttribute.UISpyLookName);
                if (testAttribute.BugNumbers != null)
                {
                    foreach (BugIssues bugIssue in testAttribute.BugNumbers)
                    {
                        Logger.LogComment("Possible issue: {0}", InternalHelper.Helpers.BugIssueToDescription(bugIssue));
                    }
                }

                if (!string.IsNullOrEmpty(testAttribute.ProblemDescription))
                    Logger.LogComment("Problem Desc : " + testAttribute.ProblemDescription);
                Logger.LogComment("Planned steps to be executed       :");
                foreach (string str in testAttribute.Description)
                {
                    Logger.LogComment("             " + step++ + ") " + str);
                }
                Logger.LogComment("".PadRight(_headerWidth, '='));
            }
            Logger.SetCodeExample(elementXmlPath, exampleCodeCall.ToString());
            //Logger.LogComment(new ProgramExampleCall(elementXmlPath, exampleCodeCall.ToString()));

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static void LogPass()
        {
            Logger.LogPass();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static void EndTest()
        {
            Logger.EndTest();
        }

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        public static void MonitorProcess(Process process)
        {
            Logger.MonitorProcess(process);
        }

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        public static void CloseLog()
        {
            Logger.CloseLog();
        }
        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        public static void ReportResults()
        {
            bool runningFromVisualUIVerify = IsRunningFromWithinUIATestManager;

            if (!IsRunningFromWithinUIATestManager)
            {
                for (int i = 0; i < 3; i++)
                {
                    Logger.LogComment("".PadRight(_headerWidth, '='));
                }                
            }

            Logger.ReportResults();

            if (!IsRunningFromWithinUIATestManager)
            {
                Logger.LogComment("".PadRight(_headerWidth, '-'));
            }
            Logger.LogComment("TOTAL INCORRECT CONFIGURATIONS(PASS) : " + s_incorrectConfigurations);
            Logger.LogComment("TOTAL KNOWN ISSUES/PUNTED(PASS)      : " + s_knownIssues);
            

            if (!IsRunningFromWithinUIATestManager)
            {
                Logger.LogComment("".PadRight(_headerWidth, '='));
            }
        }
    }
}
