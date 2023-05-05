// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using System.Windows;
using System.Collections;
using System.Windows.Documents;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Resources.Cell
{
    /// <summary/>
    public static class CellResources_Verify
    {
        /// <summary>
        /// Cell resources verification
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            DockPanel myPanel = rootElement as DockPanel;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be DockPanel");
                result = false;
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 1)
            {
                GlobalLog.LogEvidence("Should has only 1 child");
                result = false;
            }
            FlowDocumentScrollViewer myTextPanel = myChildren[0] as FlowDocumentScrollViewer;

            if (null == myTextPanel)
            {
                GlobalLog.LogEvidence("Should be FlowDocumentScrollViewer");
                result = false;
            }
            IEnumerator e = LogicalTreeHelper.GetChildren(myTextPanel.Document).GetEnumerator();
            e.MoveNext();


            Table myTable = (Table) e.Current;
            if (null == myTable)
            {
                GlobalLog.LogEvidence("Should Have a table");
                result = false;
            }
            TableRowGroup myBody = myTable.RowGroups[0];
            TableRow      myRow  = myBody.Rows[0] as TableRow;
            if (null == myRow)
            {
                GlobalLog.LogEvidence("Should Have a Row");
                result = false;
            }
            GlobalLog.LogStatus("Verifying Resources ...");

            ResourceDictionary myResources = myRow.Resources;

            if (null == myResources)
            {
                GlobalLog.LogEvidence("null == myResources");
                result = false;
            }
            if (myResources.Count != 1)
            {
                GlobalLog.LogEvidence("myResources.Count != 1");
                result = false;
            }

            String[] myKeys = new String[1];

            myResources.Keys.CopyTo(myKeys, 0);
            foreach (string key in myKeys)
            {
                GlobalLog.LogStatus("key: " + key);
            }

            GlobalLog.LogStatus("Verify foreground ...");
            if (false == myResources.Contains("foreground"))
            {
                GlobalLog.LogStatus("no resources for foreground");
                result = false;
            }
            else
            {
                Type myType = myResources["foreground"].GetType();

                if (null == myType)
                {
                    GlobalLog.LogStatus("null myResources[foreground]");
                    result = false;
                }
                else
                {
                    GlobalLog.LogStatus("Type1: " + myType.FullName);
                }
            }

            SolidColorBrush myForeground = myResources["foreground"] as SolidColorBrush;

            if (null == myForeground)
            {
                GlobalLog.LogEvidence("null == myForeground");
                result = false;
            }
            if (!Color.Equals(myForeground.Color, Colors.Red))
            {
                GlobalLog.LogEvidence("!Color.Equals(myForeground.Color, Colors.Red)");
                result = false;
            }

            TableCell myCell = myRow.Cells[0] as TableCell;
            if (null == myCell)
            {
                GlobalLog.LogEvidence("Should Have a Cell");
                result = false;
            }

            if (((SolidColorBrush)(myCell.Foreground)).Color != ((SolidColorBrush) Brushes.Red).Color)
            {
                GlobalLog.LogEvidence("Foreground should be red, actually: " + myCell.Foreground.ToString());
                result = false;
            }
            return result;
        }
    }
}
