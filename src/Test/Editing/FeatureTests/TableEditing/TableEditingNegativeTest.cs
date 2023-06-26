// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 21 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/TableEditing/ParagraphEditingWithMouse.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.ComponentModel;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data; 
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// This test only catch crashes when perfrom keyboard editing over a table.
    /// We create selections through an range in a table and perform some important keyboard actions.
    /// </summary>
    [Test(0, "TableEditing", "TableNegativeTest", MethodParameters = "/TestCaseType=TableNegativeTest", Timeout = 240, Keywords = "Setup_SanitySuite,MicroSuite")]
    [TestOwner("Microsoft"), TestWorkItem("126"), TestTactics("658"), TestBugs("")]
    public class TableNegativeTest : ManagedCombinatorialTestCase
    {
        /// <summary>Start a combination.</summary>
        protected override void DoRunCombination()
        {                        
            _selectionStart = 0;
            _prevSelectionStart = -1;
            QueueDelegate(SetUp);
        }

        void SetUp()
        {
            TestElement = new RichTextBox();
            _wrapper = new UIElementWrapper(TestElement);
            FlowDocument doc = ((RichTextBox)TestElement).Document;
            doc.Blocks.Clear();
            doc.Blocks.Add(TableEditingHelper.TableGenerator(_tableMetrix.RowGroup, _tableMetrix.Rows, _tableMetrix.Cols));
            _totalPositions = _wrapper.Start.GetOffsetToPosition(_wrapper.End);

            Log("*** Sub-combination: " +
                "SelectionStart [" + _selectionStart + "] " +
                "Totalpositions [" + _totalPositions + "]");            
            
            QueueDelegate(PrepareSelection);
        }

        void PrepareSelection()
        {            
            _wrapper.SelectionInstance.Select(_wrapper.Start.GetPositionAtOffset(_selectionStart,LogicalDirection.Forward), _wrapper.Start.GetPositionAtOffset(_selectionStart + _selectionLength));

            //make sure we are progressing for the selection start position.
            if (_wrapper.Start.GetOffsetToPosition(_wrapper.SelectionInstance.Start) <= _prevSelectionStart)
            {
                Log("...Skipping this Sub-combination as it is same as last one");
                _selectionStart++;
                if ((_selectionStart > _totalPositions)||(_selectionStart + _selectionLength > _totalPositions))
                {
                    NextCombination();
                }
                else
                {
                    SetUp();
                }                
                return;
            }
            Log("Normalized selection start offset: " + _wrapper.Start.GetOffsetToPosition(_wrapper.SelectionInstance.Start));
            Log("Normalized selection end offset: " + _wrapper.Start.GetOffsetToPosition(_wrapper.SelectionInstance.End));            

            TestElement.Focus();

            QueueDelegate(PerformKeyboardAction);            
        }

        void PerformKeyboardAction()
        {
            KeyboardInput.TypeString(_keyboardAction);

            _prevSelectionStart = _wrapper.Start.GetOffsetToPosition(_wrapper.SelectionInstance.Start);
            _selectionStart++;
            if (_selectionStart + _selectionLength > _totalPositions)
            {
                QueueDelegate(NextCombination);                
            }
            else
            {
                QueueDelegate(SetUp);
            }
        }
        
        /// <summary>Table size</summary>
        public static TableMetrix[] TableValues ={
            new TableMetrix(1, 1, 1),
            new TableMetrix(1, 1, 2),
        };

        /// <summary>Selection value for table editing</summary>
        public static object[] SelectionValues = { 0, 3 };

        /// <summary>Keyboard actions.</summary>
        public static string[] KeyValues = {
            "{ENTER}",
            "{DELETE}",
            "{BACKSPACE}",
            "^x",
            "^v",
            "a",
        };

        string _keyboardAction = string.Empty;
        int _selectionLength = 0;
        TableMetrix _tableMetrix = null;

        int _selectionStart,_prevSelectionStart;
        int _totalPositions;        
        UIElementWrapper _wrapper; 
    }

    /// <summary>Helper functions for Table Editing.</summary>
    public class TableEditingHelper
    {
        /// <summary>
        /// Check if a Position is between cells.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static bool IsPositionBetweenCells(TextPointer pointer)
        {
            return IsPositionAtBeginningOfCell(pointer) && IsPositionAtEndOfCell(pointer);
        }

        /// <summary>
        /// Check if a Position is between Rows.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static bool IsPositionBetweenRows(TextPointer pointer)
        {
            return IsPositionAtBeginningOfRow(pointer) && IsPositionAtEndOfRow(pointer);
        }

        /// <summary>
        /// check if a position is at the beginning of a Cell
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static bool IsPositionAtBeginningOfCell(TextPointer pointer)
        {
            return pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart &&
               pointer.GetAdjacentElement(LogicalDirection.Forward) is TableCell;
        }

        /// <summary>
        /// check if a position is a the beginning of a Row.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static bool IsPositionAtBeginningOfRow(TextPointer pointer)
        {
            return pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart &&
             pointer.GetAdjacentElement(LogicalDirection.Forward) is TableRow;
        }

        /// <summary>
        /// Check if a position if at the end of a Row.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static bool IsPositionAtEndOfRow(TextPointer pointer)
        {
            return pointer.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd &&
               pointer.GetAdjacentElement(LogicalDirection.Backward) is TableRow;
        }
        
        /// <summary>
        /// Check if a position is at the end of a cell.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static bool IsPositionAtEndOfCell(TextPointer pointer)
        {
            return pointer.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd &&
               pointer.GetAdjacentElement(LogicalDirection.Backward) is TableCell;
        }

        /// <summary>
        /// check if a Position is at the Beginning of a table.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static bool IsPositionAtBeginningOfTable(TextPointer pointer)
        {
            return IsPositionAtBeginningOfRow(pointer) && !IsPositionAtEndOfRow(pointer);
        }

        /// <summary>
        /// Check if a Positoin is at the end of a Table.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static bool IsPositionAtEndOfTable(TextPointer pointer)
        {
            return IsPositionAtEndOfRow(pointer) && !IsPositionAtBeginningOfRow(pointer) ;
        }

        /// <summary>
        /// Check if a Position is inside a table cell.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static bool IsPositionInsideTableCell(TextPointer pointer)
        {
           return false;
        }

        /// <summary>
        /// Create a table 
        /// </summary>
        /// <param name="rowGoup"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Table TableGenerator(int rowGoup, int row, int col)
        {
            Table result;

            result = new Table();
            for (int i = 0; i < rowGoup; i++)
            {
                result.RowGroups.Add(CreateBody(row, col));
            }
            return result;
        }

        static TableRowGroup CreateBody(int row, int col)
        {
            TableRowGroup result;

            result = new TableRowGroup();
            for (int i = 0; i < row; i++)
            {
                result.Rows.Add(CreateRow(col));
            }

            return result;
        }

        static TableRow CreateRow(int col)
        {
            TableRow result;

            result = new TableRow();

            for (int i = 0; i < col; i++)
            {
                result.Cells.Add(CreateCell());
            }

            return result;
        }

        static TableCell CreateCell()
        {
            TableCell result;

            result = new TableCell();
            result.BorderThickness = new Thickness(1, 1, 1, 1);
            result.BorderBrush = Brushes.Black; 

            result.Blocks.Add(new Paragraph(new Run("C")));
            return result;
        }
    }

    /// <summary>
    /// Table size definition.
    /// </summary>
    public class TableMetrix
    {
        /// <summary>number of rows Groups</summary>
        public int RowGroup;
        /// <summary>number of rows</summary>
        public int Rows;
        /// <summary></summary>
        public int Cols;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="rowGroup"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public TableMetrix(int rowGroup, int rows, int cols)
        {
            RowGroup = rowGroup;
            Rows = rows;
            Cols = cols;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return RowGroup + " X " + Rows + " X " + Cols;
        }
    }

    /// <summary>
    /// Regression for Regression_Bug556 - Table: Top row is not selected when the table cells are empty - they have no paragraphs within
    /// </summary>
    [TestOwner("Microsoft"), TestWorkItem("125"), TestTactics("556"), TestBugs(""), TestLastUpdatedOn("July 11, 2006")]
    public class EmptyTableSelection : CombinedTestCase
    {
        TextPointer _startPointer ; 
        int _selectionLength = 2;
        UIElementWrapper _wrapper; 
        
        /// <summary>
        /// 
        /// </summary>
        public override void RunTestCase()
        {
            RichTextBox richBox = new RichTextBox();
            richBox.Document.Blocks.Clear();
            richBox.Document.Blocks.Add(CreateATable());
            MainWindow.Content = richBox;
            _startPointer = richBox.Document.ContentStart.GetPositionAtOffset(0);
            _wrapper = new UIElementWrapper(richBox);
            QueueDelegate(GetFocus);
        }

        void GetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(StartOuterLoop);
        }
        void StartOuterLoop()
        {
            
            int pointerOffset = _startPointer.GetOffsetToPosition(_wrapper.End);
            if (pointerOffset > 0)
            {
                _startPointer = _startPointer.GetPositionAtOffset(1);
                QueueDelegate(StartInnerLoop);
            }
            else
            {
                EndTest();
            }
        }

        void StartInnerLoop()
        {
            int pointerOffset = _startPointer.GetOffsetToPosition(_wrapper.End);
            Log("SelectionLength[" + _selectionLength + "], SelectionStart Index[ " + _wrapper.Start.GetOffsetToPosition(_startPointer) + "]");
            if (pointerOffset > _selectionLength)
            {
                _wrapper.SelectionInstance.Select(_startPointer, _startPointer.GetPositionAtOffset(_selectionLength));
                string str = Test.Uis.Utils.XamlUtils.TextRange_GetXml(_wrapper.SelectionInstance);
                if (_wrapper.SelectionInstance.Start.GetOffsetToPosition(_wrapper.SelectionInstance.End) > 0)
                {
                    Verifier.Verify(str.Contains("<Table"), "Falied: selection does not contains Table!");
                }
                _selectionLength++;
                QueueDelegate(StartInnerLoop);
            }
            else 
            {
                _selectionLength = 2;
                QueueDelegate(StartOuterLoop);
            }
        }

        Table CreateATable()
        {
            Table table = new Table();
            table.BorderThickness = new Thickness(1, 1, 1, 1);
            table.BorderBrush = Brushes.Blue;
            for (int i = 0; i < 4; i++)
            {
                table.Columns.Add(new TableColumn());
            }
            TableRowGroup rowGroup = new TableRowGroup();
            table.RowGroups.Add(rowGroup);
            for (int j = 0; j<4; j++)
            {
                TableRow row = new TableRow();
                rowGroup.Rows.Add(row);
                for(int k = 0; k<4; k++)
                {
                    TableCell cell = new TableCell();
                    cell.BorderBrush = Brushes.Blue;
                    cell.BorderThickness = new Thickness(1, 1, 1, 1);
                    row.Cells.Add(cell);
                }
            }
            return table; 
        }

    }
}
