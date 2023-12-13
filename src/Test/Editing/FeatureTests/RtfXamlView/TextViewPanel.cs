// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//              Creates Text Box with Sync button for displaying
//              plain rtf or xaml text.

// avalon
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RtfXamlView
{
   
    abstract class  TextViewPanel : Grid
    {
        public TextViewPanel()
        {
            #region Add 2 rows for the grid (Text Box and Sync Button)
            // add row the plaintext view
            RowDefinitions.Add(new RowDefinition());
            // add row for the Sync button
            RowDefinitions.Add(new RowDefinition());
            #endregion

            // Set the text box
            _textBox = new TextBox();
            _textBox.AcceptsReturn = true;
            _textBox.AllowDrop = true;
            _textBox.Margin = new Thickness(2);
            _textBox.FontFamily = new FontFamily("Lucida Console");            
            _textBox.FontSize = 10.0;
            _textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _textBox.Width = 400.0;
            _textBox.Height = 100.0;
            _textBox.TextWrapping = TextWrapping.Wrap;
            _textBox.IsUndoEnabled = false;

            Grid.SetRow(_textBox, 0);
            Children.Add(_textBox);

            // Set Sync button
            _syncPanel = new SyncButtonPanel();
            _syncPanel.SyncButton.IsEnabled = false;

            Grid.SetRow(_syncPanel, 1);
            Children.Add(_syncPanel);
        }

        #region Public Properties
        public SyncButtonPanel SyncButtonPanel
        {
            get
            {
                return _syncPanel;
            }
        }
        #endregion

        public virtual ConverterError SetPlainText(string textToSet, bool bSync)
        {
            ConverterError err = new ConverterError();
            _textBox.Text = textToSet;

            if (bSync)
            {
                err = Sync();
            }
            return err;
        }

        #region Virtual methods
        public string GetPlainText()
        {
            return _textBox.Text;
        }
        #endregion

        #region Abstract methods
        public abstract ConverterError Sync();
        #endregion

        #region Protected members
        protected TextBox _textBox;
        #endregion

        #region Private members
        private SyncButtonPanel _syncPanel;
        #endregion
    }
}