// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DRT
{
    public sealed class TestCommandOverride : DrtTestSuite
    {
        public TestCommandOverride () : base("override") { }

        //---------------------------------------------------------------------
        public override DrtTest[] PrepareTests()
        {
            DRT.LoadXamlFile(@"DrtFiles\Commanding\TestCommandOverride.xaml");

            _overridePanel = (Panel)DRT.FindVisualByID("overridePanel");
            _sourceTB = (TextBox)DRT.FindVisualByID("source");
            _overridePasteTB = (TextBox)DRT.FindVisualByID("overridePaste");
            _overrideCtrlVTB = (TextBox)DRT.FindVisualByID("overrideCtrlV");

            PrepareOverrideParent();

            return new DrtTest[]
            {
                new DrtTest(VerifyOverrideCommand),
                new DrtTest(VerifyOverrideInputBinding),
            };
        }

        //---------------------------------------------------------------------
        void PrepareOverrideParent()
        {
            CommandBinding commandBinding;

            // attach a command binding for the Paste command
            commandBinding = new CommandBinding(ApplicationCommands.Paste);
            commandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnCanExecute);
            commandBinding.Executed += new ExecutedRoutedEventHandler(OnPasteParent);
            _overridePanel.CommandBindings.Add(commandBinding);

            // attach a command binding for the Custom command
            commandBinding = new CommandBinding(CustomCommand);
            commandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnCanExecute);
            commandBinding.Executed += new ExecutedRoutedEventHandler(OnCustomCommand);
            _overridePanel.CommandBindings.Add(commandBinding);

            // bind Ctl+V to the Custom command
            KeyGesture ctrlV = new KeyGesture(Key.V, ModifierKeys.Control);
            KeyBinding keyBinding = new KeyBinding(CustomCommand, ctrlV);
            _overridePanel.InputBindings.Add(keyBinding);
        }

        void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void OnPasteParent(object sender, ExecutedRoutedEventArgs e)
        {
            ++ _pasteCount;
        }

        void OnCustomCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ++ _customCount;
        }

        //---------------------------------------------------------------------
        void VerifyOverrideCommand()
        {
            // Goal:  Disable a built-in command on a child element, but allow
            //      that command to bubble to the parent.  Illustrated here using
            //      the Paste command on TextBox.
            // Method:
            //  1. install a command binding on the child that does nothing.
            //      This prevents the built-in command from executing, since
            //      the ExecutedRoutedEventArgs will have Handled = true.
            //  2. add a handler for the Executed event directly to the child.
            //      Mark it "handledEventsToo", so that it will be called, even
            //      though the command binding's handler already handled the event.
            //      The direct handler resets Handled = false.  This allows the
            //      event to bubble up to the parent.

            // Step 1.  Attach a command binding for the Paste command
            CommandBinding binding = new CommandBinding(ApplicationCommands.Paste);
            binding.CanExecute += new CanExecuteRoutedEventHandler(OnCanExecute);
            binding.Executed += new ExecutedRoutedEventHandler(DoNothing);
            _overridePasteTB.CommandBindings.Add(binding);

            // Step 2.  Add a direct handler that resets Handled = false;
            _overridePasteTB.AddHandler(CommandManager.ExecutedEvent,
                                        new ExecutedRoutedEventHandler(ResetHandled),
                                        true /*handledEventsToo*/);

            // prepare to test this technique.
            string text = _overridePasteTB.Text;
            _pasteCount = 0;

            // Copy text from the source TextBox into the clipboard
            _sourceTB.Focus();
            _sourceTB.Select(0, Int32.MaxValue);
            DRT.SendKeyboardInput(Key.LeftCtrl, true);
            DRT.SendKeyboardInput(Key.C, true);
            DRT.SendKeyboardInput(Key.C, false);
            DRT.SendKeyboardInput(Key.LeftCtrl, false);
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            // Invoke the Paste command on the child
            _overridePasteTB.Focus();
            DRT.SendKeyboardInput(Key.LeftCtrl, true);
            DRT.SendKeyboardInput(Key.V, true);
            DRT.SendKeyboardInput(Key.V, false);
            DRT.SendKeyboardInput(Key.LeftCtrl, false);
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            // Verify that the child's built-in command did not run
            DRT.AssertEqual(text, _overridePasteTB.Text, "Built-in Paste command ran when it should not have");

            // Verify that the parent's command did run
            DRT.AssertEqual(1, _pasteCount, "Disabled built-in command did not bubble to the parent");
        }

        void DoNothing(object sender, ExecutedRoutedEventArgs e)
        {
        }

        void ResetHandled(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = false;
            }
        }

        //---------------------------------------------------------------------
        void VerifyOverrideInputBinding()
        {
            // Goal:  Disable a built-in input binding on a child element, but allow
            //      that input to bubble to the parent.  Illustrated here using
            //      the Ctrl+V key (bound to the Paste command) on TextBox.
            // Method: bind the input to the special NotACommand command

            KeyGesture ctrlV = new KeyGesture(Key.V, ModifierKeys.Control);
            KeyBinding keyBinding = new KeyBinding(ApplicationCommands.NotACommand, ctrlV);
            _overrideCtrlVTB.InputBindings.Add(keyBinding);

            // prepare to test this technique.
            string text = _overrideCtrlVTB.Text;
            _customCount = 0;

            // Copy text from the source TextBox into the clipboard
            _sourceTB.Focus();
            _sourceTB.Select(0, Int32.MaxValue);
            DRT.SendKeyboardInput(Key.LeftCtrl, true);
            DRT.SendKeyboardInput(Key.C, true);
            DRT.SendKeyboardInput(Key.C, false);
            DRT.SendKeyboardInput(Key.LeftCtrl, false);
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            // Invoke the Paste command on the child
            _overrideCtrlVTB.Focus();
            DRT.SendKeyboardInput(Key.LeftCtrl, true);
            DRT.SendKeyboardInput(Key.V, true);
            DRT.SendKeyboardInput(Key.V, false);
            DRT.SendKeyboardInput(Key.LeftCtrl, false);
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            // Verify that the child's built-in command did not run
            DRT.AssertEqual(text, _overrideCtrlVTB.Text, "Built-in Ctrl+V binding ran when it should not have");

            // Verify that the parent's command did run
            DRT.AssertEqual(1, _customCount, "Disabled built-in input binding did not bubble to the parent");
        }

        //---------------------------------------------------------------------

        static readonly RoutedCommand CustomCommand = new RoutedCommand("Custom", typeof(TestCommandOverride), null);

        Panel _overridePanel;
        TextBox _sourceTB, _overridePasteTB, _overrideCtrlVTB;
        int _pasteCount, _customCount;
    }
}
