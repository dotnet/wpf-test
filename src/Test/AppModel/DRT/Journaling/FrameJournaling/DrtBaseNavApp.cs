// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Framework for DRT tests
//
using System;
using System.Reflection;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Text;

/************************************************************************

The DrtBase class provides a handy framework for running DRTs.  Here's some
terminology we use in the following explanation:

    DRT - a single exectuable, e.g. DrtUiBind.exe.  It comprises one or more
        test suites.
    Test suite - a list of DRT tests, usually related to each other.  Often
        the suite comprises all the tests that act on a given tree (which is
        either created in initialization code, or is loaded via the parser from
        a .xaml file).
    DRT test - a single method, part of a test suite.  After each test, the
        dispatcher runs to react to all pending asynchronous work the test
        may have caused.

The intent is that each suite starts off by creating (or loading) a tree, then
acts on that tree in various ways.  Often the actions (tests) occur in pairs:
one test that does something, and another test that verifies that Avalon did
the right thing in response.  Putting these in separate "tests" permits the
asynchronous parts of Avalon to run in between.

Here's how to use DrtBase.

1. Create a directory for your DRT.  E.g. devtest\DataSrv\UiBind
2. Include this file (DrtBase.cs) in the 'sources' list, using a relative path.
3. Write a class that derives from DrtBase.  Override virtuals as needed.
Follow this example:

    public sealed class MyDRTClass : DrtBase
    {
        public static int Main(string[] args)
        {
            DrtBase drt = new MyDRTClass();
            return drt.Run(args);
        }

        private MyDRTClass()
        {
            WindowTitle = "My DRT";
            TeamContact = "Wpf";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[]{
                        new MyFirstTestSuite(),
                        new MySecondTestSuite(),    // repeat as needed
                        null            // list terminator - optional
                        };
        }

        // Override this in derived classes to handle command-line arguments one-by-one.
        // Return true if handled.
        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
                return true;

            // process your own arguments here, using these parameters:
            //      arg     - current argument
            //      option  - true if there was a leading - or /.
            //      args    - all arguments
            //      k       - current index into args
            // Here's a typical sketch:

            if (option)
            {
                switch (arg)    // arg is lower-case, no leading - or /
                {
                    case "foo":             // simple boolean option:   -foo
                        _foo = true;
                        break;

                    case "use":             // option with parameter:  -use something
                        _something = args[++k];
                        break;

                    default:                // unknown option.  don't handle it
                        return false;
                }
                return true;
            }
            else                            // non-option argument:   <filename>
            {
                _files.Add(arg);
                return true;
            }

            return false;
        }

        // Print a description of command line arguments.  Derived classes should
        // override this to describe their own arguments, and then call
        // base.PrintOptions() to get the DrtBase description.
        protected override void PrintOptions()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("  filename ...  examine named files");
            Console.WriteLine("  -foo          enable the foo flag");
            Console.WriteLine("  -use something   use the named thing");
            base.PrintOptions();
        }

4. Write a class that derives from DrtTestSuite.  Follow this example:
    public sealed class MyTestSuite : DrtTestSuite
    {
        public MyTestSuite() : base("MySuiteName")
        {
            TeamContact = "Wpf";     // if different from DRT
            Contact = "Microsoft";         // if different from DRT
        }

        public override DrtTest[] PrepareTests()
        {
            // initialize the suite here.  This includes loading the tree.

            // For example to create a tree via code:
            Visual root = CreateMyTree();
            DRT.RootElement = root;

            // Or to load a tree from a .xaml file:
            DRT.LoadXamlFile(@"MyTree.xaml");

            // return the lists of tests to run against the tree
            return new DrtTest[]{

                        new DrtTest( DoSomething ),
                        new DrtTest( VerifySomething ),

                        new DrtTest( DoSomethingSychronousAndVerifyItRightAway ),

                        // repeat as needed

                        null        // list terminator - optional
                        };
        }

        // Testing an action that Avalon reacts to asynchronously:
        void DoSomething()
        {
            // your action goes here
        }

        void VerifySomething()
        {
            // check that Avalon reacted correctly.  Assert if it didn't:
            DRT.Assert(condition, "message");
            DRT.AssertEqual(expected, actual, "message");
        }

        // Testing an action that Avalon reacts to synchronously:
        void DoSomethingSychronousAndVerifyItRightAway()
        {
            // your action
            // your verification
        }
    }
5. Compile it all and run.
6. DrtBase provides various other common services you can use via the DRT
property of your suite.  Look at the public properties/methods/events below.

About logging:

DrtBase handles logging in a very nice way. Feel free to sprinkle Console.WriteLines
throughout your DRT. By default, they will be buffered into a StringBuilder, and they
will not actually be displayed unless an exception occurs.

If you specify the -verbose command line option, all messages will be printed immediately.

Finally, you can use DrtBase's Verbose property to condition some of your output, e.g.:

    if (Verbose)
        Console.WriteLine("Really detailed output");

Such messages will not be printed even if an exception occurs. But they -will- be
printed if you specify the -verbose command line option.

************************************************************************/
namespace DRT
{
    // base class for a DRT application
    public abstract class DrtBase
    {
        //------------------------------------------------------
        //
        //  Public P/M/E
        //
        //------------------------------------------------------
        protected DrtBase()
        {
            string assertExit = Environment.GetEnvironmentVariable("AVALON_ASSERTEXIT");

            if (assertExit != null && assertExit.Length > 0)
            {
                _loudAsserts = true;
                _assertsAsExceptions = true;
                _catchExceptions = false;
            }

            _delayedConsoleOutput = new StringWriter(_consoleOutputStringBuilder);
        }

        /// <summary>
        /// Run the DRT.  This means:
        ///  1. process command-line arguments
        ///  2. create a top-level window
        ///  3. run a dispatcher 
        ///  4. run each test suite
        ///  5. catch any unhandled exceptions
        ///  6. shut down when the dispatcher returns, or on WM_CLOSE
        //
        /// </summary>
        /// <param name="args">command-line arguments</param>
        /// <returns>DRT exit code -- to be returned from Main()</returns>
        public int Run(string[] args)
        {
            _retcode = ReadCommandLineArguments(args);
            if (_retcode != 0)
                return _retcode;

            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "DRT Main";
            }

            // Add some well-known assemblies to the "used" list.  This helps catch
            // problems where the DRT fails because it's using the wrong version
            // of Avalon assemblies (this can happen when the user's environment
            // has some mixture of system GAC, razzle GAC, and local files).
            //
            // Authors of derived DRT classes can make additional calls to UseType
            // (after creating the DRT object, and before calling Run on it) if
            // they use any other assemblies.
            UseType(typeof(FrameworkElement));      // PresentationFramework.dll
            UseType(typeof(Visual));                // PresentationCore.dll
            UseType(typeof(Dispatcher));             // WindowsBase.dll
            UseType(typeof(AutomationElement));        // WindowsUIAutomation.dll
            UseType(typeof(DrtBase));               // current .exe
            if (_reportInfo)
            {
                ReportDrtInfo();
                return _retcode;
            }

            if (CatchExceptions)
            {
                // log unhandled exceptions
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);
                try
                {
                    RunCore();
                }
                catch (Exception e)
                {
                    WriteDelayedOutput();
                    ReportException(e);
                }
                finally
                {
                    OutputOnlyOnFail = false;
                }
            }
            else
            {
                if (OutputOnlyOnFail)
                {
                    bool exceptionThrown = true;

                    try
                    {
                        RunCore();
                        exceptionThrown = false;
                    }
                    finally
                    {
                        if (exceptionThrown)
                        {
                            WriteDelayedOutput();
                        }

                        OutputOnlyOnFail = false;
                    }
                }
                else
                {
                    RunCore();
                }
            }

            bool fFailed = _totalFailures > 0 || _retcode != 0;

            Log(fFailed ? "FAILED" : "SUCCEEDED");
            return _retcode;
        }

        void RunCore()
        {
            try
            {
                if (_blockInput)
                {
                    Win32BlockInput(true);
                }

                // Make sure if this DRT sends any input that it will reset the screen saver.
                MakeSendInputResetScreenSaver();
                s_drt = this;
                s_drtStarted = true;
                if (_outputOnlyOnFail)
                    Console.SetOut(_delayedConsoleOutput);

                // run the tests
                this.Application.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(RunNextSuite), null);
                
                // set up the main window
                // MainWindow.AddHook(new HwndSourceHook(ApplicationFilterMessage));
                this.Application.Run();
                this.Application = null;
            }
            finally
            {
                if (_blockInput)
                {
                    Win32BlockInput(false);
                }
            }
        }

        /// <summary>
        /// Process command line arguments.
        /// </summary>
        private int ReadCommandLineArguments(string[] args)
        {
            for (int k = 0; k < args.Length; ++k)
            {
                string arg = args[k];
                bool option = (arg[0] == '-' || arg[0] == '/');

                if (option)
                    arg = arg.Substring(1).ToLower();

                if (HandleCommandLineArgument(arg, option, args, ref k))
                    continue;

                bool handled = false;

                foreach (DrtTestSuite suite in _suite)
                {
                    if (suite == null)
                        continue;

                    if (handled = suite.HandleCommandLineArgument(arg, option, args, ref k))
                        break;
                }

                if (!handled)
                {
                    Log("Unrecognized {0}: {1}", option ? "option" : "argument", arg);
                    PrintUsage();
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Override this in derived classes to handle command-line arguments one-by-one.
        /// </summary>
        /// <param name="arg">current argument</param>
        /// <param name="option">if there was a leading "-" or "/" to arg</param>
        /// <param name="args">the array of command line arguments</param>
        /// <param name="k">current index in the argument array.  passed by ref so you can increase it to "consume" arguments to options.</param>
        /// <returns>True if handled</returns>
        protected virtual bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            if (option)
            {
                switch (arg)
                {
                    case "?":
                    case "help":
                        PrintUsage();
                        Environment.Exit(0);
                        break;

                    case "k":
                    case "hold":
                        _keepAlive = true;
                        break;

                    case "verbose":
                        _verbose = true;
                        OutputOnlyOnFail = false;
                        break;

                    case "catchexceptions":
                        CatchExceptions = true;
                        break;

                    case "suite":
                        SelectSuite(args[++k]);
                        break;

                    case "trace":
                        EnableTracing();
                        break;

                    case "info":
                        _reportInfo = true;
                        break;

                    case "wait":
                        Log("Attach debugger now.  Press return to continue.");
                        Console.ReadLine();
                        break;

                    case "quietasserts":
                        _loudAsserts = false;
                        break;

                    case "loudasserts":
                        _loudAsserts = true;
                        break;

                    default:
                        return false;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Print a description of command line arguments.  Derived classes should
        /// override this to describe their own arguments, and then call
        /// base.PrintOptions() to get the DrtBase description.
        /// </summary>
        protected virtual void PrintOptions()
        {
            Log("General options:");
            Log("  -k or -hold       Keep window alive after tests are done");
            Log("  -verbose          produce verbose output");
            Log("  -suite name       run the suite with the given name");
            Log("  -suite nnn        run the suite with the given index");
            Log("  -trace            enable tracing");
            Log("  -wait             delay to allow debugger attach before start tests");
            Log("  -catchexceptions  catch exceptions (don't show JIT dialog)");
            Log("  -quietasserts     make asserts fail with a Console.WriteLine and continue");
            Log("  -loudasserts      make asserts fail loudly (with assert message box)");
        }

        /// <summary>
        /// The error code returned by the process when it terminates
        /// </summary>
        public int ReturnCode
        {
            get { return _retcode; }
            set { _retcode = value; }
        }

        /// <summary>
        /// The master list of test suites.  Derived classes should set this
        /// from the constructor.
        /// </summary>
        protected DrtTestSuite[] Suites
        {
            set { _suite = value; }
        }

        /// <summary>
        /// Add a suite to the 'selected' list.
        /// </summary>
        /// <param name="name">Name of the suite</param>
        protected void SelectSuite(string name)
        {
            int index = IndexFromString(name);

            if (index == -1)
                _selectedSuites.Add(name);
            else
                _selectedSuites.Add(index);
        }

        /// <summary>
        /// Add a suite to the 'selected' list.
        /// </summary>
        /// <param name="index">Index of the suite</param>
        protected void SelectSuite(int index)
        {
            _selectedSuites.Add(index);
        }

        /// <summary>
        /// Add a suite to the 'disabled' list.
        /// </summary>
        /// <param name="name">Name of the suite.</param>
        protected void DisableSuite(string name)
        {
            int index = IndexFromString(name);

            if (index == -1)
                _disabledSuites.Add(name);
            else
                _disabledSuites.Add(index);
        }

        /// <summary>
        /// Add a suite to the 'disabled' list.
        /// </summary>
        /// <param name="index">Index of the suite.</param>
        protected void DisableSuite(int index)
        {
            _disabledSuites.Add(index);
        }


        #region DRT settings (set these before DRT.Run)

        /// <summary>
        /// Produce verbose output.
        /// </summary>
        public bool Verbose
        {
            get { return _verbose; }
            set { _verbose = value; }
        }

        public int AssertsInSuite { get { return _cAssertsInSuite; } }

        /// <summary>
        /// Keep window alive after tests complete.
        /// </summary>
        public bool KeepAlive
        {
            get { return _keepAlive; }
            set { _keepAlive = value; }
        }

        /// <summary>
        /// Block input (important for DRTs which need to send input).  Prevent user from sending any input to the system.  (Press CTRL-ALT-DEL to unblock if you need to).
        /// </summary>
        public bool BlockInput
        {
            get { return _blockInput; }
            set
            {
                if (_blockInput != value)
                {
                    _blockInput = value;
                    if (_blockInput)
                    {
                        // Only block if the drt has started.  Otherwise we'll block at the beginning of Run.
                        if (s_drtStarted)
                        {
                            Win32BlockInput(true);
                        }
                    }
                    else
                    {
                        Win32BlockInput(false);
                    }
                }
            }
        }

        /// <summary>
        /// True if this DRT wants asserts dialog boxes to be raised on error
        /// (false will display all failures that happen in the end of the test).
        /// </summary>
        protected bool LoudAsserts { get { return _loudAsserts; } set { _loudAsserts = value; } }

        /// <summary>
        /// True if this DRT wants exceptions caught and logged to the console instead of allowing debugger attach.
        /// </summary>
        protected bool CatchExceptions { get { return _catchExceptions; } set { _catchExceptions = value; } }

        /// <summary>
        /// True if Console.Out should be redirected to a StringWriter.
        /// </summary>
        protected bool OutputOnlyOnFail
        {
            get { return _outputOnlyOnFail; }
            set
            {
                _outputOnlyOnFail = value;
                if (_outputOnlyOnFail)
                {
                    if (s_drtStarted)
                    {
                        Console.SetOut(_delayedConsoleOutput);
                    }
                }
                else
                {
                    Console.SetOut(_standardConsoleOutput);
                }
            }
        }

        /// <summary>
        /// If true, DRT.Assert will throw exceptions instead of calling Debug.Assert
        /// </summary>
        protected bool AssertsAsExceptions
        {
            get { return _assertsAsExceptions; }
            set { _assertsAsExceptions = value; }
        }

        /// <summary>
        /// Set the window title.  Only works if called before DRT.Run().
        /// </summary>
        protected string WindowTitle
        {
            get { return _windowTitle; }
            set { _windowTitle = value; }
        }

        /// <summary>
        /// Set the window size.  Only works if called before DRT.Run();
        /// </summary>
        protected Size WindowSize
        {
            get { return _windowSize; }
            set { _windowSize = value; }
        }

        /// <summary>
        ///     Set the window position.  Only works if called before DRT.Run();
        /// </summary>
        protected Point WindowPosition
        {
            get { return _windowPosition; }
            set { _windowPosition = value; }
        }

        /// <summary>
        /// Set whether or not this window should be topmost.  Must be set before the window is created.
        /// </summary>
        /// <value></value>
        protected bool TopMost
        {
            get { return s_topMost; }
            set { s_topMost = value; }
        }

        /// <summary>
        /// The team that owns the entire DRT
        /// </summary>
        public string TeamContact { get { return _teamContact; } set { _teamContact = value; } }

        /// <summary>
        /// The individual that owns the entire DRT
        /// </summary>
        public string Contact { get { return _contact; } set { _contact = value; } }

        #endregion 

        #region Public DRT properties

        /// <summary>
        /// The root element of the main window.
        /// </summary>
        public Visual RootElement
        {
            get { return s_rootElement; }
            set { s_rootElement = value; }
        }

        /// <summary>
        /// The DRT main window as a AutomationElement.
        /// </summary>
        public AutomationElement LogicalRoot
        {
            get
            {
                if (s_logicalRoot == null)
                {
                    s_logicalRoot = AutomationElement.FromHandle(MainWindow.Handle);
                }

                return s_logicalRoot;
            }
        }

        /// <summary>
        /// Return the real Console.Out (useful if using OutputOnlyOnFail, where Console.Out points to a string builder).
        /// </summary>
        public TextWriter ConsoleOut { get { return _standardConsoleOutput; } }

        /// <summary>
        /// The base directory for relative file names.  Should end with '\'.
        /// Defaults to '.\' (i.e. current directory).
        /// </summary>
        public string BaseDirectory
        {
            get { return _baseDirectory; }
            set { _baseDirectory = value; }
        }

        #endregion

        #region Suite Helpers

        /// <summary>
        /// Load the given .xaml file into the main window.
        /// </summary> 
        /// <param name="filename">name of the file, relative to the BaseDirectory</param>
        public void LoadXamlFile(string filename)
        {
            string fullname = BaseDirectory + filename;
            System.IO.Stream stream = File.OpenRead(fullname);

            RootElement = (Visual)XamlReader.Load(stream);
        }

        /// <summary>
        /// Repeat the current test, instead of moving to the next one.
        /// </summary>
        public void RepeatTest()
        {
            _fRepeatTest = true;
        }

        /// <summary>
        /// After the current test completes, pause for the given time (in
        /// milliseconds) before starting the next test.
        /// </summary>
        /// <param name="pause">Number of milliseconds to pause.</param>
        public void Pause(int pause)
        {
            _pause = pause;
        }

        /// <summary>
        /// This will suspend the tests until the Resume method is called.
        /// </summary>
        public void Suspend()
        {
            this.Suspend(1);
        }

        public void Suspend(int n)
        {
            _suspend += n;
        }

        /// <summary>
        /// Starting executing tests again after a Suspend
        /// </summary>
        public void Resume()
        {
            if (--_suspend == 0)
                s_dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(RunNextTestOperation), null);
        }

        /// <summary>
        /// After the current test completes, resume at the given test.
        /// </summary>
        /// <param name="test"></param>
        public void ResumeAt(DrtTest test)
        {
            _resumeStack.Push(test);
        }

        /// <summary>
        /// Start tests at the given frequency (in milliseconds).
        /// </summary>
        public int TestFrequency
        {
            set
            {
                _frequency = value;
                if (_frequency == 0)
                {
                    _timer = null;
                }

                if (_timer != null)
                {
                    _timer.Interval = TimeSpan.FromMilliseconds(_frequency);
                }
            }
            get
            {
                return _frequency;
            }
        }

        /// <summary>
        /// Assert that condition is true.
        /// </summary>
        /// <param name="cond">condition to test</param>
        public void Assert(bool cond)
        {
            this.Assert(cond, String.Empty, null, null);
        }

        /// <summary>
        /// Assert that condition is true.
        /// </summary>
        /// <param name="cond">condition to test</param>
        /// <param name="message">message to display if assert fails</param>
        /// <param name="arg">args for format tags in message</param>
        public void Assert(bool cond, string message, params object[] arg)
        {
            _cAssertsInSuite++;
            if (!cond)
            {
                string s = String.Format(message, arg);

                _retcode = 1;
                Log(" ASSERT failed: " + s);
                ReportDrtInfo();
                _totalFailures++;

                // Write any delayed output for debugging purposes
                WriteDelayedOutput();
                if (_loudAsserts)
                {
                    // throwing up an assert -- unblock just in case
                    BlockInput = false;
                    if (!_assertsAsExceptions)
                    {
                        System.Diagnostics.Debug.Assert(cond, s);
                    }
                    else
                    {
                        throw new Exception("Assert failed: " + s);
                    }
                }
            }
        }

        /// <summary>
        /// Assert that objects are equal. The phrase "Expected: x  Got: y" is automatically added to the message.
        /// </summary>
        /// <param name="expected">expected value</param>
        /// <param name="actual">actual value</param>
        /// <param name="message">message to display if assert fails</param>
        /// <param name="arg">args for format tags in message</param>
        public void AssertEqual(object expected, object actual, string message, params object[] arg)
        {
            if (!Object.Equals(expected, actual))
            {
                if (expected == null) expected = "NULL";

                if (actual == null) actual = "NULL";

                message += String.Format(" Expected: {0}  Got: {1}", expected, actual);
                this.Assert(false, message, arg);
            }
        }

        #endregion

        #region Uncommon functions

        /// <summary>
        /// Name of the current thread.
        /// </summary>
        public static string ThreadName
        {
            get
            {
                string s = Thread.CurrentThread.Name;

                if (s == null)
                    s = "<No Name>";

                return s;
            }
        }

        /// <summary>
        /// Write all output buffered to the StringBuilder (as a result of OutputOnlyOnFail)
        /// and then clear the buffer.
        /// </summary>
        public void WriteDelayedOutput()
        {
            if (_outputOnlyOnFail)
            {
                ConsoleOut.WriteLine(_consoleOutputStringBuilder.ToString());
                _consoleOutputStringBuilder.Remove(0, _consoleOutputStringBuilder.Length);
            }
        }

        /// <summary>
        /// Apply the given function to each element of the given list, letting
        /// Avalon settle in between.  (LISP users will recognize MAPCAR)
        /// </summary>
        /// <param name="function"></param>
        /// <param name="list"></param>
        public void ApplyFunctionToList(DispatcherOperationCallback function, IEnumerable list)
        {
            new MapCar(function, list, this);
        }

        /// <summary>
        /// Call each function in the given list, letting Avalon settle in between.
        /// </summary>
        /// <param name="list"></param>
        public void CallFunctions(IEnumerable list)
        {
            new MapList(list, this);
        }

        /// <summary>
        ///     Wait for the UCE thread to complete rendering.  This is the MIL-blessed way
        ///     to wait for rendering to complete.
        /// </summary>
        public void WaitForCompleteRender()
        {
            Type compositionTargetType = typeof(System.Windows.Media.CompositionTarget );
            Assembly mcasm = Assembly.GetAssembly(compositionTargetType);
            Type mcType = mcasm.GetType("System.Windows.Media.MediaContext");
            object mediaContext = mcType.InvokeMember("From", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { Dispatcher.CurrentDispatcher });

            mcType.InvokeMember("CompleteRender", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, mediaContext, new object[]{});
        }

        #endregion

        #region Tracing

        /// <summary>
        /// Enable trace output.
        /// </summary>
        public void EnableTracing()
        {
            ++_traceDepth;
        }

        /// <summary>
        /// Disable trace output.
        /// </summary>
        public void DisableTracing()
        {
            if (_traceDepth > 0)
                --_traceDepth;
        }

        /// <summary>
        /// Write trace message to console (if tracing is enabled).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arg"></param>
        public void Trace(string message, params object[] arg)
        {
            if (_traceDepth > 0)
            {
                Log("[trace] " + message, arg);
            }
        }

        /// <summary>
        /// Add the given assembly to the list of "used" assemblies.
        /// (These are reported after a failure).
        /// </summary>
        /// <param name="a"></param>
        public void UseAssembly(Assembly a)
        {
            _assemblies[a] = 0;
        }

        /// <summary>
        /// Add the assembly that declares the given type to the list of "used" assemblies.
        /// (These are reported after a failure).
        /// </summary>
        /// <param name="t"></param>
        public void UseType(Type t)
        {
            UseAssembly(t.Assembly);
        }

        #endregion

        #region Tree-walking helpers

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <returns></returns>
        public DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value, DependencyObject node, bool includeNode)
        {
            // see if the node itself has the right value
            if (includeNode)
            {
                object nodeValue = node.GetValue(dp);

                if (Object.Equals(value, nodeValue))
                    return node;
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for(int i = 0; i < count; i++)
            {
                DependencyObject result = FindVisualByPropertyValue(dp, value, VisualTreeHelper.GetChild(node, i), true);

                if (result != null)
                    return result;                
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <returns></returns>
        public DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value, DependencyObject node)
        {
            return FindVisualByPropertyValue(dp, value, node, true);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree (starting at the root)
        /// looking for a node with a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <returns></returns>
        /// <example>
        /// For example, to find the element with ID "foo", call
        ///  DRT.FindVisualByPropertyValue(IDProperty, "foo");
        /// </example>
        public DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value)
        {
            return FindVisualByPropertyValue(dp, value, RootElement);
        }

        /// <summary>
        /// Search the visual and logical trees looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <returns></returns>
        public DependencyObject FindElementByPropertyValue(DependencyProperty dp, object value, DependencyObject node, bool includeNode)
        {
            if (node == null)
                return null;

            // see if the node itself has the right value
            if (includeNode)
            {
                object nodeValue = node.GetValue(dp);
                if (Object.Equals(value, nodeValue))
                    return node;
            }

            DependencyObject result;
            DependencyObject child;

            // if not, recursively look at the logical children
            foreach (object currentChild in LogicalTreeHelper.GetChildren(node))
            {
                child = currentChild as DependencyObject;
                result = FindElementByPropertyValue(dp, value, child, true);
                if (result != null)
                    return result;
            }

            // then the visual children
            Visual vNode = node as Visual;
            if (vNode != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(vNode);
                for(int i = 0; i < count; i++)
                {
                    child = VisualTreeHelper.GetChild(vNode, i) as DependencyObject;
                    result = FindElementByPropertyValue(dp, value, child, true);
                    if (result != null)
                        return result;
                }                
            }

            // not found
            return null;
        }

        /// <summary>
        /// Search the visual and logical trees looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <returns></returns>
        public DependencyObject FindElementByPropertyValue(DependencyProperty dp, object value, DependencyObject node)
        {
            return FindElementByPropertyValue(dp, value, node, true);
        }

        /// <summary>
        /// Search the visual and logical trees (starting at the root)
        /// looking for a node with a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <returns></returns>
        /// <example>
        /// For example, to find the element with ID "foo", call
        ///  DRT.FindVisualByPropertyValue(IDProperty, "foo");
        /// </example>
        public DependencyObject FindElementByPropertyValue(DependencyProperty dp, object value)
        {
            return FindElementByPropertyValue(dp, value, RootElement);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        public DependencyObject FindVisualByType(Type type, DependencyObject node, bool includeNode)
        {
            // see if the node itself has the right type
            if (includeNode)
            {
                if (type == node.GetType())
                    return node;
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for(int i = 0; i < count; i++)
            {
                DependencyObject result = FindVisualByType(type, VisualTreeHelper.GetChild(node, i), true);

                if (result != null)
                    return result;
            }            

            // not found
            return null;
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        public DependencyObject FindVisualByType(Type type, DependencyObject node)
        {
            return FindVisualByType(type, node, true);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given ID.
        /// </summary>
        /// <param name="id">id of desired node</param>
        /// <param name="node">starting node for the search</param>
        public DependencyObject FindVisualByID(string id, DependencyObject node)
        {
            return FindVisualByPropertyValue(FrameworkElement.NameProperty, id, node);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given ID.
        /// </summary>
        /// <param name="id">id of desired node</param>
        public DependencyObject FindVisualByID(string id)
        {
            return FindVisualByID(id, RootElement);
        }

        /// <summary>
        /// Find the (automation) AutomationElement with a given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AutomationElement FindLogicalElementByID(string id)
        {
            PropertyCondition conds = new PropertyCondition(AutomationElement.NameProperty, id);

            return LogicalRoot.FindFirst(TreeScope.Element|TreeScope.Descendants, conds);
        }

        /// <summary>
        /// Walk up the visual tree looking for a node with a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <returns></returns>
        public Visual FindAncestorByType(Type type, Visual node, bool includeNode)
        {
            // see if the node itself has the right type
            if (includeNode)
            {
                if (type == node.GetType())
                    return node;
            }

            // if not, look at the ancestors
            for (node =(Visual) VisualTreeHelper.GetParent(node); node != null; node = (Visual) VisualTreeHelper.GetParent(node))
            {
                if (type == node.GetType())
                    return node;
            }

            // not found
            return null;
        }

        #endregion

        #region Input

        /// <summary>
        /// Move the mouse to the specified position within the given element.  x and y are
        /// coordinates within the element, (0,0) is upper left, (1,1) is lower right.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="x">horizontal position:  0.0 = left edge,  1.0 = right edge</param>
        /// <param name="y">vertical position:    0.0 = top edge,   1.0 = bottom edge</param>
        public void MoveMouse(AutomationElement e, double x, double y)
        {
            PrepareToSendInput();

            Rect rc = e.Current.BoundingRectangle;

            Input.MoveTo(new Point(rc.Left + x * rc.Width, rc.Top + y * rc.Height));
        }

        /// <summary>
        /// Move the mouse to the specified position within the given element.  x and y are
        /// coordinates within the element, (0,0) is upper left, (1,1) is lower right.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public bool MoveMouse(UIElement target, double x, double y)
        {
            PrepareToSendInput();

            // This code is paraphrased from Popup.cs.
            PresentationSource source = PresentationSource.FromVisual(target);

            if (source == null) return false;

            GeneralTransform transform;

            try
            {
                transform = target.TransformToAncestor(source.RootVisual);
            }
            catch (InvalidOperationException)
            {
                // if visuals are not connected...
                return false;
            }

            Point rootOffset;
            if (transform.TryTransform(new Point(0.0, 0.0), out rootOffset) == false)
            {
                return false;
            }
            CompositionTarget ct = source.CompositionTarget;

            rootOffset = ct.TransformToDevice.Transform(rootOffset);

            POINT topLeft = new POINT((int)rootOffset.X, (int)rootOffset.Y);

            ClientToScreen(((HwndSource)source).Handle, topLeft);

            Point size = ct.TransformToDevice.Transform(new Point(target.RenderSize.Width, target.RenderSize.Height));
            Point moveToPoint = new Point(topLeft.x + size.X * x, topLeft.y + size.Y * y);

            Input.MoveTo(moveToPoint);
            return true;
        }

        /// <summary>
        /// Click the mouse (at its current position).  Performs sanity checks before sending input.
        /// </summary>
        public void ClickMouse()
        {
            MouseButtonDown();
            MouseButtonUp();
        }

        /// <summary>
        /// Press primary mouse button (respects SwapButtons).  Performs sanity checks before sending input.
        /// </summary>
        public void MouseButtonDown()
        {
            PrepareToSendInput();
            Input.SendMouseInput(0, 0, 0, SystemParameters.SwapButtons ? SendMouseInputFlags.RightDown : SendMouseInputFlags.LeftDown);
        }

        /// <summary>
        /// Release primary mouse button (respects SwapButtons).  Performs sanity checks before sending input.
        /// </summary>
        public void MouseButtonUp()
        {
            PrepareToSendInput();
            Input.SendMouseInput(0, 0, 0, SystemParameters.SwapButtons ? SendMouseInputFlags.RightUp : SendMouseInputFlags.LeftUp);
        }

        /// <summary>
        /// Press second mouse button down -- respect SwapButtons.  Performs sanity checks before sending input.
        /// </summary>
        public void MouseSecondButtonDown()
        {
            PrepareToSendInput();
            Input.SendMouseInput(0, 0, 0, SystemParameters.SwapButtons ? SendMouseInputFlags.LeftDown : SendMouseInputFlags.RightDown);
        }

        /// <summary>
        /// Release second mouse button -- respect SwapButtons.  Performs sanity checks before sending input.
        /// </summary>
        public void MouseSecondButtonUp()
        {
            PrepareToSendInput();
            Input.SendMouseInput(0, 0, 0, SystemParameters.SwapButtons ? SendMouseInputFlags.LeftUp : SendMouseInputFlags.RightUp);
        }

        /// <summary>
        /// Press or release a key.  Performs sanity checks before sending input.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="press"></param>
        public void SendKeyboardInput(Key key, bool press)
        {
            PrepareToSendInput();
            Input.SendKeyboardInput(key, press);
        }

        #endregion

        /// <summary>
        /// The Dispatcher.
        /// </summary>
        protected static Dispatcher Dispatcher { get { return s_dispatcher; } }

        /// <summary>
        /// Get the main window.  First access creates the window.  Only works if called before DRT.Run().
        /// </summary>
        /// <value></value>
        protected HwndSource MainWindow
        {
            get
            {
                if (s_source == null)
                {
                    HwndSourceParameters param = new HwndSourceParameters(WindowTitle);
                    param.SetPosition((int)WindowPosition.X, (int)WindowPosition.Y);

                    if (WindowSize != Size.Empty)
                    {
                        param.SetSize((int)WindowSize.Width, (int)WindowSize.Height);
                    }
                    s_source = new HwndSource(param);

                    s_logicalRoot = null;
                    if (s_topMost) SetTopMost(s_source.Handle, true);
                }

                return s_source;
            }
            set { s_source = value; s_logicalRoot = null; }
        }

        public Application Application
        {
            get
            {
                if (s_application == null)
                {
                    s_application = new Application();
                    s_dispatcher = s_application.Dispatcher;

                    s_application.MainWindow = new NavigationWindow();
                    s_application.MainWindow.Title = "DrtFrameNavigation";

                    PropertyInfo info = s_application.MainWindow.GetType().GetProperty("HwndSourceWindow", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
                    object obj = info.GetValue(s_application.MainWindow, null);

                    this.MainWindow = (HwndSource)obj;
                    this.RootElement = s_application.MainWindow;
                    s_application.MainWindow.Show();
                }

                return s_application;
            }
            set
            {
                s_application = value;
            }
        }

        public void Dispose()
        {
            if (s_dispatcher != null)
            {
                if (s_source != null)
                {
                    s_source.Dispose();
                    s_source = null;
                }

                s_dispatcher.InvokeShutdown();
                s_dispatcher = null;
            }

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= new EventHandler(RunNextTestTimerTask);
                _timer = null;
            }

            s_logicalRoot = null;
        }

        #region Private Implementation

        void Log(string message)
        {
            _Logger.Log(message);
        }

        void Log(string message, params object[] args)
        {
            _Logger.Log(message, args);
        }

        //------------------------------------------------------
        //
        //  Private implementation
        //
        //------------------------------------------------------
        int IndexFromString(string s)
        {
            if (Char.IsDigit(s, 0))
            {
                try
                {
                    int index = Int32.Parse(s);

                    return index;
                }
                catch (FormatException)
                {
                }
            }

            return -1;
        }

        bool SuiteMatches(DrtTestSuite suite, object o)
        {
            if (o is int && _suiteIndex == (int)o)
                return true;

            if (o is string && suite.Name.ToLower() == ((string)o).ToLower())
                return true;

            return false;
        }

        bool IsSelected(DrtTestSuite suite)
        {
            int k;

            if (suite == null)
                return false;

            // if there are selected suites, see if our suite is one of them
            if (_selectedSuites.Count > 0)
            {
                for (k = _selectedSuites.Count - 1; k >= 0; --k)
                {
                    if (SuiteMatches(suite, _selectedSuites[k]))
                        return true;
                }

                return false;
            }

            // otherwise see if our suite is on ---- list
            for (k = _disabledSuites.Count - 1; k >= 0; --k)
            {
                if (SuiteMatches(suite, _disabledSuites[k]))
                    return false;
            }

            return true;
        }

        object RunNextSuite(object arg)
        {
            for (; _suiteIndex < _suite.Length; ++_suiteIndex)
            {
                DrtTestSuite suite = _suite[_suiteIndex];

                if (IsSelected(suite))
                {
                    if (_currentSuite != null)
                        _currentSuite.ReleaseResources();

                    ConsoleOut.WriteLine(" >Suite: " + suite.Name);
                    suite.DRT = this;
                    _currentSuite = suite;
                    _suiteInfoReported = false;
                    _test = suite.PrepareTests();
                    _testIndex = 0;
                    _cAssertsInSuite = 0;
                    ScheduleNextTest();
                    _suite[_suiteIndex] = null;     // release suite's memory
                    ++_suiteIndex;
                    return null;
                }
            }

            // all suites are done - close the app
            if (!_keepAlive)
            {
                if (_currentSuite != null)
                {
                    _currentSuite.ReleaseResources();
                    _currentSuite = null;
                }

                s_dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(Quit),
                    null);
            }
            else
            {
                // Unblock input so the user can interact with the DRT.
                Win32BlockInput(false);
            }

            return null;
        }

        void ScheduleNextTest()
        {
            if (_frequency == 0)
            {
                // treat frequency 0 as max speed; use background tasks for that
                s_dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(RunNextTestOperation),
                    null);
            }
            else
            {
                // use timer for frequencies other than 0
                if (_timer == null)
                {
                    _timer = new DispatcherTimer(DispatcherPriority.Normal, s_dispatcher);
                    _timer.Tick += new EventHandler(RunNextTestTimerTask);
                    _timer.Interval = TimeSpan.FromMilliseconds(_frequency);
                    _timer.Start();
                }
            }
        }

        object RunNextTestOperation(object arg)
        {
            RunNextTest();
            return null;
        }

        void RunNextTestTimerTask(object sender, EventArgs e)
        {
            RunNextTest();
        }

        void RunNextTest()
        {
            // honor a request to pause before starting the next test
            if (_pause > 0)
            {
                // queue a ContextIdle priority item and schedule it to ContextIdle priority in _pause milliseconds
                DispatcherTimer callbackTimer = new DispatcherTimer(DispatcherPriority.ContextIdle);
                callbackTimer.Interval = TimeSpan.FromMilliseconds(_pause);
                callbackTimer.Tick += 
                    delegate(object sender, EventArgs e) {
                        ((DispatcherTimer)sender).Stop();
                        RunNextTest();
                    };
                callbackTimer.Start();

                _pause = 0;
                return;
            }

            DrtTest test;

            if (_resumeStack.Count > 0)
            {
                test = (DrtTest)_resumeStack.Pop();
                ScheduleNextTest();
                Trace(String.Format("  continuing test {0} - {1}", _testIndex - 1, test.Method.Name));
                test();
                if (_resumeStack.Count == 0)
                    Trace(String.Format("Ending test {0}", _testIndex - 1));

                return;
            }

            test = (_testIndex < _test.Length) ? _test[_testIndex] : null;
            _fRepeatTest = false;
            if (test != null)
            {
                Trace(String.Format("Starting test {0} - {1}", _testIndex, test.Method.Name));

                // run the current test
                test();
                if (_resumeStack.Count == 0)
                    Trace(String.Format("Ending test {0}", _testIndex));

                if (!_fRepeatTest)
                    _testIndex++;

                // schedule the next test
                if (_suspend == 0)
                    ScheduleNextTest();
            }
            else
            {
                // the suite is finished
                RunNextSuite(null);
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            ReportException(args.ExceptionObject);
        }

        private void ReportException(object o)
        {
            Exception e = o as Exception;

            if (_retcode == 0)
                _retcode = 1;

            Log("");
            if (e != null)
            {
                Log("Unhandled exception on thread '{3}': {0}\n{1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace, ThreadName);
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    Log("Inner Exception was '{3}': {0}\n{1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace, ThreadName);
                }
            }
            else
            {
                Log("Unhandled exception on thread '{1}': {0}\n", o, ThreadName);
            }

            ReportDrtInfo();
            Environment.Exit(_retcode);
        }

        private void ReportDrtInfo()
        {
            if (!_drtInfoReported)
            {
                Log("");
                Log(">>> DRT Information:");
                Log("Contact: {0} or the {1} team", Contact, TeamContact);
                Log("Assemblies used (make sure these come from the expected location):");

                IDictionaryEnumerator ie = _assemblies.GetEnumerator();

                while (ie.MoveNext())
                {
                    Assembly a = ie.Key as Assembly;

                    Log("  {0}\n    from {1}", a.FullName, a.Location);
                }

                Log("<<< End of DRT Information");
                _drtInfoReported = true;
            }

            if (!_suiteInfoReported && _currentSuite != null)
            {
                Log("  >> Suite Information:");
                Log("  currently running suite '{0}'", _currentSuite.Name);
                Log("  Contact: {0} or the {1} team", (_currentSuite.Contact != null) ? _currentSuite.Contact : "(" + Contact + ")", (_currentSuite.TeamContact != null) ? _currentSuite.TeamContact : "(" + TeamContact + ")");
                Log("  << End of suite Information");
                _suiteInfoReported = true;
            }
        }

        private static IntPtr ApplicationFilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Quit the application if the source window is closed.
            if (msg == WM_CLOSE)
            {
                s_dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(Quit),
                    null);
                handled = true;
            }

            return IntPtr.Zero;
        }

        private static object Quit(object arg)
        {
            //_dispatcher.Quit();
            s_drt.Application.Shutdown();

            //_drt.Dispose();
            return null;
        }

        private void PrintUsage()
        {
            string[] args = Environment.GetCommandLineArgs();

            Log("Usage:  {0} [options]", args[0]);
            PrintOptions();

            string names = " ";

            foreach (DrtTestSuite suite in _suite)
            {
                if (suite != null)
                    names += " " + suite.Name;
            }

            Log("Suite names: {0}", names);
            foreach (DrtTestSuite suite in _suite)
            {
                if (suite != null)
                    suite.PrintOptions();
            }
        }

        //
        // Private nested classes
        //
        /// <summary>
        /// apply a function to each element of a list, allowing Avalon to settle in between
        /// </summary>
        class MapCar
        {
            public MapCar(DispatcherOperationCallback function, IEnumerable list, DrtBase drt)
            {
                _function = function;
                _ie = list.GetEnumerator();
                _drt = drt;
                _map = new DrtTest(Map);
                Test();
            }

            void Test()
            {
                if (_ie.MoveNext())
                    _drt.ResumeAt(_map);
            }

            void Map()
            {
                // call Test before applying the function - this resumes the mapcar
                // after the function is done, even if the function itself calls ResumeAt.
                object x = _ie.Current;

                Test();
                _function(x);
            }

            DispatcherOperationCallback _function;

            IEnumerator _ie;

            DrtBase _drt;

            DrtTest _map;
        }

        /// <summary>
        /// call each function on a list, allowing Avalon to settle in between
        /// </summary>
        class MapList
        {
            public MapList(IEnumerable list, DrtBase drt)
            {
                _ie = list.GetEnumerator();
                _drt = drt;
                _map = new DrtTest(Map);
                Test();
            }

            void Test()
            {
                if (_ie.MoveNext())
                    _drt.ResumeAt(_map);
            }

            void Map()
            {
                // call Test before applying the function - this resumes the mapcar
                // after the function is done, even if the function itself calls ResumeAt.
                DrtTest function = (DrtTest)_ie.Current;

                Test();
                function();
            }

            IEnumerator _ie;

            DrtBase _drt;

            DrtTest _map;
        }

        /// <summary>
        ///     Check if SendInput will stop the screensaver, and if it's not then make it so.
        /// </summary>
        private void MakeSendInputResetScreenSaver()
        {
            bool blockSendInputResets = false;

            // BLOCKSENDINPUTRESETS should be false -- "don't block sendinput resets".
            if (SystemParametersInfo(_SPI_GETBLOCKSENDINPUTRESETS, 0, ref blockSendInputResets, 0))
            {
                if (blockSendInputResets)
                {
                    blockSendInputResets = !blockSendInputResets;
                    Log("Warning: SendInput does not reset the screensaver; setting SPI_SETBLOCKSENDINPUTRESETS to {0}", blockSendInputResets);
                    if (!SystemParametersInfo(_SPI_SETBLOCKSENDINPUTRESETS, blockSendInputResets ? 1 : 0, ref blockSendInputResets, 1 /* update ini file */))
                    {
                        Log("Could not set SPI_SETBLOCKSENDINPUTRESETS to {0}", blockSendInputResets);
                    }
                }
            }
        }

        /// <summary>
        /// Does some preliminary checks to make sure it's okay to send input.  
        /// Checks to see if the screen saver is running or any power-save mode is active.
        /// Checks if the DRT window is hung.  Checks if the DRT window is foreground.
        /// </summary>
        public void PrepareToSendInput()
        {
            BlockInput = true;

            IntPtr hwnd = GetForegroundWindow();

            if (hwnd != s_source.Handle)
            {
                string error = String.Format("Warning: Foreground window {0:X} ({1}) did not match DRT window {2:X}", hwnd, GetWindowTitle(hwnd), s_source.Handle);

                if (_warningMismatchedForeground >= WarningLevel.Warning)
                {
                    Log(error);
                }

                if (_warningMismatchedForeground == WarningLevel.Error)
                {
                    throw new Exception(error);
                }
            }

            if (IsHungAppWindow(s_source.Handle))
            {
                string error = String.Format("Main window hung and has been ghosted. This is a bad time to be sending input!");

                Log(error);
                Log("Allowing dispatcher to pump input messages to un-ghost...");
                Dispatcher.Invoke(
                    DispatcherPriority.Input,
                    (DispatcherOperationCallback) delegate(object unused)
                    {
                        return null;
                    },
                    null);
                Log("done.");
            }

            if (_warningScreenSaving >= WarningLevel.Warning)
            {
                CheckScreenSaver(_SPI_GETSCREENSAVETIMEOUT, "Screen Saver");
                CheckScreenSaver(_SPI_GETLOWPOWERTIMEOUT, "Low Power Saving");
                CheckScreenSaver(_SPI_GETPOWEROFFTIMEOUT, "Power Off Saving");
            }
        }

        private void CheckScreenSaver(int spiGetTimeout, string mode)
        {
            int timeout = 0;

            if (SystemParametersInfo(spiGetTimeout, 0, ref timeout, 0))
            {
                if (timeout > 0)
                {
                    // timeout is given to us in seconds, not milliseconds
                    timeout *= 1000;

                    int idleTime = GetIdleTime();

                    //Log("{0} -- idleTime is {1} and timeout is {2}", mode, idleTime, timeout);
                    if (idleTime >= 0)
                    {
                        if (timeout > idleTime)
                        {
                            //Log("Warning: {0} has non-zero timeout. Timeout will expire in {1} s", mode, (timeout - idleTime) / 1000.0);
                        }
                        else
                        {
                            string error = String.Format(_warningScreenSaving.ToString() + ": {0} timeout expired {1} s ago", mode, (idleTime - timeout) / 1000.0);

                            if (_warningScreenSaving >= WarningLevel.Warning)
                            {
                                Log(error);
                            }

                            if (_warningScreenSaving == WarningLevel.Error)
                            {
                                throw new Exception(error);
                            }
                        }
                    }
                }
            }
        }

        #endregion 

        #region Win32 interop helpers

        public static bool SetTopMost(IntPtr WindowHandle, bool topmost)
        {
            return SetWindowPos(WindowHandle, topmost ? s_HWND_TOPMOST : s_HWND_NOTOPMOST, 0, 0, 0, 0, SWP_DRAWFRAME | SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        /// <summary>
        ///     Returns diff b/c GetTickCount and GetLastInputInfo in ms
        /// </summary>
        private int GetIdleTime()
        {
            int currTime = GetTickCount();
            LASTINPUTINFO lii = new LASTINPUTINFO();

            lii.cbSize = Marshal.SizeOf(lii);
            if (GetLastInputInfo(ref lii))
            {
                return (currTime - lii.dwTime);
            }
            else
            {
                return -1;
            }
        }

        internal struct LASTINPUTINFO
        {
            public int cbSize;

            public int dwTime;
        }

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        internal static extern int GetTickCount();

        [DllImport("user32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        internal static extern bool GetLastInputInfo(ref LASTINPUTINFO lii);

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x = 0;

            public int y = 0;

            public POINT()
            {
            }

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern int ClientToScreen(IntPtr hWnd, [In, Out] POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SystemParametersInfo(int nAction, int nParam, ref bool value, int ignore);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SystemParametersInfo(int nAction, int nParam, ref int value, int ignore);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount);

        public static string GetWindowTitle(IntPtr hwnd)
        {
            StringBuilder sb = new StringBuilder(500);

            GetWindowText(new HandleRef(null, hwnd), sb, 500);
            return sb.ToString();
        }

        int _SPI_GETBLOCKSENDINPUTRESETS = 0x1026;

        int _SPI_SETBLOCKSENDINPUTRESETS = 0x1027;

        int _SPI_GETSCREENSAVETIMEOUT = 0x000E;

        int _SPI_GETLOWPOWERTIMEOUT = 0x004F;

        int _SPI_GETPOWEROFFTIMEOUT = 0x0050;

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "BlockInput")]
        private static extern int Win32BlockInput(int fBlockIt);

        /// <summary>
        /// Prevent user from sending any input to the system.  (Press CTRL-ALT-DEL to unblock if you need to).
        /// </summary>
        private static void Win32BlockInput(bool blockIt)
        {
            Win32BlockInput(blockIt ? 1 : 0);
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        private static IntPtr s_HWND_TOPMOST = new IntPtr(-1);

        private static IntPtr s_HWND_NOTOPMOST = new IntPtr(-2);

        private const int SWP_NOMOVE = 0x0001;

        private const int SWP_NOSIZE = 0x0002;

        private const int SWP_SHOWWINDOW = 0x0040;

        private const int SWP_DRAWFRAME = 0x0020;

        [DllImport("user32.dll")]
        private static extern bool IsHungAppWindow(IntPtr hWnd);

            #endregion

        #region Private Fields
        //
        //  Private fields
        //
        private static Application s_application;

        private static Dispatcher s_dispatcher;

        private static HwndSource s_source;

        private static Visual s_rootElement;

        private static bool s_topMost = false;

        private static AutomationElement s_logicalRoot;

        private static DrtBase s_drt;

        private const int WM_CLOSE = 0x0010;

        private static bool s_drtStarted;

        private string _windowTitle = "DRT";

        private Size _windowSize = new Size(800, 600);

        private Point _windowPosition = new Point(50, 50);

        private string _baseDirectory = @".\";

        private int _retcode;

        private int _traceDepth;

        private bool _keepAlive;

        private bool _blockInput;

        private bool _verbose;

        private bool _catchExceptions = false;

        private bool _reportInfo;

        private DrtTestSuite[] _suite = new DrtTestSuite[0];

        private DrtTest[] _test = new DrtTest[0];

        private Stack _resumeStack = new Stack();

        private string _teamContact = "(unknown)";

        private string _contact = "(unknown)";

        private bool _drtInfoReported;

        private bool _suiteInfoReported;

        private DispatcherTimer _timer;

        private int _frequency = 0;

        private int _suiteIndex, _testIndex = 0;

        private DrtTestSuite _currentSuite;

        private int _pause;

        private int _suspend = 0;

        private ArrayList _selectedSuites = new ArrayList();

        private ArrayList _disabledSuites = new ArrayList();

        private Hashtable _assemblies = new Hashtable();

        private bool _fRepeatTest = false;

        private int _cAssertsInSuite;

        private bool _loudAsserts = true;

        private int _totalFailures = 0;

        private TextWriter _standardConsoleOutput = Console.Out;

        private StringBuilder _consoleOutputStringBuilder = new StringBuilder();

        private StringWriter _delayedConsoleOutput;

        private bool _outputOnlyOnFail = true;

        private bool _assertsAsExceptions = true;

        private WarningLevel _warningMismatchedForeground = WarningLevel.Warning;

        private WarningLevel _warningScreenSaving = WarningLevel.Warning;

        #endregion

        internal static DRT.Logger _Logger = new DRT.Logger("DrtFrameJournaling", "Microsoft", "Testing frame journaling");
    }

    public enum WarningLevel { Ignore, Warning, Error };

    #region DrtTestSuite class

    // A "suite" of tests, typically operating on a single tree.
    public class DrtTestSuite
    {
        protected DrtTestSuite(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Return the list of individual tests (i.e. callback methods).
        /// Derived classes should override this.
        /// A null entry in the list serves as a list terminator (as does the
        /// end of the list).
        /// </summary>
        /// <returns></returns>
        public virtual DrtTest[] PrepareTests()
        {
            return new DrtTest[0];
        }

        /// <summary>
        /// Called when the suite is completed.  Suite should release memory
        /// and other resources
        /// </summary>
        public virtual void ReleaseResources() { }

        /// <summary>
        /// The name of the suite.
        /// </summary>
        public string Name { get { return _name; } }

        string _name;

        /// <summary>
        /// The team that owns the suite.
        /// </summary>
        public string TeamContact { get { return _teamContact; } set { _teamContact = value; } }

        string _teamContact;

        /// <summary>
        /// The individual that owns the suite.
        /// </summary>
        public string Contact { get { return _contact; } set { _contact = value; } }

        string _contact;

        /// <summary>
        /// The DrtBase that is running this suite.
        /// </summary>
        public DrtBase DRT { get { return _drt; } set { _drt = value; } }

        DrtBase _drt;

        /// <summary>
        /// Override this to handle per-suite command line arguments
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="option"></param>
        /// <param name="args"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            return false;
        }

        /// <summary>
        /// Override this to print help for per-suite options.
        /// </summary>
        public virtual void PrintOptions()
        {
        }
    }

    #endregion 

    // the delegate to use for an individual "test"
    public delegate void DrtTest();
}

