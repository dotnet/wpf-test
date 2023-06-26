// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Microsoft.Test.Editing
{    
    /// <summary>
    /// This tests the functionality of IsReadOnlyCaretVisible property on TextBoxBase.
    /// In particular it tests caret navigation by down arrow key.
    /// Refer to TFS Part1 Regression_Bug74
    /// </summary>
    [Test(0, "Caret", "IsReadOnlyCaretVisibleTest", MethodParameters = "/TestCaseType:IsReadOnlyCaretVisibleTest")]
    public class IsReadOnlyCaretVisibleTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _textBoxBase = (TextBoxBase)_testControlType.CreateInstance();
            _textBoxBase.Height = 100;
            _textBoxBase.FontSize = 16;
            _textBoxBase.AcceptsReturn = true;
            _textBoxBase.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _textBoxBase.IsReadOnly = true;

            Verifier.Verify(_textBoxBase.IsReadOnlyCaretVisible == false, "Verify the default value for the property", true);

            // Assign test value
            _textBoxBase.IsReadOnlyCaretVisible = _isReadOnlyCaretVisible;

            // Assign 10 lines of content so that content goes beyond the viewport (height)
            string content = string.Empty;
            for (int i = 0; i < 10; i++)
            {
                content += "line" + i.ToString() + "\r\n";
            }

            _wrapper = new UIElementWrapper(_textBoxBase);
            _wrapper.Text = content;

            StackPanel panel = new StackPanel();
            panel.Children.Add(_textBoxBase);            
            MainWindow.Content = panel;

            QueueDelegate(AfterLoad);
        }

        private void AfterLoad()
        {
            _textBoxBase.Focus();
            _wrapper.Select(0, 0);
            DispatcherHelper.DoEvents();

            // Verify initial state
            Verifier.Verify(_textBoxBase.VerticalOffset == 0, "Verify that vertical offset is at its initial offset", true);

            // Navigate down 10 times to reach the last line
            KeyboardInput.TypeString("{DOWN 10}");

            QueueDelegate(VerifyVerticalOffset);
        }

        private void VerifyVerticalOffset()
        {
            if (_isReadOnlyCaretVisible)
            {
                Verifier.Verify(_textBoxBase.VerticalOffset > 0, "Verify that veritical offset has changed by down key", true);
            }
            else
            {
                Verifier.Verify(_textBoxBase.VerticalOffset == 0, "Verify that veritical offset has NOT changed by down key", true);
            }
            Log("Final vertical offset: " + _textBoxBase.VerticalOffset);
            
            NextCombination();
        }

        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values        
        private TextEditableType _testControlType = null;
        private bool _isReadOnlyCaretVisible = false;

        private TextBoxBase _textBoxBase;
        private UIElementWrapper _wrapper;        

        #endregion
    }
}
