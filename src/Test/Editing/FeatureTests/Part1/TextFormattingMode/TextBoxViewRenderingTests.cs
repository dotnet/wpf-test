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
    /// <summary>
    /// Verifies static rendering in TextBoxView
    /// </summary>
    [Test(0, "TextBox", "TextBoxViewRenderingInDisplayMode", MethodParameters = "/TestCaseType:TextBoxViewRenderingTests /TextFormattingmode=Display", Keywords = "TextFormattingModeTests",Disabled=true)]
    [Test(0, "TextBox", "TextBoxViewRenderingInIdealMode", MethodParameters = "/TestCaseType:TextBoxViewRenderingTests /TextFormattingmode=Ideal", Keywords = "TextFormattingModeTests",Disabled=true)]
    public class TextBoxViewRenderingTests : CustomTestCase
    {
        #region Main flow

        /// <summary>Overridden Entry Point</summary>
        public override void RunTestCase()
        {
            StackPanel sp = new StackPanel();
            _tb = new TextBox();
            _tbl = new TextBlock();

            _tbl.FontSize = _tb.FontSize = 24;
            _tbl.Width = _tb.Width = 700;
            _tbl.Height = _tb.Height = 150;
            _tb.Margin = _tb.Padding = _tb.BorderThickness = new Thickness(0);
            _tb.FontFamily = _tbl.FontFamily = new System.Windows.Media.FontFamily("Arial");

            _tbl.TextWrapping = _tb.TextWrapping = TextWrapping.Wrap;
            _tb.AcceptsReturn = true;
            _tb.SnapsToDevicePixels = _tbl.SnapsToDevicePixels = true;

            switch (ConfigurationSettings.Current.GetArgument("TextFormattingmode"))
            {
                case "Display": TextOptions.SetTextFormattingMode(_tb, TextFormattingMode.Display);
                    TextOptions.SetTextFormattingMode(_tbl, TextFormattingMode.Display);
                    break;
                case "Ideal": TextOptions.SetTextFormattingMode(_tb, TextFormattingMode.Ideal);
                    TextOptions.SetTextFormattingMode(_tbl, TextFormattingMode.Ideal);
                    break;
            }

            sp.Children.Add(_tbl);
            sp.Children.Add(_tb);
            MainWindow.Content = sp;
            MainWindow.Width = 800;
            MainWindow.Height = 700;

            _testData = TextScript.Values;
            QueueDelegate(SetData);
        }

        private void SetData()
        {
            if (_testIndex == _testData.Length + 1)
            {
                Logger.Current.ReportSuccess();
            }
            else
                if (_testIndex == _testData.Length)
                {
                    Log("\r\n-------------- English ------------------\r\n");
                    _tbl.Text = _tb.Text = stringData;
                }
                else
                {
                    Log("\r\n--------------" + _testData[_testIndex].Name + "------------------\r\n");
                    string temp = FormatLongString();
                    string str = temp;
                    str = "Cheese :)" + str;
                    Log(_testData[_testIndex].Name);
                    if (_testIndex + 1 != _testData.Length)
                    {
                        _testIndex++;
                        temp = FormatLongString();

                        str += temp;
                        Log(_testData[_testIndex].Name);
                    }
                    _tbl.Text = _tb.Text = str;
                }
            _tbl.Padding = new Thickness(_tb.GetRectFromCharacterIndex(0).TopLeft.X, _tb.GetRectFromCharacterIndex(0).TopLeft.Y, 0, 0);
            QueueDelegate(CaptureImage);
        }

        private string FormatLongString()
        {
            string temp = _testData[_testIndex].Sample;
            int count = 20;
            int index = 2;
            while (temp.Length > count)
            {
                temp = temp.Insert(count, "\r\n");
                count = 20 * index;
                index++;
            }
            temp = (temp.Length > 100) ? (temp.Substring(0, 100)) : temp;
            return temp;
        }

        private void CaptureImage()
        {
            _textBoxImage = BitmapCapture.CreateBitmapFromElement(_tb);
            _textBlockImage = BitmapCapture.CreateBitmapFromElement(_tbl);

            Rect rectForFirstChar = _tb.GetRectFromCharacterIndex(0);
            double topLeftX = rectForFirstChar.TopLeft.X;
            double topLeftY = rectForFirstChar.TopLeft.Y;

            _textBoxImage = BitmapUtils.CreateBitmapClipped(_textBoxImage, (int)topLeftX, 0, (int)topLeftY, 0);

            _textBoxImage = BitmapUtils.ColorToBlackWhite(_textBoxImage);
            _textBlockImage = BitmapUtils.ColorToBlackWhite(_textBlockImage);

            ComparisonCriteria comparisonCriteria = new ComparisonCriteria();
            comparisonCriteria.MaxPixelDistance = 2;

            bool comparision = false;
            for (double x = 0; x <= topLeftX && comparision == false; x = x + 0.5)
            {
                for (double y = 0; y <= topLeftY && comparision == false; y = y + 0.5)
                {
                    System.Drawing.Bitmap textBlockTempImage = BitmapUtils.CreateBitmapClipped(_textBlockImage, new Thickness(x, y, topLeftX - x, topLeftY - y), true);
                    GlobalLog.LogStatus("---- X:" + x + "----Y:" + y);
                    if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_textBoxImage, textBlockTempImage, comparisonCriteria, false))
                    {
                        comparision = true;
                        break;
                    }
                }
            }
            if (comparision)
            {
                _testIndex++;
                QueueDelegate(SetData);
            }
            else
            {
                Logger.Current.ReportResult(false, "FAILED TEST", false);
            }
        }

        #endregion

        #region Private Data.

        private TextBox _tb;
        private TextBlock _tbl;

        private System.Drawing.Bitmap _textBoxImage,_textBlockImage;
        private int _testIndex = 0;

        private const string stringData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\nabcdefghijklmnopqrstuvwxyz\r\n0123456789\r\n!@#$%^&*()-_+={}[]\\|\":;'<>?,./";
        private TextScript[] _testData;

        #endregion
    }
}