// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//******************************************************************* 
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
    public class TextScenarioTests : ScenarioObject, IDisposable
    {

        #region Member Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        const string        THIS = "TextScenarioTests";
        ManualResetEvent    _NotifiedEvent;
        TextPattern         _pattern;
        TextTestsHelper     _tth;    // Allows us to re-use existing helpers from non-scenario text tests

        bool                _supportsText = true;

        #endregion Member Variables

        #region constructor

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="typeOfControl")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="typeOfPattern")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="dirResults")]
        public TextScenarioTests(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, TypeOfControl.UnknownControl, TypeOfPattern.Unknown, null, testEvents, commands)
        {
            try
            {
                _tth = new TextTestsHelper(element, testSuite, priority, TypeOfControl.UnknownControl, TypeOfPattern.Unknown, null, testEvents, commands);
            }
            catch( Exception ex )
            {
                _supportsText = false;
                Comment("Unknown exception raised: " + ex.ToString() );
            }
            _NotifiedEvent = new System.Threading.ManualResetEvent(false);
        }

        #endregion constructor

        #region Scenarios

        #region Scenario: move / count

        /// -------------------------------------------------------------------
        /// <summary>
        /// Count / Move (iterate) through text for each TextUnit via a variety of scenarios
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Move / Count Scenario",
            TestSummary = "Count / Move (iterate) through text for each TextUnit via a variety of scenarios",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Arguments,
            Client = Client.ATG,
            Description = new string[] {
                    "Pre-condition: Get Scenario Pre-Conditions",
                    "Pre-condition: Set text <<sampleText argument>>",
                    "Pre-condition: Determine actual count of Text Units",
                    "Execute counting scenarios & display results", 
        })]

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        public void TextMoveCountScenario1(TestCaseAttribute testCase, object[] arguments)
        {
            if (arguments == null)
                throw new ArgumentException("arguments is null");

            if (arguments[0] == null)
                throw new ArgumentException("arguments[0] is null");

            HeaderComment(testCase);

            System.Diagnostics.Debug.Assert(arguments.Length == 1);
            System.Diagnostics.Debug.Assert(arguments[0] is TextWrapper.SampleText);
            TextWrapper.SampleText sampleText = (TextWrapper.SampleText)arguments[0];

            int[] textCounts = new int[((int)TextUnit.Document) + 1];

            // Pre-condition: Get Scenario Pre-Conditions
            TS_ScenarioPreConditions(true, CheckType.IncorrectElementConfiguration);

            // Pre-condition: Set text <<sampleText>>
            TS_SetText(sampleText, CheckType.Verification);

            // Pre-condition: Determine actual count of Text Units
            TS_GetTextCounts(ref textCounts, CheckType.Verification);

            // Execute 9 different counting scenarios
            TS_MoveCountScenarios1(textCounts, CheckType.Verification);
        }

        #endregion Scenario: move / count

        #region Scenario: Exception Madness
        /// -------------------------------------------------------------------
        /// <summary>
        /// This scenario trips up TextPattern/TextPatternRange error handling
        /// by passing in bogus arguments, incorrect arguments, arguments that
        /// (extistentially speaking) don't exist, etc.
        /// Also throws some UIA defined exceptions just for grins and giggles.
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Exception Madness",
            TestSummary = "Miscellaneous code to generate all kinds of exceptions",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ATG,
            Description = new string[] {
                    "Pre-condition: Get Scenario Pre-Conditions",
                    "Pre-condition: Set text 'String 1 String 2 String 3'",
                    "Throw generic UIA exceptions with all constructors", 
                    "Engage TextPattern error handling with conditions that will raise exceptions",
                    "Engage TextPatternRange error handling with conditions that will raise exceptions",
        })]
        public void ExceptionMadness(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            // Pre-condition: Get Scenario Pre-Conditions
            TS_ScenarioPreConditions(true, CheckType.IncorrectElementConfiguration);

            // Pre-condition: Set text 'string 1 String 2 String 3'
            TS_SetText(TextWrapper.SampleText.EasyText, CheckType.Verification);

            // Throw generic UIA exceptions with all constructors
            TS_ThrowUIAExceptions();

            // Engage TextPattern error handling with conditions that will raise exceptions 
            TS_ThrowTextPatternExceptions();

            // Engage TextPatternRange error handling with conditions that will raise exceptions
            TS_ThrowTextPatternRangeExceptions();
        }

        #endregion Scenario: Exception Madness

        #region Scenario: Attribute Madness

        /// -------------------------------------------------------------------
        /// <summary>
        /// Enumerate though supported attributes for FindAttribute/GetAttribute methods
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Attribute Madness",
            TestSummary = "Enumerate though supported attributes for FindAttribute/GetAttribute methods",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ATG,
            Description = new string[] {
                    "Pre-condition: Get Scenario Pre-Conditions",
                    "Pre-condition: Set text to <<SampleText>>",
                    "Call GetAttribute/FindAttribute for all possible attributes", 
        })]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        public void AttributeMadness(TestCaseAttribute testCase, object[] arguments)
        {
            if (arguments == null)
                throw new ArgumentException("arguments is null");

            if (arguments[0] == null)
                throw new ArgumentException("arguments[0] is null");

            HeaderComment(testCase);

            System.Diagnostics.Debug.Assert(arguments.Length == 1);
            System.Diagnostics.Debug.Assert(arguments[0] is TextWrapper.SampleText);
            TextWrapper.SampleText sampleText = (TextWrapper.SampleText)arguments[0];

            // Pre-condition: Get Scenario Pre-Conditions
            TS_ScenarioPreConditions(true, CheckType.IncorrectElementConfiguration);

            // Pre-condition: Set text 'string 1 String 2 String 3'
            TS_SetText(sampleText, CheckType.Verification);

            // Lets get and find some attributes!
            TS_AttributeMadness();
        }

        #endregion Scenario: Attribute Madness

        #region Scenario: Kitchen Sink for Code Coverage

        /// -------------------------------------------------------------------
        /// <summary>
        /// Misc. code for code coverage until Text P1s/P2s come fully online
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Kitchen Sink Scenario",
            TestSummary = "Misc. code for code coverage until Text P1s/P2s come fully online",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario | TestCaseType.Arguments,
            Client = Client.ATG,
            Description = new string[] {
                    "Pre-condition: Get Scenario Pre-Conditions",
                    "Pre-condition: Set text <<sampleText argument>>",
                    "FindText Coverage", 
                    "GetVisibleRange Coverage", 
                    "RangeFromPoint Coverage", 
        })]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        public void KitchenSink(TestCaseAttribute testCase, object[] arguments)
        {
            if (arguments == null)
                throw new ArgumentException("arguments is null");

            if (arguments[0] == null)
                throw new ArgumentException("arguments[0] is null");

            HeaderComment(testCase);

            System.Diagnostics.Debug.Assert(arguments.Length == 1);
            System.Diagnostics.Debug.Assert(arguments[0] is TextWrapper.SampleText);
            TextWrapper.SampleText sampleText = (TextWrapper.SampleText)arguments[0];

            // Pre-condition: Get Scenario Pre-Conditions
            TS_ScenarioPreConditions(true, CheckType.IncorrectElementConfiguration);

            // Pre-condition: Set text <<sampleText>>
            TS_SetText(sampleText, CheckType.Verification);

            // ignore case and forward/backwards Coverage 
            TS_Coverage_FindText();

            // RangeFromPoint Coverage 
            TS_Coverage_RangeFromPoint();
        }

        #endregion Scenario: Kitchen Sink for Code Coverage

        #region Scenario: EditSink Scenario

        /// -------------------------------------------------------------------
        /// <summary>
        /// This scenario exercises the ValuePattern support in WindowsRichEdit
        /// In theory this should be exercised by the regular UIVerify 
        /// ValuePattern support. Just
        /// </summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("EditSink Scenario",
            TestSummary = "Miscellaneous code to generate all kinds of exceptions",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ATG,
            Description = new string[] {
                    "Pre-condition: Get Scenario Pre-Conditions",
                    "Pre-condition: Validate Is MultiLine control = false",
                    "Pre-condition: Verify Value is expected value: Set Value to 123456789",
                    "Pre-condition: Acquire ValuePattern",
                    "Call ValuePattern.SetValue()", 
                    "Get  ValuePattern.Value property", 
                    "Call ValuePattern.SetValue(Non-numeric value) raising ArgumentException for numeric-only fields", 
                    "Call ValuePattern.SetValue(\"123456789\") on control whose limit is set to 1 character",
        })]
        public void EditSinkScenario(TestCaseAttribute testCase)
        {
            ValuePattern valuePattern = null;

            HeaderComment(testCase);

            // Pre-condition: Get Scenario Pre-Conditions
            TS_ScenarioPreConditions(false, CheckType.IncorrectElementConfiguration);
            
            // Precondition: Control must supports multi-line = <<expectedResult>>
            _tth.TS_IsMultiLine(false, CheckType.IncorrectElementConfiguration);

            // Pre-condition: Verify Value is expected value: Set Value to 123456789
            TS_SetText(TextWrapper.SampleText.Num123, CheckType.Verification);

            // Get value Pattern
            TS_GetValuePattern(out valuePattern, CheckType.Verification);

            // Call ValuePattern.SetValue()
            TS_SetValue(valuePattern, CheckType.Verification);

            // Get ValuePattern.Value property
            TS_Value(valuePattern, CheckType.Verification);

            // Call ValuePattern.SetValue(Non-numeric value) raising ArgumentException for numeric-only fields
            TS_SetValueAlpha( valuePattern, CheckType.Verification );

            // Call ValuePattern.SetValue("123456789") on control whose limit is set to 1 character
            TS_SetValueOversize(valuePattern, CheckType.Verification);
        }

        #endregion Scenario: EditSink Scenario

        #endregion scenarios

        #region Test Step


        // -------------------------------------------------------------------
        // Conduct move / count scenario (iterating by textunit)
        // -------------------------------------------------------------------
        private void TS_GetValuePattern(out ValuePattern valuePattern, CheckType checkType)
        {

            valuePattern = null; 
            
            try
            {
                valuePattern = m_le.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;

            }
            catch (Exception actualException)
            {
                if (Library.IsCriticalException(actualException))
                    throw;

                TestException(null, actualException.GetType(), "TS_GetValuePattern", checkType);
            }

            if (valuePattern == null)
                ThrowMe(checkType, "Unable to acquire ValuePattern for this control");
            else
                Comment("Successfully acquired ValuePattern for this control" );

            m_TestStep++;
        }

        // -------------------------------------------------------------------
        // Get ValuePattern.Value property
        // -------------------------------------------------------------------
        private void TS_Value(ValuePattern valuePattern, CheckType checkType)
        {
            bool isPassword = m_le.Current.IsPassword;
            string value = "";

            try
            {
                value = valuePattern.Current.Value;
            }
            catch (Exception actualException)
            {
                InvalidOperationException ex = new InvalidOperationException();

                if (Library.IsCriticalException(actualException))
                    throw;

                TestException(ex.GetType(), actualException.GetType(), "TS_Value", checkType);
            }

            Comment("ValuePattern.Value = " + value);
            m_TestStep++;
        }

        // -------------------------------------------------------------------
        // Call ValuePattern.SetValue()
        // -------------------------------------------------------------------
        private void TS_SetValue(ValuePattern valuePattern, CheckType checkType)
        {
            bool isReadOnly = valuePattern.Current.IsReadOnly;
            bool isPassword = m_le.Current.IsPassword;
            string value = "1";
            IntPtr hwnd = Helpers.CastNativeWindowHandleToIntPtr(m_le);

            try
            {
                valuePattern.SetValue(value);
            }
            catch (ElementNotEnabledException actualException)
            {
                if (SafeNativeMethods.IsWindowEnabled(hwnd) == false)
                    Comment("As expected, unable to set value of disabled control");
                else
                    TestException(null, actualException.GetType(), "TS_SetValue", checkType);
            }
            catch (InvalidOperationException actualException)
            {
                if (isReadOnly == true)
                {
                    Comment("As expected, unable to set value of read-only control (InvalidOperationException)");
                    m_TestStep++;
                    return;
                }
                else
                    TestException(null, actualException.GetType(), "TS_SetValue", checkType);
            }
            catch (UnauthorizedAccessException actualException)
            {
                if (isPassword == true)
                {
                    Comment("As expected, unable to set value of password control (UnauthorizedAccessException)");
                    m_TestStep++;
                    return;
                }
                else
                    TestException(null, actualException.GetType(), "TS_SetValue", checkType);
            }
            catch (Exception actualException)
            {
                if (Library.IsCriticalException(actualException))
                    throw;

                TestException(null, actualException.GetType(), "TS_SetValue", checkType);
            }

            Comment("Successfully called ValuePattern.SetValue(" + value + ")");
            m_TestStep++;
        }


        // -------------------------------------------------------------------
        // Call ValuePattern.SetValue(Non-numeric value) raising ArgumentException for numeric-only fields
        // -------------------------------------------------------------------
        private void TS_SetValueAlpha(ValuePattern valuePattern, CheckType checkType)
        {
            bool isReadOnly = valuePattern.Current.IsReadOnly;
            bool isPassword = m_le.Current.IsPassword;
            string value = "A";
            IntPtr hwnd = Helpers.CastNativeWindowHandleToIntPtr(m_le);

            try
            {
                valuePattern.SetValue(value);
            }
            catch (ArgumentException)
            {
                Comment("As expected, unable to set value of numeric only control with an alpha numeric string ('A')");
                m_TestStep++;
                return;
            }
            catch (InvalidOperationException actualException)
            {
                if (isReadOnly == true)
                {
                    Comment("As expected, unable to set value of read-only control (InvalidOperationException)");
                    m_TestStep++;
                    return;
                }
                else
                    TestException(null, actualException.GetType(), "TS_SetValue", checkType);
            }
            catch (UnauthorizedAccessException actualException)
            {
                if (isPassword == true)
                {
                    Comment("As expected, unable to set value of password control (UnauthorizedAccessException)");
                    m_TestStep++;
                    return;
                }
                else
                    TestException(null, actualException.GetType(), "TS_SetValue", checkType);
            }
            catch (Exception actualException)
            {
                if (Library.IsCriticalException(actualException))
                    throw;

                TestException(null, actualException.GetType(), "TS_SetValue", checkType);
            }

            Comment("Successfully called ValuePattern.SetValue(\"A\"), must be control that is not numeric-only");
            m_TestStep++;
        }

        // -------------------------------------------------------------------
        // Call ValuePattern.SetValue("123456789") on control whose limit is set to 1 character
        // -------------------------------------------------------------------
        private void TS_SetValueOversize(ValuePattern valuePattern, CheckType checkType)
        {
            bool isReadOnly = valuePattern.Current.IsReadOnly;
            bool isPassword = m_le.Current.IsPassword;
            string value = "123456789";
            IntPtr _hwnd = Helpers.CastNativeWindowHandleToIntPtr(m_le);
            NativeMethods.HWND hwnd = NativeMethods.HWND.Cast(_hwnd);
            IntPtr wParam = new IntPtr(1);
            IntPtr result;

            int    lastWin32Error    = Marshal.GetLastWin32Error();
            int    resultInt;

            IntPtr resultSendMessage = UnsafeNativeMethods.SendMessageTimeout(hwnd, NativeMethods.EM_LIMITTEXT, wParam, IntPtr.Zero, 1, 10000, out result);
            if (resultSendMessage == IntPtr.Zero)
            {
                throw new InvalidOperationException("SendMessageTimeout() timed out");
            }
            resultInt = unchecked((int)(long)result);

            try
            {
                valuePattern.SetValue(value);
            }
            catch (InvalidOperationException)
            {
                Comment("As expected, unable to set value of control limited to 1 character with a string \"123456789\"");
            }
            catch (UnauthorizedAccessException actualException)
            {
                if (isPassword == true)
                {
                    Comment("As expected, unable to set value of password control (UnauthorizedAccessException)");
                    m_TestStep++;
                    return;
                }
                else
                    TestException(null, actualException.GetType(), "TS_SetValue", checkType);
            }
            catch (Exception actualException)
            {
                if (Library.IsCriticalException(actualException))
                    throw;

                TestException(null, actualException.GetType(), "TS_SetValue", checkType);
            }

            Comment("Successfully called ValuePattern.SetValue(\"A\"), must be control that is not numeric-only");
            m_TestStep++;
        }
            
        // -------------------------------------------------------------------
        // Call GetATtributeValue/FindATtribute for each attribute
        // -------------------------------------------------------------------
        private void TS_AttributeMadness()
        {
            AnimationStyleAttribute();
            BackgroundColorAttribute();
            BulletStyleAttribute();
            CapStyleAttribute();
            CultureAttribute();
            FontNameAttribute();
            FontSizeAttribute();
            FontWeightAttribute();
            ForegroundColorAttribute();
            HorizontalTextAlignmentAttribute();
            IndentationFirstLineAttribute();
            IndentationLeadingAttribute();
            IndentationTrailingAttribute();
            IsHiddenAttribute();
            IsItalicAttribute();
            IsReadOnlyAttribute();
            IsSubscriptAttribute();
            IsSuperscriptAttribute();
            MarginBottomAttribute();
            MarginLeadingAttribute();
            MarginTopAttribute();
            MarginTrailingAttribute();
            OutlineStylesAttribute();
            OverlineColorAttribute();
            OverlineStyleAttribute();
            StrikethroughColorAttribute();
            StrikethroughStyleAttribute();
            TabsAttribute();
            TextFlowDirectionsAttribute();
            UnderlineColorAttribute();
            UnderlineStyleAttribute();
        }

        // -------------------------------------------------------------------
        // ignore case and forward/backwards Coverage 
        // -------------------------------------------------------------------

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "InternalHelper.Tests.Patterns.TextWrapper.Range_FindText(System.Windows.Automation.Text.TextPatternRange,System.Windows.Automation.Text.TextPatternRange@,System.String,System.Boolean,System.Boolean,System.Type,Microsoft.Test.WindowsUIAutomation.TestManager.CheckType)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        private void TS_Coverage_FindText()
        {
            Type type = null;
            ArgumentException aeEx = new ArgumentException("FAKE Argument");
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange returnedRange = null;
            string text = callingRange.GetText(-1);

            TextTestsHelper.TrimTrailingCRLF(m_le, ref text);

            if (string.IsNullOrEmpty(text) == true)
            {
                type = aeEx.GetType();
            }

            // Empty text
            _tth.Range_FindText(callingRange, ref returnedRange, "", true, true, aeEx.GetType(), CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, "", true, false, aeEx.GetType(), CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, "", false, true, aeEx.GetType(), CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, "", false, true, aeEx.GetType(), CheckType.Verification);

            // Non-matching text
            _tth.Range_FindText(callingRange, ref returnedRange, "ATG", true, true, null, CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, "ATG", true, false, null, CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, "ATG", false, true, null, CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, "ATG", false, true, null, CheckType.Verification);

            // Matching Text with matching case
            text = text.Substring(0, ((text.Length / 2)));
            _tth.Range_FindText(callingRange, ref returnedRange, text, true, true, type, CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, text, true, false, type, CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, text, false, true, type, CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, text, false, true, type, CheckType.Verification);

            // Matching Text with mis-matched case
            text = text.ToUpperInvariant();
            _tth.Range_FindText(callingRange, ref returnedRange, text, true, true, type, CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, text, true, false, type, CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, text, false, true, type, CheckType.Verification);
            _tth.Range_FindText(callingRange, ref returnedRange, text, false, true, type, CheckType.Verification);

            Comment("Called FindText kitchen sink");
            m_TestStep++;
        }

        // -------------------------------------------------------------------
        // RangeFromPoint Coverage 
        // -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        private void TS_Coverage_RangeFromPoint()
        {
            object temp = m_le.GetCurrentPropertyValue(
                                                    AutomationElement.BoundingRectangleProperty);
            Rect autoElementRect = (Rect)temp; ;
            Rect[] boundRects = new Rect[0];
            Point screenLocation = new Point();
            ArgumentException aeEx = new ArgumentException("bogus argument exception");
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange returnedRange = null;

            _tth.Range_GetBoundingRectangles(callingRange, ref boundRects, null, CheckType.Verification);

            // Point is center of topLeft corner
            screenLocation.X = autoElementRect.Left + 1;
            screenLocation.Y = autoElementRect.Top + 1;
            _tth.Pattern_RangeFromPoint(_pattern, ref returnedRange, screenLocation, null, CheckType.Verification);

            // Point is middle
            screenLocation.X = (autoElementRect.Left + autoElementRect.Right) / 2;
            screenLocation.Y = (autoElementRect.Top + autoElementRect.Bottom) / 2;
            _tth.Pattern_RangeFromPoint(_pattern, ref returnedRange, screenLocation, null, CheckType.Verification);

            // Point is inside bottom right
            screenLocation.X = autoElementRect.Right - 1;
            screenLocation.Y = autoElementRect.Bottom - 1;
            _tth.Pattern_RangeFromPoint(_pattern, ref returnedRange, screenLocation, null, CheckType.Verification);

            // Point is outside bottom right
            screenLocation.X = autoElementRect.Right + 1;
            screenLocation.Y = autoElementRect.Bottom + 1;
            _tth.Pattern_RangeFromPoint(_pattern, ref returnedRange, screenLocation, aeEx.GetType(), CheckType.Verification);

            // Point is outside top left
            screenLocation.X = autoElementRect.Left - 1;
            screenLocation.Y = autoElementRect.Top - 1;
            _tth.Pattern_RangeFromPoint(_pattern, ref returnedRange, screenLocation, aeEx.GetType(), CheckType.Verification);

            // Point is outsdie autoElement
            screenLocation.X = autoElementRect.Left - 1;
            screenLocation.Y = autoElementRect.Top - 1;
            _tth.Pattern_RangeFromPoint(_pattern, ref returnedRange, screenLocation, aeEx.GetType(), CheckType.Verification);

            // Point is outsdie autoElement
            screenLocation.X = Int32.MaxValue;
            screenLocation.Y = Int32.MaxValue;
            _tth.Pattern_RangeFromPoint(_pattern, ref returnedRange, screenLocation, aeEx.GetType(), CheckType.Verification);


            m_TestStep++;
        }

        // -------------------------------------------------------------------
        // Fill in array with correct text counts for the pattern
        // -------------------------------------------------------------------
        private void TS_GetTextCounts(ref int[] textCounts, CheckType checkType)
        {
            if (textCounts.Length != (((int)TextUnit.Document) + 1))
            {
                ThrowMe(checkType, "Invalid array size for TextUnit counts, Expected " +
                    (((int)TextUnit.Document) + 1) +
                    ", Actual = " + textCounts.Length);
            }

            for (TextUnit tu = TextUnit.Character; tu <= TextUnit.Document; tu++)
            {
                textCounts[(int)tu] = TextLibrary.CountTextUnit(tu, _pattern.DocumentRange);
            }

            Comment("Obtained correct text counts for each TextUnit");
            m_TestStep++;
        }

        // -------------------------------------------------------------------
        // Conduct move / count scenario (iterating by textunit)
        // -------------------------------------------------------------------
        private void TS_MoveCountScenarios1(int[] textCounts, CheckType checkType)
        {
            int[] textCountEmpty = new int[((int)TextUnit.Document) + 1];
            StringBuilder errors = new StringBuilder();

            // Initialize empty text counts
            for (int i = 0; i < textCountEmpty.Length; i++)
                textCountEmpty[i] = 0;

            MoveCount_DisplayMetaData();

            MoveCount_DisplayTextUnitHeader();
            MoveCount_DisplayTextUnitFooter();

            MoveCount_CorrectData(textCounts, checkType);

            MoveCount_DisplayTextUnitFooter();

            MoveCount_MoveEPBU(TextPatternRangeEndpoint.Start, Int32.MaxValue, textCounts, ref errors);
            MoveCount_MoveEPBU(TextPatternRangeEndpoint.End, Int32.MinValue, textCounts, ref errors);
            MoveCount_Move(TextPatternRangeEndpoint.Start, Int32.MaxValue, textCounts, ref errors, checkType);
            MoveCount_Move(TextPatternRangeEndpoint.End, Int32.MinValue, textCounts, ref errors, checkType);

            MoveCount_ExpandToEnclose(textCounts, ref errors, checkType);

            MoveCount_DisplayTextUnitFooter();

            MoveCount_MoveEPBU(TextPatternRangeEndpoint.Start, Int32.MinValue, textCountEmpty, ref errors);
            MoveCount_MoveEPBU(TextPatternRangeEndpoint.End, Int32.MaxValue, textCountEmpty, ref errors);
            MoveCount_Move(TextPatternRangeEndpoint.Start, Int32.MinValue, textCountEmpty, ref errors, checkType);
            MoveCount_Move(TextPatternRangeEndpoint.End, Int32.MaxValue, textCountEmpty, ref errors, checkType);

            MoveCount_DisplayTextUnitFooter();

            if (errors.ToString().Length > 0)
            {
                Comment("The following errors were encountered\n" + errors.ToString());
            }
            else
                Comment("Text Move / Count variations revealed no errors(!)");

            m_TestStep++;
        }

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
                ThrowMe(
                    CheckType.Verification, "Unable to get AutomationElement for control with focus");
            }

            // Give info about control
            GetClassName(m_le, out className, out localizedControlType);
            Comment("Automation ID = " + m_le.Current.AutomationId.ToString() +
                    "           (" + className + " / " + localizedControlType + ")");

            try
            {
                _pattern = m_le.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            }
            catch (InvalidOperationException)
            {
                Comment("Unable to get a _textPattern for this control (likely a disabled control)");
                _supportsText = false;
            }
            catch (Exception exception)
            {
                if (Library.IsCriticalException(exception))
                    throw;

                Comment("Acquiring TextPattern for automation element with focus raised exception");
                Comment("  Message = " + exception.Message);
                Comment("  Type    = " + exception.GetType().ToString());
                ThrowMe(CheckType.Verification, "Unable to proceed with test, should not have received exception acquiring TextPattern");  // hard-coded... on purpose
            }

            // Do we have a valid text pattern?    
            if ((_pattern == null) && (requireTextPattern == true ))
            {
                ThrowMe(CheckType.Verification, "Unable to proceed with test, should not have received exception acquiring TextPattern");  // hard-coded... on purpose
            }

            // Would be nice to have this information.            
            if (m_le.Current.AutomationId.ToString().Length == 0)
            {
                ThrowMe(checkType, "Unable to determine automation id of control");
            }

            m_TestStep++;
        }

        // -------------------------------------------------------------------
        // Sets the text for a control
        // -------------------------------------------------------------------
        private void TS_SetText(TextWrapper.SampleText sampleText, CheckType checkType)
        {
            string textActual = "";
            
            if( _supportsText == false )
            {
                Comment("Unable to set text, continuing");
                m_TestStep++;
                return;
            }
            
            // Calling into immense library of TextWrapper methods.
            _tth.TS_SetText(sampleText, out textActual, checkType);
        }

        // -------------------------------------------------------------------
        // Throw generic UIA exceptions
        // -------------------------------------------------------------------

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Automation.ProxyAssemblyNotLoadedException.#ctor(System.String,System.Exception)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Automation.ProxyAssemblyNotLoadedException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Automation.ElementNotAvailableException.#ctor(System.String,System.Exception)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Automation.ElementNotAvailableException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Automation.ElementNotEnabledException.#ctor(System.String,System.Exception)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Automation.ElementNotEnabledException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Exception.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]

        static private void TS_ThrowUIAExceptions()
        {
            Comment("Throwing UIA defined exceptions");

            //---------------------
            // ElementNotAvailable
            //---------------------

            try
            {
                throw new ElementNotAvailableException();
            }
            catch (ElementNotAvailableException ex)
            {
                CrackException(ex);
            }
            try
            {
                throw new ElementNotAvailableException("Bogus");
            }
            catch (ElementNotAvailableException ex)
            {
                CrackException(ex);
            }
            try
            {
                throw new ElementNotAvailableException("Bogus w/Inner Bogus", new Exception("Inner Bogus"));
            }
            catch (ElementNotAvailableException ex)
            {
                CrackException(ex);
            }

            //----------------------------
            // ElementNotEnabledException
            //----------------------------

            try
            {
                throw new ElementNotEnabledException();
            }
            catch (ElementNotEnabledException ex)
            {
                CrackException(ex);
            }
            try
            {
                throw new ElementNotEnabledException("Bogus");
            }
            catch (ElementNotEnabledException ex)
            {
                CrackException(ex);
            }
            try
            {
                throw new ElementNotEnabledException("Bogus w/Inner Bogus", new Exception("Inner Bogus"));
            }
            catch (ElementNotEnabledException ex)
            {
                CrackException(ex);
            }

            //---------------------------------
            // ProxyAssemblyNotLoadedException
            //---------------------------------

            try
            {
                throw new ProxyAssemblyNotLoadedException();
            }
            catch (ProxyAssemblyNotLoadedException ex)
            {
                CrackException(ex);
            }
            try
            {
                throw new ProxyAssemblyNotLoadedException("Bogus");
            }
            catch (ProxyAssemblyNotLoadedException ex)
            {
                CrackException(ex);
            }
            try
            {
                throw new ProxyAssemblyNotLoadedException("Bogus w/Inner Bogus", new Exception("Inner Bogus"));
            }
            catch (ProxyAssemblyNotLoadedException ex)
            {
                CrackException(ex);
            }

            m_TestStep++;
        }

        // -------------------------------------------------------------------
        // Create error conditions for TextPattern methods and raise exceptions
        // -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        private void TS_ThrowTextPatternExceptions()
        {
            TextPatternRange returnedRange = null;
            Point screenLocation = new Point();
            ArgumentException aeEx = new ArgumentException("<<Fake argument>>");
            ArgumentNullException anEx = new ArgumentNullException("<<Fake null argument>>");

            //----------------
            // RangeFromPoint
            //----------------

            screenLocation.X = Int32.MaxValue;
            screenLocation.Y = Int32.MinValue;

            // ArgumentException � if given point is outside of AutomationElement of the control
            _tth.Pattern_RangeFromPoint(_pattern, ref returnedRange, screenLocation, aeEx.GetType(), CheckType.Verification);

            //----------------
            // RangeFromChild
            //----------------

            // ArgumentNullException � if returnedRange is null 
            _tth.Pattern_RangeFromChild(_pattern, ref returnedRange, null, anEx.GetType(), CheckType.Verification);

            //// InvalidOperationException � if the child could not have come from this container.
            // 


            m_TestStep++;
        }

        // -------------------------------------------------------------------
        // Create error conditions for TextPatternRange methods and raise exceptions
        // -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "InternalHelper.Tests.Patterns.TextWrapper.Range_FindText(System.Windows.Automation.Text.TextPatternRange,System.Windows.Automation.Text.TextPatternRange@,System.String,System.Boolean,System.Boolean,System.Type,Microsoft.Test.WindowsUIAutomation.TestManager.CheckType)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        private void TS_ThrowTextPatternRangeExceptions()
        {
            int results = 0;
            bool isEqual = false;
            string text = "";
            object nullObject = null;
            TextPattern differentTextPattern = TextTestsHelper.GetDifferentTextPattern(m_le, CheckType.IncorrectElementConfiguration);
            TextPatternRange targetRange = null;

            ArgumentException aeEx = new ArgumentException("<<Fake argument>>");
            ArgumentNullException anEx = new ArgumentNullException("<<Fake null argument>>");
            TextPatternRange callingRange = null;
            ArgumentOutOfRangeException aoreEx = new ArgumentOutOfRangeException();

            //---------
            // Compare
            //---------

            // ArgumentNullException � if the range is null.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_Compare(callingRange, null, ref isEqual, anEx.GetType(), CheckType.Verification);

            // ArgumentException � if range is from another container.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_Compare(callingRange, differentTextPattern.DocumentRange, ref isEqual, aeEx.GetType(), CheckType.Verification);

            //------------------
            // CompareEndPoints
            //------------------

            // ArgumentNullException � if the range is null.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_CompareEndpoints(callingRange, TextPatternRangeEndpoint.Start, null,
                    TextPatternRangeEndpoint.Start, ref results, anEx.GetType(), CheckType.Verification);

            // ArgumentException � if range is from another container.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_CompareEndpoints(callingRange, TextPatternRangeEndpoint.Start, differentTextPattern.DocumentRange,
                    TextPatternRangeEndpoint.Start, ref results, aeEx.GetType(), CheckType.Verification);

            //---------------
            // FindAttribute
            //---------------

            // ArgumentException � if wrong type is specified.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute,
                    results, true, aeEx.GetType(), CheckType.Verification);

            // ArgumentNullException � if attribute value is null.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute,
                    null, true, anEx.GetType(), CheckType.Verification);

            // ArgumentNullException � if attribute is null.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_FindAttribute(callingRange, ref targetRange, null,
                    (object)text, true, anEx.GetType(), CheckType.Verification);

            //----------
            // FindText
            //----------

            // ArgumentNullException � if null is applied for search text argument.
            _tth.Range_FindText( callingRange, ref targetRange, null, true, true, anEx.GetType() , CheckType.Verification );

            // ArgumentException �if empty string is applied for search text argument.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_FindText(callingRange, ref targetRange, "", true, true, aeEx.GetType(), CheckType.Verification);

            //-------------------
            // GetAttributeValue
            //-------------------            

            // ArgumentNullException � if attribute is null.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_GetAttributeValue(callingRange, null, ref nullObject, anEx.GetType(), CheckType.Verification);

            //---------
            // GetText
            //---------

            // Shoduld return ArgumentOutOfRangeException per the spec.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_GetText(callingRange, ref text, Int32.MinValue, aoreEx.GetType(), CheckType.Verification);
            
            //---------------------
            // MoveEndpointByRange
            //---------------------

            // ArgumentNullException � if targetRange is null.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_MoveEndpointByRange(callingRange, TextPatternRangeEndpoint.Start, null,
                    TextPatternRangeEndpoint.Start, anEx.GetType(), CheckType.Verification);

            // ArgumentException � if range is from a different container.
            callingRange = _pattern.DocumentRange.Clone();
            _tth.Range_MoveEndpointByRange(callingRange, TextPatternRangeEndpoint.Start, differentTextPattern.DocumentRange,
                    TextPatternRangeEndpoint.Start, aeEx.GetType(), CheckType.Verification);
            m_TestStep++;
        }

        #endregion Test Step

        #region Helpers

        #region Attribute Helpers

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for AnimationStyleAttribute
        // -------------------------------------------------------------------
        private void AnimationStyleAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * AnimationStyleAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.AnimationStyleAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            // Enumerate through all values going forward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.BlinkingBackground, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.LasVegasLights, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.MarchingBlackAnts, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.MarchingRedAnts, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.None, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.Other, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.Shimmer, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.SparkleText, true, null, CheckType.Verification);

            // Enumerate through all values going backward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.BlinkingBackground, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.LasVegasLights, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.MarchingBlackAnts, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.MarchingRedAnts, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.None, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.Other, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.Shimmer, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.AnimationStyleAttribute, (object)AnimationStyle.SparkleText, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for BackgroundColorAttribute
        // -------------------------------------------------------------------
        private void BackgroundColorAttribute()
        {
            int x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * BackgroundColorAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.BackgroundColorAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (int)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BackgroundColorAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BackgroundColorAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x--;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BackgroundColorAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BackgroundColorAttribute, x, false, null, CheckType.Verification);
        }


        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for BulletStyleAttribute
        // -------------------------------------------------------------------
        private void BulletStyleAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * BulletStyleAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.BulletStyleAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            // Enumerate through all values going forward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.DashBullet, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.FilledRoundBullet, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.FilledSquareBullet, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.HollowRoundBullet, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.HollowSquareBullet, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.None, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.Other, true, null, CheckType.Verification);

            // Enumerate through all values going backward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.DashBullet, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.FilledRoundBullet, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.FilledSquareBullet, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.HollowRoundBullet, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.HollowSquareBullet, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.None, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.BulletStyleAttribute, BulletStyle.Other, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for CapStyleAttribute
        // -------------------------------------------------------------------
        private void CapStyleAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * CapStyleAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.CapStyleAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            // Enumerate through all values going forward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.AllCap, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.AllPetiteCaps, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.None, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.Other, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.PetiteCaps, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.SmallCap, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.Titling, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.Unicase, true, null, CheckType.Verification);

            // Enumerate through all values going backward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.AllCap, false, null, CheckType.IncorrectElementConfiguration);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.AllPetiteCaps, false, null, CheckType.IncorrectElementConfiguration);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.None, false, null, CheckType.IncorrectElementConfiguration);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.Other, false, null, CheckType.IncorrectElementConfiguration);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.PetiteCaps, false, null, CheckType.IncorrectElementConfiguration);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.SmallCap, false, null, CheckType.IncorrectElementConfiguration);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.Titling, false, null, CheckType.IncorrectElementConfiguration);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CapStyleAttribute, CapStyle.Unicase, false, null, CheckType.IncorrectElementConfiguration);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for CultureAttribute
        // -------------------------------------------------------------------
        private void CultureAttribute()
        {
            CultureInfo cultureValue = null;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * CultureAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.CultureAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                cultureValue = (CultureInfo)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CultureAttribute, (object)cultureValue, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CultureAttribute, (object)cultureValue, false, null, CheckType.Verification);
            }

            //Create incorrect value
            cultureValue = new CultureInfo(0x0036); // Afrikaans culture

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CultureAttribute, (object)cultureValue, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.CultureAttribute, (object)cultureValue, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for FontNameAttribute
        // -------------------------------------------------------------------
        private void FontNameAttribute()
        {
            string x = "";
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * FontNameAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.FontNameAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (string)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontNameAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontNameAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x = "ATG";

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontNameAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontNameAttribute, x, false, null, CheckType.Verification);
        }


        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for FontSizeAttribute
        // -------------------------------------------------------------------
        private void FontSizeAttribute()
        {
            double x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * FontSizeAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.FontSizeAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (double)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontSizeAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontSizeAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x--;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontSizeAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontSizeAttribute, x, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for FontWeightAttribute
        // -------------------------------------------------------------------
        private void FontWeightAttribute()
        {
            int x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * FontWeightAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.FontWeightAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (int)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontWeightAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontWeightAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x--;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontWeightAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.FontWeightAttribute, x, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for ForegroundColorAttribute
        // -------------------------------------------------------------------
        private void ForegroundColorAttribute()
        {
            int x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * ForegroundColorAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.ForegroundColorAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (int)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.ForegroundColorAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.ForegroundColorAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x--;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.ForegroundColorAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.ForegroundColorAttribute, x, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for HorizontalTextAlignmentAttribute
        // -------------------------------------------------------------------
        private void HorizontalTextAlignmentAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * HorizontalTextAlignmentAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.HorizontalTextAlignmentAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Enumerate through all possible values forward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.HorizontalTextAlignmentAttribute, HorizontalTextAlignment.Centered, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.HorizontalTextAlignmentAttribute, HorizontalTextAlignment.Justified, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.HorizontalTextAlignmentAttribute, HorizontalTextAlignment.Left, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.HorizontalTextAlignmentAttribute, HorizontalTextAlignment.Right, true, null, CheckType.Verification);

            //Enumerate through all possible values backward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.HorizontalTextAlignmentAttribute, HorizontalTextAlignment.Centered, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.HorizontalTextAlignmentAttribute, HorizontalTextAlignment.Justified, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.HorizontalTextAlignmentAttribute, HorizontalTextAlignment.Left, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.HorizontalTextAlignmentAttribute, HorizontalTextAlignment.Right, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for IndentationFirstLineAttribute
        // -------------------------------------------------------------------
        private void IndentationFirstLineAttribute()
        {
            double x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * IndentationFirstLineAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.IndentationFirstLineAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (double)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationFirstLineAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationFirstLineAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x++;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationFirstLineAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationFirstLineAttribute, x, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for IndentationLeadingAttribute
        // -------------------------------------------------------------------
        private void IndentationLeadingAttribute()
        {
            double x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * IndentationLeadingAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.IndentationLeadingAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (double)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationLeadingAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationLeadingAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x++;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationLeadingAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationLeadingAttribute, x, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for IndentationTrailingAttribute
        // -------------------------------------------------------------------
        private void IndentationTrailingAttribute()
        {
            double x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * IndentationTrailingAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.IndentationTrailingAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (double)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationTrailingAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationTrailingAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x++;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationTrailingAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IndentationTrailingAttribute, x, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for IsHiddenAttribute
        // -------------------------------------------------------------------
        private void IsHiddenAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * IsHiddenAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.IsHiddenAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Find all correct/incorrect values
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsHiddenAttribute, true, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsHiddenAttribute, true, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsHiddenAttribute, false, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsHiddenAttribute, false, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for IsItalicAttribute
        // -------------------------------------------------------------------
        private void IsItalicAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * IsItalicAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.IsItalicAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Find all correct/incorrect values
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsItalicAttribute, true, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsItalicAttribute, true, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsItalicAttribute, false, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsItalicAttribute, false, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for IsReadOnlyAttribute
        // -------------------------------------------------------------------
        private void IsReadOnlyAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * IsReadOnlyAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.IsReadOnlyAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Find all correct/incorrect values
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsReadOnlyAttribute, true, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsReadOnlyAttribute, true, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsReadOnlyAttribute, false, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsReadOnlyAttribute, false, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for IsSubscriptAttribute
        // -------------------------------------------------------------------
        private void IsSubscriptAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * IsSubscriptAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.IsSubscriptAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Find correct value
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsSubscriptAttribute, true, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsSubscriptAttribute, true, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsSubscriptAttribute, false, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsSubscriptAttribute, false, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for IsSuperscriptAttribute
        // -------------------------------------------------------------------
        private void IsSuperscriptAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * IsSuperscriptAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.IsSuperscriptAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Find correct value
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsSuperscriptAttribute, true, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsSuperscriptAttribute, true, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsSuperscriptAttribute, false, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.IsSuperscriptAttribute, false, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for MarginBottomAttribute
        // -------------------------------------------------------------------
        private void MarginBottomAttribute()
        {
            double x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * MarginBottomAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.MarginBottomAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (double)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginBottomAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginBottomAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x++;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginBottomAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginBottomAttribute, x, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for MarginLeadingAttribute
        // -------------------------------------------------------------------
        private void MarginLeadingAttribute()
        {
            double x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * MarginLeadingAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.MarginLeadingAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (double)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginLeadingAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginLeadingAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x++;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginLeadingAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginLeadingAttribute, x, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for MarginTopAttribute
        // -------------------------------------------------------------------
        private void MarginTopAttribute()
        {
            double x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * MarginTopAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.MarginTopAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (double)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginTopAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginTopAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x++;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginTopAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginTopAttribute, x, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for MarginTrailingAttribute
        // -------------------------------------------------------------------
        private void MarginTrailingAttribute()
        {
            double x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * MarginTrailingAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.MarginTrailingAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (double)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginTrailingAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginTrailingAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x++;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginTrailingAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.MarginTrailingAttribute, x, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for OutlineStylesAttribute
        // -------------------------------------------------------------------
        private void OutlineStylesAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * OutlineStylesAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.OutlineStylesAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Find all possible values going forward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OutlineStylesAttribute, OutlineStyles.Embossed, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OutlineStylesAttribute, OutlineStyles.Engraved, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OutlineStylesAttribute, OutlineStyles.None, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OutlineStylesAttribute, OutlineStyles.Outline, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OutlineStylesAttribute, OutlineStyles.Shadow, true, null, CheckType.Verification);

            //Find all possible values going backward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OutlineStylesAttribute, OutlineStyles.Embossed, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OutlineStylesAttribute, OutlineStyles.Engraved, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OutlineStylesAttribute, OutlineStyles.None, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OutlineStylesAttribute, OutlineStyles.Outline, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OutlineStylesAttribute, OutlineStyles.Shadow, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for OverlineColorAttribute
        // -------------------------------------------------------------------
        private void OverlineColorAttribute()
        {
            int x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * OverlineColorAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.OverlineColorAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (int)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineColorAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineColorAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x--;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineColorAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineColorAttribute, x, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for OverlineStyleAttribute
        // -------------------------------------------------------------------
        private void OverlineStyleAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * OverlineStyleAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.OverlineStyleAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Iterate through all possible values going forward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Dash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.DashDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.DashDotDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Dot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Double, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.DoubleWavy, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.LongDash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.None, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Other, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Single, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickDash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickDashDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickDashDotDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickLongDash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickSingle, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickWavy, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Wavy, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.WordsOnly, true, null, CheckType.Verification);

            //Iterate through all possible values going backward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Dash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.DashDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.DashDotDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Dot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Double, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.DoubleWavy, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.LongDash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.None, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Other, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Single, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickDash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickDashDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickDashDotDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickLongDash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickSingle, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.ThickWavy, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.Wavy, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.OverlineStyleAttribute, TextDecorationLineStyle.WordsOnly, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for StrikethroughColorAttribute
        // -------------------------------------------------------------------
        private void StrikethroughColorAttribute()
        {
            int x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * StrikethroughColorAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.StrikethroughColorAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (int)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughColorAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughColorAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x++;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughColorAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughColorAttribute, x, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for StrikethroughStyleAttribute
        // -------------------------------------------------------------------
        private void StrikethroughStyleAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * StrikethroughStyleAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.StrikethroughStyleAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Iterate through all possible values going forward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Dash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.DashDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.DashDotDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Dot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Double, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.DoubleWavy, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.LongDash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.None, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Other, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Single, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickDash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickDashDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickDashDotDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickLongDash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickSingle, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickWavy, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Wavy, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.WordsOnly, true, null, CheckType.Verification);

            //Iterate through all possible values going backward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Dash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.DashDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.DashDotDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Dot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Double, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.DoubleWavy, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.LongDash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.None, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Other, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Single, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickDash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickDashDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickDashDotDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickLongDash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickSingle, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.ThickWavy, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.Wavy, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.StrikethroughStyleAttribute, TextDecorationLineStyle.WordsOnly, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for TabsAttribute
        // -------------------------------------------------------------------
        private void TabsAttribute()
        {
            double[] x = new double[0];
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * TabsAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.TabsAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (double[])attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TabsAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TabsAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value(s)
            for (int i = 0; i < x.Length; i++)
            {
                x[i]++;
            }

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TabsAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TabsAttribute, x, false, null, CheckType.Verification);

        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for TextFlowDirectionsAttribute
        // -------------------------------------------------------------------
        private void TextFlowDirectionsAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * TextFlowDirectionsAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.TextFlowDirectionsAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Iterate through all values going forward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TextFlowDirectionsAttribute, FlowDirections.BottomToTop, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TextFlowDirectionsAttribute, FlowDirections.Default, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TextFlowDirectionsAttribute, FlowDirections.RightToLeft, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TextFlowDirectionsAttribute, FlowDirections.Vertical, true, null, CheckType.Verification);

            //Iterate through all values going bakcward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TextFlowDirectionsAttribute, FlowDirections.BottomToTop, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TextFlowDirectionsAttribute, FlowDirections.Default, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TextFlowDirectionsAttribute, FlowDirections.RightToLeft, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.TextFlowDirectionsAttribute, FlowDirections.Vertical, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for UnderlineColorAttribute
        // -------------------------------------------------------------------
        private void UnderlineColorAttribute()
        {
            int x = 0;
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * UnderlineColorAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.UnderlineColorAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Cast it accordingly
            if (attribValue.Equals(AutomationElement.NotSupported) == false)
            {
                x = (int)attribValue;

                //Find correct value
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineColorAttribute, x, true, null, CheckType.Verification);
                _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineColorAttribute, x, false, null, CheckType.Verification);
            }

            //Create incorrect value
            x++;

            //Try to find incorrect value (and fail)
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineColorAttribute, x, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineColorAttribute, x, false, null, CheckType.Verification);
        }

        // -------------------------------------------------------------------
        // Touch FindAttribute/GetAttributeValue for UnderlineStyleAttribute
        // -------------------------------------------------------------------
        private void UnderlineStyleAttribute()
        {
            object attribValue = null;
            TextPatternRange callingRange = _pattern.DocumentRange;
            TextPatternRange targetRange = null;

            Comment("");
            Comment("* * * * * * * * * UnderlineStyleAttribute * * * * * * * * *");
            Comment("");

            //Get correct value
            _tth.Range_GetAttributeValue(callingRange, TextPattern.UnderlineStyleAttribute, ref attribValue, null, CheckType.IncorrectElementConfiguration);

            //Iterate through all possible values going forward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Dash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.DashDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.DashDotDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Dot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Double, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.DoubleWavy, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.LongDash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.None, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Other, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Single, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickDash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickDashDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickDashDotDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickDot, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickLongDash, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickSingle, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickWavy, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Wavy, true, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.WordsOnly, true, null, CheckType.Verification);

            //Iterate through all possible values going backward
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Dash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.DashDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.DashDotDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Dot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Double, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.DoubleWavy, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.LongDash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.None, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Other, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Single, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickDash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickDashDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickDashDotDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickDot, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickLongDash, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickSingle, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.ThickWavy, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.Wavy, false, null, CheckType.Verification);
            _tth.Range_FindAttribute(callingRange, ref targetRange, TextPattern.UnderlineStyleAttribute, TextDecorationLineStyle.WordsOnly, false, null, CheckType.Verification);
        }
        #endregion Attribute Helpers

        #region Exception Helpers

        // -------------------------------------------------------------------
        // Displays contents of exception
        // -------------------------------------------------------------------
        static private void CrackException(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            Comment("Raised exception " + ex.GetType().ToString() + ", Message = " + ex.Message);
        }


        #endregion Exception Helpers

        #region Misc. Helpers

        // -------------------------------------------------------------------
        // Gets class name and localized class name for control 
        // -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        [SuppressMessage("Microsoft.Performance", "CA1807:AvoidUnnecessaryStringCreation", MessageId = "temp")]
        static internal void GetClassName(AutomationElement autoElement, out string className, out string localizedControlType)
        {
            // get base classname
            // typically this will be "RichEdit", "Edit" etc.
            object temp = autoElement.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
            if (temp != null)
            {
                className = (string)temp;
                className = className.ToLower(CultureInfo.InvariantCulture);
            }
            else
            {
                className = "###### Unable to determine Class Name ######";
            }

            // get localized classname
            temp = (string)autoElement.GetCurrentPropertyValue(AutomationElement.LocalizedControlTypeProperty);
            if (temp != null)
            {
                localizedControlType = (string)temp;
                localizedControlType = localizedControlType.ToLower(CultureInfo.InvariantCulture);
            }
            else
            {
                localizedControlType = "###### Unable to determine Localized Control Type ######";
            }
        }

        #endregion Misc. Helpers

        #region Move Count Helpers

        // -------------------------------------------------------------------
        // Iterate through the document textunit by textunit using ExpandToEnclosingUnit
        // -------------------------------------------------------------------
        internal void MoveCount_ExpandToEnclose(int[] textCounts, ref StringBuilder errors, CheckType checkType)
        {
            string call = "ExpandToEnclosingUnit()@Start";
            StringBuilder results = new StringBuilder();

            results.Append(call.PadLeft(55, ' '));
            for (TextUnit tu = TextUnit.Character; tu <= TextUnit.Document; tu++)
            {
                int count = 0;
                int compare = 0;
                bool broken = false;
                bool exception = false;
                int moveExpected = -1;

                TextPatternRange range = _pattern.DocumentRange.Clone();
                string text = range.GetText(-1);
                int textLength = text.Length;

                if (textLength > 0)
                    moveExpected = -1;
                else
                    moveExpected = 0;

                if (range.MoveEndpointByUnit(TextPatternRangeEndpoint.End,
                        TextUnit.Document, -1) != moveExpected)
                {
                    ThrowMe(checkType, "MoveEndpointByUnit(End,Document,-1) failed to return " + moveExpected);
                }
                if (range.CompareEndpoints(TextPatternRangeEndpoint.Start,
                            _pattern.DocumentRange, TextPatternRangeEndpoint.Start) != 0)
                {
                    ThrowMe(checkType, "Unable to create empty range at start of document");
                }

                while (true)
                {
                    try
                    {
                        range.ExpandToEnclosingUnit(tu);
                        count++;
                    }
                    catch (Exception ex)
                    {
                        if (Library.IsCriticalException(ex))
                            throw;

                        exception = true;
                        break;
                    }

                    if (range.GetText(-1).Length == 0)
                    {
                        broken = true;
                        break;
                    }

                    compare = _pattern.DocumentRange.CompareEndpoints(TextPatternRangeEndpoint.End,
                                      range, TextPatternRangeEndpoint.End);
                    if (compare == 0)
                        break;

                    range.MoveEndpointByRange(TextPatternRangeEndpoint.Start, range, TextPatternRangeEndpoint.End);

                }

                if (broken == true)
                {
                    errors.Append(call + "(" + tu + ") returned empty range (should never be empty)\n");
                    results.Append('|' + "Empt".PadLeft(tu.ToString().Length, ' '));
                }
                else if (exception == true)
                {
                    errors.Append(call + "(" + tu + ") raised exception\n");
                    results.Append('|' + "Eror".PadLeft(tu.ToString().Length, ' '));
                }
                else
                {
                    if (count != textCounts[(int)tu])
                    {
                        errors.Append(call + "(" + tu + ") actual count = " +
                                count + ", expected = " + textCounts[(int)tu] + "\n");
                    }
                    results.Append('|' + count.ToString(CultureInfo.InvariantCulture).PadLeft(tu.ToString().Length, ' '));
                }
            }
            Comment(results.ToString());
        }

        // -------------------------------------------------------------------
        // Call Move method to iterate through document by TextUnit
        // -------------------------------------------------------------------
        internal void MoveCount_Move(TextPatternRangeEndpoint endPoint, int countAmt, int[] textCounts, ref StringBuilder errors, CheckType checkType)
        {
            string call = "";
            StringBuilder results = new StringBuilder();

            if (endPoint == TextPatternRangeEndpoint.Start)
                call = "EmptyRange@Start.Move(TextUnit.*,";
            else
                call = "EmptyRange@Start.End(TextUnit.*,";

            if (countAmt > 0)
                call += "MaxInt)";
            else
                call += "MinInt)";

            if (textCounts[0] == 0)
                results.Append("***" + call.PadLeft(52, ' '));  // Expect 0
            else
                results.Append(call.PadLeft(55, ' '));          // expect non-zero

            for (TextUnit tu = TextUnit.Character; tu <= TextUnit.Document; tu++)
            {
                TextPatternRange range = _pattern.DocumentRange.Clone();

                if (endPoint == TextPatternRangeEndpoint.Start)
                {
                    string text = range.GetText(-1);
                    int textLength = text.Length;

                    // Based on length, what is our expected move value?
                    // 0 for empty string, -1 for non-empty
                    int moveExpected = (textLength > 0 ? -1 : 0);

                    int moveActual = range.MoveEndpointByUnit(TextPatternRangeEndpoint.End,
                                            TextUnit.Document, -1);
                    if (moveActual != moveExpected)
                    {
                        ThrowMe(checkType, "MoveEndpointByUnit(End,Document,-1) failed to return " +
                                moveExpected);
                    }
                    if (range.CompareEndpoints(endPoint, _pattern.DocumentRange, endPoint) != 0)
                    {
                        ThrowMe(checkType, "Unable to create empty range at Start of Document");
                    }
                }
                else
                {
                    string text = range.GetText(-1);
                    TextTestsHelper.TrimTrailingCRLF(m_le, ref text);
                    int textLength = text.Length;

                    // Based on length, what is our expected move value?
                    // 0 for empty string, -1 for non-empty
                    int moveExpected = (textLength > 0 ? 1 : 0);

                    int moveActual = range.MoveEndpointByUnit(TextPatternRangeEndpoint.Start,
                                            TextUnit.Document, 1);
                    if (moveActual != moveExpected)
                    {
                        ThrowMe(checkType, "MoveEndpointByUnit(End,Document,1) failed to return " +
                                moveExpected);
                    }
                    if (range.CompareEndpoints(endPoint, _pattern.DocumentRange, endPoint) != 0)
                    {
                        ThrowMe(checkType, "Unable to create empty range at End of Document");
                    }
                }

                int count = range.Move(tu, countAmt);
                int correctCount = textCounts[(int)tu];

                if (countAmt < 0)  // are we moving backwards???
                {
                    correctCount *= -1; // if so, adjust correct count accordingly
                }

                if (count != correctCount)
                {
                    errors.Append(call + " for " + tu + " actual count = " +
                            count + ", expected = " + textCounts[(int)tu] + "\n");
                }

                results.Append('|' + count.ToString(CultureInfo.InvariantCulture).PadLeft(tu.ToString().Length, ' '));
            }
            Comment(results.ToString());
        }

        // -------------------------------------------------------------------
        // Call MoveEndpointByUnit method to iterate through document by TextUnit
        // -------------------------------------------------------------------
        internal void MoveCount_MoveEPBU(TextPatternRangeEndpoint endPoint, int countAmt, int[] textCounts, ref StringBuilder errors)
        {
            string call = "MoveEndpointByUnit";
            StringBuilder results = new StringBuilder();

            if (endPoint == TextPatternRangeEndpoint.Start)
                call += "(Start,TextUnit.*,";
            else
                call += "(End,TextUnit.*,";

            if (countAmt > 0)
                call += "MaxInt)";
            else
                call += "MinInt)";

            if (textCounts[0] == 0)
                results.Append("***" + call.PadLeft(52, ' '));  // Expect 0
            else
                results.Append(call.PadLeft(55, ' '));          // expect non-zero

            for (TextUnit tu = TextUnit.Character; tu <= TextUnit.Document; tu++)
            {
                TextPatternRange range = _pattern.DocumentRange.Clone();

                int count = range.MoveEndpointByUnit(endPoint, tu, countAmt);
                int correctCount = textCounts[(int)tu];

                if (countAmt < 0)  // are we moving backwards???
                {
                    correctCount *= -1; // if so, adjust correct count accordingly
                }

                if (count != correctCount)
                {
                    errors.Append(call + " for " + tu + " actual count = " +
                            count + ", expected = " + textCounts[(int)tu] + "\n");
                }

                results.Append('|' + count.ToString(CultureInfo.InvariantCulture).PadLeft(tu.ToString().Length, ' '));
            }
            Comment(results.ToString());
        }

        // -------------------------------------------------------------------
        // Display correct results to user
        // -------------------------------------------------------------------
        static internal void MoveCount_CorrectData(int[] textCounts, CheckType checkType)
        {
            string call = "*** CORRECT VALUES ***";
            StringBuilder results = new StringBuilder();

            if (textCounts.Length != (((int)TextUnit.Document) + 1))
            {
                ThrowMe(checkType, "Invalid array size for TextUnit counts, Expected " +
                    (((int)TextUnit.Document) + 1) +
                    ", Actual = " + textCounts.Length);
            }

            results.Append(call.PadLeft(55, ' '));
            for (TextUnit tu = TextUnit.Character; tu <= TextUnit.Document; tu++)
            {

                results.Append('|' + textCounts[(int)tu].ToString(CultureInfo.InvariantCulture).PadLeft(tu.ToString().Length, ' '));
            }
            Comment(results.ToString());
        }

        // -------------------------------------------------------------------
        // Displays header information about the control
        // -------------------------------------------------------------------
        internal void MoveCount_DisplayMetaData()
        {
            string afterText = _pattern.DocumentRange.GetText(-1);

            Comment("Length = " + afterText.Length.ToString(CultureInfo.InvariantCulture).PadLeft(3));
        }

        // -------------------------------------------------------------------
        // Creates separator line for output
        // -------------------------------------------------------------------
        static internal void MoveCount_DisplayTextUnitFooter()
        {
            StringBuilder results = new StringBuilder();

            results.Append("".PadLeft(55, '-'));
            for (TextUnit tu = TextUnit.Character; tu <= TextUnit.Document; tu++)
            {
                results.Append('+' + "".PadLeft(tu.ToString().Length, '-'));
            }
            Comment(results.ToString());
        }

        // -------------------------------------------------------------------
        // Creates table header for output
        // -------------------------------------------------------------------
        static internal void MoveCount_DisplayTextUnitHeader()
        {
            StringBuilder results = new StringBuilder();

            results.Append("".PadLeft(55, ' '));
            for (TextUnit tu = TextUnit.Character; tu <= TextUnit.Document; tu++)
            {
                results.Append('|' + tu.ToString().PadLeft(tu.ToString().Length, ' '));
            }
            Comment(results.ToString());
        }

        #endregion Move Count Helpers

        #endregion Helpers

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
