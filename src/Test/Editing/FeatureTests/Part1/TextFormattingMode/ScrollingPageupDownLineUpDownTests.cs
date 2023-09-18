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
        
    /// <summary>Tests keyboard scrolling </summary>
    [Test(2, "TextBoxBase", "ScrollingPageupDownLineUpDownTests", MethodParameters = "/TestCaseType:ScrollingPageupDownLineUpDownTests", Keywords = "TextFormattingModeTests", Timeout = 500)]
    public class ScrollingPageupDownLineUpDownTests : ManagedCombinatorialTestCase
    {
        /// <summary>Starts the combinatorial engine </summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);

            if (_element is PasswordBox)
            {
                NextCombination();
            }
            else
            {
                _controlWrapper = new UIElementWrapper(_element);
                ((TextBoxBase)_element).FontSize = 20;
                ((TextBoxBase)_element).AcceptsReturn = true;
                ((TextBoxBase)_element).Height = ((TextBoxBase)_element).Width = 200;
                switch (_textFormattingMode)
                {
                    case "Ideal": TextOptions.SetTextFormattingMode(_element, TextFormattingMode.Ideal);
                        break;
                    case "Display": TextOptions.SetTextFormattingMode(_element, TextFormattingMode.Display);
                        break;
                }
                TestElement = _element;
                _count = 0;
                SetText();
                QueueDelegate(DoWindowFocus);
            }
        }

        private void DoWindowFocus()
        {
            MouseInput.MouseClick(_element);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(DoFocus);
        }

        /// <summary>DoFocus </summary>
        private void DoFocus()
        {
            _element.Focus();
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            //LINE DOWN DOESNT SCROLL BY LINE HEIGHT FOR RTB
            //so we perform 2 more

            ((TextBoxBase)_element).LineDown();
            ((TextBoxBase)_element).LineDown();
            if (_element is RichTextBox)
            {
                ((TextBoxBase)_element).LineDown();
                ((TextBoxBase)_element).LineDown();
            }
            QueueDelegate(GetUpperBoundOnLineDown);
        }

        /// <summary>GetUpperBoundOnLineDown</summary>
        private void GetUpperBoundOnLineDown()
        {
            _expectedVerticalOffsetLineOperation = ((TextBoxBase)_element).VerticalOffset;
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            ((TextBoxBase)_element).PageDown();
            ((TextBoxBase)_element).LineDown();
            ((TextBoxBase)_element).LineDown();
            QueueDelegate(GetUpperBoundOnPageDown);
        }

        /// <summary>GetUpperBoundOnPageDown </summary>
        private void GetUpperBoundOnPageDown()
        {
            _expectedVerticalOffsetPageOperation = ((TextBoxBase)_element).VerticalOffset;
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            QueueDelegate(ScrollPageDown);
        }

        /// <summary>ScrollPageDown </summary>
        private void ScrollPageDown()
        {
            _element.Focus();
            KeyboardInput.TypeString("{PGDN}");
            QueueDelegate(VerifyPageDown);
        }

        /// <summary>VerifyPageDown </summary>
        private void VerifyPageDown()
        {
            Verifier.Verify(((TextBoxBase)_element).VerticalOffset <= _expectedVerticalOffsetPageOperation, "Expected [" +
                _expectedVerticalOffsetPageOperation.ToString() + "] >= Actual [" + ((TextBoxBase)_element).VerticalOffset.ToString() + "]", true);
            _initialOffset = ((TextBoxBase)_element).VerticalOffset;
            QueueDelegate(LineDown);
        }

        /// <summary>LineDown</summary>
        private void LineDown()
        {
            KeyboardInput.TypeString("{DOWN}");
            QueueDelegate(VerifyLineDown);
        }

        /// <summary>VerifyLineDown </summary>
        private void VerifyLineDown()
        {
            double val = ((TextBoxBase)_element).VerticalOffset - _initialOffset;
            if ((val == 0) && (_count++ < 10))
            {
                QueueDelegate(LineDown);
            }
            else
            {
                _count = 0;
                Verifier.Verify(val > 0, "Expected that the text scroll on line down Actual [" + val.ToString() + "]", true);
                Verifier.Verify(val <= _expectedVerticalOffsetLineOperation, "Expected [" +
                    _expectedVerticalOffsetLineOperation.ToString() + "] >= Actual [" + val.ToString() + "]", true);
                ((TextBoxBase)_element).PageDown();
                KeyboardInput.TypeString("{UP}");
                QueueDelegate(LineUp);
            }
        }

        /// <summary>LineUp </summary>
        private void LineUp()
        {
            _initialOffset = ((TextBoxBase)_element).VerticalOffset;
            KeyboardInput.TypeString("{UP}");
            QueueDelegate(VerifyLineUp);
        }

        /// <summary>VerifyLineUp </summary>
        private void VerifyLineUp()
        {

            double val = _initialOffset - ((TextBoxBase)_element).VerticalOffset;
            Verifier.Verify(val > 0, "Expected that the text scroll on line up Actual [" + val.ToString() + "]", true);
            Verifier.Verify(val <= _expectedVerticalOffsetLineOperation, "Expected [" +
                _expectedVerticalOffsetLineOperation.ToString() + "] >= Actual [" + val.ToString() + "]", true);
            _initialOffset = ((TextBoxBase)_element).VerticalOffset;
            KeyboardInput.TypeString("{PGUP}");
            QueueDelegate(VerifyPageUp);
        }

        /// <summary>VerifyPageUp </summary>
        private void VerifyPageUp()
        {
            double val = _initialOffset - ((TextBoxBase)_element).VerticalOffset;
            Verifier.Verify(val > 0, "Expected that the text scroll on page up Actual [" + val.ToString() + "]", true);
            Verifier.Verify(val <= _expectedVerticalOffsetPageOperation, "Expected [" +
                _expectedVerticalOffsetPageOperation.ToString() + "] >= Actual [" + val.ToString() + "]", true);
            QueueDelegate(NextCombination);
        }

        /// <summary>SetText</summary>
        private void SetText()
        {
            string str = "";
            for (int i = 0; i < 200; i++)
            {
                str += i.ToString() + ") String\r\n";
            }
            _controlWrapper.Text = str;
        }

        #region privatedata.

        private FrameworkElement _element;
        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;

        private double _expectedVerticalOffsetLineOperation = 0;
        private double _expectedVerticalOffsetPageOperation = 0;
        private double _initialOffset = 0;
        private int _count = 0;
        private string _textFormattingMode = string.Empty;

        #endregion privatedata.
    }
}