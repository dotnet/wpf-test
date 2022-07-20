// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 

using System;
using System.Windows;
using System.Windows.Documents;

namespace Avalon.Test.Annotations
{
    /// <summary>
    /// Object that describes a selection within a Table.
    /// </summary>
    public class TableSelectionData : ISelectionData
    {
        public TableSelectionData(string tableName)
        {
            _mode = SelectionType.WholeTable;
            _tableName = tableName;
        }

        /// <summary>
        /// Definition for a selection relative to a table element.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="startPosition"></param>
        /// <param name="startOffset"></param>
        /// <param name="endPosition"></param>
        /// <param name="endOffset"></param>
        public TableSelectionData(string tableName, PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset)
        {
            _mode = SelectionType.RelativeToTable;
            _tableName = tableName;
            _startPosition = startPosition;
            _startOffset = startOffset;
            _endPosition = endPosition;
            _endOffset = endOffset;
        }

        /// <summary>
        /// Definition for a selection that starts outside the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="startPosition"></param>
        /// <param name="startOffset"></param>
        /// <param name="endPosition"></param>
        /// <param name="endOffset"></param>
        public TableSelectionData(string tableName, PagePosition startPosition, int startOffset, int endRow)
        {
            _mode = SelectionType.IntoTable;
            _tableName = tableName;
            _startPosition = startPosition;
            _startOffset = startOffset;
            _endRow = endRow;
        }

        /// <summary>
        /// Definition for a selection that end outside the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="startPosition"></param>
        /// <param name="startOffset"></param>
        /// <param name="endPosition"></param>
        /// <param name="endOffset"></param>
        public TableSelectionData(string tableName, int startRow, PagePosition endPosition, int endOffset)
        {
            _mode = SelectionType.OutofTable;
            _tableName = tableName;
            _endPosition = endPosition;
            _endOffset = endOffset;
            _startRow = startRow;
        }

        /// <summary>
        /// Definition for selecting a single cell of a table.
        /// </summary>
        public TableSelectionData(string tableName, int row, int column)
        {
            _mode = SelectionType.Cells;
            _tableName = tableName;
            _startRow = _endRow = row;
            _startColumn = _endColumn = column;
        }

        /// <summary>
        /// Definition for selecting a single row of a table.
        /// </summary>
        public TableSelectionData(string tableName, int startRow)
        {
            _mode = SelectionType.ByRow;
            _tableName = tableName;
            _startRow = _endRow = startRow;
            _startColumn = _endColumn = -1;
        }

        /// <summary>
        /// Definition for a selection that spans multiple rows and columns.
        /// </summary>
        public TableSelectionData(string tableName, int startRow, int startColumn, int endRow, int endColumn)
        {
            _mode = SelectionType.Cells;
            _tableName = tableName;
            _startRow = startRow;
            _startColumn = startColumn;
            _endRow = endRow;
            _endColumn = endColumn;
        }

        /// <summary>
        /// Definition for a selection that is only a portion of a table cell.
        /// </summary>
        /// <param name="tableName">Name of table to make selection on.</param>
        /// <param name="row">1 based Row number of cell.</param>
        /// <param name="column">1 based Column of cell.</param>
        /// <param name="startPosition">Position relative to cell to begin selection.</param>
        /// <param name="startOffset">Offset relative to startPosition: 0 is on cell edge, 1 is right before first character.</param>
        /// <param name="endPosition">Position relative to cell to end selection.</param>
        /// <param name="endOffset">Offset relative to enddPosition: 0 is on cell edge, -1 is right before last character.</param>
        public TableSelectionData(string tableName, int row, int column, PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset)
            : this(tableName, row, column)
        {
            _mode = SelectionType.IntraCell;
            _startPosition = startPosition;
            _startOffset = startOffset;
            _endPosition = endPosition;
            _endOffset = endOffset;
        }

        protected override TextRange DoSetSelection(SelectionModule selectionModule)
        {
            TextRange result;
            switch (_mode)
            {
                case SelectionType.ByRow:
                    result = selectionModule.Tables.Select(_tableName, _startRow);
                    break;
                case SelectionType.IntoTable:
                    result = selectionModule.Tables.Select(_tableName, _startPosition, _startOffset, _endRow);
                    break;
                case SelectionType.IntraCell:
                    result = selectionModule.Tables.Select(_tableName, _startRow, _startColumn, _startPosition, _startOffset, _endPosition, _endOffset);
                    break;
                case SelectionType.Cells:
                    result = selectionModule.Tables.Select(_tableName, _startRow, _startColumn, _endRow, _endColumn);
                    break;
                case SelectionType.OutofTable:
                    result = selectionModule.Tables.Select(_tableName, _startRow, _endPosition, _endOffset);
                    break;
                case SelectionType.RelativeToTable:
                    result = selectionModule.Tables.Select(_tableName, _startPosition, _startOffset, _endPosition, _endOffset);
                    break;
                case SelectionType.WholeTable:
                    result = selectionModule.Tables.Select(_tableName, PagePosition.Beginning, 0, PagePosition.End, 0);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result;
        }

        #region Fields

        SelectionType _mode;

        string _tableName;
        int _startRow = -1, _startColumn = -1;
        int _endRow = -1, _endColumn = -1;

        PagePosition _startPosition, _endPosition;
        int _startOffset = 0, _endOffset = 0;

        #endregion

        private enum SelectionType
        {
            SingleCell,
            Cells,
            ByRow,
            IntraCell,
            IntoTable,
            OutofTable,
            WholeTable,
            RelativeToTable
        }
    }
}
