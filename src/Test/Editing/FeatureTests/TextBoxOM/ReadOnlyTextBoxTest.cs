// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    #region Namespaces.
    using System;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using System.Threading; using System.Windows.Threading;
    using System.Windows.Documents;
    using System.Windows;
    #endregion Namespaces.

    /// <summary> ReadOnlyTextBoxTest is a class that holds the test cases for readonly mode in a textbox against user input.</summary>
    [TestOwner("Microsoft "), TestBugs(""), TestTactics("581")]
    public class ReadOnlyTextBoxTest : CommonTestCase
    {
            #region case - DragDrop
        /// <summary>Make sure that dragdrop works fine in readonly textbox </summary>
        private void DragDrop_CaseStart()
        {
            EnterFunction("DragDrop_CaseStart");
            InputMonitorManager.Current.IsEnabled = false;
            MouseInput.MouseClick(_dragFromTextBox);
            QueueDelegate(MakeSelection);
        }
        void MakeSelection()
        {
            _dragFromTextBox.SelectAll();
            MakeMouseDown(DragFromDragfromTextBox);
        }
        void DragFromDragfromTextBox()
        {
            MouseMoveSlow(10, 152, 30, 35);
            _textBox.SelectAll();
            MakeMouseUp(CheckDragFromDragfromTextBox);
        }

        void MakeMouseDown(SimpleHandler s)
        {
            MouseInput.MouseDown();
            QueueDelegate(s);
        }

        void MakeMouseUp(SimpleHandler s)
        {
            MouseInput.MouseUp();
            QueueDelegate(s);
        }

        void CheckDragFromDragfromTextBox()
        {
            Verifier.Verify(_textBox.Text == _originalString, "DragDrop: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            MakeMouseDown(DragFromReadOnlyToWritable);
        }

        void DragFromReadOnlyToWritable()
        {
            MouseMoveSlow(30, 35, 30, 340);
            MakeMouseUp(checkDragdropValue);
        }
        void checkDragdropValue()
        {
            Verifier.Verify(_textBox.Text == _originalString, "DragDrop: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            Verifier.Verify(_dropToTextBox.Text == _originalString, "DragDrop: TextBox should contain \"" + _originalString + "\" but contains \"" + _dropToTextBox.Text + "\" instead.");
            EndFunction();
            QueueDelegate(EndTest);
        }

        #endregion case - DragDrop
            #region case - KeyboardCase
        /// <summary>KeyboardCase tests keyboard operation of the readonly textbox.</summary>
        public void KeyboardCase_CaseStart()
        {
            EnterFunction("KeyboardCase_CaseStart");
            MouseInput.MouseClick(_textBox);
            QueueDelegate(KeyboardCut);
        }

        private void KeyboardCut()
        {
            KeyboardInput.TypeString("^x");  // Cut
            Verifier.Verify(_textBox.Text == _originalString, "KeyboardCut: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            QueueDelegate(KeyboardPaste);
        }

        private void KeyboardPaste()
        {
            MouseInput.MouseClick(_textBox);
            KeyboardInput.TypeString("^v");  // Paste
            Verifier.Verify(_textBox.Text == _originalString, "KeyboardPaste: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            QueueDelegate(KeyboardTyping);
        }

        private void KeyboardTyping()
        {
            MouseInput.MouseClick(_textBox);
            KeyboardInput.TypeString("This text should not affect the contents of the TextBox at all.\n");
            Verifier.Verify(_textBox.Text == _originalString, "KeyboardTyping: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            QueueDelegate(KeyboardUndo);
        }

        private void KeyboardUndo()
        {
            MouseInput.MouseClick(_textBox);
            KeyboardInput.TypeString("^z");  // Undo
            Verifier.Verify(_textBox.Text == _originalString, "KeyboardUndo: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            QueueDelegate(KeyboardRedo);
        }

        private void KeyboardRedo()
        {
            MouseInput.MouseClick(_textBox);
            KeyboardInput.TypeString("^y");  // Redo
            Verifier.Verify(_textBox.Text == _originalString, "KeyboardRedo: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            QueueDelegate(KeyboardDelete);
        }

        private void KeyboardDelete()
        {
            MouseInput.MouseClick(_textBox);
            KeyboardInput.TypeString("{Delete}");
            KeyboardInput.TypeString("{Backspace}");
            _textBox.SelectAll();
            Verifier.Verify(_textBox.Text == _originalString, "KeyboardDelete: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            EndFunction();
            QueueDelegate(EndTest);
        }
        #endregion case - KeyboardCase

        #region case - tab
        /// <summary>CheckTabbing checks the tab operation</summary>
        public void TabCase_CaseStart()
        {
            // mouse click on dropToTextBox (3rd TextBox)
            // hit tab 4 times, the tab will travel to dragFromTextBox (2nd TextBox)
            EnterFunction("TabCase_CaseStart");
            MouseInput.MouseClick(_dropToTextBox);
            _textBox.AcceptsTab = true;
            KeyboardInput.TypeString("\t\t\t\t");
            QueueDelegate(VerifyTabTrue);
        }

        private void VerifyTabTrue()
        {
            Verifier.Verify(_textBox.Text == _originalString, "CheckTabbing: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            Verifier.Verify(_textBox.IsKeyboardFocused == false, "CheckTabbing: textBox.IsKeyboardFocused should be false but it's " + _textBox.IsKeyboardFocused);
            Verifier.Verify(_dragFromTextBox.IsKeyboardFocused == true, "dragFromTextBox.IsKeyboardFocused should be true.");
            QueueDelegate(SetTabFalse);
        }

        void SetTabFalse()
        {
            _textBox.AcceptsTab = false;
            KeyboardInput.TypeString("\t\t\t\t\t\t");
            QueueDelegate(VerifyTabFalse);
        }

        private void VerifyTabFalse()
        {
            Verifier.Verify(_textBox.Text == _originalString, "CheckTabbing: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            Verifier.Verify(_textBox.IsKeyboardFocused == false, "CheckTabbing: textBox.IsKeyboardFocused should be false but it's " + _textBox.IsKeyboardFocused);
            Verifier.Verify(_dragFromTextBox.IsKeyboardFocused == true, "dragFromTextBox.IsKeyboardFocused should be true.");
            QueueDelegate(SetAcceptsReturnTrue);
        }

        private void SetAcceptsReturnTrue()
        {
            _textBox.AcceptsReturn = true;
            MouseInput.MouseClick(_textBox);
            KeyboardInput.TypeString("{Enter}");
            QueueDelegate(VerifyAcceptsReturnTrue);
        }

        void VerifyAcceptsReturnTrue()
        {
            Verifier.Verify(_textBox.Text == _originalString, "AcceptsReturn true should not be accepted. TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            QueueDelegate(SetAcceptsReturnFalse);
        }

        void SetAcceptsReturnFalse()
        {
            _textBox.AcceptsReturn = false;
            KeyboardInput.TypeString("\n");
            QueueDelegate(VerifyAcceptsReturnFalse);
        }

        void VerifyAcceptsReturnFalse()
        {
            Verifier.Verify(_textBox.Text == _originalString, "AcceptsReturn false should not be accepted. TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
            EndFunction();
            QueueDelegate(EndTest);
        }

        #endregion case - tab
        
        #region case - International
        private void InputCompositeCharacters()
        {
        }
        #endregion case - International
            #region case - MouseCase
        private void MouseDelete()
        {

        }

        private void MouseRedo()
        {

        }

        private void MouseCut()
        {

        }
        private void MouseUndo()
        {

        }
        private void MousePaste()
        {
            EnterFunction("MousePaste");
            MouseInput.MouseClick(_textBox);
            //select paste from context menu here
            Verifier.Verify(_textBox.Text == _originalString, "MousePaste: TextBox should contain \"" + _originalString + "\" but contains \"" + _textBox.Text + "\" instead.");
        }
        #endregion case - MouseCase
            #region case - Spellcheck
        private void CorrectSpellingMistake()
        {
        }

        #endregion case - Spellcheck
        #region shared methods
        /// <summary>
        /// 
        /// </summary>
        public override void Init()
        {
            System.Windows.Controls.Canvas c = new System.Windows.Controls.Canvas();
            c.Background = System.Windows.Media.Brushes.SkyBlue;
            _textBox = new System.Windows.Controls.TextBox();
            _dragFromTextBox = new System.Windows.Controls.TextBox();
            _dropToTextBox = new System.Windows.Controls.TextBox();
            c.Children.Add(_textBox as UIElement);
            c.Children.Add(_dragFromTextBox as UIElement);
            c.Children.Add(_dropToTextBox as UIElement);
            System.Windows.Controls.Canvas.SetTop(_dragFromTextBox, 150);
            System.Windows.Controls.Canvas.SetTop(_dropToTextBox, 300);
            MainWindow.Content = c;
            _textBox.Text = _originalString;
            _dragFromTextBox.Text = "This text should not change! (not readonly)";
            _textBox.TextChanged += TextBoxChanged;
            _dragFromTextBox.TextChanged += TextBoxChanged;
            _textBox.IsReadOnly = true;
            MouseInput.MouseClick(_dragFromTextBox);
            //QueueDelegate(KeyboardPaste);
        }

        private void TextBoxChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Verifier.Verify(_allowChange == true, "TextChanged event should not have been called from " + e.Source.ToString() + " but has.");
        }

        /// <summary>
        /// move the mouse pixel by pixel
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void MouseMoveSlow(int x1, int y1, int x2, int y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            double Dx = System.Math.Abs(dx);
            double Dy = System.Math.Abs(dy);
            int dragwidth = (int)((Dx > Dy) ? Dx : Dy);

            if (dragwidth != 0)
            {
                dx = dx / dragwidth;
                dy = dy / dragwidth;
                for (int i = 3; i <= dragwidth; i += 2)
                {
                    MouseInput.MouseMove(x1 + (int)(dx * i), y1 + (int)(dy * i));
                    Thread.Sleep(1);
                }
                //the last drag make sure that the mouse goes to the exact position
                MouseInput.MouseMove(x2, y2);
            }
        }
        bool _allowChange = false;
        System.Windows.Controls.TextBox _textBox;
        System.Windows.Controls.TextBox _dragFromTextBox;
        System.Windows.Controls.TextBox _dropToTextBox;
        string _originalString = "This text should not change!";
        #endregion
    }
}
