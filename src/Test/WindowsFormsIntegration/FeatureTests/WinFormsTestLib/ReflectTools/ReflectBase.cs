// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WFCTestLib.Log;
using WFCTestLib.Util;
using System.Security.Principal;
using System.Security.Policy;

namespace ReflectTools
{
    // <doc>
    // <desc>
    //  Provides engine for calling all methods that return a ScenarioResult object.
    //  Also provides logging to "results.log", where results.log is in XML format.
    //
    //  Granted all permissions for running ReflectBase tests in a secure context.  This
    //  should let ReflectBase do everything it needs to while not granting tests any
    //  extra permissions.
    // </desc>
    // </doc>
    [System.ComponentModel.DesignerCategory("Code")]
    public class ReflectBase : Form, IMessageFilter
    {
        // <doc>
        // <desc>
        //  This will allow us to exclude scenarios that we don't want to be executed.
        // </desc>
        // </doc>
        public static StringTable excludedMethods = new StringTable();

        // <doc>
        // <desc>
        //  The name of the testcase we are executing
        // </desc>
        // </doc>
        protected static string testName = "defaultTestName";

        // <summary>
        //  The optional file you can use to specify command-line parameters.  Each
        //  parameter should be on its own line
        // </summary>
        private const string OptionsFilename = "options.rb";

        // <summary>
        //  Semaphore file to notify the test driver the test has completed.
        // </summary>
        private const string SemaphoreFilename = "exe.don";

        // <summary>
        //  The name of the log file.
        // </summary>
        private const string LogFilename = "results.log";

        // <summary>
        //  At the end of the test, we translate our log into the MadDog standard MDLog
        //  format and use this filename for the output.
        // </summary>
        private const string MDLogFilename = "results.xml";

        // <summary>
        //  Results file used by MadDog drivers to determine pass/fail count.
        // </summary>
        private const string LogTxtFilename = "results.txt";


		//If this is set to true and BeginSecurityCheck is called again, then fail the scenario
		//It is an error in testcase code if BeginSecurityCheck is called more than once in the same method
		private bool _beginSecurityCheckCalledForCurrentScenario = false;

        // <doc>
        // <desc>
        //  The group of parameters we pass around the engine and to each scenario
        // </desc>
        // </doc>
        //
        // Note: this is static so that you can access the log from a place outside your scenario code, like
        // in helper functions.
        //
        protected static TParams scenarioParams;
		public static TParams ScenarioParams
		{ get { return scenarioParams; } }
		

        // <doc>
        // <desc>
        //  The logging object for this testcase
        // </desc>
        // </doc>
        //
        // Note: this is static so that you can access the log from a place outside your scenario code, like
        // in helper functions.
        //
        protected static WFCTestLib.Log.Log log = null;
		public static WFCTestLib.Log.Log Log  { get { return log; } }

        // <doc>
        // <desc>
        //  A List of ScenarioGroups that represents the Scenarios we
        //  are going to execute.
        // </desc>
        // </doc>
        protected static ArrayList tests;

        // <doc>
        // <desc>
        //  A semaphore used to determine if the engine should stop calling
        //  test scenarios. Currently set to true if the form is closing or
        //  if all tests have completed.
        // <desc>
        // <seealso member="TestIsDone"/>
        // <seealso member="OnClosed"/>
        // </doc>
        private bool _stopTests = false;

        // <doc>
        // <desc>
        //  The number of milliseconds to sleep between scenarios. Do NOT rely on this
        //  value for timing purposes and your scenario passing. Any timing required
        //  should be contained in the individual scenarios.
        // </desc>
        // <seealso member="Sleep"/>
        // </doc>
        private int _sleep = 0;

        // <doc>
        // <desc>
        //  True if the random number generator has been seeded from the
        //  command line; false otherwise;
        // </desc>
        // </doc>
        private bool _seeded = false;

        // <doc>
        // <desc>
        //  True if the random number generator has been seeded from the
        //  command line; false otherwise;
        // </desc>
        // </doc>
        private int _seed = -1;

        // <doc>
        // <desc>
        //  If true, a MessageBox is displayed before proceeding to
        //  the next scenario.
        // </desc>
        // </doc>
        private bool _pauseBetweenScenarios = false;

        // <doc>
        // <desc>
        // If true, continues running next scenario groups even if the current one has
        // a failure.
        // </desc>
        // </doc>
        private bool _stopOnFailureInGroup = false;

        // <doc>
        // <desc>
        //  If true, a MessageBox is displayed when a failure occurs.
        // </desc>
        // </doc>
        private bool _pauseOnFailure = false;

        // <doc>
        // <desc>
        // If true, sets KeepRunningTests to false when a failure occurs.
        // </desc>
        // </doc>
        private bool _stopOnFailureInScenario = false;

        // <doc>
        // <desc>
        // If true, logs a method using its fully-qualified name, rather than just the
        // method name.
        // </desc>
        // </doc>
        private bool _fullNames = false;

        // <doc>
        // <desc>
        // If false, ReflectBase stops running tests and leaves the form running so the
        // test writer can debug the testcase.
        // </desc>
        // </doc>
        private bool _keepRunningTests = true;

        // <doc>
        // <desc>
        // If false, ReflectBase doesn't pay attention to PermissionRequired attributes on
        // scenarios.  If true, ReflectBase will perform security testing on scenarios
        // marked with PermissionRequired.
        //
        // If the required permission is granted, ReflectBase will treat the test as it
        // would normally.
        //
        // If the required permission is NOT granted, ReflectBase will fail the test if it
        // passes, or pass if it throws a SecurityException.
        // </desc>
        // </doc>
        private bool _testSecurity = true;

        // <doc>
        // <desc>
        // If true, ReflectBase filters all messages and prints debug information to the log
        // if it is a WM_KEYDOWN.  Default is false.  If we ever change default to true,
        // we need code in InitTest to hook up the message filter, since this currently
        // only happens when you set the property to true.
        // </desc>
        // </doc>
        private bool _logKeystrokes = false;

        // <doc>
        // <desc>
        //  If true, begins testcase execution in the constructor, rather than after
        //  handle creation.  This is to cover the scenario where users set properties before
        //  the form's handle is created, which has led to a number of bugs.
        // </desc>
        // </doc>
        private bool _preHandleMode = false;

        /// <summary>
        /// If true, test is in manual mode.  Calls to ManualFreeze() are enabled, and ManualMode
        /// property is true.  This allows tests to behave differently if run in manual mode,
        /// e.g. ManualFreeze() when visual verification required, etc.
        /// </summary>
        private bool _manualMode = false;

		/// <summary>
		/// If true, Application.EnableVisualStyles() is called in the constructor, thus turning on
		/// Windows XP themes.
		/// </summary>
		private bool _visualStylesEnabled = true;


        /// <summary>
        /// Default is false.  If true, we will call all ScenarioResults on a separate thread.
        /// AsyncScenario is the delegate method signature that will be called on a separate thread.
        /// </summary>
        /// <value></value>
        private delegate ScenarioResult AsyncScenario(MethodInfo mi, object[] parameters);
        private bool _useMita = false;
        public bool UseMita
        {
            get { return _useMita; }
            set { _useMita = value; }
        }

        // <doc>
        // <desc>
        // A list of permissions required by the current scenario.  A scenario which requires
        // certain permissions should add them to this list.  ReflectBase will automatically clear
        // the list between scenarios.
        // </desc>
        // </doc>
        private PermissionCollection _requiredPermissions = new PermissionCollection();

        // <doc>
        // <desc>
        // Method expected to be in the stack trace of a security failure
        // </desc>
        // </doc>
		protected MethodInfo securityCheckExpectedMethod;

		//
		// If true, checks to make sure the currently tested method appears on
		// the stack of any SecurityExceptions thrown (when running in semi-trust).
		//
		private bool _stackCheck = false;

		// If true, the scenario picker dialog will be displayed
		private bool _useScenarioPicker = false;

		// If true, checks to make sure the currently tested method appears on
		// the stack of any SecurityExceptions thrown (when running in semi-trust).
		protected bool StackCheck
		{
			get { return _stackCheck; }
			set { _stackCheck = value; }
		}

        // <doc>
        // <desc>
        //  The parameters provided on the command line for the test
        // </desc>
        // <seealso member="CommandLineParameters"/>
        // </doc>
        private ArrayList _commandLineParameters;

        // <doc>
        // <desc>
        //  Indicates whether a WFC assertion or thread exception occurred.
        // </desc>
        // <seealso member="SetAssertOrExceptionFailure"/>
        // </doc>
        private bool _assertOrExceptionFailure = false;

        // <doc>
        // <desc>
        //  A short description of the assertion or thread exception that occurred.
        // </desc>
        // <seealso member="SetAssertOrExceptionFailure"/>
        // </doc>
        private string _shortErrorMsg = "";

        // <doc>
        // <desc>
        //  A detailed description of the assertion or thread exception that occurred.
        // </desc>
        // <seealso member="SetAssertOrExceptionFailure"/>
        // </doc>
        private string _detailErrorMsg = "";

        //
        // If this value is set, ReflectBase will use it as the ScenarioResult for the
        // current scenario, rather than the result returned by the scenario, or the
        // result generated by a thread exception, etc.
        //
        private ScenarioResult _overriddenScenarioResult = null;

        // <doc>
        // <desc>
        //  Counts the number of recursive calls to CreateInstance() that have
        //  occurred.  If this goes over a certain number, we barf and print an nice
        //  error message.
        // </desc>
        // <seealso member="CreateInstance"/>
        // </doc>
        private int _numIterations = 0;

        // <doc>
        // <desc>
        //  Keeps track of whether ReflectBase's InitTest was called.  It is essential that
        //  subclasses call base.InitTest(), so if they do not, we'll immediately fail the
        //  test with an error message.
        // </desc>
        // <seealso member="InitTest"/>
        // </doc>
        private bool _baseInitTestCalled = false;

		/// <summary>
		/// Same as baseInitTestCalled--if ReflectBase's BeforeScenario() is not called,
		/// we'll fail the scenario.
		/// </summary>
		private bool _baseBeforeScenarioCalled = false;

		/// <summary>
		/// Same as baseInitTestCalled--if ReflectBase's AfterScenario() is not called,
		/// we'll fail the scenario.
		/// </summary>
		private bool _baseAfterScenarioCalled = false;

		//
        // Once the test has started running, this is set to true to prevent form handle
        // recreation from restarting the test.
        //
        private bool _testStarted = false;

		// The scenario we are currently executing, or null if we're between scenarios.
		private MethodInfo _currentScenario;
		protected MethodInfo currentScenario
		{
			get { return _currentScenario; }
			private set { _currentScenario = value; }
		}

        private static string s_initialDirectory;
        // <doc>
        // <desc>
        //  Static constructor to set the apartment state to STA.  Without this being called
        //  before the first PInvoke, an exception will be thrown.
        // </desc>
        // </doc>
        static ReflectBase()
        {
			//Disable the compiler warning on this, this propset is just a backstop against a possible missing 
			//[STAThread on Main]
#pragma warning disable 618
            Thread.CurrentThread.ApartmentState = ApartmentState.STA;
#pragma warning restore 618
            s_initialDirectory = SafeMethods.GetCurrentDirectory();
        }

        private string _workingDir;
        // <doc>
        // <desc>
        //  Constructs a ReflectBase object. If there isn't a log object one is
        //  constructed. Also creates a TParams object to pass around the engine
        //  and to test cases.
        // </desc>
        // </doc>
        protected ReflectBase(String[] args) : base()
        {
            // process command line parameters
            if ( args == null )
                args = new String[0];

            // If the ReflectBaseWorkingDir env var is set, we'll use that path
            // for the working directory.
            LibSecurity.Environment.Assert();
            _workingDir = Environment.GetEnvironmentVariable("ReflectBaseWorkingDir");
            CodeAccessPermission.RevertAssert();

            if ( _workingDir != null ) {

                try {
                    SafeMethods.SetCurrentDirectory(_workingDir);
                    s_initialDirectory = _workingDir;
                }
                catch (Exception e) {
                    // Ignore exception if we get one because of invalid path, etc.
                    Console.WriteLine("Exception setting working directory.  workingDir was \"{0}\"", _workingDir);
                    Console.WriteLine(e.ToString());
                }
            }

            this._commandLineParameters = new ArrayList((IList)args);
            CheckOptionsFile();
            CheckEnvironmentOptions();
            ProcessCommandLineParameters();
            SetOptions();

			// Turn on visual styles
			if ( VisualStylesEnabled )
				Application.EnableVisualStyles();

            // Create log file
            if (log == null)
            {
                LibSecurity.UnrestrictedFileIO.Assert();
                log = new WFCTestLib.Log.Log(LogFilename);
                CodeAccessPermission.RevertAssert();
            }

            scenarioParams = new TParams(log);
            testName = this.GetType().Name;

            // start the test/log file
            scenarioParams.log.StartTest(testName);
            LogCommandLineParameters();

            if ( _workingDir != null )
                LogCommandLineParameter("workingdir", _workingDir);
            
            // If RandomUtil hasn't been seeded yet, do so now.
            if (!_seeded)
            {
                if ( Seed == -1 ) {
                    Seed = scenarioParams.ru.GetInt();
                    LogCommandLineParameter("autoseed", Seed.ToString());
                }

                _seeded = true;
                scenarioParams.ru.SeedRandomGenerator(_seed);
            }

            HandleCreated += new EventHandler(FormHandleCreated);

            // Trap thread exceptions and assertions so we don't have dialogs popping up
            LibSecurity.UnmanagedCode.Assert();
            Application.ThreadException += new ThreadExceptionEventHandler(OnThreadException);
            CodeAccessPermission.RevertAssert();

            if ( PreHandleMode ) {
                _testStarted = true;

                if ( PerformInitialization() )
                    BeginTest();
                else
                    TestIsDone(scenarioParams);
            }
        }

        // <doc>
        // <desc>
        //  We need to guarantee this Form's handle has been created before calling
        //  BeginTest(), otherwise we can end up starting the tests before the form
        //  or any of its children have even been created.
        // </desc>
        // </doc>
        private void FormHandleCreated(object sender, EventArgs ev) {
            // This is to stop the test from restarting when the form's handle is recreated
            if ( _testStarted || _preHandleMode )
                return;
            else
                _testStarted = true;

            // We need to use InvokeAsync here in order to allow the form
            // to display before BeginTest starts (or TestIsDone is called)
            if ( PerformInitialization() )
                BeginInvoke(new System.Windows.Forms.MethodInvoker(this.BeginTest));
            else
                BeginInvoke(new TestIsDoneInvoker(TestIsDone),
                    new object[] { scenarioParams });
        }

        private delegate void TestIsDoneInvoker(TParams p);

        // Calls InitTest and returns true if it succeeded, false if it failed.
        private bool PerformInitialization() {
            bool initPassed = false;

            log.WriteTag("TestInitialize", false);

            try {
                InitTest(scenarioParams);

                if ( _baseInitTestCalled )
                    initPassed = true;
                else {
                    initPassed = false;
                    scenarioParams.log.WriteLine("*****ERROR: ReflectBase.InitTest() was not called!");
                    scenarioParams.log.WriteLine("*****All test classes which override InitTest() must call base.InitTest().");
                }
            }
            catch (InitTestFailedException e) {
                log.WriteLine("Test initialization failed in class " +
                        e.TargetSite.DeclaringType.Name + ": " + e.Message);

                if ( e.InnerException != null ) {
			log.LogException(e.InnerException);
                }
            }
            catch (Exception e) {
                log.WriteLine("Test initialization failed :");
                log.WriteLine();
                //log.WriteLine(e.ToString());
				log.LogException(e);
            }

            log.CloseTag("TestInitialize");

            return initPassed;
        }

        // <doc>
        // <desc>
        //  Tests can override this method and put initialization code here, rather than
        //  in the test's constructor.  ReflectBase will catch any exceptions thrown, log
        //  them to the results file, and stop execution.
        //
        //  If test initialization fails for some reason, throw a InitTestFailedException.
        //
        //  IMPORTANT: Tests overriding this method should call base.InitTest() to ensure
        //             parent classes get properly initialized.
        // </desc>
        // </doc>
		[SecurityPermission(SecurityAction.Assert, Unrestricted=true)]
        protected virtual void InitTest(TParams p) {
            _baseInitTestCalled = true;
            p.log.WriteLine("Start time:  " + DateTime.Now.ToString(null, null));
			p.log.WriteLine("CLR Version: " + Environment.Version);
			p.log.WriteLine("Bitness:     " + (Marshal.SizeOf(typeof(IntPtr)) * 8) + "-bit");
			p.log.WriteLine("Thread ApartmentState: " + Thread.CurrentThread.GetApartmentState());
			p.log.WriteLine("Debugger attached? " + Debugger.IsAttached);

            WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (null != wp && null != wp.Identity.Name)
                p.log.WriteLine("User Identity: " + wp.Identity.Name); 
            else
                p.log.WriteLine("User Identity unknown"); 

            ListInstalledPrinters(p);
		}

        protected virtual void EndTest(TParams p)
        { }

		private static bool HasPermission(IPermission toDemand, PermissionSet toCheck)
		{
			try
			{
				toCheck.PermitOnly();
				try
				{ 
                    DemandPermission(toDemand); 
                }
				finally
				{ 
                    CodeAccessPermission.RevertPermitOnly(); 
                }//to ease debugging
				return true;
			}
			catch (SecurityException ex)
			{
				string s = ex.ToString();
				return false; 
			}
		}

		private static void DemandPermission(IPermission toDemand)
		{ 
            toDemand.Demand(); 
        }

		// <doc>
		// <desc>
		//  This will list the installed printers as well as the default printer
		// </desc>
		// </doc>
		protected void ListInstalledPrinters(TParams p)
		{
			PrinterSettings.StringCollection installedPrinters = SafeMethods.GetInstalledPrinters();
			PrinterSettings settings = new PrinterSettings();

			p.log.WriteLine("Default printer: " + SafeMethods.GetPrinterName(settings));
			p.log.WriteLine("Listing installed printers...");
			if (installedPrinters.Count <= 0) p.log.WriteLine("  NO PRINTERS INSTALLED!");
			else
			{
				foreach (string printerName in installedPrinters)
				{ p.log.WriteLine("  " + printerName); }
			}
		}

		// <doc>
        // <desc>
        //  Convenience method for sorting the scenarios contained in the given ScenarioGroup.
        //  Sorting the scenarios causes them to be executed in alphabetical order.  This can
        //  be called from BeforeScenarioGroup() in order to sort the scenarios in that group.
        //
        //  Alternatively, you can manually rearrange the scenarios however you want in
        //  BeforeScenarioGroup().
        // </desc>
        // </doc>
        protected void SortScenarioGroup(ScenarioGroup g) {
            Array.Sort(g.Scenarios, new MethodInfoComparer());
		}

		// <doc>
		// <desc>
		//  Convenience method for sorting the scenarios within a group. 
		//  Sorting the scenarios causes them to be executed in order defined by
		//	property Order on the ScenarioAttribute.  This can
		//  be called from BeforeScenarioGroup() in order to sort the scenarios in that group.
		//
		//  Alternatively, you can manually rearrange the scenarios however you want in
		//  BeforeScenarioGroup().
		// </desc>
		// </doc>
		protected void SortScenariosByOrder(ScenarioGroup g)
		{
			if ((g == null) || (g.Scenarios == null) || (g.Scenarios.Length<=1))
				return;
			
			MethodInfo[] orderedScenarios = new MethodInfo[g.Scenarios.Length];
			MethodInfo[] nonOrderedScenarios = new MethodInfo[g.Scenarios.Length];

			int currentOrderedIndex = 0;
			int currentNonOrderedIndex = 0;

			for (int i = 0; i < g.Scenarios.Length; i++)
			{
				ScenarioAttribute attr = GetScenarioAttribute(g.Scenarios[i]);

				if ((attr != null) && (attr.Order != Int32.MaxValue))
				{
					orderedScenarios[currentOrderedIndex] = g.Scenarios[i];
					currentOrderedIndex++;
				}
				else
				{
					nonOrderedScenarios[currentNonOrderedIndex] = g.Scenarios[i];
					currentNonOrderedIndex++;
				}
			}

			if(currentNonOrderedIndex > 0)
				Array.Copy(nonOrderedScenarios, 0, orderedScenarios, currentOrderedIndex, currentNonOrderedIndex);

			Array.Sort(orderedScenarios, 0, currentOrderedIndex, new ScenarioSorter());
			g.Scenarios = orderedScenarios;
		}
		// <doc>
		// <desc>
		//  Though we have no guarantee the assert will occur during the invokation of a test,
		//  we can be fairly certain that will be the case.  So, we set a flag to tell
		//  InvokeMethod that an error occurred, and set some strings so the failure can be
		//  logged.
		// </desc>
		// </doc>
		internal void SetAssertOrExceptionFailure(string shortMsg, string detailMsg)
		{
			_assertOrExceptionFailure = true;
            _shortErrorMsg = shortMsg;
            _detailErrorMsg = detailMsg;
        }

        // <doc>
        // <desc>
        //  This event handler will trap exceptions that occur on the message loop or a
        //  different thread.  It will ensure we have no WFC exception dialogs pop up
        //  during our lab runs.
        // </desc>
        // </doc>
        protected virtual void OnThreadException(Object sender, ThreadExceptionEventArgs e) {
            SetAssertOrExceptionFailure(e.Exception.GetType().ToString(), e.Exception.ToString());
        }

        // <doc>
        // <desc>
        //  This event handler will prevent WFC assertion failure dialogs from popping up.
        // </desc>
        // </doc>
        private void OnDisplayAssert(string shortMsg, string detailMsg) {
            SetAssertOrExceptionFailure(shortMsg, detailMsg);
        }

        //
        // The ScenarioResult you pass to this method will override any other results
        // obtained for the current scenario, either returned from the scenario or from
        // a thread exception, etc.
        //
        // You can use it to log a failure with a known 


        protected void OverrideCurrentScenarioResult(ScenarioResult r) {
            _overriddenScenarioResult = r;
        }

        //
        // Returns the ScenarioResult for this whole test.
        //
        protected ScenarioResult TestResults {
            [Scenario(false)]
            get { return log.TestResults; }
        }

        // <doc>
        // <desc>
        //  The parameters provided on the command line for the test
        // </desc>
        // <seealso member="commandLineParameters"/>
        // </doc>
        protected ArrayList CommandLineParameters {
            get {
                if (_commandLineParameters == null)
                    _commandLineParameters = new ArrayList();
                    
                return _commandLineParameters;
            }
        }
        
        // <doc>
        // <desc>
        //  The number of milliseconds to sleep between scenarios. Do NOT rely on this
        //  value for timing purposes and your scenario passing. Any timing required
        //  should be contained in the individual scenarios.
        // </desc>
        // <seealso member="sleep"/>
        // </doc>
        protected int Sleep {
            get { return _sleep; }
            set { _sleep = value; }
        }

        // <doc>
        // <desc>
        //  The value with which to seed the RandomUtil class's random number generator.
        //  Setting this property immediately seeds the random generator.
        // </desc>
        // <seealso member="seed"/>
        // </doc>
        protected int Seed {
            get { return _seed; }
            set {
                _seed = value;

                if ( scenarioParams == null )
                    _seeded = false;
                else {
                    _seeded = true;
                    scenarioParams.ru.SeedRandomGenerator(_seed);
                }
            }
        }

        // <doc>
        // <desc>
        //  If Pause is set to true, test will pause between each scenario.
        //  Default is false.
        // </desc>
        // <seealso member="pauseBetweenScenarios"/>
        // </doc>
        protected bool Pause {
            get { return _pauseBetweenScenarios; }
            set { _pauseBetweenScenarios = value; }
        }

        // <doc>
        // <desc>
        //  If StopOnFailureInGroup is true, test will not continue on to next scenario
        //  groups if there is a failure in the current group.  Default is false.
        // </desc>
        // <seealso member="stopOnFailureInGroup"/>
        // </doc>
        protected bool StopOnFailureInGroup {
            get { return _stopOnFailureInGroup; }
            set { _stopOnFailureInGroup = value; }
        }

        // <doc>
        // <desc>
        //  If PauseOnFailure is set to true, test will pause when a failure occurs.
        //  Default is false.
        // </desc>
        // <seealso member="pauseOnFailure"/>
        // </doc>
        protected bool PauseOnFailure {
            get { return _pauseOnFailure; }
            set { _pauseOnFailure = value; }
        }

        // <doc>
        // <desc>
        //  If StopOnFailureInGroup is true, KeepRunningTests will be set to false if
        //  a failure occurs.  Default is false.
        // </desc>
        // <seealso member="stopOnFailureInScenario"/>
        // </doc>
        protected bool StopOnFailureInScenario {
            get { return _stopOnFailureInScenario; }
            set { _stopOnFailureInScenario = value; }
        }

        // <doc>
        // <desc>
        //  If FullNames is true, test log will output the full name of the test being run,
        //  e.g. "XControl.Foo()" as opposed to just "Foo()".  Default is false.
        // </desc>
        // <seealso member="fullNames"/>
        // </doc>
        protected bool FullNames {
            get { return _fullNames; }
            set { _fullNames = value; }
        }

        // <doc>
        // <desc>
        //  Normally, KeepRunningTests is true.  If a scenario sets it to false, ReflectBase
        //  will terminate its test run and NOT quit.  This is handy for debugging a test case.
        //  You set KeepRunningTests to false and ReflectBase will stop running at that point
        //  leaving you with a test form that you can maniuplate.
        // </desc>
        // <seealso member="keepRunningTests"/>
        // </doc>
        protected bool KeepRunningTests {
            get { return _keepRunningTests; }
            set { _keepRunningTests = value; }
        }

        // <doc>
        // <desc>
        // If false, ReflectBase doesn't pay attention to PermissionRequired attributes on
        // scenarios.  If true, ReflectBase will perform security testing on scenarios
        // marked with PermissionRequired.
        //
        // If the required permission is granted, ReflectBase will treat the test as it
        // would normally.
        //
        // If the required permission is NOT granted, ReflectBase will fail the test if it
        // passes, or pass if it throws a SecurityException.
        // </desc>
        // <seealso member="testSecurity"/>
        // </doc>
        protected bool TestSecurity {
            get { return _testSecurity; }
            set { _testSecurity = value; }
        }

        // <doc>
        // <desc>
        // If true, ReflectBase filters all messages and prints debug information to the log
        // if it is a WM_KEYDOWN.  Might be helpful for debugging the various SendKeys issues
        // we've seen in our tests.
        // </desc>
        // </doc>
        protected bool LogKeystrokes {
            get { return _logKeystrokes; }
            set {
                if ( _logKeystrokes != value ) {
                    _logKeystrokes = value;

                    LibSecurity.UnmanagedCode.Assert();

                    if ( _logKeystrokes )
                        Application.AddMessageFilter(this);
                    else
                        Application.RemoveMessageFilter(this);

                    CodeAccessPermission.RevertAssert();
                }
            }
        }

        // no property set because you need to set this before the
        // test starts, i.e. on command-line
        protected bool PreHandleMode {
            get { return _preHandleMode; }
        }

        /// <summary>
        /// Default is false.  If true, test is running in manual mode.  This doesn't mean much
        /// except for the fact that ManualFreeze() is enabled, and ManualMode returns true.
        /// This allows tests to behave differently when running manually, e.g. pausing to allow
        /// visual verification, etc.
        /// </summary>
        /// <value></value>
        protected bool ManualMode {
            get { return _manualMode; }
            set { _manualMode = value; }
        }

		/// <summary>
		/// Default is true.  If true, Application.EnableVisualStyles() is called in the constructor,
		/// thus turning on Windows XP themes.
		/// </summary>
		/// <value></value>
		protected bool VisualStylesEnabled {
			get { return _visualStylesEnabled; }
			set { _visualStylesEnabled = value; }
		}

        // <doc>
        // <desc>
        // Adds a permission to the list of permissions required by the current scenario.  A
        // scenario which requires certain permissions must add them to the list.
        // ReflectBase will automatically clear the list between scenarios.
        // </desc>
        // </doc>
		[
		Obsolete("AddRequiredPermission() is obsolete.  Please use BeginSecurityCheck() instead"),
		EditorBrowsable(EditorBrowsableState.Never)
		]
        protected virtual void AddRequiredPermission(CodeAccessPermission p) {
			BeginSecurityCheck(p);
        }
		/// <summary>
		/// Begins a security check.  Called in a scenario immediately prior to a call to a
		/// member which requires security permissions.
		///
		/// ReflectBase expects one of two things to happen:
		///   1) Permission p is granted, thus this scenario runs normally (e.g. test is running in
		///      full trust).
		///   2) Permission p is denied (i.e. test is running in semi-trust), so we expect a
		///      SecurityException to be thrown.  If a SecurityException is not thrown, ReflectBase
		///      will fail this scenario.
		///
		/// NOTE: You must call EndSecurityCheck() immediately following the call to the
		///       member which you are testing.
		/// </summary>
		/// <param name="p">Permission required by the member being tested.</param>
		/// <example>
		/// protected ScenarioResult Focus(TParams p) {
		///     //...
		///     BeginSecurityCheck(LibSecurity.AllWindows);
		///     c.Focus();
		///     EndSecurityCheck();
		///     //...
		/// }
		/// </example>
		protected void BeginSecurityCheck(PermissionSet p) 
        {
			BeginSecurityCheck(p, null);
		}

		protected void BeginSecurityCheck(PermissionSet p, MethodInfo expectedMethod)
        {
			BeginSecurityCheck((CodeAccessPermission[])null, expectedMethod);
        }

		protected void BeginSecurityCheck(NamedPermissionSet p) 
        {
			BeginSecurityCheck(p, null);
		}

		protected void BeginSecurityCheck(NamedPermissionSet p, MethodInfo expectedMethod)
        {
			BeginSecurityCheck((CodeAccessPermission[])null, expectedMethod);
        }

		protected void BeginSecurityCheck(CodeAccessPermission p) 
        {
			BeginSecurityCheck(new CodeAccessPermission[] { p }, null);
		}

        protected void BeginSecurityCheck(CodeAccessPermission p, MethodInfo expectedMethod)
        {
            BeginSecurityCheck(new CodeAccessPermission[] { p }, expectedMethod);
        }

        protected void BeginSecurityCheck(CodeAccessPermission[] ps)
        {
            BeginSecurityCheck(ps, null);
        }

		protected void BeginSecurityCheck(CodeAccessPermission[] ps, MethodInfo expectedMethod)
        {
            if (_beginSecurityCheckCalledForCurrentScenario)
                { throw new ReflectBaseException("BeginSecurityCheck was called more than once in this test method, this is an error in the testcase"); }

			if (ps == null) { scenarioParams.log.WriteLine("SECURITY: doing blanket security check (no specific permissions are checked)"); }
			else
			{
				foreach (CodeAccessPermission p in ps)
				{
					if (TestSecurity)
					{ scenarioParams.log.WriteLine("SECURITY: Required permission \"{0}\" granted? {1}", p.GetType().Name, Utilities.HavePermission(p)); }

					_requiredPermissions.Add(p);
				}
			}

            _beginSecurityCheckCalledForCurrentScenario = true;
            securityCheckExpectedMethod = expectedMethod;
        }

		/// <summary>
		/// Ends a security check.  If we've reached this method without one or more of the
		/// required permissions present, that means there's a security 


		protected void EndSecurityCheck() 
		{
			// No exception was thrown
			if (TestSecurity) 
			{
				if (_requiredPermissions.Count <= 0) { throw new ReflectBaseException("FAIL (SECURITY): no SecurityException was thrown."); }
				else
				{
					foreach (CodeAccessPermission perm in _requiredPermissions)
					{
						if (!Utilities.HavePermission(perm))
							throw new ReflectBaseException("FAIL (SECURITY): didn't have \"" + perm.GetType().Name + "\", but SecurityException wasn't thrown.");
					}
				}
			}
		}

        // <summary>
        //  You can optionally specify command-line options in a file.  This is so it's
        //  easier to specify a MadDog context that relies upon a command-line switch.
        // </summary>
        private void CheckOptionsFile() {
			if (!File.Exists(OptionsFilename))
				return;
			else {
				// We no longer support this since MauiDriver supports command-line parameter requirements.
				// This was also causing problems in the lab where one tests's options.rb would be used by
				// all tests in a run.
				Console.WriteLine("Specifying flags in " + OptionsFilename + " is no longer supported.");
				return;
			}
#if false
            StreamReader reader = new StreamReader(OptionsFilename);
            string line;

            if ( commandLineParameters == null )
                commandLineParameters = new ArrayList();

            while ( (line = reader.ReadLine()) != null )
                commandLineParameters.Add(line);

            reader.Close();
#endif
        }

        /// <summary>
        /// ReflectBase options can be stored in an environment variable named "ReflectBaseOptions", in the
        /// form "/foo /bar /blah:123".  This makes it easier to specify a command-line parameter for an
        /// entire run--for example, to turn off visual styles, etc.
        /// 
        /// Subclasses can override this to pick up subclass-specific options.  They should use a different
        /// environment variable name (e.g. AutoPME should use "AutoPMEOptions"), and they should call the
        /// base class to make sure all options are picked up.
        /// </summary>
        protected virtual void CheckEnvironmentOptions() {
            LibSecurity.Environment.Assert();
            string optionsEnvVar = Environment.GetEnvironmentVariable("ReflectBaseOptions");
            CodeAccessPermission.RevertAssert();

            if (optionsEnvVar != null) {
                string[] options = optionsEnvVar.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach ( string option in options )
                    CommandLineParameters.Add(option);
            }
        }

        // <summary>
        //  Override this method if you want to programmatically set command-line
        //  options using the provided property accessors (e.g. PauseOnFailures, etc.).
        // </summary>
        protected virtual void SetOptions() {
        }

        // <doc>
        // <desc>
        //  Process the command line parameters. This is the base implementaion.
        //
        //  Classes that override this method to process their own command line
        //  parameters should make sure to call base.ProcessCommandLineParameters()
        //  AFTER they process their command line parameters.  In addition, they
        //  should remove any processed command-line parameters from the list of
        //  parameters.  Any remaining parameters that ReflectBase does not
        //  understand will cause an error message.
        //
        //  If a given parameter or parameters were processed, They are removed
        //  from the list of parameters.
        //  
        //  Processed command line parameters should be logged with the
        //  LogCommandLineParemater method.
        // </desc>
        // <seealso member="CommandLineParameters"/>
        // <seealso member="LogCommandLineParameter"/>
        // </doc>
        protected virtual void ProcessCommandLineParameters() {
            IEnumerator en = CommandLineParameters.GetEnumerator();
            // ArrayList doesn't allow removal during enumeration so we need this hack
            ArrayList paramsToRemove = new ArrayList();

            while (en.MoveNext ()) {
                string param = ((String)en.Current).ToUpperInvariant();
                try {
                    if ( param.Equals("") ) {			// workaround for msvbalib.Command
                        paramsToRemove.Add(en.Current);	// passing "" for no arguments
                    }
                    else if (param.StartsWith("/SLEEP:")) {
                        Sleep = int.Parse(param.Substring(7));
                        paramsToRemove.Add(en.Current);
                    }
                    else if (param.StartsWith("/SEED:")) {
                        Seed = int.Parse(param.Substring(6));
                        paramsToRemove.Add(en.Current);
                    }
                    else if (param.Equals("/PAUSE")) {
                        _pauseBetweenScenarios = true;
                        paramsToRemove.Add(en.Current);
                    }
                    else if (param.Equals("/STOP")) {
                        StopOnFailureInGroup = true;
                        paramsToRemove.Add(en.Current);
                    }
                    else if (param.Equals("/PAUSEFAIL")) {
                        _pauseOnFailure = true;
                        paramsToRemove.Add(en.Current);
                    }
                    else if (param.Equals("/STOPFAIL")) {
                        StopOnFailureInScenario = true;
                        paramsToRemove.Add(en.Current);
                    }
                    else if (param.Equals("/FULLNAMES")) {
                        FullNames = true;
                        paramsToRemove.Add(en.Current);
                    }
                    else if (param.Equals("/NOSECURITY")) {
                        TestSecurity = false;
                        paramsToRemove.Add(en.Current);
                    }
                    else if (param.Equals("/LOGKEYS")) {
                        LogKeystrokes = true;
                        paramsToRemove.Add(en.Current);
                    }
                    else if (param.Equals("/PREHANDLE")) {
                        _preHandleMode = true;
                        paramsToRemove.Add(en.Current);
                    }
                    else if (param.Equals("/MANUAL")) {
                        _manualMode = true;
                        paramsToRemove.Add(en.Current);
                    }
					else if (param.Equals("/NOVISUALSTYLES")) {
						_visualStylesEnabled = false;
						paramsToRemove.Add(en.Current);
					}
					else if (param.Equals("/SCENARIOPICKER")) {  
						_useScenarioPicker = true;
						paramsToRemove.Add(en.Current);
					}
					else if (param.StartsWith("/?") || param.StartsWith("/HELP"))
					{
						PrintHelp();
						ErrorExit();
					}
                }
                catch (Exception e) {
                    ErrorExit("Error processing command-line parameter: " + param + "\r\n" + e.ToString());
                }
            }

            en = paramsToRemove.GetEnumerator();
            while (en.MoveNext())
                CommandLineParameters.Remove(en.Current);
        }

        // <doc>
        // <desc>
        //  Prints out helpful command-line parameter information.  Subclasses can override
        //  this and print any additional command-line parameters.
        // </desc>
        // </doc>
        protected virtual void PrintHelp() {
            Console.WriteLine();
            Console.WriteLine("ReflectBase command-line parameters:");
            Console.WriteLine("  /? or /help        Display this usage message");
            Console.WriteLine("  /sleep:<int>       Sleep int number of milliseconds between each scenario");
            Console.WriteLine("  /seed:<int>        Provide a seed for the RandomUtil random number generator");
            Console.WriteLine("  /pause             Display a MessageBox between each scenario");
            Console.WriteLine("  /stop              Don't run next scenario groups if current one failed");
            Console.WriteLine("  /pausefail         Display a MessageBox after a scenario that fails");
            Console.WriteLine("  /stopfail          Stops running tests after a the first scenario that fails");
            Console.WriteLine("  /fullnames         Log scenarios by their fully-qualified names");
            Console.WriteLine("  /nosecurity        Do NOT perform security testing on scenarios");
            Console.WriteLine("  /logkeys           Log keystrokes that occur during the test (for debugging)");
            Console.WriteLine("  /prehandle         Execute testcases before form's handle has been created");
            Console.WriteLine("  /manual            ManualMode flag is set to true, ManualFreeze() is enabled.");
			Console.WriteLine("  /novisualstyles    Turns off visual styles (on by default).");
        }

        // <doc>
        // <desc>
        //  Logs command-line parameters if they have been set to something other
        //  than their defaults.
        //
        //  If this method is overridden, base.LogCommandLineParameters() should be called.
        // </desc>
        // </doc>
        protected virtual void LogCommandLineParameters() {
			// NOTE: For bool properties whose default value is true, remember to print out
			//		 the opposite value (i.e. !Property).
            if ( Sleep != 0 )
                LogCommandLineParameter("sleep", Sleep.ToString());
            if ( Seed != -1 )
                LogCommandLineParameter("seed", Seed.ToString());
            if ( Pause != false )
                LogCommandLineParameter("pause", Pause.ToString());
            if ( StopOnFailureInGroup != false )
                LogCommandLineParameter("stop", StopOnFailureInGroup.ToString());
            if ( PauseOnFailure != false )
                LogCommandLineParameter("pausefail", PauseOnFailure.ToString());
            if ( StopOnFailureInScenario != false )
                LogCommandLineParameter("stopfail", StopOnFailureInScenario.ToString());
            if ( _fullNames != false )
                LogCommandLineParameter("fullnames", FullNames.ToString());
            if ( TestSecurity != true )
                LogCommandLineParameter("nosecurity", (!TestSecurity).ToString());
            if ( LogKeystrokes != false )
                LogCommandLineParameter("logKeys", LogKeystrokes.ToString());
            if ( PreHandleMode != false )
                LogCommandLineParameter("prehandle", PreHandleMode.ToString());
            if ( ManualMode != false )
                LogCommandLineParameter("manual", ManualMode.ToString());
			if ( VisualStylesEnabled != true )
				LogCommandLineParameter("novisualstyles", (!VisualStylesEnabled).ToString());

            foreach (string parameter in CommandLineParameters)
                log.WriteLine("Invalid command-line parameter: " + parameter);
		}

        // <doc>
        // <desc>
        //  Writes out a <CommandLineParameter ... /> tag to the log file
        //  with the name and value of the command line parameter.
        // </desc>
        // <param term="name">
        //  The name of the command line parameter.
        // </param>
        // <param term="value">
        //  The value of the command line parameter.
        // </param>
        // </doc>
        protected void LogCommandLineParameter(String name, String value)
        {
            log.WriteTag("CommandLineParameter", true, new LogAttribute[] {
                new LogAttribute("name", name),
                new LogAttribute("value", value)
            });
        }

        //
        // This is where WM_KEYDOWNs get filtered for the /logkeys command-line
        // parameter.  Override this if you want to write any custom debug info
        // to the log file when a message gets processed.
        //
        public virtual bool PreFilterMessage(ref Message m) {
            if ( m.Msg == 0x0100 )  // WM_KEYDOWN
                scenarioParams.log.WriteLine("PreFilterMessage: WM_KEYDOWN; Handle = {0}; LParam = {1}", m.HWnd, (Keys)(int)m.WParam | ModifierKeys);

            return false;
        }

        // <doc>
        // <desc>
        //  Launches the TestEngine with the test's scenarioParams object
        // </desc>
        // </doc>
        private void BeginTest()
        {
            TestEngine(scenarioParams);
        }

        // <doc>
        // <desc>
        //   Executes the specified scenario.
        // </desc>
        // <param term=p>
        //  The parameters to pass to the scenario
        // </param>
        // <param term=group>
        //  The ScenarioGroup that this scenario is a member of
        // </param>
        // <param term=iMethod>
        //  The index into the ScenarioGroup of the method to call
        // </param>
        // <retvalue>
        //   True if the scenario passed, false otherwise.
        // </retvalue>
        // <seealso method="InvokeMethod"/>
        // </doc>
        protected virtual bool ExecuteScenario(TParams p, ScenarioGroup group, int iMethod)
        {
            MethodInfo mi = group.Scenarios[iMethod];
            String sTestDesc;

            if ( FullNames )
                sTestDesc = mi.DeclaringType + "." + mi.Name + GetParameterList(mi);
            else
                sTestDesc = mi.Name + GetParameterList(mi);

            p.log.WriteLine();

            ScenarioAttribute attr = GetScenarioAttribute(mi);

            if ( attr == null || attr.Description == null )
                p.log.StartScenario(sTestDesc);
            else
                p.log.StartScenario(sTestDesc, attr.Description);

            // call the test method
            ScenarioResult testResult = InvokeMethod(mi, p);

            if ( (object)testResult == null )
                testResult = new ScenarioResult(false, "***Test returned null ScenarioResult!***");

            p.log.EndScenario(testResult);

            if(testResult == ScenarioResult.Fail) {
                LibSecurity.UnmanagedCode.Assert();
                MessageBeep((int)MessageBoxIcon.Error);
                CodeAccessPermission.RevertAssert();
            }

            // This should process any of the msgs encurred during this test case
            Application.DoEvents();

            if ( Pause || (PauseOnFailure && testResult == ScenarioResult.Fail) ) {
			    if ( MessageBox.Show("Test Paused.\r\nStop running tests?", "ReflectBase", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes )
                    KeepRunningTests = false;
            }

            Thread.Sleep(Sleep);

            return (testResult == ScenarioResult.Pass);
        }

        // <doc>
        // <desc>
        //  Creates a Variant wrapper for a parameter type specefied by the
        //  ParameterInfo object passed in. ReflectBase scenarios have a method
        //  signature that returns a ScenarioResult and takes one or more
        //  parameters. The first parameter is always a TParams object. Subsequent
        //  parameters are used to address overridden methods. The parameters we
        //  pass in those slots are not used, but we still need to come up with a
        //  valid parameter to put there that the EE won't reject.
        // </desc>
        // <param term="pi">
        //  The ParameterInfo object that specifies everything we know about the
        //  parameter we are trying to wrap in a Variant.
        // </param>
        // <retvalue>
        //  A Variant object that wraps the desired parameter type
        // </retvalue>
        // <seealso member="InvokeMethod"/>
        // </doc>
        protected Object CreateInstance(Type type) {
            return CreateInstance(type, true);
        }

		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        protected Object CreateInstance(Type type, bool nullForRefTypes)
        {
            // We've had a few types whose constructors were such that they caused this
            // method to infinitely recurse, causing a cryptic StackOverflowException.
            // We'll try to avoid that situation and print a friendlier error message.
            //
            // If we've recursed 15 times, we'll assume we're stuck in a loop.
            if ( _numIterations > 15 )
                throw new ApplicationException("CreateInstance stuck in recursive loop");

            // We first cover special-case types that we at one point or another had a
            // problem with.  If it's not a special-case type we try to do what we can to
            // create an instance of that type.

            //
            // Special-cases: some classes are just extra tough.  We need to special case them.
            //
            if ( type == typeof(Guid) )
                return Guid.NewGuid();

            if ( type == typeof(Rectangle) )
                return Rectangle.Empty;

            if ( type == typeof(Size) )
                return Size.Empty;

            if ( type == typeof(RectangleF) )
                return RectangleF.Empty;

            if ( type == typeof(SizeF) )
                return SizeF.Empty;

            if ( type.IsPrimitive ) {
                // KevinTao 1/28/00: Yuck.  The primitives no longer have parameterless
                //                   constructors.  We need to special case them.
                //
                // Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64,
                // Char, Double, and Single.
                if ( type == typeof(Boolean) )
                    return false;
                else if ( type == typeof(Byte) || type == typeof(SByte) )
                    return (Byte)0;
                else if ( type == typeof(Int16) || type == typeof(UInt16) )
                    return (Int16)0;
                else if ( type == typeof(Int32) || type == typeof(UInt32) )
                    return (Int32)0;
                else if ( type == typeof(Int64) || type == typeof(UInt64) )
                    return (Int64)0;
                else if ( type == typeof(Char) )
                    return '0';
                else if ( type == typeof(Double) )
                    return (Double)0.0;
                else if ( type == typeof(Single) )
                    return (Single)0.0;
                else if ( type == typeof(IntPtr) )
                    return IntPtr.Zero;
                else
                    Debug.Assert(false, "Primitive type not recognized: " + type);
            }

            //
            // Try to generate a generic instance of these types.
            //

            // if the paramter is a member of an enum, we just get the first field
            // we find in the enum.
            //
            // 

            if (type.IsEnum)
                return Enum.GetValues(type).GetValue(0);

            // Special case color, whose constructors are private
            if ( type == typeof(Color) )
                return Color.Empty;

            // If it's not a value type and nullForRefTypes is true, return null!
            if ( nullForRefTypes && !type.IsValueType )
                return null;

            ConstructorInfo[] cia = type.GetConstructors();
            if (cia.Length == 0)
                return Activator.CreateInstance(type);
            else
            {
                ParameterInfo[] pai = cia[0].GetParameters();
                Object[] va = new Object[pai.Length];

                try {
                    ++_numIterations;    // keep an eye on the # of recursive calls

                    for (int i=0; i < va.Length; i++)
                        va[i] = CreateInstance(pai[i].ParameterType);
                }
                catch (ApplicationException e) {
                    // We recursively called CreateInstance() too many times.
                    // Jump back to the first call, print error message, and rethrow

                    if ( _numIterations <= 1 ) {     // back at the top level
                        log.WriteLine("ERROR: Recursive loop encountered creating " +
                            "type \"" + type+ "\".");
                        log.WriteLine("Consider special case code in " +
                            "ReflectBase.CreateInstance().");
                    }

                    throw e;
                }
                finally {
                    --_numIterations;
                }

                return cia[0].Invoke(va);
            }
        }

        // <doc>
        // <desc>
        //   Actually invokes the scenario.
        // </desc>
        // <param term="mi">
        //  The MethodInfo object that describes the scenario method we are about to call
        // </param>
        // <param term="p">
        //  The parameters to pass to the scenario we are calling
        // </param>
        // <retvalue>
        //   The ScenarioResult from the executed scenario.
        // </retvalue>
        // </doc>
        [Scenario(false)]
        protected virtual ScenarioResult InvokeMethod(MethodInfo mi, TParams p)
        {
            ScenarioResult sr = new ScenarioResult(false,
                    "ScenarioResult was not set in ReflectBase.InvokeMethod()");
            try
            {
                ParameterInfo[] pi = mi.GetParameters();
                Object[] va = new Object[pi.Length];
                va[0] = p;
                for (int i = 1; i < va.Length; i++)
                    va[i] = CreateInstance(pi[i].ParameterType);

                _requiredPermissions.Clear();    // Clear the security permissions list for the next scenario
				_baseBeforeScenarioCalled = false;

				if (!BeforeScenario(p, mi))
                    sr = new ScenarioResult(false, "BeforeScenario() returned false.  Scenario will not execute.");
                else {
					if (!_baseBeforeScenarioCalled) {
						p.log.WriteLine("*****ERROR: ReflectBase.BeforeScenario() was not called!");
						p.log.WriteLine("*****All test classes which override BeforeScenario() must call base.BeforeScenario().");
						sr = new ScenarioResult(false, "FAIL: ReflectBase.BeforeScenario() was not called.");
					}
					else {
						LibSecurity.Reflection.Assert();
                        try
                        {
                            //If use Mita is true call Scenarios on a non-UI thread. 
                            if (_useMita)
                            {
                                AsyncScenario scenario = new AsyncScenario(CallScenarioAsync);
                                IAsyncResult result = scenario.BeginInvoke(mi, va, null, null);
                                while (result.IsCompleted == false)
                                {
                                    Application.DoEvents();
                                }
                                sr = scenario.EndInvoke(result);
                            }
                            else
                                sr = (ScenarioResult)(mi.Invoke(this, va));
                        }
						finally {
							CodeAccessPermission.RevertAssert();
						}
						if (TestSecurity) {
							// No exception was thrown
							foreach (CodeAccessPermission perm in _requiredPermissions) {
								if (!Utilities.HavePermission(perm))
									sr.IncCounters(false, "FAIL (SECURITY): didn't have \"" + perm.GetType().Name + "\", but SecurityException wasn't thrown.", p.log);
							}
						}
					}
				}

                // In pre-handle mode, if a scenario causes handle creation, it needs to be special-cased or
                // excluded from pre-handle mode.  This is because once the handle is created, any scenarios
                // that run after that scenario aren't really testing pre-handle functionality.
                //

                if ((sr == ScenarioResult.Fail) && (sr.Comments == null))
                    sr.Comments = mi.Name + " FAILED";
            }
            catch ( TargetInvocationException tie ) {
                Exception e = tie.InnerException;

                if ( TestSecurity && e.GetType() == typeof(SecurityException) ) {
                    // SECURITY: Pass if the SecurityException's permission type matches one of our required permissions
                    SecurityException se = (SecurityException)e;
                    p.log.WriteLine("SecurityException thrown, permission type = {0}:", (se.PermissionType == null ? "null" : se.PermissionType.Name));
                    p.log.WriteLine(se.ToString());
                    p.log.WriteLine();

                    LibSecurity.GetPermissionState.Assert();    // need SecurityPermission to get se.PermissionState
                    p.log.WriteLine("Permission State: ");
                    p.log.WriteLine(se.PermissionState);
                    CodeAccessPermission.RevertAssert();

					sr = VerifySecurityException(p, se);
				}
				else {
                    // If something blows up, we assume that the scenario failed and we try
                    // to get some interesting information to return for debugging purposes
                    sr = new ScenarioResult(false, mi.Name + " excepted: " + e.GetType() + ": " + e.Message);
                    //p.log.WriteLine(sr.Comments + "\r\n" + e.StackTrace);
					p.log.WriteLine(sr.Comments);
					p.log.LogException(e);
                }

            }
            catch ( Exception ex ) {
                // Some exception occurred outside of the Invoke--theoretically this shouldn't happen.
                sr = new ScenarioResult(false, mi.Name + " excepted: " + ex.GetType() + ": " + ex.Message);
                //p.log.WriteLine(sr.Comments + "\r\n" + ex.StackTrace);
				p.log.WriteLine(sr.Comments);
				p.log.LogException(ex);
			}

            // Make sure no asserts or thread exceptions occurred during this test.
            // HOPEFULLY, this will assure the assert or exception won't occur outside
            // this method.
            Application.DoEvents();

            // Log any thread exceptions or asserts and fail.
            if ( _assertOrExceptionFailure ) {
                sr = new ScenarioResult(false, "Uncaught exception or assertion failure: " + _shortErrorMsg);
                p.log.WriteLine(sr.Comments + "\r\n" + _detailErrorMsg);
            }

            // If user chose to override the current scenario result we'll use it as our result.
            if ( _overriddenScenarioResult != null )
                sr = _overriddenScenarioResult;

            // Clear state for next scenario
            _overriddenScenarioResult = null;
            _assertOrExceptionFailure = false;

            try {
				_baseAfterScenarioCalled = false;
				AfterScenario(p, mi, sr);

				if (!_baseAfterScenarioCalled) {
					p.log.WriteLine("*****ERROR: ReflectBase.AfterScenario() was not called!");
					p.log.WriteLine("*****All test classes which override AfterScenario() must call base.AfterScenario().");
					sr = new ScenarioResult(false, "FAIL: ReflectBase.AfterScenario() was not called.");
				}
            }
            catch (Exception ex) {
                sr = new ScenarioResult(false, "AfterScenario() threw exception: " + ex.GetType() + ": " + ex.Message);
                //p.log.WriteLine(sr.Comments + "\r\n" + ex.StackTrace);
				p.log.WriteLine(sr.Comments);
				p.log.LogException(ex);
			}

            return sr;
		}

        [STAThread()]
        private ScenarioResult CallScenarioAsync(MethodInfo mi, object[] parameters)
        {
            return (ScenarioResult)(mi.Invoke(this, parameters));
        }


		protected bool CheckPermissionSet(ScenarioResult result, PermissionSet ps, CodeAccessPermission perm)
		{
            if (ps.IsUnrestricted() && (perm is SecurityPermission) && (perm as SecurityPermission).IsUnrestricted())
                return true;

			foreach (IPermission sp in ps)
			{
				if (CheckPermission(result, sp, perm)) { return true; }
			}

			return false;
		}

		protected bool CheckPermission(ScenarioResult result, IPermission dip, CodeAccessPermission perm)
		{
			return (dip.GetType() == perm.GetType());
		}

		[Scenario(false)]
		protected virtual ScenarioResult VerifySecurityException(TParams p, SecurityException se) 
        {
			ScenarioResult result = new ScenarioResult();

			if (_requiredPermissions.Count <= 0)
			{
				p.log.WriteLine("Unexpected security exception");
				p.log.LogException(se);

				result.IncCounters(false, "FAIL: Unexpected SecurityException was thrown", p.log);
			}
			else
			{
				IPermission dip;
				PermissionSet dps;
				LibSecurity.CrackSecurityException(se, out dip, out dps);

				// check permissions
				foreach (CodeAccessPermission perm in _requiredPermissions)
				{
					bool found = false;
					if (dps != null) { found = CheckPermissionSet(result, dps, perm); }
					else if (dip != null) { found = CheckPermission(result, dip, perm); }
					else { Debug.Assert(false, "CrackSecurityException didn't work"); }

					if (found)
					{
						p.log.WriteLine("PASS (SECURITY): SecurityException thrown for permission \"{0}\"", perm.GetType().Name);
						result.IncCounters(true);
					}
					else
					{
						result.IncCounters(false, "FAIL (SECURITY): SecurityException didn't contain required permission \"" + perm.GetType().Name + "\"", p.log);
					}
				}
			}

			// check stack if needed
			if (securityCheckExpectedMethod != null && StackCheck)
			{
				StackTrace stack = new StackTrace(se);
				bool stackCorrect = false;

				foreach(StackFrame frame in stack.GetFrames())
				{
					if (frame.GetMethod() == securityCheckExpectedMethod) { stackCorrect = true; }
				}
				
				if (stackCorrect)
				{
					p.log.WriteLine("PASS (SECURITY): SecurityException stack contained \"{0}\" as expected.", securityCheckExpectedMethod.Name);
					result.IncCounters(true);
				}
				else
				{
					result.IncCounters(false, "FAIL (SECURITY): SecurityException stack did not contain \"" + securityCheckExpectedMethod.Name + "\" as expected.", p.log);
				}
			}
			else { p.log.WriteLine("SECURITY: Ignore Exception Stack"); }

			return result;
		}

		/// <summary>
        /// This method is called before a scenario is executed.  It can be overridden to perform tasks
        /// such as re-initializing state, as well as for debugging (Utilities.ActiveFreeze() before
        /// scenarios, etc.).  This method can also be used to tell ReflectBase not to execute the
        /// scenario by returning false.  ReflectBase will skip the scenario and record a failure.
        ///
        /// When overriding this method, remember to call base.BeforeScenario().
        /// </summary>
        /// <param name="p">TParams object.</param>
        /// <param name="scenario">A MethodInfo of the scenario about to be executed.</param>
        /// <returns>True if the scenario should be executed, false otherwise.</returns>
        protected virtual bool BeforeScenario(TParams p, MethodInfo scenario) {
			currentScenario = scenario;
			_baseBeforeScenarioCalled = true;
			_beginSecurityCheckCalledForCurrentScenario = false;
			securityCheckExpectedMethod = null;
			return true;
        }

        /// <summary>
        /// This method is called after a scenario has been executed.
        ///
        /// When overriding this method, remember to call base.AfterScenario().
        /// </summary>
        /// <param name="p">TParams object.</param>
        /// <param name="scenario">A MethodInfo of the scenario that was just executed.</param>
        /// <param name="result">The ScenarioResult returned by the scenario.</param>
        protected virtual void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result) {
			currentScenario = null;
			_baseAfterScenarioCalled = true;
			_beginSecurityCheckCalledForCurrentScenario = false;
			securityCheckExpectedMethod = null;
		}

        // <doc>
        // <desc>
        //   Creates the ScenarioGroups and loops through each scenario in each
        //   group.
        // </desc>
        // <param term=p>
        //   The TParams object associated with this testcase.
        // </param>
        // <seealso member="BeforeScenarioGroup"/>
        // <seealso member="AfterScenarioGroup"/>
        // </doc>
        protected virtual void TestEngine(TParams p)
        {
            try
            {
                // We need to ensure our form is the active one so scenarios
                // doing UI interaction don't have any problems.
                SafeMethods.Activate(this);

                tests = CreateScenarioGroups(GetAllScenarios(this));

                //PairwiseModelManager modelManager = new PairwiseModelManager();
                //modelManager.InitializeDefaultModel(this);

				//This code is only run if the /SCENARIOPICKER command line arg is supplied
				if (_useScenarioPicker)
				{
					ScenarioPicker sp = new ScenarioPicker(tests, testName);
					DialogResult result = sp.ShowDialog(this);
					tests = sp.SelectedScenarios;
					sp.Dispose();
				}
				
				bool bPass = true;
                /**
                 * Run tests for each method ... collecting fail state as we go.
                 * If any tests fail, bPass = false
                 */
                for (int j = 0; j < tests.Count; j++)
                {
                    if (_stopTests) break;
                    ScenarioGroup g = (ScenarioGroup)tests[j];
					SortScenariosByOrder(g); 
					
					int groupTotal = 0;
                    int groupFail = 0;

                    p.log.WriteLine();
                    p.log.WriteTag("ScenarioGroup", false, new LogAttribute("name", g.Name));

                    if (BeforeScenarioGroup(p, g))
                    {
                        for (int i = 0; i < g.Scenarios.Length; i++)
                        {
                            if (_stopTests) break;
                            bool b = ExecuteScenario(p, g, i);
                            groupTotal++;

                            if (!b) {
                                groupFail++;

                                if ( StopOnFailureInScenario )
                                    KeepRunningTests = false;
                            }

                            bPass = b && bPass;

                            if ( !KeepRunningTests )
                                break;
                        }
                    }

                    bool bContinue = AfterScenarioGroup(p, g, groupTotal, groupFail);
                    p.log.CloseTag("ScenarioGroup");

                    if ( !KeepRunningTests ) {
                        p.log.WriteLine("********* Remaining tests not executed because KeepRunningTests flag was set to false");
                        break;
                    }
                    else if (StopOnFailureInGroup && tests.Count > (j + 1) && !bPass)
                    {
                        // bail out if something in the group we just finished executing failed.
                        // Don't print the msg if there are no more groups.  Duh!
                        int remain = tests.Count - j - 1;

                        p.log.WriteLine("********* The remaining [" + remain.ToString() + "] scenario groups not called due to failure in group [" + g.Name + "]");
                        p.log.WriteTag ("GroupsNotCalled", false);

                        for (int n = j + 1; n < tests.Count; n++)
                            p.log.WriteLine (((ScenarioGroup)tests[n]).Name);

                        p.log.CloseTag ("GroupsNotCalled");

                        break;
                    }

                    if (!bContinue) break;
                }
                EndTest(p);
                TestIsDone(p);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    TargetInvocationException ite;
                    ite = (TargetInvocationException)ex;
                    ex = ite.InnerException;
                }

                // this includes the stack trace in 8625
                //p.log.WriteLine(ex.ToString());
				p.log.LogException(ex);

                FatalError("An unexpected exception occurred: " + ex.GetType());
            }
        }

        // <summary>
        //  Creates the testcase.don file to signal that the testcase is finished running.
        //  We needed to add this so the driver could tell when the test was finished when
        //  it was run as an HREF exe.
        // </summary>
        [FileIOPermission(SecurityAction.Assert, Unrestricted=true)]
        private void CreateSemaphoreFile() {
            RestoreWorkingDirectory();
            Stream s = File.Create(SemaphoreFilename);
            s.Close();
        }

        private void RestoreWorkingDirectory()
        {
            try
            {
                if (string.IsNullOrEmpty(this._workingDir))
                { SafeMethods.SetCurrentDirectory(s_initialDirectory); }
                else
                { SafeMethods.SetCurrentDirectory(this._workingDir); }

            }
            catch (Exception ex)
            {
                try
                {
                    scenarioParams.log.WriteLine("Exception attempting to reset current directory");
                    scenarioParams.log.WriteLine("Current value: {0}", SafeMethods.GetCurrentDirectory());
                    scenarioParams.log.WriteLine("Attempted value: {0}", s_initialDirectory);
                    scenarioParams.log.WriteLine(ex.ToString());
                }
                catch (Exception)
                {
                    Console.WriteLine("Exception attempting to reset current directory");
                    Console.WriteLine("Current value: {0}", SafeMethods.GetCurrentDirectory());
                    Console.WriteLine("Attempted value: {0}", s_initialDirectory);
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted=true)]
        private void CreateMadDogResultsFiles(ScenarioResult finalResults) {
/*
// We'll skip this until MDLog is ready for prime time

            // Translate our XML log to MDLog format results.xml
            Stream log = new FileStream(LogFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WFCTestLib.XmlLogTree.Util.MDLogConverter.ConvertLog(log, MDLogFilename);
            log.Close();
*/

            // Create results.txt for driver to determine pass/fail count.  Driver only
            // parses PASSED or FAILED so we don't need to be too fancy here.
            RestoreWorkingDirectory();
            StreamWriter writer = new StreamWriter(LogTxtFilename);

            for ( int i = 0; i < finalResults.PassCount; i++ )
                writer.WriteLine("PASSED");

            for ( int i = 0; i < finalResults.FailCount; i++ )
                writer.WriteLine("FAILED");

            writer.Close();
        }

        // <doc>
        // <desc>
        //   When the test is completed, this method is called to clean up
        //   objects and close the test form.
        // </desc>
        // <param term="p">
        //   The TParams object associated with this test.
        // </param>
        // <seealso member="stopTests"/>
        // </doc>
        protected virtual void TestIsDone(TParams p)
        {
            _stopTests = true;
            p.log.EndTest();
            CreateSemaphoreFile();
            CreateMadDogResultsFiles(p.log.TestResults);

            // If a test set this to false, we want to keep the form open so the
            // test writer can mess around with and debug the form.
            //
            if ( KeepRunningTests ) {
                if ( !PreHandleMode )
                    Close();
                else {
                    // In pre-handle mode, the form isn't visible yet, so we need to show it
                    // then push close onto the message queue.  Can't call close directly because
                    // Application.Run() tries to make the form visible after this code executes
                    // and fails with an ObjectDisposed exception if we've already closed it.
                    //
                    this.Visible = true;
                    BeginInvoke(new System.Windows.Forms.MethodInvoker(this.Close));
                }
            }
			_TestComplete = true;
		}

		private bool _TestComplete = false;
		public bool TestComplete
		{ get { return _TestComplete; } }

		// <doc>
        // <desc>
        //   Closes the logfile and exits the application.
        // </desc>
        // <param term="errorString">
        //   The errror string to include in the logfile.
        // </param>
        // </doc>
        protected void FatalError(String errorString)
        {
            log.TestResults.Comments = errorString;
            log.TestResults.IncCounters(false);
            log.EndTest();
            CreateSemaphoreFile();
            ErrorExit();
        }

        [SecurityPermission(SecurityAction.Assert, Unrestricted=true)]
        protected void ErrorExit(String errorString)
        {
            if ( errorString != null )
                Console.Error.WriteLine(errorString);

            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        protected void ErrorExit() {
            ErrorExit(null);
        }

        // <doc>
        // <desc>
        //   Creates a group of scenarios. The default implementation is the group
        //   of all the scenarios in a group called "All". Classes that extend
        //   ReflectBase can group scenarios in any arbitrary fashion and should
        //   override this method to do so.
        // </desc>
        // <param term="scenarios">
        //   All of the methods that return a ScenarioResult on this class.
        // </param>
        // <retvalue>
        //   A list of ScenarioGroups.
        // </retvalue>
        // <seealso class="ScenarioGroup"/>
        // </doc>
        protected virtual ArrayList CreateScenarioGroups(MethodInfo[] scenarios)
        {
            ArrayList al = new ArrayList();
            ScenarioGroup sl = new ScenarioGroup();

            sl.Name = "All";
			sl.Scenarios = scenarios;

			al.Add(sl);

	        return al;
        }

        // <doc>
        // <desc>
        //   This method is called before a group of scenarios is executed. The
        //   default implementation does nothing. Classes that extend ReflectBase
        //   can override this method to perform custom actions.
        // </desc>
        // <param term="p">
        //   The TParams object associated with this test
        // </param>
        // <param term="g">
        //   The group of scenarios about to be executed.
        // </param>
        // <retvalue>
        //   True to execute the group; false otherwise.
        // </retvalue>
        // <seealso member="AfterScenarioGroup"/>
        // </doc>
        protected virtual bool BeforeScenarioGroup(TParams p, ScenarioGroup g)
        {
            return true;
        }

        // <doc>
        // <desc>
        //   This method is called after a group of scenarios is executed. The
        //   default implementation prints results. Classes that extend ReflectBase
        //   can override this method to perform custom actions.
        // </desc>
        // <param term="p">
        //   The TParams object associated with this test
        // </param>
        // <param term="g">
        //   The group of scenarios that was just executed.
        // </param>
        // <retvalue>
        //   True to continue executing groups; false otherwise.
        // </retvalue>
        // <seealso member="BeforeScenarioGroup"/>
        // </doc>
        protected virtual bool AfterScenarioGroup(TParams p, ScenarioGroup g, int total, int fail)
        {
            p.log.WriteLine();
            p.log.WriteTag("ClassResults", true, new LogAttribute[] {
                new LogAttribute("type", (fail==0)?"Pass":"Fail"),
                new LogAttribute("total", total.ToString()),
                new LogAttribute("fail", fail.ToString())
            });

            return true;
        }

        // <doc>
        // <desc>
        //  ReflectBase overrides the OnClosed event to notify the test engine to stop
        //  executing scenarios.
        // </desc>
        // <seealso member="stopTests"/>
        // </doc>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_stopTests == false)
            {
                _stopTests = true;
                log.TestResults.IncCounters(false);
                log.TestResults.Comments = "Test aborted by user";
            }
        }

        internal static ScenarioAttribute GetScenarioAttribute(MethodInfo mi) {
            object[] attrs = mi.GetCustomAttributes(typeof(ScenarioAttribute), true);

            if ( attrs == null || attrs.Length <= 0 )
                return null;
            if ( attrs.Length == 1 )
                return (ScenarioAttribute)attrs[0];
            else
                throw new Exception("ScenarioAttribute is defined more than once on " + mi.Name);
        }

        /// <summary>
        /// If ManualMode is true, calls Utilities.ActivePrompt() to freeze the test with the UI
        /// remaining interactive.  ActivePrompt() displays a messagebox with Yes/No buttons, and
        /// depending on which is clicked, this returns a passing or failing ScenarioResult.
        ///
        /// If ManualMode is false, calls to this method do nothing, and a failing ScenarioResult
        /// is returned.
        /// </summary>
        /// <param name="message">Message to be displayed in the messagebox.</param>
        /// <returns>
        /// If ManualMode is true, returns pass or fail depending on if Yes or No is clicked.
        /// If ManualMode is false, returns fail.
        /// </returns>
        [Scenario(false)]
        public ScenarioResult ManualPrompt(string message) {
            if (!ManualMode)
                return new ScenarioResult(false, "FAIL (manual verification): ManualPrompt() call failed since ManualMode is false.", scenarioParams.log);

            return new ScenarioResult(WFCTestLib.Util.Utilities.ActivePrompt(message), "FAIL (manual verification): " + message);
        }

        /// <summary>
        /// If ManualMode is true, calls Utilities.ActiveFreeze().  Else does nothing.
        /// </summary>
        public void ManualFreeze() {
            if (ManualMode)
                Utilities.ActiveFreeze();
        }

        /// <summary>
        /// If ManualMode is true, calls Utilities.ActiveFreeze().  Else does nothing.
        /// </summary>
        /// <param name="message">Message to display in the messagebox.</param>
        public void ManualFreeze(string message) {
            if (ManualMode)
                Utilities.ActiveFreeze(message);
        }

        // <doc>
        // <desc>
        //  Finds all methods on the provided object that return a ScenarioResult. Methods
        //  that return a ScenarioResult are called scenarios.  Methods returning ScenarioResult
        //  may be discluded from being run by using defining ScenarioAttribute(false).
        // </desc>
        // <param term="o">
        //  The Object on which to find scenarios
        // </param>
        // <retvalue>
        //  An array of MethodInfo object representing each scenario found on the object provided.
        // </retvalue>
        // </doc>
        // SECURITY
        [ReflectionPermission(SecurityAction.Assert, Unrestricted=true)]
        public static MethodInfo[] GetAllScenarios(Object o)
        {
            ArrayList l = new ArrayList();
            Type typ = o.GetType();
            MethodInfo[] mi = typ.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            // BETA2: DefaultLookup doesn't exist in Beta2; couldn't find a replacement.  Hopefully, it's the default.
            // MethodInfo[] mi = typ.GetMethods(BindingFlags.NonPublic | BindingFlags.DefaultLookup);
            for (int i=0; i<mi.Length; i++) {
                if (mi[i].ReturnType == typeof(ScenarioResult)) {
                    ScenarioAttribute attr = GetScenarioAttribute(mi[i]);

                    // Include any ScenarioResult method which doesn't define ScenarioAttribute or
                    // does define ScenarioAttribute(true)
                    if ( attr == null || (attr != null && attr.IsScenario) )
                        if (!excludedMethods.Contains(mi[i].Name))
                            l.Add(mi[i]);
                }
            }
            return (MethodInfo[])l.ToArray(typeof(MethodInfo));
        }

        // Returns an array of strings representing the command-line arguments.  This is for
        // use with VB tests, since VB doesn't have command-line params passed to the Main()
        // method.
        [EnvironmentPermission(SecurityAction.Assert, Unrestricted=true)]
        public static string[] GetCommandLineArgs() {
            string[] args = Environment.GetCommandLineArgs();
            string[] returnVal = new string[args.Length - 1];

            // Remove the first element (the executable name) from the array.
            Array.Copy(args, 1, returnVal, 0, args.Length - 1);
            return returnVal;
        }

        // <doc>
        // <desc>
        //  A helper routine that builds a string representing the parameter
        //  signature of the method specified by the MethodInfo provided
        // </desc>
        // <param term="mi">
        //  The MethodInfo from which the method's parmameter list is obtained.
        // </param>
        // <retvalue>
        //  A String representing the parameter signature of the method in question.
        // </retvalue>
        // </doc>
        public static String GetParameterList(MethodBase mi)
        {
            ParameterInfo[] pi = mi.GetParameters();
            String s = "(";
            for (int i=0; i<pi.Length; i++)
            {
                if (pi[i].ParameterType == typeof(TParams)) continue;
                s = s + pi[i].ParameterType.Name + " " + pi[i].Name;
                if (i != pi.Length - 1)
                    s = s + ", ";
            }
            s = s + ")";

            return s;
        }

        [DllImport("USER32", CharSet=CharSet.Auto)]
        extern public static bool MessageBeep(int uType);

        /// <summary>
        /// Checks to ensure that the passed in delegate throws an exception of the expected type when executed.
        /// </summary>
        /// <param name="sr">The scenarioParams in which to log the result.</param>
        /// <param name="expectedExceptionType">The resultant exception type (the exception must derive from this type.</param>
        /// <param name="testCode">The code which is expected to throw an exception.</param>
        public void ExpectException(
            ScenarioResult sr,
            Type expectedExceptionType,
            System.Windows.Forms.MethodInvoker testCode)
        {
            try
            {
                testCode();
                sr.IncCounters(false, "FAIL: Expected an exception of type " + expectedExceptionType.Name, scenarioParams.log);
            }
            catch (Exception e)
            {
                if (!expectedExceptionType.IsAssignableFrom(e.GetType()))
                {
                    sr.IncCounters(expectedExceptionType, e.GetType(), "FAIL: Incorrect exception type", scenarioParams.log);
                    throw;
                }
                sr.IncCounters(ScenarioResult.Pass);
            }
        }

		public bool SecurityCheck(
			ScenarioResult sr,
            System.Windows.Forms.MethodInvoker testCode, 
			MethodBase expectedStackMethod, 
			params CodeAccessPermission[] expectedPermissions)
		{
			TParams p = ScenarioParams;
			if (null == sr) 
			{ throw new ArgumentNullException("sr", "ScenarioResult must be specified"); }
			if (expectedPermissions == null) 
			{ throw new ArgumentNullException("expectedPermissions"); }
			if (expectedPermissions.Length < 1) 
			{ throw new ArgumentException("there must be at least one expected permission"); }

			bool testCodeSucceeded = false;

			p.log.WriteLine("Testing Security");
			if (!TestSecurity)
			{
				p.log.WriteLine("Not testing security");
				testCode();
				testCodeSucceeded = true;
				
				sr.IncCounters(true, "Not testing Security",p.log);
				return testCodeSucceeded;
			}

			foreach (CodeAccessPermission permission in expectedPermissions)
			{ 
				p.log.WriteLine("SECURITY: Required permission \"{0}\" granted? {1}", 
					permission.GetType().Name, 
					Utilities.HavePermission(permission)); 
			}

			try
			{
				testCode();
				testCodeSucceeded = true;
				foreach (CodeAccessPermission permission in expectedPermissions)
				{
					if (!Utilities.HavePermission(permission))
					{ 
						sr.IncCounters(false, 
							string.Format("FAIL (SECURITY): didn't have \"{0}\", but SecurityException wasn't thrown.",
							permission.GetType().Name ), p.log);
						p.log.WriteLine("The test code should have thrown a security exception but did not, fix this issue " +
							"before looking into further failures in this scenario");
						return false;//
						//The caller should assume that the testCode had no side affects (though it probably did)
					}
				}
				sr.IncCounters(true);
			}
			catch (SecurityException se)
			{
				p.log.WriteLine("SecurityException thrown, permission type = {0}:", 
					(se.PermissionType == null ? "[null]" : se.PermissionType.Name));

				LibSecurity.GetPermissionState.Assert();    // need SecurityPermission to get se.PermissionState
				p.log.WriteLine("Permission State: ");
				p.log.WriteLine(se.PermissionState);
				CodeAccessPermission.RevertAssert();

				bool foundRequiredPermission = false;
				foreach (CodeAccessPermission perm in expectedPermissions)
				{
					if (se.PermissionType == perm.GetType() || se.PermissionType == typeof(PermissionSet))
					{
						p.log.WriteLine("PASS (SECURITY): SecurityException thrown for permission \"{0}\"", perm.GetType().Name);
						foundRequiredPermission = true;
						continue;
					}
				}
				//Fail the scenario because there were no matching security permissions
				sr.IncCounters(foundRequiredPermission, "FAIL (SECURITY): SecurityException didn't match any required permission", p.log);

				if (null == expectedStackMethod)
				{ p.log.WriteLine("Skipping stack check (no stack frame specified)"); }
				else
				{
					StackTrace stack = new StackTrace(se);

					bool foundStackFrame = false;
					foreach (StackFrame frame in stack.GetFrames())
					{
						if (expectedStackMethod == frame.GetMethod())
						{
							foundStackFrame = true;
						}
					}
					sr.IncCounters(foundStackFrame, 
						string.Format(
						"FAIL (SECURITY): Did not find \"{0}\" as expected in the SecurityException stack.", 
						expectedStackMethod.Name), 
						p.log);
					p.log.WriteLine("Stack trace:");
					p.log.WriteLine(se.ToString());
					return testCodeSucceeded;
				}
			}
			return testCodeSucceeded;
		}
	}

    // <doc>
    // <desc>
    //  Provides IComparer support for MethodInfos.  Used by SortScenarioGroup()
    //  method.
    // </desc>
    // </doc>
    class MethodInfoComparer : IComparer {
        public int Compare(Object x, Object y) {
            if ( !(x is MethodInfo && y is MethodInfo) )
                throw new ArgumentException("Can only compare MethodInfos");

            return String.Compare(((MethodInfo)x).Name, ((MethodInfo)y).Name);
        }
    }

	// <doc>
	// <desc>
	//  Provides IComparer support for MethodInfos. Specifically orders Scenarios. Used by SortScenario()
	//  method.
	// </desc>
	// </doc>
	class ScenarioSorter: IComparer
	{
		public int Compare(Object x, Object y)
		{
			if (!(x is MethodInfo && y is MethodInfo))
				throw new ArgumentException("Can only compare MethodInfos");

			ScenarioAttribute xAttr = ReflectBase.GetScenarioAttribute((MethodInfo)x);
			ScenarioAttribute yAttr = ReflectBase.GetScenarioAttribute((MethodInfo)y);
			int orderX = 0;
			int orderY = 0;

			if (xAttr != null)
				orderX = xAttr.Order;

			if (yAttr != null)
				orderY = yAttr.Order;

			return orderX.CompareTo(orderY);
		}
	}
}
