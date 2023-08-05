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
    public class SelectionItemPatternWrapper : PatternObject
    {

        #region Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        SelectionItemPattern _pattern;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal SelectionPattern _selectionPattern;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal AutomationElement _selectionContainer;

        #endregion Variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal SelectionItemPatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _pattern = (SelectionItemPattern)GetPattern(m_le, m_useCurrent, SelectionItemPattern.Pattern);
            if (_pattern == null)
                throw new Exception(Helpers.PatternNotSupported + ": SelectionItemPattern");

            _selectionContainer = _pattern.Current.SelectionContainer;

            if (_selectionContainer != null)
            {
                _selectionPattern = _selectionContainer.GetCurrentPattern(SelectionPattern.Pattern) as SelectionPattern;
                if (_selectionPattern == null)
                    throw new ArgumentException("Could not find the SelectionContainer's SelectionPattern");
            }

        }

        #region Properties

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal bool pattern_IsSelected
        {
            get
            {
                return _pattern.Current.IsSelected;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal AutomationElement pattern_SelectionContainer
        {
            get
            {
                return _pattern.Current.SelectionContainer;
            }
        }

        #endregion Properties

        #region Element's container

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal bool container_CanSelectMultiple
        {
            get
            {
                return _selectionPattern.Current.CanSelectMultiple;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal AutomationElement[] container_GetSelection()
        {
            return _selectionPattern.Current.GetSelection();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal bool container_IsSelectionRequired
        {
            get
            {
                return _selectionPattern.Current.IsSelectionRequired;
            }
        }

        #endregion Element's container

        #region Methods

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void pattern_Select(AutomationElement element, Type expectedException, CheckType checkType)
        {
            string call = "SelectionItemPattern.Select()";
            try
            {
                SelectionItemPattern sip = element.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                Comment("Before calling " + call + " on LE(" + Library.GetUISpyLook(element) + ").IsSelected = " + sip.Current.IsSelected);
                sip.Select();
                Thread.Sleep(1);
                Comment("After calling " + call + " on LE(" + Library.GetUISpyLook(element) + ").IsSelected = " + sip.Current.IsSelected);
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

        #endregion Methods

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
    public sealed class SelectionItemTests : SelectionItemPatternWrapper
    {
        #region Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        SelectionItemPattern _pattern;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const string THIS = "SelectionItemTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static readonly string TestWhichPattern = Automation.PatternName(SelectionItemPattern.Pattern);

        #endregion Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public SelectionItemTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.SelectionItem, dirResults, testEvents, commands)
        {
            _pattern = (SelectionItemPattern)element.GetCurrentPattern(SelectionItemPattern.Pattern);
            if (_pattern == null)
                throw new Exception(Helpers.PatternNotSupported);
        }

        #region Step/Verification

        #endregion Step/Verification

        #region Tests

        #region Select


        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Select.1.0a",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Problem, /* Too many scenarios for this one test to cover, fix one scenario, another breaks */
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Element is not selected",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Select the element",
                "Step: Wait for one event to fire",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired",
                "Verify element is selected",
            })]
        public void TestSelect11(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverSelect0(false);
        }

        /// -------------------------------------------------------------------
        [TestCaseAttribute("Select.1.0a",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Problem, /* Too many scenarios for this one test to cover, fix one scenario, another breaks */
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Element is selected",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Select the element",
                "Step: Wait for one event to fire",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty event fired is undetermined",
                "Verify element is selected",
            })]
        public void TestSelect1a(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverSelect0(true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Select.1.0b",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be true",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be true",
                "Verify: Call Select on element",
                "Verify element is selected",
            })]
        public void TestSelect1b(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverSelect1(true, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Select.1.4",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be true",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be false",
                "Verify: Call Select on element",
                "Verify element is selected",
            })]
        public void TestSelect14(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverSelect1(true, false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Select.1.6",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be false",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be true",
                "Verify: Call Select on element",
                "Verify element is selected",
            })]
        public void TestSelect16(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverSelect1(false, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Select.1.10",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be false",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be false",
                "Verify: Call Select on element",
                "Verify element is selected",
            })]
        public void TestSelect110(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverSelect1(false, false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Select.1.2",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
           TestCaseType = TestCaseType.Generic | TestCaseType.Events,
           EventTested = "SelectionItemPattern.IsSelectedProperty",
           Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be true",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be true",
                "Step: Select some other element incase selection required",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Select the element",
                "Step: Wait for one event to occur",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired",
                "Verify element is selected",
            })]
        public void TestSelect12(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverSelect2(true, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Select.1.5",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be true",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be false",
                "Step: Select some other element incase selection required",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Select the element",
                "Step: Wait for one event to fire",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired",
                "Verify element is selected",
            })]
        public void TestSelect15(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverSelect2(true, false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Select.1.7",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be false",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be true",
                "Step: Select some other element incase selection required",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Select the element",
                "Step: Wait for one event to fire",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired",
                "Verify element is selected",
            })]
        public void TestSelect17(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverSelect2(false, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("Select.1.11",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be false",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be false",
                "Step: Select some other element incase selection required",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Select the element",
                "Step: Wait for one event to fire",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired",
                "Verify element is selected",
            })]
        public void TestSelect111(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverSelect2(false, false);
        }

        #endregion Select

        #region AddToSelection

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddToSelection.2.1",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be false",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be false",
                "Step: Select the element",
                "Step: Call AddToSelection on the same element and verify that InvalidOperationException is thrown",
                "Verify element is selected",
            })]
        public void TestSelect21(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverAddToSelection1(false, false, typeof(InvalidOperationException));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddToSelection.2.6",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be false",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be true",
                "Step: Select the element",
                "Step: Call AddToSelection on the same element and no exception is thrown",
                "Verify element is selected",
            })]
        public void TestSelect26(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverAddToSelection1(false, true, null);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddToSelection.2.8",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not support a selection container)",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be true",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be false",
                "Step: Select the element",
                "Step: Call AddToSelection on the same element and verify that System.InvalidOperationException is thrown",
                "Verify element is selected",
            })]
        public void TestSelect28(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverAddToSelection1(true, false, null); //typeof(InvalidOperationException));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddToSelection.2.12",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic,
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be true",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be true",
                "Step: Select the element",
                "Step: Call AddToSelection on the same element and verify that InvalidOperationException is thrown",
                "Verify element is selected",
            })]
        public void TestSelect212(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverAddToSelection1(true, true, typeof(InvalidOperationException));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddToSelection.2.2",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be true",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be true",
                "Step: Select the element",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Call AddToSelection on the same element",
                "Step: Wait for two events max",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired",
                "Verify element is selected",
            })]
        public void TestSelect22(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverAddToSelection2(true, true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddToSelection.2.7",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container)",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be false",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be true",
                "Step: Select the element",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Call AddToSelection on the same element",
                "Step: Wait for two events max",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty was not fired",
                "Verify element is selected",
            })]
        public void TestSelect27(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            // Should be false on the 1011 list box.  There may be other scenarios where it is otherwise.  If we find them, 
            this.TestDriverAddToSelection2(false, false);  
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddToSelection.2.3",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be false",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be false",
				"Step: Select some other element incase selection required",
				"Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
				"Step: Call AddToSelection on the element",
				"Step: Wait for one event to occur",
				"Verification: Verify that the SelectionItemPattern.IsSelectedProperty was not fired",
				"Verify correct elements are selected"            })]
        public void TestSelect23(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverAddToSelection3(false, false, testCase);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddToSelection.2.9",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            BugNumbers = new BugIssues[] { BugIssues.PS33 }, 
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be true",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be false",
				"Step: Select some other element incase selection required",
				"Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
				"Step: Call AddToSelection on the element",
				"Step: Wait for one event to occur",
				"Verification: Verify that the SelectionItemPattern.IsSelectedProperty was not fired",
				"Verify correct elements are selected"            
			})]
        public void TestSelect29(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverAddToSelection3(true, false, testCase);
        }

        #endregion AddToSelection

        #region RemoveFromSelection

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("RemoveFromSelection.3.1",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be true",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be false",
                "Step: Select some other element",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Call RemoveFromSelection on the same element, and verify that no exception is thrown",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty is not fired",
                "Verify element is still selected",
            })]
        public void TestRemoveFromSelection31(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverNotAbleToRemoveFromSelection(true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("RemoveFromSelection.3.3",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be true",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be true",
                "Step: Select some other element",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Remove element from selection and expect an InvalidOperationException",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty was not fired",
                "Verify element is selected",
            })]
        public void TestRemoveFromSelection33(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverNotAbleToRemoveFromSelection(true);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("RemoveFromSelection.3.5",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Verify that this test is valid for this element",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be false",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be false",
                "Step: Select the element",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Select the element",
                "Step: Wait for event to occur",
                "Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired",
                "Verify element are not selected",
            })]
        public void TestRemoveFromSelection35(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverAbleToRemoveFromSelection(false);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("RemoveFromSelection.3.2",
            Priority = TestPriorities.Pri3,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Generic | TestCaseType.Events,
            EventTested = "SelectionItemPattern.IsSelectedProperty",
            Description = new string[] 
            {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Selection container's SelectionPattern.IsSelectionRequired should be false",
                "Precondition: Selection container's SelectionPattern.CanSelectMultiple should be true",
                "Step: Select the element",
                "Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
                "Step: Select the element",
                "Step: Wait for event to occur",
                "Verify: That the SelectionItemPattern.IsSelectedProperty was fired",
                "Verify: Element are not selected",
            })]
        public void TestRemoveFromSelection32(TestCaseAttribute testCase)
        {
            this.HeaderComment(testCase);

            this.TestDriverAbleToRemoveFromSelection(true);
        }

        #endregion RemoveFromSelection

        #region AddElementToSelection S.2.X

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddElementToSelection.S.2.5",
            TestSummary = "On an element that is selected, " +
                "call SelectionItemPattern.AddElementToSelection(), " +
                "and verify that the item is selected and that SelectionItemPattern.ElementAddedToSelectionEvent was not fired, " +
                "when IsSelectionRequired = true and SupportsMulitpleSelection = false",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            BugNumbers = new BugIssues[] { BugIssues.PS33 },
            EventTested = "SelectionItemPattern.ElementAddedToSelectionEvent",
            Description = new string[] {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: There is at least 2 element in the selection container",
                "Precondition: IsSelectionRequired = true",
                "Precondition: SupportsMulitpleSelection = false",
                "Precondition: Select element",
                "Find some other random unselected element",
                "Setup ElementAddedToSelectionEvent event",
                "Call AddElementToSelection() with this element and verify that AddElementToSelection() returns true",
                "Wait for events",
                "Verify ElementAddedToSelection event did not happen",
            })]
        public void TestAddElementToSelectionS25(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            AddElementToSelectionTest2(true, false, EventFired.False);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddElementToSelection.S.2.10",
            TestSummary = "Call AddElementToSelection() on and element when there is no elements selected, " +
                "and verify that the element is selected and that ElementAddedToSelectionEvent is fired, " +
                "when IsSelectionRequired = false and SupportsMulitpleSelection = true",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationEventHandler(SelectionItemPattern.ElementAddedToSelectionEvent)",
            Description = new string[] {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: There is at least two element in the selection container",
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = true",
                "Step: Select the element",
                "Step: Find a random element that is not selected",
                "Step: Setup SelectionItemPattern.AddElementToSelection event",
                "Step: Call AddElementToSelection() with this element",
                "Step: Wait for events", 
                "Verify SelectionItemPattern.AddElementToSelection event is fired",
            })]
        public void TestAddElementToSelectionS210(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            AddElementToSelectionTest3(true);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddElementToSelection.S.2.13",
            TestSummary = "On an element that is selected, " +
                "call SelectionItemPattern.AddElementToSelection(), " +
                "and verify that the item is selected and that SelectionItemPattern.ElementAddedToSelectionEvent was not fired, " +
                "when IsSelectionRequired = false and SupportsMulitpleSelection = false",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationEventHandler(SelectionItemPattern.ElementAddedToSelectionEvent)",
            Description = new string[] {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: There is at least 2 element in the selection container",
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = false",
                "Precondition: Select element",
                "Verify that the element is the only one selected",
                "Find a random unselected element",
                "Setup ElementAddedToSelectionEvent event",
                "Call AddElementToSelection() with this element and verify that AddElementToSelection() returns true",
                "Wait for events",
                "Verify ElementAddedToSelection event did not happen",
            })]
        public void TestAddElementToSelectionS213(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            AddElementToSelectionTest2(false, false, EventFired.False);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddElementToSelection.S.2.14",
            TestSummary = "Call AddElementToSelection() on and element when there is no elements selected, " +
                "and verify that the element is selected and that ElementAddedToSelectionEvent is fired, " +
                "when IsSelectionRequired = false and SupportsMulitpleSelection = false",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Problem, /* spec now says that AddElementToSelection doesn't have to work for single slect items */
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationEventHandler(SelectionItemPattern.ElementAddedToSelectionEvent)",
            Description = new string[] {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: Verify that this test is valid for this element",
                "Precondition: There is at least one element in the selection container",
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = false",
                "Unselect all elements",
                "Find a random element that is not selected",
                "Setup SelectionItemPattern.ElementSelectionEvent event",
                "Call AddElementToSelection() with this element",
                "Verify the this this element is the only element selected", 
                "Step: Wait for events", 
                "Verify SelectionItemPattern.ElementSelectionEvent event was fired"
            })]
        public void TestAddElementToSelectionS214(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            AddElementToSelectionTest3(false);
        }

        /// -------------------------------------------------------------------
        ///<summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("AddElementToSelection.S.2.15",
            TestSummary = "On an element that is selected, " +
                "call SelectionItemPattern.AddElementToSelection(), " +
                "and verify that the item is selected and that SelectionItemPattern.ElementAddedToSelectionEvent was not fired, " +
                "when IsSelectionRequired = false and SupportsMulitpleSelection = true",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationEventHandler(SelectionItemPattern.ElementAddedToSelectionEvent)",
            Description = new string[] {
                "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
                "Precondition: There is at least 2 element in the selection container",
                "Precondition: IsSelectionRequired = false",
                "Precondition: SupportsMulitpleSelection = true",
                "Precondition: Select element",
                "Find a random unselected element",
                "Setup ElementAddedToSelectionEvent event",
                "Call AddElementToSelection() with this element and verify that AddElementToSelection() returns true",
                "Wait for events",
                "Verify ElementAddedToSelection event is fired",
            })]
        public void TestAddElementToSelectionS215(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            AddElementToSelectionTest2(false, true, EventFired.True);
        }

        #endregion AddElementToSelection

        #endregion Tests

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TestDriverSelect1(bool isSelectionRequired, bool canSelectMultiple)
        {
            // "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
            TSC_VerifyProperty(_pattern.Current.SelectionContainer != null, true, SelectionItemPattern.SelectionContainerProperty, CheckType.IncorrectElementConfiguration);

            ArrayList listOther = HelperSubtract(HelperGetContainersSelectableItems(_selectionContainer), new object[] { m_le });
            ArrayList listElement = new ArrayList();
            listElement.Add(m_le);

            // Precondition: Selection container's SelectionPattern.IsSelectionRequired should be *
            TS_SelectionRequired(isSelectionRequired, CheckType.IncorrectElementConfiguration);

            // Precondition: Selection container's SelectionPattern.CanSelectMultiple should be *
            TS_CanSelectMultiple(canSelectMultiple, CheckType.IncorrectElementConfiguration);

            // Verify: Call Select on element
            TS_Select(m_le, null, CheckType.Verification);

            // Verify element is selected
            TS_VerifyElementsAreSelected(listElement, CheckType.Verification);

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TestDriverSelect2(bool isSelectionRequired, bool canSelectMultiple)
        {
            // "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
            TSC_VerifyProperty(_pattern.Current.SelectionContainer != null, true, SelectionItemPattern.SelectionContainerProperty, CheckType.IncorrectElementConfiguration);

            ArrayList listOther = HelperSubtract(HelperGetContainersSelectableItems(_selectionContainer), new object[] { m_le });
            ArrayList listElement = new ArrayList();
            listElement.Add(m_le);

            AutomationProperty[] propertiesFired = new AutomationProperty[] { SelectionItemPattern.IsSelectedProperty };
            EventFired[] fireState = new EventFired[] { EventFired.True };

            // Precondition: Selection container's SelectionPattern.IsSelectionRequired should be *
            TS_SelectionRequired(isSelectionRequired, CheckType.IncorrectElementConfiguration);

            // Precondition: Selection container's SelectionPattern.CanSelectMultiple should be *
            TS_CanSelectMultiple(canSelectMultiple, CheckType.IncorrectElementConfiguration);

            // Step: Select some other element incase selection required
            TS_Select((AutomationElement)listOther[0], null, CheckType.IncorrectElementConfiguration);

            //  Step: Add a SelectionItemPattern.IsSelectedProperty property change event
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, propertiesFired, CheckType.Verification);

            // Step: Select the element
            TS_Select(m_le, null, CheckType.Verification);

            // Step: Wait for one event to fire
            TSC_WaitForEvents(1);

            // Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired
            TSC_VerifyPropertyChangedListener(m_le, fireState, propertiesFired, CheckType.Verification);

            // Verify element is selected
            TS_VerifyElementsAreSelected(listElement, CheckType.Verification);

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TestDriverAddToSelection1(bool isSelectionRequired, bool canSelectMultiple, Type exceptionType)
        {
            // "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container)",
            TSC_VerifyProperty(_pattern.Current.SelectionContainer != null, true, SelectionItemPattern.SelectionContainerProperty, CheckType.IncorrectElementConfiguration);

            ArrayList listElement = new ArrayList();
            listElement.Add(m_le);

            // Precondition: Selection container's SelectionPattern.IsSelectionRequired should be *
            TS_SelectionRequired(isSelectionRequired, CheckType.IncorrectElementConfiguration);

            // Precondition: Selection container's SelectionPattern.CanSelectMultiple should be *
            TS_CanSelectMultiple(canSelectMultiple, CheckType.IncorrectElementConfiguration);

            // "Step: Select the element",
            TS_Select(CheckType.Verification);

            // "Step: Call AddToSelection on the same element and verify that InvalidOperationException is thrown",
            // This happens on ("<Control><E><N>Controls Test Application</N><C>ControlType.Window</C><E><N>Combo(1005)</N><C>ControlType.ComboBox</C><I>1005</I><E><C>ControlType.List</C><I>ListBox</I><E><N>String 5</N><C>ControlType.ListItem</C></E></E></E></E></Control>")
            // TreeItem does not do this
            // ListView does not do this...
            // Believe it does not on another control.  
            TS_AddElementToSelection(m_le, exceptionType, CheckType.Verification);

            // Verify element is selected
            TS_VerifyElementsAreSelected(listElement, CheckType.Verification);

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TestDriverAddToSelection2(bool isSelectionRequired, bool canSelectMultiple)
        {
            // "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
            TSC_VerifyProperty(_pattern.Current.SelectionContainer != null, true, SelectionItemPattern.SelectionContainerProperty, CheckType.IncorrectElementConfiguration);

            ArrayList allElements = HelperGetContainersSelectableItems(_selectionContainer);

            ArrayList listElement = new ArrayList();
            listElement.Add(m_le);

            AutomationProperty[] propertiesFired = new AutomationProperty[] { SelectionItemPattern.IsSelectedProperty };
            EventFired[] fireState = new EventFired[] { EventFired.True };

            // Precondition: Selection container's SelectionPattern.IsSelectionRequired should be *
            TS_SelectionRequired(isSelectionRequired, CheckType.IncorrectElementConfiguration);

            // Precondition: Selection container's SelectionPattern.CanSelectMultiple should be *
            TS_CanSelectMultiple(canSelectMultiple, CheckType.IncorrectElementConfiguration);

            //"Step: Select the element",
            TS_Select(m_le, null, CheckType.Verification);

            //"Step: Add a SelectionItemPattern.IsSelectedProperty property change event",
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, propertiesFired, CheckType.Verification);

            //"Step: Call AddToSelection on the same element",
            TS_AddElementToSelection(m_le, null, CheckType.Verification);

            // Step: Wait for two events max
            TSC_WaitForEvents(2);

            //"Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired",
            TSC_VerifyPropertyChangedListener(m_le, fireState, propertiesFired, CheckType.Verification);

            //"Verify element is selected",
            TS_VerifyElementsAreSelected(listElement, CheckType.Verification);

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TestDriverAddToSelection3(bool isSelectionRequired, bool canSelectMultiple, TestCaseAttribute testCaseAttribute)
        {
            // "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
            TSC_VerifyProperty(_pattern.Current.SelectionContainer != null, true, SelectionItemPattern.SelectionContainerProperty, CheckType.IncorrectElementConfiguration);

            ArrayList selectableList = HelperSubtract(HelperGetContainersSelectableItems(_selectionContainer), new object[] { m_le });
            ArrayList multiSelectList = new ArrayList();
            ArrayList singleSelectList = new ArrayList();

            multiSelectList.Add((AutomationElement)selectableList[0]);
            multiSelectList.Add(m_le);

            singleSelectList.Add((AutomationElement)selectableList[0]); ;

            AutomationProperty[] propertiesFired = new AutomationProperty[] { SelectionItemPattern.IsSelectedProperty };
            EventFired[] fireState;
            if (canSelectMultiple)
                fireState = new EventFired[] { EventFired.True };
            else
                fireState = new EventFired[] { EventFired.False };

            // Precondition: Selection container's SelectionPattern.IsSelectionRequired should be *
            TS_SelectionRequired(isSelectionRequired, CheckType.IncorrectElementConfiguration);

            // Precondition: Selection container's SelectionPattern.CanSelectMultiple should be *
            TS_CanSelectMultiple(canSelectMultiple, CheckType.IncorrectElementConfiguration);

            // Step: Select some other element incase selection required
            TS_Select((AutomationElement)multiSelectList[0], null, CheckType.IncorrectElementConfiguration);

            // Step: Add a SelectionItemPattern.IsSelectedProperty property change event
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, propertiesFired, CheckType.Verification);

            // Step: Call AddToSelection on the element
            if (canSelectMultiple)
                TS_AddElementToSelection(m_le, null, CheckType.Verification);
            else if (!TS_FilterOnBug(testCaseAttribute))
                TS_AddElementToSelection(m_le, typeof(InvalidOperationException), CheckType.Verification);

            // Step: Wait for one event to occur
            TSC_WaitForEvents(1);

            // Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired
            TSC_VerifyPropertyChangedListener(m_le, fireState, propertiesFired, CheckType.Verification);

            // Verify correct elements are selected
            if (canSelectMultiple)
                TS_VerifyElementsAreSelected(multiSelectList, CheckType.Verification);
            else
                TS_VerifyElementsAreSelected(singleSelectList, CheckType.Verification);

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TestDriverNotAbleToRemoveFromSelection(bool canSelectMultiple)
        {
            // "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
            TSC_VerifyProperty(_pattern.Current.SelectionContainer != null, true, SelectionItemPattern.SelectionContainerProperty, CheckType.IncorrectElementConfiguration);

            ArrayList otherElements = HelperSubtract(HelperGetContainersSelectableItems(_selectionContainer), new object[] { m_le });
            ArrayList listElement = new ArrayList();
            listElement.Add(otherElements[0]);

            AutomationProperty[] propertiesFired = new AutomationProperty[] { SelectionItemPattern.IsSelectedProperty };
            EventFired[] fireState = new EventFired[] { EventFired.False };

            // Precondition: Selection container's SelectionPattern.IsSelectionRequired should be *
            TS_SelectionRequired(true, CheckType.IncorrectElementConfiguration);

            // Precondition: Selection container's SelectionPattern.CanSelectMultiple should be *
            TS_CanSelectMultiple(canSelectMultiple, CheckType.IncorrectElementConfiguration);

            // "Step: Select some other element",
            TS_Select((AutomationElement)otherElements[0], null, CheckType.Verification);

            // Step: Add a SelectionItemPattern.IsSelectedProperty property change event
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, propertiesFired, CheckType.Verification);

            // Step: Remove element from selection and expect no exception
            TS_RemoveElementFromSelection(m_le, null, CheckType.Verification);

            // Verification: Verify that the SelectionItemPattern.IsSelectedProperty was not fired
            TSC_VerifyPropertyChangedListener(m_le, fireState, propertiesFired, CheckType.Verification);

            // Verify element is selected
            TS_VerifyElementsAreSelected(listElement, CheckType.Verification);

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TestDriverAbleToRemoveFromSelection(bool canSelectMultiple)
        {
            // "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
            TSC_VerifyProperty(_pattern.Current.SelectionContainer != null, true, SelectionItemPattern.SelectionContainerProperty, CheckType.IncorrectElementConfiguration);

            ArrayList listElement = new ArrayList();
            listElement.Add(m_le);

            AutomationProperty[] propertiesFired = new AutomationProperty[] { SelectionItemPattern.IsSelectedProperty };
            EventFired[] fireState = new EventFired[] { EventFired.True };

            // Precondition: Selection container's SelectionPattern.IsSelectionRequired should be *
            TS_SelectionRequired(false, CheckType.IncorrectElementConfiguration);

            // Precondition: Selection container's SelectionPattern.CanSelectMultiple should be *
            TS_CanSelectMultiple(canSelectMultiple, CheckType.IncorrectElementConfiguration);

            // "Step: Select the element",
            TS_Select(CheckType.Verification);

            // Step: Add a SelectionItemPattern.IsSelectedProperty property change event
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, propertiesFired, CheckType.Verification);

            // Step: Select the element
            TS_RemoveElementFromSelection(m_le, null, CheckType.Verification);

            // Step: Wait for event to occur
            TSC_WaitForEvents(1);

            // Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired
            TSC_VerifyPropertyChangedListener(m_le, fireState, propertiesFired, CheckType.Verification);

            // Verify element are not selected
            TS_VerifyElementsAreNotSelected(listElement, CheckType.Verification);

        }

        #region Library AddElementToSelection

        /// -------------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------------
        void AddElementToSelectionTest2(bool required, bool canSelectMultiple, EventFired eventFired) // ***
        {
            AutomationElement le;
            ArrayList selected = new ArrayList();
            Type expectedException = canSelectMultiple ? null : typeof(InvalidOperationException);
            selected.Add(m_le);

            // "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
            TSC_VerifyProperty(_pattern.Current.SelectionContainer != null, true, SelectionItemPattern.SelectionContainerProperty, CheckType.IncorrectElementConfiguration);

            //"Precondition: There is at least 2 element in the selection container",
            TS_AtLeastSelectableChildCount(_selectionContainer, 2, CheckType.IncorrectElementConfiguration);

            //"Precondition: IsSelectionRequired = #1",
            TS_AtLeastOneSelectionRequired(_selectionPattern, required, CheckType.IncorrectElementConfiguration);

            //"Precondition: SupportsMulitpleSelection = #2",
            TS_SupportsMultipleSelection(this._selectionPattern, canSelectMultiple, CheckType.IncorrectElementConfiguration);

            // "Precondition: Select element",
            TS_Select(CheckType.Verification);

            // "Find a random unselected element",
            TS_GetOtherSelectionItem(m_le, out le, false, 10, CheckType.IncorrectElementConfiguration);

            // "Setup ElementAddedToSelectionEvent event",
            TSC_AddEventListener(le, SelectionItemPattern.ElementAddedToSelectionEvent, TreeScope.Element, CheckType.Verification);

            // "Call AddElementToSelection() with this element and verify that AddElementToSelection() returns true",
            if (!TS_FilterOnBug(m_TestCase))
                TS_AddElementToSelection(le, expectedException, CheckType.Verification);

            // "Wait for events",
            TSC_WaitForEvents(1);

            // "Verify ElementAddedToSelection event * happen",
            TSC_VerifyEventListener(le, SelectionItemPattern.ElementAddedToSelectionEvent, eventFired, CheckType.Verification);

        }

        /// -------------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------------
        void AddElementToSelectionTest3(bool SupportMultipleSelection) // ***
        {
            // "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
            TSC_VerifyProperty(_pattern.Current.SelectionContainer != null, true, SelectionItemPattern.SelectionContainerProperty, CheckType.IncorrectElementConfiguration);

            ArrayList selectedElements = new ArrayList();
            AutomationElement le;

            //"Precondition: There is at least one element in the selection container",
            TS_AtLeastSelectableChildCount(_selectionContainer, 1, CheckType.IncorrectElementConfiguration);

            //"Precondition: IsSelectionRequired = false",
            TS_AtLeastOneSelectionRequired(_selectionPattern, false, CheckType.IncorrectElementConfiguration);

            //"Precondition: SupportsMulitpleSelection = *",
            TS_SupportsMultipleSelection(this._selectionPattern, SupportMultipleSelection, CheckType.IncorrectElementConfiguration);

            // Precondition: Select element
            TS_Select(m_le, null, CheckType.Verification);

            // Find a random element that is not selected
            TS_GetRandomSelectableItem(_selectionContainer, out le, false, CheckType.Verification);

            // Setup ElementAddedToSelection event
            TSC_AddEventListener(le, SelectionItemPattern.ElementAddedToSelectionEvent, TreeScope.Element, CheckType.Verification);

            // Call AddElementToSelection() with this element
            TS_AddElementToSelection(le, null, CheckType.Verification);

            // Wait for events
            TSC_WaitForEvents(2);

            // Verify ElementAddedToSelection event * happen
            TSC_VerifyEventListener(le, SelectionItemPattern.ElementAddedToSelectionEvent, EventFired.True, CheckType.Verification);
        }

        #endregion Library AddElementToSelection

        #region TS_* methods

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_SelectionRequired(bool isSelectionRequired, CheckType checkType)
        {
            if (isSelectionRequired != _selectionPattern.Current.IsSelectionRequired)
                ThrowMe(checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_CanSelectMultiple(bool canSelectMultiple, CheckType checkType)
        {
            if (canSelectMultiple != _selectionPattern.Current.CanSelectMultiple)
                ThrowMe(checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>Select the class's AutomationElement</summary>
        /// -------------------------------------------------------------------
        private void TS_Select(CheckType checkType)
        {
            pattern_Select(m_le, null, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>Select some other AutomationElement than the class's</summary>
        /// -------------------------------------------------------------------
        private void TS_Select(AutomationElement element, Type expecetedException, CheckType checkType)
        {
            Comment("Selecting: " + Library.GetUISpyLook(element));

            pattern_Select(element, expecetedException, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_UnSelect(ArrayList list, CheckType checkType)
        {
            foreach (AutomationElement element in list)
            {
                HelperUnselect(element);
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_VerifyElementsAreSelected(ArrayList list, CheckType checkType)
        {
            HelperCorrectSelectionState(list, true, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_VerifyElementsAreNotSelected(ArrayList list, CheckType checkType)
        {
            HelperCorrectSelectionState(list, false, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Some tests are not valid to be ran for certain controls.  Example is the
        /// Win32 Radio buttons.  Since Win32 Radio buttons do not have a selection
        /// container, AddToSelection cannot be validated.
        /// </summary>
        /// -------------------------------------------------------------------
        private void TS_VerifyCorrectConfiguration(CheckType checkType)
        {
            if (_selectionContainer == null)
                ThrowMe(checkType, "This element does not have a selection container so cannot run this test.");

            m_TestStep++;
        }


        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TestDriverSelect0(bool currentlySelected)
        {
            // "Precondition: There is a selection container (ie. Win32 radio buttons do not have a selection container",
            TSC_VerifyProperty(_pattern.Current.SelectionContainer != null, true, SelectionItemPattern.SelectionContainerProperty, CheckType.IncorrectElementConfiguration);

            EventFired[] fireState = new EventFired[] { currentlySelected == true ? EventFired.Undetermined : EventFired.True };
            AutomationProperty[] propertiesFired = new AutomationProperty[] { SelectionItemPattern.IsSelectedProperty };
            ArrayList elementsSelected = new ArrayList();
            elementsSelected.Add(m_le);

            //"Precondition: Element is X selected"
            TSC_VerifyProperty(_pattern.Current.IsSelected, currentlySelected, "IsSelected", CheckType.IncorrectElementConfiguration);

            //  Step: Add a SelectionItemPattern.IsSelectedProperty property change event
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, propertiesFired, CheckType.Verification);

            // Step: Select the element
            TS_Select(m_le, null, CheckType.Verification);

            // Step: Wait for one event to fire
            TSC_WaitForEvents(2);

            // Verification: Verify that the SelectionItemPattern.IsSelectedProperty was fired
            TSC_VerifyPropertyChangedListener(m_le, fireState, propertiesFired, CheckType.Verification);

            // Verify element is selected
            TS_VerifyElementsAreSelected(elementsSelected, CheckType.Verification);
        }


        #endregion TS_* methods

        #region Helpers


        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static void HelperCorrectSelectionState(ArrayList list, bool shouldBeSelected, CheckType checkType)
        {
            foreach (AutomationElement el in list)
            {
                if (shouldBeSelected != ((SelectionItemPattern)el.GetCurrentPattern(SelectionItemPattern.Pattern)).Current.IsSelected)
                    ThrowMe(checkType, "Element(" + Library.GetUISpyLook(el) + ") is not selected");
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static ArrayList HelperSubtract(ArrayList collection, object[] elements)
        {
            ArrayList list = new ArrayList();
            bool found = false;

            foreach (AutomationElement element in collection)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if (Automation.Compare(element, (AutomationElement)elements[i]))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    list.Add(element);
                found = false;
            }
            return list;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static void HelperUnselect(AutomationElement element)
        {
            SelectionItemPattern sip = element.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
            if (sip.Current.IsSelected)
            {
                Comment("Removing selection from : " + Library.GetUISpyLook(element));
                sip.RemoveFromSelection();
            }
        }

        #endregion Helpers

    }
}
