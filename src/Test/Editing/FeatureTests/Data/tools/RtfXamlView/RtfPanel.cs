// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Adds the RichEdit Control *Window* for rtf rendering.  
// Controls rtf side of the RtfXamlViewer.

// Added logging; Got rid of IFormattedWindow; all of
// the rtf panel functionality is moved here from
// main app as well.

using System;
using System.Windows.Forms;

// avalon
using System.Windows;
using System.Windows.Controls;

namespace RtfXamlView
{
   class RtfPanel : TextViewPanel
   {
      #region Constructors and its related methods
      public RtfPanel()
      {
         Initialize();
      }

      public RtfPanel(LogPanel logPanel)
      {
         _logPanel = logPanel;
         Initialize();
      }

      private void Initialize()
      {
         // Create RichEdit Window
         _richEditWindow = new RichEditWindow();

         // Add RichEdit window TextChanged event
         _richEditWindow.RtfTextBox.TextChanged += new EventHandler(OnRETextChanged);
         // Add Plain window TextChanged event
         _textBox.TextChanged += new TextChangedEventHandler(OnPlainTextChanged);
         // Add Sync button Click event
         SyncButtonPanel.SyncButton.Click += new RoutedEventHandler(OnSyncButtonClick);
      }
      #endregion

      public Form GetWindow()
      {
         return _richEditWindow;
      }

      public System.Windows.Forms.RichTextBox GetRichTextBox()
      {
         return _richEditWindow.RtfTextBox;
      }

      //to communicate to and from the richedit control clearly.
     
      public string GetRichEditContentsRtf()
      {
         return _richEditWindow.ContentAsText;
      }

      //to communicate to and from the richedit control clearly.
      public void SetRichEditContents(string szRtf)
      {
         _richEditWindow.Content = szRtf;
         _richEditWindow.Refresh();
      }

      public void Reset()
      {          
         _textBox.Text = "";
         //Have to close the window to allow tom to cleanup,
         //otherwise, you get runtime errors.
         //I think this is because clr.GC interferes with Interop's timing
         //of releaseing interfaces.
         //This is better anyways as it definately puts RE in some 
         //sort of default state. 
         //Simply seting the text to "" wont.         
         _prevLocation = GetWindow().Location;
         _richEditWindow.Close();
         _richEditWindow = new RichEditWindow();

         _richEditWindow.TextChanged += new EventHandler(OnRETextChanged);
         GetWindow().Load += new EventHandler(RtfPanel_Load);         
         _richEditWindow.Show();         
         SyncButtonPanel.SyncButton.IsEnabled = false;

      }

      void RtfPanel_Load(object sender, EventArgs e)
      {
         _richEditWindow.Location = _prevLocation;
      }

      public override ConverterError Sync()
      {
         ConverterError err = new ConverterError();
         if (null != _logPanel)
         {
            _logPanel.LogInfo("RTF SYNC: Syncing...");
         }

         if (_richEditWindow == _lastUpdatedWindow)
         {
            try
            {
               string formatText = _richEditWindow.ContentAsText;
               _textBox.Text = formatText;
               _lastUpdatedWindow = _richEditWindow;
            }
            catch (Exception x)
            {
               if (null != _logPanel)
               {
                  err.errortype = ConverterErrorType.ErrorException;
                  err.ExceptionText = "RTF SYNC - " + x.Message;
                  _logPanel.LogError(err.ExceptionText);
               }
               return err;
            }
         }
         else if (_textBox == _lastUpdatedWindow)
         {
            try
            {
               string plaintext = _textBox.Text;
               _richEditWindow.Content = plaintext;
               _lastUpdatedWindow = _textBox;
            }
            catch (Exception x)
            {
               if (null != _logPanel)
               {
                  err.errortype = ConverterErrorType.ErrorException;
                  err.ExceptionText = "RTF SYNC - " + x.Message;
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
            _logPanel.LogInfo("RTF SYNC SUCCEEDED");
         }
         return err;
      }

      #region Events
      void OnRETextChanged(object sender, EventArgs e)
      {
         _lastUpdatedWindow = _richEditWindow;

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

      private RichEditWindow _richEditWindow;
      private object _lastUpdatedWindow;
      private System.Drawing.Point _prevLocation = new System.Drawing.Point(0,0);
      private LogPanel _logPanel;
   }
}
