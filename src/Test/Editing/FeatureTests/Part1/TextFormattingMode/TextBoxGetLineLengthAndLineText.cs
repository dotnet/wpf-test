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

    /// <summary> Input String Type </summary>
    enum InputStringData
    {
        Empty,
        LTR,
        RTL,
        MixedLTR_RTL,
        MixedRTL_LTR,
        MultiLine,
        Surrogate,
        InvalidTest,
    }

     /// <summary>Test TextBoxGetLineLength and GetLineText</summary>
    [Test(1, "TextBox", "TextBoxGetLineLengthAndLineText", MethodParameters = "/TestCaseType:TextBoxGetLineLengthAndLineText", Keywords = "TextFormattingModeTests")]
    public class TextBoxGetLineLengthAndLineText : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is TextBox)
            {
                _controlWrapper = new UIElementWrapper(_element);
                _tb = _element as TextBox;
                _tb.FontSize = 30;
                switch (_textFormattingMode)
                {
                    case "Ideal": TextOptions.SetTextFormattingMode(_tb, TextFormattingMode.Ideal);
                        break;
                    case "Display": TextOptions.SetTextFormattingMode(_tb, TextFormattingMode.Display);
                        break;
                }
                TestElement = _element;
                VerificationGetLineText(_tb.GetLineText(0), null);
                VerificationGetLineLength(_tb.GetLineLength(0), -1);
                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            _element.Focus();
            ExecuteTrigger();
        }

        /// <summary>Program Controller</summary>
        private void ExecuteTrigger()
        {
            switch (_inputStringDataSwitch)
            {
                case InputStringData.Empty:
                    _tb.Text = emptyText;
                    VerificationGetLineText(_tb.GetLineText(0), emptyText);
                    VerificationGetLineLength(_tb.GetLineLength(0), emptyText.Length);
                    break;

                case InputStringData.LTR:
                    _tb.Text = ltrText;
                    VerificationGetLineText(_tb.GetLineText(0), ltrText);
                    VerificationGetLineLength(_tb.GetLineLength(0), ltrText.Length);
                    break;

                case InputStringData.MixedLTR_RTL:
                    _tb.Text = ltrText + rtlText;
                    string temp = ltrText + rtlText;
                    VerificationGetLineText(_tb.GetLineText(0), temp);
                    VerificationGetLineLength(_tb.GetLineLength(0), temp.Length);
                    break;

                case InputStringData.MixedRTL_LTR:
                    _tb.Text = rtlText + ltrText;
                    temp = rtlText + ltrText;
                    VerificationGetLineText(_tb.GetLineText(0), temp);
                    VerificationGetLineLength(_tb.GetLineLength(0), temp.Length);
                    break;

                case InputStringData.MultiLine:
                    _tb.AcceptsReturn = true;
                    _tb.Text = ltrText + Environment.NewLine + secLineString;
                    VerificationGetLineText(_tb.GetLineText(1), secLineString);
                    VerificationGetLineLength(_tb.GetLineLength(1), secLineString.Length);
                    break;

                case InputStringData.RTL:
                    _tb.Text = rtlText;
                    VerificationGetLineText(_tb.GetLineText(0), rtlText);
                    VerificationGetLineLength(_tb.GetLineLength(0), rtlText.Length);
                    break;

                case InputStringData.Surrogate:
                    _tb.Text = surrogateText;
                    VerificationGetLineText(_tb.GetLineText(0), surrogateText);
                    VerificationGetLineLength(_tb.GetLineLength(0), surrogateText.Length);
                    break;

                case InputStringData.InvalidTest:
                    try
                    {
                        if (_count++ == 0)
                        {
                            VerificationGetLineText(_tb.GetLineText(-1), "dont care");
                        }
                        VerificationGetLineLength(_tb.GetLineLength(10), 0);
                        throw new ApplicationException("Expected exception when index of string is -1");
                    }
                    catch(ArgumentOutOfRangeException)
                    {
                        Log("Exception thrown as expected when index of string is -1");
                    }
                    break;

                default:
                    break;
            }
            NextCombination();
        }

        /// <summary>verification of GetLine</summary>
        private void VerificationGetLineText(string textboxText, string assignedText)
        {
            Verifier.Verify(textboxText == assignedText, "TextBox text is [" + textboxText +
                "] Assigned Text is [" + assignedText + "]", true);
        }

        /// <summary>Verification of GetText</summary>
        private void VerificationGetLineLength(int textboxTextLength, int assignedTextLength)
        {
            Verifier.Verify(textboxTextLength == assignedTextLength, "TextBox text Length is [" + textboxTextLength +
                "] Assigned Text Length is [" + assignedTextLength + "]", true);
        }

        #region data.

        private const string rtlText= "\x05d0\x05d1\x05ea\x05e9";
        private const string ltrText = "Abc";
        private const string surrogateText = "\x0302e\x0327\x0627\x0654\x0655"; //\x0302|e|\x0327|\x0627|\x0654|\x0655
        private const string emptyText = "";
        private const string secLineString = "defghi";
        private int _count = 0;

        private InputStringData _inputStringDataSwitch=0;
        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private TextBox _tb;
        private string _textFormattingMode = string.Empty;
        
        #endregion data.
    }       
}