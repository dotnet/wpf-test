// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
* Purpose: InternalHelper
* Owner: Microsoft
* Contributors:
*******************************************************************/
using System;
using System.CodeDom;
using System.Collections;
using Drawing = System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Automation;
using System.Windows;

namespace InternalHelper.Tests.Patterns
{
    using InternalHelper;
    using InternalHelper.Tests;
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class InvokePatternWrapper : PatternObject
    {
        #region Member variables
        internal InvokePattern m_pattern;
        #endregion

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal InvokePatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            Comment("Calling GetPattern(InvokePattern) on " + Library.GetUISpyLook(element));
            m_pattern = (InvokePattern)GetPattern(m_le, m_useCurrent, InvokePattern.Pattern);
        }

        #region Methods

        internal void pattern_Invoke(Type expectedException, CheckType checkType)
        {

            string call = "Invoke()";
            try
            {
                m_pattern.Invoke();
            }
            catch (Exception actualException)
            {
                if (Library.IsCriticalException(actualException))
                    throw;

                TestException(expectedException, actualException.GetType(), call, checkType);
                return;
            }
            TestNoException(expectedException, call, checkType);
        }

        #endregion Methods

    }
}


namespace Microsoft.Test.WindowsUIAutomation.Tests.Patterns
{
    using InternalHelper;
    using InternalHelper.Tests;
    using InternalHelper.Tests.Patterns;
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;

    /// <summary></summary>
    public sealed class InvokeTests : InvokePatternWrapper
    {
        const string THIS = "InvokeTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static readonly string TestWhichPattern = Automation.PatternName(InvokePattern.Pattern);

        /// -------------------------------------------------------------------
        /// <summary>Flag to tell us if we really do wnat to catch the event</summary>
        /// -------------------------------------------------------------------
        EventFired _eventAppropriate;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public InvokeTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.Invoke, dirResults, testEvents, commands)
        {
            if (m_pattern == null)
                throw new Exception(Helpers.PatternNotSupported);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        EventFired ShouldInvokeCauseEvent(AutomationElement element)
        {
            // If this is a Radion button, and is checked...
            if (element.Current.ControlType == ControlType.RadioButton)
            {
                ValuePattern vp = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                if (m_useCurrent)
                {
                    if (vp.Current.Value == "Checked")
                        return EventFired.False;
                }
                else
                {
                    if (vp.Cached.Value == "Checked")
                        return EventFired.False;
                }
            }
            return EventFired.True;
        }

        #region Tests: Pattern verification

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("CorrectNumberOfEventsFired",
            TestSummary = "Call Invoke() and verify that only one event is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,   
            Author = "Microsoft",
            BugNumbers = new BugIssues[] {BugIssues.PS32 },
            TestCaseType = TestCaseType.Events, EventTested = "AutomationEventHandler(InvokePattern.InvokedEvent)",
            Description = new string[] {
                "Precondition: Verify that this control supports SetFocus tests",
                "Step: Verify that Invoke event is required on this element, or is another pattern coving the same functionality",
                "Verify: Add InvokePattern.InvokedEvent event on the AutomationElement", 
                "Verify: Call Invoke()", 
                "Step: Wait for event",
                "Verify: that AutomationElement fired a InvokePattern.InvokedEvent event",
                "Verify: That only one InvokePattern.InvokedEvent event is fired if appropriate control"
            })]
        public void CorrectNumberOfEventsFired(TestCaseAttribute testCaseAtrribute)
        {
            int numEvents = _eventAppropriate == EventFired.True ? 1 : 0;

            if (CheckWPFDataGridElement(m_le)) // not supported; supports SelectionItemPattern instead 
                return;

            HeaderComment(testCaseAtrribute);

            //"Precondition: Verify that this control supports SetFocus tests",
            TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);

            //"Step: Verify that Invoke event is required on this element, or is another pattern coving the same functionality",
            TS_IsInvokeEventAppropriateOnObject(m_le);

            //"Step: Add InvokePattern.InvokedEvent event on the AutomationElement", 
            TSC_AddEventListener(m_le, InvokePattern.InvokedEvent, TreeScope.Element, CheckType.Verification);

            //"Verify: Call Invoke()"
            TSC_Invoke(CheckType.Verification);

            // Step: Wait for event",
            TSC_WaitForEvents(numEvents + 2);  // Two to wait for any extras comming through

            //"Verify: that AutomationElement fired a InvokePattern.InvokedEvent event",
            if (!TS_FilterOnBug(testCaseAtrribute))
                TSC_VerifyEventListener(m_le, InvokePattern.InvokedEvent, _eventAppropriate, CheckType.Verification);

            // "Verify: That only one InvokePattern.InvokedEvent event is fired if required for this element 
            if (_eventAppropriate == EventFired.True)
                TS_VerifyEventCount(m_le, numEvents, CheckType.Verification);
            else
                m_TestStep++;

        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Invoke.S.1.1.WithCode",
            TestSummary = "Call Invoke() on an element and verify that InvokePattern.InvokedEvent is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,   
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationEventHandler(InvokePattern.InvokedEvent)",
            Description = new string[] {
                "Precondition: Verify that this control supports SetFocus tests",
                "Precondition: Verify that Invoke event will get fired for this element",
                "Precondition: Verify that invoking on element is not destructive(close button), these need to be done manually",
                "Verify: Add InvokePattern.InvokedEvent event on the AutomationElement", 
                "Verify: Call Invoke()", 
                "Step: Wait for event", 
                "Verify: AutomationElement fired a InvokePattern.InvokedEvent event as appropriate"
            })]
        public void InvokeS11a(TestCaseAttribute testCaseAtrribute)
        {
            HeaderComment(testCaseAtrribute);

            //"Precondition: Verify that this control supports SetFocus tests",
            TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);

            //"Step: Verify that Invoke event will get fired for this element",
            TS_IsInvokeEventAppropriateOnObject(m_le);

            //"Precondition: Verify that invoking this element is not a know desctructive action such as a Close button
            TS_InvokeIsDestructive(m_le, CheckType.IncorrectElementConfiguration);

            //"Step: Add InvokePattern.InvokedEvent event on the AutomationElement",
            TSC_AddEventListener(m_le, InvokePattern.InvokedEvent, TreeScope.Element, CheckType.Verification);

            //"Verify: Call Invoke()"
            TSC_Invoke(CheckType.Verification);

            // Step: Wait for event",
            TSC_WaitForEvents(1);

            //"Step: Verify that AutomationElement fired a Invoked event
            if (string.Equals("DataGridCell", (string)m_le.GetCurrentPropertyValue(AutomationElement.ClassNameProperty), StringComparison.InvariantCulture))
            {
                TSC_VerifyEventListener(m_le, InvokePattern.InvokedEvent, EventFired.False, CheckType.Verification); // not required
            }
            else
            {
                TSC_VerifyEventListener(m_le, InvokePattern.InvokedEvent, _eventAppropriate, CheckType.Verification);
            }
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Invoke.WithCode",
            TestSummary = "Call Invoke() on an element and verify that InvokePattern.InvokedEvent is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,  
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            EventTested = "Invoke.InvokeEvent",
            Client = Client.ATG,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: Verify that Invoke event will get fired for this element",
                "Precondition: Verify that invoking this element is not a know desctructive action such as a Close button",
                "Verify: Add InvokePattern.InvokedEvent event on the AutomationElement", 
                "Verify: Call Invoke()", 
                "Step: Wait for event", 
                "Verify: AutomationElement fired a InvokePattern.InvokedEvent event as appropriate"
            })]
        public void InvokeS11b(TestCaseAttribute testCaseAtrribute)
        {
            HeaderComment(testCaseAtrribute);

            //"Step: Verify that Invoke event will get fired for this element",
            TS_IsInvokeEventAppropriateOnObject(m_le);

            //"Precondition: Verify that invoking this element is not a know desctructive action such as a Close button
            TS_InvokeIsDestructive(m_le, CheckType.IncorrectElementConfiguration);

            //"Step: Add InvokePattern.InvokedEvent event on the AutomationElement",
            TSC_AddEventListener(m_le, InvokePattern.InvokedEvent, TreeScope.Element, CheckType.Verification);

            //"Verify: Call Invoke()",
            TSC_Invoke(CheckType.Verification);

            // Step: Wait for event",
            TSC_WaitForEvents(1);

            //"Step: Verify that AutomationElement fired a FocusChange event
            TSC_VerifyEventListener(m_le, InvokePattern.InvokedEvent, _eventAppropriate, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Invoke.WithKeyboard",
            TestSummary = "Invoke the element with space bar and verify that InvokePattern.InvokedEvent is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Problem,
            ProblemDescription = "There is a problem when you send the space chars to the control.  Works if you manually do this, works in UISpy, but not in the test cases...randomly works with the same code...test issue",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            EventTested = "Invoke.InvokeEvent",
            Client = Client.ATG,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: Verify that Invoke event will get fired for this element",
                "Precondition: User can set focus to the element",
                "Precondition: Verify that invoking this element is not a know desctructive action such as a Close button",
                "Step: Add InvokePattern.InvokedEvent event on the AutomationElement", 
                "Step: Set focus to the element, if that can be done",
                "Step: Press whatever key is needed to invoke the element",
                "Step: Wait for event", 
                "Verify: AutomationElement fired a InvokePattern.InvokedEvent event as appropriate"
            })]
        public void InvokeWithKeyboard(TestCaseAttribute testCaseAtrribute)
        {
            HeaderComment(testCaseAtrribute);

            //"Precondition: Verify that Invoke event will get fired for this element",
            TS_IsInvokeEventAppropriateOnObject(m_le);

            //"Precondtion: user can set focus to the element",
            TSC_VerifyProperty(m_le.Current.IsKeyboardFocusable, true, "IsKeyboardFocusable", CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that invoking this element is not a know desctructive action such as a Close button
            TS_InvokeIsDestructive(m_le, CheckType.IncorrectElementConfiguration);

            //"Step: Add InvokePattern.InvokedEvent event on the AutomationElement",
            TSC_AddEventListener(m_le, InvokePattern.InvokedEvent, TreeScope.Element, CheckType.Verification);

            //"Step: Set focus to the element, if that can be done
            TS_SetFocus(m_le, null, CheckType.Verification);

            //"Step: Press whatever key is needed to invoke the element
            TS_InvokeWithKeyboard(m_le, CheckType.Verification);

            // Step: Wait for event",
            TSC_WaitForEvents(2);

            //"Step: Verify that AutomationElement fired a FocusChange event
            TSC_VerifyEventListener(m_le, InvokePattern.InvokedEvent, _eventAppropriate, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Invoke.S.1.1.WithMouseClick",
            TestSummary = "Mouse click on an element and verify that InvokePattern.InvokedEvent is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,   
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationEventHandler(InvokePattern.InvokedEvent)",
            Description = new string[] {
                "Precondition: Verify that this control supports SetFocus tests",
                "Precondition: Verify that Invoke event will get fired for this element",
                "Precondition: Verify that invoking this element is not a know desctructive action such as a Close button",
                "Verify: Add InvokePattern.InvokedEvent event on the AutomationElement",
                "Precondition: Move the mouse to the point and click",
                "Step: Wait for event",
                "Verify: AutomationElement fired a InvokePattern.InvokedEvent event as appropriate"
            })]
        public void InvokeS12(TestCaseAttribute testCaseAtrribute)
        {
            HeaderComment(testCaseAtrribute);

            //"Precondition: Verify that this control supports SetFocus tests",
            TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);

            //"Step: Verify that Invoke event will get fired for this element",
            TS_IsInvokeEventAppropriateOnObject(m_le);

            //"Precondition: Verify that invoking this element is not a know desctructive action such as a Close button
            TS_InvokeIsDestructive(m_le, CheckType.IncorrectElementConfiguration);

            //"Step: Add InvokePattern.InvokedEvent event on the AutomationElement",
            TSC_AddEventListener(m_le, InvokePattern.InvokedEvent, TreeScope.Element, CheckType.Verification);

            //"Step: Call and verify that SetFocus(AutomationElement) returns true"
            TSC_MouseClick(m_le, CheckType.IncorrectElementConfiguration); 

            // Step: Wait for event",
            TSC_WaitForEvents(1);

            //"Step: Verify that AutomationElement fired a FocusChange event
            TSC_VerifyEventListener(m_le, InvokePattern.InvokedEvent, _eventAppropriate, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Invoke.S.2.1.WithCodeOnDisabledControl",
            TestSummary = "Invoke a disabled element with Invokepattern.Invoke()",
            Priority = TestPriorities.Pri1,
           Status = TestStatus.Works,  
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationEventHandler(InvokePattern.InvokedEvent)",
            Description = new string[] {
            "Precondition: Verify that this control supports SetFocus tests",
            "Verify: That this element is disabled", 
            "Verify: Call Invoke()", 
            "Step: Wait for event", 
            "Verify: AutomationElement fired a InvokePattern.InvokedEvent event as appropriate"
            })]
        public void InvokeS21(TestCaseAttribute testCaseAtrribute)
        {
            HeaderComment(testCaseAtrribute);

            //"Precondition: Verify that this control supports SetFocus tests",
            TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);

            // Verify that the element is disabled
            TS_ElementEnabled(m_le, false, CheckType.IncorrectElementConfiguration);

            //"Step: Add FocusChangedListener event on the AutomationElement",
            TSC_AddEventListener(m_le, InvokePattern.InvokedEvent, TreeScope.Element, CheckType.Verification);

            //"Step: Call and verify that SetFocus(AutomationElement) returns true"
            TSC_Invoke(CheckType.Verification);

            // Step: Wait for event",
            TSC_WaitForEvents(1);

            //"Step: Verify that AutomationElement fired a FocusChange event
            TSC_VerifyEventListener(m_le, InvokePattern.InvokedEvent, _eventAppropriate, CheckType.Verification);
        }

        #endregion

        #region Place misc/lib code here this is specific to driving your tests

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TSC_MouseClick(AutomationElement element, CheckType checkType) //Point pt, string varPt, CheckType ct)
        {
            Point pt = new Point();

            if (!element.TryGetClickablePoint(out pt))
            {
                ThrowMe(checkType, "TryGetClickablePoint returned false");
            }

            Comment("Calling Performing mouse click @ Point(" + pt + ")");
            ATGTestInput.Input.MoveToAndClick(pt);

            m_TestStep++;
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TSC_Invoke(CheckType checkType)
        {
            pattern_Invoke(null, checkType);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        ///<summary>If Invoking this is a know destructive action, error out</summary>
        /// -------------------------------------------------------------------
        private void TS_InvokeIsDestructive(AutomationElement m_le, CheckType checkType)
        {
            if (Array.IndexOf(new string[] { "Close" }, m_le.Current.Name) != -1)
                ThrowMe(checkType, "AutomationElement is a close button");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_InvokeWithKeyboard(AutomationElement element, CheckType checkType)
        {
            ControlType controlType = element.Current.ControlType;

            if (controlType == ControlType.Button)
            {
                TS_PressKeys(true, System.Windows.Input.Key.Space);
            }
            else if (controlType == ControlType.MenuItem)
            {
                TS_PressKeys(true, System.Windows.Input.Key.Enter);
            }
            else 
            {
                TS_PressKeys(true, System.Windows.Input.Key.Space);
            }
            // m_TestStep++; The other TS_ steps increment this, don't do it again
        }


        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        void TS_IsInvokeEventAppropriateOnObject(AutomationElement element)
        {
            // if element is content
            //      if element supports{specific pattern}
            //          don't support Invoke
            // else
            //      don't support Invoke

            bool content = (bool)element.GetCurrentPropertyValue(AutomationElement.IsContentElementProperty);
            _eventAppropriate = EventFired.True;

            if (content)
            {
                if ((bool)element.GetCurrentPropertyValue(AutomationElement.IsExpandCollapsePatternAvailableProperty))
                {
                    Comment("Element is content element, and supports ExpandCollapsePattern, so Invoke event is not required");
                    _eventAppropriate = EventFired.False;
                }
                else if ((bool)element.GetCurrentPropertyValue(AutomationElement.IsValuePatternAvailableProperty))
                {
                    Comment("Element is content element, and supports ValuePattern, so Invoke event is not required");
                    _eventAppropriate = EventFired.False;
                }
                else if ((bool)element.GetCurrentPropertyValue(AutomationElement.IsRangeValuePatternAvailableProperty))
                {
                    Comment("Element is content element, and supports RangeValuePattern, so Invoke event is not required");
                    _eventAppropriate = EventFired.False;
                }
                else if ((bool)element.GetCurrentPropertyValue(AutomationElement.IsSelectionItemPatternAvailableProperty))
                {
                    Comment("Element is content element, and supports SelectionItemPattern, so Invoke event is not required");
                    _eventAppropriate = EventFired.False;
                }
                else if ((bool)element.GetCurrentPropertyValue(AutomationElement.IsTogglePatternAvailableProperty))
                {
                    Comment("Element is content element, and supports TogglePattern, so Invoke event is not required");
                    _eventAppropriate = EventFired.False;
                }
                else if ((bool)element.GetCurrentPropertyValue(AutomationElement.IsValuePatternAvailableProperty))
                {
                    Comment("Element is content element, and supports ValuePattern, so Invoke event is not required");
                    _eventAppropriate = EventFired.False;
                }
            }
            else
            {
                Comment("AutomationElement.IsContentElementProperty is false, so Invoke event is not required as the control should have a higher pattern that supports the same operation such as ExpandCollapse");
                _eventAppropriate = EventFired.Undetermined;
            }

            m_TestStep++;
        }

        #endregion Place misc/lib code here this is specific to driving your tests
    }
}
