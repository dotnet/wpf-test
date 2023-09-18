// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for key events on TextBox and RichTextBox controls

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Input/KeyEvents.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Input;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>Verifies that key events are fired correctly.</summary>
    /// <remarks>
    /// Input is generated and events are verified. Only Preview events
    /// are verified, as the TextEditor may choose to handle some events
    /// itself.
    /// </remarks>
    [Test(3, "Input", "KeyEvents", MethodParameters="/TestCaseType=KeyEvents")]
    [TestOwner("Microsoft"), TestTactics("343"), TestBugs("492,493")]
    public class KeyEvents: CustomTestCase
    {
        #region Private fields.

        private int _editableTypeIndex;
        private FrameworkElement _box;
        private int _nextEventIndex;
        private int _previewKeyDownFired;
        private int _previewKeyUpFired;

        #endregion Private fields.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _editableTypeIndex = 0;
            RunTest();
        }

        private void RunTest()
        {
            if (_editableTypeIndex == TextEditableType.Values.Length)
            {
                Logger.Current.ReportSuccess();
                return;
            }

            _nextEventIndex = 1;
            _box = TextEditableType.Values[_editableTypeIndex].CreateInstance();
            MainWindow.Content = _box;
            MainWindow.UpdateLayout();
            HookEvents();

            Log("Clicking on " + _box.GetType().Name + "...");
            MouseInput.MouseClick(_box);

            QueueDelegate(TypeKey);
        }

        private void TypeKey()
        {
            Log("Forcing keyboard focus to control...");
            _box.Focus();

            Log("Typing key...");
            KeyboardInput.TypeString("{ENTER}");

            QueueDelegate(CheckEvents);
        }

        private void VerifyEventSequence()
        {
            Verifier.Verify(_previewKeyDownFired == 1,
                "PreviewKeyDown fired first - actual: " + _previewKeyDownFired, true);
            Verifier.Verify(_previewKeyUpFired == 2,
                "PreviewKeyUp fired second - actual: " + _previewKeyUpFired, true);
        }

        private void CheckEvents()
        {
            VerifyEventSequence();

            _nextEventIndex = 1;
            Log("Typing key through Unicode packet...");
            KeyboardInput.TypeString("h");
            QueueDelegate(CheckUnicodeEvents);
        }

        private void CheckUnicodeEvents()
        {
            Log("Checking Unicode events...");
            VerifyEventSequence();

            _editableTypeIndex++;
            RunTest();
        }

        #endregion Main flow.

        #region Helper methods.

        private void HookEvents()
        {
            _box.KeyDown += new KeyEventHandler(TextBoxKeyDown);
            _box.KeyUp += new KeyEventHandler(TextBoxKeyUp);
            _box.PreviewKeyDown += new KeyEventHandler(TextBoxPreviewKeyDown);
            _box.PreviewKeyUp += new KeyEventHandler(TextBoxPreviewKeyUp);
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs args)
        {
            Log("KeyDown fired");
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs args)
        {
            Log("KeyUp fired");
        }

        private void TextBoxPreviewKeyDown(object sender, KeyEventArgs args)
        {
            Log("PreviewKeyDown fired");
            _previewKeyDownFired = _nextEventIndex++;
        }

        private void TextBoxPreviewKeyUp(object sender, KeyEventArgs args)
        {
            Log("PreviewKeyUp fired");
            _previewKeyUpFired = _nextEventIndex++;
        }

        #endregion Helper methods.
    }

    // NOTE: the file history contains a InputManagerEmulation test case.
    // Direct input emulation (an equivalent for a direct WM_KEYUP/WM_KEYDOWN send)
    // is no longer supported.
}
