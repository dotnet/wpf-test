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


    /// <summary> Choices for the Scroll calls in TextBoxBase</summary>
    enum TextBoxScrollFunction
    {
        Start,
        End,
        InvalidCalls,
    }

    /// <summary>
    /// Explicitly tests the following members:
    /// ScrollToLine (start and end)
    /// Also Implicitly tests:
    /// GetFirstVisibleIndex (TextBox) //used in helper class
    /// GetLastVisibleIndex (TextBox)  //used in helper class
    /// Horizontal/vertical offset
    /// </summary>
    [Test(0, "TextBox", "TextBoxScrollFunctionTests", MethodParameters = "/TestCaseType:TextBoxScrollFunctionTests", Timeout = 480, Keywords = "TextFormattingModeTests")]
    public class TextBoxScrollFunctionTests : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();

            if (_element is TextBox)
            {

                _controlWrapper = new UIElementWrapper(_element);

                //setting the Control Properties
                _element.Height = 150;
                _element.Width = 200;
                ((TextBoxBase)_element).FontFamily = new System.Windows.Media.FontFamily("Tahoma");
                ((TextBoxBase)_element).FontSize = 11;
                switch (_textFormattingMode)
                {
                    case "Ideal": TextOptions.SetTextFormattingMode(_element, TextFormattingMode.Ideal);
                        break;
                    case "Display": TextOptions.SetTextFormattingMode(_element, TextFormattingMode.Display);
                        break;
                }
                _controlWrapper.Wrap = _wrapText ? true : false;
                ((TextBoxBase)_element).AcceptsReturn = _acceptsReturn ? true : false;

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
                    ((TextBoxBase)_element).HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    ((TextBoxBase)_element).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }

                TestElement = _element;
                QueueDelegate(TestScrollAction);
            }
            else
            {
                NextCombination();
            }
        }


        /// <summary>Program Controller </summary>
        private void TestScrollAction()
        {
            switch (_textBoxScrollSwitch)
            {
                case TextBoxScrollFunction.Start:
                    TestScrollToLineStart();
                    break;

                case TextBoxScrollFunction.End:
                    ((TextBox)_element).ScrollToVerticalOffset(0);
                    ((TextBox)_element).ScrollToHorizontalOffset(0);
                    QueueDelegate(TestScrollToLineEnd);
                    break;


                case TextBoxScrollFunction.InvalidCalls:
                    TestInvalidCalls();
                    break;

                default:
                    break;
            }
        }

        /// <summary>Scroll to line start operation </summary>
        private void TestScrollToLineStart()
        {
            ((TextBox)_element).ScrollToLine(0);
            VerifyTestScrollToLineStart();
        }

        /// <summary>Verify Scroll to line start operation </summary>
        private void VerifyTestScrollToLineStart()
        {
            int startIndex = ((TextBox)_element).GetFirstVisibleLineIndex();
            Verifier.Verify(startIndex == 0, "Didnt scroll to first line", false);
            NextCombination();
        }

        /// <summary>Scroll to line end operation </summary>
        private void TestScrollToLineEnd()
        {
            ((TextBox)_element).ScrollToLine(40);
            QueueDelegate(VerifyTestScrollToLineEnd);
        }

        /// <summary>Verify Scroll to line start operation </summary>
        private void VerifyTestScrollToLineEnd()
        {
            int lastIndex = ((TextBox)_element).GetLastVisibleLineIndex();
            //40 because last line is  /r/n
            Verifier.Verify((lastIndex == 40) || (((TextBox)_element).VerticalOffset > 0), "didnt scroll to last line", false);
            NextCombination();
        }

        /// <summary>Test invalid calls </summary>
        private void TestInvalidCalls()
        {
            try
            {
                ((TextBox)_element).ScrollToLine(((TextBox)_element).LineCount + 1);
                throw new ApplicationException("Scroll to line accepts > line count value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("ArgumentException thrown as expected when value >line count is used");
            }

            try
            {
                ((TextBox)_element).ScrollToLine(-1);
                throw new ApplicationException("Scroll to line accepts -ve value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("ArgumentException thrown as expected when -ve value  is used");
            }
            NextCombination();
        }

        #region private  data.

        private TextBoxScrollFunction _textBoxScrollSwitch = 0;
        private FrameworkElement _element = null;
        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;
        private bool _acceptsReturn = false;
        private bool _largeMultiLineContent = false;
        private bool _wrapText = false;
        private bool _scrollVisible = true;
        private string _textFormattingMode = string.Empty;

        #endregion private  data.
    }
}