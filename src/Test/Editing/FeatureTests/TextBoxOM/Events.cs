// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for TextBox events.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/Events.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;    

    #endregion Namespaces.
    
    /// <summary>
    /// Possibilities for Keyboard Input
    /// </summary>
    enum KeyboardInputs
    {
        Alphabets,
        Numbers,
        Spaces,
        Tab,
    }  

    /// <summary>
    /// triggers used in this test case
    /// </summary>
    enum TextChangeSetAndClearTriggerList
    {
        /// <summary>Sets the Text property on the control.</summary>
        SetTextProperty,
        /// <summary>Clears the Text Property of Control.</summary>
        ClearTextProperty,
        /// <summary> clears the text property using keyboard. </summary>
        ClearTextPropertyKB,
    }    

    /// <summary>
    /// Possibilities for Keyboard Input
    /// </summary>
    enum TextChangeTextManipulationList
    {
        InterestingKeyboardInputs,
        SelectAndSetPropertyKB,
        SelectAndSetProperty,
    }

    /// <summary> Types of keyboard data  </summary>
    enum TextChangeKeyBoardDataList
    {
        NoTriggerKeyboardKeys,
        TriggerKeyboardKeys,
    }

    /// <summary>
    /// Verifies that the TextChanged event is fired a given
    /// number of times. Data should cover the following cases.
    ///
    /// - A key is typed.
    /// - Text is overwritten.
    /// - Text is backspaced.
    /// - Backspace on an empty TextBox modifies nothing.
    /// - Tabbing to another control modifies nothing.
    /// - ESC modifies nothing.
    /// - Return is typed.
    /// - Caret movements modify nothing.
    /// - Unhooking the event works as expected.
    /// </summary>
    [Test(0, "TextBox", "TextBoxTextChangedSimple", MethodParameters = "/TestCaseType=TextBoxTextChangedSimple")]
    [TestOwner("Microsoft"), TestTactics("596"), TestBugs("621,622")]
    public class TextBoxTextChangedSimple: TextBoxTestCase
    {
        #region Private data.

        private int _eventCount;
        private bool _textInputFired;
        
        class TestCaseData
        {
            internal int ExpectedEventCount;
            internal string KeyStrokes;
            internal bool Unhook;
            internal bool AcceptsReturn;
            
            internal TestCaseData(int expectedEventCount, string keyStrokes, bool unhook, bool acceptsReturn)
            {
                this.ExpectedEventCount = expectedEventCount;
                this.KeyStrokes = keyStrokes;
                this.Unhook = unhook;
                this.AcceptsReturn = acceptsReturn;
            }
        }
        
        private TestCaseData[] _cases = new TestCaseData[] {
            // Replace Tactics #597 - accepts return.
            new TestCaseData(1, "{ENTER}", false, true),
            // Replace Tactics #598 - caret.
            new TestCaseData(1, "a{LEFT}", false, false),
            // Replace Tactics #599 - backspace.
            new TestCaseData(3, "ab{BS}", false, false),
            // Replace Tactics #600 - backspace empty.
            new TestCaseData(0, "{BS}{BS}", false, false),
            // Replace Tactics #601 - escape.
            new TestCaseData(2, "a{ESC}b", false, false),
            // Replace Tactics #596 - key.
            new TestCaseData(2, "ab", false, false),
            // Replace Tactics #603 - unhook.
            new TestCaseData(0, "abc", true, false),
            // Replace Tactics #604 - overwrite. Pending work item #Regression_Bug624.
            // new TestCaseData(3, "ab+{LEFT}c", false, false),
        };
        
        private TestCaseData _caseData;
        private int _caseDataIndex;

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _caseDataIndex = 0;
            StartTestCase();
        }
        
        private void StartTestCase()
        {
            Log("\r\n\r\nRunning test case with data set #" + _caseDataIndex);
            _caseData = _cases[_caseDataIndex];
            Log("Expected event count: " + _caseData.ExpectedEventCount);

            TestTextBox.TextChanged += new TextChangedEventHandler(TextChanged);
            TestTextBox.KeyDown += ElementKeyDown;
            if (_caseData.Unhook)
            {
                TestTextBox.TextChanged -= new TextChangedEventHandler(TextChanged);
                TestTextBox.KeyDown -= ElementKeyDown;
            }

            TestTextBox.TextInput += ElementTextInput;
            MainWindow.TextInput += WindowTextInput;

            TestTextBox.AcceptsReturn = _caseData.AcceptsReturn;
            MouseInput.MouseClick(TestTextBox);
            QueueDelegate(DoTyping);
        }

        private void DoTyping()
        {
            KeyboardInput.TypeString(_caseData.KeyStrokes);
            QueueDelegate(DoVerification);
        }

        private void DoVerification()
        {
            Log("Expected event count: " + _caseData.ExpectedEventCount);
            Log("Actual event count:   " + _eventCount);
            Verifier.Verify(_caseData.ExpectedEventCount == _eventCount);

            EndTestCase();
        }
        
        private void EndTestCase()
        {
            _caseDataIndex++;
            if (_caseDataIndex < _cases.Length)
            {   
                // Clean up and start over.
                TestTextBox.Text = "";
                _eventCount = 0;
                _textInputFired = false;
                TestTextBox.TextInput -= ElementTextInput;
                MainWindow.TextInput -= WindowTextInput;
                if (!_caseData.Unhook)
                {
                    TestTextBox.TextChanged -= new TextChangedEventHandler(TextChanged);
                    TestTextBox.KeyDown -= ElementKeyDown;
                }
                
                QueueDelegate(StartTestCase);
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs args)
        {
            Verifier.Verify(args.RoutedEvent == TextBox.TextChangedEvent,
                "TextChanged called - arguments carry the TextBox.TextChangedEvent", false);
            Verifier.Verify(TestTextBox.IsKeyboardFocused, "TextBox.IsKeyboardFocused is true", false);
            Log("TextChanged event source: " + args.Source);
            Verifier.Verify(args.Source == TestTextBox, "The source of the event is the TextBox", false);
            _eventCount++;
        }

        private void ElementTextInput(object sender, TextCompositionEventArgs e)
        {
            Log("TextInput fired on element: " + sender);
            _textInputFired = true;
        }

        private void WindowTextInput(object sender, TextCompositionEventArgs e)
        {
            Log("TextInput fired on window: " + sender);
            Log("Verifying that this bubbled from edited element...");
            Verifier.Verify(_textInputFired, "Text input already fired.", true);
        }

        private void ElementKeyDown(object sender, KeyEventArgs e)
        {
            Log("KeyDown key: " + e.Key + " - sender " + sender + " - source " + e.Source);

            Verifier.Verify(sender == TestTextBox,
                "The sender of the KeyDown event is the TextBox", true);
            Verifier.Verify(e.Source == TestTextBox,
                "The source of the KeyDown event is the TextBox", true);
            Verifier.Verify(TestTextBox.IsKeyboardFocused, "TextBox.IsKeyboardFocused is true", true);
        }

        #endregion Main flow.
    }


    /// <summary>
    /// Verifies that modifying the TextBox.Text member on mouse up
    /// does not deadlock.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("595"), TestBugs("623")]
    [Test(3, "TextBox", "TextBoxReproRegression_Bug623", MethodParameters = "/TestCaseType=TextBoxReproRegression_Bug623")]
    public class TextBoxReproRegression_Bug623: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            TestTextBox.AddHandler(Mouse.MouseUpEvent,
                new MouseButtonEventHandler(TextBoxMouseUp), true);
            MouseInput.MouseClick(TestTextBox);
        }

        private void TextBoxMouseUp(object sender, MouseButtonEventArgs args)
        {
            if(args.ChangedButton != MouseButton.Left) return;

            Log("Appending text...");
            TestTextBox.Text += "some text";

            Log("Text appended...");
            QueueDelegate(new SimpleHandler(ContinueTest));
        }

        private void ContinueTest()
        {
            Log("Input did not stall. Test passes.");
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    ///Helper class - Has the commonly used TextChangeEvent functions
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("123"), TestWorkItem("104")]
    public class TextChangeEventHelper : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = EditableType.CreateInstance();
            if (_element is TextBoxBase)
            {
                ((TextBoxBase)_element).AcceptsTab = true;
                ((TextBoxBase)_element).AcceptsReturn = true;
            }

            HookUpEvent(_element);
            _fireCount = _unhookCount = 0;
            TestElement = _element;
            QueueDelegate(DoFocus);
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            _element.Focus();
        }

        /// <summary>Hooks up the handlers</summary>
        public void HookUpEvent(FrameworkElement element)
        {
            if (element is TextBoxBase)
            {
                ((TextBoxBase)element).TextChanged += TextChangedHandler;
            }
            else if (element is PasswordBox)
            {
                ((PasswordBox)element).PasswordChanged += PasswordChangedHandler;
            }
        }

        /// <summary>PasswordBox.PasswordChanged event handler</summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        public virtual void PasswordChangedHandler(object sender, RoutedEventArgs e)
        {
            Verifier.Verify((sender == _element), " event triggered by PasswordBox" + sender.GetType().Name, false);
            _fireCount++;
        }

        /// <summary>textboxbase handler</summary>
        /// <param name="sender">sender</param>
        /// <param name="e">TextChangedEventArgs</param>
        public virtual void TextChangedHandler(object sender, TextChangedEventArgs e)
        {
            Verifier.Verify((sender == _element), " event triggered by "+ sender.GetType().Name, false);
            _fireCount++;
        }

        /// <summary>remove the handler</summary>
        public void RemoveEvent(FrameworkElement element)
        {
            if (element is TextBoxBase)
            {
                ((TextBoxBase)element).TextChanged -= TextChangedHandler;
                _unhookCount++;
            }
            else if (element is PasswordBox)
            {
                ((PasswordBox)element).PasswordChanged -= PasswordChangedHandler;
                _unhookCount++;
            }
        }

        /// <summary> Test unhooking of handler </summary>
        public void RemoveHandlerTest()
        {
            _fireCount = 0;
            RemoveEvent(_element);
            SetText(_element, "hello");

            Verifier.Verify(_fireCount == 0 && _unhookCount == 1,
                 "Event was fired [" + _fireCount +"] times for " + _element.GetType().Name, true);
        }

        /// <summary>Set text programmatically</summary>
        /// <param name="element">Control element</param>
        /// <param name="text">text to be set</param>
        public void SetText(FrameworkElement element, string text)
        {

            if (element is TextBox)
            {
                ((TextBox)element).Text = text;
            }
            else if (element is RichTextBox)
            {
                ((RichTextBox)element).Document.Blocks.Add(
                    new Paragraph(new Run(text)));
            }
            else if (element is PasswordBox)
            {
                ((PasswordBox)element).Password = text;
            }
        }

        #region Public Data.

        /// <summary> type of control </summary>
        public TextEditableType EditableType;
        /// <summary> tracking of events fired and unhook events </summary>
        public int _fireCount, _unhookCount;
        /// <summary> control element </summary>
        public FrameworkElement _element;

        #endregion Public Data.
    }

    /// <summary>
    ///tests keyboard inputs to trigger a textchanged event
    /// Tests alphabets, numerics, spaces and tab
    /// Tests proper control invoked handler
    /// Tracks invocation count
    /// Tests Unhooking
    /// </summary>
    [Test(0, "TextBoxBase", "TextChangeEventKeyboardInput", MethodParameters = "/TestCaseType:TextChangeEventKeyboardInput", Timeout = 300)]
    [TestOwner("Microsoft"), TestTactics("594"), TestBugs("624"), TestWorkItem("104")]
    public class TextChangeEventKeyboardInput : TextChangeEventHelper
    {
        /// <summary>initial setup.</summary>
        protected override void DoRunCombination()
        {
            base.DoRunCombination();

            _pendingCount = _expectedCount = 3;

            QueueDelegate(ExecuteTrigger);
        }

        /// <summary> switches between different possibilities of keyboard input </summary>
        private void ExecuteTrigger()
        {
            if (_pendingCount > 0)
            {
                switch (_keyboardInputSwitch)
                {
                    case KeyboardInputs.Alphabets:
                        int index;
                        index = 'a' + _pendingCount;
                        char ch = (char)index;
                        _str = ch.ToString();
                        KeyboardInput.TypeString(_str);
                        break;

                    case KeyboardInputs.Numbers:
                        int num = _pendingCount + 1;
                        _str = num.ToString();
                        KeyboardInput.TypeString(_str);
                        break;

                    case KeyboardInputs.Spaces:
                        KeyboardInput.TypeString(" ");
                        break;

                    case KeyboardInputs.Tab:
                        if (_element is RichTextBox)
                        {
                            //On empty string Rich text Box takes tab into indentation
                            KeyboardInput.TypeString("{TAB}a");
                        }
                        if (_element is TextBox)
                        {
                            KeyboardInput.TypeString("{TAB}");
                        }
                        if (_element is PasswordBox)
                        {
                            //tab not handled in passwordbox
                            _expectedCount = 0;
                        }
                        break;

                    default:
                        break;
                }
                QueueDelegate(ExecuteTrigger);
            }
            else
            {
                QueueDelegate(CheckAfterInput);
            }
            _pendingCount--;
        }

        /// <summary>verifies the trigger count </summary>
        private void CheckAfterInput()
        {
            //because input is "tab a" and first tab goes into indentation
            if ((_element is RichTextBox) && (_keyboardInputSwitch == KeyboardInputs.Tab) && (_expectedCount > 1))
            {
                //2*expectedCount because the characters input for RichTextbox are "tab A"
                _expectedCount = _expectedCount * 2 ;
            }
            Verifier.Verify(_fireCount == _expectedCount,
                "ActualCount[" + _fireCount + "]   " + "ExpectedCount[" + _expectedCount + "]", true);

            RemoveHandlerTest();

            NextCombination();
        }

        #region Private Data.

        /// <summary>Tracks number of iterations and expected number of fired events </summary>
        public int _pendingCount, _expectedCount;
        private KeyboardInputs _keyboardInputSwitch=0;
        private string _str = "";

        #endregion Private Data.
    }

    /// <summary>
    /// checks SET and CLEAR functions (keyboard + programmatically)
    /// Tests proper control invoked handler
    /// Tracks invocation count
    /// Tests Unhooking
    /// </summary>
    [Test(2, "TextBoxBase", "TextChangeTestSetAndClear", MethodParameters = "/TestCaseType:TextChangeTestSetAndClear", Timeout = 240)]
    [TestOwner("Microsoft"), TestTactics("593"), TestBugs("625, 626"), TestWorkItem("104")]
    public class TextChangeTestSetAndClear : TextChangeEventHelper
    {
        /// <summary>initial setup.</summary>
        protected override void DoRunCombination()
        {
            base.DoRunCombination();

            _sampleData = StringData.Values;
            _pendingCount = _expectedCount = _sampleData.Length;

            _textControlWrapper = new Test.Uis.Wrappers.UIElementWrapper(_element);
            QueueDelegate(ExecuteTrigger);
        }

        /// <summary> switches between different possibilities of keyboard input</summary>
        private void ExecuteTrigger()
        {
            if (_pendingCount > 0)
            {
                if ((_sampleData[_pendingCount - 1].Value != null) && (!_sampleData[_pendingCount - 1].IsLong))
                {
                    SetText(_element, _sampleData[_pendingCount - 1].Value); //51 valid values
                    switch (_textChangeSetAndClearTrigger)
                    {
                        case TextChangeSetAndClearTriggerList.SetTextProperty:
                            break;

                        case TextChangeSetAndClearTriggerList.ClearTextProperty:
                            if (_element is TextBox)
                            {
                                ((TextBox)_element).Clear();
                            }
                            else if (_element is PasswordBox)
                            {
                                ((PasswordBox)_element).Clear();
                            }
                            else if (_element is RichTextBox)
                            {
                                //TextRange range = new TextRange(TextControlWrapper.Start, TextControlWrapper.End);
                                //range.Text = string.Empty;
                                ((RichTextBox)_element).Document.Blocks.Clear();
                            }
                            break;

                        case TextChangeSetAndClearTriggerList.ClearTextPropertyKB:
                            KeyboardInput.TypeString("^a{DEL}");
                            break;

                        default:
                            break;
                    }

                }
                else
                {
                    _expectedCount--;
                }
                QueueDelegate(ExecuteTrigger);
            }
            else
            {
                QueueDelegate(CheckAfterInput);
            }
            _pendingCount--;            
        }

        /// <summary> verifies the trigger count </summary>
        private void CheckAfterInput()
        {
            
            if ((_textChangeSetAndClearTrigger == TextChangeSetAndClearTriggerList.ClearTextProperty)||
                     (_textChangeSetAndClearTrigger == TextChangeSetAndClearTriggerList.ClearTextPropertyKB))
            {
                if (_element is RichTextBox)
                {
                    //51 valid values are set and cleared. hence _expectedCount*2
                    //event is triggered even if richTextBox is cleared("")
                    Verifier.Verify(_fireCount == (_expectedCount * 2),
                            "ActualCount[" + _fireCount + "]   " + "ExpectedCount[" + (_expectedCount * 2) + "]", true); 
                }
                else
                {
                    //51 valid values are set and cleared. hence _expectedCount*2
                    //For Password and Textbox no event is triggered when input is "" and it is already ""(cleared)
                    //hence no set and clear trigger is called. As such _expectedCount * 2 - 2
                    Verifier.Verify(_fireCount == (_expectedCount * 2 - 2),
                            "ActualCount[" + _fireCount + "]   " + "ExpectedCount[" + (_expectedCount * 2 - 2) +"]", true); 
                }
            }
            else
            {   //TextChangeSetAndClearTriggerList.SetTextProperty
                //51 valid values are set. hence _expectedCount
                //when input is "" the string is already initialised to something else
                //so for password and textbox an event DOES occur
                Verifier.Verify((_fireCount) == (_expectedCount),
                    "ActualCount[" + _fireCount + "]   " + "ExpectedCount[" + _expectedCount + "]", true);
            }

            RemoveHandlerTest();

            NextCombination();
        }

        #region Private Data.

        private TextChangeSetAndClearTriggerList _textChangeSetAndClearTrigger=0;
        /// <summary> Wrapper for control </summary>
        private Test.Uis.Wrappers.UIElementWrapper _textControlWrapper;
        private StringData[] _sampleData;
        private int _pendingCount, _expectedCount;

        #endregion Private Data.
    }


    /// <summary>
    /// Tests Looping from within handler
    /// Tests proper control invoked handler
    /// Tracks invocation count
    /// </summary>
    //[Test(1, "SubArea", TestCaseSecurityLevel.PartialTrust, "TestName", MethodParameters = "/TestCaseType:TextChangeEventTestLoop", Timeout = 120)]
    [Test(2, "TextBoxBase", "TextChangeEventTestLoop", MethodParameters = "/TestCaseType:TextChangeEventTestLoop")]
    [TestOwner("Microsoft"), TestTactics("592"), TestWorkItem("104")]
    public class TextChangeEventTestLoop : TextChangeEventHelper
    {
        /// <summary>initial setup.</summary>
        protected override void DoRunCombination()
        {
            _element = EditableType.CreateInstance();

            HookUpEventHandler(_element);
            _fireLoopCount = 0;
            _unhookCount = 0;
            TestElement = _element;

            QueueDelegate(ExecuteTrigger);
        }

        /// <summary> Hook up handlers </summary>
        public void HookUpEventHandler(FrameworkElement element)
        {
            if (element is TextBoxBase)
            {
                ((TextBoxBase)element).TextChanged += TextChangedHandler1;
            }
            else if (element is PasswordBox)
            {
                ((PasswordBox)element).PasswordChanged += TextChangedHandler1;
            }
        }

        /// <summary> switches between different possibilities of keyboard input </summary>
        private void ExecuteTrigger()
        {
            SetText(_element, "hello");
            QueueDelegate(CheckAfterInput);
        }

        /// <summary> verifies the trigger count </summary>
        private void CheckAfterInput()
        {
            Verifier.Verify(_fireLoopCount == 1,
                  "Looping. Event was fired[" + _fireLoopCount +"] times. Expected Count [1]", true);
            RemoveHandlerTest();
            NextCombination();
        }

        /// <summary> Text change handler </summary>
        private void TextChangedHandler1(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent == TextBoxBase.TextChangedEvent)
            {
                Verifier.Verify((sender == _element), " event triggered by TextBoxBase", false);
            }
            else if (e.RoutedEvent == PasswordBox.PasswordChangedEvent)
            {
                Verifier.Verify((sender == _element), " event triggered by PasswordBox", false);
            }
            _fireLoopCount++;

            // Exercising control functions to check to see if they cause a loop
            if (_fireLoopCount > 1)
            {
                return;
            }
            if (_element is TextBox)
            {
                ((TextBox)_element).Select(1, 2);
                ((TextBox)_element).CharacterCasing = CharacterCasing.Lower;
                ((TextBox)_element).CaretIndex = 1;
                ((TextBox)_element).Copy();
                ((TextBox)_element).SelectAll();
           //     SetText(_element, "once again"); //this should not cause an infinite loop
            }
            else if (_element is PasswordBox)
            {
                ((PasswordBox)_element).AllowDrop = true;
                ((PasswordBox)_element).SelectAll();
                ((PasswordBox)_element).VerifyAccess();
             //   SetText(_element, "once again");//this should not cause an infinite loop

            }
            else if (_element is RichTextBox)
            {
                ((RichTextBox)_element).SelectAll();
                ((RichTextBox)_element).ToString();
                ((RichTextBox)_element).PageRight();
                object o = ((RichTextBox)_element).GetType();
                ((RichTextBox)_element).Copy();
                bool val = ((RichTextBox)_element).CanRedo;
               // SetText(_element, "once again");//this should not cause an infinite loop
            }
        }

        #region Private Data.

        private int _fireLoopCount;

        #endregion Private Data.
    }

    /// <summary>
    ///tests keyboard slelection inputs to trigger a textchanged event
    /// Tests proper control invoked handler
    /// Tracks invocation count
    /// </summary>
    [Test(2, "TextBoxBase", "TextChangeEventKeyboardSelectionInputs", MethodParameters="/TestCaseType:TextChangeEventKeyboardSelectionInputs", Timeout=300)]
    [TestOwner("Microsoft"), TestTactics("591"), TestWorkItem("104"), TestLastUpdatedOn("June 6, 2006")]
    public class TextChangeEventKeyboardSelectionInputs : TextChangeEventHelper
    {
        /// <summary> initial setup. </summary>
        protected override void DoRunCombination()
        {
            base.DoRunCombination();

            _textControlWrapper = new Test.Uis.Wrappers.UIElementWrapper(_element);
            _sampleKeyboardData = KeyboardEditingData.GetValues(
                                KeyboardEditingTestValue.AltNumpadKeys,
                                KeyboardEditingTestValue.BoldCommandKeys,
                                KeyboardEditingTestValue.CenterJustifyCommandKeys,
                                KeyboardEditingTestValue.CopyCommandKeys,
                                KeyboardEditingTestValue.CutCommandKeys,
                                KeyboardEditingTestValue.DownArrow,
                                KeyboardEditingTestValue.PageDown,
                                KeyboardEditingTestValue.UnderlineCommandKeys,
                                KeyboardEditingTestValue.UpArrowShift);

            _sampleKeyboardTriggerData = KeyboardEditingData.GetValues(
                                KeyboardEditingTestValue.UndoCommandKeys,
                                KeyboardEditingTestValue.BackspaceControl,
                                KeyboardEditingTestValue.AlphabeticShift,
                                KeyboardEditingTestValue.Backspace,
                                KeyboardEditingTestValue.BackspaceShift,
                                KeyboardEditingTestValue.Enter,
                                KeyboardEditingTestValue.Delete);
            

            _sampleKeyboardSelectionData = KeyboardEditingData.GetValues(
                                KeyboardEditingTestValue.BackspaceShift,
                                KeyboardEditingTestValue.HomeShift,
                                KeyboardEditingTestValue.UpArrowShift,
                                KeyboardEditingTestValue.LeftArrowShift,
                                KeyboardEditingTestValue.PageUpShift,
                                KeyboardEditingTestValue.EndShift,
                                KeyboardEditingTestValue.DownArrowShift,
                                KeyboardEditingTestValue.PageDownShift,
                                KeyboardEditingTestValue.RightArrowShift
                                );

            if (_textChangeTextManipulationSwitch == TextChangeTextManipulationList.InterestingKeyboardInputs)
            {
                if (_textChangeKeyBoardDataSwitch == TextChangeKeyBoardDataList.NoTriggerKeyboardKeys)
                {
                    _pendingCount = _sampleKeyboardData.Length;
                }
                else
                {
                    _pendingCount = _sampleKeyboardTriggerData.Length;
                }
            }
            else if (_textChangeTextManipulationSwitch == TextChangeTextManipulationList.SelectAndSetPropertyKB)
            {
                _pendingCount = _sampleKeyboardSelectionData.Length;
            }
            else if (_textChangeTextManipulationSwitch == TextChangeTextManipulationList.SelectAndSetProperty)
            {
                _pendingCount = _stringDataArray.Length;
            }
            SetText(_element, "abcd efg   hij klm      nopqrstuvwxyz");
            --_fireCount;  //done so that trigger counts in executeTrigger() is easier to track

            QueueDelegate(ExecuteTrigger);
        }

        /// <summary> switches between different possibilities of keyboard input </summary>
        private void ExecuteTrigger()
        {
            if (_pendingCount > 0)
            {
                switch (_textChangeTextManipulationSwitch)
                {
                    case TextChangeTextManipulationList.InterestingKeyboardInputs:
                        switch (_textChangeKeyBoardDataSwitch)
                        {
                            case TextChangeKeyBoardDataList.NoTriggerKeyboardKeys:
                                _sampleKeyboardData[_pendingCount-1].PerformAction(_textControlWrapper, QueueAction);
                                break;

                            case TextChangeKeyBoardDataList.TriggerKeyboardKeys:
                                KeyboardInput.TypeString("{RIGHT 5}");
                                _sampleKeyboardTriggerData[_pendingCount - 1].PerformAction(_textControlWrapper, QueueAction);
                                break;

                            default:
                                break;
                        }
                        break;

                    case TextChangeTextManipulationList.SelectAndSetPropertyKB:
                        ClearRichTextBox();
                        SetText(_element, "abcd efg   hij klm      nopqrstuvwxyz" + _pendingCount.ToString());
                        _fireCount--;
                        //the top 6 combinations require the cursor to be NOT in starting position
                        SelectRTB();
                        break;

                    case TextChangeTextManipulationList.SelectAndSetProperty:
                        ClearRichTextBox();  
                        //_stringDataArray had "", " ",  "sample data"
                        SetText(_element, _stringDataArray[_pendingCount - 1]); 
                        _fireCount--;

                        SelectAndPasteProgrammatically();
                        QueueDelegate(ExecuteTrigger);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                QueueDelegate(CheckAfterInput);
            }
            _pendingCount--;
        }

        private void SelectRTB()
        {
            if (_pendingCount < 6)
            {
                KeyboardInput.TypeString("{END}");
            }
            else
            {
                KeyboardInput.TypeString("^{HOME}");
            }
            _sampleKeyboardSelectionData[_pendingCount - 1].PerformAction(_textControlWrapper, ReplaceSelection);
        }

        /// <summary> Replaces Selected string with letter A </summary>
        private void ReplaceSelection()
        {
            _element.Focus();
            if (_element is RichTextBox)
            {
                switch (_options)
                {
                    case 0:
                        KeyboardInput.TypeString(EditingCommandData.ToggleBold.KeyboardShortcut);
                        break;

                    case 1:
                        KeyboardInput.TypeString(EditingCommandData.ToggleItalic.KeyboardShortcut);
                        break;

                    case 2:
                        KeyboardInput.TypeString(EditingCommandData.ToggleUnderline.KeyboardShortcut);
                        break;
                }
                Log("Option number used "+_options.ToString());
                _options = ++_options % 3;
            }
            else
            {
                KeyboardInput.TypeString("{DELETE}A");
                --_fireCount;
            }
            QueueDelegate(QueueAction);
        }

        /// <summary> calls Delegate so that keyboard input is processesed in right order </summary>
        private void QueueAction()
        {
            QueueDelegate(ExecuteTrigger);
        }

        /// <summary> Clears the Richtextbox </summary>
        private void ClearRichTextBox()
        {
            //The value needs to be reset because one of the combinations erases the text
            //clears RichTextBox, Otherwise text gets appended
            if (_element is RichTextBox)
            {
                ((RichTextBox)_element).Document.Blocks.Clear();
                _fireCount--;
            }
        }

        /// <summary> sets the text of control programmatically using SelectedText and Paste functions </summary>
        private void SelectAndPasteProgrammatically()
        {
            //However there is no trigger for "" hence the --firecount statement after settext reduces to 5
            if (_element is TextBox)
            {
                ((TextBox)_element).Select(2, 5);
                ((TextBox)_element).SelectedText = "b";
            }
            else
                if ((_element is PasswordBox) || (_element is RichTextBox))
                {
                    Clipboard.SetDataObject("hello");
                    QueueDelegate(PasteClipboard);
                }
        }

        /// <summary> pastes whatever is in the clipboard </summary>
        private void PasteClipboard()
        {
            if (_element is PasswordBox)
            {
                ((PasswordBox)_element).Paste(); //whatever is in the clipboard
            }
            else if (_element is RichTextBox)
            {
                ((RichTextBox)_element).Paste();
            }
        }

        /// <summary> verifies the trigger count </summary>
        private void CheckAfterInput()
        {
            if (_textChangeTextManipulationSwitch == TextChangeTextManipulationList.InterestingKeyboardInputs)
            {
                if (_textChangeKeyBoardDataSwitch == TextChangeKeyBoardDataList.NoTriggerKeyboardKeys)
                {
                    if (_element is RichTextBox)
                    {
                        //triggered for center alignment
                        Verifier.Verify(_fireCount == 1,
                        "NoTriggerKeyboardKeys ActualCount[" + _fireCount + "]   " + "ExpectedCount[1]", true);
                    }
                    else
                    {
                        Verifier.Verify(_fireCount == 0,
                        "NoTriggerKeyboardKeys ActualCount[" + _fireCount + "]   " + "ExpectedCount[0]", true);
                    }
                }
                else if (_element is PasswordBox)
                {
                    //TextChangeKeyBoardDataList.TriggerKeyboardKeys
                    // -2 because Enter and Undo cannot be processed 
                    {
                        Verifier.Verify(_fireCount == (_sampleKeyboardTriggerData.Length - 2),
                        "TriggerKeyboardKeys ActualCount[" + _fireCount + "]   " + "ExpectedCount[" + (_sampleKeyboardTriggerData.Length - 2) + "]", true);
                    }
                }
                else
                {
                    //TextChangeKeyBoardDataList.TriggerKeyboardKeys
                    Verifier.Verify(_fireCount == (_sampleKeyboardTriggerData.Length),
                    "TriggerKeyboardKeys Actual Count[" + _fireCount + "]   " + "ExpectedCount[" + (_sampleKeyboardTriggerData.Length) + "]", true);
                }
            }
            else if (_textChangeTextManipulationSwitch == TextChangeTextManipulationList.SelectAndSetPropertyKB)
            {
                if (_element is PasswordBox)
                {
                    Verifier.Verify(_fireCount == (_sampleKeyboardSelectionData.Length - 1),
                        "SelectAndSetPropertyKB Actual Count [" + _fireCount + "]   " + "ExpectedCount[" + _sampleKeyboardSelectionData.Length + " - 1]", true);
                }
                else
                {
                    Verifier.Verify(_fireCount == _sampleKeyboardSelectionData.Length,
                        "SelectAndSetPropertyKB Actual Count [" + _fireCount + "]   " + "ExpectedCount[" + _sampleKeyboardSelectionData.Length + "]", true);
                }
            }
            else if (_textChangeTextManipulationSwitch == TextChangeTextManipulationList.SelectAndSetProperty)
            {
                Verifier.Verify(_fireCount == (_stringDataArray.Length),
                    "SelectAndSetProperty Actual Count [" + _fireCount + "]   " + "ExpectedCount[" + (_stringDataArray.Length)+ "]", true);
            }

            RemoveHandlerTest();
            NextCombination();
        }

        #region Private Data.

        private int _pendingCount;
        private TextChangeTextManipulationList _textChangeTextManipulationSwitch=0;
        private TextChangeKeyBoardDataList _textChangeKeyBoardDataSwitch=0;
        private KeyboardEditingData[] _sampleKeyboardData;
        private KeyboardEditingData[] _sampleKeyboardTriggerData;
        private KeyboardEditingData[] _sampleKeyboardSelectionData;
        private string[] _stringDataArray ={ "", " ", "sample data" };
        /// <summary>   control wrapper </summary>
        private Test.Uis.Wrappers.UIElementWrapper _textControlWrapper;
        private int _options = 0;

        #endregion Private Data.
    }

    /// <summary>Test the Text/Selection changed events and copy/Paste events for recoverbility.</summary>
    [TestOwner("Microsoft"), TestTactics("590"), TestBugs(""), TestWorkItem("103"), TestLastUpdatedOn("6-15-2006")]
    public class TextControlRecoverbilityTest : TextChangeEventHelper
    {
        /// <summary>initial setup.</summary>
        protected override void DoRunCombination()
        {
            base.DoRunCombination();

            _textControlWrapper = new Test.Uis.Wrappers.UIElementWrapper(_element);
            HookUpEvents(_textControlWrapper.Element as FrameworkElement);
            QueueDelegate(ChangeContent);
        }

        void ChangeContent()
        {
            //Try to catch the exception thrown from the event handler.
            try
            {
                if (_textChange)
                {
                    _textControlWrapper.Text =_setContent;
                }
                else
                {
                    if(_textControlWrapper.Element is TextBoxBase)
                    {
                        _textControlWrapper.SelectionInstance.Text= _setContent; 
                    }
                    else
                    {
                        _textControlWrapper.Text =_setContent ;
                    }
                }

                throw new Exception("No DummyException is caught!");
            }
            catch (DummyException e)
            {
                RemoveEvents(_textControlWrapper.Element as FrameworkElement);
                Log("we will ignore the excpetion: " + e.Message);
            }

            //Verify that the text is set event though an exception is through.
            Verifier.Verify(_textControlWrapper.Text.Contains(_setContent), "Failed: content is not set after the recovery!");
            
            //Recoverbility test. make sure that the control still works for user actions.
            _textControlWrapper.Text = "";
            KeyboardInput.TypeString(_typedContent);
            QueueDelegate(VerifyRecoveryFromChangedEvents);
        }

        void VerifyRecoveryFromChangedEvents()
        {
            Verifier.Verify(_textControlWrapper.Text.Contains(_typedContent), "Failed: Not able to input text after the control is recovered!");
            _textControlWrapper.SelectAll();
            if (!(_textControlWrapper.Element is PasswordBox))
            {
                QueueDelegate(TestCopyPasteEvent);
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        void TestCopyPasteEvent()
        {
            DataObject.AddCopyingHandler(_textControlWrapper.Element, new DataObjectCopyingEventHandler(CopyEventHandler));
            DataObject.AddPastingHandler(_textControlWrapper.Element, new DataObjectPastingEventHandler(PasteEventHandler));
            
            //Try to catch to exception thrown from copy/Paste event handler. 
            try
            {
                ((TextBoxBase)_textControlWrapper.Element).Copy();

                throw new Exception("No DummyException is caught!");
            }
            catch (DummyException e)
            {
                Log("we will ignore the excpetion: " + e.Message);
            }

            try
            {
                ((TextBoxBase)_textControlWrapper.Element).Paste();
                throw new Exception("No DummyException is caught!");
            }
            catch (DummyException e)
            {
                Log("we will ignore the excpetion: " + e.Message);
            }
            _textControlWrapper.Clear();
            
            //typing still works after control is recovered.
            KeyboardInput.TypeString(_finalText);

            QueueDelegate(VerifyCopyPasteEventHandler);
        }

        void VerifyCopyPasteEventHandler()
        {
            Verifier.Verify(_textControlWrapper.Text.Contains(_finalText), "Failed: Not able to input text after the control is recovered copyPasteEvent crash!");
     
            NextCombination();
        }

        void HookUpEvents(FrameworkElement element)
        {
            if (element is TextBoxBase)
            {
                ((TextBoxBase)element).TextChanged += TextChangedEventHandler;
                ((TextBoxBase)element).SelectionChanged += SelectionChangedEventHandler;            
            }
            else if (element is PasswordBox)
            {
                ((PasswordBox)element).PasswordChanged += PasswordChangedEventHandler;
            }
        }

        void RemoveEvents(FrameworkElement element)
        {
            if (element is TextBoxBase)
            {
                ((TextBoxBase)element).TextChanged -= TextChangedEventHandler;
                ((TextBoxBase)element).SelectionChanged -= SelectionChangedEventHandler;
            }
            else if (element is PasswordBox)
            {
                ((PasswordBox)element).PasswordChanged -= PasswordChangedEventHandler;
            }
        }

        void PasteEventHandler(object obj, DataObjectPastingEventArgs args)
        {
            bool caughtException = false;
            DataObject.RemovePastingHandler(obj as DependencyObject, new DataObjectPastingEventHandler(PasteEventHandler));
            
            //Try to cause crash inside the events handler
            try
            {
                args.FormatToApply = "Junck Formats";
                _textControlWrapper.Text = null;                 
            }
            catch (Exception exception)
            {
                caughtException = true;
                Log("Ingore this exception in PasteEventHandler! " + exception.Message);
            }
            Verifier.Verify(caughtException == true, "Failed - We did not caught the expected exception in PasteEventHandler!");
           
            throw new DummyException("Dummy exception!");  
        }

        void CopyEventHandler(object obj, DataObjectCopyingEventArgs args)
        {
            bool caughtException = false;
            DataObject.RemoveCopyingHandler(obj as DependencyObject, new DataObjectCopyingEventHandler(CopyEventHandler));
            
            //Try to cause crash inside the events handler
            try
            {
                _textControlWrapper.SelectionInstance.Text = null; 
            }
            catch (Exception exception)
            {
                caughtException = true;
                Log("Ingore this exception in CopyEventHandler!" + exception.Message);
            }

            Verifier.Verify(caughtException == true, "Failed - We did not caught the expected exception in CopyEventHandler!");
            
            throw new DummyException("Dummy exception!");
        }

        void PasswordChangedEventHandler(object sender, RoutedEventArgs e)
        {
            bool caughtException = false;

            //Try to cause crash inside the events handler
            try
            {
                ((PasswordBox)sender).MaxLength = -5;
            }
            catch (Exception exception)
            {
                caughtException = true; 
                Log("Ingore this exception in PasswordChangedEventHandler! " + exception.Message);
            }
            Verifier.Verify(caughtException == true, "Failed - We did not caught the expected exception in PasswordChangedEventHandler!");
            throw new DummyException("Dummy exception!");
        }

        void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            bool caughtException = false;

            //Try to cause crash inside the events handler
            try
            {
                _textControlWrapper.SelectionInstance.Text = null;
            }
            catch(Exception exception)
            {
                caughtException = true;
                Log("Ingored the excpetion caught inside the event handler! " + exception.Message);
            }
            Verifier.Verify(caughtException == true, "Failed - We did not caught the expected exception in TextChangedEventHandler!");
            throw new DummyException("Dummy exception!");
        }

        void SelectionChangedEventHandler(object sender, RoutedEventArgs e)
        {
            bool caughtException = false;

            //Try to cause crash inside the events handler
            try
            {
                _textControlWrapper.Text = null;
            }
            catch (Exception exception)
            {
                caughtException = true;
                Log("Ingored the excpetion caught inside the event handler! " + exception.Message);
            }
            Verifier.Verify(caughtException == true, "Failed - We did not caught the expected exception in SelectionChangedEventHandler!");
            
            throw new DummyException("Dummy exception!");
        }

        #region Private Data.

        /// <summary> Wrapper for control </summary>
        private Test.Uis.Wrappers.UIElementWrapper _textControlWrapper;
        
        /// <summary>TextChange or SelectionChange</summary>
        private bool _textChange=false;

        private string _setContent = "Set some junck content!";

        private string _typedContent = "Input Text afer recovery!";
        
        private string _finalText = "Final Text!";

        #endregion Private Data.
    }

    internal class DummyException : Exception
    {
        public DummyException(string str)
            : base(str)
        {
        }
    }
}
