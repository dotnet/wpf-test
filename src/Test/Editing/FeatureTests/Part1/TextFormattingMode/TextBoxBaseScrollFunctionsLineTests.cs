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


    enum FunctionName
    {
        LineDown,
        LineUp,
        LineLeft,
        LineRight,
    }
	
    /// <summary>
    /// Explicitly tests the following members:
    /// LineDown
    /// LineUp
    /// LineRight
    /// LineLeft
    /// 
    /// Also Implicitly tests:
    /// GetFirstVisibleIndex (TextBox) //used in helper class
    /// GetLastVisibleIndex (TextBox)  //used in helper class
    /// Horizontal/vertical offset
    /// </summary>
    [Test(0, "TextBoxBase", "TextBoxBaseScrollFunctionsLineTests", MethodParameters = "/TestCaseType:TextBoxBaseScrollFunctionsLineTests", Timeout = 500, Keywords = "TextFormattingModeTests")]
    public class TextBoxBaseScrollFunctionsLineTests : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        /// <summary> filter for combinations read</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType == TextEditableType.GetByName("PasswordBox"))
                return false;
            //RichTextBox doesnt have TextWrapping property. It always wraps. Hence 
            //we only test for Wrap==true
            if ((_editableType == TextEditableType.GetByName("RichTextBox")) && (!_wrapText))
                return false;
            return true;
        }

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            element = _editableType.CreateInstance();

            if (element is PasswordBox)
            {
                NextCombination();
            }
            else
            {
                _controlWrapper = new UIElementWrapper(element);
                if (element is RichTextBox)
                {
                    ((RichTextBox)element).Document.PageWidth = 600;
                }
                //setting the Control Properties
                element.Height = 150;
                element.Width = 200;
                ((TextBoxBase)element).FontFamily = new System.Windows.Media.FontFamily("Tahoma");
                ((TextBoxBase)element).FontSize = 11;
                switch (_textFormattingMode)
                {
                    case "Ideal": TextOptions.SetTextFormattingMode(element, TextFormattingMode.Ideal);
                        break;
                    case "Display": TextOptions.SetTextFormattingMode(element, TextFormattingMode.Display);
                        break;
                }
                _controlWrapper.Wrap = _wrapText ? true : false;
                ((TextBoxBase)element).AcceptsReturn = _acceptsReturn ? true : false;

                if (_largeMultiLineContent)
                {
                    string str = "";
                    for (int i = 0; i < 40; i++)
                    {
                        str += TextUtils.RepeatString(i.ToString() + "> sample data :P", 10) + "\r\n";
                    }
                    _controlWrapper.Text = str;
                }
                else if (_largeMultiLineContent == false)
                {
                    _controlWrapper.Text = "0>Sample data :)Sample data :)S a m p l e data";
                }

                if (_scrollVisible)
                {
                    ((TextBoxBase)element).HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    ((TextBoxBase)element).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }
                _verticalOffset = 0;//sets cursor to (0,0)

                TestElement = element;
                QueueDelegate(DoFocus);
            }
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            MouseInput.MouseClick(element);
            QueueDelegate(StartCase);
        }

        /// <summary>Focus on element</summary>
        private void StartCase()
        {
            _initialNumber = 0;
            _finalNumber = 0;
            element = (FrameworkElement)_controlWrapper.Element;
            if (element is RichTextBox)
            {
                ((TextBoxBase)element).Padding = new Thickness(0);
                ((TextBoxBase)element).Margin = new Thickness(0);
            }
            if (_largeMultiLineContent)
            {
                QueueDelegate(InitializeVerticalLineOffsetValue);
            }
            else if ((_controlWrapper.Wrap == false) && (_largeMultiLineContent == false))
            {
                KeyboardInput.TypeString("^{HOME}");
                QueueDelegate(InitializeHorizontalLineOffsetValue);
            }
            else
            {
                KeyboardInput.TypeString("^{HOME}");
                QueueDelegate(ExecuteScrollAction);
            }
        }

        /// <summary>Program controller</summary>
        public void ExecuteScrollAction()
        {
            switch (_functionSwitch)
            {
                case FunctionName.LineDown:
                    {
                        if (element is TextBox)
                        {
                            _verticalLineOffset = GetLineHeight(element);
                        }
                        QueueDelegate(LineDown);
                        break;
                    }

                case FunctionName.LineUp:
                    {
                        if (element is TextBox)
                        {
                            _verticalLineOffset = GetLineHeight(element);
                        }
                        QueueDelegate(LineUp);
                        break;
                    }

                case FunctionName.LineRight:
                    {
                        LineRight();
                        break;
                    }

                case FunctionName.LineLeft:
                    {
                        if (element is RichTextBox)
                        {
                            FlowDocument fd = ((RichTextBox)element).Document;
                            fd.ClearValue(FlowDocument.PageWidthProperty);
                        }
                        SetCaret();
                        QueueDelegate(LineLeft);
                        break;
                    }

                default:
                    break;
            }
        }

        /// <summary>Base for setting vertical line offset values</summary>
        public void InitializeVerticalLineOffsetValue()
        {
            _initialOffset = (int)((TextBoxBase)element).VerticalOffset;
            //included so that mouse clicks out of TEXTBOX
            //& prevent mouseClick on element when calculating vertical offset
            //current case does so because of racearoundconditions with queuedelegate
            ((TextBoxBase)element).LineDown();
            QueueDelegate(SetVerticalLineOffset);
        }

        /// <summary>Initialize vertical line offset values</summary>
        public void SetVerticalLineOffset()
        {
            int finalOffset = (int)((TextBoxBase)element).VerticalOffset;
            _verticalLineOffset = (int)(finalOffset - _initialOffset);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(InitializeHorizontalLineOffsetValue);
        }

        /// <summary>Base for seeting the horizontal line offset values</summary>
        public void InitializeHorizontalLineOffsetValue()
        {
            _initialOffset = (int)((TextBoxBase)element).HorizontalOffset;
            ((TextBoxBase)element).LineRight();
            QueueDelegate(SetHorizontalLineOffset);
        }

        /// <summary>Initialize the horizontal line offset values</summary>
        public void SetHorizontalLineOffset()
        {
            int finalOffset = (int)((TextBoxBase)element).HorizontalOffset;
            _horizontalLineOffset = (int)(finalOffset - _initialOffset);
            QueueDelegate(ExecuteScrollAction);
        }

        /// <summary> sets cursor </summary>
        public void LineDown()
        {
            //element.Focus();
            MouseInput.MouseClick(element);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(LineDownOperation);
        }

        /// <summary> Performs Line Down operation </summary>
        private void LineDownOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _yvalForFirstString);
            ((TextBoxBase)element).LineDown();
            QueueDelegate(VerifyLineDown);
        }

        /// <summary> Verifies Line Down Operation </summary>
        public void VerifyLineDown()
        {
            //lines move upwards. so for RTB it is necessary to skip the line that goes up
            //Hence PREV value of Y is used
            _finalImage = BitmapCapture.CreateBitmapFromElement(element);

            _finalNumber = _controlWrapper.GetIndexOfFirstStringInWindow(_yvalForFirstString, out _yvalForFirstString);
            if (_largeMultiLineContent)
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of 2 LineDown calls", false);
                Verifier.Verify((int)(((TextBoxBase)element).VerticalOffset) == (_verticalLineOffset), "Vertical Offset is NOT == the number of LineDown invocations Expected [" +
                    _verticalLineOffset.ToString() + "] actual[" + ((TextBoxBase)element).VerticalOffset.ToString() + "]", false);
                //1 LINE DOWN
                VerifyLineDownTBandRTB(1);
            }
            else
            {
                Verifier.Verify((int)(((TextBoxBase)element).VerticalOffset) == 0, "Vertical Offset is NOT == the number of LineDown invocations Expected [" +
                    _verticalLineOffset.ToString() + "] actual[" + ((TextBoxBase)element).VerticalOffset.ToString() + "]", false);
                //1 LINE DOWN
                VerifyLineDownTBandRTB(0);
            }
        }

        /// <summary>Verifies Line Numbers on Line Down operation </summary>
        /// <param name="num">this is the number of lines moved down</param>
        public void VerifyLineDownTBandRTB(int num)
        {
            int _linesScrolled = (_finalNumber - _initialNumber);
            if (_controlWrapper.Wrap == false)
            {
                //RichTextBox has padding inbetween lines and this can cause problems is determinig line numbers 
                Verifier.Verify(_finalNumber == _initialNumber + num, element.GetType().Name + " Scrolled down [" +
                        _linesScrolled.ToString() + "] lines. Expected Count [1] \r\n. Initial Line Number [" + _initialNumber.ToString() +
                        "] Final Line Number [" + _finalNumber.ToString() + "] \r\n", false);
            }
            else
            {   // if the text is scrollable (>1 line) then linedown is called 1 times...so it has to be <2
                //but for RTB the wrap makes it lose the margin resulting in 2 lines down
                Verifier.Verify(_finalNumber <= 2, element.GetType().Name +
                                " Scrolled down [" + _linesScrolled.ToString() + "] lines. Expected Count [1] \r\n", false);
            }
            NextCombination();
        }

        /// <summary> sets cursor </summary>
        public void LineUp()
        {
            KeyboardInput.TypeString("{PGDN}{PGDN}");
            QueueDelegate(LineUpOperation);
        }

        /// <summary> Performs Line Up operation </summary>
        public void LineUpOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _yvalForFirstString);
            _verticalOffset = ((TextBoxBase)element).VerticalOffset;
            ((TextBoxBase)element).LineUp();
            QueueDelegate(VerifyLineUp);
        }

        /// <summary> Verifies Line Up Operation </summary>
        public void VerifyLineUp()
        {
            //Line moves down from the top. So we need to find the first string. so use default Y value
            _finalImage = BitmapCapture.CreateBitmapFromElement(element);
            _finalNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _yvalForFirstString);
            if (_largeMultiLineContent)
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of 2 LineUp calls", false);

                int _actualOffset = (int)(((TextBoxBase)element).VerticalOffset);
                int initialOffset = (int)(_verticalOffset);
                Log("current offset +" + _actualOffset.ToString() + " iNITIAL OFFSET=" + initialOffset.ToString() + "LINE HEIGHT=" + _verticalLineOffset.ToString());
                if (element is TextBox)
                {
                    int CurrentLineHeight = initialOffset - _actualOffset;
                    int calculatedLineHeight = (int)(_verticalLineOffset);
                    Verifier.Verify(calculatedLineHeight <= CurrentLineHeight + 1 || calculatedLineHeight >= CurrentLineHeight - 1,
                        "Vertical Offset is NOT equal to the # of LineUp invocations Expected[" + CurrentLineHeight.ToString() +
                        "+-1] Actual [" + calculatedLineHeight.ToString() + "+-1]", false);
                }
                else
                {
                    Verifier.Verify(_actualOffset == (initialOffset - (int)(_verticalLineOffset)), "Vertical Offset is NOT equal to the # of LineUp invocations", false);
                }

                //1 LINE up
                VerifyLineUpTBandRTB(1);
            }
            else
            {
                Verifier.Verify((int)(((TextBoxBase)element).VerticalOffset) == 0, "Vertical Offset is twice the number of LineDown invocations", false);
                //1 LINE UP
                VerifyLineUpTBandRTB(0);
            }
        }

        /// <summary>Verifies Line Numbers on Line Up operation </summary>
        public void VerifyLineUpTBandRTB(int num)
        {
            int _linesScrolled = (_initialNumber - _finalNumber);
            if (_controlWrapper.Wrap == false)
            {
                Verifier.Verify(_finalNumber == _initialNumber - num, element.GetType().Name + " Scrolled up [" +
                    _linesScrolled.ToString() + "] lines. Expected Count [2] \r\n", false);
            }
            else
            {
                //if the text is long and occupies more than 2 lines
                if (element is TextBox)
                {
                    Verifier.Verify(((_finalNumber == 0) || (_finalNumber == _initialNumber - num)), element.GetType().Name + " Scrolled up [" + _linesScrolled.ToString() + "] lines. Expected Count [0 / 1]  \r\n", false);
                }
                else if (element is RichTextBox)
                {
                    //the aditional line for RTB is because of wrap
                    //normally when lineUp is performed margin is considered too....
                    //because of wrap this margin is lost.. so oneline up results in 2 lines up
                    Verifier.Verify(((_finalNumber - _initialNumber == 0) || (_linesScrolled <= 2)), element.GetType().Name + " Scrolled up [" + _linesScrolled.ToString() + "] lines. Expected Count [0 / 2]  \r\n", false);
                }
            }
            NextCombination();
        }

        /// <summary> sets cursor </summary>
        public void LineRight()
        {
            KeyboardInput.TypeString("^{HOME}");
            ((TextBoxBase)element).ScrollToHorizontalOffset(0); //just to be sure
            QueueDelegate(LineRightOperation);
        }

        /// <summary> Performs Line Right operation </summary>
        public void LineRightOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(element);
            _initialString = _controlWrapper.GetFirstStringInWindow(0, out _yvalForFirstString);
            _initialOffset = ((TextBoxBase)element).HorizontalOffset;
            ((TextBoxBase)element).LineRight();
            ((TextBoxBase)element).LineRight();
            ((TextBoxBase)element).LineRight();

            QueueDelegate(VerifyLineRight);
        }

        /// <summary> Verifies Line Right Operation </summary>
        private void VerifyLineRight()
        {
            _finalImage = BitmapCapture.CreateBitmapFromElement(element);
            // 0 is passed because u want the first string
            _finalString = _controlWrapper.GetFirstStringInWindow(0, out _yvalForFirstString);
            _finalOffset = ((TextBoxBase)element).HorizontalOffset;

            if ((_controlWrapper.Wrap == false) || (element is RichTextBox))
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false,
                                "Images are same. They should be diff because of LineRight calls", false);

                Verifier.Verify(((int)(_finalOffset - _initialOffset) == ((int)(_horizontalLineOffset) * 3)),
                                "Horizontal Offset is NOT equal to the # of LineRight invocations", false);
            }
            else if (_controlWrapper.Wrap == true)
            {
                // this is unstable               
                //  Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(initialImage, finalImage, out differenceImage) == false, "Images are different. They should be same because theres no text wrap", false);
                Verifier.Verify(_finalOffset == 0, "Offset should be 0. cant move right", false);
            }
            VerifyLineRightTBandRTB();
        }

        /// <summary>Verifies Line Numbers on Line Right operation </summary>
        public void VerifyLineRightTBandRTB()
        {
            if (element is TextBox)
            {
                //equals operation because GetFirstStringInWindow returns complete string for textbox, NOT from pointer
                Verifier.Verify(_initialString.Equals(_finalString) == true, "ScrollViewer didnt retain focus on the string. Must have moved wrongly", false);
            }
            else if (element is RichTextBox)
            {
                Verifier.Verify(_initialString.Length > _finalString.Length, "ScrollViewer didnt move right as expected", false);
            }
            NextCombination();
        }

        /// <summary> sets cursor </summary>
        public void LineLeft()
        {
            KeyboardInput.TypeString("{END}");
            QueueDelegate(LineLeftOperation);
        }

        /// <summary> Performs Line Left operation </summary>
        private void LineLeftOperation()
        {
            _initialOffset = ((TextBoxBase)element).HorizontalOffset;
            _initialImage = BitmapCapture.CreateBitmapFromElement(element);
            _initialString = _controlWrapper.GetFirstStringInWindow(0, out _yvalForFirstString);

            ((TextBoxBase)element).LineLeft();
            ((TextBoxBase)element).LineLeft();
            QueueDelegate(VerifyLineLeft);
        }

        /// <summary> Verifies Line Left Operation </summary>
        private void VerifyLineLeft()
        {
            _finalImage = BitmapCapture.CreateBitmapFromElement(element);
            // 0 is passed because u want the first string
            _finalString = _controlWrapper.GetFirstStringInWindow(0, out _yvalForFirstString);
            _finalOffset = ((TextBoxBase)element).HorizontalOffset;

            if ((_controlWrapper.Wrap == false) && (element is TextBox))
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of LineLeft calls", false);

                Verifier.Verify(((int)(_initialOffset) - (int)(_finalOffset) == ((int)(_horizontalLineOffset) * 2)), "Horizontal Offset is NOT equal to the # of LineLeft invocations", false);
            }
            else
            {
                Verifier.Verify(_finalOffset == 0, "Offset should be 0. cant move left", false);
            }

            VerifyLineLeftTBandRTB();
        }

        /// <summary>Verifies Line Numbers on Line Left operation </summary>
        public void VerifyLineLeftTBandRTB()
        {
            if (element is TextBox)
            {
                Verifier.Verify(_initialString.Equals(_finalString) == true, "ScrollViewer didnt retain focus on the string. Must have moved wrongly", false);
            }
            else if (element is RichTextBox)
            {
                Verifier.Verify(_initialString.Length == _finalString.Length, "ScrollViewer sholdnt move right since text wraps", false);
            }
            NextCombination();
        }

#region Helper Functions

        /// <summary>Gets Line Height </summary>
        private int GetLineHeight(FrameworkElement element)
        {
            System.Windows.Media.FontFamily fontFamily = (System.Windows.Media.FontFamily)element.GetValue(TextElement.FontFamilyProperty);
            double fontSize = (double)element.GetValue(TextElement.FontSizeProperty);
            int height = 0;

            if ((TextOptions.GetTextFormattingMode(element)) == TextFormattingMode.Ideal)
            {
                height = (int)(fontFamily.LineSpacing * fontSize);
            }
            else
            {
#if TESTBUILD_NET_ATLEAST_462
                double pixelsPerDip = VisualTreeHelper.GetDpi(element).PixelsPerDip;
                object objHeight = ReflectionUtils.InvokeInstanceMethod((object)fontFamily, "GetLineSpacingForDisplayMode", new object[] { fontSize, pixelsPerDip });
#else
                object objHeight = ReflectionUtils.InvokeInstanceMethod((object)fontFamily, "GetLineSpacingForDisplayMode", new object[] { fontSize });
#endif

                height = (int)((double)objHeight);
            }
            return height;
        }

        /// <summary>Sets the caret to (0,0) </summary>
        public void SetCaret()
        {
            if (element is TextBox)
            {
                ((TextBox)element).Select(0, 0);
            }
            else
            {
                ((RichTextBox)element).Selection.Select(_controlWrapper.SelectionInstance.Start, _controlWrapper.SelectionInstance.Start);
            }
        }

#endregion Helper Functions

#region private data.

        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;
        private bool _acceptsReturn = false;
        private bool _largeMultiLineContent = false;
        private bool _wrapText = false;
        private bool _scrollVisible = true;
        private double _verticalOffset;
        private string _textFormattingMode = string.Empty;
        private System.Drawing.Bitmap _initialImage,_finalImage,_differenceImage;

        private double _yvalForFirstString;
        private FunctionName _functionSwitch = 0;

        private int _initialNumber;
        private int _finalNumber;

        private string _initialString = null;
        private string _finalString = null;

        private double _initialOffset;
        private double _finalOffset;

        private double _horizontalLineOffset;
        private double _verticalLineOffset;

        private FrameworkElement element;

#endregion private data.
    }
}