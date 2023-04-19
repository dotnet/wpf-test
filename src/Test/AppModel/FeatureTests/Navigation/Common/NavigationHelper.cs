// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test.Loaders;           // ApplicationMonitor
using Microsoft.Test.Logging;
using System.Windows.Automation;
using Microsoft.Test;                 // Result (for tests)
using Microsoft.Test.Input;           // TestLog, TestStage

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class NavigationHelper
    {
        private static Log s_frmwk = Log.Current;
        private static Result s_currentResult = Result.Fail;

        #region globals: file names
        public static string ExpectedFileName = String.Empty;
        public static string ActualFileName = String.Empty;
        public static int ActualTestCount = 0;
        public static int ExpectedTestCount = 0;

        #endregion

        #region globals: event hit counters
        public static int NumExpectedNavigatingEvents = 0;
        public static int NumExpectedNavigatedEvents = 0;
        public static int NumExpectedNavigationStoppedEvents = 0;
        public static int NumExpectedFragmentNavigationEvents = 0;
        public static int NumExpectedLoadCompletedEvents = 0;
        public static int NumExpectedNavigationFailedEvents = 0;
        
        /// <summary>
        /// Ref: http://msdn.microsoft.com/en-us/library/system.windows.navigation.navigationservice.navigationprogress.aspx
        /// NavigationProgress Event is raised once for every 1024 bytes of content downloaded from the page/frame we 
        /// are navigating to. Therefore, 
        /// NumExpectedNavigationProgressEvents = NavigationProgressEventArgs.MaxBytes/1024  + 1(if the page size is not a multiple of 1024)
        /// </summary>
        public static int NumExpectedNavigationProgressEvents = 0;
        /// <summary>
        /// contains the value of NavigationProgressEventArgs.MaxBytes that we capture in the handler to NavigationProgressed Event handler
        /// </summary>
        public static int NumMaxBytesExpectedNavigationProgressEvent = 0;

        public static int NumActualNavigatingEvents = 0;
        public static int NumActualNavigatedEvents = 0;
        public static int NumActualNavigationStoppedEvents = 0;
        public static int NumActualFragmentNavigationEvents = 0;
        public static int NumActualLoadCompletedEvents = 0;
        public static int NumActualNavigationFailedEvents = 0;
        public static int NumActualNavigationProgressEvents = 0;

        public static int ExpectedEventTotal = 0;
        public static int ActualEventTotal = 0;
        #endregion

        #region TestLog methods
        public static void CreateLog(string testName)
        {
            if (s_frmwk == null)
                s_frmwk =  LogManager.BeginTest(testName);
            else
                Output("Test Log already exists: " + Log.Current.Name);

            Log.Current.CurrentVariation.LogMessage("Initializing Test Log for '" + testName + "' test");
        }

        public static void Output(string statusMsg)
        {
            if ((Log.Current != null)&& (Log.Current.CurrentVariation != null))
            {                
                Log.Current.CurrentVariation.LogMessage(statusMsg);
            }
            else
            {
                LogManager.LogMessageDangerously(statusMsg);
            }
        }

        public static void SetStage(TestStage currStage)
        {
            Output("*** Current Test Stage: " + currStage.ToString() + " ***");
        }

        public static String GetTestName()
        {
            return Log.Current.Name;
        }

        public static void ExitWithError(string failMsg)
        {
            Log.Current.CurrentVariation.LogMessage(failMsg);
            NavigationHelper.CacheTestResult(Result.Fail);
            Shutdown();
        }

        // Ignore the test case and exit
        public static void ExitWithIgnore(string ignoreMsg)
        {
            Log.Current.CurrentVariation.LogMessage(ignoreMsg);
            NavigationHelper.CacheTestResult(Result.Ignore);
            Shutdown();
        }

        public static void PassTest(string passMsg)
        {
            Log.Current.CurrentVariation.LogMessage("SUCCESS!!! " + passMsg);
            NavigationHelper.CacheTestResult(Result.Pass);
            Shutdown();
        }

        
        #endregion

        #region Helpers: log test results
        public static bool CompareEventCounts()
        {

            /// Calculate Expected Number of NavigationProgress Events
            int BytesTransferredPerNavigationProgressEvent = 1024;
            NumExpectedNavigationProgressEvents = NumMaxBytesExpectedNavigationProgressEvent / BytesTransferredPerNavigationProgressEvent +
                ((NumMaxBytesExpectedNavigationProgressEvent % BytesTransferredPerNavigationProgressEvent == 0) ? 0 : 1);

            // Compare the actual and expected event counts for each navigation event
            if (ExpectedTestCount != ActualTestCount)
            {
                Fail("Expected test count was " + ExpectedTestCount, "Actual test count was " + ActualTestCount);
                return false;
            }

            Output(string.Format("Event Count: Navigating       : (expected:Obtained): ({0}:{1})", NumExpectedNavigatingEvents, NumActualNavigatingEvents));
            if (NumExpectedNavigatingEvents != NumActualNavigatingEvents)
            {
                Fail("Expected Navigating event count was " + NumExpectedNavigatingEvents, "Actual Navigating event count was " + NumActualNavigatingEvents);
                return false;
            }

            Output(string.Format("Event Count: Navigated        : (expected:Obtained): ({0}:{1})", NumExpectedNavigatedEvents, NumActualNavigatedEvents));
            if (NumExpectedNavigatedEvents != NumActualNavigatedEvents)
            {
                Fail("Expected Navigated event count was " + NumExpectedNavigatedEvents, "Actual Navigated event count was " + NumActualNavigatedEvents);
                return false;
            }

            Output(string.Format("Event Count: NavigationStopped: (expected:Obtained): ({0}:{1})", NumExpectedNavigationStoppedEvents, NumActualNavigationStoppedEvents));
            if (NumExpectedNavigationStoppedEvents != NumActualNavigationStoppedEvents)
            {
                Fail("Expected NavigationStopped event count was " + NumExpectedNavigationStoppedEvents, "Actual NavigationStopped event count was " + NumActualNavigationStoppedEvents);
                return false;
            }

            Output(string.Format("Event Count: FragmentNavigation:(expected:Obtained): ({0}:{1})", NumExpectedFragmentNavigationEvents, NumActualFragmentNavigationEvents));
            if (NumExpectedFragmentNavigationEvents != NumActualFragmentNavigationEvents)
            {
                Fail("Expected FragmentNavigation event count was " + NumExpectedFragmentNavigationEvents, "Actual FragmentNavigation event count was " + NumActualFragmentNavigationEvents);
                return false;
            }

            Output(string.Format("Event Count: LoadCompleted    : (expected:Obtained): ({0}:{1})", NumExpectedLoadCompletedEvents, NumActualLoadCompletedEvents));
            if (NumExpectedLoadCompletedEvents != NumActualLoadCompletedEvents)
            {
                Fail("Expected LoadCompleted event count was " + NumExpectedLoadCompletedEvents, "Actual LoadCompleted event count was " + NumActualLoadCompletedEvents);
                return false;
            }

            Output(string.Format("Event Count: NavigationFailed : (expected:Obtained): ({0}:{1})", NumExpectedNavigationFailedEvents, NumActualNavigationFailedEvents));
            if (NumExpectedNavigationFailedEvents != NumActualNavigationFailedEvents)
            {
                Fail("Expected NavigationFailed event count was " + NumExpectedNavigationFailedEvents, "Actual NavigationFailed event count was " + NumActualNavigationFailedEvents);
                return false;
            }

            Output(string.Format("Event Count: NavigationProgress:(expected:Obtained): ({0}:{1})", NumExpectedNavigationProgressEvents, NumActualNavigationProgressEvents));
            if (NumExpectedNavigationProgressEvents != NumActualNavigationProgressEvents)
            {
                Fail("Expected NavigationProgress event count was " + NumExpectedNavigationProgressEvents, "Actual NavigationProgress event count was " + NumActualNavigationProgressEvents);
                return false;
            }

            // expected and actual event totals
            ExpectedEventTotal =
                NumExpectedNavigatingEvents +
                NumExpectedNavigatedEvents +
                NumExpectedNavigationStoppedEvents +
                NumExpectedFragmentNavigationEvents +
                NumExpectedLoadCompletedEvents +
                NumExpectedNavigationFailedEvents +
                NumExpectedNavigationProgressEvents;

            ActualEventTotal =
                NumActualNavigatingEvents +
                NumActualNavigatedEvents +
                NumActualNavigationStoppedEvents +
                NumActualFragmentNavigationEvents +
                NumActualLoadCompletedEvents +
                NumActualNavigationFailedEvents +
                NumActualNavigationProgressEvents;

            Output(string.Format("Event Count: Total            : (Expected:Obtained): ({0}:{1})", ExpectedEventTotal, ActualEventTotal));
            Output(string.Format("Tests Count: Total            : (Expected:Obtained): ({0}:{1})", ExpectedTestCount, ActualTestCount));

            // Make sure nobody is calling this method if expected event totals or expected 
            // test counts are zero. 
            Output("CompareEventCounts() is called with ExpectedEventTotal = 0 and ExpectedTestCount = 0.");
            if (ExpectedEventTotal == 0 && ExpectedTestCount == 0)
            {
                Fail("CompareEventCounts() is called with ExpectedEventTotal = 0 and ExpectedTestCount = 0.");
                return false;
            }

            // Compare the total event counts
            return ExpectedEventTotal == ActualEventTotal;
        }

        public static void FinishTest(bool passed)
        {
            if (passed)
            {
                if (NavigationHelper.CompareEventCounts())
                {
                    Pass("Navigation test passes - test counts and event counts match");
                }
                else
                {
                    Fail("Navigation test fails - test counts or event counts don't match");
                }
            }
            else
            {
                Fail("Navigation test fails - a previous subtest failed");
            }
        }

        // test case blatantly failed - log failure & clean up
        public static void Fail(string errorMsg)
        {
            Output("UNEXPECTED ERROR: " + errorMsg);
            Output("- - - FAIL:  Navigation Test completed - - -");
            s_currentResult = Result.Fail;

            Shutdown();
        }

        public static void Fail(string expectedResult, string actualResult)
        {
            Output("EXPECTED: " + expectedResult);
            Output("ACTUAL: " + actualResult);
            Output("- - - FAIL: Navigation Test completed - - -");
            s_currentResult = Result.Fail;
            Shutdown();
        }

        public static void Pass(string successMsg)
        {
            Output("SUCCESS!! " + successMsg);
            Output("+ + + PASS: Navigation Test completed + + +");
            s_currentResult = Result.Pass;

            Shutdown();
        }

        public static void Shutdown()
        {            
            if (s_frmwk.CurrentVariation != null)
            {
                s_frmwk.CurrentVariation.LogResult(s_currentResult);
                Output("Shutting down application");

                s_frmwk.CurrentVariation.Close();
                s_frmwk.Close();
            }
            ApplicationMonitor.NotifyStopMonitoring();
        }

        public static void CacheTestResult(Result result)
        {
            s_currentResult = result;
        }

        public static void CacheTestResult(Result result, string comment)
        {
            Output(comment);
            s_currentResult = result;
        }

        #endregion

        #region Helpers: file name comparison
        public static bool CompareFileNames(string expectedFileName, Uri actualFile)
        {
            bool passed = false;
            if (expectedFileName == (getFileName(actualFile)))
            {
                Output("File name is correct: " + expectedFileName);
                passed = true;
            }
            else
            {
                Output(string.Format("File name is not correct (expected:obtained): (({0}):({1}))", getFileName(actualFile) + " and expected file name is:  ", expectedFileName));
            }
            return passed;
        }

        /// <summary>
        /// This is a simple parsing method to check the file name without directory structure
        /// </summary>
        /// <param name="bpu">Uri representing path to the file</param>
        /// <returns>File name, stripped of preceding path information</returns>
        public static string getFileName(Uri bpu)
        {
            string fileString = bpu.ToString();
            int lastSlashIndexFwd = fileString.LastIndexOf('/');
            int lastSlashIndexBack = fileString.LastIndexOf('\\');

            if (lastSlashIndexFwd >= 0)
                return fileString.Substring(lastSlashIndexFwd + 1);
            else if (lastSlashIndexBack >= 0)
                return fileString.Substring(lastSlashIndexBack + 1);
            else if ((lastSlashIndexFwd == -1) && (lastSlashIndexBack == -1))
                return fileString;
            else
                return null;
        }
        #endregion

        
    }
}

