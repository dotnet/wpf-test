// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Controls;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Layout
{
    [Test(2, "PropertyTests", "Code Coverage - Table", MethodName = "Run")]
    public class TableCodeCoverer : AvalonTest
    {
        private Table _table1;
        private Window _w1;

        public TableCodeCoverer()
        {
            CreateLog = false;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        private TestResult Initialize()
        {
            _w1 = new Window();
            UISetup(_w1, out _table1);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w1.Close();            
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            PassMethods(_table1);
            FailMethods(_table1);
            GetProperties(_table1);            
            return TestResult.Pass;
        }

        private void UISetup(Window w1, out Table table1)
        {
            table1 = new Table();
            TableRowGroup tblrg = new TableRowGroup();
            TableRow trow = new TableRow();
            TableCell tc = new TableCell();
            trow.Cells.Add(tc);
            trow.Cells.Add(new TableCell());
            tblrg.Rows.Add(trow);
            (table1 as IAddChild).AddText(" ");
            table1.RowGroups.Add(tblrg);
            tc.FlowDirection = FlowDirection.LeftToRight;
            FlowDocumentReader reader = new FlowDocumentReader();
            FlowDocument fd = new FlowDocument(table1);
            reader.Document = fd;
            w1.Content= reader;
            w1.Show();

            FigureLength fl = new FigureLength(20);
            FigureLength fl1 = new FigureLength(20);
            fl.Equals(fl1);
            int hashcode = fl.GetHashCode();            
        }

        private void PassMethods(Table table1)
        {            
            table1.Columns.Add(new TableColumn());
            table1.Columns.Add(new TableColumn());

            TestLog tableColumnCollectionPassMethods = new TestLog("TableColumnCollection Pass Methods");

            TableColumnCollection tcColl = table1.Columns;
            Array arr = Array.CreateInstance(typeof(TableColumn), tcColl.Count);
            tcColl.CopyTo(arr, 0);
            tcColl.CopyTo(new TableColumn[10], 0);

            // go beyond allocated size
            TableColumn tc1 = new TableColumn();
            tcColl.Add(tc1);
            tcColl.Add(new TableColumn());
            tcColl.Add(new TableColumn());
            tcColl.Add(new TableColumn());
            tcColl.Add(new TableColumn());
            tcColl.Add(new TableColumn());
            tcColl.Insert(0, new TableColumn());
            tcColl.Add(new TableColumn());
            tcColl.Add(new TableColumn());

            int cap = tcColl.Capacity;
            tcColl.Capacity = cap;
            tcColl.Contains(tc1);
            tcColl.IndexOf(tc1);
            tcColl.IndexOf(null);

            tcColl.Remove(tc1);
            tcColl.RemoveRange(2, 4);
            tcColl.Clear();
            tcColl.Contains(null);

            tcColl.TrimToSize();

            tableColumnCollectionPassMethods.Result = TestResult.Pass;
            tableColumnCollectionPassMethods.Close();

            #region TableRowGroupCollection

            TestLog tableRowGroupCollectionPassMethods = new TestLog("TableRowGroupCollection Pass Methods");

            TableRowGroupCollection tblrgColl = table1.RowGroups;
            tblrgColl.Contains(new TableRowGroup());

            TableRowGroup[] newArray = new TableRowGroup[4];
            tblrgColl.Insert(1, new TableRowGroup());
            tblrgColl.Insert(2, new TableRowGroup());
            tblrgColl.Insert(3, new TableRowGroup());
            tblrgColl.CopyTo(newArray, 0);
            tblrgColl.CopyTo(Array.CreateInstance(typeof(TableRowGroup), 4), 0);
            int Cap = tblrgColl.Capacity;
            object root = tblrgColl.SyncRoot;
            int indexOf = tblrgColl.IndexOf(tblrgColl[0]);
            indexOf = tblrgColl.IndexOf(new TableRowGroup());
            tblrgColl.RemoveAt(3);
            tblrgColl.RemoveRange(2, 1);
            tblrgColl.Capacity = 2;
            tblrgColl.TrimToSize();

            TableRowGroup tblrg = tblrgColl[0];

            tableRowGroupCollectionPassMethods.Result = TestResult.Pass;
            tableRowGroupCollectionPassMethods.Close();

            #endregion

            #region TableCellCollection

            TestLog tableCellCollectionPassMethods = new TestLog("TableCell Pass Methods");

            TableCell[] newArray1 = new TableCell[2];
            TableCellCollection tcellColl = ((TableRowGroup)table1.RowGroups[0]).Rows[0].Cells;
            tcellColl.CopyTo(Array.CreateInstance(typeof(TableCell), 2), 0);
            tcellColl.CopyTo(newArray1, 0);
            root = tcellColl.SyncRoot;
            indexOf = tcellColl.IndexOf(new TableCell());
            tcellColl.RemoveRange(0, 1);
            Cap = tcellColl.Capacity;
            tcellColl.Capacity = 2;

            tableCellCollectionPassMethods.Result = TestResult.Pass;
            tableCellCollectionPassMethods.Close();

            #endregion

            #region TableRowCollection

            TestLog tableRowCollectionPassMethods = new TestLog("TableRowCollection Pass Methods");

            TableRowCollection trColl = ((TableRowGroup)table1.RowGroups[0]).Rows;
            trColl.Add(new TableRow());
            TableRow[] newArray2 = new TableRow[2];
            trColl.CopyTo(Array.CreateInstance(typeof(TableRow), 2), 0);
            trColl.CopyTo(newArray2, 0);
            root = trColl.SyncRoot;
            indexOf = trColl.IndexOf(new TableRow());
            trColl.RemoveRange(0, 1);
            Cap = trColl.Capacity;
            trColl.Capacity = 2;
            trColl.TrimToSize();

            tableRowCollectionPassMethods.Result = TestResult.Pass;
            tableRowCollectionPassMethods.Close();

            #endregion
        }

        private void FailMethods(Table table1)
        {
            TestLog tableFailMethods = new TestLog("Table Fail Methods");
            
            try
            {
                (table1 as IAddChild).AddText("Nothing");
            }
            catch (ArgumentException) { }

            try
            {
                (table1 as IAddChild).AddChild(null);
            }
            catch (ArgumentNullException) { }

            TableColumnCollection tcColl = table1.Columns;
            Array arr = Array.CreateInstance(typeof(TableColumn), tcColl.Count);
            try
            {
                tcColl.CopyTo(null, 0);
            }
            catch (ArgumentNullException) { }

            try
            {
                tcColl.CopyTo(arr, -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tcColl.CopyTo(arr, 10);
            }
            catch (ArgumentException) { }

            try
            {
                arr = Array.CreateInstance(typeof(TableColumn), 10, 20);
                tcColl.CopyTo(arr, -1);
            }
            catch (ArgumentException) { }

            TableColumn[] tcArr = null;

            try
            {
                tcColl.CopyTo(tcArr, 0);
            }
            catch (ArgumentNullException) { }

            tcArr = new TableColumn[10];
            try
            {
                tcColl.CopyTo(tcArr, -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tcColl.CopyTo(tcArr, 11);
            }
            catch (ArgumentException) { }

            try
            {
                tcColl.Add(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                TableColumn tc1 = tcColl[20];
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tcColl.Insert(0, null);
            }
            catch (ArgumentNullException) { }

            try
            {
                tcColl.Remove(null);
            }
            catch (ArgumentNullException) { }
            
            TableColumn tcoll1 = new TableColumn();
            tcColl.Remove(tcoll1);           

            try
            {
                tcColl.RemoveRange(0, tcColl.Count);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tcColl.Add(new TableColumn());
                tcColl.RemoveRange(0, -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tcColl.Add(new TableColumn());
                tcColl.Add(new TableColumn());
                tcColl.RemoveRange(0, 4);
            }
            catch (ArgumentException) { }

            tableFailMethods.Result = TestResult.Pass;
            tableFailMethods.Close();

            #region TableRowGroupCollection

            TestLog tableRowGroupFailMethods = new TestLog("TableRowGroup Fail Methods");

            TableRowGroup[] newArrTableRowGroup = null;
            Array arrTableRowGroup = null;
            TableRowGroupCollection tblrgColl = table1.RowGroups;
            try
            {
                tblrgColl.Add(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                tblrgColl.Contains(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                TableRowGroup trg = tblrgColl[-1];
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tblrgColl.CopyTo(arrTableRowGroup, 0);
            }
            catch (ArgumentNullException) { }

            try
            {
                tblrgColl.CopyTo(newArrTableRowGroup, 0);
            }
            catch (ArgumentNullException) { }

            try
            {
                tblrgColl.CopyTo(Array.CreateInstance(typeof(TableRowGroup), 2), -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                newArrTableRowGroup = new TableRowGroup[4];
                tblrgColl.CopyTo(newArrTableRowGroup, -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tblrgColl.CopyTo(Array.CreateInstance(typeof(TableRowGroup), 2), 25);
            }
            catch (ArgumentException) { }

            try
            {
                tblrgColl.CopyTo(newArrTableRowGroup, 25);
            }
            catch (ArgumentException) { }

            try
            {
                tblrgColl.Remove(null);
            }
            catch (ArgumentNullException) { }
           
            tblrgColl.Remove(new TableRowGroup());
           
            try
            {
                tblrgColl.RemoveAt(-1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tblrgColl.RemoveRange(-1, 0);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tblrgColl.RemoveRange(0, -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tblrgColl.RemoveRange(0, 25);
            }
            catch(ArgumentException) { }

            TableRowGroup tblrg = tblrgColl[0];
            try
            {
                (tblrg as IAddChild).AddChild(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                (tblrg as IAddChild).AddText("Text");
            }
            catch (ArgumentException) { }

            try
            {
                (tblrg as IAddChild).AddChild(new TableCell());
            }
            catch(ArgumentException) { }

            tableRowGroupFailMethods.Result = TestResult.Pass;
            tableRowGroupFailMethods.Close();

            #endregion

            #region TableCellCollection

            TestLog tableCellCollectionFailMethods = new TestLog("TableCellCollection Fail Methods");

            TableCell[] newArrTableCell = null;
            Array arrTableCell = null;
            TableCellCollection tcellColl = ((TableRowGroup)table1.RowGroups[0]).Rows[0].Cells;
            try
            {
                tcellColl.Add(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                tcellColl.CopyTo(arrTableCell, 0);
            }
            catch (ArgumentNullException) { }

            try
            {
                tcellColl.CopyTo(newArrTableCell, 0);
            }
            catch (ArgumentNullException) { }

            try
            {
                tcellColl.CopyTo(Array.CreateInstance(typeof(TableCell), 2), 25);
            }
            catch (ArgumentException) { }

            try
            {
                newArrTableCell = new TableCell[4];
                tcellColl.CopyTo(newArrTableCell, 25);
            }
            catch (ArgumentException) { }

            try
            {
                tcellColl.CopyTo(Array.CreateInstance(typeof(TableCell), 2), -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tcellColl.CopyTo(newArrTableCell, -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                TableCell tcell1 = tcellColl[-1];
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tcellColl.Insert(-1, null);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tcellColl.Insert(0, null);
            }
            catch (ArgumentNullException) { }

            try
            {
                tcellColl.Remove(null);
            }
            catch (ArgumentNullException) { }

            tcellColl.Remove(new TableCell());
           
            try
            {
                tcellColl.RemoveRange(-1, 0);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tcellColl.RemoveRange(0, -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                tcellColl.RemoveRange(0, 25);
            }
            catch (ArgumentException) { }

            tableCellCollectionFailMethods.Result = TestResult.Pass;
            tableCellCollectionFailMethods.Close();

            #endregion

            #region TableRowCollection

            TestLog tableRowCollectionFailMethods = new TestLog("TableRowCollection Fail Methods");

            TableRow[] newArrTableRow = null;
            Array arrTableRow = null;
            TableRowCollection trColl = ((TableRowGroup)table1.RowGroups[0]).Rows;
            try
            {
                trColl.Add(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                trColl.CopyTo(newArrTableRow, 0);
            }
            catch (ArgumentNullException) { }

            try
            {
                trColl.CopyTo(arrTableRow, 0);
            }
            catch (ArgumentNullException) { }

            try
            {
                trColl.CopyTo(Array.CreateInstance(typeof(TableCell), 2), -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                newArrTableRow = new TableRow[4];
                trColl.CopyTo(newArrTableRow, -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                trColl.CopyTo(Array.CreateInstance(typeof(TableCell), 2), 25);
            }
            catch (ArgumentException) { }

            try
            {
                trColl.CopyTo(newArrTableRow, 25);
            }
            catch (ArgumentException) { }

            try
            {
                trColl.CopyTo(Array.CreateInstance(typeof(TableCell), 2, 3), 25);
            }
            catch (ArgumentException) { }

            try
            {
                trColl.Insert(0, null);
            }
            catch (ArgumentNullException) { }

            try
            {
                trColl.Insert(-1, null);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                trColl.Remove(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                trColl.RemoveRange(-1, 0);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                trColl.RemoveRange(0, -1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                trColl.RemoveRange(0, 25);
            }
            catch(ArgumentException) { }

            tableRowCollectionFailMethods.Result = TestResult.Pass;
            tableRowCollectionFailMethods.Close();

            #endregion

            TestLog figureLengthFailMethods = new TestLog("FigureLength Fail Methods");
 
            try
            {
                FigureLength fl1 = new FigureLength(double.NaN, FigureUnitType.Pixel);
            }
            catch(ArgumentException) { }

            try
            {
                FigureLength fl2 = new FigureLength(double.PositiveInfinity, FigureUnitType.Pixel);
            }
            catch(ArgumentException) { }

            try
            {
                FigureLength fl3 = new FigureLength(-1, FigureUnitType.Pixel);
            }
            catch(ArgumentOutOfRangeException) { }

            figureLengthFailMethods.Result = TestResult.Pass;
            figureLengthFailMethods.Close();
        }

        private void GetProperties(Table table1)
        {
            ReflectionHelper rh = ReflectionHelper.WrapObject(table1);
        }       
    }
}
