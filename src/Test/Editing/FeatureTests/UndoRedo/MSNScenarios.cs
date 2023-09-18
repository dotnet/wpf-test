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
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.ComponentModel;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Collections;
    using System.Windows;
    using System.Threading;
    using System.Windows.Threading;
    using Microsoft.Test.Imaging;


    #endregion Namespaces.

    /// <summary>
    /// UndoTypeingTest
    /// </summary>
    public class MSNScenariosTest : CommonTestCase
    {
        /// <summary>TypedString</summary>
        string _typedString = "AbcdeF";

        string _selectedText="bcde";

		TextBox _textBox;

		TextBox _quteTextbox;

        /// <summary>Constructor - UndoTypeingTest </summary>
        public MSNScenariosTest()
        {
            //we want to runing this test in the MSN scenario if it exist. Otherwise we will create our own text box.
            EnterFuction("MSNScenariosTest");
            StartupPage = "MSN.xaml";
            EndFunction();
        }

        /// <summary>Find the TextBox and set the size of the main windows</summary>
        public override void Init()
        {
            EnterFuction("Init");

            _textBox = ElementUtils.FindElement(MainWindow.Content as FrameworkElement, "TextBoxSearch") as TextBox;

            MainWindow.Width = 470;
            MainWindow.Height = 543;
            EndFunction();
        }

        /// <summary> This case tests 1) typing, bold, Cut, Undo, Redo, Paste, Scroll bar </summary>
        public void TextEditing_Scrolling()
        {
            EnterFuction("Basic_TextEditing_Scroll");

            //set the focus in the textbox
            MouseInput.MouseClick(_textBox);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(TypeStringToSearchTextBox));            
        }

        /// <summary>action TypeStringToSearchTextBox </summary>
        void TypeStringToSearchTextBox()
        {
            EnterFuction("TypeStringToSearchTextBox");
            Test.Uis.Utils.KeyboardInput.TypeString(_typedString);
            EndFunction();
			Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifyTypedStringAndPerfromSelection));
		}

        /// <summary>action VerifyTypedStringAndPerfromBold </summary>
        void VerifyTypedStringAndPerfromSelection()
        {
			EnterFuction("VerifyTypedStringAndPerfromSelection");
			Thread.Sleep(1500);
            Verifier.Verify(_textBox.Text == _typedString, CurrentFunction + " - String typed in textbox is wrong!!! expected: " + _typedString + " Actual: " + _textBox.Text);
            _textBox.Select(1, 4);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifySelectionAndPerfromCut));
         }

        /// <summary>action VerifyBoldAndPerfromCut </summary>
		void VerifySelectionAndPerfromCut()
		{
			EnterFuction("VerifySelectionAndPerfromCut");
			Thread.Sleep(1500);

            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_textBox);

            Verifier.Verify(_textBox.Text == _typedString, CurrentFunction + " - String typed in textbox is wrong!!! expected: " + _typedString + " Actual: " + _textBox.Text);
            Verifier.Verify(textSelection.Text == "bcde", CurrentFunction + " - failed: selection wrong text!!! Expected: bcde" + " Acutal: " + textSelection.Text);
            RoutedCommand CutCommand = ApplicationCommands.Cut;
            CutCommand.Execute(null, (UIElement)_textBox);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifyCutAndPerformUndo));
        }

        /// <summary>action VerifyCutAndPerformUndo </summary>
        void VerifyCutAndPerformUndo()
        {
            EnterFuction("VerifyCutAndPerformUndo");
            Thread.Sleep(1500);
            Verifier.Verify(_textBox.Text == "AF", CurrentFunction + " - String typed in textbox is wrong!!! expected: AF" + " Actual: " + _textBox.Text);
            RoutedCommand UndoCommand = ApplicationCommands.Undo;
            UndoCommand.Execute(null, (UIElement)_textBox);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifyUndoAndPerformRedo));
        }

        /// <summary>action VerifyUndoAndPerformRedo </summary>
        void VerifyUndoAndPerformRedo()
        {
            EnterFuction("VerifyUndoAndPerformRedo");
            Thread.Sleep(1500);
            Verifier.Verify(_textBox.Text == _typedString, CurrentFunction + " - String typed in textbox is wrong!!! expected: " + _typedString + " Actual: " + _textBox.Text);
            RoutedCommand RedoCommand = ApplicationCommands.Redo;
            RedoCommand.Execute(null, (UIElement)_textBox);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(verifyRedoAndScrollVertically));
        }

        /// <summary>action verifyRedoAndScrollVertically </summary>
        void verifyRedoAndScrollVertically()
        {
            EnterFuction("verifyRedoAndScrollVertically");
            Thread.Sleep(1500);
            Verifier.Verify(_textBox.Text == "AF", CurrentFunction + " - String typed in textbox is wrong!!! expected: AF" + " Actual: " + _textBox.Text);
            ScrollViewer sv = ElementUtils.FindElement(MainWindow.Content as FrameworkElement, "scrollviewer") as ScrollViewer;
            Verifier.Verify(null != sv, CurrentFunction + " - Can't find scrollviewer!!!");

            Rect rc = ElementUtils.GetScreenRelativeRect(sv as UIElement);

            MouseInput.MouseClick((int)(rc.BottomRight.X) - 8, (int)( rc.BottomRight.Y)- 25);
            MouseInput.MouseClick((int)(rc.BottomRight.X) - 8, (int)(rc.BottomRight.Y) - 25);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(ScrollHorizontally));
        }

        /// <summary>action ScrollHorizontally </summary>
        void ScrollHorizontally()
        {
            EnterFuction("ScrollHorizontally");
            Thread.Sleep(1500);

            ScrollViewer sv = ElementUtils.FindElement(MainWindow.Content as FrameworkElement, "scrollviewer") as ScrollViewer;
            Rect rc = ElementUtils.GetScreenRelativeRect(sv as UIElement);

            MouseInput.MouseClick((int)(rc.BottomRight.X) - 25, (int)(rc.BottomRight.Y) - 8);
            MouseInput.MouseClick((int)(rc.BottomRight.X) - 25, (int)(rc.BottomRight.Y) - 8);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(SetCaretToQuoteTextBox));
        }

        /// <summary>action SetCaretToQuoteTextBox </summary>
        void SetCaretToQuoteTextBox()
        {
            EnterFuction("SetCaretToQuoteTextBox");
            Thread.Sleep(1500);
            _quteTextbox = ElementUtils.FindElement(MainWindow.Content as FrameworkElement, "QuteTextBox") as TextBox;
            _quteTextbox.Text = _typedString;
            Verifier.Verify(_quteTextbox != null, CurrentFunction + " - Failed to find the Quote TextBox!!!");
            Verifier.Verify(_quteTextbox.Text == _typedString, CurrentFunction + " - we expect text:'" + _typedString + "'!!! Acutal text:" + _quteTextbox.Text);
            MouseInput.MouseClick(_quteTextbox);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(SelectAllTextInQuoteTextBox));
        }

        /// <summary>action SelectAllTextInQuoteTextBox </summary>
        void SelectAllTextInQuoteTextBox()
        {
            EnterFuction("SelectAllTextInQuoteTextBox");
            Thread.Sleep(1500);
            _quteTextbox.SelectAll();
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerfromPasteToQuteTextBox));
        }

        /// <summary>action PerfromPasteToQuteTextBox </summary>
        void PerfromPasteToQuteTextBox()
        {
            EnterFuction("PerfromPasteToQuteTextBox");
            Thread.Sleep(1500);
            RoutedCommand PasteCommand = ApplicationCommands.Paste;
            PasteCommand.Execute(null, (UIElement)_quteTextbox);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifierPastedTextInQuteTextBox));
        }

        /// <summary>action VerifierPastedTextInQuteTextBox </summary>
        void VerifierPastedTextInQuteTextBox()
        {
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_quteTextbox);

            EnterFuction("VerifierPastedTextInQuteTextBox");
            Verifier.Verify(textSelection.Text == null || textSelection.Text == string.Empty, CurrentFunction + " - After paste, we expected no text selected!!! Actual selected text: " + textSelection.Text);
			Thread.Sleep(1500);
			_quteTextbox.SelectAll();
			Verifier.Verify(_quteTextbox.Text == _selectedText, CurrentFunction + " - Wrong text pasted!!! expected : " + _selectedText + " Actual: " + _quteTextbox.Text);

            RoutedCommand UndoCommand = ApplicationCommands.Undo;
            UndoCommand.Execute(null, (UIElement)_quteTextbox);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifierUndoAndPerfromRedo));
        }

        /// <summary>action VerifierUndoAndPerfromRedo </summary>
        void VerifierUndoAndPerfromRedo()
        {
            EnterFuction("VerifierUndoAndPerfromRedo");
            Thread.Sleep(1500);
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_quteTextbox);

            Verifier.Verify(_quteTextbox.Text == _typedString, CurrentFunction + " - we expect text: '" + _typedString + "'!!! Acutal text:" + _quteTextbox.Text);
            Verifier.Verify(textSelection.Text == _typedString, CurrentFunction + " - Selected text should be: '" + _typedString + "'!!! Actual Selected Text: "  + textSelection.Text );
            // Regression_Bug309 is fixed 
            RoutedCommand RedoCommand = ApplicationCommands.Redo;
            RedoCommand.Execute(null, (UIElement)_quteTextbox);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifyRedoResultAndEndTheCase));
            EndFunction();
        }

        /// <summary>action VerifyRedoResultAndEndTheCase </summary>
        void VerifyRedoResultAndEndTheCase()
        {
            EnterFuction("VerifyRedoResultAndEndTheCase");
            Thread.Sleep(1500);
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_quteTextbox);

            Verifier.Verify(_quteTextbox.Text == _selectedText, CurrentFunction + " - Wrong text pasted!!! expected : " + _selectedText + " Actual: " + _quteTextbox.Text);
            Verifier.Verify(textSelection.Text =="bcde", CurrentFunction + " - After paste, we expected no text selected!!! Actual selected text: " + textSelection.Text);
            MyLogger.ReportSuccess();
            EndFunction();
        }        
    }
}