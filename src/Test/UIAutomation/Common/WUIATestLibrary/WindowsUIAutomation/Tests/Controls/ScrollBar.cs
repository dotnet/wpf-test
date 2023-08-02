// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
* Purpose: InternalHelper
* Owner: Microsoft
* Contributors:
*******************************************************************/
using System;
using System.Windows.Automation;
using System.Collections;

namespace Microsoft.Test.WindowsUIAutomation.Tests.Controls
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
	public sealed class ScrollBarControlTests : ControlObject
    {
		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		const string THIS = "ScrollBarControlTests";

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public const string TestSuite = NAMESPACE + "." + THIS;

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public ScrollBarControlTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, IApplicationCommands commands)
            :
        base(element, TestSuite, priority, TypeOfControl.ScrollBarControl, dirResults, testEvents, commands)
        {
        }

        #region Test Cases called by TestObject.RunTests()

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("IsRangeValuePatternAvailableProperty.1",
            TestSummary = "This functionality only is required to be supported on the scroll bar if ScrollPattern is not supported on the container that has the scrollbars.",
            TestCaseType = TestCaseType.Generic,
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Description = new string[] 
                {
                    "Precondition: AutomationElement does not have a valid hwnd, ie. this scrollbar is a sub componet",
                    "Precondition: Get the scrollbars parent control",
                    "Precondition: Parent's IsScrollPatternAvailableProperty == false",
                    "Verification: AutomationElement.IsRangeValuePatternAvailableProperty == true"
                })]
        public void IsTextPatternAvailableProperty1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            AutomationElement parent;

            //"Precondition: AutomationElement does not have a valid hwnd, ie. this scrollbar is a sub componet",
            TSC_VerifyProperty(m_le.Current.NativeWindowHandle, 0, true, "Current.NativeWindowHandle", CheckType.IncorrectElementConfiguration);

            //"Precondition: Get the scrollbars parent control",
            TS_GetScrollbarsParent(m_le, out parent, CheckType.IncorrectElementConfiguration);

            //"Precondition: parent.IsScrollPatternAvailableProperty == false",
            TSC_VerifyProperty(parent.GetCurrentPropertyValue(AutomationElement.IsScrollPatternAvailableProperty), false, "AutomationElement.IsScrollPatternAvailableProperty", CheckType.IncorrectElementConfiguration);

            //"Verification: AutomationElement.IsRangeValuePatternAvailableProperty == true"
            TSC_VerifyProperty((bool)m_le.GetCurrentPropertyValue(AutomationElement.IsRangeValuePatternAvailableProperty), true, "AutomationElement.IsRangeValuePatternAvailableProperty", CheckType.Verification);
        }


        #endregion Test Cases called by TestObject.RunTests()

        #region Misc

        /// -------------------------------------------------------------------
        /// <summary>Get the scrollbars parent AutomationElement</summary>
        /// -------------------------------------------------------------------
        private void TS_GetScrollbarsParent(AutomationElement le, out AutomationElement parent, CheckType checkType)
        {
            Helpers.GetParentHwndControl(le, out parent);
            m_TestStep++;
        }

        #endregion Misc
    }
}
