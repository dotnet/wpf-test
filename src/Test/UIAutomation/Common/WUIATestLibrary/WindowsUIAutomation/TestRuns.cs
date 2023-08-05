// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//******************************************************************* 
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Collections;
using System.Threading;
using System.Reflection;
using System.Xml;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.CodeDom;
using System.Security;
using System.Security.Permissions;
using System.Resources;
using System.Windows.Automation.Text;

namespace Microsoft.Test.WindowsUIAutomation
{
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.Tests.Patterns;
    using Microsoft.Test.WindowsUIAutomation.Tests.Controls;
    using Microsoft.Test.WindowsUIAutomation.Tests.Scenarios;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
    using Microsoft.Test.WindowsUIAutomation.Logging;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using InternalHelper.Enumerations;
    using InternalHelper.Tests;
    using InternalHelper;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public sealed class TestRuns
    {

        // This is an array of elements that if we find them as child elements,
        // only run one of the children...don't run every sibling with the 
        // same control type, there is no need to test out each and every
        // list item if we are testing out a controls children
        static string[] s_duplicate = new string[] { 
            ControlType.Button.ProgrammaticName,
            ControlType.Custom.ProgrammaticName,
            ControlType.DataItem.ProgrammaticName,
            ControlType.Group.ProgrammaticName,
            ControlType.HeaderItem.ProgrammaticName, 
            ControlType.Hyperlink.ProgrammaticName,
            ControlType.Image.ProgrammaticName,
            ControlType.ListItem.ProgrammaticName,
            ControlType.MenuItem.ProgrammaticName,
            ControlType.Separator.ProgrammaticName,
            ControlType.SplitButton.ProgrammaticName,
            ControlType.TabItem.ProgrammaticName,
            ControlType.Thumb.ProgrammaticName,
            ControlType.TreeItem.ProgrammaticName
        };

        /// <summary>
        /// Defined so it would pass FxCop rules.
        /// </summary>
        TestRuns() { }

        #region Data Members and Properties

        /// -------------------------------------------------------------------
        /// <summary>
        /// Command line argument that sets TestObject property indicating if 
        /// UIVerify should preserve existing content for controls that support
        /// TextPattern/ValuePattenrn. 
        /// Set to TRUE if contents hould be preserved, i.e. not clobbered
        /// </summary>
        /// -------------------------------------------------------------------
        public static bool NoClobber
        {
            get { return TestObject._noClobber; }
            set { TestObject._noClobber = value; }
        }

        #endregion Data Members and Properties

        #region Entry for running tests

        #region Scenarios specific tests

        ///  ------------------------------------------------------------------
        /// <summary>
        /// Run the specified scenario test
        /// </summary>
        ///  ------------------------------------------------------------------
        public static bool RunScenarioTest(string testSuite, string testName, object arguments, bool testEvents, IApplicationCommands commands)
        {
            return RunScenarioTest(null, testSuite, testName, arguments, testEvents, commands);
        }
        ///  ------------------------------------------------------------------
        /// <summary>
        /// Run the specified pattern test
        /// </summary>
        ///  ------------------------------------------------------------------
        public static bool RunScenarioTest(AutomationElement element, string testSuite, string testName, object arguments, bool testEvents, IApplicationCommands commands)
        {

            object testObject = TestObject.GetScenarioTestObject(testSuite, element, testEvents, TestPriorities.PriAll, commands);

            // Test this object
            return ((TestObject)testObject).InvokeSceanario(testSuite, testName, arguments);

        }

        #endregion


        ///  ------------------------------------------------------------------
        ///  <summary></summary>
        ///  ------------------------------------------------------------------
        static public IWUIALogger Logger
        {
            get
            {
                return UIVerifyLogger.BackEndLogger;
            }
            set
            {
                UIVerifyLogger.BackEndLogger = value;
            }
        }

        #region Pattern specific tests

        ///  ------------------------------------------------------------------
        /// <summary>
        /// Run the specified pattern test
        /// </summary>
        ///  ------------------------------------------------------------------
        public static bool RunPatternTest(AutomationElement element, bool testEvents, bool testChildren, string testSuite, string test, object arguments, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    object testObject = TestObject.GetPatternTestObject(testSuite, element, testEvents, TestPriorities.PriAll, commands);
                    passed &= ((TestObject)testObject).InvokeTest(testSuite, test, arguments);
                    if (testChildren)
                    {
                        passed &= RunPatternTestOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(element), testEvents, testChildren, testSuite, test, arguments, commands);
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Run all the tests associated with the pattern as defiend by testSuite
        /// </summary>
        /// -------------------------------------------------------------------
        public static bool RunPatternTests(AutomationElement element, bool testEvents, bool testChildren, string testSuite, TestCaseType testCaseType, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    object testObject = TestObject.GetPatternTestObject(testSuite, element, testEvents, TestPriorities.PriAll, commands);
                    passed &= ((TestObject)testObject).InvokeTests(testSuite, testCaseType);
                    if (testChildren)
                    {
                        passed &= RunPatternTestsOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(element), testEvents, testChildren, testSuite, testCaseType, commands);
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Run all the tests associated with the defined pattern and that meet 
        /// the criteria as supplied as arguments
        /// </summary>
        /// -------------------------------------------------------------------
        public static bool RunPatternTests(AutomationElement element, bool testEvents, TestPriorities priority, TestCaseType testCaseType, string testSuite, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    object testObject = TestObject.GetPatternTestObject(testSuite, element, testEvents, priority, commands);
                    passed &= ((TestObject)testObject).InvokeTests(testSuite, testCaseType);
                }

                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        #endregion Pattern specific tests

        #region Control specific tests

        ///  ------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ///  ------------------------------------------------------------------
        public static bool RunControlTests(AutomationElement element, bool testEvents, bool testChildren, TestPriorities priority, TestCaseType testCaseType, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    string testSuite = TestObject.GetTestType(element);
                    object testObject = TestObject.GetControlTestObject(element, testEvents, priority, commands);
                    passed &= ((TestObject)testObject).InvokeTests(testSuite, testCaseType);
                    if (testChildren)
                    {
                        passed &= RunControlTestsOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(element), testEvents, testChildren, priority, testCaseType, commands);
                    }

                }

                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        private static bool RunControlTestsOnDescendants(AutomationElement element, bool testEvents, bool testChildren, TestPriorities priority, TestCaseType testCaseType, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    string testSuite = TestObject.GetTestType(element);
                    object testObject = TestObject.GetControlTestObject(element, testEvents, TestPriorities.PriAll, commands);
                    passed &= ((TestObject)testObject).InvokeTests(testSuite, testCaseType);
                    if (testChildren)
                    {
                        passed &= RunControlTestsOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(element), testEvents, testChildren, priority, testCaseType, commands);
                        passed &= RunControlTestsOnDescendants(TreeWalker.ControlViewWalker.GetNextSibling(element), testEvents, testChildren, priority, testCaseType, commands);
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }

            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Run the specified control test by name
        /// </summary>
        /// -------------------------------------------------------------------
        public static bool RunControlTest(AutomationElement element, bool testEvents, string testSuite, string test, object arguments, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    object testObject = TestObject.GetControlTestObject(element, testEvents, TestPriorities.PriAll, commands);
                    passed &= ((TestObject)testObject).InvokeTest(testSuite, test, arguments);
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        #endregion Control specific tests

        #region Versions of All* tests

        ///  ------------------------------------------------------------------
        /// <summary>
        /// Run all the tests for patterns, control type, and AutomationElement that meet the criteria as supplied as arguments
        /// </summary>
        /// <returns></returns>
        ///  ------------------------------------------------------------------
        public static bool RunAllTests(AutomationElement element, bool testEvents, TestPriorities priority, TestCaseType testCaseType, bool testChildren, bool normalize, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    passed &= RunAllControlTests(element, testEvents, priority, testCaseType, testChildren, normalize, commands);
                    passed &= RunAutomationElementTests(element, testEvents, priority, testCaseType, testChildren, normalize, commands);
                    passed &= RunAllPatternTests(element, testEvents, priority, testCaseType, testChildren, normalize, commands);
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Run all the supported pattern tests associated with the element and that meet
        /// the criteria as supplied as arguments
        /// </summary>
        /// -------------------------------------------------------------------
        public static bool RunAllPatternTests(AutomationElement element, bool testEvents, TestPriorities priority, TestCaseType testCaseType, bool testChildren, bool normalize, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    ArrayList al = Helpers.GetPatternSuitesForAutomationElement(element);

                    foreach (string testSuite in al)
                    {
                        passed &= RunPatternTests(element, testEvents, priority, testCaseType, testSuite, commands);
                    }

                    if (testChildren)
                    {
                        passed &= RunAllPatternTestsOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(element), testEvents, testChildren, priority, normalize, testCaseType, commands);
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Run all control tests, pattern tests, and automation element tests
        /// </summary>
        /// -------------------------------------------------------------------
        public static bool RunAllControlTests(AutomationElement element, bool testEvents, TestPriorities priority, TestCaseType testCaseType, bool testChildren, bool normalize, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    passed &= RunControlTests(element, testEvents, false, priority, testCaseType, commands);

                    if (testChildren)
                    {
                            passed &= RunAllControlTestOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(element), testEvents, testChildren, priority, normalize, testCaseType, commands);
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Run all control tests, pattern tests, and automation element tests
        /// </summary>
        /// -------------------------------------------------------------------
        public static bool RunStressTests(AutomationElement element, bool testEvents, bool testChildren, int numThreads, TestLab  testLab, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    object testObject = TestObject.GetStressTestObject(element, testEvents, testChildren, numThreads, testLab, commands);
                    passed &= ((TestObject)testObject).InvokeTests(Microsoft.Test.WindowsUIAutomation.Tests.Scenarios.StressScenarioTests.TestSuite, TestCaseType.Generic);
                }

                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;            
        }


        #endregion Versions of All* tests

        #region Misc


        /// -------------------------------------------------------------------
        /// <summary>
        /// Run all the AutomationElement tests on the element that meet the 
        /// criteria as supplied as arguments
        /// </summary>
        /// -------------------------------------------------------------------
        public static bool RunAutomationElementTests(AutomationElement element, bool testEvents, TestPriorities priority, TestCaseType testCaseType, bool testChildren, bool normalize, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    object test = new AutomationElementTests(element, priority, null, testEvents, TypeOfControl.UnknownControl, commands);

                    passed &= ((TestObject)test).InvokeTests(AutomationElementTests.TestSuite, testCaseType);

                    if (testChildren)
                    {
                        passed &= RunAutomationElementTestsOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(element), testEvents, testChildren, priority, normalize, testCaseType, commands);
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Run the Automation test as defined by test
        /// </summary>
        /// -------------------------------------------------------------------
        public static bool RunAutomationElementTest(AutomationElement element, bool testEvents, string test, bool testChildren, object arguments, bool normalize, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    object testObject = new AutomationElementTests(element, TestPriorities.PriAll, null, testEvents, TypeOfControl.UnknownControl, commands);

                    passed &= ((TestObject)testObject).InvokeTest(AutomationElementTests.TestSuite, test, arguments);

                    if (testChildren)
                    {
                        passed &= RunAutomationElementTestOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(element), testEvents, test, testChildren, arguments, normalize, commands);
                    }

                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Recursively tests the children Logical Elements
        /// </summary>
        /// -------------------------------------------------------------------
        static bool RunAutomationElementTestOnDescendants(AutomationElement element, bool testEvents, string test, bool testChildren, object arguments, bool normalize, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {

                    AutomationElement tempLe;
                    ArrayList list;
                    Hashtable ht = GetHashedElements(element);
                    IDictionaryEnumerator enumerator = ht.GetEnumerator();
                    Random rnd = new Random(unchecked((int)DateTime.Now.Ticks));

                    // Add the nodes to the tree.  Some of the nodes we may want to remove
                    // because of redundancy for specific nodes as defined in the m_Duplicate
                    // ArrayList.
                    while (enumerator.MoveNext())
                    {
                        // Get the list of elements
                        list = (ArrayList)ht[enumerator.Key];

                        if (normalize && Array.IndexOf(s_duplicate, enumerator.Key) != -1)
                        {// We want to normalize the list, only to uniuque elements
                            tempLe = (AutomationElement)list[rnd.Next(list.Count)];

                            passed &= RunAutomationElementTest(tempLe, testEvents, test, false, arguments, normalize, commands);

                            if (testChildren)
                            {
                                passed &= RunAutomationElementTestOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(tempLe), testEvents, test, testChildren, arguments, normalize, commands);
                            }
                        }
                        else
                        {// We don't want to normalize, just do everything
                            foreach (AutomationElement el in list)
                            {
                                passed &= RunAutomationElementTest(el, testEvents, test, false, arguments, normalize, commands);
                                if (testChildren)
                                {
                                    passed &= RunAutomationElementTestOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(el), testEvents, test, testChildren, arguments, normalize, commands);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }
        #endregion Misc


        #endregion Entry for running tests

        #region Misc
        /// -------------------------------------------------------------------
        /// <summary>
        /// Generates a hash table. The key is the localized control type.  The 
        /// element is an ArrayList of the actual elements that correspond to 
        /// the control type.
        /// </summary>
        /// -------------------------------------------------------------------
        static Hashtable GetHashedElements(AutomationElement element)
        {
            Hashtable ht = new Hashtable();

            // Hash the elements into unique buckets so we can pick random elements in each bucket if
            // we so desire later in code that follows this.
            for (AutomationElement tempElement = element; tempElement != null; tempElement = TreeWalker.ControlViewWalker.GetNextSibling(tempElement))
            {
                string controlType;

                if (tempElement.Current.ControlType != null)
                {
                    controlType = tempElement.Current.ControlType.ProgrammaticName;

                    // Add the entry if it does not exist.
                    if (!ht.ContainsKey(controlType))
                    {
                        ht.Add(controlType, new ArrayList());
                    }

                    // Add the element to the key's ArrayList.
                    ((ArrayList)ht[controlType]).Add(tempElement);
                }
            }

            return ht;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static bool RunPatternTestOnDescendants(AutomationElement element, bool testEvents, bool testChildren, string testSuite, string test, object arguments, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    try
                    {
                        //AutomationIdentifier IsPatternAvaliable = Helpers.GetPropertyByName(Automa
                        // Try to instantiate a test object with this pattern.  If the pattern is no supported, then 
                        object testObject = TestObject.GetPatternTestObject(testSuite, element, testEvents, TestPriorities.PriAll, commands);
                        
                        passed &= ((TestObject)testObject).InvokeTest(testSuite, test, arguments);
                    }
                    catch (Exception)
                    {
                        // Trying to get a pattern that is not supported.  Eat this so that we can continue to do the children
                    }


                    if (testChildren)
                    {
                        passed &= RunPatternTestOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(element), testEvents, testChildren, testSuite, test, arguments, commands);
                        passed &= RunPatternTestOnDescendants(TreeWalker.ControlViewWalker.GetNextSibling(element), testEvents, testChildren, testSuite, test, arguments, commands);
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }

            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static bool RunPatternTestsOnDescendants(AutomationElement element, bool testEvents, bool testChildren, string testSuite, TestCaseType testCaseType, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {

                    object testObject = TestObject.GetPatternTestObject(testSuite, element, testEvents, TestPriorities.PriAll, commands);

                    // Test this object
                    passed &= ((TestObject)testObject).InvokeTests(testSuite, testCaseType);
                    if (testChildren)
                    {
                        passed &= RunPatternTestsOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(element), testEvents, testChildren, testSuite, testCaseType, commands);
                        passed &= RunPatternTestsOnDescendants(TreeWalker.ControlViewWalker.GetNextSibling(element), testEvents, testChildren, testSuite, testCaseType, commands);
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }

            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static bool RunAllPatternTestsOnDescendants(AutomationElement element, bool testEvents, bool testChildren, TestPriorities priority, bool normalize, TestCaseType testCaseType, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    AutomationElement tempLe;
                    ArrayList list;
                    Hashtable ht = GetHashedElements(element);
                    IDictionaryEnumerator enumerator = ht.GetEnumerator();
                    Random rnd = new Random(unchecked((int)DateTime.Now.Ticks));

                    // Add the nodes to the tree.  Some of the nodes we may want to remove
                    // because of redundancy for specific nodes as defined in the m_Duplicate
                    // ArrayList.
                    while (enumerator.MoveNext())
                    {
                        list = (ArrayList)ht[enumerator.Key];
                        if (normalize && Array.IndexOf(s_duplicate, enumerator.Key) != -1)
                        {
                            // Remove defined items that we don't want duplicates of
                            tempLe = (AutomationElement)list[rnd.Next(list.Count)];

                            ArrayList al = Helpers.GetPatternSuitesForAutomationElement(tempLe);

                            foreach (string testSuite in al)
                            {
                                passed &= RunAllPatternTests(tempLe, testEvents, priority, testCaseType, false, normalize, commands);
                            }

                            if (testChildren)
                            {
                                passed &= RunAllPatternTestsOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(tempLe), testEvents, testChildren, priority, normalize, testCaseType, commands);
                            }
                        }
                        else
                        {
                            // Add everything else whether duplicate or not
                            foreach (AutomationElement el in list)
                            {
                                passed &= RunAllPatternTests(el, testEvents, priority, testCaseType, false, normalize, commands);
                                if (testChildren)
                                {
                                    passed &= RunAllPatternTestsOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(el), testEvents, testChildren, priority, normalize, testCaseType, commands);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static bool RunAllControlTestOnDescendants(AutomationElement element, bool testEvents, bool testChildren, TestPriorities priority, bool normalize, TestCaseType testCaseType, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {
                    AutomationElement tempLe;
                    ArrayList list;

                    Hashtable ht = GetHashedElements(element);
                    IDictionaryEnumerator enumerator = ht.GetEnumerator();
                    Random rnd = new Random(unchecked((int)DateTime.Now.Ticks));

                    // Add the nodes to the tree.  Some of the nodes we may want to remove
                    // because of redundancy for specific nodes as defined in the m_Duplicate
                    // ArrayList.
                    while (enumerator.MoveNext())
                    {
                        list = (ArrayList)ht[enumerator.Key];
                        if (normalize && Array.IndexOf(s_duplicate, enumerator.Key) != -1)
                        {
                            // Remove defined items that we don't want duplicates of
                            tempLe = (AutomationElement)list[rnd.Next(list.Count)];
                            passed &= RunAllControlTests(tempLe, testEvents, priority, testCaseType, false, normalize, commands);
                            if (testChildren)
                            {
                                passed &= RunAllControlTestOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(tempLe), testEvents, testChildren, priority, normalize, testCaseType, commands);
                            }
                        }
                        else
                        {
                            // Add everything else whether duplicate or not
                            foreach (AutomationElement el in list)
                            {
                                passed &= RunAllControlTests(el, testEvents, priority, testCaseType, false, normalize, commands);
                                if (testChildren)
                                {
                                    passed &= RunAllControlTestOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(el), testEvents, testChildren, priority, normalize, testCaseType, commands);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Recursively tests the children Logical Elements
        /// </summary>
        /// -------------------------------------------------------------------
        static bool RunAutomationElementTestsOnDescendants(AutomationElement element, bool testEvents, bool testChildren, TestPriorities priority, bool normalize, TestCaseType testCaseType, IApplicationCommands commands)
        {
            bool passed = true;

            if (element != null)
            {
                try
                {

                    AutomationElement tempLe;
                    ArrayList list;
                    Hashtable ht = GetHashedElements(element);
                    IDictionaryEnumerator enumerator = ht.GetEnumerator();
                    Random rnd = new Random(unchecked((int)DateTime.Now.Ticks));

                    // Add the nodes to the tree.  Some of the nodes we may want to remove
                    // because of redundancy for specific nodes as defined in the m_Duplicate
                    // ArrayList.
                    while (enumerator.MoveNext())
                    {
                        list = (ArrayList)ht[enumerator.Key];
                        if (normalize && Array.IndexOf(s_duplicate, enumerator.Key) != -1)
                        {
                            // Remove defined items that we don't want duplicates of
                            tempLe = (AutomationElement)list[rnd.Next(list.Count)];

                            passed &= RunAutomationElementTests(tempLe, testEvents, priority, testCaseType, false, normalize, commands);
                            if (testChildren)
                            {
                                passed &= RunAutomationElementTestsOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(tempLe), testEvents, testChildren, priority, normalize, testCaseType, commands);
                            }
                        }
                        else
                        {
                            // Add everything else whether duplicate or not
                            foreach (AutomationElement el in list)
                            {
                                passed &= RunAutomationElementTests(el, testEvents, priority, testCaseType, false, normalize, commands);
                                if (testChildren)
                                {
                                    passed &= RunAutomationElementTestsOnDescendants(TreeWalker.ControlViewWalker.GetFirstChild(el), testEvents, testChildren, priority, normalize, testCaseType, commands);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    UIVerifyLogger.LogUnexpectedError(exception);
                }
            }
            return passed;
        }

        #endregion Misc

        public static void StopRunningTests()
        {
            TestObject.CancelRun = true;
        }
    }
}
