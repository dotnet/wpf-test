// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a tool for editing Ad hoc and Lab analyzing for RichEditing

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 18 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Text/BVT/Interactive/EditingExaminer.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Windows;
    using System.IO;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Reflection;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Xml;
    using Test.Uis.TestTypes;
    using System.Windows.Controls.Primitives;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>Provides a tool for Edting Ad hoc and Lab analyzing</summary>
    public class EditingExaminer: CustomTestCase
    {    
        #region Public methods.

        /// <summary>Start EditingExaminer</summary>
        public override void RunTestCase()
        {
            try
            {
                _table = new Hashtable();
                SetUpLayout();
                RefreshTable();
                
                _mainRichTextBox.TextChanged += new TextChangedEventHandler(MainRichTextBox_OnTextChanged);
                _mainRichTextBox.Selection.Changed += new EventHandler(Selection_Changed);
  
                _editTab.SelectionChanged += new SelectionChangedEventHandler(tabControl_SelectionChanged);
                _commandLineInputBox.SetUp(_immediateWindow, _table, _errorTextBox);
               
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }
      
        #endregion public methods

        #region Event handlers

        void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpDateTabInfo();
        }
 
        void MainRichTextBox_OnTextChanged(object o, TextChangedEventArgs args)
        {
            //check if document is the same.
            if (!(_mainRichTextBox.Document == _table["Document"]))
            {
                RefreshTable();
            }
            //
            UpDateTabInfo();
        }

        void Selection_Changed(object sender, EventArgs e)
        {
            //On selection changed, we will updated the Xaml over the selection.
            _selectionXamlTextBox.Text = IndentXaml(XamlUtils.TextRange_GetXml(_mainRichTextBox.Selection));
        }
        
        #endregion Event handlers.

        #region private methods.
        
        void RefreshTable()
        {
            if (_table == null)
            {
                _table = new Hashtable();
            }
            _table.Clear();
            _table.Add("RichTextBox", _mainRichTextBox);
            _table.Add("Selection", _mainRichTextBox.Selection);
            _table.Add("Document", _mainRichTextBox.Document);
            _table.Add("null", null);
            _table.Add("True", true);
            _table.Add("False", false);
            _table.Add("Panel", _mainEditPanel);
        }

        void UpDateTabInfo()
        {
            try
            {
                TabItem selectedItem = _tabControl.SelectedItem as TabItem;
                switch (selectedItem.Header.ToString())
                {
                    case "CoreXaml":
                        TextRange range = new TextRange(_coreXamlTextBox.Document.ContentStart, _coreXamlTextBox.Document.ContentEnd); 
                        if (_editTab.SelectedItem == _editTab.Items[1])
                        {
                           XamlUtils.TextRange_SetXml(range, ColoringXaml(IndentXaml(XamlWriter.Save(((TabItem)_editTab.SelectedItem).Content))));
                        }
                        else 
                        {

                            XamlUtils.TextRange_SetXml(range,  ColoringXaml(IndentXaml(XamlWriter.Save(_mainRichTextBox.Document))));
                        }
                        break;
                    case "Selection":
                        _selectionXamlTextBox.Text = IndentXaml(XamlUtils.TextRange_GetXml(_mainRichTextBox.Selection)).Replace("xml:space=\"preserve\" ",string.Empty);
                        break;
                    case "EditingXaml":
                       _textSerializedXamlTextBox.Text = (IndentXaml(XamlUtils.TextRange_GetXml(new TextRange(_mainRichTextBox.Document.ContentEnd, _mainRichTextBox.Document.ContentStart))).Replace("xml:space=\"preserve\" ",string.Empty));
                        break;
                    case "TreeView":
                       DocumentTreeView.SetupTreeView(_textTreeView, _mainRichTextBox.Document);
                        break;
                    case "Clipboard":
                        _clipboardXaml.Text = IndentXaml(GetClipboardXaml()).Replace("xml:space=\"preserve\" ", string.Empty);
                        break;
                    default:
                        OnError(new Exception("Can't find specified TabItem!"));
                        break; 
                }
                OnError(null);
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        void SetUpLayout()
        {
            try
            {
                //set window properties
                MainWindow.Title = "EditingExaminer";
                MainWindow.ResizeMode = ResizeMode.CanMinimize;
                MainWindow.MinHeight = 600;
                MainWindow.MinWidth = 700;

                MainWindow.ResizeMode = ResizeMode.CanResizeWithGrip;
                MainWindow.SizeChanged += new SizeChangedEventHandler(window_SizeChanged);

                //Create Top StackPanel
                _topLevelPanel = new StackPanel();
                _topLevelPanel.ClipToBounds = true;

                //Create panel2
                _panel1 = new StackPanel();
                _panel1.Orientation = Orientation.Horizontal;
                _topLevelPanel.Children.Add(_panel1);
                _editTab = new TabControl();
                _panel1.Children.Add(_editTab);

                TabItem tItem;
                tItem = new TabItem();
                tItem.Header = "RichTextBox";

                //Create RichTetxtBox
                _mainRichTextBox = new RichTextBox();
                _mainRichTextBox.AcceptsTab = true;
                _mainRichTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _mainRichTextBox.ClipToBounds = true;
                tItem.Content = _mainRichTextBox;
                _editTab.Items.Add(tItem);

                tItem = new TabItem();
                tItem.Header = "Panel";
                _mainEditPanel = new StackPanel();
                tItem.Content = _mainEditPanel;
                _editTab.Items.Add(tItem);              

                //Create Panel for xaml TextBox
                _panel2 = new StackPanel();
                _topLevelPanel.Children.Add(_panel2);

                //create TabControl
                _tabControl = new TabControl();
                _tabControl.HorizontalContentAlignment = HorizontalAlignment.Left;
                _tabControl.SelectionChanged += new SelectionChangedEventHandler(tabControl_SelectionChanged);
                _panel2.Children.Add(_tabControl);

                _panel3 = new StackPanel();
                _panel2.Children.Add(_panel3);
                _panel2.Orientation = Orientation.Horizontal;
                _label = new Label();
                _label.HorizontalContentAlignment = HorizontalAlignment.Center;
                _label.Content = "Immediate Window";
                _panel3.Children.Add(_label);
                _immediateWindow = new TextBox();
                _immediateWindow.AcceptsReturn = true;
                _immediateWindow.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _immediateWindow.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                _immediateWindow.IsReadOnly = true;
                _commandLineInputBox = new InputBox();
                _commandLineInputBox.IsEditable = true;
                _commandLineInputBox.IsReadOnly = false;
                _panel3.Children.Add(_immediateWindow);
                _panel3.Children.Add(_commandLineInputBox);

                //Create TabItem for TextContainer
                TabItem item = new TabItem();
                item.Header = "TreeView";
                _textTreeView = new TreeView();
                item.Content = _textTreeView;
                item.Height = _panel2.Height;
                _tabControl.Items.Add(item);

                //Create TabItem for selection
                item = new TabItem();
                item.Header = "Selection";
                _selectionXamlTextBox = new TextBox();
                _selectionXamlTextBox.AcceptsReturn = true;
                _selectionXamlTextBox.AcceptsTab = true;
                _selectionXamlTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _selectionXamlTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                item.Content = _selectionXamlTextBox;
                _tabControl.Items.Add(item);

                //Create TabItem for Whole document
                item = new TabItem();
                item.Header = "EditingXaml";
                _textSerializedXamlTextBox = new TextBox();
                _textSerializedXamlTextBox.AcceptsReturn = true;
                _textSerializedXamlTextBox.AcceptsTab = true;
                _textSerializedXamlTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _textSerializedXamlTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                item.Content = _textSerializedXamlTextBox;
                _tabControl.Items.Add(item);

                //Create TabItem for CoreParser
                item = new TabItem();
                item.Header = "CoreXaml";
                _coreXamlTextBox = new RichTextBox();
                _coreXamlTextBox.AcceptsReturn = true;
                _coreXamlTextBox.AcceptsTab = true;
                _coreXamlTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _coreXamlTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                item.Content = _coreXamlTextBox;
                _tabControl.Items.Add(item);

                //Create TabItem for Clipboard Xaml
                item = new TabItem();
                item.Header = "Clipboard";
                _clipboardXaml = new TextBox();
                _clipboardXaml.IsReadOnly = true;
                _clipboardXaml.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _clipboardXaml.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                item.Content = _clipboardXaml;
                _tabControl.Items.Add(item);

                //error panel
                _errorTextBox = new TextBox();
                _errorTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _errorTextBox.IsReadOnly = true;
                _errorTextBox.Height = 60;
                _errorTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                _errorTextBox.BorderThickness = new Thickness(2);
                _errorTextBox.Text = "error\r\nerror";
                _topLevelPanel.Children.Add(_errorTextBox);

                //Set ContextMenu for each TextBox
                _textSerializedXamlTextBox.ContextMenu = CreateContextMenu("SetXamlWithEditingParser", "OpenInNewWindow");
                _selectionXamlTextBox.ContextMenu = CreateContextMenu("SetSelectionWithXaml", "OpenInNewWindow");
                _coreXamlTextBox.ContextMenu = CreateContextMenu("SetXamlWithCoreParser", "OpenInNewWindow");
                _clipboardXaml.ContextMenu = CreateContextMenu("GetClipboardXaml", "");

                MainWindow.Content = _topLevelPanel;

                MainWindow.Width = 900;
                MainWindow.Height = 750;
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        ContextMenu CreateContextMenu(string header, string header1)
        {
            ContextMenu menu;
            MenuItem item;
            menu = new ContextMenu();
            
            //add cut command.
            item = new MenuItem();
            item.Header = "Cut";
            item.Command = System.Windows.Input.ApplicationCommands.Cut;
            menu.Items.Add(item);

            //add copy command.
            item = new MenuItem();
            item.Header = "Copy";
            item.Command = System.Windows.Input.ApplicationCommands.Copy;
            menu.Items.Add(item);

            //add Paste command.
            item = new MenuItem();
            item.Header = "Paste";
            item.Command = System.Windows.Input.ApplicationCommands.Paste;
            menu.Items.Add(item);

            //add Serialization commands.
            item = new MenuItem();
            item.Header = header;

            item.Click += new RoutedEventHandler(SetParsedXaml);
            menu.Items.Add(item);

            //add Serialization commands.
            if (header1 != "")
            {
                item = new MenuItem();
                item.Header = header1;
                item.Click += new RoutedEventHandler(OpenWindow);
                menu.Items.Add(item);
            }


            return menu;
        }

        void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                _topLevelPanel.Width = MainWindow.ActualWidth - 10;
                _topLevelPanel.Height = MainWindow.ActualHeight - 34;

                _editTab.Width = _topLevelPanel.Width;
                _editTab.Height = _topLevelPanel.Height / 2;
                _mainRichTextBox.Width = _editTab.Width -16 ;
                _mainRichTextBox.Height = _editTab.Height;
                _mainEditPanel.Width = _editTab.Width - 16 ;
                _mainEditPanel.Height = _editTab.Height;                

                _errorTextBox.Height = 60;
                _panel2.Width = _topLevelPanel.Width;
                _panel2.Height = _topLevelPanel.Height / 2 - _errorTextBox.ActualHeight;
                _tabControl.Width = _panel2.Width * 0.50;
                _tabControl.Height = _panel2.Height;
                _textTreeView.Height = _panel2.Height - _commandLineInputBox.ActualHeight - 7;

                _panel3.Width = _panel2.Width * 0.50 ;
                _panel3.Height = _panel2.Height;
                _immediateWindow.Height = _panel3.Height - _commandLineInputBox.ActualHeight - _label.ActualHeight;
                _commandLineInputBox.Width = _panel3.Width;
                _immediateWindow.Width = _panel3.Width ;
            }
            catch (Exception exception)
            {
                OnError(exception);
            }
        }

        void OpenWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                TabItem selectedItem = _tabControl.SelectedItem as TabItem;
                _xamlWindow = new Window();
                _xamlWindow.Title = selectedItem.Header.ToString();
                _xamlWindow.Height = 600;
                _xamlWindow.SizeChanged += new SizeChangedEventHandler(w_SizeChanged);
                switch(selectedItem.Header.ToString())
                {
                    case "Selection":

                        _xamlWindow.Tag = _selectionXamlTextBox;
                        break;

                    case "EditingXaml":
                        _xamlWindow.Tag = _textSerializedXamlTextBox;
                        break;

                    case "CoreXaml":
                        _xamlWindow.Tag = _coreXamlTextBox;
                        break;

                    default:
                        break;
                }
                
                StackPanel sp = new StackPanel();
                TextBox tb = new TextBox();
                tb.Text = ((TextBox)(selectedItem.Content)).Text;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                tb.BorderThickness = new Thickness(0);
                tb.Height = _xamlWindow.Height - 50;

                tb.AcceptsReturn = true;
                sp.Children.Add(tb);
                _xamlWindow.Content = sp;
                _xamlWindow.Closing += new System.ComponentModel.CancelEventHandler(w_Closing);
                _xamlWindow.ShowDialog();
                tb.Focus();
            }
            catch (Exception exception)
            {
                OnError(exception);
            }
        }

        void w_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window w = sender as Window;
            ((TextBox)(((StackPanel)(w.Content)).Children[0])).Height = w.Height - 50;
        }

        void w_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Window w = sender as Window;
            TextBox tb = w.Tag as TextBox;
            TextBox source = ((TextBox)(((StackPanel)(w.Content)).Children[0]));
            ((TabItem)(tb.Parent)).IsSelected = true;
            tb.Text = source.Text;
        }

        void SetParsedXaml(object sender, RoutedEventArgs e)
        {
            object passedObject;
            try
            {
                TabItem selectedItem = _tabControl.SelectedItem as TabItem;
                if (selectedItem != null)
                {
                    switch (selectedItem.Header.ToString())
                    {
                        case "CoreXaml":
                            TextRange range = new TextRange(_coreXamlTextBox.Document.ContentStart, _coreXamlTextBox.Document.ContentEnd);
                            passedObject = ParseXaml(range.Text);
                            if (passedObject is FlowDocument)
                            {
                                _mainRichTextBox.Document = (FlowDocument)ParseXaml(range.Text);
                                _mainRichTextBox.Selection.Changed += new EventHandler(Selection_Changed);
                            }
                            else if (passedObject is FrameworkElement)
                            {
                                _mainEditPanel = (FrameworkElement)passedObject;
                                _editTab.Items.Remove(_tabControl.Items[1]);
                                ((TabItem)_editTab.Items[1]).Content = _mainEditPanel;
                                RefreshTable();                              
                            }
                            break;
                        case "Selection":
                            XamlUtils.TextRange_SetXml(_mainRichTextBox.Selection, _selectionXamlTextBox.Text);
                            break;
                        case "EditingXaml":
                            XamlUtils.TextRange_SetXml(new TextRange(_mainRichTextBox.Document.ContentEnd, _mainRichTextBox.Document.ContentStart), _textSerializedXamlTextBox.Text);
                            break;
                        case "Clipboard":
                            UpDateTabInfo();
                            break;
                    }
                }
                OnError(null);
            }
            catch (Exception exception)
            {
                OnError(exception);
            }
        }

        object ParseXaml(string str)
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

        string IndentXaml(string xaml)
        {
            //open the string as an XML node
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xaml);
            XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc);

            //write it back onto a stringWriter
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter);
            xmlWriter.Formatting = System.Xml.Formatting.Indented;
            xmlWriter.WriteNode(nodeReader, false);

            string result = stringWriter.ToString();
            xmlWriter.Close();

            return result;
        }

        string GetClipboardXaml()
        {
            MemoryStream stream;
            string result;
           
            stream = Clipboard.GetData(DataFormats.XamlPackage) as MemoryStream;
            if (stream != null)
            {
                //Need find a way to retriew the xaml from xamlpackage.
                result = "<Xalm>Under implmenting for retrivewing Xaml from Xamlpackage</Xaml>";
            }
            else
            {
                object o = Clipboard.GetData(DataFormats.Xaml);
                result = (o == null) ? "<Xaml>No Xaml format on Clipboard</Xaml>" : (string)o;
            }

            return result; 

        }

        string ColoringXaml(string xaml)
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

        string ProcessEachTag(string str)
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

        void OnError(Exception e)
        {
            if (e != null)
            {
                //_commandLineInputBox.Text = "";
                _errorTextBox.Text = e.Message + "\r\n" + e.StackTrace;
            }
            else
            {
                _errorTextBox.Text = string.Empty;
            }
        }

        #endregion private methods.

        #region Private fields.

        private RichTextBox _coreXamlTextBox;
        private TextBox _textSerializedXamlTextBox;
        private TextBox _selectionXamlTextBox;
        private TextBox _clipboardXaml;
        private TextBox _errorTextBox;
        private TextBox _immediateWindow;
        private InputBox _commandLineInputBox;
        private RichTextBox _mainRichTextBox;
        private TreeView _textTreeView;
        private TabControl _tabControl;
        private Hashtable _table;
        private StackPanel _topLevelPanel = null;
        private StackPanel _panel1 = null;
        private StackPanel _panel2 = null;
        private StackPanel _panel3 = null;
        private Label _label = null;
        private Window _xamlWindow = null;
        private FrameworkElement _mainEditPanel;
        private TabControl _editTab;

       
        #endregion Private fields.
    }

    /// <summary>
    /// Helper class for setting up the tree view
    /// </summary>
    public class DocumentTreeView
    {
        /// <summary>
        /// Method for Seting up the TreeView
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static TreeView SetupTreeView(TreeView treeView, FlowDocument document)
        {
            TreeViewItem root;
            if (treeView == null)
            {
                treeView = new TreeView();
                treeView.Visibility = Visibility.Visible;
            }

            treeView.Items.Clear();
            root = new TreeViewItem();
            root.Header = CreateHeader("Document");
            root.IsExpanded = true;
            treeView.Items.Add(root);
            AddCollection(root, document.Blocks as IList);

            return treeView;
        }

        static void AddCollection(TreeViewItem item, IList list)
        {

            for (int i = 0; i < list.Count; i++)
            {
                TreeViewItem titem = new TreeViewItem();
                item.Items.Add(titem);
                titem.IsExpanded = true;
                titem.Header = CreateHeader(list[i].GetType().Name);
                AddItem(titem, list[i] as TextElement);
            }
        }

        static object CreateHeader(string name)
        {
            Label result;
            result = new Label();
            result.Content = name;
            result.Foreground = Brushes.Blue;
            result.Height = 13;
            result.FontSize = 11;
            result.Margin = new Thickness(0, 0, 0, 0);
            result.BorderThickness = new Thickness(0, 0, 0, 0);
            result.Padding = new Thickness(0, 0, 0, 0);
            return result;
        }
        static void AddItem(TreeViewItem item, TextElement textElement)
        {
            TreeViewItem grandItem;
            string str; 

            if (textElement is InlineUIContainer)
            {
                grandItem = new TreeViewItem();
                if (((InlineUIContainer)textElement).Child != null)
                {
                    str = ((InlineUIContainer)textElement).Child.GetType().Name;
                }
                else
                {
                    str = "null";
                }
                grandItem.Header = CreateHeader(str);
                grandItem.IsExpanded = true;
                item.Items.Add(grandItem);
            }
            else if (textElement is BlockUIContainer)
            {
                grandItem = new TreeViewItem();
                if (((BlockUIContainer)textElement).Child != null)
                {
                    str = ((BlockUIContainer)textElement).Child.GetType().Name;
                }
                else
                {
                    str = "null";
                }
                grandItem.Header = CreateHeader(str);

                grandItem.IsExpanded = true;
                item.Items.Add(grandItem);
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
            //at last, the element should be a inline (Run) and we try to show its text.
            else if (textElement is Inline)
            {
                TextRange range = new TextRange(((Inline)textElement).ContentEnd, ((Inline)textElement).ContentStart);
                item.Header = CreateHeader(((Label)(item.Header)).Content + " - [" + range.Text + "]");
            }
        }
    }

    /// <summary>
    /// This class parse the command line commands.
    /// </summary>
    public class CommandLine
    {
        Hashtable _table;
        ObjectItem _result;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="table"></param>
        public CommandLine(string commandLine, Hashtable table)
        {
            _table = table;
            _result = new ObjectItem(null, null);
            DoParse(commandLine);
        }

        /// <summary>
        /// recursively parse the commandline.
        /// </summary>
        /// <param name="commandLine"></param>
        void DoParse(string commandLine)
        {
            string strLeft;
            string strRight;
            object left;
            string str = commandLine.Replace("  ", " ");
            str = str.Trim();
            //Asignment
            if (str.Contains("="))
            {

                string[] strs = str.Split(new char[] { '=' });
                if (strs.Length != 2 || strs[0].Length == 0 || str[1] == 0)
                {
                    _result.Error = "syntax error: Don't know how to do Assignment!";
                    return;
                }
                strLeft = strRight = string.Empty;
                strLeft = strs[0];
                strRight = strs[1];
                strLeft = strLeft.Trim();
                strRight = strRight.Trim();
                _result = new CommandLine(strRight, _table).Result;

                //Set a value to a variable in the Table.
                if (_table.Contains(strLeft))
                {
                    _table.Remove(strLeft);
                    _table.Add(strLeft, Result.Value);
                }
                //Assign a value to a property.
                else if (strLeft.Contains("."))
                {
                    left = new CommandLine(strLeft.Substring(0, strLeft.LastIndexOf(".")), _table).Result.Value;
                    if (left != null)
                    {
                        ReflectionUtils.SetProperty(left, strLeft.Substring(strLeft.LastIndexOf(".") + 1), Result.Value);
                    }
                    else
                    {
                        Result.Error = "Failed: can't set value to " + strLeft + "!";
                    }
                }
                //make a declaration
                else
                {
                    _table.Add(strLeft, Result.Value);
                }
            }
            //invoke a method, Creating a object.
            else if (str.EndsWith(")"))
            {

                if (str.StartsWith("new "))
                {
                    str = str.Substring(4).Trim();
                }
                left = null;
                strRight = strLeft = str.Substring(0, str.IndexOf("("));

                //if there is dot, we are invoke a method from a object
                if (strLeft.LastIndexOf(".") >= 0)
                {
                    strLeft = strLeft.Substring(0, strLeft.LastIndexOf("."));
                    left = new CommandLine(strLeft, _table).Result.Value;
                }

                //when left is null, we are invoke an constructor, otherwise we are invoke a method.
                if (left != null)
                {
                    //Get the command line start from the method name
                    //this will get the argument list.
                    str = str.Substring(strRight.LastIndexOf(".") + 1);
                }

                Result.Value = InvokMethod(str, left);
            }

            //Get - Retrive data
            else
            {
                if (_table.Contains(str))
                {
                    _result.Value = _table[str];
                    return;
                }
                left = null;
                strRight = str;

                if (str.Contains("."))
                {
                    strLeft = str.Substring(0, str.IndexOf("."));
                    left = _table[strLeft];

                    //If we found the object from the Table, we need the property name to retrieve the property value.
                    if (left != null && str.Length - (strLeft.Length + 1) > 0)
                    {
                        strRight = str.Substring(str.IndexOf(".") + 1, str.Length - (strLeft.Length + 1));
                    }
                }
                //strRight could be the following:
                //  1.  strRight=\"abc\"        - string in double quotes
                //  2.  strRight = 12345        - integer  
                //  3.  strRight = 123.456      - double
                //Thus, we should always call get value event if the left is null.
                _result = Get_Value(left, strRight);

                if (Result.Error != null && Result.Error.Length > 0)
                {
                    //if the expression is just for a static member of a class
                    _result = GetProperty(null, strRight);
                }
            }
        }

        /// <summary>
        /// Create an object. 
        ///     1. new a object: new abc(p1, p2, p3...)
        ///     2. Create a instance for a primative values. for example
        ///     3. Create instance for Enum.
        ///     4. create a instance for struct.
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="objectInstance"></param>
        /// <returns></returns>
        object InvokMethod(string commandLine, object objectInstance)
        {
            string name;
            string arg;
            string[] args;
            object[] objs;
            object returnValue;
            commandLine = commandLine.Trim();
            objs = null;
            returnValue = false;
            name = commandLine.Substring(0, commandLine.IndexOf("("));
            arg = commandLine.Substring(commandLine.IndexOf("(") + 1, commandLine.IndexOf(")") - (commandLine.IndexOf("(") + 1));
            arg = commandLine.Substring(commandLine.IndexOf("(") + 1);
            arg = arg.Substring(0, arg.Length - 1);
            if (arg.Length > 0)
            {
                args = arg.Trim().Split(new char[] { ',' });
                objs = new object[args.Length];

                for (int i = 0; i < args.Length; i++)
                {
                    objs[i] = new CommandLine(args[i], _table).Result.Value;
                }
            }
            else
            {
                objs = new object[0];
            }
            //If there is an instance, we should invoke a instance methods. 
            if (objectInstance != null)
            {
                try
                {
                    //try the instance methods first.
                    returnValue = ReflectionUtils.InvokeInstanceMethod(objectInstance, name, objs);
                }
                catch 
                {
                    //try the static methods.
                    //if there the invoking failed here, we got the Exception message in the Error box.
                    returnValue = ReflectionUtils.InvokeStaticMethod(objectInstance as Type, name, objs);
                }
            }
            //create a object. 
            else
            {
                returnValue = ReflectionUtils.CreateInstanceOfType(name, objs);
            }
            return returnValue;
        }

        /// <summary>
        /// This method helps recursivly retrive propeties.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        ObjectItem Get_Value(object o, string name)
        {
            ObjectItem result;
            string left;
            int index;
            string parseError = "Parser error: the command line can't be evaluated!";

            result = new ObjectItem(null, null);
            if (/*o == null ||*/ name == null || name == string.Empty)
            {
                return result;
            }

            index = name.IndexOf('.');
            index = (index > 0) ? index : name.Length;
            left = name.Substring(0, index);

            result = GetProperty(o, left);

            if (result.Error != null && result.Error.Length > 0)
            {
                result.Error = parseError;
            }

            if (left != name)
            {
                if (result.Value == null)
                {
                    result.Error = parseError;
                }
                else
                {
                    result = Get_Value(result.Value, name.Substring(index + 1));
                }
            }

            return result;
        }

        ObjectItem GetProperty(object objectInstance, string propertyName)
        {
            object returnValue = null;
            for (int i = 0; i <= 8; i++)
            {
                try
                {
                    switch (i)
                    {
                        case 0:
                            returnValue = ReflectionUtils.GetField(objectInstance, propertyName);
                            break;
                        case 1:

                            returnValue = ReflectionUtils.GetProperty(objectInstance, propertyName);
                            break;
                        case 2:
                            returnValue = ReflectionUtils.GetStaticField(objectInstance as Type, propertyName);
                            break;
                        case 3:
                            returnValue = ReflectionUtils.GetInterfaceProperty(objectInstance, propertyName, propertyName);
                            break;
                        case 4:
                            returnValue = Convert.ToInt32(propertyName);
                            break;
                        case 5:
                            returnValue = Convert.ToDouble(propertyName);
                            break;
                        case 6:
                            if (propertyName.StartsWith("\"") && propertyName.EndsWith("\""))
                            {
                                returnValue = propertyName.Substring(1, propertyName.Length - 2);
                            }
                            else
                            {
                                throw new Exception("error");
                            }
                            break;
                        case 7:

                            returnValue = ReflectionUtils.GetStaticProperty(objectInstance as Type, propertyName);
                            break;
                        case 8:
                            if (objectInstance == null)
                            {
                                returnValue = ReflectionUtils.FindType(propertyName);
                            }
                            else
                            {
                                throw new Exception("");
                            }
                            break; 
                    }
                    return new ObjectItem(returnValue, "");
                }
                catch 
                {
                    //Ignore the exception.
                }
                if (returnValue != null)
                {
                    break;
                }
            }
            return new ObjectItem(null, "Parser error: the command line can't be evaluated!");
        }

        /// <summary>
        /// retrieve the parsed object.
        /// </summary>
        public ObjectItem Result
        {
            get
            {
                return _result;
            }
        }
    }

    /// <summary>
    /// class for hold a parsed object
    /// </summary>
    public class ObjectItem
    {
        private object _value;
        private string _error;

        /// <summary>
        /// Constructor for creating a object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="error"></param>
        public ObjectItem(object value, string error)
        {
            _value = value;
            _error = error;
        }

        /// <summary>
        /// override the ToSring method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (Error == null || Error == string.Empty) ? (Value == null) ? "null" : Value.ToString() : Error;
        }

        /// <summary>
        /// Get the parsed object
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Get the error message.
        /// </summary>
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
            }
        }
    }


    /// <summary>
    /// Customize the RichTextBox for the Command line. 
    /// </summary>
    internal class InputBox : ComboBox
    {
        #region public methods
        public InputBox() : base()
        {
            _commandList = new ArrayList();
            this.PreviewKeyUp += new KeyEventHandler(InputBox_PreviewKeyUp);
        }
        public void SetUp(TextBox box1, Hashtable table, TextBox box2)
        {
            _table = table;
            _immediateWindow = box1;
            _errorTextBox = box2;
            FocusEvent(null, null);
            this.GotFocus += new RoutedEventHandler(FocusEvent);
            this.LostFocus += new RoutedEventHandler(FocusEvent);
            _inputTextBox = ReflectionUtils.GetProperty(this, "EditableTextBoxSite") as TextBox;

            _inputTextBox.TextChanged += new TextChangedEventHandler(TextChanged);
        }

        void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_inputTextBox.SelectionStart == 0 && _inputTextBox.SelectionLength == 0 && _inputTextBox.Text.Length > 0)
            {
                _inputTextBox.SelectionStart = _inputTextBox.Text.Length;
            }
        }

        #endregion public methods

        #region private methods

        void InputBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.Return:
                        string command;
                        CommandLine commandline; 

                        //parse the command line
                        command = this.Text.Replace("\r\n", "");
                        if (command.Length == 0)
                        {
                            return;
                        }
                        commandline = new CommandLine(command, _table);
                        command += "\r\n[" + commandline.Result.ToString() + "]\r\n";
                        _immediateWindow.Text += command;
                        _immediateWindow.ScrollToEnd();

                        //update history list.
                        //if there is an exception during parsing time. the history is not updated.
                        for (int i = 0; i < _commandList.Count; i++)
                        {
                            if (this.Text == (string)_commandList[i])
                            {
                                _commandList.RemoveAt(i);
                                i = _commandList.Count;
                            }
                        }
                        _commandList.Insert(0, this.Text.Replace("\r\n", ""));
                        
                        this.Text = "";
                        break;
                    case Key.Up:
                        if (_upCounter < _commandList.Count)
                        {
                            this.Text = _commandList[_upCounter] as string;
                            _upCounter++;
                        }
                        break;
                    case Key.Down:
                        if (_upCounter > 0)
                        {
                            _upCounter--;
                            this.Text = _commandList[_upCounter] as string;
                        }
                        break;
                    default:
                        _upCounter = 0;
                        break;
                }

                //check to see if the count of dot in the input box is changed. 
                if (e.Key == Key.Down || e.Key == Key.Up)
                {
                    //SetDefaultItems();
                }
                else
                {
                    CheckDots();
                }
                OnError(null);
            }
            catch (Exception ex)
            {
                OnError(ex);      
            }
        }

        void OnError(Exception e)
        {
            if (e != null)
            {
                this.Text = "";
                _errorTextBox.Text = e.Message + "\r\n" + e.StackTrace;
            }
            else
            {
                _errorTextBox.Text = string.Empty;
            }
        }

        void CheckDots()
        {
            int currentDotcounter;
            string str = this.Text;
            currentDotcounter = 0;

            //Count the currently dot 
            while (str.Contains("."))
            {
                currentDotcounter++;
                str = str.Substring(str.IndexOf(".") + 1);
            }

            //if Dot count changed, perfrom a new search
            //if it start with "System.ABc.aa", it is a namespace.
            if (currentDotcounter != _dotCounter)
            {
                _dotCounter = currentDotcounter;
                RefreshComboBoxItems();
            }
        }

        void FocusEvent(object sender, RoutedEventArgs e)
        {
            _dotCounter = 0;
            if (e != null && e.RoutedEvent.Name == "GotFocus")
            {
                this.Foreground = Brushes.Black;
                this.Text = "";
                
                RefreshComboBoxItems();
            }
            else
            {
                this.Foreground = Brushes.Gray;
                this.Text = "Type Command here!";
            }
        }

        void SetDefaultItems()
        {
            ComboBoxItem item;
            string str;
            IDictionaryEnumerator enumerator = _table.GetEnumerator();
            str = this.Text;
            this.Items.Clear();
            this.Text = str;
            _inputTextBox.SelectionStart = str.Length;
            while (enumerator.MoveNext())
            {
                item = new ComboBoxItem();
                item.Content = enumerator.Key;
                this.Items.Add(item);
            }
        }

        void RefreshComboBoxItems()
        {
            Type type;
            PropertyInfo[] pInfo;
            MethodInfo[] mInfos;
            FieldInfo[] fInfos;
            BindingFlags bindingAttr;
            ArrayList strList;
            string str1;
            string str; 
            
            bindingAttr = BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.FlattenHierarchy;
            
            if (_dotCounter == 0)
            {
                SetDefaultItems();
                return; 
            }
            str = this.Text.Substring(0, this.Text.LastIndexOf("."));
            str1 = str + ".";            
            try
            {
                if (str.Contains("="))
                {
                    str = str.Substring(str.LastIndexOf("=") + 1);
                }

                //the following code may not work for something like this: box.abc(null, def).Abc(ab, ab)
                if (str.Contains(","))
                {
                    str = str.Substring(str.LastIndexOf(",") + 1);
                }
                else if (str.Contains("("))
                {
                    str = str.Substring(str.LastIndexOf("(") + 1);
                }
                str = str.Trim();
                CommandLine commandline = new CommandLine(str, _table);

                if (commandline.Result.Value != null)
                {
                    type = commandline.Result.Value.GetType();
                }
                else
                {
                    type = ReflectionUtils.FindType(str);
                }

                if (type != null)
                {
                    strList = new ArrayList();
                    pInfo = type.GetProperties(bindingAttr);
                    str = this.Text;
                    this.Items.Clear();
                    this.Text = str;
                    _inputTextBox.SelectionStart = str.LastIndexOf(".") + 1;
                    _inputTextBox.SelectionLength = str.Length - str.LastIndexOf(".") + 1;
                    foreach (PropertyInfo info in pInfo)
                    {
                        if (!strList.Contains(info.Name))
                        {
                            strList.Add(info.Name);
                            AddComboBoxitem(str1 + info.Name);
                        }
                    }
                    mInfos = type.GetMethods(bindingAttr);
                    foreach (MethodInfo minfo in mInfos)
                    {

                        if (!strList.Contains(minfo.Name))
                        {
                            strList.Add(minfo.Name);
                            AddComboBoxitem(str1 + minfo.Name);
                        }
                    }
                    fInfos = type.GetFields(bindingAttr);
                    foreach (FieldInfo fInfo in fInfos)
                    {
                        if (!strList.Contains(fInfo.Name))
                        {
                            strList.Add(fInfo.Name);
                            AddComboBoxitem(str1 + fInfo.Name);
                        }
                    }
                }
            }
            catch 
            {
                //OnError(ex);
            }
        }

        void AddComboBoxitem(string str)
        {
            ComboBoxItem item = new ComboBoxItem();
            item.Content = str;
            this.Items.Add(item);
        }

        #endregion private methods

        #region private fields
        
        private ArrayList _commandList;
        private int _upCounter;
        Hashtable _table;
        TextBox _immediateWindow;
        TextBox _errorTextBox;
        TextBox _inputTextBox;
        int _dotCounter;

        #endregion private fileds.
    }
}
