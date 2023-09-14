// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.

/******************************************************************* 
* Purpose: InternalHelper
*******************************************************************/
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows;

namespace InternalHelper.Tests.Patterns
{
    using InternalHelper.Tests;
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
    /// -----------------------------------------------------------------------
    /// <summary>
    /// Test Verifies that SynchronizedInputPattern fires appropriate events for different combinations of key and mouse inputs
    /// </summary>
    /// -----------------------------------------------------------------------
    public class SynchronizedInputPatternWrapper : PatternObject
    {
        #region Member variables
        internal SynchronizedInputPattern m_pattern;
        internal string classNameOfCurrentElement; 
        #endregion

        /// -------------------------------------------------------------------
        /// <summary>
        /// Wrapper class that helps to test SynchronizedInputPattern
        /// </summary>
        /// -------------------------------------------------------------------
        internal SynchronizedInputPatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            Comment("Calling GetPattern(SynchronizedInputPattern) on " + Library.GetUISpyLook(element));
            m_pattern = (SynchronizedInputPattern)GetPattern(m_le, m_useCurrent, SynchronizedInputPattern.Pattern);
            classNameOfCurrentElement = element.Current.ClassName;
        }


        #region Methods

        internal void pattern_SynchronizedInput(Type expectedException, CheckType checkType, SynchronizedInputType inputType)
        {

            string call = "SynchronizedInput()";
            try
            {
                m_pattern.StartListening(inputType);
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

        internal void pattern_Cancel(Type expectedException, CheckType checkType)
        {
            string call = "Cancel()";
            try
            {
                m_pattern.Cancel();
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
    using InternalHelper.Tests.Patterns;
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;

    /// <summary>
    /// Tests to verify different SynchronizedInputPattern events
    /// </summary>
    public sealed class SynchronizedInputTests : SynchronizedInputPatternWrapper
    {
        const string THIS = "SynchronizedInputTests";
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// <summary>Defines which UIAutomation Pattern this tests</summary>
        public static readonly string TestWhichPattern = SynchronizedInputPattern.Pattern != null ? Automation.PatternName(SynchronizedInputPattern.Pattern) : null;
        
        /// Flag to tell us if we really do want to catch the event        
        EventFired _eventAppropriate;
        // Stores as Key/Value pairs key:ClassName, Value:Keyborad input for testing 
        public static readonly Dictionary<string, System.Windows.Input.Key> keyboardInputMapping = new Dictionary<string, System.Windows.Input.Key>()
        {
           {"Button",System.Windows.Input.Key.Space},
           {"CheckBox",System.Windows.Input.Key.Space},
           {"ComboBox",System.Windows.Input.Key.End},
           {"DocumentViewer",System.Windows.Input.Key.Right},
           {"FlowDocumentScrollViewer",System.Windows.Input.Key.Home},
           {"FlowDocumentPageViewer",System.Windows.Input.Key.End},
           {"FlowDocumentReader",System.Windows.Input.Key.F3},
           {"Hyperlink",System.Windows.Input.Key.Enter},
           {"HeaderSite",System.Windows.Input.Key.Enter},// Focus on button and listen on expander
           {"ListBox",System.Windows.Input.Key.Down},
           {"ListView",System.Windows.Input.Key.Down},           
           {"RepeatButton",System.Windows.Input.Key.Space},
           {"RichTextBox",System.Windows.Input.Key.Down}, 
           {"RadioButton",System.Windows.Input.Key.Space}, 
           {"Menu",System.Windows.Input.Key.Left}, 
           {"MenuItem",System.Windows.Input.Key.Down},
           {"Slider",System.Windows.Input.Key.End},
           {"ScrollViewer",System.Windows.Input.Key.Home},
           {"TabItem",System.Windows.Input.Key.End}, 
           {"TextBox",System.Windows.Input.Key.Down},
           //Disabling TreeView testing because we cannot call SetFocus() on it.  By design.
           //{"TreeView",System.Windows.Input.Key.Home},
           {"TreeViewItem",System.Windows.Input.Key.Down},
           {"ToolBar",System.Windows.Input.Key.Home},           
           {"ToggleButton",System.Windows.Input.Key.Space},
        };

        // Stores as Key/Value pairs key:ClassName, Value:sub part to send mouse input 
        internal static readonly Dictionary<string, string> elementNamesForMouseInputs = new Dictionary<string, string>()
        {
           {"DocumentViewer","LineRight"},
           {"Expander","HeaderSite"},
           {"FlowDocumentPageViewer","IncreaseZoom"},
           {"FlowDocumentScrollViewer","LineDown"},
           {"FlowDocumentReader","IncreaseZoom"},
           {"RichTextBox","LineUp"},
           {"ScrollBar","LineRight"},
           {"ScrollViewer","LineUp"},
           {"TreeView","Expander"},
           {"ToolBar","OverflowButton"},
        };

        // Stores as Key/Value pairs key:ClassName, Value:Tests that should to be perfomed on the control 
        public static readonly Dictionary<string, List<string>> classNamesToTestEvents = new Dictionary<string, List<string>>()
        {
           {"Button", new List<string> {"KeyDown","KeyUp","MouseLeftDown","MouseLeftUp"}},
           {"CheckBox", new List<string> {"KeyDown","KeyUp","MouseLeftDown","MouseLeftUp"}},
           {"ComboBox", new List<string> {"KeyDown","MouseLeftDown","MouseLeftUp","MouseRightDown"}},
           {"DocumentViewer", new List<string> {"KeyDown","MouseLeftDown","MouseLeftUp"}},
           {"Expander", new List<string> {"KeyDown", "KeyUp","MouseLeftDown","MouseLeftUp"}},
           {"FlowDocumentScrollViewer", new List<string> {"KeyDown", "MouseLeftDown","MouseLeftUp","MouseRightDown"}},
           {"FlowDocumentPageViewer", new List<string> {"KeyDown","MouseLeftDown","MouseLeftUp","MouseRightDown"}},            
           {"Hyperlink", new List<string> {"KeyDown","MouseLeftUp"}},
           {"ListBoxItem", new List<string> {"MouseLeftDown","MouseRightDown"}},
           {"ListBox", new List<string> {"KeyDown"}},
           {"MenuItem", new List<string> {"KeyDown","KeyUp","MouseLeftDown","MouseLeftUp","MouseRightDown"}},// Due to test limitations KeyUp and KeyDown are cannot be verified
           {"Menu", new List<string> {"KeyDown"}},           
           {"RadioButton", new List<string> {"KeyDown","KeyUp","MouseLeftDown","MouseLeftUp"}},
           {"RepeatButton", new List<string> {"KeyDown","KeyUp","MouseLeftDown","MouseLeftUp"}},
           {"RichTextBox", new List<string> {"KeyDown","MouseLeftUp"}},
           {"Slider", new List<string> {"KeyDown", "MouseLeftDown","MouseLeftUp"}},           
           {"ScrollBar", new List<string> {"MouseLeftDown","MouseLeftUp"}},
           {"ScrollViewer", new List<string> {"KeyDown", "MouseLeftDown","MouseLeftUp"}},
           {"TabItem", new List<string> {"MouseLeftDown"}},
           {"TabControl", new List<string> {"KeyDown"}},
           {"TextBox", new List<string> {"KeyDown", "MouseLeftDown","MouseLeftUp","MouseRightDown"}},
           {"Thumb", new List<string> {"MouseLeftDown","MouseLeftUp"}},
           {"ToggleButton", new List<string> {"KeyDown", "KeyUp","MouseLeftDown","MouseLeftUp"}},
           {"ToolBar", new List<string> {"KeyDown", "MouseLeftDown","MouseRightDown"}},
           //Disabling TreeView testing because we cannot call SetFocus() on it.  By design.
           //{"TreeView", new List<string> {"KeyDown"}},
           {"TreeViewItem", new List<string> {"KeyDown", "MouseLeftDown"}},//MouseLeftDown -not working
        };
        
        public SynchronizedInputTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.SynchronizedInput, dirResults, testEvents, commands)
        {
            if (m_pattern == null)
                throw new Exception(Helpers.PatternNotSupported);
        }

        
        EventFired ShouldSynchronizedInputCauseEvent(AutomationElement element)
        {
            return EventFired.True;
        }

        #region Tests: Pattern verification

        /// -------------------------------------------------------------------
        ///<summary>
        /// Test Verifies that SynchronizedInputPattern.InputReachedTargetEvent is fired for SynchronizedInputType.KeyDown
        ///</summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("InputReachedTargetEvent.KeyDown",
            TestSummary = "Call SynchronizedInput() and verify that InputReachedTargetEvent is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft Corp.",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationEventHandler(SynchronizedInputPattern.InputReachedTargetEvent)",
            Description = new string[] {
                "Step 1: Verify that SetFocus is supported for the AutomationElement",
                "Step 2: Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement",
                "Step 3: Add listener for SynchronizedInputPattern.InputReachedTargetEvent event on the AutomationElement",                 
                "Step 4: Start listening to SynchronizedInputType.KeyDown on the AutomationElement",   
                "Step 5: Set focus on the AutomationElement",   
                "Step 6: Send input to the AutomationElement or one of its children",
                "Step 7: Wait for event to fire",                              
                "Step 8: Stop Listening to the events",
                "Step 9: Verify that SynchronizedInputPattern.InputReachedTargetEvent event is fired",  
            })]
        public void InputReachedTargetEventWithKeyDownEvents(TestCaseAttribute testCaseAtrribute)
        {
            if (classNamesToTestEvents.ContainsKey(classNameOfCurrentElement))
            {
                List<string> temp = classNamesToTestEvents[classNameOfCurrentElement];
                if (temp.Contains("KeyDown"))
                {
                    AutomationElement elementToSendInput = null;
                    HeaderComment(testCaseAtrribute);
                    // Verify that SetFocus is supported for the AutomationElement
                    TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);
                    // Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement        
                    TS_IsSynchronizedInputEventAppropriateOnObject(m_le);
                    // Add listener for SynchronizedInputPattern.InputReachedTargetEvent event on the AutomationElement 
                    TSC_AddEventListener(m_le, SynchronizedInputPattern.InputReachedTargetEvent, TreeScope.Element, CheckType.Verification);
                    // Start listening to SynchronizedInputType.KeyDown on the AutomationElement
                    TSC_SynchronizedInput(CheckType.Verification, SynchronizedInputType.KeyDown);
                    // Set focus on the AutomationElement and Send input to the AutomationElement or one of its children   
                    elementToSendInput = TS_GetElementToSendInput(m_le, true);
                    if (elementToSendInput == null)
                    {
                        TS_SetFocus(m_le, null, CheckType.Verification);
                        TS_KeyboardInput(m_le, CheckType.Verification);
                    }
                    else
                    {
                        TS_SetFocus(elementToSendInput, null, CheckType.Verification);
                        TS_KeyboardInput(elementToSendInput, CheckType.Verification);
                    }
                    // Wait for event to fire
                    TSC_WaitForEvents(1);
                    // Stop Listening to the events
                    TSC_Cancel(CheckType.Verification);
                    // Verify that SynchronizedInputPattern.InputReachedTargetEvent event is fired
                    if (_eventAppropriate == EventFired.True)
                    {
                        TSC_VerifyEventListener(m_le, SynchronizedInputPattern.InputReachedTargetEvent, _eventAppropriate, CheckType.Verification);
                    }
                }
            }
        }

        /// -------------------------------------------------------------------
        ///<summary>
        ///Test Verifies that SynchronizedInputPattern.InputReachedTargetEvent is fired for SynchronizedInputType.KeyUp
        ///</summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("InputReachedTargetEvent.KeyUp",
            TestSummary = "Call SynchronizedInput() and verify that only one InputReachedElement is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft Corp.",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationEventHandler(SynchronizedInputPattern.InputReachedTargetEvent)",
            Description = new string[] {
                "Step 1: Verify that SetFocus is supported for the AutomationElement",
                "Step 2: Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement",
                "Step 3: Add listener for SynchronizedInputPattern.InputReachedTargetEvent event on the AutomationElement",                 
                "Step 4: Start listening to SynchronizedInputType.KeyUp on the AutomationElement",   
                "Step 5: Set focus on the AutomationElement",   
                "Step 6: Send input to the AutomationElement or one of its children",
                "Step 7: Wait for event to fire",                             
                "Step 8: Stop Listening to the events",
                "Step 9: Verify that SynchronizedInputPattern.InputReachedTargetEvent event is fired",   
            })]
        public void InputReachedTargetEventWithKeyUpEvents(TestCaseAttribute testCaseAtrribute)
        {
            if (classNamesToTestEvents.ContainsKey(classNameOfCurrentElement))
            {
                List<string> temp = classNamesToTestEvents[classNameOfCurrentElement];
                if (temp.Contains("KeyUp"))
                {
                    AutomationElement elementToSendInput = null;
                    HeaderComment(testCaseAtrribute);
                    TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);
                    TS_IsSynchronizedInputEventAppropriateOnObject(m_le);
                    TSC_AddEventListener(m_le, SynchronizedInputPattern.InputReachedTargetEvent, TreeScope.Element, CheckType.Verification);
                    TSC_SynchronizedInput(CheckType.Verification, SynchronizedInputType.KeyUp);
                    elementToSendInput = TS_GetElementToSendInput(m_le, true);
                    if (elementToSendInput == null)
                    {
                        TS_SetFocus(m_le, null, CheckType.Verification);
                        TS_KeyboardInput(m_le, CheckType.Verification);
                    }
                    else
                    {
                        TS_SetFocus(elementToSendInput, null, CheckType.Verification);
                        TS_KeyboardInput(elementToSendInput, CheckType.Verification);
                    }
                    TSC_WaitForEvents(1);
                    TSC_Cancel(CheckType.Verification);
                    if (_eventAppropriate == EventFired.True)
                    {
                        TSC_VerifyEventListener(m_le, SynchronizedInputPattern.InputReachedTargetEvent, _eventAppropriate, CheckType.Verification);
                    }
                }
            }
        }

        /// -------------------------------------------------------------------
        ///<summary>
        /// Test Verifies that SynchronizedInputPattern.InputReachedTargetEvent is fired for SynchronizedInputType.MouseLeftButtonDown
        ///</summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("InputReachedTargetEvent.MouseLeftButtonDown",
            TestSummary = "Call SynchronizedInput() and verify that InputReachedTargetEvent is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft Corp.",
            TestCaseType = TestCaseType.Events,
            EventTested = "SynchronizedInputPattern.InputReachedTargetEvent",
            Description = new string[] {  
                "Step 1: Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement",
                "Step 2: Add listener for SynchronizedInputPattern.InputReachedTargetEvent event on the AutomationElement",                 
                "Step 3: Start listening to SynchronizedInputType.MouseLeftButtonDown on the AutomationElement",  
                "Step 4: Send input to the AutomationElement or one of its sub parts",
                "Step 5: Wait for event to fire",                                
                "Step 6: Stop Listening to the events",
                "Step 7: Verify that SynchronizedInputPattern.InputReachedTargetEvent event is fired",
            })]
        public void InputReachedTargetWithMouseLeftButtonDown(TestCaseAttribute testCaseAtrribute)
        {
            if (classNamesToTestEvents.ContainsKey(classNameOfCurrentElement))
            {
                List<string> temp = classNamesToTestEvents[classNameOfCurrentElement];
                if (temp.Contains("MouseLeftDown"))
                {
                    AutomationElement elementToSendInput = null;
                    HeaderComment(testCaseAtrribute);
                    // Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement        
                    TS_IsSynchronizedInputEventAppropriateOnObject(m_le);
                    // Add listener for SynchronizedInputPattern.InputReachedTargetEvent event on the AutomationElement
                    TSC_AddEventListener(m_le, SynchronizedInputPattern.InputReachedTargetEvent, TreeScope.Element, CheckType.Verification);
                    // Start listening to SynchronizedInputType.MouseLeftButtonDown on the AutomationElement 
                    TSC_SynchronizedInput(CheckType.Verification, SynchronizedInputType.MouseLeftButtonDown);
                    // Send input to the AutomationElement or one of its sub parts
                    elementToSendInput = TS_GetElementToSendInput(m_le, false);
                    if (elementToSendInput == null)
                    {
                        TS_MouseInput(m_le, CheckType.Verification, true);
                    }
                    else
                    {
                        TS_MouseInput(elementToSendInput, CheckType.Verification, true);
                    }
                    // Wait for event to fire
                    TSC_WaitForEvents(1);
                    // Stop listening to the events
                    TSC_Cancel(CheckType.Verification);
                    // Verify that SynchronizedInputPattern.InputReachedTargetEvent event is fired
                    if (_eventAppropriate == EventFired.True)
                    {
                        TSC_VerifyEventListener(m_le, SynchronizedInputPattern.InputReachedTargetEvent, _eventAppropriate, CheckType.Verification);
                    }
                }
            }
        }

        /// -------------------------------------------------------------------
        ///<summary>
        /// Test Verifies that SynchronizedInputPattern.InputReachedTargetEvent is fired for SynchronizedInputType.MouseLeftButtonUp
        ///</summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("InputReachedTargetEvent.MouseLeftButtonUp",
            TestSummary = "Call SynchronizedInput() and verify that InputReachedTargetEvent is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft Corp.",
            TestCaseType = TestCaseType.Events,
            EventTested = "SynchronizedInputPattern.InputReachedTargetEvent",
            Description = new string[] {  
                "Step 1: Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement",
                "Step 2: Add listener for SynchronizedInputPattern.InputReachedTargetEvent event on the AutomationElement",                 
                "Step 3: Start listening to SynchronizedInputType.MouseLeftButtonUp on the AutomationElement",  
                "Step 4: Send input to the AutomationElement or one of its sub parts",
                "Step 5: Wait for event to fire",
                "Step 6: Stop Listening to the events",
                "Step 7: Verify that SynchronizedInputPattern.InputReachedTargetEvent event is fired",   
            })]
        public void InputReachedTargetWithMouseLeftButtonUp(TestCaseAttribute testCaseAtrribute)
        {
            if (classNamesToTestEvents.ContainsKey(classNameOfCurrentElement))
            {
                List<string> temp = classNamesToTestEvents[classNameOfCurrentElement];
                if (temp.Contains("MouseLeftUp"))
                {
                    AutomationElement elementToSendInput = null;
                    HeaderComment(testCaseAtrribute);
                    TS_IsSynchronizedInputEventAppropriateOnObject(m_le);
                    TSC_AddEventListener(m_le, SynchronizedInputPattern.InputReachedTargetEvent, TreeScope.Element, CheckType.Verification);
                    TSC_SynchronizedInput(CheckType.Verification, SynchronizedInputType.MouseLeftButtonUp);
                    elementToSendInput = TS_GetElementToSendInput(m_le, false);
                    if (elementToSendInput == null)
                    {
                        TS_MouseInput(m_le, CheckType.Verification, true);
                    }
                    else
                    {
                        TS_MouseInput(elementToSendInput, CheckType.Verification, true);
                    }
                    TSC_WaitForEvents(1);
                    TSC_Cancel(CheckType.Verification);
                    if (_eventAppropriate == EventFired.True)
                    {
                        TSC_VerifyEventListener(m_le, SynchronizedInputPattern.InputReachedTargetEvent, _eventAppropriate, CheckType.Verification);
                    }
                }
            }
        }

        /// -------------------------------------------------------------------
        ///<summary>
        /// Test Verifies that SynchronizedInputPattern.InputReachedTargetEvent is fired for SynchronizedInputType.MouseRightButtonDown
        ///</summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("InputReachedTargetEvent.MouseRightButtonDown",
            TestSummary = "Call SynchronizedInput() and verify that InputReachedTargetEvent is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft Corp.",
            TestCaseType = TestCaseType.Events,
            EventTested = "SynchronizedInputPattern.InputReachedTargetEvent",
            Description = new string[] {  
                "Step 1: Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement",
                "Step 2: Add listener for SynchronizedInputPattern.InputReachedTargetEvent event on the AutomationElement",                 
                "Step 3: Start listening to SynchronizedInputType.MouseRightButtonDown on the AutomationElement",  
                "Step 4: Send input to the AutomationElement or one of its sub parts",
                "Step 5: Wait for event to fire",
                "Step 6: Stop Listening to the events",
                "Step 7: Verify that SynchronizedInputPattern.InputReachedTargetEvent event is fired",
            })]
        public void InputReachedTargetWithMouseRightButtonDown(TestCaseAttribute testCaseAtrribute)
        {
            if (classNamesToTestEvents.ContainsKey(classNameOfCurrentElement))
            {
                List<string> temp = classNamesToTestEvents[classNameOfCurrentElement];
                if (temp.Contains("MouseRightDown"))
                {
                    AutomationElement elementToSendInput = null;
                    HeaderComment(testCaseAtrribute);
                    TS_IsSynchronizedInputEventAppropriateOnObject(m_le);
                    TSC_AddEventListener(m_le, SynchronizedInputPattern.InputReachedTargetEvent, TreeScope.Element, CheckType.Verification);
                    TSC_SynchronizedInput(CheckType.Verification, SynchronizedInputType.MouseRightButtonDown);
                    elementToSendInput = TS_GetElementToSendInput(m_le, false);
                    if (elementToSendInput == null)
                    {
                        TS_MouseInput(m_le, CheckType.Verification, false);
                    }
                    else
                    {
                        TS_MouseInput(elementToSendInput, CheckType.Verification, false);
                    }
                    TSC_WaitForEvents(1);
                    TSC_Cancel(CheckType.Verification);
                    if (_eventAppropriate == EventFired.True)
                    {
                        TSC_VerifyEventListener(m_le, SynchronizedInputPattern.InputReachedTargetEvent, _eventAppropriate, CheckType.Verification);
                    }
                }
            }
        }

        /// -------------------------------------------------------------------
        ///<summary>
        ///Test Verifies that SynchronizedInputPattern.InputDiscardedEvent is fired for Keyboard Inputs
        ///</summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("InputDiscardedEvent.KeyDown",
            TestSummary = "Call SynchronizedInput() and verify that InputDiscardedEvent is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft Corp.",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationEventHandler(SynchronizedInputPattern.InputDiscardedEvent)",
            Description = new string[] {
                "Precondition: Verify that this test is performed only on Button.xaml file", 
                "Step 1: Verify that SetFocus is supported for the AutomationElement",
                "Step 2: Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement",
                "Step 3: Add listener for SynchronizedInputPattern.InputDiscardedEvent event on the AutomationElement",                 
                "Step 4: Start listening to SynchronizedInputType.KeyDown on the AutomationElement",   
                "Step 5: Set focus on the AutomationElement",   
                "Step 6: Send input to the sibling of the AutomationElement",                 
                "Step 7: Wait for event to fire",
                "Step 8: Stop Listening to the events",
                "Step 9: Verify that SynchronizedInputPattern.InputDiscardedEvent event is fired",  
            })]
        public void InputDiscardedEventWithKeyInputs(TestCaseAttribute testCaseAtrribute)
        {
            AutomationElement sibling, parent;
            // Perform this test on Button.xaml file only
            if (m_le.Current.ControlType.Equals(ControlType.Button))
            {
                // Find the parent automationelement
                TreeWalker treeWalker = TreeWalker.ControlViewWalker;
                parent = treeWalker.GetParent(m_le);
                if (parent != null)
                {
                    // This step identifies that test is being performed only on Button.xaml file
                    sibling = parent.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.AutomationIdProperty, "SiblingButton"));
                    if (sibling != null)
                    {
                        HeaderComment(testCaseAtrribute);
                        // Precondition: Verify that this control supports SetFocus tests
                        TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);
                        // Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement
                        TS_IsSynchronizedInputEventAppropriateOnObject(m_le);
                        // Add SynchronizedInputPattern.InputDiscardedEvent event on the AutomationElement
                        TSC_AddEventListener(m_le, SynchronizedInputPattern.InputDiscardedEvent, TreeScope.Element, CheckType.Verification);
                        // Start listening to SynchronizedInputType.KeyDown on the AutomationElement"
                        TSC_SynchronizedInput(CheckType.Verification, SynchronizedInputType.KeyDown);
                        // Set Focus on the sibling of the automationelement
                        TS_SetFocus(sibling, null, CheckType.Verification);
                        // Send input to the automationelement
                        TS_KeyboardInput(m_le, CheckType.Verification);
                        // Wait for event to fire
                        TSC_WaitForEvents(1);
                        // Stop listening to the event
                        TSC_Cancel(CheckType.Verification);
                        // Verify that SynchronizedInputPattern.InputDiscardedEvent event is fired
                        if (_eventAppropriate == EventFired.True)
                        {
                            TSC_VerifyEventListener(m_le, SynchronizedInputPattern.InputDiscardedEvent, _eventAppropriate, CheckType.Verification);
                        }
                    }
                }
            }
        }

        // -------------------------------------------------------------------
        //<summary>
        //Test Verifies that SynchronizedInputPattern.InputDiscardedEvent is fired for Mouse Inputs
        //</summary>
        // -------------------------------------------------------------------
        [TestCaseAttribute("InputDiscardedEvent.MouseLeftButtonDown",
            TestSummary = "Call SynchronizedInput() and verify that InputDiscardedEvent is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft Corp.",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationEventHandler(SynchronizedInputPattern.InputDiscardedEvent)",
            Description = new string[] {
                "Precondition: Verify that this test is performed only on ListBoxItem.xaml file", 
                "Step 1: Verify that SetFocus is supported for the AutomationElement",
                "Step 2: Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement",
                "Step 3: Add listener for SynchronizedInputPattern.InputDiscardedEvent event on the AutomationElement",                 
                "Step 4: Start listening to SynchronizedInputType.MouseLeftDown on the AutomationElement",   
                "Step 5: Set focus on the AutomationElement",   
                "Step 6: Send input so that current automation element such that it is outside the visible area to the user",                 
                "Step 7: Wait for event to fire",
                "Step 8: Stop Listening to the events",
                "Step 9: Verify that SynchronizedInputPattern.InputDiscardedEvent event is fired",  
            })]
        public void InputDiscardedEventWithMouseInputs(TestCaseAttribute testCaseAtrribute)
        {
            AutomationElement sibling, parent;
            // Perform this test on ListBoxItem.xaml file only
            if (m_le.Current.ControlType.Equals(ControlType.ListItem))
            {
                // Find the parent automationelement
                TreeWalker treeWalker = TreeWalker.ControlViewWalker;
                parent = treeWalker.GetParent(m_le);
                if (parent != null)
                {
                    // This step identifies that test is being performed only on ListBoxItem.xaml file
                    sibling = parent.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.AutomationIdProperty, "SiblingListItem"));
                    if (sibling != null)
                    {
                        HeaderComment(testCaseAtrribute);
                        // Precondition: Verify that this control supports SetFocus tests
                        TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);
                        //Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement
                        TS_IsSynchronizedInputEventAppropriateOnObject(m_le);
                        //Add SynchronizedInputPattern.InputDiscardedEvent event on the AutomationElement
                        TSC_AddEventListener(m_le, SynchronizedInputPattern.InputDiscardedEvent, TreeScope.Element, CheckType.Verification);
                        //Start listening to SynchronizedInputType.MouseLeftButtonDown on the AutomationElement"
                        TSC_SynchronizedInput(CheckType.Verification, SynchronizedInputType.MouseLeftButtonDown);
                        //Perform test action(Send input so that current automation element such that it is outside the visible area to the user) and send mouse left down
                        TS_SetInitialConditionAndSendMouseInput(m_le, CheckType.Verification);
                        //Wait for event to fire
                        TSC_WaitForEvents(1);
                        //Stop listening to the event
                        TSC_Cancel(CheckType.Verification);
                        //Verify that SynchronizedInputPattern.InputDiscardedEvent event is fired
                        if (_eventAppropriate == EventFired.True)
                        {
                            TSC_VerifyEventListener(m_le, SynchronizedInputPattern.InputDiscardedEvent, _eventAppropriate, CheckType.Verification);
                        }
                    }
                }
            }
        }

        /// -------------------------------------------------------------------
        ///<summary>
        ///Test Verifies that SynchronizedInputPattern.InputReachedOtherElementEvent is fired
        ///</summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("InputReachedOtherElementEvent.MouseLeftButtonDown",
            TestSummary = "Call SynchronizedInput() and verify that InputReachedOtherElementEvent is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft Corp.",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationEventHandler(SynchronizedInputPattern.InputReachedOtherElementEvent)",
            Description = new string[] {
                "Precondition: Verify that this test is performed only on ListBox.xaml file", 
                "Step 2: Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement",
                "Step 3: Add listener for SynchronizedInputPattern.InputReachedOtherElementEvent event on the AutomationElement",                 
                "Step 4: Start listening to SynchronizedInputType.MouseLeftButtonDown on the AutomationElement", 
                "Step 6: Send input to the AutomationElement",                 
                "Step 7: Wait for event to fire",
                "Step 8: Stop Listening to the events",
                "Step 9: Verify that SynchronizedInputPattern.InputReachedOtherElementEvent event is fired",  
            })]
        public void InputReachedOtherElementEventWithMouseInputs(TestCaseAttribute testCaseAtrribute)
        {
            AutomationElement elementToSendInput;
            // Perform this test on ListBox.xaml file only
            if (m_le.Current.ClassName.Equals("ListBox"))
            {
                // This step identifies that test is being performed only on ListBox.xaml file
                elementToSendInput = m_le.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.AutomationIdProperty, "listboxitem1"));
                if (elementToSendInput != null)
                {
                    HeaderComment(testCaseAtrribute);
                    // Verify that SynchronizedInputPattern is a supported pattern for the AutomationElement        
                    TS_IsSynchronizedInputEventAppropriateOnObject(m_le);
                    // Add listener for SynchronizedInputPattern.InputReachedOtherElementEvent event on the AutomationElement
                    TSC_AddEventListener(m_le, SynchronizedInputPattern.InputReachedOtherElementEvent, TreeScope.Element, CheckType.Verification);
                    // Start listening to SynchronizedInputType.MouseLeftButtonDown on the AutomationElement 
                    TSC_SynchronizedInput(CheckType.Verification, SynchronizedInputType.MouseLeftButtonDown);
                    // Send input to the listboxitem
                    TS_MouseInput(elementToSendInput, CheckType.Verification, true);
                    // Wait for event to fire
                    TSC_WaitForEvents(1);
                    // Stop listening to the events
                    TSC_Cancel(CheckType.Verification);
                    // Verify that SynchronizedInputPattern.InputReachedOtherElementEvent event is fired
                    if (_eventAppropriate == EventFired.True)
                    {
                        TSC_VerifyEventListener(m_le, SynchronizedInputPattern.InputReachedOtherElementEvent, _eventAppropriate, CheckType.Verification);
                    }
                }
            }
        }       
        
        #endregion

        #region Place misc/lib code here this is specific to driving your tests
       
        internal void TSC_SynchronizedInput(CheckType checkType, SynchronizedInputType inputType)
        {
            pattern_SynchronizedInput(null, checkType, inputType);
            m_TestStep++;
        }

        internal void TSC_Cancel(CheckType checkType)
        {
            pattern_Cancel(null, checkType);
            m_TestStep++;
        }

        internal AutomationElement TS_GetElementToSendInput(AutomationElement element, bool keyboardInput)
        {
            AutomationElement elementToSendInput = null;
            string elementName = null;
            string className = element.Current.ClassName;

            if (keyboardInput)
            {
                switch (className)
                {
                    case "ToolBar":
                        elementName = "toolbarbutton";
                        break;
                    case "TabControl":
                        elementName = "tabitem";
                        break;
                    case "Menu":
                        elementName = "menuitem";
                        break;
                    case "Expander":
                        elementName = "HeaderSite";
                        break;
                }

                if (elementName != null)
                {
                    elementToSendInput = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.AutomationIdProperty, elementName));
                }                
            }
            else
            {                
                if (elementNamesForMouseInputs.ContainsKey(className))
                {
                    elementToSendInput = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.AutomationIdProperty, elementNamesForMouseInputs[className]));
                    //Workaround: The element name of ScrollViewer for mouse inputs is named 'PART_LineUpButton' on win8 and winblue
                    if ((elementToSendInput == null) && (className == "ScrollViewer"))
                    {
                        elementToSendInput = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.AutomationIdProperty, "PART_LineUpButton"));
                    }
                }
            }
            return elementToSendInput;
        }

        internal void TS_KeyboardInput(AutomationElement element, CheckType checkType)
        {
            string className = element.Current.ClassName;
            System.Windows.Input.Key inputString;

            if (element.Current.AutomationId == "toolbarbutton")
            {
                inputString = keyboardInputMapping["ToolBar"];
                TS_PressKeys(true, inputString); 
            }            
            else if (keyboardInputMapping.ContainsKey(className))
            {
                inputString = keyboardInputMapping[className];
                TS_PressKeys(true, inputString);                              
            }
        }        

        internal void TS_MouseInput(AutomationElement element, CheckType checkType, bool mouseLeftDown) 
        {
            if (mouseLeftDown)
            {
                TS_SendLeftMouseClick(element, checkType);
            }
            else
            {
                TS_SendRightMouseClick(element, checkType);
            }

            m_TestStep++;
        }

        internal void TS_SetInitialConditionAndSendMouseInput(AutomationElement element, CheckType checkType)
        {
            AutomationElement parent;
            Point pt = new Point();

            if (!element.TryGetClickablePoint(out pt))
            {
                ThrowMe(checkType, "TryGetClickablePoint returned false");
            }

            TreeWalker treeWalker = TreeWalker.ControlViewWalker;
            parent = treeWalker.GetParent(element);
            if (parent != null)
            {
                TS_PressKeys(true, System.Windows.Input.Key.End); 
            }

            Comment("Calling Performing mouse click @ Point(" + pt + ")");
            ATGTestInput.Input.MoveToAndClick(pt);

            m_TestStep++;
        }

        internal void TS_IsSynchronizedInputEventAppropriateOnObject(AutomationElement element)
        {

            bool content = (bool)element.GetCurrentPropertyValue(AutomationElement.IsSynchronizedInputPatternAvailableProperty);
            _eventAppropriate = EventFired.False;

            if ((bool)element.GetCurrentPropertyValue(AutomationElement.IsSynchronizedInputPatternAvailableProperty))
            {
                _eventAppropriate = EventFired.True;
            }
            else
            {
                _eventAppropriate = EventFired.Undetermined;
            }

            m_TestStep++;
        }

        #endregion Place misc/lib code here this is specific to driving your tests
    }
}
