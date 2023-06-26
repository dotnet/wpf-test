// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections;
using System.Drawing;
using DrawingPoint = System.Drawing.Point;
using System.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Point = System.Windows.Point;
using System.Windows.Threading;

using Microsoft.Test.Imaging;
using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.TextEditing;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Test.Uis.TextEditing
{
    /// <summary> TextUndoRegressionTest is a class that holds the regression test cases for textUndo.</summary>
    [TestOwner("Microsoft"), TestBugs("135, 136, 451, 137, 138, 63"), TestTactics("463")]
    [Test(1, "UndoRedo", "TextRegressionTest", MethodParameters = "/TestCaseType=TextRegressionTest /Case=RunAllCases /Priority=1")]
    public class TextRegressionTest : CommonTestCase
    {
        System.Windows.Controls.TextBox _textBox;
        UIElement _originalContent;

        Test.Uis.Wrappers.UIElementWrapper _textboxWraper;

        /// <summary>
        /// TextRegressionTest
        /// </summary>
        public TextRegressionTest()
        {
            base.StartupPage = "RegressionInTextBox.xaml";
        }

        /// <summary> Init</summary>
        public override void Init()
        {
            _textBox = ElementUtils.FindElement(MainWindow.Content as FrameworkElement, "textbox") as TextBox;
            Test.Uis.Utils.MouseInput.MouseClick(_textBox);
            _originalContent = (UIElement)MainWindow.Content;
            _textboxWraper = new Test.Uis.Wrappers.UIElementWrapper(_textBox);
        }

        #region case Regression_Bug134
        /// <summary>Text Box can handle suragate pairs</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1,  "Regression_Bug134")]
        public void Regression_Bug134()
        {
            EnterFuction("Regression_Bug134");
            _textBox.Text = "\uD8D2\uFDE3";
            QueueDelegate(EndTest);
        }
        #endregion 

        #region case - Regression_Bug135
        /// <summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug135")]
        public void Regression_Bug135()
        {
            EnterFuction("Regression_Bug135");
            MyLogger.Log("This scenario for this bug is invalid, We can get the Text control from TextBox to enable and disable the editor. Meanwhile, textEditor is going to be internal.");
            QueueDelegate(EndTest);
        }
        #endregion 
        
        #region case - Regression_Bug136
        /// <summary>
       [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug136")]
        public void Regression_Bug136()
        {
            EnterFuction("SimpleTyping");
            _textBox.Text="";

            //set the focus in the textbox
            MouseInput.MouseClick(_textBox);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(BeginTyping));
        }

        void BeginTyping()
        {
            EnterFuction("BeginTyping");
            KeyboardInput.TypeString("abc");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Select_character_c));
        }

        void Select_character_c()
        {
            EnterFuction("Select_character_c");
            Sleep();
            Verifier.Verify("abc" == _textBox.Text, "TextBox should contain abc!!! actual: " + _textBox.Text);
            _textBox.Select(2, 1);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerfromFirstCut));
        }

        void PerfromFirstCut()
        {
            EnterFuction("PerfromFirstCut");
            Sleep();
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_textBox);
            Verifier.Verify("abc" == _textBox.Text, "TextBox should contain abc!!! actual: " + _textBox.Text);
            Verifier.Verify(textSelection.Text == "c", "Selected text is c!!! actual: " + textSelection.Text);
            KeyboardInput.TypeString("^x");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Select_character_a));
        }

        void Select_character_a()
        {
            EnterFuction("Select_character_a");
            Sleep();
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_textBox);
            Verifier.Verify("ab" == _textBox.Text, "TextBox should contain ab!!! actual: " + _textBox.Text);
            Verifier.Verify(textSelection.Text == "", "No text should be selected!!! actual: " + textSelection.Text);
            _textBox.Select(0, 1);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerfromSecondCut));
        }

        void PerfromSecondCut()
        {
            EnterFuction("PerfromSecondCut");
            Sleep();
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_textBox);
            Verifier.Verify("ab" == _textBox.Text, "TextBox should contain ab!!! actual: " + _textBox.Text);
            Verifier.Verify(textSelection.Text == "a", "selected text should be a!!! actual: " + textSelection.Text);
            KeyboardInput.TypeString("^x");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerfromFirstUndo));
        }

        void PerfromFirstUndo()
        {
            EnterFuction("PerfromFirstUndo");
            Sleep();
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_textBox);
            Verifier.Verify("b" == _textBox.Text, "TextBox should contain b!!! actual: " + _textBox.Text);
            Verifier.Verify(textSelection.Text == "", "No text should be selected!!! actual: " + textSelection.Text);
            KeyboardInput.TypeString("^z");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerfromSecondUndo));
        }

        void PerfromSecondUndo()
        {
            EnterFuction("PerfromSecondUndo");
            Sleep();
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_textBox);
            Verifier.Verify("ab" == _textBox.Text, "TextBox should contain b!!! actual: " + _textBox.Text);
            Verifier.Verify(textSelection.Text == "a", "Selected text should be a!!! actual: " + textSelection.Text);
            KeyboardInput.TypeString("^z");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(LogResult));
        }

        void LogResult()
        {
            EnterFuction("LogResult");
            Sleep();
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_textBox);
            Verifier.Verify("abc" == _textBox.Text, "TextBox should contain b!!! actual: " + _textBox.Text);
            Verifier.Verify(textSelection.Text == "c", "Selected text should be c!!! actual: " + textSelection.Text);
            QueueDelegate(EndTest);
        }
        #endregion
        #region case - Regression_Bug137
        /// <summary>Regression_Bug137</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1,  "Regression_Bug137")]
        public void Regression_Bug137()
        {
            EnterFunction("Regression_Bug137");
            MyLogger.Log("This regression is not needed anymore sincd AllowDrag is not there anymore");
            QueueDelegate(EndTest);
        }
        #endregion

        #region case - Regression_Bug138
        /// <summary>Regression_Bug138 </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1,  "Regression_Bug138")]
        public void Regression_Bug138()
        {
            EnterFuction("Regression_Bug138_CaseStart");
            MyLogger.Log("This regression is no longer valid due to textBox feature change, new rich editing is allowed in TextBox");
            EndFunction();
            QueueDelegate(EndTest);
        }
       
        #endregion

        #region case - Regression_Bug63
        /// <summary>Regression_Bug63 </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug63")]
        public void Regression_Bug63()
        {
            EnterFuction("Regression_Bug63");
            _textBox.AcceptsReturn = true;

            QueueDelegate(Regression_Bug63_DoAction);
        }

        void Regression_Bug63_DoAction()
        {
            KeyboardInput.TypeString("ab{enter}efgjunck");
            QueueDelegate(Regression_Bug63_Done);
        }

        void Regression_Bug63_Done()
        {
            Test.Uis.Wrappers.UIElementWrapper tw = new Test.Uis.Wrappers.UIElementWrapper(_textBox);
            Rect rec1 = tw.GetGlobalCharacterRect(1);
            Rect rec2 = tw.GetGlobalCharacterRect(10);
            if (rec1.Y + 2 > rec2.Y)
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: it seems only one line in the text!!!");
            }
            MainWindow.Content = _originalContent;
            QueueDelegate(EndTest);
        }
        #endregion
    }
        
    /// <summary>RichTextRegressionTest</summary>
    [TestOwner("Microsoft"), TestBugs("144, 147, 146, 145, 160, 148, 163, 162, 161, 159, 443, 141, 150, 149, 445, 151, 153, 142, 154, 143, 155, 156, 157, 158"), TestTactics("467")]
    [Test(1, "RichTextBox", "RichTextRegressionTest", MethodParameters = "/TestCaseType=RichTextRegressionTest /Priority=1")]
    public class RichTextRegressionTest : RichEditingBase
    {
        new void StartCasesRunning()
        {
            int pri = ConfigurationSettings.Current.GetArgumentAsInt("Priority");
            int index = 0;
            while (index < TestDataArayList.Count)
            {
                if (((RichEditingData)TestDataArayList[index]).Priority != pri)
                {
                    TestDataArayList.RemoveAt(index);
                }
                else
                    index++;
            }
            if (TestDataArayList.Count > 0)
            {
                SetInitValue(((RichEditingData)TestDataArayList[0]).InitialXaml);
                QueueDelegate(RichEditingDataKeyBoardExecution);
            }
            else
            {
                QueueDelegate(EndTest);
            }
        }

        /// <summary>cases that contains keyboard action only </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug443")]
        public void ActionCases()
        {
            if (TestDataArayList == null)
                TestDataArayList = new System.Collections.ArrayList();
            TestDataArayList.Clear();
            //regression test for Regression_Bug140
            TestDataArayList.Add(new RichEditingData("regression test for Regression_Bug140", "", "abc{enter}d{backspace 3}^a", "ab\r\n", "ab\r\n", 1, 1, true, 1));

            //regression test for Regression_Bug141
            TestDataArayList.Add(new RichEditingData("regression test for Regression_Bug141", "", "abc{backspace}^z{left}{Delete}", "ab\r\n", "", 0, 1, true, 1));

            //regression case - Regression_Bug142_CaseStart - TextEditorInstance.OnPageUpDownKey() pass a null value for TextPointer to TextView.GetRectFromTextPointer()
            TestDataArayList.Add(new RichEditingData("regression case for Regression_Bug142", "", "adasf{PGUP}", "adasf\r\n", "", 0, 1, true, 1));

            //Regression case - Regression_Bug143 - calling into TextParagraphResult with out-of-range position throws an exception
            TestDataArayList.Add(new RichEditingData("Regression case for Regression_Bug143", "<Paragraph>blah</Paragraph>", "{HOME}{ENTER}{UP}+{END}", "\r\nblah\r\n", "", 0, 2, true, 1));

            //Regression test for Regression_Bug150
            //Now Ctrl-m will delete all the selected text
            //TestDataArayList.Add(new RichEditingData("Regression test for Regression_Bug150", "", "a{enter}bcd+{left 2}^m^+m", "a\r\nbcd\r\n", "cd\r\n", 1, 2, false, 1));

            //Regression test for Regression_Bug445
            //this case may need to be fixed.
            //TestDataArayList.Add(new RichEditingData("Regression test for Regression_Bug445", "<Paragraph>blah</Paragraph><Paragraph>another paragraph</Paragraph>", "{LEFT}^a text^a", " text\r\n", " text\r\n", 1, 1, true, 1));

            QueueDelegate(EndTest);
          //  StartCasesRunning();
        }

        #region Regresion test - Regression_Bug144
        /// <summary>Regression test for Regression_Bug144: Underline in RichTextBox is clipped by the size of TextHighlight.</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug144")]
        public void Regression_Bug144()
        {
            EnterFunction("Regression_Bug144");
            MouseInput.MouseClick(TextControlWraper.Element);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(Regression_Bug144_SetFocus);
            EndFunction();
        }

        void Regression_Bug144_SetFocus()
        {
            KeyboardInput.TypeString("abc{ENTER}abc{ENTER}abc{ENTER}abc{ENTER}abc{ENTER}abc{ENTER}^a");
            QueueDelegate(Regression_Bug144_TakeFirstImage);
            EndFunction();
        }

        void Regression_Bug144_TakeFirstImage()
        {
            EnterFunction("Regression_Bug144_TakeFirstImage");
            
            Bitmap bp = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(TextControlWraper.Element);
            
            //note 60 pixel height will cross to line so that the underline will be captured.
            bp = Microsoft.Test.Imaging.BitmapUtils.CreateSubBitmap(bp, new Rectangle(15, 10, 20, 60));
           
            TestDataArayList = new ArrayList();
            
            //save the bitmap for verification.
            TestDataArayList.Add(bp);

            ((RichTextBox)(TextControlWraper.Element)).SelectAll();
            //create underline 
            ((RichTextBox)(TextControlWraper.Element)).Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            QueueDelegate(Regression_Bug144_TakeSecondImage);
            EndFunction();
        }

        void Regression_Bug144_TakeSecondImage()
        {
            EnterFunction("Regression_Bug144_TakeSecondImage");
            Bitmap bp1, bp2, bp3;
           
            bp1 = TestDataArayList[0] as Bitmap; 
            bp2 = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(TextControlWraper.Element);
            
            //capture the bitmap in the same area.
            bp2 = Microsoft.Test.Imaging.BitmapUtils.CreateSubBitmap(bp2, new Rectangle(15, 10, 20, 60));
            bp3 = null;
            //Note: the bug show that the next line overlaps the underline of the previous line.
            //since all the line are the same, we expect no change for the image if the bug is not fixed.
            if (Microsoft.Test.Imaging.ComparisonOperationUtils.AreBitmapsEqual(bp1, bp2, out bp3))
            {
                MyLogger.Log(CurrentFunction + " - Failed: UnderLine is not rendered!");

                MyLogger.LogImage(bp1, "Regression_Bug144_Image1");
                MyLogger.LogImage(bp2, "Regression_Bug144_Image2");
                if (bp3 != null)
                {
                    MyLogger.LogImage(bp3, "Regression_Bug144_Image3");
                }
                pass = false;  
            }
            TestDataArayList = null; 
            EndTest();
            EndFunction();
        }
        #endregion 

        #region Regression test - Regression_Bug145
        /// <summary>check the illegal content</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug145")]
        public void Regression_Bug145()
        {
            EnterFunction("Regression_Bug145");
            try
            {
                pass = false;
                SetInitValue("a<Paragraph>b</Paragraph>");
            }
            catch (Exception e)
            {
                pass = true;
                MyLogger.Log("Expected exception message: " + e.Message); 
            }
            if (!pass)
                QueueDelegate(EndTest);
            try
            {
                pass = false;
                SetInitValue("a");
            }
            catch (Exception e)
            {
                pass = true;
                MyLogger.Log("Expected exception message: " + e.Message); 
            }
            if (!pass)
                QueueDelegate(EndTest);
            try
            {
                pass = false;
                SetInitValue("a<Paragraph>b</Paragraph>c");
            }
            catch (Exception e)
            {
                MyLogger.Log("Expected exception message: " + e.Message); 
                pass = true;
            }
            QueueDelegate(EndTest);
        }
        #endregion 

        #region Regression test - Regression_Bug146
        /// <summary>Regression case - Regression_Bug146</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug146")]
        public void Regression_Bug146()
        {
            EnterFuction("Regression_Bug146");
            SetInitValue("");
            KeyboardInput.TypeString("{ENTER}{BACKSPACE}+{END}{END}");
        }

        void Regression_Bug146_Done()
        {
            Bitmap bp= null; 
            if (!TextControlWraper.IsCaretRendered(out bp, true))
            {
                pass = false;
                MyLogger.LogImage(bp, "Regression_Bug146_image");
                MyLogger.Log("Regression_Bug146 Failed - Please check Regression_Bug146_Image to verify if Caret is rendered!"); 
            }
            EndTest();
        }
        #endregion 

        #region Regression test - Regression_Bug147
        /// <summary>Regression test - Regression_Bug147</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug147")]
        public void Regression_Bug147()
        {
            EnterFunction("Regression_Bug147");
            MainWindow.Content = Test.Uis.Utils.XamlUtils.ParseToObject("<FlowDocumentPageViewer xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><FlowDocument><Paragraph>ddddddddddd</Paragraph>\r\n\r\n</FlowDocument></FlowDocumentPageViewer>");
            QueueDelegate(Regression_Bug147_DoMouseClick);
            EndFunction();
        }

        void Regression_Bug147_DoMouseClick()
        {
            EnterFunction("Regression_Bug147_DoMouseClick");
            Rect rec = ElementUtils.GetScreenRelativeRect(MainWindow.Content as UIElement);
            MouseInput.MouseClick((int)rec.Right - 50, (int)rec.Top + 45);
            SetInitValue("");
            QueueDelegate(EndTest);
        }
        #endregion 

        #region Regression test - Regression_Bug148
        /// <summary>Regression_Bug436 </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug148")]
        public void Regression_Bug148()
        {
        EnterFunction("Regression_Bug148");
            SetInitValue("");
            QueueDelegate(Regression_Bug148_TypeContent);
            EndFunction();
        }

        void Regression_Bug148_TypeContent()
        {
            EnterFunction("Regression_Bug148_TypeContent");
            KeyboardInput.TypeString("a{ENTER}b{ENTER}c");
            QueueDelegate(Regression_Bug148_MouseSelection);
            EndFunction();
        }
        
        void Regression_Bug148_MouseSelection()
        {
            EnterFunction("Regression_Bug148_MouseSelection");
            Rect rec1 = TextControlWraper.GetGlobalCharacterRect(6);
            Rect rec2 = TextControlWraper.GetGlobalCharacterRect(11);
            MouseInput.MouseDragPressed(new Point(rec1.Left + 1, rec1.Top + 1), new Point(rec2.Right + 10, rec2.Bottom));
            QueueDelegate(Regression_Bug148_DoDragdrop);
            EndFunction();
        }

        void Regression_Bug148_DoDragdrop()
        {
            EnterFunction("Regression_Bug148_DoDragdrop");
            InputMonitorManager.Current.IsEnabled = false;
            Rect rec1 = TextControlWraper.GetGlobalCharacterRect(6);
           
            MouseInput.MouseDragPressed(new Point(rec1.Left + 5, rec1.Top + 5), new Point(rec1.Left + 1, rec1.Top - 35));
            QueueDelegate(Regression_Bug148_Done);
            EndFunction();
        }
        
        void Regression_Bug148_Done()
        {
            EnterFunction("Regression_Bug148_Done");
            CheckRichedEditingResults("b\r\nc\r\na\r\n", "b\r\nc\r\n", 2, 3, TextControlWraper);
            InputMonitorManager.Current.IsEnabled = false;
            KeyboardInput.TypeString("^z");
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion 

        #region Regression test - Regression_Bug149
        /// <summary> Mouse click on paragraph should not thrown any exception. </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug149")]
        public void Regression_Bug149()
        {
            EnterFuction("Regression_Bug149");
            SetInitValue("");
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Regression_Bug149_Typing));
            EndFunction();
        }

        /// <summary>Type some text in the RichTextBox.</summary>
        void Regression_Bug149_Typing()
        {
                EnterFuction("Regression_Bug149_Typing");
                Sleep();
                KeyboardInput.TypeString("AA{enter}XX{enter}CC{enter}DD");
                EndFunction();
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Regression_Bug150_DoMouseClick));
        }
        
        /// <summary>Regression_Bug150_DoMouseClick, when perfrom mouse click we don't expected any exception.</summary>
        void Regression_Bug150_DoMouseClick()
        {
                EnterFuction("Regression_Bug150_DoMouseClick");
                Sleep();
        TextPointer start = TextControlWraper.SelectionInstance.Start;
        TextPointerContext type = start.GetPointerContext(LogicalDirection.Forward);

            if (type != TextPointerContext.Text)
                {
                    start = start.GetNextContextPosition(LogicalDirection.Forward);
                }

        Rect rec = TextControlWraper.GetGlobalCharacterRect(start, 1);
        int x = (int)(rec.Left + rec.Right) / 2;
            int y = (int)((rec.Top + rec.Bottom) / 2);
                //Just do bounch of click
                MouseInput.MouseClick(x, y);
            MouseInput.MouseClick(x, y);
                MouseInput.MouseClick(x, y + (int)rec.Height);

                for (int i = 0; i < 20; i++)
                {
                    MouseInput.MouseClick(x + i, y + i);
                }
        EndFunction();
                //end case
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(EndTest));
        }
    
        #endregion

        #region Regression case - Regression_Bug151
        /// <summary>Regression_Bug151</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug151")]
        public void Regression_Bug151()
        {
        EnterFunction("Regression_Bug151");
            SetInitValue("---");
            EndFunction();
            QueueDelegate(Regression_Bug151_DoAction);
        }

        void Regression_Bug151_DoAction()
        {
            EnterFuction("Regression_Bug151_DoAction");
            KeyboardInput.TypeString("^a{LEFT}");
            EndFunction();
            QueueDelegate(Regression_Bug151_Done);
        }

        void Regression_Bug151_Done()
        {
            EnterFunction("Regression_Bug151_Done");
            Bitmap CaretImage = null;
            pass = TextControlWraper.IsCaretRendered(out CaretImage, false);
            if (!pass)
            {
                MyLogger.LogImage(CaretImage, "Regression_Bug151_CaretImage");
                MyLogger.Log(CurrentFunction, " - Failed: Caret is not Rendered! Please Check Regression_Bug151_CaretImage_.png!!!");
        }
            //This case could have bogus failure since FindCaret method is not 100% sure.
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion
            
        #region Regression case - Regression_Bug153_Start - Caret is not visible after Ctrl-{home}
        /// <summary>Regression_Bug153_Start - Caret is not visible after Ctrl-{home}</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug153")]
        public void Regression_Bug153()
        {
        EnterFunction("Regression_Bug153");
            SetInitValue("");
            QueueDelegate(Regression_Bug153_Action);
        }

        void Regression_Bug153_Action()
        {
            KeyboardInput.TypeString("    {ENTER 3}^{HOME}");
            QueueDelegate(Regression_Bug153_Done);
        }

        void Regression_Bug153_Done()
        {
            EnterFunction("Regression_Bug153_Done");
            Bitmap CaretImage = null;
            pass = TextControlWraper.IsCaretRendered(out CaretImage, false);
            if (!pass)
            {
                MyLogger.LogImage(CaretImage, "Regression_Bug153_CaretImage");
                MyLogger.Log(CurrentFunction, " - Failed: Caret is not Rendered! Please Check Regression_Bug153_CaretImage_.png!!!");
            }
            //This case could have bogus failure since FindCaret method is not 100% sure.
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion      
        
        #region Regression case - Regression_Bug154 - TextEditor: decreasing paragraph indent on first-level list item does not create paragraph
        /// <summary>Regression_Bug154</summary>
        [TestCase(LocalCaseStatus.Broken, CasePriority.p1, "Regression_Bug154")]
        public void Regression_Bug154()
        {
            EnterFunction("Regression_Bug154");
            SetInitValue("<List><ListItem>foo</ListItem></List>");
            QueueDelegate(Regression_Bug154_DecreaseIndent);
            EndFunction();
        }

        void Regression_Bug154_DecreaseIndent()
        {
            EnterFuction("Regression_Bug154_DecreaseIndent");
            KeyboardInput.TypeString("{HOME}^+M");
            EndFunction();
            QueueDelegate(Regression_Bug154_Done);
        }
        
        void Regression_Bug154_Done()
        {
            if (Occurency(TextControlWraper.XamlText, "<Paragraph") <= 0)
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - failed: no Paragraph exist!!! Please check the xaml: " + TextControlWraper.XamlText);
            }
            QueueDelegate(EndTest);
        }
        #endregion
            
        #region Regression case - Regression_Bug155 - Assert fired: Undo unit is out of sync with TextTree!
        /// <summary>Regression_Bug155: this 
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug155")]
        public void Regression_Bug155()
        {
            EnterFuction("Regression_Bug155");
            SetInitValue("");
            QueueDelegate(Regression_Bug155_Dotyping);
            EndFunction();
        }

        void Regression_Bug155_Dotyping()
        {
            EnterFuction("Regression_Bug155_Dotyping");
            KeyboardInput.TypeString("i");
            QueueDelegate(Regression_Bug155_ProgramaticallyInsertText);
            EndFunction();
        }
        
        void Regression_Bug155_ProgramaticallyInsertText()
        {
            EnterFuction("Regression_Bug155_ProgramaticallyInsertText");
            TextControlWraper.SelectionInstance.End.InsertTextInRun("tem 1");
            QueueDelegate(Regression_Bug155_PerfromUndo);
            EndFunction();
        }
        void Regression_Bug155_PerfromUndo()
        {
            EnterFuction("Regression_Bug155_PerfromUndo");
            KeyboardInput.TypeString("^a");
            //pass with no exception.
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion
        
        #region Regression case - Regression_Bug156 - Caret position is out of Button when Button contains TextPanel with TextEditor enabled
        
        /// <summary>Regression_Bug156</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug156")]
        public void Regression_Bug156()
        {
            EnterFuction("Regression_Bug156");
            StackPanel fp = new StackPanel();
            Button b = new Button();
            fp.Children.Add(b);
            RichTextBox tp = new RichTextBox();
            tp.Selection.Text = "Button";
            b.Content = tp;
            b.Name = "button";
            TextControlWraper = new UIElementWrapper(tp);
            MainWindow.Content = fp;
            QueueDelegate(Regression_Bug156_SetCaret);
            EndFunction();
        }
        
        void Regression_Bug156_SetCaret()
        {
            EnterFunction("Regression_Bug156_SetCaret");
            MouseClicksOnACharacter(TextControlWraper, 3, CharRectLocations.Center, 1);
            QueueDelegate(Regression_Bug156_Done);
            EndFunction();
        }
        
        void Regression_Bug156_Done()
        {
            EnterFunction("Regression_Bug156_Done");
            Button b = ElementUtils.FindElement(MainWindow.Content as FrameworkElement, "button") as Button;
            if (b == null)
            {
                FailedIf(true, CurrentFunction + " - Failed: can't find a button!!!");
                return;
            }
            Rect rc1 = TextControlWraper.GetGlobalCharacterRect(TextControlWraper.SelectionInstance.Start);
            Rect rc2 = ElementUtils.GetScreenRelativeRect(b);
            FailedIf(rc1.X <= rc2.X || rc1.Y <= rc2.Y || rc1.BottomRight.X >= rc2.BottomRight.X || rc1.BottomRight.Y >= rc2.BottomRight.Y, CurrentFunction + " - Failed: Caret is not on right position");
            QueueDelegate(EndTest);
            base.Init();
            EndFunction();
        }
        #endregion
        
        #region Regression case - Regression_Bug157 - LEAK: MSNTabNavigation System.Windows.Documents.TextEditorInstanceWeakReference Leak
        /// <summary>Regression case - Regression_Bug157 </summary>
        [TestCase(LocalCaseStatus.UnderDevelopment, CasePriority.p1, "Regression_Bug157")]
        public void Regression_Bug157()
        {
            StackPanel fp = new StackPanel();

            int y = CountHandlers();
            MainWindow.Content = fp;
            for (int i = 0; i < 48; i++)
                fp.Children.Add(new TextBox());
            QueueDelegate(Regression_Bug157_DeleteTextBox);
        }

        void Regression_Bug157_DeleteTextBox()
        {

            StackPanel fp = MainWindow.Content as StackPanel;
            fp.Children.RemoveRange(0, 48);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            QueueDelegate(Regression_Bug157_Done);
        }

        void Regression_Bug157_Done()
        {
            int x = CountHandlers();
            if (x > 0)
            {
            pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: We still have some referent to TextEditorInstanceWeakReference!!! Number of reference: [" + x.ToString() + "]");
            }
            QueueDelegate(EndTest);
        }

        int CountHandlers()
        {
            Dispatcher CTX = Dispatcher.CurrentDispatcher;
            int x = 0;
            EventHandler dg = Test.Uis.Utils.ReflectionUtils.GetField(CTX, "Disposed") as EventHandler;
            Delegate[] os = dg.GetInvocationList();
            foreach (Delegate obj in os)
            {
                MulticastDelegate handler = obj as MulticastDelegate;

                Log("Event in Disposed list: [" +
                    handler.Method.DeclaringType.Name + "." + handler.Method.Name + "]");
                if (handler.Method.Name.Contains("OnDispatcherDisposed") &&
                handler.Method.DeclaringType.Name.IndexOf("TextEditor") != -1)
                {
                 x++;
                }
            }
        return x;
        }

        #endregion
       
        #region Regression case - Regression_Bug158 - We should not No default indentation for Paragraph.
        /// <summary>Regression_Bug158</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug158")]
        public void Regression_Bug158()
        {
            EnterFunction("Regression_Bug158");
            SetInitValue("");
            QueueDelegate(Regression_Bug158_Dotyping);
        }

        void Regression_Bug158_Dotyping()
        {
            KeyboardInput.TypeString("a{ENTER}b");
            QueueDelegate(Regression_Bug158_Done);
        }

        void Regression_Bug158_Done()
        {
            Point p1 = FindLocation(TextControlWraper, 1, CharRectLocations.LeftEdge);
            Point p2=ElementUtils.GetScreenRelativePoint(TextControlWraper.Element, new Point(0, 0));
            //should count the border thinkness. Note: indentation is normally larger thank 10px.
            if (Math.Abs(p1.X - p2.X) > 11)
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: the should be no defult indentation, please visually check it!!!");
            }
            QueueDelegate(EndTest);
        }
        #endregion 
        
        #region Regression case - Regression_Bug159: DoPaste should try to get rid of the invalid character and do the paste without the invalid char
        /// <summary>Regression Regression_Bug159</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug159: DoPaste should try to get rid of the invalid character and do the paste without the invalid char")]
        public void Regression_Bug159()
        {
            EnterFuction("Regression_Bug159");
            SetInitValue("");
            DataObject dobj = new DataObject(DataFormats.Xaml, "<Paragraph>&#0x03</Paragragph>");
            QueueDelegate(DoPaste);
        }

        void DoPaste()
        {
            EnterFunction("DoPaste");
            KeyboardInput.TypeString("^v");
            //Pass if no exception.
            QueueDelegate(EndTest);
        }
        #endregion
        
        #region Regression case - Regression_Bug160
        /// <summary>Regression Regression_Bug160 </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug160")]
        public void Regression_Bug160()
        {
            EnterFunction("Regression_Bug160");
            SetInitValue("<Paragraph><Button>Button</Button></Paragraph><Paragraph>More Text</Paragraph>");
            QueueDelegate(Regression_Bug160_DoMouseClick);
            EndFunction();
        }
        
        void Regression_Bug160_DoMouseClick()
        {
            EnterFunction("Regression_Bug160_DoMouseClick");
            Rect rect = TextControlWraper.GetGlobalCharacterRect(2);
            MouseInput.MouseClick((int)rect.Right + 5, (int)rect.Top + 3);
            KeyboardInput.TypeString("junck");
            QueueDelegate(Regression_Bug160_Done);
        }

        void Regression_Bug160_Done()
        {
            FailedIf(!TextControlWraper.Text.Contains("junck"), "Text should contain[junck], Please check the text[" + TextControlWraper.Text + "]");
            EndTest();
        }
        #endregion 
        
        #region Regression case - Regression_Bug161: WCP - Caret is invisible after a selection (with multiple lines) is cut/deleted

        /// <summary>Regression_Bug161: WCP - Caret is invisible after a selection (with multiple lines) is cut/deleted</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug161")]
        public void Regression_Bug161()
        {
            EnterFuction("Regression_Bug161");
            SetInitValue("");
            QueueDelegate(Regression_Bug161_SetCaret);
            EndFunction();
        }

        void Regression_Bug161_SetCaret()
        {
            EnterFunction("Regression_Bug161_SetCaret");
            KeyboardInput.TypeString("a{enter}b{enter}c{up}{end}+{up}^x");
            QueueDelegate(Regression_Bug161_Done);
            EndFunction();
        }

        void Regression_Bug161_Done()
        {
            EnterFunction("Regression_Bug161_Done");
            Bitmap bp;
            if (!TextControlWraper.IsCaretRendered(out bp, false))
            {
                MyLogger.LogImage(bp, "CaretImageForRegression_Bug161");
                MyLogger.Log(CurrentFunction + " - Failed: Caret is not rendered. Please check image file - CaretImageForRegression_Bug161");
                pass = false;
            }
            QueueDelegate(EndTest);
            base.Init();
            EndFunction();
        }

        #endregion
        
        #region Regression case - Regression_Bug162 Paragraph editing: when creating a new paragraph, 'running' attributes may be lost
        /// <summary>Regression_Bug162 Paragraph editing: when creating a new paragraph, 'running' attributes may be lost</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug162 Paragraph editing: when creating a new paragraph, 'running' attributes may be lost")]
        public void Regression_Bug162()
        {
            EnterFunction("Regression_Bug162");
            SetInitValue("<Paragraph>content goes here</Paragraph>");
            QueueDelegate(Regression_Bug162_TypeSpringLoadBold);
            EndFunction();
        }

        void Regression_Bug162_TypeSpringLoadBold()
        {
            EnterFunction("Regression_Bug162_TypeSpringLoadBold");
            KeyboardInput.TypeString("^{home}{RIGHT 8}" + EditingCommandData.ToggleBold.KeyboardShortcut 
                + "some{Enter}Text+{Left 4}");
            QueueDelegate(Regression_Bug162_done);
            EndFunction();
        }

        void Regression_Bug162_done()
        {
            EnterFunction("Regression_Bug162_done");
            TextRange range = new TextRange(TextControlWraper.Start, TextControlWraper.End);
            string xaml = XamlUtils.TextRange_GetXml(range).ToLower(System.Globalization.CultureInfo.InvariantCulture);
            if (Occurency(xaml, "bold") < 1)
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: Can't find bold text in Selection!!! Please check the xaml[" + xaml + "]");
            }
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion 
        
        #region Regression case = Regression_Bug163 Lexicon: ArgumentException thrown with message 'Requested distance is outside the content of the associated TextTree' when deleting content
        /// <summary>Regression_Bug163 Lexicon: ArgumentException thrown with message 'Requested distance is outside the content of the associated TextTree' when deleting content</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Regression_Bug163 ArgumentException thrown with message 'Requested distance is outside the content of the associated TextTree' when deleting content")]
        public void Regression_Bug163()
        {
            EnterFunction("Regression_Bug163");
            SetInitValue("<Paragraph>First Line</Paragraph><Paragraph>Second Line</Paragraph><Paragraph>Third Line</Paragraph><Paragraph>Forth Line</Paragraph>");
            QueueDelegate(Regression_Bug163_PerfromActions);
            EndFunction();
        }

        void Regression_Bug163_PerfromActions()
        {
            EnterFunction("Regression_Bug163_PerfromActions");
            Rect rect = TextControlWraper.GetGlobalCharacterRect(18);
            MouseInput.MouseClick((int)(rect.Left), (int)((rect.Top + rect.Bottom)/2));
            KeyboardInput.TypeString("{ENTER}^+{home}{delete}");
            QueueDelegate(Regression_Bug163_Done);
            EndFunction();
        }

        void Regression_Bug163_Done()
        {
            EnterFunction("Regression_Bug163_Done");
            //check the selected content is deleted. No excpetion for this bug.
            CheckRichedEditingResults("cond Line\r\nThird Line\r\nForth Line\r\n", "", 0, TextControlWraper);
            QueueDelegate(EndTest);
        }
        #endregion
    }

    /// <summary>RichTextRegressionTest</summary>
    [TestOwner("Microsoft"), TestBugs("164,452, 165"), TestTactics("464")]
    [Test(2, "RichTextBox", "RichTextBoxIntegrationBugs", MethodParameters = "/TestCaseType=RichTextBoxIntegrationBugs")]
    public class RichTextBoxIntegrationBugs : CommonTestCase
    {        
        private RichTextBox _testRTB;        

        #region Regression test - Regression_Bug164
        /// <summary>Runs the test</summary>
        public override void RunTestCase()
        {
            StackPanel testPanel = new StackPanel();
            _testRTB = new RichTextBox();
            _testRTB.Width = 400;
            _testRTB.Height = 400;
            ((Paragraph)_testRTB.Document.Blocks.FirstBlock).Inlines.Add(new Run("Testing Regression_Bug164"));

            testPanel.Children.Add(_testRTB);
            MainWindow.Content = testPanel;
            QueueDelegate(InsertEmptyParagraph);
        }

        private void InsertEmptyParagraph()
        {
            Log("Verifying that Regression_Bug164 doesnt repro");
            MouseInput.MouseClick(_testRTB);
            Log("Inserting empty paragraph by pressing enter twice");
            KeyboardInput.TypeString("{HOME}{RIGHT 8}{ENTER}{ENTER}{UP}");
            QueueDelegate(InsertTable);
        }

        private void InsertTable()
        {
            DependencyObject tempDependencyObject;

            Log("Inserting Table");
            TextPointer tempPointer = _testRTB.Selection.Start;                        
            Table testTable = new Table();
            testTable.BorderThickness = new Thickness(1, 1, 1, 1);

            TableColumn tableColumn1 = new TableColumn();
            tableColumn1.Width = new GridLength(1, GridUnitType.Star);
            TableColumn tableColumn2 = new TableColumn();
            tableColumn2.Width = new GridLength(1, GridUnitType.Star);

            TableRowGroup body = new TableRowGroup();
            testTable.RowGroups.Add(body);

            TableRow row1 = new TableRow();
            body.Rows.Add(row1);
            TableCell cell1Row1 = new TableCell(new Paragraph(new Run("Row1Column1")));
            cell1Row1.BorderThickness = new Thickness(1, 1, 1, 1);            
            row1.Cells.Add(cell1Row1);
            TableCell cell2Row1 = new TableCell(new Paragraph(new Run("Row1Column2")));
            cell2Row1.BorderThickness = new Thickness(1, 1, 1, 1);            
            row1.Cells.Add(cell2Row1);

            TableRow row2 = new TableRow();
            body.Rows.Add(row2);
            TableCell cell1Row2 = new TableCell(new Paragraph(new Run("Row2Column1")));
            cell1Row2.BorderThickness = new Thickness(1, 1, 1, 1);            
            row2.Cells.Add(cell1Row2);
            TableCell cell2Row2 = new TableCell(new Paragraph(new Run("Row2Column2")));
            cell2Row2.BorderThickness = new Thickness(1, 1, 1, 1);            
            row2.Cells.Add(cell2Row2);

            tempDependencyObject = _testRTB.Selection.End.Parent;
            while (tempDependencyObject.GetType() != typeof(Paragraph))
            {
                tempDependencyObject = ((TextElement)tempDependencyObject).Parent;
            }
            ((Paragraph)tempDependencyObject).SiblingBlocks.InsertBefore((Paragraph)tempDependencyObject, testTable);
             
            QueueDelegate(VerifyTableExists);
        }

        private void VerifyTableExists()
        {
            UIElementWrapper testRTBWrapper = new UIElementWrapper(_testRTB);
            Verifier.Verify(XamlUtils.TextRange_GetXml(testRTBWrapper.TextRange).IndexOf("Table") != -1,
                "Verifying that table is inserted in RichTextBox", true);
            Log("Regression_Bug164 didnt repro");
            QueueDelegate(ReproRegression_Bug165);            
        }

        #endregion Regression test - Regression_Bug164        

        #region Regression test - Regression_Bug165

        private void ReproRegression_Bug165()
        {
            Log("Verifying that Regression_Bug165 doesnt repro");

            string reproXaml = @"<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>"  +
                "<Paragraph>" +
                "<Span>Text</Span>" +
                "<Span FontWeight='Bold'><LineBreak/></Span>" +
                "<Span FontWeight='Bold'><LineBreak/></Span>" +
                "<Span>Text</Span>" +
                "</Paragraph>" +
                "</Section>";

            _testRTB = new RichTextBox();
            _testRTB.Document.Blocks.Clear();            
            TextRange tr = new TextRange(_testRTB.Document.ContentStart, _testRTB.Document.ContentEnd);
            XamlUtils.TextRange_SetXml(tr, reproXaml);
            MainWindow.Content = _testRTB;
            QueueDelegate(DoFocus);            
        }

        private void DoFocus()
        {
            _testRTB.Focus();
            QueueDelegate(DoSelectAllAndBold);
        }

        private void DoSelectAllAndBold()
        {
            KeyboardInput.TypeString("^a" + EditingCommandData.ToggleBold.KeyboardShortcut);
            QueueDelegate(VerifyRegression_Bug165);
        }

        private void VerifyRegression_Bug165()
        {
            Paragraph para = ((Paragraph)_testRTB.Document.Blocks.FirstBlock);
            foreach (Inline inlineElement in para.Inlines)
            {
                Verifier.Verify(inlineElement.FontWeight == FontWeights.Bold,
                    "Verifying that bold is applied to all inline elements", false);
            }
            Log("Regression_Bug165 didnt repro");
            Logger.Current.ReportSuccess();
        }

        #endregion Regression test - Regression_Bug165
    }
    
    /// <summary>
    /// Verifies that only character formatting properties should be used in springloaded collection
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("465"), TestWorkItem("71"), TestBugs("453")]
    [Test(2, "RichEditing", "SpringLoadingRegressions", MethodParameters = "/TestCaseType:SpringLoadingRegressions")]
    public class SpringLoadingRegressions : CustomTestCase
    {
        #region Main Flow
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {            
            _rtb = new RichTextBox();
            _rtb.Height = 500;
            _rtb.Width = 500;
            _rtbWrapper = new UIElementWrapper(_rtb);            

            _table = new Table();
            _tableBody = new TableRowGroup();
            
            _tableRow = new TableRow();
            _tableRow.Background = System.Windows.Media.Brushes.LightBlue;

            _tableCell1 = new TableCell(new Paragraph(new Run("Cell1")));
            _tableCell1.BorderBrush = System.Windows.Media.Brushes.Red;
            _tableCell1.BorderThickness = new Thickness(2);

            _tableCell2 = new TableCell(new Paragraph(new Run("Cell2")));            
            
            _tableRow.Cells.Add(_tableCell1);
            _tableRow.Cells.Add(_tableCell2);

            _tableBody.Rows.Add(_tableRow);

            _table.RowGroups.Add(_tableBody);
            if (_rtb.Document.Blocks.Count == 0)
            {
                _rtb.Document.Blocks.Add(_table);
            }
            else
            {
                _rtb.Document.Blocks.InsertBefore(_rtb.Document.Blocks.FirstBlock, _table);
            }

            StackPanel _panel = new StackPanel();
            _panel.Children.Add(_rtb);
            MainWindow.Content = _panel;
            QueueDelegate(SetFocusInTableCell);
        }

        private void SetFocusInTableCell()
        {
            TextPointer tpCellEnd = _tableCell1.ContentEnd.GetInsertionPosition(LogicalDirection.Backward);            
            Rect _clickPosition = tpCellEnd.GetCharacterRect(LogicalDirection.Backward);
            Rect _rtbPosition = ElementUtils.GetScreenRelativeRect(_rtb);            
            MouseInput.MouseClick((int)(_clickPosition.X + _rtbPosition.X), (int)(_clickPosition.Y + _rtbPosition.Y));
            QueueDelegate(PerformAction);
        }

        private void PerformAction()
        {
            KeyboardInput.TypeString("{END}{BACKSPACE}{ENTER}abc def");
            QueueDelegate(VerifySpringLoading);
        }

        private void VerifySpringLoading()
        {
            System.Windows.Media.Brush _borderBrush = (System.Windows.Media.Brush)_rtb.Selection.GetPropertyValue(Block.BorderBrushProperty);
            Verifier.Verify(_borderBrush != System.Windows.Media.Brushes.Red,
                "Verifying that BorderBrush property is not springloaded", true);
            Thickness _borderThickness = (Thickness)_rtb.Selection.GetPropertyValue(Block.BorderThicknessProperty);
            Verifier.Verify(_borderThickness != new Thickness(2),
                "Verifying that BorderThickness property is not springloaded", true);

            Logger.Current.ReportSuccess();
        }
        #endregion Main Flow

        #region Private fields
        RichTextBox _rtb;
        UIElementWrapper _rtbWrapper;
        Table _table;
        TableRowGroup _tableBody;
        TableRow _tableRow;
        TableCell _tableCell1, _tableCell2;
        #endregion Private fields
    }

    /// <summary>
    /// Verifies regressions on TextRange
    /// Regression_Bug166
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("466"), TestWorkItem(""), TestBugs("166")]
    [Test(2, "RichTextBox", "TextRangeRegressions", MethodParameters = "/TestCaseType=TextRangeRegressions")]
    public class TextRangeRegressions : CustomTestCase
    {
        #region Main Flow

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 500;
            _rtb.Width = 500;
            _rtb.Document.Blocks.Clear();
            _rtbWrapper = new UIElementWrapper(_rtb);
            _run1 = new Run("Repro for Regression_Bug166");
            _para1 = new Paragraph(_run1);
            _rtb.Document.Blocks.Add(_para1);
            _rtb.Selection.Select(_run1.ContentStart, _run1.ContentEnd);

            StackPanel _panel = new StackPanel();
            _panel.Children.Add(_rtb);
            MainWindow.Content = _panel;
            QueueDelegate(SetFirstForeground);
        }

        private void SetFirstForeground()
        {
            _rtb.Focus();
            LinearGradientBrush brush1 = new LinearGradientBrush(Colors.Red, Colors.Blue, 0);
            _rtb.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush1);

            QueueDelegate(SetSecondForeground);
        }

        private void SetSecondForeground()
        {
            _brushRetValue1 = (LinearGradientBrush)_rtb.Selection.GetPropertyValue(TextElement.ForegroundProperty);

            LinearGradientBrush brush2 = new LinearGradientBrush(Colors.Green, Colors.Black, 0);
            _rtb.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush2);

            QueueDelegate(VerifyForegroundChanged);
        }

        private void VerifyForegroundChanged()
        {
            _brushRetValue2 = (LinearGradientBrush)_rtb.Selection.GetPropertyValue(TextElement.ForegroundProperty);

            Verifier.Verify(_brushRetValue1 != _brushRetValue2, "Verifying that the Foreground property has changed", true);
            Logger.Current.ReportSuccess();
        }
        
        #endregion Main Flow

        #region Private fields

        RichTextBox _rtb;
        UIElementWrapper _rtbWrapper;
        Paragraph _para1;
        Run _run1;
        LinearGradientBrush _brushRetValue1,_brushRetValue2;

        #endregion Private fields
    }
}
