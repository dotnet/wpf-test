// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//*******************************************************************
using System;
using System.Windows.Automation;
using System.Windows;
using System.CodeDom;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Windows.Input;

namespace Microsoft.Test.WindowsUIAutomation.Tests.Scenarios
{
    using InternalHelper;
    using InternalHelper.Tests;
    using InternalHelper.Tests.Patterns;
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.TestManager;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class TopLevelEventsScenarioTests : ScenarioObject
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const string THIS = "TopLevelEventsScenarioTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary>Notepad Format menu</summary>
        /// -------------------------------------------------------------------
        const string MENU_FORMAT = "Item 3";

        /// -------------------------------------------------------------------
        /// <summary>Notepad Font menu</summary>
        /// -------------------------------------------------------------------
        const string MENU_FONT = "Item 33";

        /// -------------------------------------------------------------------
        /// <summary>Notepad File menu</summary>
        /// -------------------------------------------------------------------
        const string MENU_FILE = "Item 1";

        /// -------------------------------------------------------------------
        /// <summary>Timings variables</summary>
        /// -------------------------------------------------------------------
        int _MAX_TIME = 5000;

        /// -------------------------------------------------------------------
        /// <summary>Timings variables</summary>
        /// -------------------------------------------------------------------
        int _INCREMENT = 100;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TopLevelEventsScenarioTests(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, TypeOfControl.UnknownControl, TypeOfPattern.Unknown, null, testEvents, commands)
        {
        }

        #region Tests

        #region WindowClosedEvent
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("WindowClosedEvent.1",
            TestSummary = "Verify that when a user closes the application, that the WindowClosedEvent is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "WindowPattern.WindowClosedEvent",
            Description = new string[] {
                "Step: Start the application",
                "Step: Add WindowClosedEvent listener for application",
                "Step: Close the application",
                "Step: Wait for one event to occur",
                "Verify: That the WindowClosedEvent is fired for the application"
            })]
        public void TstWindowClosedEvent1(TestCaseAttribute testCase, object[] arguments)
        {
            Library.ValidateArgumentNonNull(testCase, "testCase");
            Library.ValidateArgumentNonNull(arguments, "arguments");
            Library.ValidateArgumentNonNull(arguments[0], "arguments[0]");

            string appPath = (string)arguments[0];

            HeaderComment(testCase);

            // "Step: Start the application",
            TS_OpenWindow(appPath, null, CheckType.Verification);

            // "Step: Add WindowClosedEvent listener for application",
            TSC_AddEventListener(m_le, WindowPattern.WindowClosedEvent, TreeScope.Element, CheckType.Verification);

            // "Step: Close the aplication",
            TS_CloseWindow(m_le, CheckType.Verification);

            // "Step: Wait for one event to occur",
            TSC_WaitForEvents(5);

            // "Verify: That the WindowClosedEvent is fired for the application"
            TSC_VerifyEventListener(null, WindowPattern.WindowClosedEvent, EventFired.True, CheckType.Verification);

        }


        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("WindowClosedEvent.2",
            TestSummary = "Verify when the Run Dialog window is closed, WindowClosed event is fired",
            Priority = TestPriorities.Pri2,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "WindowPattern.WindowClosedEvent",
            Description = new string[] {
                "Step: Open start menu",
                "Step: Open run dialog",
                "Step: Add WindowClosedEvent listener for application",
                "Step: Press ESC to dismiss the window",
                "Step: Wait for one event to occur",
                "Verify: That the WindowClosed is fired"
            })]
        public void TstWindowClosedEvent2(TestCaseAttribute testCase)
        {
            AutomationElement runDialog;
            HeaderComment(testCase);

            // "Step: Open start menu",
            TS_OpenStartMenu(CheckType.Verification);

            // "Step: Open run dialog",
            TS_OpenRunDialog(out runDialog, CheckType.Verification);

            // "Step: Add WindowClosedEvent listener for application",
            TSC_AddEventListener(runDialog, WindowPattern.WindowClosedEvent, TreeScope.Element, CheckType.Verification);

            // "Step: Press ESC to dismiss the window",
            TS_PressKeys(true, Key.Escape);

            // "Step: Wait for one event to occur",
            TSC_WaitForEvents(1);

            // "Verify: That the WindowClosedEvent is fired for the application"
            TSC_VerifyEventListener(null, WindowPattern.WindowClosedEvent, EventFired.True, CheckType.Verification);
        }

        #endregion WindowClosedEvent

        #region WindowOpenedEvent

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("WindowOpenedEvent.1",
            TestSummary = "Verify that when a user opens the application, that the WindowOpenedEvent is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "WindowPattern.WindowOpenedEvent",
            Description = new string[] {
                "Step: Add WindowOpenedEvent listener for application",
                "Step: Start the application",
                "Step: Wait for one event to occur",
                "Verify: WindowOpenedEvent is fired for the application",
                "Cleanup: Close the application",
            })]
        public void TstWindowOpenEvent1(TestCaseAttribute testCase, object[] arguments)
        {
            Library.ValidateArgumentNonNull(testCase, "testCase");
            Library.ValidateArgumentNonNull(arguments, "arguments");
            Library.ValidateArgumentNonNull(arguments[0], "arguments[0]");

            Exception cachedException = null;

            string appPath = (string)arguments[0];

            HeaderComment(testCase);

            // "Step: Add WindowOpenedEvent listener for application",
            TSC_AddEventListener(AutomationElement.RootElement, WindowPattern.WindowOpenedEvent, TreeScope.Children, CheckType.Verification);

            // "Step: Start the application",
            TS_OpenWindow(appPath, null, CheckType.Verification);

            try
            {
                // "Step: Wait for one event to occur
                TSC_WaitForEvents(2);

                // "Verify: That the WindowOpenedEvent is fired for the application"
                TSC_VerifyEventListener(m_le, WindowPattern.WindowOpenedEvent, EventFired.True, CheckType.Verification);
            }
            catch (Exception error)
            {
                cachedException = error;
            }
            finally
            {
                // "Cleanup: Close the application",
                TS_CloseWindow(m_le, CheckType.Verification);

                if (cachedException != null)
                    throw cachedException;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("WindowOpenedEvent.2",
            TestSummary = "Verify that when a user opens font dialog in notepad, that the WindowOpenedEvent is fired",
            Priority = TestPriorities.Pri2,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "WindowPattern.WindowOpenedEvent",
            Description = new string[] {
                 "Step: Start the application",
                 "Step: Get the 'Format' menu item",
                 "Step: Expand the Format menu",
                 "Step: Get the 'Font..' menu item",
                 "Step: Add WindowOpenedEvent listener for application",
                 "Step: Invoke the font dialog in notepad.exe",
                 "Step: Wait for one event to occur",
                 "Step: Find the automationelement for the font dialog",
                 "Verify: WindowOpenedEvent is fired",
                 "Cleanup: Close the Font dialog",
                 "Cleanup: Close the application",
            })]
        public void TstWindowOpenedEvent2(TestCaseAttribute testCase, object[] arguments)
        {
            Library.ValidateArgumentNonNull(testCase, "testCase");
            Library.ValidateArgumentNonNull(arguments, "arguments");
            Library.ValidateArgumentNonNull(arguments[0], "arguments[0]");
            string appPath = (string)arguments[0];

            AutomationElement menu = null;
            AutomationElement dialog = null;
            Exception cachedException = null;

            HeaderComment(testCase);

            // "Step: Start the application",
            TS_OpenWindow(appPath, null, CheckType.Verification);

            try
            {
                // "Step: Get the 'Format' menu item
                TS_GetMenu(MENU_FORMAT, m_le, out menu, CheckType.Verification);

                // "Step: Expand the Format menu",
                TS_ExpandCollapsePatternExpand(menu, CheckType.Verification);

                // "Step: Get the 'Font..' menu item
                TS_GetMenu(MENU_FONT, menu, out menu, CheckType.Verification);

                // "Step: Add WindowOpenedEvent listener for application",
                TSC_AddEventListener(AutomationElement.RootElement, WindowPattern.WindowOpenedEvent, TreeScope.Subtree, CheckType.Verification);

                // "Step: Invoke the font dialog in notepad.exe 
                TS_InvokePatternInvoke(menu, CheckType.Verification);

                // "Step: Wait for one event to occur
                TSC_WaitForEvents(1);

                // "Step: Find the automationelement for the font dialog"
                TS_FindDialog(m_le, out dialog, CheckType.Verification);

                // "Verify: WindowOpenedEvent is fired",
                TSC_VerifyEventListener(dialog, WindowPattern.WindowOpenedEvent, EventFired.True, CheckType.Verification);
            }
            catch (Exception error)
            {
                cachedException = error;
            }
            finally
            {
                // "Step: Close the Font dialog
                if (dialog != null)
                    TS_CloseWindow(dialog, CheckType.Verification);

                // "Cleanup: Close the application",
                if (m_le != null)
                    TS_CloseWindow(m_le, CheckType.Verification);

                if (cachedException != null)
                    throw cachedException;
            }
        }
        
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("WindowOpenedevent.3",
            TestSummary = "Verify when the Run Dialog window is opened, WindowOpened event is fired",
            Priority = TestPriorities.Pri1,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "WindowPattern.WindowOpenedEvent",
            Description = new string[] {
                "Step: Open Start menu",
                "Step: Add WindowOpenedEvent listener for application",
                "Step: Open Run Dialog",
                "Step: Wait for one event to occur",
                "Verify: That the WindowClosedEvent is fired for the application",
                "Step: Press ESC to dismiss the window",
            })]
        public void TstWindowOpenedEvent3(TestCaseAttribute testCase)
        {
            AutomationElement dialog = null;
            Exception cachedException = null;

            HeaderComment(testCase);

            try
            {
                // "Step: Open Start menu",
                TS_OpenStartMenu(CheckType.Verification);

                // "Step: Add WindowOpenedEvent listener for application",
                TSC_AddEventListener(AutomationElement.RootElement, WindowPattern.WindowOpenedEvent, TreeScope.Subtree | TreeScope.Element, CheckType.Verification);

                // "Step: Open Run Dialog",
                TS_OpenRunDialog(out dialog, CheckType.Verification);

                // "Step: Wait for one event to occur",
                TSC_WaitForEvents(1);

                //"Verify: WindowPattern.WindowOpenedEvent is fired",
                TSC_VerifyEventListener(dialog, WindowPattern.WindowOpenedEvent, EventFired.True, CheckType.Verification);
            }
            catch (Exception error)
            {
                cachedException = error;
            }
            finally
            {
                // "Step: Press ESC to dismiss the window",
                if (dialog != null)
                    TS_PressKeys(true, Key.Escape);

                if (cachedException != null)
                    throw (cachedException);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_OpenStartMenu(CheckType checkType)
        {
            AutomationElement element = null;

            if (null == (element = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "Shell_TrayWnd"))))
                ThrowMe(checkType, "Could not find the Shell_TrayWnd window");

            if (null == (element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))))
                ThrowMe(checkType, "Could not find the Start Menu");

            if (false == (bool)element.GetCurrentPropertyValue(AutomationElement.IsInvokePatternAvailableProperty))
                ThrowMe(checkType, "Button does not support Invoke");

            ((InvokePattern)(element.GetCurrentPattern(InvokePattern.Pattern))).Invoke();

            AutomationElement startMenu = null;
            DateTime maxTime = DateTime.Now + TimeSpan.FromSeconds(5);
            int sleeptime = 500;
            for (; ; )
            {
                if (DateTime.Now > maxTime)
                    ThrowMe(checkType, "Could not find the start menu after invoking start button");
                startMenu = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "DV2ControlHost"));
                if (null != startMenu)
                    break;
                Thread.Sleep(sleeptime);
            }
            Comment("Start menu was opened verified by the ablility to find the DV2ControlHost window");
            m_TestStep++;

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_OpenRunDialog(out AutomationElement runDialog, CheckType checkType)
        {
            Comment("Attempting to open the Run dialog");

            AutomationElement element = null;
            if (null == (element = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "DV2ControlHost"))))
                ThrowMe(checkType, "Could not find the start menu after invoking start button");

            if (null == (element = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit))))
                ThrowMe(checkType, "Could not find the search edit window to type in the run.lnk");

            Debug.Assert((bool)element.GetCurrentPropertyValue(AutomationElement.IsValuePatternAvailableProperty));

            element.SetFocus();

            // Wait till the edit window has the focus
            DateTime maxTime = DateTime.Now + TimeSpan.FromSeconds(5);
            int sleeptime = 500;
            for (; ; )
            {
                if (DateTime.Now > maxTime)
                    ThrowMe(checkType, "Could not set focus to the edit window");
                if (true == (AutomationElement.FocusedElement == element))
                    break;
                Thread.Sleep(sleeptime);
            }

            ((ValuePattern)element.GetCurrentPattern(ValuePattern.Pattern)).SetValue("run.lnk");
            ATGTestInput.Input.SendKeyboardInput(Key.Enter);

            runDialog = null;
            maxTime = DateTime.Now + TimeSpan.FromSeconds(5);

            // Wait till the run dialog is opened...
            for (; ; )
            {
                if (DateTime.Now > maxTime)
                    ThrowMe(checkType, "Could not find the run dialog after invoking start button");
                runDialog = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "#32770"));
                if (null != runDialog)
                    break;
                Thread.Sleep(sleeptime);
            }

            m_TestStep++;

        }

        #endregion WindowClosedEvent

        #region MenuOpenedEvent

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("MenuOpenedEvent.1",
            TestSummary = "Verify that when a menu is opened, that the WindowOpenedEvent is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "AutomationElement.MenuOpenedEvent",
            Description = new string[] {
                "Step: Start the application",
                "Step: Add WindowOpenedEvent listener for application",
                "Step: Find the FileMenu and then expand it",
                "Step: Expand the 'File' menu",
                "Step: Wait for one event to occur",
                "Verify: That the WindowOpenedEvent is fired for the application",
                "Cleanup: Close the application"
            })]
        public void TstMenuOpenedEvent1(TestCaseAttribute testCase, object[] arguments)
        {
            Library.ValidateArgumentNonNull(testCase, "testCase");
            Library.ValidateArgumentNonNull(arguments, "arguments");
            Library.ValidateArgumentNonNull(arguments[0], "arguments[0]");
            string appPath = (string)arguments[0];
            
            AutomationElement fileMenuElement = null;
            Exception cachedException = null; 

            HeaderComment(testCase);

            // "Step: Start the application",
            TS_OpenWindow(appPath, null, CheckType.Verification);

            try
            {
                // "Step: Add WindowOpenedEvent listener for application",
                TSC_AddEventListener(m_le, AutomationElement.MenuOpenedEvent, TreeScope.Subtree, CheckType.Verification);

                // "Find the FileMenu and then expand it.",
                TS_GetMenu(MENU_FILE, m_le, out fileMenuElement, CheckType.Verification);

                // "Step: Expand the 'File' menu",
                TS_ExpandCollapsePatternExpand(fileMenuElement, CheckType.Verification);

                // "Step: Wait for one event to occur
                TSC_WaitForEvents(2);

                // "Verify: That the WindowOpenedEvent is fired for the application"
                // Getting First Child of the FileMenuElement, because that's the source of the event
                TSC_VerifyEventListener(TreeWalker.ControlViewWalker.GetFirstChild(fileMenuElement), AutomationElement.MenuOpenedEvent, EventFired.True, CheckType.Verification);
            }
            catch (Exception error)
            {
                cachedException = error;
            }
            finally
            {
                // "Close the application"
                if (m_le != null)
                    TS_CloseWindow(m_le, CheckType.Verification);

                if (cachedException != null)
                    throw cachedException;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("MenuOpenedEvent.2",
            TestSummary = "Verify that when a user opens start menu, that the MenuOpenedEvent is fired",
            Priority = TestPriorities.Pri1,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "AutomationElement.MenuOpenedEvent",
            Description = new string[] {
                "Step: Get the Start menu button",
                "Precondition: Verify that the start menu is visible",
                "Step: Add MenuOpenedEvent listener for start button",
                "Step: Invoke the Start Button",
                "Step: Wait for one event to occur",
                "Verify: That the WindowOpenedEvent is fired for the application",
                "Step: Press ESC to close the Start Menu"            
            })]
        public void TstMenuOpenedEvent2(TestCaseAttribute testCase)
        {
            Exception cachedException = null;

            HeaderComment(testCase);

            // "Step: Get the Start menu button",
            TS_GetStartMenuButton(ref m_le, CheckType.Verification);

            // Precondition: Verify that the start menu is visible",
            TSC_VerifyProperty(m_le.Current.IsOffscreen, false, "IsOffscreen", CheckType.IncorrectElementConfiguration);

            // "Step: Add MenuOpenedEvent listener for start button",
            TSC_AddEventListener(AutomationElement.RootElement, AutomationElement.MenuOpenedEvent, TreeScope.Element | TreeScope.Subtree, CheckType.Verification);

            try
            {
                // "Step: Invoke the Start Button",
                TS_InvokePatternInvoke(m_le, CheckType.Verification);

                // "Step: Wait for one event to occur
                TSC_WaitForEvents(1);

                // "Verify: That the WindowOpenedEvent is fired for the application"
                TSC_VerifyEventListener(TreeWalker.ControlViewWalker.GetFirstChild(TreeWalker.ControlViewWalker.GetParent(m_le)), AutomationElement.MenuOpenedEvent, EventFired.True, CheckType.Verification);
            }
            catch (Exception error)
            {
                cachedException = error;
            }
            finally
            {
                // "Step: Press ESC to close the Start Menu"
                TS_PressKeys(true, Key.Escape);

                if (cachedException != null)
                    throw cachedException;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("MenuOpenedEvent.3",
            TestSummary = "Verify that when invoking a menu in notepad using keyboard, that the MenuOpenedEvent is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "MenuOpenedEvent",
            Description = new string[] {
                "Step: Start the application",
                "Step: Add MenuOpenedEvent listener for application",
                "Step: Invoke the font dialog in notepad.exe by using the keys Alt, O",
                "Step: Wait for one event to occur",
                "Step: Get the format menu",
                "Verify: That the MenuOpenedEvent is fired for the application",
                "Step: Press ESC to remove focus from the menu",
                "Cleanup: Close the application",
            })]
        public void TstMenuOpenedEvent3(TestCaseAttribute testCase, object[] arguments)
        {
            Library.ValidateArgumentNonNull(testCase, "testCase");
            Library.ValidateArgumentNonNull(arguments, "arguments");
            Library.ValidateArgumentNonNull(arguments[0], "arguments[0]");
            string appPath = (string)arguments[0];

            Exception cachedException = null;
            AutomationElement menu = null;

            HeaderComment(testCase);

            // "Step: Start the application",
            TS_OpenWindow(appPath, null, CheckType.Verification);

            try
            {
                // "Step: Add MenuOpenedEvent listener for application",
                TSC_AddEventListener(AutomationElement.RootElement, AutomationElement.MenuOpenedEvent, TreeScope.Subtree, CheckType.Verification);

                // "Step: Invoke the font dialog in notepad.exe by using the keys Alt, O
                TS_PressKeys(true, Key.LeftAlt, Key.O);

                // "Step: Wait for one event to occur
                TSC_WaitForEvents(1);

                // "Step: Get the format menu",
                TS_GetMenu(MENU_FORMAT, m_le, out menu, CheckType.Verification);

                // "Verify: That the MenuOpenedEvent is fired for the application"
                TSC_VerifyEventListener(TreeWalker.ControlViewWalker.GetFirstChild(menu), AutomationElement.MenuOpenedEvent, EventFired.True, CheckType.Verification);
            }
            catch (Exception error)
            {
                cachedException = error;
            }
            finally
            {

                // "Step: Press ESC to remove focus from the menu
                TS_PressKeys(true, Key.Escape);

                // "Cleanup: Close the application",
                if (m_le != null)
                    TS_CloseWindow(m_le, CheckType.Verification);

                if (cachedException != null)
                    throw cachedException;
            }
        }

        #endregion MenuOpenedEvent

        #region MenuClosedEvent

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("MenuClosedEvent.1",
            TestSummary = "Verify that when invoking a menu in notepad using keyboard, then press ESC, that the MenuClosedEvent is fired",
            Priority = TestPriorities.Pri2,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "AutomationElement.MenuClosedEvent",
            Description = new string[] {
                 "Step: Start the application",
                 "Step: Add WindowOpenedEvent listener for application",
                 "Step: Get the menu",
                 "Step: Expand the menu",
                 "Step: Collapse the menu",
                 "Step: Wait for one event to occur",
                 "Verify: WindowOpenedEvent is fired",
                 "Cleanup: Close the application",
            })]
        public void TstMenuClosedEvent1(TestCaseAttribute testCase, object[] arguments)
        {
            //


            Library.ValidateArgumentNonNull(testCase, "testCase");
            Library.ValidateArgumentNonNull(arguments, "arguments");
            Library.ValidateArgumentNonNull(arguments[0], "arguments[0]");
            string appPath = (string)arguments[0];
            Exception cachedException = null;

            AutomationElement menu = null;

            HeaderComment(testCase);

            // "Step: Start the application",
            TS_OpenWindow(appPath, null, CheckType.Verification);

            try
            {
                // "Step: Add WindowOpenedEvent listener for application",
                TSC_AddEventListener(AutomationElement.RootElement, AutomationElement.MenuClosedEvent, TreeScope.Element, CheckType.Verification);

                // "Step: Get the menu",
                TS_GetMenu(MENU_FILE, m_le, out menu, CheckType.Verification);

                // "Step: Expand the menu",
                TS_ExpandMenu(menu, CheckType.Verification);

                // "Step: Collapse the menu",
                TS_CollapseMenu(menu, CheckType.Verification);

                // "Step: Wait for one event to occur
                TSC_WaitForEvents(1);

                // "Verify: WindowOpenedEvent is fired",
                TSC_VerifyEventListener(null, AutomationElement.MenuClosedEvent, EventFired.True, CheckType.Verification);
            }
            catch (Exception error)
            {
                cachedException = error;
            }
            finally
            {
                // "Cleanup: Close the application",
                if (m_le != null)
                    TS_CloseWindow(m_le, CheckType.Verification);

                if (cachedException != null)
                    throw cachedException;
            }
        }

        #endregion MenuClosedEvent

        #region ToolTipOpenedEvent

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ToolTipOpenedEvent.1",
            TestSummary = "Verify that when a tooltip is displayed, that the AutomationElement.ToolTipOpenedEvent is fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "AutomationElement.ToolTipOpenedEvent",
            Description = new string[] {
                "Step: Get the Start menu button",
                "Precondition: Verify that the start menu is visible",
                "Step: Add AutomationElement.ToolTipOpenedEvent listener",
                "Step: Move the mouse to the middle of the Start menu button so that an AutomationElement.ToolTipOpenedEvent is fired",
                "Step: Wait for one event to occur",
                "Precondition: Find the corrent tooltip",
                "Verify: That the AutomationElement.ToolTipOpenedEvent is fired ..."            
            })]
        public void TstToolTipOpenedEvent1(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            // "Step: Get the Start menu button",
            TS_GetStartMenuButton(ref m_le, CheckType.Verification);

            // Precondition: Verify that the start menu is visible",
            TSC_VerifyProperty(m_le.Current.IsOffscreen, false, "IsOffscreen", CheckType.IncorrectElementConfiguration);

            // "Step: Add WindowOpenedEvent listener for application",
            TSC_AddEventListener(AutomationElement.RootElement, AutomationElement.ToolTipOpenedEvent, TreeScope.Subtree, CheckType.Verification);

            // "Step: Move the mouse to the middle of the Start menu button so that an AutomationElement.ToolTipOpenedEvent is fired",
            TS_MoveTo(m_le, CheckType.Verification);

            // "Step: Wait for one event to occur
            TSC_WaitForEvents(3);

            // "Step: Find the corrent tooltip",
            // Sometimes the tooltip won't come up so if this happens, just bail for now and catch it the next time we run
            TS_ToolTipCurrentlyOpened(TreeWalker.ControlViewWalker.GetParent(m_le), ref m_le, CheckType.IncorrectElementConfiguration);

            // "Verify: That the WindowOpenedEvent is fired for the application"
            TSC_VerifyEventListener(m_le, AutomationElement.ToolTipOpenedEvent, EventFired.True, CheckType.Verification);
        }

        #endregion ToolTipOpenedEvent

        #region ToolTipClosedEvent

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ToolTipClosedEvent.1",
            TestSummary = "Verify that when a tooltip is displayed then goes away, that the AutomationElement.ToolTipClosedEvent is fired",
            Priority = TestPriorities.Pri2,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "AutomationElement.ToolTipClosedEvent",
            Description = new string[] {
                "Precondition: Need to cause a tooltip to show, so: Get the Start menu button",
                "Precondition: Verify that the start menu is visible",
                "Precondition: Add WindowOpenedEvent listener for application",
                "Precondition: Move the mouse to the middle of the Start menu button so that an AutomationElement.ToolTipOpenedEvent is fired",
                "Precondition: Wait for ToolTipOpenedEvent event to occur",
                "Precondition: Find the corrent tooltip",
                "Precondition: Remove all listeners",
                "Step: Add ToolTipClosedEvent listener for application",
                "Step: Move mouse to the 256,0, so that ToolTipClosedEvent can be fired",
                "Step: Wait for one event to occur",
                "Verify: That the ToolTipClosedEvent is fired for the application" 
            })]
        public void TstToolTipClosedEvent1(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            AutomationElement tooltip = null;

            // "Precondition: Need to cause a tooltip to show, so: Get the Start menu button",
            TS_GetStartMenuButton(ref m_le, CheckType.Verification);

            // Precondition: Verify that the start menu is visible",
            TSC_VerifyProperty(m_le.Current.IsOffscreen, false, "IsOffscreen", CheckType.IncorrectElementConfiguration);

            // "Precondition: Add WindowOpenedEvent listener for application",
            TSC_AddEventListener(AutomationElement.RootElement, AutomationElement.ToolTipOpenedEvent, TreeScope.Subtree, CheckType.Verification);

            // "Precondition: Move the mouse to the middle of the Start menu button so that an AutomationElement.ToolTipOpenedEvent is fired",
            TS_MoveTo(m_le, CheckType.Verification);

            // "Precondition: Wait for ToolTipOpenedEvent event to occur
            TSC_WaitForEvents(3);

            // "Precondition: Find the corrent tooltip",
            TS_ToolTipCurrentlyOpened(TreeWalker.ControlViewWalker.GetParent(m_le), ref tooltip, CheckType.IncorrectElementConfiguration);

            // Precondition: Remove all listeners
            TS_RemoveAllListeners(CheckType.Verification);

            // "Step: Add ToolTipClosedEvent listener for application",
            TSC_AddEventListener(AutomationElement.RootElement, AutomationElement.ToolTipClosedEvent, TreeScope.Element | TreeScope.Subtree, CheckType.Verification);

            // "Step: Move mouse to the 256,0, so that ToolTipClosedEvent can be fired.
            ATGTestInput.Input.MoveTo(new Point(256, 0)); m_TestStep++;

            // "Step: Wait for one event to occur
            //waiting for 5 seconds here because that's how long it takes for tooltipevents to propagate.
            TSC_WaitForEvents(5);

            // "Verify: That the ToolTipClosedEvent is fired for the application"
            TSC_VerifyEventListener(tooltip, AutomationElement.ToolTipClosedEvent, EventFired.True, CheckType.Verification);

        }

        #endregion ToolTipClosedEvent

        #region BoundingRectangleEvent

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("BoundingRectangleEvent.1",
            TestSummary = "Test that BoundingRectanglePropertyChangedEvent is fired when ctrltest is resized",
            Priority = TestPriorities.Pri1,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "AutomationElement.BoundingRectanglePropertyChangedEvent",
            Description = new string[] {
                "Step: Start the application",
                "Step: Resize the Ctrltest to a known size (100,100)",
                "Step: Add BoundingRectangleProperty listener for application",
                "Step: Resize Ctrltest to (300,300)",
                "Step: Wait for one event to occur",
                "Verify: That the BoundingRectangleProperty Event is fired ...",
                "Step: Close Application",
            })]
        public void TstBoundingRectangleEvent1(TestCaseAttribute testCase, object[] arguments)
        {
            Library.ValidateArgumentNonNull(testCase, "testCase");
            Library.ValidateArgumentNonNull(arguments, "arguments");
            Library.ValidateArgumentNonNull(arguments[0], "arguments[0]");

            Exception cachedException = null;
            string appPath = (string)arguments[0];

            HeaderComment(testCase);

            // "Step: Start the application",
            TS_OpenWindow(System.IO.Directory.GetCurrentDirectory() + @"\" + appPath, null, CheckType.Verification);

            // "Step: Resize the application to a known size",
            TS_ResizeWindow(m_le, 100, 100, CheckType.IncorrectElementConfiguration);

            try
            {
                // "Step: Add BoundingRectangleProperty listener for application",
                TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);

                // "Resize Ctrltest
                TS_ResizeWindow(m_le, 300, 300, CheckType.Verification);

                // "Step: Wait for one event to occur
                TSC_WaitForEvents(1);

                // "Verify: That the BoundingRectangleProperty event is fired for the application"
                TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);
            }
            catch (Exception error)
            {
                cachedException = error;
            }
            finally
            {
                // "Close Application",
                TS_CloseWindow(m_le, CheckType.Verification);

                if (cachedException != null)
                    throw cachedException;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("BoundingRectangleEvent.2",
            TestSummary = "Test that BoundingRectanglePropertyChangedEvent is fired when ctrltest is minimized",
            Priority = TestPriorities.Pri2,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            Client = Client.ATG,
            EventTested = "AutomationElement.BoundingRectanglePropertyChangedEvent",
            Description = new string[] {
                "Step: Start the application",
                "Step: Add BoundingRectangleProperty listener for application",
                "Step: minimize Ctrltest",
                "Step: Wait for one event to occur",
                "Verify: That the BoundingRectangleProperty Event is fired ...",
                "Step: Close Application",
            })]
        public void TstBoundingRectangleEvent2(TestCaseAttribute testCase, object[] arguments)
        {
            Library.ValidateArgumentNonNull(testCase, "testCase");
            Library.ValidateArgumentNonNull(arguments, "arguments");
            Library.ValidateArgumentNonNull(arguments[0], "arguments[0]");

            string appPath = (string)arguments[0];

            HeaderComment(testCase);

            // "Step: Start the application",
            TS_OpenWindow(System.IO.Directory.GetCurrentDirectory() + @"\" + appPath, null, CheckType.Verification);

            // "Step: Add BoundingRectangleProperty listener for application",
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);

            // "Minimize Ctrltest
            TS_SetWindowVisualState(m_le, WindowVisualState.Minimized, CheckType.Verification);

            // "Step: Wait for one event to occur
            TSC_WaitForEvents(1);

            // "Verify: That the BoundingRectangleProperty event is fired for the application"
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);

            // "Close Application",
            TS_CloseWindow(m_le, CheckType.Verification);

        }

        #endregion BoundingRectangleEvent


        #endregion Tests

        #region Test Steps

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_SupportsMainMenu(bool expected, CheckType checkType)
        {
            bool supports = _appCommands.SupportsIWUIMenuCommands();
            if (supports != expected)
                ThrowMe(checkType, "Supports menus != " + expected);

            Comment("Application supports menus");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_CloseWindow(AutomationElement element, CheckType checkType)
        {
            WindowPattern wp = element.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;

            if (wp == null)
                ThrowMe(checkType, "Could not find the WindowPattern");

            wp.Close();
            Comment("Called WindowPattern.Close() on the element");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>Opens the application specified by the path, and returns the AutomationElement for the application</summary>
        /// -------------------------------------------------------------------
        private void TS_OpenWindow(string appPath, string arguments, CheckType checkType)
        {
            IntPtr handle;
            Process process = null;
            Comment("Starting {0}", appPath);

            Library.StartApplication(appPath, arguments, out handle, out m_le, out process);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_GetFirstSubmenu(ref TestMenu menu, CheckType checkType)
        {

            IWUIMenuCommands menuCommands = _appCommands.GetIWUIMenuCommands();
            TestMenu m = menuCommands.GetMenuBar();
            if (m == null)
                ThrowMe(checkType, "Could not get menu bar");


            if ((m = m.GetFirstSubMenu()) == null)
                ThrowMe(checkType, "Could not get 1st sub menu");

            Comment("Found menu : " + m.AutomationElement.Current.Name);

            menu = m;

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_ExpandMenu(TestMenu tmenu, CheckType checkType)
        {
            // Step: Expand the 1st menu so that a WindowOpenedEvent is fired
            Comment(tmenu.AutomationElement.Current.Name);
            tmenu.Expand();

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_GetMenuItemsSubMenu(TestMenu tmenu, out AutomationElement menu, CheckType checkType)
        {
            menu = TreeWalker.ControlViewWalker.GetFirstChild(tmenu.AutomationElement);
            Comment("Found 1st child of '" + tmenu.AutomationElement.Current.Name + "' -> '" + menu.Current.Name + "'");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_GetStartMenuButton(ref AutomationElement startButtonElement, CheckType checkType)
        {
            AutomationElement element;

            element = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "Shell_TrayWnd"));
            if (element == null)
                ThrowMe(checkType, "Could not find the tray the the Start menu button is located");

            element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AccessKeyProperty, "Ctrl + Esc"));
            if (element == null)
                ThrowMe(checkType, "Could not find the Start menu button with AutomationElement.AccessKeyProperty == Ctrl + Esc");

            startButtonElement = element;

            Comment("Found the start menu (" + Library.GetUISpyLook(startButtonElement) + ")");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_MoveTo(AutomationElement element, CheckType checkType)
        {
            ATGTestInput.Input.MoveTo(element);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_InvokePatternInvoke(AutomationElement element, CheckType checkType)
        {
            InvokePattern ip = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if (ip == null)
                ThrowMe(checkType, "Element[" + element.Current.Name + "] does not support InvokePattern");

            ip.Invoke();

            Comment("InvokePattern.Invoke(element[" + element.Current.Name + "])");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_ExpandCollapsePatternExpand(AutomationElement element, CheckType checkType)
        {
            ExpandCollapsePattern ecp = element.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            if (ecp == null)
                ThrowMe(checkType, "Element[" + element.Current.Name + "] does not support ExpandCollapsePattern");

            ecp.Expand();
            Comment("Expanded element[" + element.Current.Name);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_ToolTipCurrentlyOpened(AutomationElement parent, ref AutomationElement element, CheckType checkType)
        {
            AutomationElement tip = parent.FindFirst(
                    TreeScope.Children,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolTip));

            if (tip == null)
                ThrowMe(checkType, "Could not find ToolTip");

            Comment("Found Tooltip[" + Library.GetUISpyLook(tip) + "]");

            element = tip;

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_FindDialog(AutomationElement root, out AutomationElement dialogElement, CheckType checkType)
        {
            // "Step: Find the automationelement for the font dialog"
            dialogElement = root.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ClassNameProperty, "#32770"));
            if (dialogElement == null)
                ThrowMe(checkType, "Could not find dialog");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_RemoveAllListeners(CheckType checkType)
        {
            Automation.RemoveAllEventHandlers();

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_ExpandMenu(AutomationElement menu, CheckType checkType)
        {
            if (false == (bool)menu.GetCurrentPropertyValue(AutomationElement.IsExpandCollapsePatternAvailableProperty))
            {
                ThrowMe(checkType, "Menu does not support ExpandCollapsePattern");
            }

            int timeout = 0;

            ExpandCollapsePattern ecp = menu.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            ecp.Expand();

            // Yikes sleep!  But we are testing events, so cannot rely on eventing to determine when we are done.
            while (ecp.Current.ExpandCollapseState == ExpandCollapseState.Collapsed && timeout < _MAX_TIME)
            {
                Thread.Sleep(_INCREMENT);
                timeout += _INCREMENT;
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_CollapseMenu(AutomationElement menu, CheckType checkType)
        {
            if (false == (bool)menu.GetCurrentPropertyValue(AutomationElement.IsExpandCollapsePatternAvailableProperty))
            {
                ThrowMe(checkType, "Menu does not support ExpandCollapsePattern");
            }

            int timeout = 0;

            ExpandCollapsePattern ecp = menu.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            ecp.Collapse();

            // Yikes sleep!  But we are testing events, so cannot rely on eventing to determine when we are done.
            while (ecp.Current.ExpandCollapseState != ExpandCollapseState.Collapsed && timeout < _MAX_TIME)
            {
                Thread.Sleep(_INCREMENT);
                timeout += _INCREMENT;
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_GetMenu(string id, AutomationElement root, out AutomationElement menu, CheckType checkType)
        {

            if (root == m_le)
            {   // Special case to get past the system menu
                // Get app's menu bar
                root = root.FindFirst(TreeScope.Subtree,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "MenuBar"));
            }

            menu = root.FindFirst(TreeScope.Subtree,
                new AndCondition(new System.Windows.Automation.Condition[]{new PropertyCondition(AutomationElement.AutomationIdProperty, id), 
                                            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem)}));
            if (menu == null)
                ThrowMe(checkType, "Could not find the menu (" + id + ")");

            m_TestStep++;
        }
        #endregion Test Steps
    }
}
