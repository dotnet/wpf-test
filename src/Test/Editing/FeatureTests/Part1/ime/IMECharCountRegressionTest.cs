// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Loggers;
using Test.Uis.TestTypes;

namespace Test.Uis.TextEditing
{
    /// <summary>
    /// Provides regression coverage for TFS Part1 Regression_Bug70
    /// </summary>
    [Test(1, "TextOM", "IMECharCountRegressionTest", MethodParameters = "/TestCaseType:IMECharCountRegressionTest")]
    public class IMECharCountRegressionTest : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        public override void RunTestCase()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 100;
            _rtb.Width = 300;
            _rtb.FontSize = 12;

            _list = new List();
            for (int i = 0; i < numberOfListItems; i++)
            {
                Paragraph p = new Paragraph(new Run(string.Format("item{0}", i)));
                ListItem listItem = new ListItem(p);
                _list.ListItems.Add(listItem);
            }
            _rtb.Document.Blocks.Clear();
            _rtb.Document.Blocks.Add(_list);

            _panel = new StackPanel();
            _panel.Children.Add(_rtb);
            MainWindow.Content = _panel;

            QueueDelegate(AfterLoad); 
        }

        // Performs  the repro steps here.
        private void AfterLoad()
        {
            // Get the 3rd ListItem which is the last ListItem
            ListItem listItem3 = _list.ListItems.LastListItem;
            Separate(listItem3);
            Combine(listItem3);

            QueueDelegate(DoVerification);
        }

        private void DoVerification()
        {
            Verifier.Verify(_list.ListItems.Count == numberOfListItems, 
                "Verifying the number of list items after the editing operations", true);
            Logger.Current.ReportSuccess();
        }

        private void Separate(ListItem listItem)
        {
            if (listItem.PreviousListItem != null)
            {
                List newList = new List();
                newList.MarkerStyle = TextMarkerStyle.Decimal;
                listItem.List.SiblingBlocks.InsertBefore(listItem.List, newList);
                newList.ListItems.Add(listItem.PreviousListItem);
            }
        }

        private void Combine(ListItem listItem)
        {
            if ((listItem.List.PreviousBlock != null) && (listItem.List.PreviousBlock is List))
            {
                // Absorb the last ListItem of the previous block to the current List
                ListItem prevBlockListItem = ((List)listItem.List.PreviousBlock).ListItems.LastListItem;

                // Remove the list from blocks so that list no longer is in tree
                prevBlockListItem.List.SiblingBlocks.Remove(prevBlockListItem.List);

                // Isolate the ListItem from the block removed
                prevBlockListItem.List.ListItems.Remove(prevBlockListItem);

                // Insert to the current list
                listItem.List.ListItems.InsertBefore(listItem.List.ListItems.FirstListItem, prevBlockListItem);
            }
        }

        #endregion

        #region Private fields

        private RichTextBox _rtb = null;
        private StackPanel _panel = null;        
        private List _list = null;
        private const int numberOfListItems = 3;

        #endregion
    }
}
