// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
// Defines helper delegates to log detailed test execution steps.
// Defines default delegate implementation. Provides log function
// that use default delegate implementation to log test details. Test Case can
// change the delegate used for logging using properties of LogHelper object.
//
// Note: LogMessageWindow should be set before using Log functions
//
//
//

using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.Animation
{

    /// <summary>
    /// Delegate for logging test execution steps
    /// </summary>
    public delegate void LogMessage(string message);

    /// <summary>
    /// Delegate for logging test result
    /// </summary>
    public delegate void LogTestResult(bool result, string message);

    /// <summary>
    /// Defines default delegate implementations. Provides log function
    /// that use default delegate implementation to log test details. Test case can
    /// change the delegate using properties of LogHelper object.
    /// </summary>
    public class LogHelper : WindowTest
    {
        #region Public Helper APIs

        /// <summary>
        /// Constructor
        /// </summary>
        static LogHelper()
        {
            s_logMessageFunc  = new LogMessage(DefaultLogMessageFunc);
            s_logWindow = null;
            ResetPartialResults();
        }

        /// <summary>
        /// Wrapper function that calls the delegate to log test execution steps.
        /// Helper functions in Microsoft.Test.Animation should always call
        /// this function to log test execution steps
        /// </summary>
        /// <param name="message"></param>
        public static void LogTestMessage(string message)
        {
            s_logMessageFunc(message);
        }

        /// <summary>
        /// Wraps the call that log test result
        /// </summary>
        /// <param name="pass">partial result</param>
        /// <param name="reason">description</param>
        public static void LogTest(bool pass, string reason)
        {
            // accumulate test result
            s_partialResult &= pass;

            // echo this ...
            string message = "[ TestCase Result: " + (pass ? "PASS" : "FAIL") + " ] " + reason;

            // save this
            s_lastResultMessage = reason;

            // if we have a log window, color it accordingly
            if (s_logWindow != null )
            {
                QueueTestResultLog tl = new QueueTestResultLog();
                tl.LogFinalResult(message,s_logWindow);
                GlobalLog.LogEvidence(message);
            }
        }

        /// <summary>
        /// Wraps the call that log test result using Automation Framework
        /// </summary>
        /// <param name="pass">partial result</param>
        /// <param name="reason">description</param>
        public static void LogSubTest(bool pass, string reason, int subTestID )
        {
            // accumulate test result
            s_partialResult &= pass;

            // accumulate sub-variants
            if ( pass )
            {
                s_subTestPass++;
            }
            else
            {
                s_subTestFail++;
            }

            // echo this ...
            string message = String.Format( "[ TestCase Result: {0} SubTest({2}) ] {1}",
                (pass ? "PASS" : "FAIL"), reason, subTestID );

            // save this
            s_lastResultMessage = reason;

            // if we have a log window, color it accordingly
            if (s_logWindow != null )
            {
                QueueTestResultLog tl = new QueueTestResultLog();
                tl.LogFinalResult(message,s_logWindow);
            }
        }


        /// <summary>
        /// Sets the internal result to true, so that it can be anded with partial results
        /// </summary>
        public static void ResetPartialResults()
        {
            s_partialResult = true;
            s_subTestPass = 0;
            s_subTestFail = 0;
        }

        /// <summary>
        /// Wraps the call that log test result
        /// </summary>
        /// <param name="reason">description</param>
        public static void LogTestToFramework(string reason)
        {
            Queue.Flush();

            string message = "Unknown";
            try
            {
                message = String.Format(
                    "[- FINAL REPORTED RESULT: {0} -] {1}",
                    (s_partialResult ? "PASS" : "FAIL"), reason );
            }
            catch( System.Exception ex )
            {
                LogTestMessage( ex.ToString() );
            }

            // log variations when available
            TestLog frmwk = new TestLog("AnimationOnElement Test");
            frmwk.LogStatus(message);

            if ( s_subTestPass != 0 || s_subTestFail != 0 )
            {
                frmwk.Result = TestResult.Pass;
            }
            else
            {
                frmwk.Result = TestResult.Fail;
            }
            frmwk.Close();

        }

        /// <summary>
        /// Set/Get the delegate used for logging test message
        /// </summary>
        public static LogMessage LogMessageDelegate
        {
            set
            {
                s_logMessageFunc = value;
            }
            get
            {
                return s_logMessageFunc;
            }
        }

        /// <summary>
        /// Set/Get DockPanel used for Logging Messages
        /// Set LogMessageWindow to null if you don't want to log to Avalon Window
        /// </summary>
        public static DockPanel LogMessageWindow
        {
            set
            {
                s_logWindow = value;
                if ( value != null )
                {
                    Hyperlink hypLink = null;
                    if( hypLink == null )
                    {
                        // add a default back link ..
                        System.Windows.Documents.Hyperlink backLink = new System.Windows.Documents.Hyperlink();
                        backLink.Name                                = s_backLinkID;
                        System.Windows.Controls.TextBlock tb = new System.Windows.Controls.TextBlock();
                        System.Windows.Controls.DockPanel.SetDock(tb, System.Windows.Controls.Dock.Top);
                        backLink.NavigateUri = new Uri("Integration.xaml", UriKind.RelativeOrAbsolute);
                        tb.Inlines.Add(backLink);
                        ((System.Windows.Markup.IAddChild)(backLink)).AddText("[ Back to main page ]");
                        ((System.Windows.Markup.IAddChild)(s_logWindow)).AddChild(tb);
                    }
                }
            }
            get
            {
                return s_logWindow;
            }
        }

        /// <summary>
        /// Returns the last Log Message
        /// </summary>
        public static string Message
        {
            set
            {
                s_messageText = value;
            }

            get
            {
                return s_messageText;
            }

        }

        /// <summary>
        /// Returns the last Test Result Message
        /// </summary>
        public static string LastResultMessage
        {
            get
            {
                return s_lastResultMessage;
            }
        }


        #endregion Public Helper APIs

        #region Internal functions

        /// <summary>
        /// Default implemenation of LogMessage delegate
        /// </summary>
        /// <param name="message"></param>
        internal static void DefaultLogMessageFunc(string message)
        {
            // If LogWindow is not NULL - user want to log to an Avalon Window too
            if ( s_logWindow != null )
            {
                QueueTestLog tl = new QueueTestLog();
                tl.LogIntermediateMessage( message, s_logWindow );
            }
        }

        #endregion Internal functions


        #region Data
        // Delegate used for logging
        private  static LogMessage s_logMessageFunc;
        // DockPanel - Parent of the System.Windows.Documents.Text UIElement that display Log Message
        private static DockPanel s_logWindow;
        // Log Message Text
        private static string s_messageText;
        // Partial Test Result
        private static bool s_partialResult;
        // Last Partial Test Result message
        private static string s_lastResultMessage;
        // BackLink Name
        private static string s_backLinkID = "BackLink";
        // Partial results
        private static int s_subTestPass = 0;
        private static int s_subTestFail = 0;
        #endregion Data

    }


    #region Public Log Objects

    /// <summary>
    /// Uses the Avalon Dispatcher queueing to write to a log UIElement
    /// </summary>
    public class QueueTestLog
    {
        protected string      message   = null; // log message
        protected Panel   logWindow = null; // element to append the text to


        public QueueTestLog()
        {
        }

        /// <summary>
        /// Static log function pushes an instance into the queue
        /// </summary>
        public void LogIntermediateMessage( string msg, Panel el)
        {
            message     = msg;
            logWindow   = el;

            el.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new DispatcherOperationCallback(WriteLog),
                null);
        }

        /// <summary>
        /// Dispatch adds the text nodes to the tree
        /// </summary>
        protected virtual object WriteLog(object arg)
        {
            if ( logWindow != null && message != null)
            {
                // create border
                System.Windows.Controls.Border bd =
                    new System.Windows.Controls.Border();
                DockPanel.SetDock(bd, System.Windows.Controls.Dock.Top);

                // Color Failures in Red
                if (    message.IndexOf("FAIL") != -1   ||
                        message.IndexOf("ERROR") != -1  ||
                        message.IndexOf("False") != -1  )
                {
                    System.Windows.Media.Brush br =
                        new System.Windows.Media.SolidColorBrush( Color.FromScRgb(0.8F, 1.0F, 0.2F, 0.2F) );
                    bd.Background = br;
                }
                // Color Passes in Green
                if (    message.IndexOf("SUCCESS") != -1    ||
                        message.IndexOf("PASS") != -1       ||
                        message.IndexOf("True") != -1       )
                {
                    System.Windows.Media.Brush br =
                        new System.Windows.Media.SolidColorBrush( Color.FromScRgb(0.8F, 0.2F, 1.0F, 0.2F) );
                    bd.Background = br;
                }

                // create text
                System.Windows.Controls.TextBlock tx =
                    new System.Windows.Controls.TextBlock();

                // Add text to node
                tx.Text = message;
                // Add text to boder
                ((System.Windows.Markup.IAddChild)bd).AddChild(tx);
                // Add border to window

                // this adds data in order (new content at top of log window)
                logWindow.Children.Add(bd);

                // The window where content is fed into doesn't scroll to the recently added 
                // data, so we invalidate to give it a chance to autoscroll.
                logWindow.InvalidateArrange();
                logWindow.InvalidateMeasure();
                logWindow.BringIntoView();
                logWindow.UpdateLayout();
                return true;
            }
            else
                return false;
        }
    }


    /// <summary>
    /// Queue logger that also colorized the background Red/Green on the
    /// test case result.
    /// </summary>
    class QueueTestResultLog : QueueTestLog
    {
        public QueueTestResultLog() : base()
        {
        }

        /// <summary>
        /// Colorize the log window element background to green/red. Usually the
        /// last log entry in the test case
        /// </summary>
        public void LogFinalResult( string msg, Panel el )
        {
            message     = msg;
            logWindow   = el;

            el.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new DispatcherOperationCallback(WriteLog),
                null);
        }

        /// <summary>
        /// /// Dispatch adds the text nodes to the tree and sets the background color on pass/fail
        /// </summary>
        protected override object WriteLog(object arg)
        {
            // log text
            base.WriteLog(null);

            // set log window color
            if ( logWindow != null && message != null)
            {
                System.Windows.Controls.Border bd =
                    ((System.Windows.FrameworkElement)logWindow).Parent
                        as System.Windows.Controls.Border;
                if ( bd != null )
                {
                    if (message.IndexOf("FAIL") != -1 || message.IndexOf("ERROR") != -1)
                    {
                        System.Windows.Media.Brush br =
                            new System.Windows.Media.SolidColorBrush( Color.FromScRgb(1.0F, 1.0F, 0.4F, 0.4F) );
                        bd.Background = br;
                    }
                    if (message.IndexOf("SUCCESS") != -1 || message.IndexOf("PASS") != -1 )
                    {
                        System.Windows.Media.Brush br =
                            new System.Windows.Media.SolidColorBrush( Color.FromScRgb(1.0F, 0.4F, 1.0F, 0.4F) );
                        bd.Background = br;
                    }
                }
                return true;
            }
            else
                return false;
        }
    }


    /// <summary>
    /// A simple logger that writes to a file. Asserts the
    /// appropriate permissions.
    /// </summary>
    public sealed class TestLogFile
    {
        /// <summary>
        /// Internal memeber with path name
        /// </summary>
        private string _filePath;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">log path</param>
        public TestLogFile( string path )
        {
            _filePath = path;
        }

        /// <summary>
        /// Logs to a file.
        /// Opens the file, appends and closes on each call, appends newline.
        /// </summary>
        /// <param name="msg">log string</param>
        public void Log2( string msg )
        {
            TrustedHelper.WriteToFile( msg, _filePath );
        }
    }

    #endregion Public Log Objects
}
