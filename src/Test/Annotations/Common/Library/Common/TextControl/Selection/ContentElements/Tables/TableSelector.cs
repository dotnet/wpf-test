// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Module for creating selection inside tables in FlowDocuments.
using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Collections.Generic;

namespace Avalon.Test.Annotations
{
    public class TableSelector : FlowElementSelector
    {

        #region Constructor

        public TableSelector(SelectionModule selectionModule)
            : base(selectionModule)
        {            
            // Nothing.
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Select a specific cell within a table.
        /// </summary>
        /// <param name="tableIdx">Name property of table to select within.</param>
        /// <param name="columnIdx">1 based Index of column to select.</param>
        /// <param name="rowIdx">1 based Index of row to select.</param>
        public TextRange Select(string name, int rowIdx, int columnIdx)
        {
            VerifyDocument();
            PrintStatus("Selecting ('" + columnIdx + "', '" + rowIdx + ") in Table '" + name + "'.");
            ElementPosition table = FindTable(name);
            ElementPosition cell = FindCell(table, rowIdx, columnIdx);
            return selectionModule.Select(cell.Start, cell.End);
        }

        /// <summary>
        /// Select the set of cells specified by the following range of row/column tuples.
        /// </summary>
        /// <param name="name">Name of table to select within.</param>
        /// <param name="startRow">1 based index of row to start selection from.</param>
        /// <param name="startColumn">1 based index of column to start selection from.</param>
        /// <param name="endRow">1 based index of row to end selection in.</param>
        /// <param name="endColumn">1 based index of column to start selection in.</param>
        public TextRange Select(string name, int startRow, int startColumn, int endRow, int endColumn)
        {
            VerifyDocument();
            PrintStatus("Selection from (" + startColumn + ", " + startRow + ") to (" + endColumn + ", " + endRow + ") in Table '" + name + "'.");
            ElementPosition table = FindTable(name);
            ElementPosition startCell = FindCell(table, startRow, startColumn);
            ElementPosition endCell = FindCell(table, endRow, endColumn);
            return selectionModule.Select(startCell.Start, endCell.End);
        }

        /// <summary>
        /// Select a set of Rows of a table.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        /// <returns></returns>
        public TextRange Select(string name, int startRow)
        {
            VerifyDocument();
            PrintStatus("Selecting row " + startRow + ".");
            ElementPosition table = FindTable(name);
            ElementPosition row = FindRow(table, startRow);
            return selectionModule.Select(row.Start, row.End);
        }

        /// <summary>
        /// Create a selection within a cell of a table.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="startPosition"></param>
        /// <param name="startOffset"></param>
        /// <param name="endPosition"></param>
        /// <param name="endOffset"></param>
        /// <returns></returns>
        public TextRange Select(string name, int row, int column, PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset)
        {
            VerifyDocument();
            PrintStatus("Selecting from (" + startPosition + ", " + startOffset + ") to (" + endPosition + ", " + endOffset + ") in cell (" + row + ", " + column + ").");
            ElementPosition table = FindTable(name);
            ElementPosition cell = FindCell(table, row, column);
            return Select(cell, startPosition, startOffset, endPosition, endOffset);
        }

        /// <summary>
        /// Create a selection relative to specified Table.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="startPosition"></param>
        /// <param name="startOffset"></param>
        /// <param name="endPosition"></param>
        /// <param name="endOffset"></param>
        /// <returns></returns>
        public TextRange Select(string name, PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset)
        {
            VerifyDocument();
            PrintStatus("Selecting from (" + startPosition + ", " + startOffset + ") to (" + endPosition + ", " + endOffset + ") in table '" + name + "'.");
            ElementPosition table = FindTable(name);
            return Select(table, startPosition, startOffset, endPosition, endOffset);
        }

        /// <summary>
        /// Create a selection that either starts outside the table but ends inside.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="startPosition"></param>
        /// <param name="startOffset"></param>
        /// <param name="endPosition"></param>
        /// <param name="endOffset"></param>
        /// <returns></returns>
        public TextRange Select(string name, PagePosition startPosition, int startOffset, int endRow)
        {
            VerifyDocument();
            PrintStatus("Selecting from (" + startPosition + ", " + startOffset + ") to row " + endRow + " in table '" + name + "'.");
            ElementPosition table = FindTable(name);
            TextPointer startPointer = CreatePointer(table, startPosition);
            startPointer = (TextPointer)selectionModule.CreatePointer(startPointer, startOffset);
            ElementPosition rowPosition = FindRow(table, endRow);
            return selectionModule.Select(startPointer, rowPosition.End);
        }

        /// <summary>
        /// Create a selection that either starts insided the table but ends outside.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="startPosition"></param>
        /// <param name="startOffset"></param>
        /// <param name="endPosition"></param>
        /// <param name="endOffset"></param>
        /// <returns></returns>
        public TextRange Select(string name, int startRow, PagePosition endPosition, int endOffset)
        {
            VerifyDocument();
            PrintStatus("Selecting from row " + startRow + " to (" + endPosition + ", " + endOffset + ") in table '" + name + "'.");
            ElementPosition table = FindTable(name);
            TextPointer endPointer = CreatePointer(table, endPosition);
            endPointer = (TextPointer)selectionModule.CreatePointer(endPointer, endOffset);
            ElementPosition rowPosition = FindRow(table, startRow);
            return selectionModule.Select(rowPosition.Start, endPointer);
        }

        #endregion

        #region Protected Methods

        ElementPosition FindCell(ElementPosition table, int row, int column)
        {
            ElementPosition rowPosition = FindRow(table, row);
            ElementPosition cell = FindNthElement(typeof(TableCell), rowPosition.Start, EndOfDocument, column);
            return cell;
        }

        ElementPosition FindRow(ElementPosition table, int row) 
        {
            ElementPosition rowPosition = FindNthElement(typeof(TableRow), table.Start, EndOfDocument, row);
            return rowPosition;
        }

        /// <summary>
        /// Find table with the given name.
        /// </summary>
        /// <param name="name">Name of Table element to find.</param>
        /// <returns>TextPointer just before the start of the Table with the given name.</returns>
        ElementPosition FindTable(string name)
        {
            return FindElementWithName(typeof(Table), name);
        }

        #endregion
    }
}	
