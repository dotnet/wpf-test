// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using XamlCompilerCommon;

namespace XamlCommon.Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Logger implementation to get callbacks from XamlCompilerCommon 
    /// </summary>
    public class XamlCompilerTestLogger : TestLogger
    {
        /// <summary>
        /// Log the evidence
        /// </summary>
        /// <param name="message">message string</param>
        public override void LogEvidence(string message)
        {
            GlobalLog.LogEvidence(message);
        }

        /// <summary>
        /// Log the test as failed
        /// </summary>
        public override void LogFail()
        {
            TestLog.Current.Result = TestResult.Fail;
        }

        /// <summary>
        /// Log the given file
        /// </summary>
        /// <param name="fileName">filename of the file to log</param>
        public override void LogFile(string fileName)
        {
            GlobalLog.LogFile(fileName);
        }

        /// <summary>
        /// Log the test as passed
        /// </summary>
        public override void LogPass()
        {
            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Log test status
        /// </summary>
        /// <param name="status">status message to log</param>
        public override void LogStatus(string status)
        {
            GlobalLog.LogStatus(status);
        }
    }
}
