// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Test.WindowsUIAutomation.Logging;
using Microsoft.Test.Logging;

namespace Microsoft.Test.WindowsUIAutomation.Logging
{
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class Logger : BaseLogger, IWUIALogger
    {

        #region Variables

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        TestLog _logger = null;

        #endregion Variables

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        public Logger()
            : base()
        {
            // Was never being used!
            //string TestAssemblyName = "UIVerify Tests";
        }

        #region IWUIALogger Overrides

        /// ---------------------------------------------------------------
        /// <summary>
        /// Some frameworks such as Piper will kill a process if it
        /// is closed incorrectly and hangs around.  If the framework
        /// does not support this, it will NOP out there
        /// </summary>
        /// <param name="process"></param>
        /// ---------------------------------------------------------------
        void IWUIALogger.MonitorProcess(Process process)
        {
            LogManager.LogProcessDangerously(process.Id);
            base.MonitorProcess(process);
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Close out the Piper log
        /// </summary>
        /// ---------------------------------------------------------------
        void IWUIALogger.CloseLog()
        {
            base.CloseLog();
        }

        #endregion IWUIALogger Overrides

        #region MethodOverrides

        /// ---------------------------------------------------------------
        /// <summary>
        /// Specifies that a test is starting
        /// </summary>
        /// ---------------------------------------------------------------
        protected override void WriteStartTest(object testName)
        {
            _logger = new TestLog(testName.ToString());
            _logger.Result = Microsoft.Test.Logging.TestResult.Pass;
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// Specifies that a test is ending
        /// </summary>
        /// ---------------------------------------------------------------
        protected override void WriteOutEndTest()
        {
            _logger.Close();
            _logger = null;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Log error
        /// </summary>
        /// -------------------------------------------------------------------
        protected override void WriteOutFail(object comment)
        {
            if (_logger == null)
            {
                GlobalLog.LogStatus(comment.ToString());
            }
            else
            {
                WriteOutComment(comment);
                _logger.Result = Microsoft.Test.Logging.TestResult.Fail;
            }
        }

        /// ---------------------------------------------------------------
        /// <summary>Write out the information for passing the test</summary>
        /// ---------------------------------------------------------------
        protected override void WriteOutPass()
        {
            _logger.Result = Microsoft.Test.Logging.TestResult.Pass;
        }

        /// -------------------------------------------------------------------
        /// <summary>Write out a comment</summary>
        /// -------------------------------------------------------------------
        protected override void WriteOutComment(object comment)
        {
            if (_logger != null)
                _logger.LogStatus(comment.ToString());
            else
                GlobalLog.LogStatus(comment.ToString());
        }
        #endregion MethodOverrides
    }
}
