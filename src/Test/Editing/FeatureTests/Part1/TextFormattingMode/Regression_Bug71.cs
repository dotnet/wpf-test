// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Tests with to verify Regression_Bug71

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

    [Test(0, "TextBox", "Regression_Bug71", MethodParameters = "/TestCaseType:Regression_Bug71", Timeout = 500, Keywords = "TextFormattingModeTests")]
    public class Regression_Bug71 : ManagedCombinatorialTestCase
    {
        protected override void DoRunCombination()
        {
            _textbox = new TextBox();
            _textbox.Clear();
           switch (_textFormattingMode)
            {
                case "Ideal": TextOptions.SetTextFormattingMode(_textbox, TextFormattingMode.Ideal);
                    break;
                case "Display": TextOptions.SetTextFormattingMode(_textbox, TextFormattingMode.Display);
                    break;
            }

            _textbox.FontSize = _fontSize;
            _textbox.FontFamily = new System.Windows.Media.FontFamily(_fontFamily);
            _wrapper = new UIElementWrapper(_textbox);
            TestElement = _textbox;

            QueueDelegate(SetContentAndDoVerification);
        }

        private void SetContentAndDoVerification()
        {
            int position = 0;
            int lineCount = _textContent.Length;           
            _textbox.Focus();

            //Adding text to the textbox
            _textbox.Text = _textContent[0] + Environment.NewLine; 
            for (position = 1; position < lineCount; position++)
            {
                _textbox.Text = _textbox.Text + _textContent[position] + Environment.NewLine;  
            }
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(500);
            //Verify Text and length
            for (position = 0; position < lineCount; position++)
            {                
                VerifyLineTextAndLength(position);               
            }
            NextCombination();
        }

        private void VerifyLineTextAndLength(int index)   
        {
            string expectedText = string.Empty;
            string actualText = string.Empty;
            int expectedLength = 0;
            int actualLength = 0;

            actualText = _textbox.GetLineText(index).Trim();
            expectedText = _textContent[index];
            actualLength = _textbox.GetLineLength(index);
            expectedLength = expectedText.Length + 2;

            Verifier.Verify(actualText.Equals(expectedText), "Verifying that contents of TextBox at Line [" + index + "] are same. Actual  [" + actualText + "] Expected [" + expectedText + "]", true);
            Verifier.Verify(actualLength == expectedLength, "Verifying that line length of TextBox at Line [" + index + "] is same. Actual  [" + actualLength + "] Expected [" + expectedLength + "]", true);           
                 
        }
        #region Private fields

        private UIElementWrapper _wrapper = null;
        private TextBox _textbox;
        private string _textFormattingMode = string.Empty;
        private int _fontSize = 0;
        private string _fontFamily = string.Empty;
        private string[] _textContent = new string[] { "1", "2", "abc", "123abc", "AAAAAAAAAAAA", "AB12ef**12", "7", "8", "9", "10", "11", "12", "BBBBBBBBB", "HELLO WORLD", "aeiou", "HI" };

        #endregion
    }
}