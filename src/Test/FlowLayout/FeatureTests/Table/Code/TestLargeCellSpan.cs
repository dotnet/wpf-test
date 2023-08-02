// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* 
    Regression_Bug30
    Typographic Tables: Setting RowSpan larger than remaining columns causes exception in MeaseureCore

    Test that table does not throw an exception if TableCell.RowSpan is larger than remaining columns
*/
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Table</area>
    /// <owner>Microsoft</owner>
    /// <priority>2</priority>
    /// <description>
    /// Testing table large cellspan.
    /// </description>
    /// </summary>
    [Test(2, "Table", "LargeCellSpan")]
    public class LargeCellSpan : AvalonTest
    {
        private Window _w;

        #region Test case members

        public struct CellSpanAction
        {
            public static CellSpanAction Create(TableCell cell, int r, int c)
            {
                CellSpanAction action = new CellSpanAction();
                action.TableCell = cell;
                action.RowSpan = r;
                action.ColumnSpan = c;
                return action;
            }

            public int RowSpan;
            public int ColumnSpan;
            public TableCell TableCell;

            public TestResult Apply()
            {
                this.TableCell.RowSpan = RowSpan;
                this.TableCell.ColumnSpan = ColumnSpan;
                return TestResult.Pass;
            }
        }

        #endregion

        #region Constructor

        public LargeCellSpan()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
        }

        #endregion

        #region Test Steps
    
        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            TestResult tempResult = TestResult.Unknown;

            Status("Initialize");
            _w = new Window();
            Table t = new Table();
            TableRowGroup b = new TableRowGroup();
            TableRow r = new TableRow();

            TableCell c = new TableCell();
            r.Cells.Add(c);
            r.Cells.Add(new TableCell());

            b.Rows.Add(new TableRow());
            b.Rows.Add(new TableRow());
            b.Rows.Add(r);

            FlowDocumentScrollViewer textPanel = new FlowDocumentScrollViewer();
            textPanel.Document = new FlowDocument();
            ((System.Windows.Markup.IAddChild)textPanel.Document).AddChild(t);
            t.RowGroups.Add(b);
            _w.Content = textPanel;

            CellSpanAction[] actions = {
                CellSpanAction.Create(c, 1, 1),
                CellSpanAction.Create(c, 1, 2),
                CellSpanAction.Create(c, 1, 5),
                CellSpanAction.Create(c, 1, 1),            
                CellSpanAction.Create(c, 2, 1),
                CellSpanAction.Create(c, 5, 1),
                CellSpanAction.Create(c, 1, 1)
            };

            _w.Show();

            foreach (CellSpanAction action in actions)
            {
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                tempResult = ApplyAction(action);
                if (tempResult == TestResult.Fail)
                {
                    return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        #endregion

        private TestResult ApplyAction(CellSpanAction a)
        {
            Status("Setting ColSpan " + a.ColumnSpan + "RowSpan=" + a.RowSpan);
            return a.Apply();
        }
    }
}
