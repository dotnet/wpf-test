// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Test.Uis.TextEditing
{
    #region Namespaces.
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using System.Drawing;
    using Microsoft.Test.Imaging;
    using System.Windows.Controls.Primitives;
    using Microsoft.Test.Display;
    #endregion Namespaces.

    /// <summary>
    /// Regression Regression_Bug196 - Undo units are generated for each character typed.
    /// </summary>
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics("25"), TestBugs("196")]
    public class RegressionTest_Regression_Bug196 : CombinedTestCase
    {
        UIElementWrapper _wrapper;
        /// <summary>
        /// start test case
        /// </summary>
        public override void RunTestCase()
        {
            _wrapper = new UIElementWrapper(new TextBox());
            MainWindow.Content = _wrapper.Element;
            QueueDelegate(SetFocus);
        }
        void SetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(PerfromTyping);
        }
        void PerfromTyping()
        {
            KeyboardInput.TypeString("abcd");
            QueueDelegate(VerifyTyping);
        }
        void VerifyTyping()
        {
            Verifier.Verify(_wrapper.Text.Contains("abcd"), "Wrong text is typed in. Expected[abcd], Actual[" + _wrapper.Text + "]");
            ((TextBox)_wrapper.Element).Undo();
            QueueDelegate(AfterUndo);
        }
        void AfterUndo()
        {
            Verifier.Verify(_wrapper.Text.Length == 0, "Wrong Text in TextBox, Expected[], Actual[" + _wrapper.Text + "]");
            EndTest();
        }
    }

    /// <summary>
    /// regression Regression_Bug197 - Caret always get reset to infront of first line after redo
    /// </summary>
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics("25"), TestBugs("197")]
    public class RegressionTest_Regression_Bug197 : CombinedTestCase
    {
        UIElementWrapper _wrapper;
        /// <summary>
        /// Start test case
        /// </summary>
        public override void RunTestCase()
        {
            _wrapper = new UIElementWrapper(new RichTextBox());
            MainWindow.Content = _wrapper.Element;
            QueueDelegate(SetFocus);
        }

        void SetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(PerformTyping);
        }
        void PerformTyping()
        {
            KeyboardInput.TypeString("a{ENTER}b{ENTER}c{ENTER}d^Z^y");
            QueueDelegate(VerifyResult);
        }

        void VerifyResult()
        {
            string str = _wrapper.SelectionInstance.Start.GetTextInRun(LogicalDirection.Backward);
            Verifier.Verify(str == "d", "Wrong text after Undo. Expected[d], actual[" + str + "]");
            EndTest();
        }
    }

    /// <summary>
    /// Verifies that when copy is done through context menu on selected text
    /// and another window is clicked , the selection is no longer visible.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("697"), TestBugs("671"), TestWorkItem("123"), TestLastUpdatedOn("April 27-2006")]
    public class RegressionTest_Regression_Bug671 : CombinedTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Logger.Current.Log("*********************************************");
            Logger.Current.Log("-----TestCase Last Updated on 04-27-2006-----");
            Logger.Current.Log("*********************************************");
            _stackPanel = new StackPanel();
            _rtb = new RichTextBox();
            _tb = new TextBox();

            _rtb.Height = _tb.Height = 200;
            _rtb.Width = _tb.Width = 200;

            _rtb.Document.Blocks.Clear();
            _rtb.Document.Blocks.Add(new Paragraph(new Run(_str)));
            _tb.Text = _str;

            _stackPanel.Children.Add(_rtb);
            _stackPanel.Children.Add(_tb);

            MainWindow.Width = 400;
            MainWindow.Content = _stackPanel;

            _secWindow = new Window();
            _secWindow.Height = _secWindow.Width = 400;
            _secWindow.Left = 600;
            _secWindow.Top = 0;
            _secWindow.Show();
            _count = 0;
            QueueDelegate(ExecuteTrigger);
        }

        private void ExecuteTrigger()
        {
            MainWindow.BringIntoView();
            _control = (_count == 0) ? (UIElement)_tb : (UIElement)_rtb;
            ((TextBoxBase)_control).SelectAll();
            _control.Focus();
            QueueDelegate(OpenContextMenu);
        }

        private void OpenContextMenu()
        {
            _initial = BitmapCapture.CreateBitmapFromElement(_control);
            Logger.Current.LogImage(_initial, "IIIII");
            KeyboardInput.TypeString("+{F10}");
            QueueDelegate(MoveDownToCopy);
        }

        private void MoveDownToCopy()
        {
            KeyboardInput.TypeString("{DOWN 2}{ENTER}");
            QueueDelegate(ClickOnSecWindow);
        }

        private void ClickOnSecWindow()
        {
            MouseInput.MouseClick(new System.Windows.Point(700*Monitor.Dpi.x/96.0F, 200));
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0,0,1), new SimpleHandler(GetFinalImage),null);
        }

        private void GetFinalImage()
        {
            _final = BitmapCapture.CreateBitmapFromElement(_control);
            ComparisonCriteria _criteria = new ComparisonCriteria();
            _criteria.MaxColorDistance = 0.01f;
            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_initial, _final,_criteria,false ) == false)
            {
                if (_count == 0)
                {
                    _count++;
                    QueueDelegate(ExecuteTrigger);
                }
                else
                {
                    EndTest();
                }
            }
            else
            {
                Logger.Current.Log("Images are supposed to be different --- Initial --- Final");
                Logger.Current.LogImage(_initial, "Initial");
                Logger.Current.LogImage(_final, "Final");
                Verifier.Verify(false, "Test FAILED!");
                EndTest();
            }
        }


        #region data.

        private RichTextBox _rtb;
        private TextBox _tb;
        private StackPanel _stackPanel;
        private UIElement _control;

        private string _str = "HEY YOU!!";
        private Bitmap _initial, _final;
        private int _count = 0;

        Window _secWindow;

        #endregion data.
    }
    
    /// <summary>
    /// Regression Test for Regression_Bug198
    /// </summary>
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics("25"), TestBugs("198")]
    public class RegressionTest_Regression_Bug198 : CombinedTestCase
    {
        UIElementWrapper _wrapper;
        /// <summary>
        /// Start test case
        /// </summary>
        public override void RunTestCase()
        {
            RichTextBox box = new RichTextBox();
            box.Height = 500;
            box.Width = 500;
            _wrapper = new UIElementWrapper(box);
            MainWindow.Content = _wrapper.Element;
            _wrapper.XamlText = "<Paragraph>This is a<LineBreak />real test</Paragraph>";
            QueueDelegate(SetFocus);
        }

        void SetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(PerformTyping);
        }

        void PerformTyping()
        {
            Rect rect = ElementUtils.GetScreenRelativeRect(_wrapper.Element);
            MouseInput.MouseClick((int)rect.X + 490, (int)rect.Y + 10);
            QueueDelegate(VerifyText);
        }

        void VerifyText()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Start.GetOffsetToPosition(_wrapper.SelectionInstance.End) == 0, "Failed - No text should be selected!");

            TextRange rangeLeft = new TextRange(_wrapper.Start, _wrapper.SelectionInstance.Start);
            TextRange rangeRight = new TextRange( _wrapper.SelectionInstance.End, _wrapper.End);

            Verifier.Verify(rangeLeft.Text == "This is a", "Failed. Expected[This is a], Actual[" + rangeLeft.Text + "]");
            Verifier.Verify(rangeRight.Text == "\r\nreal test\r\n", "Failed. Expected[This is a\r\n], Actual[" + rangeRight.Text + "]");
            KeyboardInput.TypeString("{RIGHT}");
            QueueDelegate(VerifyTextAferKeyAction);
        }
        
        void VerifyTextAferKeyAction()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Start.GetOffsetToPosition(_wrapper.SelectionInstance.End) == 0, "Failed - No text should be selected!");

            TextRange rangeLeft = new TextRange(_wrapper.Start, _wrapper.SelectionInstance.Start);
            TextRange rangeRight = new TextRange(_wrapper.SelectionInstance.End, _wrapper.End);

            Verifier.Verify(rangeLeft.Text == "This is a\r\n", "Failed. Expected[This is a], Actual[" + rangeLeft.Text + "]");
            Verifier.Verify(rangeRight.Text == "real test\r\n", "Failed. Expected[This is a\r\n], Actual[" + rangeRight.Text + "]");
            KeyboardInput.TypeString("{RIGHT}");
            EndTest();
        }
    }
    
    /// <summary>
    /// Regression Test for Regression_Bug199
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("25"), TestBugs("199, 197, 837, 838, 839"), TestLastUpdatedOn("Aug 14, 2006")]
    public class RegressionTest_Regression_Bug199 : CombinedTestCase
    {
        RichTextBox _richTextBox;
        int _count; 
        /// <summary>
        /// Start Test 
        /// </summary>
        public override void RunTestCase()
        {
            _richTextBox = new RichTextBox();
            MainWindow.Content = _richTextBox;
            _richTextBox.Document.Blocks.Clear();
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run());
            para.Inlines.Add(new Run());
            para.Inlines.Add(new Run());
            _richTextBox.Document.Blocks.Add(para);
            QueueDelegate(SetFocus);
        }
        void SetFocus()
        {
            _richTextBox.Focus();
            QueueDelegate(PerfromKeyboardActions);
        }
        void PerfromKeyboardActions()
        {
            KeyboardInput.TypeString("{HOME}{END}");
            QueueDelegate(RegressionTest_Regression_Bug197);
        }

        void RegressionTest_Regression_Bug197()
        {
            KeyboardInput.TypeString("^Aa{ENTER}b{ENTER}c{ENTER}^zd+{LEFT}");
            QueueDelegate(End_Regression_Bug197);
        }

        void End_Regression_Bug197()
        {
            Verifier.Verify(_richTextBox.Selection.Text == "d", "Failed - Selected Text should be[d], Actual[" + _richTextBox.Selection.Text + "]");
            QueueDelegate(RegressionTest_Regression_Bug837);
        }

        void RegressionTest_Regression_Bug837()
        {
            KeyboardInput.TypeString("^A{DELETE}aBOLDe{LEFT}+{LEFT 4}^B{END}{left 2}+{Left 2}");
            QueueDelegate(Delete_Regression_Bug837); 
        }

        void Delete_Regression_Bug837()
        {
            Paragraph para = _richTextBox.Document.Blocks.FirstBlock as Paragraph;
            _count = para.Inlines.Count;
            KeyboardInput.TypeString("{DELETE}");
            QueueDelegate(End_Regression_Bug837); 
        }

        void End_Regression_Bug837()
        {
            Paragraph para = _richTextBox.Document.Blocks.FirstBlock as Paragraph;
            Verifier.Verify(_count == para.Inlines.Count, "Failed: expected[" +_count + "] runs, actual[" + para.Inlines.Count + "]runs");
            QueueDelegate(RegressionTest_Regression_Bug838);
        }

        void RegressionTest_Regression_Bug838()
        {
            _richTextBox = new RichTextBox();
            MainWindow.Content = _richTextBox; 
            QueueDelegate(SetFocus_Regression_Bug838);
        }

        void SetFocus_Regression_Bug838()
        {
            _richTextBox.Focus();
            QueueDelegate(TypeText_Regression_Bug838);
        }

        void TypeText_Regression_Bug838()
        {
            KeyboardInput.TypeString("abcdef+{LEFT 3}{DELETE}^z+{LEFT}");
            QueueDelegate(DoRedo_Regression_Bug838);
        }
        void DoRedo_Regression_Bug838()
        {
            _richTextBox.Redo();

            Paragraph para = _richTextBox.Document.Blocks.FirstBlock as Paragraph; 
            Run run= para.Inlines.FirstInline as Run;
            Verifier.Verify(run.Text == "abc", "Failed - Expected Text[abc], Actual[" + run.Text + "]");
            QueueDelegate(RegressionTest_Regression_Bug839);
        }

        void RegressionTest_Regression_Bug839()
        {
            _richTextBox.Document.Blocks.Clear();
            KeyboardInput.TypeString("Text+{ENTER}m{LEFT 2}");
            QueueDelegate(RegressionTest_Regression_Bug839_Delete);
        }

        void RegressionTest_Regression_Bug839_Delete()
        {
            KeyboardInput.TypeString("{DELETE}");
            QueueDelegate(RegressionTest_Regression_Bug839_End);
        }

        void RegressionTest_Regression_Bug839_End()
        {
            Paragraph para = _richTextBox.Document.Blocks.FirstBlock as Paragraph;
            Run run = para.Inlines.FirstInline as Run;
            
            //check text.
            Verifier.Verify(run.Text == "Textm", "Failed: expected Text[Textm], Actual Text[" + run.Text + "]");

            //Check runs
            Verifier.Verify(para.Inlines.Count == 1, "Failed: expected[1] run, Actual[" + para.Inlines.Count + "] run");

            _count = _richTextBox.Document.Blocks.Count;
            //check Paragraph
            Verifier.Verify(_count == 1, "Failed: Expected[1] Paragraph, Actual[ " + _count + "] Paragraph");

            QueueDelegate(EndTest);
        }
    }
}
