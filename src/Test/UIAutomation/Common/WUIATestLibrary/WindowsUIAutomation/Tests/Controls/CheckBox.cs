// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
* Purpose: 
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
	public sealed class CheckBoxControlTests : ControlObject
    {
		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		const string THIS = "CheckBoxControlTests";

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public const string TestSuite = NAMESPACE + "." + THIS;

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public CheckBoxControlTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, IApplicationCommands commands)
            :
        base(element, TestSuite, priority, TypeOfControl.CheckBoxControl, dirResults, testEvents, commands)
        {
        }

        #region Test Cases called by TestObject.RunTests()

        #endregion Test Cases called by TestObject.RunTests()

        #region Misc
        #endregion Misc
    }
}
