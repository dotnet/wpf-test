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
	public sealed class TextControlTests : ControlObject
    {
		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		const string THIS = "TextControlTests";

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public const string TestSuite = NAMESPACE + "." + THIS;

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public TextControlTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, IApplicationCommands commands)
            :
        base(element, TestSuite, priority, TypeOfControl.TextControl, dirResults, testEvents, commands)
        {
        }

        #region Test Cases called by TestObject.RunTests()

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("IsTextPatternAvailableProperty.1",
           TestSummary = "Depends on framework whether TextPattern will be supported on text elements.  For Avalon elements TextPattern will be supported.  For legacy text elements it will not be able to be supported.  Any new framework supporting UIAutomation must expose TextPattern for text elements.",
            TestCaseType = TestCaseType.Generic,
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Description = new string[] 
                {
                    "Precondition: AutomationElement has a valid hwnd",
                    "Precondition: FrameworkID = Win32",
                    "Verification: AutomationElement.IsTextPatternAvailableProperty == false",
                })]
        public void IsTextPatternAvailableProperty1(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            // "Precondition: Element has a valid hwnd",
            TSC_VerifyProperty(m_le.Current.NativeWindowHandle, 0, false, "Current.NativeWindowHandle", CheckType.IncorrectElementConfiguration);

            // "Verification: HelpTextProeprty != null",
            TSC_VerifyProperty(m_le.Current.FrameworkId, "Win32", true, "Current.FrameworkId", CheckType.IncorrectElementConfiguration);

            // "Verification: HelpTextProeprty != \"\"",
            TSC_VerifyProperty((bool)m_le.GetCurrentPropertyValue(AutomationElement.IsTextPatternAvailableProperty), false, "AutomationElement.IsTextPatternAvailableProperty", CheckType.Verification);
        }

        /// ---------------------------------------------------------------
        ///<summary></summary>
        /// ---------------------------------------------------------------
        [TestCaseAttribute("IsTextPatternAvailableProperty.2",
           TestSummary = "Depends on framework whether TextPattern will be supported on text elements.  For Avalon elements TextPattern will be supported.  For legacy text elements it will not be able to be supported.  Any new framework supporting UIAutomation must expose TextPattern for text elements.",
            TestCaseType = TestCaseType.Generic,
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Description = new string[] 
                {
                    "Precondition: AutomationElement has a valid hwnd",
                    "Precondition: FrameworkID != Win32",
                    "Verification: AutomationElement.IsTextPatternAvailableProperty == true",
                })]
        public void IsTextPatternAvailableProperty2(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);

            // "Precondition: Element has a valid hwnd",
            TSC_VerifyProperty(m_le.Current.NativeWindowHandle, 0, false, "Current.NativeWindowHandle", CheckType.IncorrectElementConfiguration);

            // "Verification: HelpTextProeprty != null",
            TSC_VerifyProperty(m_le.Current.FrameworkId, "Win32", false, "Current.FrameworkId", CheckType.IncorrectElementConfiguration);

            // "Verification: HelpTextProeprty != \"\"",
            TSC_VerifyProperty((bool)m_le.GetCurrentPropertyValue(AutomationElement.IsTextPatternAvailableProperty), false, "AutomationElement.IsTextPatternAvailableProperty", CheckType.Verification);
        }


        #endregion Test Cases called by TestObject.RunTests()

        #region Misc
        #endregion Misc
    }
}
