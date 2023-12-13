// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************************
 *
 *
 * Description: Heler.cs implement the immediate window with the following
 * features:
 *      1. Create tree view for FlowDocument
 *      2. Xaml Helper to save and load xaml
 *      3. Indent the xaml
 *      4. Coloring the xaml in RichTextBox 
 *
 *******************************************************************************/

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

namespace EditingExaminer
{
    /// <summary>
    /// provide help function for tree view the document.
    /// </summary>
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
            DocumentTreeVeiwItem root;

            if (treeView == null)
            {
                treeView = new TreeView();
                treeView.Visibility = Visibility.Visible;
            }
            if (document != null)
            {
                treeView.Items.Clear();
                root = new DocumentTreeVeiwItem();
                root.Header = "Document";
                root.ItemObject = document; 
                treeView.Items.Add(root);
                AddCollection(root, document.Blocks as IList);
            }
         
            return treeView;
        }

        static void AddCollection(TreeViewItem item, IList list)
        {

            for (int i = 0; i < list.Count; i++)
            {
                DocumentTreeVeiwItem titem = new DocumentTreeVeiwItem();
                item.Items.Add(titem);
                titem.Header = list[i].GetType().Name;
                titem.ItemObject = list[i]; 
                AddItem(titem, list[i] as TextElement);
            }
        }

        static void AddItem(TreeViewItem item, TextElement textElement)
        {
            DocumentTreeVeiwItem childItem;

            if (textElement is InlineUIContainer)
            {
                childItem = new DocumentTreeVeiwItem();
                childItem.Header = ((InlineUIContainer)textElement).Child.GetType().Name;
                childItem.ItemObject = ((InlineUIContainer)textElement).Child;
                item.Items.Add(childItem);
            }
            else if (textElement is BlockUIContainer)
            {
                childItem = new DocumentTreeVeiwItem();
                childItem.Header = ((BlockUIContainer)textElement).Child.GetType().Name;
                childItem.ItemObject = textElement;
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
                childItem = new DocumentTreeVeiwItem();
                childItem.ItemObject = textElement;
                item.Header = item.Header + " - [" + range.Text + "]";
            }
        }

        static void TableTreeView(TreeViewItem item, Table table)
        {
            DocumentTreeVeiwItem item1, item2;
            foreach (TableRowGroup rg in table.RowGroups)
            {
                foreach (TableRow tr in rg.Rows)
                {
                    item1 = new DocumentTreeVeiwItem();
                    item1.ItemObject = tr;
                    item1.Header = "TableRow";
                    item.Items.Add(item1);
                    foreach (TableCell tc in tr.Cells)
                    {
                        item2 = new DocumentTreeVeiwItem();
                        item2.Header = "TableCell";
                        item1.Items.Add(item2);
                        item2.ItemObject = tc ;
                        AddCollection(item2, tc.Blocks);
                    }
                }
            }
        }
    }

    /// <summary>
    /// provide help functions for process Xaml.
    /// </summary>
    public class XamlHelper
    {
        /// <summary>
        /// Get xaml from TextRange.Xml property
        /// </summary>
        /// <param name="range">TextRange</param>
        /// <returns>return a string serialized from the TextRange</returns>
        public static string TextRange_GetXml(TextRange range)
        {
            MemoryStream mstream;

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            mstream = new MemoryStream();
            range.Save(mstream, DataFormats.Xaml);

            //must move the stream pointer to the beginning since range.save() will move it to the end.
            mstream.Seek(0, SeekOrigin.Begin);

            //Create a stream reader to read the xaml.
            StreamReader stringReader = new StreamReader(mstream);

            return stringReader.ReadToEnd();
        }

        /// <summary>
        /// Set xml to TextRange.Xml property.
        /// </summary>
        /// <param name="range">TextRange</param>
        /// <param name="xaml">Xaml to be set</param>
        public static void TextRange_SetXml(TextRange range, string xaml)
        {
            MemoryStream mstream;
            if (null == xaml)
            {
                throw new ArgumentNullException("xaml");
            }
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            mstream = new MemoryStream();
            StreamWriter sWriter = new StreamWriter(mstream);

            mstream.Seek(0, SeekOrigin.Begin); //this line may not be needed.
            sWriter.Write(xaml);
            sWriter.Flush();

            //move the stream pointer to the beginning. 
            mstream.Seek(0, SeekOrigin.Begin);

            range.Load(mstream, DataFormats.Xaml);
        }

        /// <summary>
        /// parse a string to avalon object.
        /// </summary>
        /// <param name="str">string to be parsed</param>
        /// <returns>return an object</returns>
        public static object ParseXaml(string str)
        {
            MemoryStream ms = new MemoryStream(str.Length);
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(str);
            sw.Flush();

            ms.Seek(0, SeekOrigin.Begin);

            ParserContext pc = new ParserContext();

            pc.BaseUri = new Uri(System.Environment.CurrentDirectory + "/");
           
            return XamlReader.Load(ms, pc);          
        }

        public static string IndentXaml(string xaml)
        {
            //open the string as an XML node
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xaml);
            XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc);

            //write it back onto a stringWriter
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter);
            xmlWriter.Formatting = System.Xml.Formatting.Indented;
            xmlWriter.Indentation = 4;
            xmlWriter.IndentChar = ' ';
            xmlWriter.WriteNode(nodeReader, false);

            string result = stringWriter.ToString();
            xmlWriter.Close();

            return result;
        }

        /// <summary>
        /// Remove the indentation from a xaml 
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        public static string RemoveIndentation(string xaml)
        {
            if(xaml.Contains("\r\n    "))
            {
               return RemoveIndentation(xaml.Replace("\r\n    ", "\r\n"));
            }
            else 
            {
                return xaml.Replace("\r\n", "");
            }
        }

        /// <summary>
        /// Set the formats on the xaml tages.
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        public static string ColoringXaml(string xaml)
        {
            string[] strs;
            string value = "";
            string s1, s2;
            s1 = "<Section xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph>";
            s2 = "</Paragraph></Section>";

            strs = xaml.Split(new char[] { '<' });
            for (int i = 1; i < strs.Length; i++)
            {
                value += ProcessEachTag(strs[i]);
            }
            return s1 + value + s2;
        }

        static string ProcessEachTag(string str)
        {
            string front = "<Run Foreground=\"Blue\">&lt;</Run>";
            string end = "<Run Foreground=\"Blue\">&gt;</Run>";
            string frontWithSlash = "<Run Foreground=\"Blue\">&lt;/</Run>";
            string endWithSlash = "<Run Foreground=\"Blue\"> /&gt;</Run>";//a space is added.
            string tagNameStart = "<Run FontWeight=\"Bold\">";
            string propertynameStart = "<Run Foreground=\"Red\">";
            string propertyValueStart = "\"<Run Foreground=\"Blue\">";
            string endRun = "</Run>";
            string returnValue;
            string[] strs;
            int i = 0;

            //Front Mark is removed the tag look like the following:
            //1. abc d="??">...
            //2. /abc> 
            //3. abc />
            if (str.StartsWith("/"))
            {   //if the tag is a end tag, we will remove the "/"
                returnValue = frontWithSlash;
                str = str.Substring(1).TrimStart();
            }
            else
            {
                returnValue = front;
            }
            strs = str.Split(new char[] { '>' });
            str = strs[0];
            i = (str.EndsWith("/")) ? 1 : 0;

            str = str.Substring(0, str.Length - i).Trim();

            if (str.Contains("="))//the tag has property
            {
                //set tagName 
                returnValue += tagNameStart + str.Substring(0, str.IndexOf(" ")) + endRun + " ";
                str = str.Substring(str.IndexOf(" ")).Trim();
            }
            else //no property
            {
                returnValue += tagNameStart + str.Trim() + endRun + " ";
                //nothing left to parse
                str = "";
            }

            //Take care of properties:
            while (str.Length > 0)
            {
                returnValue += propertynameStart + str.Substring(0, str.IndexOf("=")) + endRun + "=";
                str = str.Substring(str.IndexOf("\"") + 1).Trim();
                returnValue += propertyValueStart + str.Substring(0, str.IndexOf("\"")) + endRun + "\" ";
                str = str.Substring(str.IndexOf("\"") + 1).Trim();
            }

            if (returnValue.EndsWith(" "))
            {
                returnValue = returnValue.Substring(0, returnValue.Length - 1);
            }

            returnValue += (i == 1) ? endWithSlash : end;

            //Add the content after the ">"
            returnValue += strs[1];

            return returnValue;
        }
    }

    /// <summary>
    /// DocumentTreeVeiwItem is a TreeviewItem that hold hold a object inside the RichTextBox.
    /// </summary>
    internal class DocumentTreeVeiwItem : TreeViewItem
    {
        object _object;
        public DocumentTreeVeiwItem()
        {
            this.IsExpanded = true; 
        }

        /// <summary>
        /// return the object representated by this item. 
        /// </summary>
        public object ItemObject
        {
            get
            {
                return _object; 
            }
            set
            {
                //we don't accept null
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _object = value; 
            }
        }
    }
}
