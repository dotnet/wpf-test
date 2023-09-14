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

    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class SelectionPatternWrapper : PatternObject
    {

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal SelectionPattern _pattern;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        bool _Contiguous; //calendar only accept contigious selection

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal SelectionPatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _pattern = (SelectionPattern)GetPattern(m_le, m_useCurrent, SelectionPattern.Pattern);

            ControlType ct = m_le.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
            _Contiguous = ct == ControlType.Calendar;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal bool pattern_CanSelectMultiple
        {
            get
            {
                return _pattern.Current.CanSelectMultiple;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal AutomationElement[] pattern_GetSelection()
        {
            return _pattern.Current.GetSelection();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal bool pattern_IsSelectionRequired
        {
            get
            {
                return _pattern.Current.IsSelectionRequired;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal SelectionItemPattern getSelectionItemPattern(AutomationElement element, CheckType checkType)
        {
            SelectionItemPattern sip = (SelectionItemPattern)element.GetCurrentPattern(SelectionItemPattern.Pattern);

            if (sip == null)
                ThrowMe(checkType, "\"" + Library.GetUISpyLook(element) + " does not support SelectionItemPattern");

            return sip;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal bool pattern_IsSelected(AutomationElement element, CheckType checkType)
        {
            SelectionItemPattern sip = getSelectionItemPattern(element, checkType);
            bool IsSelected = sip.Current.IsSelected;

            Comment("SelectionItemPattern.IsSelected returned " + IsSelected + " for '" + Library.GetUISpyLook(element) + "'");
            return sip.Current.IsSelected;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void pattern_RemoveElementFromSelection(AutomationElement element, Type expectedException, CheckType checkType)
        {

            string call = "SelectionItemPattern.RemoveFromSelection()";
            string le = Library.GetUISpyLook(element);

            try
            {
                SelectionItemPattern sip = getSelectionItemPattern(element, checkType);
                Comment("Before calling " + call + " on LE(" + le + ").IsSelected = " + sip.Current.IsSelected);
                sip.RemoveFromSelection();
                Comment("After calling " + call + " on LE(" + le + ").IsSelected = " + sip.Current.IsSelected);
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
    public sealed class SelectionTests : SelectionPatternWrapper
    {
        #region Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const string THIS = "SelectionTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static readonly string TestWhichPattern = Automation.PatternName(SelectionPattern.Pattern);

        #endregion Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public SelectionTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.Selection, dirResults, testEvents, commands)
        {
        }

        #region Tests

        #region CurrentSelection S.5.X

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("CurrentSelection.S.5.1",
            TestSummary = "Remove selection from all elements when IsSelectionRequired = false and SupportsMulitpleSelection = false",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = false",
                "Remove selection from all elements",
                "Verify that there are no elements selected"
            })]
        public void TestCurrentSelectionS51(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            CurrentSelectionTest1(false, false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("CurrentSelection.S.5.2",
            TestSummary = "Call SelectElement and verify that all other elements are not selected when IsSelectionRequired = false and SupportsMulitpleSelection = false",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = false",
                "Precondition: There are at least 2 selection elements",
                "Precondition: If this is a combo box, expand it to see the list",
                "Get a random elements",
                "Verify that this item is selectable",
                "Select this random element with the side effect that only one item is selected and verify that SelectElement() returns true",
                "Verify that the selection and unselected elements are correct"
            })]
        public void TestCurrentSelectionS52(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            CurrentSelectionTest2(false, false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("CurrentSelection.S.5.4(w/code)",
            TestSummary = "Select a random amount of elements, " +
                "and verify that they are selected, " +
                "when IsSelectionRequired = false and SupportsMulitpleSelection = true",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: There is at least one element in the selection container",
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = true",
                "Select a random collection of child elements",
                "Verify correct count of items selected"
            })]
        public void TestCurrentSelectionS54(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            CurrentSelectionTest4(false, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("CurrentSelection.S.5.5",
            TestSummary = "Remove selection from all elements when IsSelectionRequired = false and SupportsMulitpleSelection = true",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = true",
                "Remove selection from all elements",
                "Verify that there are no elements selected"
            })]
        public void TestCurrentSelectionS55(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            CurrentSelectionTest1(false, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("CurrentSelection.S.5.6",
            TestSummary = "Call SelectElement and verify that all other elements are not selected when IsSelectionRequired = false and SupportsMulitpleSelection = true",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = true",
                "Precondition: There are at least 2 selection elements",
                "Precondition: If this is a combo box, expand it to see the list",
                "Get a random elements",
                "Verify that this item is selectable",
                "Select this random element with the side effect that only one item is selected and verify that SelectElement() returns true",
                "Verify that the selection and unselected elements are corRect"
            })]
        public void TestCurrentSelectionS56(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            CurrentSelectionTest2(false, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("CurrentSelection.S.5.9",
            TestSummary = "Call SelectElement and verify that all other elements are not selected when IsSelectionRequired = true and SupportsMulitpleSelection = false",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: IsSelectionRequired = true",
                "Precondition: SupportsMulitpleSelection = false",
                "Precondition: There are at least 2 selection elements",
                "Precondition: If this is a combo box, expand it to see the list",
                "Get a random elements",
                "Verify that this item is selectable",
                "Select this random element with the side effect that only one item is selected and verify that SelectElement() returns true",
                "Verify that the selection and unselected elements are corRect"
            })]
        public void TestCurrentSelectionS59(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            CurrentSelectionTest2(true, false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("CurrentSelection.S.5.11",
            TestSummary = "Call SelectElement and verify that all other elements are not selected when IsSelectionRequired = true and SupportsMulitpleSelection = true",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: IsSelectionRequired = true",
                "Precondition: SupportsMulitpleSelection = true",
                "Precondition: There are at least 2 selection elements",
                "Precondition: If this is a combo box, expand it to see the list",
                "Get a random elements",
                "Verify that this item is selectable",
                "Select this random element with the side effect that only one item is selected and verify that SelectElement() returns true",
                "Verify that the selection and unselected elements are corRect"
            })]
        public void TestCurrentSelectionS511(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            CurrentSelectionTest2(true, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("CurrentSelection.S.5.12(w/code)",
            TestSummary = "Call SelectionSelect a random amount of elements, " +
                "and verify that they are selected, " +
                "when IsSelectionRequired = true and SupportsMulitpleSelection = false",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: There is at least one element in the selection container",
                "Precondition: IsSelectionRequired = true",
                "Precondition: SupportsMulitpleSelection = false",
                "Select a random collection of child elements",
                "Verify correct count of items selected"
            })]
        public void TestCurrentSelectionS512(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute);
            CurrentSelectionTest4(true, true);
         
        }

        private void VerifyInstructionCountWithStepsTaken(TestCaseAttribute testCaseAttribute)
        {
            if (testCaseAttribute.Description.Length != m_TestStep)
                ThrowMe(CheckType.Verification, "Description is off from steps taken in test");
        }

        #endregion CurrentSelection

        #region SupportsMultipleSelectionProperty S.6.X

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("SupportsMultipleSelectionProperty.S.6.1",
            TestSummary = "Verify that you can patternSelect multiple elements, " +
                "when IsSelectionRequired = true and SupportsMulitpleSelection = true",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: There is at least two elements in the selection container",
                "Precondition: IsSelectionRequired = true",
                "Precondition: SupportsMulitpleSelection = true",
                "Get two random elements",
                "Precondition: Verify that this 1st element found is selectable",
                "Precondition: Verify that this 2nd element found is selectable",
                "Select the 1st element",
                "Verify that the 1st element is selected",
                "Call AddElementToSelection() on the second element",
                "Verify the element is selected and no excpetion is thrown"
            })]
        public void TestSupportsMultipleSelectionPropertyS61(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            SupportsMultipleSelectionProperty_Test1(true, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("SupportsMultipleSelectionProperty.S.6.2",
            TestSummary = "Verify that you cannot patternSelect multiple elements, " +
                "when IsSelectionRequired = true and SupportsMulitpleSelection = false",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            BugNumbers = new BugIssues[] {BugIssues.PS33 },
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: There is at least two elements in the selection container",
                "Precondition: IsSelectionRequired = true",
                "Precondition: SupportsMulitpleSelection = false",
                "Get two random elements",
                "Precondition: Verify that this 1st element found is selectable",
                "Precondition: Verify that this 2nd element found is selectable",
                "Select the 1st element",
                "Verify that the 1st element is selected",
                "Call AddElementToSelection() on the second element",
                "Verify that the element is not selected and InvalidOperationException is thrown"
            })]
        public void TestSupportsMultipleSelectionPropertyS62(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            SupportsMultipleSelectionProperty_Test1(true, false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("SupportsMultipleSelectionProperty.S.6.3",
            TestSummary = "Verify that you can patternSelect multiple elements, " +
                "when IsSelectionRequired = false and SupportsMulitpleSelection = true",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: There is at least two elements in the selection container",
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = true",
                "Get two random elements",
                "Precondition: Verify that this 1st element found is selectable",
                "Precondition: Verify that this 2nd element found is selectable",
                "Select the 1st element",
                "Verify that the 1st element is selected",
                "Call AddElementToSelection() on the second element is selected",
                "Verify element is selected and no exception is thrown"
            })]
        public void TestSupportsMultipleSelectionPropertyS63(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            SupportsMultipleSelectionProperty_Test1(false, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("SupportsMultipleSelectionProperty.S.6.4",
            TestSummary = "Verify that you cannot patternSelect multiple elements, " +
                "when IsSelectionRequired = false and SupportsMulitpleSelection = false",
            Priority = TestPriorities.Pri1,
            BugNumbers = new BugIssues[] {BugIssues.PS33 },
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: There is at least two elements in the selection container",
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = false",
                "Get two random elements",
                "Precondition: Verify that this 1st element found is selectable",
                "Precondition: Verify that this 2nd element found is selectable",
                "Select the 1st element",
                "Verify that the 1st element is selected",
                "Call AddElementToSelection() on the second element",
                "Verify the element is not selected and exception is thrown"
            })]
        public void TestSupportsMultipleSelectionPropertyS64(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            SupportsMultipleSelectionProperty_Test1(false, false);
        }

        #endregion SupportsMultipleSelectionProperty

        #region IsSelectionRequired S.7.X

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("IsSelectionRequired.S.7.1",
            TestSummary = "When IsSelectionRequired = true and SupportsMulitpleSelection = true, verify that try RemoveElementFromSelection() succeeds",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: IsSelectionRequired = true",
                "Precondition: SupportsMulitpleSelection = true",
                "Get a random element",
                "Select the random element",
                "Verify that the element is selected",
                "Verify that only one item is selected",
                "Call RemoveElementFromSelection and verify results with SelectionRequired",
                "Verify that the element is selected",
                "Verify that only one item is selected",
            })]
        public void TestAtLeastOneSelectionRequiredS71(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            AtLeastOneSelectionRequired_Test1(true, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("IsSelectionRequired.S.7.2",
            TestSummary = "When IsSelectionRequired = true and SupportsMulitpleSelection = false, verify that try RemoveElementFromSelection() does not succeeds",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: IsSelectionRequired = true",
                "Precondition: SupportsMulitpleSelection = false",
                "Get a random element",
                "Select the random element",
                "Verify that the element is selected",
                "Verify that only one item is selected",
                "Call RemoveElementFromSelection and verify results with SelectionRequired",
                "Verify that the element is selected",
                "Verify that only one item is selected",
            })]
        public void TestAtLeastOneSelectionRequiredS72(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            AtLeastOneSelectionRequired_Test1(true, false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("IsSelectionRequired.S.7.3",
            TestSummary = "Verify that you cannot remove a selected element from selection, " +
                "when IsSelectionRequired = false and SupportsMulitpleSelection = true",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = true",
                "Get a random element",
                "Select the random element",
                "Verify that the element is selected",
                "Verify that only one item is selected",
                "Call RemoveElementFromSelection",
                "Verify that the element is not selected",
                "Verify that there are no other elements selected",
            })]
        public void TestAtLeastOneSelectionRequiredS73(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            AtLeastOneSelectionRequired_Test1(false, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("IsSelectionRequired.S.7.4",
            TestSummary = "Verify that you cannot remove a selected element from selection, " +
                "when IsSelectionRequired = false and SupportsMulitpleSelection = false",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = false",
                "Get a random element",
                "Select the random element",
                "Verify that the element is selected",
                "Verify that only one item is selected",
                "Call RemoveElementFromSelection",
                "Verify that the element is not selected",
                "Verify that no other element is selected",
            })]
        public void TestAtLeastOneSelectionRequiredS74(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            AtLeastOneSelectionRequired_Test1(false, false);
        }

        #endregion IsSelectionRequired S.7.X

        #region General Tests

        #region Library IsSelectionRequired

        /// -------------------------------------------------------------------
        /// <summary></summary>
        ///        
        /// -------------------------------------------------------------------
        void AtLeastOneSelectionRequired_Test1(bool selectionRequired, bool supportMultipleSelection)
        {
            AutomationElement le;
            ArrayList names = new ArrayList();
            names.Add("le");

            //"Precondition: IsSelectionRequired = *",
            TS_AtLeastOneSelectionRequired(this._pattern, selectionRequired, CheckType.IncorrectElementConfiguration);

            //"Precondition: SupportsMulitpleSelection = *",
            TS_SupportsMultipleSelection(this._pattern, supportMultipleSelection, CheckType.IncorrectElementConfiguration);

            // Get a random element
            TSC_GetRandomSelectionItemElement(out le, 1, ref names, CheckType.IncorrectElementConfiguration);

            // Select the random element
            TSC_SelectElement(le, names[0].ToString(), null, CheckType.Verification);

            // Verify that the element is selected
            TS_IsSelected(le, true, CheckType.Verification);

            // Verify that only one item is selected
            TSC_VerifySelectionCount(m_le, 1, CheckType.Verification);  

            // Call RemoveElementFromSelection and verify results with SelectionRequired
            Type exceptionType;
            if (pattern_IsSelectionRequired)
                exceptionType = typeof(InvalidOperationException);
            else
                exceptionType = null;

            TSC_RemoveElementFromSelection(le, exceptionType, CheckType.Verification);

            // Verify that the element is selected
            TS_IsSelected(le, selectionRequired, CheckType.Verification);

            // Verify that only one item is selected
            if (pattern_IsSelectionRequired)
            {
                if (!CheckWPFDataGridElement(m_le))
                {
                    TSC_VerifySelectionCount(m_le, 1, CheckType.Verification);
                }
            }
            else
                TSC_VerifySelectionCount(m_le, 0, CheckType.Verification);

        }

        #endregion Library IsSelectionRequired

        #region Library SelectElement

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void SelectElementTest1(bool selectionRequired, bool supportMultipleSelection) // ***
        {
            AutomationElement le;
            ArrayList selected = new ArrayList();
            ArrayList statements = new ArrayList();

            //"Precondition: There is at least one element in the selection container",
            TS_AtLeastSelectableChildCount(m_le, 1, CheckType.IncorrectElementConfiguration);

            //"Precondition: IsSelectionRequired = *",
            TS_AtLeastOneSelectionRequired(this._pattern, selectionRequired, CheckType.IncorrectElementConfiguration);

            //"Precondition: SupportsMulitpleSelection = *",
            TS_SupportsMultipleSelection(this._pattern, supportMultipleSelection, CheckType.IncorrectElementConfiguration);

            //"Identify a random element in the collection",
            TSC_GetRandomSelectionItemElement(out le, 1, CheckType.IncorrectElementConfiguration);

            //"Verify that this element is selectable",
            TS_IsSelectable(le, true, CheckType.IncorrectElementConfiguration);

            //"SelectElement the random element so that it is the only one item selected",
            TSC_SelectElement(le, "le", null, CheckType.Verification);

            // Verify element is selected
            TS_IsSelected(le, true, CheckType.Verification);

            // Setup ElementAddedToSelection event
            TSC_AddEventListener(le, SelectionItemPattern.ElementAddedToSelectionEvent, TreeScope.Element, CheckType.Verification);

            //"Steps: SelectElement and verify that SelectElement returned *",
            TSC_SelectElement(le, "le", null, CheckType.Verification);

            // Wait for event
            TSC_WaitForEvents(1);

            // Verify that the element is selected
            TS_IsSelected(le, true, CheckType.Verification);

            // Verify event happened
            TSC_VerifyEventListener(le, SelectionItemPattern.ElementAddedToSelectionEvent, EventFired.False, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void SelectElementTest2(bool SelectionRequired, bool SupportMultipleSelection) // ***
        {
            AutomationElement tle_1;
            AutomationElement tle_2 = null;

            //"Precondition: There is at least two element in the selection container",
            TS_AtLeastSelectableChildCount(m_le, 2, CheckType.IncorrectElementConfiguration);

            //"Precondition: IsSelectionRequired = *",
            TS_AtLeastOneSelectionRequired(this._pattern, SelectionRequired, CheckType.IncorrectElementConfiguration);

            //"Precondition: SupportsMulitpleSelection = *",
            TS_SupportsMultipleSelection(this._pattern, SupportMultipleSelection, CheckType.IncorrectElementConfiguration);

            //"Identify a random element in the collection",
            TSC_GetRandomSelectionItemElement(out tle_1, 1, CheckType.IncorrectElementConfiguration);

            //"Verify that this element is selectable",
            TS_IsSelectable(tle_1, true, CheckType.IncorrectElementConfiguration);

            //"SelectElement one random element so that it is the only one item selected",
            TSC_SelectElement(tle_1, "le", null, CheckType.Verification);

            // Get another random element
            TS_GetOtherSelectionItem(tle_1, out tle_2, false, 10, CheckType.Verification);

            // Verify second element is selectable
            TS_IsSelectable(tle_2, true, CheckType.IncorrectElementConfiguration);

            // Setup ElementAddedToSelection event
            TSC_AddEventListener(tle_2, SelectionItemPattern.ElementSelectedEvent, TreeScope.Element, CheckType.Verification);

            //"Steps: SelectElement and verify that SelectElement returned *",
            TSC_SelectElement(tle_2, "le", null, CheckType.Verification);

            // Wait for event
            TSC_WaitForEvents(1);

            // Verify event happened
            TSC_VerifyEventListener(tle_2, SelectionItemPattern.ElementSelectedEvent, EventFired.True, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void SelectElementTest3(bool selectionRequired, bool supportMultipleSelection) //***
        {
            AutomationElement le;
            ArrayList selectedElements = new ArrayList();

            //"Precondition: There is at least two element in the selection container",
            TS_AtLeastSelectableChildCount(m_le, 2, CheckType.IncorrectElementConfiguration);

            //"Precondition: IsSelectionRequired = *",
            TS_AtLeastOneSelectionRequired(this._pattern, selectionRequired, CheckType.IncorrectElementConfiguration);

            //"Precondition: SupportsMulitpleSelection = *",
            TS_SupportsMultipleSelection(this._pattern, supportMultipleSelection, CheckType.IncorrectElementConfiguration);

            // Select one or more random number of elements by calling AddToSelection but at least leave one element unselected
            TSC_SelectRandomElements(out selectedElements, m_le, 1, Helpers.GetSelectableItems(m_le).Count - 1, CheckType.Verification);

            // Retrieve a random unselected element
            TS_GetRandomSelectableItem(m_le, out le, false, CheckType.Verification);

            // Setup ElementAddedToSelection event
            TSC_AddEventListener(le, SelectionItemPattern.ElementSelectedEvent, TreeScope.Element, CheckType.Verification);

            // Select the element
            TSC_SelectElement(le, "le", null, CheckType.Verification);

            // Wait for event
            TSC_WaitForEvents(1);

            // Verify ElementAddedToSelection event did/didnot happen
            TSC_VerifyEventListener(le, SelectionItemPattern.ElementSelectedEvent, EventFired.True, CheckType.Verification);
        }

        #endregion SelectElement Library

        #region Library RemoveElementFromSelection

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyIsSelected(ArrayList EntireList, ArrayList SelectionStateList, CheckType checkType)
        {
            int index = 0;
            bool selected;
            SelectionItemPattern sip;
            foreach (AutomationElement l in EntireList)
            {
                sip = (SelectionItemPattern)l.GetCurrentPattern(SelectionItemPattern.Pattern);
                if (sip == null)
                    ThrowMe(checkType, "AutomationElement(" + Library.GetUISpyLook(l) + ") does not support SelectionItemPattern");
                selected = sip.Current.IsSelected;
                if (!selected.Equals((bool)SelectionStateList[index++]))
                {
                    ThrowMe(checkType, "AutomationElement(" + Library.GetUISpyLook(l) + ").IsSelected to return " + selected + " but should have been " + !selected);
                }
            }
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_BuildSelectionList(ref ArrayList EntireList, ref ArrayList IsSelectedResults, CheckType checkType)
        {
            EntireList = Helpers.GetSelectableItems(m_le);
            IsSelectedResults = new ArrayList(EntireList.Count);
            SelectionItemPattern sip;

            foreach (AutomationElement l in EntireList)
            {
                sip = (SelectionItemPattern)l.GetCurrentPattern(SelectionItemPattern.Pattern);
                if (sip == null)
                    ThrowMe(checkType, "AutomationElement " + Library.GetUISpyLook(l) + " does not support SelectionItemPattern");
                IsSelectedResults.Add(sip.Current.IsSelected);
            }
            m_TestStep++;
        }

        #endregion Library RemoveElementFromSelection

        #region Library CurrentSelection

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void CurrentSelectionTest1(bool selectionRequired, bool canSelectMultiple)
        {
            //"Precondition: IsSelectionRequired = *",
            TS_AtLeastOneSelectionRequired(this._pattern, selectionRequired, CheckType.IncorrectElementConfiguration);

            //"Precondition: SupportsMulitpleSelection = *",
            TS_SupportsMultipleSelection(this._pattern, canSelectMultiple, CheckType.IncorrectElementConfiguration);

            // Remove selection from all elements
            TS_UnselectAll(m_le, CheckType.Verification);

            // Verify that the selection and unselected elements are corRect
            TS_CurrentEqualsSelection(m_le, new ArrayList(), CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        ///         
        /// -------------------------------------------------------------------
        void CurrentSelectionTest2(bool selectionRequired, bool canSelectMultiple)
        {

            AutomationElement le;
            ArrayList selectedItems = new ArrayList();

            //0 "Precondition: IsSelectionRequired = *",
            TS_AtLeastOneSelectionRequired(this._pattern, selectionRequired, CheckType.IncorrectElementConfiguration);

            //1 "Precondition: SupportsMulitpleSelection = *",
            TS_SupportsMultipleSelection(this._pattern, canSelectMultiple, CheckType.IncorrectElementConfiguration);

            //2 "Precondition: At least 2 elements",
            TS_AtLeastSelectableChildCount(m_le, 2, CheckType.IncorrectElementConfiguration);

            //3 Combo box must be expanded
            ControlType controlType = m_le.Current.ControlType;

            if (controlType == ControlType.ComboBox)
            {
                ExpandCollapsePattern expandCollapse = m_le.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;

                if (expandCollapse == null)
                    ThrowMe(CheckType.Verification, "Combo box must support ExpandCollapsePattern");

                if (expandCollapse.Current.ExpandCollapseState == ExpandCollapseState.Collapsed)
                {
                    ThrowMe(CheckType.IncorrectElementConfiguration, "Requires combo box be expanded to complete test");
                }
            }
            m_TestStep++;

            //4 "Get a random elements",
            TS_GetRandomSelectableItem(m_le, out le, CheckType.IncorrectElementConfiguration);

            //5 "Verify that this item is selectable",
            TS_IsSelectable(le, true, CheckType.IncorrectElementConfiguration);

            //6 "Select this random element with the side effect that only one item is selected and verify that SelectElement() returns true
            TSC_SelectElement(le, "le", null, CheckType.Verification);

            //7 Verify that the selection and unselected elements are corRect
            selectedItems.Add(le);
            if (!CheckWPFDataGridElement(m_le)) 
            {
                TS_CurrentEqualsSelection(m_le, selectedItems, CheckType.Verification);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void CuaarrentSelectionTest3(bool selectionRequired, bool canSelectMultiple)
        {

            ArrayList selectedItems = new ArrayList();

            //"Precondition: IsSelectionRequired = *",
            TS_AtLeastOneSelectionRequired(this._pattern, selectionRequired, CheckType.IncorrectElementConfiguration);

            //"Precondition: SupportsMulitpleSelection = *",
            TS_SupportsMultipleSelection(this._pattern, canSelectMultiple, CheckType.IncorrectElementConfiguration);

            //Precondition: There are no children elements in the selection container
            selectedItems = Helpers.GetSelectableItems(m_le);
            if (!selectedItems.Count.Equals(0))
                ThrowMe(CheckType.IncorrectElementConfiguration, "There is(are) " + selectedItems.Count + " element(s) but requires 0 for test");
            m_TestStep++;

            // Verify that the IEnumerable count is 0 also
            if (!Helpers.SelectionCount(m_le).Equals(0))
                ThrowMe(CheckType.Verification, TestCaseCurrentStep);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        ///       
        /// -------------------------------------------------------------------
        void CurrentSelectionTest4(bool selectionRequired, bool canSelectMultiple)
        {
            // NOTE: Changing this to get random selection and test
            ArrayList selectedItems;

            // "Precondition: There is at least one element in the selection container",
            TS_AtLeastSelectableChildCount(m_le, 1, CheckType.IncorrectElementConfiguration);

            //"Precondition: IsSelectionRequired = *",
            TS_AtLeastOneSelectionRequired(this._pattern, selectionRequired, CheckType.IncorrectElementConfiguration);

            //"Precondition: SupportsMulitpleSelection = *",
            TS_SupportsMultipleSelection(this._pattern, canSelectMultiple, CheckType.IncorrectElementConfiguration);

            // Select a random collection of child elements
            TSC_SelectRandomElements(out selectedItems, m_le, 1, 100, CheckType.IncorrectElementConfiguration);

            // Verify correct count of items selected
            if (!CheckWPFDataGridElement(m_le))
            TSC_VerifySelectionCount(m_le, selectedItems.Count, CheckType.Verification);

        }

        #endregion Library CurrentSelection

        #region Library SupportsMultipleSelectionProperty

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void SupportsMultipleSelectionProperty_Test1(bool selectionRequired, bool supportMultipleSelection)
        {
            //TODOTODO
            if (CheckWPFDataGridElement(m_le))  // TODOTODO: DG peers selecton broken!!!
                return;

            //"0: Precondition: There is at least two elements in the selection container",
            TS_AtLeastSelectableChildCount(m_le, 2, CheckType.IncorrectElementConfiguration);

            //"1: Precondition: IsSelectionRequired = *",
            TS_AtLeastOneSelectionRequired(this._pattern, selectionRequired, CheckType.IncorrectElementConfiguration);

            //"2: Precondition: SupportsMulitpleSelection = *",
            TS_SupportsMultipleSelection(this._pattern, supportMultipleSelection, CheckType.IncorrectElementConfiguration);

            // 3: Get two random elements
            ArrayList les = Helpers.GetSelectableItems(m_le);
            int FirstItem = (int)Helpers.RandomValue((int)0, les.Count);
            int SecondItem = FirstItem;

            // Calendar requires contigunous
            ControlType ct = (ControlType)m_le.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty);
            if (ct != null && ct == ControlType.Calendar)
                if (FirstItem.Equals(les.Count - 1))
                    SecondItem = FirstItem - 1;
                else
                    SecondItem = FirstItem + 1;
            else
                while (SecondItem.Equals(FirstItem))
                    SecondItem = (int)Helpers.RandomValue((int)0, les.Count);

            Comment("AutomationElement[FirstItem] = " + Library.GetUISpyLook(((AutomationElement)les[FirstItem])));
            Comment("AutomationElement[SecondItem] = " + Library.GetUISpyLook(((AutomationElement)les[SecondItem])));
            m_TestStep++;

            // TODOTODO
            Comment("The class name for FirstItem = " + (string)((AutomationElement)les[FirstItem]).Current.ClassName);
            Comment("The class name for SecondItem = " + (string)((AutomationElement)les[SecondItem]).Current.ClassName);          

            // 4: Verify that this 1st element found is selectable
            TS_IsSelectable((AutomationElement)les[FirstItem], true, CheckType.IncorrectElementConfiguration);

            // 5: Verify that this 2nd element found is selectable
            TS_IsSelectable((AutomationElement)les[SecondItem], true, CheckType.IncorrectElementConfiguration);

            // 6: Select the 1st element
            TSC_SelectElement((AutomationElement)les[FirstItem], "le", null, CheckType.Verification);

            // 7: Verify that the 1st element is selected
            TS_IsSelected((AutomationElement)les[FirstItem], true, CheckType.Verification);

            AutomationElement le1 = (AutomationElement)les[SecondItem];
            ArrayList actSelected = new ArrayList();

            bool Results = true;

            Type exceptionType;
            if (true == supportMultipleSelection)
                exceptionType = null;
            else
                exceptionType = typeof(InvalidOperationException);

            // 8: Call AddElementToSelection() on the second element and verify the success/failure of 
            //    the call matches up with SupportsMultipleSelectionProperty
            if (!TS_FilterOnBug(m_TestCase))
                TS_AddElementToSelection((AutomationElement)les[SecondItem], exceptionType, CheckType.Verification);

            if (!TS_FilterOnBug(m_TestCase))
            {
                switch (Results)
                {
                    case false:
                        {
                            actSelected = Library_GetAllSelectElements(true);
                            switch (pattern_CanSelectMultiple)
                            {
                                case true:
                                    {
                                        switch (actSelected.Count)
                                        {
                                            case 1: // AddElementToSelection(false) : CanSelectMultiple(true)
                                                ThrowMe(CheckType.Verification, "AddElementToSelection() returned 'false' and only 1 element is selected but CanSelectMultiple = true");
                                                break;
                                            case 2: // AddElementToSelection(false) : CanSelectMultiple(true)
                                                ThrowMe(CheckType.Verification, "AddElementToSelection() returned 'false', but 2 elements are selected");
                                                break;
                                            default:
                                                ThrowMe(CheckType.Verification, "There are " + actSelected.Count + " elements selected");
                                                break;
                                        }
                                        break;
                                    }
                                case false:
                                    {
                                        switch (actSelected.Count)
                                        {
                                            case 1: // AddElementToSelection(false) : CanSelectMultiple(false)
                                                // corRect
                                                break;
                                            default:
                                                ThrowMe(CheckType.Verification, "There are " + actSelected.Count + " elements selected when AddElementToSelection() returned false, and CanSelectMultiple = false ");
                                                break;
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    case true: // AddElementToSelection
                        {
                            actSelected = Library_GetAllSelectElements(true);
                            for (int i = 0; i < actSelected.Count; i++)  // TODOTODO
                            {
                                Comment("the " + i + " item in actSelected = " + actSelected.ToString());
                            }
                            
                            switch (pattern_CanSelectMultiple)
                            {
                                case true: // AddElementToSelection(true) : CanSelectMultiple(true)
                                    {
                                        switch (actSelected.Count)
                                        {
                                            case 1:
                                                ThrowMe(CheckType.Verification, "AddElementToSelection() returned 'true' but only 1 element is selected");
                                                break;
                                            case 2: // AddElementToSelection(true) : CanSelectMultiple(true) == 2 selections
                                                // CorRect
                                                break;
                                            default:
                                                ThrowMe(CheckType.Verification, "There are " + actSelected.Count + " selected.  Something real goofy happened.");
                                                break;
                                        }
                                        break; //case true: //CanSelectMultiple
                                    }
                                case false: //AddElementToSelection(true) : CanSelectMultiple(false)
                                    {
                                        switch (actSelected.Count)
                                        {
                                            case 1:
                                                // $
                                                break;
                                            case 2:
                                                ThrowMe(CheckType.Verification, "CanSelectMultiple returns false incorRectly: AddElementToSelection() returned true, and two elements were selected");
                                                break;
                                            default:
                                                ThrowMe(CheckType.Verification, "There are " + actSelected.Count + " elements selected when AddElementToSelection() returned true, and CanSelectMultiple = false ");
                                                break;
                                        }
                                        break; //case false: //CanSelectMultiple
                                    }
                            }
                            break;
                        } //AddElementToSelection
                }
                m_TestStep++;
            }
        }
            
        #endregion Library SupportsMultipleSelectionProperty

        #endregion General Tests

        #endregion Tests

        #region Test Steps

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TSC_VerifySelectionCount(AutomationElement element, int ExpectedCount, CheckType checkType)
        {
            int count = 0;
            SelectionPattern sp = m_le.GetCurrentPattern(SelectionPattern.Pattern) as SelectionPattern;

            AutomationElement[] aes = sp.Current.GetSelection();
            foreach (AutomationElement ae in aes)
            {
                count++;
            }

            if (count != ExpectedCount)
            {
                ThrowMe(checkType, "Selection count equal " + count + " and expected " + ExpectedCount);
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TSC_GetRandomSelectionItemElement(out AutomationElement element, int numberofelements, CheckType checkType)
        {
            ArrayList statements = new ArrayList();
            ArrayList names = new ArrayList();

            TSC_GetRandomSelectionItemElement(out element, numberofelements, ref names, checkType);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TSC_GetRandomSelectionItemElement(out AutomationElement element, int numberofelements, ref ArrayList leNames, CheckType checkType)
        {
            ArrayList list = Helpers.GetSelectableItems(m_le);

            int[] indexes = new int[numberofelements];
            int r = 0;
            int index = 0;
            // Find numberofindex random elements
            for (int i = 0; i < 10; i++)
            {
                r = (int)Helpers.RandomValue(0, list.Count);
                if (Array.IndexOf(indexes, r).Equals(-1))
                {
                    indexes[index++] = r;
                }
                if (index.Equals(numberofelements))
                    break;
            }

            if (list.Count < 1)
                ThrowMe(checkType, "Could not find any SelectionItems");

            element = (AutomationElement)list[r];
            Comment("Found AutomationElement " + Library.GetUISpyLook(element));

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TSC_RemoveElementFromSelection(AutomationElement element, Type expectedException, CheckType checkType)
        {
            pattern_RemoveElementFromSelection(element, expectedException, checkType);
            m_TestStep++;
        }

        #endregion Test steps

        #region Libraries

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        ArrayList Library_GetAllSelectElements(bool Selected)
        {
            SelectionItemPattern sip;

            ArrayList arrayList = Helpers.GetSelectableItems(m_le);

            Comment("arrayList count is : " + arrayList.Count); // TODOTODO: DG.DataItem.SupportsMultipleSelectionProperty.S.6.3 

            ArrayList retList = new ArrayList();

            foreach (AutomationElement l in arrayList)
            {
                sip = (SelectionItemPattern)(l.GetCurrentPattern(SelectionItemPattern.Pattern));
                if (sip != null)
                {
                    if (sip.Current.IsSelected.Equals(Selected))
                    {
                        retList.Add(l);

                        // TODOTODO
                        Comment("The class name for l = " + l.Current.ClassName);
                        Comment("and the AutomationElement[l] = " + Library.GetUISpyLook(l));

                        AutomationElement ae = sip.Current.SelectionContainer;
                        Comment("The class name for ae = " + (string)ae.GetCurrentPropertyValue(AutomationElement.ClassNameProperty));
                    }
                }
            }
            return retList;
        }

        #endregion Libraries
    }
}
