// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Simulate keybaord input under background layout.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 13 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Text/BVT/textom/TextRangeTest.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    using System.Windows;
    using System.Windows.Controls; 
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;    
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;    
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test.Display;    

    #endregion Namespaces.

    /// <summary>
    /// Background Layout for Keyboard input .  
    /// </summary>
    [Test(2, "TextBoxBase", "BackgroundLayoutKeyboardInput", MethodParameters = "/TestCaseType=BackgroundLayoutKeyboardInput", Timeout = 440)]
    [TestOwner("Microsoft"), TestTactics("30"), TestBugs(""), TestWorkItem("4, 5, 6")]
    public class BackgroundLayoutKeyboardInput : ManagedCombinatorialTestCase
    {
        /// <summary>
        /// Override the base class
        /// </summary>
        protected override void  DoRunCombination()
        {
            _queue = new QueueHelper(DispatcherPriority.Background);
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            
            TestElement = _wrapper.Element as FrameworkElement;
            if (_wrapper.Element is TextBox)
            {
                ((TextBox)_wrapper.Element).TextWrapping = TextWrapping.Wrap;
            }
            QueueDelegate(AppendLargeContent);   
        }

        void AppendLargeContent()
        {
            _wrapper.Element.Focus();
           
            QueueDelegate(DoTyping);
        }

        void DoTyping()
        {
            _queue = new QueueHelper(DispatcherPriority.Background);
            _wrapper.Text = BackgroundLayoutData.MegaString;
            if (_keyboardAction == "^v")
            {
                Clipboard.SetData(DataFormats.Text, "xxx");
            }
            Test.Uis.Utils.KeyboardInput.NoDelaySendString(_keyboardAction);
            VerifyLayoutReady();
        }

        void VerifyLayoutReady()
        {    
            //if failed here, we need to increase the size of the doucment.
                Verifier.Verify(!_wrapper.End.HasValidLayout, "Failed: Layout of docuemnt end is valid, you may have a bogus pass!");
            //try to get the layout info at the doucment end. Don't expect a crash here.
            //Note: calling GetCharacterRect will valid the layout. so we don't try to call too early. 
            Rect rec1 = _wrapper.End.GetCharacterRect(LogicalDirection.Forward);

            QueueDelegate(VerifyKeyboardInput);
        }

        void VerifyKeyboardInput()
        {
            string str;
            
            switch (_keyboardAction)
            {
                case "^v":
                case "xxx":
                    //Since the caret is at the beginning of the document, we expect typed content appears at the beginning.
                    Verifier.Verify(_wrapper.Text.Substring(0, 3) == "xxx", "Failed: Did not find [xxx] at the beginning of the document!");
                    break; 
                case "{END}":
                    //{END} send the caret to the end of the first line. 
                    Verifier.Verify(_wrapper.IsCaretAtEndOfLine, "Failed: Caret did not move the the end of the first line!");
                  
                    break;
                case "{PGDN}":
                    //We are not sure how much of the content is laied out, but it should have more than 2 lines.
                    int num = _wrapper.LineNumberOfTextPointer(_wrapper.SelectionInstance.Start);
                    Verifier.Verify(num > 2, "Failed: Page down should move down several lines! Actual line#[" + num + "]"); 
                    break;
                case "{RIGHT}":
                    str = _wrapper.SelectionInstance.Start.GetTextInRun(LogicalDirection.Backward);
                    Verifier.Verify(str == BackgroundLayoutData.MegaString.Substring(0, 1), "Failed: {RIGHT} move caret to wrong location!");
                    break;
                case "^a":
                    Verifier.Verify(_wrapper.SelectionInstance.Text.Length >= BackgroundLayoutData.MegaString.Length, "Failed: Expect selected length[" + BackgroundLayoutData.MegaString.Length + 
                        "], Actual[" +_wrapper.SelectionInstance.Text.Length + "]");
                    break;
                case "{RIGHT}{BACKSPACE}":
                case "{DELETE}":
                    str = _wrapper.Text.Substring(0, 1);
                    Verifier.Verify(str == BackgroundLayoutData.MegaString.Substring(1, 1), "Failed: on deleting the first character!");
                    break; 
                case "+{RIGHT}":
                    //Verify that selection can be made.
                    Verifier.Verify("a" == _wrapper.SelectionInstance.Text, "Failed - Selected text expected[a], Actual[" + _wrapper.SelectionInstance.Text + "]");
                    break; 
                default:
                    break; 
            }
            //if (KeyboardAction == "xxx")
            //{
            //    CaretVerifier caretVerfier = new CaretVerifier(_wrapper.Element as FrameworkElement);
            //    caretVerfier.VerifyCaretRendered(NextCombination, true);
            //}
            //else
            //{
                QueueDelegate(NextCombination);
            //}
        }

 

        /// <summary>
        /// return an arry of key strokes.
        /// </summary>
        public static string[] KeyboardActions
        {
            get
            {
                return  new string[] {
                    "xxx", 
                    "{END}",
                    "^{END}",
                    "{PGDN}",
                    "{RIGHT}",
                    "{Delete}", 
                    "{RIGHT}{BACKSPACE}",
                    "+{RIGHT}", 
                    "^v", 
                    "^a",
                };
            }
        }

        private QueueHelper _queue; 
        private UIElementWrapper _wrapper;
        string _keyboardAction=string.Empty; 
        private TextEditableType _editableType=null;
    }
    internal class BackgroundLayoutData
    {
        private static string s_str = "an an an ";
        private static int s_repeat = 16; 
        /// <summary>
        /// Supply a long string
        /// </summary>
        public static string MegaString
        {
            get
            {
                string str = s_str; 
                for (int i = 0; i < s_repeat; i++)
                {
                    str += str;
                }
                return str;
            }
        }
        public static string RepeatToMegaString(string str, int power)
        {
            s_str = str;
            s_repeat = power;
            return MegaString;
        }
        
    }

    /// <summary>
    /// Test the background layout on a control, we don't expected crash.
    /// Other verification is not that important here.
    /// </summary>
    [Test(2, "TextBoxBase", "BackgroundLayoutControls", MethodParameters = "/TestCaseType=BackgroundLayoutControls", Timeout = 240, Versions = "3.0SP1,3.0SP2")]
    [TestOwner("Microsoft"), TestTactics("31"), TestBugs("385"), TestWorkItem("7, 8")]
    public class BackgroundLayoutControls : ManagedCombinatorialTestCase
    {
        /// <summary>
        /// Start to run combination
        /// </summary>
        protected override void DoRunCombination()
        {
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            
            TestElement = _wrapper.Element as FrameworkElement;
            if (_wrapper.Element is TextBox)
            {
                ((TextBox)_wrapper.Element).TextWrapping = TextWrapping.Wrap;
            }
            QueueDelegate(SetControlStatus);   
        }

        void SetControlStatus()
        {
            _wrapper.Text = BackgroundLayoutData.MegaString;
            
            switch(_setStatus)
            {
                case "Disable":
                    //disable/Enabled should not cause any problem under Background layout.
                    _wrapper.Element.IsEnabled = false;
                    _wrapper.Element.IsEnabled = true;
                    break; 
                case "NotInVisualTree":
                    //remove from tree and re-attached to the tree. 
                    TestElement = null;
                    TestElement = _wrapper.Element as FrameworkElement;
                    break; 
                case "OutOfFocus":
                    //Set focus out.
                    Test.Uis.Utils.KeyboardInput.NoDelaySendString("{TAB}");
                    break;
                case "APIAccess":
                    if (_wrapper.Element is TextBox)
                    {
                        CallTextBoxAPIs();
                    }
                    else
                    {
                        CallRichTextBoxAPIs();
                    }
                    break; 
            }
            NextCombination(); 
        }

        void CallTextBoxAPIs()
        {
            TextBox box = _wrapper.Element as TextBox;
            box.AcceptsReturn = true;
            box.AcceptsReturn = false;
            box.AcceptsTab = true;
            box.AcceptsTab = false;
            box.PageDown();
            box.PageLeft();
            box.PageUp();
            box.Paste();
            box.Select(0, box.Text.Length - 10);
            box.Copy();
            box.Paste();
            box.ScrollToEnd();
            TextBoxSellChecking(box);
            
            //Add more TextBox APIs.
            box.Text = box.Text; 
        }
     
        void CallRichTextBoxAPIs()
        {
            RichTextBox box = _wrapper.Element as RichTextBox;
            BlockCollection blocks = box.Document.Blocks; 
            blocks.InsertAfter(blocks.FirstBlock, new Paragraph(new Run("unckle")));
            blocks.InsertBefore(blocks.FirstBlock, new Paragraph(new Run("unckle")));
            box.SelectAll();
            box.Copy();
            box.Paste();
            box.Undo();
            box.Redo();
            RichTextBoxSpellChecking(box);
        }

        void TextBoxSellChecking(TextBox box)
        {
            if(!(box.GetSpellingError(1)==null))
            {
                Verifier.Verify(box.GetSpellingError(1) == null, "SpellError should be null when spell check is not enabled by default!");
            }
            box.SpellCheck.IsEnabled=true;
            Verifier.Verify(box.GetSpellingError(1)!=null, "spellError should not be null!");
        }

        void RichTextBoxSpellChecking(RichTextBox box)
        {
            TextPointer pointer;
            Paragraph p;
            p = box.Document.Blocks.FirstBlock as Paragraph;
            pointer = ((Run)p.Inlines.FirstInline).ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
            Verifier.Verify(box.GetSpellingError(pointer) == null, "SpellError should be null when spell check is not enabled by default!");
            
            box.SpellCheck.IsEnabled = true;
            Verifier.Verify(box.GetSpellingError(pointer) != null, "SpellError should not be null when spell check is enabled!");
        }

        /// <summary>
        /// get conrol 
        /// </summary>
        public static  string[] ControlStatus
        {
            get
            {
                return new string[]
                {
                    "Disable",
                    "NotInVisualTree", 
                    "OutOfFocus",
                    "APIAccess",
                }; 
            }
        }

        private UIElementWrapper _wrapper;
        private string _setStatus=string.Empty; 
        private TextEditableType _editableType=null;
    }


    /// <summary>
    /// Test the squiggle rendering for backgroun layout.
    /// we enable the spell check after the background layout starts.
    /// We don't expected any crash, event spell and layout both works on background priority. 
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("32"), TestBugs(""), TestWorkItem("8, 6, 9")]
    public class BackgrounLayoutForSpellRendering : ManagedCombinatorialTestCase
    {
        /// <summary>
        /// Start to run combination
        /// </summary>
        protected override void DoRunCombination()
        {
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
           
            _queue = new QueueHelper(DispatcherPriority.Background);

            TestElement = _wrapper.Element as FrameworkElement;
            if (_wrapper.Element is TextBox)
            {
                ((TextBox)_wrapper.Element).TextWrapping = TextWrapping.Wrap;
            }
            ((TextBoxBase)_wrapper.Element).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            
            //note we use the power to be 15 for purpose
            _wrapper.Text = _badWord + BackgroundLayoutData.RepeatToMegaString(" good", 15);
            
            //set background priority queue delegate
            if (_action == ActionsForBackgroundLayout.MouseSelection)
            {
                _queue.QueueDelegate(PerfromSelection);
            }
            else if (_action == ActionsForBackgroundLayout.DragDrop)
            {
                //make a selction
                TextPointer p1 = _wrapper.Start;
                TextPointer p2;
                if (!p1.IsAtInsertionPosition)
                {
                    p1 = p1.GetInsertionPosition(LogicalDirection.Forward);
                }
                p2 = p1.GetPositionAtOffset(_badWord.Length);
                _wrapper.SelectionInstance.Select(p1, p2);
                _queue.QueueDelegate(DoDragDrop);
            }
            else
            {
                _queue.QueueDelegate(CaptureFirstImage);
            }
        }
        void CaptureFirstImage()
        {
            _nosquiggle = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(TestElement);
           
            //reassign the same value, we change the power to 14.
            _wrapper.Text = _badWord + BackgroundLayoutData.RepeatToMegaString(" good", 14) ;
                     
            //the spell enable should be afer the background layout. but we still expected the squiggle to be shown.
            ((TextBoxBase)_wrapper.Element).SpellCheck.IsEnabled = true;
            
            //we need to "Tab" the focus out of the TextBox or RichTextbox so that care is not shown.
            Test.Uis.Utils.KeyboardInput.NoDelaySendString("{Tab}");
            _queue.QueueDelegate(SpellCheckDone);
        }

        void DoDragDrop()
        {
            _wrapper.Element.Focus();

            int startIndex, endIndex;
            TextPointer p = _wrapper.Start;
            if (!p.IsAtInsertionPosition)
            {
                p = p.GetInsertionPosition(LogicalDirection.Forward);
            }
            //we only won't to drag a little
            startIndex = _wrapper.Start.GetOffsetToPosition(p) + 1;
            endIndex = startIndex + _badWord.Length;
            Rect r1 = _wrapper.GetGlobalCharacterRect(startIndex, LogicalDirection.Forward);
            Rect r2 = _wrapper.GetGlobalCharacterRect(endIndex, LogicalDirection.Forward);

            MouseInput.MouseDragInOtherThread(new System.Windows.Point(r1.Left, r1.Top), new System.Windows.Point(r2.Left, r2.Bottom), true, TimeSpan.FromSeconds(0), new SimpleHandler(DragDropDone), Application.Current.Dispatcher);
        }

        void PerfromSelection()
        {
            //we just want to make sure that a selection can be make. 
            int startIndex = 0;
            if (TestElement is RichTextBox)
            {
                TextPointer p = _wrapper.Start;
                if (!p.IsAtInsertionPosition)
                {
                    p = p.GetInsertionPosition(LogicalDirection.Forward);
                }
                startIndex = _wrapper.Start.GetOffsetToPosition(p);
            }
            Rect r1 = _wrapper.GetGlobalCharacterRect(startIndex, LogicalDirection.Forward);
            Rect r2 = _wrapper.GetGlobalCharacterRect(startIndex + _badWord.Length, LogicalDirection.Forward);
            MouseInput.MouseDragInOtherThread(new System.Windows.Point(r1.Left, r1.Top), new System.Windows.Point(r2.Left, r2.Bottom), true, TimeSpan.FromSeconds(0), new SimpleHandler(SelectionDone), Application.Current.Dispatcher);
        }

        void SpellCheckDone()
        {
            _squiggle = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(TestElement);
            Bitmap difference;
            bool b = Microsoft.Test.Imaging.ComparisonOperationUtils.AreBitmapsEqual(_nosquiggle, _squiggle, out difference);
            Verifier.Verify(!b, "Squiggle is not show!");
            QueueDelegate(NextCombination);
        }

        void DragDropDone()
        {
            Log("DragDropFinish");
            TextRange range = new TextRange(_wrapper.Start, _wrapper.SelectionInstance.End);
            Verifier.Verify(_wrapper.SelectionInstance.Text == _badWord, "Selected Text expected[" + _badWord + "], Actual[" + _wrapper.SelectionInstance.Text + "]");
            Verifier.Verify(range.Text.Length == _badWord.Length + 1, "range.text expected[ " + _badWord + "], Actual[" + range.Text + "]");
            _queue.QueueDelegate(NextCombination);
        }

        void SelectionDone()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Text == _badWord, "Selected Text expected[" + _badWord + "], Actual[" + _wrapper.SelectionInstance.Text + "]");
            _queue.QueueDelegate(NextCombination);
        }

        private Bitmap _nosquiggle;
        private Bitmap _squiggle;
        private QueueHelper _queue;
        private string _badWord = "bawrod"; 
        private UIElementWrapper _wrapper;
        private TextEditableType _editableType=null;
        private ActionsForBackgroundLayout _action=0;
    }
    /// <summary>
    /// Action defined for background Layout
    /// </summary>
    public enum ActionsForBackgroundLayout
    {
        /// <summary>
        /// test spell check
        /// </summary>
        SpellCheck,
        /// <summary>
        /// Test mouse selection
        /// </summary>
        MouseSelection,
        /// <summary>
        /// Test drag/drop
        /// </summary>
        DragDrop,
    }

    /// <summary>
    /// Regression Test - Pasting large amounts of text into TextBox and then scrolling causes an infinite scrolling loop
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("29"), TestBugs("386"), TestLastUpdatedOn("Aug 8, 2006")]
    public class RegressionTest_Regression_Bug386 : CombinedTestCase
    {
        TextBox _textBox;

        /// <summary>
        /// start the test run
        /// </summary>
        public override void RunTestCase()
        {
            if (Monitor.Dpi.x == 120)
            {
                EndTest();
            }
            else
            {
                _textBox = new TextBox();

                //the same setting as the repro.xaml for the bug.
                _textBox.Height = 480;
                _textBox.Width = 640;
                MainWindow.Height = 600;
                MainWindow.Width = 800;
                _textBox.AcceptsReturn = true;
                _textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _textBox.TextWrapping = TextWrapping.Wrap;
                MainWindow.Content = _textBox;

                _textBox.Text = BuildContent();
                QueueDelegate(SetFocus);
            }
        }

        void SetFocus()
        {
            _textBox.Focus();
            QueueDelegate(GetTheLastLine);
        }

        void GetTheLastLine()
        {
            int index = _textBox.GetLastVisibleLineIndex();
            Verifier.Verify(index > 20, "Failed - Last visiable line index [" + index.ToString() + "] is less than 20!");
            KeyboardInput.TypeString("^{END}^{HOME}");
            QueueDelegate(ScrollWithScrollBar);
        }

        void ScrollWithScrollBar()
        {
            System.Windows.Point p1  =  ElementUtils.GetScreenRelativePoint(_textBox, new System.Windows.Point(_textBox.Width-8, 23));
            InputMonitorManager.Current.IsEnabled = false;
            MouseInput.MouseDown(p1);

            //expected no infinit loop
            System.Windows.Point p2 = new System.Windows.Point(p1.X, p1.Y + _textBox.Height - 35);
            MouseInput.MouseDragInOtherThread(p1, p2, false, TimeSpan.Zero,
                OnMouseUp, Dispatcher.CurrentDispatcher);
        }

        void OnMouseUp()
        {
            MouseInput.MouseUp();
            QueueDelegate(DoAPIScrolling);
        }

        void DoAPIScrolling()
        {
            //expected no infinit loop
            _textBox.ScrollToHome();
            _textBox.ScrollToEnd();
            _textBox.ScrollToHome();
            _textBox.ScrollToLine(_textBox.LineCount - 1);
            _textBox.SelectAll();
            _textBox.Copy();
            _textBox.Paste();
            QueueDelegate(TestFinished);
        }

        void TestFinished()
        {
            EndTest();
        }

        string BuildContent()
        {
            string returnContent="";
            for (int i = 0; i < 1536; i++)
            {
                returnContent +="asdfsadfsadf sda fsdaf sdaf sd asdf saf sadf sda\r\n";
            }
            return returnContent + "asdfsadfsadf sda fsdaf sdaf sd asdf saf sadf asdfsadfsadf sda fsdaf sdaf sd asdf saf sadf sda";
        }
    }
}