// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Also adds regression coverage for TFS Part1 Regression_Bug357
    /// </summary>
    [Test(1, "IME", "SetPreferredImeConversionModeTest", MethodParameters = "/TestCaseType:SetPreferredImeConversionModeTest", Timeout = 120, Keywords = "JapaneseIME")]
    public class SetPreferredImeConversionModeTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            // This wont work with OSVersion < 6. Look at TFS Part1 Regression_Bug357
            if (System.Environment.OSVersion.Version.Major < 6)
            {
                Log("Skipping this combination since OSVersion is < 6");
                NextCombination();
                return;
            }

            _textBoxBase = (TextBoxBase)_testControlType.CreateInstance();
            _textBoxBase.Height = 200;
            _textBoxBase.FontSize = 24;

            _wrapper = new UIElementWrapper(_textBoxBase);

            // A dummy textbox used to perform unfocus operation on the main textboxbase.
            _dummyTextBox = new TextBox();

            _panel = new StackPanel();
            _panel.Children.Add(_textBoxBase);
            _panel.Children.Add(_dummyTextBox);
            MainWindow.Content = _panel;

            QueueDelegate(TestSetPreferredImeConversionModeAPI);
        }

        private void TestSetPreferredImeConversionModeAPI()
        {
            _textBoxBase.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.Japanese);
            KeyboardInput.SetActiveInputLocale(InputLocaleData.JapaneseMsIme2002.Identifier);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.IMEWaitTimeMs);            

            InputMethod.SetPreferredImeState(_textBoxBase, InputMethodState.On);
            InputMethod.SetPreferredImeConversionMode(_textBoxBase, testImeConversionModeValues);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.IMEWaitTimeMs);            

            // Unfocus from the textboxbase
            _dummyTextBox.Focus();

            // Focus back on textboxbase. The changes in the ImeConversionMode should now show up.
            _textBoxBase.Focus();

            Log("Current ImeConversionMode: " + InputMethod.GetPreferredImeConversionMode(_textBoxBase).ToString());
            Verifier.Verify(InputMethod.GetPreferredImeConversionMode(_textBoxBase) == testImeConversionModeValues,
                "Verifying the value of ImeConversionMode after setting it", true);

            KeyboardInput.TypeString(contentToType);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            Verifier.Verify(_wrapper.Text[0] == expectedContentAfterTyping[0],
                "Verifying the content after typing after ImeConversionMode is set", true);

            NextCombination();
        }

        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values        
        private TextEditableType _testControlType = null;

        private TextBoxBase _textBoxBase;
        private UIElementWrapper _wrapper;
        private TextBox _dummyTextBox;
        private StackPanel _panel;

        private const ImeConversionModeValues testImeConversionModeValues = ImeConversionModeValues.Roman |
                ImeConversionModeValues.Katakana |
                ImeConversionModeValues.Native |
                ImeConversionModeValues.FullShape;

        // Sample content to type for testing ImeConversionMode
        private const string contentToType = "a{ENTER}";
        private const string expectedContentAfterTyping = "ア";

        #endregion
    }
}