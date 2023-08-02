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

    enum EventHappened
    {
        Yes,
        No,
        Undetermined
    }

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class WindowPatternWrapper : PatternObject
    {
        #region Variables
        WindowPattern _pattern = null;
        #endregion

        #region constructor

        /// <summary></summary>
        protected WindowPatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _pattern = (WindowPattern)GetPattern(m_le, m_useCurrent, WindowPattern.Pattern);
        }

        #endregion Constructor

        #region Properties

        internal WindowInteractionState pattern_getWindowInteractionState
        {
            get
            {
                if (m_useCurrent == true)
                    return _pattern.Current.WindowInteractionState;
                else
                    return _pattern.Cached.WindowInteractionState;
            }
        }

        internal WindowVisualState pattern_getVisualState
        {
            get
            {
                if (m_useCurrent == true)
                    return _pattern.Current.WindowVisualState;
                else
                    return _pattern.Cached.WindowVisualState;
            }
        }

        internal bool pattern_getCanMaximize
        {
            get
            {
                if (m_useCurrent == true)
                    return _pattern.Current.CanMaximize;
                else
                    return _pattern.Cached.CanMaximize;
            }
        }
        internal bool pattern_getCanMinimize
        {
            get
            {
                if (m_useCurrent)
                    return _pattern.Current.CanMinimize;
                else
                    return _pattern.Cached.CanMinimize;
            }
        }

        internal bool pattern_getIsModal
        {
            get
            {
                if (m_useCurrent)
                    return _pattern.Current.IsModal;
                else
                    return _pattern.Cached.IsModal;

            }
        }

        #endregion Properties

        #region Methods

        internal void pattern_SetWindowVisualState(WindowVisualState state, Type expectedException, CheckType checkType)
        {
            string call = "SetWindowVisualState(" + state + ")";
            try
            {
                _pattern.SetWindowVisualState(state);
            }
            catch (Exception actualException)
            {
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
	public sealed class WindowTests : WindowPatternWrapper
    {
        #region Member variables

        const string THIS = "WindowTests";

        /// <summary></summary>
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// <summary>Defines which UIAutomation Pattern this tests</summary>
        public static readonly string TestWhichPattern = Automation.PatternName(WindowPattern.Pattern);

        #endregion

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public WindowTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.Window, dirResults, testEvents, commands)
        {
        }

        #region WindowPattern.SetWindowState

        //		/// -------------------------------------------------------------------
        //		/// <summary>
        //		/// Verifies that the interaction state is correct
        //		/// </summary>
        //		/// <param name="state"></param>
        //		/// <param name="checkType"></param>
        //		/// -------------------------------------------------------------------
        //		void TS_VerifyInteractionState (int state, CheckType checkType)
        //		{
        //			int good = (int)pattern_getWindowInteractionState & state;
        //			if (!good.Equals(0))
        //			{
        //				m_TestStep++;
        //			}
        //			else
        //			{
        //				ThrowMe ("InteractioState == " + pattern_getWindowInteractionState, checkType);
        //			}
        //		}


        #endregion WindowPattern.SetWindowState

        #region WindowPattern.MaximizableProperty

        ///<summary>TestCase: Window.MaximizableProperty.S.6.1</summary>
        [TestCaseAttribute("Window.MaximizableProperty.S.6.1",
             Priority = TestPriorities.Pri1,
             TestCaseType = TestCaseType.Generic,
             Status = TestStatus.Works,
             Author = "Microsoft",
             Description = new string[]{
											"Precondition: Verify that MaximizableProperty is true",
											"Verification: That you can set the VisualState property to WindowVisualState.Maximizable",
											"Verification: That the WindowVisualState is WindowVisualState.Maximizable after setting the property"
										}
             )]
        public void TestMaximizeS61(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            // "Verify that MinimizableProperty is true",
            TSC_VerifyProperty(pattern_getCanMaximize, true, "Maximizable", CheckType.IncorrectElementConfiguration);

            // "Verify that you can set the VisualState property to WindowVisualState.Maximizable",
            TS_VerifySetVisualState(WindowVisualState.Maximized, null, CheckType.Verification);

            // "Verify that the WindowVisualState is WindowVisualState.Maximizable after setting the property"
            TS_VerifyVisualState(WindowVisualState.Maximized, true, CheckType.Verification);
        }

        ///<summary>TestCase: Window.MaximizableProperty.S.6.2</summary>
        [TestCaseAttribute("Window.MaximizableProperty.S.6.2",
            Priority = TestPriorities.Pri1,
            TestCaseType = TestCaseType.Generic,
            Status = TestStatus.Works,
            Author = "Microsoft",
             Description = new string[]{
											"Precondition: Maximizable property is false",
											"Verify that setting the VisualState property to WindowVisualState.Maximizable returns false",
											"Verify that the WindowVisualState is not WindowVisualState.Maximizable after setting the property"
										}
             )]
        public void TestMaximizeS62(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            // "Precondition: Maximizable property is false",
            TSC_VerifyProperty(pattern_getCanMaximize, false, "Maximizable", CheckType.IncorrectElementConfiguration);

            // "Verify that you can set the VisualState property to WindowVisualState.Maximizable",
            TS_VerifySetVisualState(WindowVisualState.Maximized, typeof(InvalidOperationException), CheckType.Verification);

            // "Verify that the WindowVisualState is WindowVisualState.Maximizable after setting the property"
            TS_VerifyVisualState(WindowVisualState.Maximized, false, CheckType.Verification);
        }

        #endregion WindowPattern.MaximizableProperty

        #region WindowPattern.MinimizableProperty

        ///<summary>TestCase: Window.MinimizableProperty.S.7.2</summary>
        [TestCaseAttribute("Window.MinimizableProperty.S.7.2",
            Priority = TestPriorities.Pri1,
            TestCaseType = TestCaseType.Generic,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
											"Precondition: Minimizable property is false",
											"Verify that you cannot set the VisualState property to WindowVisualState.Minimized and that SetVisualStats returns false",
											"Verify that the WindowVisualState is not WindowVisualState.MinimizableProperty after setting the VisualState property"
										}
             )]
        public void TestMinimizePropertyS72(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            // "Precondition: Minimizable property is false",,
            TSC_VerifyProperty(pattern_getCanMinimize, false, "Minimizable", CheckType.IncorrectElementConfiguration);

            // "Verify that you cannot set the VisualState property to WindowVisualState.Minimizable",
            TS_VerifySetVisualState(WindowVisualState.Minimized, typeof(InvalidOperationException), CheckType.Verification);

            // "Verify that the WindowVisualState is not WindowVisualState.Minimizable after setting the property"
            TS_VerifyVisualState(WindowVisualState.Minimized, false, CheckType.Verification);
        }

        #endregion WindowPattern.MinimizableProperty

        #region WindowPattern.ResizableProperty

        #endregion WindowPattern.ResizableProperty

        #region WindowPattern.ModalProperty

        ///<summary>TestCase: Window.ModalProperty.S.10.1</summary>
        [TestCaseAttribute("Window.ModalProperty.S.10.1",
            Priority = TestPriorities.Pri1,
            TestCaseType = TestCaseType.Generic,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[] {
				"Precondition: Modal property is true",
			})]
        public void TestWindowModalS101(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            // Precondition: Resizable = false
            TSC_VerifyProperty(pattern_getIsModal, true, "Modal", CheckType.IncorrectElementConfiguration);
        }

        #endregion WindowPattern.ModalProperty

        #region Verification

        //        /// -------------------------------------------------------------------
        //        void TS_Minimizable(bool ShouldBeMinimized, CheckType checkType)
        //        {
        //            if (!pattern_getCanMinimize.Equals(ShouldBeMinimized))
        //                ThrowMe("CanMinimize == " + pattern_getCanMinimize, checkType);
        //
        //            m_TestStep++;
        //		}

        /// -------------------------------------------------------------------
        void TS_VerifyVisualState(WindowVisualState WindowVisualState, bool shouldBe, CheckType checkType)
        {
            if (!pattern_getWindowInteractionState.Equals(WindowVisualState).Equals(shouldBe))
                ThrowMe(checkType, TestCaseCurrentStep + ": WindowVisualState = " + pattern_getVisualState);

            Comment("WindowVisualState = " + pattern_getWindowInteractionState);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        void TS_VerifySetVisualState(WindowVisualState WindowVisualState, Type expectedException, CheckType checkType)
        {
            pattern_SetWindowVisualState(WindowVisualState, expectedException, checkType);
            m_TestStep++;
        }

        #endregion Verification

    }
}
