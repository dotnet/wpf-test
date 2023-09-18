// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Regression coverage for Part1 Regression_Bug75: Verifies that attempts to programmatically
//  access the clipbard when running in partial trust fails silently.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Microsoft.Test.Discovery;
using Test.Uis.Data;
using Test.Uis.TestTypes;
using Test.Uis.Loggers;
using Test.Uis.Wrappers;
using Test.Uis.Utils;

namespace Microsoft.Test.Editing
{
    /// <summary>
    /// Verifies Part1 Regression_Bug75: Attempts to access ClipBoard through TextBoxBase.Copy() fails silently in Partial Trust
    /// </summary>
    [Test(1, "PartialTrust", "ProgrammaticAccessToClipBoardInPTTest", MethodParameters = "/TestCaseType:ProgrammaticAccessToClipBoardInPTTest /XbapName=EditingTestDeployPart1", SupportFiles = @"FeatureTests\Editing\EditingTestDeployPart1*", Timeout = 150)]
    public class ProgrammaticAccessToClipBoardInPTTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _textBoxBase = (TextBoxBase)_testControlType.CreateInstance();
            _textBoxBase.FontSize = 24;
            _textBoxBase.Height = 230;
            _textBoxBase.Width = 300;
            _textBoxBase.Focusable = true;

            _wrapper = new UIElementWrapper(_textBoxBase);
            _wrapper.Text = _contentForClipBoard;

            StackPanel panel = new StackPanel();
            panel.Children.Add(_textBoxBase);
            MainWindow.Content = panel;

            QueueDelegate(DoProgrammaticAccessToClipbard);
        }

        private void DoProgrammaticAccessToClipbard()
        {
            _textBoxBase.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // Select all content
            _textBoxBase.SelectAll();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // Copy all content - This should not work, but also should not crash
            _textBoxBase.Copy();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // Set new content on the TextBox
            _wrapper.Text = "new text";
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // Select all content
            _textBoxBase.SelectAll();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // textBoxBase.Paste() already fails silently, so cannot paste that way.
            // Ctrl+v works, but it should NOT paste the content we tried to copy above.
            Microsoft.Test.Input.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(System.Windows.Input.Key.V, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(System.Windows.Input.Key.V, false);
            Microsoft.Test.Input.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, false);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            Log(string.Format("Current text in container: {0}", _wrapper.Text));

            // Verify that the content we tried to copy is not copied to the container.
            Verifier.Verify(!_wrapper.Text.ToLowerInvariant().Contains(_contentForClipBoard.ToLowerInvariant()), "Verifying that the intial text of the container was not progammatically copied.", true);

            NextCombination();
        }

        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values                
        private TextEditableType _testControlType = null;

        private UIElementWrapper _wrapper;
        private TextBoxBase _textBoxBase;
        private string _contentForClipBoard = "Content for the ClipBoard";

        #endregion
    }
}
