// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Misc Table bugs regression control
//
//

using System;
using System.Threading;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;

namespace DRT
{
    // 
    // Table DRT's.
    // 
    internal sealed class TableCellAlignmentSuite : FlowLayoutExtSuite
    {

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TableCellAlignmentSuite() : base("TableCellAlignment")
        {
            this.Contact = "Microsoft";
        }

        /// <summary>
        /// Creates array of callbacks
        /// </summary>
        /// <returns>Array of callbacks</returns>
        protected override DrtTest[] CreateTests()
        {

            return new DrtTest[]
            {
                new DrtTest(Root),                      new DrtTest(VerifyLayoutCreate),
                new DrtTest(ChangeAlignmentOfOneCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(ChangeAlignmentOfAllCells), new DrtTest(VerifyLayoutFinalize),
            };
        }

        internal Table GetTableFromTextPanel(UIElement uiElt)
        {
            if(!(uiElt is FlowDocumentScrollViewer))
                return null;

            FlowDocumentScrollViewer fdsv = (FlowDocumentScrollViewer) uiElt;
            IEnumerator e = LogicalTreeHelper.GetChildren(fdsv.Document).GetEnumerator();
            e.MoveNext();

            return (Table) e.Current;
        }

        /// <summary>
        /// Creates DRT's tree
        /// </summary>
        /// <returns>Root of the tree</returns>
        internal void Root()
        {
            FlowDocumentScrollViewer fdsv = (FlowDocumentScrollViewer)LoadFromXaml("TableCellAlignment.xaml");

            _root = GetTableFromTextPanel(fdsv);
            _contentRoot.Child = fdsv;
        }

        internal void ChangeAlignmentOfOneCell()
        {
            string[] cellTargets = new string[] {
                "BodyOneCellA",
                "BodyOneCellB",
            };

            for (int i = 0; i < cellTargets.Length; ++i)
            {
                TableCell cell = (TableCell)LogicalTreeHelper.FindLogicalNode(_root, cellTargets[i]);
                ChangeAlignmentHelper(cell);
            }
        }

        internal void ChangeAlignmentOfAllCells()
        {
            foreach (TableRow row in _root.RowGroups[0].Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    ChangeAlignmentHelper(cell);
                }
            }
        }

        private void ChangeAlignmentHelper(TableCell cell)
        {
            cell.TextAlignment = (TextAlignment)(((int)cell.TextAlignment + 1) % 3);
        }

        private Table _root;
    }
}
