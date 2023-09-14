// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//******************************************************************* 
using System;
using System.Windows;
using System.Windows.Automation;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Globalization;

// This suppresses warnings #'s not recognized by the compiler.
#pragma warning disable 1634, 1691

namespace InternalHelper.Tests
{
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using InternalHelper;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class ControlObject : TestObject
    {
        #region Variables

        internal const string NAMESPACE = IDSStrings.IDS_NAMESPACE_UIVERIFY + "." + IDSStrings.IDS_NAMESPACE_CONTROL;

        /// ------------------------------------------------------------------------
        /// <summary>
        /// XML file of the correct properties associated with a control
        /// </summary>
        /// ------------------------------------------------------------------------
        const string XMLPROPERTIES = "FootPrint_Properties.xml";

        /// ------------------------------------------------------------------------
        /// <summary>
        /// XML file of the correct patterns associated with a control
        /// </summary>
        /// ------------------------------------------------------------------------
        const string XMLPATTERNS = "FootPrint_Patterns.xml";

        /// ------------------------------------------------------------------------
        /// <summary>
        /// XML file of the correct control view structure of a control
        /// </summary>
        /// ------------------------------------------------------------------------
        const string XMLCONTROLVIEW = "FootPrint_ControlView.xml";

        /// ------------------------------------------------------------------------
        /// <summary>
        /// XML file of the correct content view structure of a control
        /// </summary>
        /// ------------------------------------------------------------------------
        const string XMLCONTENTVIEW = "FootPrint_ContentView.xml";

        /// ------------------------------------------------------------------------
        /// <summary>
        /// Stores the XML for properties that is pulled from the resource file
        /// </summary>
        /// ------------------------------------------------------------------------
        XmlDocument _propertiesDoc = null;

        /// ------------------------------------------------------------------------
        /// <summary>
        /// Stores the XML for patterns that is pulled from the resource file
        /// </summary>
        /// ------------------------------------------------------------------------
        XmlDocument _patternsDoc = null;

        /// ------------------------------------------------------------------------
        /// <summary>
        /// Stores the XML for structure of the control in control view
        /// </summary>
        /// ------------------------------------------------------------------------
        XmlDocument _controlViewDoc = null;

        /// ------------------------------------------------------------------------
        /// <summary>
        /// Stores the XML for structure of the control in content view
        /// </summary>
        /// ------------------------------------------------------------------------
        XmlDocument _contentViewDoc = null;

        /// ------------------------------------------------------------------------
        /// <remarks>
        /// Used to determine which view we want to test of the control
        /// </remarks>
        /// ------------------------------------------------------------------------
        internal enum Views
        {
            Content,
            Control,
            Raw
        }

        /// ------------------------------------------------------------------------
        /// <summary>
        /// Used to display multiple messages in the same test run for control footprinting
        /// </summary>
        /// ------------------------------------------------------------------------
        string _errorBuffer;

        /// ------------------------------------------------------------------------
        /// <summary>
        /// Used in conjunction with _errorBuffer to list all the errors
        /// </summary>
        /// ------------------------------------------------------------------------
        int _itemCount;

        /// ------------------------------------------------------------------------
        /// <summary>
        /// Used to reference into the XML files for FrameworkId
        /// </summary>
        /// ------------------------------------------------------------------------
        const string DEFAULT_FRAMEWORK = "Default";


        #endregion Variables

        /// ------------------------------------------------------------------------
        /// <summary></summary>
        /// ------------------------------------------------------------------------
        public ControlObject(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, TypeOfPattern.Unknown, dirResults, testEvents, commands)
        {
            _testCaseSampleType = TestCaseSampleType.Control;
            _appCommands = commands;
        }

        #region StructureChange

        #region StructureChange Tests

        /// ------------------------------------------------------------------------
        /// <summary>
        /// <summary>TestCase: BulkRemove.1</summary>
        /// </summary>
        /// ------------------------------------------------------------------------
        [TestCaseAttribute("BulkRemove.1",
            TestSummary = "Remove many items and verify that ChildrenBulkRemoved is fired",
            Priority = TestPriorities.Pri1,
            TestCaseType = TestCaseType.Events, EventTested = "StructureChangedEventHandler(StructureChangeType.ChildrenBulkRemoved)",
            Status = TestStatus.Works,
            Author = "Automation",
            Description = new string[] {
                "Precondition: Determine if the application supports the custom interface callback for specific test applications",
                "Precondition: Determine is this appliction supports driving structure changes",
                "Precondition: Determine if the specific element supports structure change",
                "Step: Add LogicalTreeStructureListener",
                "Step: Bulk remove elements, causing a StructureChangeType.ChildrenBulkRemoved to occur",
                "Step: Wait for the LogicalStructureChangedEvent to be fired",
                "Step: That LogicalStructureChangedEvent was fired",
                "Step: Reset the control to it's initial state",
			})]
        public void TestBulkRemove(TestCaseAttribute testAttribute)
        {
            HeaderComment(testAttribute);
            TestLogicalStructureChangeEvent(StructureChangeType.ChildrenBulkRemoved);
        }

        /// <summary>
        /// <summary>TestCase: BulkAdd.1</summary>
        /// </summary>
        [TestCaseAttribute("BulkAdd.1",
            TestSummary = "Add many items and verify that ChildrenBulkAdded is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works, 
            TestCaseType = TestCaseType.Events, EventTested = "StructureChangedEventHandler(StructureChangeType.ChildrenBulkAdded)",
            Author = "Automation",
            Description = new string[] {
                "Precondition: Determine if the application supports the custom interface callback for specific test applications",
                "Precondition: Determine is this appliction supports driving structure changes",
                "Precondition: Determine if the specific element supports structure change",
                "Step: Add LogicalTreeStructureListener",
                "Step: Bulk add elements, causing a StructureChangeType.ChildrenBulkAdded to occur",
                "Step: Wait for the LogicalStructureChangedEvent to be fired",
                "Step: That LogicalStructureChangedEvent was fired",
                "Step: Reset the control to it's initial state",
        })]
        public void TestBulkAdd(TestCaseAttribute testAttribute)
        {
            HeaderComment(testAttribute);
            TestLogicalStructureChangeEvent(StructureChangeType.ChildrenBulkAdded);
        }

        /// <summary>
        /// <summary>TestCase: ChildAdd.1</summary>
        /// </summary>
        [TestCaseAttribute("ChildAdd.1",
            TestSummary = "Add an item and verify that ChildAdded is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            BugNumbers = new BugIssues[] {BugIssues.PS31 },
            TestCaseType = TestCaseType.Events, EventTested = "StructureChangedEventHandler(StructureChangeType.ChildAdd)",
            Author = "Automation",
            Description = new string[] {
                "Precondition: Determine if the application supports the custom interface callback for specific test applications",
                "Precondition: Determine is this appliction supports driving structure changes",
                "Precondition: Determine if the specific element supports structure change",
                "Step: Add LogicalTreeStructureListener",
                "Step: Add element, causing a StructureChangeType.ChildAdded to occur",
                "Step: Wait for the LogicalStructureChangedEvent to be fired",
                "Step: That LogicalStructureChangedEvent was fired",
                "Step: Reset the control to it's initial state",
        })]
        public void TestChildAdd(TestCaseAttribute testAttribute)
        {
            HeaderComment(testAttribute);
            TestLogicalStructureChangeEvent(StructureChangeType.ChildAdded);
        }

        /// <summary>
        /// <summary>TestCase: ChildRemove.1</summary>
        /// </summary>
        [TestCaseAttribute("ChildRemove.1",
            TestSummary = "Remove an item and verify that ChildRemoved is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            BugNumbers = new BugIssues[] {BugIssues.PS31 },
            TestCaseType = TestCaseType.Events, EventTested = "StructureChangedEventHandler(StructureChangeType.ChildRemoved)",
            Author = "Automation",
            Description = new string[] {
                "Precondition: Determine if the application supports the custom interface callback for specific test applications",
                "Precondition: Determine is this appliction supports driving structure changes",
                "Precondition: Determine if the specific element supports structure change",
                "Step: Add LogicalTreeStructureListener",
                "Step: Remove an element, causing a StructureChangeType.ChildRemoved to occur",
                "Step: Wait for the LogicalStructureChangedEvent to be fired",
                "Step: That LogicalStructureChangedEvent was fired",
                "Step: Reset the control to it's initial state",
        })]
        public void TestChildRemove(TestCaseAttribute testAttribute)
        {
            HeaderComment(testAttribute);
            TestLogicalStructureChangeEvent(StructureChangeType.ChildRemoved);
        }

        /// <summary>
        /// <summary>TestCase: Reorder.1</summary>
        /// </summary>
        [TestCaseAttribute("Reorder.1",
            TestSummary = "Remove the items and verify that ChildrenReordered is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            TestCaseType = TestCaseType.Events, EventTested = "StructureChangedEventHandler(StructureChangeType.ChildrenReordered)",
            Author = "Automation",
            Description = new string[] {
                "Precondition: Determine if the application supports the custom interface callback for specific test applications",
                "Precondition: Determine is this appliction supports driving structure changes",
                "Precondition: Determine if the specific element supports structure change",
                "Step: Add LogicalTreeStructureListener",
                "Step: Reorder the elements, causing a StructureChangeType.ChildrenReordered to occur",
                "Step: Wait for the LogicalStructureChangedEvent to be fired",
                "Step: That LogicalStructureChangedEvent was fired",
                "Step: Reset the control to it's initial state",
        })]
        public void TestReorder(TestCaseAttribute testAttribute)
        {
            HeaderComment(testAttribute);
            TestLogicalStructureChangeEvent(StructureChangeType.ChildrenReordered);
        }

        /// <summary>
        /// <summary>TestCase: Invalidate.1</summary>
        /// </summary>
        [TestCaseAttribute("Invalidate.1",
            TestSummary = "Invalidate the combo box and verify that ChildrenInvalidated is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            TestCaseType = TestCaseType.Events, EventTested = "StructureChangedEventHandler(StructureChangeType.ChildrenInvalidated)",
            Author = "Automation",
            Description = new string[] {
                "Precondition: Determine if the application supports the custom interface callback for specific test applications",
                "Precondition: Determine is this appliction supports driving structure changes",
                "Precondition: Determine if the specific element supports structure change",
                "Step: Add LogicalTreeStructureListener",
                "Step: Invalidate the elements, causing a StructureChangeType.ChildrenInvalidated to occur",
                "Step: Wait for the LogicalStructureChangedEvent to be fired",
                "Step: That LogicalStructureChangedEvent was fired",
                "Step: Reset the control to it's initial state",
			})]
        public void TestInvalidate(TestCaseAttribute testAttribute)
        {
            HeaderComment(testAttribute);
            TestLogicalStructureChangeEvent(StructureChangeType.ChildrenInvalidated);
        }
        #endregion IWUILogicalStructureChangedEvent


        /// -------------------------------------------------------------------
        /// <summary>
        /// Use the application's callback to cause a structure change to occur
        /// </summary>
        /// -------------------------------------------------------------------
        void TS_CauseStructureChange(AutomationElement element, StructureChangeType structureChangeType, CheckType ct)
        {
            if (_structChange == null)
                _structChange = _appCommands.GetIWUIStructureChange();

            _structChange.CauseStructureChange(element, structureChangeType);

            Comment(structureChangeType + " has occured");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Tests to see if the element supports testing the structure change events
        /// </summary>
        /// -------------------------------------------------------------------
        private void TS_ElementSupportChangeEvents(AutomationElement element, StructureChangeType structureChangeType, CheckType checkType)
        {
            if (_structChange == null)
                _structChange = _appCommands.GetIWUIStructureChange();

            if (!_structChange.DoesControlSupportStructureChange(element, structureChangeType))
                ThrowMe(checkType);

            Comment("Control supports testing " + structureChangeType.ToString());

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Main driver for the tests to call that will cause some structure 
        /// change even to occur.
        /// </summary>
        /// -------------------------------------------------------------------
        internal void TestLogicalStructureChangeEvent(StructureChangeType structureChangeType)
        {

            EventObject.EventList = new ArrayList();
            Exception caughtException = null;
            try
            {
                // 0) Precondition: Determine if the application supports the custom interface callback for specific test applications
                TS_SupportsApplicationCallback(CheckType.IncorrectElementConfiguration);

                // 1) Precondition: Determine is this appliction supports driving structure changes
                TS_SupportsStructureChangeEvents(CheckType.IncorrectElementConfiguration);

                // 2) Precondition: Determine if the specific element supports structure change
                TS_ElementSupportChangeEvents(m_le, structureChangeType, CheckType.IncorrectElementConfiguration);

                // 4) Step: Add LogicalTreeStructureListener
                TS_AddLogicalTreeStructureListener(true, m_le, TreeScope.Element, CheckType.Verification);

                // 5) Step: Invoke the 'Bulk Add' menu item	
                TS_CauseStructureChange(m_le, structureChangeType, CheckType.Verification);

                // 6) Step: Wait for the LogicalStructureChangedEvent to be fired
                TSC_WaitForEvents(1);

                // 7) Step: That LogicalStructureChangedEvent was fired
                if (m_le.Current.FrameworkId.ToLower() == "win32" &&
                    m_le.Current.ControlType == ControlType.List &&
                    (structureChangeType == StructureChangeType.ChildAdded || structureChangeType == StructureChangeType.ChildRemoved))
                {
                    // Win32 controls can't determine when ChildAdd?Remove happens so fire a ChildInvalidated instead
                    TS_VerifyLogicalStructureChangedEventArgs(m_le, EventFired.True, StructureChangeType.ChildrenInvalidated, CheckType.Verification);
                }
                else
                {
                    if (!TS_FilterOnBug(m_TestCase))
                        TS_VerifyLogicalStructureChangedEventArgs(m_le, EventFired.True, structureChangeType, CheckType.Verification);
                }
            }
            catch (Exception exception)
            {
                caughtException = exception;
            }
            finally
            {
                if (caughtException is IncorrectElementConfigurationForTestException)
                {
                    m_TestStep++;
                }
                else
                {
                    // 3) Step: Reset the control to it's initial state
                    TS_ResetControlToInitialState(m_le, CheckType.Verification);
                }
                if (null != caughtException)
                    throw caughtException;
            }

        }

        #endregion StructureChange

        #region FootPrint

        const string XML_WARN_IF_NULL = "!NULLW";    // warn if not !null
        const string XML_WARN_IF_NOT_NULL = "NULLW";    // warn if not null

        // _tbl_OptionsPattern
        const string XML_NEVER = "N";
        const string XML_OPTIONAL = "D"; //Depends
        const string XML_REQUIRED = "R";

        // _tbl_OptionsProperties
        const string XML_FALSE = "F";
        const string XML_TRUE = "T";
        const string XML_NON_EMPTY_STRING = "S";
        const string XML_NaN = "NAN";
        const string XML_NOT_NaN = "!NAN";
        const string XML_NULL = "NULL";
        const string XML_NOTNULL = "!NULL";

        /// ---------------------------------------------------------------
        /// <summary>
        /// Test Content View
        /// </summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Test Control's Control View",
            Priority = TestPriorities.Pri2,
            TestCaseType = TestCaseType.Generic,
            Status = TestStatus.Works,  
            Author = "Automation",
            Description = new string[] {
                "Test Control's Control View",
            })]
        public void TestControlView(TestCaseAttribute testAttribute)
        {
            HeaderComment(testAttribute);

            TS_TestControlView();
        }
        /// ---------------------------------------------------------------
        /// <summary>
        /// Test Control Content View
        /// </summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Test Control's Content View",
            Priority = TestPriorities.Pri2,
            TestCaseType = TestCaseType.Generic,
           Status = TestStatus.Works,  
            Author = "Automation",
            Description = new string[] {
                "Test Control's Content View",
            })]
        public void TestContentView(TestCaseAttribute testAttribute)
        {
            HeaderComment(testAttribute);

            TS_TestContentView();
        }
        /// ---------------------------------------------------------------
        /// <summary>
        /// Test Control Properties
        /// </summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Test Control's Properties",
            Priority = TestPriorities.Pri2,
            TestCaseType = TestCaseType.Generic,
            Status = TestStatus.Works,   
            Author = "Automation",
            Description = new string[] {
                "Test the control's properties",
            })]
        public void TestControlProperties(TestCaseAttribute testAttribute)
        {
            HeaderComment(testAttribute);

            TS_TestControlProperties();
        }
        /// ---------------------------------------------------------------
        /// <summary>
        /// Test Control Patterns
        /// </summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Test Control's Patterns",
            Priority = TestPriorities.Pri2,
            TestCaseType = TestCaseType.Generic,
           Status = TestStatus.Works,   
            Author = "Automation",
            Description = new string[] {
                "Test the control's patterns",
            })]
        public void TestControlPatterns(TestCaseAttribute testAttribute)
        {
            HeaderComment(testAttribute);

            TS_TestControlPatterns();
        }
        /// -------------------------------------------------------------------------
        /// <summary>
        /// Common code that is called from the Control tests to verify the properties associated with a control
        /// </summary>
        /// -------------------------------------------------------------------------
        internal void TS_TestControlProperties()
        {
            if (CheckWPFDataGridElement(m_le)) // 
                return; 

            string[] frameworks = new string[] { m_le.Current.FrameworkId, DEFAULT_FRAMEWORK };

            ControlType ctType = m_le.Current.ControlType;
            XmlNode node;

            // Things such as GetBoundingRect, GetClickablePoint return diffent values if they are on or off the viewable portion, 
            // so bail out with an invalid configuration
            if (m_le.Current.IsOffscreen == true)
            {
                // If we can scroll into view, try it
                if ((bool)m_le.GetCurrentPropertyValue(AutomationElement.IsScrollItemPatternAvailableProperty))
                {
                    ScrollItemPattern sip = m_le.GetCurrentPattern(ScrollItemPattern.Pattern) as ScrollItemPattern;
                    try
                    {
                        Comment("Scrolling element({0}) into view", Library.GetUISpyLook(m_le));
                        sip.ScrollIntoView();
                    }
                    catch (InvalidOperationException)
                    {
                        ThrowMe(CheckType.IncorrectElementConfiguration, "Cannot test properties for elements that are not visible");
                    }
                }
                if (m_le.Current.IsOffscreen == true)
                    ThrowMe(CheckType.IncorrectElementConfiguration, "Cannot test properties for elements that are not visible");
                Comment("Element(" + Library.GetUISpyLook(m_le) + ") is now visible for testing");
            }

            // There is a better way of doing this, by quering the XML for specifics, but need to get this written
            ArrayList seenProps = new ArrayList();
            string errors = "";
            string errorWarnings = "";

            foreach (string frameWorkId in frameworks)
            {
                foreach (XmlNode n in GetNodes(XMLPROPERTIES, ctType, frameWorkId))
                {
                    node = n;
                    node = node.NextSibling;

                    if (node == null)
                        return;



                    // Go through all the XML nodes for the properties that this control supports.
                    // This will only test the properties that it finds in the XML.
                    while (node != null)
                    {
                        string property = new StringBuilder(node.Name).Replace("_", ".").ToString();
                        AutomationProperty ap = Helpers.GetPropertyByName(property);
                        if (seenProps.IndexOf(ap.ProgrammaticName) == -1)
                        {
                            seenProps.Add(ap.ProgrammaticName);

                            string patternName = property.Substring(0, property.IndexOf("."));

                            if ((patternName == "AutomationElement") || (PatternSupported(patternName)))
                            {

                                object currentValue = m_le.GetCurrentPropertyValue(ap);

                                Comment("Validating {0}.GetCurrentPropertyValue({1})={2}", ctType.ProgrammaticName, property, currentValue);

                                switch (node.InnerText.ToUpper(CultureInfo.CurrentCulture))
                                {
                                    case XML_NON_EMPTY_STRING:
                                        {
                                            if (currentValue is string)
                                            {
                                                string obj = (string)currentValue;
                                                if (String.IsNullOrEmpty(obj))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": IsNullOrEmpty() returned true but should have been a valid string\n";
                                            }
                                            else
                                            {
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": did not return correct data type, but returned " + currentValue.GetType() + "\n";
                                            }
                                            break;
                                        }
                                    case XML_FALSE:
                                        {
                                            if (currentValue is bool)
                                            {
                                                bool obj = (bool)currentValue;
                                                if (obj != false)
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": did not return a false, but returned " + obj + "\n";
                                            }
                                            else
                                            {
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": did not return correct data type, but returned " + currentValue.GetType() + "\n";
                                            }
                                            break;
                                        }
                                    case XML_TRUE:
                                        {
                                            if (currentValue is bool)
                                            {
                                                bool obj = (bool)currentValue;
                                                if (obj != true)
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": did not return a true, but returned " + obj + "\n";
                                            }
                                            else
                                            {
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": did not return correct data type, but returned " + currentValue.GetType() + "\n";
                                            }
                                            break;
                                        }
                                    case XML_REQUIRED:
                                        {
                                            if (currentValue == null || currentValue == AutomationElement.NotSupported)
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": did not return a valid value, but returned " + (currentValue == null ? "<null>" : currentValue) + "\n";
                                            ;

                                            break;
                                        }
                                    case XML_NEVER:
                                        {
                                            if (currentValue != null || currentValue != AutomationElement.NotSupported)
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": did not return AutomationElement.NotSupported but returned " + currentValue + "\n";

                                            break;
                                        }
                                    case XML_NULL:
                                        {
                                            if (currentValue != null)
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": was expected to return null but returned (" + currentValue + ")\n";

                                            break;
                                        }

                                    case XML_WARN_IF_NOT_NULL:
                                        {
                                            if (currentValue != null)
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errorWarnings = "// " + property + ": was expected to return null but did not" + "\n";

                                            break;
                                        }

                                    case XML_NOTNULL:
                                        {
                                            if (currentValue == null)
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": returned null and was expected to have a valid value" + "\n";

                                            break;
                                        }
                                    case XML_WARN_IF_NULL:
                                        {
                                            if (currentValue == null)
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errorWarnings = "// " + property + ": returned null and was expected to have a valid value" + "\n";

                                            break;
                                        }

                                    case XML_NaN:
                                        {
                                            if (currentValue == null)
                                            {
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": returned 'null' and should have returned double.NaN\n";
                                            }
                                            else if (currentValue is Point)
                                            {
                                                Point obj = (Point)currentValue;
                                                if (!IsNaN(obj.X))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Point.X returned '" + obj.X + "' and should have returned double.NaN\n";
                                                if (!IsNaN(obj.Y))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Point.Y returned '" + obj.Y + "' and should have returned double.NaN\n";
                                            }
                                            else if (currentValue is Rect)
                                            {
                                                Rect obj = (Rect)currentValue;
                                                if (!IsNaN(obj.Bottom))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Rect.Bottom returned '" + obj.Bottom + "' and should have returned double.NaN\n";
                                                if (!IsNaN(obj.Top))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Rect.Top returned '" + obj.Top + "' and should have returned double.NaN\n";
                                                if (!IsNaN(obj.Left))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Rect.Left returned '" + obj.Left + "' and should have returned double.NaN\n";
                                                if (!IsNaN(obj.Right))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Rect.Right returned '" + obj.Right + "' and should have returned double.NaN\n";
                                            }
                                            else if (!IsNaN(currentValue))
                                            {
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": returned NaN and should have" + "\n";
                                            }
                                            break;
                                        }
                                    case XML_NOT_NaN:
                                        {
                                            if (currentValue == null)
                                            {
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": returned 'null' and should not have returned double.NaN\n";
                                            }
                                            else if (currentValue is Point)
                                            {
                                                Point obj = (Point)currentValue;
                                                if (IsNaN(obj.X))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Point.X should not have returned double.NaN" + "\n";
                                                if (IsNaN(obj.Y))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Point.Y should not have returned double.NaN" + "\n";
                                            }
                                            else if (currentValue is Rect)
                                            {
                                                Rect obj = (Rect)currentValue;
                                                if (IsNaN(obj.Bottom))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Rect.Bottom should not have returned double.NaN" + "\n";
                                                if (IsNaN(obj.Top))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Rect.Top should not have returned double.NaN" + "\n";
                                                if (IsNaN(obj.Left))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Rect.Left should not have returned double.NaN" + "\n";
                                                if (IsNaN(obj.Right))
                                                    if (!TS_FilterOnBug(m_le, ap, false))
                                                        errors += "// " + property + ": Rect.Right should not have returned double.NaN" + "\n";
                                            }
                                            else if (IsNaN(currentValue))
                                            {
                                                if (!TS_FilterOnBug(m_le, ap, false))
                                                    errors += "// " + property + ": returned NaN and should not have" + "\n";
                                            }
                                            break;
                                        }
                                    default:

                                        throw new Exception("// " + property + ": Bad fomatted XML, need to handle " + node.InnerText + " for " + property);
                                }
                            }
                        }
                        
                        node = node.NextSibling;
                    }
                }
            }

            if (!string.IsNullOrEmpty(errors))
                ThrowMe(CheckType.Verification, errors.Substring(0, errors.LastIndexOf("\n")));

            if (!string.IsNullOrEmpty(errorWarnings))
                ThrowMe(CheckType.Verification, errorWarnings);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns wether the number is not a number
        /// </summary>
        /// -------------------------------------------------------------------
        private bool IsNaN(object currentValue)
        {
            if (currentValue is Single)
                return (Single.IsNaN((Single)currentValue));
            if (currentValue is Double)
                return (Double.IsNaN((double)currentValue));
            ThrowMe(CheckType.Verification, "UIV needs to handle the type(" + currentValue.GetType().ToString());
            return false;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Calls GetPropertyValue for AutomationElement.Is*PatternAvailableProperty
        /// </summary>
        /// -------------------------------------------------------------------
        private bool PatternSupported(string patternName)
        {
            patternName = "AutomationElement.Is" + patternName + "AvailableProperty";
            AutomationProperty ap = Helpers.GetPropertyByName(patternName);
            return (bool)m_le.GetCurrentPropertyValue(ap);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_TestControlPatterns()
        {
            if (CheckWPFDataGridElement(m_le)) // 
                return; 

            ControlType ctType = m_le.Current.ControlType;
            string temp = "";

            // Get the 1st node in the pattern list
            foreach (XmlNode n in GetNodes(XMLPATTERNS, ctType, DEFAULT_FRAMEWORK))
            {
                XmlNode node = n;

                if (node == null)
                    return;

                // Get the patterns that the control currently implements
                AutomationPattern[] patterns = m_le.GetSupportedPatterns();

                // Set up the diffent buckets
                ArrayList patternsFoundOnControl = new ArrayList();
                ArrayList patternsRequiredOnControl = new ArrayList();
                ArrayList patternsOptionalOnControl = new ArrayList();
                ArrayList patternsNeverAllowedOnControl = new ArrayList();

                // get the current patterns and add them to the list
                if (patterns.Length > 0)
                {
                    Comment("Found the following pattern(s) implemented on the control:");

                    foreach (AutomationPattern ap in patterns)
                    {
                        temp = Automation.PatternName(ap) + "Pattern";
                        Comment("   -" + temp);
                        patternsFoundOnControl.Add(temp);
                    }
                }

                // place the correct XML node in the correct array list
                node = node.NextSibling;

                while (node != null)
                {
                    switch (node.InnerText.ToUpper(CultureInfo.CurrentCulture))
                    {
                        case XML_REQUIRED:
                            patternsRequiredOnControl.Add(node.Name);
                            break;
                        case XML_OPTIONAL:
                            patternsOptionalOnControl.Add(node.Name);
                            break;
                        case XML_NEVER:
                            patternsNeverAllowedOnControl.Add(node.Name);
                            break;
                        default:
                            throw new Exception("Need to handle " + node.InnerText);
                    }

                    node = node.NextSibling;
                }

                if (patternsRequiredOnControl.Count > 0)
                {
                    Comment("Control is required to support the following patterns: ");
                    foreach (string s in patternsRequiredOnControl)
                        Comment("   -" + s);
                }

                if (patternsOptionalOnControl.Count > 0)
                {
                    Comment("Control can optionally support the following patterns: ");
                    foreach (string s in patternsOptionalOnControl)
                        Comment("   -" + s);
                }

                if (patternsNeverAllowedOnControl.Count > 0)
                {
                    Comment("Control should never support the following patterns: ");
                    foreach (string s in patternsNeverAllowedOnControl)
                        Comment("   -" + s);
                }

                StringBuilder errorText = new StringBuilder();

                // Test required not in supported patterns
                foreach (string pattern in patternsRequiredOnControl)
                {
                    if (!patternsFoundOnControl.Contains(pattern))
                    {
                        errorText.Append("Control should have supported pattern(" + pattern + ")");
                    }
                }

                // Test pattern that should never be supported
                foreach (string pattern in patternsNeverAllowedOnControl)
                {
                    if (patternsFoundOnControl.Contains(pattern))
                        errorText.Append("Control should never support pattern(" + pattern + ")");
                }

                // If we found an error, then report it
                if (!string.IsNullOrEmpty(errorText.ToString()))
                    ThrowMe(CheckType.Verification, errorText.ToString());
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Called from the controls test to test the structure of the 
        /// ControlView
        /// </summary>
        /// -------------------------------------------------------------------
        internal void TS_TestControlView()
        {
            if (CheckWPFDataGridElement(m_le)) // 
                return; 

            TestContentControlViewHelper(XMLCONTROLVIEW, m_le, "");
            if (_errorBuffer.Length > 0)
                ThrowMe(CheckType.Verification, _errorBuffer);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Called from the controls test to test the structure of the 
        /// ContentView
        /// </summary>
        /// -------------------------------------------------------------------
        internal void TS_TestContentView()
        {
            if (CheckWPFDataGridElement(m_le)) // 
                return; 

            TestContentControlViewHelper(XMLCONTENTVIEW, m_le, "");
            if (_errorBuffer.Length > 0)
                ThrowMe(CheckType.Verification, _errorBuffer);

            m_TestStep++;
        }


        /// -------------------------------------------------------------------
        /// <summary>
        /// Helper for TestControlViuew and TestContentView
        /// </summary>
        /// -------------------------------------------------------------------
        internal void TestContentControlViewHelper(string xmlFileName, AutomationElement element, string indent)
        {
            _errorBuffer = "";
            _itemCount = 0; //reset how many erros we get

            foreach (XmlNode node in GetNodes(xmlFileName, element.Current.ControlType, DEFAULT_FRAMEWORK))
            {
                bool process = true;
                // Check to see if the proeprty requires a specific
                // pattern to be supported.  If it does and the element supports the 
                // pattern, then test the structure 
                if (node.NextSibling != null && node.NextSibling.Name == "PatternID")
                {
                    // Pattern specific, tests to see if we should be testing this
                    AutomationProperty ap = Helpers.GetPropertyByName("AutomationElement.Is" + node.NextSibling.InnerText + "AvailableProperty");
                    if (!(bool)element.GetCurrentPropertyValue(ap))
                        process = false; // Doesn't support the pattern
                }
                if (process)
                {
                    // Call into the recursive test method
                    TestStructure(node.NextSibling, element, xmlFileName, Views.Control, indent);
                }
            }
        }

        // -------------------------------------------------------------------
        // Test the structure of the element and it's children recursively 
        // according to the xml file.
        // -------------------------------------------------------------------
        internal void TestStructure(XmlNode node, AutomationElement element, string xmlDocname, Views view, string preBuffer)
        {
            if (node == null)
                return;

            string control = Library.GetUISpyLook(element);
            Comment(preBuffer + "Element: " + control);

            // Test all the immediate children for correctness
            for (; node != null; node = node.NextSibling)
            {
                string name = node.Name;
                string value = node.InnerText;

                Comment(preBuffer + "There should be " + value + " '" + name + "' child elements on the control: " + control);
                ControlType controlType = Helpers.GetControlTypeByName(name);

                // This checks the actual number of children for the determined ControlType.  It
                // requires the format of the XmlNode to be in the following format:
                //
                //          {num}[{num}]
                //  of
                //          {num+}
                //
                // So an example would be
                //
                //          {0}{1}{2}
                //
                // meaning that there can be 1, or 2 or 3 of these ControlType, but nothing else.
                //
                // Another exmple would be
                //
                //          {1+}
                //
                // Meaning that there needs to be 1 or more of the ControlType.
                //
                if (controlType != null)
                {
                    string[] param = value.Split('{', '}');
                    System.Windows.Automation.Condition cond = null;

                    // ContentView will only pull in elements where then IsContentElementProperty = true
                    // ControlView will only pull in any elements where then IsContentElementProperty = {false|true}
                    switch (view)
                    {
                        case Views.Content:
                            cond = new AndCondition(
                                new PropertyCondition(AutomationElement.ControlTypeProperty, controlType),
                                new PropertyCondition(AutomationElement.IsContentElementProperty, true)
                                    );
                            break;
                        case Views.Control:
                            cond = new PropertyCondition(AutomationElement.ControlTypeProperty, controlType);
                            break;
                        case Views.Raw:
                            throw new NotImplementedException();
                    }

                    AutomationElementCollection aec = element.FindAll(TreeScope.Children, cond);

                    if (param.Length == 3 && param[1].IndexOf('+') != -1)
                    {// {num+}   
                        string num = param[1];
                        int minNum = Convert.ToInt16(num.Substring(0, num.IndexOf("+")), CultureInfo.CurrentCulture); //already did a check so will be a valid number

                        if (minNum > aec.Count)
                        {   // BAD, needs to be equal or greater to the number given
                            _errorBuffer += "ERROR_" + ++_itemCount + ":  " + element.Current.ControlType.ProgrammaticName + " should have at least " + minNum + " " + name + ", but only has " + aec.Count + ".  ";
                            //pass = false;
                        }
                        else
                        {   // GOOD
                            //pass = true;
                            switch (aec.Count)
                            {
                                case 1:
                                    Comment(preBuffer + "...There is " + aec.Count + " " + name);
                                    break;
                                case 0:
                                default:
                                    Comment(preBuffer + "...There are " + aec.Count + " " + name + "s");
                                    break;
                            }
                        }
                    }
                    else
                    {// {num}[{num}]   
                        // Is the count correct?
                        if (Array.IndexOf(param, aec.Count.ToString(CultureInfo.CurrentCulture)) != -1)
                        {   // GOOD
                            switch (aec.Count)
                            {
                                case 1:
                                    Comment(preBuffer + "...There is " + aec.Count + " " + name);
                                    break;
                                case 0:
                                default:
                                    Comment(preBuffer + "...There are " + aec.Count + " " + name + "s");
                                    break;
                            }
                            //pass = true;
                        }
                        else
                        {   // BAD

                            // Basically build up the string for diagnostics
                            StringBuilder buffer = new StringBuilder("in \"" + view.ToString() + " View\", ");
                            string OR = " or ";
                            foreach (string s in param)
                            {
                                if (!string.IsNullOrEmpty(s))
                                    buffer.Append(s + " or ");
                            }
                            buffer.Remove(buffer.Length - OR.Length, OR.Length);

                            _errorBuffer += "ERROR_" + ++_itemCount + ":  " + element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) + " should have had " + buffer + " control(s) with ControlType = " + Helpers.GetProgrammaticName(controlType) + ", but had " + aec.Count + ".  ";
                            //pass = false;
                        }
                    }
                    if (aec.Count > 0)
                    {
                        int count = 0;
                        string div = "---------------------------------------------------------------------------------".Substring(0, 80 - preBuffer.Length);
                        foreach (AutomationElement ae in aec)
                        {
                            // Call into the recursive test method
                            foreach (XmlNode xmlNode in GetNodes(xmlDocname, controlType, DEFAULT_FRAMEWORK))
                            {
                                Comment(div);
                                string test = "- Child #" + ++count + " " + Helpers.GetProgrammaticName(ae.Current.ControlType);
                                Comment(preBuffer + "Start " + test);
                                XmlNode x = xmlNode;
                                if (x != null)
                                {
                                    x = x.NextSibling;

                                    if (x != null)
                                        TestContentControlViewHelper(xmlDocname, ae, preBuffer + "     ");
                                    //TestStructure(x, /* GetNodes(xmlDocname, controlType).NextSibling*/ ae, xmlDocname, view, "   " + preBuffer);
                                }
                                Comment(preBuffer + "Done  " + test);
                                Comment(div);
                            }
                        }
                    }
                }
            }
        }
        #endregion FootPrint

        #region helpers

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        XmlNodeList GetNodes(string xmlDoc, ControlType controlType, string frameworkId)
        {

            // If this is a custom, no need to go further since I don't know what it supports
            if (controlType == ControlType.Custom)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Can't determine the ControlType to check for correct properties");

            XmlDocument doc = null;

            // If the document has not been initialized yet, get it
            string rootNode = String.Empty;
            switch (xmlDoc)
            {
                case XMLPROPERTIES:
                    {
                        if (_propertiesDoc == null)
                        {
                            _propertiesDoc = new XmlDocument();
                            _propertiesDoc.LoadXml(Helpers.GetResourceString(xmlDoc));
                        }
                        doc = _propertiesDoc;
                        break;
                    }

                case XMLPATTERNS:
                    {
                        if (_patternsDoc == null)
                        {
                            _patternsDoc = new XmlDocument();
                            _patternsDoc.LoadXml(Helpers.GetResourceString(xmlDoc));
                        }
                        doc = _patternsDoc;
                        break;
                    }

                case XMLCONTROLVIEW:
                    {
                        if (_controlViewDoc == null)
                        {
                            _controlViewDoc = new XmlDocument();
                            _controlViewDoc.LoadXml(Helpers.GetResourceString(xmlDoc));
                        }
                        doc = _controlViewDoc;
                        break;
                    }

                case XMLCONTENTVIEW:
                    {
                        if (_contentViewDoc == null)
                        {
                            _contentViewDoc = new XmlDocument();
                            _contentViewDoc.LoadXml(Helpers.GetResourceString(xmlDoc));
                        }
                        doc = _contentViewDoc;
                        break;
                    }

                default:
                    throw new Exception("Need to handle " + xmlDoc);
            }

            XmlNodeList list;

            string controlTypeName = Helpers.GetProgrammaticName(controlType);

            // Try the specific framework 1st, if we don't find anything specific about 
            // this controltype, then use the default implentation.
            //list = doc.SelectNodes("//ProviderControlType[. = \"" + m_le.Current.FrameworkId + "." + controlTypeName + "\"]");
            list = doc.SelectNodes("//ProviderControlType[. = \"" + frameworkId + "." + controlTypeName + "\"]");

            //if (list.Count == 0)
            //{
            //    // If we don't get anything, then try the default
            //    list = doc.SelectNodes("//ProviderControlType[. = \"" + "Default." + controlTypeName + "\"]");
            //}

            return list;

        }
        #endregion helpers
    }
}
