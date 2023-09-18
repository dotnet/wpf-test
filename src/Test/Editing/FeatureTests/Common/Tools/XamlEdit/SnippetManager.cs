// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace XamlPadEdit
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Xml.Serialization;
    using System.Xml;
    using System.Xml.XPath;
    using System.Resources;
    using System.Globalization;

    #endregion Namespaces

    /// <summary>
    /// Use this class to encapsulate the title and text used for
    /// inserting bits of XAML in XamlPadEdit.
    /// </summary>
    public class Snippet
    {
        #region Constructors

        /// <summary>Initializes a new XamlPadEdit.Snippet instance with an empty title and body.</summary>
        public Snippet()
        {
            _body = _title = "";
        }

        /// <summary>Initializes a new XamlPadEdit.Snippet instance with the specified title and body.</summary>
        /// <param name='title'>Title for the snippet.</param>
        /// <param name='body'>Insertion text for the snippet.</param>
        public Snippet(string title, string body)
        {
            this._title = title; 
            this._body = body;
        }

        #endregion Constructors

        #region Public properties

        /// <summary>Insertion text for the snippet.</summary>
        public string Body
        {
            get 
            { 
                return _body; 
            }

            set 
            { 
                _body = (value == null) ? "" : value; 
            }
        }

        /// <summary>User-friendly snippet title.</summary>
        public string Title
        {
            get 
            { 
                return _title; 
            }

            set 
            { 
                _title = (value == null) ? "" : value; 
            }
        }

        #endregion Public properties

        #region Public methods

        public override string ToString()
        {
            return _title;
        }

        #endregion Public methods

        #region Private fields

        /// <summary>Insertion text for the snippet.</summary>
        private string _body;

        /// <summary>User-friendly snippet title.</summary>
        private string _title;

        #endregion Private fields
    }

    /// <summary>Use this delegate to receive a notification when a SnippetAction is clicked.</summary>
    public delegate void SnippetActionExecuteCallback(TextBox textbox, SnippetManager manager);

    /// <summary>Use this class to encapsulate actions on snippets.</summary>
    /// <remarks>
    /// SnippetAction instances are merged with snippets to create
    /// a uniform collection for data binding to context menus.
    /// </remarks>
    public class SnippetAction
    {
        #region Constructors

        /// <summary>
        /// Initializes a new SnippetAction, providing a menu item
        /// caption and a callback to invoke when the user clicks it.
        /// </summary>
        public SnippetAction(string caption, SnippetActionExecuteCallback executeCallback)
        {
            if (caption == null)
            {
                throw new ArgumentNullException("caption");
            }
            if (executeCallback == null)
            {
                throw new ArgumentNullException("executeCallback");
            }

            this._caption = caption;
            this._executeCallback = executeCallback;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>Executes the action encapsulated by this object.</summary>
        public void Execute(TextBox textbox, SnippetManager manager)
        {
            _executeCallback(textbox, manager);
        }

        /// <summary>Provides a string representation of this object.</summary>
        public override string ToString()
        {
            return this._caption;
        }

        #endregion Public methods

        #region Private fields

        /// <summary>Menu item caption for this action.</summary>
        private string _caption;

        /// <summary>Callback method for this action.</summary>
        private SnippetActionExecuteCallback _executeCallback;

        #endregion Private fields
    }

    /// <summary>Use this class to hold a list of snippets.</summary>
    public class SnippetList : ObservableCollection<Snippet> { }

    /// <summary>Use this class to manage overall snippet support in XamlPadEdit.</summary>
    public class SnippetManager
    {
        #region Constructors

        /// <summary>Initializes a new XamlPadEdit.SnippetManager instance with no content.</summary>
        public SnippetManager()
        {
            this._snippets = new SnippetList();
            this._brushSnippets = new SnippetList();
            this._sampleSnippets = new SnippetList();
            this._styleSnippets = new SnippetList();
            this._contextMenuItems = new ObservableCollection<object>();
            this._contextMenuBrushItems = new ObservableCollection<object>();
            this._contextMenuStyleItems = new ObservableCollection<object>();
            this._contextMenuSampleItems = new ObservableCollection<object>();
        }

        #endregion Constructors

        #region Public properties

        /// <summary>List of snippets being managed.</summary>
        public SnippetList Snippets
        {
            get 
            { 
                return _snippets; 
            }
            set
            {
                _snippets = value;
            }
        }

        /// <summary>List of snippets being managed.</summary>
        public SnippetList StyleSnippets
        {
            get
            {
                return _styleSnippets;
            }

            set
            {
                _styleSnippets = value;
            }
        }

        /// <summary>List of snippets being managed.</summary>
        public SnippetList BrushSnippets
        {
            get
            {
                return _brushSnippets;
            }
            set
            {
                _brushSnippets = value;
            }
        }

        /// <summary>List of snippets being managed.</summary>
        public SnippetList SampleSnippets
        {
            get
            {
                return _sampleSnippets;
            }
            set
            {
                _sampleSnippets = value;
            }
        }


        /// <summary>Bindable list of items appropriate for a context menu.</summary>
        public ObservableCollection<object> ContextMenuItems
        {
            get 
            { 
                return _contextMenuItems; 
            }
        }

        /// <summary>Bindable list of items appropriate for a context menu.</summary>
        public ObservableCollection<object> ContextMenuStyleItems
        {
            get
            {
                return _contextMenuStyleItems;
            }
        }

        /// <summary>Bindable list of items appropriate for a context menu.</summary>
        public ObservableCollection<object> ContextMenuBrushItems
        {
            get
            {
                return _contextMenuBrushItems;
            }
        }

        /// <summary>Bindable list of items appropriate for a context menu.</summary>
        public ObservableCollection<object> ContextMenuSampleItems
        {
            get
            {
                return _contextMenuSampleItems;
            }
        }

        /// <summary>Actions supported on the manager.</summary>
        public SnippetAction[] SnippetActions
        {
            get
            {
                if (s_snippetActions == null)
                {
                    s_snippetActions = new SnippetAction[] {
                    new SnippetAction("Edit Snippets...", delegate(TextBox textbox, SnippetManager manager) {
                        manager.EditSnippets();
                    }),
                    new SnippetAction("Cut", delegate(TextBox textbox, SnippetManager manager) {
                        textbox.Cut();
                    }),
                    new SnippetAction("Copy", delegate(TextBox textbox, SnippetManager manager) {
                        textbox.Copy();
                    }),
                    new SnippetAction("Paste", delegate(TextBox textbox, SnippetManager manager) {
                        textbox.Paste();
                    }),
                    new SnippetAction("Select All", delegate(TextBox textbox, SnippetManager manager) {
                        textbox.SelectAll();
                    }),
                    new SnippetAction("Undo", delegate(TextBox textbox, SnippetManager manager) {
                        textbox.Undo();
                    }),
                    new SnippetAction("Redo", delegate(TextBox textbox, SnippetManager manager) {
                        textbox.Redo();
                    }),
                };
                }
                return s_snippetActions;
            }
        }

        #endregion Public properties

        #region Public methods

        /// <summary>
        /// Displays a modal dialog box for the user to edit the managed snippets.
        /// </summary>
        public void EditSnippets()
        {
            SnippetsWindow window = new SnippetsWindow(this);
            window.ShowDialog();
            RefreshContextMenuItems();
        }


        /// <summary>Loads snippets for the current application.</summary>
        public bool LoadSnippets()
        {
            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain())
                {
                    // Check to see whether there are any files that match this
                    // (exact) pattern. We want to avoid exceptions as much as possible,
                    // as this is on the startup path and makes debugging with
                    // first-chance exception enabled harder.
                    if (file.GetFileNames(FileName).Length != 1)
                    {
                        return false;
                    }

                    using (IsolatedStorageFileStream stream =
                        new IsolatedStorageFileStream(FileName, FileMode.Open, file))
                    {
                        XmlSerializer serializer;

                        serializer = new XmlSerializer(_snippets.GetType());
                        _snippets = (SnippetList)serializer.Deserialize(stream);
                        RefreshContextMenuItems();
                        return true;
                    }
                }
            }
            catch (System.InvalidOperationException)
            {
                // InvalidOperationException will be thrown when the Snippet
                // or SnippetList classes change. We can't recover from this,
                // but we can continue with an empty collection.
                return false;
            }
            catch (System.Security.SecurityException)
            {
                // SecurityException will be thrown if we don't have any
                // access at all. We gracefully degrade to an empty collection
                // in this case.
                return false;
            }
        }

        /// <summary>Saves snippets for the current application.</summary>
        public void SaveSnippets()
        {
            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain())
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FileName, FileMode.Create, file))
                {
                    XmlSerializer serializer = new XmlSerializer(_snippets.GetType());
                    serializer.Serialize(stream, _snippets);
                }
            }
            catch (System.Security.SecurityException)
            {
                // SecurityException will be thrown if we don't have access
                // to the isolated storage. We ignore this exception and
                // continue.
            }
        }

        /// <summary>Sets the snippet collection to a well-known, hard-coded list.</summary>
        public void SetDefaultSnippets()
        {
            Snippet[] brushDefaults = null;
            Snippet[] styleDefaults = null;
            Snippet[] sampleDefaults = null;


            string str = Properties.Resources.XamlSnippets;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(str);
            XmlNodeList items = doc.SelectNodes("Snippets/Category");
            for (int i = 0; i < items.Count; i++)
            {

                XmlNode node = items[i];
                int count = 0;
                if (i == 0)
                {
                    styleDefaults = new Snippet[node.ChildNodes.Count];
                    foreach (XmlNode node1 in node)
                    {
                        styleDefaults[count++] = new Snippet(GetName(node1.OuterXml), (node1.InnerText));
                    }
                }
                else
                if (i == 1)
                {
                    brushDefaults = new Snippet[node.ChildNodes.Count];
                    foreach (XmlNode node1 in node)
                    {
                        brushDefaults[count++] = new Snippet(GetName(node1.OuterXml), (node1.InnerText));
                    }
                }
                else if(i==2)
                {

                    sampleDefaults = new Snippet[node.ChildNodes.Count];
                    foreach (XmlNode node1 in node)
                    {
                        sampleDefaults[count++] = new Snippet(GetName(node1.OuterXml), (node1.InnerText));
                    }
                }
            }

            Snippet[] defaults = new Snippet[] 
            {
                new Snippet("Comment Selection", "<!-- | -->"),
                new Snippet("Standalone StackPanel",
                    "<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                    "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>\r\n</StackPanel>"),
                new Snippet("Multiline TextBox",
                    "<TextBox AcceptsReturn='True' VerticalScrollBarVisibility='Visible'></TextBox>"),
                new Snippet("RichTextBox",
                    "<RichTextBox xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'\r\n" +
                    " xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>\r\n" +
                    " <FlowDocument>\r\n" +
                    "  <Paragraph>This is some text</Paragraph>\r\n" +
                    " </FlowDocument>\r\n" +
                    "</RichTextBox>"),
                new Snippet("Table",
                    "    <Table>\r\n"+
                    "	<Table.Resources>\r\n"+
                    "		<Style TargetType='{x:Type TableCell}'>\r\n"+
                    " 			<Setter Property='BorderThickness' Value='1' /><Setter Property='BorderBrush' Value='Red' />\r\n"+
                    " 		</Style>\r\n"+
                    "	</Table.Resources>\r\n"+
                    "	<TableRowGroup>\r\n"+
                    "	    <TableRow>\r\n"+
                    "		    <TableCell ><Paragraph>text1</Paragraph></TableCell>\r\n"+
                    "		    <TableCell><Paragraph>text2</Paragraph></TableCell>\r\n"+
                    "	    </TableRow>\r\n"+
                    "	    <TableRow>\r\n"+
                    "		    <TableCell><Paragraph>text3</Paragraph></TableCell>\r\n"+
                    "		    <TableCell><Paragraph>text4</Paragraph></TableCell>\r\n"+
                    "	    </TableRow>\r\n"+
                    "	</TableRowGroup>\r\n"+
                    "    </Table>"),
                new Snippet("Standalone FlowDocumentPageViewer",
                    "<FlowDocumentPageViewer xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'\r\n" +
                    " xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>\r\n" +
                    " <FlowDocument>\r\n" +
                    "  <Paragraph>This is some text: a&#x0302;e&#x0307;.</Paragraph>\r\n" +
                    " </FlowDocument>\r\n" +
                    "</FlowDocumentPageViewer>"),
            };

            _snippets.Clear();
            _styleSnippets.Clear();
            _brushSnippets.Clear();
            _sampleSnippets.Clear();

            foreach (Snippet s in defaults)
            {
                _snippets.Add(s);
            }
            foreach (Snippet s in styleDefaults)
            {
                _styleSnippets.Add(s);
            }
            foreach (Snippet s in brushDefaults)
            {
               _brushSnippets.Add(s);
            }
            foreach (Snippet s in sampleDefaults)
            {
                _sampleSnippets.Add(s);
            }

            RefreshContextMenuItems();
        }

        private string GetName(string str)
        {
            int begin = str.IndexOf("Name", 0);
            int end = str.IndexOf(">", begin + 1);
            str = str.Substring(begin + 4, end - begin - 4);
            str = str.Replace(">", "");
            str = str.Replace("<", "");
            str = str.Replace("=", "");
            str = str.Replace("\"", "");
            str = str.Trim();
            return str;
        }

        #endregion Public methods

        #region Private methods

        private void RefreshContextMenuItems()
        {
            _contextMenuItems.Clear();
            _contextMenuStyleItems.Clear();
            _contextMenuBrushItems.Clear();
            _contextMenuSampleItems.Clear();

            foreach (object o in Snippets)
            {
                _contextMenuItems.Add(o);
            }
            foreach (object o in SnippetActions)
            {
                _contextMenuItems.Add(o);
            }
            foreach (object o in StyleSnippets)
            {
                _contextMenuStyleItems.Add(o);
            }
            foreach (object o in BrushSnippets)
            {
                _contextMenuBrushItems.Add(o);
            }
            foreach (object o in SampleSnippets)
            {
                _contextMenuSampleItems.Add(o);
            }
        }

        #endregion Private methods

        #region Private fields

        private SnippetList _snippets;
        private SnippetList _brushSnippets,_styleSnippets,_sampleSnippets;
        private ObservableCollection<object> _contextMenuItems;
        private ObservableCollection<object> _contextMenuBrushItems;
        private ObservableCollection<object> _contextMenuSampleItems;
        private ObservableCollection<object> _contextMenuStyleItems;
        private static SnippetAction[] s_snippetActions;
        private const string FileName = "snippets.xml";

        #endregion Private fields
    }
}
