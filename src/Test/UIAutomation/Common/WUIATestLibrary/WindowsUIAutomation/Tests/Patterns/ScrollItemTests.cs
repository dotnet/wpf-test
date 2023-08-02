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
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Automation;

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
    public class ScrollItemPatternWrapper : PatternObject
    {

        #region Variables

        /// -----------------------------------------------------------------------
        /// <summary></summary>
        /// -----------------------------------------------------------------------
        ScrollItemPattern _pattern;

        /// -----------------------------------------------------------------------
        /// <summary></summary>
        /// -----------------------------------------------------------------------
        internal ScrollPattern _scrollPattern;

        /// -----------------------------------------------------------------------
        /// <summary></summary>
        /// -----------------------------------------------------------------------
        internal AutomationElement _container;

        /// -----------------------------------------------------------------------
        /// <summary></summary>
        /// -----------------------------------------------------------------------
        internal const double _HUNDRED = 100D;

        /// -----------------------------------------------------------------------
        /// <summary></summary>
        /// -----------------------------------------------------------------------
        internal const double _ZERO = 0D;


        #endregion Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal ScrollItemPatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _pattern = (ScrollItemPattern)GetPattern(m_le, m_useCurrent, ScrollItemPattern.Pattern);
            if (_pattern == null)
                throw new Exception(Helpers.PatternNotSupported + ": ScrollItemPattern");

            // Find the ScrollPattern
            _container = m_le;

            while (_container != null && !(bool)_container.GetCurrentPropertyValue(AutomationElement.IsScrollPatternAvailableProperty))
                _container = TreeWalker.ControlViewWalker.GetParent(_container);

            // Check to see if we actual found the container of the scrollitem
            if (_container == null)
                throw new Exception(" Element does not have a container with ScrollPattern");

            Comment("Found scroll container: " + Library.GetUISpyLook(_container));

            _scrollPattern = (ScrollPattern)_container.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;

            System.Diagnostics.Debug.Assert(_scrollPattern != null);

        }

        #region Properties

        #endregion Properties

        #region Methods

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void pattern_ScrollIntoView(Type expectedException, CheckType checkType)
        {
            string call = "ScrollIntoView()";

            try
            {
                //Comment("Before " + call + ", BoundingRectagle = '" + m_le.Current.BoundingRectangle + "'");
                _pattern.ScrollIntoView();
                Comment("After " + call + ", BoundingRectagle = '" + m_le.Current.BoundingRectangle + "'");
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

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void helper_SetScrollPercent(double horizontalPercent, double verticalPercent, Type expectedException, CheckType checkType)
        {
            string call = "SetScrollPercent(" + horizontalPercent + ", " + verticalPercent + ")";
            string erroStr = expectedException == null ? "none" : expectedException.ToString();

            try
            {
                Comment("Before " + call + " HorizontalScrollPercent = " + _scrollPattern.Current.HorizontalScrollPercent + ", VerticalScrollPercent = '" + _scrollPattern.Current.VerticalScrollPercent + "'");

                // We really don't care if control can scroll to where we can based on the 
                // scrollability, just do something as much as we can and exit gracefully
                if ((bool)_container.GetCurrentPropertyValue(ScrollPattern.HorizontallyScrollableProperty) && (bool)_container.GetCurrentPropertyValue(ScrollPattern.VerticallyScrollableProperty))
                    _scrollPattern.SetScrollPercent(horizontalPercent, verticalPercent);
                else if ((bool)_container.GetCurrentPropertyValue(ScrollPattern.HorizontallyScrollableProperty) && !(bool)_container.GetCurrentPropertyValue(ScrollPattern.VerticallyScrollableProperty))
                    _scrollPattern.SetScrollPercent(horizontalPercent, ScrollPattern.NoScroll);
                else if (!(bool)_container.GetCurrentPropertyValue(ScrollPattern.HorizontallyScrollableProperty) && (bool)_container.GetCurrentPropertyValue(ScrollPattern.VerticallyScrollableProperty))
                    _scrollPattern.SetScrollPercent(ScrollPattern.NoScroll, verticalPercent);

                Thread.Sleep(1);

                Comment("After " + call + " HorizontalScrollPercent = " + _scrollPattern.Current.HorizontalScrollPercent + ", VerticalScrollPercent = '" + _scrollPattern.Current.VerticalScrollPercent + "'");
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

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
	public sealed class ScrollItemTests : ScrollItemPatternWrapper
    {
        #region Member variables

        /// -------------------------------------------------------------------
        /// <summary>
        /// Used to identify the assembly name
        /// </summary>
        /// -------------------------------------------------------------------
        const string THIS = "ScrollItemTests";

        /// <summary></summary>
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// <summary>Defines which UIAutomation Pattern this tests</summary>
        public static readonly string TestWhichPattern = Automation.PatternName(ScrollItemPattern.Pattern);

        #endregion Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ScrollItemTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.ScrollItem, dirResults, testEvents, commands)
        {
        }

        #region Tests

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ScrollIntoView.1",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: Parent's IsOffScreen == false",
                "Verification: Call ScrollIntoView"
            })]
        public void TestScrollItemBVT1(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            TSC_VerifyProperty(TreeWalker.ControlViewWalker.GetParent(m_le).Current.IsOffscreen, false, "IsOffscreen", CheckType.IncorrectElementConfiguration);

            pattern_ScrollIntoView(null, CheckType.Verification);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ScrollIntoView.2",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
				"Verification: Verify that the elements container is visible",
                "Step: Scroll to 0, 0",
                "Step: Call ScrollIntoView()",
                "Verification: The the element is within the view port of the parent"
            })]
        public void TestScrollItem1(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);
            TestDriverScrollIntoView1(_ZERO, _ZERO);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ScrollIntoView.3",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
				"Verification: Verify that the elements container is visible",
                "Step: Scroll to 0, 100",
                "Step: Call ScrollIntoView()",
                "Verification: The the element is within the view port of the parent"
            })]
        public void TestScrollItem2(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);
			TestDriverScrollIntoView1(_ZERO, _HUNDRED);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ScrollIntoView.4",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
				"Verification: Verify that the elements container is visible",
                "Step: Scroll to 100, 0",
                "Step: Call ScrollIntoView()",
                "Verification: The the element is within the view port of the parent"
            })]
        public void TestScrollItem3(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);
            TestDriverScrollIntoView1(_HUNDRED, _ZERO);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ScrollIntoView.5",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
				"Verification: Verify that the elements container is visible",
                "Step: Scroll to 100, 100",
                "Step: Call ScrollIntoView()",
                "Verification: The the element is within the view port of the parent"
            })]
        public void TestScrollItem4(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);
            TestDriverScrollIntoView1(_HUNDRED, _HUNDRED);
        }

        #endregion Tests

        #region TestDrivers

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TestDriverScrollIntoView1(double left, double top)
        {
			// "Verification: Verify that the elementss container is visible
			TSC_VerifyProperty(_container.Current.IsOffscreen, false, "Current.IsOffscreen", CheckType.IncorrectElementConfiguration);

            // "Step: Scroll to left, top",
            TS_Scroll(left, top, CheckType.Verification);

            // "Step: Call ScrollIntoView()",
            TS_ScrollIntoView(CheckType.Verification);

            // "Verification: The the element is within the view port of the parent"
            TS_IsElementWithinContainer(CheckType.Verification);
        }

        #endregion TestDrivers

        #region TS_* methods

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_ScrollIntoView(CheckType checkType)
        {
            pattern_ScrollIntoView(null, CheckType.Verification);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_Scroll(double left, double top, CheckType checkType)
        {
            helper_SetScrollPercent(left, top, null, checkType);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_IsElementWithinContainer(CheckType checkType)
        {
            Rect cRect = _container.Current.BoundingRectangle;
            Rect eRect = m_le.Current.BoundingRectangle;

            // only one point needs to be withing the parents rect
            if (!PointWithin(_container.Current.BoundingRectangle, new Point(eRect.Left, eRect.Top)) &&
                !PointWithin(_container.Current.BoundingRectangle, new Point(eRect.Right, eRect.Top)) &&
                !PointWithin(_container.Current.BoundingRectangle, new Point(eRect.Left, eRect.Bottom)) &&
                !PointWithin(_container.Current.BoundingRectangle, new Point(eRect.Right, eRect.Bottom))
                )
                ThrowMe(checkType, "Element's rect(" + eRect + ") is not within ScrollPattern element's rect(" + cRect + ")");

            m_TestStep++;
        }

        #endregion TS_* methods

        #region Helpers

        /// -------------------------------------------------------------------
        /// <summary>
        /// Determine if a point with within the rectangle
        /// </summary>
        /// -------------------------------------------------------------------
        bool PointWithin(Rect rect, Point point)
        {
            if (!(rect.Left <= point.X && point.X <= rect.Right) && !(rect.Bottom <= point.Y && point.Y <= rect.Top))
                return false;
            else
                return true;
        }

        #endregion Helpers

    }
}
