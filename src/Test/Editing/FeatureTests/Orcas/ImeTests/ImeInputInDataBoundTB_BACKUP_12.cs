// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Test.Uis.TextEditing
{
    /// <summary>
    /// This test case adds coverage for InputMethod.SetPreferredImeConversionMode() API.
    /// Also adds regression coverage for Part1 Regression_Bug357
    /// </summary>
    [Test(1, "IME", "ImeInputInDataBoundTB", MethodParameters = "/TestCaseType:ImeInputInDataBoundTB", Timeout = 120, Keywords = "KoreanIME")]
    public class ImeInputInDataBoundTB : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the test case</summary>
        public override void RunTestCase()
        {
            _sourceTextBlock = new TextBlock();
            _sourceTextBlock.Height = 100;
            _sourceTextBlock.Text = sourceTextBlockContent;

            _textBox = new TextBox();
            _textBox.Height = 200;
            _textBox.FontSize = 24;

            // Setup DataBinding
            Binding binding = new Binding("Text");
            binding.Source = _sourceTextBlock;
            _textBox.SetBinding(TextBox.TextProperty, binding);

            // A dummy textbox used to perform unfocus operation on the main textboxbase.
            _dummyTextBox = new TextBox();

            _panel = new StackPanel();
            _panel.Children.Add(_sourceTextBlock);
            _panel.Children.Add(_textBox);
            _panel.Children.Add(_dummyTextBox);
            MainWindow.Content = _panel;

            QueueDelegate(TestImeInput);
        }

        private void TestImeInput()
        {
            IMEHelper.SetUpIMEKeyboardLayout(IMELocales.Korean, _dummyTextBox, MainWindow);

            // Put the focus in the actual TextBox where test is done
            _textBox.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            KeyboardInput.TypeString("{END}");
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            KeyboardInput.TypeString(contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            string expectedContent = sourceTextBlockContent + expectedImeContentAfterTyping;
            Verifier.Verify(_textBox.Text == expectedContent, "Verifying content after Ime input in a data-bound TextBox: " +
                "Actual [" + _textBox.Text + "] Expected [" + expectedContent + "]", true);

            Logger.Current.ReportSuccess();
        }

        #endregion

        #region Private fields

        private TextBlock _sourceTextBlock;
        private TextBox _textBox,_dummyTextBox;
        private StackPanel _panel;

        private const string sourceTextBlockContent = "abc";

        // Sample content to type for testing ImeConversionMode
        private const string contentToTypeInIME = "qixms{RIGHT}";
        private const string expectedImeContentAfterTyping = "뱌튼";

        #endregion
    }
}