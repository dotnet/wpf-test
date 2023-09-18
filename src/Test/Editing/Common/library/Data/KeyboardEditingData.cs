// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data to be used when editing text with the keyboard.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Data/KeyboardEditingData.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;    // for XmlLanguage
    using System.Windows.Threading;

    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Interesting values for a keyboard editing dimension.
    /// </summary>
    public enum KeyboardEditingTestValue
    {
        #region KeyboardEditingTestValue values.

        /// <summary>Alphabetic characters.</summary>
        Alphabetic,
        /// <summary>Alphabetic characters with shift pressed.</summary>
        AlphabeticShift,
        /// <summary>Numeric characters.</summary>
        Numeric,
        /// <summary>Numeric characters with shift pressed.</summary>
        NumericShift,
        /// <summary>Dead keys.</summary>
        DeadKeys,
        /// <summary>Alt+numeric keypad Unicode character.</summary>
        AltNumpadKeys,
        /// <summary>Enter character.</summary>
        Enter,
        /// <summary>Enter character with shift pressed.</summary>
        EnterShift,
        /// <summary>Delete key.</summary>
        Delete,
        /// <summary>Delete key with Control pressed.</summary>
        DeleteControl,
        /// <summary>Backspace key.</summary>
        Backspace,
        /// <summary>Backspace key with Control pressed.</summary>
        BackspaceControl,
        /// <summary>Backspace key with shift pressed.</summary>
        BackspaceShift,
        /// <summary>Space bar.</summary>
        Space,
        /// <summary>Tab key.</summary>
        Tab,
        /// <summary>Tab key with Shift pressed.</summary>
        TabShift,
        /// <summary>Left Ctrl+Shift.</summary>
        ControlShiftLeft,
        /// <summary>Right Ctrl+Shift.</summary>
        ControlShiftRight,
        /// <summary>Left arrow key.</summary>
        LeftArrow,
        /// <summary>Right arrow key.</summary>
        RightArrow,
        /// <summary>Up arrow key.</summary>
        UpArrow,
        /// <summary>Down arrow key.</summary>
        DownArrow,
        /// <summary>Home key.</summary>
        Home,
        /// <summary>Home key.</summary>
        End,
        /// <summary>Page up key.</summary>
        PageUp,
        /// <summary>Page down key.</summary>
        PageDown,
        /// <summary>Left arrow key with Shift pressed.</summary>
        LeftArrowShift,
        /// <summary>Right arrow key with Shift pressed.</summary>
        RightArrowShift,
        /// <summary>Up arrow key with Shift pressed.</summary>
        UpArrowShift,
        /// <summary>Down arrow key with Shift pressed.</summary>
        DownArrowShift,
        /// <summary>Home key with Shift pressed.</summary>
        HomeShift,
        /// <summary>Home key with Shift pressed.</summary>
        EndShift,
        /// <summary>Page up key with Shift pressed.</summary>
        PageUpShift,
        /// <summary>Page down key with Shift pressed.</summary>
        PageDownShift,
        /// <summary>Left arrow key with Control pressed.</summary>
        LeftArrowControl,
        /// <summary>Right arrow key with Control pressed.</summary>
        RightArrowControl,
        /// <summary>Up arrow key with Control pressed.</summary>
        UpArrowControl,
        /// <summary>Down arrow key with Control pressed.</summary>
        DownArrowControl,
        /// <summary>Left arrow key with Control and Shift pressed.</summary>
        LeftArrowControlShift,
        /// <summary>Right arrow key with Control and Shift pressed.</summary>
        RightArrowControlShift,
        /// <summary>Up arrow key with Control and Shift pressed.</summary>
        UpArrowControlShift,
        /// <summary>Down arrow key with Control and Shift pressed.</summary>
        DownArrowControlShift,
        /// <summary>Home key with Control pressed.</summary>
        HomeControl,
        /// <summary>Home key with Control pressed.</summary>
        EndControl,
        /// <summary>Page up key with Control pressed.</summary>
        PageUpControl,
        /// <summary>Page down key with Control pressed.</summary>
        PageDownControl,
        /// <summary>Keyboard input for Undo command.</summary>
        UndoCommandKeys,
        /// <summary>Keyboard input for Redo</summary>
        RedoCommandKeys,
        /// <summary>Keyboard input for Bold</summary>
        BoldCommandKeys,
        /// <summary>Keyboard input for copy</summary>
        CopyCommandKeys,
        /// <summary>Keyboard input for UnderLine</summary>
        UnderlineCommandKeys,
        /// <summary>Keyboard input for Italic</summary>
        ItalicCommandKeys,
        /// <summary>Keyboard input for CenterJustify</summary>
        CenterJustifyCommandKeys,
        /// <summary>Keyboard input for RightJustify</summary>
        RightJustifyCommandKeys,
        /// <summary>Keyboard input for LeftJustify</summary>
        LeftJustifyCommandKeys,
        /// <summary>Keyboard input for increasing indentation</summary>
        IncreaseIndentationCommandKeys,
        /// <summary>Keyboard input for decreasing indentation</summary>
        DecreaseIndentationCommandKeys,
        /// <summary>Keyboard input for paste</summary>
        PasteCommandKeys,
        /// <summary>Keyboard input for cut</summary>
        CutCommandKeys,
        /// <summary>Keyboard input for increasing the fontsize</summary>
        IncreaseFontSizeCommandKeys,
        /// <summary>Keyboard input for decreasing the fontsize</summary>
        DecreaseFontSizeCommandKeys,

        #endregion KeyboardEditingTestValue values.
    }

    /// <summary>
    /// Provides information about interesting keyboard editing cases.
    /// </summary>
    public sealed class KeyboardEditingData
    {

        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private KeyboardEditingData() { }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Captures information about what is required before editing.
        /// </summary>
        /// <param name='wrapper'>Wrapper for element to capture.</param>
        public KeyboardEditingState CaptureBeforeEditing(UIElementWrapper wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            return new KeyboardEditingState(wrapper);
        }

        /// <summary>
        /// Gets the value matching the specified editing action.
        /// </summary>
        /// <param name='testValue'>Editing action to retrieve.</param>
        /// <returns>The KeyboardEditingData for the specified KeyboardEditingTestValue.</returns>
        public static KeyboardEditingData GetValue(KeyboardEditingTestValue testValue)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i].TestValue == testValue)
                {
                    return Values[i];
                }
            }

            throw new ArgumentException(
                "Unable to retrieve KeyboardEditingData for value " + testValue + ".", "testValue");
        }

        /// <summary>
        /// Gets an array of interesting values matching the
        /// specified editing actions.
        /// </summary>
        /// <param name='testValues'>Values to retrieve.</param>
        /// <returns>
        /// An array of interesting values matching the specified
        /// editing actions.
        /// </returns>
        /// <example>To get the alphabetic editing actions, use the
        /// following code: <code>
        /// KeyboardEditingData[] alphabeticActions;
        /// alphabeticActions = KeyboardEditingData.GetValues(
        ///   KeyboardEditingTestValue.Alphabetic,
        ///   KeyboardEditingTestValue.AlphabeticShift,
        /// );</code></example>
        public static KeyboardEditingData[] GetValues(params KeyboardEditingTestValue[] testValues)
        {
            KeyboardEditingData[] result;

            if (testValues == null)
            {
                throw new ArgumentNullException("testValues");
            }

            result = new KeyboardEditingData[testValues.Length];
            for (int i = 0; i < testValues.Length; i++)
            {
                for (int j = 0; j < Values.Length; j++)
                {
                    if (testValues[i] == Values[j].TestValue)
                    {
                        result[i] = Values[j];
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the input string for the specified key gesture.
        /// </summary>
        /// <param name="gesture">Gesture to translate to input string.</param>
        /// <returns>Input string that matches the specified gesture.</returns>
        public static string KeyGestureToString(KeyGesture gesture)
        {
            string result;

            if (gesture == null)
            {
                throw new ArgumentNullException("gesture");
            }

            result = gesture.Key.ToString();
            if ((gesture.Modifiers & ModifierKeys.Alt) != 0) throw new Exception("look up alt key");
            if ((gesture.Modifiers & ModifierKeys.Shift) != 0) result = "+" + result;
            if ((gesture.Modifiers & ModifierKeys.Control) != 0) result = "^" + result;

            return result;
        }

        /// <summary>
        /// Performs the keyboard editing action on the element.
        /// </summary>
        /// <param name='wrapper'>Wrapper for element to edit.</param>
        /// <param name='handler'>Callback for action completion.</param>
        public void PerformAction(UIElementWrapper wrapper, SimpleHandler handler)
        {
            PerformAction(wrapper, handler, false);
        }

        /// <summary>
        /// Performs the keyboard editing action on the element.
        /// </summary>
        /// <param name='wrapper'>Wrapper for element to edit.</param>
        /// <param name='handler'>Callback for action completion.</param>
        /// <param name='preferCommand'>Whether a command should be used if available.</param>
        public void PerformAction(UIElementWrapper wrapper, SimpleHandler handler,
            bool preferCommand)
        {
            bool queueHandler;

            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            //Clear the Clipboard before Copy/Cut is performed.
            if ((this.Command != null)&&
                ((this.Command == ApplicationCommands.Copy)||(this.Command == ApplicationCommands.Cut)))
            {
                Clipboard.Clear();
            }

            queueHandler = handler != null;
            if (preferCommand && this.Command != null)
            {
                this.Command.Execute(null, wrapper.Element);
            }
            else if (this._execute != null)
            {
                this._execute(wrapper, handler, preferCommand);
                queueHandler = false;
            }
            else if (this._executionTypeString.Length > 0)
            {
                KeyboardInput.TypeString(this._executionTypeString);
            }
            else if (this.Command != null)
            {
                bool commandTyped;

                commandTyped = false;
                foreach (InputGesture gesture in Command.InputGestures)
                {
                    if (gesture is KeyGesture)
                    {
                        KeyboardInput.TypeString(KeyGestureToString((KeyGesture)gesture));
                        break;
                    }
                }
                if (!commandTyped)
                {
                    this.Command.Execute(null, wrapper.Element);
                }
            }
            else
            {
                Logger.Current.Log("Action unimplemented: " + this.TestValue);
            }

            if (queueHandler)
            {
                QueueHelper.Current.QueueDelegate(handler);
            }
        }

        /// <summary>Returns a string representing the selection.</summary>
        /// <returns>A string representing the selection.</returns>
        public override string ToString()
        {
            return TestValue.ToString();
        }

        /// <summary>
        /// Verifies that the editing operation was performed
        /// I suggest that we don't perfrom verfication in the Editing Data class due to the follwong reasons:
        ///     1. The verification here are going to be very complicated.
        ///     2. It won't meet the verification requirments of a specific feature.
        ///     3. Users could be confused by the verification here since they may think that we have made a verify for them and me the most inportant of part of their testing.
        ///
        /// successfully.
        /// </summary>
        /// <param name='previousState'>
        /// State captured before editing.
        /// </param>
        public void VerifyEditing(KeyboardEditingState previousState)
        {
            if (previousState == null)
            {
                throw new ArgumentNullException("previousState");
            }

            try
            {
                InternalVerifyEditing(previousState);
            }
            catch
            {
                // The PasswordBox OM is not supported.
                if (!previousState.IsPasswordBox)
                {
                    // This logs the current state of the tree, accessed through the
                    // reference in the wrapper held by the state object.
                    TextTreeLogger.LogContainer("text-verification-log",
                        previousState.Wrapper.SelectionInstance.Start,
                        previousState.Wrapper.SelectionInstance.Start, "Selection.Start",
                        previousState.Wrapper.SelectionInstance.End, "Selection.End");
                }
                throw;
            }
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Command that this keyboard action is associated with, possibly null.</summary>
        public RoutedCommand Command
        {
            get { return this._command; }
        }

        /// <summary>Execution string that this keyboard action is associated with, possibly empty string.</summary>
        public string ExecutionString
        {
            get { return this._executionTypeString; }
        }

        /// <summary>Whether this action moves the caret or selection.</summary>
        public bool IsNavigationAction
        {
            get
            {
                return
                    _testValue >= KeyboardEditingTestValue.LeftArrow &&
                    _testValue <= KeyboardEditingTestValue.PageDownControl;
            }
        }

        /// <summary>Whether this action is expected to only have effects on rich controls.</summary>
        public bool IsRichTextOnly
        {
            get
            {
                return
                    (_testValue == KeyboardEditingTestValue.BoldCommandKeys) ||
                    (_testValue == KeyboardEditingTestValue.CenterJustifyCommandKeys) ||
                    (_testValue == KeyboardEditingTestValue.DecreaseIndentationCommandKeys) ||
                    (_testValue == KeyboardEditingTestValue.IncreaseIndentationCommandKeys) ||
                    (_testValue == KeyboardEditingTestValue.ItalicCommandKeys) ||
                    (_testValue == KeyboardEditingTestValue.LeftJustifyCommandKeys) ||
                    (_testValue == KeyboardEditingTestValue.RightJustifyCommandKeys) ||
                    (_testValue == KeyboardEditingTestValue.UnderlineCommandKeys);
            }
        }

        /// <summary>Whether this action is an undo unit boundary.</summary>
        public bool IsUndoBoundary
        {
            get
            {
                return
                    _testValue == KeyboardEditingTestValue.Enter ||
                    _testValue == KeyboardEditingTestValue.EnterShift ||
                    _testValue == KeyboardEditingTestValue.Delete ||
                    _testValue == KeyboardEditingTestValue.DeleteControl ||
                    _testValue == KeyboardEditingTestValue.Backspace ||
                    _testValue == KeyboardEditingTestValue.BackspaceControl ||
                    IsNavigationAction ||
                    Command != null;
            }
        }

        /// <summary>Value for keyboard editing case.</summary>
        public KeyboardEditingTestValue TestValue
        {
            get { return _testValue; }
        }

        /// <summary>Keyboard editing actions that change character format</summary>
        public static KeyboardEditingData[] CharacterFormattingValues
        {
            get
            {
                System.Collections.Generic.List<KeyboardEditingData> values;

                values = new System.Collections.Generic.List<KeyboardEditingData>(Values.Length);
                foreach (KeyboardEditingData data in Values)
                {
                    if (data.TestValue == KeyboardEditingTestValue.BoldCommandKeys ||
                        data.TestValue == KeyboardEditingTestValue.ItalicCommandKeys ||
                        data.TestValue == KeyboardEditingTestValue.UnderlineCommandKeys ||
                        data.TestValue == KeyboardEditingTestValue.IncreaseFontSizeCommandKeys ||
                        data.TestValue == KeyboardEditingTestValue.DecreaseFontSizeCommandKeys)
                    {
                        values.Add(data);
                    }
                }
                return values.ToArray();
            }
        }

        /// <summary>Keyboard editing actions that are associated with commands.</summary>
        public static KeyboardEditingData[] CommandValues
        {
            get
            {
                System.Collections.Generic.List<KeyboardEditingData> values;

                values = new System.Collections.Generic.List<KeyboardEditingData>(Values.Length);
                foreach (KeyboardEditingData data in Values)
                {
                    if (data.Command != null)
                    {
                        values.Add(data);
                    }
                }

                return values.ToArray();
            }
        }

        /// <summary>Keyboard editing actions that modify content.</summary>
        public static KeyboardEditingData[] EditingValues
        {
            get
            {
                System.Collections.Generic.List<KeyboardEditingData> values;

                values = new System.Collections.Generic.List<KeyboardEditingData>(Values.Length);
                foreach (KeyboardEditingData data in Values)
                {
                    if (!data.IsNavigationAction)
                    {
                        values.Add(data);
                    }
                }
                return values.ToArray();
            }
        }

        /// <summary>Keyboard editing actions that move the caret or selection.</summary>
        public static KeyboardEditingData[] NavigationValues
        {
            get
            {
                System.Collections.Generic.List<KeyboardEditingData> values;

                values = new System.Collections.Generic.List<KeyboardEditingData>(Values.Length);
                foreach (KeyboardEditingData data in Values)
                {
                    if (data.IsNavigationAction)
                    {
                        values.Add(data);
                    }
                }
                return values.ToArray();
            }
        }

        /// <summary>Keyboard editing actions that act as undo boundary units.</summary>
        public static KeyboardEditingData[] UndoBoundayValues
        {
            get
            {
                System.Collections.Generic.List<KeyboardEditingData> values;

                values = new System.Collections.Generic.List<KeyboardEditingData>(Values.Length);
                foreach (KeyboardEditingData data in Values)
                {
                    if (data.IsUndoBoundary)
                    {
                        values.Add(data);
                    }
                }

                return values.ToArray();
            }
        }

        /// <summary>Keyboard editing actions that create undo units.</summary>
        public static KeyboardEditingData[] PageNavigationValues
        {
            get
            {
                System.Collections.Generic.List<KeyboardEditingData> values;

                values = new System.Collections.Generic.List<KeyboardEditingData>(Values.Length);
                foreach (KeyboardEditingData data in Values)
                {
                    if (data.TestValue == KeyboardEditingTestValue.PageDown ||
                        data.TestValue == KeyboardEditingTestValue.PageUp ||
                        data.TestValue == KeyboardEditingTestValue.PageUpShift ||
                        data.TestValue == KeyboardEditingTestValue.PageDownShift)
                    {
                        values.Add(data);
                    }
                }
                return values.ToArray();
            }
        }

        /// <summary>Keys for Delete, Backspace, Home, End</summary>
        public static KeyboardEditingData[] DeleteBackSpaceHomeEndValues
        {
            get
            {
                System.Collections.Generic.List<KeyboardEditingData> values;

                values = new System.Collections.Generic.List<KeyboardEditingData>(Values.Length);
                foreach (KeyboardEditingData data in Values)
                {
                    if (data.TestValue == KeyboardEditingTestValue.Delete ||
                        data.TestValue == KeyboardEditingTestValue.Backspace ||
                        data.TestValue == KeyboardEditingTestValue.Home ||
                        data.TestValue == KeyboardEditingTestValue.End)
                    {
                        values.Add(data);
                    }
                }
                return values.ToArray();
            }
        }

        /// <summary>Keyboard editing actions that create undo units.</summary>
        public static KeyboardEditingData[] UndoTypingValues
        {
            get
            {
                System.Collections.Generic.List<KeyboardEditingData> values;

                values = new System.Collections.Generic.List<KeyboardEditingData>(Values.Length);
                foreach (KeyboardEditingData data in Values)
                {
                    if (data.TestValue == KeyboardEditingTestValue.Alphabetic ||
                        data.TestValue == KeyboardEditingTestValue.AlphabeticShift ||
                        data.TestValue == KeyboardEditingTestValue.Numeric ||
                        data.TestValue == KeyboardEditingTestValue.NumericShift ||
                        data.TestValue == KeyboardEditingTestValue.DeadKeys)
                    {
                        values.Add(data);
                    }
                }
                return values.ToArray();
            }
        }

        /// <summary>Interesting values for testing text selection.</summary>
        public static KeyboardEditingData[] Values = new KeyboardEditingData[] {
            ForCallback (KeyboardEditingTestValue.Alphabetic,       ExecuteAlphabetic, delegate(KeyboardEditingState s) { VerifyInsertion(s, "a"); }),
            ForCallback (KeyboardEditingTestValue.AlphabeticShift,  ExecuteAlphabeticShift, delegate(KeyboardEditingState s) { VerifyInsertion(s, "A"); }),
            ForCallback (KeyboardEditingTestValue.Numeric,          ExecuteNumeric, delegate(KeyboardEditingState s) { VerifyInsertion(s, "5"); }),
            ForCallback (KeyboardEditingTestValue.NumericShift,     ExecuteNumericShift, delegate(KeyboardEditingState s) { VerifyInsertion(s, "%"); }),
            ForCallback (KeyboardEditingTestValue.DeadKeys,         ExecuteDeadKeys, delegate(KeyboardEditingState s) { VerifyInsertion(s, "�"); }),
            ForValue    (KeyboardEditingTestValue.AltNumpadKeys,    "", null, null),
            ForValue    (KeyboardEditingTestValue.Enter,            "{ENTER}", EditingCommands.EnterParagraphBreak, VerifyEnter),
            ForValue    (KeyboardEditingTestValue.EnterShift,       "+{ENTER}", EditingCommands.EnterLineBreak, null),
            ForValue    (KeyboardEditingTestValue.Delete,           "{DELETE}", EditingCommands.Delete, VerifyDelete),
            ForValue    (KeyboardEditingTestValue.DeleteControl,    "^{DELETE}", EditingCommands.DeleteNextWord, null),
            ForValue    (KeyboardEditingTestValue.Backspace,        "{BACKSPACE}", EditingCommands.Backspace, VerifyBackspace),
            ForValue    (KeyboardEditingTestValue.BackspaceShift,   "+{BACKSPACE}", EditingCommands.Backspace, null),
            ForValue    (KeyboardEditingTestValue.BackspaceControl, "^{BACKSPACE}", EditingCommands.DeletePreviousWord, null),
            ForValue    (KeyboardEditingTestValue.Space,            "{SPACE}", null, delegate(KeyboardEditingState s) { VerifyInsertion(s, " "); }),
            ForValue    (KeyboardEditingTestValue.Tab,              "{TAB}", EditingCommands.TabForward, null), //Add VerifyTab once the verification logic is fixed
            ForValue    (KeyboardEditingTestValue.TabShift,         "+{TAB}", EditingCommands.TabBackward, null),
            ForCallback (KeyboardEditingTestValue.ControlShiftLeft,  ExecuteControlShiftLeft,  delegate(KeyboardEditingState s) { VerifyControlShift(s, false); }),
            ForCallback (KeyboardEditingTestValue.ControlShiftRight, ExecuteControlShiftRight, delegate(KeyboardEditingState s) { VerifyControlShift(s, true); }),
            ForValue    (KeyboardEditingTestValue.LeftArrow,        "{LEFT}", EditingCommands.MoveLeftByCharacter, VerifyLeft),
            ForValue    (KeyboardEditingTestValue.RightArrow,       "{RIGHT}", EditingCommands.MoveRightByCharacter, VerifyRight),
            ForValue    (KeyboardEditingTestValue.UpArrow,          "{UP}", EditingCommands.MoveUpByLine, VerifyUp),
            ForValue    (KeyboardEditingTestValue.DownArrow,        "{DOWN}", EditingCommands.MoveDownByLine, VerifyDown),
            ForValue    (KeyboardEditingTestValue.Home,             "{HOME}", EditingCommands.MoveToLineStart, VerifyHome),
            ForValue    (KeyboardEditingTestValue.End,              "{END}", EditingCommands.MoveToLineEnd, VerifyEnd),
            ForValue    (KeyboardEditingTestValue.PageUp,           "{PGUP}", EditingCommands.MoveUpByPage, delegate(KeyboardEditingState s) { VerifyPageUpDown(s, false, false); }),
            ForValue    (KeyboardEditingTestValue.PageDown,         "{PGDN}", EditingCommands.MoveDownByPage, delegate(KeyboardEditingState s) { VerifyPageUpDown(s, true, false); }),
            ForValue    (KeyboardEditingTestValue.LeftArrowShift,   "+{LEFT}", EditingCommands.SelectLeftByCharacter, VerifyLeftShift),
            ForValue    (KeyboardEditingTestValue.RightArrowShift,  "+{RIGHT}", EditingCommands.SelectRightByCharacter, VerifyRightShift),
            ForValue    (KeyboardEditingTestValue.UpArrowShift,     "+{UP}", EditingCommands.SelectUpByLine, null),
            ForValue    (KeyboardEditingTestValue.DownArrowShift,   "+{DOWN}", EditingCommands.SelectDownByLine, null),
            ForValue    (KeyboardEditingTestValue.HomeShift,        "+{HOME}", EditingCommands.SelectToLineStart, null),
            ForValue    (KeyboardEditingTestValue.EndShift,         "+{END}", EditingCommands.SelectToLineEnd, null),
            ForValue    (KeyboardEditingTestValue.PageUpShift,      "+{PGUP}", EditingCommands.SelectUpByPage, delegate(KeyboardEditingState s) { VerifyPageUpDown(s, false, true); }),
            ForValue    (KeyboardEditingTestValue.PageDownShift,    "+{PGDN}", EditingCommands.SelectDownByPage, delegate(KeyboardEditingState s) { VerifyPageUpDown(s, true, true); }),
            ForValue    (KeyboardEditingTestValue.LeftArrowControl, "^{LEFT}", EditingCommands.MoveLeftByWord, VerifyLeftControl),
            ForValue    (KeyboardEditingTestValue.RightArrowControl,"^{RIGHT}", EditingCommands.MoveRightByWord, VerifyRightControl),
            ForValue    (KeyboardEditingTestValue.UpArrowControl,   "^{UP}", EditingCommands.MoveUpByParagraph, null),
            ForValue    (KeyboardEditingTestValue.DownArrowControl, "^{DOWN}", EditingCommands.MoveDownByParagraph, null),
            ForValue    (KeyboardEditingTestValue.PageUpControl,    "^{PGUP}", EditingCommands.MoveUpByPage, null),
            ForValue    (KeyboardEditingTestValue.PageDownControl,  "^{PGDN}", EditingCommands.MoveDownByPage, null),
            ForValue    (KeyboardEditingTestValue.HomeControl,      "^{HOME}", EditingCommands.MoveToDocumentStart, null),
            ForValue    (KeyboardEditingTestValue.EndControl,       "^{END}", EditingCommands.MoveToDocumentEnd, null),
            ForValue    (KeyboardEditingTestValue.LeftArrowControlShift,          "^+{LEFT}", EditingCommands.SelectLeftByWord, VerifyLeftControlShift),
            ForValue    (KeyboardEditingTestValue.RightArrowControlShift,         "^+{RIGHT}", EditingCommands.SelectRightByWord, VerifyRightControlShift),
            ForValue    (KeyboardEditingTestValue.IncreaseIndentationCommandKeys, "^m", EditingCommands.IncreaseIndentation, null),
            ForValue    (KeyboardEditingTestValue.DecreaseIndentationCommandKeys, "^+m", EditingCommands.DecreaseIndentation, null),
            ForValue    (KeyboardEditingTestValue.UndoCommandKeys,  "^z", ApplicationCommands.Undo, null),
            ForValue    (KeyboardEditingTestValue.RedoCommandKeys,  "^y", ApplicationCommands.Redo, null),
            ForValue    (KeyboardEditingTestValue.BoldCommandKeys,  EditingCommandData.ToggleBold.KeyboardShortcut, EditingCommands.ToggleBold, VerifyBold),
            ForValue    (KeyboardEditingTestValue.CopyCommandKeys,  "^c", ApplicationCommands.Copy, delegate(KeyboardEditingState s) { VerifyCopy(s); }),
            ForValue    (KeyboardEditingTestValue.UnderlineCommandKeys, EditingCommandData.ToggleUnderline.KeyboardShortcut, EditingCommands.ToggleUnderline, VerifyUnderline),
            ForValue    (KeyboardEditingTestValue.ItalicCommandKeys, EditingCommandData.ToggleItalic.KeyboardShortcut, EditingCommands.ToggleItalic, VerifyItalic),
            ForCommand  (KeyboardEditingTestValue.CenterJustifyCommandKeys, EditingCommands.AlignCenter, null),
            ForCommand  (KeyboardEditingTestValue.RightJustifyCommandKeys, EditingCommands.AlignRight, null),
            ForCommand  (KeyboardEditingTestValue.LeftJustifyCommandKeys, EditingCommands.AlignLeft, null),
            ForValue    (KeyboardEditingTestValue.PasteCommandKeys, "^v", ApplicationCommands.Paste, delegate(KeyboardEditingState s) { VerifyPaste(s); }),
            ForValue    (KeyboardEditingTestValue.CutCommandKeys,   "^x", ApplicationCommands.Cut, delegate(KeyboardEditingState s) { VerifyCut(s); }),
            ForValue    (KeyboardEditingTestValue.IncreaseFontSizeCommandKeys, "^]", EditingCommands.IncreaseFontSize, delegate(KeyboardEditingState s) { VerifyIncreaseDecreaseFontSize(s, true); }),
            ForValue    (KeyboardEditingTestValue.DecreaseFontSizeCommandKeys, "^[", EditingCommands.DecreaseFontSize, delegate(KeyboardEditingState s) { VerifyIncreaseDecreaseFontSize(s, false); }),
        };

        #endregion Public properties.

        #region Private methods.

        /// <summary>
        /// Deletes the selection in the KeyboardEditingState container,
        /// and calculates the resulting insertion index.
        /// </summary>
        /// <param name="state">State to delete selection in.</param>
        /// <param name="insertionIndex">New insertion index after selection.</param>
        private static void DeleteSelection(KeyboardEditingState state, out int insertionIndex)
        {
            ArrayTextContainer container;           // Reference to container for content.
            int firstDeletionIndex;                 // Index to first symbol to delete.
            int lastDeletionIndex;                  // Index to last symbol to delete.
            int deletionIndex;                      // Index used to delete content.

            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            // Consider the case when there is no selection to delete.
            if (state.SelectionStartDistance == state.SelectionEndDistance)
            {
                insertionIndex = state.SelectionStartDistance;
                return;
            }

            // Capture selection state in local variables.
            container = state.Container;
            firstDeletionIndex = state.SelectionStartDistance;
            lastDeletionIndex = state.SelectionEndDistance - 1;

            // Delete every element, moving backward. Characters
            // are simply removed; inlines are preserved unless
            // they become empty; blocks are merged.
            deletionIndex = lastDeletionIndex;
            while (deletionIndex >= firstDeletionIndex)
            {
                switch (container[deletionIndex].Context)
                {
                    case TextPointerContext.Text:
                        container.DeleteContent(deletionIndex, deletionIndex);
                        deletionIndex--;
                        lastDeletionIndex--;
                        break;
                    case TextPointerContext.ElementStart:
                        if (deletionIndex - 1 > 0 &&
                            container[deletionIndex - 1].Context == TextPointerContext.ElementEnd &&
                            container[deletionIndex - 1].Element.GetType() == container[deletionIndex].Element.GetType())
                        {
                            container.DeleteContent(deletionIndex - 1, deletionIndex);
                            deletionIndex -= 2;
                            lastDeletionIndex -= 2;
                        }
                        break;
                    default:
                        deletionIndex--;
                        break;
                }
            }

            // Adjust index for content insertion.
            insertionIndex = firstDeletionIndex;
        }

        /// <summary>Describes the specified ArrayTextContainer.</summary>
        /// <param name='container'>Container to describe.</param>
        /// <returns>A multiline description of the container contents.</returns>
        private string DescribeContainer(ArrayTextContainer container)
        {
            StringBuilder builder;

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            builder = new StringBuilder();
            for (int i = 0; i < container.Count; i++)
            {
                builder.Append(i);
                builder.Append("\t");
                builder.Append(container[i].ToString());
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        /// <summary>Executes the Alphabetic typing action.</summary>
        private static void ExecuteAlphabetic(UIElementWrapper wrapper, SimpleHandler handler,
            bool preferCommand)
        {
            ExecuteKeyboardBased("a", "a", wrapper, handler, preferCommand);
        }

        /// <summary>Executes the Shift+Alphabetic typing action.</summary>
        private static void ExecuteAlphabeticShift(UIElementWrapper wrapper, SimpleHandler handler,
            bool preferCommand)
        {
            ExecuteKeyboardBased("+a", "A", wrapper, handler, preferCommand);
        }

        /// <summary>Executes the left-hand-side ctrl+shift pressing action.</summary>
        private static void ExecuteControlShiftLeft(UIElementWrapper wrapper, SimpleHandler handler,
            bool preferCommand)
        {
            KeyboardInput.PressVirtualKey(Win32.VK_LSHIFT);
            KeyboardInput.PressVirtualKey(Win32.VK_LCONTROL);
            KeyboardInput.ReleaseVirtualKey(Win32.VK_LSHIFT);
            KeyboardInput.ReleaseVirtualKey(Win32.VK_LCONTROL);

            if (handler != null)
            {
                QueueHelper.Current.QueueDelegate(handler);
            }
        }

        /// <summary>Executes the right-hand-side ctrl+shift pressing action.</summary>
        private static void ExecuteControlShiftRight(UIElementWrapper wrapper, SimpleHandler handler,
            bool preferCommand)
        {            
            KeyboardInput.PressVirtualKey(Win32.VK_RSHIFT);
            KeyboardInput.PressVirtualKey(Win32.VK_RCONTROL);
            KeyboardInput.ReleaseVirtualKey(Win32.VK_RSHIFT);
            KeyboardInput.ReleaseVirtualKey(Win32.VK_RCONTROL);            

            if (handler != null)
            {                
                QueueHelper.Current.QueueDelegate(handler);
            }
        }

        /// <summary>Executes the dead key typing action.</summary>
        private static void ExecuteDeadKeys(UIElementWrapper wrapper, SimpleHandler handler,
            bool preferCommand)
        {
            KeyboardInput.SetActiveInputLocale(InputLocaleData.SpanishArgentina.Identifier);
            QueueHelper.Current.QueueDelegate(
                new System.Threading.TimerCallback(TypeDeadKeys),
                new object[] { handler });
        }

        /// <summary>Executes a keyboard-based typing action.</summary>
        private static void ExecuteKeyboardBased(string textToType, string textToInsert,
            UIElementWrapper wrapper, SimpleHandler handler, bool preferCommand)
        {
            if (preferCommand)
            {
                // The permission is required to access the keyboard primary device.
                new System.Security.Permissions.SecurityPermission(
                    System.Security.Permissions.PermissionState.Unrestricted)
                    .Assert();

                System.Windows.Input.TextCompositionEventArgs args;

                args = new TextCompositionEventArgs(Keyboard.PrimaryDevice,
                    new TextComposition(InputManager.Current, wrapper.Element, textToInsert));
                args.RoutedEvent = System.Windows.Input.TextCompositionManager.TextInputEvent;
                wrapper.Element.RaiseEvent(args);
            }
            else
            {
                KeyboardInput.TypeString(textToType);
            }

            if (handler != null)
            {
                QueueHelper.Current.QueueDelegate(handler);
            }
        }

        /// <summary>Executes the Numeric typing action.</summary>
        private static void ExecuteNumeric(UIElementWrapper wrapper, SimpleHandler handler,
            bool preferCommand)
        {
            ExecuteKeyboardBased("5", "5", wrapper, handler, preferCommand);
        }

        /// <summary>Executes the Shift+Numeric typing action.</summary>
        private static void ExecuteNumericShift(UIElementWrapper wrapper, SimpleHandler handler,
            bool preferCommand)
        {
            ExecuteKeyboardBased("+5", "%", wrapper, handler, preferCommand);
        }

        /// <summary>
        /// Finds the following caret index in a given direction, for the
        /// specified text container.
        /// </summary>
        /// <param name="arrayTextContainer">Container to search.</param>
        /// <param name="startDistance">Distance at which to start search.</param>
        /// <param name="directionInLeftToRight">
        /// Direction in which to look for next caret index (will be flipped
        /// for right-to-left controls).
        /// </param>
        /// <param name="isExtending">Whether selection is extending to this position.</param>
        /// <returns>
        /// The index of the next caret position (possibly at container.Length), or
        /// -1 if there are no futher caret indices in the specified
        /// direction.
        /// </returns>
        private static int FindNextCaretIndex(ArrayTextContainer arrayTextContainer,
            int startDistance, LogicalDirection directionInLeftToRight, bool isExtending)
        {
            int moveDistance;               // Delta for movement.
            FlowDirection flowDirection;    // FlowDirection on caret.
            int result;

            System.Diagnostics.Trace.Assert(arrayTextContainer != null);

            result = startDistance;
            moveDistance = (directionInLeftToRight == LogicalDirection.Forward)? 1 : -1;

            // Account for right-to-left movement.
            flowDirection = (FlowDirection)arrayTextContainer.GetValue(startDistance,
                Control.FlowDirectionProperty);
            if (flowDirection == FlowDirection.RightToLeft)
            {
                moveDistance = -moveDistance;
            }

            // Move the result in the correct direction.
            while(true)
            {
                result += moveDistance;
                if (result < 0 || result > arrayTextContainer.Count)
                {
                    return -1;
                }
                if (IsCaretBoundary(arrayTextContainer, result, isExtending))
                {
                    return result;
                }
            }
        }

        /// <summary>
        /// Finds the following caret index in a given direction, for the
        /// specified string.
        /// </summary>
        /// <param name="element">A TextBox or PasswordBox to search.</param>
        /// <param name="startIndex">Index in which to start search.</param>
        /// <param name="directionInLeftToRight">
        /// Direction in which to look for next caret index (will be flipped
        /// for right-to-left controls).
        /// </param>
        /// <returns>
        /// The index of the next caret position (possibly at text.Length), or
        /// -1 if there are no futher caret indices in the specified
        /// direction.
        /// </returns>
        private static int FindNextCaretIndex(UIElement element, int startIndex,
            LogicalDirection directionInLeftToRight)
        {
            LogicalDirection direction;
            Control control;
            string text;

            System.Diagnostics.Trace.Assert(element != null);

            control = (Control)element;

            text = new UIElementWrapper(element).TextForNavigation;

            direction = directionInLeftToRight;
            if (control.FlowDirection == FlowDirection.RightToLeft)
            {
                direction = (direction == LogicalDirection.Backward)?
                    LogicalDirection.Forward : LogicalDirection.Backward;
            }

            return FindNextCaretIndex(text, startIndex, direction,
                control.Language, control.FlowDirection, control.FontFamily);
        }

        /// <summary>
        /// Determines if the only thing between a given pointer in a
        /// container and a block boundary are insignificant tags.
        /// </summary>
        private static bool IsNearBlock(ArrayTextContainer container,
            int index, LogicalDirection direction)
        {
            DirectionHelper helper;

            helper = new DirectionHelper(direction);
            while (true)
            {
                ArrayTextSymbol adjacentSymbol;
                if (index == -1 || index > container.Count)
                {
                    return false;
                }

                adjacentSymbol = container[index + helper.DirectionOffset];
                if (adjacentSymbol.Context == TextPointerContext.Text)
                {
                    return false;
                }
                if (adjacentSymbol.Context == helper.ExitElementContext &&
                    adjacentSymbol.Element is Paragraph)
                {
                    return true;
                }
                index += helper.MoveDistance;
            }
        }

        /// <summary>Finds the next word boundary index in the given container.</summary>
        /// <param name="container">Container to examine.</param>
        /// <param name="flowDirection">Direction in which text is laid out at the start index.</param>
        /// <param name="startIndex">Index at which to start searching.</param>
        /// <param name="directionInLeftToRight">
        /// Direction in which to search assumine left-to-right text.
        /// </param>
        /// <param name="isStartValid">
        /// Whether the index at which to start searching should be evaluated
        /// as a potential word boundary.
        /// </param>
        /// <param name="extendingSelection">
        /// Whether the search should consider selection-extension word
        /// boundaries as well (which include the after-end-of-paragraph
        /// position in rich text).
        /// </param>
        /// <returns></returns>
        private static int FindNextWordBoundaryIndex(ArrayTextContainer container,
            FlowDirection flowDirection, int startIndex,
            LogicalDirection directionInLeftToRight, bool isStartValid,
            bool extendingSelection)
        {
            DirectionHelper helper;
            int result;

            helper = new DirectionHelper(directionInLeftToRight);
            if (flowDirection == FlowDirection.RightToLeft)
            {
                helper = new DirectionHelper(helper.OppositeDirection);
            }

            result = startIndex;

            if (!isStartValid)
            {
                // Move until we are in a position that normalizes to something
                // different.
                int startInsertionPosition;
                int resultInsertionPosition;
                bool blockBoundaryCrossed;

                startInsertionPosition = container.GetInsertionPosition(startIndex,
                    helper.Direction, extendingSelection, out blockBoundaryCrossed);
                do
                {
                    result += helper.MoveDistance;
                    if (result < 0 || result > container.Count)
                    {
                        return -1;
                    }
                    resultInsertionPosition = container.GetInsertionPosition(
                        result, helper.Direction, extendingSelection, out blockBoundaryCrossed);
                } while (resultInsertionPosition == startInsertionPosition);
            }

            while (result >= 0 && result <= container.Count)
            {
                // The edges of the text are word boundaries.
                if (result == 0 || result == container.Count)
                {
                    result = container.GetInsertionPosition(result, helper.OppositeDirection,
                        extendingSelection);
                    if (!IsCaretBoundary(container, result, extendingSelection))
                    {
                        result = container.GetInsertionPosition(result, helper.Direction,
                            extendingSelection);
                    }
                    return result;
                }

                // Look for structural stop positions.
                if (IsNearBlock(container, result, LogicalDirection.Backward))
                {
                    // Move forward until adjacent to text.
                    while ((container[result].Context != TextPointerContext.Text) &&
                        !(container[result].Context == TextPointerContext.ElementEnd &&
                          container[result].Element is Paragraph) &&
                        !(container[result].Context == TextPointerContext.ElementEnd &&
                          container[result].Element is Run))
                    {
                        result++;
                    }

                    return result;
                }
                else if (IsNearBlock(container, result, LogicalDirection.Forward))
                {
                    // Move backward until within run.
                    while ((container[result - 1].Context != TextPointerContext.Text) &&
                        !(container[result - 1].Context == TextPointerContext.ElementStart &&
                          container[result - 1].Element is Paragraph) &&
                        !(container[result - 1].Context == TextPointerContext.ElementStart &&
                          container[result - 1].Element is Run))
                    {
                        result--;
                    }
                    return result;
                }
                else if (container[result - 1].Context == TextPointerContext.Text &&
                    container[result].Context == TextPointerContext.Text)
                {
                    char before, after, next;

                    before = container[result - 1].Character;
                    after = container[result].Character;
                    next = (helper.Direction == LogicalDirection.Forward)? after : before;

                    // Consider characters that never merge with anything.
                    if (after == '\r')
                    {
                        return result;
                    }
                    if (IsCharStandaloneWord(next))
                    {
                        return result;
                    }

                    // Consider trailing space as belonging to previous word.
                    if (char.IsWhiteSpace(before) && !char.IsWhiteSpace(after))
                    {
                        return result;
                    }

                    if (char.IsPunctuation(before) != char.IsPunctuation(after) &&
                        !char.IsWhiteSpace(after))
                    {
                        return result;
                    }
                }

                result += helper.MoveDistance;
            }
            return -1;
        }

        /// <summary>
        /// Finds the following word boundary in a given direction, for the
        /// specified string.
        /// </summary>
        /// <param name="element">A TextBox or PasswordBox to search.</param>
        /// <param name="startIndex">Index in which to start search.</param>
        /// <param name="directionInLeftToRight">
        /// Direction in which to look for next caret index (will be flipped
        /// for right-to-left controls).
        /// </param>
        /// <param name="isStartValid">Whether startIndex is an acceptable position.</param>
        /// <param name="extendingSelection">
        /// Whether the search should consider selection-extension word
        /// boundaries as well (which include the after-end-of-paragraph
        /// position in rich text).
        /// </param>
        /// <returns>
        /// The index of the next word boundary index (possibly at text.Length), or
        /// -1 if there are no futher word boundary indices in the specified
        /// direction.
        /// </returns>
        private static int FindNextWordBoundaryIndex(UIElement element, int startIndex,
            LogicalDirection directionInLeftToRight, bool isStartValid, bool extendingSelection)
        {
            string text;

            System.Diagnostics.Trace.Assert(element != null);

            text = new UIElementWrapper(element).TextForNavigation;
            return FindNextWordBoundaryIndex(new ArrayTextContainer(text),
                ((Control)element).FlowDirection, startIndex, directionInLeftToRight,
                isStartValid, extendingSelection);
        }

        /// <summary>
        /// Finds the following caret index in a given direction, for the
        /// specified string.
        /// </summary>
        /// <param name="text">Text that caret moves in.</param>
        /// <param name="startIndex">Index in which to start search.</param>
        /// <param name="direction">Direction in which to look for next caret index.</param>
        /// <param name="language">Default XmlLanguage for string.</param>
        /// <param name="flowDirection">Direction in which text is laid out.</param>
        /// <param name="fontFamily">Font family used to render text.</param>
        /// <returns>
        /// The index of the next caret position (possible text.Length), or
        /// -1 if there are no futher caret indices in the specified
        /// direction.
        /// </returns>
        private static int FindNextCaretIndex(string text, int startIndex,
            LogicalDirection direction, XmlLanguage language, FlowDirection flowDirection,
            System.Windows.Media.FontFamily fontFamily)
        {
            List<int> caretPositions;   // List of caret positions.
            int listIndex;              // Index into caretPositions list.
            int result;                 // Next caret position from startIndex.

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (language == null)
            {
                throw new ArgumentNullException("language");
            }

            result = -1;
            caretPositions = TextAnalyzer.ListValidCaretPositions(text, language.GetEquivalentCulture(), flowDirection, fontFamily);
            listIndex = 0;

            if (direction == LogicalDirection.Forward)
            {
                // Look at all caret positions for one that is beyond our
                // start index.
                do
                {
                    if (caretPositions[listIndex] > startIndex)
                    {
                        return caretPositions[listIndex];
                    }
                    else
                    {
                        listIndex++;
                    }
                } while (listIndex < caretPositions.Count);

                // No further position found; return -1.
                System.Diagnostics.Trace.Assert(result == -1);
            }
            else
            {
                // Look at all caret positions, and return the last one
                // before the start index.
                do
                {
                    if (caretPositions[listIndex] < startIndex)
                    {
                        result = caretPositions[listIndex];
                        listIndex++;
                    }
                    else
                    {
                        // No need to look any further.
                        break;
                    }
                } while (listIndex < caretPositions.Count);
            }

            return result;
        }

        /// <summary>
        /// Creates a new KeyboardEditingData for the given value.
        /// </summary>
        private static KeyboardEditingData ForValue(KeyboardEditingTestValue testValue,
            string executionString, RoutedCommand command, VerifyCallback verify)
        {
            KeyboardEditingData result;

            result = new KeyboardEditingData();
            result._testValue = testValue;
            result._executionTypeString = executionString;
            result._command = command;
            result._verify = verify;

            return result;
        }

        /// <summary>
        /// Creates a new KeyboardEditingData for the given value.
        /// </summary>
        private static KeyboardEditingData ForCallback(KeyboardEditingTestValue testValue,
            ExecutionCallback execute, VerifyCallback verify)
        {
            KeyboardEditingData result;

            result = new KeyboardEditingData();
            result._testValue = testValue;
            result._executionTypeString = String.Empty;
            result._execute = execute;
            result._verify = verify;

            return result;
        }

        /// <summary>
        /// Creates a new KeyboardEditingData for the given value associated with
        /// a command.
        /// </summary>
        private static KeyboardEditingData ForCommand(KeyboardEditingTestValue testValue,
            RoutedCommand command, VerifyCallback verify)
        {
            KeyboardEditingData result;

            result = new KeyboardEditingData();
            result._testValue = testValue;
            result._command = command;
            result._executionTypeString = String.Empty;
            result._verify = verify;

            return result;
        }

        /// <summary>
        /// Gets the character index for the last visible insertion point on
        /// the specified line.
        /// </summary>
        private static int GetLastIPIndexOnLine(TextBox textbox, int lineIndex)
        {
            if (textbox == null)
            {
                throw new ArgumentNullException("textbox");
            }

            if (TextSelectionData.IsLastLine(textbox, lineIndex))
            {
                return textbox.Text.Length;
            }
            else
            {
                int result;

                result = textbox.GetCharacterIndexFromLineIndex(lineIndex) +
                    textbox.GetLineLength(lineIndex);
                if (TextSelectionData.IsDelimitedLine(textbox, lineIndex))
                {
                    result -= 2;
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the line index for the specified position.
        /// </summary>
        /// <param name="textbox">TextBox hosting position.</param>
        /// <param name="positionIndex">Index of position.</param>
        /// <param name="direction">Direction of position.</param>
        /// <returns>The line index of the specified position.</returns>
        private static int GetPositionLineIndex(TextBox textbox,
            int positionIndex, LogicalDirection direction)
        {
            int result;

            if (textbox == null)
            {
                throw new ArgumentNullException("textbox");
            }

            result = textbox.GetLineIndexFromCharacterIndex(positionIndex);

            // If the index is on a line boundary and facing backward,
            // it is on the line above rather than the line below.
            if (positionIndex > 0 &&
                textbox.GetLineIndexFromCharacterIndex(positionIndex - 1) == result - 1 &&
                direction == LogicalDirection.Backward)
            {
                result--;
            }
            // If the index is right after a newline, wrapping the line,
            // it is on the line above rather than the line below.
            else if (positionIndex > 2 &&
                textbox.Text.Substring(positionIndex - 2, 2) == "\r\n" &&
                direction == LogicalDirection.Backward)
            {
                result--;
            }

            return result;
        }

        private void InternalVerifyEditing(KeyboardEditingState previousState)
        {
            // No changes expected if the control didn't have focus.
            if (!previousState.IsKeyboardFocused)
            {
                VerifyTextUnchanged(previousState);
                return;
            }

            if (_verify != null)
            {
                _verify(previousState);
            }
            else
            {
                Logger.Current.Log("Verification unimplemented for " + this.TestValue);
            }
        }

        /// <summary>
        /// Checks whether the symbol at the specified index is a caret position.
        /// </summary>
        /// <returns>
        /// true if the symbol at the specified index is a caret position,
        /// false otherwise.
        /// </returns>
        private static bool IsCaretBoundary(ArrayTextContainer arrayTextContainer, int index,
            bool selectionExtension)
        {
            if (arrayTextContainer == null)
            {
                throw new ArgumentNullException("arrayTextContainer");
            }
            if (index < 0 || index > arrayTextContainer.Count)
            {
                throw new ArgumentOutOfRangeException("index", index,
                    "index should be between 0 and " + arrayTextContainer.Count);
            }

            // At index == 0, we are outside of any blocks, so this is never
            // a caret boundary position (except for completely empty documents).
            if (index == 0)
            {
                return arrayTextContainer.Count == 0;
            }

            if (index < arrayTextContainer.Count &&
                arrayTextContainer[index].Context == TextPointerContext.Text)
            {
                string run;
                int runStartIndex;
                List<int> caretPositions;

                arrayTextContainer.GetRun(index, out run, out runStartIndex);
                caretPositions = TextAnalyzer.ListValidCaretPositions(run,
                    ((XmlLanguage)arrayTextContainer.GetValue(index, Control.LanguageProperty)).GetEquivalentCulture(),
                    (FlowDirection)arrayTextContainer.GetValue(index, Control.FlowDirectionProperty),
                    (System.Windows.Media.FontFamily)arrayTextContainer.GetValue(index, Control.FontFamilyProperty));
                return caretPositions.Contains(index - runStartIndex);
            }
            else if (arrayTextContainer[index - 1].Context == TextPointerContext.Text)
            {
                return true;
            }
            else if (
                index < arrayTextContainer.Count &&
                arrayTextContainer[index - 1].Context == TextPointerContext.ElementStart &&
                arrayTextContainer[index].Context == TextPointerContext.ElementEnd &&
                arrayTextContainer[index].Element is Run)
            {
                // An empty run is a valid caret position.
                return true;
            }
            else if (selectionExtension &&
                index == arrayTextContainer.Count &&
                arrayTextContainer[index-1].Context == TextPointerContext.ElementEnd &&
                arrayTextContainer[index-1].Element is Paragraph)
            {
                // If selection is extending, the end of the last paragraph
                // is also a valid position. Note that the 'last paragraph'
                // detection is very crude in this implementation - the
                // last paragraph of the last table cell should be detected but
                // will not.
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether the specified character is always standalone for
        /// word-breaking behavior.
        /// </summary>
        /// <param name="c">Character to examine.</param>
        /// <returns>true if c is considered a word by itself; false otherwise.</returns>
        /// <remarks>
        /// This method compares c to well-known control characters and
        /// languages Avalon does not support advanced word breaking.
        ///
        /// For example, in Japanese and Chinese, words need to be
        /// contextually found in the given text stream; in these cases,
        /// we fall back to considering every character a standalone word.
        /// </remarks>
        private static bool IsCharStandaloneWord(char c)
        {
            TextScript script;

            if (c == '\t')
            {
                return true;
            }

            script = TextScript.GetCharacterScript(c);
            if (script == null)
            {
                return false;
            }
            else
            {
                return
                    script.Name == "CJK Unified Ideographs";
            }
        }

        /// <summary>
        /// Types a sequence of keys including dead keys.
        /// </summary>
        private static void TypeDeadKeys(object objectHandler)
        {
            SimpleHandler handler;  // Handler to call back when finished.

            Logger.Current.Log("Typing dead keys with input locale: " +
                KeyboardInput.GetActiveInputLocaleString());
            KeyboardInput.TypeString(";a");
            handler = objectHandler as SimpleHandler;
            if (handler != null)
            {
                QueueHelper.Current.QueueDelegate(handler);
            }
        }

        /// <summary>
        /// Verify that Bold command was processed correctly
        /// </summary>
        /// <param name="initialState">initial state of the element.</param>
        private static void VerifyBold(KeyboardEditingState initialState)
        {
            object fontWeight, wordFontWeight;
            TextRange wordRange;
            bool isInAWord = false;
            TextSelection currentSelection;

            fontWeight = wordFontWeight = null;

            //Verify that selection indexes didnt change
            Verifier.Verify(initialState.SelectionStartDistance == initialState.Wrapper.SelectionStart,
                "Verifying that SelectionStartDistance didnt change after Bold command", true);
            Verifier.Verify(initialState.SelectionLength == initialState.Wrapper.SelectionLength,
                "Verifying that SelectionLength didnt change after Bold command", true);

            //For RichTextBox verify that the formatting properties are applied.
            if (initialState.Wrapper.IsElementRichText)
            {
                Verifier.Verify(initialState.Text == initialState.Wrapper.Text,
                    "Verifying that contents of RichTextBox havent changed after Bold command", true);

                //Current FontWeight
                fontWeight = initialState.Wrapper.SelectionInstance.GetPropertyValue(TextElement.FontWeightProperty);

                currentSelection = initialState.Wrapper.SelectionInstance;
                if ( (currentSelection.Start.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)&&
                    (currentSelection.Start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text) )
                {
                    isInAWord = true;
                }

                //Current WordFontWeight: If selection is empty and the caret is in the middle
                //of the word, formatting should be applied to the entire word.
                if ((initialState.Wrapper.SelectionInstance.IsEmpty)&&
                    (isInAWord))
                {
                    wordRange = new TextRange(initialState.Wrapper.SelectionInstance.Start, initialState.Wrapper.SelectionInstance.End);
                    wordFontWeight = wordRange.GetPropertyValue(TextElement.FontWeightProperty);
                }

                //When selection is not empty and it has mixed content (includes normal content and bolded content),
                //formatting properties at the selectionstart are used and toggled through out the selection
                if ((FontWeight)initialState.SelectionStartFontWeight == FontWeights.Normal)
                {
                    Verifier.Verify((FontWeight)fontWeight == FontWeights.Bold,
                        "Verifying that FontWeight is Bold after Bold command", true);

                    //Comparison when selection is empty
                    if ((initialState.Wrapper.SelectionInstance.IsEmpty)&&
                        (isInAWord))
                    {
                        Verifier.Verify((FontWeight)wordFontWeight == FontWeights.Bold,
                            "Verifying that FontWeight of the word is Bold after Bold command", true);
                    }
                }
                else if ((FontWeight)initialState.SelectionStartFontWeight == FontWeights.Bold)
                {
                    Verifier.Verify((FontWeight)fontWeight == FontWeights.Normal,
                        "Verifying that FontWeight is toggled to Normal after Bold command", true);

                    //Comparison when selection is empty
                    if ((initialState.Wrapper.SelectionInstance.IsEmpty)&&
                        (isInAWord))
                    {
                        Verifier.Verify((FontWeight)wordFontWeight == FontWeights.Normal,
                            "Verifying that FontWeight of the word is toggled to Normal after Bold command", true);
                    }
                }
            }
            else
            {
                //Contents should not change for TextBox and passwordBox
                VerifyTextUnchanged(initialState);
            }
        }

        /// <summary>
        /// Verify that the Backspace key was processed correctly.
        /// </summary>
        private static void VerifyBackspace(KeyboardEditingState state)
        {
            if (state.IsReadOnly)
            {
                VerifyTextUnchanged(state);
                return;
            }

            if (state.Wrapper.IsElementRichText)
            {
                int actualCaretPosition;

                //BackSpaceIndexStart and BackSpaceIndexEnd are pre-calculated when capturing KeyboardState.
                if (state.BackSpaceIndexEnd> state.BackSpaceIndexStart)
                {
                    actualCaretPosition = TextUtils.GetDistanceFromStart(state.Wrapper.SelectionInstance.Start);
                    
                    //Note: when all the content of a Paragraph is deleted, the empty Run is removed.
                    if (state.Wrapper.SelectionInstance.Start.Parent is Paragraph)
                    {
                        actualCaretPosition++;
                        state.Container.DeleteContent(state.BackSpaceIndexStart - 1, state.BackSpaceIndexEnd);
                    }
                    else if (state.Wrapper.SelectionInstance.Start.Parent is FlowDocument)
                    {
                        //When there is no block in the paragraph, we need to adjust the caret pointer.
                        actualCaretPosition +=2;
                        state.Container.DeleteContent(state.BackSpaceIndexStart - 2, state.BackSpaceIndexEnd + 1);
                    }
                    else
                    {
                        state.Container.DeleteContent(state.BackSpaceIndexStart, state.BackSpaceIndexEnd - 1);
                    }
                    
                    Verifier.Verify(actualCaretPosition == state.BackSpaceIndexStart, "Caret position is not correct!" +
                        " Expected[" + state.BackSpaceIndexStart + "], Actual[" + actualCaretPosition + "]");
                    
                    //no text should be in the container if no paragraph or no run.
                    if (state.Wrapper.SelectionInstance.Start.Parent is FlowDocument)
                    {
                        Verifier.Verify(state.Wrapper.Text == "\r\n" || state.Wrapper.Text == string.Empty, "No text should be in the RichTextBox!");
                    }
                }

                VerifyMatch(state.Container, state.Wrapper.Start, "after Backspace key");
            }
            else
            {
                string text;    // Expected text.

                VerifyEmptySelection(state, "Selection is empty after Backspace key.");

                text = state.Text;
                if (state.SelectionLength > 0)
                {
                    VerifyInsertion(state, "");
                }
                else if (state.SelectionStartDistance == 0)
                {
                    VerifyTextUnchanged(state);
                }
                else
                {
                    bool match;

                    // Backspace deletes a single character, except
                    // for surrogate pairs and newlines.
                    if (state.SelectionStartDistance > 1 &&
                        text.Substring(state.SelectionStartDistance - 2, 2) == "\r\n")
                    {
                        // Remove \r\n pair.
                        text = text.Remove(state.SelectionStartDistance - 2, 2);
                    }
                    else if (state.SelectionStartDistance > 1 &&
                        Char.IsHighSurrogate(text, state.SelectionStartDistance - 2) &&
                        Char.IsLowSurrogate(text, state.SelectionStartDistance - 1))
                    {
                        // Remove surrogate pair.
                        text = text.Remove(state.SelectionStartDistance - 2, 2);
                    }
                    else
                    {
                        // Remove a single character.
                        text = text.Remove(state.SelectionStartDistance - 1, 1);
                    }
                    match = state.Wrapper.Text == text;
                    if (!match)
                    {
                        Verifier.Verify(false, "Text [" + state.Wrapper.Text +
                            "] is as expected [" + text + "]", false);
                    }
                }
            }
        }

        /// <summary> Verifies copy actions </summary>
        private static void VerifyCopy(KeyboardEditingState state)
        {
            string str = state.Wrapper.SelectionInstance.Text;
            IDataObject _clipboardData = Clipboard.GetDataObject();
            IDataObject _clipboardData1 = ClipboardWrapper.GetDataObject();
            string _finalString;

            bool got_val= _clipboardData.GetDataPresent(DataFormats.Text);
            if (got_val)
            {
                _finalString = _clipboardData1.GetData("System.String", true).ToString();
            }
            else
            {
                _finalString  = string.Empty;
            }
            if (state.Wrapper.Element is TextBoxBase)
            {
                Verifier.Verify(_finalString == str, "passed", true);
            }
            else
            {
                Verifier.Verify(_finalString == string.Empty, "clipboard string: ["+_finalString+"]", true);
            }

            //check XAML for RichtextBox           
            if (state.Wrapper.Element is RichTextBox)
            {
                if (state.Wrapper.SelectionInstance.IsEmpty)
                {
                    str = string.Empty;
                }
                else
                {
                    str = XamlUtils.TextRange_GetXml(state.Wrapper.SelectionInstance);
                }
                got_val = _clipboardData.GetDataPresent(DataFormats.Xaml);
                if (got_val)
                {
                    _finalString = _clipboardData1.GetData(DataFormats.Xaml.ToString(), true).ToString();
                }

                Verifier.Verify(_finalString == str, "XAML of copied string and selected text is not same" , true);
            }

            //Verifies that copy doesnt copy text into a cleared clipboard
            if (state.Wrapper.Element is PasswordBox)
            {
                _clipboardData = null;
                _clipboardData = Clipboard.GetDataObject();

                string[] arr = _clipboardData.GetFormats();
                bool _val = false;
                int _count = 0;

                while ((_val == false) && (_count < arr.Length))
                {
                    _val = _clipboardData.GetDataPresent(arr[_count]);
                    _count++;
                }
                Verifier.Verify(_val == false, "The clipboard has data after being cleared and performing cut operation on passwordbox", true);
            }
        }

        /// <summary> Verifies cut actions </summary>
        private static void VerifyCut(KeyboardEditingState state)
        {
            FrameworkElement _element = (FrameworkElement)(state.Wrapper.Element);
            UIElementWrapper _controlWrapper = new UIElementWrapper(_element);
            if (state.Wrapper.Element is TextBoxBase)
            {
                    string _stringBeforeSelection = state.PartBeforeSelection;
                    string _stringAfterSelection = state.PartAfterSelection;

                    string _expectedFinalString = _stringBeforeSelection + _stringAfterSelection;
                    string _initialSelection = (state.SelectedText == "") ? null : state.SelectedText;
                    int _initialStrLength = state.Text.Length;
                    int _selectionLength = state.SelectionLength;
                    int _expectedFinalLength= _initialStrLength - _selectionLength;
                    string _initialXaml = state.SelectionXAMLtext;
                    int _stringLength = state.Text.Length;
                    string _finalClipBoardString;

                    IDataObject _clipboardData = Clipboard.GetDataObject();
                    bool got_val = _clipboardData.GetDataPresent(DataFormats.Text);
                    if (got_val)
                    {
                        _finalClipBoardString = _clipboardData.GetData("System.String", true).ToString();
                    }
                    else
                    {
                        _finalClipBoardString = null;
                    }

                    //Verifies that string lengths are correct after cut
                    int _finalStringLength = _controlWrapper.Text.Length;
                    Verifier.Verify(_finalStringLength == _expectedFinalLength, "String lengths after cut are not equal Expected[" +
                        _expectedFinalLength + "] Actual Length:[" + _finalStringLength + "]", true);

                    //verifies that the strings are as expected
                    string _finalText = _controlWrapper.Text;
                    Verifier.Verify(_finalText == _expectedFinalString, "Strings after cut are not equal Expected[" +
                                        _expectedFinalString + "] Actual Length:[" + _finalText + "]", true);

                    //verifies that clipboard has the right cut text
                    string _clipboardText = _finalClipBoardString;
                    Verifier.Verify(_clipboardText == _initialSelection, "Selected Strings are not equal Expected[" +
                        _initialSelection + "] Actual Length:[" + _clipboardText + "]", true);

                    if (state.Wrapper.Element is RichTextBox)
                    {
                        //Verifies that the cut xaml retains properties in the clipboard
                        string _clipboardXAML="";
                        got_val = _clipboardData.GetDataPresent(DataFormats.Xaml);
                        if (got_val)
                        {
                            _clipboardXAML = _clipboardData.GetData(DataFormats.Xaml.ToString(), true).ToString();
                            Verifier.Verify(_clipboardXAML == _initialXaml, "XAML of copied string and selected text is not same", true);
                        }
                        TextRange _tRange = new TextRange(_controlWrapper.Start, _controlWrapper.SelectionInstance.Start);
                        string _currentXamlForPartBeforeSelection=XamlUtils.TextRange_GetXml( _tRange);
                        _tRange = new TextRange(_controlWrapper.SelectionInstance.End, _controlWrapper.End);
                        string _currentXamlForPartAfterSelection = XamlUtils.TextRange_GetXml(_tRange);

                        Verifier.Verify(ModifyXamlForComparasion(state._XamlForPartAfterSelection) == ModifyXamlForComparasion(_currentXamlForPartAfterSelection), "xaml for after selection not equal after cut", true);
                        Verifier.Verify(ModifyXamlForComparasion(state._XamlForPartBeforeSelection) == ModifyXamlForComparasion(_currentXamlForPartBeforeSelection), "xaml for before selection not equal after cut", true);
                    }
            }
            else if (state.Wrapper.Element is PasswordBox)
            {
                //Verifies that copy doesnt copy text into a cleared clipboard
                IDataObject _clipboardData = null;
                _clipboardData= Clipboard.GetDataObject();

                string[] arr=_clipboardData.GetFormats();
                bool _val=false;
                int _count = 0;

                while ((_val == false) && (_count < arr.Length))
                {
                    _val = _clipboardData.GetDataPresent(arr[_count]);
                    _count++;
                }
                Verifier.Verify(_val == false, "The clipboard has data after being cleared and performing cut operation on passwordbox", true);

                //Verifies that length of paswords remain same
                Verifier.Verify(_controlWrapper.Text.Length == state.Text.Length, "String lenths after cut are nto equal Expected[" +
                state.Text.Length + "] Actual Length:[" + _controlWrapper.Text.Length + "]", true);

                //Verifies that password strings remain same
                string _finalText = _controlWrapper.Text;
                Verifier.Verify(_finalText == state.Wrapper.Text, "Strings after cut are not equal Expected[" +
                                    state.Wrapper.Text + "] Actual Length:[" + _finalText + "]", true);

            }
        }

        static private string ModifyXamlForComparasion(string str)
        {
            string returnStr;
            returnStr = str.Substring(str.IndexOf(">") + 1);
            returnStr = returnStr.Substring(0, returnStr.LastIndexOf("<"));
            returnStr = returnStr.Replace("<Run></Run>", "");
            
            //remove the outer <Paragraph> tag.
            if (returnStr.StartsWith("<Paragraph"))
            {
                returnStr = returnStr.Substring(returnStr.IndexOf(">") + 1);
                returnStr = returnStr.Substring(0, returnStr.LastIndexOf("<"));
            }
            return returnStr; 
        }
        /// <summary> Verifies paste actions </summary>
        private static void VerifyPaste(KeyboardEditingState state)
        {
            FrameworkElement _element = (FrameworkElement)(state.Wrapper.Element);
            UIElementWrapper _controlWrapper = new UIElementWrapper(_element);
            string _finalString;
            int _initialSelectionLength = state.SelectionLength;

            TextPointer _startPtr, _newStartPtr, _pointerToInsertedText, _pointerToEndOfInsertedText;
            int _initialOffset;
            TextRange _tRange;
            string _firstPartOfFinalString, _lastPartOfFinalString, _initialStringWithoutSelection, _finalStringWithoutSelection;

            string _finalClipBoardString;
            IDataObject _clipboardData = Clipboard.GetDataObject();

            bool got_val = _clipboardData.GetDataPresent(DataFormats.Text);
            if (got_val)
            {
                _finalClipBoardString = _clipboardData.GetData("System.String", true).ToString();
            }
            else
            {
                _finalClipBoardString = string.Empty;
            }

            if (_element is PasswordBox)
            {
                //Passwordbox doesnt support paste
                _initialOffset = state.SelectionStartDistance;
                string _initialString = state.Text;
                Verifier.Verify(state.Text == _controlWrapper.Text, " Should be equal Actual [" +
                    _controlWrapper.Text + "] Expected [" + state.Text + "]", true);

            }
            //For passwordbox getting a textpointer throws an error, hence the below is applied only to textboxbase
            if (_element is TextBoxBase)
            {
                //Verifies selected text is correctly inserted
                _startPtr = state.SelectionStart;
                _initialOffset = state.SelectionStartDistance;
                _newStartPtr = _controlWrapper.Start;
                _pointerToInsertedText = _newStartPtr.GetPositionAtOffset(_initialOffset, LogicalDirection.Forward);
                _pointerToEndOfInsertedText = _controlWrapper.SelectionInstance.Start;
                _tRange = new TextRange(_pointerToInsertedText, _pointerToEndOfInsertedText);
                _finalString = _tRange.Text;
                Verifier.Verify(_finalString.TrimEnd(new char[] { '\r', '\n' }) == _finalClipBoardString.TrimEnd(new char[] { '\r', '\n' }), "Strings are not pasted correctly Expected[" +
                        _finalString + "] Actual String:[" + _finalClipBoardString + "]", true);


                //Verifies text without selection
                _tRange = new TextRange(_controlWrapper.Start, _pointerToInsertedText);
                _firstPartOfFinalString = _tRange.Text;
                _tRange = new TextRange(_controlWrapper.SelectionInstance.Start, _controlWrapper.End);
                _lastPartOfFinalString = _tRange.Text;
                _initialStringWithoutSelection = state.PartBeforeSelection + state.PartAfterSelection;
                _finalStringWithoutSelection = _firstPartOfFinalString + _lastPartOfFinalString;

                Verifier.Verify(_initialStringWithoutSelection.TrimEnd(new char[] { '\r', '\n' }) == _finalStringWithoutSelection.TrimEnd(new char[] { '\r', '\n' }), "Strings are not pasted correctly Expected[" +
                        _initialStringWithoutSelection + "] Actual String:[" + _finalStringWithoutSelection + "]", true);

                //verifies that pasted text retains the properties it had in clipboard
                if (_controlWrapper.Element is RichTextBox)
                {
                    ((RichTextBox)_element).Selection.Select(_pointerToInsertedText, _pointerToEndOfInsertedText);
                    string _pastedXaml = XamlUtils.TextRange_GetXml(_controlWrapper.SelectionInstance);

                    string _clipboardXAML = "";
                    got_val = _clipboardData.GetDataPresent(DataFormats.Xaml);
                    if (got_val)
                    {
                        _clipboardXAML = _clipboardData.GetData(DataFormats.Xaml.ToString(), true).ToString();
                        Verifier.Verify(ModifyXamlForComparasion(_clipboardXAML) == ModifyXamlForComparasion(_pastedXaml), "XAML of copied string and selected text is not same", true);
                    }
                }

            }
        }

        /// <summary>
        /// Verify that a Ctrl+Shift pair were processed correctly.
        /// </summary>
        /// <param name="state">Previous state of control.</param>
        /// <param name="right">Whether the right-hand side pair was pressed.</param>
        private static void VerifyControlShift(KeyboardEditingState state, bool right)
        {
            FlowDirection direction;
            FlowDirection actualValue;

            direction = (right) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            if (state.Wrapper.IsElementRichText)
            {
                TextPointer cursor;

                cursor = state.Wrapper.SelectionInstance.Start;
                while (cursor.CompareTo(state.Wrapper.SelectionInstance.End) <= 0)
                {
                    //The FlowDirection is changed only on Paragraph.
                    if (cursor.Paragraph !=null )
                    {
                        actualValue = cursor.Paragraph.FlowDirection;
                        if (direction == FlowDirection.RightToLeft && !KeyboardInput.IsBidiInputLanguageInstalled())
                        {
                            //asume no one has made change to flow direction programatically.
                            Verifier.Verify(FlowDirection.LeftToRight == actualValue,
                                "Element value " + actualValue + " matches expected value " + FlowDirection.LeftToRight + "[BIDI lanugages is not installed]");
                        }
                        else
                        {
                            Verifier.Verify(direction == actualValue,
                                "Element value " + actualValue + " matches expected value " + direction);
                        }
                    }
                    
                    //if (!cursor.MoveToNextContextPosition(LogicalDirection.Forward))
                    if ((cursor = cursor.GetNextContextPosition(LogicalDirection.Forward)) == null)
                    {
                        break;
                    }
                }
            }
            else
            {
                //asume no one has made change to flow direction programatically.
                actualValue = (FlowDirection)state.Wrapper.Element.GetValue(TextBoxBase.FlowDirectionProperty);
                
                if (direction == FlowDirection.RightToLeft && !KeyboardInput.IsBidiInputLanguageInstalled())
                {
                    Verifier.Verify(actualValue == FlowDirection.LeftToRight,
                        "Control value " + actualValue + " matches expected value " + FlowDirection.LeftToRight + "[BIDI lanugages is not installed]");
                }
                else
                {
                    Verifier.Verify(direction == actualValue,
                        "Control value " + actualValue + " matches expected value " + direction);
                }
            }
        }

        /// <summary>
        /// Verify that the Delete key was processed correctly.
        /// </summary>
        private static void VerifyDelete(KeyboardEditingState state)
        {
            if (state.IsReadOnly)
            {
                VerifyTextUnchanged(state);
                return;
            }
            if (state.Wrapper.IsElementRichText)
            {
                int actualCaretPosition;

                //DeleteIndexStart and DeleteIndexEnd are pre-calculated when capturing KeyboardState.
                if (state.DeleteIndexEnd > state.DeleteIndexStart)
                {
                    actualCaretPosition = TextUtils.GetDistanceFromStart(state.Wrapper.SelectionInstance.Start);

                    //Note: when all the content of a Paragraph is deleted, the empty Run is removed.
                    if (state.Wrapper.SelectionInstance.Start.Parent is Paragraph)
                    {
                        actualCaretPosition++;
                        state.Container.DeleteContent(state.DeleteIndexStart - 1, state.DeleteIndexEnd);
                    }
                    else if (state.Wrapper.SelectionInstance.Start.Parent is FlowDocument)
                    {
                        //When delete clear the doucment, we need to adjust to caret pointer.
                        actualCaretPosition += 2 ;
                        state.Container.DeleteContent(state.BackSpaceIndexStart - 2, state.BackSpaceIndexEnd + 1);
                    }
                    else
                    {
                        state.Container.DeleteContent(state.DeleteIndexStart, state.DeleteIndexEnd - 1);
                    }

                    Verifier.Verify(actualCaretPosition == state.DeleteIndexStart, "Caret position is not correct!" +
                        " Expected[" + state.DeleteIndexStart + "], Actual[" + actualCaretPosition + "]");
                }

                VerifyMatch(state.Container, state.Wrapper.Start, "after Delete key");
            }
            else
            {
                string text;    // Expected text.

                VerifyEmptySelection(state, "Selection is empty after Delete key.");

                text = state.Text;
                if (state.SelectionLength > 0)
                {
                    VerifyInsertion(state, "");
                }
                else if (state.SelectionStartDistance == text.Length)
                {
                    VerifyTextUnchanged(state);
                }
                else
                {
                    bool match;

                    if (state.SelectionStartDistance < text.Length - 1 &&
                        text.Substring(state.SelectionStartDistance, 2) == "\r\n")
                    {
                        // Remove \r\n pair.
                        text = text.Remove(state.SelectionStartDistance, 2);
                    }
                    else if (state.SelectionStartDistance < text.Length - 1 &&
                        Char.IsHighSurrogate(text, state.SelectionStartDistance) &&
                        Char.IsLowSurrogate(text, state.SelectionStartDistance + 1))
                    {
                        // Remove surrogate pair.
                        text = text.Remove(state.SelectionStartDistance, 2);
                    }
                    else
                    {
                        // Remove a single character.
                        text = text.Remove(state.SelectionStartDistance, 1);
                    }
                    match = state.Wrapper.Text == text;
                    if (!match)
                    {
                        Verifier.Verify(false, "Text [" + state.Wrapper.Text +
                            "] is as expected [" + text + "]", false);
                    }
                }
            }
        }

        /// <summary>
        /// Verify that the Down key was processed correctly.
        /// </summary>
        private static void VerifyDown(KeyboardEditingState state)
        {
            VerifyEmptySelection(state, "Selection empty after Down key press.");

            if (state.Wrapper.IsElementRichText)
            {
                int previousEndLineIndex ;
                int currentCaretLineIndex;
                int maxLineIndex;

                previousEndLineIndex = state.Wrapper.LineNumberOfTextPointer(state.SelectionEnd);
                currentCaretLineIndex = state.Wrapper.LineNumberOfTextPointer(state.Wrapper.SelectionInstance.Start);
                maxLineIndex = state.Wrapper.LineNumberOfTextPointer(state.Wrapper.SelectionInstance.Start.DocumentEnd);

                if (currentCaretLineIndex != previousEndLineIndex + 1)
                {
                    string debugMessage="Line index of seleciton end before down key is [" + previousEndLineIndex + "] " +
                                        "Line index of seleciton start after down key is [" + currentCaretLineIndex + "] "+
                                        "Line index of document end is [" + maxLineIndex + "]";
                    Verifier.Verify(maxLineIndex == previousEndLineIndex &&
                        maxLineIndex == currentCaretLineIndex, debugMessage);
                }
            }
            else if (state.Wrapper.Element is PasswordBox)
            {
                Verifier.Verify(
                    state.Wrapper.SelectionInstance.Text.Length == 0,
                    "Selection is collapsed after Down in PasswordBox.", true);
            }
            else
            {
                TextBox textbox;
                int oldLineIndex, currentLineIndex;
                int collapsedIndex;
                LogicalDirection collapsedDirection;
                bool oldSelectionCollapsed;
                Rect rect1; 

                textbox = state.Wrapper.Element as TextBox;
                if (textbox == null)
                {
                    return;
                }

                Verifier.Verify(textbox.SelectionLength == 0,
                    "Selection is collapsed after down arrow key.", true);

                // Figure out where the selection collapses to.
                oldSelectionCollapsed = state.SelectionStartDistance == state.SelectionEndDistance;
                if (!oldSelectionCollapsed &&
                    state.SelectionEndDistance > 1 &&
                    textbox.Text.Substring(state.SelectionEndDistance - 2, 2) == "\r\n")
                {
                    collapsedIndex = state.SelectionEndDistance - 2;
                    collapsedDirection = LogicalDirection.Backward;
                }
                else
                {
                    collapsedIndex = state.SelectionEndDistance;
                    collapsedDirection = state.SelectionEndDirection;
                }

                // If we were on the last line, nothing should happen.
                oldLineIndex = GetPositionLineIndex(textbox, collapsedIndex, collapsedDirection);
                if (oldLineIndex == textbox.LineCount - 1)
                {
                    Verifier.Verify(
                        textbox.SelectionStart == state.SelectionEndDistance,
                        "Selection does not move for last-line Down arrow key " +
                        "(currently at " + textbox.SelectionStart + ", expected " +
                        "at previous end-of-selection " + state.SelectionEndDistance + ")",
                        true);
                }
                else
                {
                    rect1 = state.Wrapper.GetElementRelativeCharacterRect(state.Wrapper.SelectionInstance.End, 0, LogicalDirection.Forward);

                    //if the the caret is at the last visible line and there are more lines down which is note visuble
                    //done key will not change the relateive location (vertically) of caret.
                    if ((int)rect1.Top == (int)state.SelectionEndtRelatedToControl.Top)
                    {
                        currentLineIndex = textbox.GetLineIndexFromCharacterIndex(textbox.SelectionStart);
                        Verifier.Verify(currentLineIndex == oldLineIndex + 1,
                            "Current line index should be one more than the old index!, Current[" + currentLineIndex.ToString() + ", Old[" + oldLineIndex.ToString() + "]");
                    }
                    else
                    {
                        Point currentCaretPosition;

                        if (oldSelectionCollapsed)
                        {
                            currentCaretPosition = state.Wrapper.GetDocumentRelativeCaretPosition();
                            Verifier.Verify(currentCaretPosition.Y > state.CaretPosition.Y,
                                "After Down, caret top (" + currentCaretPosition.Y + " is below " +
                                "previous caret position (" + state.CaretPosition.Y + ").", true);
                        }
                    }
                }
            }
        }

        /// <summary>Verifies that the selection in the current state is empty.</summary>
        /// <param name='state'>State wrapping element to verify.</param>
        /// <param name='message'>Message to log.</param>
        private static void VerifyEmptySelection(KeyboardEditingState state, string message)
        {
            int length;

            if (state.IsPasswordBox)
            {
                length = state.Wrapper.GetSelectedText(false, false).Length;
            }
            else
            {
                TextSelection selection;

                selection = state.Wrapper.SelectionInstance;
                length = selection.Start.CompareTo(selection.End);
            }
            Verifier.Verify(length == 0, message, false);
        }

        /// <summary>
        /// Verify that the result of pressing the End key is correct.
        /// </summary>
        private static void VerifyEnd(KeyboardEditingState state)
        {
            VerifyEmptySelection(state, "Selection empty after End key press.");

            if (state.Wrapper.IsElementRichText)
            {
                int beforeLineIndex;
                int afterLineIndex;

                beforeLineIndex = state.Wrapper.LineNumberOfTextPointer(state.SelectionEnd);
                
                //Selection.End is at the beginning of a line and there is a selection. We need to adjust the index.
                if (state.SelectionStart.GetOffsetToPosition(state.SelectionEnd) > 0 && state.SelectionEnd.IsAtLineStartPosition)
                {
                    beforeLineIndex--;
                }

                afterLineIndex = state.Wrapper.LineNumberOfTextPointer(state.Wrapper.SelectionInstance.Start);
                Verifier.Verify(beforeLineIndex == afterLineIndex,
                    "Caret should be on the same line as previous selection end", true);

                Verifier.Verify(state.Wrapper.IsCaretAtEndOfLine, "Caret should be at the end of the line after End key!", true);

                //no change should be made to the TextContainer.
                VerifyMatch(state.Container, state.Wrapper.Start,
                    "No change to text tontainer after End Key!");
            }
            else if (state.Wrapper.Element is PasswordBox)
            {
                // Simply verify that the caret is at the end of all content.
                Verifier.Verify(state.Wrapper.SelectionStart == state.Text.Length,
                    "After End in PasswordBox, caret is at end of text.", true);

            }
            else
            {
                // Verify that the caret is at the end of its line.
                TextBox textbox;
                int currentLineIndex;
                int currentIndex;
                int oldIndex;
                int oldLineIndex;
                int lastIndexOnLine;

                textbox = state.Wrapper.Element as TextBox;
                if (textbox == null)
                {
                    return;
                }

                currentIndex = textbox.SelectionStart;
                currentLineIndex = GetPositionLineIndex(textbox, currentIndex,
                    state.Wrapper.SelectionInstance.Start.LogicalDirection);
                if (state.SelectionEndDistance == state.SelectionMovingDistance)
                {
                    oldIndex = state.SelectionMovingDistance;
                    oldLineIndex = GetPositionLineIndex(textbox, oldIndex,
                        state.SelectionMovingDirection);
                }
                else
                {
                    oldIndex = state.SelectionEndDistance;
                    oldLineIndex = GetPositionLineIndex(textbox, oldIndex,
                        state.SelectionEndDirection);
                }

                Verifier.Verify(currentLineIndex == oldLineIndex,
                    "After End, selection is on line (" + currentLineIndex +
                    ") as it was before (" + oldLineIndex + ")", true);

                lastIndexOnLine = GetLastIPIndexOnLine(textbox, currentLineIndex);
                Verifier.Verify(currentIndex == lastIndexOnLine,
                    "After End, selection is on last IP index of line", true);
            }
        }

        /// <summary>
        /// Verify that the result of pressing the Enter key is correct.
        /// </summary>
        private static void VerifyEnter(KeyboardEditingState state)
        {
            if (state.IsReadOnly)
            {
                VerifyTextUnchanged(state);
                return;
            }

            if (state.Wrapper.IsElementRichText)
            {
                if (state.AcceptsReturn)
                {
                    VerifyEnterForRichTextBox(state);
                }
                else
                {
                    VerifyTextUnchanged(state);
                }                
            }
            else
            {
                if (state.AcceptsReturn)
                {
                    VerifyInsertion(state, "\r\n");
                }
                else
                {
                    VerifyTextUnchanged(state);
                }
            }
        }

        /// <summary>
        /// Check the enter action for RichTextBox
        /// </summary>
        /// <param name="state"></param>
        private static void  VerifyEnterForRichTextBox(KeyboardEditingState state)
        {
            if (state.Wrapper.IsTextPointerInsideTextElement(state.Wrapper.SelectionInstance.Start, typeof(Table)) ||
               state.Wrapper.IsTextPointerInsideTextElement(state.Wrapper.SelectionInstance.End, typeof(Table)))
            {
            }
            else if (state.Wrapper.IsTextPointerInsideTextElement(state.Wrapper.SelectionInstance.Start, typeof(ListItem)) ||
                state.Wrapper.IsTextPointerInsideTextElement(state.Wrapper.SelectionInstance.End, typeof(ListItem)))
            {
            }
            else
            {
              VerifyEnterToCreateAParagraph(state); 
            }
        }

        /// <summary>
        /// Verify that a enter to create a paragraph.
        /// We will check the following:
        ///    1. Text before the selection is unchanged.
        ///    2. Text after the selection is unchanged.
        ///    3. Xaml before the selection is unchanged (for fomats)
        ///    4. xaml after the selection is unchanged (for formats)
        ///    5. caret is at the beginning of the next line.
        ///    6. Selected text is deleted.
        /// </summary>
        /// <param name="State"></param>
        private static void VerifyEnterToCreateAParagraph(KeyboardEditingState State)
        {
            string textBeforeSelection, xamltextBeforeSelection;
            string textAfterSelection, xamltextAfterSelection;
            int lineIndexAfter;
            TextPointer pointer;

            textBeforeSelection = new TextRange(State.Wrapper.Start, State.Wrapper.SelectionInstance.Start).Text;
            textAfterSelection = new TextRange(State.Wrapper.SelectionInstance.End, State.Wrapper.End).Text;
            pointer = State.Wrapper.SelectionInstance.Start.GetNextInsertionPosition(LogicalDirection.Backward);
            xamltextBeforeSelection = XamlUtils.TextRange_GetXml(new TextRange(State.Wrapper.Start, pointer));
            xamltextAfterSelection =XamlUtils.TextRange_GetXml( new TextRange(State.Wrapper.SelectionInstance.End, State.Wrapper.End));

            //Verify the text before and after to selection.
            Verifier.Verify(textBeforeSelection == State.PartBeforeSelection + "\r\n",
                "Text before selection does is not expected after ENTER key! Before action is[" + State.PartBeforeSelection +"\r\n" + "]. After action is[" + textBeforeSelection + "]");
            if (!State.Wrapper.IsCaretAtEndOfDocument)
            {
                Verifier.Verify(textAfterSelection == State.PartAfterSelection,
                    "Text after selection does is not expected after ENTER key! Before action is[" + State.PartAfterSelection + "]. After action is[" + textAfterSelection + "]");
            }
            else
            {
            }

            //Verify the xaml text before and after to selection.
            //when document is empty, there may not be a <run> in the first Paragraph. we don't verify the xaml at this situation.
            if (!(State.PartBeforeSelection == string.Empty || State.PartBeforeSelection == null))
            {
                Verifier.Verify(xamltextBeforeSelection == State._XamlForPartBeforeSelection,
                  "Xaml text before selection does is not expected after ENTER key! Before action is[" + State._XamlForPartBeforeSelection + "]. After action is[" + xamltextBeforeSelection + "]");
            }
            //if the orginal selection reaches all the way to the end of the doucment, the caret should be at the list insertion pointer.
            //xaml comparsion is ingnored.
            if (!State.Wrapper.IsCaretAtEndOfDocument)
            {
                Verifier.Verify(xamltextAfterSelection == State._XamlForPartAfterSelection,
                    "Xaml text after selection does is not expected after ENTER key! Before action is[" + State._XamlForPartAfterSelection + "]. After action is[" + xamltextAfterSelection + "]");
            }
            else
            {
            }

            //Verify selection is deleted 
            Verifier.Verify(0 == State.Wrapper.SelectionInstance.Start.CompareTo(State.Wrapper.SelectionInstance.End), 
                "Selection start and Selection end does not pointer to the same location!");

            //Caret is at the beginning of the next line.
            Verifier.Verify(State.Wrapper.SelectionInstance.Start.IsAtLineStartPosition,
                "Selection start after endter key should be at the line start!");
            Verifier.Verify(State.Wrapper.SelectionInstance.End.IsAtLineStartPosition,
                "Selection end after endter key should be at the line start!");

            lineIndexAfter = State.Wrapper.LineNumberOfTextPointer(State.Wrapper.SelectionInstance.Start);
            Verifier.Verify(lineIndexAfter == State.lineIndexAtSelectionStart + 1,
                "Ater enter key, line index of selection start should increase by 1! line index before[" + State.lineIndexAtSelectionStart + "], line index after[" + lineIndexAfter + "]");

        }

        /// <summary>
        /// Verify that the Home key was processed correctly.
        /// </summary>
        private static void VerifyHome(KeyboardEditingState state)
        {

            // Password boxes are single line, so we look for a collapsed
            // selection at the start of the control.
            if (state.Wrapper.Element is PasswordBox)
            {
                Verifier.Verify(state.Wrapper.SelectionStart == 0,
                    "After Home key, caret is at the start of PasswordBox.", true);
            }
            else
            {
                // We are only verifying the following:
                // - Caret is on a line edge (facing forward, which means in-the-same-line).
                // - Caret is before or at the same place as the previous position.
                // - Selection is collapsed.

                int beforeLineIndex;
                int afterLineIndex;

                beforeLineIndex = state.lineIndexAtSelectionStart;
                afterLineIndex = state.Wrapper.LineNumberOfTextPointer(state.Wrapper.SelectionInstance.Start);
               
                Verifier.Verify(beforeLineIndex == afterLineIndex,
                    "Caret should be on the same line as previous selection start", true);

                // Coverage for Regression_Bug28.
                Verifier.Verify(state.Wrapper.SelectionInstance.Start.IsAtLineStartPosition,
                    "After Home key, caret is on line boundary.", true);

                Verifier.Verify(state.SelectionStartDistance >= TextUtils.GetDistanceFromStart(state.Wrapper.SelectionInstance.Start),
                    "After Home key, caret at same position or before.", true);

                //no change should be made to the TextContainer.
                VerifyMatch(state.Container, state.Wrapper.Start,
                    "No change to text tontainer after HOME key!");
            }

            VerifyEmptySelection(state, "Selection empty after Home key press.");
        }

        /// <summary>
        /// Verify that Italic command was processed correctly
        /// </summary>
        /// <param name="initialState">initial state of the element.</param>
        private static void VerifyItalic(KeyboardEditingState initialState)
        {
            object fontStyle, wordFontStyle;
            TextRange wordRange;
            bool isInAWord = false;
            TextSelection currentSelection;

            fontStyle = wordFontStyle = null;

            //Verify that selection indexes didnt change
            Verifier.Verify(initialState.SelectionStartDistance == initialState.Wrapper.SelectionStart,
                "Verifying that SelectionStartDistance didnt change after Italic command", true);
            Verifier.Verify(initialState.SelectionLength == initialState.Wrapper.SelectionLength,
                "Verifying that SelectionLength didnt change after Italic command", true);

            //For RichTextBox verify that the formatting properties are applied.
            if (initialState.Wrapper.IsElementRichText)
            {
                Verifier.Verify(initialState.Text == initialState.Wrapper.Text,
                    "Verifying that contents of RichTextBox havent changed after Italic command", true);

                //Current FontStyle
                fontStyle = initialState.Wrapper.SelectionInstance.GetPropertyValue(TextElement.FontStyleProperty);

                currentSelection = initialState.Wrapper.SelectionInstance;
                if ((currentSelection.Start.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text) &&
                    (currentSelection.Start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text))
                {
                    isInAWord = true;
                }

                //Current WordFontStyle: If selection is empty and the caret is in the middle
                //of the word, formatting should be applied to the entire word.
                if ((initialState.Wrapper.SelectionInstance.IsEmpty)&&
                    (isInAWord))
                {
                   wordRange = new TextRange(initialState.Wrapper.SelectionInstance.Start, initialState.Wrapper.SelectionInstance.End);
                   wordFontStyle = wordRange.GetPropertyValue(TextElement.FontStyleProperty);
                }

                //When selection is not empty and it has mixed content (included normal content and italized content)
                //formatting properties at selection start are used and toggled through out the selection
                if ((FontStyle)initialState.SelectionStartFontStyle == FontStyles.Normal)
                {
                    Verifier.Verify((FontStyle)fontStyle == FontStyles.Italic,
                        "Verifying that FontStyle is Italized after Italic command", true);

                    //Comparison when selection is empty
                    if ((initialState.Wrapper.SelectionInstance.IsEmpty)&&
                        (isInAWord))
                    {
                        Verifier.Verify((FontStyle)wordFontStyle == FontStyles.Italic,
                            "Verifying that FontStyle of the word is Italized after Italic command", true);
                    }
                }
                else if ((FontStyle)initialState.SelectionStartFontStyle == FontStyles.Italic)
                {
                    Verifier.Verify((FontStyle)fontStyle == FontStyles.Normal,
                        "Verifying that FontStyle is toggled to Normal after Italic command", true);

                    //Comparison when selection is empty
                    if ((initialState.Wrapper.SelectionInstance.IsEmpty)&&
                        (isInAWord))
                    {
                        Verifier.Verify((FontStyle)wordFontStyle == FontStyles.Normal,
                            "Verifying that FontStyle of the word is toggled to Normal after Italic command", true);
                    }
                }
            }
            else
            {
                //Contents should not change in TextBox and PasswordBox
                VerifyTextUnchanged(initialState);
            }
        }

        private static void VerifyPageUpDown(KeyboardEditingState initialState, bool pageDown, bool shiftKeyPressed)
        {
            int selectionStartCrossLines;
            int selectionEndCroosLines;
            int initialSelectionStartAtLine;
            int initialSelectionEndtAtLine;
            int FinalSelectionStartAtLine;
            int FinalSelectionEndtAtLine;
            Rect rect;

            if (initialState.Wrapper.Element is PasswordBox)
            {
                Logger.Current.Log("Verifying PgUp/PgDown on PasswordBox as Home / End movements.");
                if (pageDown)
                {
                    if (shiftKeyPressed)
                    {
                    }
                    //Page Down not supported
                }
                else
                {
                    if (shiftKeyPressed)
                    {
                    }
                    //Page Up not supported
                }
                return;
            }

            selectionStartCrossLines = initialState.Wrapper.LinesInAPageFromTextPointer(initialState.SelectionStart, pageDown);
            selectionEndCroosLines = initialState.Wrapper.LinesInAPageFromTextPointer(initialState.SelectionEnd, pageDown);
            selectionStartCrossLines = pageDown ? selectionEndCroosLines : 0 - selectionEndCroosLines;
            selectionEndCroosLines = pageDown ? selectionEndCroosLines : 0 - selectionEndCroosLines;

            initialSelectionStartAtLine = initialState.Wrapper.LineNumberOfTextPointer(initialState.SelectionStart);
            initialSelectionEndtAtLine = initialState.Wrapper.LineNumberOfTextPointer(initialState.SelectionEnd);
            FinalSelectionStartAtLine = initialState.Wrapper.LineNumberOfTextPointer(initialState.Wrapper.SelectionInstance.Start);
            FinalSelectionEndtAtLine = initialState.Wrapper.LineNumberOfTextPointer(initialState.Wrapper.SelectionInstance.End);

            // Verify that the scroll bar scrolls as expected:
            //Note: When relative location of initial selectionstart/end change, the scroll bars will scroll
            rect = initialState.Wrapper.GetElementRelativeCharacterRect(initialState.SelectionStart, 0, LogicalDirection.Forward);
            if ((int)rect.Top != (int)initialState.SelectionStartRelatedToControl.Top)
            {
                if (pageDown)
                    Verifier.Verify(((TextBoxBase)initialState.Wrapper.Element).VerticalOffset > initialState.VerticalOffset, "The scroll bar should scroll when press page down!");
                else
                    Verifier.Verify(((TextBoxBase)initialState.Wrapper.Element).VerticalOffset < initialState.VerticalOffset, "The scroll bar should scroll when press page down!");
            }

            //verify shift PageUp/Down for selected lines
            if (shiftKeyPressed)
            {
                if (pageDown)
                {
                    bool SelectionEndIsActive = (FinalSelectionEndtAtLine - FinalSelectionStartAtLine) == (selectionEndCroosLines + initialSelectionEndtAtLine - initialSelectionStartAtLine);
                    bool SelectionStartIsActive = (FinalSelectionEndtAtLine - FinalSelectionStartAtLine) == Math.Abs(selectionEndCroosLines - (initialSelectionEndtAtLine - initialSelectionStartAtLine));
                    Verifier.Verify(SelectionEndIsActive || SelectionStartIsActive, "Selection is not correct after Shift_PageDown!");
                }
                else
                {
                    bool SelectionEndIsActive = (FinalSelectionEndtAtLine - FinalSelectionStartAtLine) == Math.Abs(selectionEndCroosLines - (initialSelectionEndtAtLine - initialSelectionStartAtLine));
                    bool SelectionStartIsActive = (FinalSelectionEndtAtLine - FinalSelectionStartAtLine) == (selectionEndCroosLines + (initialSelectionEndtAtLine - initialSelectionStartAtLine));
                    Verifier.Verify(SelectionEndIsActive || SelectionStartIsActive, "Selection is not correct after Shift_PageDown!");
                }
            }

            //Verify Pageup/Down.
            if (!(FinalSelectionStartAtLine == initialSelectionStartAtLine + selectionStartCrossLines
                || FinalSelectionEndtAtLine == initialSelectionEndtAtLine + selectionEndCroosLines))
            {
                Verifier.Verify(false, "Caret move to the unexpected line!");
            }

            //Verify the First and Last line
            if (initialSelectionStartAtLine == initialSelectionEndtAtLine
                && initialSelectionEndtAtLine == FinalSelectionStartAtLine &&
                FinalSelectionStartAtLine == FinalSelectionEndtAtLine)
            {
                if (pageDown)
                {
                    
                }
                else
                {
                }

                if (!shiftKeyPressed)
                {
                    Verifier.Verify(initialState.Wrapper.SelectionInstance.Text.Length == 0,
                        "Selection should be empty when do page donw/up when selection is initially at the first or last line!");
                }
            }
        }

        private static void VerifyIncreaseDecreaseFontSize(KeyboardEditingState initialState, bool isIncrease)
        {
            if (initialState.Wrapper.IsElementRichText)
            {
                Verifier.Verify(initialState.Text == initialState.Wrapper.Text,
                    "Verifying that contents of RichTextBox havent changed after Increase/Decrease FontSize command", true);

                if (initialState.SelectionLength > 0)
                {
                    //When selection has text with multiple fontsizes, it picks the fontsize at the
                    //begining of the selection, applies it to the entire selection and then increments it.
                    //Behavior different than in word.
                    if (initialState.SelectionFontSize == double.NaN)
                    {
                        double actualFontSize = (double)initialState.Wrapper.SelectionInstance.GetPropertyValue(
                            TextElement.FontSizeProperty);
                        bool condition = (isIncrease) ? (actualFontSize > initialState.SelectionStartFontSize) :
                            (actualFontSize < initialState.SelectionStartFontSize);
                        Verifier.Verify(condition, "FontSize has not changed", true);
                    }
                    else //all the text in the selection has same fontsize.
                    {
                        double actualFontSize = (double)initialState.Wrapper.SelectionInstance.GetPropertyValue(
                            TextElement.FontSizeProperty);
                        bool condition = (isIncrease) ? (actualFontSize > initialState.SelectionFontSize) :
                            (actualFontSize < initialState.SelectionFontSize);
                        Verifier.Verify(condition, "FontSize has not changed", true);
                    }
                }
                else
                {
                    //When no selection, there should be no change. It just spring loads the property.
                    VerifyTextUnchanged(initialState);
                    return;
                }
            }
            else
            {
                //For TextBox and PasswordBox contents shouldnt change.
                VerifyTextUnchanged(initialState);
                return;
            }
        }

        private static void VerifyMatch(ArrayTextContainer container, TextPointer pointer,
            string containerStateDescription)
        {
            ArrayTextContainer pointerContainer;
            bool match;

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }

            pointerContainer = new ArrayTextContainer(pointer);
            match = container.Matches(pointerContainer);
            if (!match)
            {
                string log;
                int linesToWrite;

                log = "Container (left) does not match content (right) " +
                    containerStateDescription + "\r\n";
                linesToWrite = (container.Count > pointerContainer.Count)?
                    container.Count : pointerContainer.Count;
                for (int i = 0; i < linesToWrite; i++)
                {
                    if (i < container.Count)
                    {
                        log += container[i].ToString() + "\t";
                    }
                    else
                    {
                        log += "[no symbol]\t";
                    }
                    if (i < pointerContainer.Count)
                    {
                        log += pointerContainer[i].ToString() + "\t";
                    }
                    else
                    {
                        log += "[no symbol]\t";
                    }
                    log += "\r\n";
                }

                Verifier.Verify(match, log);
            }
        }

        /// <summary>
        /// Verifies that the specified text was inserted into the selection.
        /// </summary>
        private static void VerifyInsertion(KeyboardEditingState state, string text)
        {
            string expectedText;
            int insertionPosition;

            if (state.IsReadOnly)
            {
                VerifyTextUnchanged(state);
                return;
            }

            if (state.Wrapper.IsElementRichText)
            {
                XmlLanguage language;
                int insertionIndex;

                DeleteSelection(state, out insertionIndex);

                // Consider whether the new symbol will be inserted into an
                // existing paragraph.
                if (!state.Container.IsInParagraph(insertionIndex) &&
                    insertionIndex < state.Container.Count)
                {
                    Logger.Current.Log("Cannot find Paragraph parent for index " + insertionIndex);
                    state.Container.InsertElement(insertionIndex, insertionIndex, new Paragraph());
                    insertionIndex++;
                }

                // Create a Run around the text to insert, if required.
                bool isRunAvailable;
                isRunAvailable = state.Container.GetParentElement(insertionIndex) is Run;
                if (!isRunAvailable)
                {
                    state.Container.InsertElement(insertionIndex, insertionIndex, new Run());
                    insertionIndex++;
                }

                // In Overtype mode, the replacement is done after the selection
                // is deleted (as long as it's not a \r\n pair, a line break
                // or a block delimiter).
                if (state.IsOvertypeModeEnabled)
                {
                    if (insertionIndex < state.Container.Count &&
                        state.Container[insertionIndex].Context == TextPointerContext.Text &&
                        state.Container[insertionIndex].Character != '\r')
                    {
                        state.Container.DeleteCombinedCharacters(insertionIndex);
                    }
                }

                // If the text is in a different language, it needs to be
                // surrounded by an element with CultureInfo in that language.
                language = (XmlLanguage)
                    state.Container.GetValue(state.SelectionStartDistance, FrameworkElement.LanguageProperty);
                if (InputLanguageManager.Current.CurrentInputLanguage.IetfLanguageTag != language.IetfLanguageTag)
                {
                    return;
                    /*
                    Logger.Current.Log("Container before element insertion:\r\n" + DescribeContainer(state.Container));
                    state.Container.InsertElement(state.SelectionStartDistance,
                        state.SelectionStartDistance, new Inline());
                    Logger.Current.Log("Container after element insertion:\r\n" + DescribeContainer(state.Container));
                    state.Container[state.SelectionStartDistance].LocalValues[FrameworkElement.LanguageProperty] = language;
                    insertionIndex = state.SelectionStartDistance + 1;
                    */
                }
                state.Container.InsertText(insertionIndex, text);
                VerifyMatch(state.Container, state.Wrapper.Start, "after text insertion");
            }
            else
            {
                insertionPosition = state.SelectionStartDistance;
                expectedText = state.Text.Insert(insertionPosition, text);
                if (state.SelectionLength != 0)
                {
                    //this is to ensure that we do not remove /r/n in the case where there is selection and selection ends with \r\n
                    //we do not overwrite \r\n in this case
                    if ((text.Length != 0) && expectedText.Substring(insertionPosition + text.Length + state.SelectionLength - 2).StartsWith("\r\n"))
                    {
                        expectedText = expectedText.Remove(insertionPosition + text.Length,
                        state.SelectionLength - 2);
                    }
                    else
                    {
                        expectedText = expectedText.Remove(insertionPosition + text.Length,
                            state.SelectionLength);
                    }
                }

                // In Overtype mode, the replacement is done *after* the selection
                // is deleted (as long as it's not a \r\n pair) in Word.
                // In RichEdit (and the current Avalon implementation), it's
                // only done if the selection was collapsed.
                if (state.IsOvertypeModeEnabled && state.SelectionLength == 0)
                {
                    if (insertionPosition + text.Length < expectedText.Length &&
                        expectedText[insertionPosition + text.Length] != '\r')
                    {
                        expectedText = TextUtils.RemoveCombinedCharacters(expectedText,
                            insertionPosition + text.Length);
                    }
                }
                Verifier.Verify(state.Wrapper.Text == expectedText,
                    "Text [" + state.Wrapper.Text + "] matches expected [" + expectedText + "]", false);
            }
        }

        /// <summary>
        /// Verify that the Left key was processed correctly.
        /// </summary>
        private static void VerifyLeft(KeyboardEditingState state)
        {
            VerifyEmptySelection(state, "Selection empty after Left key press.");

            if (state.Wrapper.IsElementRichText)
            {
               TextRange currentToPreviousStart;
               currentToPreviousStart = new TextRange(state.SelectionStart, state.Wrapper.SelectionInstance.Start);
               if (state.SelectionStart.CompareTo(state.SelectionEnd) == 0)
               {
                    // Verify caret movement.
                    if (currentToPreviousStart.Text.Length > 1)
                    {
                        //cross lines
                        Verifier.Verify(currentToPreviousStart.Text ==Environment.NewLine,
                            "Caret should move to a previous place on Left key.");
                    }
                    else if (currentToPreviousStart.Text.Length == 0)
                    {
                        //verify is already at the beginning of the doucment.
                        TextRange StartToCaret = new TextRange(state.SelectionStart.DocumentStart, state.SelectionStart);

                        //check the text
                        Verifier.Verify(StartToCaret.Text == string.Empty,
                            "No text at the left of the caret when caret is at the beginning of the doucment!");

                        //check if there is any close tag to the left of the caret.
                        Verifier.Verify(state.Wrapper.IsCaretAtBeginningOfDocument,
                            "Caret must be at the beginning!");
                    }
                }
                else
                {
                    // Verify selection collapsing.
                    Verifier.Verify(0 == state.Wrapper.SelectionInstance.Start.CompareTo(state.SelectionStart),
                        "Selection does not collapse to the left after Left key!");
                }
            }
            else
            {
                int insertionIndex;
                int currentStartIndex;

                insertionIndex = state.SelectionStartDistance;
                currentStartIndex = state.Wrapper.SelectionStart;
                if (state.SelectionLength > 0)
                {
                    // Verify selection collapsing.
                    Verifier.Verify(insertionIndex == currentStartIndex,
                        "Selection collapses on Left key.", true);
                }
                else
                {
                    insertionIndex = FindNextCaretIndex(state.Wrapper.Element, insertionIndex,
                        LogicalDirection.Backward);
                    if (insertionIndex == -1)
                    {
                        insertionIndex = state.SelectionStartDistance;
                    }
                    Verifier.VerifyValue("caret position after Left",
                        insertionIndex, currentStartIndex);
                }
            }
        }

        /// <summary>
        /// Verify that the Left key with Control pressed was processed correctly.
        /// </summary>
        private static void VerifyLeftControl(KeyboardEditingState state)
        {
            VerifyLeftRightControl(state, "Ctrl+Left",
                state.SelectionStartDistance, LogicalDirection.Backward);
        }

        /// <summary>Verifies that Ctrl+Left or Ctrl+Right was processed correctly.</summary>
        private static void VerifyLeftRightControl(KeyboardEditingState state,
            string operation, int initialStart, LogicalDirection directionInLeftToRight)
        {
            bool selectionCollapsed;
            int currentStart;
            int expectedStart;

            // Ctrl+Left is handled by collapsing to the start, then looking
            // for the closest word-boundary (possibly the collapsing point
            // iif there was no collapsing).
            VerifyEmptySelection(state, "selection is empty after " + operation);

            selectionCollapsed = state.SelectionLength > 0;
            currentStart = state.Wrapper.SelectionStart;
            expectedStart = initialStart;

            if (state.Wrapper.IsElementRichText)
            {
                expectedStart = FindNextWordBoundaryIndex(state.Container,
                    (FlowDirection)state.Container.GetValue(expectedStart, Control.FlowDirectionProperty),
                    expectedStart, directionInLeftToRight, selectionCollapsed, false);
            }
            else
            {
                expectedStart = FindNextWordBoundaryIndex(state.Wrapper.Element,
                    expectedStart, directionInLeftToRight, selectionCollapsed, false);
            }

            if (expectedStart == -1)
            {
                expectedStart = initialStart;
            }

            Verifier.Verify(currentStart == expectedStart,
                "Current start is " + currentStart + " but expected start is " +
                expectedStart + " after " + operation + " when selection-collapsed is " +
                selectionCollapsed + ".\r\n" + state.Container.DescribeSymbols(), false);
        }

        /// <summary>
        /// Verify that the Left key with Control+Shift pressed was processed correctly.
        /// </summary>
        private static void VerifyLeftControlShift(KeyboardEditingState state)
        {
            VerifyLeftRightControlShift(state, "Control+Shift+Left", LogicalDirection.Backward);
        }

        /// <summary>
        /// Verifies that the Left or Right key with Control and Shift pressed was
        /// processed correctly.
        /// </summary>
        private static void VerifyLeftRightControlShift(KeyboardEditingState state,
            string operation, LogicalDirection directionInLeftToRight)
        {
            const bool isStartValidFalse = false;

            int currentMovingDistance;
            int expectedMovingDistance;

            // Ctrl+Shift+Left is handled by extending to active selection edge
            // to the next word-boundary.
            currentMovingDistance = state.Wrapper.SelectionMovingPointerDistance;
            expectedMovingDistance = state.SelectionMovingDistance;

            if (state.Wrapper.IsElementRichText)
            {
                expectedMovingDistance = FindNextWordBoundaryIndex(state.Container,
                    (FlowDirection)state.Container.GetValue(expectedMovingDistance, Control.FlowDirectionProperty),
                    expectedMovingDistance, directionInLeftToRight, isStartValidFalse, false);
            }
            else
            {
                expectedMovingDistance = FindNextWordBoundaryIndex(state.Wrapper.Element,
                    expectedMovingDistance, directionInLeftToRight, isStartValidFalse, true);
            }

            if (expectedMovingDistance == -1)
            {
                expectedMovingDistance = state.SelectionMovingDistance;
            }

            if (state.SelectionMovingDistance == state.Container.Count)
            {
                // Starting from the end, a Ctrl+Shift+Right might snap back.
                return;
            }

            Verifier.Verify(currentMovingDistance == expectedMovingDistance,
                "Current moving edge is " + currentMovingDistance + ", expected moving edge is " +
                expectedMovingDistance + " after " + operation + " from " +
                state.SelectionMovingDistance + ".\r\n" +
                state.Container.DescribeSymbols(), false);
        }

        /// <summary>
        /// Verifies that the Left or Right key with Shift pressed was
        /// processed correctly.
        /// </summary>
        private static void VerifyLeftRightShift(KeyboardEditingState state,
            string operation, LogicalDirection directionInLeftToRight)
        {
            int expectedDistance;
            int currentDistance;

            if (state.Wrapper.IsElementRichText)
            {
                expectedDistance = FindNextCaretIndex(state.Container,
                    state.SelectionMovingDistance, directionInLeftToRight, true);
            }
            else
            {
                expectedDistance = FindNextCaretIndex(state.Wrapper.Element,
                    state.SelectionMovingDistance, directionInLeftToRight);
            }

            // If there is no other caret index, the moving pointer should
            // have remained where it was.
            if (expectedDistance == -1)
            {
                expectedDistance = state.SelectionMovingDistance;
            }

            currentDistance = state.Wrapper.SelectionMovingPointerDistance;

            Verifier.Verify(currentDistance == expectedDistance,
                "Moving position index " + currentDistance +
                ", previously at " + state.SelectionMovingDistance +
                " is at expected position " + expectedDistance + " after " + operation);
        }

        /// <summary>Verify that the Left key with Shift pressed was processed correctly.</summary>
        private static void VerifyLeftShift(KeyboardEditingState state)
        {
            VerifyLeftRightShift(state, "Shift+Left", LogicalDirection.Backward);
        }

        /// <summary>Verify that the Right key was processed correctly.</summary>
        private static void VerifyRight(KeyboardEditingState state)
        {
            VerifyEmptySelection(state, "Selection empty after Right key press.");

            if (state.Wrapper.IsElementRichText)
            {
                TextRange currentToPreviousEnd;

                currentToPreviousEnd = new TextRange(state.Wrapper.SelectionInstance.Start, state.SelectionEnd );

                if (state.SelectionStart.CompareTo(state.SelectionEnd) == 0)
                {
                    // Verify caret movement.
                    if (currentToPreviousEnd.Text.Length > 1)
                    {
                        int expectedDistance;

                        expectedDistance = FindNextCaretIndex(state.Container,
                            state.SelectionEndDistance, LogicalDirection.Forward, false);
                        if (expectedDistance == -1)
                        {
                            expectedDistance = state.SelectionEndDistance;
                        }

                        // Cross lines.
                        Verifier.VerifyValue("start distance",
                            expectedDistance, TextUtils.GetDistanceFromStart(state.Wrapper.SelectionInstance.Start));
                    }
                    else if (currentToPreviousEnd.Text.Length == 0)
                    {
                        //verify is already at the End of the doucment.
                        TextRange EndToCaret = new TextRange(state.SelectionEnd.DocumentEnd, state.SelectionEnd);

                        //check the text. Text could be a new line or empty string. 
                        Verifier.Verify(EndToCaret.Text == Environment.NewLine || EndToCaret.Text==string.Empty,
                            "No text at the right of the caret when caret is at the end of the doucment!");

                        //check if there is any close tag to the left of the caret.
                        Verifier.Verify(state.Wrapper.IsCaretAtEndOfDocument,
                            "Caret must be at the end!");
                    }
                }
                else
                {
                    // Verify selection collapsing.
                    TextPointer tempTp  = state.SelectionEnd;
                    int length;

                    //Make sure that the selection end collapse to an insertion point
                    tempTp = tempTp.GetInsertionPosition(LogicalDirection.Forward);

                    length=state.Wrapper.SelectionInstance.End.CompareTo(tempTp);
                    Verifier.Verify(0 == length,
                        "Selection does not collapse to the right after RIGHT key!");
                }
            }
            else
            {
                int insertionIndex;
                int currentStartIndex;

                insertionIndex = state.SelectionEndDistance;
                currentStartIndex = state.Wrapper.SelectionStart;
                if (state.SelectionLength > 0)
                {
                    // Verify selection collapsing.
                    Verifier.Verify(insertionIndex == currentStartIndex,
                        "Selection collapses to end on Right key.", true);
                }
                else
                {
                    insertionIndex = FindNextCaretIndex(state.Wrapper.Element,
                        insertionIndex, LogicalDirection.Forward);
                    if (insertionIndex == -1)
                    {
                        insertionIndex = state.SelectionEndDistance;
                    }
                    Verifier.Verify(insertionIndex == currentStartIndex,
                        "Caret moves to the following position on Right key " +
                        "(expected " + insertionIndex + ", found at " +
                        currentStartIndex + ")", false);
                }
            }
        }

        /// <summary>
        /// Verify that the Right key with Control pressed was processed correctly.
        /// </summary>
        private static void VerifyRightControl(KeyboardEditingState state)
        {
            VerifyLeftRightControl(state, "Ctrl+Right",
                state.SelectionEndDistance, LogicalDirection.Forward);
        }

        /// <summary>
        /// Verify that the Right key with Control+Shift pressed was processed correctly.
        /// </summary>
        private static void VerifyRightControlShift(KeyboardEditingState state)
        {
            VerifyLeftRightControlShift(state, "Control+Shift+Right", LogicalDirection.Forward);
        }

        /// <summary>Verify that the Right key with Shift pressed was processed correctly.</summary>
        private static void VerifyRightShift(KeyboardEditingState state)
        {
            VerifyLeftRightShift(state, "Shift+Right", LogicalDirection.Forward);
        }

        /// <summary>Verifies that the selection has not changed.</summary>
        private static void VerifySelectionUnchanged(KeyboardEditingState state,
            string operationDescription)
        {
            int newStart = state.Wrapper.SelectionStart;
            int newLength = state.Wrapper.SelectionLength;

            if (state == null)
            {
                throw new ArgumentNullException("state");
            }
            if (operationDescription == null)
            {
                throw new ArgumentNullException("operationDescription");
            }

            Verifier.Verify(newStart == state.SelectionStartDistance,
                "Selection start " + newStart + " is unchanged from " +
                state.SelectionStart + " for " + operationDescription);
            Verifier.Verify(newLength == state.SelectionLength,
                "Selection length " + newLength + " is unchanged from " +
                state.SelectionLength + " for " + operationDescription);
        }

        /// <summary>
        /// Verify that the Tab key was processed correctly.
        /// </summary>
        private static void VerifyTab(KeyboardEditingState state)
        {
            bool acceptsTab = false;
            // PasswordBox controls (and expectedly others) don't have
            // an AcceptsTab property - they behave as if it were false.
            if (state.Wrapper.Element is TextBoxBase)
            {
                acceptsTab = ((TextBoxBase)state.Wrapper.Element).AcceptsTab;
            }
            if(acceptsTab)
            {
                if (state.Wrapper.IsElementRichText)
                {
                    VerifyTabForRichTextBox(state);
                }
                else
                {
                    VerifyInsertion(state, "\t");
                }
            }
            else
            {
                DependencyObject parent;

                VerifyTextUnchanged(state);

                // Very simple algorithm. If the parent of the control
                // has focusable siblings, then focus should have moved.
                parent = LogicalTreeHelper.GetParent(state.Wrapper.Element);
                if (parent == null)
                {
                    Verifier.Verify(state.Wrapper.Element.IsKeyboardFocused,
                        "Control with no parent still has focus.", true);
                }
                else
                {
                    IEnumerable siblings;
                    siblings = LogicalTreeHelper.GetChildren(parent);
                    foreach (DependencyObject sibling in siblings)
                    {
                        if (sibling != null && sibling != state.Wrapper.Element &&
                            sibling is FrameworkElement && ((FrameworkElement)sibling).IsEnabled)
                        {
                            Verifier.Verify(!state.Wrapper.Element.IsKeyboardFocused,
                                "Control loses focus on Tab key when AcceptsTab is false.", true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// verify the Tab key when RichTextBox.AcceptsTab=true;
        /// </summary>
        /// <param name="state"></param>
        private static void VerifyTabForRichTextBox(KeyboardEditingState state)
        {
            if(state.Wrapper.IsTextPointerInsideTextElement(state.SelectionEnd, typeof(List)) ||
                state.Wrapper.IsTextPointerInsideTextElement(state.SelectionStart, typeof(List)))
            {
            }
            else if(state.Wrapper.IsTextPointerInsideTextElement(state.SelectionEnd, typeof(Table)) ||
                state.Wrapper.IsTextPointerInsideTextElement(state.SelectionStart, typeof(Table)))
            {
            }
            else
            {
                //if caret is at the beginning of a line or there is something selected, "Tab" key will increase the indentation.
                if (state.SelectionLength > 0
                    || state.SelectionStart.IsAtLineStartPosition)
                {
                    int selectionStartAtLine ;
                    int selectionEndAtline ;
                    int lines ;
                    double initialLeft, initialRight, currentLeft, currentRight;
                    Thickness currentMargine;
                    TextPointer pointer;
                    Block blockElement, selectionStartBlockElement, selectionEndBlockElement;

                    selectionStartAtLine = state.Wrapper.LineNumberOfTextPointer(state.SelectionStart);
                    selectionEndAtline = state.Wrapper.LineNumberOfTextPointer(state.SelectionEnd);
                    lines = state.BlockMargines.Length - 1;
                    selectionStartBlockElement = state.Wrapper.GetBlockParentForTextPointer(state.SelectionStart);

                    if (state.SelectionEnd.CompareTo(state.SelectionEnd.DocumentEnd) == 0)
                    {
                        //when selection end is at the end of a document, we need to move back to the insertion position
                        selectionEndBlockElement = state.Wrapper.GetBlockParentForTextPointer(state.SelectionEnd.GetInsertionPosition(LogicalDirection.Backward));
                    }
                    else
                    {
                        selectionEndBlockElement = state.Wrapper.GetBlockParentForTextPointer(state.SelectionEnd);
                    }

                    pointer = state.Wrapper.Start;
                    if (!(pointer.IsAtInsertionPosition))
                    {
                        pointer = pointer.GetInsertionPosition(LogicalDirection.Forward);
                    }
                    blockElement = state.Wrapper.GetBlockParentForTextPointer(pointer);

                    //increase the indentation. Margin is getting larger for the selected blocks
                    for (int i = 1; i <= lines; i++)
                    {
                        blockElement = state.Wrapper.GetBlockParentForTextPointer(pointer);
                        currentMargine = blockElement.Margin;
                        initialLeft = (double.IsNaN(state.BlockMargines[i].Left)) ? 0.0 : state.BlockMargines[i].Left;
                        initialRight = (double.IsNaN(state.BlockMargines[i].Right)) ? 0.0 : state.BlockMargines[i].Right;
                        currentLeft = (double.IsNaN(currentMargine.Left )) ? 0.0 : currentMargine.Left;
                        currentRight = (double.IsNaN(currentMargine.Right)) ? 0.0 : currentMargine.Right;

                        //margin should be changed.
                        if (i >= selectionStartAtLine && i <= selectionEndAtline)
                        {
                            if (state.SelectionEnd.IsAtLineStartPosition && i == selectionEndAtline && state.SelectionLength > 0)
                            {
                                Verifier.Verify(initialLeft == currentLeft,
                                    "Margin should not be changed by Tab key when selection end is at line start!", true);
                                Verifier.Verify(initialRight == currentRight,
                                        "Margin should not be changed by Tab key when Selection end is at line start!", true);
                            }
                            else
                            {
                                Verifier.Verify(currentLeft > initialLeft,
                                    "Margin should be changed by Tab key when line contains selection!", true);
                            }
                        }
                        //need to check the line break before or after the selection.
                        else if (i < selectionStartAtLine)
                        {
                            if (blockElement == selectionStartBlockElement)
                            {
                                Verifier.Verify(currentLeft > initialLeft,
                                        "Margin should be changed by Tab key when line has the same block parent as selectoin start!", true);

                            }
                            else
                            {
                                Verifier.Verify(currentLeft == initialLeft,
                                        "Margin should not be changed by Tab key when line does not have the same block parent as selectoin start!", true);
                                Verifier.Verify(currentRight == initialRight,
                                        "Margin should not be changed by Tab key when line does not have the same block parent as selectoin start!", true);
                            }
                        }
                        else
                        {
                            if (blockElement == selectionEndBlockElement)
                            {
                                Verifier.Verify(currentLeft > initialLeft,
                                    "Margin should be changed by Tab key when line has the same block parent as selectoin end!", true);

                            }
                            else
                            {
                                Verifier.Verify(initialLeft== currentLeft,
                                        "Margin should not be changed by Tab key when line does not have the same block parent as selectoin end!", true);
                                Verifier.Verify(initialRight == currentRight,
                                        "Margin should not be changed by Tab key when line does not have the same block parent as selectoin end!", true);
                            }
                        }
                        if (i < lines)
                        {
                            pointer = pointer.GetLineStartPosition(1);
                            if (!pointer.IsAtInsertionPosition)
                            {
                                pointer = pointer.GetInsertionPosition(LogicalDirection.Forward);
                            }
                        }
                    }

                    //Verify that selection indexes didnt change
                    Verifier.Verify(state.SelectionStartDistance ==
                        state.Wrapper.Start.GetOffsetToPosition(state.Wrapper.SelectionInstance.Start),
                        "Verifying that SelectionStartDistance did not change after tab key", true);
                    //when Regression_Bug30 is fixed, we need to remove the if statement.
                    if (state.SelectionEnd.CompareTo(state.SelectionEnd.DocumentEnd) != 0)
                    {
                        Verifier.Verify(state.SelectionLength == state.Wrapper.SelectionLength,
                            "Verifying that SelectionLength did not change after Tab key", true);
                    }

                    //Verify That the content of the RichTextBox is not changed.
                    Verifier.Verify(state.Text == state.Wrapper.Text,
                        "Text in Rich control is not supposed to change. Expected[" + state.Text +
                        "], Actual[" + state.Wrapper.Text + "]");
                    VerifyMatch(state.Container, state.Wrapper.Start, "abc");
                }
                else
                {
                    VerifyInsertion(state, "\t");
                }
            }
        }

        /// <summary>
        /// Verify that Underline command was processed correctly
        /// </summary>
        /// <param name="initialState">initial state of the element.</param>
        private static void VerifyUnderline(KeyboardEditingState initialState)
        {
            object textDecorationCollection, wordTextDecorationCollection;
            TextRange wordRange;
            bool isInAWord = false;
            TextSelection currentSelection;

            textDecorationCollection = wordTextDecorationCollection = null;

            //Verify that selection indexes didnt change
            Verifier.Verify(initialState.SelectionStartDistance == initialState.Wrapper.SelectionStart,
                "Verifying that SelectionStartDistance didnt change after Underline command", true);
            Verifier.Verify(initialState.SelectionLength == initialState.Wrapper.SelectionLength,
                "Verifying that SelectionLength didnt change after Underline command", true);

            //For RichTextBox verify that the formatting properties are applied.
            if (initialState.Wrapper.IsElementRichText)
            {
                Verifier.Verify(initialState.Text == initialState.Wrapper.Text,
                    "Verifying that contents of RichTextBox havent changed after Underline command", true);

                //Current TextDecorationCollection
                textDecorationCollection = initialState.Wrapper.SelectionInstance.GetPropertyValue(Inline.TextDecorationsProperty);

                currentSelection = initialState.Wrapper.SelectionInstance;
                if ((currentSelection.Start.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text) &&
                    (currentSelection.Start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text))
                {
                    isInAWord = true;
                }

                //Current WordTextDecorationCollection: If selection is empty and the caret
                //is in the middle of the word, formatting should be applied to the entire word.
                if ((initialState.Wrapper.SelectionInstance.IsEmpty)&&
                    (isInAWord))
                {
                    wordRange = new TextRange(initialState.Wrapper.SelectionInstance.Start, initialState.Wrapper.SelectionInstance.End);
                    wordTextDecorationCollection = wordRange.GetPropertyValue(Inline.TextDecorationsProperty);
                }

                //Selection is not empty and it has mixed content (included normal content and underlined content)
                //formatting properties at selection start are used and toggled properties are applied to the entire selection.
                if ((TextDecorationCollection)initialState.SelectionStartTextDecoration == null)
                {
                    Verifier.Verify((TextDecorationCollection)textDecorationCollection == TextDecorations.Underline,
                        "Verifying that TextDecorations has underline after Underline command", true);

                    //Comparison when selection is empty
                    if ((initialState.Wrapper.SelectionInstance.IsEmpty)&&
                        (isInAWord))
                    {
                        Verifier.Verify((TextDecorationCollection)wordTextDecorationCollection == TextDecorations.Underline,
                            "Verifying that TextDecorations of the word has underline after Underline command", true);
                    }
                }
                else if ((TextDecorationCollection)initialState.SelectionStartTextDecoration == TextDecorations.Underline)
                {
                    Verifier.Verify((TextDecorationCollection)textDecorationCollection == null,
                        "Verifying that TextDecorations is toggled to Normal after Underline command", true);

                    //Comparison when selection is empty
                    if ((initialState.Wrapper.SelectionInstance.IsEmpty)&&
                        (isInAWord))
                    {
                        Verifier.Verify((TextDecorationCollection)wordTextDecorationCollection == null,
                            "Verifying that TextDecorations of the word is toggled to Normal after Underline command", true);
                    }
                }
            }
            else
            {
                //Contents should not change in TextBox and RichTextBox
                VerifyTextUnchanged(initialState);
            }
        }

        /// <summary>
        /// Verify that the text has not changed in the wrapped control.
        /// </summary>
        private static void VerifyTextUnchanged(KeyboardEditingState state)
        {
            if (state.XamlText != null)
            {
                Verifier.Verify(state.Wrapper.Text == state.Text,
                    "Plain text unchanged, as expected: [" + state.Wrapper.Text + "]", false);
                Verifier.Verify(state.Wrapper.XamlText == state.XamlText,
                    "Rich text unchanged, as expected: [" + state.Wrapper.XamlText + "]", true);
            }
            else
            {
                Verifier.Verify(state.Wrapper.Text == state.Text,
                    "Text unchanged, as expected: [" + state.Wrapper.Text + "]", true);
            }
        }


        

        /// <summary>
        /// Verify that the Up key was processed correctly.
        /// </summary>
        private static void VerifyUp(KeyboardEditingState state)
        {
            VerifyEmptySelection(state, "Selection empty after Up key press.");

            if (state.Wrapper.IsElementRichText)
            {
                int previousStartLineIndex;
                int currentCaretLineIndex;

                previousStartLineIndex = state.Wrapper.LineNumberOfTextPointer(state.SelectionStart);
                currentCaretLineIndex = state.Wrapper.LineNumberOfTextPointer(state.Wrapper.SelectionInstance.Start);

                if (currentCaretLineIndex + 1 != previousStartLineIndex)
                {
                    string debugMessage = "Line index of seleciton end before down key is [" + previousStartLineIndex + "]" +
                                        "Line index of seleciton start after down key is [" + currentCaretLineIndex + "]";
                    Verifier.Verify(1 == previousStartLineIndex &&
                        1 == currentCaretLineIndex, debugMessage);
                }
            }
            else
            {
                TextBox textbox;
                int oldLineIndex, currentLineIndex;

                textbox = state.Wrapper.Element as TextBox;
                if (textbox == null)
                {
                    return;
                }

                // Down arrow key should verify that collapsing occurs
                // as well, but then use the end of the selection.
                Verifier.Verify(textbox.SelectionLength == 0,
                    "Selection is collapsed after up arrow key.", true);

                // If we were on the first line, nothing should happen.
                oldLineIndex = GetPositionLineIndex(textbox, state.SelectionMovingDistance, state.SelectionMovingDirection);
                if (oldLineIndex == 0)
                {
                    Verifier.Verify(
                        textbox.SelectionStart == state.SelectionStartDistance,
                        "Selection does not move for first-line Up arrow key (old start=" +
                        state.SelectionStartDistance + ", new start=" + textbox.SelectionStart,
                        true);
                }
                else
                {
                    Rect rect1 = state.Wrapper.GetElementRelativeCharacterRect(state.Wrapper.Start, 0, LogicalDirection.Forward);
                    Rect rect2 = state.Wrapper.GetElementRelativeCharacterRect(state.Wrapper.SelectionInstance.Start, 0, LogicalDirection.Forward);

                    //if the the caret is at the first visible line and there are more lines above which is note visuble
                    //Up key will not change the relateive location of caret.
                    if (rect1.Top < 0 && (int)rect2.Top == (int)state.SelectionStartRelatedToControl.Top)
                    {
                        currentLineIndex = textbox.GetLineIndexFromCharacterIndex(textbox.SelectionStart);
                        Verifier.Verify(currentLineIndex + 1 == oldLineIndex, 
                            "Current line index should be one less than the old index!, Current[" + currentLineIndex.ToString() +", Old[" + oldLineIndex.ToString() + "]"); 
                    }
                    else
                    {
                        Point currentCaretPosition;

                        currentCaretPosition = state.Wrapper.GetDocumentRelativeCaretPosition();
                        if (state.SelectionLength == 0)
                        {
                            Verifier.Verify(currentCaretPosition.Y < state.CaretPosition.Y,
                                "After UP, caret top (" + currentCaretPosition.Y + " is above " +
                                "previous caret position (" + state.CaretPosition.Y + ").", true);
                        }
                    }
                }
            }
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>Value for keyboard editing setting.</summary>
        private KeyboardEditingTestValue _testValue;

        /// <summary>Command that can be used to perform this action (null if not available).</summary>
        private RoutedCommand _command;

        /// <summary>String that can be typed to perform this action (empty if not available).</summary>
        private string _executionTypeString;

        /// <summary>Method that can be invoked to perform this action.</summary>
        private ExecutionCallback _execute;

        /// <summary>Method to be invoked to verify this action.</summary>
        private VerifyCallback _verify;

        #endregion Private fields.


        #region Inner types.

        /// <summary>
        /// Callback delegate type for performing an action.
        /// </summary>
        delegate void ExecutionCallback(UIElementWrapper wrapper, SimpleHandler handler, bool preferCommand);

        /// <summary>
        /// Callback delegate type for verifying an action.
        /// </summary>
        delegate void VerifyCallback(KeyboardEditingState state);

        #endregion Inner types.

        #region Tests.

        /*

        [STAThread]
        private static void Main()
        {
            TestFindNextWordBoundaryIndexPlain();
            TestFindNextWordBoundaryIndexRich();
            TestIsCharStandaloneWord();
        }

        private static void Assert(bool condition, string conditionDescription)
        {
            if (!condition) throw new ApplicationException("Assertion failed: " + conditionDescription);
        }

        private static void TestFindNextWordBoundaryIndexPlain()
        {
            ArrayTextContainer container;

            // Verify that plain text finds the right word boundaries.
            container = new ArrayTextContainer("one two");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                0, LogicalDirection.Forward, true, false) == 0,
                "Start of plain text is word boundary.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                0, LogicalDirection.Forward, false, false) == 4,
                "Start of plain text can move to next word boundary.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                1, LogicalDirection.Forward, false, false) == 4,
                "Middle of word in plain text can move to next word boundary.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                4, LogicalDirection.Forward, false, false) == 7,
                "Start word in plain text can move to next word boundary.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                7, LogicalDirection.Forward, false, false) == -1,
                "End-of-text in plain text returns -1.");

            // Verify the empty text cases.
            container = new ArrayTextContainer("");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                0, LogicalDirection.Forward, false, false) == -1,
                "Empty text handled correctly for false start-is-valid.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                0, LogicalDirection.Forward, true, false) == 0,
                "Empty text handled correctly for true start-is-valid.");

            // Verify tab handling.
            container = new ArrayTextContainer("\t");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                0, LogicalDirection.Forward, false, false) == 1,
                "Tab handled correctly for plain text at start.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                1, LogicalDirection.Forward, false, false) == -1,
                "Tab handled correctly for plain text at end.");
        }

        private static void TestFindNextWordBoundaryIndexRich()
        {
            ArrayTextContainer container;

            // Verify that empty text is handled gracefully.
            container = new ArrayTextContainer();
            container.InsertElement(0, 0, new Paragraph());
            container.InsertElement(1, 1, new Run());
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                0, LogicalDirection.Forward, true, true) == 2,
                "Empty text handled correctly for absolute first position.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                1, LogicalDirection.Forward, true, true) == 2,
                "Empty text handled correctly for position between P and R with start-valid flag.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                1, LogicalDirection.Forward, false, true) == 4,
                "Empty text handled correctly for position between P and R.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                2, LogicalDirection.Forward, true, true) == 2,
                "Empty text handled correctly for position between R and /R.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                2, LogicalDirection.Forward, false, true) == 4,
                "Empty text handled correctly for position between R and /R with start-invalid flag.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                3, LogicalDirection.Forward, false, true) == 4,
                "Empty text handled correctly for position between /R and /P.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                4, LogicalDirection.Forward, true, true) == 4,
                "Empty text handled correctly for position after /P with start-valid flag.");
            Assert(FindNextWordBoundaryIndex(container, FlowDirection.LeftToRight,
                4, LogicalDirection.Forward, false, true) == -1,
                "Empty text handled correctly for position after /P with start-invalid flag.");
        }

        private static void TestIsCharStandaloneWord()
        {
            Assert(IsCharStandaloneWord('\t'), "'\\t' is a standalone word character.");
            Assert(!IsCharStandaloneWord('a'), "'a' is not a standalone word character.");
        }

        */

        #endregion Tests.
    }
}
