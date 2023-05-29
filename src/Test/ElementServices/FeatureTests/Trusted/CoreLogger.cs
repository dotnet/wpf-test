// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Very thin convenience wrapper around ClientTestRuntime's 
 *          logging.
 * Contributors: 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Very thin convenience wrapper around ClientTestRuntime's logging.
    /// </summary>
    public static class CoreLogger
    {

        /// <summary>
        /// Set true if you want to have a ThreadId number before each LogStatus
        /// </summary>
        public static bool LogThreadId = false;

        private static Result? s_cachedLogResult;

        private static string WriteThreadId()
        {
            string threadId = "";

            if (LogThreadId)
            {
                threadId = "ThreadId " + Thread.CurrentThread.ManagedThreadId.ToString() + ": ";
            }

            return threadId;
        }

        // CoreLogger has two use semantics - throwing an exception at point of
        // failure which means that reaching EndVariation indicates the test
        // passes, and calling LogTestResult proactively. In order to support
        // both behavior we cache the log result from LogTestResult. When
        // EndVariation is called we can see whether there is a cached log
        // result. If so it is the former case and we just end the variation.
        // If it is null the test is assumed to be using the former behavior,
        // in which case we log a pass.

        public static void BeginVariation()
        {
            s_cachedLogResult = null;
            Log.Current.CreateVariation(DriverState.TestName);
        }

        public static void BeginVariation(string name)
        {
            s_cachedLogResult = null;
            Log.Current.CreateVariation(name);
        }

        public static void EndVariation()
        {
            if (s_cachedLogResult == null)
            {
                Variation.Current.LogResult(Result.Pass);
            }
            s_cachedLogResult = null;
            Log.Current.CurrentVariation.Close();
        }

        /// <summary>
        /// Forwards to GlobalLog.LogStatus().
        /// </summary>
        public static void LogStatus(string text)
        {

            GlobalLog.LogStatus(WriteThreadId() + text);
        }

        /// </summary>        
        public static void LogStatus(string text, ConsoleColor consoleColor)
        {
            GlobalLog.LogStatus(WriteThreadId() + text);
        }

        /// <summary>
        /// Formats a log message based on an Exception.
        /// </summary>
        [LoggingSupportFunction]
        public static void LogException(Exception exception)
        {
            // This simply calls Exception.ToString() to log the top Exception
            // and all of its inner exceptions.
            LogStatus(exception.ToString(), ConsoleColor.Red);
        }

        /// <summary>
        /// Logs a test result with a comment.
        /// </summary>
        public static void LogTestResult(bool bResult, string commentStr)
        {
            bool debuggerBreak = false;
            string vars = Environment.GetEnvironmentVariable("CoreDebugBreak");
            if (!String.IsNullOrEmpty(vars))
            {
                vars = vars.Trim().ToLowerInvariant();                
                if (vars == "true")
                {                    
                    debuggerBreak = true;
                }
            }

            if (!bResult)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    CoreLogger.LogStatus("Debugger Break");
                    System.Diagnostics.Debugger.Break();
                }
                else if (debuggerBreak)
                {
                    CoreLogger.LogStatus("Debugger Launch");
                    System.Diagnostics.Debugger.Launch();
                }
            }

            if (Variation.Current == null)
            {
                throw new InvalidOperationException("Test tried to log result with first calling BeginVariation");
            }
            if (s_cachedLogResult != null)
            {
                throw new InvalidOperationException("Test tried to log result a second time");
            }

            Variation.Current.LogMessage(commentStr);
            if (bResult)
            {
                Variation.Current.LogResult(Result.Pass);
                s_cachedLogResult = Result.Pass;
            }
            else
            {
                Variation.Current.LogResult(Result.Fail);
                s_cachedLogResult = Result.Fail;
            }
        }

        /// <summary>
        /// Adds a process id to the monitored processes collection. The monitored ids 
        /// may be used to kill processes when a timeout occurs.
        /// </summary>
        public static void MonitorProcess(Process process)
        {
            LogManager.LogProcessDangerously(process.Id);
        }

        /// <summary>
        /// Kills all processes in the monitored processes collection that have not exited already.
        /// </summary>
        public static void KillMonitoredProcesses()
        {
            try
            {
                //Note: Test Infrastructure ensures all processes including driver/test are terminated after test is over.
                //Harness.Current.KillMonitoredProcesses();
            }
            catch (System.ComponentModel.Win32Exception we)
            {
                // Report everything except access-denied errors. 

                // System.Diagnostics.Process.Kill() throws this exception if the process could not be terminated.
                // On x64 platforms we sometimes see spurious access-denied errors, probably because the process has already exited.

                CoreLogger.LogStatus("Tried to kill processes, encountered Win32 error " + we.NativeErrorCode.ToString("x") + ":\n" + we.ToString());

                if (we.NativeErrorCode != NativeConstants.ERROR_ACCESS_DENIED)
                {
                    throw;
                }
            }
            catch (InvalidOperationException ioe)
            {
                // Ignore any invalid operations. 

                // System.Diagnostics.Process.Kill() throws this exception if the process has already exited.
                // Handling this here lets a test pass selected by Area continue without aborting.

                CoreLogger.LogStatus("Tried to kill processes, encountered invalid operation:\n" + ioe.ToString());
            }

        }

        /// <summary>
        /// This API is to support the using statement: Start:End and End:Text
        /// </summary>
        [LoggingSupportFunction]
        public static IDisposable AutoStatus(string text)
        {
            IDisposable iDisposable = null;
            lock (s_syncObject)
            {
                iDisposable = new AutoStatus(text);
            }

            return iDisposable;
        }

        static object s_syncObject = new Object();
    }

    ///<summary>
    /// Enables logged sections of test code to be automatically wrapped by
    /// a comment with indentation.
    ///</summary>
    [Serializable]
    public class AutoStatus : IDisposable
    {
        ///<summary>
        ///</summary>
        [LoggingSupportFunction]
        internal AutoStatus(string comment)
        {
            _comment = comment;

            _ident = CreateIdentation(++Indentation);

            CoreLogger.LogStatus("Start: " + _ident + _comment);
        }

        ///<summary>
        ///</summary>
        [LoggingSupportFunction]
        public void Dispose()
        {
            CoreLogger.LogStatus("End: " + _ident + _comment);
        }

        ///<summary>
        ///</summary>
        [ThreadStatic]
        public int Indentation = 0;

        string _ident = "";

        string CreateIdentation(int v)
        {
            string s = "";
            for (int i = 0; i < v; i++)
                s += " ";
            return s;
        }

        string _comment = String.Empty;
    }
}


