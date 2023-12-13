// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//              Creates Window with Xaml RichTextBox control
//              (for rendering text displayed in Xaml Text Panel)
//

using System.IO;

// avalon
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace RtfXamlView
{
   class XamlWindow : Window//, IFormattedViewWindow
   {
      public XamlWindow()
      {
         // Set the window
         this.Name = "XamlViewWindow";
         this.Title = "XAML View Window";
         this.Width = 450;
         this.Height = 450;
         this.Background = new SolidColorBrush(Color.FromRgb(153, 198, 241)); ;
         this.ShowInTaskbar = false;
         this.WindowStyle = WindowStyle.ToolWindow;

         // Set RichTextBox control
         _xamlTextBox = new RichTextBox();         

         _xamlTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

         //ripped from lexiconEditor.cs source***************************
         // We are called from the Parser which sets restricted security level.
         // To get access to non-public RichTextBox control we temporarily unrestrict premissions.
         new System.Security.Permissions.ReflectionPermission(
             System.Security.Permissions.PermissionState.Unrestricted)
             .Assert();
         _xamlTextBox.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(PastingEventHandler));
         ((ContentControl)this).Content = _xamlTextBox;
         _fCopyPaste = true;
      }

      //to check the clipboard format to get the richtext format.
      private void PastingEventHandler(object sender, DataObjectPastingEventArgs e)
      {
         if (e.FormatToApply == DataFormats.Xaml)
         {
            // XAML content is already available; we need not do anything else.
         }
         else if (!e.SourceDataObject.GetDataPresent(DataFormats.Rtf))
         {
         }
      }

      #region Public Properties

      // Get Properties
      public RichTextBox XamlTextBox
      {
         get
         {
            return _xamlTextBox;
         }
      }

      public bool CopyPaste
      {
         get { return _fCopyPaste; }
         set { _fCopyPaste = (bool)value; }
      }

      public string ContentRTF
      {
         set
         {
            Clipboard.Clear();
            IDataObject ido = new DataObject("Rich Text Format", value);
            Clipboard.SetDataObject(ido);
            _xamlTextBox.SelectAll();
            _xamlTextBox.Paste();

            //Reset the UndoIsEnabled to clear the undo stack
            _xamlTextBox.IsUndoEnabled = false;
            _xamlTextBox.IsUndoEnabled = true;

            UpdateLayout();
         }
         get
         {
            Clipboard.Clear();
            _xamlTextBox.SelectAll();
            _xamlTextBox.Copy();
            IDataObject ido = Clipboard.GetDataObject();
            return ido.GetData(DataFormats.Rtf).ToString();
         }
      }
      #endregion

      #region IFormattedViewWindow Members
      public string ContentAsText
      {
         get
         {
            if (_fCopyPaste == true)
            {
               Clipboard.Clear();
               _xamlTextBox.SelectAll();
               _xamlTextBox.Copy();
               IDataObject ido = Clipboard.GetDataObject();
               return ido.GetData("Xaml").ToString();
            }
            else
               return XamlWriter.Save(_xamlTextBox);
         }
      }

      public new object Content
      {
         set
         {
            string xamlText = (string)value;
            if (_fCopyPaste == true)
            {
               Clipboard.Clear();
               IDataObject ido = new DataObject("Xaml", xamlText);
               Clipboard.SetDataObject(ido);
               _xamlTextBox.SelectAll();
               _xamlTextBox.Paste();
            }
            else
            {
               _xamlTextBox.Document = (FlowDocument)Parse(xamlText);
            }
            UpdateLayout();
         }
      }
      #endregion

      #region Private Utility Methods
      private object Parse(string str)
      {
         MemoryStream ms = new MemoryStream(str.Length);
         StreamWriter sw = new StreamWriter(ms);
         sw.Write(str);
         sw.Flush();
         ms.Seek(0, SeekOrigin.Begin);

         object obj = XamlReader.Load(ms);
         if (obj is FixedDocument)
         {
            FixedDocument panel = obj as FixedDocument;
            obj = panel.Pages[0].GetPageRoot(false);
         }
         sw.Close();
         return obj;
      }
      #endregion

      #region Events
      protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
      {
         e.Cancel = true;
      }
      #endregion

      private RichTextBox _xamlTextBox;
      private bool _fCopyPaste;
   }
}