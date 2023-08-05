// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//******************************************************************* 
using Microsoft.Test.WindowsUIAutomation.Core;
using Microsoft.Test.WindowsUIAutomation.Logging;
// ------------------------------------------------------------------
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using System.CodeDom;
using System.Text;
using System.Resources;
using System.Diagnostics;
using System.Threading;
using MS.Win32;

// This suppresses warnings #'s not recognized by the compiler.
#pragma warning disable 1634, 1691

namespace Microsoft.Test.WindowsUIAutomation.Core
{
    using InternalHelper.Enumerations;
    using InternalHelper.Tests;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.TestManager;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class Library
    {
        /// -------------------------------------------------------------------
        /// <summary>
        /// Time to wait for events to happen
        /// </summary>
        /// -------------------------------------------------------------------
        internal const int MAXTIME = 60000;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Default waiting is 1/10th second between calls
        /// </summary>
        /// -------------------------------------------------------------------
        internal const int TIMEWAIT = 100;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Helps out with formating of the output
        /// </summary>
        /// -------------------------------------------------------------------
        const string PREBUFFER = "    ";

        /// -------------------------------------------------------------------
        /// <summary>
        /// Enumerations to define which ProxyTable to load
        /// </summary>
        /// -------------------------------------------------------------------
        public enum ProxyTable
        {
            /// ---------------------------------------------------------------
            /// <summary>Standard proxies that are shipped with WUI</summary>
            /// ---------------------------------------------------------------
            Default,

            /// ---------------------------------------------------------------
            /// <summary>Default fallback proxy</summary>
            /// ---------------------------------------------------------------
            Msaa,

            /// ---------------------------------------------------------------
            /// <summary>Standard Win32 and WinForms proxies</summary>
            /// ---------------------------------------------------------------
            Win32WinForms
        }

        /// -------------------------------------------------------------------
        /// <summary>Start application with ShellExecute with default timeout</summary>
        /// -------------------------------------------------------------------
        static public void StartApplicationShellExecute(string appPath, string caption, string arguments, out AutomationElement element)
        {
            StartApplicationShellExecute(appPath, caption, arguments, out element, MAXTIME);
        }

        /// -------------------------------------------------------------------
        /// <summary>Start application with ShellExecute</summary>
        /// -------------------------------------------------------------------
        static public void StartApplicationShellExecute(string appPath, string caption, string arguments, out AutomationElement element, int timeOutMilliSeconds)
        {

            Trace.WriteLine("Calling ShellExecute (" + appPath + ")");
            IntPtr hInstance = UnsafeNativeMethods.ShellExecute(IntPtr.Zero, null, appPath, arguments, null, 1 /*SW_SHOWNORMAL */);

            // hINstance really doesn't do you anything except tell you that it has failed if <= 32
            if (hInstance.ToInt64() <= 32)
            {
                Trace.WriteLine("Error stating application: " + hInstance);
                throw new Exception("UnsafeNativeMethods.ShellExecute() returned " + hInstance);
            }

            IntPtr hwnd = IntPtr.Zero;

            DateTime end = DateTime.Now + TimeSpan.FromMilliseconds(timeOutMilliSeconds);

            // Need to hunt for the window when you use ShellExecute unfortunately, but this is required 
            // call for security reasons.  StartProcess is not allowed.
            element = null;
            end = DateTime.Now + TimeSpan.FromMilliseconds(timeOutMilliSeconds);
            while (null == element)
            {
                if (DateTime.Now > end)
                    throw new TimeoutException("Could not find " + appPath + " in " + timeOutMilliSeconds / 1000 + " seconds  - Current time(" + DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + ":" + DateTime.Now.Millisecond.ToString("00") + ")");
                Thread.Sleep(TIMEWAIT);

                element = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, caption));
            }
        }
        /// -------------------------------------------------------------------------
        /// <summary>
        /// Start the application and retrieve it's AutomationElement as a member variable.
        /// </summary>
        /// -------------------------------------------------------------------------
        static public void StartApplication(string appPath, string arguments, out IntPtr handle, out AutomationElement element, out Process process)
        {
            Library.ValidateArgumentNonNull(appPath, "appPath");

            element = null;

            appPath = appPath.ToUpper();

            ProcessStartInfo psi = new ProcessStartInfo();

            process = new Process();

            psi.FileName = appPath;

            if (arguments != null)
            {
                psi.Arguments = arguments;
            }

            UIVerifyLogger.LogComment("{0}Starting({1})", PREBUFFER, appPath);
            process.StartInfo = psi;

#pragma warning suppress 6506 // We create a new one so don't need to validate it
            process.Start();

            UIVerifyLogger.MonitorProcess(process);

            UIVerifyLogger.LogComment("{0}Waiting for max of {1} seconds for application to start", PREBUFFER, MAXTIME / 1000);
            DateTime finishTime = DateTime.Now + TimeSpan.FromMilliseconds(MAXTIME);

            while (process.MainWindowHandle.Equals(IntPtr.Zero))
            {
                if (DateTime.Now > finishTime)
                    throw new Exception("Could not find " + appPath + " in " + MAXTIME + " milliseconds");

                Thread.Sleep(TIMEWAIT);

                process.Refresh();
            }

            // Special case code for these to test applications that are really console windows that start
            // up another test application.  Don't take the console window, but the test application that 
            // it shells.
            string title = string.Empty;
            if (appPath.IndexOf("WIN32PATTERNTARGET.EXE") != -1)
                title = "Win32 Pattern Target";

            if (title == string.Empty & appPath.IndexOf("WIN32TREETARGET.EXE") != -1)
                title = "Win32 Tree Test";

            // If it is special case, then find the window by title
            if (title != string.Empty)
            {
                finishTime = DateTime.Now + TimeSpan.FromMilliseconds(MAXTIME);
                handle = IntPtr.Zero;

                while (handle == IntPtr.Zero)
                {
                    if (DateTime.Now > finishTime)
                        throw new ArgumentException("Could not find the " + title + " window in " + MAXTIME + " milliseconds");

                    Thread.Sleep(TIMEWAIT);

                    handle = SafeNativeMethods.FindWindow(null, title);
                }

                element = AutomationElement.FromHandle(handle);
            }
            else // assume normal application window
            {
                handle = process.MainWindowHandle;
            }

            UIVerifyLogger.LogComment("{0}{1} started", PREBUFFER, appPath);
            element = AutomationElement.FromHandle(handle);


            UIVerifyLogger.LogComment("{0}Obtained an AutomationElement for {1}", PREBUFFER, appPath);
        }

        // -------------------------------------------------------------------
        // Closes Process
        // -------------------------------------------------------------------
        static public void StopApplication(Process process, string processName, int sleepTime)
        {
            UIVerifyLogger.LogComment("{0}Closing {1}", PREBUFFER, processName);

            if (process == null)
                throw new NullReferenceException();

            try
            {
                process.Kill();
                UIVerifyLogger.LogComment("{0}{1} has been terminated", PREBUFFER, processName);
            }
            catch (InvalidOperationException)
            {
                UIVerifyLogger.LogComment("{0}{1} has already been terminated", PREBUFFER, processName);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns a string that resembles output of UISpy tree
        /// </summary>
        /// -------------------------------------------------------------------
        static public string GetUISpyLook(AutomationElement le)
        {
            if (le == null)
                return "AutomationElement == null";

            string buffer = "";


            // Wrap calls since in the ExceptionNotFound tests, calling the properties throws an exception
            try
            {
                buffer += "\"" + le.Current.LocalizedControlType + "\" ";
            }
            catch (ElementNotAvailableException)
            {
                // For the ElementNotAvailableException tests, this might justifiably get thrown.
                buffer += "\"ElementNotAvailable\"";
            }
            try
            {
                buffer += "\"" + le.Current.Name + "\" ";
            }
            catch (ElementNotAvailableException)
            {
                // For the ElementNotAvailableException tests, this might justifiably get thrown.
                // Do something so that OARC does not complain
                buffer += "\"ElementNotAvailable\"";
            }
            try
            {
                buffer += "\"" + le.Current.AutomationId + "\" ";
            }
            catch (ElementNotAvailableException)
            {
                // For the ElementNotAvailableException tests, this might justifiably get thrown.
                // Do something so that OARC does not complain
                buffer += "\"ElementNotAvailable\"";
            }

            return buffer;
        }

        /// -------------------------------------------------------------------
        /// Check that specified argument is non-null, if so, throw exception
        /// -------------------------------------------------------------------
        static public void ValidateArgumentNonNull(object obj, string argName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(argName + "== null");
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Checks for critical nonrecoverable errors
        /// </summary>
        /// -------------------------------------------------------------------
        static public bool IsCriticalException(Exception exception)
        {
            return exception is SEHException || exception is NullReferenceException || exception is StackOverflowException || exception is OutOfMemoryException || exception is System.Threading.ThreadAbortException;
        }
    }
}
