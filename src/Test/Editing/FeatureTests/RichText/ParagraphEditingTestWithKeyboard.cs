// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.Utils;

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/RichText/ParagraphEditingTestWithKeyboard.cs $")]

namespace Test.Uis.TextEditing
{    
    /// <summary>
    /// Test Paragraph editing using keyboard.
    /// Verify:
    /// 1) correct selection
    /// 2) typing
    /// 3) undo/redo,
    /// 4) cross <P></P> editing
    /// 5) cut
    /// 6) paste
    /// 7) delete,
    /// 8) backspace
    /// 9) left,right,up,down,etc
    ///</summary>
    [Test(0, "RichEditing", "ParagraphEditingTestWithKeyboard", MethodParameters = "/TestCaseType=ParagraphEditingTestWithKeyboard /Case=RunAllCases")]
    [TestOwner("Microsoft"), TestTactics("692,467"), TestBugs("329, 532, 533,328, 534, 835, 396, 333, 529, 836, 90, 430, 87, 88, 331, 330, 339, 322, 323, 324, 325, 326, 327, 152, 535")]
    public class ParagraphEditingTestWithKeyboard : RichEditingBase
    {        
        #region cases - BasicEditinginParagraph

        /// <summary>CaretNavigationForParagraph</summary>
        [TestCase(LocalCaseStatus.Ready, "CaretNavigationForParagraph")]
        public void CaretNavigationForParagraph()
        {
            EnterFuction("OtherWaysToSplitAndMerge");
            TestDataArayList = new ArrayList();
            //{Home}
            TestDataArayList.Add(new RichEditingData("Home key To Set caret to the beginning of a line", "", "a{enter}d{home}+{Right}", "a\r\nd\r\n", "d", 0, 2, true, 0));

            //Ctrl->{Home} /{Down}
            TestDataArayList.Add(new RichEditingData("Ctrl->Home key To Set caret to the beginning of a line", "", "a{enter}d^{home}+{Down}", "a\r\nd\r\n", "a\r\n", 1, 2, true, 0));

            //{End} /{Up}
            TestDataArayList.Add(new RichEditingData("End and Up key for Paragraph", "", "a{enter}d{Left}{Up}{End}+{Left}", "a\r\nd\r\n", "a", 0, 2, true, 0));

            //Ctrl->{End} /{Up}
            TestDataArayList.Add(new RichEditingData("Ctrl-{End}/{Up}key for Paragraph", "", "a{enter}d{Left}{Up}^{End}+{Left}", "a\r\nd\r\n", "d", 0, 2, true, 0));

            //{PageUp}
            TestDataArayList.Add(new RichEditingData("PageUp Key for Paragraph", "", "a{enter}d{PGUP}+{Left}", "a\r\nd\r\n", "a", 0, 2, true, 0));

            //{PageDown}
            TestDataArayList.Add(new RichEditingData("PageDown key for Paragraph", "", "a{enter}d^{home}{PGDN}+{Right}", "a\r\nd\r\n", "d", 0, 2, true, 0));

            SetInitValue("");
            QueueDelegate(RichEditingDataKeyBoardExecution);
            EndFunction();
        }

        /// <summary>BasicEditinginParagraph starts the test</summary>
        [TestCase(LocalCaseStatus.Ready, "Basic Editingin for Paragraph")]
        public void BasicEditinginParagraph()
        {
            EnterFuction("BasicEditinginParagraph");
            TextRange MyRange = new TextRange(TextControlWraper.Start, TextControlWraper.End);
            MyRange.Text = String.Empty;
            MyRange.Select(MyRange.End, MyRange.End);
            
            //set the focus in the textpanel
            MouseInput.MouseClick(TextControlWraper.Element);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(TypeTextIntoTextPanel));
            EndFunction();
        }

        //TypeTextIntoTextPanel
        private void TypeTextIntoTextPanel()
        {
            EnterFuction("TypeTextIntoTextPanel");
            KeyboardInput.TypeString("abc{enter}de+{Left 2}");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(MakeSelectionCrossParagraph));
        }

        //MakeSelectionCrossParagraph
        private void MakeSelectionCrossParagraph()
        {
            EnterFuction("MakeSelectionCrossParagraph");
            CheckRichedEditingResults("abc\r\nde\r\n", "de", 0,TextControlWraper);
            MyLogger.Log(CurrentFunction + " - Make selection cross Paragraph");
            KeyboardInput.TypeString("+{Left}");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(CutCrossParagraphSelection));
        }

        //CutCrossParagraphSelection
        private void CutCrossParagraphSelection()
        {
            EnterFuction("CutCrossParagraphSelection");
            CheckRichedEditingResults("abc\r\nde\r\n", "\r\nde", 2, TextControlWraper);
            MyLogger.Log(CurrentFunction + " - Perfrom cut on selection");
            KeyboardInput.TypeString("^x");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(UndoCutOnSelection));
        }

        //UndoCutOnSelection
        private void UndoCutOnSelection()
        {
            EnterFuction("UndoCutOnSelection");

            //Regression_Bug334 is fixed. After a selection is cut. Selection.Text should be a empty string rather than "\r\n"
            CheckRichedEditingResults("abc\r\n", "", 0, TextControlWraper);
            MyLogger.Log(CurrentFunction + " perfrom undo on cuting selection.");
            KeyboardInput.TypeString("^z");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(RedoCutOnSelection));
        }

        //RedoCutOnSelection
        private void RedoCutOnSelection()
        {
            EnterFuction("RedoCutOnSelection");
            CheckRichedEditingResults("abc\r\nde\r\n", "\r\nde", 2, TextControlWraper);
            MyLogger.Log(CurrentFunction + " - Perfrom redo cuting on selection");
            KeyboardInput.TypeString("^y");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(TypeMoreParagraphs));
        }

        //TypeMoreParagraphs
        private void TypeMoreParagraphs()
        {
            EnterFuction("TypeMoreParagraphs");

            CheckRichedEditingResults("abc\r\n", "", 0, TextControlWraper);
            MyLogger.Log(CurrentFunction + "type more text.");
            KeyboardInput.TypeString("d{enter}eee{enter}www{enter}ggg");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(SelectAllTextInTextPanel));
        }

        //SelectAllTextInTextPanel
        private void SelectAllTextInTextPanel()
        {
            EnterFuction("SelectAllTextInTextPanel");

            CheckRichedEditingResults("abcd\r\neee\r\nwww\r\nggg\r\n", "", 0, TextControlWraper);
            //perfrom select all
            KeyboardInput.TypeString("{Left 4}^a");
            EndFunction();
            QueueDelegate( DeleteCrossParagraphs);
        }

        //DeleteCrossParagraphs
        private void DeleteCrossParagraphs()
        {
            EnterFuction("DeleteCrossParagraphs");

            CheckRichedEditingResults("abcd\r\neee\r\nwww\r\nggg\r\n", "abcd\r\neee\r\nwww\r\nggg\r\n", 4, TextControlWraper);
            //note: Up key send the caret to the beginning of the first line.
            KeyboardInput.TypeString("^{Up}{RIGHT 2}{down}+{RIGHT 3}{DELETE}");

            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(EndBasicEditingforParagraph));
        }

        //EndBasicEditingforParagraph
        private void EndBasicEditingforParagraph()
        {
            EnterFuction("EndBasicEditingforParagraph");

            CheckRichedEditingResults("abcd\r\neeww\r\nggg\r\n", "", 0, TextControlWraper);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(PasteMultipleParagraphs);

            EndFunction();
        }

        private void PasteMultipleParagraphs()
        {
            EnterFuction("PasteMultipleParagraphs");

            // Delete all content, type a paragraph with an empty paragraph
            // above it, then copy and paste it multiple times (the first time
            // it should overwrite itself).
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(CheckPasteMultipleParagraphs);

            EndFunction();
        }

        private void CheckPasteMultipleParagraphs()
        {
            EnterFuction("CheckPasteMultipleParagraphs");

            EndFunction();
            QueueDelegate(EndTest);
        }

        #endregion

        #region Regression case - Regression_Bug333

        /// <summary>
        /// the bug is filed on RichTextBox, however, we don't RichTextbox does not any textcomtent property exposed. 
        /// so, I used textpanel for the regression 
        /// </summary>
        [TestCase(LocalCaseStatus.UnderDevelopment, "Regression Test for Regression_Bug333")]
        public void Regression_Bug333()
        {
            EnterFuction("Regression_Bug333");
                        
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(Regression_Bug333_End);
        }

        private void Regression_Bug333_End()
        {
            EnterFuction("Regression_Bug333_End");
            CheckRichedEditingResults("First Line.\n Selcond Line.", "First Line.\n Selcond Line.", 0, TextControlWraper);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion   
  
        #region Regression case - Regression_Bug332

        /// <summary>Regression_Bug332</summary>
        [TestCase(LocalCaseStatus.UnderDevelopment, "Regression Test for Regression_Bug332")]
        public void Regression_Bug332()
        {
            EnterFunction("Regression_Bug332");
            SetInitValue("<Paragraph>xyz<TextBox>Textbox</TextBox></Paragraph>");
            EndFunction();
            QueueDelegate(Regression_Bug332_MouseClickTextBox);
        }

        private void Regression_Bug332_MouseClickTextBox()
        {
            EnterFunction("Regression_Bug332_MouseClickTextBox");
            TextPointer na = TextControlWraper.SelectionInstance.Start;
            CheckRichedEditingResults("xyz \r\n", "xyz \r\n", 1, TextControlWraper);
            Rect rect = TextControlWraper.GetGlobalCharacterRect(na, 2);
            //the first mouse click is needed due to Regression_Bug542
            MouseInput.MouseClick((int)rect.X, (int)(rect.Y + rect.Height / 2));
            MouseInput.MouseClick((int)rect.X + 60, (int)(rect.Y + rect.Height / 2));
            EndFunction();
            QueueDelegate(Regression_Bug332_EditInTextBox);
        }

        private void Regression_Bug332_EditInTextBox()
        {
            EnterFunction("Regression_Bug332_EditInTextBox");
            KeyboardInput.TypeString("junck");
            CheckRichedEditingResults("xyz \r\n", "", 0, TextControlWraper);
            TextPointer na = TextControlWraper.SelectionInstance.Start;
            Rect rect = TextControlWraper.GetGlobalCharacterRect(na, 2);
            MouseInput.MouseClick((int)rect.X, (int)(rect.Y + rect.Height / 2));
            EndFunction();
            QueueDelegate(Regression_Bug332_Done);
        }

        private void Regression_Bug332_Done()
        {
            EnterFunction("Regression_Bug332_Done");
            CheckRichedEditingResults("xyz \r\n", "", 0, TextControlWraper);
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion

        #region Regression case - Regression_Bug331

        /// <summary>Regression_Bug331</summary>
        [TestCase(LocalCaseStatus.Broken, "Regression Test for Regression_Bug331")]
        public void Regression_Bug331()
        {
            EnterFunction("Regression_Bug331");
            
            SetInitValue("abcdef");
            EndFunction();
            QueueDelegate(Regression_Bug331_IncreaseIdent);
        }

        private void Regression_Bug331_IncreaseIdent()
        {
            EnterFunction("Regression_Bug331_IncreaseIdent");
            KeyboardInput.TypeString("^M^a");
            EndFunction();
            QueueDelegate(Regression_Bug331_Done);
        }

        private void Regression_Bug331_Done()
        {
            EnterFunction("Regression_Bug331_Done");
            string str=XamlUtils.TextRange_GetXml(TextControlWraper.SelectionInstance);
            int x = Occurency(str, "list");
            if (x > 0)
                pass = false;
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion

        #region Regression case - Regression_Bug330

        /// <summary>Regression_Bug330 </summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug330")]
        public void Regression_Bug330()
        {
            EnterFunction("Regression_Bug330");
            
            SetInitValue("");
            EndFunction();
            QueueDelegate(Regression_Bug330_IncreaseDecreaseIndent);
        }

        private void Regression_Bug330_IncreaseDecreaseIndent()
        {
            EnterFunction("Regression_Bug330_IncreaseDecreaseIndent");
            KeyboardInput.TypeString("abc{enter}def{enter}ghi^{end}{up}^M^+M^+M^+M^a");
            EndFunction();
            QueueDelegate(Regression_Bug330_Done);
        }

        private void Regression_Bug330_Done()
        {
            EnterFunction("Regression_Bug330_Done");
            CheckRichedEditingResults("abc\r\ndef\r\nghi\r\n", "abc\r\ndef\r\nghi\r\n", 3, TextControlWraper);
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion 

        #region Regression case - Regression_Bug319

        /// <summary>Regression_Bug319</summary>
        [TestCase(LocalCaseStatus.UnderDevelopment, "Regression Test for Regression_Bug319")]
        public void Regression_Bug319()
        {
            EnterFunction("Regression_Bug319");
            
            SetInitValue("<Paragraph></Paragraph>");
            EndFunction();
            QueueDelegate(Regression_Bug319_DoUndo);
        }

        private void Regression_Bug319_DoUndo()
        {
            EnterFunction("Regression_Bug319_DoUndo");
            KeyboardInput.TypeString("abc{enter}def{enter}ghi^{end}{up}^M^+M^+M^+M^z^a");
            EndFunction();
            QueueDelegate(Regression_Bug319_Done);
        }

        private void Regression_Bug319_Done()
        {
            EnterFunction("Regression_Bug319_Done");
            CheckRichedEditingResults("abc\r\ndef\r\nghi\r\n", "abc\r\ndef\r\nghi\r\n", 3, TextControlWraper);
            QueueDelegate(EndTest);
            EndFunction();

        }

        #endregion

        #region Regression case - Regression_Bug322

        /// <summary></summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug322")]
        public void Regression_Bug322()
        {
            EnterFunction("Regression_Bug322");
            
            SetInitValue("");
            EndFunction();
            QueueDelegate(Regression_Bug322_SetSelection);
        }

        private void Regression_Bug322_SetSelection()
        {
            EnterFunction("Regression_Bug322_SetSelection");
            TextControlWraper.SelectionInstance.Text = "\uD8D2\uFDE3";
            EndFunction();
            QueueDelegate(Regression_Bug322_DoCopyPaste);
        }

        private void Regression_Bug322_DoCopyPaste()
        {
            EnterFunction("Regression_Bug322_DoCopyPaste");
            KeyboardInput.TypeString("^c^v");
            EndFunction();
            QueueDelegate(Regression_Bug322_Done);
        }

        private void Regression_Bug322_Done()
        {
            EnterFunction("Regression_Bug322_Done");
            //pass if no crash.
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion 

        #region Regression case - Regression_Bug323

        /// <summary>Regression_Bug323</summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug323")]
        public void Regression_Bug323()
        {
            EnterFunction("Regression_Bug323");
            
            SetInitValue("");
            EndFunction();
            QueueDelegate(Regression_Bug323_PerfromAction);
        }

        private void Regression_Bug323_PerfromAction()
        {
            EnterFunction("Regression_Bug323_PerfromAction");
            KeyboardInput.TypeString("s" + EditingCommandData.ToggleBold.KeyboardShortcut + "Hello");
            EndFunction();
            QueueDelegate(Regression_Bug323_TurnOffBold);
        }

        private void Regression_Bug323_TurnOffBold()
        {
            EnterFunction("Regression_Bug323_TurnOffBold");
            string str = XamlUtils.TextRange_GetXml(TextControlWraper.TextRange);
            //we expected the Bold in the xaml.
            if (!str.Contains("Bold"))
            {
                MyLogger.Log(CurrentFunction + " - Failed: expected bold in the xaml!!! Actual xaml: " + str);
                pass = false;
            }

            KeyboardInput.TypeString("{BackSpace 5}" + EditingCommandData.ToggleBold.KeyboardShortcut + "abc");
            EndFunction();
            QueueDelegate(Regression_Bug323_Done);
        }

        private void Regression_Bug323_Done()
        {
            EnterFunction("Regression_Bug323_Done");
            string str = XamlUtils.TextRange_GetXml(TextControlWraper.TextRange);
            //we don't expected the Bold in the xaml.
            if (str.Contains("Bold")) 
            {
                pass = false;
            }
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion 

        #region Regression case - Regression_Bug324

        /// <summary>Regression_Bug324</summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug324")]
        public void Regression_Bug324()
        {
            EnterFunction("Regression_Bug324");
            SetInitValue("<Paragraph></Paragraph>");
            EndFunction();
            QueueDelegate(Regression_Bug324_PerfromAction);
        }

        private void Regression_Bug324_PerfromAction()
        {
            EnterFunction("Regression_Bug324_PerfromAction");
            KeyboardInput.TypeString("s" + EditingCommandData.ToggleBold.KeyboardShortcut + "Hello{end}world");
            EndFunction();
            QueueDelegate(Regression_Bug324_TypeMore);
        }

        private void Regression_Bug324_TypeMore()
        {
            KeyboardInput.TypeString("{home}{delete 8}");
            QueueDelegate(Regression_Bug324_Done);
        }

        private void Regression_Bug324_Done()
        {
            EnterFunction("Regression_Bug324_Done");
            string str = XamlUtils.TextRange_GetXml(TextControlWraper.TextRange);
            CheckRichedEditingResults("rld\r\n", "", 0, TextControlWraper);
            //we expected the Bold in the xaml.
            if (!str.Contains("Bold"))
            {
                MyLogger.Log(CurrentFunction + " - Failed: expected bold in the xaml!!! Actual xaml: " + str);
                pass = false;
            }
            EndFunction();
            QueueDelegate(EndTest);
        }

        #endregion

        #region regression case - Regression_Bug325

        /// <summary>Regression_Bug325</summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug325")]
        public void Regression_Bug325()
        {
            EnterFunction("Regression_Bug325");
            
            SetInitValue("");
            EndFunction();
            QueueDelegate(Regression_Bug325_APPlyBold);
        }

        private void Regression_Bug325_APPlyBold()
        {
            EnterFunction("Regression_Bug325_APPlyBold");
            KeyboardInput.TypeString("abc{enter}def{enter}^a" + EditingCommandData.ToggleBold.KeyboardShortcut);
            EndFunction();
            QueueDelegate(Regression_Bug325_Done);
        }

        private void Regression_Bug325_Done()
        {
            EnterFunction("Regression_Bug325_Done");
            CheckRichedEditingResults("abc\r\ndef\r\n\r\n", "abc\r\ndef\r\n\r\n", 3, TextControlWraper);
            QueueDelegate(EndTest);
        }

        #endregion 

        #region Regression case - Regression_Bug326

        /// <summary>Regression_Bug326</summary>
        [TestCase(LocalCaseStatus.UnderDevelopment, "Regression Test for Regression_Bug326")]
        public void Regression_Bug326()
        {
            EnterFunction("Regression_Bug326");
            
            SetInitValue("<Paragraph><Bold>text</Bold></Paragraph><Paragraph><Bold>text</Bold></Paragraph>");
            EndFunction();
            QueueDelegate(Regression_Bug326_ApplyBold);
        }

        private void Regression_Bug326_ApplyBold()
        {
            EnterFunction("Regression_Bug326_ApplyBold");
            string str = XamlUtils.TextRange_GetXml(TextControlWraper.TextRange);
            if (!str.Contains("Bold") || str.Contains("<Inline FontWeight=\"Normal\">text</Inline>"))
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: content is not bolded!!! Check the XAML: " + str);
            }
            KeyboardInput.TypeString("^a" + EditingCommandData.ToggleBold.KeyboardShortcut);
            EndFunction();
            QueueDelegate(Regression_Bug326_Done);
        }

        private void Regression_Bug326_Done()
        {
            EnterFunction("Regression_Bug326_Done");
            string str = XamlUtils.TextRange_GetXml(TextControlWraper.TextRange);
            if (str.Contains("Bold"))
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: content is bolded!!! Check the XAML: " + str);
            }
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion

        #region Regression case - Regression_Bug327

        /// <summary>Regression_Bug327</summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug327")]
        public void Regression_Bug327()
        {
            EnterFunction("Regression_Bug327");
            
            SetInitValue("<Paragraph><Bold>para1.</Bold></Paragraph><Paragraph>abc.</Paragraph>");
            EndFunction();
            QueueDelegate(Regression_Bug327_DoMouseClick);
        }

        private void Regression_Bug327_DoMouseClick()
        {
            EnterFunction("Regression_Bug327_DoMouseClick");
            TextPointer tc = TextControlWraper.SelectionInstance.Start;
            Rect rect = TextControlWraper.GetGlobalCharacterRect(tc, 6);
            MouseInput.MouseClick((int)rect.Left, (int)(rect.Top + rect.Height / 2));
            EndFunction();
            QueueDelegate(Regression_Bug327_ApplyRight);
        }

        private void Regression_Bug327_ApplyRight()
        {
            EnterFunction("Regression_Bug327_ApplyRight");
            KeyboardInput.TypeString("{RIGHT}");
            EndFunction();
            QueueDelegate(Regression_Bug327_Done);
        }

        private void Regression_Bug327_Done()
        {
            EnterFunction("Regression_Bug327_Done");
            //no exception, then pass
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion 

        #region regression case - Regression_Bug328

        /// <summary>Regression_Bug328</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "Regression Test for Regression_Bug328")]
        public void Regression_Bug328()
        {
            EnterFuction("Regression_Bug328");
            SetInitValue("<Paragraph>a" +RepeatString("bbbbbbbbbbbb", 10) + "<LineBreak></LineBreak>" +"ccccc</Paragraph><Paragraph>ZZZZZ</Paragraph>");
            QueueDelegate(Regression_Bug328_MoveCaretDown);
            EndFunction();
        }

        private void Regression_Bug328_MoveCaretDown()
        {
            KeyboardInput.TypeString("^{HOME}^{DOWN}+{RIGHT}");
            QueueDelegate(Regression_Bug328_CheckCaretAtLastParagraph);
        }

        private void Regression_Bug328_CheckCaretAtLastParagraph()
        {
            EnterFunction("Regression_Bug328_CheckCaretAtLastParagraph");
            FailedIf(TextControlWraper.SelectionInstance.Text !="Z", CurrentFunction + " - Failed: Text after caret should be[Z], Actual[" + TextControlWraper.SelectionInstance.Text + "]");
            if (!pass)
            {
                EndTest();
            }
            else
            {
                KeyboardInput.TypeString("{HOME}^{UP}+{RIGHT}");
            }
            QueueDelegate(Regression_Bug328_CheckCaretAtFirstLine);
            EndFunction();
        }

        private void Regression_Bug328_CheckCaretAtFirstLine()
        {
            EnterFunction("Regression_Bug328_CheckCaretAtFirstLine");
            FailedIf(TextControlWraper.SelectionInstance.Text != "a", CurrentFunction + " - Failed: Text after caret should be[a], Actual[" + TextControlWraper.SelectionInstance.Text + "]");
            EndTest();
            EndFunction();
        }

        #endregion

        #region regression case - Regression_Bug329

        /// <summary>Exception should be thrown when set up invalid xaml</summary>
        public void Regression_Bug329()
        {
            EnterFunction("Regression_Bug329");
            try
            {
                pass = false;
                SetInitValue("<Paragraph>a</Paragraph>invalid");
            }
            catch (System.Windows.Markup.XamlParseException e)
            {
                MyLogger.Log(e.Message);
                pass = true;
            }
            EndTest();
        }

        #endregion
     }
}
