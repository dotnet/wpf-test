// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//      support
//

#define CODE_ANALYSIS  // Required for FxCop suppression attributes
using System;
using System.Globalization;
using Drawing = System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows;
using System.CodeDom;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Diagnostics.CodeAnalysis; // Required for FxCop suppression attributes
using InternalHelper;
using MS.Win32;

namespace Microsoft.Test.WindowsUIAutomation.Tests.Scenarios
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
    public class AvalonTextScenarioTests: ScenarioObject, IDisposable
    {

        #region Member Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;     // used by calling test harness

        const string        THIS = "AvalonTextScenarioTests";       // used by calling test harness
        TextPattern         _pattern;                               // Local variable for TextPattern of current automation element
        ManualResetEvent    _NotifiedEvent;                         // used by Dispose

        #endregion Member Variables

        #region constructor

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="typeOfControl")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="typeOfPattern")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="dirResults")]
        public AvalonTextScenarioTests(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, TypeOfControl.UnknownControl, TypeOfPattern.Unknown, null, testEvents, commands)
        {
            _NotifiedEvent = new System.Threading.ManualResetEvent(false);
        }

        #endregion constructor

        #region Avalon Scenario Tests


        /// -------------------------------------------------------------------
        /// <summary>
        /// Template for Avalon Text Pattern Scenarios
        /// This particular scenario takes no arguments from the calling
        /// test harness/scenario driver
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Avalon Scenario #1 (no arguments)",
            TestSummary = "Template for an Avalon TextPattern Scenario that does not take any arguments from calling test harness driver",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,   
            Client = Client.ATG,
            Description = new string[] {
                    "Pre-condition: Get Scenario Pre-Conditions",
                    "TBD",
        })]
        public void AvalonScenario1(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            // Pre-condition: Get Scenario Pre-Conditions
            TS_ScenarioPreConditions(true, CheckType.IncorrectElementConfiguration);

            // TBD
            Comment("This test has passed");
            m_TestStep++;
        }


        /// -------------------------------------------------------------------
        /// <summary>
        /// Template for Avalon Text Pattern Scenarios
        /// This particular scenario takes two (string) arguments from the calling
        /// test harness/scenario driver
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Avalon Scenario #2 (takes arguments)",
            TestSummary = "Template for an Avalon TextPattern Scenario that *DOES* take arguments from calling test harness driver",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Arguments,
            Client = Client.ATG,
            Description = new string[] {
                    "Pre-condition: Get Scenario Pre-Conditions",
                    "Cause test to suceed or fail based on value of second string argument",
        })]
        public void AvalonScenario2(TestCaseAttribute testCase, object[] arguments)
        {
            // Validate arguments passed to test
            if (arguments == null)
                throw new ArgumentException("arguments is null");

            if (arguments[0] == null)
                throw new ArgumentException("arguments[0] is null");

            if (arguments[1] == null)
                throw new ArgumentException("arguments[1] is null");

            if (arguments.Length != 2)
                throw new ArgumentException("arguments is wrong length");

            if (!(arguments[0] is string) || !(arguments[1] is string))
                throw new ArgumentException("arguments are of wrong type");

            // cast arguments to local variables
            string string1 = (string)arguments[0];
            string string2 = (string)arguments[1];

            HeaderComment(testCase);

            // Pre-condition: Get Scenario Pre-Conditions
            TS_ScenarioPreConditions(true, CheckType.IncorrectElementConfiguration);

            // Cause test to suceed or fail based on value of second string argument
            Comment("string1 = " + string1);
            Comment("string2 = " + string2);
            if (string2 == "IncorrectElementConfiguration")
                ThrowMe(CheckType.IncorrectElementConfiguration,
                         "Some condition required for the test to actually be performed wasn't met. " +
                         "Exit gracefully without setting a failure condition for this scenario");
            else if (string2 == "Verification")
                ThrowMe(CheckType.Verification, "Test failed, call ThrowMe with CheckType.Verification to log this test scenario as a failure");
            else
                Comment("This test has passed");

            m_TestStep++; // increment counter as a best practice
        }


        #endregion Avalon Scenario Tests

        #region helpers

        // -------------------------------------------------------------------
        // Determine if the application we are hitting supports knowledge about the control's text implementation
        // -------------------------------------------------------------------
        private void TS_ScenarioPreConditions(bool requireTextPattern, CheckType checkType)
        {
            string className;
            string localizedControlType;

            // This is hard-coded as a critical failure
            if (m_le == null)
            {
                ThrowMe( CheckType.Verification, "Unable to get AutomationElement for control with focus");
            }

            // Give info about control 
            TextLibrary.GetClassName(m_le, out className, out localizedControlType);
            Comment("Automation ID = " + m_le.Current.AutomationId.ToString() +
                    "           (" + className + " / " + localizedControlType + ")");

            try
            {
                _pattern = m_le.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            }
            catch (Exception exception)
            {
                if (Library.IsCriticalException(exception))
                    throw;

                Comment("Acquiring TextPattern for automation element with focus raised exception");
                Comment("  Message = " + exception.Message);
                Comment("  Type    = " + exception.GetType().ToString());
                ThrowMe(checkType, "Unable to proceed with test, should not have received exception acquiring TextPattern");  // hard-coded... on purpose
            }

            m_TestStep++;
        }

        #endregion helpers

        #region Dispose

        /// -------------------------------------------------------------------
        /// <summary>
        /// Dispose method(s)
        /// </summary>
        /// -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)  // Required by OACR for member _NotifiedEvent
        {
            if (disposing)
            {
                if (_NotifiedEvent != null)
                {
                    _NotifiedEvent.Close();
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Dispose method(s)
        /// </summary>
        /// -------------------------------------------------------------------
        public void Dispose()  // Required by OACR for member _NotifiedEvent
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion Dispose
    }
}
