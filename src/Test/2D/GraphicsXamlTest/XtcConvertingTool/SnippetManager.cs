// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Test.Graphics
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

    #endregion Namespaces

    /// <summary>
    /// Use this class to encapsulate the title and text used for
    /// inserting bits of XAML in XamlPad.
    /// </summary>
    public class Snippet
    {
        #region Constructors

        /// <summary>Initializes a new XamlPad.Snippet instance with an empty title and body.</summary>
        public Snippet()
        {
            _body = _title = "";
        }

        /// <summary>Initializes a new XamlPad.Snippet instance with the specified title and body.</summary>
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

    /// <summary>Use this class to manage overall snippet support in XamlPad.</summary>
    public class SnippetManager
    {
        #region Constructors

        /// <summary>Initializes a new XamlPad.SnippetManager instance with no content.</summary>
        public SnippetManager()
        {
            this._snippets = new SnippetList();
            this._contextMenuItems = new ObservableCollection<object>();
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

        /// <summary>Bindable list of items appropriate for a context menu.</summary>
        public ObservableCollection<object> ContextMenuItems
        {
            get 
            { 
                return _contextMenuItems; 
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
            Snippet[] defaults = new Snippet[] 
            {
                new Snippet("Comment Selection", "<!-- | -->"),
                new Snippet("Standalone StackPanel",
                    "<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                    "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>\r\n</StackPanel>"),
                new Snippet("Multiline TextBox",
                    "<TextBox AcceptsReturn='True' VerticalScrollBarVisibility='Visible'></TextBox>"),
                new Snippet("Standalone FlowDocumentPageViewer",
                    "<FlowDocumentPageViewer xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'\r\n" +
                    " xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>\r\n" +
                    " <FlowDocument>\r\n" +
                    "  <Paragraph>This is some text: a&#x0302;e&#x0307;.</Paragraph>\r\n" +
                    " </FlowDocument>\r\n" +
                    "</FlowDocumentPageViewer>"),
                new Snippet("Animated Background",
                    "<Foo.Background>\r\n" +
                    "  <SolidColorBrush><SolidColorBrush.Color>\r\n" +
                    "  <ColorAnimation From='White' To='Green' Duration='0:0:2' RepeatBehavior='Forever' AutoReverse='True' />\r\n" +
                    "  </SolidColorBrush.Color></SolidColorBrush>\r\n" +
                    "</Foo.Background>"),
            };

            _snippets.Clear();
            foreach (Snippet s in defaults)
            {
                _snippets.Add(s);
            }
            RefreshContextMenuItems();
        }

        #endregion Public methods

        #region Private methods

        private void RefreshContextMenuItems()
        {
            _contextMenuItems.Clear();

            foreach (object o in Snippets)
            {
                _contextMenuItems.Add(o);
            }
            foreach (object o in SnippetActions)
            {
                _contextMenuItems.Add(o);
            }
        }

        #endregion Private methods

        #region Private fields

        private SnippetList _snippets;
        private ObservableCollection<object> _contextMenuItems;
        private static SnippetAction[] s_snippetActions;
        private const string FileName = "snippets.xml";

        #endregion Private fields
    }
}

