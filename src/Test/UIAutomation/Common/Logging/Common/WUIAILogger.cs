// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description:		Testing interface

using System;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using Microsoft.Test.WindowsUIAutomation.Logging;

namespace Microsoft.Test.WindowsUIAutomation.Logging
{
    /// -----------------------------------------------------------------------
    /// <summary>
    /// Interface into the different loggers
    /// </summary>
    /// -----------------------------------------------------------------------
    public interface IWUIALogger
    {
        /// -------------------------------------------------------------------
        /// <summary>
        /// Closing the log allows the logger to write the information out
        /// if it is able to do so.
        /// </summary>
        /// -------------------------------------------------------------------
        void CloseLog();

        /// -------------------------------------------------------------------
        /// <summary>
        /// Some frameworks such as Piper will kill a process if it
        /// is closed incorrectly and hangs around.  If the framework
        /// does not support this, it will NOP
        /// </summary>
        /// <param name="process">Process to monitor and kill if bad things happen</param>
        /// -------------------------------------------------------------------
        void MonitorProcess(Process process);

        /// -------------------------------------------------------------------
        /// <summary>
        /// Specifies that a test is starting
        /// </summary>
        /// <param name="testName">Name of the test that will be ran</param>
        /// -------------------------------------------------------------------
        void StartTest(string testName);

        /// -------------------------------------------------------------------
        /// <summary>
        /// Specifies that a test is ending
        /// </summary>
        /// -------------------------------------------------------------------
        void EndTest();

        /// -------------------------------------------------------------------
        /// <summary>
        /// Report that the test had an error
        /// </summary>
        /// -------------------------------------------------------------------
        void LogError(Exception exception, bool displayTrace);

        /// -------------------------------------------------------------------
        /// <summary>
        /// Report that the test had an error
        /// </summary>
        /// -------------------------------------------------------------------
        void LogError(string error);

        /// -------------------------------------------------------------------
        /// <summary>
        /// Add a comment to the test summary
        /// </summary>
        /// <param name="comment">Comment to send to the output</param>
        /// -------------------------------------------------------------------
        void LogComment(object comment);

        /// -------------------------------------------------------------------
        /// <summary>Log that the test passed</summary>
        /// -------------------------------------------------------------------
        void LogPass();

        /// -------------------------------------------------------------------
        /// <summary>
        /// Report summary results
        /// </summary>
        /// -------------------------------------------------------------------
        void ReportResults();

        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns a report summary results
        /// </summary>
        /// -------------------------------------------------------------------
        object Results { get;}

        /// -------------------------------------------------------------------
        /// <summary>
        /// Log unexpected error that the test didn't expect
        /// </summary>
        /// -------------------------------------------------------------------
        void LogUnexpectedError(Exception exception, bool displayTrace);

        /// -------------------------------------------------------------------
        /// <summary>
        /// Store the information to make a call into the test case at a later time
        /// </summary>
        /// -------------------------------------------------------------------
        void CacheCodeExampleVariables(string elementPath, string exampleCall);
    }
}
