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
    /// Runs a test case by typing a string and verifying the resulting selection.
    /// </summary>
    [Test(0, "TextBox", "MaxMinLinesTestInDisplayMode", MethodParameters = "/TestCaseType:MaxMinLinesTests /TextFormattingmode=Display", Keywords = "TextFormattingModeTests")]
    [Test(0, "TextBox", "MaxMinLinesTestInIdealMode", MethodParameters = "/TestCaseType:MaxMinLinesTests /TextFormattingmode=Ideal", Keywords = "TextFormattingModeTests")]
    public class MaxMinLinesTests : CustomTestCase
    {
        #region TestCaseData

        internal enum Mode { None = 0, OnlyMinLines = 1, OnlyMaxLines = 2, Both = 3 };

        /// <summary>Data driven test cases.</summary>
        internal class TestData
        {
            #region PrivateData

            Mode _testMode;
            int _minLines;
            int _maxLines;
            int _expLines;
            double _height;
            double _minHeight;
            double _maxHeight;
            string _text;
            bool _acceptsReturn;
            bool _textWrapping;
            int _fontSize;
            string _fontFamily;

            bool _doStep2;
            int _nextStepType; // (=1 for Add), (=2 for Delete)
            int _expLinesStep2;
            string _textStep2;
            int _numOfDel;

            #endregion PrivateData

            #region InternalProperties
            internal Mode TestMode { get { return this._testMode; } }
            internal int MinLines { get { return this._minLines; } }
            internal int MaxLines { get { return this._maxLines; } }
            internal int ExpLines { get { return this._expLines; } }
            internal double Height { get { return this._height; } }
            internal double MinHeight { get { return this._minHeight; } }
            internal double MaxHeight { get { return this._maxHeight; } }
            internal string Text { get { return this._text; } }
            internal bool AcceptsReturn { get { return this._acceptsReturn; } }
            internal bool Wrap { get { return this._textWrapping; } }
            internal int FontSize { get { return this._fontSize; } }
            internal string FontFamily { get { return this._fontFamily; } }
            internal bool DoStep2
            {
                get { return this._doStep2; }
                set { _doStep2 = value; }
            }
            internal int NextStepType { get { return this._nextStepType; } }
            internal int ExpLinesStep2 { get { return this._expLinesStep2; } }
            internal string TextStep2 { get { return this._textStep2; } }
            internal int NumOfDel { get { return this._numOfDel; } }
            #endregion InternalProperties

            internal TestData(Mode testMode, int minLines, int maxLines, int expLines,
                              string text, bool acceptsReturn, bool wrap, int fontSize)
            {
                this._testMode = testMode;
                this._minLines = minLines;
                this._maxLines = maxLines;
                this._expLines = expLines;

                this._text = text;
                this._acceptsReturn = acceptsReturn;
                this._textWrapping = wrap;
                this._fontSize = fontSize;

                this._height = 0;
                this._minHeight = 0;
                this._maxHeight = 0;
                this._fontFamily = "Tahoma";

                this._doStep2 = false;
                this._nextStepType = 0;
                this._expLinesStep2 = 0;
                this._textStep2 = "";
                this._numOfDel = 0;
            }

            internal TestData(Mode testMode, int minLines, int maxLines, int expLines,
                              string text, bool acceptsReturn, bool wrap, int fontSize,
                              bool doStep2, int expLinesStep2, string textStep2,
                              int nextStepType, int numOfDel)
            {
                this._testMode = testMode;
                this._minLines = minLines;
                this._maxLines = maxLines;
                this._expLines = expLines;

                this._text = text;
                this._acceptsReturn = acceptsReturn;
                this._textWrapping = wrap;
                this._fontSize = fontSize;

                this._height = 0;
                this._minHeight = 0;
                this._maxHeight = 0;
                this._fontFamily = "Tahoma";

                this._expLinesStep2 = expLinesStep2;
                this._doStep2 = doStep2;
                this._textStep2 = textStep2;
                this._nextStepType = nextStepType;
                this._numOfDel = numOfDel;
            }

            internal TestData(Mode testMode, int minLines, int maxLines, int expLines,
                              string text, bool acceptsReturn, bool wrap, int fontSize,
                              double height, double minHeight, double maxHeight, string fontFamily)
            {
                this._testMode = testMode;
                this._minLines = minLines;
                this._maxLines = maxLines;
                this._expLines = expLines;

                this._text = text;
                this._acceptsReturn = acceptsReturn;
                this._textWrapping = wrap;
                this._fontSize = fontSize;

                this._height = height;
                this._minHeight = minHeight;
                this._maxHeight = maxHeight;
                this._fontFamily = fontFamily;

                this._expLinesStep2 = 0;
                this._doStep2 = false;
                this._textStep2 = "";
                this._nextStepType = 0;
                this._numOfDel = 0;
            }

            internal static TestData[] TestCases = new TestData[] {
                //For testing
                //new TestData(Mode.Both, 1, 2, 1, "This is test1", false, true, 24),

                //Dont set MinLines and MaxLines (Case:0)
                //new TestData(Mode.None, 0, 0, 2, "Test is test1 Test is test2", true, true, 24),

                //Simple MinLines scenario tests (Case: 1-4)
                new TestData(Mode.OnlyMinLines, 1, 0, 1, "Test is test1", false, true, 24),
                new TestData(Mode.OnlyMinLines, 2, 0, 1, "Test is test1", true, true, 16),
                new TestData(Mode.OnlyMinLines, 2, 0, 2, "Test is test1 Test is test2", true, true, 24),
                new TestData(Mode.OnlyMinLines, 2, 0, 3, "Test is test1 Test is test2 Test is test3", true, true, 24),

                //MinLines scenario: 2 Step tests (Case: 5-6)
                new TestData(Mode.OnlyMinLines, 2, 0, 1, "Test is test1", true, true, 24, true, 5, " Test is test2 Test is test3 Test is test4 Test is test5 EndTest", 1, 0),
                new TestData(Mode.OnlyMinLines, 2, 0, 3, "Test is test1 Test is test2 Test is test3", true, true, 24, true, 1, "", 2, 28),

                //Simple MaxLines scenario tests (Case: 7-10)
                new TestData(Mode.OnlyMaxLines, 0, 1, 1, "Test is test1", false, true, 32),
                new TestData(Mode.OnlyMaxLines, 0, 3, 2, "Test is test1 Test is test2", true, true, 24),
                new TestData(Mode.OnlyMaxLines, 0, 2, 2, "Test is test1 Test is test2 Test is test3", true, true, 24),
                new TestData(Mode.OnlyMaxLines, 0, 10, 2, "Test is test1 Test is test2", true, true, 24),

                //MinLines scenario: 2 Step tests (Case: 11-12)
                new TestData(Mode.OnlyMaxLines, 0, 3, 2, "Test is test1 Test is test2", true, true, 24, true, 3, " Test is test3 Test is test4 Test is test5", 1, 0),
                new TestData(Mode.OnlyMaxLines, 0, 2, 2, "Test is test1 Test is test2 Test is test3", true, true, 24, true, 1, "", 2, 28),

                //Simple MinLines & MaxLines scenario tests (Case: 13-17)
                new TestData(Mode.Both, 1, 1, 1, "Test is test1", false, true, 46),
                new TestData(Mode.Both, 3, 4, 1, "Test is test1", false, true, 18),
                new TestData(Mode.Both, 1, 1, 1, "Test is test1 Test is test2", true, true, 24),
                new TestData(Mode.Both, 2, 4, 3, "Test is test1 Test is test2 Test is test3", true, true, 24),
                new TestData(Mode.Both, 2, 4, 4, "Test is test1 Test is test2 Test is test3 Test is test4 Test is test5 Test is test6", true, true, 24),

                //MinLines & MaxLines scenario: 2 Step tests (Case: 18-20)
                new TestData(Mode.Both, 2, 4, 3, "Test is test1 Test is test2 Test is test3", true, true, 24, true, 4, " Test is test4 Test is test5 Test is test6", 1, 0),
                new TestData(Mode.Both, 2, 4, 3, "Test is test1 Test is test2 Test is test3", true, true, 24, true, 1, "", 2, 28),
                new TestData(Mode.Both, 2, 4, 4, "Test is test1 Test is test2 Test is test3 Test is test4 Test is test5 Test is test6", true, true, 24, true, 3, "", 2, 42),

                //MinLines, MaxLines, Height, MinHeight & MaxHeight conflicting scenarios. (Case: 21-29)
                new TestData(Mode.OnlyMinLines, 5, 0, 3, "Test is test1 Test is test2 Test is test3 Test is test4 Test is test5 Test is test6", false, true, 24, 0, 0, 90, "Arial"),
                new TestData(Mode.OnlyMinLines, 5, 0, 2, "Test is test1 Test is test2 Test is test3 Test is test4 Test is test5 Test is test6", false, true, 24, 60, 0, 0, "Arial"),
                new TestData(Mode.OnlyMinLines, 5, 0, 3, "Test is test1 Test is test2 Test is test3", true, true, 24, 0, 150, 0, "Arial"),

                new TestData(Mode.OnlyMaxLines, 0, 2, 3, "Test is test1 Test is test2 Test is test3 Test is test4 Test is test5 Test is test6", false, true, 24, 0, 0, 90, "Arial"),
                new TestData(Mode.OnlyMaxLines, 0, 2, 3, "Test is test1 Test is test2 Test is test3 Test is test4 Test is test5 Test is test6", false, true, 24, 90, 0, 0, "Arial"),
                new TestData(Mode.OnlyMaxLines, 0, 1, 3, "Test is test1 Test is test2 Test is test3", true, true, 24, 0, 150, 0, "Arial"),

                new TestData(Mode.Both, 5, 7, 3, "Test is test1 Test is test2 Test is test3 Test is test4 Test is test5 Test is test6", false, true, 24, 0, 0, 90, "Arial"),
                new TestData(Mode.Both, 5, 10, 2, "Test is test1 Test is test2 Test is test3 Test is test4 Test is test5 Test is test6", false, true, 24, 60, 0, 0, "Arial"),
                new TestData(Mode.Both, 5, 5, 3, "Test is test1 Test is test2 Test is test3", true, true, 24, 0, 150, 0, "Arial"),

                //Checking invalid values
            };
        }

        #endregion TestCaseData

        /// <summary>Current test data being used</summary>
        private TestData _testData;

        /// <summary>Current TestData index being tested.</summary>
        int _currentIndex = 0;
        int _endIndex;

        bool _testFailed = false;
        int[] _testCaseFailed = new int[TestData.TestCases.Length];

        bool _isStep2 = false;

        Canvas _canvasPanel;
        TextBox _testTB;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _endIndex = TestData.TestCases.Length;
            for (int i = 0; i < _testCaseFailed.Length; i++)
                _testCaseFailed[i] = 0;
            if (ConfigurationSettings.Current.GetArgumentAsInt("Repro") != 0)
            {
                _currentIndex = ConfigurationSettings.Current.GetArgumentAsInt("Repro");
                _endIndex = _currentIndex + 1;
            }
            RunCase();
        }

        /// <summary>Runs for each test in this test case</summary>
        private void RunCase()
        {
            if (_currentIndex < _endIndex)
            {
                _testData = TestData.TestCases[_currentIndex];
                Log("********** Running test case: " + _currentIndex + " **********");
                SetUpTestCase();
            }
            else
            {
                if (!_testFailed)
                    Logger.Current.ReportSuccess();
                else
                {
                    string failMessage = "The following test cases have failed: [";
                    for (int i = 0; i < _testCaseFailed.Length; i++)
                    {
                        if (_testCaseFailed[i] == -1)
                            failMessage += " " + i + ",";
                    }
                    failMessage += "]";
                    failMessage += "\nTo re-run just the failed test case, add the following command line to the existing one: /Repro:<test case number>";
                    Log(failMessage);
                    Logger.Current.ReportResult(false, "MaxMinLines test case has failed", false);
                }
            }
        }

        /// <summary>Sets up the test based on _testData.</summary>
        private void SetUpTestCase()
        {
            _canvasPanel = new Canvas();
            _testTB = new TextBox();

            _testTB.Width = 200;
            _testTB.BorderBrush = System.Windows.Media.Brushes.Transparent;
            _testTB.FontFamily = new System.Windows.Media.FontFamily(_testData.FontFamily);

            _testTB.FontSize = _testData.FontSize;
            switch (_testData.TestMode)
            {
                case Mode.None:
                    break;
                case Mode.OnlyMinLines:
                    _testTB.MinLines = _testData.MinLines;
                    break;
                case Mode.OnlyMaxLines:
                    _testTB.MaxLines = _testData.MaxLines;
                    break;
                case Mode.Both:
                    _testTB.MinLines = _testData.MinLines;
                    _testTB.MaxLines = _testData.MaxLines;
                    break;
                default:
                    break;
            }

            if (_testData.AcceptsReturn)
            {
                _testTB.AcceptsReturn = true;
            }
            if (_testData.Wrap)
            {
                _testTB.TextWrapping = TextWrapping.Wrap;
            }
            if (_testData.Height != 0)
            {
                _testTB.Height = _testData.Height;
            }
            if (_testData.MinHeight != 0)
            {
                _testTB.MinHeight = _testData.MinHeight;
            }
            if (_testData.MaxHeight != 0)
            {
                _testTB.MaxHeight = _testData.MaxHeight;
            }
            switch (ConfigurationSettings.Current.GetArgument("TextFormattingmode"))
            {
                case "Display": TextOptions.SetTextFormattingMode(_testTB, TextFormattingMode.Display);
                    break;
                case "Ideal": TextOptions.SetTextFormattingMode(_testTB, TextFormattingMode.Ideal);
                    break;
            }
            _testTB.Text = _testData.Text;           

            MainWindow.Content = _canvasPanel;
            _canvasPanel.Children.Add(_testTB);

            QueueDelegate(TestHeight);
        }

        private void TestHeight()
        {
            int expectedLineCount;
            int lineCount;
            string message, result;
            System.Drawing.Bitmap bitmapTB, fullBitmapTB;
            //The value of padding different between Win7 and Win8.             
            //So we adjust the value of overheadPixels from 7 to 5 in reasonable range.
            const int overheadPixels = 5; //This is a rough overhead in pixels which add up to the textbox's
            //height in addition FontSize*NumberOfLines.
            float xFactor, yFactor; //Dpi scaling factors

            //adjusted values for Dpi
            int adjustedFontSizeForDpi;
            double adjustedMaxHeightForDpi, adjustedMinHeightForDpi;
            double adjustedHeightForDpi, adjustedActualHeightForDpi;

            UIElementWrapper.HighDpiScaleFactors(out xFactor, out yFactor);
            adjustedFontSizeForDpi = (int)((float)_testData.FontSize * yFactor);
            adjustedMaxHeightForDpi = _testData.MaxHeight * yFactor;
            adjustedMinHeightForDpi = _testData.MinHeight * yFactor;
            adjustedHeightForDpi = _testData.Height * yFactor;
            adjustedActualHeightForDpi = _testTB.ActualHeight * yFactor;
            fullBitmapTB = BitmapCapture.CreateBitmapFromElement(_testTB);

            if (!_isStep2)
            {
                expectedLineCount = _testData.ExpLines;
            }
            else
            {
                expectedLineCount = _testData.ExpLinesStep2;
            }

            //Verify the line count
            bitmapTB = CountLines(_testTB, out lineCount);
            if (lineCount != expectedLineCount)
            {
                Logger.Current.LogImage(bitmapTB, "TestTBLineCount" + _currentIndex);
                result = " Fail: Lines in TextBox [" + lineCount + "]" +
                             " Expected Lines [" + expectedLineCount + "]";
                message = "TC#" + _currentIndex + result;
                Logger.Current.ReportResult(false, message, true);
                _testFailed = true;
                _testCaseFailed[_currentIndex] = -1;
            }

            //Verify that lineCount < MaxLines if MaxLines is set
            if ((_testData.TestMode == Mode.OnlyMaxLines) || (_testData.TestMode == Mode.Both))
            {
                //only when height, maxheight or minheight is not specified.
                if ((adjustedHeightForDpi == 0) && (adjustedMaxHeightForDpi == 0) && (adjustedMinHeightForDpi == 0))
                {
                    Log("Verifying CountLines <= MaxLines");

                    if (lineCount > _testData.MaxLines)
                    {
                        result = " Fail: Lines in TextBox [" + lineCount + "]" +
                                 " cant be more than MaxLines [" + _testData.MaxLines + "]";
                        message = "TC#" + _currentIndex + result;
                        Logger.Current.ReportResult(false, message, true);
                        _testFailed = true;
                        _testCaseFailed[_currentIndex] = -1;
                    }
                }
            }

            //Verify that MinLines is respected even if text requires less number of lines
            if ((_testData.TestMode == Mode.OnlyMinLines) || (_testData.TestMode == Mode.Both))
            {
                //only when height, maxheight or minheight is not specified.
                if ((adjustedHeightForDpi == 0) && (adjustedMaxHeightForDpi == 0) && (adjustedMinHeightForDpi == 0))
                {
                    if ((lineCount <= _testData.MinLines) && (adjustedHeightForDpi == 0))
                    {
                        //The following condition holds only when we are testing with less number (<=5)
                        //of lines inside the TextBox.
                        //( (MinLines*FontSize + overheadPixels) < Bitmap.Height < ((MinLines+1)*FontSize + overheadPixels) )

                        Log("Verifying that MinLines is respected even if text requires less number of lines");
                        if (!((((_testTB.MinLines * adjustedFontSizeForDpi) + overheadPixels) < fullBitmapTB.Height) &&
                               (fullBitmapTB.Height <= (((_testTB.MinLines + 1) * adjustedFontSizeForDpi)) + overheadPixels)))
                        {
                            result = " Fail: MinLines [" + _testTB.MinLines + "]" +
                                     " FontSize [" + _testData.FontSize + "]" +
                                     " AdjustedFontSizeForDpi [" + adjustedFontSizeForDpi + "]" +
                                     " BitmapHeight [" + fullBitmapTB.Height.ToString() + "]";
                            message = "TC#" + _currentIndex + result;
                            Logger.Current.ReportResult(false, message, true);
                            _testFailed = true;
                            _testCaseFailed[_currentIndex] = -1;
                        }
                    }
                }
            }

            //Verify that Height, MaxHeight and MinHeight override the MinLines and MaxLines values.
            if ((adjustedMinHeightForDpi != 0) || (adjustedMaxHeightForDpi != 0) || (adjustedHeightForDpi != 0))
            {
                if ((adjustedMinHeightForDpi != 0) && (fullBitmapTB.Height < (int)adjustedMinHeightForDpi))
                {
                    result = " Fail: MinHeight [" + _testData.MinHeight + "]" +
                             " AdjustedMinHeightForDpi [" + adjustedMinHeightForDpi + "]" +
                             " BitmapHeight [" + fullBitmapTB.Height.ToString() + "]";
                    message = "TC#" + _currentIndex + result;
                    Logger.Current.ReportResult(false, message, true);
                    _testFailed = true;
                    _testCaseFailed[_currentIndex] = -1;
                }

                if ((adjustedMaxHeightForDpi != 0) && (fullBitmapTB.Height > (int)adjustedMaxHeightForDpi))
                {
                    result = " Fail: MaxHeight [" + _testData.MaxHeight + "]" +
                             " AdjustedMaxHeightForDpi [" + adjustedMaxHeightForDpi + "]" +
                             " BitmapHeight [" + fullBitmapTB.Height.ToString() + "]";
                    message = "TC#" + _currentIndex + result;
                    Logger.Current.ReportResult(false, message, true);
                    _testFailed = true;
                    _testCaseFailed[_currentIndex] = -1;
                }

                if ((adjustedHeightForDpi != 0) && (fullBitmapTB.Height != (int)adjustedHeightForDpi))
                {
                    result = " Fail: Height [" + _testData.Height + "]" +
                             " AdjustedHeightForDpi [" + adjustedHeightForDpi + "]" +
                             " BitmapHeight [" + fullBitmapTB.Height.ToString() + "]";
                    message = "TC#" + _currentIndex + result;
                    Logger.Current.ReportResult(false, message, true);
                    _testFailed = true;
                    _testCaseFailed[_currentIndex] = -1;
                }
            }

            //Verify that ActualHeight and BitmapHeight are greater than lineCount*FontSize
            Log("Verifying ActualHeight and BitmapHeight > lineCount*(int)FontSize");
            if ((adjustedActualHeightForDpi <= (lineCount * adjustedFontSizeForDpi)) ||
                 (fullBitmapTB.Height <= (lineCount * adjustedFontSizeForDpi)))
            {
                result = " Fail: ActualHeight [" + _testTB.ActualHeight + "]" +
                         " AdjustedActualHeightForDpi [" + adjustedActualHeightForDpi + "]" +
                         " lineCount [" + lineCount + "]" +
                         " FontSize [" + _testData.FontSize + "]" +
                         " AdjustedFontSizeForDpi [" + adjustedFontSizeForDpi + "]";
                message = "TC#" + _currentIndex + result;
                Logger.Current.ReportResult(false, message, true);
                _testFailed = true;
                _testCaseFailed[_currentIndex] = -1;
            }

            Log("Bitmap Height          : " + fullBitmapTB.Height);
            Log("TextBox Computed Height: " + _testTB.ActualHeight.ToString());
            Log("TextBox Computed Height adjusted for Dpi: " + adjustedActualHeightForDpi.ToString());

            if (_testData.DoStep2)
            {
                if (_testData.NextStepType == 1)
                {
                    QueueDelegate(new SimpleHandler(AddStep));
                }
                else
                {
                    QueueDelegate(new SimpleHandler(DeleteStep));
                }
                _isStep2 = true;
            }
            else
            {
                _isStep2 = false;
                _currentIndex++;
                QueueDelegate(new SimpleHandler(RunCase));
            }
        }

        private void AddStep()
        {
            _testData.DoStep2 = false;
            Log("----- Performing Add Step -----");
            MouseInput.MouseClick(_testTB);
            KeyboardInput.TypeString("^{END}" + _testData.TextStep2);
            QueueDelegate(new SimpleHandler(TestHeight));
        }

        private void DeleteStep()
        {
            _testData.DoStep2 = false;
            Log("----- Performing Delete Step -----");
            MouseInput.MouseClick(_testTB);
            KeyboardInput.TypeString("^{END}" + "{BACKSPACE " + _testData.NumOfDel + "}");
            QueueDelegate(new SimpleHandler(TestHeight));
        }

        /// <summary>
        /// Counts the number of lines rendered on the specified TextBox.
        /// </summary>
        /// <param name="textBox">Control to count lines in.</param>
        /// <param name="lineCount">On return, number of lines counted.</param>
        /// <returns>The snapshot of the text box content.</returns>
        private System.Drawing.Bitmap CountLines(TextBox textBox, out int lineCount)
        {
            if (textBox == null)
            {
                throw new ArgumentNullException("textBox");
            }

            // Create a sub-bitmap to account for borders. Borders
            // are not calculated now, but the default should never be thicker
            // than two pixels!
            using (System.Drawing.Bitmap b = BitmapCapture.CreateBitmapFromElement(textBox))
            using (System.Drawing.Bitmap noBorders = BitmapUtils.CreateSubBitmap(
                    b, new System.Drawing.Rectangle(2, 2, b.Width - 4, b.Height - 4)))
            {
                System.Drawing.Bitmap noBordersBW = BitmapUtils.ColorToBlackWhite(noBorders);
                lineCount = BitmapUtils.CountTextLines(noBordersBW);
                return noBordersBW;
            }
        }
    }       
}