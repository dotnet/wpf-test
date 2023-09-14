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
using System.Drawing;
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

    enum Expected
    {
        True,
        False
    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class ScrollPatternWrapper : PatternObject
    {
        #region Variables

        // Pattern variable
        ScrollPattern _pattern;

        #endregion Variables

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		internal ScrollPatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _pattern = (ScrollPattern)GetPattern(m_le, m_useCurrent, ScrollPattern.Pattern);
        }

        #region Properties

        internal bool pattern_getHorizontallyScrollable { get { return _pattern.Current.HorizontallyScrollable; } }
        internal double pattern_getHorizontalScrollPercent { get { return _pattern.Current.HorizontalScrollPercent; } }
        internal double pattern_getHorizontalViewSize { get { return _pattern.Current.HorizontalViewSize; } }
        internal double pattern_getVerticalViewSize { get { return _pattern.Current.VerticalViewSize; } }
        internal bool pattern_getVerticallyScrollable { get { return _pattern.Current.VerticallyScrollable; } }
        internal double pattern_getVerticalScrollPercent { get { return _pattern.Current.VerticalScrollPercent; } }

        #endregion Properties

        #region Methods

        internal void pattern_Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount, Type expectedException, CheckType checkType)
        {

            string call = "Scroll(" + horizontalAmount + ", " + verticalAmount + ")";
            try
            {
                Comment("Before " + call + " VerticalScrollPercent = " + pattern_getVerticalScrollPercent + ", HorizontalScrollPercent = '" + pattern_getHorizontalScrollPercent + "'");
                _pattern.Scroll(horizontalAmount, verticalAmount);
                Comment("After " + call + " VerticalScrollPercent = " + pattern_getVerticalScrollPercent + ", HorizontalScrollPercent = '" + pattern_getHorizontalScrollPercent + "'");
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

        internal void pattern_SetScrollPercent(double horizontalPercent, double verticalPercent, Type expectedException, CheckType checkType)
        {
            string call = "SetScrollPercent(" + horizontalPercent + ", " + verticalPercent + ")";
            string erroStr = expectedException == null ? "none" : expectedException.ToString();

            try
            {
                Comment("Before " + call + " HorizontalScrollPercent = " + pattern_getHorizontalScrollPercent + ", VerticalScrollPercent = '" + pattern_getVerticalScrollPercent + "'");
                _pattern.SetScrollPercent(horizontalPercent, verticalPercent);
                Thread.Sleep(1);
                Comment("After " + call + " HorizontalScrollPercent = " + pattern_getHorizontalScrollPercent + ", VerticalScrollPercent = '" + pattern_getVerticalScrollPercent + "'");
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
    public sealed class ScrollTests : ScrollPatternWrapper
    {
        #region Member variables

        const double _HUNDRED = 100D;
        const double _ZERO = 0D;
        const double _FIFTY = 50D;
        const double _NEGONE = -1D;
        const double _SCROLLMAX = _HUNDRED;
        const double _SCROLLMIN = _ZERO;
        const double _NOSCROLL = -1D;
        const double _ACCURACY = 0.00000001D;

        double _verticalLargeIncrement = _NEGONE;
        double _horizontalLargeIncrement = _NEGONE;
        double _verticalSmallIncrement = _NEGONE;
        double _horizontalSmallIncrement = _NEGONE;
        double _verticalLargeDecrement = _NEGONE;
        double _horizontalLargeDecrement = _NEGONE;
        double _verticalSmallDecrement = _NEGONE;
        double _horizontalSmallDecrement = _NEGONE;
        double _horizontalViewSize = _NEGONE;
        bool _horizontalScrollable = false;
        double _verticalViewSize = _NEGONE;
        bool _verticalScrollable = false;
        ArrayList _validHPositions = new ArrayList();
        ArrayList _validVPositions = new ArrayList();
        bool _canRunTests = true;

        enum ScrollDirection
        {
            Horizontal, Vertical
        }

        enum ReturnVal
        {
            False
        }

        enum RandomBoundaries
        {
            between,
            CanIncludeUpper,
            CanIncludeLower,
            CanIncludeBoth
        }

        enum EndLocation
        {
            Zero,
            Hundred,
            Inbetween
        }

        const string THIS = "ScrollTests";

        /// <summary></summary>
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// <summary>Defines which UIAutomation Pattern this tests</summary>
        public static readonly string TestWhichPattern = Automation.PatternName(ScrollPattern.Pattern);

        #endregion Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ScrollTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.Scroll, dirResults, testEvents, commands)
        {
            DetermineValidPositions();
        }

        #region Step/Verification

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        new void HeaderComment(TestCaseAttribute testCaseAttribute)
        {
            // Make sure we have set up the tests correctly and know the valid locations
            if (!_canRunTests)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Initialization did not compete, so cannot run tests");

            base.HeaderComment(testCaseAttribute);
            CommentValidLocations();
        }

        /// -----------------------------------------------------------------------
        /// <summary></summary>
        /// -----------------------------------------------------------------------
        void ArrayListAdd(ArrayList al, object Object, bool AllowDupes)
        {
            if (AllowDupes)
                al.Add(Object);
            else
            {
                if (al.IndexOf(Object).Equals(-1))
                {
                    al.Add(Object);
                }
            }

        }

        /// -------------------------------------------------------------------
        /// <summary>Reset the position to a specific location</summary>
        /// -------------------------------------------------------------------
        void ResetScrollPosition(double horzPos, double vertPos)
        {
            pattern_SetScrollPercent(horzPos, vertPos, null, CheckType.Verification);
            if (horzPos != -1)
            {
                double actualVal = pattern_getHorizontalScrollPercent;
                if (!actualVal.Equals(horzPos))
                {
                    //_canRunTests = false;
                    Comment("WARNING-WARNING-WARNING: After calling SetScrollPercent(), HorizontalScrollPercent is '" + actualVal + "' and should be " + horzPos + " which is required to run the Scroll tests");
                }
            }

            if (vertPos != -1)
            {
                double actualVal = pattern_getVerticalScrollPercent;
                if (!actualVal.Equals(vertPos))
                {
                    //_canRunTests = false;
                    Comment("WARNING-WARNING-WARNING: After calling SetScrollPercent(), VerticalScrollPercent is '" + actualVal + "' and should be " + vertPos + " which is required to run the Scroll tests");
                }
            }
        }

        /// -----------------------------------------------------------------------
        /// <summary></summary>
        /// -----------------------------------------------------------------------
        void DetermineValidPosition(ScrollDirection direction, ref ArrayList array)
        {

            _horizontalViewSize = pattern_getHorizontalViewSize;
            _horizontalScrollable = pattern_getHorizontallyScrollable;
            _verticalViewSize = pattern_getVerticalViewSize;
            _verticalScrollable = pattern_getVerticallyScrollable;

            // Do some excersise here to make sure we can make calls with no exceptions, 
            // and to get some code coverage
            if (pattern_getVerticallyScrollable && pattern_getHorizontallyScrollable)
            {
                try
                {
                    pattern_Scroll(ScrollAmount.SmallIncrement, ScrollAmount.SmallIncrement, null, CheckType.Verification);
                    pattern_SetScrollPercent(0, 0, null, CheckType.Verification);
                }
                catch (TestErrorException)
                {
                }
            }
            else if (pattern_getHorizontallyScrollable && !pattern_getVerticallyScrollable)
            {
                try
                {
                    pattern_Scroll(ScrollAmount.SmallIncrement, ScrollAmount.NoAmount, null, CheckType.Verification);
                    pattern_SetScrollPercent(0, -1, null, CheckType.Verification);
                }
                catch (TestErrorException)
                {
                }

            }
            else if (!pattern_getHorizontallyScrollable && pattern_getVerticallyScrollable)
            {
                try
                {
                    pattern_Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallIncrement, null, CheckType.Verification);
                    pattern_SetScrollPercent(-1, 0, null, CheckType.Verification);
                }
                catch (TestErrorException)
                {
                }

            }


            // Need to wrap each call since some increments are not supported such as with LargeIncrement and the tab control
            switch (direction)
            {
                case ScrollDirection.Horizontal:

                    // >>> Get 0
                    ResetScrollPosition(_ZERO, _NEGONE);
                    ArrayListAdd(array, pattern_getHorizontalScrollPercent, false);

                    //  >>> Get 100
                    ResetScrollPosition(_HUNDRED, _NEGONE);
                    ArrayListAdd(array, pattern_getHorizontalScrollPercent, false);

                    // >>> Get SmallIncrement
                    ResetScrollPosition(_ZERO, _NEGONE);
                    try
                    {
                        pattern_Scroll(ScrollAmount.SmallIncrement, ScrollAmount.NoAmount, null, CheckType.Verification);
                        _horizontalSmallIncrement = pattern_getHorizontalScrollPercent;
                        ArrayListAdd(array, pattern_getHorizontalScrollPercent, false);
                    }
                    catch (TestErrorException)
                    {
                    }

                    // >>> Get LargeIncrement
                    ResetScrollPosition(_ZERO, _NEGONE);
                    try
                    {
                        pattern_Scroll(ScrollAmount.LargeIncrement, ScrollAmount.NoAmount, null, CheckType.Verification);
                        _horizontalLargeIncrement = pattern_getHorizontalScrollPercent;
                        ArrayListAdd(array, pattern_getHorizontalScrollPercent, false);
                    }
                    catch (TestErrorException)
                    {
                    }

                    // >>> Get SmallDecrement
                    ResetScrollPosition(_HUNDRED, _NEGONE);
                    try
                    {
                        pattern_Scroll(ScrollAmount.SmallDecrement, ScrollAmount.NoAmount, null, CheckType.Verification);
                        _horizontalSmallDecrement = _HUNDRED - pattern_getHorizontalScrollPercent;
                        ArrayListAdd(array, pattern_getHorizontalScrollPercent, false);
                    }
                    catch (TestErrorException)
                    {
                    }

                    // >>> Get LargeDecrement
                    ResetScrollPosition(_HUNDRED, _NEGONE);
                    try
                    {
                        pattern_Scroll(ScrollAmount.LargeDecrement, ScrollAmount.NoAmount, null, CheckType.Verification);
                        _horizontalLargeDecrement = _HUNDRED - pattern_getHorizontalScrollPercent;
                        ArrayListAdd(array, pattern_getHorizontalScrollPercent, false);
                    }
                    catch (TestErrorException)
                    {
                    }

                    break;
                case ScrollDirection.Vertical:
                    Comment("Determining valid Verical directions:");

                    // >>> Get 0
                    ResetScrollPosition(_NEGONE, _ZERO);
                    ArrayListAdd(array, pattern_getVerticalScrollPercent, false);

                    // >>> Get 100
                    ResetScrollPosition(_NEGONE, _HUNDRED);
                    ArrayListAdd(array, pattern_getVerticalScrollPercent, false);

                    // >>> Get SmallIncrement
                    ResetScrollPosition(_NEGONE, _ZERO);
                    try
                    {
                        pattern_Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallIncrement, null, CheckType.Verification);
                        _verticalSmallIncrement = pattern_getVerticalScrollPercent;
                        ArrayListAdd(array, pattern_getVerticalScrollPercent, false);
                    }
                    catch (TestErrorException)
                    {
                    }

                    // Get LargeIncrement from 0
                    ResetScrollPosition(_NEGONE, _ZERO);
                    try
                    {
                        pattern_Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement, null, CheckType.Verification);
                        _verticalLargeIncrement = pattern_getVerticalScrollPercent;
                        ArrayListAdd(array, pattern_getVerticalScrollPercent, false);
                    }
                    catch (TestErrorException)
                    {
                    }

                    // Get SmallDecrement from 100
                    ResetScrollPosition(_NEGONE, _HUNDRED);
                    try
                    {
                        pattern_Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallDecrement, null, CheckType.Verification);
                        _verticalSmallDecrement = _HUNDRED - pattern_getVerticalScrollPercent;
                        ArrayListAdd(array, pattern_getVerticalScrollPercent, false);
                    }
                    catch (TestErrorException)
                    {
                    }

                    // Get LargeDecrement from 100
                    pattern_SetScrollPercent(_NEGONE, _HUNDRED, null, CheckType.Verification);
                    try
                    {
                        pattern_Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeDecrement, null, CheckType.Verification);
                        _verticalLargeDecrement = _HUNDRED - pattern_getVerticalScrollPercent;
                        ArrayListAdd(array, pattern_getVerticalScrollPercent, false);
                    }
                    catch (TestErrorException)
                    {
                    }

                    break;
                default:
                    throw new Exception("Unhandled argument");
            }

            // Now get x random elements between SmallDecrement and SmallIncrement
            if (array.Count > 3)
            {
                double min = (double)array[1];
                double max = (double)array[3];
                double loc, loc2;
                for (int i = 0; i < 20; i++)
                {
                    loc = (double)Helpers.RandomValue(min, max);
                    if (direction.Equals(ScrollDirection.Horizontal))
                    {
                        // WCP: HorizontalScrollPercent = loc;
                        pattern_SetScrollPercent(loc, _NEGONE, null, CheckType.Verification);
                        loc = pattern_getHorizontalScrollPercent;  //patternScroll may jump to near position

                        // Make sure we can duplicate this.
                        pattern_SetScrollPercent(loc, _NEGONE, null, CheckType.Verification);
                        loc2 = pattern_getHorizontalScrollPercent;

                        if (loc == loc2)
                        {
                            if (array.IndexOf(loc).Equals(-1))
                            {
                                System.Diagnostics.Trace.WriteLine("H:" + loc);
                                ArrayListAdd(array, loc, false);
                            }
                        }
                    }
                    else
                    {
                        // WCP: VerticalScrollPercent = loc;
                        pattern_SetScrollPercent(_NEGONE, loc, null, CheckType.Verification);
                        loc = pattern_getVerticalScrollPercent;	 //patternScroll may jump to near position

                        // Make sure we can duplicate this.
                        pattern_SetScrollPercent(_NEGONE, loc, null, CheckType.Verification);
                        loc2 = pattern_getVerticalScrollPercent;	 //patternScroll may jump to near position
                        
                        if (loc == loc2)
                            if (array.IndexOf(loc).Equals(-1))
                            {
                                System.Diagnostics.Trace.WriteLine("V:" + loc);
                                ArrayListAdd(array, loc, false);
                            }
                    }
                }
            }
            array.Sort();
            foreach (object l in array)
            {
                Comment(" Found : " + l.ToString());
            }
        }
        #endregion Step/Verification

        #region Tests

        #region SetScrollPercent S.1

        void TestC_SetScrollPercent(bool HScrollable, bool VScrollable, double PreConitionHPercent, double PreConitionVPercent,
            double newHPercent,
            double newVPercent,
            double expectedHPercent,
            double expectedVPercent,
            EventFired HEventFire,
            EventFired VEventFire,
            Type exceptionExpected
            )
        {

            //"4 Step: Set HorizontalPercent = *",
            TSC_ScrollPercent(PreConitionHPercent, ScrollDirection.Horizontal, CheckType.IncorrectElementConfiguration);

            //"5 Step: Verify that it was set correctly",
            TSC_VerifyScrollPercent(PreConitionHPercent, ScrollDirection.Horizontal, CheckType.Verification);

            //"6 Step: Set VerticalPercent = 0",
            TSC_ScrollPercent(PreConitionVPercent, ScrollDirection.Vertical, CheckType.IncorrectElementConfiguration);

            //"7 Step: Verify that it was set correctly",
            if (!TS_FilterOnBug(m_TestCase))
                TSC_VerifyScrollPercent(PreConitionVPercent, ScrollDirection.Vertical, CheckType.Verification);

            //"8 Step: Add a property changed listener for HorizontalScrollPercentProperty",
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { ScrollPattern.HorizontalScrollPercentProperty }, CheckType.Verification);

            //"9 Step: Add a property changed listener for VerticalScrollPercentProperty",
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { ScrollPattern.VerticalScrollPercentProperty }, CheckType.Verification);

            //"10 Call SetScrollPercent (NoScroll,0) and verify that method returns true",
            TSC_SetScrollPercent(newHPercent, newVPercent, exceptionExpected, CheckType.Verification);

            // 11 Wait for 2 events
            TSC_WaitForEvents(2);

            //"12 Verify that HorizontalPercent = NoScroll",
            if (!TS_FilterOnBug(m_TestCase)) 
                TSC_VerifyScrollPercent(expectedHPercent, ScrollDirection.Horizontal, CheckType.Verification);

            //"13 Verify that VerticalPercent = 0",
            if (!TS_FilterOnBug(m_TestCase)) 
                TSC_VerifyScrollPercent(expectedVPercent, ScrollDirection.Vertical, CheckType.Verification);

            //"14 Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { HEventFire }, new AutomationProperty[] { ScrollPattern.HorizontalScrollPercentProperty }, CheckType.Verification);

            //"15 Verify that the firing of the event listener VerticalScrollPercentProperty is false",
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { VEventFire }, new AutomationProperty[] { ScrollPattern.VerticalScrollPercentProperty }, CheckType.Verification);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.1</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.1",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            TestSummary = "Verify that SetScrollPercent(0, NoScroll) succeeds, when position is at (0, NoScroll)(Test events)",
            Author = "Microsoft",
            Description = new string[]
			{
				"Precondition: Verify that HorizontalScrollable = false",
				"Precondition: Verify that VerticalScrollable = true",
				"Step: Set HorizontalPercent = NoScroll",
				"Step: Verify that it was set correctly",
				"Step: Set VerticalPercent = 0",
				"Step: Verify that it was set correctly",
				"Step: Add a property changed listener for HorizontalScrollPercentProperty",
				"Step: Add a property changed listener for VerticalScrollPercentProperty",
				"Call SetScrollPercent (NoScroll,0) and verify that method returns true",
				"Verify: Wait for the two events", 
				"Verify that HorizontalPercent = NoScroll",
				"Verify that VerticalPercent = 0", 
				"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined", 
				"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
			}
        )]
        public void TestSetScrollPercent11(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _ZERO, _NOSCROLL, _ZERO, _NOSCROLL, _ZERO, EventFired.Undetermined, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.2</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.2",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            TestSummary = "Verify that SetScrollPercent(NoScroll, RandomValue) succeeds, when position is at (NoScroll, 0)(Test events)",
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random vertical position V greater than 0",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll, V) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll",
			"Verify that VerticalPercent = V", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
		 }
             )]
        public void TestSetScrollPercent12(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;

            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _ZERO, _NOSCROLL, newV, _NOSCROLL, newV, EventFired.Undetermined, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.3</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.3",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            BugNumbers = new BugIssues[] {BugIssues.PS36 },
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            TestSummary = "Verify that SetScrollPercent(NoScroll, RandomValue) succeeds, when position is at (NoScroll, 0)(Test events)",
            Author = "Microsoft",
            Description = new string[]
			{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random vertical position newV greater than 0",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that HorizontalPercent is set correctly",
			"Step: Set VerticalPercent = 0",
			"Step: Verify that VerticalPercent is set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll, newV) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll",
			"Verify that VerticalPercent = newV", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
			}
        )]
        public void TestSetScrollPercent13(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _ZERO, _NOSCROLL, newV, _NOSCROLL, newV, EventFired.Undetermined, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.4</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.4",
            Priority = TestPriorities.Pri0,
            TestSummary = "Verify that SetScrollPercent(NoScroll, 100) succeeds, when position is at (NoScroll, 0)(Test events)",
            Status = TestStatus.Works,     
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Author = "Microsoft",
            Description = new string[]
			{
				"Precondition: Verify that HorizontalScrollable = false",
				"Precondition: Verify that VerticalScrollable = true",
				"Step: Set HorizontalPercent = NoScroll",
				"Step: Verify that it was set correctly",
				"Step: Set VerticalPercent = 0",
				"Step: Verify that it was set correctly",
				"Step: Add a property changed listener for HorizontalScrollPercentProperty",
				"Step: Add a property changed listener for VerticalScrollPercentProperty",
				"Call SetScrollPercent (NoScroll,100) and verify that method returns true",
				"Verify: Wait for the two events", 
				"Verify that HorizontalPercent = NoScroll",
				"Verify that VerticalPercent = 100", 
				"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
				"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
			}
            )]
        public void TestSetScrollPercent14(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _ZERO, _NOSCROLL, _HUNDRED, _NOSCROLL, _HUNDRED, EventFired.Undetermined, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.5</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.5",
            Priority = TestPriorities.Pri1,
            TestSummary = "Verify that SetScrollPercent(NoScroll, 0>x>100) succeeds, when position is at (NoScroll, 0)(Test events)",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            BugNumbers = new BugIssues[] {BugIssues.PS36 },
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random vertical position V greater than 0",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll, M =TSC_RandomPosition between 0 and 100) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll",
			"Verify that VerticalPercent = M", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is false",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
		 }
             )]
        public void TestSetScrollPercent15(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;

            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _ZERO, _NOSCROLL, newV, _NOSCROLL, newV, EventFired.Undetermined, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.6</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.6",
            Priority = TestPriorities.Pri0,
            TestSummary = "Verify that SetScrollPercent(NoScroll, x<0) fails, when position is at (NoScroll, 0)(Test events)",
            Status = TestStatus.Works, Author = "Microsoft",
           TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
           Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll,TSC_RandomPosition(-100, -2) < 0 & != NoScroll) and verify that throws exception",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll", 
            "Verify that VerticalPercent = 0", 
			"Verify that the firing of the event listener HorizonatcalScrollPercentProperty is undetermined", /* */
            "Verify that the firing of the event listener VerticalScrollPercentProperty is undetermined", /* */
		 }
             )]
        public void TestSetScrollPercent16(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;

            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = (double)Helpers.RandomValue(-_HUNDRED, -2D);
            /* */
            TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _ZERO, _NOSCROLL, newV, _NOSCROLL, _ZERO, EventFired.Undetermined, EventFired.Undetermined, typeof(ArgumentOutOfRangeException));
        }

        ///<summary>TestCase: SetScrollPercent.S.1.7</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.7",
           Priority = TestPriorities.Pri0,
           TestSummary = "Verify that SetScrollPercent(NoScroll, 101) fails, when position is at (NoScroll, 0)(Test events)",
           Status = TestStatus.Works,
           TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
           Author = "Microsoft",
           Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll, 101) and verify that method returns false",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll",
			"Verify that VerticalPercent = 0", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is false",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is false",
		 }
             )]
        public void TestSetScrollPercen(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;

            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _ZERO, _NOSCROLL, 101D, _NOSCROLL, _ZERO, EventFired.False, EventFired.False, typeof(ArgumentOutOfRangeException));
        }

        ///<summary>TestCase: SetScrollPercent.S.1.8</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.8",
            Priority = TestPriorities.Pri1,
            TestSummary = "Verify that when HorizontalScrollable = false and VerticalScrollable = true, calling SetScrollPercent(0>H>100, 0>=V>=100 ), scroll fails and does not fire any events",
            Status = TestStatus.Works,
            BugNumbers = new BugIssues[] { BugIssues.PS36 },
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random vertical position V greater than 0",
			"Precondition: Get a random horizontal position H between 0 and 100",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (V, H)and verify that System.InvalidOperationException is thrown",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll",
			"Verify that VerticalPercent = 0", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is false", 
			"Verify that the firing of the event listener VerticalScrollPercentProperty is false",
		 }
             )]
        public void TestSetScrollPercent18(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _ZERO, newH, newV, _NOSCROLL, newV, EventFired.False, EventFired.True, typeof(System.InvalidOperationException));
        }

        ///<summary>TestCase: SetScrollPercent.S.1.9</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.9",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            TestSummary = "Verify that SetScrollPercent(NoScroll, 0) succeeds, when position is at (NoScroll, 0<x<100)(Test events)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random vertical position V between 0 and 100",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = V",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll, 0) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll", 
            "Verify that VerticalPercent = 0", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
		 }
             )]
        public void TestSetScrollPercent19(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, newV, _NOSCROLL, _ZERO, _NOSCROLL, _ZERO, EventFired.Undetermined, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.10</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.10",
            Priority = TestPriorities.Pri1,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            TestSummary = "Verify that SetScrollPercent(NoScroll, 0<x<100) succeeds, when position is at (NoScroll, 0<x<100)(Test events)",
            Status = TestStatus.Works,
            BugNumbers = new BugIssues[]{BugIssues.PS36},
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random vertical position V between 0 and 100",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = V",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll, V) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll", 
			"Verify that VerticalPercent = V", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined", 
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		}
             )]
        public void TestSetScrollPercent110(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;

            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, newV, _NOSCROLL, newV, _NOSCROLL, newV, EventFired.False, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.11</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.11",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            TestSummary = "Verify that SetScrollPercent(NoScroll, 100) succeeds, when position is at (NoScroll, 0<x<100)(Test events)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random vertical position V between 0 and 100",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = V",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll, 100) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll", 
			"Verify that VerticalPercent = 100", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
		 }
             )]
        public void TestSetScrollPercent111(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, newV, _NOSCROLL, _HUNDRED, _NOSCROLL, _HUNDRED, EventFired.Undetermined, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.12</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.12",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            TestSummary = "Verify that SetScrollPercent(NoScroll, 0<x<100) succeeds, when position is at (NoScroll, 0<x<100)(Test events)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random vertical position V between 0 and 100",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = V",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll, V) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll",
			"Verify that VerticalPercent = No Change", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
		 }
             )]
        public void TestSetScrollPercent112(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;

            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, newV, _NOSCROLL, pattern_getVerticalScrollPercent, _NOSCROLL, pattern_getVerticalScrollPercent, EventFired.Undetermined, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.13</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.13",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            TestSummary = "Verify that SetScrollPercent(NoScroll, NoScroll) succeeds, when position is at (NoScroll, 0<x<100)(Test events)",
            Status = TestStatus.Works, Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random vertical position V between 0 and 100",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = V",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll,NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll",
			"Verify that VerticalPercent = Previous VerticalScrollPercentProperty", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is false",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is false",
		 }
             )]
        public void TestSetScrollPercent113(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, newV, _NOSCROLL, _NOSCROLL, _NOSCROLL, newV, EventFired.False, EventFired.False, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.14</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.14",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            TestSummary = "Verify that SetScrollPercent(NoScroll, 0) succeeds, when position is at (NoScroll, 100)(Test events)",
            Status = TestStatus.Works,     
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = 100",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll,0) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll",
			"Verify that VerticalPercent = 0", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
		 }
             )]
        public void TestSetScrollPercent114(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _HUNDRED, _NOSCROLL, _ZERO, _NOSCROLL, _ZERO, EventFired.Undetermined, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.15</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.15",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random vertical position newV less than 100",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = 100",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll, newV) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll", 
			"Verify that VerticalPercent = newV", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
		 }
             )]
        public void TestSetScrollPercent115(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _HUNDRED, _NOSCROLL, newV, _NOSCROLL, newV, EventFired.Undetermined, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.16</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.16",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,     
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = true",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = 100",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll,100) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll", 
			"Verify that VerticalPercent = 100", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined", 
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		}
             )]
        public void TestSetScrollPercent116(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _HUNDRED, _NOSCROLL, _HUNDRED, _NOSCROLL, _HUNDRED, EventFired.Undetermined, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.17</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.17",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Step: Set HorizontalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (0,NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = 0", 
            "Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent117(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _ZERO, _NOSCROLL, _ZERO, _NOSCROLL, _ZERO, _NOSCROLL, EventFired.Undetermined, EventFired.Undetermined, null);
        }

        void DetermineValidPositions()
        {
            if (_validHPositions.Count.Equals(0))
                if (pattern_getHorizontallyScrollable.Equals(true))
                    DetermineValidPosition(ScrollDirection.Horizontal, ref _validHPositions);

            if (_validVPositions.Count.Equals(0))
                if (pattern_getVerticallyScrollable.Equals(true))
                    DetermineValidPosition(ScrollDirection.Vertical, ref _validVPositions);
        }

        void CommentValidLocations()
        {
            int index = 0;
            Comment("Valid horizontal locations:");
            foreach (double val in _validHPositions)
            {
                Comment("{0}# : {1}", index++, val);
            }
            index = 0;
            Comment("Valid vertical locations:");
            foreach (double val in _validVPositions)
            {
                Comment("{0}# : {1}", index++, val);
            }
        }

        void MinMaxPosition(ref ArrayList array, ref double min, ref double max)
        {
            if (array.Count.Equals(0))
                return;

            int lower = 0;
            int upper = array.Count - 1;

            while ((double)array[lower] < min)
                lower++;

            while ((double)array[upper] > max)
                upper--;

            min = lower;
            max = upper;
        }
        double TSC_RandomPosition(ScrollDirection direction, double min, double max, RandomBoundaries rb, bool increment, CheckType ct)
        {
            string str = "Looking for a position ";
            switch (rb)
            {
                case RandomBoundaries.between:
                    str += "between " + min + " and " + max;
                    break;
                case RandomBoundaries.CanIncludeBoth:
                    str += "between and including " + min + " and " + max;
                    break;
                case RandomBoundaries.CanIncludeLower:
                    str += "greater than or equal to " + min + " and less than " + max;
                    break;
                case RandomBoundaries.CanIncludeUpper:
                    str += "greater than " + min + " and less than or equal to " + max;
                    break;
                default:
                    throw new Exception("Unhandled argument");
            }

            //Comment (str);

            if (min < 0 || max < 0 || min > _HUNDRED || max > _HUNDRED)
                return _NOSCROLL;

            switch (direction)
            {
                case ScrollDirection.Horizontal:
                    if (_validHPositions.Count.Equals(0))
                    {
                        if (increment)
                            m_TestStep++;
                        return _ZERO; // ZZZ NoScroll;
                    }
                    else
                        MinMaxPosition(ref this._validHPositions, ref min, ref max);
                    break;
                case ScrollDirection.Vertical:
                    if (_validVPositions.Count.Equals(0))
                    {
                        if (increment)
                            m_TestStep++;
                        return _ZERO; // ZZZ NoScroll;
                    }
                    else
                        MinMaxPosition(ref this._validVPositions, ref min, ref max);
                    break;
            }

            switch (rb)
            {
                case RandomBoundaries.between:
                    if (min == double.MaxValue)
                        throw new Exception("Cannot increase min past" + double.MaxValue);
                    else
                        min++;

                    if (max == double.MinValue)
                        throw new Exception("Cannot decrease max past " + double.MinValue);
                    else
                        max--;

                    break;
                case RandomBoundaries.CanIncludeBoth:
                    break;
                case RandomBoundaries.CanIncludeLower:
                    max--;
                    break;
                case RandomBoundaries.CanIncludeUpper:
                    min++;
                    break;
            }

            if (min > max)
                ThrowMe(ct, str + " : There are no scrollable positions that meet the criteria");

            // Is a scrollbar going to be larger thant 32K positions?  Nah????
            int val = (int)Helpers.RandomValue((int)min, (int)max + 1);

            switch (direction)
            {
                case ScrollDirection.Horizontal:
                    Comment("Found " + _validHPositions[val]);

                    if (increment)
                        m_TestStep++;

                    return (double)_validHPositions[val];

                case ScrollDirection.Vertical:
                    Comment("Found " + _validVPositions[val]);

                    if (increment)
                        m_TestStep++;

                    return (double)_validVPositions[val];

                default:
                    throw new Exception("Argument not handled");
            }
        }


        ///<summary>TestCase: SetScrollPercent.S.1.18</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.18",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random vertical position newH greater than 0",
			"Step: Set HorizontalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newH, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = newN",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent118(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _ZERO, _NOSCROLL, newH, _NOSCROLL, newH, _NOSCROLL, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.19</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.19",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random horizontal position newH greater than 0",
			"Step: Set HorizontalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newH, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = newH",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent119(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _ZERO, _NOSCROLL, newH, _NOSCROLL, newH, _NOSCROLL, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.20</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.20",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
        {
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random horizontal position newH greater than 0",
			"Step: Set HorizontalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newH, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = newH", 
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent120(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _ZERO, _NOSCROLL, newH, _NOSCROLL, newH, _NOSCROLL, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.21</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.21",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,     
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Step: Set HorizontalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (100, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = 100", 
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent121(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _ZERO, _NOSCROLL, _HUNDRED, _NOSCROLL, _HUNDRED, _NOSCROLL, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.22</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.22",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Step: Set HorizontalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (M=Random(-100, -2),NoScroll) and verify that method returns false",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent122(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newH = (double)Helpers.RandomValue(-_HUNDRED, -2D);
            this.TestC_SetScrollPercent(HScrollable, VScrollable, _ZERO, _NOSCROLL, newH, _NOSCROLL, _ZERO, _NOSCROLL, EventFired.Undetermined, EventFired.Undetermined, typeof(ArgumentOutOfRangeException));
        }

        ///<summary>TestCase: SetScrollPercent.S.1.23</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.23",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Step: Set HorizontalPercent = 0",
			"Step: Verify that it was set to 0",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly to NoScroll",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (101, NoScroll) and verify that method returns false",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = 0", 
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is false",
		 }
             )]
        public void TestSetScrollPercent123(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            try
            {
                this.TestC_SetScrollPercent(HScrollable, VScrollable, _ZERO, _NOSCROLL, 101D, _NOSCROLL, _ZERO, _NOSCROLL, EventFired.Undetermined, EventFired.False, typeof(ArgumentOutOfRangeException));
            }
            // the parameters are wrong so an out of range Exception is expected
            catch (System.ArgumentOutOfRangeException)
            {
                Comment("ArgumentOutOfRangeException was thrown");
            }
        }

        ///<summary>TestCase: SetScrollPercent.S.1.24</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.24",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random horizontal position newH greater than 0",
			"Step: Set HorizontalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newH,NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll", 
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is false",
		 }
             )]
        public void TestSetScrollPercent124(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _ZERO, _NOSCROLL, newH, _NOSCROLL, newH, _NOSCROLL, EventFired.Undetermined, EventFired.False, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.25</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.25",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random horizontal position newH",
			"Precondition: Get a random horizontal position preH that is different than newH",
			"Step: Set HorizontalPercent = preH",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newH, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = 0", "Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent125(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);
            double preH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);

            VerifyNotSame(ref preH, "preH", newH, "newH", _validHPositions, ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, preH, _NOSCROLL, newH, _NOSCROLL, newH, _NOSCROLL, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.26</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.26",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random horizontal position preH",
			"Precondition: Get a random horizontal position newH that is different than preH",
			"Step: Set HorizontalPercent = preH",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newH, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = newH",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent126(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);
            double preH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);

            VerifyNotSame(ref preH, "preH", newH, "newH", _validHPositions, ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, preH, _NOSCROLL, newH, _NOSCROLL, newH, _NOSCROLL, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.27</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.27",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random horizontal position preH",
			"Step: Set HorizontalPercent = preH",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (preH, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = No Change",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent127(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double preH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, preH, _NOSCROLL, preH, _NOSCROLL, preH, _NOSCROLL, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.28</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.28",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random horizontal position preH less than 100",
			"Step: Set HorizontalPercent = preH",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (100, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = 100",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent128(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double preH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, preH, _NOSCROLL, _HUNDRED, _NOSCROLL, _HUNDRED, _NOSCROLL, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.29</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.29",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random horizontal position preH",
			"Step: Set HorizontalPercent = preH",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = HorizontalPercent",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is false",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent129(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double preH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, preH, _NOSCROLL, _NOSCROLL, _NOSCROLL, preH, _NOSCROLL, EventFired.False, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.30</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.30",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random horizontal position newH greater than 0",
			"Step: Set HorizontalPercent = 100",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newH, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = M",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent130(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _HUNDRED, _NOSCROLL, newH, _NOSCROLL, newH, _NOSCROLL, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.31</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.31",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,     
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Step: Set HorizontalPercent = 100",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (0, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = 0",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent131(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _HUNDRED, _NOSCROLL, _ZERO, _NOSCROLL, _ZERO, _NOSCROLL, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.32</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.32",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,     
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = false",
			"Step: Set HorizontalPercent = 100",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (100, NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = 100",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent132(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _HUNDRED, _NOSCROLL, _HUNDRED, _NOSCROLL, _HUNDRED, _NOSCROLL, EventFired.Undetermined, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.33</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.33",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = false",
			"Precondition: Get a random horizontal position newH",
			"Precondition: Get a random vertical position newV",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newH, newV) and verify that method throws System.InvalidOperationException",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll", 
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is false",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is false",
		 }
             )]
        public void TestSetScrollPercent133(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);
            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _NOSCROLL, newH, newV, _NOSCROLL, _NOSCROLL, EventFired.False, EventFired.False, typeof(System.InvalidOperationException));
        }

        ///<summary>TestCase: SetScrollPercent.S.1.34</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.34",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = false",
			"Precondition: Verify that VerticalScrollable = false",
			"Step: Set HorizontalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = NoScroll",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (NoScroll,NoScroll) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = NoScroll",
			"Verify that VerticalPercent = NoScroll", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is false",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is false",
		 }
             )]
        public void TestSetScrollPercent134(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, _NOSCROLL, _NOSCROLL, _NOSCROLL, _NOSCROLL, _NOSCROLL, _NOSCROLL, EventFired.False, EventFired.False, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.35</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.35",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random horizontal position preH",
			"Precondition: Get a random horizontal position newH different from preH",
			"Step: Set HorizontalPercent = preH",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = 0",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newH, 0) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = newH",
			"Verify that VerticalPercent = 0", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is Undetermined",
		 }
             )]
        public void TestSetScrollPercent135(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double preH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);
            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);

            VerifyNotSame(ref newH, "newH", preH, "preH", _validHPositions, ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, preH, _ZERO, newH, _ZERO, newH, _ZERO, EventFired.Undetermined, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.36.WithCode</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.36.WithCode",
            TestSummary = "For an element that supports horizontal and vertical scrolling, patternScroll from a random veritical and horizontal position, to a new random vertical and horizontal position.  Verify that the position is set accordingly and that events are fired for HorizontalScrollPercentProperty and VerticalScrollPercentProperty.",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random horizontal position preH",
			"Precondition: Get a random horizontal position newH different from preH",
			"Precondition: Get a random horizontal position preV",
			"Precondition: Get a random horizontal position newV different from preV",
			"Step: Set HorizontalPercent = preH",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = preV",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newV, newH) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = newH",
			"Verify that VerticalPercent = preH", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
		 }
             )]
        public void TestSetScrollPercent136(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);


            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double preV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);
            double preH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);
            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);
            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);

            VerifyNotSame(ref newV, "newV", preV, "preV", _validVPositions, ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, CheckType.IncorrectElementConfiguration);
            VerifyNotSame(ref newH, "newH", preH, "preH", _validHPositions, ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeBoth, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, preH, preV, newH, newV, newH, newV, EventFired.True, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.37.WithCode</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.37.WithCode",
            TestSummary = "For an element that supports horizontal and vertical scrolling, scroll from a random position whose veritical and horizontal position are both less than 100, to a new random horizontal position less than 100 and a vertical position of 100.  Verify that the position is set accordingly and that events are fired for HorizontalScrollPercentProperty and VerticalScrollPercentProperty.",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Author = "Microsoft", Description = new string[]
		{
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random horizontal position newH less than 100",
			"Precondition: Get a random horizontal position preH less than 100 and different than preH",
			"Precondition: Get a random vertical position preV less than 100",
			"Step: Set HorizontalPercent = preH",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = preV",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (newH, 100) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = newH",
			"Verify that VerticalPercent = 100", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is undetermined",
		 }
             )]
        public void TestSetScrollPercent137(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double newH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double preH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            VerifyNotSame(ref newH, "newH", preH, "preH", _validHPositions, ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeLower, CheckType.IncorrectElementConfiguration);

            double preV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, preH, preV, newH, _HUNDRED, newH, _HUNDRED, EventFired.True, EventFired.Undetermined, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.38</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.38",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] 
        {
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random horizontal position preH between 0 and 100",
			"Precondition: Get a random vertical position preV between 0 and 100",
			"Precondition: Get a random vertical position newV between 0 and 100 and different than preV",

			"Step: Set HorizontalPercent = prevH",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = prevV",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (0, newV) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = 0",
			"Verify that VerticalPercent = newV", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
		 }
             )]
        public void TestSetScrollPercent138(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double preH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);
            double preV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);
            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);

            VerifyNotSame(ref newV, "newV", preV, "preV", _validVPositions, ScrollDirection.Vertical, 0, _HUNDRED, RandomBoundaries.between, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, preH, preV, _ZERO, newV, _ZERO, newV, EventFired.True, EventFired.True, null);
        }

        ///<summary>TestCase: SetScrollPercent.S.1.39</summary>
        [TestCaseAttribute("SetScrollPercent.S.1.39",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] 
        {
			"Precondition: Verify that HorizontalScrollable = true",
			"Precondition: Verify that VerticalScrollable = true",
			"Precondition: Get a random horizontal position preH between 0 and 100",
			"Precondition: Get a random vertical position preV between 0 and 100",
			"Precondition: Get a random vertical position newV between 0 and 100 and not preV",
			"Step: Set HorizontalPercent = preH",
			"Step: Verify that it was set correctly",
			"Step: Set VerticalPercent = preV",
			"Step: Verify that it was set correctly",
			"Step: Add a property changed listener for HorizontalScrollPercentProperty",
			"Step: Add a property changed listener for VerticalScrollPercentProperty",
			"Call SetScrollPercent (100, newV) and verify that method returns true",
			"Verify: Wait for the two events", 
			"Verify that HorizontalPercent = 100",
			"Verify that VerticalPercent = M", 
			"Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
			"Verify that the firing of the event listener VerticalScrollPercentProperty is true",
		 }
             )]
        public void TestSetScrollPercent139(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double preH = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);
            double preV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);
            double newV = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.between, true, CheckType.IncorrectElementConfiguration);

            VerifyNotSame(ref newV, "newV", preV, "preV", _validVPositions, ScrollDirection.Vertical, _ZERO, _HUNDRED, RandomBoundaries.between, CheckType.IncorrectElementConfiguration);

            this.TestC_SetScrollPercent(HScrollable, VScrollable, preH, preV, _HUNDRED, newV, _HUNDRED, newV, EventFired.True, EventFired.True, null);
        }
        #endregion SetScrollPercent

        #region Scroll S.2
        void TestScroll(bool HScrollable,
            bool VScrollable,
            double PreConitionHPercent,
            double PreConitionVPercent,
            ScrollAmount arg_HScrollAmount,
            ScrollAmount arg_VScrollAmount,
            bool expectedMethReturn,
            object ExpectedHPercent,
            object ExpectedVPercent,
            EventFired HEventFire,
            EventFired VEventFire)
        {
            //Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)
            TS_ControlsSupportsLargeIncrementOrDecrement(m_le, arg_HScrollAmount, arg_VScrollAmount, CheckType.IncorrectElementConfiguration);

            //2 "Step: Set HorizontalPercent = PreConitionHPercent",
            TSC_ScrollPercent(PreConitionHPercent, ScrollDirection.Horizontal, CheckType.IncorrectElementConfiguration);

            //3 "Step: Verify that it was set correctly",
            TSC_VerifyScrollPercent(PreConitionHPercent, ScrollDirection.Horizontal, CheckType.Verification);

            //4 "Step: Set VerticalPercent = 0",
            TSC_ScrollPercent(PreConitionVPercent, ScrollDirection.Vertical, CheckType.IncorrectElementConfiguration);

            //5 "Step: Verify that it was set correctly",
            TSC_VerifyScrollPercent(PreConitionVPercent, ScrollDirection.Vertical, CheckType.Verification);

            //6 "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { ScrollPattern.HorizontalScrollPercentProperty }, CheckType.Verification);

            //7 "Step: Add a property changed listener for VerticalScrollPercentProperty",
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { ScrollPattern.VerticalScrollPercentProperty }, CheckType.Verification);

            //8 "Call SetScrollPercent (NoScroll,0) and verify that method returns true",
            TSC_Scroll(arg_HScrollAmount, arg_VScrollAmount, CheckType.Verification);

            //9 "Verify that HorizontalPercent = *",
            TSC_VerifyScrollPercent(ExpectedHPercent, ScrollDirection.Horizontal, CheckType.Verification);

            //10 "Verify that VerticalPercent = *",
            TSC_VerifyScrollPercent(ExpectedVPercent, ScrollDirection.Vertical, CheckType.Verification);

            // 11 "Step: Wait for events",
            TSC_WaitForEvents(1);

            //11 "Verify that the firing of the event listener HorizontalScrollPercentProperty is Undetermined",
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { HEventFire }, new AutomationProperty[] { ScrollPattern.HorizontalScrollPercentProperty }, CheckType.Verification);

            //12 "Verify that the firing of the event listener VerticalScrollPercentProperty is false",
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { VEventFire }, new AutomationProperty[] { ScrollPattern.VerticalScrollPercentProperty }, CheckType.Verification);
        }


        ///<summary>TestCase: Scroll.S.2.15</summary>
        [TestCaseAttribute("Scroll.S.2.15",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = False",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random vertical position preV less than 100",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (NoAmount,LargeDecrement) and verify that method returns True",
            "Verify that HorizontalPercent = NoScroll",
            "Verify that VerticalPercent = 0", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was True",
		})]
        public void TestScrollScrollS215(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = _NOSCROLL;
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _verticalLargeIncrement, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.LargeDecrement,		//ScrollAmount
                true,														//Expected return value from Scroll
                _NOSCROLL, _ZERO,												//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);					//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.17</summary>
        [TestCaseAttribute("Scroll.S.2.17",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,    
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = False",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random vertical position preV between and including (100 - LargeAmount) and 100",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (NoAmount,LargeDecrement) and verify that method returns True",
            "Verify that HorizontalPercent = NoScroll",
            "Verify that VerticalPercent = (preV - LargeScrollAmount) or zero", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was true",
		})]
        public void TestScrollScrollS217(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = _NOSCROLL;
            double preV = TSC_RandomPosition(ScrollDirection.Vertical, _verticalLargeDecrement, _HUNDRED, RandomBoundaries.CanIncludeBoth, true, CheckType.IncorrectElementConfiguration);
            double VExpected = preV - _verticalLargeDecrement < _ZERO ? _ZERO : preV - _verticalLargeDecrement;

            this.TestScroll(HScrollable, VScrollable,												//*Scrollable
                H, preV,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.LargeDecrement,		//ScrollAmount
                true,														//Expected return value from Scroll
                _NOSCROLL, VExpected,								//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);					//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.18</summary>
        [TestCaseAttribute("Scroll.S.2.18",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = False",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random vertical position preV between SmallAmount than 100",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = V where V =TSC_RandomPosition(0, SmallScrollAmount)",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (NoAmount,SmallDecrement) and verify that method returns True",
            "Verify that HorizontalPercent = NoScroll",
            "Verify that VerticalPercent = 0", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was true",
		})]
        public void TestScrollScrollS218(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = _NOSCROLL;
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _verticalSmallIncrement, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.SmallDecrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                _NOSCROLL, _ZERO,											//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.19</summary>
        [TestCaseAttribute("Scroll.S.2.19",
                Priority = TestPriorities.Pri0,
                Status = TestStatus.Works,
                Author = "Microsoft",
                TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
                Description = new string[] {
                "Precondition: Verify that the scroll thumb is < Page",
                "Precondition: Verify that HorizontalScrollable = False",
                "Precondition: Verify that VerticalScrollable = True",
                "Precondition: Get a random vertical position preV greater than (SmallAmount * 2)",
                "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
                "Step: Set HorizontalPercent = NoScroll",
                "Step: Verify that it was set correctly",
                "Step: Set VerticalPercent preV",
                "Step: Verify that it was set correctly",
                "Step: Add a property changed listener for HorizontalScrollPercentProperty",
                "Step: Add a property changed listener for VerticalScrollPercentProperty",
                "Call Scroll (NoAmount,SmallDecrement) and verify that method returns True",
                "Verify that HorizontalPercent = NoScroll",
                "Verify that VerticalPercent = 0<V<100", 
                "Step: Wait for events", 
                "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
                "Verify that the firing of the event listener VerticalScrollPercentProperty was true",
		})]
        public void TestScrollS219(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = _NOSCROLL;
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _verticalSmallIncrement + _verticalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);

            TSC_VerifyGreaterThanOnePage(ScrollDirection.Vertical, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.SmallDecrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                _NOSCROLL, EndLocation.Inbetween,											//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.2</summary>
        [TestCaseAttribute("Scroll.S.2.2",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = False",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random vertical position preV greater than SmallAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (NoAmount,SmallDecrement) and verify that method returns True",
            "Verify that HorizontalPercent = NoScroll",
            "Verify that VerticalPercent = preV - SmallScrollAmount", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was true",
		})]
        public void TestScrollS22(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = _NOSCROLL;
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _verticalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);

            double VExpected = V - _verticalSmallIncrement < _ZERO ? _ZERO : V - _verticalSmallIncrement;

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.SmallDecrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                _NOSCROLL, VExpected,											//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.2.1</summary>
        [TestCaseAttribute("Scroll.S.2.2.1",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,    
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = False",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random vertical position preV greater than or equal to (100 - LargeAmount) and less than 100",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (NoAmount,LargeIncrement) and verify that method returns True",
            "Verify that HorizontalPercent = NoScroll",
            "Verify that VerticalPercent = 100", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was true",
		})]
        public void TestScrollS221(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = _NOSCROLL;
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _HUNDRED - _verticalLargeIncrement, _HUNDRED, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.LargeIncrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                _NOSCROLL, _HUNDRED,											//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.22</summary>
        [TestCaseAttribute("Scroll.S.2.22",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that the scroll thumb is < Page",
            "Precondition: Verify that HorizontalScrollable = False",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random vertical position preV less than 100 - LargeAmount - SmallAmount)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (NoAmount,LargeIncrement) and verify that method returns True",
            "Verify that HorizontalPercent = NoScroll",
            "Verify that VerticalPercent = 0<V<100", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was true",
		})]
        public void TestScrollS222(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_verticalLargeIncrement + _verticalSmallIncrement > _HUNDRED)
                ThrowMe(CheckType.IncorrectElementConfiguration, "VLargeAmount == 100");

            double H = _NOSCROLL;
            double V = TSC_RandomPosition(ScrollDirection.Vertical, 0, _HUNDRED - (_verticalLargeIncrement + _verticalSmallIncrement), RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            TSC_VerifyGreaterThanOnePage(ScrollDirection.Vertical, CheckType.IncorrectElementConfiguration);
            this.TestScroll(HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.LargeIncrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                _NOSCROLL, EndLocation.Inbetween,											//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.23</summary>
        [TestCaseAttribute("Scroll.S.2.23",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = False",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random vertical position preV less than 100 - (LargeAmount + SmallAmount)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent =preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (NoAmount,LargeIncrement) and verify that method returns True",
            "Verify that HorizontalPercent = NoScroll",
            "Verify that VerticalPercent = V + LargeScrollAmount", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was true",
		})]
        public void TestScrollS223(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_verticalLargeIncrement + _verticalSmallIncrement > _HUNDRED)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            double H = _NOSCROLL;
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED - (_verticalLargeIncrement + _verticalSmallIncrement), RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            double VExpected = V + _verticalLargeIncrement; // > _HUNDRED ? _HUNDRED :  V + m_VFLargeAmount;

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.LargeIncrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                _NOSCROLL, VExpected,											//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.24</summary>
        [TestCaseAttribute("Scroll.S.2.24",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,    
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = False",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random vertical position preV less than 100 and greater than 100 - SmallAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (NoAmount,LargeIncrement) and verify that method returns True",
            "Verify that HorizontalPercent = NoScroll",
            "Verify that VerticalPercent = 100", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was true",
		})]
        public void TestScrollS224(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = _NOSCROLL;
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _HUNDRED - _verticalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.LargeIncrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                _NOSCROLL, _HUNDRED,											//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.25</summary>
        [TestCaseAttribute("Scroll.S.2.25",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that the scroll thumb is < Page",
            "Precondition: Verify that HorizontalScrollable = False",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random vertical position preV less than 100 - (LargeAmount + SmallAmount)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (NoAmount,LargeIncrement) and verify that method returns True",
            "Verify that HorizontalPercent = NoScroll",
            "Verify that VerticalPercent = 0<V<100", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was true",
		})]
        public void TestScrollS225(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;

            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_verticalLargeIncrement + _verticalSmallIncrement > _HUNDRED)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            double H = _NOSCROLL;
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED - (_verticalLargeIncrement + _verticalSmallIncrement), RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            TSC_VerifyGreaterThanOnePage(ScrollDirection.Vertical, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.LargeIncrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                _NOSCROLL, EndLocation.Inbetween,											//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.26</summary>
        [TestCaseAttribute("Scroll.S.2.26",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = False",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random vertical position preV less than 100 - LargeAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (NoAmount,LargeIncrement) and verify that method returns True",
            "Verify that HorizontalPercent = NoScroll",
            "Verify that VerticalPercent = V + SmallScrollAmount", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was Undetermined", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was true",
		})]
        public void TestScrollS226(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = false;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = _NOSCROLL;
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED - _verticalLargeIncrement, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            double VExpected = V + _verticalLargeIncrement > _HUNDRED ? _HUNDRED : V + _verticalLargeIncrement;

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.NoAmount, ScrollAmount.LargeIncrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                _NOSCROLL, VExpected,											//Expected *ScrollPercent after Scroll
                EventFired.Undetermined, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.27</summary>
        [TestCaseAttribute("Scroll.S.2.27",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH greater than 0 and less than or equal to LargeAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = H where H =TSC_RandomPosition(0, LargeScrollAmount)",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeDecrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = 0",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was true", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollS227(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, 0, _horizontalLargeIncrement, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeDecrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                _ZERO, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }
        ///<summary>TestCase: Scroll.S.2.28</summary>
        [TestCaseAttribute("Scroll.S.2.28",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
                "Precondition: Verify that HorizontalScrollable = True",
                "Precondition: Verify that VerticalScrollable = False",
                "Precondition: Get a random horizontal position preH greater than 0 and less than or equal to LargeAmount",
                "Precondition: Page is smaller than scrollable area",
                "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
                "Step: Set HorizontalPercent = prevH",
                "Step: Verify that it was set correctly",
                "Step: Set VerticalPercent = NoScroll",
                "Step: Verify that it was set correctly",
                "Step: Add a property changed listener for HorizontalScrollPercentProperty",
                "Step: Add a property changed listener for VerticalScrollPercentProperty",
                "Call SetScrollPercent (NoScroll,0) and verify that method returns true",
                "Verify that HorizontalPercent = 0<H<100",
                "Verify that VerticalPercent = NoScroll",
                "Step: Wait for events",
                "Verify that the firing of the event listener HorizontalScrollPercentProperty is true",
                "Verify that the firing of the event listener VerticalScrollPercentProperty is undetermines",
		})]
        public void TestScrollS228(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_horizontalLargeIncrement + _horizontalSmallIncrement > _HUNDRED)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _horizontalLargeIncrement + _horizontalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;

            TSC_VerifyGreaterThanOnePage(ScrollDirection.Horizontal, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeDecrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                EndLocation.Inbetween, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.29</summary>
        [TestCaseAttribute("Scroll.S.2.29",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH greater than LargeAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeDecrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = H - LargeScrollAmount",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was true", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollS229(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _horizontalLargeIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;

            double HExpected = H - _horizontalLargeIncrement < _ZERO ? _ZERO : H - _horizontalLargeIncrement;
            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeDecrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                HExpected, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.3</summary>
        [TestCaseAttribute("Scroll.S.2.3",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH greater than 0 and less than or equal to SmallAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = H where H =TSC_RandomPosition(0, SmallScrollAmount)",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (SmallDecrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = 0",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollS23(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _horizontalSmallDecrement, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.SmallDecrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                _ZERO, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.31</summary>
        [TestCaseAttribute("Scroll.S.2.31",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "IncorrectElementConfiguration: Verify that the Horizontal HSmallScrollAmount is less than ViewSize",
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH greater than LargeAmount + SmallAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (SmallDecrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = 0<H<100",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollS231(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _horizontalSmallIncrement + _horizontalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;

            this.TSC_VerifyGreaterThanOnePage(ScrollDirection.Horizontal, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.SmallDecrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                EndLocation.Inbetween, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.32</summary>
        [TestCaseAttribute("Scroll.S.2.32",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,     
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH greater SmallAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (SmallDecrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = H -SmallScrollAmount",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollS232(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _horizontalSmallDecrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;
            double HExpected = H - _horizontalSmallIncrement < _ZERO ? _ZERO : H - _horizontalSmallIncrement;

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.SmallDecrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                HExpected, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.33</summary>
        [TestCaseAttribute("Scroll.S.2.33",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,     
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH greater or equal to 100 - LargeAmount and less than 100",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeIncrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = 100",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollScrollS233(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _HUNDRED - _horizontalLargeIncrement, _HUNDRED, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;
            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeIncrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                _HUNDRED, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.34</summary>
        [TestCaseAttribute("Scroll.S.2.34",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that the scroll thumb is < Page",
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH less than 100 - (LaregAmount + SmallAmount)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeIncrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = 0<H<100",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollScrollS234(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_horizontalLargeIncrement + _horizontalSmallIncrement > _HUNDRED)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED - (_horizontalLargeIncrement + _horizontalSmallIncrement), RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;

            TSC_VerifyGreaterThanOnePage(ScrollDirection.Horizontal, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeIncrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                EndLocation.Inbetween, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.35</summary>
        [TestCaseAttribute("Scroll.S.2.35",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH less than 100 - LaregAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeIncrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = H + LargeScrollAmount",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollScrollS235(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED - _horizontalLargeIncrement, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;
            double HExpected = H + _horizontalLargeIncrement > _HUNDRED ? _HUNDRED : H + _horizontalLargeIncrement;
            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeIncrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                HExpected, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.36</summary>
        [TestCaseAttribute("Scroll.S.2.36",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,     
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH less than 100 and greater than 100 - SmallAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeIncrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = 100",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollScrollS236(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _HUNDRED - _horizontalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;
            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeIncrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                _HUNDRED, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.37</summary>
        [TestCaseAttribute("Scroll.S.2.37",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that the scroll thumb is < Page",
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH less than 100 - (LaregAmount + SmallAmount)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeIncrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = 0<H<100",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollScrollS237(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_horizontalLargeIncrement + _horizontalSmallIncrement > _HUNDRED)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED - (_horizontalLargeIncrement + _horizontalSmallIncrement), RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;

            TSC_VerifyGreaterThanOnePage(ScrollDirection.Horizontal, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeIncrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                EndLocation.Inbetween, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.38</summary>
        [TestCaseAttribute("Scroll.S.2.38",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = False",
            "Precondition: Get a random horizontal position preH less than 100 - LargeAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = H where H =TSC_RandomPosition(0, 100 - SmallScrollAmount)",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = NoScroll",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeIncrement,NoAmount) and verify that method returns True",
            "Verify that HorizontalPercent = H + SmallScrollAmount",
            "Verify that VerticalPercent = NoScroll", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was Undetermined",
		})]
        public void TestScrollScrollS238(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = false;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED - _horizontalLargeIncrement, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double V = _NOSCROLL;
            double HExpected = H + _horizontalLargeIncrement > _HUNDRED ? _HUNDRED : H + _horizontalLargeIncrement;
            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeIncrement, ScrollAmount.NoAmount,		//ScrollAmount
                true,													//Expected return value from Scroll
                HExpected, _NOSCROLL,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.Undetermined);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.55</summary>
        [TestCaseAttribute("Scroll.S.2.55",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that the scroll thumb is < Page",
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random horizontal position preH greater than LargeAmount + SmallAmount",
            "Precondition: Get a random vertical position preV less than 100 - (SmallAmount * 2)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeDecrement,ForwarBySmall) and verify that method returns True",
            "Verify that HorizontalPercent = 0<H<100",
            "Verify that VerticalPercent = 0<V<100", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was True",
		})]
        public void TestScrollScrollS255(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_horizontalLargeIncrement + _horizontalSmallIncrement > _HUNDRED || _verticalLargeIncrement + _verticalSmallIncrement > _HUNDRED)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _horizontalLargeIncrement + _horizontalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED - (_verticalSmallIncrement + _verticalSmallIncrement), RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            TSC_VerifyGreaterThanOnePage(ScrollDirection.Horizontal, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeDecrement, ScrollAmount.SmallIncrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                EndLocation.Inbetween, EndLocation.Inbetween,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.56</summary>
        [TestCaseAttribute("Scroll.S.2.56",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random horizontal position preH greater than LargeAmount",
            "Precondition: Get a random vertical position preV less than (100 - SmallAmount)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeDecrement,ForwarBySmall) and verify that method returns True",
            "Verify that HorizontalPercent = H - LargeScrollAmount",
            "Verify that VerticalPercent = V + SmallScrollAmount", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was True",
		})]
        public void TestScrollScrollS256(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _horizontalLargeIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED - _verticalSmallIncrement, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double HExpected = H - _horizontalLargeIncrement < _ZERO ? _ZERO : H - _horizontalLargeIncrement;
            double VExpected = V + _verticalSmallIncrement > _HUNDRED ? _HUNDRED : V + _verticalSmallIncrement;

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeDecrement, ScrollAmount.SmallIncrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                HExpected, VExpected,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.57</summary>
        [TestCaseAttribute("Scroll.S.2.57",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that the scroll thumb is < Page",
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random horizontal position preH greater than LargeAmount + SmallAmount)",
            "Precondition: Get a random vertical position preV less than 100 - (LargeAmount + SmallAmount)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (SmallDecrement,LargeIncrement) and verify that method returns True",
            "Verify that HorizontalPercent = 0<H<100",
            "Verify that VerticalPercent = 0<V<100", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was True",
		})]
        public void TestScrollScrollS257(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_horizontalLargeIncrement + _horizontalSmallIncrement > _HUNDRED || _verticalLargeIncrement + _verticalSmallIncrement > _HUNDRED || _verticalLargeIncrement == _ZERO)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _horizontalLargeIncrement + _horizontalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED - (_verticalLargeIncrement + _verticalSmallIncrement), RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);

            TSC_VerifyGreaterThanOnePage(ScrollDirection.Vertical, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.SmallDecrement, ScrollAmount.LargeIncrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                EndLocation.Inbetween, EndLocation.Inbetween,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.58</summary>
        [TestCaseAttribute("Scroll.S.2.58",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random horizontal position preH greater than SmallAmount",
            "Precondition: Get a random vertical position preV less than 100 - LargeAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (SmallDecrement,LargeIncrement) and verify that method returns True",
            "Verify that HorizontalPercent = H - SmallScrollAmount",
            "Verify that VerticalPercent = V + LargeScrollAmount", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was True",
		})]
        public void TestScrollScrollS258(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _horizontalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _ZERO, _HUNDRED - _verticalLargeIncrement, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double HExpected = H - _horizontalSmallIncrement < _ZERO ? _ZERO : H - _horizontalSmallIncrement;
            double VExpected = V + _verticalLargeIncrement > _HUNDRED ? _HUNDRED : V + _verticalLargeIncrement;

            if (_verticalLargeIncrement == _ZERO)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.SmallDecrement, ScrollAmount.LargeIncrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                HExpected, VExpected,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.59</summary>
        [TestCaseAttribute("Scroll.S.2.59",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that the scroll thumb is < Page",
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random horizontal position preH less than 100 - (LargeAmount + SmallAmount)",
            "Precondition: Get a random vertical position preV greater (SmallAmount * 2)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeIncrement,SmallDecrement) and verify that method returns True",
            "Verify that HorizontalPercent = 0<H<100",
            "Verify that VerticalPercent = 0<V<100", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was True",
		})]
        public void TestScrollScrollS259(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_horizontalLargeIncrement + _horizontalSmallIncrement > _HUNDRED || _verticalLargeIncrement + _verticalSmallIncrement > _HUNDRED)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED - (_horizontalLargeIncrement + _horizontalSmallIncrement), RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _verticalSmallIncrement + _verticalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);

            TSC_VerifyGreaterThanOnePage(ScrollDirection.Horizontal, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeIncrement, ScrollAmount.SmallDecrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                EndLocation.Inbetween, EndLocation.Inbetween,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.6</summary>
        [TestCaseAttribute("Scroll.S.2.6",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random horizontal position preH less than 100 - (LargeAmount + SmallAmount)",
            "Precondition: Get a random vertical position preV greater (SmallAmount * 2)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeIncrement,SmallDecrement) and verify that method returns True",
            "Verify that HorizontalPercent = H + LargeScrollAmount",
            "Verify that VerticalPercent = V - SmallScrollAmount", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was True",
		})]
        public void TestScrollScrollS26(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_horizontalLargeIncrement + _horizontalSmallIncrement > _HUNDRED || _verticalLargeIncrement + _verticalSmallIncrement > _HUNDRED)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED - (_horizontalLargeIncrement + _horizontalSmallIncrement), RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _verticalSmallIncrement + _verticalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double HExpected = H + _horizontalLargeIncrement > _HUNDRED ? _HUNDRED : H + _horizontalLargeIncrement;
            double VExpected = V - _verticalSmallIncrement < _ZERO ? _ZERO : V - _verticalSmallIncrement;
            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeIncrement, ScrollAmount.SmallDecrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                HExpected, VExpected,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.61</summary>
        [TestCaseAttribute("Scroll.S.2.61",
            Priority = TestPriorities.Pri0, /* Pri0 */
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that the scroll thumb is < Page",
            "Precondition: Verify that the scroll thumb is < Page",
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random horizontal position preH less than 100 - (LargeAmount + SmallAmount)",
            "Precondition: Get a random vertical position preV greater (LargeAmount + SmallAmount)",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeIncrement,LargeDecrement) and verify that method returns True",
            "Verify that HorizontalPercent = 0<H<100",
            "Verify that VerticalPercent = 0<V<100", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was True",
		})]
        public void TestScrollScrollS261(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            if (_verticalLargeIncrement + _verticalSmallIncrement > _HUNDRED || _verticalLargeIncrement + _verticalSmallIncrement > _HUNDRED || _verticalLargeIncrement == _ZERO)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED - (_horizontalLargeIncrement + _horizontalSmallIncrement), RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _verticalLargeIncrement + _verticalSmallIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);

            TSC_VerifyGreaterThanOnePage(ScrollDirection.Horizontal, CheckType.IncorrectElementConfiguration);
            TSC_VerifyGreaterThanOnePage(ScrollDirection.Vertical, CheckType.IncorrectElementConfiguration);

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeIncrement, ScrollAmount.LargeDecrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                EndLocation.Inbetween, EndLocation.Inbetween,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.True);				//Event Fired
        }

        ///<summary>TestCase: Scroll.S.2.62</summary>
        [TestCaseAttribute("Scroll.S.2.62",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, EventTested = "AutomationPropertyChangedEventHandler(ScrollPattern.HorizontalScrollPercentProperty)/AutomationPropertyChangedEventHandler(ScrollPattern.VerticalScrollPercentProperty)",
            Description = new string[] {
            "Precondition: Verify that HorizontalScrollable = True",
            "Precondition: Verify that VerticalScrollable = True",
            "Precondition: Get a random horizontal position preH less than 100 - LargeAmount",
            "Precondition: Get a random vertical position preV greater than LargeAmount",
            "Precondition: Verify that the ScrollAmount values are valid for this AutomationElement (ie. Tab does not support ScrollAmount.LargeIncrement or ScrollAmount.LargeDecrement)",
            "Step: Set HorizontalPercent = preH",
            "Step: Verify that it was set correctly",
            "Step: Set VerticalPercent = preV",
            "Step: Verify that it was set correctly",
            "Step: Add a property changed listener for HorizontalScrollPercentProperty",
            "Step: Add a property changed listener for VerticalScrollPercentProperty",
            "Call Scroll (LargeIncrement,LargeDecrement) and verify that method returns True",
            "Verify that HorizontalPercent = H + SmallScrollAmount",
            "Verify that VerticalPercent = V - LargeScrollAmount", 
            "Step: Wait for events", 
            "Verify that the firing of the event listener HorizontalScrollPercentProperty was True", 
            "Verify that the firing of the event listener VerticalScrollPercentProperty was True",
		})]
        public void TestScrollScrollS262(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            bool HScrollable = true;
            bool VScrollable = true;
            TSC_VerifyScrollableDirections(HScrollable, VScrollable);

            double H = TSC_RandomPosition(ScrollDirection.Horizontal, _ZERO, _HUNDRED - _horizontalLargeIncrement, RandomBoundaries.CanIncludeLower, true, CheckType.IncorrectElementConfiguration);
            double V = TSC_RandomPosition(ScrollDirection.Vertical, _verticalLargeIncrement, _HUNDRED, RandomBoundaries.CanIncludeUpper, true, CheckType.IncorrectElementConfiguration);
            double HExpected = H + _horizontalLargeIncrement > _HUNDRED ? _HUNDRED : H + _horizontalLargeIncrement;
            double VExpected = V - _verticalLargeIncrement < _ZERO ? _ZERO : V - _verticalLargeIncrement;

            if (_verticalLargeIncrement == _ZERO)
                ThrowMe(CheckType.IncorrectElementConfiguration, "LargeAmount == 100");

            this.TestScroll(
                HScrollable, VScrollable,												//*Scrollable
                H, V,														//*InitialPercent
                ScrollAmount.LargeIncrement, ScrollAmount.LargeDecrement,		//ScrollAmount
                true,													//Expected return value from Scroll
                HExpected, VExpected,											//Expected *ScrollPercent after Scroll
                EventFired.True, EventFired.True);				//Event Fired
        }

        #endregion Scroll

        #region HorizontalScrollPercentProperty S.4

        ///<summary>TestCase: HorizontalScrollPercentProperty.S.4.1</summary>
        [TestCaseAttribute("HorizontalScrollPercentProperty.S.4.1",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: HorizontalScrollable = false",
            "Verify: HorizontalScrollPercent = NoScroll"
            }
             )]
        public void TestHorizontalScrollPercentProperty41(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyScrollable(ScrollDirection.Horizontal, false, CheckType.IncorrectElementConfiguration);
            TSC_VerifyScrollPercent(_NOSCROLL, ScrollDirection.Horizontal, CheckType.Verification);
        }

        ///<summary>TestCase: HorizontalScrollPercentProperty.S.4.2</summary>
        [TestCaseAttribute("HorizontalScrollPercentProperty.S.4.2",
            Priority = TestPriorities.Pri0,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: HorizontalViewSize = 100",
            "Verify: HorizontalScrollPercent = NoScroll"
            }
             )]
        public void TestHorizontalScrollPercentProperty42(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            TSC_VerifyViewSize(ScrollDirection.Horizontal, CodeBinaryOperatorType.IdentityEquality, _HUNDRED, CheckType.Verification);
            TSC_VerifyScrollPercent(_NOSCROLL, ScrollDirection.Horizontal, CheckType.Verification);
        }

        ///<summary>TestCase: HorizontalScrollPercentProperty.S.4.3</summary>
        [TestCaseAttribute("HorizontalScrollPercentProperty.S.4.3",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: HorizontalScrollable = true",
            "Precondition: HorizontalScrollPercent = far left",
            "Verify: HorizontalScrollPercent = 0"
            }
            )]
        public void TestHorizontalScrollPercentProperty43(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyScrollable(ScrollDirection.Horizontal, true, CheckType.IncorrectElementConfiguration);
            TSC_ScrollPercent(_ZERO, ScrollDirection.Horizontal, CheckType.Verification);
            TSC_VerifyScrollPercent(_ZERO, ScrollDirection.Horizontal, CheckType.Verification);
        }

        ///<summary>TestCase: HorizontalScrollPercentProperty.S.4.4</summary>
        [TestCaseAttribute("HorizontalScrollPercentProperty.S.4.4",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: HorizontalScrollable = true",
            "Precondition: HorizontalScrollPercent = far right",
            "Verify: HorizontalScrollPercent = 100"
            }
            )]
        public void TestHorizontalScrollPercentProperty44(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyScrollable(ScrollDirection.Horizontal, true, CheckType.IncorrectElementConfiguration);
            TSC_ScrollPercent(_HUNDRED, ScrollDirection.Horizontal, CheckType.IncorrectElementConfiguration);
            TSC_VerifyScrollPercent(_HUNDRED, ScrollDirection.Horizontal, CheckType.Verification);
        }

        #endregion HorizontalScrollPercentProperty

        #region VerticalScrollPercentProperty S.5

        ///<summary>TestCase: VerticalScrollPercentProperty.S.5.1</summary>
        [TestCaseAttribute("VerticalScrollPercentProperty.S.5.1",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: VerticalScrollable = false",
            "Verify: VerticalScrollPercent = NoScroll"
            }
            )]
        public void TestVerticalScrollPercentProperty51(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            TSC_VerifyScrollable(ScrollDirection.Vertical, false, CheckType.IncorrectElementConfiguration);
            TSC_VerifyScrollPercent(_NOSCROLL, ScrollDirection.Vertical, CheckType.Verification);
        }

        ///<summary>TestCase: VerticalScrollPercentProperty.S.5.2</summary>
        [TestCaseAttribute("VerticalScrollPercentProperty.S.5.2",
            Priority = TestPriorities.Pri0,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: pattern_getVerticalViewSize = 100",
            "Verify: VerticalScrollPercent = NoScroll"
            }
            )]
        public void TestVerticalScrollPercentProperty52(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            TSC_VerifyViewSize(ScrollDirection.Vertical, CodeBinaryOperatorType.IdentityEquality, _HUNDRED, CheckType.IncorrectElementConfiguration);
            TSC_VerifyScrollPercent(_NOSCROLL, ScrollDirection.Vertical, CheckType.Verification);
        }

        ///<summary>TestCase: VerticalScrollPercentProperty.S.5.3</summary>
        [TestCaseAttribute("VerticalScrollPercentProperty.S.5.3",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: VerticalScrollable = true",
            "Precondition: VerticalScrollPercent = top",
            "Verify: VerticalScrollPercent = 0"
            }
             )]
        public void TestVerticalScrollPercentProperty53(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyScrollable(ScrollDirection.Vertical, true, CheckType.IncorrectElementConfiguration);
            TSC_ScrollPercent(_ZERO, ScrollDirection.Vertical, CheckType.Verification);
            TSC_VerifyScrollPercent(_ZERO, ScrollDirection.Vertical, CheckType.Verification);
        }

        ///<summary>TestCase: VerticalScrollPercentProperty.S.5.4</summary>
        [TestCaseAttribute("VerticalScrollPercentProperty.S.5.4",
            Priority = TestPriorities.Pri0,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: VerticalScrollable = true",
            "Verify: Set VerticalScrollPercent = 100",
            "Verify: VerticalScrollPercent = 100"
            }
            )]
        public void TestVerticalScrollPercentProperty54(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyScrollable(ScrollDirection.Vertical, true, CheckType.IncorrectElementConfiguration);
            TSC_ScrollPercent(_HUNDRED, ScrollDirection.Vertical, CheckType.Verification);
            TSC_VerifyScrollPercent(_HUNDRED, ScrollDirection.Vertical, CheckType.Verification);
        }
        #endregion

        #region HorizontallyScrollableProperty S.6

        ///<summary>TestCase: HorizontallyScrollableProperty.S.6.1</summary>
        [TestCaseAttribute("HorizontallyScrollableProperty.S.6.1",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: HorizontalScrollable = false", 
            "Precondition: HorizontalViewSize = 100",
            "Verify: HorizontalScrollable = false"
            }
            )]
        public void TestHorizontallyScrollableProperty61(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyScrollable(ScrollDirection.Horizontal, false, CheckType.IncorrectElementConfiguration);
            TSC_VerifyViewSize(ScrollDirection.Horizontal, CodeBinaryOperatorType.IdentityEquality, _HUNDRED, CheckType.IncorrectElementConfiguration);
            TSC_VerifyScrollable(ScrollDirection.Horizontal, false, CheckType.Verification);
        }

        ///<summary>TestCase: HorizontallyScrollableProperty.S.6.2</summary>
        [TestCaseAttribute("HorizontallyScrollableProperty.S.6.2",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: HorizontalScrollable = true",
            "Precondition: 0 < HorizontalViewSize",
            "Precondition: HorizontalViewSize < 100",
            "Verify: HorizontalScrollable = true"
            }
            )]
        public void TestHorizontallyScrollableProperty62(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyScrollable(ScrollDirection.Horizontal, true, CheckType.IncorrectElementConfiguration);
            TSC_VerifyViewSize(ScrollDirection.Horizontal, CodeBinaryOperatorType.GreaterThan, _ZERO, CheckType.IncorrectElementConfiguration);
            TSC_VerifyViewSize(ScrollDirection.Horizontal, CodeBinaryOperatorType.LessThan, _HUNDRED, CheckType.IncorrectElementConfiguration);
            TSC_VerifyScrollable(ScrollDirection.Horizontal, true, CheckType.Verification);
        }

        ///<summary>TestCase: HorizontallyScrollableProperty.S.6.6</summary>
        [TestCaseAttribute("HorizontallyScrollableProperty.S.6.6",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Problem,  
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: HorizontalViewSize = 0",
            "Verify: HorizontalScrollable = false"
            }
            )]
        public void TestHorizontallyScrollableProperty66(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyViewSize(ScrollDirection.Horizontal, CodeBinaryOperatorType.IdentityEquality, _ZERO, CheckType.IncorrectElementConfiguration);
            TSC_VerifyScrollable(ScrollDirection.Horizontal, false, CheckType.Verification);
        }

        #endregion HorizontallyScrollableProperty

        #region VerticallyScrollableProperty S.7

        ///<summary>TestCase: VerticallyScrollableProperty.S.7.1</summary>
        [TestCaseAttribute("VerticallyScrollableProperty.S.7.1",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: pattern_getVerticalViewSize = false",
            "Precondition: pattern_getVerticalViewSize = 100",
            "Verify: VerticalScrollable = false"
            }
            )]
        public void TestVerticallyScrollableProperty1(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyScrollable(ScrollDirection.Vertical, false, CheckType.IncorrectElementConfiguration);
            TSC_VerifyViewSize(ScrollDirection.Vertical, CodeBinaryOperatorType.IdentityEquality, _HUNDRED, CheckType.IncorrectElementConfiguration);
            TSC_VerifyScrollable(ScrollDirection.Vertical, false, CheckType.Verification);
        }

        ///<summary>TestCase: VerticallyScrollableProperty.S.7.2</summary>
        [TestCaseAttribute("VerticallyScrollableProperty.S.7.2",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: pattern_getVerticalViewSize = true",
            "Precondition: 0<pattern_getVerticalViewSize<100",
            "Verify: VerticalScrollable = true"
            }
            )]
        public void TestVerticallyScrollableProperty2(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyScrollable(ScrollDirection.Vertical, true, CheckType.IncorrectElementConfiguration);

            if (!(pattern_getVerticalViewSize < _HUNDRED))
                ThrowMe(CheckType.IncorrectElementConfiguration, "pattern_getVerticalViewSize = " + pattern_getVerticalViewSize);

            m_TestStep++;
            this.TSC_VerifyScrollable(ScrollDirection.Vertical, true, CheckType.Verification);
        }

        ///<summary>TestCase: VerticallyScrollableProperty.S.7.6</summary>
        [TestCaseAttribute("VerticallyScrollableProperty.S.7.6",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Problem,  
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: pattern_getVerticalViewSize = 0",
            "Verify: VerticalScrollable = false"
            }
            )]
        public void TestVerticallyScrollableProperty6(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            // 
            if (pattern_getVerticalViewSize != _ZERO)
                ThrowMe(CheckType.IncorrectElementConfiguration, "pattern_getVerticalViewSize = " + pattern_getVerticalViewSize);

            m_TestStep++;
            this.TSC_VerifyScrollable(ScrollDirection.Vertical, false, CheckType.Verification);
        }

        #endregion VerticallyScrollableProperty

        #region HorizontalViewSizeProperty S.8

        ///<summary>TestCase: HorizontalViewSizeProperty.S.8.3</summary>
        [TestCaseAttribute("HorizontalViewSizeProperty.S.8.3",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]
            {
            "Precondition: HorizontalScrollable = false",
            "Verify: HorizontalViewSize = 100"
            }
            )]
        public void TestHorizontalViewSizeProperty3(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);
            TSC_VerifyScrollable(ScrollDirection.Horizontal, false, CheckType.IncorrectElementConfiguration);
            TSC_VerifyViewSize(ScrollDirection.Horizontal, CodeBinaryOperatorType.IdentityEquality, _HUNDRED, CheckType.Verification);
        }
        #endregion HorizontalViewSizeProperty

        #region patternVerticalViewSizeProperty S.9
        #endregion patternVerticalViewSizeProperty

        #region getScrollable

        ///<summary>TestCase: Scrollable.S.1.1</summary>
        [TestCaseAttribute("Scrollable.S.1.1",
            TestSummary = "If Vertically/HorizontallyScrollable = true, then Vertically/HorizontalScrollPercent >= 0",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
            "verify: If HorizontallyScrollable = true, then HorizontalScrollPercent >= 0", 
            "Verify: If VerticallyScrollable = true, then VerticalScrollPercent >= 0", 
            "Verify: If HorizontalScrollable = false, then HorizontalScrollPercent == -1", 
            "Verify: If VerticallyScrollable = false, then VerticalScrollPercent == -1"
            })]
        public void Scrollable1(TestCaseAttribute testCaseAttribue)
        {
            HeaderComment(testCaseAttribue);

            //"Verify: If HorizontallyScrollable = true, then HorizontalScrollPercent >= 0", 
            // 
            if (pattern_getHorizontallyScrollable.Equals(true))
                if (pattern_getHorizontalScrollPercent < _ZERO)
                    ThrowMe(CheckType.Verification, "HorizontallyScrollable == true, but HorizontalScrollPercent < 0");

            m_TestStep++;

            //"Verify: If VerticallyScrollable = false, then VerticalScrollPercent >= 0", 
            // 
            if (pattern_getVerticallyScrollable.Equals(true))
                if (pattern_getVerticalScrollPercent < _ZERO)
                    ThrowMe(CheckType.Verification, "VerticallyScrollable == true, but VerticalScrollPercent < 0");

            m_TestStep++;

            //"Verify: If HorizontalScrollable = true, then HorizontalScrollPercent == -1", 
            // 
            if (pattern_getHorizontallyScrollable.Equals(false))
                if (!pattern_getHorizontalScrollPercent.Equals(_NEGONE))
                    ThrowMe(CheckType.Verification, "HorizontalScrollable == false, but HorizontalScrollPercent != -1");

            m_TestStep++;

            //"Verify: If VerticallyScrollable = true, then VerticalScrollPercent == -1"
            // 
            if (pattern_getVerticallyScrollable.Equals(false))
                if (!pattern_getVerticalScrollPercent.Equals(_NEGONE))
                    ThrowMe(CheckType.Verification, "VerticallyScrollable == false, but VerticalScrollPercent != -1");
            m_TestStep++;

        }

        #endregion getScrollable

        #region Not Sure

        void TSC_VerifyScrollable(ScrollDirection direction, bool IsScrollable, CheckType ct)
        {
            switch (direction)
            {
                case ScrollDirection.Horizontal:
                    {
                        Comment("HorizontallyScrollable = " + pattern_getHorizontallyScrollable);
                        if (pattern_getHorizontallyScrollable != IsScrollable)
                            ThrowMe(ct);
                        break;
                    }
                case ScrollDirection.Vertical:
                    {
                        Comment("VerticallyScrollable = " + pattern_getVerticallyScrollable);
                        if (pattern_getVerticallyScrollable != IsScrollable)
                            ThrowMe(ct);
                        break;
                    }
                default:
                    throw new Exception("Invalid argument");
            }
            m_TestStep++;
        }

        void TSC_VerifyGreaterThanOnePage(ScrollDirection direction, CheckType ct)
        {

            switch (direction)
            {
                case ScrollDirection.Horizontal:
                    {
                        if (pattern_getHorizontallyScrollable.Equals(false) || _horizontalLargeIncrement == _HUNDRED)
                            ThrowMe(ct, "Page is smaller than scrollable area");
                        break;
                    }

                case ScrollDirection.Vertical:
                    {
                        if (pattern_getVerticallyScrollable.Equals(false) || _verticalLargeIncrement == _HUNDRED)
                            ThrowMe(ct, "Page is smaller than scrollable area");
                        break;
                    }

                default:
                    throw new Exception("Invalid argument");
            }
            Comment("ScrollPage = " + _horizontalLargeIncrement);
            m_TestStep++;
        }

        void TSC_ScrollPercent(double position, ScrollDirection direction, CheckType checkType)
        {
            switch (direction)
            {
                case ScrollDirection.Horizontal:
                    // WCP: HorizontalScrollPercent = position;
                    pattern_SetScrollPercent(position, _NEGONE, null, checkType);
                    Comment("Assign HorizontalScrollPercent = " + position);
                    break;
                case ScrollDirection.Vertical:
                    // WCP: VerticalScrollPercent = position;
                    pattern_SetScrollPercent(_NEGONE, position, null, checkType);
                    Comment("Assign VerticalScrollPercent = " + position);
                    break;
                default:
                    throw new Exception("Invalid argument");
            }
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TSC_SetScrollPercent(double horizontalPercent, double verticalPercent, Type exceptionExpected, CheckType checkType)
        {
            pattern_SetScrollPercent(horizontalPercent, verticalPercent, exceptionExpected, checkType);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Some controls don't scroll correctly.  For these known controls, set 
        /// the accuracy level to 2.0F, for other controls, set the accuracy to 
        /// 0.005F.
        /// </summary>
        /// -------------------------------------------------------------------
        double GetScrollAccuracy(string className)
        {
            string [] badScrollers = new string []{"richedit","syslistview32"};
            className = className.ToLower(CultureInfo.InvariantCulture);

            foreach(string bad in badScrollers)
            {
                if (className.IndexOf(bad) != -1)
                    return 2.0F;
            }

            return 0.005F;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TSC_VerifyScrollPercent(object ExpectedScrollPos, ScrollDirection scrollDirection, CheckType ct)
        {
            double ACCURACY = GetScrollAccuracy(m_le.Current.ClassName);

            object val;
            double SmallAmount = 1;

            //which property value are we getting
            switch (scrollDirection)
            {
                case ScrollDirection.Horizontal:
                    val = pattern_getHorizontalScrollPercent;
                    SmallAmount = this._horizontalSmallIncrement;
                    Comment("Current HorizontalScrollPercent = " + pattern_getHorizontalScrollPercent);
                    break;
                case ScrollDirection.Vertical:
                    val = pattern_getVerticalScrollPercent;
                    SmallAmount = this._verticalSmallIncrement;
                    Comment("Current VerticalScrollPercent = " + pattern_getVerticalScrollPercent);
                    break;
                default:
                    throw new Exception("Invalid argument");
            }

            if (ExpectedScrollPos is double | ExpectedScrollPos is double | ExpectedScrollPos is int)
            {
                string throwComment = "ScrollPos should have been " + ExpectedScrollPos + " but was not";

                // truncate up to the third digit after the decimal
                if (Math.Abs((double)val - (double)ExpectedScrollPos) > ACCURACY)
                    ThrowMe(ct, "ScrollPos should have been " + ExpectedScrollPos + " but was " + val);
            }
            else
            {
                string throwComment;

                switch ((EndLocation)ExpectedScrollPos)
                {
                    case EndLocation.Hundred:
                        {
                            throwComment = "ScrollPos should have been 100 but is " + val + " instead";

                            if (!val.Equals(_HUNDRED))
                                ThrowMe(ct, throwComment);

                            break;
                        }
                    case EndLocation.Inbetween:
                        {
                            throwComment = "ScrollPos should have been between 0 and 100 but was " + val + " instead";

                            if (((double)val == _HUNDRED) || ((double)val == _ZERO))
                                ThrowMe(ct, throwComment);

                            break;
                        }
                    case EndLocation.Zero:
                        {
                            throwComment = "ScrollPos should have been 0 but is " + val + " instead";

                            if (!val.Equals(_ZERO))
                                ThrowMe(ct, throwComment);

                            break;
                        }
                    default:
                        throw new NotImplementedException();

                }

            }
            m_TestStep++;
        }

        void TSC_VerifyViewSize(ScrollDirection direction, CodeBinaryOperatorType codeType, double CompareValue, CheckType ct)
        {
            double viewSize = 0;
            switch (direction)
            {
                case ScrollDirection.Horizontal:
                    {
                        viewSize = pattern_getHorizontalViewSize;
                        Comment("Current HorizontalViewSize = " + viewSize + "(" + viewSize.GetType().ToString() + ")");
                        switch (codeType)
                        {
                            case CodeBinaryOperatorType.GreaterThan:
                                {
                                    if (viewSize <= CompareValue)
                                        ThrowMe(ct);

                                    break;
                                }

                            case CodeBinaryOperatorType.LessThan:
                                {
                                    if (viewSize >= CompareValue)
                                        ThrowMe(ct);
                                    break;
                                }

                            case CodeBinaryOperatorType.IdentityEquality:
                                {
                                    if (Math.Abs(viewSize - CompareValue) > _ACCURACY)
                                        ThrowMe(ct);
                                    break;
                                }

                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                    break;

                case ScrollDirection.Vertical:
                    {
                        viewSize = pattern_getVerticalViewSize;
                        Comment("Current VerticalViewSize = " + viewSize + "(" + viewSize.GetType().ToString() + ")");
                        switch (codeType)
                        {
                            case CodeBinaryOperatorType.GreaterThan:
                                {
                                    if (viewSize <= CompareValue)
                                        ThrowMe(ct);

                                    break;
                                }

                            case CodeBinaryOperatorType.LessThan:
                                {
                                    if (viewSize >= CompareValue)
                                        ThrowMe(ct);

                                    break;
                                }

                            case CodeBinaryOperatorType.IdentityEquality:
                                {
                                    if (Math.Abs(viewSize - CompareValue) > _ACCURACY)
                                        ThrowMe(ct);
                                    break;
                                }

                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                    break;
            }
            m_TestStep++;
        }

        void TSC_Scroll(ScrollAmount scrollHAmount, ScrollAmount scrollVAmount, CheckType checkType)
        {
            pattern_Scroll(scrollHAmount, scrollVAmount, null, checkType);
            m_TestStep++;
        }

        #endregion

        #endregion Tests

        #region Verification

        void TSC_VerifyScrollableDirections(bool HScrollable, bool VScrollable)
        {
            Comment("---------------");
            Comment("HorizontalForwardLargeAmount = " + _horizontalLargeIncrement);
            Comment("HorizontalBackLargeAmount = " + _horizontalLargeDecrement);
            Comment("HorizontalForwardSmallAmount = " + _horizontalSmallIncrement);
            Comment("HorizontalBackSmallAmount = " + _horizontalSmallDecrement);
            Comment("---------------");
            Comment("VerticalForwardLargeAmount = " + _verticalLargeIncrement);
            Comment("VerticalBackLargeAmount = " + _verticalLargeDecrement);
            Comment("VerticalForwardSmallAmount = " + _verticalSmallIncrement);
            Comment("VerticalBackSmallAmount = " + _verticalSmallDecrement);
            Comment("---------------");
            Comment("Valid horizontal positions: " + _validHPositions.Count);
            Comment("Valid vertical positions: " + _validVPositions.Count);
            Comment("---------------");

            //0 "Precondition: Verify that HorizontalScrollable = false",
            TSC_VerifyScrollable(ScrollDirection.Horizontal, HScrollable, CheckType.IncorrectElementConfiguration);

            //1 "Precondition: Verify that VerticalScrollable = true",
            TSC_VerifyScrollable(ScrollDirection.Vertical, VScrollable, CheckType.IncorrectElementConfiguration);
        }

        /// -------------------------------------------------------------------
        /// <summary>Some controls do not support this method of scrolling</summary>
        /// -------------------------------------------------------------------
        private void TS_ControlsSupportsLargeIncrementOrDecrement(AutomationElement le, ScrollAmount arg_HScrollAmount, ScrollAmount arg_VScrollAmount, CheckType checkType)
        {
            if (le.Current.ControlType == ControlType.Tab && (arg_HScrollAmount == ScrollAmount.LargeDecrement || arg_HScrollAmount == ScrollAmount.LargeIncrement))
                ThrowMe(checkType, le.Current.ControlType.ProgrammaticName + " does not support LargeIncrement or LargeDecrement");

            m_TestStep++;
        }

        #endregion Verification

        /// -------------------------------------------------------------------
        /// <summary>Changes the position to a new random position if the two are the same</summary>
        /// -------------------------------------------------------------------
        private void VerifyNotSame(ref double positionToChange, string positionToChangeStr, double otherPosition, string otherPositionStr, ArrayList validPositions, ScrollDirection scrollDirection, double min, double max, RandomBoundaries boundaries, CheckType checkType)
        {
            if (positionToChange.Equals(otherPosition))  
            {
                if (validPositions.Count < 4)
                    ThrowMe(CheckType.IncorrectElementConfiguration, validPositions.Count.ToString() + " valid scroll positions, not enough for test (4 minimum). Is your display resolution set too low?");

                for (int i = 0; i < 10 && (positionToChange.Equals(otherPosition)); i++)
                {
                    Comment("\"{0}\"({1}) and \"{2}\"({3}) were found to be the same. Find a new \"{0}\"", positionToChange, positionToChangeStr, otherPosition, otherPositionStr);

                    // false says don't incremenet m_TestStep TSC_RandomPosition
                    positionToChange = TSC_RandomPosition(scrollDirection, min, max, boundaries, false, checkType);
                }
                if (positionToChange.Equals(otherPosition))
                    ThrowMe(CheckType.Verification, "Cannot find a random \"{0}\" position", positionToChangeStr);

                Comment("Found new \"{0}\" position : {1}", positionToChangeStr, positionToChange);
            }
        }
    }
}
