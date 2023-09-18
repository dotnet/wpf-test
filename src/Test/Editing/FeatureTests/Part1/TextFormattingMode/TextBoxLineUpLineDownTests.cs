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
    
    
    enum InputTrigger
    {
        Programmatical,
        // Keyboard,
    }
    /// <summary>Tests  Line Up and Line down Operation for DCR 42757 </summary>
    [Test(0, "TextBoxBase", "TextBoxLineUpLineDownTests", MethodParameters = "/TestCaseType:TextBoxLineUpLineDownTests", Keywords = "TextFormattingModeTests", Timeout = 480)]
    public class TextBoxLineUpLineDownTests : ManagedCombinatorialTestCase
    {
        /// <summary>Starts the combinatorial engine </summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);

            if (_element is TextBox)
            {
                _element.Height = 150;
                _element.Width = 200;
                ((TextBox)_element).FontFamily = new System.Windows.Media.FontFamily(_fontFamily);
                ((TextBox)_element).FontSize = _fontSize;
                switch (_textFormattingMode)
                {
                    case "Ideal": TextOptions.SetTextFormattingMode(_element, TextFormattingMode.Ideal);
                        break;
                    case "Display": TextOptions.SetTextFormattingMode(_element, TextFormattingMode.Display);
                        break;
                }

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
                ((TextBox)_element).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                TestElement = _element;
                QueueDelegate(ExecuteTrigger);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>Program Controller </summary>
        private void ExecuteTrigger()
        {
            _lineHeight = GetLineHeight();
            _element.Focus();
            KeyboardInput.TypeString("^{HOME}");

            //UNCOMMENT THE ENUM INPUTTRIGGER WHEN LINE SCROLLS BY LINE HEIGHT
            if (_inputSwitch == InputTrigger.Programmatical)
            {
                QueueDelegate(LineDownOperation);
            }
            else
            {
                QueueDelegate(LineDownOperationKeyBoard);
            }
        }

        /// <summary> Performs Line Down operation programmatically </summary>
        private void LineDownOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _yvalueForFirstString);
            ((TextBox)_element).LineDown();
            QueueDelegate(VerifyLineDown);
        }

        /// <summary> Performs Line Down operation through keyboard - click on scrollbar</summary>
        private void LineDownOperationKeyBoard()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _yvalueForFirstString);

            System.Windows.Point p = ActionItemWrapper.GetScreenRelativeOrigin(_element);
            p.X = p.X + _element.Width - 10;
            p.Y = p.Y + _element.Height - 10;
            MouseInput.MouseClick(p);
            QueueDelegate(VerifyLineDown);
        }

        /// <summary> Verifies Line Down Operation </summary>
        public void VerifyLineDown()
        {
            int currentOffset = (int)(((TextBoxBase)_element).VerticalOffset);
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            
            _finalNumber = _controlWrapper.GetIndexOfFirstStringInWindow(_yvalueForFirstString, out _yvalueForFirstString);
            if (_largeMultiLineContent)
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of LineDown call", true);
                Verifier.Verify((currentOffset <= (_lineHeight) + 1) && (currentOffset >= (_lineHeight) - 1), "Vertical Offset hasnt changed according to 1 line Down. Expected [" +
                    _lineHeight.ToString() + "+-1] Actual [" + ((TextBoxBase)_element).VerticalOffset.ToString() + "]", true);
                //1 LINE DOWN
                VerifyLineDownTBandRTB(1);
            }
            else
            {
                Verifier.Verify((int)(((TextBoxBase)_element).VerticalOffset) == 0, "Vertical Offset is NOT =0. Expected [0] Actual [" +
                    ((TextBoxBase)_element).VerticalOffset.ToString() + "]", true);
                //1 LINE DOWN
                VerifyLineDownTBandRTB(0);
            }
        }

        /// <summary>Verifies Line Numbers on Line Down operation </summary>
        /// <param name="num">this is the number of lines moved down</param>
        public void VerifyLineDownTBandRTB(int num)
        {
            int linesScrolled = (_finalNumber - _initialNumber);
            Verifier.Verify(_finalNumber == _initialNumber + num, _element.GetType().Name + " Scrolled down [" +
                    linesScrolled.ToString() + "] lines. Expected Count [" + num.ToString() + "] \r\n", true);
            LineUp();
        }

        /// <summary> Performs Line Up operation </summary>
        public void LineUp()
        {
            ((TextBox)_element).LineDown();
            ((TextBox)_element).LineDown();
            if (_inputSwitch == InputTrigger.Programmatical)
            {
                QueueDelegate(LineUpOperation);
            }
            else
            {
                QueueDelegate(LineUpOperationKeyBoard);
            }
        }

        /// <summary> Performs Line Up operation programmatically</summary>
        public void LineUpOperation()
        {
            _initialOffset = (int)((TextBox)_element).VerticalOffset;
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _yvalueForFirstString);
            ((TextBoxBase)_element).LineUp();
            QueueDelegate(VerifyLineUp);
        }

        /// <summary> Performs Line Up operation  keyboard</summary>
        public void LineUpOperationKeyBoard()
        {
            _initialOffset = (int)((TextBox)_element).VerticalOffset;
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _yvalueForFirstString);
            System.Windows.Point p = ActionItemWrapper.GetScreenRelativeOrigin(_element);
            p.X = p.X + _element.Width - 10;
            p.Y = p.Y + 10;
            MouseInput.MouseClick(p);
            QueueDelegate(VerifyLineUp);
        }

        /// <summary> Verifies Line Up Operation </summary>
        public void VerifyLineUp()
        {
            //Line moves down from the top. So we need to find the first string. so use default Y value
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            _finalNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _yvalueForFirstString);
            if (_largeMultiLineContent)
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of LineUp calls", true);

                int actualOffset = (int)(((TextBoxBase)_element).VerticalOffset);
                int diff = (_initialOffset - actualOffset);
                Verifier.Verify(((diff <= _lineHeight + 1) && (diff >= _lineHeight - 1)), "Vertical Offset is NOT equal to the # of LineUp invocations Expected [" +
                    _lineHeight.ToString() + "+-1] Actual[" + diff.ToString() + "]", true);
                //1 LINE up
                VerifyLineUpTBandRTB(1);
            }
            else
            {
                Verifier.Verify((int)(((TextBoxBase)_element).VerticalOffset) == 0, "Vertical Offset should be 0. single line content", true);
                //1 LINE UP
                VerifyLineUpTBandRTB(0);
            }
        }

        /// <summary>Verifies Line Numbers on Line Up operation </summary>
        public void VerifyLineUpTBandRTB(int num)
        {
            int linesScrolled = (_initialNumber - _finalNumber);
            Verifier.Verify(_finalNumber == _initialNumber - num, _element.GetType().Name + " Scrolled up [" +
                 linesScrolled.ToString() + "] lines. Expected Count [" + num.ToString() + "] \r\n", true);
            NextCombination();
        }

        #region Helper Functions

        /// <summary>Gets Line Height </summary>
        private int GetLineHeight()
        {
            System.Windows.Media.FontFamily fontFamily = (System.Windows.Media.FontFamily)_element.GetValue(TextElement.FontFamilyProperty);
            double fontSize = (double)_element.GetValue(TextElement.FontSizeProperty);

            int height = 0;

            if ((TextOptions.GetTextFormattingMode(_element)) == TextFormattingMode.Ideal)
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
        

        #endregion Helper Functions

        #region private data.

        private int _initialNumber;
        private System.Drawing.Bitmap _initialImage,_finalImage,_differenceImage;
        private int _initialOffset;
        private int _finalNumber;
        private bool _largeMultiLineContent = false;
        private int _lineHeight;
        private double _yvalueForFirstString;

        private InputTrigger _inputSwitch = 0;
        private int _fontSize = 0;
        private string _fontFamily = null;
        private FrameworkElement _element;
        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;
        private string _textFormattingMode = string.Empty;

        #endregion private data.
    }
}