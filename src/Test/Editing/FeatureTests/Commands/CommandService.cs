// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for the CommandService class.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 6 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/Editing/TextEditorTests.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.ComponentModel;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
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
    /// Superclass for test cases using CommandService.
    /// </summary>
    public abstract class CommandServiceTestCase : CustomTestCase
    {
        /// <summary>Gets the first CommandBinding for the specified command.</summary>
        /// <param name='bindings'>Collection of links to search.</param>
        /// <param name='command'>Command sought.</param>
        /// <returns>
        /// The first CommandBinding for the command. If not found, an exception is thrown.
        /// </returns>
        protected CommandBinding GetCommandBindingForCommand(
            CommandBindingCollection bindings, RoutedCommand command)
        {
            if (bindings == null)
            {
                throw new ArgumentNullException("bindings");
            }
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            
            foreach (CommandBinding binding in bindings)
            {
                if (binding.Command == command)
                {
                    return binding;
                }
            }

            Log("Unable to find command binding for command " + command.Name);
            
            throw new Exception("No command binding found for command " + command.Name);
        }

        /// <summary>Forces a query for a status update of the specified binding.</summary>
        /// <param name='binding'>Binding to update.</param>
        /// <remarks>
        /// Note that the update may be deferred until idle time.
        /// </remarks>
        protected void ForceQueryUpdate(CommandBinding binding)
        {
            //
            // In the current implementation, the update can only be
            // forced by dirtying the Enabled property value. The value
            // is set locally, but a real update is set to take place when
            // the application goes idle (2003-09-04).
            //
            binding.CanExecute += new CanExecuteRoutedEventHandler(OnQueryEnabled);
        }

        private void OnQueryEnabled(object target, CanExecuteRoutedEventArgs args)
        {
            //
            // If you want to handle Comand's IsEnabled and participate in deciding the enabledness,
            // one should be in the route when QueryEnabled handlers are called. You should attach
            // a QueryEnabled eventhandler by adding a CommandBinding and set the args.IsEnabled to 
            // false/true as the case may be. This is by design. 
            // Currently we route QueryEnabled Event when you call IsEnabled property on a Command,
            // so setter here don't make sense.
            //
            args.CanExecute = !args.CanExecute;
        }

        /// <summary>
        /// Verifies that the specified command is enabled by examining the
        /// given command bindings.
        /// </summary>
        /// <param name='bindings'>Bindings to examine.</param>
        /// <param name='command'>Command to verify.</param>
        /// <param name='enabled'>Expected enabled value.</param>
        protected void VerifyCommandEnabled(CommandBindingCollection bindings,
            RoutedCommand command, bool enabled)
        {
            CommandBinding binding = GetCommandBindingForCommand(bindings, command);
            Log("Enabled state: " + binding.Command.CanExecute(null) + " (expected " + enabled + ")");
            Verifier.Verify(binding.Command.CanExecute(null) == enabled);
        }

        private RoutedCommand _internalCommand;
        private bool _internalCommandEnabled;
        private bool _internalEnabled;
        /// <summary>
        /// Verifies whether the specified command is enabled.
        /// </summary>
        /// <param name='command'>Command to verify.</param>
        /// <param name='target'>Target object for the command.</param>
        /// <param name='enabled'>Expected enabled value.</param>
        protected void VerifyCommandEnabled(RoutedCommand command, 
            IInputElement target, bool enabled)
        {
            bool commandEnabled;    // RoutedCommand report of enabled-ness.
            
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            
            commandEnabled = command.CanExecute(null, target);
            _internalCommand = command;
            _internalEnabled = enabled;
            _internalCommandEnabled = commandEnabled;

            QueueDelegate( VerificationOfEnabledCommand);
        }

        private void VerificationOfEnabledCommand()
        {
            Log("Enabled state for command " + _internalCommand.Name + ": " +
                _internalCommandEnabled + " (expected " + _internalEnabled + ")");
            Verifier.Verify(_internalCommandEnabled == _internalEnabled);
        }
    }

    /// <summary>
    /// Verifies that the CommandBinding work for:
    /// 1.	Verify paste is disabled when clipboard is empty.
    /// 2.	Verify paste is enabled when clipboard has text.
    /// 3.	Verify cut/copy is disabled when selection is empty.
    /// 4.	Verify cut/copy is enabled when selection has text.
    /// 5.	Verify Undo/Redo is disabled when undo/redo stack is empty.
    /// 6.	Verify Undo/Redo is enabled when undo/redo stack is not empty.
    /// Note that this test case will required privileges to access the
    /// clipboard, so it will not run in the default SEE.
    [Test(0, "Editor", "CommandServiceClipboardQuery", MethodParameters = "/TestCaseType=CommandServiceClipboardQuery")]
    [TestOwner("Microsoft"), TestTactics("68")]
    public class CommandServiceClipboardQuery : CommandServiceTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    Log("Creating a TextBox...");
                    _textBox = new TextBox();
                    _UIEW = new UIElementWrapper(_textBox);
                    MainWindow.Content = _textBox;
                    _tb = _textBox as TextBoxBase;
                }
                else
                {
                    Log("Creating a RichTextBox...");
                    _richTextBox = new RichTextBox();
                    _UIEW = new UIElementWrapper(_richTextBox);
                    MainWindow.Content = _richTextBox;
                    _tb = _richTextBox as TextBoxBase;
                }

                MainWindow.UpdateLayout();
                _UIEW.Element.Focus();
                Log("******************************");

                Log("Verifying that paste is disabled when the clipboard is empty...");
                Test.Uis.Wrappers.Win32.SafeEmptyClipboard();
                VerifyCommandEnabled(ApplicationCommands.Paste, _UIEW.Element, true);

                Log("Verifying that paste is enabled when the clipboard has text...");
                WinForms.ClipboardSetDataObject("text");
                VerifyCommandEnabled(ApplicationCommands.Paste, _UIEW.Element, true);

                Log("Verifying that cut/copy is disabled when selection is empty...");
                _UIEW.Select(0, 0);
                VerifyCommandEnabled(ApplicationCommands.Copy, _UIEW.Element, false);
                VerifyCommandEnabled(ApplicationCommands.Cut, _UIEW.Element, false);

                Log("Verifying that cut/copy is enabled when selection has text...");
                _UIEW.Text = "text";
                _UIEW.Select(0, 4);
                VerifyCommandEnabled(ApplicationCommands.Copy, _UIEW.Element, true);
                VerifyCommandEnabled(ApplicationCommands.Cut, _UIEW.Element, true);

                Log("Verifying that undo/redo is disabled when undostack is empty...");
                _UIEW.Text = "text";
                _UIEW.Select(0, 4);
                DoCut();
            }
        }

        private void DoCut()
        {
            KeyboardInput.TypeString("^x");
            QueueDelegate(DoUndoRedo);
        }

        private void DoUndoRedo()
        {
            if (_tb.CanUndo)
            {
                VerifyCommandEnabled(ApplicationCommands.Undo, _UIEW.Element, true);
                VerifyCommandEnabled(ApplicationCommands.Redo, _UIEW.Element, true);
            }
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
        #region Private fields.
        private TextBox _textBox;
        private RichTextBox _richTextBox;
        private UIElementWrapper _UIEW;
        private TextBoxBase _tb;
        #endregion Private fields.
    }
}
