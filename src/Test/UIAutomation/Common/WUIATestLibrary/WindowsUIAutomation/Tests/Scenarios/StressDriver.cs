// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * UIAutomation Stress tests cases
 * 
 * This file is used by the Avalon Stress UIAutomationStress project, and the 
 * UIVerify test framework.  Any changes to this file needs to be build in 
 * Client/AccessibleTech, and Client/WcpTests/Stress/Fremwork
 
********************************************************************/

using System;
using System.Windows.Automation;
using System.Windows;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Test.WindowsUIAutomation.Tests.Scenarios
{
    using UIAStressHelpers;
    using InternalHelper;
    using InternalHelper.Tests;
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using UIA = System.Windows.Automation;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class StressScenarioTests : ScenarioObject
    {
        #region Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const string THIS = "StressScenarioTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        bool _testChildren = true;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        int _numThreads = 1;

        /// -------------------------------------------------------------------
        /// <summary>AutomationElement of the application</summary>
        /// -------------------------------------------------------------------
        UIA.AutomationElement _applicationElement;

        /// -------------------------------------------------------------------
        /// <summary>AutomationElement of the application</summary>
        /// -------------------------------------------------------------------
        public UIA.AutomationElement ApplicationElement
        {
            get { return _applicationElement; }
            set { _applicationElement = value; }
        }

        /// -------------------------------------------------------------------
        /// <summary>Avalaible AutomationProperties</summary>
        /// -------------------------------------------------------------------
        ArrayList _automationProperties = null;

        /// -------------------------------------------------------------------
        /// <summary>Avalaible AutomationPatterns</summary>
        /// -------------------------------------------------------------------
        ArrayList _automationPatterns = null;

        TestLab _testLab;

        #endregion Variables

        /// -------------------------------------------------------------------
        /// <summary>Call this to initialize the class so that one can call the
        /// entry points depending on which framework is used</summary>
        /// -------------------------------------------------------------------
        public StressScenarioTests(UIA.AutomationElement element, bool testEvents, bool testChildren, int numThreads, TestLab testLab, IApplicationCommands commands)
            :
            base(element, null, TestPriorities.PriAll, TypeOfControl.UnknownControl, TypeOfPattern.Unknown, null, testEvents, commands)
        {
            _testChildren = testChildren;
            _numThreads = numThreads;
            _testLab = testLab;
            //PopulateAutomationElementProperties(ref _automationProperties, ref _automationPatterns);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// FRAMEWORK: UIVERIFY
        /// This is the entry location for the UIVerify framework.  E:\Depots\vbl_wcp_avalon_dev\testsrc\windowstest\client\AccessibleTech\UIVerify\WUIATestLibrary\InternalHelper\Tests\PatternObject.cs
        /// It is meant to run from within UIVerify framework.
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("UIVerify Stress",
            TestSummary = "Run general purpose stress from Tactics/wtt lab, or Visual UIVerify",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] { })]
        public void UIVerifyStressDriver(TestCaseAttribute testCase)
        {
            // Run until the user presses Cancel button
            StressDriver(-1, Scenario.General);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// FRAMEWORK: AVALON STRESS LAB
        /// This is the entry location for the Avalon Stress 
        /// Lab.  It is not meant to run from within UIVerify.
        /// </summary>
        /// -------------------------------------------------------------------

        internal void AvalonStressDriver(int actionCount, Scenario scenario)
        {
            StressDriver(actionCount, scenario);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// This is the main generic stress driver called from the different users.
        /// This is called from the framework entry point functions AvalonStressDriver
        /// and UIVerifyStressDriver.
        /// </summary>
        /// -------------------------------------------------------------------
        private void StressDriver(int actionCount, Scenario scenario)
        {
            Library.ValidateArgumentNonNull(m_le, "m_le");
            Console.WriteLine(Library.GetUISpyLook(m_le));

            Thread[] thread = new Thread[_numThreads];

            if (thread == null)
                throw new Exception("Could not initialize thread objects");

            for (int i = 0; i < _numThreads; i++)
            {
                _appCommands.TraceMethod("Starting Thread \"" + i + "\"");
                thread[i] = new Thread(new ParameterizedThreadStart(StressTestsDrivers.RunStressTests));
                thread[i].Start(new PacketInfo(_applicationElement, m_le, _appCommands, _testLab, actionCount, _automationProperties, _automationPatterns, scenario));
            }

            // wait till it has stopped
            while (!TestObject.CancelRun && thread[0].ThreadState != System.Threading.ThreadState.Stopped)
                Thread.Sleep(1000);

            //Clean up the threads
            for (int i = 0; i < _numThreads; i++)
            {
                thread[i].Abort();
            }
        }
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Class that has the actual tests.  Call RunStressTests to start the diver that 
    /// drives the tests.
    /// </summary>
    /// -----------------------------------------------------------------------
    internal class StressTestsDrivers
    {
        #region Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        Hashtable _testMethods = new Hashtable();
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        UIA.AutomationElement _curElement;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        UIA.AutomationElement _originalElement;
        /// -------------------------------------------------------------------
        /// <summary>Application element that has _originalElement</summary>
        /// -------------------------------------------------------------------
        UIA.AutomationElement _applicationElement;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        IApplicationCommands _appCommands;
        /// -------------------------------------------------------------------
        /// <summary>Milliseconds to sleep between tests as defined by the
        /// stress lab</summary>
        /// -------------------------------------------------------------------
        int _sleepBetweenTests = 100;
        /// -------------------------------------------------------------------
        /// <summary>
        /// Determines how many times an action is executed.  Pass -1 means 
        /// do indefinitely.
        /// </summary>
        /// -------------------------------------------------------------------
        int _actionCount;
        /// -------------------------------------------------------------------
        /// <summary>
        /// Available AutomationProperties
        /// </summary>
        /// -------------------------------------------------------------------
        ArrayList _automationProperties;
        /// -------------------------------------------------------------------
        /// <summary>
        /// Available AutomationPatterns
        /// </summary>
        /// -------------------------------------------------------------------
        ArrayList _automationPatterns;
        /// -------------------------------------------------------------------
        /// <summary>
        /// Random generator
        /// </summary>
        /// -------------------------------------------------------------------
        Random _rnd = new Random();

        ArrayList _contextElements = new ArrayList();

        #endregion Variables

        /// -------------------------------------------------------------------
        /// <summary>
        /// Static entry to the stress code.  This calls StressTests 
        /// which then runs the stress code
        /// </summary>
        /// -------------------------------------------------------------------
        static public void RunStressTests(object threadInfoObj)
        {
            StressTestsDrivers sd = new StressTestsDrivers((PacketInfo)threadInfoObj);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Main driver code that calls the actual tests.  This method will 
        /// continue call the tests until either the test application has gone 
        /// away, or the thread is aborted.
        /// </summary>
        /// -------------------------------------------------------------------
        StressTestsDrivers(PacketInfo threadInfo)
        {
            //AutomationPattern[] patterns = null;
            PacketInfo ti = (PacketInfo)threadInfo;
            _originalElement = ti.AutomationElement;
            _appCommands = ti.AppCommands;
            _actionCount = ti.ActionCount;
            _applicationElement = ti.ApplicationElement;
            _curElement = _originalElement;
            _automationProperties = ti.AutomationProperties;
            _automationPatterns = ti.AutomationPatterns;
            UIA.AutomationElement element = null;
            MethodInfo method = null;
            ArrayList methodList = null;
            
            // Build the table that will reference all the methods in this assembly 
            // that has a StressBuckets custom attribute defined.
            BuildMethodsTable(ti.TestLab, TestWorks.Stable, ti.Scenario);

            BuildElementTable(_originalElement);

            // Loop and run actions
            for (int actionCount = 0; ; actionCount++)
            {
                try
                {
                    string patternName = string.Empty;

                    // 
                    if (_rnd.Next(_patterns.Count + 1) == _patterns.Count)
                    {
                        patternName = "AutomationElement";
                    }
                    else
                    {
                        patternName = (string)_patterns[_rnd.Next(_patterns.Count)];
                    }

                    // Is there a ArrayList of methods associated with the pattern?
                    if (null != (methodList = (ArrayList)(_testMethods[patternName])))
                    {
                        // Find a specific test in the patterns collection...
                        if (null != (method = (MethodInfo)methodList[_rnd.Next(methodList.Count)]))
                        {
                            // If the test has set it;s 
                            if (Stable(method.Name, ti.TestLab))
                            {
                                element = GetRandomObjectByType(patternName);

                                // If we found an element that supports the pattern, then run the test
                                if (null != (element = GetRandomObjectByType(patternName)))
                                {
                                    ti.AppCommands.TraceMethod(method.Name);
#if PENTEST
                                    try
                                    {
#endif
                                        method.Invoke(this, new object[] { element });
#if PENTEST
                                    }
                                    catch (Exception)
                                    {
                                    }
#endif

                                }
                            }
                        }
                    }
                }
                // High level exceptions that can occur everywhere
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // Hot key already registered
                }

            }
        }

        /// -------------------------------------------------------------------
        /// <summary>Return whether the attribute is set to Stable</summary>
        /// -------------------------------------------------------------------
        internal static bool Stable(string methodName, TestLab testLab)
        {
            Type type = typeof(StressTests);
            MethodInfo method = type.GetMethod(methodName);
            StressBuckets testAttribute = (StressBuckets)method.GetCustomAttributes(true)[0];
            return testAttribute.TestWorks == TestWorks.Stable
                && testAttribute.BugNumber == 0;
                //&& (testAttribute.TestLab & testLab) == testLab;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Return a random element that supports the pattern
        /// </summary>
        /// -------------------------------------------------------------------
        private UIA.AutomationElement GetRandomObjectByType(string patternName)
        {
            Type type = Assembly.GetExecutingAssembly().GetType("UIAStressHelpers." + patternName);
            ArrayList list = new ArrayList();

            if (type != null)
            {
                foreach (object uia in _contextElements)
                {
                    if (uia.GetType() == type)
                    {
                        // Build a list of elements that support the pattern
                        list.Add(((UIAutomationTestTypes)uia).AutomationElement);
                    }
                }
            }

            if (list.Count != 0)
            {
                // return a random element
                return (UIA.AutomationElement)list[_rnd.Next(list.Count)];
            }
            else
                return null;

        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Get a list of patterns that UIA supports
        /// </summary>
        /// -------------------------------------------------------------------
        static ArrayList GetPatternList()
        {
            Type baseType = typeof(System.Windows.Automation.WindowPattern).BaseType;

            Assembly automationAssemblyName = Assembly.GetAssembly(baseType);

            ArrayList list = new ArrayList();

            foreach (Type type in automationAssemblyName.GetTypes())
            {
                if (type.BaseType == baseType)
                    if (!list.Contains(type.Name))
                        list.Add(type.Name);
            }
            return list;
        }

        ArrayList _patterns = null;

        private void BuildElementTable(UIA.AutomationElement _originalElement)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(UIA.AutomationElement));
            Assembly curAssembly = Assembly.GetCallingAssembly();

            // Get all patterns currently supported by UIA
            if (_patterns == null)
                _patterns = GetPatternList();

            try
            {
                // Use refelction to build up the list of elements
                foreach (UIA.AutomationElement element in _originalElement.FindAll(UIA.TreeScope.Subtree, UIA.Condition.TrueCondition))
                {
                    // Everything supports the AutomationElement "pattern" so add them
                    // as AutomationStressTests.AutomationElement
                    _contextElements.Add(new AutomationElement(element));

                    // If the AutomationElement supports a specific pattern, then 
                    // create an UIAutomationTestTypes based on the specific 
                    // pattern that is supported.  If the AutomationElement
                    // supports more than one pattern, then create another
                    // UIAutomationTestTypes of the other patterns and store them
                    // for later retrieval using the GetRandomObjectByType()
                    foreach (string patternName in _patterns)
                    {
                        // Get the particular Is<Pattern>Supported property such as IsScrollPatternSupported
                        FieldInfo fi = assembly.GetType("System.Windows.Automation.AutomationElement").GetField("Is" + patternName + "AvailableProperty");

                        // Use the property to get the actual value of the Is<Pattern>Supported 
                        // associated with the specific AutomationElement
                        if ((bool)element.GetCurrentPropertyValue((UIA.AutomationProperty)fi.GetValue(element)))
                        {
                            // If it's not the close button, create the AutomationStressTests.<Pattern> and store
                            // it in the context for later retrieval using GetRandomObjectByType()
                            if (patternName != UIA.InvokePattern.Pattern.ProgrammaticName && element.Current.Name != "Close")
                            {
                                _appCommands.TraceMethod("Creating UIAStressHelpers." + patternName);
                                _contextElements.Add(Activator.CreateInstance(curAssembly.GetType("UIAStressHelpers." + patternName), new object[] { element }));
                            }
                        }
                    }
                }
            }
            catch (InvalidCastException e)
            {
                System.Diagnostics.Debug.Assert(false);
                Console.WriteLine(e.ToString());
            }

        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// This method will randomly navigate to an AutomationElement relative 
        /// of the _curElement variable.  It will then look at the supported 
        /// patterns of the AutomationElement and randomly call one of the supported
        /// patterns methods.
        /// 
        /// If it catches an exception from the method that is invoked, it will
        /// throw the inner exception that the method threw.
        /// </summary>
        /// -------------------------------------------------------------------
        private void InvokeRandomMethod(string patternName, bool dumpStressBucketAttribute)
        {
            MethodInfo method = null;

            // Verify that the pattern has an ArrayList of methods associated
            // with the patterName passed in.  If it does not then just exit
            // gracefully and continue.
            string pattern = patternName.Replace("Identifiers.Pattern", "");
            if (_testMethods.ContainsKey(pattern))
            {

                // Get the methods associated with the bucket type
                ArrayList list = (ArrayList)_testMethods[patternName];

                // Find a random method assocated with the bucket type
                method = (MethodInfo)list[_rnd.Next(list.Count)];
                System.Diagnostics.Debug.Assert(method != null);

                if (method != null)
                {
                    try
                    {
                        if (dumpStressBucketAttribute)
                            Logging.Logger.LogComment("TBD"); //(StressBuckets)(method.GetCustomAttributes(false)[0]), true, null);

                        // Invoke the random method assocuated with the bucket
                        method.Invoke(this, null);

                        // Let the other processes take a time slice as required by the stress lab
                        Thread.Sleep(_sleepBetweenTests);
                    }
                    // This should only ever throw TargetInvocationException from 
                    // an Invoked method.  The InnerException is the true exception.
                    catch (TargetInvocationException error)
                    {

                        Logging.Logger.LogComment("TBD"); //Error in Method<" + Thread.CurrentThread.ManagedThreadId + "> = " + method.Name, false, null);
                        throw error.InnerException;
                    }
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Build up the _methods variable with all the methods associated with the patterns, 
        /// AutomationElement, etc. buckets so we can call them.
        /// 
        /// </summary>


        private void BuildMethodsTable(TestLab testLab, TestWorks testWorks, Scenario scenario)
        {
            Type type = Type.GetType("Microsoft.Test.WindowsUIAutomation.Tests.Scenarios.StressTests");

            System.Diagnostics.Debug.Assert(type != null);

            if (type != null)
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
                {
                    foreach (Attribute attr in method.GetCustomAttributes(false))
                    {
                        if (attr is StressBuckets)
                        {
                            StressBuckets sb = (StressBuckets)attr;

                            if (((sb.TestWorks & testWorks) == testWorks) &&
                                (((sb.TestLab & testLab) == testLab) || testLab == TestLab.PushPullData) &&
                                ((sb.Scenario & scenario) == scenario)
                                )
                            {
                                // Only create the 
                                if (!_testMethods.ContainsKey(sb.PatternName))
                                {
                                    ArrayList list = new ArrayList();
                                    _testMethods.Add(sb.PatternName, list);
                                    Console.WriteLine(sb.PatternName);
                                }
                                _appCommands.TraceMethod("Adding: " + method.Name);
                                ((ArrayList)_testMethods[sb.PatternName]).Add(method);
                            }
                        }
                    }
                }
            }
        }

    }
}

namespace UIAStressHelpers
{
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
    using Microsoft.Test.WindowsUIAutomation.Tests.Scenarios;
    using UIA = System.Windows.Automation;

    /// -----------------------------------------------------------------------
    /// <summary>
    /// To pass parameters to a thread, you need an 'object' datatype.  This 
    /// class serves as the object to the paramaterized thread that is called 
    /// from the StressTests to Scenarios.StressTests.RunStressTests
    /// </summary>
    /// -----------------------------------------------------------------------
    class PacketInfo
    {
        #region Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        UIA.AutomationElement _element;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        UIA.AutomationElement _applicationElement;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        IApplicationCommands _appCommands;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        TestLab _testLab;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        int _actionCount;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        ArrayList _automationProperties = null;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        ArrayList _automationPatterns = null;
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        Scenario _scenario;

        #endregion Variables

        #region Properties
        /// -------------------------------------------------------------------
        /// <summary>Get the scenario tests to run</summary>
        /// -------------------------------------------------------------------
        public Scenario Scenario { get { return _scenario; } }
        /// -------------------------------------------------------------------
        /// <summary>Available AutomationProperties</summary>
        /// -------------------------------------------------------------------
        public ArrayList AutomationPatterns { get { return _automationPatterns; } }
        /// -------------------------------------------------------------------
        /// <summary>Available AutomationProperties</summary>
        /// -------------------------------------------------------------------
        public ArrayList AutomationProperties { get { return _automationProperties; } }
        /// -------------------------------------------------------------------
        /// <summary>AutomationElement that is the top level window</summary>
        /// -------------------------------------------------------------------
        public UIA.AutomationElement ApplicationElement { get { return _applicationElement; } }
        /// -------------------------------------------------------------------
        /// <summary>Automation that the test will start from for testing</summary>
        /// -------------------------------------------------------------------
        public UIA.AutomationElement AutomationElement { get { return _element; } }
        /// -------------------------------------------------------------------
        /// <summary>Used for the callback to print out the results</summary>
        /// -------------------------------------------------------------------
        public IApplicationCommands AppCommands { get { return _appCommands; } }
        /// -------------------------------------------------------------------
        /// <summary>Where it is valid to run this test</summary>
        /// -------------------------------------------------------------------
        internal TestLab TestLab { get { return _testLab; } }
        /// -------------------------------------------------------------------
        /// <summary>Number of actions to run.  If it is -1, then 
        /// run continuous</summary>
        /// -------------------------------------------------------------------
        public int ActionCount { get { return _actionCount; } }

        #endregion Properties

        /// -------------------------------------------------------------------
        /// <summary>Constructor of object to pass to the thread</summary>
        /// -------------------------------------------------------------------
        public PacketInfo(
            UIA.AutomationElement applicationElement,
            UIA.AutomationElement element,
            IApplicationCommands appCommands,
            TestLab testLab,
            int actionCount,
            ArrayList automationProperties,
            ArrayList automationPatterns,
            Scenario scenario)
        {
            _applicationElement = applicationElement;
            _element = element;
            _appCommands = appCommands;
            _testLab = testLab;
            _actionCount = actionCount;
            _automationProperties = automationProperties;
            _automationPatterns = automationPatterns;
            _scenario = scenario;
            _testLab = testLab;
        }
    }

    #region UIAutomationTestTypes

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Need to store each reference to the AutomationElement by pattern type
    /// 
    /// </summary>






    public class UIAutomationTestTypes
    {
        #region Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        UIA.AutomationElement _automationElement;

        public UIA.AutomationElement AutomationElement
        {
            get { return _automationElement; }
            set { _automationElement = value; }
        }

        #endregion Variables
        public UIAutomationTestTypes(UIA.AutomationElement element)
        {
            _automationElement = element;
        }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class AutomationElement : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public AutomationElement(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class DockPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public DockPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class ExpandCollapsePattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ExpandCollapsePattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class GridItemPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public GridItemPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class GridPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public GridPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class InvokePattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public InvokePattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class MultipleViewPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public MultipleViewPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class RangeValuePattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public RangeValuePattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class ScrollItemPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ScrollItemPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class ScrollPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ScrollPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class SelectionItemPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public SelectionItemPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class SelectionPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public SelectionPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class TextPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TextPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class TogglePattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TogglePattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class TransformPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TransformPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class ValuePattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ValuePattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }
    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class WindowPattern : UIAutomationTestTypes
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public WindowPattern(UIA.AutomationElement automationElement) : base(automationElement) { }
    }

    #endregion UIAutomationTestTypes

}
