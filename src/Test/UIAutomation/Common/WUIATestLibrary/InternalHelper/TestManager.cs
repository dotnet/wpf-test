// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
* Purpose: 
* Owner: Microsoft
* Contributors:
*
* Change Number:    $Change:  $
* Revision:         $Revision: 1 $

* Filename:         $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/Common/Library/Microsoft/Test/WindowsUIAutomation/Logging/TestAtributes.cs
*******************************************************************/
using System;
using System.IO;
using System.Collections;

namespace Microsoft.Test.WindowsUIAutomation.TestManager
{
    using Microsoft.Test.WindowsUIAutomation;


    /// -------------------------------------------------------------------
    /// <summary>Class used for the BugIssues enumerations</summary>
    /// -------------------------------------------------------------------
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    class BugDescriptionAttribute : Attribute
    {
        string _description = string.Empty;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public string Description
        {
            get { return _description; }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public BugDescriptionAttribute(string problemDescription)
        {
            _description = problemDescription;
        }
    }

    /// -----------------------------------------------------------------------
    /// <summary>Enumeration used for the TestCaseAttributes class</summary>
    /// -----------------------------------------------------------------------
    [Flags]
    public enum BugIssues
    {
        /// -------------------------------------------------------------------
        /// <summary>" Win32-RichEdit: Win32 RichEdit SetScrollPercent(100, -1) scrolls past 100%"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-RichEdit: SetScrollPercent(100, -1) scrolls past 100%")]
        PS1,

        /// -------------------------------------------------------------------
        /// <summary>"Win32-TextPattern-Any: TextPatten.TextSelectionChangedEvent not caught unless application has focus"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-TextPattern-Any: TextPatten.TextSelectionChangedEvent not caught unless application has focus")]
        PS2,
    
        /// -------------------------------------------------------------------
        /// <summary>" Win32-TextPattern-RichEdit: Move on empty range at end of document in Win32:RichEdit returns -1 instead of 0 for all TextUnits except Formt"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-TextPattern-RichEdit: Move on empty range at end of document in Win32:RichEdit returns -1 instead of 0 for all TextUnits except Formt")]
        PS3,

        /// -------------------------------------------------------------------
        /// <summary>"Win32-TextPattern-Any: TextPatternRange.Move(*,MaxInt/MinInt) returns inconsistent results across RichEdit and Edit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-TextPattern-Any: TextPatternRange.Move(*,MaxInt/MinInt) returns inconsistent results across RichEdit and Edit")]
        PS4,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-TextPattern-Any: TextPatternRange.Move(*,1) on DocumentRange returns 2 for some select TextUnits on partial document range"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-TextPattern-Any: TextPatternRange.Move(*,1) on DocumentRange returns 2 for some select TextUnits on partial document range")]
        PS5,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-Edit: HOT: FindText returns range that is trimmed by one character on Win32 Edit Control"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Edit: HOT: FindText returns range that is trimmed by one character on Win32 Edit Control")]
        PS6,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-Edit: HOT: MoveEndpointByUnit(End, TextUnit.Leine, 1 ) returning 1, not 0, for non-empty text range on Win32 Edit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Edit: HOT: MoveEndpointByUnit(End, TextUnit.Leine, 1 ) returning 1, not 0, for non-empty text range on Win32 Edit")]
        PS7,

        /// -------------------------------------------------------------------
        /// <summary>"Win32-TextPattern-Any: GetVisbileRange raises assertion for multi-line edit controls when last line of text is visible."</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-TextPattern-Any: GetVisbileRange raises assertion for multi-line edit controls when last line of text is visible.")]
        PS8,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-Edit: MoveEndpointByUnit(Start, TextUnit.Line, -1 ) returning 0, not -1, for non-empty multi-line text range on Win32 Edit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Edit: MoveEndpointByUnit(Start, TextUnit.Line, -1 ) returning 0, not -1, for non-empty multi-line text range on Win32 Edit")]
        PS9,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-RichEdit: TextPatternRange.Move(textunit,1) returns -1 for Win32 RichEdit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-RichEdit: TextPatternRange.Move(<textunit>,1) returns -1 for Win32 RichEdit")]
        PS10,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-Edit: TextPatternRange.GetVisibleRange() raises InvalidOperation exception on Win32 Edit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Edit: TextPatternRange.GetVisibleRange() raises InvalidOperation exception on Win32 Edit")]
        PS11,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-RichEdit: TextPatternRange.GetText(n) raises assertion on debug builds on Win32 RichEdit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-RichEdit: TextPatternRange.GetText(n) raises assertion on debug builds on Win32 RichEdit")]
        PS12,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-Edit: ScrollIntoView behavior is inconsistent for some combinations of range size on single line Win32 Edit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Edit: ScrollIntoView behavior is inconsistent for some combinations of range size on single line Win32 Edit")]
        PS13,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-RichEdit: ScrollIntoView behavior is inconsistent for some combinations of range size on single line Win32 RichEdit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-RichEdit: ScrollIntoView behavior is inconsistent for some combinations of range size on single line Win32 RichEdit")]
        PS14,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-RichEdit: Win32 (RichEdit) TextPatternRange.MoveEndpoint(Start, Document, 1) doesn't move start point entire length of document"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-RichEdit: TextPatternRange.MoveEndpoint(Start, Document, 1) doesn't move start point entire length of document")]
        PS15,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-Edit: Win32 (Edit) TextPatternRange.Move(Character, <netagive#>) has a off by one error in its return value for Win32:Edit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Edit: TextPatternRange.Move(Character, <netagive#>) has a off by one error in its return value for Win32:Edit")]
        PS16,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-RichEdit: Win32 (RichEdit) TextPatternRange.Move(..., <netagive#>) gives inconsistent and misleading behavior on Win32:RichEdit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-RichEdit: TextPatternRange.Move(..., <netagive#>) gives inconsistent and misleading behavior on Win32:RichEdit")]
        PS17,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-RichEdit: Win32 (RichEdit) TextPattenRange.Move(Character,1) across a \r retuns 0 instead of 1"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-RichEdit: TextPattenRange.Move(Character,1) across a \r retuns 0 instead of 1")]
        PS18,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-Edit: Win32 (Edit) MultiLine TextPatternRange:GetBoundingRectangles raises IndexOutofRangeException on multi-line Win32:Edit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Edit: MultiLine TextPatternRange:GetBoundingRectangles raises IndexOutofRangeException on multi-line Win32:Edit")]
        PS19,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-Edit: BVT BLOCKER: InvalidOperationException raised from TextPatternRange.GetText(-1) on Win32 Edit for Jpn Tablet XP SP2"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Edit: InvalidOperationException raised from TextPatternRange.GetText(-1) on Win32 Edit for Jpn Tablet XP SP2")]
        PS20,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-RichEdit: TextPattern.Move does not contain an entire text unit in Win32RichEdit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-RichEdit: TextPattern.Move does not contain an entire text unit in Win32RichEdit")]
        PS21,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-Edit: TextPattern.Move does not contain an entire text unit in Win32 Edit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Edit: TextPattern.Move does not contain an entire text unit in Win32 Edit")]
        PS22,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-Edit: TextPattern ScrollIntoView does not have consistent or reliable behavior for Win32 Edit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Edit: TextPattern ScrollIntoView does not have consistent or reliable behavior for Win32 Edit")]
        PS23,

        /// -------------------------------------------------------------------
        /// <summary>" Win32-RichEdit: TextPattern ScrollIntoView does not have consistent or reliable behavior for Win32 RichEdit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-RichEdit: TextPattern ScrollIntoView does not have consistent or reliable behavior for Win32 RichEdit")]
        PS24,

        /// -------------------------------------------------------------------
        /// <summary>" Win32:RichEdit: TEXTPATTERN: Examine usage of RichEdit Text Object Model for Move/Scroll functions."</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32:RichEdit: TEXTPATTERN: Examine usage of RichEdit Text Object Model for Move/Scroll functions.")]
        PS25,

        /// -------------------------------------------------------------------
        /// <summary>"Win32 Listview: Dataitem does not fire Value property change event"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Listview : Dataitem does not fire Value property change event")]
        PS26,

        /// -------------------------------------------------------------------
        /// <summary>"Name property is not getting fired on Win32 controls"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32: Name property is not getting fired on Win32 controls")]
        PS27,

        /// -------------------------------------------------------------------
        /// <summary>"Win32 TabItems does not fire FocusChange events"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-TabItem : Does not fire FocusChange events")]
        PS28,

        /// -------------------------------------------------------------------
        /// <summary> "Win32 ListItems do not fire FocusEvents althought KeyboardFocuasble = true"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-ListItem : Does not fire FocusEvents althought KeyboardFocuasble = true")]
        PS29,

        /// -------------------------------------------------------------------
        /// <summary> "(Win32 Combo) : Setting focus to a combo box or DataItem with an edit control does not propogate the focus event to the control, but only the edit"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("WIn32-ComboBox : Setting focus to a combo box or DataItem with an edit control does not propogate the focus event to the control, but only the edit")]
        PS30,

        /// -------------------------------------------------------------------
        /// <summary> "(Win32 Tree): When performing child addand child remove to a tree control, the propertychange event is getting fired on the tree item, not the control"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Tree : When performing child addand child remove to a tree control, the propertychange event is getting fired on the tree item, not the control")]
        PS31,

        /// -------------------------------------------------------------------
        /// <summary> "Win32 Hyperlink: No InvokedEvent is fired when hyperlink is invoked"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Hyperlink : No InvokedEvent is fired when hyperlink is invoked")]
        PS32,

        /// -------------------------------------------------------------------
        /// <summary> "AddToSelection of an item in a non-multiple selectable container (list box, tab, tree) should throw InvalidOperationException (Avalon and Proxies)"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32 : AddToSelection of an item in a non-multiple selectable container (list box, tab, tree) should throw InvalidOperationException (Avalon and Proxies)")]
        PS33,

        /// -------------------------------------------------------------------
        /// <summary> "UIAutomationCore.: Assert whith right popup menu on Desktop"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("UIAutomationCore : Assert whith right popup menu on Desktop")]
        PS34,

        /// -------------------------------------------------------------------
        /// <summary> "Win32-Spin : When focus is set ot spin control, AutomationElement.SetFocus causes Assert"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-Spin : When focus is set to spin control, AutomationElement.SetFocus causes Assert")]
        PS35,

        /// -------------------------------------------------------------------
        /// <summary> "(Win32 List): Calling RangeValuePattern.SetValue does not set the value correctly"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Win32-List : Calling RangeValuePattern.SetValue does not set the value correctly")]
        PS36,

        /// -------------------------------------------------------------------
        /// <summary>"TextPattern.ExpandToEnclosingUnit(*) gives inconsistent results when expanding an empty range at end of document (Win32 EDIT)"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("TextPattern.ExpandToEnclosingUnit(*) gives inconsistent results when expanding an empty range at end of document (Win32 EDIT))")]
        PS37,

        /// -------------------------------------------------------------------
        /// <summary>"TextPattern.ExpandToEnclosingUnit(*) gives inconsistent results when expanding an empty range at end of document (Win32 RICHEDIT)"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("TextPattern.ExpandToEnclosingUnit(*) gives inconsistent results when expanding an empty range at end of document (Win32 RICHEDIT)")]
        PS38,
    
            /// -------------------------------------------------------------------
        /// <summary>"TextPattern.ExpandToEnclosingUnit(*) gives inconsistent results when expanding an empty range at end of document (Win32 RICHEDIT)"</summary>
        /// -------------------------------------------------------------------
        [BugDescriptionAttribute("Winform-TogglePattern.Toggle() Checked Listitem does not fire TogglePattern.ToggleStateProperty when calling TogglePattern.Toggle()")]
        PS39,
}

    /// -------------------------------------------------------------------
    /// <summary>
    /// Flag for specifying priority of test to run
    /// </summary>
    /// -------------------------------------------------------------------
    [FlagsAttribute]
    public enum TestPriorities
    {
        /// <summary></summary>
        BuildVerificationTest = 1,
        /// <summary></summary>
        Pri0 = 2,
        /// <summary></summary>
        Pri1 = 4,
        /// <summary></summary>
        Pri2 = 8,
        /// <summary></summary>
        Pri3 = 16,
        /// <summary></summary>
        PriAll = BuildVerificationTest | Pri0 | Pri1 | Pri2 | Pri3
    }

    /// ---------------------------------------
    /// <summary>
    /// Filter to determine what type this test is
    /// </summary>
    /// ---------------------------------------
    [Flags]
    public enum TestCaseType
    {
        /// <summary>
        /// Can be ran within LeVerify application and does not require any
        /// up front planning other than identifying the element to test.
        /// </summary>
        Generic = 1,
        /// <summary>
        /// Is a test that run from a controlled environment since it may take some up front
        /// planning such as opening Notepad, or may require arguments
        /// </summary>
        Scenario = 2,
        /// <summary>
        /// Whether the test case tests out events or not
        /// </summary>
        Events = 4,
        /// <summary>
        /// Arguements are required for the test case
        /// </summary>
        Arguments = 8,
        /// <summary>
        /// Test must preserve content of control (for TextPattern and ValuePattern tests)
        /// </summary>
        PreservesContent = 16,
    }

    /// ---------------------------------------
    /// <summary>
    /// Id's the client that is asking for this test
    /// </summary>
    /// ---------------------------------------
    public enum Client
    {
        /// <summary></summary>
        NoneSet,
        /// <summary></summary>
        ActiveContentWizard,
        /// <summary></summary>
        Hoolie,
        /// <summary></summary>
        ATG,
        /// <summary></summary>
        Narrator,
        /// <summary></summary>
        ScreenReader
    }

    /// ---------------------------------------
    /// <summary>
    /// States the status of a test
    /// </summary>
    /// ---------------------------------------
    public enum TestStatus
    {
        /// <summary>Test works</summary>
        Works = 1,
        /// <summary>Under development</summary>
        WorkingOn = 2,
        /// <summary>Problem with the test</summary>
        Problem = 3,
        /// <summary>Bug entered on the test</summary>
        BugEntered = 4,
        /// <summary>Test is blocked</summary>
        Blocked = 5,
        /// <summary>Test ready to be checked into Wtt</summary>
        ReadyToAddToWtt = 6,
        /// <summary>Test is covered in another test</summary>
        CoveredInOtherTest,
    }

    /// ------------------------------------------------------------------------
    /// <summary></summary>
    /// ------------------------------------------------------------------------
    internal enum CheckType
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        IncorrectElementConfiguration,
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        Verification,
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        InformationalException,
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        Warning,
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        KnownProductIssue
        
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Identify provider being used
    /// </summary>
    /// -----------------------------------------------------------------------
    public enum TypeOfProvider
    {
        /// <summary>Win32 Proxy</summary>
        Win32,
        /// <summary>Avalon Proxy</summary>
        Avalon,
        /// <summary>DUI Proxy</summary>
        DUI,
        /// <summary>MSAA Proxy</summary>
        MSAA,
        /// <summary>Unknown Proxy (will default to Win32)</summary>
        Unknown
    };

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Class that stores the code examples that are passed to the XmlLogger
    /// in calls to LogComment()
    /// </summary>
    /// -----------------------------------------------------------------------
    public class ProgramExampleCall
    {
        /// -------------------------------------------------------------------
        /// <summary>Xml path to the object being tested</summary>
        /// -------------------------------------------------------------------
        string _elementPath;

        /// -------------------------------------------------------------------
        /// <summary>Actual TestRuns() call to the test associated with _elementPath</summary>
        /// -------------------------------------------------------------------
        string _exampleCall;

        /// -------------------------------------------------------------------
        /// <summary>XmlPath to the element obtained from 
        /// InternalHelpers.GetAutomationElementByControlTypePath()</summary>
        /// -------------------------------------------------------------------
        public string ElementPath
        {
            get { return _elementPath; }
        }

        /// -------------------------------------------------------------------
        /// <summary>Example code that calls the test with the element</summary>
        /// -------------------------------------------------------------------
        public string ExampleCall
        {
            get { return _exampleCall; }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// </summary>
        /// <param name="elementPath">XmlPath to the element obtained from InternalHelpers.GetAutomationElementByControlTypePath()</param>
        /// <param name="exampleCall">Example code that calls the test with the element</param>
        /// -------------------------------------------------------------------
        public ProgramExampleCall(string elementPath, string exampleCall)
        {
            _elementPath = elementPath;
            _exampleCall = exampleCall;
        }
    }

    /// <summary>
    /// This is the custom attribues on each test cases
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class TestCaseAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TestName"></param>
        public TestCaseAttribute(string TestName)
        {
            _testName = TestName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TestCaseAttribute()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TestName"></param>
        /// <param name="priority"></param>
        /// <param name="TestStatus"></param>
        /// <param name="Author"></param>
        /// <param name="Description"></param>
        public TestCaseAttribute(string TestName, TestPriorities priority, TestStatus TestStatus, string Author, string[] Description)
        {
            _testName = TestName;
            _status = TestStatus;
            _description = Description;
            priority = Priority;
            _author = Author;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TestSummary"></param>
        /// <param name="TestName"></param>
        /// <param name="priority"></param>
        /// <param name="TestStatus"></param>
        /// <param name="Author"></param>
        /// <param name="Description"></param>
        public TestCaseAttribute(string TestSummary, string TestName, TestPriorities priority, TestStatus TestStatus, string Author, string[] Description)
        {
            _testSummary = TestSummary;
            _testName = TestName;
            Status = TestStatus;
            _description = Description;
            priority = Priority;
            _author = Author;
        }

        string _testSummary;

        string _testName;

        string _author;

        TestStatus _status;

        TestPriorities _priority;

        BugIssues[] _bugNumbers;

        string[] _description;

        string _problemDescription;

        string _eventTested = null;

        TestCaseType _testCaseType = TestCaseType.Generic;

        Client _client;

        string _UISpyLookName;

        int _wttJob = 0;

        /// -------------------------------------------------------------------
        /// <summary>Wtt JobID</summary>
        /// -------------------------------------------------------------------
        public int WttJob
        {
            get { return _wttJob; }
            set { _wttJob = value; }
        }

        /// -------------------------------------------------------------------
        /// <summary>String generated by Libary.GetUISpyLook</summary>
        /// -------------------------------------------------------------------
        public string UISpyLookName
        {
            get { return _UISpyLookName; }
            set { _UISpyLookName = value; }
        }

        /// <summary>
        /// Summary of what the test does
        /// </summary>
        /// <value></value>
        public string TestSummary { set { _testSummary = value; } get { return _testSummary; } }

        /// <summary>
        /// Name of the test
        /// </summary>
        /// <value></value>
        public string TestName { set { _testName = value; } get { return _testName; } }

        /// <summary>
        /// Author of the test
        /// </summary>
        /// <value></value>
        public string Author { set { _author = value; } get { return _author; } }

        /// <summary>
        /// Status of the test
        /// </summary>
        /// <value></value>
        public TestStatus Status { set { _status = value; } get { return _status; } }

        /// <summary>
        /// Priority of the test
        /// </summary>
        /// <value></value>
        public TestPriorities Priority { set { _priority = value; } get { return _priority; } }

        /// <summary>
        /// Bug number associated with the test
        /// </summary>
        public BugIssues[] BugNumbers { set { _bugNumbers = value; } get { return _bugNumbers; } }

        /// <summary>
        /// Instructions that the test willl execute
        /// </summary>
        public string[] Description { set { _description = value; } get { return _description; } }

        /// <summary>
        /// Description if the test has any problems
        /// </summary>
        public string ProblemDescription { set { _problemDescription = value; } get { return _problemDescription; } }

        /// <summary>
        /// Type of test
        /// </summary>
        public TestCaseType TestCaseType 
        { 
            set 
            {
                // Scenarios are explicitly called so should not 
                // have any other TestCaseType associated with them.
                // Anything other and we want to make sure we | it 
                // with the default TestCaseType.Generic
                if (TestCaseType == TestCaseType.Scenario)
                    _testCaseType = value;
                else
                    _testCaseType |= value;
            } 
            get { return _testCaseType; } }

        /// <summary>
        /// Used for reporting when TestCaseType = Events
        /// </summary>
        /// <value></value>
        public string EventTested { set { _eventTested = value; } get { return _eventTested; } }

        /// <summary>
        /// Client associated with the test
        /// </summary>
        /// <value></value>
        public Client Client { set { _client = value; } get { return _client; } }
    }
}
