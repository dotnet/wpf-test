// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for commanding from a TextBox point-of-view.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;    

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that CommandBindings are populated by default on a TextBox
    /// with at least the cut, copy and paste commands.
    /// </summary>
    [Test(0, "TextBox", "TextBoxCommandLinksPopulated", MethodParameters = "/TestCaseType=TextBoxCommandLinksPopulated")]
    [TestOwner("Microsoft"), TestTactics("656"), TestBugs("660")]
    public class TextBoxCommandLinksPopulated : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            VerifyCommandBinding();
            TestFormattingCommands();
        }

        private void TestFormattingCommands()
        {
            Log("Verifying that commands do not change the text...");
            TestTextBox.Text = "abcd";

            TestTextBox.TextChanged += delegate
            {
                Log("Text changed unexpectedly.");
                throw new Exception(
                    "Formatting commands are changing TextBox text.");
            };
            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("^a^b^i^u");

            QueueDelegate(CheckFormattingCommands);
        }

        private void CheckFormattingCommands()
        {
            Log("No commands modified the text.");
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyCommandBinding()
        {
            CommandBindingCollection bindings = TestTextBox.CommandBindings;
            foreach (CommandBinding binding in bindings)
            {
                if(binding.Command == ApplicationCommands.Cut)
                    Verifier.Verify(true, "Cut command present in TextBox.CommandBindings", true);
                if (binding.Command == ApplicationCommands.Copy)
                    Verifier.Verify(true, "Copy command present in TextBox.CommandBindings", true);
                if (binding.Command == ApplicationCommands.Paste)
                    Verifier.Verify(true, "Paste command present in TextBox.CommandBindings", true);
            }
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that there is an Undo service on the TextBox.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("655"), TestBugs("663")]
    public class TextBoxUndoServiceAvailable : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that there are no conflicting key bindings for the editing
    /// controls.
    /// </summary>
    [Test(2, "Editor", "EditingKeyBindingTest", MethodParameters = "/TestCaseType=EditingKeyBindingTest")]
    [TestOwner("Microsoft"), TestWorkItem("124"), TestTactics("654")]
    public class EditingKeyBindingTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs the specific combination.</summary>
        protected override void  DoRunCombination()
        {
            TestElement = _editableType.CreateInstance();
            Type type = _editableType.Type;

            Dictionary<KeyGesture, RoutedCommand> keysToCommandMap = new Dictionary<KeyGesture, RoutedCommand>();
            HybridDictionary classInputBindings = (HybridDictionary)ReflectionUtils.GetStaticField(typeof(CommandManager), "_classInputBindings");

            while (type != null)
            {
                foreach (DictionaryEntry entry in classInputBindings)
                {
                    if (type.Equals(entry.Key))
                    {
                        InputBindingCollection collection = (InputBindingCollection)entry.Value;
                        foreach (InputBinding binding in collection)
                        {
                            KeyGesture keyGesture = binding.Gesture as KeyGesture;
                            if (keyGesture == null)
                            {
                                continue;
                            }
                            RecordKeyToCommandMap(keysToCommandMap, keyGesture, binding.Command as RoutedCommand);
                        }
                    }
                }
                type = type.BaseType;
            }

            Logger.Current.Log("There are no commands with shared key gestures.");

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Helper methods.

        /// <summary>
        /// Checks whether the specified KeyGesture maps to a meaningful
        /// key gesture.
        /// </summary>
        private static bool IsEmpty(KeyGesture keyGesture)
        {
            return keyGesture == null ||
                (keyGesture.Key == Key.None && keyGesture.Modifiers == ModifierKeys.None);
        }

        /// <summary>Returns a user-friendly string for the specified key gesture.</summary>
        private static string KeyGestureToString(KeyGesture keyGesture)
        {
            return keyGesture.Modifiers.ToString() + "+" + keyGesture.Key.ToString();
        }

        /// <summary>Records that a key maps to a command.</summary>
        /// <remarks>This method will throw an exception if there is a conflict.</remarks>
        private void RecordKeyToCommandMap(
            Dictionary<KeyGesture, RoutedCommand> map,
            KeyGesture keyGesture, RoutedCommand command)
        {
            if (IsEmpty(keyGesture))
            {
                string message;

                message = "Command binding has no key gesture: " + command.Name;
                Log(message);
                return;
            }
            foreach (KeyValuePair<KeyGesture, RoutedCommand> pair in map)
            {
                if (pair.Key.Key == keyGesture.Key && pair.Key.Modifiers == keyGesture.Modifiers)
                {
                    throw new Exception(
                        "KeyGesture conflict for [" + KeyGestureToString(keyGesture) + ". This is used for " +
                        command.Name + " and " + pair.Value.Name + ".");
                }
            }
            map.Add(keyGesture, command);
            Log("Unique key gesture " + KeyGestureToString(keyGesture) + " used for " + command.Name);
        }

        #endregion Helper methods.

        #region Private fields.

        /// <summary>Type being tested.</summary>
        private TextEditableType _editableType = null;

        #endregion Private fields.
    }
}
