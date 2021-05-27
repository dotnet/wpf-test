// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------
//  Description: Default test logger which just logs to the console (e.g. harness-less logger).
//  Creator: derekme
//  Date Created: 9/8/05
//---------------------------------------------------------------------

using Annotations.Test.Framework.Internal;
using Microsoft.Test.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Threading;

namespace Annotations.Test.Framework
{
	/// <summary>
	/// A testing framework that subclasses can leverage to develop shorter, clearer, and simpler to understand tests.
	/// </summary>
	/// <remarks>
	/// Pass/Failed status is controled by throwing TestPassedException or TestFailedExceptions.  This is done instead of
	/// synchronously shutting down the app so that it is possible to run multiple tests cases in a single process.
	/// </remarks>
	public class TestSuite
	{
		#region Constructors

		/// <summary>
		/// Constructor for a TestSuite.
		/// </summary>
		public TestSuite() 
		{
            // Intialize Current.
            _current = this;
		}

		#endregion Constructors

        #region Static Members

        /// <summary>
        /// Call PrintUsage on given Suite.
        /// </summary>
        /// <param name="suite"></param>
        public static void PrintUsage(TestSuite suite)
        {
            suite.PrintUsage();
        }

        /// <summary>
        /// The TestSuite representing the current Application context.
        /// </summary>
        public static TestSuite Current 
        {
            get
            {
                return _current;
            }
        }
        private static TestSuite _current = null;        

        #endregion

        #region Public Methods

        /// <summary>
        /// Run all TestSuites, TestCases, and TestVariations encapsulated
        /// by this TestSuite.
        /// </summary>
        /// <remarks>
        /// Running may be done asynchronously and in such cases this method
        /// will return before running is complete.  Use the IsFinished and
        /// Finished event to monitor a running TestSuite.
        /// </remarks>
        /// <param name="args">Command line args</param>
        public virtual void Run(string[] args)
        {
            EnsureStart();
            InitializeArgs(args);
            
            TestSuiteRunner runner = SetupTestRunner();
            runner.Finished += OnRunFinished;
            runner.RunTests(TestCases); // 
        }        

        /// <summary>
        /// Run a specific TestCase
        /// </summary>
        /// /// <remarks>
        /// Running may be done asynchronously and in such cases this method
        /// will return before running is complete.  Use the IsFinished and
        /// Finished event to monitor a running TestSuite.
        /// </remarks>
        /// <param name="caseNumber">Name of test to run.</param>
        /// <param name="args">Full command line args.</param>
        public void Run(string caseNumber, string[] args)
        {
            EnsureStart();
            InitializeArgs(args);

            // Ensure case number.
            _caseNumber = caseNumber;
            if (string.IsNullOrEmpty(CaseNumber))
            {
                PrintUsage();
                throw new InvalidOperationException("Incorrect usage, must specify a test case number to run.");
            }

            TestCase test = CreateTestCase(CaseNumber);
            // If test doesn't exist in current Suite, check other suites.
            if (test.TestMethod == null && Suites.Count > 0)
            {
                TestSuite suite = FindTestSuiteWithTestId(CaseNumber);
                if (suite == null)
                    throw new ArgumentException("Couldn't find TestSuite with definition for TestId '" + CaseNumber + "'.");
                suite.Finished += OnRunFinished;
                suite.Run(CaseNumber, Args);
            }
            else
            {
                TestSuiteRunner runner = SetupTestRunner();
                runner.Finished += OnRunFinished;
                runner.RunVariations(test.Variations);
            }
        }        

        /// <summary>
        /// Add an EventHandler with a timer to the Dispatcher queue for this TestSuite.
        /// </summary>
        /// <param name="handler">Handler called when 'delay' has expired.</param>
        /// <param name="delay">Lenght of time before 'handler' is ivoked by Dispatcher.</param>
        public void QueueTimerTask(EventHandler handler, TimeSpan delay)
        {
            DispatcherTimer timer = new DispatcherTimer(delay, DispatcherPriority.ApplicationIdle, handler, Dispatcher.CurrentDispatcher);
            timer.Start();
        }        

        /// <summary>
        /// Adds the given task to event queue in this application's context.  
        /// </summary>
        /// <exception cref="TestFailedException">If an exception occurs while queuing.</exception>
        public void queueTask(DispatcherOperationCallback task, Object arg)
        {
            BeginInvoke(task, arg);
        }
        /// <summary>
        /// Adds the given task to event queue in this application's context.  
        /// </summary>
        /// <exception cref="TestFailedException">If an exception occurs while queuing.</exception>
        public void queueTask(DispatcherOperationCallback task, Object arg, DispatcherPriority priority)
        {
            BeginInvoke(task, arg, priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="arg"></param>
        public void BeginInvoke(DispatcherOperationCallback task, Object arg)
        {
            BeginInvoke(task, arg, DispatcherPriority.Background);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="arg"></param>
        /// <param name="priority"></param>
        public void BeginInvoke(DispatcherOperationCallback task, Object arg, DispatcherPriority priority)
        {
            try
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(priority, new DispatcherOperationCallback(task), arg);
            }
            catch (Exception E)
            {
                failTest("The following exception was thrown queuing task \"" + task.ToString() + "\": " + E.ToString() + "\n");
            }
        }

        /// <summary>
        /// Asserts that 'cond' is true.  Throws a TestFailedException if false.
        /// </summary>
        /// <exception cref="TestFailedException">If condition is false.</exception>
        /// <param name="msg">Message to include in failure message.</param>
        /// <param name="cond"></param>
        public void Assert(string msg, bool cond)
        {
            if (!cond)
                throw new TestFailedException("Assertion failed: " + msg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="instance"></param>
        public void AssertNotNull(string msg, object instance)
        {
            Assert(msg, instance != null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="instance"></param>
        public void AssertNull(string msg, object instance)
        {
            Assert(msg, instance == null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool Compare(double arg1, double arg2, double tolerance)
        {
            bool result = false;
            if (double.IsNaN(arg1) || double.IsInfinity(arg1))
            {
                result = double.Equals(arg1, arg2);
            }
            else
            {
                double delta = arg1 - arg2;
                result = Math.Abs(delta) <= Math.Abs(tolerance);
            }
            return result;
        }

        /// <summary>
        /// Assert that these two doubles are within some tolerance of one another.
        /// </summary>
        public void AssertEquals(string msg, double arg1, double arg2, double tolerance)
        {
            Assert("Expected \"" + arg1 + "\" but was \"" + arg2 + "\": " + msg, Compare(arg1, arg2, tolerance));
        }

        /// <summary>
        /// Assert that these two sizes are within some tolerance of one another.
        /// </summary>
        public void AssertEquals(string msg, Size arg1, Size arg2)
        {
            printStatus(string.Format("[DPI = {0}]", Microsoft.Test.Display.Monitor.Dpi.x.ToString()));

            if (Microsoft.Test.Display.Monitor.Dpi.x == 120)
            {
                Assert("Width : Expected \"" + arg1.Width + "\" but was \"" + arg2.Width + "\": " + msg, Compare(arg1.Width, arg2.Width, 1));
                Assert("Height : Expected \"" + arg1.Height + "\" but was \"" + arg2.Height + "\": " + msg, Compare(arg1.Height, arg2.Height, 1));
            }
            else
            {
                AssertEquals("Verify Window Size.", (object)arg1, (object)arg2);
            }
        }

        /// <summary>
        /// Asserts that the given objects are equal.  Uses .Equals() method.
        /// </summary>
        /// <exception cref="TestFailedException">If given objects are not equal.</exception>
        /// <param name="msg">Message to include in failure message.</param>
        /// <param name="obj1">Expected value.</param>
        /// <param name="obj2">Value to compare.</param>
        public void AssertEquals(string msg, object obj1, object obj2)
        {
            if (!Object.Equals(obj1, obj2))
            {
                string obj1tag = (obj1 == null) ? "null" : obj1.ToString();
                string obj2tag = (obj2 == null) ? "null" : obj2.ToString();
                throw new TestFailedException("Expected \"" + obj1tag + "\" but was \"" + obj2tag + "\": " + msg);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public void AssertEquals(string msg, object[] expected, object[] actual)
        {
            AssertEquals("Expected number of elements differ: " + msg, expected.Length, actual.Length);
            AssertEquals(msg, expected.GetEnumerator(), actual.GetEnumerator());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="array"></param>
        /// <param name="list"></param>
        public void AssertEquals(string msg, object[] array, IList list)
        {
            AssertEquals("Expected number of elements differ: " + msg, array.Length, list.Count);
            AssertEquals(msg, array.GetEnumerator(), list.GetEnumerator());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="expectedObjs"></param>
        /// <param name="actualObjs"></param>
        public void AssertEquals(string msg, IEnumerator expectedObjs, IEnumerator actualObjs)
        {
            bool match = true;
            string expected = "";
            string actual = "";
            while (expectedObjs.MoveNext())
            {
                Assert("Verify actual is same size as expected.", actualObjs.MoveNext());
                if (!object.Equals(expectedObjs.Current, actualObjs.Current))
                {
                    match = false;
                }

                expected += expectedObjs.Current + ", ";
                actual += actualObjs.Current + ", ";
            }
            if (!match)
            {
                failTest(msg + " Expected '{" + expected + "}' but was '{" + actual + "}'.");
            }
        }

        /// <summary>
        /// Throw TestFailedException with given message. Indicates that test
        /// case has reached a state of failure.
        /// </summary>
        public void failTest(string msg)
        {
            throw new TestFailedException(msg);
        }

        /// <summary>
        /// Throws TestPassedException with given messsage.  Indicates that
        /// test case has passed.
        /// </summary>
        public void passTest(string msg)
        {
            throw new TestPassedException(msg);
        }

        /// <summary>
        /// Log status message.
        /// </summary>
        [LoggingSupportFunctionAttribute]
        public void printStatus(string msg)
        {
            GlobalLog.LogStatus(msg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="contents"></param>
        [LoggingSupportFunctionAttribute]
        public void LogToFile(string filename, Stream contents)
        {
            TestLog.Current.LogFile(filename, contents);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        [LoggingSupportFunctionAttribute]
        public void LogToFile(string filename)
        {
            printStatus("Logging file: " + filename);
            TestLog.Current.LogFile(filename);
        }

        #endregion Public Methods

        #region Public Properties
        
        /// <summary>
        /// Get Set of suites contained by this suite.
        /// </summary>
        public IList<TestSuite> Suites
        {
            get
            {
                return _innerSuites;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return _isFinished;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Locate argument which matches the given regular expression.
        /// </summary>
        /// <param name="args">Arguments to parse.</param>
        /// <param name="criteria">Regex to match.</param>
        /// <returns>Match object of a match or null if no parameters match.</returns>
        protected Match FindParameter(string[] args, Regex criteria)
        {
            foreach (string arg in args)
            {
                Match match = criteria.Match(arg);
                if (match.Success)
                    return match;
            }
            return null;
        }

        /// <summary>
        /// Perform a high level cleanup to ensure that Variations do not interfer with one another.
        /// </summary>
        [TestCase_Cleanup()]
        protected virtual void CleanupVariation()
        {
            // For subclasses to override.
        }

		/// <summary>
		/// Subclasses override to select and actually run code that corresponds to the current TestCase.
		/// </summary>
        /// <remarks>
        /// If this method is not overridden then it will automatically try and select
        /// a method to run using reflection.
        /// </remarks>
		protected virtual void RunTestCase()
		{
            // Deprecated!  This will be invoked via reflection to be backwards compatibile until transition is complete.
            throw new ArgumentException("No test case with id '" + CaseNumber + "' was found.");
		}      

		/// <summary>
		/// Way for TestSuite implementations to declare and document the command line arguments that they support.
		/// </summary>
		/// <remarks>
		/// - Overrides of this method should generally call base.UsageParameters() and then append new parameters.  
		/// Exceptions to this rule are if the subclass completely overrides the ProcessArgs(string [] args) method.
		/// - Subclasses should also implement UsageExamples().
		/// </remarks>
		/// <returns>List of all command line parameters that are recognized by TestSuite implementation.</returns>
		protected virtual IList<string> UsageParameters()
		{
			IList<string> usage = new List<string>();
            usage.Add("/dimension=[A,B,C] - (optional) will run test case for each dimension in comma delimited list.");
            usage.Add("[/interactive|/i] - perform setup actions then wait and allow user to interactively test.");
            usage.Add("[-k] - wait at end of test (runs a usual but upon pass or fail windows does not shutdown to allow manual inspection).");
            return usage;
		}

		/// <summary>
		/// Way for TestSuite implementations to provide usage examples of what parameters to pass in order to get
		/// certain functionality.
		/// </summary>
		/// <remarks>
		/// - Overrides of this method should generally call base.UsageExamples() and then append new examples.  
		/// Exceptions to this rule are if the subclass completely overrides the ProcessArgs(string [] args) method.
		/// - Subclasses should also implement UsageParameters().
		/// </remarks>
		/// <returns>List of examples of how to run a TestSuite in each of its supported modes.</returns>
		protected virtual IList<string> UsageExamples()
		{
			IList<string> examples = new List<string>();
            examples.Add("'XXX.exe param1 /dimension=[A,B,C]' - will run XXX with 3 variations [param1 A, param1 B, param1 C].");
            examples.Add("'XXX.exe /dimension=[A,B] /dimension=[1,2]' - will run XXX with 4 variations [A 1, A 2, B 1, B 2].");
            examples.Add("'XXX.exe case1 -k' - run 'case1' but don't close the window at the end.");
            examples.Add("'XXX.exe case1 /i' - performs 'case1's setup steps then lets user play.");
            return examples;
		}

		/// <summary>
		/// Preferred place for all TestSuite implementations to parse the command line args that they are given.
		/// </summary>
		/// <remarks>
		/// Overriders should make sure to override UsageParameters() and UsageExamples() to ensure discoverablity 
		/// and documentation of commandline args that TestSuite implementations understand.
		/// </remarks>
		/// <param name="args">list of command line args.</param>
		public virtual void ProcessArgs(string[] args)
		{
			foreach (string arg in args)
			{
                if (arg.Equals("?") || arg.Equals("/help"))
                {
                    PrintUsage();
                    break;
                }                
			}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual RunnerMode DetermineRunnerMode(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[args.Length - 1].Equals("/interactive") || args[args.Length - 1].Equals("/i"))
                    return RunnerMode.SetupOnly;
                else if (args[args.Length - 1].Equals("-k"))
                    return RunnerMode.HoldAtEnd;
            }
            return RunnerMode.Normal;
        }

        /// <summary>
        /// Create and intialize new TestSuiteRunner.
        /// </summary>
        /// <returns></returns>
        protected virtual TestSuiteRunner SetupTestRunner()
        {
            TestSuiteRunner runner = new TestSuiteRunner();
            runner.Mode = DetermineRunnerMode(Args);
            return runner;
        }        

        #endregion

        #region Protected Properties

        /// <summary>
        /// Arguments for Suite to process.
        /// </summary>
        public string[] Args
        {
            get { return _args; }
            set 
            {
                if (_args != null)
                    throw new InvalidOperationException("TestVariations have already been frozen, Args cannot be changed while TestSuite is running.");
                _args = value; 
            }
        }

        /// <summary>
        /// Deprecated!
        /// </summary>
        public string CaseNumber
        {
            get
            {
                return _caseNumber;
            }
        }        

        #endregion

        #region Events

        /// <summary>
        /// Signaled when TestSuite is finished running.
        /// </summary>
        public event FinishedEventHandler Finished;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void FinishedEventHandler(object sender, TestSuiteCompleteEventArgs e);

        #endregion

        #region Private Methods

        /// <summary>
        /// Find TestSuite that contains test with the given name.
        /// </summary>
        /// <param name="testId">Name of test to find.</param>
        /// <returns>TestSuite that contains test with the given name of null if none was found.</returns>
        private TestSuite FindTestSuiteWithTestId(string testId)
        {
            foreach (TestSuite suite in Suites)
            {
                IList<TestCase> tests = suite.TestCases;
                IEnumerator<TestCase> testEnumerator = tests.GetEnumerator();
                while (testEnumerator.MoveNext())
                {
                    if (testEnumerator.Current.Id.Equals(testId))
                    {
                        return suite;
                    }
                }
            }
            return null;
        }

        private void InitializeArgs(string[] args)
        {
            // Make sure that cmdArgs array is never null.
            _args = (args == null) ? new string[0] : args;

            foreach (TestSuite nestedSuite in Suites)
            {
                nestedSuite.Args = _args;
            }
        }

        private void EnsureStart()
        {
            if (!_isFinished)
                throw new InvalidOperationException("TestSuite is not finished running, wait for Finished event before starting another run.");
            _isFinished = false;
        }

        private void OnRunFinished(object sender, TestSuiteCompleteEventArgs e)
        {
            if (e.NumberOfVariationsRun == 0)
                throw new InvalidProgramException("TestSuite is finished but no TestVariations were run!");

            // Signal Finished.
            _isFinished = true;
            if (Finished != null)
                Finished(this, e);                
        }
        
        /// <summary>
        /// Create a TestCase object for the given test Id.
        /// </summary>
        /// <param name="id">Id of the test case to initialize.</param>
        /// <returns>Fully initialized TestCase object for given id.</returns>
        private TestCase CreateTestCase(string id)
        {
            // Initialize TestCase.
            TestCase testCase = new TestCase(this, id, Args);

            // Find Test method.
            testCase.TestMethod = GetTestMethod(id);
            // Find Setup and Cleanup methods.
            if (testCase.TestMethod != null)
            {
                FindSupportMethods(testCase, this.GetType());
            }

            return testCase;
        }

        /// <summary>
        /// Recursively search the type hierarchy, beginning with the given type, looking for
        /// Setup and Cleanup methods for the current TestCase.
        /// </summary>
        /// <param name="test">TestCase to find setup and cleanup methods for.</param>
        /// <param name="targetType">Object type to begin search from.</param>
        private void FindSupportMethods(TestCase test, Type targetType)
        {
            if (targetType == null)
                throw new ArgumentException("TargetType cannot be null, error in recursion.");

            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (MethodInfo method in methods)
            {
                object[] attributes = method.GetCustomAttributes(typeof(TestCase_SupportMethod), true);
                if (attributes.Length > 0) 
                {
                    foreach (object attribute in attributes)
                    {
                        TestCase_SupportMethod candidateAttribute = (TestCase_SupportMethod)attribute;
                        if (candidateAttribute.CaseRegex.Match(test.Id).Success)
                        {
                            // Match to SetupMethod
                            if (candidateAttribute is TestCase_Setup)
                            {
                                if (ShouldOverrideSupportMethod(test.Id, test.SetupMethod, candidateAttribute))
                                    test.SetupMethod = method;
                            }
                            // Match to CleanupMethod
                            else
                            {
                                if (ShouldOverrideSupportMethod(test.Id, test.CleanupMethod, candidateAttribute))
                                    test.CleanupMethod = method;
                            }
                        }
                    }
                }
            }

            // Stop searching once we have found both a setup and cleanup method or we reach the top of 
            // our object hierarchy.
            if (!targetType.Equals(typeof(TestSuite)) && (test.SetupMethod == null || test.CleanupMethod == null))
                FindSupportMethods(test, targetType.BaseType);
        }

        /// <summary>
        /// Determine rules for which SupportMethod should be chosen if there are multiple ones that match
        /// the current Testcase Id.  
        /// Rules applied in order of priority:
        ///    1. If only method.
        ///    2. If exact match to TestId.
        ///    3. If matches but not only wildcard.
        ///    4. If wildcard only.
        /// </summary>
        /// <param name="testId">Id of test.</param>
        /// <param name="current">Method we are determing if we should override, may be null.</param>
        /// <param name="candidate">Attribute of candidate method which will be used to make the decision.</param>
        /// <returns>True if the MethodInfo matching the candidate attribute should override the current support method.</returns>
        private bool ShouldOverrideSupportMethod(string testId, MethodInfo current, TestCase_SupportMethod candidate)
        {
            return (current == null ||
                    candidate.CaseRegex.ToString().Equals(testId) ||
                    !candidate.CaseRegex.ToString().Equals(TestCase_SupportMethod.DEFAULT));
        }

        /// <summary>
        /// Find method for executing the test case with given id.  First looks at
        /// method names, if no method name matches the id exactly then looks at
        /// method attributes. Returns null if no method is found.
        /// </summary>
        /// <param name="id">Test case id to find method for.</param>
        /// <returns>Method corresponding to testcase id or null if none exists.</returns>
        private MethodInfo GetTestMethod(string id)
        {
            MethodInfo testMethod = this.GetType().GetMethod(
                                            id,
                                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                            null,
                                            Type.EmptyTypes,
                                            null);

            // If there isn't a method who's name matches the case number then look at all 
            // the method's attributes to find the correct one.
            if (testMethod == null)
            {
                MethodInfo [] methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (MethodInfo method in methods)
                {
                    object[] attributes = method.GetCustomAttributes(typeof(TestId), false);
                    if (attributes.Length != 0 && (attributes[0] as TestId).Id.Equals(id))
                    {
                        testMethod = method;
                        break;
                    }
                }
            }

            return testMethod;
        }

        /// <summary>
        /// Print out help information on how to use this TestSuite implementation:
        /// </summary>
        private void PrintUsage()
        {
            IList<string> parameters = UsageParameters();
            IList<string> usageExamples = UsageExamples();

            GlobalLog.LogStatus("Usage:");
            foreach (string param in parameters)
            {
                GlobalLog.LogStatus("\t" + param);
            }

            GlobalLog.LogStatus("");
            GlobalLog.LogStatus("Examples:");
            foreach (string example in usageExamples)
            {
                GlobalLog.LogStatus("\t" + example);
            }
        }                
       
        #endregion Private Methods

        #region Internal Properties

        internal IList<TestCase> TestCases
        {
            get
            {
                if (_testCases == null)
                {
                    _testCases = new List<TestCase>();
                    // Always return empty list if we are of type TestSuite.
                    if (!this.GetType().Equals(typeof(TestSuite)))
                    {
                        MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        foreach (MethodInfo method in methods)
                        {
                            // Filter property methods.
                            if (!method.Name.StartsWith("get_") && !method.Name.StartsWith("set_"))
                            {
                                // Test methods cannot be Virtual or take parameters.
                                if (method.GetParameters().Length == 0)
                                {
                                    object[] attributes;
                                    // If method is explicitly marked as a Test.
                                    if ((attributes = method.GetCustomAttributes(typeof(TestId), false)).Length != 0)
                                    {
                                        TestId testAttribute = (TestId)attributes[0];
                                        // If name is not set use method name instead.
                                        if (!string.IsNullOrEmpty(testAttribute.Id))
                                            _testCases.Add(CreateTestCase(testAttribute.Id));
                                        else
                                            _testCases.Add(CreateTestCase(method.Name));
                                    }
                                    // If method is not marked as anything else.
                                    else if (!method.IsVirtual &&
                                                (method.GetCustomAttributes(false).Length == 0 ||
                                                method.GetCustomAttributes(typeof(TestCase_SupportMethod), false).Length == 0 &&
                                                method.GetCustomAttributes(typeof(TestCase_Helper), false).Length == 0))
                                    {
                                        _testCases.Add(CreateTestCase(method.Name));
                                    }
                                }
                            }
                        }
                    }
                    // Include all Tests in nested TestSuites.
                    foreach (TestSuite nestedSuite in Suites)
                    {
                        IList<TestCase> nestedTests = nestedSuite.TestCases;
                        for (int i = 0; i < nestedTests.Count; i++)
                            _testCases.Add(nestedTests[i]);
                    }
                }
                return _testCases;
            }
        }
        IList<TestCase> _testCases = null;


        #endregion

        #region Private Fields   

        internal string _caseNumber;
        private string[] _args;

        private IList<TestSuite> _innerSuites = new List<TestSuite>();

        bool _isFinished = true;        

		#endregion Private Fields        
	}

    internal class TestResultException : Exception
    {
        public TestResultException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Exception that signals that a test case has exited with
    /// a FAILED state.
    /// </summary>
    internal class TestFailedException : TestResultException
    {
        public TestFailedException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Exception that signals that a test case has exited with
    /// a PASSED state.
    /// </summary>
    internal class TestPassedException : TestResultException
    {
        public TestPassedException(string msg) : base(msg) { }
    }

}
