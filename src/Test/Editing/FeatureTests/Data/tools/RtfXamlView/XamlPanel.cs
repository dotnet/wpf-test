// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Adds the RichTextBox (Xaml) Control *Window* for xaml rendering.  
// Controls xaml side of the RtfXamlViewer.

// Added logging; Got rid of IFormattedWindow; all of
// the xaml panel functionality is moved here from
// main app as well.

using System;

// avalon
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RtfXamlView
{
    class XamlPanel : TextViewPanel
    {
        #region Constructors and its related methods
        public XamlPanel()
        {
            Initialize();
        }

        public XamlPanel(LogPanel logPanel)
        {
            _logPanel = logPanel;
            Initialize();
        }

        private void Initialize()
        {
            // Create RichTextBox window
            _xamlWindow = new XamlWindow();

            // Add Xaml window TextChanged event
            _xamlWindow.XamlTextBox.TextChanged += new TextChangedEventHandler(OnXamlTextChanged);
            // Add Plain window TextChanged event
            _textBox.TextChanged += new TextChangedEventHandler(OnPlainTextChanged);
            // Add Sync button Click event
            SyncButtonPanel.SyncButton.Click += new RoutedEventHandler(OnSyncButtonClick);
        }
        #endregion

        public bool UseCopyPaste(bool fNewValue)
        {
            bool fOldValue = _xamlWindow.CopyPaste;
            _xamlWindow.CopyPaste = fNewValue;
            return fOldValue;
        }

        public Window GetWindow()
        {
            return _xamlWindow;
        }

        public System.Windows.Controls.RichTextBox GetRichTextBox()
        {
            return _xamlWindow.XamlTextBox;
        }

        public void Reset()
        {
            _textBox.Text = "";
            _xamlWindow.XamlTextBox.Document = new FlowDocument(new Paragraph(new Run()));
            SyncButtonPanel.SyncButton.IsEnabled = false;
        }

        public override ConverterError SetPlainText(string textToSet, bool bSync)
        {
            ConverterError err = new ConverterError();
            if (_xamlWindow.CopyPaste == true)
                _textBox.Text = textToSet;
            else
                _textBox.Text = "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" + textToSet + "</FlowDocument>";

            if (bSync)
            {
                err = Sync();
            }
            return err;
        }

        //This works via cliboard. ContentRtf is a property that
        //will build an rtf data object and paste it into the control 
        //as xaml, thus forcing the convertion.        
        public ConverterError SetRTF(string textToSet, bool bSync)
        {
            ConverterError err = new ConverterError();
            err.errortype = ConverterErrorType.ErrorNone;

            _xamlWindow.ContentRTF = textToSet;

            if (bSync)
            {
                err = Sync();
            }
            return err;
        }

        //Forces teh control to do a copy and thus convert the XAML 
        //it contains into rtf, returning it to the string. 
        //Clears the clipboard and leaves the copy on the clibaord.
        //when using this function, wrap the call in a try catch or
        //you will miss conversion exceptions
       public string GetXamlAsRTF()
       {
          return _xamlWindow.ContentRTF;
       }
        
        //added this for a bit more clarity. This doesn't sync,
       // it simply does the same things that SetPlainText would do,
       //sans sync and possibly logging.
       public ConverterError SetXaml(string szXaml)
       {
          string szSet;
          ConverterError err = new ConverterError();
          err.errortype = ConverterErrorType.ErrorNone;

           try
          {
             szSet = szXaml;
             _xamlWindow.Content = szSet;
          }
          catch (Exception x)
          {
                err.errortype = ConverterErrorType.ErrorException;
                err.ExceptionText = x.Message;
          }
          return err;
       }

       //this should be wrapped in a try catch block to catch exceptions
       public string GetXaml()
       {
          return _xamlWindow.ContentAsText;
       }

        public override ConverterError Sync()
        {
            ConverterError err = new ConverterError();
            if (null != _logPanel)
            {
                _logPanel.LogInfo("XAML SYNC: Syncing...");
            }

            if (_xamlWindow == _lastUpdatedWindow)
            {
                try
                {
                    if (_xamlWindow.CopyPaste == true)
                        _textBox.Text = _xamlWindow.ContentAsText;
                    else
                        _textBox.Text = "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" + ExtractRichContent(_xamlWindow.ContentAsText) + "</FlowDocument>";
                    
                }
                catch (Exception x)
                {
                    if (null != _logPanel)
                    {
                        err.errortype = ConverterErrorType.ErrorException;
                        err.ExceptionText = "XAML SYNC - " + x.Message;
                        _logPanel.LogError(err.ExceptionText);
                    }
                    return err;
                }
            }
            else if (_textBox == _lastUpdatedWindow)
            {
                try
                {
                    _xamlWindow.Content = _textBox.Text;
                }
                catch (Exception x)
                {
                    if (null != _logPanel)
                    {
                        err.errortype = ConverterErrorType.ErrorException;
                        err.ExceptionText = "XAML SYNC - " + x.Message;
                        _logPanel.LogError(err.ExceptionText);
                    }
                    return err;
                }
            }
            else
            {
                return err;
            }

            // Now that we are synced, lets disable the sync button
            SyncButtonPanel.SyncButton.IsEnabled = false;

            if (null != _logPanel)
            {
                _logPanel.LogInfo("XAML SYNC SUCCEEDED");
            }
            return err;
        }


        public string ExtractRichContent(string richContent)
        {
           int contentStart;
            int tagStart = richContent.IndexOf("<FlowDocument");
            if (tagStart != -1)
            {
               int nextTagStart = richContent.IndexOf("<FlowDocument", tagStart + 1);
               while (nextTagStart != -1)
               {
                  tagStart = nextTagStart;
                  nextTagStart = richContent.IndexOf("<FlowDocument", nextTagStart + 1);
               }
               contentStart = richContent.IndexOf(">", tagStart) + 1;
            }
            else
            {
                return richContent;
            }

            string strEnd = "</FlowDocument>";
            int contentEnd = richContent.IndexOf(strEnd, tagStart);
            if (contentEnd == -1)
            {
                return "";
            }
            int tagEnd = richContent.IndexOf(strEnd) + strEnd.Length;
            string content = richContent.Substring(contentStart, contentEnd - contentStart);
            return content;
        }

        #region Events
        void OnXamlTextChanged(object sender, TextChangedEventArgs e)
        {
            _lastUpdatedWindow = _xamlWindow;

            SyncButtonPanel.SyncButton.IsEnabled = true;
        }

        void OnPlainTextChanged(object sender, TextChangedEventArgs e)
        {
            _lastUpdatedWindow = _textBox;

            SyncButtonPanel.SyncButton.IsEnabled = true;
        }

        void OnSyncButtonClick(object sender, RoutedEventArgs e)
        {
            Sync();
        }
        #endregion

        private XamlWindow _xamlWindow;
        private object _lastUpdatedWindow;

        private LogPanel _logPanel;
    }
}
