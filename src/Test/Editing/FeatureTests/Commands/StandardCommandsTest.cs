// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Repro test cases for the StandardCommands class. 

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Commands/StandardCommandsTest.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.ComponentModel;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that the following commands work properly:
    /// Select All, Cut, Copy, Paste, Delete, Undo, Redo.
    /// </summary>
    [Test(0, "Editor", "PlainTextCommandsTest", MethodParameters = "/TestCaseType=PlainTextCommandsTest")]
    [TestOwner("Microsoft"),TestTactics("69")]
    public class PlainTextCommandsTest: CustomTestCase
    {
        class CaseData
        {
            private string _textToType;
            private string _expectedText;
            private string _expectedSelection;

            /// <summary>Text to type.</summary>
            public string TextToType { get { return _textToType; } }
            /// <summary>Expected text after typing, null to ignore.</summary>
            public string ExpectedText { get { return _expectedText; } }
            /// <summary>Expected selection text after typing, null to ignore.</summary>
            public string ExpectedSelection { get { return _expectedSelection; } }

            public CaseData(string textToType, string expectedText,
                string expectedSelection)
            {
                this._textToType = textToType;
                this._expectedText = expectedText;
                this._expectedSelection = expectedSelection;
            }

            public static CaseData[] Cases = new CaseData[] {
                new CaseData("abc", "abc", ""),
                // Verify that Cut works on text.
                new CaseData("+{LEFT}^c", "abc", "c"),
                // Verify that Paste works.
                new CaseData("^v", "abc", ""),
                new CaseData("^v", "abcc", ""),
                // Verify that Cut on empty content does nothing (Regression_Bug3)
                new CaseData("+{LEFT}", "abcc", "c"),
                new CaseData("^x", "abc", ""),
                new CaseData("^v", "abcc", ""),
                // Verify that Delete removes text.
                new CaseData("^a{DEL}", "", ""),
                // Verify that Copy with no content does nothing. (Regression_Bug3)
                new CaseData("x^a", "x", "x"),
                new CaseData("^c^v", "x", ""),
                // Verify that Copy with content changes clipboard.
                new CaseData("^a{DEL}x{LEFT}z^a^c{END}^v", "zxzx", ""),
                // Verify that Undo will remove copied text.
                new CaseData("^z", "zx", null),
                // Verify that Redo will reinsert copied text.
                new CaseData("^y", "zxzx", null),
                // Verify that whitespace can be copied by itself.
                new CaseData("^a{DEL}  ^a^c{end}^v", "    ", null),
            };
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Setting up TextBox...");
            _textbox = new TextBox();
            _wrapper = new UIElementWrapper(_textbox);
            MainWindow.Content = _textbox;            
            _caseIndex = 0;
            QueueDelegate(StartTest);
        }

        private void StartTest()
        {
            _textbox.Focus();
            _caseData = CaseData.Cases[_caseIndex];
            KeyboardInput.TypeString(_caseData.TextToType);
            QueueDelegate(CheckTest);
        }

        private void CheckTest()
        {
            if (_caseData.ExpectedText != null)
            {
                Log("Expected text: [" + _caseData.ExpectedText + "]");
                Log("Current text:  [" + _wrapper.Text + "]");
                Verifier.Verify(_caseData.ExpectedText == _wrapper.Text);
            }
            if (_caseData.ExpectedSelection != null)
            {
                Log("Expected selection: [" + _caseData.ExpectedSelection + "]");
                Log("Current selection:  [" + _wrapper.GetSelectedText(false, false) + "]");
                Verifier.Verify(_caseData.ExpectedSelection == _wrapper.GetSelectedText(false, false));
            }

            _caseIndex++;
            if (_caseIndex == CaseData.Cases.Length)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                QueueDelegate(StartTest);
            }
        }

        private CaseData _caseData;
        private int _caseIndex;
        private TextBox _textbox;
        private UIElementWrapper _wrapper;
    }
}
