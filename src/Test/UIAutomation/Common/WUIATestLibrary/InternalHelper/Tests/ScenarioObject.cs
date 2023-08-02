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
using System.Windows;
using System.Windows.Automation;
using System.Collections;
using System.Drawing;
using System.CodeDom;
using System.Text;
using System.Threading;

// This suppresses warnings #'s not recognized by the compiler.
#pragma warning disable 1634, 1691

namespace InternalHelper.Tests
{
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
	using Microsoft.Test.WindowsUIAutomation.TestManager;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class ScenarioObject : TestObject
    {
		internal const string NAMESPACE = IDSStrings.IDS_NAMESPACE_UIVERIFY + "." + IDSStrings.IDS_NAPESPACE_SCENARIO;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ScenarioObject(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
		base (element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _testCaseSampleType = TestCaseSampleType.Scenario;
		}
	}
}
