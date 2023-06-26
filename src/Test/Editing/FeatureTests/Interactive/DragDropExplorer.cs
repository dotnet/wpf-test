// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides an application to test drag/drop features.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Interactive/DragDropExplorer.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.IO;

    using System.ComponentModel.Design;
    using Drawing = System.Drawing;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;

    using Test.Uis.Data;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// An interactive application to test drag/drop featuers.
    /// </summary>
    public class DragDropExplorer: CustomTestCase
    {
        #region Public methods.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            CreateUserInterface();
        }

        #endregion Public methods.

        #region Private methods.

        private void CreateUserInterface()
        {
            _topPanel = new DockPanel();
            _dropLabel = new TextBlock();
            _dropPanel = new RichTextBox();
            _textBoxLabel = new TextBlock();
            _textBox = new TextBox();
            _richBoxLabel = new TextBlock();
            _richBox = new RichTextBox();

            _dropLabel.Text = "Drop on this panel to examine a data object.";
            _textBoxLabel.Text = "Use this TextBox to test plain text.";
            _richBoxLabel.Text = "Use this RichTextBox to test rich text.";
            _dropPanel.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            MainWindow.FontSize = 16;
            _dropPanel.Background = new LinearGradientBrush(
                Colors.White, Colors.SlateBlue, 90);
            _dropPanel.AllowDrop = true;
            _dropPanel.PreviewDragEnter += DropPanelDragEnter;
            _dropPanel.IsReadOnly = true;
            _dropPanel.MaxHeight = 300;

            DockPanel.SetDock(_dropLabel, Dock.Top);
            DockPanel.SetDock(_dropPanel, Dock.Top);
            DockPanel.SetDock(_textBoxLabel, Dock.Top);
            DockPanel.SetDock(_textBox, Dock.Top);
            DockPanel.SetDock(_richBoxLabel, Dock.Top);
            DockPanel.SetDock(_richBox, Dock.Top);

            _topPanel.Children.Add(_dropLabel);
            _topPanel.Children.Add(_dropPanel);
            _topPanel.Children.Add(_textBoxLabel);
            _topPanel.Children.Add(_textBox);
            _topPanel.Children.Add(_richBoxLabel);
            _topPanel.Children.Add(_richBox);

            MainWindow.Content = _topPanel;
        }

        private string GetDataAsString(object data)
        {
            Stream stream;

            stream = data as Stream;
            if (stream != null)
            {
                using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                {
                    return reader.ReadToEnd().Replace("\x0000", "\r\n");
                }
            }
            return data.ToString();
        }

        private void DropPanelDragEnter(object sender, DragEventArgs e)
        {
            IDataObject data;
            string description;
            string nl;
            string[] formats;

            nl = "<LineBreak />";
            data = e.Data;
            description = "Data present: " + (data != null);
            if (data != null)
            {
                formats = data.GetFormats();
                foreach(string format in formats)
                {
                    object rawData;

                    description += nl + "Format: " + format;

                    Logger.Current.Log("Getting data for: " + format);
                    rawData = data.GetData(format);
                    if (rawData == null)
                    {
                        description += " - no data available";
                        continue;
                    }

                    description += " - data type: " + rawData.GetType().Name;
                    if (IsFormat(format, DataFormats.CommaSeparatedValue))
                    {
                        description += nl + "--" + nl + GetDataAsString(rawData) + nl + "--";
                    }
                    else if (IsFormat(format, "Rich TextBlock Format") ||
                             IsFormat(format, DataFormats.Html) ||
                             IsFormat(format, DataFormats.UnicodeText) ||
                             IsFormat(format, DataFormats.Xaml))
                    {
                        description += nl + "--" + nl + XmlEncode(rawData.ToString())
                            .Replace("\r\n", nl) + nl + "--";
                    }
                }
            }
            TextRange myRange = new TextRange(_dropPanel.Document.ContentStart, _dropPanel.Document.ContentEnd);
            XamlUtils.TextRange_SetXml(myRange, "<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"
                + description + "</FlowDocument>");
        }

        private static string XmlEncode(string xml)
        {
            return xml.Replace("&", "&amp;").Replace("<", "&lt;").
                Replace(">", "&gt;").Replace("'", "&quot;");
        }

        /// <summary>
        /// Returns whether the specified format matches another format.
        /// </summary>
        private static bool IsFormat(string format, string formatToMatch)
        {
            return String.Compare(format, formatToMatch, true,
                System.Globalization.CultureInfo.InvariantCulture) == 0;
        }

        #endregion Private methods.

        #region Private fields.

        private DockPanel _topPanel;
        private TextBlock _dropLabel;
        private RichTextBox _dropPanel;
        private TextBlock _textBoxLabel;
        private TextBox _textBox;
        private TextBlock _richBoxLabel;
        private RichTextBox _richBox;

        #endregion Private fields.
    }
}
