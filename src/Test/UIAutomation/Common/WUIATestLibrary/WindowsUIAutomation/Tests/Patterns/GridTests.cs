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
    public class GridPatternWrapper : PatternObject
    {
        #region Variables

        // Pattern variable
        GridPattern _pattern;

        #endregion Variables

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		internal GridPatternWrapper(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _pattern = (GridPattern)GetPattern(m_le, m_useCurrent, GridPattern.Pattern);
        }

        #region Properties

        internal int pattern_getColumnCount
        {
            get
            {
                if (m_useCurrent)
                    return _pattern.Current.ColumnCount;
                else
                    return _pattern.Cached.ColumnCount;
            }
        }
        internal int pattern_getRowCount
        {
            get
            {
                if (m_useCurrent)
                    return _pattern.Current.RowCount;
                else
                    return _pattern.Cached.RowCount;
            }
        }

        #endregion Properties

        #region Methods

        internal AutomationElement pattern_GetItem(int row, int column, bool expectedException, CheckType checkType)
        {
            AutomationElement element = null;

            string call = "GetItem(" + row + ", " + column + ")";
            try
            {
                element = _pattern.GetItem(row, column);
            }
            catch (InvalidOperationException e)
            {
                if (!expectedException)
                    ThrowMe(checkType, call + " threw an expection unexpectedly : " + e.Message);
                Comment("Successfully called " + call + " with exception thrown as expected");
                return null;
            }

            if (expectedException)
                ThrowMe(checkType, call + " did not throw an expection as expected");

            Comment("Successfully called " + call + " without an exception thrown");

            return element;
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
    public sealed class GridTests : GridPatternWrapper
    {
        #region Member variables

        const string THIS = "GridTests";

        #endregion Member variables

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public const string TestSuite = NAMESPACE + "." + THIS;

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public static readonly string TestWhichPattern = Automation.PatternName(GridPattern.Pattern);

		/// -------------------------------------------------------------------
		/// <summary></summary>
		/// -------------------------------------------------------------------
		public GridTests(AutomationElement element, TestPriorities priority, string dirResults, bool testEvents, TypeOfControl typeOfControl, IApplicationCommands commands)
            :
            base(element, TestSuite, priority, typeOfControl, TypeOfPattern.Grid, dirResults, testEvents, commands)
        {
        }

        #region Tests

        ///<summary>TestCase: GridPattern.S.1.1/2/3</summary>
        [TestCaseAttribute("GridPattern.S.1.1/2/3",
             Priority = TestPriorities.Pri1,
             Status = TestStatus.Works,
             Author = "Microsoft",
             Description = new string[]{
											"Verify that all cells within ColumnCount and RowCount return !null"
										}
             )]
        public void GetCellS11(TestCaseAttribute testCase)
        {
            HeaderComment(testCase);

            if (CheckWPFDataGridElement(m_le))   
                return;

            AutomationElement le;

            Comment("The row and cell counts are " + pattern_getRowCount + " and " + pattern_getColumnCount);
            for (int col = 0; col < pattern_getColumnCount; col++)
            {
                for (int row = 0; row < pattern_getRowCount; row++)
                {
                    Comment("Looking at GetCell(" + row + ", " + col + ")");
                    le = pattern_GetItem(row, col, false, CheckType.Verification);

                    //


                    if (le == null)
                    {
                        ThrowMe(CheckType.Verification, "GetCell(" + row + ", " + col + ") returned null");
                    }
                }
            }
            m_TestStep++;
        }

        #endregion Tests

        #region Step/Verification
        #endregion Step/Verification

        #region Library

        #endregion Library

    }
}
