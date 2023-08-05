// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
* Purpose: InternalHelper
* Owner: Microsoft
* Contributors:
*******************************************************************/
using System;
using System.Globalization;
using System.Windows.Automation;
using System.Windows;
using System.Collections;
using System.Windows.Forms;
using Drawing = System.Drawing;
using System.Threading;
using MS.Win32;

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
    public class TransformPatternWrapper : PatternObject
    {
        #region Enums
        internal enum TrueFalseValues
        {
            True,
            False,
            Undetermined
        }
        internal enum WindowPosition
        {
            Exact,
            Within
        }
        internal enum HasFocus
        {
            Yes,
            No,
            DontCare
        }
        #endregion Enums

        #region Variables
        internal TransformPattern m_pattern = null;

// 

        #endregion

        #region constructor

        /// <summary></summary>
        protected TransformPatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            m_pattern = (TransformPattern)GetPattern(m_le, m_useCurrent, TransformPattern.Pattern);
        }

        #endregion

        #region Methods

        internal bool pCanMove
        {
            get
            {
                return m_pattern.Current.CanMove;
            }
        }

        internal bool pCanResize
        {
            get
            {
                return m_pattern.Current.CanResize;
            }
        }

        internal bool pCanRotate
        {
            get
            {
                return m_pattern.Current.CanRotate;
            }
        }

        internal void pattern_Rotate(double degrees, Type expectedException, CheckType checkType)
        {
            string call = "TransformPattern.Rotate(" + degrees + ")";
            
            Comment("Calling " + call);
            
            try
            {
                m_pattern.Rotate(degrees);
            }
            catch (Exception error)
            {
                if (expectedException != error.GetType())
                    ThrowMe(checkType, call + " threw an expection unexpectedly : " + error.Message);
                Comment("Successfully called " + call + " with exception thrown as expected");
                return;
            }

            if (expectedException != null)
                ThrowMe(checkType, call + " did not throw an expection(" + expectedException.GetType() + " as expected");

            Comment("Successfully called " + call + " without an exception thrown");
        }

        internal void pattern_Move(double x, double y, Type expectedException, CheckType checkType)
        {
            string call = "TransformPattern.Move(" + x + ", " + y + ")";

            Comment("Calling " + call);

            try
            {
                m_pattern.Move(x, y);
            }
            catch (Exception error)
            {
                if (expectedException != error.GetType())
                    ThrowMe(checkType, call + " threw an expection unexpectedly : " + error.Message);
                Comment("Successfully called " + call + " with exception thrown as expected");
                return;
            }

            if (expectedException != null)
                ThrowMe(checkType, call + " did not throw an expection(" + expectedException.GetType() + " as expected");

            Comment("Successfully called " + call + " without an exception thrown");

        }

        internal void pattern_Resize(double width, double height, Type expectedException, CheckType checkType)
        {
            string call = "TransformPattern.Resize(" + width + ", " + height + ")";

            Comment("Calling " + call);

            try
            {
                m_pattern.Resize(width, height);
            }
            catch (Exception error)
            {
                if (expectedException != error.GetType())
                    ThrowMe(checkType, call + " threw an expection unexpectedly : " + error.Message);
                Comment("Successfully called " + call + " with exception thrown as expected");
                return;
            }

            if (expectedException != null)
                ThrowMe(checkType, call + " did not throw an expection(" + expectedException.GetType() + " as expected");

            Comment("Successfully called " + call + " without an exception thrown");
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

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
	public sealed class TransformTests : TransformPatternWrapper
    {
        #region Member variables

        /// <summary>
        /// Specific interface associated with this pattern
        /// </summary>
        new TransformPattern m_pattern = null;


        #endregion Member variables
        const string THIS = "TransformTests";

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public const string TestSuite = NAMESPACE + "." + THIS;

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public static readonly string TestWhichPattern = Automation.PatternName(TransformPattern.Pattern);

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public TransformTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.Transform, dirResults, testEvents, commands)
        {
            m_pattern = (TransformPattern)element.GetCurrentPattern(TransformPattern.Pattern);
            if (m_pattern == null)
                throw new Exception(Helpers.PatternNotSupported);
        }

        #region Rotate 1.X

        ///<summary>TestCase: CanRotate.1.6</summary>
        [TestCaseAttribute("CanRotate.1.6",
            TestSummary = "If an element can rotate, call Transform.Rotate and verify that the BoundingRect property change event was fired",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: TransformPattern.CanRotate == true", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Rotate with the random degree is called successfully", 
                "Verify: The BoundingRectangle PropertyChange event is fired", 
                "Step: Move the element back to it's orginal location",
        })]
        public void CanRotate16(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            Rect oldRect = m_le.Current.BoundingRectangle;

			int degree = (int)Helpers.RandomValue((int)0, (int)359);

            try
            {
                //"Precondition: TransformPattern.CanRotate == true", 
                TSC_VerifyProperty(pCanRotate, true, "TransformPattern.CanRotate", CheckType.IncorrectElementConfiguration);

                //"Step: Setup a BoundingRectangle PropertyChange event", 
                TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);

                //"Step: Verify that Rotate with the random degree is called successfully", 
                TS_Rotate(degree, null, CheckType.Verification);

                //"Verify: The BoundingRectangle PropertyChange event is fired",
                TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);
            }
            finally // incase we throw, we can come back in here and clean up
            {
                //"Step: Move the element back to it's orginal location",
                if (m_pattern.Current.CanRotate == true)
                    TS_Rotate(-degree, null, CheckType.Verification);
            }
        }

        #endregion Rotate

        #region Move 2.X

        ///<summary>TestCase: Move.2.1</summary>
        [TestCaseAttribute("Move.2.1",
            TestSummary = "Move the element to position (0,0) which is not the current location",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: TransformPattern.CanMove == true",
                "Precondition: If the element is at position (0,0), then move it to (10,10)",
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move(0,0) is called successfully", 
                "Step: Wait for event to occur",
                "Verify: The BoundingRectangle PropertyChange event is fired",
                "Verify: Element's BoundingRectangle is correct",
                "Step: Move the window back to it's original position",
        })]
        public void Move21(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            Point oldPoint = new Point(m_le.Current.BoundingRectangle.Left, m_le.Current.BoundingRectangle.Top);

            try
            {
                // 
                if (!CheckControlClass(m_le, "GridSplitter"))
                {
                    //"Precondition: TransformPattern.CanMove == true", 
                    TSC_VerifyProperty(pCanMove, true, TransformPattern.CanMoveProperty, CheckType.IncorrectElementConfiguration);

                    //"Step: Move the element to some position other than zero. If position is (0,0) move to (10,10)", 
                    if (oldPoint.X == 0 & oldPoint.Y == 0)
                        TS_Move(new Point(10, 10), null, CheckType.Verification);
                    else
                    {
                        Comment("Element is not at position (0,0)");
                        m_TestStep++;
                    }

                    //"Step: Setup a BoundingRectangle PropertyChange event", 
                    TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);

                    //"Step: Verify that Move(0,0) is called successfully", 
                    TS_Move(new Point(0, 0), null, CheckType.Verification);

                    // Step: Wait for event to occur
                    TSC_WaitForEvents(2);

                    //"Verify: The BoundingRectangle PropertyChange event is fired",
                    TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);
                    
                    // Verify BoundingRect is correct
                    TS_VerifyBoundingRect(new Rect(0, 0, m_le.Current.BoundingRectangle.Width, m_le.Current.BoundingRectangle.Height), m_le.Current.BoundingRectangle, CheckType.Verification);
                }
            }
            finally // incase we throw expection, come back and clean up
            {
                if (oldPoint.X != m_le.Current.BoundingRectangle.Left || oldPoint.Y != m_le.Current.BoundingRectangle.Top)
                {
                    //"Step: Move the window back to it's original position",
                    Comment("Moving element back to it's original position");
                    TS_Move(oldPoint, null, CheckType.Verification);
                }
                else
                {
                    m_TestStep++;
                }
            }
        }

        ///<summary>TestCase: Move.2.2</summary>
        [TestCaseAttribute("Move.2.2",
            TestSummary = "Move the element to a random location on the desktop",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Step: Obtain a random point on the desktop", 
                "Precondition: TransformPattern.CanMove == true", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move() with the random point is called successfully",
                "Step: Wait for event to get fired",
                "Verify: The BoundingRectangle PropertyChange event is fired", 
                "Verify: Element was moved to the correct position by validating the BoundingRectangle", 
                "Step: Move the window back to it's original position",
        })]
        public void Move22(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            Point point = new Point();

            //"Step: Obtain a random point on the desktop", 
            TS_GetRandomOtherScreenLocation(out point, true, CheckType.Verification);
            TestMove1(true, CheckType.IncorrectElementConfiguration, null, WindowPosition.Exact, EventFired.True, point, true, HasFocus.DontCare);
        }

        ///<summary>TestCase: Move.2.3</summary>
        [TestCaseAttribute("Move.2.3",
            TestSummary = "Move the element to it's current location",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: TransformPattern.CanMove == true", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move() with the current location is called successfully",
                "Step: Wait for event to get fired",
                "Verify: The BoundingRectangle PropertyChange event is not fired", 
                "Verify: Element was still at the current location", 
        })]
        public void Move23(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            TestMove1(true, CheckType.IncorrectElementConfiguration, null, WindowPosition.Exact, EventFired.False, new Point(m_le.Current.BoundingRectangle.X, m_le.Current.BoundingRectangle.Y), false, HasFocus.DontCare);
        }

        ///<summary>TestCase: Move.2.4</summary>
        [TestCaseAttribute("Move.2.4",
            TestSummary = "Move to point where the element results in partially outside of screen and partially inside screen",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: Obtain a position on the screen where the element will be partially off the parent's viewport",
                "Precondition: TransformPattern.CanMove == true", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move() with the the current locations is called successfully", 
                "Step: Wait for event to get fired",
                "Verify: The BoundingRectangle PropertyChange event is fired", 
                "Verify: Element has moved to the desired location",
                "Step: Move the element back to it's original position"
        })]
        public void Move24(TestCaseAttribute testCase)
        {
            




            //This test only applies to Window but Transform pattern can conditionally apply to other controls as well.
            if (CheckControlClass(m_le, "Window"))
            {
                HeaderComment(testCase);
                Rect rect = new Rect();

                TS_GetRandomOverlappingParentViewport(out rect, CheckType.IncorrectElementConfiguration);
                TestMove1(true, CheckType.IncorrectElementConfiguration, null, WindowPosition.Exact, EventFired.True, new Point(rect.X, rect.Y), true, HasFocus.DontCare);
            }
        }

        ///<summary>TestCase: Move.2.5</summary>
        [TestCaseAttribute("Move.2.5",
            TestSummary = "Move to point where the element is entirely outside of screen",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: Obtain a random position where the element intirely off the screen", 
                "Precondition: TransformPattern.CanMove == true", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move() with the random point is called successfully",
                "Step: Wait for event to get fired",
                "Verify: The BoundingRectangle PropertyChange event is fired if the element is moved", 
                "Verify: Element is within the screen cordinates", 
                "Step: Move the element back to it's original position"
        })]
        public void Move25(TestCaseAttribute testCase)
        {
            //This test only applies to Window but Transform pattern can conditionally apply to other controls as well.
            if (CheckControlClass(m_le, "Window"))
            {
                HeaderComment(testCase);

                Rect rect = new Rect();

                TS_GetRandomEntirelyOutsideParentViewport(out rect, CheckType.IncorrectElementConfiguration);
                TestMove1(true, CheckType.IncorrectElementConfiguration, null, WindowPosition.Within, EventFired.False, new Point(rect.X, rect.Y), true, HasFocus.DontCare);
            }
        }

        ///<summary>TestCase: Move.2.8</summary>
        [TestCaseAttribute("Move.2.8",
            TestSummary = "Non-movable element, call Move and verify that it does not move",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: Obtain a random position on the desktop", 
                "Precondition: TransformPattern.CanMove == false", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move() with the random point throws InvalidOperationException",
                "Step: Wait for event to get fired",
                "Verify: The BoundingRectangle PropertyChange event is not fired", 
                "Verify: Element has not moved", 
        })]
        public void Move28(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            Point point = new Point();

            TS_GetRandomOtherScreenLocation(out point, true, CheckType.IncorrectElementConfiguration);
            TestMove1(false, CheckType.IncorrectElementConfiguration, typeof(InvalidOperationException), WindowPosition.Exact, EventFired.False, point, false, HasFocus.DontCare);
        }

        ///<summary>TestCase: Move.2.12</summary>
        [TestCaseAttribute("Move.2.12",
            TestSummary = "Element is maximized, but does not cover entire desktop (Console window)",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: Obtain a random position on the desktop", 
                "Precondition: Call WindowPattern.SetWindowVisualState(WindowVisualState.Maximize)", 
                "Verify:  WindowVisualState = WindowVisualState.Maximize", 
                "Precondition: Element does not cover enitire current screen", 
                "Precondition: TransformPattern.CanMove == true", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move() with the random point throws InvalidOperationException",
                "Step: Wait for event to get fired",
                "Verify: The BoundingRectangle PropertyChange event is not fired", 
                "Verify: Element has not moved",
        })]
        public void Move212(TestCaseAttribute testCase)
        {
            //This test only applies to Window but Transform pattern can conditionally apply to other controls as well.
            if (CheckControlClass(m_le, "Window"))
            {
                HeaderComment(testCase);

                Point point = new Point();

                TS_SetWindowVisualState(m_le, WindowVisualState.Maximized, CheckType.IncorrectElementConfiguration);
                TS_VerifyWindowVisualState(m_le, WindowVisualState.Maximized, CheckType.IncorrectElementConfiguration);
                TS_WindowNotCoveredEntireScreen(CheckType.IncorrectElementConfiguration);
                TestMove1(true, CheckType.Verification, typeof(InvalidOperationException), WindowPosition.Exact, EventFired.False, point, false, HasFocus.DontCare);
            }
        }

        ///<summary>TestCase: Move.2.13</summary>
        [TestCaseAttribute("Move.2.13",
            TestSummary = "Element does not have the focus",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Step: Obtain a random location on the screen other than current location",
                "Step: Ensure the AutomationElement does not have focus by calling SetFocus(AutomationElement.RootElement.FirstChild) - OS's Start Button",
                "Precondition: TransformPattern.CanMove == true", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move() with the random point is called successfully",
                "Step: Wait for event to get fired",
                "Verify: The BoundingRectangle PropertyChange event is fired", 
                "Verify: Element has moved to the new location", 
                "Step: Move the element back to it's original position"
        })]
        public void Move213(TestCaseAttribute testCase)
        {
            
            if (!CheckControlClass(m_le, "GridSplitter"))
            {
                HeaderComment(testCase);

                Point point = new Point();

                //"Step: Obtain a random point on the desktop", 
                TS_GetRandomOtherScreenLocation(out point, true, CheckType.Verification);
                TestMove1(true, CheckType.IncorrectElementConfiguration, null, WindowPosition.Exact, EventFired.True, point, true, HasFocus.No);
            }
        }

        ///<summary>TestCase: Move.2.17</summary>
        [TestCaseAttribute("Move.2.17",
            TestSummary = "Move a minimized non-movable element(ie. window minimized to task bar)",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri1, Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Step: Obtain a random location on the screen other than current location", 
                "Step: Miminimize the element",
                /* TestMove1 */
                "Verify: TransformPattern.CanMove == false", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move() with the random point is called successfully",
                "Step: Wait for event to get fired",
                "Verify: The BoundingRectangle PropertyChange event is not fired", 
                "Verify: Element has not moved to the new location", 
                "Step: Move the element back to it's original position if previous steps failed",
                "Step: Set the element's WindowVisualState back to it's original state"
        })]
        public void Move217(TestCaseAttribute testCase)
        {
            //This test only applies to Window but Transform pattern can conditionally apply to other controls as well.
            if (CheckControlClass(m_le, "Window"))
            {
                HeaderComment(testCase);

                Point point = new Point();

                WindowVisualState originalState = ((WindowPattern)m_le.GetCurrentPattern(WindowPattern.Pattern)).Current.WindowVisualState;

                try
                {

                    //"Step: Obtain a random point on the desktop", 
                    TS_GetRandomOtherScreenLocation(out point, true, CheckType.Verification);
                    TS_SetWindowVisualState(m_le, WindowVisualState.Minimized, CheckType.IncorrectElementConfiguration);
                    TestMove1(false, CheckType.Verification, typeof(InvalidOperationException), WindowPosition.Exact, EventFired.False, point, true, HasFocus.DontCare);
                }
                finally
                {
                    TS_SetWindowVisualState(m_le, originalState, CheckType.IncorrectElementConfiguration);
                }
            }
        }

        ///<summary>TestCase: Move.2.18</summary>
        [TestCaseAttribute("Move.2.18",
            TestSummary = "Move a minimized movable element(ie. MDI window minimized)",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Step: Obtain a random location on the screen other than current location", 
                "Step: Miminimize the element",
                /* TestMove1 */
                "Verify: TransformPattern.CanMove == true", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move() with the random point is called successfully",
                "Step: Wait for event to get fired",
                "Verify: The BoundingRectangle PropertyChange event is fired", 
                "Verify: Element has moved to the new location", 
                "Step: Move the element back to it's original position", 
                "Step: Set the element's WindowVisualState back to it's original state"
        })]
        public void Move218(TestCaseAttribute testCase)
        {
            //This test only applies to Window but Transform pattern can conditionally apply to other controls as well.
            if (CheckControlClass(m_le, "Window"))
            {
                HeaderComment(testCase);

                Point point = new Point();
                WindowVisualState originalState = ((WindowPattern)m_le.GetCurrentPattern(WindowPattern.Pattern)).Current.WindowVisualState;

                try
                {
                    //"Step: Obtain a random point on the desktop", 
                    TS_GetRandomOtherScreenLocation(out point, true, CheckType.Verification);
                    TS_SetWindowVisualState(m_le, WindowVisualState.Minimized, CheckType.IncorrectElementConfiguration);
                    TestMove1(true, CheckType.Verification, null, WindowPosition.Exact, EventFired.True, point, true, HasFocus.DontCare);
                }
                finally
                {
                    TS_SetWindowVisualState(m_le, originalState, CheckType.IncorrectElementConfiguration);
                }
            }
        }

        ///<summary>TestCase: Move.2.19</summary>
        [TestCaseAttribute("Move.2.19",
            TestSummary = "Element is maximized, and covers entire screen",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri1, Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: Call WindowPattern.SetWindowVisualState(WindowVisualState.Maximize)", 
                "Verify:  WindowVisualState = WindowVisualState.Maximize", 
                "Precondition: Element does cover enitire current screen", 
                /* TestMove1 */
                "Verify: TransformPattern.CanMove == false", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Move() with the random point throws InvalidOperationException",
                "Step: Wait for event to get fired",
                "Verify: The BoundingRectangle PropertyChange event is not fired", 
                "Verify: Element has not moved",
                "Step: Move the element back to it's original position", 
                "Step: Set the element's WindowVisualState back to it's original state",
        })]
        public void Move219(TestCaseAttribute testCase)
        {
            //This test only applies to Window but Transform pattern can conditionally apply to other controls as well.
            if (CheckControlClass(m_le, "Window"))
            {
                HeaderComment(testCase);

                Point point = new Point();

                WindowVisualState originalState = ((WindowPattern)m_le.GetCurrentPattern(WindowPattern.Pattern)).Current.WindowVisualState;

                try
                {
                    TS_SetWindowVisualState(m_le, WindowVisualState.Maximized, CheckType.IncorrectElementConfiguration);
                    TS_VerifyWindowVisualState(m_le, WindowVisualState.Maximized, CheckType.IncorrectElementConfiguration);
                    TS_WindowCoversEntireScreen(CheckType.IncorrectElementConfiguration);
                    TestMove1(false, CheckType.Verification, typeof(InvalidOperationException), WindowPosition.Exact, EventFired.False, point, true, HasFocus.DontCare);
                }
                finally
                {
                    TS_SetWindowVisualState(m_le, originalState, CheckType.IncorrectElementConfiguration);
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TestMove1(bool canMove, CheckType checkType, Type expectedException, WindowPosition placement, EventFired eventFired, Point point, bool moveWindowBackToOriginalPostion, HasFocus hasFocus)
        {
            Point oldPoint = new Point(m_le.Current.BoundingRectangle.Left, m_le.Current.BoundingRectangle.Top);

            try
            {
                //"Precondition: TransformPattern.CanMove == true", 
                TSC_VerifyProperty(pCanMove, canMove, "TransformPattern.CanMove", CheckType.IncorrectElementConfiguration);

                //"Step: Ensure the AutomationElement does not have focus by calling SetFocus(AutomationElement.RootElement.FirstChild) - "OS's Start Button"",
                switch (hasFocus)
                {
                    case HasFocus.No:
                        TSC_SetFocusVerifyWithEvent(ControlFirstChild(AutomationElement.RootElement), CheckType.Verification);
                        break;

                    case HasFocus.DontCare:
                        break;

                    default:
                        throw new ArgumentException("Need to handle a HasFocus enum");
                }

                //"Step: Setup a BoundingRectangle PropertyChange event", 
                TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);

                //"Step: Verify that Move with the random point is called successfully", 
                TS_Move(point, expectedException, CheckType.Verification);

                //"Step: Wait for event to get fired",
                TSC_WaitForEvents(2);

                //
                if (!CheckControlClass(m_le, "GridSplitter"))
                {
                    if (placement == WindowPosition.Exact)
                    {
                        //"Verify: The BoundingRectangle PropertyChange event is fired",
                        TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { eventFired }, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);
                        if (eventFired == EventFired.True)
                        {
                            // Verify element was moved to the correct position by validating the BoundingRectangle
                            TS_VerifyBoundingRect(new Rect(point.X, point.Y, m_le.Current.BoundingRectangle.Width, m_le.Current.BoundingRectangle.Height), m_le.Current.BoundingRectangle, CheckType.Verification);
                        }
                        else
                        {
                            // Verify element was not moved to the correct position by validating the BoundingRectangle
                            TS_VerifyBoundingRect(new Rect(oldPoint.X, oldPoint.Y, m_le.Current.BoundingRectangle.Width, m_le.Current.BoundingRectangle.Height), m_le.Current.BoundingRectangle, CheckType.Verification);
                        }
                    }
                    else if (placement == WindowPosition.Within)
                    {
                        // "Verify: The BoundingRectangle PropertyChange event is fired if the element is moved",
                        if (m_le.Current.BoundingRectangle.Left == point.X && m_le.Current.BoundingRectangle.Top == point.Y)
                        {
                            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.False }, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);
                        }
                        else
                        {
                            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);
                        }

                        // OS does not allow off the screen so will move it back to a good location
                        TS_VerifyPlacedWithinScreen(CheckType.Verification);

                    }
                }
            }
            finally // incase we throw expection, come back and clean up
            {
                if (moveWindowBackToOriginalPostion)
                {
                    //"Step: Move the window back to it's original position",
                    Comment("Moving window back to it's original position");
                    if (oldPoint != new Point(m_le.Current.BoundingRectangle.Left, m_le.Current.BoundingRectangle.Top))
                        TS_Move(oldPoint, null, CheckType.Verification);
                    else
                        m_TestStep++;
                }
            }
        }
        #endregion Move

        /// <summary>
        /// Verify control class name - targetting GridSplitter control. 
        /// Issues:
        ///     a. after move to (0,0) --> the check for BoundingRectangle propertychange event failed
        ///     b. after move to random spot on the desktop --> the check for BoundingRectangle propertychange event failed
        ///     c. the bug can be verified with the WPF Test Results Viewer app - as per Ivo. 
        /// </summary>
        /// <param name="element">element to work on</param>
        /// <returns>true if the given class name, else false</returns>
        internal bool CheckControlClass(AutomationElement element, string controlClass)
        {
            string className = (string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
            return className == controlClass;
        }

        #region Resize 3.X
        ///<summary>TestCase: Resize.3.2</summary>
        [TestCaseAttribute("Resize.3.2",
            TestSummary = "If a element is sizable, then call Transform.Resize on the window and verify that it resized and that the events were fired",
            TestCaseType = TestCaseType.Events, 
            EventTested = "AutomationPropertyChangedEventHandler(AutomationElement.BoundingRectangleProperty)",
            Priority = TestPriorities.Pri0,
            ProblemDescription = "Cannot determine exact corrdinates of a window, there may be some 'steping' of valid locations",
            Status = TestStatus.Problem,
            Author = "Microsoft",
            Description = new string[] {
                "Precondition: TransformPattern.CanResize == true", 
                "Precondition: Obtain a random width/height of the element", 
                "Step: Setup a BoundingRectangle PropertyChange event", 
                "Step: Verify that Resize with the random point is called successfully", 
                "Verify: The BoundingRectangle PropertyChange event is fired",
                "Verify: BoundingRect is correct",
                "Step: Move the window back to it's original position",
        })]
        public void Resize32(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            Rect oldRect = m_le.Current.BoundingRectangle;
            int Height = 0, Width = 0;
            try
            {
                //"Precondition: TransformPattern.CanResize == true", 
                TSC_VerifyProperty(pCanResize, true, "TransformPattern.CanResize", CheckType.IncorrectElementConfiguration);

                //"Precondition: Obtain a random width/height of the element", 
                TS_GetRandomWindowSize(out Width, out Height, CheckType.IncorrectElementConfiguration);

                //"Step: Setup a BoundingRectangle PropertyChange event", 
                TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);

                //"Step: Verify that Resize with the random point is called successfully", 
                TS_Resize(Width, Height, null, CheckType.Verification);

                //"Verify: The BoundingRectangle PropertyChange event is fired",
                TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { AutomationElement.BoundingRectangleProperty }, CheckType.Verification);

                // Verify BoundingRect is correct
                TS_VerifyBoundingRect(new Rect(oldRect.Left, oldRect.Top, Width, Height), m_le.Current.BoundingRectangle, CheckType.Verification);
            }
            finally // incase we throw, we can come back in here and clean up
            {
                //"Step: Move the window back to it's original position",
                TS_Resize(oldRect.Width, oldRect.Height, null, CheckType.Verification);
            }
        }
        #endregion Resize

        #region CanRotateProperty 4.X

        ///<summary>TestCase: CanRotate.4.1</summary>
        [TestCaseAttribute("CanRotateProperty.4.1",
            TestSummary = "Verify that the value of CanRotate is correct",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Arguments,
            Priority = TestPriorities.Pri0,
            Client = Client.ATG,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
                "Verify: The value of CanRotate is equal the expected value", 
        })]
        public void CanRotateProperty41(TestCaseAttribute testCase, object[] argument)
        {
            HeaderComment(testCase);
            if (argument.Length != 1)
                throw new ArgumentException();

            // Get the value passed into the test case
            bool expectedValue = (bool)(argument[0]);

            // Get the actual value
            bool actual = m_pattern.Current.CanRotate;

            Comment("TransformPattern.CanRotate == " + actual);

            if (actual != expectedValue)
                ThrowMe(CheckType.Verification, "TransformPattern.CanRotate != " + actual);
        }
        #endregion CanRotate


        #region TS_

        /// ----------------------------------------------------------------------------
        void TS_Resize(double width, double height, Type expectedException, CheckType checkType)
        {
            pattern_Resize(width, height, expectedException, checkType);
            m_TestStep++;
        }

        /// ----------------------------------------------------------------------------
        void TS_Rotate(double degree, Type expectedException, CheckType checkType)
        {
            pattern_Rotate(degree, expectedException, checkType);
            m_TestStep++;
        }

        /// ----------------------------------------------------------------------------
        void TS_GetRandomWindowSize(out int Width, out int Height, CheckType checkType)
        {

            object objHwnd = m_le.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);
            //			object objHwnd = m_le.RawElement.GetPropertyValue(RawElement.NativeWindowHandleProperty);
            IntPtr ptr = IntPtr.Zero;

            if (objHwnd is IntPtr)
                ptr = (IntPtr)objHwnd;
            else
                ptr = new IntPtr(Convert.ToInt64(objHwnd, CultureInfo.CurrentCulture));

            if (ptr == IntPtr.Zero)
                ThrowMe(checkType, "Could not get the handle to the element(window)");


            NativeMethods.RECT originalRect = new NativeMethods.RECT();
            NativeMethods.RECT smallRect = new NativeMethods.RECT();

            // get original size so we can restore after
            SafeNativeMethods.GetWindowRect(ptr, ref originalRect);

            // try to move it to it's smallest size so we can determine the smallest size this can be
            SafeNativeMethods.MoveWindow(ptr, 1, 1, 1, 1, 0);

            // get the random window size 
            SafeNativeMethods.GetWindowRect(ptr, ref smallRect);
            Width = (int)Helpers.RandomValue(smallRect.right - smallRect.left, originalRect.right - originalRect.left);
            Height = (int)Helpers.RandomValue(smallRect.bottom - smallRect.top, originalRect.bottom - originalRect.top);

            // put the window back.
            SafeNativeMethods.MoveWindow(ptr, (int)originalRect.left, (int)originalRect.top, (int)(originalRect.right - originalRect.left), (int)(originalRect.bottom - originalRect.top), -1);
            Comment("Found random window size of (" + Width + " x " + Height + ")");
            m_TestStep++;
        }

        /// ----------------------------------------------------------------------------
        void TS_Move(Point point, Type expectedException, CheckType checkType)
        {
            pattern_Move(point.X, point.Y, expectedException, checkType);
            m_TestStep++;
        }

        /// ----------------------------------------------------------------------------
        void TS_VerifyPlacedWithinScreen(CheckType checkType)
        {
            Rect rect = m_le.Current.BoundingRectangle;
            Screen screen = Screen.FromPoint(new System.Drawing.Point((int)rect.Left, (int)rect.Top));
            Drawing.Rectangle dRect = new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

            ThrowMeAssert(checkType,
                Drawing.Rectangle.Intersect(screen.WorkingArea, dRect).Equals(dRect),
                "Verify element is withing working area",
                String.Format("Element {0} is not withing {1}", dRect, screen.WorkingArea));

            m_TestStep++;
        }

        /// ----------------------------------------------------------------------------
        void TS_WindowNotCoveredEntireScreen(CheckType checkType)
        {
            Rect rect = m_le.Current.BoundingRectangle;
            Screen screen = Screen.FromPoint(new System.Drawing.Point((int)(rect.Left + ((rect.Right - rect.Left) / 2)), (int)(rect.Top + ((rect.Bottom - rect.Top) / 2))));
            Drawing.Rectangle AppRect = new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

            // Normal maximized window that fills entire selected screen
            if (Drawing.Rectangle.Intersect(screen.WorkingArea, AppRect).Equals(screen.WorkingArea))
                ThrowMe(checkType, "Element fills the entire screen");

            m_TestStep++;
        }

        /// ----------------------------------------------------------------------------
        void TS_WindowCoversEntireScreen(CheckType checkType)
        {
            Rect rect = m_le.Current.BoundingRectangle;
            Screen screen = Screen.FromPoint(new System.Drawing.Point((int)(rect.Left + ((rect.Right - rect.Left) / 2)), (int)(rect.Top + ((rect.Bottom - rect.Top) / 2))));
            Drawing.Rectangle AppRect = new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

            // Normal maximized window that fills entire selected screen
            if (Drawing.Rectangle.Intersect(screen.WorkingArea, AppRect).Equals(AppRect))
                ThrowMe(checkType, "Element does not fill the entire screen");

            m_TestStep++;
        }

        /// ----------------------------------------------------------------------------
        void TS_GetRandomEntirelyOutsideParentViewport(out Rect rect, CheckType checkType)
        {
            int xLeft = 0, yTop = 0;
            Rect meRect = m_le.Current.BoundingRectangle;

            Comment("Element.BoundingRectangle = " + meRect);

            // determine if it is a top level window
            // 
            WindowPattern wp = m_le.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;

            if (wp == null)
                throw new Exception("Object does not support WindowPattern...not sure what to do.  May be a case where it is within a window application and not a topmost");

            // Brain dead, just get the exents
            Screen[] screens = Screen.AllScreens;
            Drawing.Rectangle rectScreen = new Drawing.Rectangle();

            // Build up and intersection of all screens
            foreach (Screen screen in screens)
            {
                rectScreen = System.Drawing.Rectangle.Union(rectScreen, screen.WorkingArea);
            }

            Comment("Union of all screen working areas is = (" + rectScreen + ")");

            Drawing.Rectangle intersectRect;

            do
            {
                xLeft = (int)Helpers.RandomValue(int.MinValue, int.MaxValue);
                yTop = (int)Helpers.RandomValue(int.MinValue, int.MaxValue);
                intersectRect = Drawing.Rectangle.Intersect(rectScreen, new Drawing.Rectangle(xLeft, yTop, (int)meRect.Width, (int)meRect.Height));
            } while (!intersectRect.IsEmpty);

            rect = new Rect(xLeft, yTop, meRect.Width, meRect.Height);
            Comment("Random screen area returned is (" + rect + ")");
            m_TestStep++;
        }

        /// ----------------------------------------------------------------------------
        void TS_GetRandomOverlappingParentViewport(out Rect rect, CheckType checkType)
        {
            int xLeft = 0, yTop = 0;
            Rect meRect = m_le.Current.BoundingRectangle;

            Comment("Element.BoundingRectangle = " + meRect);

            // determine if it is a top level window
            // 
            WindowPattern wp = m_le.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            if (wp == null)
                throw new Exception("Object does not support WindowPattern...not sure what to do.  May be a case where it is within a window application and not a topmost");

            Screen[] screens = Screen.AllScreens;
            int randomScreen = (int)Helpers.RandomValue(0, screens.Length);
            System.Drawing.Rectangle rectScreen = screens[randomScreen].WorkingArea;

            Comment("Selection screen[" + randomScreen + "].WorkingArea = " + rectScreen);

            // Use 2 since we are taking off one pixel overlapp on each side
            double CYCAPTION = SafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYCAPTION);
            double CXBORDER = SafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXBORDER);
            double CYBORDER = SafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYBORDER);

            rect = new Rect(
                rectScreen.Left - meRect.Width + (CXBORDER * 1.2),
                rectScreen.Top - CYCAPTION / 2,
                rectScreen.Width + (meRect.Width - (CXBORDER * 1.2)),
                rectScreen.Height + (CYCAPTION / 2) - CXBORDER * 1.2);

            // When we have at least two corners outside of the viewport, we have it
            int count;

            do
            {
                count = 0;
                xLeft = (int)Convert.ToDouble(Helpers.RandomValue(rect.Left, rect.Right));
                yTop = (int)Convert.ToDouble(Helpers.RandomValue(rect.Top, rect.Bottom));
                count += xLeft < rectScreen.Left ? 1 : 0;
                count += yTop < rectScreen.Top ? 1 : 0;
                count += xLeft + meRect.Width > rectScreen.Right ? 1 : 0;
                count += yTop + meRect.Height > rectScreen.Bottom ? 1 : 0;

            } while (count < 2);

            rect = new Rect(xLeft, yTop, meRect.Width, meRect.Height);
            //_move(rect.X, rect.Y);
            Comment("Found random location overlapping parent viewport : " + rect);
            m_TestStep++;
        }

        /// ----------------------------------------------------------------------------
        void TS_GetRandomOtherScreenLocation(out Point point, bool AllowOrigin, CheckType checkType)
        {
            Screen[] screens = Screen.AllScreens;
            int randomScreen = (int)Helpers.RandomValue(0, screens.Length);
            System.Drawing.Rectangle rectScreen = screens[randomScreen].WorkingArea;

            Comment("Selection screen[" + randomScreen + "].WorkingArea = " + rectScreen);

            int xLeft, yTop;
            double currentX = m_le.Current.BoundingRectangle.Left;
            double currentY = m_le.Current.BoundingRectangle.Top;

            // Can't just use 0 or 1 since Left/Top may be negative numbers
            if (AllowOrigin)
            {
                xLeft = (int)Helpers.RandomValue(rectScreen.Left, rectScreen.Right);
                yTop = (int)Helpers.RandomValue(rectScreen.Top, rectScreen.Bottom);
            }
            else
            {
                // I assume Left/Top and Right/Bottom will allow a valid location so won't get endless loop<yikes>
                // Don't allow current position, or 0
                do
                {
                    xLeft = (int)Helpers.RandomValue(rectScreen.Left, rectScreen.Right);
                } while (xLeft == currentX | xLeft == 0);

                do
                {
                    yTop = (int)Helpers.RandomValue(rectScreen.Top, rectScreen.Bottom);
                } while (yTop == currentY | yTop == 0);
            }

            point = new Point(xLeft, yTop);
            Comment("Random screen location = " + point);
            m_TestStep++;
        }

        #endregion TS_


    }
}
