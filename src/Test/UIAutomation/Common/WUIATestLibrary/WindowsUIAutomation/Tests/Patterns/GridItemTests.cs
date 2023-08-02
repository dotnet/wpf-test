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

namespace Microsoft.Test.WindowsUIAutomation.Tests.Patterns
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
    public sealed class GridItemTests : PatternObject
    {
        #region Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        GridItemPattern _pattern = null;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const string THIS = "GridItemTests";

        #endregion Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public static readonly string TestWhichPattern = Automation.PatternName(GridItemPattern.Pattern);

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public GridItemTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.GridItem, dirResults, testEvents, commands)
        {
            _pattern = (GridItemPattern)element.GetCurrentPattern(GridItemPattern.Pattern);
            if (_pattern == null)
				throw new Exception(Helpers.PatternNotSupported);
        }

        #region Tests

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("GridItem.ContainingGridProperty.S.1.1",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(GridItemPattern.ContainingGridProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Step: Traverse up the tree to find a AutomationElement that supports GridPattern",
                "Step: Get the ContainingGrid property",
                "Step: Verify that the two AutomationElements are the same"
            })]
        public void TestGridItemContainingGridPropertyS11(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            AutomationElement grid1 = null;
            AutomationElement grid2 = null;

            //"Step: Traverse up the tree to find a AutomationElement that supports GridPattern",
            TS_GetContainGridByNavigation(m_le, out grid1, CheckType.Verification);

            //"Step: Get the ContainingGrid property",
            TS_GetContainGridByCall(out grid2, CheckType.Verification);

            //"Step: Verify that the two AutomationElements are the same"
            TS_AreTheseTheSame(grid1, grid2, true, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("GridItem.ContainingGridProperty.S.1.3",
            Priority = TestPriorities.Pri0,
            TestCaseType = TestCaseType.Events,
            EventTested = "AutomationPropertyChangedEventHandler(GridItemPattern.ContainingGridProperty)",
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Step: Get the AutomationElement's col",
                "Step: Get the AutomationElement's row",
                "Step: Add and event listener for ContainingGridProperty property change event",
                "Step: Call containing grid's GetCell(row, col)",
                "Step: Wait for event to occur",
                "Step: Verify that listener for ContainingGridProperty property change event did not fire",
                "Step: Verify that AutomationElement and one from GetCell() are the same"
            })]
        public void TestGridItemContainingGridPropertyS13(TestCaseAttribute checkType)
        {
            HeaderComment(checkType);
            int col;
            int row;
            AutomationElement cell = null;

            if (CheckWPFDataGridElement(m_le))   
                return;

            //"Step: Get the AutomationElement's col",
            TS_GetColumn(out col, CheckType.Verification);

            //"Step: Get the AutomationElement's row",
            TS_GetRow(out row, CheckType.Verification);

            //"Step: Add and event listener for ContainingGridProperty property change event",
            TSC_AddPropertyChangedListener(m_le, TreeScope.Element, new AutomationProperty[] { GridItemPattern.ContainingGridProperty }, CheckType.Verification);

            //"Step: Call containing grid's GetCell(row, col)",
            TS_GetCell(row, col, out cell, CheckType.Verification);

            // "Step: Wait for event to occur",
            TSC_WaitForEvents(1);

            //"Step: Verify that listener for ContainingGridProperty property change event did not fire",
            TSC_VerifyPropertyChangedListener(m_le, new EventFired[] { EventFired.False }, new AutomationProperty[] { GridItemPattern.ContainingGridProperty }, CheckType.Verification);

            //"Step: Verify that AutomationElement and one from GetCell() are the same"
            TS_AreTheseTheSame(m_le, cell, true, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("GridItem.ColumnProperty.S.3.1",
            Priority = TestPriorities.BuildVerificationTest,
            TestCaseType = TestCaseType.Arguments,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Verify: Argument supplied is the expected ColumnProperty(y) of the logical element and is of System.Int32 type",
                "Cell is at (x, y), verify that ColumnProperty returns y: ",
            })]
        public void TestGridItemColumnPropertyS31(TestCaseAttribute checkType, object args)
        {
            HeaderComment(checkType);

            if (args == null)
                throw new ArgumentException();

            if (!args.GetType().Equals(typeof(int)))
                ThrowMe(CheckType.Verification, "Invalid argument type");

            if (!_pattern.Current.Column.Equals(Convert.ToInt32(args, CultureInfo.CurrentUICulture)))
                ThrowMe(CheckType.Verification, "ColumnProperty returned " + _pattern.Current.Column);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("GridItem.RowSpanProperty.S.4.1",
            Priority = TestPriorities.BuildVerificationTest,
            TestCaseType = TestCaseType.Arguments,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Description = new string[]{
                "Verify: Argument supplied is the expected RowSpanProperty with System.Int32 type",
                "Cell is merged according to argument supplied",
            })]
        public void TestGridItemColumnPropertyS41(TestCaseAttribute checkType, object args)
        {
            HeaderComment(checkType);

            if (args == null)
                throw new ArgumentException();

            if (!args.GetType().Equals(typeof(int)))
                ThrowMe(CheckType.Verification, "Invalid argument type");

            if (!_pattern.Current.ColumnSpan.Equals(Convert.ToInt32(args, CultureInfo.CurrentUICulture)))
                ThrowMe(CheckType.Verification, "ColumnSpan returned " + _pattern.Current.ColumnSpan);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_GetContainGridByCall(out AutomationElement grid, CheckType ct)
        {
            grid = Grid;
            Comment("Found ContainingGrid(" + Library.GetUISpyLook(grid) + ")");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_GetCell(int row, int col, out AutomationElement cell, CheckType ct)
        {
            cell = (AutomationElement)GridPat.GetItem(row, col);
            Comment("GetCell(" + row + ", " + col + ") = " + Library.GetUISpyLook(cell));
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_GetColumn(out int col, CheckType ct)
        {
            col = _pattern.Current.Column;
            Comment("Getting column count(" + col + ")");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_GetRow(out int row, CheckType ct)
        {
            row = _pattern.Current.Row;
            Comment("Getting row count(" + row + ")");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_AreTheseTheSame(AutomationElement le1, AutomationElement le2, bool ShouldTheyBe, CheckType ct)
        {
            Comment("Comparing AutomationElements (" + Library.GetUISpyLook(le1) + ") and (" + Library.GetUISpyLook(le2) + ")");
            bool results = Automation.Compare(le1, le2).Equals(ShouldTheyBe);
            if (!results)
                ThrowMe(ct, "Compare(" + Library.GetUISpyLook(le1) + ", " + Library.GetUISpyLook(le2) + ") = " + results + " but should be " + ShouldTheyBe);
            Comment("Compare(" + Library.GetUISpyLook(le1) + ", " + Library.GetUISpyLook(le2) + ") == " + ShouldTheyBe);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_GetContainGridByNavigation(AutomationElement element, out AutomationElement grid, CheckType ct)
        {
            grid = element;
            while (
                !(bool)grid.GetCurrentPropertyValue(AutomationElement.IsGridPatternAvailableProperty)
                && element != AutomationElement.RootElement
                )
            {
                grid = TreeWalker.RawViewWalker.GetParent(grid);
                if (grid == null)
                    ThrowMe(ct, "There were not ancestors that suupported GridPattern");
            }

            if (element == AutomationElement.RootElement)
                ThrowMe(ct, "Could not find parent element that supports GridPattern");

            Comment("Found containing grid w/navigation(" + Library.GetUISpyLook(grid) + ")");
            m_TestStep++;
        }

        #endregion Tests

        #region Step/Verification

        #endregion Step/Verification

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        GridPattern GridPat
        {
            get
            {
                return (GridPattern)((AutomationElement)_pattern.Current.ContainingGrid).GetCurrentPattern(GridPattern.Pattern);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        AutomationElement Grid
        {
            get
            {
                return (AutomationElement)_pattern.Current.ContainingGrid;
            }
        }
    }
}
