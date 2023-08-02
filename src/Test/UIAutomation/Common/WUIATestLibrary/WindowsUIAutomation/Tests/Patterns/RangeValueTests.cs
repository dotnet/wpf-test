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

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class RangeValuePatternWrapper : PatternObject
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private RangeValuePattern _pattern;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        protected RangeValuePatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _pattern = (RangeValuePattern)GetPattern(m_le, m_useCurrent, RangeValuePattern.Pattern);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void pattern_SetValue(double newVal)
        {
            Comment("Current value == " + pattern_Value);
            Comment("Calling RangeValuePattern.SetValue(" + newVal + ")");
            if (newVal > patternMaximum)
            {
                if (TS_FilterOnBug(m_TestCase, false))
                    _pattern.SetValue(patternMaximum);
                else
                    _pattern.SetValue(newVal);
            }
            else
            {
                _pattern.SetValue(newVal);
            }
            
            Comment("Current value == " + pattern_Value);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal double pattern_Value
        {
            get
            {
                return Convert.ToDouble(_pattern.Current.Value, CultureInfo.CurrentCulture);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal double patternMaximum
        {
            get
            {
                return Convert.ToDouble(_pattern.Current.Maximum, CultureInfo.CurrentCulture);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal double patternMinimum
        {
            get
            {
                return Convert.ToDouble(_pattern.Current.Minimum, CultureInfo.CurrentCulture);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal bool patternIsReadOnly
        {
            get
            {
                return _pattern.Current.IsReadOnly;
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
	public sealed class RangeValueTests : RangeValuePatternWrapper
    {
        #region Member variables

        #endregion Member variables

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		const string THIS = "RangeValueTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static readonly string TestWhichPattern = Automation.PatternName(RangeValuePattern.Pattern);

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public RangeValueTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.RangeValue, dirResults, testEvents, commands)
        {
        }

        #region Tests

        #region SetValue

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("RangeValuePattern.SetValue.S.1.1",
            TestSummary = "Call SetValue() with a different value other than the current value and verify that the value is set correctly and a PropertyChangeEvent is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,  
            Author = "Microsoft",
            BugNumbers = new BugIssues[] { BugIssues.PS32, BugIssues.PS36, BugIssues.PS1 },
            TestCaseType = TestCaseType.Events, 
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: ReadOnly = false", 
                "Precondition: Minimum != Maximum", 
                "Step: Verify Minimum is less than Maximum", 
                "Verify that there is a position between the patterns min and max",
                "Step: Call SetValue() with a random valid value other than Minimum/Maximum and that SetValue() returns true",
                "Step: Verify that Value returns this value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Call SetValue(random value) and that SetValue() returns true",
                "Step: Wait for event",
                "Step: Verify that the Value is correct",
                "Step: Verify that the PropertyChangeEvent event is fired and the random value is passed into the event",
            }
            )]
        public void TestSetValueS11(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            RangeValue_SetValue_Test1(pattern_Value, true, false);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("RangeValuePattern.SetValue.S.1.3A",
            TestSummary = "Call SetValue(MIN) when the current value != MIN and verify that the value is set correctly and a PropertyChangeEvent is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,  
            Author = "Microsoft",
            BugNumbers = new BugIssues[] {BugIssues.PS36 },
            TestCaseType = TestCaseType.Events, 
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: ReadOnly = false",	
                "Precondition: Minimum != Maximum", 
                "Step: Verify Minimum is less than Maximum", 
                "Verify that there is a position between the patterns min and max",
                "Step: Call SetValue() with a random valid value other than Minimum and that SetValue() returns true",
                "Step: Verify that Value returns this value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Call SetValue(Minimum) and that SetValue() returns true",
                "Step: Wait for event",
                "Step: Verify that the Value is corRect",
                "Step: Verify that the PropertyChangeEvent event is fired and Minimum is passed into the event",
            }
            )]
        public void TestSetValueS13A(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            RangeValue_SetValue_Test1(patternMinimum, true, false);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("RangeValuePattern.SetValue.S.1.3B",
            TestSummary = "Call SetValue(MAX) when the current value != MAX and verify that the value is set correctly and a PropertyChangeEvent is fired",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            BugNumbers = new BugIssues[] {BugIssues.PS36 },
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events, 
            EventTested = "AutomationPropertyChangedEventHandler(ValuePattern.ValueProperty)",
            Description = new string[] {
                "Precondition: ReadOnly = false",	
                "Precondition: Minimum != Maximum", 
                "Step: Verify Minimum is less than Maximum", 
                "Verify that there is a position between the patterns min and max",
                "Step: Call SetValue() with a random valid value other than Minimum and that SetValue() returns true",
                "Step: Verify that Value returns this value",
                "Step: Add event that will catch PropertyChangeEvent",
                "Step: Call SetValue(Maximum) and that SetValue() returns true",
                "Step: Wait for event",
                "Step: Verify that the Value is correct",
                "Step: Verify that the PropertyChangeEvent event is fired and Maximum is passed into the event",
            })]
        public void TestSetValueS13B(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            RangeValue_SetValue_Test1(patternMaximum, true, false);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("RangeValuePattern.ValueProperty.S.2.1",
            TestSummary = "Verify that Value returns a value that is between or equal to Minimum and Maximum",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works, 
            Author = "Microsoft",
            Description = new string[]{
                "Step: Verify Minimum is less than Maximum",
                "Step: Verify that Minimum <= Value <= Maximium",
            })]
        public void TestValuePropertyS21(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);

            // Verify Minimum is less than Maximum
            TSC_VerifyMinLessThanMax(CheckType.Verification);

            object i = pattern_Value;

            // Verify that Minimum <= Value <= Maximium
            TS_VerifyMinimumLessThanEqualValueAsObjectLessThanEqualMaximum(patternMinimum, pattern_Value, patternMaximum, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("RangeValuePattern.ValueProperty.TestDataTypes",
            TestSummary = "Verify that datatype of Value is the same as the datatype of Minimum and Maximum",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works, 
            Author = "Microsoft",
            Description = new string[] {
                "Verify: Datatype of Value is the same as the datatype of Minimum and Maximum"
            })]
        public void TestValueCorrectDataTypes(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            if (pattern_Value.GetType() != patternMaximum.GetType())
                ThrowMe(CheckType.Verification, "Datatype of the Value (" + pattern_Value.GetType() + ") property != datatype of Maximum (" + patternMaximum.GetType() + ")");

            if (pattern_Value.GetType() != patternMinimum.GetType())
                ThrowMe(CheckType.Verification, "Datatype of the Value (" + pattern_Value.GetType() + ") property != datatype of Minimum (" + patternMaximum.GetType() + ")");

            m_TestStep++;
        }

        #endregion Tests

        #endregion Tests

        #region TestDrivers

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TSC_VerifyPositionBetweenMinMax(CheckType checkType)
        {
            if (Math.Abs(patternMinimum - patternMaximum) < 2)
                ThrowMe(checkType, "There are no values between Minimum  and Maximum");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void RangeValue_SetValue_Test1(double newValue, bool returnValue, bool isCurrentPosition)
        {
            // Verify ReadOnly = false
            TSC_VerifyReadOnly(false, CheckType.IncorrectElementConfiguration);

            // Verify Minimum != Maximum
            TSC_VerifyMinNotEqualMax(CheckType.IncorrectElementConfiguration);

            // Verify Minimum is less than Maximum
            TSC_VerifyMinLessThanMax(CheckType.Verification);

            // Verify that there is a position between the patterns min and max
            TSC_VerifyPositionBetweenMinMax(CheckType.IncorrectElementConfiguration);

            // Set value to ####
            if (!isCurrentPosition.Equals(true))
            {
                TS_SetOtherRandomValue(newValue, CheckType.IncorrectElementConfiguration);

                // TS_SetOtherRandomValue set and verified that it is what is should be, so increment
                m_TestStep++;
            }
            else
            {
                TS_SetValue(newValue, true, CheckType.IncorrectElementConfiguration);

                // Verify that Value returns this value
                TS_VerifyValue(newValue, true, CheckType.Verification);
            }

            // Add event that will catch PropertyChangeEvent 
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { RangeValuePattern.ValueProperty }, CheckType.Verification);

            // Set the value of the pattern to #######
            TS_SetValue(newValue, returnValue, CheckType.Verification);

            // Wait for events
            TSC_WaitForEvents(1);

            // Verify that the Value is corRect
            if (!TS_FilterOnBug(m_TestCase))
                TS_VerifyValue(newValue, true, CheckType.Verification);

            // 
            if (!CheckControlClass(m_le, "ProgressBar"))
            {
                // Verify that the PropertyChangeEvent event is fired and the random string is passed into the event
                if (isCurrentPosition.Equals(true))
                {
                    TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.Undetermined }, new AutomationProperty[] { RangeValuePattern.ValueProperty }, CheckType.Verification);
                }
                else
                {
                    TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.True }, new AutomationProperty[] { RangeValuePattern.ValueProperty }, CheckType.Verification);
                }
            }
        }

        #endregion TestDrivers

        #region Test Steps

        /// -------------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------------
        void TS_SetOtherRandomValue(double notValue, CheckType checkType)
        {
            int TRIES = 10, tries = 0;
            double curValue = pattern_Value;
            double NotValue = notValue;
            double min = patternMinimum;
            double max = patternMaximum;
            double tryVal;

            ControlType controlType = m_le.Current.ControlType;
            object locControlType = null;

            if (controlType == ControlType.Custom)
            {   // go for the localized control type if this is a custom and see what we can do...
                locControlType = m_le.GetCurrentPropertyValue(AutomationElement.LocalizedControlTypeProperty);
                if (locControlType == null || locControlType.ToString().Length == 0 || locControlType == AutomationElement.NotSupported)
                    ThrowMe(checkType, "Can not determine how to set the value of this controls since LocalizedControlTypeProperty returned either '' || null || NotSupported");
                locControlType = Convert.ToString(locControlType, CultureInfo.CurrentCulture).ToUpper(CultureInfo.CurrentCulture);
            }

            do
            {
                if (tries++ > TRIES)
                    ThrowMe(checkType, "Tried " + tries + " times to get to other position than current(" + curValue + ")");

                if (controlType == ControlType.Custom)
                {
                    switch (locControlType.ToString())
                    {
                        case "IP ADDRESS":
                            {
                                break;
                            }
                    }
                    break;
                }
                else
                {
                    tryVal = (double)Helpers.RandomValue(min, max);
                }

                // 
                if (CheckControlClass(m_le, "ProgressBar"))
                {
                    // "Verify: Calling RangeValuePattern.SetValue() throws an InvalidOperationException",
                    TSC_VerifySetValueOnRO(tryVal, CheckType.Verification);
                }
                else
                {
                    // Set the value
                    pattern_SetValue(tryVal);
                }

                // And now do a check to see if the value is different
            } while (pattern_Value == NotValue);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------------
        void TS_SetValue(double val, bool ReturnValue, CheckType checkType)
        {
            if (CheckControlClass(m_le, "ProgressBar"))
            {
                // "Verify: Calling RangeValuePattern.SetValue() throws an InvalidOperationException",
                TSC_VerifySetValueOnRO(val, CheckType.Verification);
            }
            else
            {
                pattern_SetValue(val);
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------------
        void TSC_VerifyReadOnly(bool IsReadOnly, CheckType checkType)
        {
            if (!CheckControlClass(m_le, "ProgressBar"))
            {
                if (!patternIsReadOnly.Equals(IsReadOnly))
                    ThrowMe(checkType, "IsReadOnly != " + IsReadOnly);
            }

            Comment("IsReadOnly is " + IsReadOnly);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------------
        /// <summary>Verify ProgressBar's SetValue() should throw InvalidOperationException</summary>
        /// -------------------------------------------------------------------------
        void TSC_VerifySetValueOnRO(double val, CheckType checktype)
        {
            try
            {
                pattern_SetValue(val);
            }
            catch (InvalidOperationException)
            {
                //m_TestStep++;
                return;
            }

            ThrowMe(checktype, "Calling RangeValuePattern.SetValue on ProgressBar should throw InvalidOperationException");
        }

        /// -------------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------------
        void TS_VerifyValue(object ExpectedValue, bool ShouldBeEqual, CheckType checkType)
        {
            // 
            if (!CheckControlClass(m_le, "ProgressBar"))
            {
                Comment("Current value == " + pattern_Value.ToString(CultureInfo.CurrentCulture));

                double CurrentValue = pattern_Value;
                double dExpectedValue = Convert.ToDouble(ExpectedValue, CultureInfo.CurrentCulture);

                if (ShouldBeEqual)
                {   // equal

                    if (GenericMath.CompareTo(CurrentValue, dExpectedValue) != 0)
                    {
                        ThrowMe(checkType, "Value() returned " + CurrentValue + " but was expecting " + ExpectedValue);
                    }
                }
                else
                {   // not equal

                    if (GenericMath.CompareTo(CurrentValue, dExpectedValue) == 0)
                    {
                        ThrowMe(checkType, "Value() returned " + CurrentValue + " but was expecting " + ExpectedValue);
                    }
                }
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------------
        void TSC_VerifyMinLessThanMax(CheckType checkType)
        {
            double min = Convert.ToDouble(patternMinimum, CultureInfo.CurrentCulture);
            double max = Convert.ToDouble(patternMaximum, CultureInfo.CurrentCulture);

            if (!(min < max))
                ThrowMe(checkType, "Minimum(" + min + ") !<= Maximum(" + max + ")");

            Comment("RangeValue.Minimun(" + min + ") < RangeValue.Maximum(" + max + ")");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------------
        void TSC_VerifyMinNotEqualMax(CheckType checkType)
        {
            double min = Convert.ToDouble(patternMinimum, CultureInfo.CurrentCulture);
            double max = Convert.ToDouble(patternMaximum, CultureInfo.CurrentCulture);

            if (min == max)
                ThrowMe(checkType, "RangeValue.Minimum(" + min + ") == " + "RangeValue.Maximum(" + max + ")");

            Comment("RangeValue.Minimun(" + min + ") != " + "RangeValue.Maximum(" + max + ")");
            m_TestStep++;
        }

        #endregion Test Steps

        #region Helpers
        /// <summary>
        /// Verify the control's class name
        /// </summary>
        /// <param name="element">element to eval</param>
        /// <param name="controlClass">class name to verify against</param>
        /// <returns>true if match</returns>
        internal bool CheckControlClass(AutomationElement element, string controlClass)
        {
            string className = (string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
            return className == controlClass;
        }
        #endregion

    }
}
