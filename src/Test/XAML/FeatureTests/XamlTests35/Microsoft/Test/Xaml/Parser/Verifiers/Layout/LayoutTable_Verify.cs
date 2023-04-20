// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Layout
{
    /// <summary/>
    public static class LayoutTable_Verify
    {
        /// <summary>
        /// LayoutTable_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Inside LayoutXamlVerifiers.LayoutTableVerify()...");

            DockPanel myPanel = rootElement as DockPanel;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be DockPanel");
                result = false;
            }

            UIElementCollection myChildren = myPanel.Children;

            FlowDocumentScrollViewer myTextPanel = myChildren[0] as FlowDocumentScrollViewer;

            if (null == myTextPanel)
            {
                GlobalLog.LogEvidence("Should be a FlowDocumentScrollViewer");
                result = false;
            }


            IEnumerator e = LogicalTreeHelper.GetChildren(myTextPanel.Document).GetEnumerator();
            e.MoveNext();

            Table myTable = (Table) e.Current;

            if (null == myTable)
            {
                GlobalLog.LogEvidence("No Table");
                result = false;
            }

            GlobalLog.LogStatus("Verifying columns ...");

            TableColumnCollection myColumns = myTable.Columns;

            if (myColumns.Count != 3)
            {
                GlobalLog.LogEvidence("myColumns.Count != 3");
                result = false;
            }

            TableColumn column1 = (TableColumn) LogicalTreeHelper.FindLogicalNode(rootElement, "Column1");
            TableColumn column2 = (TableColumn) LogicalTreeHelper.FindLogicalNode(rootElement, "Column2");
            TableColumn column3 = (TableColumn) LogicalTreeHelper.FindLogicalNode(rootElement, "Column3");

            if (column1.Width.Value != 160)
            {
                GlobalLog.LogEvidence("column1.Width.Value!= 160");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(column1.Background)).Color, Colors.Black))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(column1.Background)).Color!= Colors.Black)");
                result = false;
            }
            if (column3.Width.Value != 160)
            {
                GlobalLog.LogEvidence("column3.Width.Value!= 160");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(column3.Background)).Color, Colors.Black))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(column3.Background)).Color!= Colors.Black)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(column2.Background)).Color, Colors.DarkGray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(column2.Background)).Color!= Colors.DarkGray)");
                result = false;
            }

            // header
            GlobalLog.LogStatus("Verifying header ...");

            TableRowGroup myHeader = myTable.RowGroups[0];

            if (myHeader.Rows.Count != 2)
            {
                GlobalLog.LogEvidence("myHeader.Rows.Count!= 2");
                result = false;
            }

            TableRow r1 = (TableRow) LogicalTreeHelper.FindLogicalNode(rootElement, "R1");
            TableRow r2 = (TableRow) LogicalTreeHelper.FindLogicalNode(rootElement, "R2");

            if (!myHeader.Rows.Contains(r1))
            {
                GlobalLog.LogEvidence("!myHeader.Rows.Contains(r1)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(r1.Background)).Color, Colors.DarkGray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(r1.Background)).Color!= Colors.DarkGray)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(r2.Background)).Color, Colors.DarkGray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(r2.Background)).Color!= Colors.DarkGray)");
                result = false;
            }
            if (r1.Cells.Count != 1)
            {
                GlobalLog.LogEvidence("r1.Cells.Count!= 1");
                result = false;
            }
            if (r2.Cells.Count != 3)
            {
                GlobalLog.LogEvidence("r2.Cells.Count!= 3");
                result = false;
            }

            TableCell c1 = (TableCell) LogicalTreeHelper.FindLogicalNode(rootElement, "C1");
            TableCell c2 = (TableCell) LogicalTreeHelper.FindLogicalNode(rootElement, "C2");
            TableCell c3 = (TableCell) LogicalTreeHelper.FindLogicalNode(rootElement, "C3");
            TableCell c4 = (TableCell) LogicalTreeHelper.FindLogicalNode(rootElement, "C4");

            if (!r1.Cells.Contains(c1))
            {
                GlobalLog.LogEvidence("!r1.Cells.Contains(c1)");
                result = false;
            }
            if (!r2.Cells.Contains(c2))
            {
                GlobalLog.LogEvidence("!r2.Cells.Contains(c2)");
                result = false;
            }
            if (!r2.Cells.Contains(c3))
            {
                GlobalLog.LogEvidence("!r2.Cells.Contains(c3)");
                result = false;
            }
            if (!r2.Cells.Contains(c4))
            {
                GlobalLog.LogEvidence("!r2.Cells.Contains(c4)");
                result = false;
            }
            GlobalLog.LogStatus("Verifying cell ...");
            if (!Color.Equals(((SolidColorBrush)(c1.Foreground)).Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(c1.Foreground)).Color!= Colors.Yellow)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(c2.Foreground)).Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(c2.Foreground)).Color!= Colors.Yellow)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(c3.Foreground)).Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(c3.Foreground)).Color!= Colors.Yellow)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(c4.Foreground)).Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(c4.Foreground)).Color != Colors.Yellow)");
                result = false;
            }
            if (c1.ColumnSpan != 3)
            {
                GlobalLog.LogEvidence("c1.ColumnSpan != 3");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(c1.BorderBrush)).Color, Colors.White))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(c1.BorderBrush)).Color != Colors.White)");
                result = false;
            }
            if (c1.Padding != new Thickness(6))
            {
                GlobalLog.LogEvidence("c1.Padding != new Thickness(6)");
                result = false;
            }
            if (c2.BorderThickness != new Thickness(1))
            {
                GlobalLog.LogEvidence("c2.BorderThickness != new Thickness(1)");
                result = false;
            }

            // footer
            GlobalLog.LogStatus("Verifying footer ...");
            TableRowGroup myFooter = myTable.RowGroups[2];

            if (myFooter.Rows.Count != 2)
            {
                GlobalLog.LogEvidence("myFooter.Rows.Count != 2");
                result = false;
            }

            TableRow r3 = myFooter.Rows[0];
            TableRow r4 = myFooter.Rows[1];

            if (null == r3)
            {
                GlobalLog.LogEvidence("Missed row  r3 in footer");
                result = false;
            }
            if (null == r4)
            {
                GlobalLog.LogEvidence("Missed row r4 in footer");
                result = false;
            }
            if (!myFooter.Rows.Contains(r3))
            {
                GlobalLog.LogEvidence("!myFooter.Rows.Contains(r3)");
                result = false;
            }
            if (!myFooter.Rows.Contains(r4))
            {
                GlobalLog.LogEvidence("!myFooter.Rows.Contains(r4)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(r3.Background)).Color, Colors.DarkGray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(r3.Background)).Color != Colors.DarkGray)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(r4.Background)).Color, Colors.DarkGray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(r4.Background)).Color != Colors.DarkGray)");
                result = false;
            }
            if (r3.Cells.Count != 3)
            {
                GlobalLog.LogEvidence("r3.Cells.Count != 3");
                result = false;
            }
            if (r4.Cells.Count != 1)
            {
                GlobalLog.LogEvidence("r4.Cells.Count != 1");
                result = false;
            }

            TableCell c5 = r4.Cells[0];

            if (!r4.Cells.Contains(c5))
            {
                GlobalLog.LogEvidence("!r4.Cells.Contains(c5)");
                result = false;
            }
            GlobalLog.LogStatus("Verifying cell ...");
            if (!Color.Equals(((SolidColorBrush)(c5.Foreground)).Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(c5.Foreground)).Color != Colors.Yellow)");
                result = false;
            }
            if (c5.ColumnSpan != 3)
            {
                GlobalLog.LogEvidence("c5.ColumnSpan != 3");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(c5.BorderBrush)).Color, Colors.White))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(c5.BorderBrush)).Color != Colors.White)");
                result = false;
            }
            if (c5.Padding != new Thickness(6))
            {
                GlobalLog.LogEvidence("c5.Padding != new Thickness(6)");
                result = false;
            }
            if (c5.BorderThickness != new Thickness(1))
            {
                GlobalLog.LogEvidence("c5.BorderThickness != new Thickness(1)");
                result = false;
            }

            // body
            GlobalLog.LogStatus("Verifying body ...");
            TableRowGroup myBody = myTable.RowGroups[1];

            if (myBody.Rows.Count != 4)
            {
                GlobalLog.LogEvidence("myBody.Rows.Count != 4");
                result = false;
            }

            TableRow r5 = (TableRow) LogicalTreeHelper.FindLogicalNode(rootElement, "R5");
            TableRow r6 = (TableRow) LogicalTreeHelper.FindLogicalNode(rootElement, "R6");
            TableRow r7 = (TableRow) LogicalTreeHelper.FindLogicalNode(rootElement, "R7");
            TableRow r8 = (TableRow) LogicalTreeHelper.FindLogicalNode(rootElement, "R8");

            if (!myBody.Rows.Contains(r5))
            {
                GlobalLog.LogEvidence("!myBody.Rows.Contains(r5)");
                result = false;
            }
            if (!myBody.Rows.Contains(r6))
            {
                GlobalLog.LogEvidence("!myBody.Rows.Contains(r6)");
                result = false;
            }
            if (!myBody.Rows.Contains(r7))
            {
                GlobalLog.LogEvidence("!myBody.Rows.Contains(r7)");
                result = false;
            }
            if (!myBody.Rows.Contains(r8))
            {
                GlobalLog.LogEvidence("!myBody.Rows.Contains(r8)");
                result = false;
            }

            if (r5.Cells.Count != 3)
            {
                GlobalLog.LogEvidence("r5.Cells.Count != 3");
                result = false;
            }
            if (r8.Cells.Count != 1)
            {
                GlobalLog.LogEvidence("r8.Cells.Count != 1");
                result = false;
            }

            TableCell c6 = (TableCell) LogicalTreeHelper.FindLogicalNode(rootElement, "C6");

            if (!r5.Cells.Contains(c6))
            {
                GlobalLog.LogEvidence("!r5.Cells.Contains(c6)");
                result = false;
            }

            GlobalLog.LogStatus("Verifying cell ...");
            if (!Color.Equals(((SolidColorBrush)(c6.Foreground)).Color, Colors.White))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(c6.Foreground)).Color != Colors.White)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(c6.BorderBrush)).Color, Colors.White))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(c6.BorderBrush)).Color != Colors.White)");
                result = false;
            }
            if (c6.Padding != new Thickness(6))
            {
                GlobalLog.LogEvidence("c6.Padding != new Thickness(6)");
                result = false;
            }
            if (c6.BorderThickness != new Thickness(1))
            {
                GlobalLog.LogEvidence("c6.BorderThickness != new Thickness(1)");
                result = false;
            }
            TextRange range = new TextRange((c6.ContentStart), (c6.ContentEnd));
            if (range.Text != "Cell 2")
            {
                GlobalLog.LogEvidence("range.Text != Cell 2");
                result = false;
            }
            return result;
        }
    }
}
