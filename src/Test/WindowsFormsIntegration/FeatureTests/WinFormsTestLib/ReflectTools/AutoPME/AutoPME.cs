// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using ReflectTools;
using WFCTestLib.Log;
using WFCTestLib.Util;
using System.IO;

namespace ReflectTools.AutoPME
{
    // <doc>
    // <desc>
    //  This class is a test engine used to test object-oriented classes. The
    //  object heirarchy looks something like this:
    //              Class to test:          AutoPME test class:
    //      ---------------------------------------------------
    //              Form                    XForm
    //      extends ContainerControl        XContainerControl
    //      extends ScrollableControl       XScrollableControl
    //      extends RichControl             XRichControl            *
    //      extends Control                 XControl                *
    //      extends MarshalByValueComponent XComponent              *
    //      extends Object                  XObject                 *
    //      extends                         AutoPME                 **
    //      extends                         ReflectBase             **
    //      extends                         Form
    //      ...
    //
    //  The class on the right contains scenarios used to test each
    //  public method of the corresponding class on the left.
    //  Classes marked with a "*" are common classes and the scenarios
    //  are provided by the ReflectTools library. Classes marked with a
    //  "**" are the actual engine classes that determine which scenarios
    //  should be called and keep track of results including logging.
    // </desc>
    // <seealso class="ReflectTools.ReflectBase"/>
    // </doc>
    public abstract class AutoPME : ReflectBase
    {
        // Prefixes found on event handler add/remove methods.  AutoPME does not count
        // event handler add/remove methods toward the required scenario count.
        //
        private const string AddHandlerPrefix = "add_";
        private const string RemoveHandlerPrefix = "remove_";
        private const string GetPropertyPrefix = "get_";
        private const string SetPropertyPrefix = "set_";
		private const string NoBaseRunFlagName = "AutoPME: NoBase";

        // <doc>
        // <desc>
        //   Determines if the object being tested is added to the form before executing scenarios.
        // </desc>
        // </doc>
        protected static bool AddControlToForm = true ;

        // <doc>
        // <desc>
        //   The group of scenarios we expected to see but didn't.
        // </desc>
        // </doc>
        private static Hashtable s_missingScenariosInGroup = new Hashtable();

        // <doc>
        // <desc>
        //   The group of scenarios we didn't expect to see but did.
        // </desc>
        // </doc>
        private static Hashtable s_extraScenariosInGroup = new Hashtable();

		//
		// We set this to true if the NoBase run flag is present.  We only use
		// this variable in InitTest to output some debug info.
		//
		private bool _noBaseRunFlagDetected = false;

        //
        // If true, base-class tests will be excluded.  Only the tests on the target class
        // will be run.
        //
        private bool _noBase = false;

        //
        // If true, only the autotest scenarios will be run.
        //
        private bool _autoTestOnly = false;

        //
        // If true, do not consider missing scenarios as failures.
        //
        private bool _ignoreMissing = false;

		//
		// If true, ignore NoBase run flag--i.e. even if it's present we still want
		// to run all scenarios.
		//
		private bool _ignoreNoBaseRunFlag = false;

        //
        // The number of scenarios from the current group that we've excluded.  Used
        // to keep total counts consistent between pre-handle mode and normal mode runs.
        //
        private int _numExcludedScenarios = 0;

		// If set to true, we don't make sure the tested method is on the stack of SecurityExceptions
		// This is reset to false in BeforeScenario().
		private bool _ignoreStackForCurrentScenario = false;

        // <doc>
        // <desc>
        //   Constructs an AutoPME object
        // </desc>
        // </doc>
        protected AutoPME(String[] args) : base(args) {}

        //
        // We initialize the size and location of the form to random values.  This is a bit tricky
        // since the screen resolution can cause potential failures when part of a form is off-screen.
        // We do a little hackery to try to allieviate this problem.
        //
        protected override void InitTest(TParams p) {
			base.InitTest(p);

            Size maxSize = new Size(1024, 768);         // Max size of the form
            double maxOffScreen = 1.0 / 8.0;            // Maximum portion of the form that's off-screen.
            double minOnScreen = 1.0 - maxOffScreen;    // Minimum portion of the form that's on-screen.

            // We output the screen resolution and desktop area so if necessary, a test owner can
            // exactly reproduce the conditions under which their test ran.
            Rectangle screen = SystemInformation.WorkingArea;

	    if (_noBaseRunFlagDetected)
	    {
		p.log.WriteLine();
		p.log.WriteLine("*****NOTE: \"" + NoBaseRunFlagName + "\" run flag detected; will run with NoBase option on*****");
		p.log.WriteLine();
	    }
            

	    p.log.WriteLine("Screen resolution:          " + SystemInformation.PrimaryMonitorSize);
            p.log.WriteLine("Desktop working area:       " + SystemInformation.WorkingArea);

            // Constant max size so at least the form size will remain the same when run with the
            // same random seed, regardless of resolution.
            this.Size = p.ru.GetSize(100, maxSize.Width, 100, maxSize.Height);

            // Pick a starting location such that most of the form is visible.  Starting location
            // is dependent on resolution, but the algorithm we use here is to maintain the same
            // distance from the lower-right corner of the screen.  Thus, theoretically, in most
            // resolutions, you'll get the same part of the form on-screen or off-screen.
            int dx = p.ru.GetRange((int)(minOnScreen * Size.Width), maxSize.Width);    // dist from right edge
            int dy = p.ru.GetRange((int)(minOnScreen * Size.Height), maxSize.Height);  // dist from bottom of screen

            // Make sure it's not too far offscreen too the left or up.
            int x = Math.Max(screen.Width - dx, (int)(-maxOffScreen * Size.Width));
            int y = Math.Max(screen.Height - dy, 0);   // Min is 0 since we don't want the titlebar offscreen.

            this.DesktopBounds = new Rectangle(new Point(x, y), Size);
            p.log.WriteLine("Form desktop bounds set to: " + this.DesktopBounds);

            this.Text = "[ Started at " + DateTime.Now.ToString(null, null) + " ] - " + Class.FullName;
        }

        //
        // If NoBase is true, scenario groups on the target class's base classes will be excluded
        // from execution.  Default is false.
        //
        protected bool NoBase {
            get {
                return _noBase && ! IgnoreNoBaseRunFlag;
            }
            set {
				if (!value)
					_noBaseRunFlagDetected = false;

				_noBase = value;
            }
        }

        //
        // If AutoTestOnly is true, only AutoTest scenarios will be executed.
        //
        protected bool AutoTestOnly {
            get {
                return _autoTestOnly;
            }
            set {
                _autoTestOnly = value;
            }
        }

        //
        // If true, do not consider missing scenarios as failures.
        //
        protected bool IgnoreMissing {
            get {
                return _ignoreMissing;
            }
            set {
                _ignoreMissing = value;
            }
        }

		protected bool IgnoreNoBaseRunFlag
		{
			get { return _ignoreNoBaseRunFlag; }
			set { _ignoreNoBaseRunFlag = value; }
		}

        protected override void CheckEnvironmentOptions() {
            base.CheckEnvironmentOptions();

            LibSecurity.Environment.Assert();
            string optionsEnvVar = Environment.GetEnvironmentVariable("AutoPMEOptions");
            CodeAccessPermission.RevertAssert();

            if (optionsEnvVar != null)
            {
                string[] options = optionsEnvVar.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries); 

                foreach (string option in options)
                    CommandLineParameters.Add(option);
            }
		}
		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		protected void QuickExit()
		{
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		protected void NotifyAlreadyinVB6()
		{
			const string AutoPMEOptions = "AutoPMEOptions";
			string currentEnv = Environment.GetEnvironmentVariable(AutoPMEOptions);
			if (null != currentEnv)
			{
				currentEnv = currentEnv.Replace("/RunInVB6", "");
				Environment.SetEnvironmentVariable(AutoPMEOptions, currentEnv);
			}
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		private int StartAndWaitForProcess(string exeName, string arguments)
		{
			Process p = Process.Start(exeName, arguments);
			p.WaitForExit();
			return p.ExitCode;
		}

		private void RunVB6Loop()
		{
			NotifyAlreadyinVB6();
			StartAndWaitForProcess("VB6Stub.exe", Assembly.GetEntryAssembly().Location);
			QuickExit();
		}

		// <doc>
        // <desc>
        //  Process any AutoPME command line parameters here.
        // </desc>
        // </doc>
        protected override void ProcessCommandLineParameters() {
            ArrayList paramsToRemove = new ArrayList();

            foreach ( string arg in CommandLineParameters ) {
                string argUpper = arg.ToUpper(System.Globalization.CultureInfo.InvariantCulture);

				if (arg.ToUpper().StartsWith("/RUNINVB6"))
				{
					RunVB6Loop();
				}

				if ( argUpper.StartsWith("/NOBASE") ) {
                    NoBase = true;
                    paramsToRemove.Add(arg);
                }
                else if ( argUpper.StartsWith("/AUTOTESTONLY") ) {
                    AutoTestOnly = true;
                    paramsToRemove.Add(arg);
                }
                else if ( argUpper.StartsWith("/IGNOREMISSING") ) {
                    IgnoreMissing = true;
                    paramsToRemove.Add(arg);
                }
				else if (argUpper.StartsWith("/IGNORENOBASEFLAG"))
				{
					IgnoreNoBaseRunFlag = true;
					paramsToRemove.Add(arg);
				}
                else if (argUpper.StartsWith("/STACKCHECK"))
                {
                    StackCheck = true;
                    paramsToRemove.Add(arg);
				}
				else if (argUpper.StartsWith("/SCENARIOPICKER"))
				{
					IgnoreMissing = true;
					//Don't remove the parameter because we want ReflectBase to pick it up.
				}

            }

            foreach ( string arg in paramsToRemove )
                CommandLineParameters.Remove(arg);

            base.ProcessCommandLineParameters();
        }

        //
        // Print additional AutoPME command-line switches.
        //
        protected override void PrintHelp() {
            base.PrintHelp();
            Console.WriteLine();
            Console.WriteLine("AutoPME command-line parameters:");
            Console.WriteLine("  /nobase            Do not run base class scenario groups");
            Console.WriteLine("  /autotestonly      Only execute AutoTest scenarios");
            Console.WriteLine("  /ignoremissing     Do not consider missing scenarios as failures");
			Console.WriteLine("  /ignorenobaseflag  Do not exclude base scenarios even if run flag present");
            Console.WriteLine("  /stackcheck        Check SecurityException stacks for current tested method");
        }

        protected override void LogCommandLineParameters() {
            base.LogCommandLineParameters();

            if ( NoBase != false )
                LogCommandLineParameter("nobase", NoBase.ToString());

            if ( AutoTestOnly != false )
                LogCommandLineParameter("autotestonly", AutoTestOnly.ToString());

            if ( IgnoreMissing != false )
                LogCommandLineParameter("ignoremissing", IgnoreMissing.ToString());

			if (IgnoreNoBaseRunFlag != false)
				LogCommandLineParameter("ignorenobaseflag", IgnoreNoBaseRunFlag.ToString());

            if (StackCheck != false)
                LogCommandLineParameter("stackcheck", StackCheck.ToString());
        }

        // <doc>
        // <desc>
        //  The type of the object we are testing. Cool tests can use the
        //  typeof() operator. VB tests will need to call GetType() on an
        //  instance of that class (for now).
        // </desc>
        // </doc>
        protected abstract Type Class { get; }

		/// <summary>
		/// Returns p.target casted to type T.  Can be used as a replacement to the usual
		/// GetFoo() methods we have been defining in our AutoPME tests.
		/// </summary>
		/// <returns>p.target casted to type T.</returns>
		protected T GetTarget<T>(TParams p) {
			if (p.target is T)
				return (T)p.target;
			else
				throw new InvalidCastException(string.Format("p.target was type {0} instead of expected type {1}", p.target.GetType().ToString(), typeof(T).ToString()));
		}

        private string GetMethodSignature(MethodBase m) {
            return m.Name + GetParameterList(m);
        }

        // <doc>
        // <desc>
        //  Allows a Scenario to tell the AutoPME engine that it is overriding
        //  a Scenario even though the method involved is not implemented at
        //  that "level" of the test class.
        // </desc>
        // <param term="scenario">
        //  The scenario that is to override the base implementation.
        // </param>
        // </doc>
        protected void OverrideScenario(MethodBase scenario)
        {
            for (int i = 0; i < tests.Count; i++)
            {
                ScenarioGroup sg = (ScenarioGroup)tests[i];
                ArrayList missing = (ArrayList)s_missingScenariosInGroup[sg];
                for (int j = 0; j < missing.Count; j++)
                {
                    if (MatchScenarioToMethod(scenario, (MethodInfo)missing[j]))
                    {
                        log.WriteTag("OverrideScenario", true, new LogAttribute[] {
                            new LogAttribute("group", sg.Name),
                            new LogAttribute("scenario", GetMethodSignature(scenario))
                        });

                        missing.Remove(missing[j]);
                        break;      // move on to next scenario group
                    }
                }
            }
        }

        // <doc>
        // <desc>
        //   If TParams.target is a decendant of Control, add it to the form
        //   and give it a random size and location within the form's client area.
        // </desc>
        // <param term="p">
        //   The TParams object that contains the target control to add to the form.
        // </param>
        // <retvalue>
        //   Returns true if the target is a Control, false otherwise.
        // </retvalue>
        // </doc>
        // SECURITY
        protected bool AddObjectToForm(TParams p)
        {
			if (!AddControlToForm || !(p.target is Control) )
				return false;

            Control c = (Control)p.target;

            if ( SafeMethods.GetParent(c) == null ) {
                // figure out a random location for this control in the client area
                Point pLocation = c.Location;
                pLocation = p.ru.GetPoint(new Point(this.ClientSize));

                // If we're running without AllWindows permission, there will be a security
                // bubble that can ---- up painting scenarios if the control is under it.
                if ( !Utilities.HavePermission(LibSecurity.AllWindows) && pLocation.Y < 200 ) {
                    pLocation.Y = 200;
                    this.Height = Math.Max(this.Height, 250);
                    p.log.WriteLine("SECURITY: Adjusting control location and form size to avoid security bubble.");
                }

                p.log.WriteLine("RANDOM=setting location to: " + pLocation.ToString());
                c.Location = (pLocation);

                //set random size also
                Size size = c.Size;
                size = p.ru.GetSize(this.ClientSize);
                p.log.WriteLine("RANDOM=setting size to: " + size.ToString());
                c.Size = size;

                this.Controls.Add(c);
                this.Invalidate();
            }
			return true;
        }

        // <doc>
        // <desc>
        //  This method is called after a group of scenarios is executed. It
        //  overrides that base implementation provided by ReflectBase. Here
        //  we determine if we called a scenario for each public method on
        //  the class. If we find a method that did not have a corresponding
        //  scenario called we print out a list of all missing methods
        //  including method signature. If all public methods had
        //  corresponding scenarios then an attaboy is logged instead.
        //
        //  If IgnoreMissing is true, we don't consider missing scenarios
        //  to be failures.
        // </desc>
        // <param term="p">
        //  The TParams object we have associated with this testcase
        // </param>
        // <param term="g">
        //  The ScenarioGroup we just finished executing through
        // </param>
        // <param term="total">
        //  The total number of scenarios that were executed in this group
        // </param>
        // <param term="fail">
        //  The number of scenarios that failed in this group
        // </param>
        // <retvalue>
        //  True to continue executing subsequent ScenarioGroups; false otherwise.
        //  This implementation always returns true unless StopOnFailureInGroup is true.
        // </retvalue>
        // </doc>
        protected override bool AfterScenarioGroup(TParams p, ScenarioGroup g, int total, int fail)
        {
            bool fullyImplemented = true;           // All scenarios are implemented
            ArrayList missing = (ArrayList)s_missingScenariosInGroup[g];

			if (!IgnoreMissing)
			{
				fail += missing.Count;
				total += missing.Count;
			}
			
            p.log.WriteLine();
            p.log.WriteTag("ClassResults", false, new LogAttribute[] {
                new LogAttribute("type", (fail==0)?"Pass":"Fail"),
                new LogAttribute("total", total.ToString()),
                new LogAttribute("fail", fail.ToString())
            });

            // In AutoTest mode we don't want to gripe about missing scenarios
            if ( AutoTestOnly )
                return true;

            if (missing == null || missing.Count == 0)
                p.log.WriteLine("*****Good work, 100% implemented*****");
            else
            {
                fullyImplemented = false;
                p.log.WriteLine("*****The Following methods are not covered*****");

                if ( !IgnoreMissing ) {
                    p.log.TestResults.FailCount += missing.Count;

                    if ( p.log.TestResults.Comments == null )
                        p.log.TestResults.Comments = "Missing some test scenarios on class: " + g.Name;
                }

                for (int i = 0; i < missing.Count; i++)
                {
                    MethodInfo mi = (MethodInfo)missing[i];
                    p.log.WriteTag("MissingScenario", true, new LogAttribute("name", GetMethodSignature(mi)));
                }
            }

            // Output extra methods--these don't count as failures but is useful to know anyway
            ArrayList extra = (ArrayList)s_extraScenariosInGroup[g];
            p.log.WriteLine();

            if ( extra == null || extra.Count == 0 )
                p.log.WriteLine("*****No extra scenarios*****");
            else {
                p.log.WriteLine("*****The following methods are extra scenarios*****");

                for ( int i = 0; i < extra.Count; i++ ) {
                    MethodInfo mi = (MethodInfo)extra[i];
                    p.log.WriteTag("ExtraScenario", true, new LogAttribute("name", GetMethodSignature(mi)));
                }
            }

            p.log.CloseTag("ClassResults");

            if ( PreHandleMode || NoBase ) {
                p.log.TestResults.PassCount += _numExcludedScenarios;
                _numExcludedScenarios = 0;
            }

            if ( StopOnFailureInGroup )
                return fullyImplemented;        // We stop if there are missing methods
            else
                return true;
        }

		protected override bool BeforeScenario(TParams p, MethodInfo scenario) {
			_ignoreStackForCurrentScenario = false;
			return base.BeforeScenario(p, scenario);
		}

		protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result) {
			base.AfterScenario(p, scenario, result);
		}

		//
		// We override VerifySecurityException so we can make sure the current scenario name
		// (i.e. the method we're testing) is on the stack of the exception being thrown.
		// This is so we are sure there are no false positives, where someone does something
		// like this:
		//
		//     BeginSecurityCheck(LibSecurity.UnrestrictedFileIO);
		//     x.TestFoo(new FileStream("bar.txt"));
		//     EndSecurityCheck();
		//
		// where the FileStream ctor throws the expected SecurityException instead of TestFoo(),
		// which is what we're testing.
		//
		[Scenario(false)]
		protected override ScenarioResult VerifySecurityException(TParams p, SecurityException se) 
		{
			bool origStackCheck = StackCheck;

			if (_ignoreStackForCurrentScenario) { StackCheck = false; }
			else if (securityCheckExpectedMethod != null) { securityCheckExpectedMethod = currentScenario; }

			ScenarioResult result = base.VerifySecurityException(p, se);

			if (_ignoreStackForCurrentScenario) { StackCheck = origStackCheck; }

			return result;
		}

		/// <summary>
		/// Compares a method name with the corresponding scenario method name.  We need this special logic
		/// for private interface impls, where method.Name returns something like "IFoo.Bar". so we need
		/// to just make sure everything after the last dot matches the scenario name.
		/// </summary>
		/// <param name="methodName"></param>
		/// <param name="scenarioMethodName"></param>
		/// <returns></returns>
		private bool CompareMethodNames(string methodName, string scenarioMethodName) {
			return methodName.Substring(methodName.LastIndexOf(".") + 1) == scenarioMethodName;
		}

		/// <summary>
		/// Returns true if declaringType is this type (i.e. Class property) or this type is
		/// a subclass of declaringType.
		/// </summary>
		/// <param name="declaringType"></param>
		/// <returns></returns>
		private bool MatchDeclaringType(Type declaringType) {
			return declaringType == Class || Class.IsSubclassOf(declaringType);
		}

		/// <summary>
		/// AutoPME requires that the current method being tested appears on the stack of the
		/// security exception.  In some cases, this may be impossible (e.g. declarative demands
		/// don't appear on the stack since they're thrown at the caller's JIT time).  This
		/// overload of BeginSecurityCheck allows you to specify that AutoPME should ignore
		/// the stack of the SecurityException.
		/// </summary>
		/// <param name="p">Permission required by the tested method.</param>
		/// <param name="ignoreStack">True if AutoPME should ignore the stack.</param>
		/// <param name="reason">Reason why the stack should be ignored.</param>
		protected void BeginSecurityCheck(CodeAccessPermission p, bool ignoreStack, string reason) {
			if (ignoreStack) {
				if (string.IsNullOrEmpty(reason))
					throw new ReflectBaseException("Parameter \"reason\" cannot be null or empty if \"ignoreStack\" is true");
				else
					scenarioParams.log.WriteLine("SECURITY: Ignoring stack check.  Reason: \"" + reason + "\"");
			}

			BeginSecurityCheck(p);
			_ignoreStackForCurrentScenario = ignoreStack;
		}

		protected void BeginSecurityCheck(CodeAccessPermission[] p, bool ignoreStack, string reason) {
			if (ignoreStack) {
				if (string.IsNullOrEmpty(reason))
					throw new ReflectBaseException("Parameter \"reason\" cannot be null or empty if \"ignoreStack\" is true");
				else
					scenarioParams.log.WriteLine("SECURITY: Ignoring stack check.  Reason: \"" + reason + "\"");
			}

			BeginSecurityCheck(p);
			_ignoreStackForCurrentScenario = ignoreStack;
		}

		//
		// (12/10/03) We're currently grandfathering tests which use AddRequiredPermissions()
		// and letting them skip the stack check. We'll probably revisit this at a later date.
		//
		[
		Obsolete("AddRequiredPermission() is obsolete.  Please use BeginSecurityCheck() instead"),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		protected override void AddRequiredPermission(CodeAccessPermission p) {
			BeginSecurityCheck(p, true, "AddRequiredPermission()");
		}


        // <doc>
        // <desc>
        //   Subclasses of AutoPME must implement CreateObject. AutoPME.TestEngine will
        //   fill in the "target" field with the value this function returns.
        // </desc>
        // <param term="p">
        //   The TParams object the testing class will use.
        // </param>
        // <retvalue>
        //   The object to actually test against.
        // </retvalue>
        // </doc>
		protected abstract Object CreateObject(TParams p);

        // <doc>
        // <desc>
        //  Groups the methods on the class to test against by the object where
        //  the method is actually implemented. This method overrides the default
        //  implementation provided by ReflectBase.
        // </desc>
        // <param term="scenarios">
        //  The list of scenarios implemented by the testcase
        // </param>
        // <retvalue>
        //  A List of ScenarioGroups.
        // </retvalue>
        // <seealso class="ReflectTools.ScenarioGroup"/>
        // </doc>
        protected override ArrayList CreateScenarioGroups(MethodInfo[] scenarios)
        {
            // group the methods on the target class by the class they are implemented on
            MethodInfo[] methodsOnTarget = this.Class.GetMethods(BindingFlags.Instance|BindingFlags.Public);
            Hashtable htTarget = GroupMethodsByDeclaringType(methodsOnTarget);

            // group the methods on the test class by the Xclass they are implemented on
            Hashtable htTest = GroupMethodsByDeclaringType(scenarios);

            Type currentTestType = this.GetType();
            Type currentType = this.Class;
            ArrayList ret = new ArrayList();

            // Loop up the inheritance chain of the target class and the test class.  It's possible
            // for there to be more test classes than there are target classes.
            while (currentTestType != null) {

                // If we're in AutoTestOnly mode, skip all the classes except for AutoTest.
                if ( AutoTestOnly && currentTestType != typeof(AutoTest) ) {
                    if ( currentType != null )
                        currentType = currentType.BaseType;

                    currentTestType = currentTestType.BaseType; 
                    continue;
                }

                ArrayList al = (ArrayList)htTest[currentTestType];

                if (al != null)
                {
                    ScenarioGroup sg = new ScenarioGroup();
                    ArrayList expected;

                    // currentType can be null if we have test classes that don't have a corresponding
                    // target class on the target's inheritance chain.  We treat all these scenarios as
                    // extra scenarios.
                    if ( currentType == null )
                        expected = new ArrayList();
                    else {
                        // It's possible a child class overrides ALL methods on its parent
                        // (e.g. Rectangle), so it never got filled in GroupMethodsByDeclaringType
                        if ( htTarget[currentType] == null )
                            expected = new ArrayList();
                        else
                            expected = (ArrayList)htTarget[currentType];
                    }
                        
                    InitMissingScenariosForGroup(sg, scenarios, expected);
                    InitExtraScenariosForGroup(sg, al, expected);

                    sg.Name = currentTestType.Name;
                    sg.Scenarios = (MethodInfo[])al.ToArray(typeof(MethodInfo));
                    ret.Add(sg);
                }

// We now create all scenario groups even if NoBase is specified.  We just exclude all the
// scenarios in the base classes so the total count is the same (and MadDog doesn't complain
// about missing scenarios.
//                if ( NoBase )   // Stop after creating the first scenario group of NoBase flag was set
//                    break;

                if ( currentType != null )
                    currentType = currentType.BaseType;

                currentTestType = currentTestType.BaseType;
            }

			return ret;
        }

		private bool IsExcludedMethod(MethodInfo mi) {
            bool excluded = mi.Name.StartsWith(AddHandlerPrefix) || mi.Name.StartsWith(RemoveHandlerPrefix) ||
                            mi.Name.StartsWith("ShouldSerialize");

            // In pre-handle mode, we don't expect methods to be run.
            if ( PreHandleMode ) {
                excluded = excluded || !(mi.Name.StartsWith(GetPropertyPrefix) || mi.Name.StartsWith(SetPropertyPrefix));
                object[] attrs = mi.GetCustomAttributes(typeof(PreHandleScenarioAttribute), false);

                if ( attrs != null && attrs.Length > 0 ) {
                    if ( attrs.Length > 1 )
                        throw new Exception("PreHandleScenarioAttribute is defined more than once on " + mi.Name);

                    if ( excluded ) {
                        // If it's excluded so far, check if it has PreHandleScenarioAttribute(true)
                        if ( ((PreHandleScenarioAttribute)attrs[0]).IsPreHandleScenario == true )
                            excluded = false;
                    }
                    else {
                        // If it's not excluded, check if it has PreHandleScenarioAttribute(false)
                        if ( ((PreHandleScenarioAttribute)attrs[0]).IsPreHandleScenario == false )
                            excluded = true;
                    }
                }
            }

            return excluded;
        }

        //
        // Go through the scenario list and remove excluded scenarios if we're in pre-handle mode.
        // A scenario is excluded if it's not a property get/set, and it doesn't have a
        // PreHandleScenario(true) attribute on it, or if it is a property and has
        // PreHandleScenario(false) on it.
        //
        protected override bool BeforeScenarioGroup(TParams p, ScenarioGroup g) {
			if (NoBase && g.Name != this.GetType().Name) {
				// NoBase mode, and this is a base class scenario group.  We need to exclude all these scenarios.
				foreach (MethodInfo mi in g.Scenarios) {
					_numExcludedScenarios++;
					LogExcludedMethod(p, mi);
				}

				g.Scenarios = new MethodInfo[0];
			}

			if ( PreHandleMode ) {
                ArrayList methods = new ArrayList(g.Scenarios.Length);

                foreach ( MethodInfo mi in g.Scenarios ) {
                    // if it's not excluded, we'll keep the method, else if it has the
                    // OverrideScenario attribute on it, consider it overridden (and
                    // thus, not missing in the base class).
                    if ( !IsExcludedMethod(mi) )
                        methods.Add(mi);
                    else {
                        _numExcludedScenarios++;
						LogExcludedMethod(p, mi);
					}
                }

                g.Scenarios = (MethodInfo[])methods.ToArray(typeof(MethodInfo));
            }

            return base.BeforeScenarioGroup(p, g);
		}

		private void LogExcludedMethod(TParams p, MethodInfo mi)
		{
			if (!mi.IsDefined(typeof(OverrideScenarioAttribute), false))
				p.log.WriteTag("ExcludedScenario", true, new LogAttribute("name", GetMethodSignature(mi)));
			else
			{
				// This method is an overridden scenario
				p.log.WriteTag("ExcludedScenario", false, new LogAttribute("name", GetMethodSignature(mi)));
				OverrideScenario(mi);
				p.log.CloseTag("ExcludedScenario");
			}
		}

		// <doc>
        // <desc>
        //  Determines the list of methods in the list of expected scenarios do not
        //  have a corresponding scenario. This list is added to the missingScenariosInGroup
        //  Hashtable under the key of the ScenarioGroup where the scenarios were expected.
        // </desc>
        // <param term="key">
        //  The key to the missingScenariosInGroup Hashtable under which the list is stored.
        // </param>
        // <param term="scenarios">
        //  The list of scenarios that are actually implemented by the testcase.
        // </param>
        // <param term="expected">
        //  The list of scenarios that are expected to be implemented by the testcase
        // </param>
        // <seealso member="missingScenariosInGroup"/>
        // <seealso member="MatchScenarioToMethod"/>
        // </doc>
        private void InitMissingScenariosForGroup(ScenarioGroup key, MethodInfo[] scenarios, ArrayList expected)
        {
			// KEVINTAO 10/17/03: Modified to search the entire scenario list for an expected test.
			//    This essentially eliminates the need for OverrideScenario.  Scott and I couldn't
			//    think of a reason why you'd want a failure in a base class where the scenario was
			//    implemented in a child class.
            ArrayList missing = new ArrayList(expected);

            for (int i=0; i<expected.Count; i++)
            {
                MethodInfo miExp = (MethodInfo)expected[i];

                if ( miExp.Name.StartsWith(AddHandlerPrefix) || miExp.Name.StartsWith(RemoveHandlerPrefix) ||
                     miExp.Name.StartsWith("ShouldSerialize") )
                    missing.Remove(miExp);
                else {
                    for (int j=0; j<scenarios.Length; j++) {
                        if (MatchScenarioToMethod(scenarios[j], miExp)) {
                            missing.Remove(miExp);
                            break;
                        }
                    }
                }
            }

			s_missingScenariosInGroup[key] = missing;
        }

        // <doc>
        // <desc>
        //  Creates the list of methods that are in the AutoPME test which are not required
        //  by the class being tested (i.e. extra test methods).  This is useful for easily
        //  finding methods which have been removed or renamed in the target class.
        //
        //  This list is added to the extraScenariosInGroup Hashtable under the key of the
        //  ScenarioGroup where the scenarios were expected.
        // </desc>
        // <param term="key">
        //  The key to the extraScenariosInGroup Hashtable under which the list is stored.
        // </param>
        // <param term="implemented">
        //  The list of scenarios that are actually implemented by the testcase.
        // </param>
        // <param term="expected">
        //  The list of scenarios that are expected to be implemented by the testcase
        // </param>
        // <seealso member="extraScenariosInGroup"/>
        // <seealso member="MatchScenarioToMethod"/>
        // </doc>
        private void InitExtraScenariosForGroup(ScenarioGroup key, ArrayList implemented, ArrayList expected) {
            ArrayList extra = new ArrayList(implemented);

            for (int i = 0; i < implemented.Count; i++)
            {
                MethodInfo miImp = (MethodInfo)implemented[i];

                for (int j = 0; j < expected.Count; j++) {
                    if (MatchScenarioToMethod(miImp, (MethodInfo)expected[j]))
                        extra.Remove(miImp);
                }
            }

            s_extraScenariosInGroup[key] = extra;
        }

        // <doc>
        // <desc>
        //  Groups the methods provided by the Type of the object where
        //  the method is actually implemented.
        // </desc>
        // <param term="mia">
        //  The array of MethodInfos that need to be grouped.
        // </param>
        // <retvalue>
        //  A Hashtable containing lists of MethodInfos keyed by the
        //  Type of the object where the methods are implemented.
        // </retvalue>
        // </doc>
        private Hashtable GroupMethodsByDeclaringType(MethodInfo[] mia)
        {
            Hashtable ht = new Hashtable();

            for (int i=0; i<mia.Length; i++)
            {
                ArrayList al = (ArrayList)ht[mia[i].DeclaringType];
                if (al == null)
                {
                    al = new ArrayList();
                    ht[mia[i].DeclaringType] = al;
                }
                al.Add(mia[i]);
            }

            return ht;
        }

        // <doc>
        // <desc>
        //  Determines if the specified Object is a Control and if if is also
        //  a child of the testcase form.
        //
        // </desc>
        // <param term="o">
        //  The object to determine if it is a child of the test form or not
        // </param>
        // <retvalue>
        //  True if the control is a child of the test form; false otherwise.
        // </retvalue>
        // </doc>
        private bool IsChildOfThisForm(Object o)
        {
            if (o is Control)
            {
                Control c = (Control)o;
                int count = this.Controls.Count;

                for (int n = 0; n < count; n++)
                    if (this.Controls[n] == c)
                        return true;
            }

            return false;
        }

        // <doc>
        // <desc>
        //  Compares two MethodInfo objects and returns true if the implemented scenario
        //  matches the expected method.
        //  Note, the parameter signature must match as well.
        // </desc>
        // <param term="miImp">
        //  The MethodInfo of the scenario as implemented
        // </param>
        // <param term="miExp">
        //  The MethodInfo of the method we are trying to match
        // </param>
        // <retvalue>
        //  True if the implemented method matches the expected method; false otherwise.
        // </retvalue>
        // <seealso member="CompareArgumentLists"/>
        // </doc>
        private bool MatchScenarioToMethod(MethodBase miImp, MethodBase miExp)
        {
            String testMethodName = miImp.Name;
            String methodName = miExp.Name;

			if (CompareMethodNames(methodName, testMethodName))
                return CompareArgumentLists(miImp, miExp);
            return false;
        }

        // <doc>
        // <desc>
        //  Compares the parameter lists for two methods and returns true if
        //  they are the same. The first parameter in the list specified by miImp
        //  is ignored.
        // </desc>
        // <param term="miImp">
        //  The MethodInfo of the implemented scenario.
        // </param>
        // <param term="miExp">
        //  The MethodInfo of the method we are trying to match
        // </param>
        // <retvalue>
        //  True if the parameter lists are the same; false otherwise.
        // </retvalue>
        // </doc>
        private bool CompareArgumentLists(MethodBase miImp, MethodBase miExp)
        {
            ParameterInfo[] piImp = miImp.GetParameters();
            ParameterInfo[] piExp = miExp.GetParameters();

            // the test has an extra parameter for the TParams
            if (piImp.Length - 1 != piExp.Length)
            {
                //log.WriteLine("1 " + GetParameterList(miImp) + " <> " + GetParameterList(miExp));
                return false;
            }

            for (int i = 0; i < piExp.Length; i++)
            {
                if (piImp[i+1].ParameterType != piExp[i].ParameterType)
                {
                    //log.WriteLine("2 " + GetParameterList(miImp) + " <> " + GetParameterList(miExp));
                    return false;
                }
            }

            return true;
        }

        // <doc>
        // <desc>
        //  Forces the target testing object to be created and if appropriate
        //  added to the testing form. Once done, the base implementation of
        //  the TestEngine is called.
        // </desc>
        // <param term="p">
        //  The test parameters for this testcase.
        // </param>
        // <seealso class="ReflectBase" member="TestEngine"/>
        // </doc>
        protected override void TestEngine(TParams p)
        {
            try
            {
                //bool bPass = true;
                p.target = CreateObject(p);

                // create our test object and store it
                if((! (p.target is Form)) && AddControlToForm)
                    AddObjectToForm(p);

                base.TestEngine(p);
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

                TestIsDone(p);
            }
        }

        // Overridden to check for the OverrideScenario attribute before running a test.
        [Scenario(false)]
        protected override ScenarioResult InvokeMethod(MethodInfo mi, TParams p) {
            if ( mi.IsDefined(typeof(OverrideScenarioAttribute), false) )
                OverrideScenario(mi);

            ScenarioResult retVal = base.InvokeMethod(mi, p);

            // If we're in pre-handle mode and the control's handle has been created, dispose of it,
            // create a new one, and add it to the form.
            //
            if (PreHandleMode && p.target != null && p.target is Control && ((Control)p.target).IsHandleCreated) {
                p.log.WriteLine("InvokeMethod: PreHandleMode and p.target's handle was created.  Disposing and recreating using CreateObject()");

                ((Control)p.target).Dispose();
                p.target = CreateObject(p);

                if (!(p.target is Form) && AddControlToForm)
                    AddObjectToForm(p);
            }
            
            return retVal;
        }
    }

    // This is an alternate way of overriding a scenario.  You can use this instead of the
    // OverrideScenario(MethodBase m) method.
    [AttributeUsage(AttributeTargets.Method, Inherited=false, AllowMultiple=false)]
    public class OverrideScenarioAttribute : Attribute { }
}
