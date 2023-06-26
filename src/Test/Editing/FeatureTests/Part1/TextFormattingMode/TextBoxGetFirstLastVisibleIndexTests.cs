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

    /// <summary>Tests GetFirLastVisibleLineIndex in TextBox </summary>
    [Test(0, "TextBox", "TextBoxGetFirstLastVisibleIndexTests", MethodParameters = "/TestCaseType:TextBoxGetFirstLastVisibleIndexTests", Keywords = "TextFormattingModeTests")]
    public class TextBoxGetFirstLastVisibleIndexTests : ManagedCombinatorialTestCase
    {
        /// <summary>Starts the combinatorial engine </summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);
            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version;

            if (_element is TextBox)
            {
                _controlWrapper = new UIElementWrapper(_element);
                _tb = _element as TextBox;
                _tb.FontSize = 20;
                _tb.FontWeight = FontWeights.Bold;

                //There are a little different height between win7 and win8 when after pagedown,
                //so reduce 4 pixels on height on win8.                
                if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1))
                {
                    _tb.Height = 116;
                }
                else
                {
                    _tb.Height = 120;
                }
                _tb.Width = 100;
                _tb.FontFamily = new System.Windows.Media.FontFamily("TAHOMA");
                _tb.TextWrapping = _textWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
                _tb.Text = _multiLine ? GetText() : "This is a textbox Not a RichTextBox";
                switch (_textFormattingMode)
                {
                    case "Ideal": TextOptions.SetTextFormattingMode(_tb, TextFormattingMode.Ideal);
                        break;
                    case "Display": TextOptions.SetTextFormattingMode(_tb, TextFormattingMode.Display);
                        break;
                }
                TestElement = _element;
                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        private void DoFocus()
        {
            _element.Focus();
            QueueDelegate(InitialVerification);
        }

        /// <summary>Program Controller </summary>
        private void InitialVerification()
        {
            System.Drawing.Bitmap bmp1 = BitmapCapture.CreateBitmapFromElement(_element);
            _numberOfLines = BitmapUtils.CountTextLines(bmp1);
            VerifyLineIndex(_numberOfLines);
            QueueDelegate(DoPageDown);
        }

        private void VerifyLineIndex(int numberOfLines)
        {
            Verifier.Verify(numberOfLines == (_tb.GetLastVisibleLineIndex() + 1), "LastVisible Line Index Expected [" +
                numberOfLines.ToString() + "] Actual [" + _tb.GetLastVisibleLineIndex() + "] + 1 (since its 0 based)", true);
            Verifier.Verify(0 == _tb.GetFirstVisibleLineIndex(), "FirstVisible Line Index Expected [0" +
                 "] Actual [" + _tb.GetFirstVisibleLineIndex() + "] ", true);
            _firstPageLastLine = _tb.GetLastVisibleLineIndex();
        }

        private void DoPageDown()
        {
            _tb.PageDown();
            QueueDelegate(VerifyPageDown);
        }

        private void VerifyPageDown()
        {
            System.Drawing.Bitmap bmp1 = BitmapCapture.CreateBitmapFromElement(_element);
            int numberOfLinesAfterPageDown = BitmapUtils.CountTextLines(bmp1);
            Logger.Current.LogImage(bmp1, "bmp");
            Log("NumberOfLines =[" + _numberOfLines.ToString() + "] After PageDown [" + numberOfLinesAfterPageDown.ToString() + "]");
            if (_multiLine == true)
            {
                int lastLineNumber = (_firstPageLastLine == _tb.GetFirstVisibleLineIndex()) ? (_numberOfLines * 2 - 1) : (_numberOfLines * 2); //-1 because last overlapping line becomes first line on page down

                lastLineNumber = (numberOfLinesAfterPageDown >= _numberOfLines + 1) ? (lastLineNumber + 1) : lastLineNumber;
                //Increment the line number by 1 because if the line overlaps then the second page will have 6 lines and hence (numberOfLines * 2 - 1 wont be correct.. needs to be incremented
                Verifier.Verify(lastLineNumber == (_tb.GetLastVisibleLineIndex() + 1), "--LastVisible Line Index Expected [" +
                    lastLineNumber.ToString() + "] Actual [" + _tb.GetLastVisibleLineIndex() + "] + 1 (since its 0 based)", true);
                Verifier.Verify(numberOfLinesAfterPageDown >= (_tb.GetFirstVisibleLineIndex() + 1), "#of lines Expected >=[" +
                    _numberOfLines.ToString() + "] Actual [" + numberOfLinesAfterPageDown.ToString() + "] ", true);
            }
            else
            {
                VerifyLineIndex(_numberOfLines);
            }
            NextCombination();
        }

        private string GetText()
        {
            string str = "";
            for (int i = 0; i < 10; i++)
            {
                str += i.ToString() + "> " + TextUtils.RepeatString("lllll dltk ", 3) + "\r\n";
            }
            return str;
        }

        private int _firstPageLastLine = 0;
        private FrameworkElement _element;
        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;
        private int _numberOfLines = 0;
        private bool _textWrap = false;
        private bool _multiLine = false;
        private TextBox _tb = null;
        private string _textFormattingMode = string.Empty;        
    }      
}