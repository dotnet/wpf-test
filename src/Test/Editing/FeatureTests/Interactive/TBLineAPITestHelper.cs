// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides an interactive way to test TextBox Line API.

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
    using System.Windows.Data;
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

    /// <summary>Provides an interactive way to test TextBox Line API.</summary>
    public class TBLineAPITestHelper : CustomTestCase
    {
        #region Private fields

        Canvas _centerPanel;
        StackPanel _rightPanel;
        WrapPanel _topPanel;
        DockPanel _mainPanel;

        TextBox _tb;

        TextBlock _statusBlock;

        CheckBox _acceptsReturnCB;

        TextBox _heightTB,_widthTB,_hSV,_vSV,_fontSizeTB,_fontFamilyTB,_fontWeightTB,_fontStyleTB, _textDecorationsTB,_textAlignmentTB,_hCAlignmentTB,_vCAlignmentTB,_flowDirectionTB,_wrappingTB;

        TextBox _charIndexTB,_lineIndexTB,_leadingRectTB,_trailingRectTB,_lineLengthTB,_lineTextTB, _firstVisibleLineIndexTB,_lastVisibleLineIndexTB,_lineCountTB;

        Button _firstVisibleLine,_lastVisibleLine;

        System.Windows.Shapes.Rectangle _charRectangle;

        #endregion Private fields
        
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetUpPanels();

            SetUpControlForPropertyValues();

            _tb = new TextBox();            
            _tb.Text = "TextBox Line API Test";
            _tb.MouseMove += new System.Windows.Input.MouseEventHandler(tb_MouseMove);
            _tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);

            _charRectangle = new System.Windows.Shapes.Rectangle();
            _charRectangle.Visibility = Visibility.Hidden;
            _charRectangle.Stroke = Brushes.Aquamarine;
            _charRectangle.StrokeThickness = 1; 

            SetBindings();

            _centerPanel.Children.Add(_tb);
            _centerPanel.Children.Add(_charRectangle);

            MainWindow.Content = _mainPanel;
            MainWindow.Width = 900;
            MainWindow.Height = 700;
            MainWindow.Title = "TextBox Line API test helper";
            MainWindow.Show();
        }

        void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            _lineCountTB.Text = _tb.LineCount.ToString();
        }

        void tb_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rect leadingRect, trailingRect;

            TextBox testTB = (TextBox)sender;
            _charIndexTB.Text = testTB.GetCharacterIndexFromPoint(e.GetPosition(testTB), true).ToString();
            _lineIndexTB.Text = testTB.GetLineIndexFromCharacterIndex(Int32.Parse(_charIndexTB.Text)).ToString();
            leadingRect = testTB.GetRectFromCharacterIndex(Int32.Parse(_charIndexTB.Text), false);
            trailingRect = testTB.GetRectFromCharacterIndex(Int32.Parse(_charIndexTB.Text), true);
            _leadingRectTB.Text = RectToString(leadingRect);
            _trailingRectTB.Text = RectToString(trailingRect);
            
            _lineLengthTB.Text = testTB.GetLineLength(Int32.Parse(_lineIndexTB.Text)).ToString();
            _lineTextTB.Text = testTB.GetLineText(Int32.Parse(_lineIndexTB.Text)).ToString();

            _firstVisibleLineIndexTB.Text = _tb.GetFirstVisibleLineIndex().ToString();
            _lastVisibleLineIndexTB.Text = _tb.GetLastVisibleLineIndex().ToString();

            _lineCountTB.Text = _tb.LineCount.ToString();

            if (_tb.FlowDirection == FlowDirection.LeftToRight)
            {
                _charRectangle.SetValue(Canvas.LeftProperty, leadingRect.X);
            }
            else
            {
                _charRectangle.SetValue(Canvas.LeftProperty, _tb.Width - leadingRect.X);
            }
            _charRectangle.SetValue(Canvas.TopProperty, leadingRect.Y);
            _charRectangle.Height = leadingRect.Height;
            _charRectangle.Width = Math.Abs(trailingRect.X - leadingRect.X);
            _charRectangle.Visibility = Visibility.Visible;
        }

        private void SetUpPanels()
        {
            _mainPanel = new DockPanel();

            _topPanel = new WrapPanel();
            _topPanel.Orientation = Orientation.Horizontal;
            _topPanel.SetValue(DockPanel.DockProperty, Dock.Top);
            _topPanel.Background = Brushes.LightGray;

            _rightPanel = new StackPanel();
            _rightPanel.Orientation = Orientation.Vertical;
            _rightPanel.SetValue(DockPanel.DockProperty, Dock.Right);
            _rightPanel.Background = Brushes.LightGray;
            _rightPanel.Width = 200;

            _statusBlock = new TextBlock();
            _statusBlock.MinHeight = 25;
            _statusBlock.Background = Brushes.LightGray;
            _statusBlock.SetValue(DockPanel.DockProperty, Dock.Bottom);

            _centerPanel = new Canvas();
            _centerPanel.Background = Brushes.DimGray;

            _mainPanel.Children.Add(_topPanel);
            _mainPanel.Children.Add(_statusBlock);
            _mainPanel.Children.Add(_rightPanel);
            _mainPanel.Children.Add(_centerPanel);
        }

        private void SetUpControlForPropertyValues()
        {
            #region TopPanel

            _acceptsReturnCB = new CheckBox();
            _acceptsReturnCB.Content = "AcceptsReturn";
            _acceptsReturnCB.IsChecked = true;
            _acceptsReturnCB.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            _acceptsReturnCB.FlowDirection = FlowDirection.RightToLeft;
            _acceptsReturnCB.Background = Brushes.Aquamarine;

            Label heightLabel = new Label();
            heightLabel.Content = "Height";
            _heightTB = new TextBox();
            _heightTB.Text = "200";

            Label widthLabel = new Label();
            widthLabel.Content = "Width";
            _widthTB = new TextBox();
            _widthTB.Text = "400";
            _heightTB.Background = _widthTB.Background = Brushes.Aquamarine;

            Label hSVLable = new Label();
            hSVLable.Content = "HSV Visibility";
            _hSV = new TextBox();
            _hSV.Text = "Auto";

            Label vSVLable = new Label();
            vSVLable.Content = "VSV Visibility";
            _vSV = new TextBox();
            _vSV.Text = "Auto";
            _hSV.Background = _vSV.Background = Brushes.Aquamarine;

            Label wrappingLabel = new Label();
            wrappingLabel.Content = "TextWrapping";
            _wrappingTB = new TextBox();
            _wrappingTB.Text = "Wrap";
            _wrappingTB.Background = Brushes.Aquamarine;

            Label fontSizeLabel = new Label();
            fontSizeLabel.Content = "FontSize";
            _fontSizeTB = new TextBox();
            _fontSizeTB.Text = "24";

            Label fontFamilyLabel = new Label();
            fontFamilyLabel.Content = "FontFamily";
            _fontFamilyTB = new TextBox();
            _fontFamilyTB.Text = "Palatino Linotype";

            Label fontWeightLabel = new Label();
            fontWeightLabel.Content = "FontWeight";
            _fontWeightTB = new TextBox();
            _fontWeightTB.Text = "Normal";

            Label fontStyleLabel = new Label();
            fontStyleLabel.Content = "FontStyle";
            _fontStyleTB = new TextBox();
            _fontStyleTB.Text = "Normal";

            Label textDecorationsLabel = new Label();
            textDecorationsLabel.Content = "TextDecorations";
            _textDecorationsTB = new TextBox();
            _textDecorationsTB.Text = "";

            _fontSizeTB.Background = _fontFamilyTB.Background = _fontWeightTB.Background =
                _fontStyleTB.Background = _textDecorationsTB.Background = Brushes.Aquamarine;

            Label textAlignmentLabel = new Label();
            textAlignmentLabel.Content = "TextAlignment";
            _textAlignmentTB = new TextBox();
            _textAlignmentTB.Text = "Left";

            Label hCAlignmentLabel = new Label();
            hCAlignmentLabel.Content = "HorizontalContentAlignment";
            _hCAlignmentTB = new TextBox();
            _hCAlignmentTB.Text = "Left";

            Label vCAlignmentLabel = new Label();
            vCAlignmentLabel.Content = "VerticalContentAlignment";
            _vCAlignmentTB = new TextBox();
            _vCAlignmentTB.Text = "Top";

            Label flowDirectionLabel = new Label();
            flowDirectionLabel.Content = "FlowDirection";
            _flowDirectionTB = new TextBox();
            _flowDirectionTB.Text = "LeftToRight";
            _textAlignmentTB.Background = _flowDirectionTB.Background =
                _hCAlignmentTB.Background = _vCAlignmentTB.Background = Brushes.Aquamarine;

            _topPanel.Children.Add(_acceptsReturnCB);
            _topPanel.Children.Add(heightLabel);
            _topPanel.Children.Add(_heightTB);
            _topPanel.Children.Add(widthLabel);
            _topPanel.Children.Add(_widthTB);
            _topPanel.Children.Add(hSVLable);
            _topPanel.Children.Add(_hSV);
            _topPanel.Children.Add(vSVLable);
            _topPanel.Children.Add(_vSV);
            _topPanel.Children.Add(wrappingLabel);
            _topPanel.Children.Add(_wrappingTB);
            _topPanel.Children.Add(fontSizeLabel);
            _topPanel.Children.Add(_fontSizeTB);
            _topPanel.Children.Add(fontFamilyLabel);
            _topPanel.Children.Add(_fontFamilyTB);
            _topPanel.Children.Add(fontWeightLabel);
            _topPanel.Children.Add(_fontWeightTB);
            _topPanel.Children.Add(fontStyleLabel);
            _topPanel.Children.Add(_fontStyleTB);
            _topPanel.Children.Add(textDecorationsLabel);
            _topPanel.Children.Add(_textDecorationsTB);
            _topPanel.Children.Add(textAlignmentLabel);
            _topPanel.Children.Add(_textAlignmentTB);
            _topPanel.Children.Add(hCAlignmentLabel);
            _topPanel.Children.Add(_hCAlignmentTB);
            _topPanel.Children.Add(vCAlignmentLabel);
            _topPanel.Children.Add(_vCAlignmentTB);
            _topPanel.Children.Add(flowDirectionLabel);
            _topPanel.Children.Add(_flowDirectionTB);

            #endregion TopPanel

            #region RightPanel

            Label charIndexLabel = new Label();
            charIndexLabel.Content = "CharacterIndex";
            _charIndexTB = new TextBox();
            _charIndexTB.MouseEnter += new System.Windows.Input.MouseEventHandler(charIndexTB_lineIndexTB_MouseEnter);
            _charIndexTB.MouseLeave += new System.Windows.Input.MouseEventHandler(charIndexTB_lineIndexTB_MouseLeave);
            _charIndexTB.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(charIndexTB_PreviewKeyDown);

            Label lineIndexLabel = new Label();
            lineIndexLabel.Content = "LineIndex";
            _lineIndexTB = new TextBox();
            _lineIndexTB.MouseEnter += new System.Windows.Input.MouseEventHandler(charIndexTB_lineIndexTB_MouseEnter);
            _lineIndexTB.MouseLeave += new System.Windows.Input.MouseEventHandler(charIndexTB_lineIndexTB_MouseLeave);
            _lineIndexTB.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(lineIndexTB_PreviewKeyDown);

            _charIndexTB.Background = _lineIndexTB.Background = Brushes.Aquamarine;

            Label leadingRectLabel = new Label();
            leadingRectLabel.Content = "LeadingRectForCharIndex";
            _leadingRectTB = new TextBox();

            Label trailingRectLabel = new Label();
            trailingRectLabel.Content = "TrailingRectForCharIndex";
            _trailingRectTB = new TextBox();

            Label lineLengthLabel = new Label();
            lineLengthLabel.Content = "LineLength";
            _lineLengthTB = new TextBox();

            Label lineTextLabel = new Label();
            lineTextLabel.Content = "LineText";
            _lineTextTB = new TextBox();

            _firstVisibleLineIndexTB = new TextBox();
            _firstVisibleLine = new Button();
            _firstVisibleLine.Margin = new Thickness(0, 5, 0, 0);
            _firstVisibleLine.Content = "GetFirstVisibleLineIndex";
            _firstVisibleLine.Click += delegate
            {
                _firstVisibleLineIndexTB.Text = _tb.GetFirstVisibleLineIndex().ToString();
            };

            _lastVisibleLineIndexTB = new TextBox();
            _lastVisibleLine = new Button();
            _lastVisibleLine.Margin = new Thickness(0, 5, 0, 0);
            _lastVisibleLine.Content = "GetLastVisibleLineIndex";
            _lastVisibleLine.Click += delegate
            {
                _lastVisibleLineIndexTB.Text = _tb.GetLastVisibleLineIndex().ToString();
            };

            Label lineCountLabel = new Label();
            lineCountLabel.Content = "LineCount";
            _lineCountTB = new TextBox();

            _leadingRectTB.IsReadOnly = _trailingRectTB.IsReadOnly =
                _lineLengthTB.IsReadOnly = _lineTextTB.IsReadOnly =
                _firstVisibleLineIndexTB.IsReadOnly = _lastVisibleLineIndexTB.IsReadOnly =
                _lineCountTB.IsReadOnly = true;

            _leadingRectTB.Background = _trailingRectTB.Background =
                _lineLengthTB.Background = _lineTextTB.Background =
                _firstVisibleLineIndexTB.Background = _lastVisibleLineIndexTB.Background =
                _lineCountTB.Background = Brushes.WhiteSmoke;

            _rightPanel.Children.Add(charIndexLabel);
            _rightPanel.Children.Add(_charIndexTB);
            _rightPanel.Children.Add(lineIndexLabel);
            _rightPanel.Children.Add(_lineIndexTB);
            _rightPanel.Children.Add(leadingRectLabel);
            _rightPanel.Children.Add(_leadingRectTB);
            _rightPanel.Children.Add(trailingRectLabel);
            _rightPanel.Children.Add(_trailingRectTB);
            _rightPanel.Children.Add(lineLengthLabel);
            _rightPanel.Children.Add(_lineLengthTB);
            _rightPanel.Children.Add(lineTextLabel);
            _rightPanel.Children.Add(_lineTextTB);
            _rightPanel.Children.Add(_firstVisibleLine);
            _rightPanel.Children.Add(_firstVisibleLineIndexTB);
            _rightPanel.Children.Add(_lastVisibleLine);
            _rightPanel.Children.Add(_lastVisibleLineIndexTB);
            _rightPanel.Children.Add(lineCountLabel);
            _rightPanel.Children.Add(_lineCountTB);

            #endregion RightPanel
        }

        void charIndexTB_lineIndexTB_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _statusBlock.Inlines.Clear();
            _statusBlock.Inlines.Add(new Run("Enter a value and press Enter to get values for that particular characterIndex"));
        }

        void charIndexTB_lineIndexTB_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _statusBlock.Inlines.Clear();
        }

        void lineIndexTB_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Rect leadingRect, trailingRect;

            if (e.Key == System.Windows.Input.Key.Return)
            {
                try
                {
                    _charIndexTB.Text = _tb.GetCharacterIndexFromLineIndex(Int32.Parse(_lineIndexTB.Text)).ToString();
                    leadingRect = _tb.GetRectFromCharacterIndex(Int32.Parse(_charIndexTB.Text), false);
                    trailingRect = _tb.GetRectFromCharacterIndex(Int32.Parse(_charIndexTB.Text), true);
                    _leadingRectTB.Text = RectToString(leadingRect);
                    _trailingRectTB.Text = RectToString(trailingRect);
                    _lineLengthTB.Text = _tb.GetLineLength(Int32.Parse(_lineIndexTB.Text)).ToString();
                    _lineTextTB.Text = _tb.GetLineText(Int32.Parse(_lineIndexTB.Text)).ToString();

                    _firstVisibleLineIndexTB.Text = _tb.GetFirstVisibleLineIndex().ToString();
                    _lastVisibleLineIndexTB.Text = _tb.GetLastVisibleLineIndex().ToString();

                    if (_tb.FlowDirection == FlowDirection.LeftToRight)
                    {
                        _charRectangle.SetValue(Canvas.LeftProperty, leadingRect.X);
                    }
                    else
                    {
                        _charRectangle.SetValue(Canvas.LeftProperty, _tb.Width - leadingRect.X);
                    }
                    _charRectangle.SetValue(Canvas.TopProperty, leadingRect.Y);
                    _charRectangle.Height = leadingRect.Height;
                    _charRectangle.Width = Math.Abs(trailingRect.X - leadingRect.X);
                    _charRectangle.Visibility = Visibility.Visible;
                }
                catch (Exception exception)
                {
                    _statusBlock.Inlines.Clear();
                    _statusBlock.Inlines.Add(new Run("Failed: "));
                    _statusBlock.Inlines.Add(new Run(exception.ToString()));
                }
            }
        }

        void charIndexTB_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Rect leadingRect, trailingRect;

            if (e.Key == System.Windows.Input.Key.Return)
            {
                try
                {
                    leadingRect = _tb.GetRectFromCharacterIndex(Int32.Parse(_charIndexTB.Text), false);
                    trailingRect = _tb.GetRectFromCharacterIndex(Int32.Parse(_charIndexTB.Text), true);
                    _leadingRectTB.Text = RectToString(leadingRect);
                    _trailingRectTB.Text = RectToString(trailingRect);
                    _lineIndexTB.Text = _tb.GetLineIndexFromCharacterIndex(Int32.Parse(_charIndexTB.Text)).ToString();
                    _lineLengthTB.Text = _tb.GetLineLength(Int32.Parse(_lineIndexTB.Text)).ToString();
                    _lineTextTB.Text = _tb.GetLineText(Int32.Parse(_lineIndexTB.Text)).ToString();

                    _firstVisibleLineIndexTB.Text = _tb.GetFirstVisibleLineIndex().ToString();
                    _lastVisibleLineIndexTB.Text = _tb.GetLastVisibleLineIndex().ToString();

                    if (_tb.FlowDirection == FlowDirection.LeftToRight)
                    {
                        _charRectangle.SetValue(Canvas.LeftProperty, leadingRect.X);
                    }
                    else
                    {
                        _charRectangle.SetValue(Canvas.LeftProperty, _tb.Width - leadingRect.X);
                    }
                    _charRectangle.SetValue(Canvas.TopProperty, leadingRect.Y);
                    _charRectangle.Height = leadingRect.Height;
                    _charRectangle.Width = Math.Abs(trailingRect.X - leadingRect.X);
                    _charRectangle.Visibility = Visibility.Visible;
                }
                catch (Exception exception)
                {
                    _statusBlock.Inlines.Clear();
                    _statusBlock.Inlines.Add(new Run("Failed: "));
                    _statusBlock.Inlines.Add(new Run(exception.ToString()));
                }
            }
        }

        private void SetBindings()
        {
            Binding acceptsReturnBinding = new Binding("IsChecked");
            acceptsReturnBinding.Mode = BindingMode.OneWay;
            acceptsReturnBinding.Source = _acceptsReturnCB;
            _tb.SetBinding(TextBox.AcceptsReturnProperty, acceptsReturnBinding);

            Binding heightBinding = new Binding("Text");
            heightBinding.Mode = BindingMode.OneWay;
            heightBinding.Source = _heightTB;
            _tb.SetBinding(TextBox.HeightProperty, heightBinding);

            Binding widthBinding = new Binding("Text");
            widthBinding.Mode = BindingMode.OneWay;
            widthBinding.Source = _widthTB;
            _tb.SetBinding(TextBox.WidthProperty, widthBinding);

            Binding hSVBinding = new Binding("Text");
            hSVBinding.Mode = BindingMode.OneWay;
            hSVBinding.Source = _hSV;
            _tb.SetBinding(TextBox.HorizontalScrollBarVisibilityProperty, hSVBinding);

            Binding vSVBinding = new Binding("Text");
            vSVBinding.Mode = BindingMode.OneWay;
            vSVBinding.Source = _vSV;
            _tb.SetBinding(TextBox.VerticalScrollBarVisibilityProperty, vSVBinding);

            Binding wrappingBinding = new Binding("Text");
            wrappingBinding.Mode = BindingMode.OneWay;
            wrappingBinding.Source = _wrappingTB;
            _tb.SetBinding(TextBox.TextWrappingProperty, wrappingBinding);

            Binding fontSizeBinding = new Binding("Text");
            fontSizeBinding.Mode = BindingMode.OneWay;
            fontSizeBinding.Source = _fontSizeTB;
            _tb.SetBinding(TextBox.FontSizeProperty, fontSizeBinding);

            Binding fontFamilyBinding = new Binding("Text");
            fontFamilyBinding.Mode = BindingMode.OneWay;
            fontFamilyBinding.Source = _fontFamilyTB;
            _tb.SetBinding(TextBox.FontFamilyProperty, fontFamilyBinding);

            Binding fontWeightBinding = new Binding("Text");
            fontWeightBinding.Mode = BindingMode.OneWay;
            fontWeightBinding.Source = _fontWeightTB;
            _tb.SetBinding(TextBox.FontWeightProperty, fontWeightBinding);

            Binding fontStyleBinding = new Binding("Text");
            fontStyleBinding.Mode = BindingMode.OneWay;
            fontStyleBinding.Source = _fontStyleTB;
            _tb.SetBinding(TextBox.FontStyleProperty, fontStyleBinding);

            Binding textDecorationsBinding = new Binding("Text");
            textDecorationsBinding.Mode = BindingMode.OneWay;
            textDecorationsBinding.Source = _textDecorationsTB;
            _tb.SetBinding(TextBox.TextDecorationsProperty, textDecorationsBinding);

            Binding textAlignmentBinding = new Binding("Text");
            textAlignmentBinding.Mode = BindingMode.OneWay;
            textAlignmentBinding.Source = _textAlignmentTB;
            _tb.SetBinding(TextBox.TextAlignmentProperty, textAlignmentBinding);

            Binding hCAlignmentBinding = new Binding("Text");
            hCAlignmentBinding.Mode = BindingMode.OneWay;
            hCAlignmentBinding.Source = _hCAlignmentTB;
            _tb.SetBinding(TextBox.HorizontalContentAlignmentProperty, hCAlignmentBinding);

            Binding vCAlignmentBinding = new Binding("Text");
            vCAlignmentBinding.Mode = BindingMode.OneWay;
            vCAlignmentBinding.Source = _vCAlignmentTB;
            _tb.SetBinding(TextBox.VerticalContentAlignmentProperty, vCAlignmentBinding);

            Binding flowDirectionBinding = new Binding("Text");
            flowDirectionBinding.Mode = BindingMode.OneWay;
            flowDirectionBinding.Source = _flowDirectionTB;
            _tb.SetBinding(TextBox.FlowDirectionProperty, flowDirectionBinding);
        }

        private string RectToString(Rect rect)
        {
            string str;
            str = "X(" + rect.X.ToString("F") + "),";
            str += "Y(" + rect.Y.ToString("F") + "),";
            str += "W(" + rect.Width.ToString("F") + "),";
            str += "H(" + rect.Height.ToString("F") + ")";

            return str;
        }
    }
}