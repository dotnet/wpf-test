// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/UndoRedo/UndoFormating.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Markup;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// UndoTypeingTest
    /// </summary>
    [Test(0, "UndoRedo", "UndoFormatingTest", MethodParameters = "/TestCaseType=UndoFormatingTest /Case=UndoFormating /Format=bold", Timeout = 120)]
    [TestOwner("Microsoft"), TestTitle("UndoFormatingTest"), TestTactics("366"), TestLastUpdatedOn("Jan 25, 2007")]
    public class UndoFormatingTest : RichEditingBase
    {
        /// <summary>TypedString</summary>
        string _typedString = "abc def ghi jkl mno pqr stu vwx yz1 234 567 890";

        /// <summary>Format</summary>
        string _format="bold";

        /// <summary>FormatVerifyString</summary>
        string _formatVerifyString;

        /// <summary>SelectedText</summary>
        string _selectedText;

        TextSelection _tSelection;

        #region case - UndoFormating

        /// <summary>UndoHotKey</summary>
        [TestCase(LocalCaseStatus.Ready, "Test For Undo HotKey")]
        public void UndoFormating()
        {
            EnterFuction("UndoFormating");
            _typedString = "abcde";

            int strLength = _typedString.Length;
            _tSelection = TextControlWraper.SelectionInstance;

            //set the focus in the textpanel
            MouseInput.MouseClick(TextControlWraper.Element);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(TypeTextIntoRichTextBox));
            EndFunction();
        }

        void TypeTextIntoRichTextBox()
        {
            // Type and make selection.
            KeyboardInput.TypeString(_typedString + "{Left}+{Left 3}");
            QueueHelper.Current.QueueDelegate(PerfromFormating);
        }

        void PerfromFormating()
        {
            EnterFuction("PerfromFormating");

            _selectedText = "bcd";
            switch (_format)
            {
                case "bold":
                    RoutedCommand BoldCommand = EditingCommands.ToggleBold;
                    BoldCommand.Execute(null, (UIElement)TextControlWraper.Element);
                    _formatVerifyString = "FontWeight=\"Bold\"";
					_format = "italic";
					break;
                case "italic":
                    RoutedCommand ItalicCommand = EditingCommands.ToggleItalic;
                    ItalicCommand.Execute(null, (UIElement)TextControlWraper.Element);
                    _formatVerifyString = "Italic";
					_format = string.Empty;
					break;

                default:
                    Verifier.Verify(false, CurrentFunction +  " - Is format(" + _format + ") supported yet???");
                    break;
            }

            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(AfterFormatingCommandsIssued));
            EndFunction();
        }

        /// <summary>AfterFormatingCommandsIssued</summary>
        void AfterFormatingCommandsIssued()
        {
            EnterFuction("AfterFormatingCommandsIssued");
            verifyResults(true);
			Sleep();

			//perfrom First Undo
            Test.Uis.Utils.KeyboardInput.TypeString("^z");
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(AfterUndoAction));
        }

        void AfterUndoAction()
        {
            EnterFuction("AfterUndoAction");
            verifyResults(false);
			Sleep();

			//perfrom Second Undo
            KeyboardInput.TypeString("^y");
            QueueHelper.Current.QueueDelegate(AfterRedoAction);
            EndFunction();
        }

        void AfterRedoAction()
        {
            EnterFuction("AfterRedoAction");
            verifyResults(true);
			Sleep();
			if (_format != string.Empty)
			{
				QueueHelper.Current.QueueDelegate(UndoFormating);
			}
			else
			{
				QueueHelper.Current.QueueDelegate(EndTest);
			}
			EndFunction();
        }

        #endregion

        void verifyResults(bool Formated)
        {
            EnterFuction("verifyResults");

            string TempString = XamlUtils.TextRange_GetXml(_tSelection);

            if (Formated)
                Verifier.Verify(TempString.Contains(_formatVerifyString), CurrentFunction + " -" +  _format + " RoutedCommand failed!!!");
            else
                Verifier.Verify(!TempString.Contains(_formatVerifyString), CurrentFunction + " - " + _format + " RoutedCommand failed!!!");

            Verifier.Verify(_tSelection.Text == _selectedText, CurrentFunction + " - Selected text is wrong!" + "expected: bcd" + " actual: " + _tSelection.Text);
            EndFunction();
        }
    }

    /// <summary>
    /// Test that paragraph elements are handled correctly in undo/redo
    /// for the following operations: cut, copy, paste, split, merge.
    /// </summary>
    /// <remarks>
    /// The following execution modes are expected:
    /// Pri-1: EditingTest.exe /TestCaseType=ParagraphUndoRedo /Pri=1 (~8s)
    /// Pri-2: EditingTest.exe /TestCaseType=ParagraphUndoRedo /Pri=2 (~11s)
    /// </remarks>
    [TestOwner("Microsoft"), TestWorkItem("365"), TestTactics("123")]
    public class ParagraphUndoRedo: ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads a combination and determines whether it should run.</summary>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);
            if (result)
            {
                result = !this._editableType.IsPassword;
            }

            return result;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            FrameworkElement control;   // Control being tested.

            // Prepare the control to be tested.
            control = _editableType.CreateInstance();
            control.SetValue(TextBoxBase.AcceptsReturnProperty, true);

            _wrapper = new UIElementWrapper(control);
            _undoModel = new TextUndoModel(_wrapper);
            _undoModel.CaptureUndo();

            TestElement = control;
            QueueDelegate(StartInteraction);
        }

        private void StartInteraction()
        {
            MouseInput.MouseClick(_wrapper.Element);
            KeyboardInput.TypeString("----");
            QueueDelegate(AfterTyping);
        }

        private void AfterTyping()
        {
            _undoModel.CaptureUndo();
            KeyboardInput.TypeString("{ENTER}");
            QueueDelegate(AfterParagraphCreation);
        }

        private void AfterParagraphCreation()
        {
            _undoModel.CaptureUndo();
            KeyboardInput.TypeString("second paragraph");
            QueueDelegate(AfterOtherTyping);
        }

        private void AfterOtherTyping()
        {
            _undoModel.CaptureUndo();

            // Test a cut operation.
            KeyboardInput.TypeString("^a^x");
            QueueDelegate(AfterCut);
        }

        private void AfterCut()
        {
            _undoModel.CaptureUndo();
            _undoModel.VerifyUndoOperations(3, RestoreFromCut);
        }

        private void RestoreFromCut()
        {
            KeyboardInput.TypeString("^z");
            QueueDelegate(PerformCopy);
        }

        private void PerformCopy()
        {
            // Account for the last cut undone.
            _undoModel.PerformUndo();

            KeyboardInput.TypeString("^c");
            QueueDelegate(AfterCopy);
        }

        private void AfterCopy()
        {
            // Note that a copy operation should *not* be generated.
            _undoModel.VerifyUndoOperations(3, PerformSplit);
        }

        private void PerformSplit()
        {
            KeyboardInput.TypeString("^{HOME}{RIGHT}{RIGHT}{ENTER}");
            QueueDelegate(AfterSplit);
        }

        private void AfterSplit()
        {
            _undoModel.CaptureUndo();
            _undoModel.VerifyUndoOperations(3, PerformMerge);
        }

        private void PerformMerge()
        {
            KeyboardInput.TypeString("^{HOME}+{END}+{RIGHT}{DEL}");
            QueueDelegate(AfterMerge);
        }

        private void AfterMerge()
        {
            _undoModel.CaptureUndo();
            _undoModel.VerifyUndoOperations(3, PerformEmptySplit);
        }

        private void PerformEmptySplit()
        {
            KeyboardInput.TypeString("^{END}{ENTER}");
            QueueDelegate(AfterEmptySplit1);
        }

        private void AfterEmptySplit1()
        {
            _undoModel.CaptureUndo();
            KeyboardInput.TypeString("{ENTER}");
            QueueDelegate(AfterEmptySplit2);
        }

        private void AfterEmptySplit2()
        {
            _undoModel.CaptureUndo();
            _undoModel.VerifyUndoOperations(3, NextCombination);
        }

        #endregion Main flow.

        #region Private data.

        /// <summary>EditableType of control tested.</summary>
        private TextEditableType _editableType=null;

        /// <summary>Model of undo/redo stacks.</summary>
        private TextUndoModel _undoModel;

        /// <summary>Wrapper for control being tested.</summary>
        private UIElementWrapper _wrapper;

        #endregion Private data.
    }

    /// <summary>
    /// Regression Test for Regression_Bug307 - Undoing a change in FlowDirection whose property is data binded, throws an exception
    /// UndoStack get cleared, the side effect will be that that Undo action does not work. and No exception two.
    /// </summary>
    public class RegressionTest_Regression_Bug307 : CombinedTestCase
    {
        string _xaml = "<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>"
                    +"<TextBox Name='FD' Width='100' Background='LightGray'>LeftToRight</TextBox>"
                    + "<TextBox Height='100' Width='300' AcceptsReturn='True' VerticalScrollBarVisibility='Auto' "
                    + "FlowDirection='{Binding Path=Text, ElementName=FD, Mode=TwoWay}'>Change the FlowDirection and then Control+Z"
                    + "</TextBox></StackPanel>";
        TextBox _box1, _box2;
        string _fd; 
       
        /// <summary>
        /// Start to run the test.
        /// </summary>
        public override void RunTestCase()
        {
            StackPanel panel = (StackPanel)Test.Uis.Utils.XamlUtils.ParseToObject(_xaml);
            _box1 = panel.Children[0] as TextBox;
            _box2 = panel.Children[1] as TextBox;

            //make a change for the flowDirection that is bounded to the box2.
            _box1.Text = "RightToLeft";
            _fd = _box1.Text; 
            MainWindow.Content = panel;
            QueueDelegate(PerfromUndo);
        }
        void PerfromUndo()
        {
            //we don't expected Crash on perfroming Undo.
            _box1.Undo();
            _box2.Undo();
            _box1.Focus();
            QueueDelegate(KeyUndoBox1);
        }

        void KeyUndoBox1()
        {
            //we don't expected Crash on perfroming Undo.
            KeyboardInput.TypeString("^z");
            _box2.Focus();
            QueueDelegate(KeyUndoBox2);
        }

        void KeyUndoBox2()
        {
            Log("Expected FlowDirection[" + _fd + "]");
            Log("Actual FlowDirection[" + _box2.FlowDirection.ToString() + "]");
            Verifier.Verify(_fd == _box1.Text, "Failed - the content in the First TextBox is changed. Expected[" + _fd + "Actual[" + _box1.Text + "]");
            Verifier.Verify(_fd == _box2.FlowDirection.ToString(), "Failed: Data binding won't match.");

            //we don't expected Crash on perfroming Undo.
            KeyboardInput.TypeString("^z");
            QueueDelegate(Done);
        }

        void Done()
        {
            Log("Expected FlowDirection[" + _fd + "]");
            Log("Actual FlowDirection[" + _box2.FlowDirection.ToString() + "]");
            Verifier.Verify(_fd == _box1.Text, "Failed - the content in the First TextBox is changed. Expected[" + _fd + "Actual[" + _box1.Text + "]");
            Verifier.Verify(_fd == _box2.FlowDirection.ToString(), "Failed: Data binding won't match.");
            EndTest();
        }

    }
}
