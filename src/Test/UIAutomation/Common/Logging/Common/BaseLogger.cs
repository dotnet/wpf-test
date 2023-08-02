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

namespace Microsoft.Test.WindowsUIAutomation.Logging
{
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class RunResults
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal int _numberOfTests = 0;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal int _numberOfPasses = 0;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal int _numberOfTestFailures = 0;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal int _numberOfUnexpectedExceptions = 0;

    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class BaseLogger : RunResults, IWUIALogger
    {
        #region Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        protected string _elementPath;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        protected string _exampleCall;

        /// -------------------------------------------------------------------
        /// <summary>AutomationElement paths that report errors</summary>
        /// -------------------------------------------------------------------
        protected ArrayList _elementPaths = new ArrayList();

        /// -------------------------------------------------------------------
        /// <summary>Code that reports erros</summary>
        /// -------------------------------------------------------------------
        protected ArrayList _exampleCalls = new ArrayList();

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        protected enum TestResult
        {
            // Summary:
            // The result cannot be determined 
            // 
            // Remarks:
            // You should use this to indicate that an unexpected failure has occured that
            // prevents your test from determining the quality of the targeted
            // functionality.  
            Unknown = 0,
            //
            // Summary:
            // The targeted functionality was verified successfully.  
            Pass = 1,
            //
            // Summary:
            // The targeted functionality was verified as broken.  
            Fail = 2,
        }

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        protected DateTime _DateTimeStart = DateTime.Now;

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        bool _isTestRunning = false;

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        protected string _runningTest;

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        protected TestResult _testResult = TestResult.Pass;

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        protected ArrayList _Errors = new ArrayList();

        #endregion Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public BaseLogger() { }

        #region IWUIALogger Members

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual public void MonitorProcess(Process process)
        {
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual public void StartTest(string testName)
        {
            Debug.Assert(_isTestRunning == false, "A test(" + _runningTest + ") is already running.  Need to call TestEnd first");
            _numberOfTests++;

            _testResult = TestResult.Pass;
            _runningTest = testName;
            _isTestRunning = true;

            WriteStartTest(testName);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Closing the log allows the logger to write the information out
        /// if it is able to do so.
        /// </summary>
        /// -------------------------------------------------------------------
        virtual public void CloseLog()
        {

        }
        
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual public void EndTest()
        {
            if (_testResult == TestResult.Pass)
            {
                _numberOfPasses++;
            }
            else if (_testResult == TestResult.Fail)
            {
                _numberOfTestFailures++;
            }

            _isTestRunning = false;

            WriteOutEndTest();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual public void LogError(Exception exception, bool displayTrace)
        {
            _testResult = TestResult.Fail;

            // UIVerify Framework sets the inner exception
            exception = exception.InnerException == null ? exception : exception.InnerException;

            LogResults(TestResult.Fail, FormatMessage(exception, displayTrace));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual public void LogError(string error)
        {
            _testResult = TestResult.Fail;

            LogResults(TestResult.Fail, error);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual public void LogComment(object comment)
        {
            WriteOutComment(comment);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual public void LogPass()
        {
            //_numberOfPasses++;
            WriteOutPass();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual public void ReportResults()
        {
            WriteOutResults();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual public void LogUnexpectedError(Exception exception, bool displayTrace)
        {
            _numberOfUnexpectedExceptions++;

            _testResult = TestResult.Fail;

            LogResults(TestResult.Fail, FormatMessage(exception, displayTrace));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual public void CacheCodeExampleVariables(string elementPath, string exampleCall)
        {
            _elementPath = elementPath;
            _exampleCall = exampleCall;
            LogComment("Path : " + _elementPath);
            LogComment("Call : " + _exampleCall);
        }

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        virtual protected void LogResults(TestResult testResult, string message)
        {
            switch (testResult)
            {
                case TestResult.Fail:
                    {
                        WriteOutFail("###### : " + message);
                        CacheErrorInfo(message);
                        break;
                    }
                case TestResult.Pass:
                    {
                        WriteOutPass();
                        break;
                    }
                case TestResult.Unknown:
                    {
                        WriteOutFail("###### : " + message);
                        CacheErrorInfo(message);
                        break;
                    }
                default:
                    throw new Exception("Need to implement new TestResult value(" + testResult + ")");
            }
        }

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        virtual public object Results
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Test started at : " + _DateTimeStart.ToLongTimeString() + "\n");
                sb.Append("Test ended at : " + DateTime.Now.ToLongTimeString() + "\n");
                sb.Append("TOTAL TESTS: " + _numberOfTests + "\n");
                sb.Append("TOTAL PASSES: " + _numberOfPasses + "\n");
                sb.Append("TOTAL VERIFICATION FAILURES: " + _numberOfTestFailures + "\n");
                sb.Append("TOTAL UNEXPECTED EXCEPTIONS FAILURES: " + _numberOfUnexpectedExceptions + "\n");
                return sb.ToString();
            }
        }

        #endregion

        #region Overloads

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual protected void WriteOutTestStep(object error)
        {
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual protected void WriteOutUnexpectedError(object error)
        {
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual protected void WriteOutComment(object comment)
        {
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual protected void WriteOutPass()
        {
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual protected void WriteOutFail(object comment)
        {
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual protected void WriteOutEndTest()
        {
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual protected void WriteOutResults()
        {
            if (_Errors.Count > 0)
            {
                WriteOutComment("");
                WriteOutComment("");
                WriteOutComment("".PadRight(80, '-'));
                WriteOutComment("Failures");
                for (int i = 0; i < _Errors.Count; i++)
                {
                    WriteOutComment("//" + "".PadRight(78, '-'));
                    WriteOutComment("//  Test Error:  " + _Errors[i].ToString());
                    if (_elementPaths != null)
                    {
                        if (_elementPaths[i] != null)
                            WriteOutComment(_elementPaths[i].ToString());
                        if (_exampleCalls[i] != null)
                            WriteOutComment(_exampleCalls[i].ToString());
                    }
                }
            }
            WriteOutComment("");
            WriteOutComment("");
            WriteOutComment("".PadRight(90, '='));
            WriteOutComment("Test started at : " + _DateTimeStart.ToLongTimeString());
            WriteOutComment("Test ended at : " + DateTime.Now.ToLongTimeString());
            WriteOutComment("Total test run time : " + ((TimeSpan)(DateTime.Now - _DateTimeStart)).ToString());
            WriteOutComment("");
            WriteOutComment("TOTAL TESTS: " + _numberOfTests);
            WriteOutComment("TOTAL PASSES: " + _numberOfPasses);
            WriteOutComment("TOTAL VERIFICATION FAILURES: " + _numberOfTestFailures);
            WriteOutComment("TOTAL UNEXPECTED EXCEPTIONS FAILURES: " + _numberOfUnexpectedExceptions);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual protected void WriteStartTest(object testName)
        {
            Console.WriteLine("Test start: " + testName.ToString());
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        virtual protected void WriteCodeExample(string elementPath, string exampleCall)
        {
            Console.WriteLine("Path : " + elementPath);
            Console.WriteLine("Call : " + exampleCall);
        }

        #endregion Overloads

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        private void CacheErrorInfo(string message)
        {
            _exampleCalls.Add(_exampleCall);
            _elementPaths.Add(_elementPath);
            _Errors.Add(_runningTest + ":" + message);
        }

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        string FormatMessage(Exception exception, bool displayTrace)
        {
            string buffer = GetTime();
            int exceptionCount = 0;

            if (!displayTrace)
            {
                buffer += exception.Message;
            }
            else
            {
                do
                {
                    buffer += "\n----------------------------------\nInner Exception(" + ++exceptionCount + ")\n" +
                        "Source : " + exception.Source + "\n" +
                        "Message: " + exception.Message + "\n" +
                        "Trace  : " + exception.StackTrace;
                    exception = exception.InnerException;
                } while (exception != null);
            }
            return buffer;
        }

        /// ---------------------------------------------------------------
        /// <summary></summary>
        /// ---------------------------------------------------------------
        private string GetTime()
        {
            return string.Format("[{0}:{1}:{2}:{3}]", DateTime.Now.Hour.ToString("00"), DateTime.Now.Minute.ToString("00"), DateTime.Now.Second.ToString("00"), DateTime.Now.Millisecond.ToString("00").Substring(0, 2));
        }


    }
}

		
