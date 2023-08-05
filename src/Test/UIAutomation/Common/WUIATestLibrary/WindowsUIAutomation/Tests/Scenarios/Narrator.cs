// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//******************************************************************* 
using System;
using System.CodeDom;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Automation;
using System.Windows;

namespace Microsoft.Test.WindowsUIAutomation.Tests.Scenarios
{
    using InternalHelper.Enumerations;
    using InternalHelper.Tests;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
    using Microsoft.Test.WindowsUIAutomation.TestManager;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    class NarratorScenarioTests : ScenarioObject
    {
        const string THIS = "NarratorScenarioTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        ManualResetEvent _notifiedEvent = new System.Threading.ManualResetEvent(false);

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public NarratorScenarioTests(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, TypeOfControl.UnknownControl, TypeOfPattern.Unknown, null, testEvents, commands)
        {
        }

        #region Scenarios

        /// -------------------------------------------------------------------
        /// <summary>
        /// Starting point for tests (template)
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Sample call to scenario without arguments",
            TestSummary = "Test showing test with no argument...note no 'object[] arguments' in the formal arguments",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.Narrator,
            Description = new string[] {
                "Step: empty test case", 
        })]
        public void NarratorScenario1(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            // blah blah blah...

        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Sample scenario for Narrator tester
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Sample call to scenario with arguments",
            TestSummary = "Sample call showing how to pass in arguments",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Arguments,
            Client = Client.Narrator,
            Description = new string[] {
                "Verify Name != null", 
                "Verify Name != string.Empty", 
                "Verify name is what is passed in"
        })]
        public void NarratorScenario1(TestCaseAttribute testCase, object[] arguments)
        {
            HeaderComment(testCase);

            // Unpack the arguments
            string name = (string)arguments[0];

            // "Verify Name != null", 
            TS_VerifyNotNull(CheckType.Verification);

            // "Verify Name != string.Empty", 
            TS_VerifyNotEmpty(CheckType.Verification);

            // Verify name is what is passed in
            TS_VerifyName(name, CheckType.Verification);
        }

        #endregion Scenarios

        #region Tests

        /// -------------------------------------------------------------------
        /// <summary>
        /// Verify that the AutomationElement.NameProperty != null
        /// </summary>
        /// -------------------------------------------------------------------
        void TS_VerifyNotNull(CheckType checkType)
        {
            if (m_le.GetCurrentPropertyValue(AutomationElement.NameProperty) as string == null)
                ThrowMe(checkType, "NameProperty == null and should not");

            Comment(Library.GetUISpyLook(m_le) + "'s NameProperty != null");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Verify that AutomationElement.NameProperty == string.Empty
        /// </summary>
        /// -------------------------------------------------------------------
        void TS_VerifyNotEmpty(CheckType checkType)
        {
            if (m_le.GetCurrentPropertyValue(AutomationElement.NameProperty) as string == string.Empty)
                ThrowMe(checkType, "NameProperty == string.Empty");

            Comment(Library.GetUISpyLook(m_le) + "'s NameProperty != string.Empty");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Verify that AutomationElement.NameProperty == known string
        /// </summary>
        /// -------------------------------------------------------------------
        void TS_VerifyName(string name, CheckType checkType)
        {
            string curName = m_le.Current.Name;
            if (curName != name)
                ThrowMe(checkType, "NameProperty != " + name + ", but is '" + curName + "'");

            Comment("Name is correct: '" + curName + "'");

            m_TestStep++;
        }

        #endregion Tests

        #region Supporting code

        #endregion Supporting code

    }
}
