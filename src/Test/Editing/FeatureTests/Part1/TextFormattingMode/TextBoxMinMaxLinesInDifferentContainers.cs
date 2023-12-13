// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Few tests with TextOptions.TextFormattingMode set to Ideal and Display

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;     
    using System.Globalization;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Input;
    using Microsoft.Test.Threading;

    using Test.Uis.Data;    
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;   
    
    #endregion Namespaces.    
    
    /// <summary>Tests MinMaxLines in different containers </summary>
    [Test(2, "TextBox", "TextBoxMinMaxLinesInDifferentContainersForDisplayMode", MethodParameters = "/TestCaseType:TextBoxMinMaxLinesInDifferentContainers /TextFormattingmode=Display", Keywords = "TextFormattingModeTests")]
    [Test(2, "TextBox", "TextBoxMinMaxLinesInDifferentContainersForIdealMode", MethodParameters = "/TestCaseType:TextBoxMinMaxLinesInDifferentContainers /TextFormattingmode=Ideal", Keywords = "TextFormattingModeTests")]
    public class TextBoxMinMaxLinesInDifferentContainers : CustomTestCase
    {

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _count = _panels.Length;
            LoopDifferentContainers();
        }

        /// <summary>Program controller</summary>
        private void LoopDifferentContainers()
        {
            if (_count > 0)
            {
                _p = _panels[_count - 1];
                _textbox = new TextBox();
                _textbox.FontSize = 20;
                _textbox.MinLines = 2;
                _textbox.MaxLines = 4;
                _textbox.AcceptsReturn = true;
                switch (ConfigurationSettings.Current.GetArgument("TextFormattingmode"))
                {
                    case "Display": TextOptions.SetTextFormattingMode(_textbox, TextFormattingMode.Display);
                        break;
                    case "Ideal": TextOptions.SetTextFormattingMode(_textbox, TextFormattingMode.Ideal);
                        break;
                }
                Log("\r\n******************** TEXTBOX WITHIN " + _p.GetType().ToString() + "******************************\r\n");
                _p.Children.Add(_textbox);
                MainWindow.Content = _p;
                _count--;
                QueueDelegate(SetMinimalText);
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        /// <summary>sets single line text</summary>
        private void SetMinimalText()
        {
            _textbox.Text = singleLineString;
            QueueDelegate(VerifyTextboxHeight);
        }

        /// <summary>Verifies the height of textbox</summary>
        private void VerifyTextboxHeight()
        {
            double height = GetLineHeight();
            double estimatedHeight = 0;
            if ((_p is Grid) || (_p is DockPanel))
            {
                estimatedHeight = height * _textbox.MaxLines;
            }
            else
            {
                estimatedHeight = height * _textbox.MinLines;
            }
            Verifier.Verify((estimatedHeight < (_textbox.ViewportHeight + 2.0)) && (estimatedHeight > (_textbox.ViewportHeight - 2.0)),
                "EstimatedHeight [" + estimatedHeight.ToString() + "] == Actual ViewPortHeight [" + _textbox.ViewportHeight.ToString() + "] +-2", true);
            QueueDelegate(SetMultiLineText);
        }

        /// <summary>Sets multiline text</summary>
        private void SetMultiLineText()
        {
            _textbox.Text = multiLineString;
            QueueDelegate(VerifyTextboxHeightMultiLine);
        }

        /// <summary>Verifies the height and number of lines in textbox</summary>
        private void VerifyTextboxHeightMultiLine()
        {
            double height = GetLineHeight();
            double estimatedHeight = 0;
            estimatedHeight = height * _textbox.MaxLines;
            Verifier.Verify((estimatedHeight < (_textbox.ViewportHeight + 2.0)) && (estimatedHeight > (_textbox.ViewportHeight - 2.0)),
                "EstimatedHeight [" + estimatedHeight.ToString() + "] == Actual ViewPortHeight [" + _textbox.ViewportHeight.ToString() + "] +-2", true);

            int numberOfLines = _textbox.GetLastVisibleLineIndex() - _textbox.GetFirstVisibleLineIndex() + 1; //BECAUSE ITS 0 BASED
            Verifier.Verify(numberOfLines == _textbox.MaxLines, "Estimated number of lines in view [" + numberOfLines.ToString() + "] == Actual MaxLines [" +
                _textbox.MaxLines.ToString() + "]", true);

            QueueDelegate(LoopDifferentContainers);
        }

        /// <summary>Gets LineHeight</summary>
        private double GetLineHeight()
        {
            System.Windows.Media.FontFamily fontFamily = _textbox.FontFamily;
            double fontSize = _textbox.FontSize;
            double height = 0;

            if ((TextOptions.GetTextFormattingMode(_textbox)) == TextFormattingMode.Ideal)
            {               
                height = fontFamily.LineSpacing * fontSize;
            }
            else
            {
#if TESTBUILD_NET_ATLEAST_462
                double pixelsPerDip = VisualTreeHelper.GetDpi(textbox).PixelsPerDip;
                object objHeight = ReflectionUtils.InvokeInstanceMethod((object)fontFamily, "GetLineSpacingForDisplayMode", new object[] { fontSize, pixelsPerDip });
#else
                object objHeight = ReflectionUtils.InvokeInstanceMethod((object)fontFamily, "GetLineSpacingForDisplayMode", new object[] { fontSize });
#endif
                height = (double)objHeight;                
            }
            return height;
        }

        #region data.

        private Panel[] _panels = { new StackPanel(), new DockPanel(), new Grid(), new Canvas(), new WrapPanel() };
        private int _count = 0;
        private TextBox _textbox;
        private const string singleLineString = "Say Hello";
        private const string multiLineString = "Say 1\r\nSay 2\r\nSay 3\r\nSay 4\r\nSay 5\r\nSay 6\r\nSay 7\r\nSay 8\r\nSay 9\r\nSay 10\r\n";
        private Panel _p = null;

        #endregion data.
    }
}