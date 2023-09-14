// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
* Purpose: 
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

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class ValueWrapper : PatternObject
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        ValuePattern _pattern;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal ValueWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _pattern = (ValuePattern)GetPattern(m_le, m_useCurrent, ValuePattern.Pattern);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void pattern_SetValue(object val, Type expectedException, CheckType checkType)
        {
            string newVal = Convert.ToString(val, CultureInfo.CurrentUICulture);

            string call = "SetValue(" + newVal + ")";

            try
            {
                Comment("Before " + call + " Value = " + pattern_Value);
                _pattern.SetValue(newVal);
                Comment("After " + call + " Value = " + pattern_Value);

                // Are we in a "no-clobber" state?  If so, bail gracefully
                // This is why we throw an IncorrectElementConfiguration instead of
                // default value of checkType
                if (_noClobber == true)
                {
                    ThrowMe(CheckType.IncorrectElementConfiguration, "/NOCLOBBER flag is set, cannot update the value of the control, exiting gracefully");
                }
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
        internal bool pattern_IsReadOnly
        {
            get
            {
                return _pattern.Current.IsReadOnly;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal string pattern_Value
        {
            get
            {
                return _pattern.Current.Value;
            }
        }
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
	public sealed class ValueTests : ValueWrapper
    {
        #region Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const int Checked = 0;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const int Unchecked = 1;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const int Indeterminate = 2;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        bool[] _checkedStated = new bool[3];

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const string THIS = "ValueTests";

        #endregion Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static readonly string TestWhichPattern = Automation.PatternName(ValuePattern.Pattern);

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ValueTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.Value, dirResults, testEvents, commands)
        {
        }

        #region Tests: Pattern Specific

        #region SetValue
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.1",
            TestSummary = "Set the Value to a random valid value based on the AutomationElement.ControlTypeProperty",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,  
            Author = "Microsoft",
            BugNumbers = new BugIssues[] { BugIssues.PS26},
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify ReadOnly = false", 
                "Step: Get the value of the current Value", 
                "Step: Verify that the old value does not equal the new random value", 
                "Step: Add event that will catch PropertyChangeEvent", 
                "Step: Set the value of the pattern to the random valid value according to the AutomationElement.ControlTypeProperty", 
                "Step: Wait for PropertyChangeEvent", 
                "Verify that the PropertyChangeEvent event is fired and the random string is passed into the event", 
                "Verify that Value is set correctly to the new value"
        })]
        public void TestSetValue11(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            SetValue_Test1(false, Helpers.GetRandomValidValue(m_le, null), testCase);
        }
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.2",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,   
            Author = "Microsoft",
            BugNumbers = new BugIssues[] { BugIssues.PS26 },
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify ReadOnly = false",
                "Step: Get the value of the current Value",
                "Step: Verify that the old value does not equal the new random value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the value of the pattern to \"\"", 
                "Step: Wait for event", 
                "Verify that the PropertyChangeEvent event is fired and the random string is passed into the event",
                "Verify that Value is set correctly to the new value"
             })]
        public void TestSetValue12(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            if (m_le.Current.ControlType != ControlType.Edit)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Test is designed for edit control");

            SetValue_Test1(false, "", testCase);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.3",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,   
            Author = "Microsoft",
            BugNumbers = new BugIssues[] { BugIssues.PS26 },
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify ReadOnly = false", 
                "Step: Get the value of the current Value", 
                "Step: Verify that the old value does not equal to very large string", 
                "Step: Add event that will catch PropertyChangeEvent", 
                "Step: Set the value of the pattern to the very large string", 
                "Step: Step: Wait for event", 
                "Verify that the PropertyChangeEvent event is fired and the random string is passed into the event", 
                "Verify that Value is set correctly to the new value"
        })]
        public void TestSetValue13(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            SetValue_Test1(false, Helpers.GetRandomValidValue(m_le, true), testCase);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.7",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,   
            Author = "Microsoft",
            BugNumbers = new BugIssues[] { BugIssues.PS26}, 
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify ReadOnly = false",
                "Step: Get the value of the current Value",
                "Step: Verify that the old value does not equal to small string",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the value of the pattern to the very large string", 
                "Step: Step: Wait for event", 
                "Verify that the PropertyChangeEvent event is fired and the random string is passed into the event",
                "Verify that Value is set correctly to the new value"
            })]
        public void TestSetValue17(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            SetValue_Test1(false, Helpers.GetRandomValidValue(m_le, false), testCase);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.9",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,   
            Author = "Microsoft",
            BugNumbers = new BugIssues[] { BugIssues.PS26},
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify that this control's ReadOnly == false",
                "Step: Verify that this control supports string types",
                "Step: Get the value of the current Value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the value of the pattern to same string", 
                "Step: Wait for event", 
                "Verify that the PropertyChangeEvent event is Undetermined",
            })]
        public void TestSetValueS19(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            SetValue_SetToSameValue(ObjectTypes.String, false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.10",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Problem,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify that this control's ReadOnly == false",
                "Step: Verify that this control supports the enum ItemCheckState types",
                "Step: Get the value of the current Value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the value of the pattern to same ItemCheckState", 
                "Step: Wait for event", 
                "Verify that the PropertyChangeEvent event is Undetermined",
            })]
        public void TestSetValue10(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            SetValue_SetToSameValue(ObjectTypes.ItemCheckState, false, EventFired.Undetermined);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.11",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,  
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify that this control's ReadOnly == false",
                "Step: Verify that this control supports the Integer types",
                "Step: Get the value of the current Value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the value of the pattern to same Integer", 
                "Step: Wait for event", 
                "Verify that the PropertyChangeEvent event is Undetermined",
            })]
        public void TestSetValue111(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            SetValue_SetToSameValue(ObjectTypes.Int32, false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.12",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Problem,
            ProblemDescription = "There is a problem in that it will not throw if you pass a wrong datatype such as Int64 to the existing data type of Int32 when you can cast the value such as 10(Int64) to the corRect type 10(Int32)",
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: Verify that this control's ReadOnly == false",
                "Step: Determine a random non-supported data type",
                "Step: Get random value of a non-supported random object type",
                "Step: Add PropertyChangeEvent event listener",
                "Step: Call SetValue with the non-supporting data type and verify that an exception is thrown",
                "Step: Step: Wait for event",
                "Verify that PropertyChangeEvent event does get fired"
            })]
        public void TestSetValue112(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            object otherType;
            ObjectTypes objectType;

            // Precondition: Verify that this control's ReadOnly == false
            TS_VerifyReadOnly(false, CheckType.IncorrectElementConfiguration);

            // Get random value of a non-supported random object type of the current pattern
            TS_GetNonSupportedDataType(out objectType, pattern_Value, CheckType.IncorrectElementConfiguration);

            // Get a random value of the non-supported data type
            TS_GetRandomValue(out otherType, pattern_Value, objectType, true, false, CheckType.Verification);

            // Add PropertyChangeEvent event listener
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);
            // Call SetValue with the non-supporting data type and verify that an exception is thrown
            TS_SetValue(otherType, false, typeof(ArgumentException), CheckType.Verification);
            TSC_WaitForEvents(1);

            // Verify that no event is fired
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.16",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,  
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify that control is read only",
                "Precondition: Verify that this control supports 'String' types",
                "Step: Get a random valid value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the pattern to this value", 
                "Step: Step: Wait for event", 
                "Verify the pattern is set to this value",
                "Verify that the PropertyChangeEvent event is not fired"
            })]
        public void TestSetValue116(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            this.SetValue_SetToRandomValue(ObjectTypes.String, true, true, false, false, false, EventFired.False, typeof(InvalidOperationException));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.17",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,  
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify that control is read only",
                "Precondition: Verify that this control supports 'ItemCheckState' types",
                "Step: Get a random valid value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the pattern to this value", 
                "Step: Wait for event", 
                "Verify the pattern is not set to this value",
                "Verify that the PropertyChangeEvent event is not fired"
            })]
        public void TestSetValue117(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            this.SetValue_SetToRandomValue(ObjectTypes.ItemCheckState, false, true, false, false, true, EventFired.False, typeof(InvalidOperationException));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.18",
            Priority = TestPriorities.Pri0,
           Status = TestStatus.Works,  
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify that control is read only",
                "Precondition: Verify that this control supports Integer types",
                "Step: Get a random valid value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the pattern to this value", 
                "Step: Wait for event", 
                "Verify the pattern is not set to this value",
                "Verify that the PropertyChangeEvent event is not fired"
            })]
        public void TestSetValue118(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            this.SetValue_SetToRandomValue(ObjectTypes.Int32, true, true, false, false, false, EventFired.False, typeof(InvalidOperationException));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.19",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,   
            Author = "Microsoft",
            BugNumbers = new BugIssues[] { BugIssues.PS26}, 
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify ReadOnly = false",
                "Step: Get the value of the current Value",
                "Step: Verify that the old value does not equal to ''",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the value of the pattern to ''", 
                "Step: Wait for event", 
                "Verify that the PropertyChangeEvent event is fired and the random string is passed into the event",
                "Verify that Value is set correctly to the new value"
            })]
        public void TestSetValue119(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            ControlType ct = m_le.Current.ControlType;

            if (ct == null)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Cannot determine if control will accept '' since AutomationElement.ControlTypeProperty returns null");

            if (ct == ControlType.Custom)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Cannot determine if control will accept '' since AutomationElement.ControlTypeProperty returns ControlType.Custom");

            // Which controls support value???
            if ((ct != ControlType.Edit) &&
                (ct != ControlType.ListItem) &&
                (ct != ControlType.TreeItem)
                )
                ThrowMe(CheckType.IncorrectElementConfiguration, Helpers.GetProgrammaticName(ct) + " do not support ''");

            SetValue_Test1(false, "", testCase);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.27",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,  
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify that control is not read only",
                "Precondition: Verify that this control supports 'DateTime' types",
                "Step: Get a random valid value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the pattern to this value", 
                "Step: Wait for event", 
                "Verify the pattern is set to this value",
                "Verify that the PropertyChangeEvent event is fired"
            })]
        public void TestSetValue127(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            AutomationIdentifier ai = m_le.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as AutomationIdentifier;

            if (ai == ControlType.Calendar)
                SetValue_SetToRandomValue(ObjectTypes.Date, false, false, true, true, true, EventFired.True, null);
            else
                SetValue_SetToRandomValue(ObjectTypes.DateTime, false, false, true, true, true, EventFired.True, null);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.28",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,  
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify that this control's ReadOnly == false",
                "Precondition: Verify that this control supports 'DateTime' data type",
                "Step: Get the current value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the value of the pattern to the current value", 
                "Step: Wait for event", 
                "Verify that the PropertyChangeEvent event is not fired"
            })]
        public void TestSetValue128(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            SetValue_SetToSameValue(ObjectTypes.DateTime, pattern_Value, EventFired.False);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("ValuePattern.SetValue.S.1.29",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,  
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: Verify ReadOnly = false",
                "Step: Get the value of the current Value",
                "Step: Verify that the old value does not equal the current value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Set the value of the pattern to a random string", 
                "Step: Step: Wait for event", 
                "Verify that the PropertyChangeEvent event is fired and the random string is passed into the event",
                "Verify that Value is set correctly to the new value"
            })]
        public void TestSetValue_DataGridCell(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);
            SetValue_CustomDataGridCell(false, Helpers.GetRandomValidValue(m_le, false), testCase);
        }

        #endregion SetValue

        #region ValueProperty
        // No controlled environment test cases
        #endregion ValueProperty

        #region IsReadOnlyProperty
        // No controlled environment test cases
        #endregion IsReadOnlyProperty

        #endregion Tests

        #region Test Wrappers

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void SetValue_Test1(bool readOnly, object newValue, TestCaseAttribute testCaseAttribute)
        {
            object oldValue;
            ControlType ct = m_le.Current.ControlType;

            if (ct == ControlType.Custom)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Cannot verify setting ControlType.Custom is correct");

            // Verify ReadOnly = false
            TS_VerifyReadOnly(readOnly, CheckType.IncorrectElementConfiguration);

            if (ct == null)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Cannot determine specific control since AutomationElement.ControlTypeProperty returns null");

            Comment("DataType: " + newValue.GetType() + "; New Value: " + newValue);

            // Get the value of the current Value
            TS_GetValue(out oldValue, CheckType.IncorrectElementConfiguration);

            // Verify that the old value does not equal the new random value
            TS_VerifyObjectNotEqual(newValue, oldValue, CheckType.IncorrectElementConfiguration);

            // Add event that will catch PropertyChangeEvent 
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);

            // Set the value of the pattern to the random string
            TS_SetValue(newValue, true, null, CheckType.Verification);

            // Wait for event
            TSC_WaitForEvents(1);

            // Verify that the PropertyChangeEvent event is fired and the random string is passed into the event
            if (!TS_FilterOnBug(testCaseAttribute))
                TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);

            // Verify that Value is set correctly to the new value
            TS_VerifyValue(newValue, true, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void SetValue_CustomDataGridCell(bool readOnly, object newValue, TestCaseAttribute testCaseAttribute)
        {
            object oldValue;
            ControlType ct = m_le.Current.ControlType;

            if (ct != ControlType.Custom && m_le.Current.ClassName.ToLower() == "datagridcell")
                ThrowMe(CheckType.IncorrectElementConfiguration, "Cannot verify setting where Element is not DataGridCell");

            // Verify ReadOnly = false
            TS_VerifyReadOnly(readOnly, CheckType.IncorrectElementConfiguration);

            if (ct == null)
                ThrowMe(CheckType.IncorrectElementConfiguration, "Cannot determine specific control since AutomationElement.ControlTypeProperty returns null");

            Comment("DataType: " + newValue.GetType() + "; New Value: " + newValue);

            // Get the value of the current Value
            TS_GetValue(out oldValue, CheckType.IncorrectElementConfiguration);

            // Verify that the old value does not equal the new random value
            TS_VerifyObjectNotEqual(newValue, oldValue, CheckType.IncorrectElementConfiguration);

            // Add event that will catch PropertyChangeEvent 
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);

            // Set the value of the pattern to the random string
            TS_SetValue(newValue, true, null, CheckType.Verification);

            // Wait for event
            TSC_WaitForEvents(1);

            // Verify that the PropertyChangeEvent event is fired and the random string is passed into the event
            if (!TS_FilterOnBug(testCaseAttribute))
                TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);

            // Verify that Value is set correctly to the new value
            TS_VerifyValue(newValue, true, CheckType.Verification);
        }


        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void SetValue_SetToSameValue(ObjectTypes dataType, object Value, EventFired eventFired)
        {
            object val;
            // Precondition: Verify that this control is not ReadOnly
            TS_VerifyReadOnly(false, CheckType.IncorrectElementConfiguration);

            // Precondition: Verify that this control supports #### types
            TS_VerifyObjectType(pattern_Value, dataType, true, CheckType.IncorrectElementConfiguration);

            // Get the current value
            TS_GetValue(out val, CheckType.Verification);

            // Add event that will catch PropertyChangeEvent 
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);

            // Set the value of the pattern to the random string
            TS_SetValue(Value, true, null, CheckType.Verification);

            TSC_WaitForEvents(1);

            // Verify that the PropertyChangeEvent event is fired and the random string is passed into the event
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { eventFired }, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void SetValue_SetToSameValue(ObjectTypes dataType, bool CanBeNull)
        {
            object val;
            // Precondition: Verify that this control is not ReadOnly
            TS_VerifyReadOnly(false, CheckType.IncorrectElementConfiguration);

            // Verify that this control supports string types
            TS_SupportsDataType(dataType, pattern_Value, CanBeNull, CheckType.IncorrectElementConfiguration);

            // Get the value of the current Value
            TS_GetValue(out val, CheckType.Verification);

            // Add event that will catch PropertyChangeEvent 
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);

            // Set the value of the pattern to same *
            TS_SetValue(val, true, null, CheckType.Verification);

            TSC_WaitForEvents(1);

            // Verify that the PropertyChangeEvent event is fired and the random string is passed into the event
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.Undetermined }, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void SetValue_SetToRandomValue(ObjectTypes dataType, bool CanBeNull, bool ShouldBeReadOnly, bool MethodReturnValue, bool DoesValueChange, bool RandomValDifferentFromCurrentValue, EventFired eventFired, Type expectedException)
        {
            object val = pattern_Value;

            // Verify that control is not read only
            TS_VerifyReadOnly(ShouldBeReadOnly, CheckType.IncorrectElementConfiguration);

            // Verify that this control supports ######### types
            TS_VerifyObjectType(pattern_Value, dataType, CanBeNull, CheckType.IncorrectElementConfiguration);

            // Get a random valid value
            TS_GetRandomValue(out val, pattern_Value, dataType, true, RandomValDifferentFromCurrentValue, CheckType.Verification);

            // Add event that will catch PropertyChangeEvent 
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);

            // Set the pattern to this value
            TS_SetValue(val, MethodReturnValue, expectedException, CheckType.Verification);

            // Wait for event
            TSC_WaitForEvents(1);

            // Verify the pattern is set to this value
            TS_VerifyValue(val, DoesValueChange, CheckType.Verification);

            // Verify that the PropertyChangeEvent event is *
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { eventFired }, new AutomationProperty[] { ValuePattern.ValueProperty }, CheckType.Verification);
        }

        #endregion Test Wrappers

        #region Test Steps

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_SetValue(object val, bool ReturnValue, Type exceptionExpected, CheckType ct)
        {
            pattern_SetValue(val, exceptionExpected, ct);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyObjectNotEqual(object val1, object val2, CheckType ct)
        {
            if (val1.Equals(val2))
                ThrowMe(ct, TestCaseCurrentStep + ": Values are equal");

            Comment(" '" + val1 + "' != '" + val2 + "'");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_GetValue(out object val, CheckType ct)
        {
            val = pattern_Value;
            Comment(" Current Value = " + val);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyReadOnly(bool isReadOnly, CheckType checkType)
        {
            if (isReadOnly != pattern_IsReadOnly)
                ThrowMe(checkType);

            Comment(" IsReadOnly property is " + pattern_IsReadOnly);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyValue(object val, bool isEqual, CheckType checkType)
        {

            string pValue = pattern_Value;

            // Trim trailing CR/LF if we're on a richedit
            TextTestsHelper.TrimTrailingCRLF(m_le, ref pValue);

            bool equal = GenericMath.AreEquals(pValue, val.ToString());

            if (!equal.Equals(isEqual))
                ThrowMe(checkType, "ValuePattern.Value = '" + pValue + "'(" + pValue.GetType() + ") and expected '" + val + "'(" + val.GetType() + ")");

            Comment("Is " + (isEqual == true ? "equal" : "not equal") + " to ValuePattern.Value (" + pValue + ")");
            m_TestStep++;
        }

        #endregion Test Steps
    }
}
