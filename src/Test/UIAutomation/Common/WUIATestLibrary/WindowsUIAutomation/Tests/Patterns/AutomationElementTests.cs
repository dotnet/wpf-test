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
using System.Reflection;
using MS.Win32;
using Accessibility;
using System.Runtime.InteropServices;
using System.Text;

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
    public class AutomationElementWrapper : PatternObject
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal AutomationElementWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
        base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _testCaseSampleType = TestCaseSampleType.AutomationElement;
        }
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

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public sealed class AutomationElementTests : AutomationElementWrapper
    {
        #region Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------


        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        Drawing.Color _maskColor = Drawing.Color.FromArgb(0, 255, 0);

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        Drawing.Color _textColor = Drawing.Color.FromArgb(0, 0, 255);

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        Rect _parentRect;

        #endregion Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const string THIS = "AutomationElementTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS; //TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public AutomationElementTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, AutomationElementTests.TestSuite, priority, typeOfControl, TypeOfPattern.Unknown, dirResults, testEvents, commands)
        {
        }

        #region AutomationElementWrapper

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        object pattern_GetProperty(AutomationElement element, AutomationProperty property)
        {
            if (m_useCurrent)
                return element.GetCurrentPropertyValue(property);
            else
                return element.GetCachedPropertyValue(property);
        }

        #endregion AutomationElementWrapper

        #region Property Change

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("AutomationElement.PropertyChange.IsOffScreen.1",
            TestSummary = "Verify that PropertyChangeEvent is fired for AutomationElement.IsOffScreen when the application is moved off screen and back on screen",
            Priority = TestPriorities.Pri1,
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            EventTested = "PropertyChange.IsOffScreen",
            Status = TestStatus.Problem,
            ProblemDescription = "Can't determine when an object fires the property.  Need to talk to Thomas more about this",
            Client = Client.ATG,
            Description = new string[] 
            {
                "Move the element to position 0,0 on the screen",
                "Step: Add Property Change Listener",
                "Move the element to position off the screen",
                "Step: Wait for 1 event",
                "Verify: That a property change event was fired",
                "Move the element back onto the screen to position 0,0 on the screen",
                "Step: Wait for 1 event",
                "Verify: That a property change event was fired",
                "Move the element to original location",
            })]
        public void PropertyChangeIsOffScreen1(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            Rect originalRect = Rect.Empty;
            AutomationElement windowElement = null;
            Exception tempException = null;

            AutomationProperty[] properties = new AutomationProperty[] { AutomationElement.IsOffscreenProperty };
            EventFired[] eventsFired = new EventFired[] { EventFired.True };

            try
            {
                // Move the element to position 0,0 on the screen
                TS_MoveElementsWindow(m_le, 0, 0, out originalRect, out windowElement, CheckType.IncorrectElementConfiguration);

                // "Step: Add Property Change Listener",
                TSC_AddPropertyChangedListener(m_le, TreeScope.Element, properties, CheckType.Verification);

                // Move the element to position off the screen
                TS_MoveElementsWindow(windowElement, int.MaxValue / 2, int.MaxValue / 2, CheckType.IncorrectElementConfiguration);

                // "Step: Wait for 1 event",
                TSC_WaitForEvents(1);

                // "Verify: That a property change event was fired"
                TSC_VerifyPropertyChangedListener(m_le, eventsFired, properties, CheckType.Verification);

                // Move the element back onto the screen to position 0,0 on the screen
                TS_MoveElementsWindow(windowElement, 0, 0, CheckType.IncorrectElementConfiguration);

                // "Step: Wait for 1 event",
                TSC_WaitForEvents(1);

                // "Verify: That a property change event was fired"
                TSC_VerifyPropertyChangedListener(m_le, eventsFired, properties, CheckType.Verification);

                // Move the element to original location
                TS_MoveElementsWindow(windowElement, originalRect.X, originalRect.Y, CheckType.IncorrectElementConfiguration);
            }
            catch (TestErrorException exception)
            {
                tempException = exception;
            }
            finally
            {
                // Something might have failed the test, so let's make sure we put the window back to the original location
                if (windowElement != null && originalRect != Rect.Empty)
                {
                    TS_MoveElementsWindow(windowElement, originalRect.X, originalRect.Y, CheckType.IncorrectElementConfiguration);
                }
                if (tempException != null)
                    throw tempException;
            }

        }


        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("AutomationElement.PropertyChange.Name.1",
            TestSummary = "Verify that PropertyChangeEvent is fired for AutomationElement.NameProperty",
            Priority = TestPriorities.Pri2,
            BugNumbers = new BugIssues[] {BugIssues.PS27 },
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            EventTested = "PropertyChange.Name",
            Status = TestStatus.Works,
            Client = Client.ATG,
            Description = new string[] 
                    {
                        "Precondition: Verify that this control supports testing PropertyChanges",
                        "Step: Reset the control to it's original state",
                        "Step: Display the current AutomationElement.NameProperty",
                        "Step: Add Property Change Listener",
                        "Step: Change the name of the element",
                        "Step: Wait for 1 event",
                        "Step: Display the current AutomationElement.NameProperty",
                        "Verify: That a property change event was fired",
                        "Step: Reset the control to it's initial state",
                        "Step: Display the current AutomationElement.NameProperty",
                    }
           )
        ]
        public void PropertyChangeName1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);
            Exception caughtException = null;

            try
            {
                AutomationProperty[] properties = new AutomationProperty[] { AutomationElement.NameProperty };
                EventFired[] eventsFired = new EventFired[] { EventFired.True };

                // "Precondition: Verify that this control supports testing PropertyChanges",
                TS_SupportsPropertyChangeEvents(CheckType.IncorrectElementConfiguration);

                // "Step: Reset the control to it's original state",
                TS_ResetControlToInitialState(m_le, CheckType.Verification);

                //"Step: Display the current name",
                TS_LogProperty(m_le, AutomationElement.NameProperty, CheckType.Verification);

                // "Step: Add Property Change Listener",
                TSC_AddPropertyChangedListener(m_le, TreeScope.Element, properties, CheckType.Verification);

                // "Step: Change the name of the element",
                TS_CausePropertyChange(m_le, AutomationElement.NameProperty, CheckType.Verification);

                // "Step: Wait for 1 event",
                TSC_WaitForEvents(1);

                //"Step: Display the new name",
                TS_LogProperty(m_le, AutomationElement.NameProperty, CheckType.Verification);

                // "Verify: That a property change event was fired"
                if (!TS_FilterOnBug(testCaseAttribute))
                    TSC_VerifyPropertyChangedListener(m_le, eventsFired, properties, CheckType.Verification);
            }
            catch (Exception error)
            {
                caughtException = error;
            }
            finally
            {
                if (!(caughtException is IncorrectElementConfigurationForTestException))
                {
                    // Step: Reset the control to it's initial state
                    TS_ResetControlToInitialState(m_le, CheckType.Verification);

                    //"Step: Display the name",
                    TS_LogProperty(m_le, AutomationElement.NameProperty, CheckType.Verification);
                }

                if (caughtException != null)
                    throw caughtException;
            }
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("AutomationElement.PropertyChange.Enabled.1",
            TestSummary = "Verify that PropertyChangeEvent is fired when disabled element is enabled",
            BugNumbers = new BugIssues[] { BugIssues.PS27 },
            Priority = TestPriorities.Pri2,
            TestCaseType = TestCaseType.Scenario | TestCaseType.Events,
            EventTested = "PropertyChange.IsEnable",
            Status = TestStatus.Works,
            Client = Client.ATG,
            Description = new string[] 
                    {
                        "Precondition: Verify that this control supports testing PropertyChanges",
                        "Step: Reset the control to it's original state",
                        "Step: Display the current AutomationElement.IsEnabledProperty",
                        "Step: Add Property Change Listener",
                        "Step: Change the AutomationElement.IsEnabledProperty of the element",
                        "Step: Wait for 1 event",
                        "Step: Display the current AutomationElement.IsEnabledProperty",
                        "Verify: That a property change event was fired",
                        "Step: Reset the control to it's initial state",
                        "Step: Display the current AutomationElement.IsEnabledProperty",
                    }
            )
        ]
        public void PropertyChangeEnabled1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);
            
            Exception caughtException = null;

            try
            {

                AutomationProperty[] properties = new AutomationProperty[] { AutomationElement.NameProperty };
                EventFired[] eventsFired = new EventFired[] { EventFired.True };

                // "Precondition: Verify that this control supports testing PropertyChanges",
                TS_SupportsPropertyChangeEvents(CheckType.IncorrectElementConfiguration);

                // "Step: Reset the control to it's original state",
                TS_ResetControlToInitialState(m_le, CheckType.Verification);

                //"Step: Display the current AutomationElement.IsEnabledProperty",
                TSC_VerifyProperty(m_le.Current.IsEnabled, true, AutomationElement.IsEnabledProperty, CheckType.Verification);

                // "Step: Add Property Change Listener",
                TSC_AddPropertyChangedListener(m_le, TreeScope.Element, properties, CheckType.Verification);

                // "Step: Change the AutomationElement.IsEnabledProperty of the element",
                TS_CausePropertyChange(m_le, AutomationElement.IsEnabledProperty, CheckType.Verification);

                // "Step: Wait for 1 event",
                TSC_WaitForEvents(1);

                //"Step: Display the current AutomationElement.IsEnabledProperty",
                TSC_VerifyProperty(m_le.Current.IsEnabled, false, AutomationElement.IsEnabledProperty, CheckType.Verification);

                // "Verify: That a property change event was fired"
                if (!TS_FilterOnBug(testCaseAttribute))
                    TSC_VerifyPropertyChangedListener(m_le, eventsFired, properties, CheckType.Verification);
            }
            catch (Exception error)
            {
                caughtException = error;
            }
            finally
            {
                if (!(caughtException is IncorrectElementConfigurationForTestException))
                {

                    // Step: Reset the control to it's initial state
                    TS_ResetControlToInitialState(m_le, CheckType.Verification);

                    //"Step: Display the current AutomationElement.IsEnabledProperty",
                    TS_LogProperty(m_le, AutomationElement.IsEnabledProperty, CheckType.Verification);
                }

                if (caughtException != null)
                    throw caughtException;
            }

        }

        #endregion Property Change

        #region DPI Bounding Rect work
        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("HwndWindowRect.1.UIA",
           TestSummary = "Verify UIA rectangle is correct for Hwnd based window",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Problem,
            ProblemDescription = "This does not work on HighDPI120 since GetWindowRect returns non scaled numbers",
            Description = new string[] 
                {
                "Precondition: Element has a valid hwnd",
                "Verification: Obtain the element's rectangle using Win32's GetWindowRect()",
                "Verification: That the AutomationElement.GetBoundingRectangle is the same as the Win32's GetWindowRect()",
                })]
        public void HwndWindowRect1UIA(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            Rect rc = new Rect();

            //"Precondition: Element has a valid hwnd",
            TSC_VerifyProperty(m_le.Current.NativeWindowHandle, 0, false, "Current.NativeWindowHandle", CheckType.IncorrectElementConfiguration);

            //"Precondition: Obtain the element's rectangle using Win32 GetWindowRect()",
            TS_GetWin32GetWindowRect(m_le, out rc, CheckType.Verification);

            //"Verify: That the AutomationElement.GetBoundingRectangle is the same as the Win32 GetWindowRect()",
            TS_VerifyBoundingRect(m_le, rc, CheckType.Verification);
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("HwndWindowRect.2.UIA",
           TestSummary = "Verify UIA rectangle available",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Description = new string[] 
                {
                "Verification: Obtain AutomationElement.GetBoundingRectangle (regression for TitleBar )",
                })]
        public void HwndWindowRect2UIA(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            // Get the UIA reported window rect
            Rect rcUIA = m_le.Current.BoundingRectangle;

            Comment("UIA BoundingRectangle returned (Left={0}, Top={1}, Right={2}, Bottom={3}", rcUIA.Left, rcUIA.Top, rcUIA.Right, rcUIA.Bottom);
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("HwndWindowRect.1.MSAA",
            TestSummary = "Verify MSAA rectangle is correct for Hwnd based window",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Description = new string[] 
                {
                "Precondition: Element has a valid hwnd",
                "Verification: Obtain the element's rectangle using IAccessible::get_accLocation()",
                "Verification: Obtain the element's rectangle using Win32 GetWindowRect()",
                "Verification: That the AutomationElement.GetBoundingRectangle is the same as IAccessible::get_accLocation()",
                })]
        public void HwndWindowRect1MSAA(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            Accessibility.IAccessible acc;

            Rect rc = new Rect();

            //"Precondition: Element has a valid hwnd",
            TSC_VerifyProperty(m_le.Current.NativeWindowHandle, 0, false, "Current.NativeWindowHandle", CheckType.IncorrectElementConfiguration);

            // "Verification: Obtain the element's rectangle using IAccessible::get_accLocation()",
            TS_GetIAccessibleFromAutomationElement(m_le, out acc, CheckType.Verification);

            //"Verification: Obtain the element's rectangle using Win32 GetWindowRect()",
            TS_GetaccLocation(m_le, out rc, CheckType.Verification);

            //"Verify: That the AutomationElement.GetBoundingRectangle is the same as the Win32 GetWindowRect()",
            TS_VerifyBoundingRect(m_le, rc, CheckType.Verification);
        }

        #endregion DPI Bounding Rect work

        #region 1.1 KeyboardFocusable //Pri0 property

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("IsKeyboardFocusable.1.1.1",
            TestSummary = "If IsKeyboardFocusable = false, call SetFocus() and verify that KeyboardFocusabe is still false and no events were fired",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Description = new string[] 
                {
                "Precondition: Verify that this control supports SetFocus tests, some controls such as MenuItem's do not support SetFocus()",
                "Precondition: IsKeyboardFocusable == false",
                "Verify: Call SetFocus() and expect an exception to be thrown",
                "Verify: IsKeyboardFocusable still false"
                })]
        public void TestKeyboardFocusable111(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            //"Precondition: Verify that this control supports SetFocus tests",
            TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);

            // "Precondition: IsKeyboardFocusable == false",
            TSC_VerifyProperty(pattern_GetProperty(m_le, AutomationElement.IsKeyboardFocusableProperty), false, AutomationElement.IsKeyboardFocusableProperty, CheckType.IncorrectElementConfiguration);

            // "Verify: call SetFocus and expect an exception to be thrown",
            TS_SetFocus(m_le, typeof(InvalidOperationException)/*null*/, CheckType.Verification);

            // "Verify: IsKeyboardFocusable still false"
            TSC_VerifyProperty(pattern_GetProperty(m_le, AutomationElement.IsKeyboardFocusableProperty), false, AutomationElement.IsKeyboardFocusableProperty, CheckType.Verification);
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("KeyboardFocusable.1.1.2",
            TestSummary = "If KeyboardFocusable = true, call SetFocus() and verify that IsKeyboardFocusable is still true and PropertyChangedEvent was fired",
            Priority = TestPriorities.Pri1, //Should be Pri0
            Status = TestStatus.Works,
            Description = new string[] 
                {
                "Precondition: Verify that this control supports SetFocus tests, some controls such as MenuItem's do not support SetFocus()",
                "Precondition: IsKeyboardFocusable == true",
                "Verify: Call SetFocus() set focus to the element",
                "Verify: IsKeyboardFocusable still true"
                })]
        public void TestKeyboardFocusable112(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            //"Precondition: Verify that this control supports SetFocus tests",
            TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);

            // "Precondition: IsKeyboardFocusable == true",
            TSC_VerifyProperty(pattern_GetProperty(m_le, AutomationElement.IsKeyboardFocusableProperty), true, AutomationElement.IsKeyboardFocusableProperty, CheckType.IncorrectElementConfiguration);

            // "Verify: call SetFocus set focus to the element",
            TS_SetFocus(m_le, null, CheckType.Verification);

            //"Verify: IsKeyboardFocusable still true"
            TSC_VerifyProperty(pattern_GetProperty(m_le, AutomationElement.IsKeyboardFocusableProperty), true, AutomationElement.IsKeyboardFocusableProperty, CheckType.Verification);
        }

        #endregion

        #region 1.2 KeyboardHelpText

        #endregion

        #region 1.3 IsPasswordProperty //Pri0 property

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("IsPasswordProperty.1.3.1",
            TestSummary = "For Win32 controls, look at the style bit and verify that it conforms to the correct value obtained from the IsPasswordProperty",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Scenario,
            Status = TestStatus.WorkingOn,
            Description = new string[] 
                {
                    "Scenario not written yet"
                })]
        public void TestIsPasswordProperty131(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("IsPasswordProperty.1.3.2",
            TestSummary = "If IsPasswordProperty == true, Calling ValuePattern.Value throws an InvalidOperastionException",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Description = new string[] 
                    {
                    "Precondition: AutomationElement.IsPasswordProperty == true",
                    "Verify: Calling ValuePattern.Value does not return the password",
                    })]
        public void TestIsPasswordProperty132(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            // "Precondition: AutomationElement.IsPasswordProperty == true",
            TSC_VerifyProperty(pattern_GetProperty(m_le, AutomationElement.IsPasswordProperty), true, AutomationElement.IsKeyboardFocusableProperty, CheckType.IncorrectElementConfiguration);

            // "Verify: Calling ValuePattern.Value does not return the password",
            TS_PasswordText(m_le, CheckType.Verification);
        }

        #endregion IsPasswordProperty

        #region 1.4 LabelBy //Pri0 property

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("LabelBy.1.4.1",
            TestSummary = "Verify that the specific control specified has this set correctly.",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.WorkingOn,
            Description = new string[] 
                    {
                        "Scenario not written"
                    })]
        public void TestIsLabelBy141(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);
        }

        #endregion 1.4 LabelBy

        #region 1.5 HelpTextProperty //Pri2 property

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("HelpTextProperty.1.5.2",
            TestSummary = "With a known control that has an hwnd, verify that KeyboardHelpText != string.IsNullOrEmpty, and KeyboardHelpText != NotSupported",
            TestCaseType = TestCaseType.Scenario,
            Priority = TestPriorities.Pri1,
            Status = TestStatus.WorkingOn,
            Description = new string[] 
                {
                    "Precondition: AutomationElement has a valid hwnd",
                    "Verification: HelpTextProeprty != null",
                    "Verification: HelpTextProeprty != \"\"",
                    "Verification: HelpTextProeprty != AutomationElement.NotSupported",
                })]
        public void HelpTextProperty152(TestCaseAttribute testCaseAttribute)
        {
            // "Precondition: Element has a valid hwnd",
            TSC_VerifyProperty(m_le.Current.NativeWindowHandle, 0, false, "Current.NativeWindowHandle", CheckType.IncorrectElementConfiguration);

            // "Verification: HelpTextProeprty != null",
            TSC_VerifyProperty(m_le.Current.HelpText, null, false, "Current.HelpText", CheckType.IncorrectElementConfiguration);

            // "Verification: HelpTextProeprty != \"\"",
            TSC_VerifyProperty(m_le.Current.HelpText, "", false, "Current.HelpText", CheckType.IncorrectElementConfiguration);

            // "Verification: HelpTextProeprty != AutomationElement.NotSupported",
            TSC_VerifyProperty(m_le.Current.HelpText, AutomationElement.NotSupported, false, "Current.HelpText", CheckType.IncorrectElementConfiguration);
        }

        #endregion 1.5 HelpTextProperty

        #region 1.7 AutomationIdProperty //Pri0 property

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("AutomationIdProperty.1.7.1",
            TestSummary = "Test that the AutomationElement has no sibling with the same AutomationId",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Description = new string[] 
                    {
                    "Very: No siblings have the same AutomationIdProperty"
                    })]
        public void AutomationIdProperty171(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);
            AutomationElement element = TreeWalker.ControlViewWalker.GetFirstChild(TreeWalker.ControlViewWalker.GetParent(m_le));

            ArrayList list = new ArrayList();

            string id;
            while (element != null)
            {
                id = pattern_GetProperty(element, AutomationElement.AutomationIdProperty) as string;
                // Comment("Found ID(" + id + ") on :" + Library.GetControlTypePath(element));
                if (!string.IsNullOrEmpty(id))
                {
                    if (list.IndexOf(id) == -1)
                        list.Add(id);
                    else
                    {
                        ThrowMe(CheckType.Verification, "There were duplicate AutomationIdProperty's where AutomationIdProperty = " + id);
                    }
                }
                element = TreeWalker.ControlViewWalker.GetNextSibling(element);
            }
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("AutomationIdProperty.1.7.8",
            TestSummary = "Test that items that have the ControlView = true, has an AutomationIdProperty that is not string.Empty",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Description = new string[] 
            {
                "PreCondition: Element must support ControlType property to determine if supporting ControlType is required",
                "Precondition: Element does not support the WindowPattern, thus is not a window",
                "Precondition: IsControlElementProperty == true",
                "Precondition: ControlType != ListItem (known item that does not support AutomationIdProperty)",
                "Precondition: ControlType != DataItem (known item that does not support AutomationIdProperty)",
                "Precondition: ControlType != TreeItem (known item that does not support AutomationIdProperty)",
                "Precondition: ControlType != TabViewItem (known item that does not support AutomationIdProperty)",
                "Precondition: ControlType.Parent != ListItem(known item that does not support AutomationIdProperty)",
                "Precondition: ControlType.Parent != DataItem(known item that does not support AutomationIdProperty)",
                "Precondition: WPF DataGrid - ControlType != Header(known item that does not support AutomationIdProperty)",
                "Precondition: WPF DataGrid - ControlType != HeaderItem(known item that does not support AutomationIdProperty)",
                "Precondition: WPF DataGrid - ControlType != Custom(RowsPresenter-known item that does not support AutomationIdProperty)",
                "Verify: AutomationIdProperty != string.Empty",
                "Verify: AutomationIdProperty != 0"
            })]
        public void AutomationIdProperty178(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            ControlType ct = m_le.Current.ControlType;
            ControlType pct = TreeWalker.ControlViewWalker.GetParent(m_le).Current.ControlType;


            Comment("AutomationIdProperty = " + pattern_GetProperty(m_le, AutomationElement.AutomationIdProperty));
            TSC_VerifyProperty(ct, null, false, "ControlType", CheckType.IncorrectElementConfiguration);
            TSC_VerifyProperty((bool)m_le.GetCurrentPropertyValue(AutomationElement.IsWindowPatternAvailableProperty), false, AutomationElement.IsWindowPatternAvailableProperty, CheckType.IncorrectElementConfiguration);
            TSC_VerifyProperty(m_le.Current.IsControlElement, true, true, AutomationElement.IsControlElementProperty, CheckType.IncorrectElementConfiguration);
            TSC_VerifyProperty(Helpers.GetProgrammaticName(ct), Helpers.GetProgrammaticName(ControlType.ListItem), false, "ControlType", CheckType.IncorrectElementConfiguration);
            TSC_VerifyProperty(Helpers.GetProgrammaticName(ct), Helpers.GetProgrammaticName(ControlType.DataItem), false, "ControlType", CheckType.IncorrectElementConfiguration);
            TSC_VerifyProperty(Helpers.GetProgrammaticName(ct), Helpers.GetProgrammaticName(ControlType.TreeItem), false, "ControlType", CheckType.IncorrectElementConfiguration);
            TSC_VerifyProperty(Helpers.GetProgrammaticName(ct), Helpers.GetProgrammaticName(ControlType.TabItem), false, "ControlType", CheckType.IncorrectElementConfiguration);
            TSC_VerifyProperty(Helpers.GetProgrammaticName(pct), Helpers.GetProgrammaticName(ControlType.ListItem), false, "ControlType", CheckType.IncorrectElementConfiguration);
            TSC_VerifyProperty(Helpers.GetProgrammaticName(pct), Helpers.GetProgrammaticName(ControlType.DataItem), false, "ControlType", CheckType.IncorrectElementConfiguration);
            if (CheckWPFDataGridElement(m_le))
            {
                TSC_VerifyProperty(Helpers.GetProgrammaticName(ct), Helpers.GetProgrammaticName(ControlType.Header), false, "ControlType", CheckType.IncorrectElementConfiguration);
                TSC_VerifyProperty(Helpers.GetProgrammaticName(ct), Helpers.GetProgrammaticName(ControlType.HeaderItem), false, "ControlType", CheckType.IncorrectElementConfiguration);
                TSC_VerifyProperty(Helpers.GetProgrammaticName(pct), Helpers.GetProgrammaticName(ControlType.DataGrid), false, "ControlType", CheckType.IncorrectElementConfiguration);
            }            
            TSC_VerifyProperty(m_le.Current.AutomationId, string.Empty, false, AutomationElement.AutomationIdProperty, CheckType.Verification);
            TSC_VerifyProperty(m_le.Current.AutomationId, 0, false, AutomationElement.AutomationIdProperty, CheckType.Verification);
        }

        #endregion 1.7 AutomationIdProperty

        #region 1.8 LocalizedControlType
        [TestCaseAttribute("LocalizedControlType.1.8.1",
           TestSummary = "Test that an element has a non zero length string associated with the localized control type",
           Priority = TestPriorities.Pri0,
           Status = TestStatus.Works,
           Description = new string[] 
            {
                "Verify: LocalizedControlType != null",
                "Verify: LocalizedControlType.Length != 0",
                "Verify: LocalizedControlType != string.Empty",
            })]
        public void LocalizedControlType181(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            TSC_VerifyProperty(m_le.Current.LocalizedControlType, null, false, "LocalizedControlType", CheckType.IncorrectElementConfiguration);
            TSC_VerifyProperty(m_le.Current.LocalizedControlType.Length, 0, false, "LocalizedControlType", CheckType.IncorrectElementConfiguration);
            TSC_VerifyProperty(m_le.Current.LocalizedControlType, string.Empty, false, "LocalizedControlType", CheckType.IncorrectElementConfiguration);

        }
        #endregion 1.8 LocalizedControlType

        #region Static Tests

        #endregion

        #region Successful calls to GetCurrentPattern

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("GetCurrentPattern.1",
            TestSummary = "Enumerate through all the Automation.Pattern variables, and call GetCurrentPattern().  If the value of Is*PatternAvailableProperty true, verify that GetCurrentPattern() succeeds, else if Is*PatternAvailableProperty is false then InvalidOperationException is thrown ",
            Priority = TestPriorities.Pri2,
            Status = TestStatus.Works,
            Description = new string[]
            {
                "Verify success call to GetCurrentPattern(*Pattern.PatternObject)"
            })]
        public void TestDockPattern(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            string IDS_PatternAvailableProperty = "PatternAvailableProperty";

            Assembly assembly = Assembly.GetAssembly(typeof(AutomationElement));
            AutomationPattern patternId;
            bool isSupported;

            foreach (FieldInfo fi in assembly.GetType("System.Windows.Automation.AutomationElement").GetFields())
            {
                if (fi.FieldType == typeof(AutomationProperty))
                {
                    if (fi.Name.EndsWith(IDS_PatternAvailableProperty))
                    {
                        // Convert IsDockPatternAvailableProperty -> Dock
                        string name = fi.Name.Substring("Is".Length, fi.Name.IndexOf(IDS_PatternAvailableProperty) - "Is".Length);

                        //The pattern might be visible through reflection but if it is not registred the value will be null.
                        if ((fi.GetValue(m_le)) != null)
                        {
                            // isSupported = (bool)m_le.GetCurrentPropertyValue(AutomationElement.IsDockPatternAvailableProperty);
                            isSupported = (bool)m_le.GetCurrentPropertyValue((AutomationProperty)fi.GetValue(m_le));

                            // Get the Pattern field scuh as "DockPattern.Pattern"
                            patternId = (AutomationPattern)assembly.GetType("System.Windows.Automation." + name + "Pattern").GetField("Pattern").GetValue(m_le);

                            try
                            {
                                object p = m_le.GetCurrentPattern(patternId);
                                if (!isSupported)
                                    ThrowMe(CheckType.Verification, "GetCurrentPattern() should have thrown an InvalidOperationException");
                            }
                            catch (InvalidOperationException)
                            {
                                if (isSupported)
                                    ThrowMe(CheckType.Verification, "GetCurrentPattern() should not have thrown an InvalidOperationException");
                            }
                        }
                    }
                }
            }
            m_TestStep++;
        }

        #endregion

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("GetCurrentPropertyValue.1",
            TestSummary = "Enumerating through all AutomationProperties using reflection and call GetCurrentPropertyValue().  Verrify that no exception is thrown",
            Priority = TestPriorities.Pri2,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] 
                {
                    "Verify success call to all GetCurrentPropertyValue(AutomationElement property)",
                    "Verify success call to all GetCurrentPropertyValue(AutomationPattern property)",
                })]
        public void TestProperty1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            Assembly assembly = Assembly.GetAssembly(typeof(AutomationElement));
            AutomationProperty property = null; ;

            // ----------------------------------------------------------
            // Do the AutomationProperties off of the AutomationElement
            // ----------------------------------------------------------
            foreach (FieldInfo fi in assembly.GetType("System.Windows.Automation.AutomationElement").GetFields())
            {
                if (fi.FieldType == typeof(AutomationProperty))
                {
                    try
                    {
                        // Get the AutomationProperty using Reflection
                        property = (AutomationProperty)fi.GetValue(m_le);

                        //The pattern might be visible through reflection but if it is not registred the value will be null.
                        if (property != null)
                        {
                            Comment("Calling GetCurrentPropertyValue(" + property.ProgrammaticName + ")");
                            object objValue = m_le.GetCurrentPropertyValue(property);
                        }
                    }
                    catch (Exception exception)
                    {
                        ThrowMe(CheckType.Verification, "Calling GetCurrentPropertyValue(" + Helpers.GetProgrammaticName(property) + " threw and exception(" + exception.InnerException.GetType());
                    }
                }
            }
            m_TestStep++;

            // ----------------------------------------------------------
            // Do the AutomationProperties off of the Pattern objects
            // ----------------------------------------------------------
            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType == typeof(BasePattern))
                {
                    foreach (FieldInfo fi in type.GetFields())
                    {
                        if (fi.FieldType == typeof(AutomationProperty))
                        {
                            try
                            {
                                property = (AutomationProperty)fi.GetValue(m_le);

                                //The pattern might be visible through reflection but if it is not registred the value will be null.
                                if (property != null)
                                {
                                    object objValue = m_le.GetCurrentPropertyValue(property);
                                }
                            }
                            catch (Exception exception)
                            {
                                ThrowMe(CheckType.Verification, "Calling GetCurrentPropertyValue(" + Helpers.GetProgrammaticName(property) + " threw and exception(" + exception.InnerException.GetType());
                            }
                        }
                    }
                }
            }
            m_TestStep++;
        }

        #region Tests

        #region Navigation

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.1",
            TestSummary = "If NextSibling != null, verify NextSiblings.PreviousSibling is current AutomationElement",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Precondition: AutomationElement's NextSibling != null", 
                    "Verify that the NextSibling.PreviousSibling != null", 
                    "Verify that my NextSibling's PreviousSibling is me"
                })]
        public void TestNavigation1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            //"Precondition: AutomationElement's NextSibling != null",
            Comment("Testing AutomationElement(" + Library.GetUISpyLook(m_le) + ")");
            if (ControlNextSibling(m_le) == null)
                ThrowMe(CheckType.IncorrectElementConfiguration);
            Comment("NextSibling != null");
            m_TestStep++;

            //"Verify that the NextSibling.PreviousSibling != null",
            if (TreeWalker.ControlViewWalker.GetNextSibling(m_le) == null)
                ThrowMe(CheckType.Verification);
            Comment("NextSibling.PreviousSibling != null");
            m_TestStep++;
            
            //"Verify that my NextSibling's PreviousSibling is me"
            if (!Automation.Compare(m_le, ControlPreviousSibling(ControlNextSibling(m_le))))
                ThrowMe(CheckType.Verification, "me(" + Library.GetUISpyLook(m_le) + ") != me.NextSibling.PreviousSibling(" + Library.GetUISpyLook(ControlPreviousSibling(ControlNextSibling(m_le))) + ")");
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.2",
            TestSummary = "If GetPreviousSibling != null, verify GetPreviousSibling.NextSibling is current AutomationElement",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Precondition: AutomationElement's PreviousSibling != null", 
                    "Verify that my PreviousSibling's NextSibling is me"
                })]
        public void TestNavigation2(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);
            if (TreeWalker.ControlViewWalker.GetPreviousSibling(m_le) == null)
                ThrowMe(CheckType.IncorrectElementConfiguration);
            m_TestStep++;

            if (!Automation.Compare(m_le, ControlNextSibling(ControlPreviousSibling(m_le))))
                ThrowMe(CheckType.Verification, "me(" + Library.GetUISpyLook(m_le) + ") != me.PreviousSibling.NextSibling(" + ControlNextSibling(ControlPreviousSibling(m_le)) + ")");
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.3",
            TestSummary = "If FirstChild != null, verify GetParent == parents FirstChild",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Precondition: AutomationElement's FirstChild != null", 
                    "Verify that my FirstChild's Parent is me"
                })]
        public void TestNavigation3(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);
            if (ControlFirstChild(m_le) == null)
                ThrowMe(CheckType.IncorrectElementConfiguration);
            m_TestStep++;

            if (!Automation.Compare(m_le, ControlParent(ControlFirstChild(m_le))))
                ThrowMe(CheckType.Verification, "me(" + Library.GetUISpyLook(m_le) + ") != me.FirstChild.Parent(" + ControlNextSibling(ControlPreviousSibling(m_le)) + ")");
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.4",
            TestSummary = "If LastChild != null, verify elements LastChild's parent is element",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Precondition: AutomationElement's LastChild != null", 
                    "Verify that my LastChild's Parent is me"
                })]
        public void TestNavigation4(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);
            if (ControlFirstChild(m_le) == null)
                ThrowMe(CheckType.IncorrectElementConfiguration);
            m_TestStep++;

            if (!Automation.Compare(m_le, TreeWalker.ControlViewWalker.GetParent(ControlLastChild(m_le))))
                ThrowMe(CheckType.Verification, "me(" + Library.GetUISpyLook(m_le) + ") != me.LastChild.Parent(" + ControlParent(ControlLastChild(m_le)) + ")");
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.5",
            TestSummary = "If element's parent != null, verify that element is child of parent element",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify that my Parent has siblings", 
                    "Verify that I am a child of my Parent"
                })]
        public void TestNavigation5(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            bool found = false;
            string me = Library.GetUISpyLook(m_le);
            AutomationElement tempElement = m_le;
            
            // Get the parent
            tempElement = ControlParent(tempElement);
            Comment("Parent: " + Library.GetUISpyLook(tempElement));

            // Get the 1st Child
            tempElement = ControlFirstChild(tempElement);
            Comment("Parent 1st child: " + Library.GetUISpyLook(tempElement));

            //"Verify that my Parent has siblings"
            if (tempElement == null)
                ThrowMe(CheckType.Verification, "Parent has no siblings");

            m_TestStep++;

            // "Verify that I am a child of my Parent"
            do
            {
                //Comment(" Comparing sibling(" + sibling.GetCurrentPropertyValue(AutomationElement.LocalizedControlTypeProperty) + ") with " + me);
                Comment("Comparing sibling(" + Library.GetUISpyLook(tempElement) + ") with me(" + me + ")");
                if (Automation.Compare(tempElement, m_le))
                    found = true;
            } while (found.Equals(false) && (tempElement = ControlNextSibling(tempElement)) != null);

            if (!found)
                ThrowMe(CheckType.Verification, "Could not find me(" + me + ") as a child of (" + ControlParent(m_le).GetCurrentPropertyValue(AutomationElement.LocalizedControlTypeProperty) + ")");

            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.6",
            TestSummary = "Verify elements last sibling is element's parents last child",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Step: Get the last Sibling(me.NextSibling.NextSibling...)", 
                    "Step: Verify that this element is the same as my Parent's LastChild"
                })]
        public void TestNavigation6(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            ArrayList array = new ArrayList();

            //"Step: Get the last Sibling(me.NextSibling.NextSibling...)",
            AutomationElement sibling = m_le;

            while (ControlNextSibling(sibling) != null)
            {
                array.Add(sibling);
                sibling = ControlNextSibling(sibling);
                Comment("Navigating with NextSibling to " + Library.GetUISpyLook(sibling));
                foreach (AutomationElement element in array)
                {
                    if (Automation.Compare(element, sibling))
                    {
                        Comment("Found: " + Library.GetUISpyLook(element) + " - and - " + Library.GetUISpyLook(sibling));
                        ThrowMe(CheckType.Verification, "Found a circular linked list of children");
                    }
                }
            }

            m_TestStep++;

            //"Step: Verify that this element is the same as my Parent's LastChild"
            if (!Automation.Compare(sibling, ControlLastChild(ControlParent(m_le))))
                ThrowMe(CheckType.Verification, "My.LastSibling(" + Library.GetUISpyLook(sibling) + ") != me.Parent.LastChild(" + ControlLastChild(ControlParent(m_le)) + ")");
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.7",
            TestSummary = "Verify elements first sibling is element's parents first child",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Step: Get the AutomationElement's first Sibling(AutomationElement.PreviousSibling.PreviousSibling...)", 
                    "Step: Verify that this element is the same as the AutomationElement's Parent.FirstChild"
                })]
        public void TestNavigation7(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            ArrayList previousSiblings = new ArrayList();

            //"Step: Get the first Sibling(me.PreviousSibling...)",
            AutomationElement sibling = m_le;

            while (ControlPreviousSibling(sibling) != null)
            {
                previousSiblings.Add(sibling);
                sibling = ControlPreviousSibling(sibling);
                foreach (AutomationElement element in previousSiblings)
                    if (Automation.Compare(element, sibling))
                        ThrowMe(CheckType.Verification, "Found a circular linked list of children[" + Library.GetUISpyLook(element) + "]");

                Comment("Navigating with PreviousSibling to " + Library.GetUISpyLook(sibling));
            }
            m_TestStep++;

            //"Step: Verify that this element is the same as my Parent's FirstChild"
            if (!Automation.Compare(sibling, ControlFirstChild(ControlParent(m_le)))) // m_le.Parent.FirstChild))
                ThrowMe(CheckType.Verification, "me.FirstSibling(" + Library.GetUISpyLook(sibling) + ") != me.Parent.FirstChild(" + ControlFirstChild(ControlParent(m_le)) + ")");
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.8",
            TestSummary = "Verify elements oldest ancestor is the AutomationElement.RootElement",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft, Microsoft",
            Description = new string[] {
                    "Step: Get my root element(me.Parent...)", 
                    "Step: Verify that this element is the same AutomationElement.RootElement"
                })]
        public void TestNavigation8(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            //"Step: Get my root element(me.Parent...)",
            AutomationElement parent = m_le;

            while (TreeWalker.ControlViewWalker.GetParent(parent) != null)
            {
                parent = TreeWalker.ControlViewWalker.GetParent(parent);
                Comment("Navigating with Parent to " + Library.GetUISpyLook(parent));
            }
            m_TestStep++;

            //"Step: Verify that this element is the same AutomationElement.RootElement"
            Comment("Comparing '" + Library.GetUISpyLook(parent) + "' and '" + Library.GetUISpyLook(AutomationElement.RootElement) + "'");
            if (!Automation.Compare(parent, AutomationElement.RootElement))
                ThrowMe(CheckType.Verification, "My TopMostParent(" + Library.GetUISpyLook(parent) + ") != AutomationElement.RootElement(" + AutomationElement.RootElement + ")");
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.9",
            TestSummary = "Verify elements first child's previous sibling is null",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify: The the element has children...AutomationElement.FirstChild != null", 
                    "Verify: My FirstChild.PreviousSibling == null"
                })]
        public void TestNavigation9(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            //"Verify: The the element has children...AutomationElement.FirstChild != null", 
            if (ControlFirstChild(m_le) == null)
                ThrowMe(CheckType.IncorrectElementConfiguration);
            m_TestStep++;

            //"Verify: My FirstChild.PreviousSibling == null",
            if (ControlPreviousSibling(ControlFirstChild(m_le)) != null)
                ThrowMe(CheckType.Verification, "FirstChild.PreviousSibling(" + Library.GetUISpyLook(ControlPreviousSibling(ControlFirstChild(m_le))) + ") should have been null");
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.10",
            TestSummary = "Verify element's last child's previous sibling is not null",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify: The the element has children...AutomationElement.FirstChild != null", 
                    "Verify: My LastChild.NextSibling == null"
                })]
        public void TestNavigation10(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            //"Verify: The the element has children...AutomationElement.FirstChild != null", 
            if (ControlLastChild(m_le) == null)
                ThrowMe(CheckType.IncorrectElementConfiguration);
            m_TestStep++;

            //"Verify: My LastChild.NextSibling == null", 
            if (ControlNextSibling(ControlLastChild(m_le)) != null)
                ThrowMe(CheckType.Verification, "LastChild.NextSibling(" + Library.GetUISpyLook(ControlNextSibling(ControlLastChild(m_le))) + ") should have been null");
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.11",
            TestSummary = "Verify element's last childs parent equals elements first childs parent",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify: The the element has children...AutomationElement.FirstChild != null", 
                    "Verify: My AutomationElement.LastChild.Parent == AutomationElement.FirstChild.Parent"
                })]
        public void TestNavigation11(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            //"Verify: The the element has children...AutomationElement.FirstChild != null",
            if (ControlFirstChild(m_le) == null)
                ThrowMe(CheckType.IncorrectElementConfiguration, "FirstChild == null");
            m_TestStep++;

            //"Verify: My LastChild.NextSibling == null", 
            TS_Compare_AutomationElement(ControlParent(ControlLastChild(m_le)), ControlParent(ControlFirstChild(m_le)), CheckType.Verification);
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.ControlLastChild.1",
            TestSummary = "Verify no exceptions are not thrown when calling TreeWalker.ControlViewWalker.GetLastChild",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify success call to LastChild"
                })]
        public void TestLastChild1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            AutomationElement obj = ControlLastChild(m_le);
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.ControlNextSibling.1",
            TestSummary = "Verify no exceptions are not thrown when calling TreeWalker.ControlViewWalker.GetNextSibling",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify success call to NextSibling"
                })]
        public void TestNextSibling1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            AutomationElement obj = ControlNextSibling(m_le);
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.ControlParent.1",
            TestSummary = "Verify no exceptions are not thrown when calling TreeWalker.ControlViewWalker.GetParent",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify success call to Parent"
                })]
        public void TestParent1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            AutomationElement obj = ControlParent(m_le);
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.ControlPreviousSibling.1",
            TestSummary = "Verify no exceptions are not thrown when calling TreeWalker.ControlViewWalker.GetPreviousSibling",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify success call to PreviousSibling"
                })]
        public void TestPreviousSibling1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            AutomationElement obj = ControlPreviousSibling(m_le);
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("Navigation.ControlFirstChild.1",
            TestSummary = "Verify no exceptions are not thrown when calling TreeWalker.ControlViewWalker.GetFirstChild",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify success call to FirstChild"
                })]
        public void TestFirstChild1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            AutomationElement obj = ControlFirstChild(m_le);
            m_TestStep++;
        }

        #endregion Navigation

        #region NameProperty

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("NameProperty.1",
            TestSummary = "Verify that if the NameProperty != AutomationElement.NotSupported, that NameProperty != null or empty string",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Problem,
            ProblemDescription = "Need resolution how relavent this is...there have been issues with Speech on this",
            Author = "Microsoft",
            Description = new string[] {
                    "Verify: AutomationElement.NameProperty != AutomationElement.NotSupported", 
                    "Verify: AutomationElement.NameProperty != null", 
                    "Verify: AutomationElement.NameProperty != \"\""
                })]
        public void TestNameProperty1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            object name = m_le.GetCurrentPropertyValue(AutomationElement.NameProperty);

            if (name.Equals(AutomationElement.NotSupported))
                ThrowMe(CheckType.Verification);
            m_TestStep++;

            if (name.ToString() == null)
                ThrowMe(CheckType.Verification);
            m_TestStep++;

            if (name.ToString().Trim().Length == 0)
                ThrowMe(CheckType.Verification);
            m_TestStep++;
        }
        #endregion NameProperty

        #region BoundingRect

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("BoundingRect.1",
            TestSummary = "Verify that BoundingRectangleProperty returns an acceptable value",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Precondition: AutomationElement.BoundingRectangleProperty != Rect.Empty",
                    "Verify: AutomationElement.BoundingRectangleProperty != AutomationElement.NotSupported", 
                    "Verify: AutomationElement.BoundingRectangleProperty != null", 
                    "Verify: AutomationElement.BoundingRectangleProperty != \"\"",
                    "Verify: AutomationElement.BoundingRectangle Top is less then Bottom",
                    "Verify: AutomationElement.BoundingRectangle left is less then Right",
                    "Verify: AutomationElement.BoundingRectangle Top != Infinity", 
                    "Verify: AutomationElement.BoundingRectangle Left != Infinity",
                    "Verify: AutomationElement.BoundingRectangle Right != Infinity",
                    "Verify: AutomationElement.BoundingRectangle Bottom != Infinity"
                })]
        public void TestBoundingRect2(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            Rect BoundingRect = (Rect)m_le.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

            Comment("GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty) returned {0}", BoundingRect.ToString());

            // Precondition: AutomationElement.BoundingRectangleProperty != Rect.Empty
            if (BoundingRect == Rect.Empty)
            {
                // See Collapsing zero-area Rects to Rect.Empty breaks sparkle UIA testing
                Comment("Empty rectangle, no need to continue test steps/validation");
                m_TestStep = testCaseAttribute.Description.Length;
                return;
            }
            m_TestStep++;

            // Verify: AutomationElement.BoundingRectangleProperty != AutomationElement.NotSupported
            if (BoundingRect.Equals(AutomationElement.NotSupported))
                ThrowMe(CheckType.Verification);

            m_TestStep++;

            // Verify: AutomationElement.BoundingRectangleProperty != null
            if (BoundingRect.ToString(CultureInfo.CurrentUICulture) == null)
                ThrowMe(CheckType.Verification);

            m_TestStep++;

            // Verify: AutomationElement.BoundingRectangleProperty != \"\"
            if (BoundingRect.ToString(CultureInfo.CurrentUICulture).Trim().Length == 0)
                ThrowMe(CheckType.Verification);

            m_TestStep++;

            // Verify: AutomationElement.BoundingRectangle Top is less then Bottom
            Comment("Fail if BoundingRect.Top ({0}) > BoundingRect.Bottom ({1})", BoundingRect.Top, BoundingRect.Bottom);
            if (BoundingRect.Top > BoundingRect.Bottom)
                ThrowMe(CheckType.Verification);

            m_TestStep++;

            // Verify: AutomationElement.BoundingRectangle left is less then Right
            Comment("Fail if BoundingRect.Left ({0}) > BoundingRect.Right ({1})", BoundingRect.Left, BoundingRect.Right);
            if (BoundingRect.Left > BoundingRect.Right)
                ThrowMe(CheckType.Verification);

            m_TestStep++;

            //"Verify: AutomationElement.BoundingRectangle Top != Infinity", 
            if (double.IsInfinity(BoundingRect.Top))
                ThrowMe(CheckType.Verification);

            m_TestStep++;

            //"Verify: AutomationElement.BoundingRectangle Left != Infinity",
            if (double.IsInfinity(BoundingRect.Left))
                ThrowMe(CheckType.Verification);

            m_TestStep++;

            //"Verify: AutomationElement.BoundingRectangle Right != Infinity",
            if (double.IsInfinity(BoundingRect.Right))
                ThrowMe(CheckType.Verification);

            m_TestStep++;

            //"Verify: AutomationElement.BoundingRectangle Bottom != Infinity",
            if (double.IsInfinity(BoundingRect.Bottom))
                ThrowMe(CheckType.Verification);

            m_TestStep++;
        }

        #endregion BoundingRect

        #region FromPoint

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("FromPoint.1",
            TestSummary = "If the BoundingRect returns a viewable area, walk the z-order and determine if there is indeed a point that is returned from FromPoint that is correct as there are times when children can cover the actual element",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Problem,
            ProblemDescription = "Providers do not always calculate this correctly, and is not a high priority to fix",
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: AutomationElement's BoundingRect return a positive (viewable) area", 
                "Step: Create a masked image bitmap of the logical element", 
                "Step: Mask out all points where the logical element's children overlap the logical element", 
                "Step: Mask out all points where the logical element extends past it's parents bounding rectangle", 
                "Step: Save this masked image to c:\test.bmp", 
                "Step: Find a clickable point from this array", 
                "Verify: Call FromPoint();", 
                "Verify: AutomationElement obtained from FromPoint is the AutomationElement we are testing",
            })]
        public void TestFromPoint(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            Drawing.Bitmap availableLocations = null;
            Rect LERc = m_le.Current.BoundingRectangle;
            Point pt = new Point(-1, -1);
            double ParentLeft = LERc.Left;
            double ParentTop = LERc.Top;
            AutomationElement clickLe = null;

            //"Precondition: AutomationElement's BoundingRect return a positive (viewable) area"
            TS_PositiveBoundingRect(m_le, CheckType.IncorrectElementConfiguration);

            // Step: Create a masked image bitmap of the logical element
            TS_InitializeClickablePoints(m_le, ref availableLocations);

            // Step: Mask out all points where the logical element's children overlap the logical element
            TS_MaskOutChildrenRects(m_le, ref availableLocations);

            // Step: Mask out all points where the logical element extends past it's parents bounding rectangle
            TS_MaskOutParentRect(m_le, ref availableLocations);

            // Step: Save this masked image to c:\test.bmp
            TS_SaveBitmap(availableLocations);

            // Step: Find a clickable point from this array
            TS_FindFirstClickablePoint(m_le, ref availableLocations, ref pt);

            // Convert to absolute corrdinates
            pt.X += m_le.Current.BoundingRectangle.X;
            pt.Y += m_le.Current.BoundingRectangle.Y;

            // Verify: Call FromPoint();
            TS_FromPoint(ref clickLe, pt, CheckType.Verification);

            // Verify: AutomationElement obtained from FromPoint is the AutomationElement we are testing
            TS_Compare_AutomationElement(clickLe, m_le, CheckType.Verification);
        }

        #endregion FromPoint

        #region FromHWND

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("NativeWindowHandleProperty.1",
            TestSummary = "Verify that NativeWindowHandleProperty returns the correct handle",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            TestCaseType = TestCaseType.Arguments,
            Author = "Microsoft",
            Description = new string[] {
                    "Verification: That the FromHWND will return the correct AutomationElement"
                })]
        public void TestFromHandle1(TestCaseAttribute testCaseAttribute, object[] arguments)
        {

            if (arguments == null)
                throw new ArgumentException();

            HeaderComment(testCaseAttribute);

            IntPtr hwnd1 = (IntPtr)arguments[0];
            IntPtr hwnd2;

            object objHwnd = m_le.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);

            if (objHwnd is IntPtr)
                hwnd2 = (IntPtr)objHwnd;
            else
                hwnd2 = new IntPtr(Convert.ToInt64(objHwnd, CultureInfo.CurrentCulture));

            if (!hwnd1.Equals(hwnd2))
                ThrowMe(CheckType.Verification, "The IntPtr passed as an argument(" + hwnd1 + ") to the test is not the same IntPtr(" + hwnd2 + ") returned from GetSupportedProperties(ControlElement.NativeWindowHandleProperty)");
            m_TestStep++;
        }

        #endregion FromHWND

        #region FocusedElement

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("FocusedElement.1",
            TestSummary = "Verify that obtaining the AutomationElement.FocusedElement property does not throw any exception",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Step: Verify that one can call AutomationElement.FocusedElement without an exception", 
                })]
        public void TestGetFocus1(TestCaseAttribute testCaseAttribute)
        {

            HeaderComment(testCaseAttribute);

            AutomationElement element = null;

            //"Step: Verify taht one can call GetFocus() while the element has the focus", 
            TS_FocusedElement(ref element, CheckType.Verification);

        }

        #endregion FocusedElement

        #region SetFocus

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("SeatFocusaaa.1",
            TestSummary = "After removing the focus from an element, call SetFocus() programmatically and verify that the element receives the focus and the event is fired",
            Priority = TestPriorities.Pri1,
            BugNumbers = new BugIssues[] { BugIssues.PS28, BugIssues.PS29, BugIssues.PS30, BugIssues.PS35 },
            Status = TestStatus.Works,
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationFocusChangedEventHandler()",
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: Some controls are not persisted after they loose focus (ie, Floating edit in Excel), don't tests these",
                "Precondition: Verify that the element's IsOffScreen = false",
                "Precondition: Verify that this control supports SetFocus tests",
                "Precondition: Verify that the object's IsFocusableProperty == true",
                "Step: Ensure the AutomationElement does not have focus by calling SetFocus(RANDOM)",
                "Step: Verify that the AutomationElement's HasKeyboardFocusProperty == false",
                "Step: Add FocusChangedListener event on the AutomationElement",
                "Step: Call SetFocus(AutomationElement)",
                "Step: Verify that the element or one of it's children have the focus now",
                "Step: Wait for events",
                "Step: Verify that AutomationElement or one of it's children fired the FocusChangeEvent, or if the AutomationElement is a title bar, then the child in the application",
                })]
        public void TestSetFocus1a(TestCaseAttribute testCaseAttribute)
        {
            string EventHandlerVar = "m_FocusChange";

            HeaderComment(testCaseAttribute);

            // "Precondition: Some controls are not persisted after they loose focus (ie, Floating edit in Excel), don't tests these",
            TS_VerifyControlPersistsAfterLoosesFocus(m_le, CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that the element's IsOffScreen = false",
            TSC_VerifyProperty(m_le.Current.IsOffscreen, false, "Current.IsOffscreen", CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that this control supports SetFocus tests",
            TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that the object's IsFocusableProperty == true",
            TSC_VerifyProperty(m_le.Current.IsKeyboardFocusable, true, "AutomationElement.IsKeyboardFocusableProperty", CheckType.IncorrectElementConfiguration);

            //"Step: Ensure the AutomationElement does not have focus by calling SetFocus(AutomationElement.RootElement.FirstChild)",
            TS_RandomSetFocusToOtherControl(m_le, CheckType.Verification);

            //"Step: Verify that the AutomationElement's HasKeyboardFocusProperty == false",
            TSC_VerifyProperty(m_le.Current.HasKeyboardFocus, false, "AutomationElement.HasKeyboardFocusProperty", CheckType.Verification);

            //"Step: Add FocusChangedListener event on the AutomationElement",
            TSC_AddFocusChangedListener(EventHandlerVar, CheckType.Verification);

            //"Step: Call SetFocus(AutomationElement)",
            //if (!TS_FilterOnBug(testCaseAttribute))
            TS_SetFocus(m_le, null, CheckType.Verification);

            //"Step: Verify that the element or one of it's children have the focus now",
            if (!TS_FilterOnBug(testCaseAttribute))
                TS_Focused(m_le, true, CheckType.Verification);

            //"Step: Wait for events",
            if (!TS_FilterOnBug(testCaseAttribute))
                TSC_WaitForEvents(1);

            //"Step: Verify that AutomationElement or child fired the FocusChangeEvent, or if the AutomationElement is a title bar, then the child in the application",
            if (!TS_FilterOnBug(testCaseAttribute))
                TS_VerifyFocusChangeEventHelper(EventHandlerVar);
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        private void TS_Focused(AutomationElement m_le, bool shouldbe, CheckType checkType)
        {
            Comment(InternalHelper.Helpers.GetXmlPathFromAutomationElement(m_le) + " has the focus");
            AutomationElement element = m_le.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.HasKeyboardFocusProperty, true));
            if (shouldbe && (element == null))
                ThrowMe(checkType, "There were no element or it's children that had focus");
            else if (!shouldbe && (element != null))
                ThrowMe(checkType, "There was an element or it's children that had focus");
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("SetFocus.2",
            TestSummary = "If IsKeyboardFocusable == false, call SetFocus() and verify that the IsKeyBoardFocusable property == false",
            Priority = TestPriorities.Pri2,
            Status = TestStatus.Problem,
            ProblemDescription="Need to set focus to another control first! This won't work!!",
            BugNumbers = new BugIssues[] { BugIssues.PS35, BugIssues.PS30 },
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationFocusChangedEventHandler()",
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: Some controls are not persisted after they loose focus (ie, Floating edit in Excel), don't tests these",
                "Precondition: Verify that the element's IsOffScreen = false",
                "Precondition: Verify that this control supports SetFocus tests",
                "Precondition: Verify that the object's IsKeyboardFocusable == true",
                "Step: Add FocusChangedListener event on the AutomationElement",
                "Step: Call and verify that SetFocus(AutomationElement) returns true",
                "Step: Wait for events",
                "Step: Verify that AutomationElement or child fired the FocusChangeEvent, or if the AutomationElement is a title bar, then the child in the application",
                "Precondition: Verify that the object's IsFocusableProperty == true",
            })]
        public void TestSetFocus2(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            // "Precondition: Some controls are not persisted after they loose focus (ie, Floating edit in Excel), don't tests these",
            TS_VerifyControlPersistsAfterLoosesFocus(m_le, CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that the element's IsOffScreen = false";
            TSC_VerifyProperty(m_le.Current.IsOffscreen, false, "Current.IsOffscreen", CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that this control supports SetFocus tests",
            TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that the object's IsFocusableProperty == false",
            TSC_VerifyProperty(m_le.Current.IsKeyboardFocusable, true, "AutomationElement.IsKeyboardFocusableProperty", CheckType.IncorrectElementConfiguration);

            //"Step: Add FocusChangedListener event on the AutomationElement",
            TSC_AddFocusChangedListener("", CheckType.Verification);

            //"Step: Call and verify that SetFocus(AutomationElement) returns true"

            TS_SetFocus(m_le, null, CheckType.Verification);

            //"Step: Wait for events"
            if (!TS_FilterOnBug(testCaseAttribute))
                TSC_WaitForEvents(1);

            //"Step: Verify that AutomationElement or child fired the FocusChangeEvent, or if the AutomationElement is a title bar, then the child in the application",
            if (!TS_FilterOnBug(testCaseAttribute))
                TS_VerifyFocusChangeEventHelper("");

            //"Precondition: Verify that the object's IsFocusableProperty == true",
            if (!TS_FilterOnBug(testCaseAttribute))
                TSC_VerifyProperty(m_le.Current.IsKeyboardFocusable, true, "AutomationElement.IsKeyboardFocusableProperty", CheckType.IncorrectElementConfiguration);
        }

        /// ------------------------------------------------------------------------
        ///<summary></summary>
        /// ------------------------------------------------------------------------
        [TestCaseAttribute("SetFocus.3",
            TestSummary = "If IsKeyboardFocusable == true, verify that when the element does not have the focus and you call SetFocus(), AutomationElement.HasKeyboardFocusProperty == true",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            BugNumbers = new BugIssues[] { BugIssues.PS28, BugIssues.PS29, BugIssues.PS30, BugIssues.PS35 },
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationFocusChangedEventHandler()",
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: Some controls are not persisted after they loose focus (ie, Floating edit in Excel), don't tests these",
                "Precondition: Verify that the element's IsOffScreen = false",
                "Precondition: Verify that this control supports SetFocus tests",
                "Precondition: Verify that the object's IsFocusableProperty == true",
                "Step: Ensure the AutomationElement does not have focus by calling SetFocus(AutomationElement.RootElement.FirstChild)",
                "Step: Verify that the AutomationElement's HasKeyboardFocusProperty == false",
                "Step: Add FocusChangedListener event on the AutomationElement",
                "Step: Call and verify that SetFocus(AutomationElement) returns true",
                "Step: Wait for events",
                "Step: Verify that AutomationElement or child fired the FocusChangeEvent, or if the AutomationElement is a title bar, then the child in the application",
                "Step: Verify that the AutomationElement's HasKeyboardFocusProperty == true",
            })]
        public void TestSetFocus3(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            // "Precondition: Some controls are not persisted after they loose focus (ie, Floating edit in Excel), don't tests these",
            TS_VerifyControlPersistsAfterLoosesFocus(m_le, CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that the element's IsOffScreen = false";
            TSC_VerifyProperty(m_le.Current.IsOffscreen, false, "Current.IsOffscreen", CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that this control supports SetFocus tests",
            TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that the object's IsFocusableProperty == true",
            TSC_VerifyProperty(m_le.Current.IsKeyboardFocusable, true, "AutomationElement.IsKeyboardFocusableProperty", CheckType.IncorrectElementConfiguration);

            //"Step: Ensure the AutomationElement does not have focus by calling SetFocus(AutomationElement.RootElement.FirstChild) - "OS's Start Button"",
            TS_RandomSetFocusToOtherControl(m_le, CheckType.Verification);

            //"Step: Verify that the AutomationElement's HasKeyboardFocusProperty == false",
            TSC_VerifyProperty(m_le.Current.HasKeyboardFocus, false, "AutomationElement.HasKeyboardFocusProperty", CheckType.Verification);

            //"Step: Add FocusChangedListener event on the AutomationElement",
            TSC_AddFocusChangedListener("", CheckType.Verification);

            //"Step: Call and verify that SetFocus(AutomationElement) returns true"
            if (!TS_FilterOnBug(testCaseAttribute))
                TS_SetFocus(m_le, null, CheckType.Verification);

            //"Step: Wait for events"
            if (!TS_FilterOnBug(testCaseAttribute))
                TSC_WaitForEvents(1);

            //"Step: Verify that AutomationElement or child fired the FocusChangeEvent, or if the AutomationElement is a title bar, then the child in the application",
            if (!TS_FilterOnBug(testCaseAttribute))
                TS_VerifyFocusChangeEventHelper("");

            //"Step: Verify that the AutomationElement's HasKeyboardFocusProperty == true",
            if (!TS_FilterOnBug(testCaseAttribute))
                TS_VerifyOnElementOrChild(m_le, AutomationElementIdentifiers.HasKeyboardFocusProperty, true, true, CheckType.Verification);
           
        }


        /// ------------------------------------------------------------------------
        ///<summary></summary>
        /// ------------------------------------------------------------------------
        [TestCaseAttribute("SetFocus.4",
            TestSummary = "Verify that if IsKeyboardFocusableProperty = true, calling SetFocus() should not throw an exception.  If IsKeyboardFocusableProperty = false, then SetFocus() should throw InvalidOperationException",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Precondition: Verify that this control supports SetFocus tests, some controls such as MenuItem's do not support SetFocus()",
                    "If IsKeyboardFocusableProperty == true, SetFocus() should not throw an exception, else it should throw InvalidOperationException"
            })]
        public void TestSetFocus4(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            //"Precondition: Verify that this control supports SetFocus tests",
            TS_VerifySetFocusIsOK(m_le, CheckType.IncorrectElementConfiguration);

            bool canFocus = (bool)m_le.GetCurrentPropertyValue(AutomationElement.IsKeyboardFocusableProperty);
            Type exType = (canFocus == true) ? null : typeof(InvalidOperationException);
            Comment("IsKeyboardFocusableProperty(" + Library.GetUISpyLook(m_le) + ") = " + canFocus);

            // "If IsKeyboardFocusableProperty == true, SetFocus should not throw an exception, else it should throw InvalidOperationException"
            // 

            if (!CheckControlClass(m_le, "FlowDocumentReader"))
            {
                TS_SetFocus(m_le, exType, CheckType.Verification);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_VerifyFocusChangeEventHelper(string EventHandlerVar)
        {
            //"Step: Verify that AutomationElement recieves focus, or if the AutomationElement is a title bar, then the child in the application
            if (m_le.Current.ControlType == ControlType.TitleBar || m_le.Current.ControlType == ControlType.MenuBar)
            {
                AutomationElement app = m_le;
                while (app != null && app.Current.ControlType != ControlType.Window)
                    app = TreeWalker.ControlViewWalker.GetParent(app);

                if (app == null)
                    ThrowMe(CheckType.Verification, "Could not find the application window of the element");

                if (app == AutomationElement.RootElement)
                    ThrowMe(CheckType.Verification, "Could not find the application window");

                TSC_VerifyFocusedChangeEvent(app, EventFired.True, EventHandlerVar, CheckType.Verification);
            }
            else
            {
                TSC_VerifyFocusedChangeEvent(m_le, EventFired.True, EventHandlerVar, CheckType.Verification);
            }
        }
        #endregion SetFocus

        #region GetClickablePoint

        /// ------------------------------------------------------------------------
        ///<summary></summary>
        /// ------------------------------------------------------------------------
        [TestCaseAttribute("ClickablePointProperty.1",
            TestSummary = "Verify that AutomationElement.ClickablePointProperty returned a point withing the BoundingRectangle",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify if element is not off screen, AutomationElement.ClickablePointProperty is within the BoundingRectangle",
                    "Verify the same for the element's last sibling (to cover the case where IsOffscreen is true)"
                })]
        public void TestClickablePointProperty2(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            // check the element itself
            TestClickablePointPropertyHelper(testCaseAttribute, m_le);

            // Earlier tests (e.g. TestSetFocus*) require IsOffscreen == false, so the checks
            // for (IsOffscreen ==> ClickablePoint = NaN) never actually check anything.  To cover
            // the IsOffscreen == true case, do the same checks on the test element's last sibling.
            AutomationElement lastSibling = ControlLastChild(ControlParent(m_le));
            TestClickablePointPropertyHelper(testCaseAttribute, lastSibling);
        }

        private void TestClickablePointPropertyHelper(TestCaseAttribute testCaseAttribute, AutomationElement element)
        {
            m_TestStep++;

            object obj = element.GetCurrentPropertyValue(AutomationElement.ClickablePointProperty, true);

            if (obj == null)
                ThrowMe(CheckType.Verification, "GetCurrentPropertyValue(AutomationElement.ClickablePointProperty) not return null, but should either return a valid number, {NaN,NaN}, or AutomationElement.NotSupported");

            if (obj.Equals(AutomationElement.NotSupported))
            {
                return;
            }

            if (!(obj is Point))
            {
                ThrowMe(CheckType.Verification, "GetCurrentPropertyValue(AutomationElement.ClickablePointProperty) returned an expected type(" + obj.GetType().ToString());
            }

            Point pt = (Point)obj;

            if (element.Current.IsOffscreen)
            {
                // when the element is offscreen, WPF returns (NaN, NaN), but UIA translates this
                // to (2147483648, 2147483648).  The requirement is only that the point be offscreen,
                // but finding the screen size is expensive.  For simplicity, assume an enormous screen
                // size and do the comparisons in a way that works for all variants of the value (the
                // doc for NaN says to use CompareTo rather than comparison operators).
                Rect screenRect = new Rect(0, 0, int.MaxValue / 2, int.MaxValue / 2);
                if (   pt.X.CompareTo(screenRect.Left) >= 0
                    && pt.X.CompareTo(screenRect.Right) < 0
                    && pt.Y.CompareTo(screenRect.Top) >= 0
                    && pt.Y.CompareTo(screenRect.Bottom) < 0)
                {
                    ThrowMe(CheckType.Verification, "Elements whose IsOffscreen property is true should return an offscreen point for ClickablePointProperty");
                }
            }
            else
            { 
                Rect rc = element.Current.BoundingRectangle;

                if (pt.X < rc.Left)
                    ThrowMe(CheckType.Verification, "ClickablePointProperty.X(" + pt.X + ") < BoundingRectangle.Left(" + rc.Left + ")");

                if (pt.X > rc.Right)
                    ThrowMe(CheckType.Verification, "ClickablePointProperty.X(" + pt.X + ") > BoundingRectangle.Right(" + rc.Right + ")");

                if (pt.Y < rc.Top)
                    ThrowMe(CheckType.Verification, "ClickablePointProperty.Y(" + pt.Y + ") < BoundingRectangle.Top(" + rc.Top + ")");

                if (pt.Y > rc.Bottom)
                    ThrowMe(CheckType.Verification, "ClickablePointProperty.Y(" + pt.Y + ") > BoundingRectangle.Bottom(" + rc.Bottom + ")");
            }
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("GetClickablePoint.2",
            TestSummary = "Verify that the point returned from GetClickablePoint() is within the boundaries returned from the element's BoundaryRectangleProperty",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Step: Call GetClickablePoint()", 
                    "Step: If NoClickablePointException is not thrown, verify that pt returned from GetClickablePoint is within the AutomationElement's BoundingRectangle"
                })]
        public void TestGetClickablePoint2(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            Point pnt;
            Rect rc = m_le.Current.BoundingRectangle;

            //"Step: Call GetClickablePoint()",
            try
            {
                pnt = m_le.GetClickablePoint();
            }
            catch (System.Windows.Automation.NoClickablePointException)
            {
                Comment("NoClickablePointException thrown, there is no clickable point");
                m_TestStep = testCaseAttribute.Description.Length;
                return;
            }
            m_TestStep++;

            //"Step: If NoClickablePointException is not thrown, verify that pt returned from GetClickablePoint is within the AutomationElement's BoundingRectangle
            if (pnt.X < rc.Left)
                ThrowMe(CheckType.Verification, "GetClickablePoint.X(" + pnt.X + ") < BoundingRectangle.Left(" + rc.Left + ")");

            if (pnt.X > rc.Right)
                ThrowMe(CheckType.Verification, "GetClickablePoint.X(" + pnt.X + ") > BoundingRectangle.Right(" + rc.Right + ")");

            if (pnt.Y < rc.Top)
                ThrowMe(CheckType.Verification, "GetClickablePoint.Y(" + pnt.Y + ") < BoundingRectangle.Top(" + rc.Top + ")");

            if (pnt.Y > rc.Bottom)
                ThrowMe(CheckType.Verification, "GetClickablePoint.Y(" + pnt.Y + ") > BoundingRectangle.Bottom(" + rc.Bottom + ")");
            m_TestStep++;
        }

        #endregion

        #region TryGetClickablePoint

        /// ------------------------------------------------------------------------
        ///<summary></summary>
        /// ------------------------------------------------------------------------
        [TestCaseAttribute("TryClickablePoint.1",
            TestSummary = "Create a masked image of the clickable area by checking parents and z-order and verify that TryClickablePoint falls within this region",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Problem,  
            ProblemDescription="There are spec issues when this should return a value or not",
            Author = "Microsoft",
            Description = new string[] {
                    "Precondition: AutomationElement's BoundingRect return a positive (viewable) area", 
                    "Precondition: Verify that the element's Current.IsOffScreen property is not false",
                    "Step: Create a masked image bitmap of the logical element", 
                    "Step: Mask out all points where the logical element's children overlap the logical element", 
                    "Step: Mask out all points where the logical element extends past it's parents bounding rectangle", 
                    "Step: Save this masked image to c:\\test.bmp", 
                    "Step: Find a clickable point from this array", 
                    "Verify: Call TryGetClickablePoint() based on what we found from the array and verify correct call to TryGetClickablePoint()"
                })]
        public void TestTryClickablePoint1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            Drawing.Bitmap availableLocations = null;
            Rect LERc = m_le.Current.BoundingRectangle;
            Point pt = new Point(-1, -1);
            Point UIVerifyFoundPr = new Point(-1, -1);
            double ParentLeft = LERc.Left;
            double ParentTop = LERc.Top;

            //"Precondition: AutomationElement's BoundingRect return a positive (viewable) area"
            TS_PositiveBoundingRect(m_le, CheckType.IncorrectElementConfiguration);

            //"Precondition: Verify that the element's Current.IsOffScreen property is not false",
            TSC_VerifyProperty(m_le.Current.IsOffscreen, false, "Current.IsOffscreen", CheckType.IncorrectElementConfiguration);

            // Step: Create a masked image bitmap of the logical element
            TS_InitializeClickablePoints(m_le, ref availableLocations);

            // Step: Mask out all points where the logical element's children overlap the logical element
            TS_MaskOutChildrenRects(m_le, ref availableLocations);

            // Step: Mask out all points where the logical element extends past it's parents bounding rectangle
            TS_MaskOutParentRect(m_le, ref availableLocations);

            // Step: Save this masked image to c:\test.bmp
            TS_SaveBitmap(availableLocations);

            // Step: Find a clickable point from this array
            TS_FindFirstClickablePoint(m_le, ref availableLocations, ref UIVerifyFoundPr);

            Point pnt = new Point(-1, -1);

            if (UIVerifyFoundPr.X.Equals(double.NaN) && UIVerifyFoundPr.Y.Equals(double.NaN))
            {
                // InternalHelper found a cliokable point so Automation should also
                if (m_le.TryGetClickablePoint(out pnt).Equals(true))
                    ThrowMe(CheckType.Verification, "Even through test did not find a clickable point, TryGetClickablePoint() returned true and the point of (" + pnt + ")");
            }
            else
            {
                UIVerifyFoundPr.X += LERc.Left;
                UIVerifyFoundPr.Y += LERc.Top;
                if (m_le.TryGetClickablePoint(out pnt).Equals(false))
                {
                    Comment("InternalHelper calculated  point: AutomationElement.FromPoint(" + UIVerifyFoundPr + ") returned " + Library.GetUISpyLook(AutomationElement.FromPoint(UIVerifyFoundPr)));
                    Comment("TryGetClickablePoint point: AutomationElement.FromPoint(" + pnt + ") returned " + Library.GetUISpyLook(AutomationElement.FromPoint(pnt)));
                    ThrowMe(CheckType.Verification, "TryGetClickablePoint() returned false even through there was a point at " + UIVerifyFoundPr + ".  TryGetClickablePoint returned the point(" + pnt + ")");
                }
            }
            m_TestStep++;
        }
        #endregion TryGetClickablePoint

        #region GetSupportedPatterns

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("GetSupportedPatterns.1",
            TestSummary = "Verify that a call to GetSupportedPatterns succeeds without throwing an exception",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify success call to GetSupportedPatterns()"
                })]
        public void TestGetSupportedPatterns(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            AutomationPattern[] obj = m_le.GetSupportedPatterns();
            m_TestStep++;
        }
        #endregion

        #region GetSupportedProperties

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("GetSupportedProperties.1",
            TestSummary = "Verify that a call to GetSupportedProperties succeeds without throwing an exception",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify success call to GetSupportedProperties()"
                })]
        public void TestGetSupportedProperties(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            AutomationProperty[] obj = m_le.GetSupportedProperties();
            m_TestStep++;
        }

        #endregion

        #region GetHashCode

        /// ------------------------------------------------------------------------
        ///<summary></summary>
        /// ------------------------------------------------------------------------
        [TestCaseAttribute("GetHashCode.1",
            TestSummary = "Verify that a call to GetSupportedProperties succeeds without throwing an exception",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify success call to GetHashCode"
                })]
        public void TestGetHashCode(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            int pnt = m_le.GetHashCode();
            m_TestStep++;
        }

        #endregion

        #region GetType

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("GetType.1",
            TestSummary = "Verify that a call to GetType succeeds without throwing an exception",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify success call to GetType()"
                })]
        public void TestGetType(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            Type obj = m_le.GetType();
            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("ControlTypeProperty.1",
            TestSummary = "Verify that AutomationElement.ControlTypeProperty does not return null or AutomationElement.NotSupported",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                    "Verify that GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) does not return null", 
                    "Verify that GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) does not return AutomationElement.NotSupported"
                })]
        public void TestControlTypeProperty(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            ControlType ct = m_le.Current.ControlType;

            TSC_VerifyProperty(ct, null, false, "ControlType", CheckType.Verification);
            TSC_VerifyProperty(ct, AutomationElement.NotSupported, false, "ControlType", CheckType.Verification);

        }

        #endregion

        #region ToString

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("ToString.1",
            TestSummary = "Verify that a call to ToString() succeeds without throwing an exception",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Verify success call to ToString()"
                })]
        public void TestToString1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);
            try
            {
                string obj = m_le.ToString();
            }
            catch (Exception e)
            {
                ThrowMe(CheckType.Verification, e.Message);
            }
            m_TestStep++;
        }

        #endregion

        #endregion Tests

        #region BVT Specific Tests

        #region Tests with arguments

        /// ------------------------------------------------------------------------
        ///<summary></summary>
        /// ------------------------------------------------------------------------
        [TestCaseAttribute("A.BoundingRectangle.1",
            TestCaseType = TestCaseType.Arguments,
            Priority = TestPriorities.BuildVerificationTest,
            Status = TestStatus.Works, Author = "Microsoft",
            Description = new string[] {
                "Verify: that the BoundingRectangle is as defined by the specificed paramaters"
            })]
        public void TestABoundingRectangle1(TestCaseAttribute testCaseAttribute, object[] argument)
        {
            // NOTE: There is only one step, so don't need to increment the counter
            HeaderComment(testCaseAttribute);

            if (argument == null)
                throw new ArgumentException();

            Rect shouldBeRect = (Rect)argument[0];
            Rect actualRect = m_le.Current.BoundingRectangle;

            Comment("Should be Rect(" + shouldBeRect + ")");
            Comment("Actual Rect(" + actualRect + ")");
            if (!shouldBeRect.Equals(actualRect))
                ThrowMe(CheckType.Verification);
            m_TestStep++;
        }

        /// ------------------------------------------------------------------------
        ///<summary></summary>
        /// ------------------------------------------------------------------------
        [TestCaseAttribute("GetSupportedProperties.ReturnsRequiredProperties",
            TestSummary = "Verify that the properties returned from GetSupportedPropertues returned non null vlaues",
            TestCaseType = TestCaseType.Arguments,
            Priority = TestPriorities.BuildVerificationTest,
            Status = TestStatus.Works, Author = "Microsoft",
            Description = new string[] {
                    "Step: List out the supplied AutomationProperty[] supplied as an argument to the test", 
                    "Verify: Verify that all values supplied in the function parameter are values retured from GetSupportedProperties()", 
                    "Verify: Verify that GetCurrentPropertyValue() returns a valid value, or null.  If AutomationElement.NotSupported or an exception are thrown, report an error",
                })]
        public void TestGetSupportedProperties1(TestCaseAttribute testCaseAttribute, object[] argument)
        {
            HeaderComment(testCaseAttribute);

            if (argument == null)
                throw new ArgumentException();

            object property;
            AutomationProperty[] properties = m_le.GetSupportedProperties();
            AutomationProperty[] arg_properties = new AutomationProperty[argument.Length];

            Array.Copy(argument, arg_properties, argument.Length);
            argument = null;
            TS_ListOutPropertyArray(arg_properties);
            TS_ListOutPropertyArray(properties);

            // Search to make sure we find all our patterns
            foreach (AutomationProperty prop in arg_properties)
            {
                Comment(" Calling GetCurrentPropertyValue(" + prop.ToString() + ")");
                if (Array.IndexOf(properties, prop).Equals(-1))
                    ThrowMe(CheckType.Verification, "Could not find " + prop);

                property = (object)m_le.GetCurrentPropertyValue(prop);
                if (property == null)
                {
                    ThrowMe(CheckType.Verification, "GetCurrentPropertyValue(" + prop.ToString() + ") returned null");
                }
            }
            m_TestStep++;
        }

        #endregion Tests with arguments

        #region SetFocus

        #endregion

        #endregion

        /// -------------------------------------------------------------------
        /// <summary>
        /// Test the call to GetClickablePoint()
        /// </summary>
        /// -------------------------------------------------------------------
        void TS_GetClickablePoint(out Point pt, bool ShouldBeAPoint, CheckType ct)
        {
            pt = new Point();
            if (ShouldBeAPoint)
            {
                try
                {
                    pt = m_le.GetClickablePoint();
                    Comment("GetClickablePoint() did not throw NoClickablePointException as expected");
                }
                catch (System.Windows.Automation.NoClickablePointException)
                {
                    ThrowMe(CheckType.Verification, "GetClickablePoint() threw NoClickablePointException and should not have");
                }
            }
            else
            {
                try
                {
                    pt = m_le.GetClickablePoint();
                    ThrowMe(CheckType.Verification, "GetClickablePoint() did not throw NoClickablePointException and should have");
                }
                catch (System.Windows.Automation.NoClickablePointException)
                {
                    Comment("GetClickablePoint() threw NoClickablePointException as expected");
                }
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Test that a password edit box doesn't return a password when calling ValuePattern.Value
        /// </summary>
        /// -------------------------------------------------------------------
        void TS_PasswordText(AutomationElement element, CheckType checktype)
        {
            // ValuePattern.Value should not return the password.
            // This is enforced differently, depending on version and compat switches.

            try
            {
                ValuePattern vp;
                string buffer;

                if (m_useCurrent)
                {
                    vp = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                    buffer = vp.Current.Value;
                }
                else
                {
                    vp = element.GetCachedPattern(ValuePattern.Pattern) as ValuePattern;
                    buffer = vp.Cached.Value;
                }

                // In .NET 4.7.1+, Value returns empty string
                if (String.Empty != buffer)
                {
                    ThrowMe(checktype, "ValuePattern.Value returned a non-empty string");
                }
            }
            catch (InvalidOperationException)
            {
                // In .NET 4.7 (and compatible), InvalidOperationException is thrown
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_ListOutPropertyArray(AutomationProperty[] properties)
        {
            // List out the ones it found
            Comment("GetSupportedProperties returned: ");
            foreach (AutomationProperty prop in properties)
            {
                Comment("    " + prop.ToString());
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        void TS_SaveBitmap(Drawing.Bitmap bm)
        {
            string path = System.IO.Directory.GetCurrentDirectory() + @"\AutomationElementTest.bmp";
            Comment("Saving image to " + path);
            bm.Save(path);
            TestObject.PSFileList.Add(path);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Remove any point where the logicla element falls outside of
        /// the boundaries of the parent containing control.
        /// </summary>
        /// -------------------------------------------------------------------
        void TS_MaskOutParentRect(AutomationElement element, ref Drawing.Bitmap bm)
        {
            Rect leRect = element.Current.BoundingRectangle;
            Rect parentRect = ControlParent(element).Current.BoundingRectangle;

            // Cut it at the edge of the screen
            parentRect = new Rect(parentRect.Left < 0 ? 0 : parentRect.Left, parentRect.Top < 0 ? 0 : parentRect.Top, parentRect.Right - parentRect.Left, parentRect.Bottom - parentRect.Top);
            leRect = new Rect(leRect.Left < 0 ? 0 : leRect.Left, leRect.Top < 0 ? 0 : leRect.Top, leRect.Right - leRect.Left, leRect.Bottom - leRect.Top);

            // Offset it since the origin of the bitmap is 0,0
            parentRect.Offset(leRect.X, leRect.Y);

            Rect outsideRect;
            Drawing.Brush brush = new Drawing.SolidBrush(_maskColor);
            Drawing.Graphics g = Drawing.Graphics.FromImage(bm);

            // Is the element wholy within the parent, just return...most cases
            if (parentRect.Contains(leRect))
            {
                Comment("Element is not cliped by the parent");
                m_TestStep++;
                return;
            }

            // 

            outsideRect = new Rect(leRect.Left, leRect.Top, leRect.Right, parentRect.Top);
            g.FillRectangle(brush, new Drawing.RectangleF((float)outsideRect.X, (float)outsideRect.Y, (float)outsideRect.Width, (float)outsideRect.Height));

            // Extends out the right
            outsideRect = new Rect(parentRect.Right, leRect.Top, leRect.Bottom, leRect.Right);
            g.FillRectangle(brush, new Drawing.RectangleF((float)outsideRect.X, (float)outsideRect.Y, (float)outsideRect.Width, (float)outsideRect.Height));

            // Extends out the left
            outsideRect = new Rect(leRect.Left, leRect.Top, parentRect.Left, leRect.Bottom);
            g.FillRectangle(brush, new Drawing.RectangleF((float)outsideRect.X, (float)outsideRect.Y, (float)outsideRect.Width, (float)outsideRect.Height));

            // Extends out the bottom
            outsideRect = new Rect(leRect.Left, parentRect.Bottom, leRect.Right, leRect.Bottom);
            g.FillRectangle(brush, new Drawing.RectangleF((float)outsideRect.X, (float)outsideRect.Y, (float)outsideRect.Width, (float)outsideRect.Height));

            brush.Dispose();

            Comment("Element is cliped by the parent and it's rectangle has been removed from the element's displayed rectangle");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Mask out any child rects that might be within the logical elements container.  
        /// An example of this is the list box and how the list items completely 
        /// fill in the listbox container window.  On return, pixels that are covered
        /// by a child, are set to m_maskColor.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bm">Bitmap rectagle that represents the bouding rectangle of element</param>
        /// -------------------------------------------------------------------
        void MaskOutChildrenRectsRecursion(AutomationElement element, ref Drawing.Bitmap bm)
        {
            if (element == null)
                return;

            AutomationElement child = ControlFirstChild(element);

            if (child != null)
            {
                Drawing.Brush brush = new Drawing.SolidBrush(_maskColor);
                Drawing.Graphics g = Drawing.Graphics.FromImage(bm);

                do
                {
                    // Bitmap starts at 0,0, so get child relative cordinates to parent's
                    Rect absRect = child.Current.BoundingRectangle;

                    // Only do this if the child's element is not empty
                    if (!absRect.Equals(Rect.Empty))
                    {
                        Rect relRect = absRect;

                        relRect.Offset(-_parentRect.Left, -_parentRect.Top);

                        string c = Library.GetUISpyLook(child);

                        Comment("Found child" + c + "  at location relative rect(" + relRect + ") which is absolute rect(" + absRect + ")");

                        // mask out the child from the parent's rectangle
                        g.FillRectangle(brush, new Drawing.RectangleF((float)relRect.X, (float)relRect.Y, (float)relRect.Width, (float)relRect.Height));
                        MaskOutChildrenRectsRecursion(child, ref bm);
                    }

                    child = ControlNextSibling(child);
                } while (child != null);

                brush.Dispose();
            }
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        void TS_MaskOutChildrenRects(AutomationElement element, ref Drawing.Bitmap bm)
        {
            Comment("Masking out any child elements");
            _parentRect = element.Current.BoundingRectangle;
            MaskOutChildrenRectsRecursion(element, ref bm);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// After you get the array and mask everything out, look through the 
        /// array for any points that are clickable.
        /// </summary>
        /// -------------------------------------------------------------------
        void TS_FindFirstClickablePoint(AutomationElement element, ref Drawing.Bitmap bm, ref Point pt)
        {
            pt = new Point(double.NaN, double.NaN);
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    if (!bm.GetPixel(x, y).Equals(_maskColor))
                    {
                        pt.X = x;
                        pt.Y = y;

                        Point ptOffset = pt;

                        ptOffset.Offset(element.Current.BoundingRectangle.X, element.Current.BoundingRectangle.Y);
                        Comment("Found clickable point at relative point(" + pt + ") which is absolute point(" + ptOffset + ")");
                        m_TestStep++;
                        return;
                    }
                }
            }
            Comment("Could not find clickable point");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Setup an array of locations that map to possible clickable
        /// locations.  Call TS_MaskOutChildrenRects and TS_MaskOutParentRect
        /// to find true locations that are clickable
        /// </summary>
        /// -------------------------------------------------------------------
        void TS_InitializeClickablePoints(AutomationElement element, ref Drawing.Bitmap bm)
        {
            int x = (int)element.Current.BoundingRectangle.Width;
            int y = (int)element.Current.BoundingRectangle.Height;

            bm = new Drawing.Bitmap(x, y);

            // Fill the area with red
            Drawing.Graphics g = System.Drawing.Graphics.FromImage(bm);
            Drawing.Brush brush = new Drawing.SolidBrush(Drawing.Color.FromArgb(255, 0, 0));

            g.FillRectangle(brush, 0, 0, x, y);
            Comment("Bitmap has been created in memory");

            brush.Dispose();

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        void DisplayRect(string text, Rect rc)
        {
            Comment(text + " == (" + "Left:" + rc.Left + ", " + "Right:" + rc.Right + ", " + "Top:" + rc.Top + ", " + "Bottom:" + rc.Bottom + ")");
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        void TS_PositiveBoundingRect(AutomationElement element, CheckType ct)
        {
            Rect LERect = element.Current.BoundingRectangle;

            DisplayRect("AutomationElement's BoundingRect", LERect);
            if (LERect.Width.Equals(Double.NegativeInfinity))
                ThrowMe(CheckType.IncorrectElementConfiguration, "Logical Element's Width property == NegativeInfinity");

            if (LERect.Height.Equals(Double.NegativeInfinity))
                ThrowMe(CheckType.IncorrectElementConfiguration, "Logical Element's Hieght property == NegativeInfinity");

            if (LERect.Width.Equals(0.0))
                ThrowMe(CheckType.IncorrectElementConfiguration, "Logical Element's Width property == 0");

            if (LERect.Height.Equals(0.0))
                ThrowMe(CheckType.IncorrectElementConfiguration, "Logical Element's Hieght property == 0");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        private void TS_VerifyElementOrChildHasFocus(AutomationElement element, CheckType checkType)
        {
            AutomationElement focusedElement = AutomationElement.FocusedElement;
            Comment("Element with the focus: " + Library.GetUISpyLook(focusedElement));

            while (!Automation.Compare(focusedElement, AutomationElement.RootElement) && !Automation.Compare(focusedElement, element))
                focusedElement = TreeWalker.ControlViewWalker.GetParent(focusedElement);

            if (!Automation.Compare(focusedElement, element))
                ThrowMe(checkType, "Element or one of it's children did not have the focus");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Get the window rect using the Win32 API GetWindowRect.  If there is
        /// a buddy control (ie. Spinner), then take the union of the element's 
        /// bounding rectangle, and the buddy controls bounding rectangle.
        /// </summary>
        /// -------------------------------------------------------------------
        private void TS_GetWin32GetWindowRect(AutomationElement element, out Rect rectObject, CheckType checktype)
        {
            NativeMethods.RECT rcObject = new NativeMethods.RECT();
            NativeMethods.RECT rcBuddy = new NativeMethods.RECT();
            
            Rect rectBuddy = new Rect();
            IntPtr intPtrElement = Helpers.CastNativeWindowHandleToIntPtr(element);

            if (intPtrElement == IntPtr.Zero)
                ThrowMe(checktype, "Could not get a window handle to the element");

            // Get the AutomationElement Win32 bounding rectangle
            SafeNativeMethods.GetWindowRect(intPtrElement, ref rcObject);
            rectObject = new Rect(rcObject.left, rcObject.top, rcObject.right - rcObject.left, rcObject.bottom - rcObject.top);

            // If this has a buddy control, get it and it's bounding rectangle
            IntPtr hwndBuddy = Helpers.SendMessage(intPtrElement, NativeMethods.UDM_GETBUDDY, IntPtr.Zero, IntPtr.Zero);
            if (hwndBuddy != IntPtr.Zero)
            {
                SafeNativeMethods.GetWindowRect(hwndBuddy, ref rcBuddy);
                rectBuddy = new Rect(rcBuddy.left, rcBuddy.top, rcBuddy.right - rcBuddy.left, rcBuddy.bottom - rcBuddy.top);
                rectObject = Rect.Union(rectBuddy, rectObject);
            }

            m_TestStep++;
        }


        /// -------------------------------------------------------------------
        /// <summary>
        /// Get the window rect using the IAccessible::get_accLocation.  If there is
        /// a buddy control (ie. Spinner), then take the union of the element's 
        /// bounding rectangle, and the buddy controls bounding rectangle.
        /// </summary>
        /// -------------------------------------------------------------------
        private void TS_GetaccLocation(AutomationElement element, out Rect rcObject, CheckType checktype)
        {
            IAccessible accObject = null;
            IAccessible accBuddy = null;
            Rect rcBuddy = Rect.Empty;
            int accleft, acctop, accwidth, accheight;
            int bdyleft, bdytop, bdywidth, bdyheight;
            object accobjChild = null;
            IntPtr intPtrElement = Helpers.CastNativeWindowHandleToIntPtr(element);

            if (intPtrElement == IntPtr.Zero)
                ThrowMe(checktype, "Could not get a window handle to the element");

            // Get the AutomationElement IAccessible 
            int retValue = Helpers.GetIAccessibleFromWindow(intPtrElement, 0, ref accObject);
            if (accObject == null || retValue != NativeMethods.S_OK)
                ThrowMe(CheckType.Verification, "Could not get the IAccessible for the element");

            // Get the AutomationElement IAccessible location and convert to Rect so we can use Rect.Union
            accObject.accLocation(out accleft, out acctop, out accwidth, out accheight, accobjChild);
            rcObject = new Rect(accleft, acctop, accwidth, accheight);

            // If this has a buddy control, get it and it's bounding rectangle and take the union of the buddy
            // rectangle and elements rectangle
            IntPtr hwndBuddy = Helpers.SendMessage(intPtrElement, NativeMethods.UDM_GETBUDDY, IntPtr.Zero, IntPtr.Zero);
            if (hwndBuddy != IntPtr.Zero)
            {
                retValue = Helpers.GetIAccessibleFromWindow(hwndBuddy, 0, ref accBuddy);
                if (accBuddy == null || retValue == NativeMethods.S_FALSE)
                {
                    ThrowMe(CheckType.Verification, "Element has a buddy control, but could not get the IAccessible for the buddy to calculate the rectangle");
                }

                // Get the location of the buddy control and create a union of the two locations
                accBuddy.accLocation(out bdyleft, out bdytop, out bdywidth, out bdyheight, accobjChild);
                rcBuddy = new Rect(bdyleft, bdytop, bdywidth, bdyheight);
                rcObject = Rect.Union(rcBuddy, rcObject);
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Determines if rectangle == AutomationElement.BoundingRect
        /// </summary>
        /// -------------------------------------------------------------------
        private void TS_VerifyBoundingRect(AutomationElement element, Rect otherRc, CheckType checkType)
        {
            // Get the UIA reported window rect
            Rect rcUIA = element.Current.BoundingRectangle;

            Comment("UIA BoundingRectangle returned (Left={0}, Top={1}, Right={2}, Bottom={3}", rcUIA.Left, rcUIA.Top, rcUIA.Right, rcUIA.Bottom);
            Comment("Other rectangle was (Left={0}, Top={1}, Right={2}, Bottom={3}", otherRc.Left, otherRc.Top, otherRc.Right, otherRc.Bottom);

            if (otherRc != rcUIA)
                ThrowMe(checkType);

            m_TestStep++;
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        private void TS_VerifyControlPersistsAfterLoosesFocus(AutomationElement m_le, CheckType checkType)
        {
            ControlType ct = m_le.Current.ControlType;

            if (ct == ControlType.Edit)
                if (TreeWalker.ControlViewWalker.GetParent(m_le).Current.ControlType == ControlType.DataItem)
                    ThrowMe(checkType, "Edit's in DataItems disappear when they loose focus");

            m_TestStep++;
        }

        /// <summary>
        /// This is a checker to verify whether or not a control's class is a given class name
        /// </summary>
        /// <param name="element">element to check</param>
        /// <param name="controlClass">given class name</param>
        /// <returns>true if the given class, else false</returns> 
        internal bool CheckControlClass(AutomationElement element, string controlClass)
        {
            string className = (string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
            return className == controlClass;
        }
    }
}
