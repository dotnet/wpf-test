// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides an interactive case for TextBox.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Interactive/TextExplorer.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;

    using System.ComponentModel.Design;
    using Drawing = System.Drawing;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Input;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>Provides an interactive test case for TextBox.</summary>
    public class TextExplorer: CustomTestCase
    {
        #region Public methods.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _topPanel = new StackPanel();
            _textbox = new TextBox();
            _status = new TextBox();

            _switchControlButton = new Button();
            _switchControlButton.Click += delegate {
                _viewingPlainText = !_viewingPlainText;
                UpdateSwitchControl();
            };

            _textbox.Text =
                "This is line one." + Environment.NewLine +
                "This is line two.";
            _textbox.MaxLines = 8;
            _textbox.SelectionChanged += TextBoxSelectionChanged;

            _fontSizeTextBox = new TextBox();
            _fontSizeTextBox.Text = "24";

            _fontFamilyTextBox = new TextBox();
            _fontFamilyTextBox.Text = "Lucida Console";

            _setFontButton = new Button();
            _setFontButton.Content = "Set Font";
            _setFontButton.Click += delegate
            {
                _textbox.FontFamily = new FontFamily(_fontFamilyTextBox.Text);
                _textbox.FontSize = double.Parse(_fontSizeTextBox.Text);
            };

            _status.FontFamily = new FontFamily("Lucida Console");
            _status.FontSize = 18f;

            _topPanel.Children.Add(_switchControlButton);
            _topPanel.Children.Add(_textbox);
            _topPanel.Children.Add(_fontSizeTextBox);
            _topPanel.Children.Add(_fontFamilyTextBox);
            _topPanel.Children.Add(_setFontButton);
            _topPanel.Children.Add(_status);

            SetupTextPastingUI();
            SetupRichBoxDisplayUI();
            SetupAnnotationsUI();

            _viewingPlainText = false;
            UpdateSwitchControl();

            MainWindow.Content = _topPanel;

            MainWindow.Show();
        }

        private void UpdateSwitchControl()
        {
            if (_viewingPlainText)
            {
                _switchControlButton.Content = "Switch to RichTextBox";
                _imageScroller.Visibility =
                    _richBox.Visibility =
                    _pastePanel.Visibility = 
                    _annotationsPanel.Visibility = 
                    Visibility.Collapsed;
                _fontSizeTextBox.Visibility =
                    _fontFamilyTextBox.Visibility = 
                    _textbox.Visibility =
                    _setFontButton.Visibility = 
                    Visibility.Visible;
            }
            else
            {
                _switchControlButton.Content = "Switch to TextBox";
                _fontSizeTextBox.Visibility =
                    _fontFamilyTextBox.Visibility =
                    _textbox.Visibility =
                    _setFontButton.Visibility =
                    Visibility.Collapsed;
                _imageScroller.Visibility =
                    _richBox.Visibility = 
                    _pastePanel.Visibility = 
                    _annotationsPanel.Visibility = 
                    Visibility.Visible;
            }
        }

        private void SetupRichBoxDisplayUI()
        {
            Image containerImage;

            _richBox = new RichTextBox();
            _imageScroller = new ScrollViewer();
            containerImage = new Image();

            _imageScroller.Content = containerImage;
            _imageScroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _richBox.TextChanged += delegate { UpdateContainer(containerImage); };
            _richBox.Selection.Changed += delegate { UpdateContainer(containerImage); };

            _topPanel.Children.Add(_richBox);
            _topPanel.Children.Add(_imageScroller);
        }

        private int _imageId;

        private void UpdateContainer(Image image)
        {
            Uri uri;
            string path;
            UIElementWrapper wrapper;

            wrapper = new UIElementWrapper(_richBox);
            _imageId++;
            TextTreeLogger.LogContainer("text-explorer-log-" + _imageId, wrapper.SelectionInstance.Start,
                wrapper.SelectionInstance.Start, "Selection.Start",
                wrapper.SelectionInstance.End, "Selection.End");
            path = System.IO.Path.Combine(System.Environment.CurrentDirectory, "text-explorer-log-" + _imageId + ".png");
            uri = new Uri("file://" + path);
            image.Source = new BitmapImage(uri);

            QueueHelper.Current.QueueDelegate((SimpleHandler) delegate {
                StatusText =
                    "Selection Start: " + DescribePointer(wrapper.SelectionInstance.Start) +
                    "\r\nSelection End: " + DescribePointer(wrapper.SelectionInstance.End) +
                    "\r\nSelection Moving: " + DescribePointer(wrapper.SelectionMovingPointer);
                if ((bool)_updatePasteBox.IsChecked)
                {
                    _pasteBox.Text = XamlUtils.TextRange_GetXml(_richBox.Selection);
                }
                try
                {
                    System.IO.File.Delete(path);
                }
                catch (System.IO.IOException)
                {
                    StatusText += "\r\nUnable to delete " + path + ".";
                }
            });
        }

        private void SetupTextPastingUI()
        {
            _pastePanel = new StackPanel();
            _pasteBox = new TextBox();
            _updatePasteBox = new CheckBox();

            _pasteBox.AcceptsReturn = true;
            _pasteBox.TextWrapping = TextWrapping.Wrap;
            _pasteBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _pasteBox.MinLines = 4;
            _pasteBox.MaxLines = 8;

            _updatePasteBox.Content = "Update Paste Box";
            _updatePasteBox.IsChecked = false;

            Button pasteButton = new Button();
            pasteButton.Click += delegate
            {
                XamlUtils.TextRange_SetXml(_richBox.Selection, _pasteBox.Text);
            };
            pasteButton.Content = "Paste XML";

            _pastePanel.Children.Add(_updatePasteBox);
            _pastePanel.Children.Add(_pasteBox);
            _pastePanel.Children.Add(pasteButton);
            _topPanel.Children.Add(_pastePanel);
        }

        private void SetupAnnotationsUI()
        {
            _annotationsPanel = new StackPanel();
            StackPanel buttonStrip = new StackPanel();
            FlowDocumentPageViewer viewer = new FlowDocumentPageViewer();
            Button enableAnnotations = new Button();
            Button createHighlight = new Button();
            Button createInk = new Button();
            Button createText = new Button();

            enableAnnotations.Content = "Enable Annotations";
            enableAnnotations.Click += delegate {
                enableAnnotations.IsEnabled = false;

                FlowDocument document = _richBox.Document;
                _richBox.Document = new FlowDocument();
                viewer.Document = document;
                viewer.Height = 200;
                System.Windows.Annotations.AnnotationService service = new System.Windows.Annotations.AnnotationService(viewer);
                service.Enable(new System.Windows.Annotations.Storage.XmlStreamStore(new System.IO.MemoryStream()));
                service.Store.AutoFlush = true;
            };

            createHighlight.Command = System.Windows.Annotations.AnnotationService.CreateHighlightCommand;
            createHighlight.Content = "Highlight";
            createHighlight.CommandTarget = viewer;
            createInk.Command = System.Windows.Annotations.AnnotationService.CreateInkStickyNoteCommand;
            createInk.Content = "Ink";
            createInk.CommandTarget = viewer;
            createText.Command = System.Windows.Annotations.AnnotationService.CreateTextStickyNoteCommand;
            createText.Content = "Text";
            createText.CommandTarget = viewer;

            buttonStrip.Orientation = Orientation.Horizontal;
            buttonStrip.Children.Add(enableAnnotations);
            buttonStrip.Children.Add(createHighlight);
            buttonStrip.Children.Add(createInk);
            buttonStrip.Children.Add(createText);

            _annotationsPanel.Children.Add(buttonStrip);
            _annotationsPanel.Children.Add(viewer);

            _topPanel.Children.Add(_annotationsPanel);
        }

        private static string DescribePointer(TextPointer pointer)
        {
            if (pointer == null)
            {
                return null;
            }
            else
            {
                if (!pointer.HasValidLayout)
                {
                    return TextUtils.GetDistanceFromStart(pointer).ToString() +
                        " (" + pointer.LogicalDirection.ToString().Substring(0, 1) + ")" +
                        " - no layout info";
                }
                else
                {
                    return TextUtils.GetDistanceFromStart(pointer).ToString() +
                        " (" + pointer.LogicalDirection.ToString().Substring(0, 1) + ")" +
                        ((pointer.IsAtLineStartPosition)? " " : " not ") + "at line boundary";
                }
            }
        }

        #endregion Public methods.

        #region Event handlers.

        private void TextBoxSelectionChanged(object sender, RoutedEventArgs args)
        {
            QueueHelper.Current.QueueDelegate((SimpleHandler) delegate {
                UIElementWrapper wrapper;
                string text;

                wrapper = new UIElementWrapper(_textbox);

                text = "Selected Text: [" + _textbox.SelectedText + "]";
                text +=
                    "\r\nSelection Start: " + DescribePointer(wrapper.SelectionInstance.Start) +
                    "\r\nSelection End: " + DescribePointer(wrapper.SelectionInstance.End) +
                    "\r\nSelection Moving: " + DescribePointer(wrapper.SelectionMovingPointer);

                StatusText = text;
            });
        }

        #endregion Event handlers.

        #region Private properties.

        /// <summary>Status text displayed to user.</summary>
        private string StatusText
        {
            get { return _status.Text; }
            set { _status.Text = value; }
        }

        #endregion Private properties.

        #region Private fields.

        private TextBox _textbox;
        private TextBox _fontSizeTextBox;
        private TextBox _fontFamilyTextBox;
        private RichTextBox _richBox;
        private Button _setFontButton;
        private Button _switchControlButton;
        private StackPanel _topPanel;
        private TextBox _status;
        private ScrollViewer _imageScroller;
        private StackPanel _pastePanel;
        private TextBox _pasteBox;
        private CheckBox _updatePasteBox;
        private StackPanel _annotationsPanel;

        private bool _viewingPlainText;

        private static string s_nl = Environment.NewLine;

        #endregion Private fields.
    }
}
