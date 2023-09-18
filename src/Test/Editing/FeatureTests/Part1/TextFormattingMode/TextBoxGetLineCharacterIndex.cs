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

    /// <summary> Locale Info </summary>
    enum InputStringChoices
    {
        Single,
        Multi,
    }

    /// <summary> API called </summary>
    enum FunctionChoices
    {
        GetCharIndexFromLineIndex,
        GetLineIndexFromCharIndex,
        InvalidCalls,
    }

    enum InputStringDataChoices
    {
        Empty,
        Multi,
    }
    
    /// <summary> Tests for GetLineIndexFromCharIndex and GetCharIndexFromLineIndex </summary>
    [Test(0, "TextBox", "TextBoxGetLineCharacterIndex", MethodParameters = "/TestCaseType=TextBoxGetLineCharacterIndex", Keywords = "TextFormattingModeTests")]
    public class TextBoxGetLineCharacterIndex : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is TextBox)
            {
                InitializeTextBox();
                Verifier.Verify(_textBox.GetLineIndexFromCharacterIndex(-1) == -1, "GetLineIndexFromCharacterIndex(-1) before attaching to tree returns -1", true);
                Verifier.Verify(_textBox.GetCharacterIndexFromLineIndex(-1) == -1, "GetCharacterIndexFromLineIndex(-1) before attaching to tree returns -1", true);
                TestElement = _element;
                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>focuses on the control element</summary>
        private void DoFocus()
        {
            _element.Focus();
            ProgramController();
        }

        /// <summary>ProgramController</summary>
        private void ProgramController()
        {
            switch (_functionNameSwitch)
            {
                case FunctionChoices.GetCharIndexFromLineIndex:
                    VerifyGetCharIndexFromLineIndex();
                    break;

                case FunctionChoices.GetLineIndexFromCharIndex:
                    VerifyGetLineIndexFromCharIndex();
                    break;

                case FunctionChoices.InvalidCalls:
                    VerifyInvalidCalls();
                    break;

                default:
                    break;
            }
            NextCombination();
        }

        /// <summary>VerifyGetCharIndexFromLineIndex</summary>
        private void VerifyGetCharIndexFromLineIndex()
        {
            int lineIndices = (_inputStringSwitch == InputStringDataChoices.Multi) ? 1 : 0;
            for (int i = 0; i <= lineIndices; i++)
            {
                int charIndex = _textBox.GetCharacterIndexFromLineIndex(i);
                int expectedIndex = (i == 0) ? 0 : 4; //AB->2 \R\N->2 
                Verifier.Verify(charIndex == expectedIndex, "GetCharIndexFromLineIndex for Line [" + i.ToString() +
                    "] is Actual [" + charIndex.ToString() + "] Expected [" + expectedIndex.ToString() + "]", true);
            }
        }

        /// <summary>VerifyGetLineIndexFromCharIndex</summary>
        private void VerifyGetLineIndexFromCharIndex()
        {
            int charIndices = (_inputStringSwitch == InputStringDataChoices.Multi) ? (firstLine.Length + 2 + secLine.Length + 1) : 0; //+1 for EOD symbol
            for (int i = 0; i < charIndices; i++)
            {
                int lineIndex = _textBox.GetLineIndexFromCharacterIndex(i);
                int expectedIndex;
                // Behavior change in 4.0 (Look at TFS Part1 Regression_Bug72)
                // In 4.0 with TextBox content [ab\r\ncd], GetLineLindexFromCharacterIndex() returns 2 starting from char index 3
                // In 3.0/3.5 with TextBox content [ab\r\ncd], GetLineLindexFromCharacterIndex() returns 2 starting from char index 2
                expectedIndex = (i <= (firstLine.Length + 1)) ? 0 : 1;
                Verifier.Verify(lineIndex == expectedIndex, "GetLineIndexFromCharacterIndex for CharIndex [" + i.ToString() +
                    "] is Actual [" + lineIndex.ToString() + "] Expected [" + expectedIndex.ToString() + "]", true);
            }
        }

        /// <summary>VerifyInvalidCalls</summary>
        private void VerifyInvalidCalls()
        {
            try
            {
                _textBox.GetLineIndexFromCharacterIndex(-1);
                throw new ApplicationException("GetLineIndexFromCharacterIndex(-1) Doesnt throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("GetLineIndexFromCharacterIndex(-1) throws ArgumentOutOfRangeException as Expected");
            }
            try
            {
                _textBox.GetLineIndexFromCharacterIndex(firstLine.Length + 2 + secLine.Length + 2);
                throw new ApplicationException("GetLineIndexFromCharacterIndex(7) Doesnt throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("GetLineIndexFromCharacterIndex(-1) throws ArgumentOutOfRangeException as Expected");
            }
            try
            {
                _textBox.GetCharacterIndexFromLineIndex(-1);
                throw new ApplicationException("GetCharacterIndexFromLineIndex(-1) Doesnt throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("GetCharacterIndexFromLineIndex(-1) throws ArgumentOutOfRangeException as Expected");
            }
            try
            {
                _textBox.GetCharacterIndexFromLineIndex(2);
                throw new ApplicationException("GetCharacterIndexFromLineIndex(2) Doesnt throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("GetCharacterIndexFromLineIndex(2) throws ArgumentOutOfRangeException as Expected");
            }
        }

        #region Helpers.

        /// <summary>Initialize TextBox</summary>
        private void InitializeTextBox()
        {
            _textBox = _element as TextBox;
            _textBox.FontWeight = FontWeights.Bold;
            _textBox.FontSize = 40;
            _textBox.FlowDirection = (_flowDirectionProperty == true) ? (FlowDirection.LeftToRight) : (FlowDirection.RightToLeft);
            _textBox.Text = (_inputStringSwitch == InputStringDataChoices.Multi) ? (firstLine + Environment.NewLine + secLine) : "";
            switch (_textFormattingMode)
            {
                case "Ideal": TextOptions.SetTextFormattingMode(_textBox, TextFormattingMode.Ideal);
                    break;
                case "Display": TextOptions.SetTextFormattingMode(_textBox, TextFormattingMode.Display);
                    break;
            }
            _controlWrapper = new UIElementWrapper(_element);

        }

        #endregion Helpers.

        #region data.

        private TextBox _textBox;
        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private InputStringDataChoices _inputStringSwitch = 0;
        private FunctionChoices _functionNameSwitch = 0;

        private bool _flowDirectionProperty = false; //true == LTR false == RTL
        private const string firstLine = "ab";
        private const string secLine = "cd";
        private string _textFormattingMode = string.Empty;
        #endregion data.
    }       
}
