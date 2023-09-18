// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Unit testing for Line API of TextBox class 

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Drawing;
    using System.Threading;    

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using LHPoint = System.Windows.Point;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

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
    [Test(0, "TextBox", "MaxMinLinesTest", MethodParameters = "/TestCaseType:MaxMinLinesTest")]
    [TestOwner("Microsoft"), TestTitle("MaxMinLinesTest"),
     TestBugs("575, 699, 701, 702"), TestTactics("585")]
    public class MaxMinLinesTest : CustomTestCase
    {
        #region TestCaseData

        internal enum Mode {None=0, OnlyMinLines=1, OnlyMaxLines=2, Both=3};

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
            bool _wrap;
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
            internal bool Wrap { get { return this._wrap; } }
            internal int FontSize { get { return this._fontSize; } }
            internal string FontFamily { get { return this._fontFamily; } }
            internal bool DoStep2 { get { return this._doStep2; }
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
                _testMode = testMode;
                _minLines = minLines;
                _maxLines = maxLines;
                _expLines = expLines;

                _text = text;
                _acceptsReturn = acceptsReturn;
                _wrap = wrap;
                _fontSize = fontSize;

                _height = 0;
                _minHeight = 0;
                _maxHeight = 0;
                _fontFamily = "Tahoma";

                _doStep2 = false;
                _nextStepType = 0;
                _expLinesStep2 = 0;
                _textStep2 = "";
                _numOfDel = 0;

            }

            internal TestData(Mode testMode, int minLines, int maxLines, int expLines,
                              string text, bool acceptsReturn, bool wrap, int fontSize,
                              bool doStep2, int expLinesStep2, string textStep2,
                              int nextStepType, int numOfDel)
            {
                _testMode = testMode;
                _minLines = minLines;
                _maxLines = maxLines;
                _expLines = expLines;

                _text = text;
                _acceptsReturn = acceptsReturn;
                _wrap = wrap;
                _fontSize = fontSize;

                _height = 0;
                _minHeight = 0;
                _maxHeight = 0;
                _fontFamily = "Tahoma";

                _expLinesStep2 = expLinesStep2;
                _doStep2 = doStep2;
                _textStep2 = textStep2;
                _nextStepType = nextStepType;
                _numOfDel = numOfDel;
            }

            internal TestData(Mode testMode, int minLines, int maxLines, int expLines,
                              string text, bool acceptsReturn, bool wrap, int fontSize,
                              double height, double minHeight, double maxHeight, string fontFamily)
            {
                _testMode = testMode;
                _minLines = minLines;
                _maxLines = maxLines;
                _expLines = expLines;

                _text = text;
                _acceptsReturn = acceptsReturn;
                _wrap = wrap;
                _fontSize = fontSize;

                _height = height;
                _minHeight = minHeight;
                _maxHeight = maxHeight;
                _fontFamily = fontFamily;

                _expLinesStep2 = 0;
                _doStep2 = false;
                _textStep2 = "";
                _nextStepType = 0;
                _numOfDel = 0;
            }

            internal static TestData[] TestCases = new TestData[] {
                //For testing
                //new TestData(Mode.Both, 1, 2, 1, "This is test1", false, true, 24),

                //Dont set MinLines and MaxLines (Case:0)
               // new TestData(Mode.None, 0, 0, 2, "Test is test1 Test is test2", true, true, 24),

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

        bool _isStep2=false;

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
            if (_testData.Height !=0)
            {
                _testTB.Height = _testData.Height;
            }
            if (_testData.MinHeight !=0)
            {
                _testTB.MinHeight = _testData.MinHeight;
            }
            if (_testData.MaxHeight != 0)
            {
                _testTB.MaxHeight = _testData.MaxHeight;
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
            Bitmap bitmapTB, fullBitmapTB;
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
            bitmapTB = TextBoxRenderTyping.CountLines(_testTB, out lineCount);
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
    }

    /// <summary>
    /// Test case to test the height of the textbox with different settings
    /// </summary>
    [Test(0, "TextBox", "TextBoxHeightTest", MethodParameters = "/TestCaseType:TextBoxHeightTest")]
    [TestOwner("Microsoft"), TestTitle("TextBoxHeightTest"),
    TestBugs("576,575,703"), TestTactics("584")]
    public class TextBoxHeightTest : CustomTestCase
    {
        StackPanel _fPanel;
        TextBox _testTB1,_testTB2,_testTB3,_testTB4,_testTB5;
        DockPanel _dPanel;
        TextBox _testTextBox;
        int _oldBitmapHeight,_newBitmapHeight;
        double _oldHeight,_newHeight;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _fPanel = new StackPanel();

            _testTB1 = new TextBox();
            _testTB1.Width = 150;
            _testTB1.TextWrapping = TextWrapping.Wrap;
            _testTB1.FontSize = 20;
            _testTB1.FontFamily = new System.Windows.Media.FontFamily("Tahoma");
            _testTB1.Text = "This is test1";

            _testTB2 = new TextBox();
            _testTB2.MinLines = 1;
            _testTB2.Width = 150;
            _testTB2.TextWrapping = TextWrapping.Wrap;
            _testTB2.FontSize = 20;
            _testTB2.FontFamily = new System.Windows.Media.FontFamily("Tahoma");
            _testTB2.Text = "This is test1";

            _testTB3 = new TextBox();
            _testTB3.MinLines = 1;
            _testTB3.MaxLines = 1;
            _testTB3.Width = 150;
            _testTB3.TextWrapping = TextWrapping.Wrap;
            _testTB3.FontSize = 20;
            _testTB3.FontFamily = new System.Windows.Media.FontFamily("Tahoma");
            _testTB3.Text = "This is test1";

            _testTB4 = new TextBox();
            _testTB4.MinLines = 1;
            _testTB4.MaxLines = 2;
            _testTB4.Width = 150;
            _testTB4.TextWrapping = TextWrapping.Wrap;
            _testTB4.FontSize = 20;
            _testTB4.FontFamily = new System.Windows.Media.FontFamily("Tahoma");
            _testTB4.Text = "This is test1";

            _testTB5 = new TextBox();
            _testTB5.MaxLines = 1;
            _testTB5.Width = 150;
            _testTB5.TextWrapping = TextWrapping.Wrap;
            _testTB5.FontSize = 20;
            _testTB5.FontFamily = new System.Windows.Media.FontFamily("Tahoma");
            _testTB5.Text = "This is test1";

            _fPanel.Children.Add(_testTB1);
            _fPanel.Children.Add(_testTB2);
            _fPanel.Children.Add(_testTB3);
            _fPanel.Children.Add(_testTB4);
            _fPanel.Children.Add(_testTB5);

            MainWindow.Content = _fPanel;
            MainWindow.Width = 750;

            QueueDelegate(new SimpleHandler(TestHeights));
        }

        /// <summary>Tests that all four text box have same height.</summary>
        private void TestHeights()
        {
            Bitmap fullBitmapTB;
            int[] heightTB = new int[5];

            Log("Testing that height of the textbox is the same with different kind of settings");

            fullBitmapTB = BitmapCapture.CreateBitmapFromElement(_testTB1);
            heightTB[0] = fullBitmapTB.Height;
            Log("Height of TextBox #1: [" + heightTB[0] + "]");

            fullBitmapTB = BitmapCapture.CreateBitmapFromElement(_testTB2);
            heightTB[1] = fullBitmapTB.Height;
            Log("Height of TextBox #2: [" + heightTB[1] + "]");

            fullBitmapTB = BitmapCapture.CreateBitmapFromElement(_testTB3);
            heightTB[2] = fullBitmapTB.Height;
            Log("Height of TextBox #3: [" + heightTB[2] + "]");

            fullBitmapTB = BitmapCapture.CreateBitmapFromElement(_testTB4);
            heightTB[3] = fullBitmapTB.Height;
            Log("Height of TextBox #4: [" + heightTB[3] + "]");

            fullBitmapTB = BitmapCapture.CreateBitmapFromElement(_testTB5);
            heightTB[4] = fullBitmapTB.Height;
            Log("Height of TextBox #5: [" + heightTB[4] + "]");

            Verifier.Verify(((heightTB[0] == heightTB[1]) && (heightTB[1] == heightTB[2]) && (heightTB[2] == heightTB[3]) && (heightTB[3] == heightTB[4])),
                  "Verifying that height of the textbox is consistent amond different settings",
                true);

            Log("Heights of the all 5 textboxes are the same. Regression_Bug576 didnt repro");

            TestRegression_Bug575();
        }

        private void TestRegression_Bug575()
        {
            Log("Veryfing Regression_Bug575");
            _dPanel = new DockPanel();
            _testTextBox = new TextBox();
            _testTextBox.MaxLines = 1;
            _testTextBox.FontSize = 32;
            _testTextBox.SetValue(DockPanel.DockProperty, Dock.Top);
            _testTextBox.AcceptsReturn = true;
            _dPanel.Children.Add(_testTextBox);

            MainWindow.Content = _dPanel;
            QueueDelegate(DoInputAction);
        }

        private void DoInputAction()
        {
            _oldHeight = _testTextBox.ActualHeight;
            _oldBitmapHeight = BitmapCapture.CreateBitmapFromElement(_testTextBox).Height;
            Log("Bitmap Height of TextBox before hitting Enter: [" + _oldBitmapHeight + "]");
            Log("Actual Height of TextBox before hitting Enter: [" + Math.Round(_oldHeight, 1) + "]");
            Log("Hitting Enter key");
            MouseInput.MouseClick(_testTextBox);
            KeyboardInput.TypeString("{ENTER}");
            QueueDelegate(TestHeightAfterEnter);
        }

        private void TestHeightAfterEnter()
        {
            _newBitmapHeight = BitmapCapture.CreateBitmapFromElement(_testTextBox).Height;
            _newHeight = _testTextBox.ActualHeight;
            Log("Bitmap Height of TextBox after hitting Enter: [" + _newBitmapHeight + "]");
            Log("Actual Height of TextBox after hitting Enter: [" + Math.Round(_newHeight, 1) + "]");
            Verifier.Verify(_oldBitmapHeight == _newBitmapHeight,
                "Checking that bitmap height of TextBox didnt change", true);
            Verifier.Verify(Math.Round(_oldHeight,1) == Math.Round(_newHeight,1),
                "Checking that actual height of TextBox didnt change", true);
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Verifies that the GetCharacterIndexFromPoint API behaves as expected.
    /// </summary>
    [Test(0, "TextBox", "TextBoxGetCharacterIndexFromPointTest", MethodParameters = "/TestCaseType=TextBoxGetCharacterIndexFromPointTest")]
    [TestOwner("Microsoft"), TestTactics("583"), TestBugs("632")]
    public class TextBoxGetCharacterIndexFromPointTest: CustomCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Gets the dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            HitTestPointData[] hitTestPoints;
            TextEditableType[] textEditableTypes;

            textEditableTypes =
                (ConfigurationSettings.Current.GetArgumentAsBool("ExhaustiveControls"))?
                TextEditableType.Values : TextEditableType.PlatformTypes;
            hitTestPoints = s_hitTestPointValues;

            return new Dimension[] {
                new Dimension("HitTestPoint", hitTestPoints),
                new Dimension("SnapToText", new object[] { true, false }),
                new Dimension("StringData", new object[] {StringData.LatinScriptData}),
                new Dimension("TextEditableType", textEditableTypes),
            };
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _hitTestPoint = (HitTestPointData)values["HitTestPoint"];
            _stringData = (StringData)values["StringData"];
            _editableType = (TextEditableType)values["TextEditableType"];
            _snapToText = (bool)values["SnapToText"];

            // Only accept TextBox and sub-instances, and ignore
            // long data for BVTs.
            return (typeof(TextBox).IsAssignableFrom(_editableType.Type)) &&
                (!_stringData.IsLong) &&
                (_hitTestPoint.Point == HitTestPoint.BeforeStart);
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            if (_textbox == null || _textbox.GetType() != _editableType.Type)
            {
                _textbox = (TextBox)_editableType.CreateInstance();
                _textbox.TextWrapping = TextWrapping.Wrap;
                TestElement = _textbox;
            }
            if (_textbox.Text != _stringData.Value)
            {
                _textbox.Text = _stringData.Value;
            }
            QueueDelegate(CheckPoint);
        }

        private void CheckPoint()
        {
            int characterIndex;     // Character index returned by API.
            string description;     // Result description.
            TextLayoutModel layout; // Model of character layout.
            LHPoint point;          // Point being hit-tested.
            int unitIndex;          // Unit index from model hit-test.

            layout = new TextLayoutModel(new UIElementWrapper(_textbox));
            layout.CaptureLayoutInformation();
            point = _hitTestPoint.GetPoint(layout);
            characterIndex = _textbox.GetCharacterIndexFromPoint(point, _snapToText);
            unitIndex = layout.GetUnitIndexFromPoint(point, _snapToText);
            
            description = "Hit test point: " + point + 
                "; character index: " + characterIndex + "; expected index: " + unitIndex;
            if (unitIndex != characterIndex)
            {
                Log("Text length: " + _textbox.Text.Length);
                Log(layout.DescribeModel());
                Log(description);
                throw new Exception(description);
            }

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields.

        private TextBox _textbox;

        private TextEditableType _editableType;
        private StringData _stringData;
        private bool _snapToText;
        private HitTestPointData _hitTestPoint;

        #endregion Private fields.

        #region Inner types.


        /// <summary>Provides interesting places to hit-test in a plain text document.</summary>
        private enum HitTestPoint
        {
            /// <summary>In RTL, to the left of the first character.</summary>
            BeforeStart,
            // /// <summary>In RTL, to the right of a line, within the general bounding block.</summary>
            // AtEndOfLineInBlock,
            // /// <summary>In RTL, to the right of a line, outside the general bounding block.</summary>
            // AtEndOfLineOutOfBlock,
            /// <summary>In RTL, to the right of the last character.</summary>
            AfterEnd,
            // /// <summary>Under the general bounding block but within it horizontally.</summary>
            // UnderBlockInBlock,
            // /// <summary>Under the general bounding block and outside of it horizontally.</summary>
            // UnderBlockOutOfBlock,
        }


        delegate LHPoint GetHitTestPointCallback(TextLayoutModel layout);

        class HitTestPointData
        {
            internal HitTestPointData(HitTestPoint point, GetHitTestPointCallback callback)
            {
                this._point = point;
                this._callback = callback;
            }

            internal LHPoint GetPoint(TextLayoutModel layout)
            {
                return this._callback(layout);
            }

            internal HitTestPoint Point
            {
                get { return _point; }
            }

            /// <summary>String representation of the class instance</summary>
            /// <returns>A string which represents the class instance</returns>
            public override string ToString()
            {
                return this.Point.ToString();
            }

            private HitTestPoint _point;
            private GetHitTestPointCallback _callback;
        }

        private static HitTestPointData[] s_hitTestPointValues = new HitTestPointData[] {
            new HitTestPointData(HitTestPoint.BeforeStart, GetPointForBeforeStart),
            new HitTestPointData(HitTestPoint.AfterEnd, GetPointForAfterEnd),
        };

        private static LHPoint GetPointForAfterEnd(TextLayoutModel layout)
        {
            LHPoint result;
            Rect lastRect;

            if (layout == null)
            {
                throw new ArgumentNullException("layout");
            }
            if (layout.Units.Count == 0)
            {
                throw new ArgumentException(
                    "TextLayoutModel should not have zero units.", "layout");
            }

            lastRect = layout.Units[layout.Units.Count-1].Rectangle;
            result = new LHPoint(lastRect.Right + 2, lastRect.Top + lastRect.Height / 2);
            return result;
        }

        private static LHPoint GetPointForBeforeStart(TextLayoutModel layout)
        {
            LHPoint result;
            Rect firstRect;

            if (layout == null)
            {
                throw new ArgumentNullException("layout");
            }
            if (layout.Units.Count == 0)
            {
                throw new ArgumentException(
                    "TextLayoutModel should not have zero units.", "layout");
            }

            firstRect = layout.Units[0].Rectangle;
            result = new LHPoint(firstRect.Left - 2, firstRect.Top + firstRect.Height / 2);
            return result;
        }

        #endregion Inner types.
    }


    /// <summary>Tests MinMaxLines in different containers </summary>
    [Test(2, "TextBox", "TextBoxMinMaxLinesIDifferentContainers", MethodParameters = "/TestCaseType:TextBoxMinMaxLinesIDifferentContainers")]
    [TestOwner("Microsoft"), TestTactics("582"), TestWorkItem("102")]
    public class TextBoxMinMaxLinesIDifferentContainers : CustomTestCase
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
                _tb = new TextBox();
                _tb.FontSize = 20;
                _tb.MinLines = 2;
                _tb.MaxLines = 4;
                _tb.AcceptsReturn = true;
                Log("\r\n******************** TEXTBOX WITHIN " + _p.GetType().ToString() + "******************************\r\n");
                _p.Children.Add(_tb);
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
            _tb.Text = _singleLineString;
            QueueDelegate(VerifyTextboxHeight);
        }

        /// <summary>Verifies the height of textbox</summary>
        private void VerifyTextboxHeight()
        {
            double _height = GetLineHeight();
            double _estimatedHeight=0;
            if ((_p is Grid)||(_p is DockPanel))
            {
                _estimatedHeight = _height * _tb.MaxLines;
            }
            else
            {
                _estimatedHeight = _height * _tb.MinLines;
            }
            Verifier.Verify((_estimatedHeight < (_tb.ViewportHeight + 2.0)) && (_estimatedHeight > (_tb.ViewportHeight - 2.0)),
                "EstimatedHeight [" + _estimatedHeight.ToString() + "] == Actual ViewPortHeight [" + _tb.ViewportHeight.ToString() + "] +-2", true);
            QueueDelegate(SetMultiLineText);
        }

        /// <summary>Sets multiline text</summary>
        private void SetMultiLineText()
        {
            _tb.Text = _multiLineString;
            QueueDelegate(VerifyTextboxHeightMultiLine);
        }

        /// <summary>Verifies the height and number of lines in textbox</summary>
        private void VerifyTextboxHeightMultiLine()
        {
            double _height = GetLineHeight();
            double _estimatedHeight = 0;
            _estimatedHeight = _height * _tb.MaxLines;
            Verifier.Verify((_estimatedHeight < (_tb.ViewportHeight + 2.0)) && (_estimatedHeight > (_tb.ViewportHeight - 2.0)),
                "EstimatedHeight [" + _estimatedHeight.ToString() + "] == Actual ViewPortHeight [" + _tb.ViewportHeight.ToString() + "] +-2", true);

            int _numberOfLines =  _tb.GetLastVisibleLineIndex() - _tb.GetFirstVisibleLineIndex() + 1; //BECAUSE ITS 0 BASED
            Verifier.Verify(_numberOfLines == _tb.MaxLines, "Estimated number of lines in view [" + _numberOfLines.ToString() + "] == Actual MaxLines [" +
                _tb.MaxLines.ToString() + "]", true);

            QueueDelegate(LoopDifferentContainers);
        }

        /// <summary>Gets LineHeight</summary>
        private double GetLineHeight()
        {
           System.Windows.Media.FontFamily fontFamily = _tb.FontFamily;
            double fontSize = _tb.FontSize;
            return fontFamily.LineSpacing * fontSize;
        }

        #region data.

        private Panel[] _panels = { new StackPanel(), new DockPanel(), new Grid(), new Canvas(), new WrapPanel()};
        private int _count =0;
        private TextBox _tb;
        private string _singleLineString = "Say Hello";
        private string _multiLineString = "Say 1\r\nSay 2\r\nSay 3\r\nSay 4\r\nSay 5\r\nSay 6\r\nSay 7\r\nSay 8\r\nSay 9\r\nSay 10\r\n";
        private Panel _p = null;

        #endregion data.
    }
}
