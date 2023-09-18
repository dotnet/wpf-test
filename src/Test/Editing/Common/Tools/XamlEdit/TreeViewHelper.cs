// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections;
using System.Windows;
using System.IO;
using System.Xml;
using System.Windows.Markup;

namespace  XamlPadEdit
{
    class TreeViewhelper
    {
        /// <summary>
        /// Create or fill up a treee view of document content
        /// </summary>
        /// <param name="treeView">Pass an TreeView control</param>
        /// <param name="document">Pass in an Document object</param>
        /// <returns></returns>
        public static TreeView SetupTreeView(TreeView treeView, FlowDocument document)
        {
            TreeViewItem root;
            if (treeView == null)
            {
                treeView = new TreeView();
                treeView.Visibility = Visibility.Visible;
            }
            if (document != null)
            {
                
                treeView.Items.Clear();
                root = new TreeViewItem();
                root.Header = "Document";
                root.IsExpanded = true;
                root.Tag = document;
                treeView.Items.Add(root);
                AddCollection(root, document.Blocks as IList);
            }

            return treeView;
        }

        static void AddCollection(TreeViewItem item, IList list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TreeViewItem titem = new TreeViewItem();
                titem.IsExpanded = true;
                titem.Header = list[i].GetType().Name;
                titem.Tag = list[i];
                item.Items.Add(titem);
                AddItem(titem, list[i] as TextElement);
            }
        }

        static void AddItem(TreeViewItem item, TextElement textElement)
        {
            TreeViewItem childItem;

            if (textElement is InlineUIContainer)
            {
                childItem = new TreeViewItem();
                childItem.Header = ((InlineUIContainer)textElement).Child.GetType().Name;
                childItem.Tag = ((InlineUIContainer)textElement).Child;
                childItem.IsExpanded = true;
                item.Items.Add(childItem);
            }
            else if (textElement is BlockUIContainer)
            {
                childItem = new TreeViewItem();
                childItem.Header = ((BlockUIContainer)textElement).Child.GetType().Name;
                childItem.Tag = ((BlockUIContainer)textElement).Child;
                childItem.IsExpanded = true;
                item.Items.Add(childItem);
            }
            else if (textElement is Span)
            {
                AddCollection(item, ((Span)textElement).Inlines);
            }
            else if (textElement is Paragraph)
            {
                AddCollection(item, ((Paragraph)textElement).Inlines);
            }
            else if (textElement is List)
            {
                AddCollection(item, ((List)textElement).ListItems);
            }
            else if (textElement is ListItem)
            {
                AddCollection(item, ((ListItem)textElement).Blocks);
            }
            else if (textElement is Table)
            {
                TableTreeView(item, textElement as Table);
            }
            else if (textElement is AnchoredBlock)
            {
                Floater floater = textElement as Floater;
                AddCollection(item, ((AnchoredBlock)textElement).Blocks);
            }

            //at last, the element should be a inline (Run) and we try to show its text.
            else if (textElement is Inline)
            {
                TextRange range = new TextRange(((Inline)textElement).ContentEnd, ((Inline)textElement).ContentStart);
                string decorations = "";
                if (((Inline)textElement).TextDecorations == TextDecorations.Underline)
                {
                    decorations = " UnderLine ";
                }
                else
                if (((Inline)textElement).TextDecorations==(TextDecorations.Baseline))
                {
                    decorations = " Baseline ";
                }
                else
                    if (((Inline)textElement).TextDecorations == TextDecorations.OverLine)
                {
                    decorations = " OverLine ";
                }
                else
                    if (((Inline)textElement).TextDecorations == TextDecorations.Strikethrough)
                {
                    decorations = " StrikeThrough ";
                }

                if(((Inline)textElement).FontWeight == FontWeights.Bold)
                {
                        decorations += " Bold ";
                }
                if (((Inline)textElement).FontStyle == FontStyles.Italic)
                {
                    decorations += " Italic ";
                }

                item.Header = item.Header + decorations + " - [" + range.Text + "]";
            }
        }

        static void TableTreeView(TreeViewItem item, Table table)
        {
            TreeViewItem item1, item2;
            foreach (TableRowGroup rg in table.RowGroups)
            {
                foreach (TableRow tr in rg.Rows)
                {
                    item1 = new TreeViewItem();
                    item1.IsExpanded = true;
                    item1.Header = "TableRow";
                    item1.Tag = tr;
                    item.Items.Add(item1);
                    foreach (TableCell tc in tr.Cells)
                    {
                        item2 = new TreeViewItem();
                        item2.Header = "TableCell";
                        item2.Tag = tc;
                        item1.Items.Add(item2);
                        item2.IsExpanded = true;
                        AddCollection(item2, tc.Blocks);
                    }
                }
            }
        }
    }
}
