// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
// Base DRT exe stub and queued test state machine
//
//

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Threading;

using System.Text.RegularExpressions;
using System.Reflection;
using System.Security.Permissions;
using System.Windows;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Markup;
using System.Windows.Interop;

namespace DrtPayloads
{
#region PayloadsDrtHost
    //=====================================================================
    /// <summary>
    /// base DRT app and exe stub, creates app window and launches test
    /// </summary>
    public abstract class PayloadsDrtHost
    {
        private     string                      _suiteName;
        private     string                      _owner;

        private     DispatcherTimer             _pauseTimer;
        private     DispatcherOperationCallback _pausedTask;
        private     object                      _pausedTaskArgs;

        protected   bool                        _quietMode = true;
        private     bool                        _keepAppAlive;
        private     bool                        _hasWrittenLog;
        private     StringCollection            _suiteArgs = new StringCollection();     // argument passed to the test suite

        private     Dispatcher                  _dispatcher;
        private     HwndSource                  _source;

        protected   int                         _retCode;
        protected   string                      _verboseLogText;

        const       string                      PayLoadDrtFileDir=@".\";

        public abstract void            QueueTests();
        public abstract object          Run(object arg);

        public PayloadsDrtHost(string suiteName, string owner)
        {
            _suiteName = suiteName;
            _owner = owner;
            Console.WriteLine(String.Format("DRT test {0} owner [{1}]", suiteName, owner));
            Console.WriteLine("Use /? for usage, /v for verbose output, /p to pause for attach, and /k for keep alive");
            _verboseLogText = "";
        }

        public enum LogLevel
        {
            Progress,                   // Progress of test (appears on console)
            Verbose,                    // Detailed info (appears on console if /v) else accumulated into _verboseLogText
            Error                       // Test failure message (appears on console)
        }

        /// <summary>
        /// Write text to log literally (without end of line added)
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="msg"></param>
        public void LogText(LogLevel logLevel, string msg)
        {
            switch (logLevel)
            {
                case LogLevel.Progress:
                    _verboseLogText += msg;
                    Console.Write(msg);
                    break;

                case LogLevel.Verbose:
                    _verboseLogText += msg ;
                    if (!_quietMode)
                    {
                        Console.Write(msg);
                    }
                    break;

                case LogLevel.Error:
                    _verboseLogText += msg;
                    if (!_quietMode)
                    {
                        Console.Write(msg);
                        Console.WriteLine("-------- Test Failed --------");
                        _retCode = 1;
                    }

                    break;

                default:
                    LogErrorLine(String.Format("Test error - called LogText with bad LogLevel {0}", logLevel));
                    break;
            }
        }

        /// <summary>
        /// Writes text to log with end of line added
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="msg"></param>
        public void LogLine(LogLevel logLevel, string msg)
        {
            LogText(logLevel, msg + "\n");
        }

        public void LogProgressLine(string msg)
        {
            LogLine(LogLevel.Progress, msg);
        }

        public void LogVerboseLine(string msg)
        {
            LogLine(LogLevel.Verbose, msg);
        }

        public void LogErrorLine(string msg)
        {
            LogLine(LogLevel.Error, msg);
        }

        public StringCollection SuiteArgs
        {
            get     { return _suiteArgs; }
        }

        /// <summary>
        /// Main entry for a DRT application. Call this method from your DRT's Exe stub:
        /// <code>
        ///     class DrtFixedPanel : PayLoadsTestSuite {
        ///         public static int Main(String [] args) {
        ///             PayLoadsDrtHost drt = new PayLoadsDrtHost();
        ///             return drt.LaunchDRT(args,  new DrtFixedPanel(drt));
        ///         }
        ///         
        ///         public override void RunTests()
        ///         {
        ///             // do some test
        ///             // do some test
        ///             // do some test
        ///         }
        ///     }
        /// </code>
        /// </summary>
        /// <param name="args">expecting no command line arguments</param>
        /// <param name="drtObj">type of a class that will run the actual test,
        /// the drtObj is expected to expose a constructor that takes a MS.Internal reference parameter</param>
        /// <returns></returns>
        public int LaunchDRT(String [] args)
        {
            if (ProcessCmdArgs(args))
            {
                _dispatcher = Dispatcher.CurrentDispatcher;

                // Create windows;
                int windowWidth, windowHeight, windowStyle;
                windowWidth  = 720;
                windowHeight = 720;
                // windowStyle  = 0x10CF0000;
                windowStyle = NativeMethods.WS_THICKFRAME + NativeMethods.WS_CAPTION + NativeMethods.WS_VISIBLE + NativeMethods.WS_SYSMENU + NativeMethods.WS_MINIMIZEBOX;

                HwndSourceParameters param = new HwndSourceParameters("PayLoadsDrtHost", windowWidth, windowHeight);
                param.WindowStyle = windowStyle;
                param.SetPosition(0, 0);
                _source = new HwndSource(param);

                // hook up helper to listen to WM_CLOSE to shutdown the application
                _source.AddHook(new HwndSourceHook(ApplicationFilterMessage)); 
                NativeMethods.SetWindowTextW(_source.Handle, AppDomain.CurrentDomain.FriendlyName);

                _dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(Run), null);
                Dispatcher.Run();
            }

            if (_retCode == 1)
            {
                Console.WriteLine("DRT Failed");
            }
            else
            {
                Console.WriteLine("DRT Succeeded");
            }

            return _retCode;
        }


        public void PostTask(DispatcherOperationCallback innerTask)
        {
            Task task = new Task(this, innerTask, null);

            task.Run(_dispatcher);
        }

        public void PostTask(DispatcherOperationCallback innerTask, object arg)
        {
            Task task = new Task(this, innerTask, arg);

            task.Run(_dispatcher);
        }


        public void DelayPostTask(DispatcherOperationCallback innerTask, object arg, int pause)
        {
            Debug.Assert(pause > 0);
            _pausedTask = innerTask;
            _pausedTaskArgs = arg;
            if (_pauseTimer == null)
            {
                _pauseTimer = new DispatcherTimer();
                _pauseTimer.Tick += new EventHandler(DelayedTest);
            }
            LogProgressLine(String.Format("pausing {0} milliseconds", pause));
            _pauseTimer.Interval = TimeSpan.FromMilliseconds(pause);
            _pauseTimer.Start();
        }

        private void DelayedTest(object sender, EventArgs args)
        {
            _pauseTimer.Stop();
            PostTask(_pausedTask, _pausedTaskArgs);
        }

        public void AddTree(UIElement root)
        {
            _source.RootVisual = root;
        }

        public object LoadXaml(string fileName)
        {
            // Prepare to load xaml content
            FileInfo info = new FileInfo(fileName);
            Uri baseUri = new Uri(info.DirectoryName + @"\");

            Stream stream = info.OpenRead();

            ParserContext pc = new ParserContext();

            pc.BaseUri = baseUri;

            // Load xaml markup
            object sourceTree = XamlReader.Load(stream, pc);
            
            return sourceTree;
        }


        public Stream LoadFile(string fileName)
        {
            string file = PayLoadDrtFileDir + fileName;
            return File.OpenRead(file);
        }

        public HwndSource Source
        {
            get
            {
                return _source;
            }
        }

        // ------------------------------------------------------------------
        // Hook for shutting down the application when the window is closed
        //
        //      hwnd    - hwnd of the window
        //      msg     - window message
        //      wParam  - 
        //      lParam  -
        //      handled - out param, always returns false
        //
        // Returns: IntPtr.Zero 
        // ------------------------------------------------------------------
        private IntPtr ApplicationFilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Quit the application of the source window is closed.
            if (msg == NativeMethods.WM_CLOSE)
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ShutDownDRT), true);
                handled = true;
            }

            return IntPtr.Zero;
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// End the DRT app
        /// </summary>
        public object ShutDownDRT(object force)
        {
            Console.WriteLine("Ending DRT...");
            if ((bool)force || !_keepAppAlive) 
            {
                if (_pauseTimer != null)
                {
                    _pauseTimer.Stop();
                    _pauseTimer.Tick -= new EventHandler(DelayedTest);
                    _pauseTimer = null;
                }
                _dispatcher.InvokeShutdown();
            }
            return null;
        }

        public void ProcessError()
        {
            if (_quietMode)
            {
                Console.WriteLine("-------- Verbose output of failing test --------");
                Console.Write(_verboseLogText);
                Console.WriteLine("-------- End of verbose failure output --------");
            }

            // write to file
            string testLogFile = _suiteName + ".log";
            StreamWriter testLog = new StreamWriter(testLogFile, _hasWrittenLog);
            _hasWrittenLog = true;

            testLog.WriteLine("-------- Verbose output of failing test --------");
            testLog.Write(_verboseLogText);
            testLog.WriteLine("-------- End of verbose failure output --------");
            testLog.Close();

            _retCode = 1;
        }

        public void ResetVerboseLog()
        {
            _verboseLogText = "";
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// Assert the condition to be true. Otherwise set the DRT return code to 1.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="msg">message associated with condition</param>
        public void Assert(bool condition, string msg)
        {
            if (condition)
            {
                return;
            }

            LogErrorLine(msg);
            throw new VerifyException();
        }

        //
        // Verify "got" == "expected" otherwise report error. 
        //
        public void DrtVerify(string context, string errorMessage, object expected, object got)
        {
            if (expected == got)
            {
                return;
            }

            if (got.Equals(expected))
            {
                return;
            }

            Assert(false, ConcatMsg(context, errorMessage, expected, got));
        }


        //---------------------------------------------------------------------
        protected string ConcatMsg(string context, string errorMessage, object expected, object got)
        {
            return context + " -- " + errorMessage
                + ": EXPECTED=" + ((expected != null) ? expected.ToString() : "<null>")
                + ", GOT="      + ((got != null) ? got.ToString() : "<null>");
        }


 
        //---------------------------------------------------------------------
        private bool ProcessCmdArgs(string [] args)
        {
            bool okToContinue = true;

            _quietMode = true;
            _keepAppAlive = false;

            // Parse options: (/|-)option[:value]
            Regex regex = new Regex(@"(-|/)(((?<option>\w+):(?<value>\S+))|(?<option>\S+))");
            for (int i = 0; i < args.Length; i++)
            {
                Match m = regex.Match(args[i]);
                string option = m.Groups["option"].Value.ToUpper();
                string optValue = m.Groups["value"].Value;

                if (option.Length > 0)
                {
                    switch (option)
                    {
                        case "?":
                            PrintUsage();
                            okToContinue = false;
                            break; 

                        case "V":
                            Console.WriteLine("Verbose");
                            _quietMode = false;
                            break;
                        case "K":
                            Console.WriteLine("Keep Alive");
                            _keepAppAlive = true;
                            break;

                        case "P":
                            Console.WriteLine("Attach debugger and hit enter to continue");
                            Console.ReadLine();
                            break;

                        case "C":
                            _suiteArgs.Add(optValue);
                            break;
                    }
                }
            }
            return okToContinue;
        }

        public virtual void PrintUsage()
        {
            Console.WriteLine("Usage: " + AppDomain.CurrentDomain.FriendlyName + " [/?] [/v] [/k] [/c:<testArgs>] [/c:<testArgs>] ...");
            Console.WriteLine("  /?         Print Usage");
            Console.WriteLine("  /v         Verbose: output more status info");
            Console.WriteLine("  /p         Pause for attaching debugger");
            Console.WriteLine("  /k         KeepAlive: do not quit after DRT is run");
        }

        internal class VerifyException : ApplicationException
        { }

        internal class Task
        {
            private DispatcherOperationCallback _task;
            private Object                      _obj;
            private PayloadsDrtHost             _host;

            internal Task(PayloadsDrtHost host, DispatcherOperationCallback task, Object obj)
            {
                _task = task;
                _obj  = obj;
                _host = host;
            }

            internal void Run(Dispatcher dispatcher)
            {
                dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(TestRun), _obj);
            }

            internal object TestRun(object obj)
            {
                bool exceptionThrown = true;
                try
                {
                    _task(obj);
                    exceptionThrown = false;
                }
                catch (VerifyException)
                {
                    _host.ProcessError();
                    exceptionThrown = false; //not really an exception
                    _host.ShutDownDRT(true);
                }
                finally
                {
                    if (exceptionThrown)
                    {
                        _host.LogErrorLine("DRT threw an exception");
                        _host.ProcessError();
                    }

                    _host.ResetVerboseLog();
                }
                return null;
            }
        }

    }
    #endregion



    //=========================================================================
    //
    // A test suite contains multiple tests
    //
    #region Base class for TestSuite
    public abstract class PayloadsTestSuite : PayloadsDrtHost
    {
        public PayloadsTestSuite(string suiteName, string owner) : base(suiteName, owner)
        { }

        public override object Run(object arg)
        {
            QueueTests();
            return null;
        }
    }
#endregion




    // ------------------------------------------------------------------
    //
    //  Native methods
    //
    // ------------------------------------------------------------------

    #region Native Methods

    // ------------------------------------------------------------------
    // Win32 managed wrapper.
    // ------------------------------------------------------------------
    [System.Security.SuppressUnmanagedCodeSecurity()]
    internal class NativeMethods
    {
        // --------------------------------------------------------------
        // Windows styles
        // --------------------------------------------------------------
        internal const int WS_CAPTION       = 0x00C00000;
        internal const int WS_VISIBLE       = 0x10000000;
        internal const int WS_SYSMENU       = 0x00080000;
        internal const int WS_MINIMIZEBOX   = 0x00020000;
        internal const int WS_THICKFRAME    = 0x00040000;
        // --------------------------------------------------------------
        // Windows styles
        // --------------------------------------------------------------
        internal const int WM_CLOSE         = 0x0010;

        // --------------------------------------------------------------
        // System metrics flags
        // --------------------------------------------------------------
        internal const int SM_CYCAPTION     = 4;
        internal const int SM_CXFIXEDFRAME  = 7;
        internal const int SM_CYFIXEDFRAME  = 8;
        internal const int SM_CXFRAME       = 32;
        internal const int SM_CYFRAME       = 33;

        // --------------------------------------------------------------
        // RECT struct
        // --------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT 
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;

            internal RECT(int left, int top, int right, int bottom) 
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        // --------------------------------------------------------------
        // GetClientRect
        // --------------------------------------------------------------
        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal static extern bool GetClientRect(IntPtr hWnd, [In, Out] ref RECT rect);

        // --------------------------------------------------------------
        // GetSystemMetrics
        // --------------------------------------------------------------
        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal static extern int GetSystemMetrics(int nIndex);

        // --------------------------------------------------------------
        // SetWindowText
        // --------------------------------------------------------------
        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Unicode)]
        internal static extern bool SetWindowTextW(IntPtr hWnd, string text);
    }

    #endregion Native Methods
}
