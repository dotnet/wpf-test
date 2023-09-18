// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 22 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/Text/BVT/RichText/ParagraphEditingTestWithKeyboard.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Management;    

    #endregion Namespaces.

    /// <summary>This class contains BVT cases for deleting Paragraph by user actions.</summary>
    [Test(0, "RichEditing", "PargraphDeletingAndMergingBVT", MethodParameters = "/TestCaseType=PargraphDeletingAndMergingBVT /Priority=0", Timeout=300)]
    [TestOwner("Microsoft"), TestTactics("694"), TestBugs("840, 541, 540, 536, 529, 530, 430, 531, 532, 533, 534, 535, 152, 528"), TestWorkItem("147, 148")]
    public class PargraphDeletingAndMergingBVT : RichEditingBase
    {
        /// <summary>use {backspace} to delete paragraph. Plese don't change the cases order since some cases depends on the previoius cases</summary>
        [TestCase(LocalCaseStatus.Ready, "BackSpace to delete Paragraph")]
        public void BasicBVT_Actions()
        {
            EnterFuction("BasicBVT_Actions");
            
            if (TestDataArayList == null)
                TestDataArayList = new ArrayList();
            
            TestDataArayList.Clear();
            
            #region backspace
            
            //Undo/redo for enter to creating paragraph
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "a{enter}{backspace}", "a\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^z", "a\r\n\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^y", "a\r\n", "", 0, 1, true, 0));//Redo

            //Undo/redo, backspace to delete a paragraph
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "{enter}{backspace}", "\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^z", "\r\n\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^y", "\r\n", "", 0, 1, true, 0));//Redo

            //Do backspace should not delete the only pargraph in the doucment.
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "a{enter}{backspace}{HOME}{backspace 3}", "a\r\n", "", 0, 1, true, 0));

            //Bakck space merge two paragraph
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "ab{left}{enter}{backspace}", "ab\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^z", "a\r\nb\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^y", "ab\r\n", "", 0, 1, true, 0));//Redo

            //BackSpace from the end of the document.
            TestDataArayList.Add(new RichEditingData("BackSpace key", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^{End}{backspace}", "a\r\n\r\n", "", 0, 2, false, 0));
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "{backspace}", "a\r\n", "", 0, 1, true, 0));

            //Backspace on Selection containing a paragraph.
            TestDataArayList.Add(new RichEditingData("Backspace on Selection containing a paragraph.", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^{Home}+{Right 2}{backspace}", "b\r\n", "", 0, 1, true, 0));

            //Backspace on selection that contains all contains of a RichTextBox
            TestDataArayList.Add(new RichEditingData("BackSpace on selection that contains all contains of a RichTextBox", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^a{backspace}", "", "",0, 0, true, 0));

            //Backspace on selection that cross paragraphs
            TestDataArayList.Add(new RichEditingData("Backspace on selection that cross paragraphs", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph><Paragraph>c</Paragraph>", "^{Home}+{Right 4}{backspace}", "c\r\n", "", 0, 1, true, 0));
            
            #endregion 
            
            #region delete

            //Delete at the end of a paragraph will delete the followed paragraph.
            //Undo, Redo
            TestDataArayList.Add(new RichEditingData("DeleteKey_Actions", "", "a{enter}{Left}{Delete}", "a\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("DeleteKey_Actions", "", "^z", "a\r\n\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("DeleteKey_Actions", "", "^y", "a\r\n", "", 0, 1, true, 0));//Redo

            //Delete works for empty paragraph
            //Undo, Redo
            TestDataArayList.Add(new RichEditingData("Delete an empty line", "", "{enter}{Left}{Delete}", "\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("Delete an empty line", "", "^z", "\r\n\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("Delete an empty line", "", "^y", "\r\n", "", 0, 1, true, 0));//Redo

            //Do Delete should not delete the only pargraph in the doucment.
            TestDataArayList.Add(new RichEditingData("Delete key", "", "a{enter}{Left}{DELETE}", "a\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("Delete key", "", "{Home}{DELETE 2}", "\r\n", "", 0, 1, true, 0));

            //Delete merge paragraphs
            //Undo/Redo
            TestDataArayList.Add(new RichEditingData("Delete merge paragraphs", "", "ab{left}{enter}{LEFT}{DELETE}", "ab\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("Delete merge paragraphs", "", "^z", "a\r\nb\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("Delete merge paragraphs", "", "^y", "ab\r\n", "", 0, 1, true, 0));//Redo

            //Delete from the begining of the document.
            TestDataArayList.Add(new RichEditingData("Delete from the begining of the document", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^{HOME}{DELETE}", "\r\nb\r\n", "", 0, 2, false, 0));
            TestDataArayList.Add(new RichEditingData("Delete from the begining of the document", "", "{DELETE}", "b\r\n", "", 0, 1, true, 0));

            //Delete on Selection containing a paragraph
            TestDataArayList.Add(new RichEditingData("Delete on Selection containing a paragraph", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^{Home}+{Right 2}{Delete}", "b\r\n", "", 0, 1, true, 0));

            //Delete all contain in RichTextbox
            TestDataArayList.Add(new RichEditingData("Delete all contain in RichTextbox", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^a{Delete}", "", "", 0, 0, true, 0));

            //Delete Selection that crosses Paragraph
            TestDataArayList.Add(new RichEditingData("//Delete Selection that crosses Paragraph", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph><Paragraph>c</Paragraph>", "^{Home}+{Right 4}{Delete}", "c\r\n", "", 0, 1, true, 0));

            #endregion 
            
            #region cut/paste

            //cut selection that contains two paragraphs
            TestDataArayList.Add(new RichEditingData("cut selection that contains two paragraphs", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph><Paragraph>c</Paragraph>", "^{Home}+{Right 4}^x", "c\r\n", "", 0, 1, true, 0));

            //Cut selection that contains 1 paragraph
            TestDataArayList.Add(new RichEditingData("Cut selection that contains 1 paragraph", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^{Home}+{Right 2}^x", "b\r\n", "", 0, 1, true, 0));

            //Paste paragraphs
            TestDataArayList.Add(new RichEditingData("cut Selection", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^a^x", "", "", 0, 0, true, 0));
            TestDataArayList.Add(new RichEditingData("paste at beginning of a paragraph", "<Paragraph>x</Paragraph>", "{Home}^v", "a\r\nb\r\nx\r\n", "", 0, 3, true, 0));
            TestDataArayList.Add(new RichEditingData("paste at end of a paragraph", "<Paragraph>x</Paragraph>", "{END}^v", "xa\r\nb\r\n\r\n", "", 0, 3, true, 0));
            TestDataArayList.Add(new RichEditingData("paste on end of a empty RichTextBox", "", "{END}^v", "a\r\nb\r\n\r\n", "", 0, 3, true, 0));
            TestDataArayList.Add(new RichEditingData("paste on beginning of a empty RichTextBox", "", "{HOME}^v", "a\r\nb\r\n\r\n", "", 0, 3, true, 0));

            //Cut empty Paragraph and paste it
            TestDataArayList.Add(new RichEditingData("cut an empty Paragraph selected from right to left", "", "a{enter}+{LEFT}^x", "a\r\n", "", 0, 1, true, 0));
            TestDataArayList.Add(new RichEditingData("Paste any empty Paragraph after typing a character", "", "b^v", "b\r\n\r\n", "", 0, 2, true, 0));
            TestDataArayList.Add(new RichEditingData("Paste a empty paragraph at the beginning", "", "b{LEFT}^v", "\r\nb\r\n", "", 0, 2, true, 0));
            TestDataArayList.Add(new RichEditingData("Cut an empty paragraph selected from left to right", "", "a{enter}{LEFT}+{RIGHT}^x", "a\r\n", "", 0, 1, true, 0));

            //Cut selection that contains 1 paragraph
            TestDataArayList.Add(new RichEditingData("Cut selection that contains 1 paragraph", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^{Home}+{Right 2}^x", "b\r\n", "", 0, 1, true, 0));

            //Control backspace: delete between selection and previous Regression_Bug528
            TestDataArayList.Add(new RichEditingData("ctrl-backspace with selection cross paragraph", "", "ab{enter}cd{left}+{left 3}^{backspace}", "ad\r\n", "", 0, 1, true, 0));

            #endregion 
            
            StartCasesRunning();
            EndFunction();
        }

        /// <summary>Regression tests</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "regression test")]
        public void RegressionTest_BVT()
        {
            TestDataArayList = new System.Collections.ArrayList();
            //Regression case - Regression_Bug529
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug529", "", "abcd^a{ENTER}", "\r\n\r\n", "", 0, 2, true, 0));

            //Regression case - Regression_Bug530
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug530", "", "abc{enter}def{Up}{Home}{Right}+{Right 2}+{End}+{Home}", "abc\r\ndef\r\n", "a", 0, 2, true, 0));

            //Regression case - Regression_Bug430
            TestDataArayList.Add(new RichEditingData("Regression - Regression_Bug430", "<Paragraph><Span FontSize=\"24pt\"><Run>abc</Run><Run FontSize=\"22pt\">def</Run><Run>ghi</Run></Span></Paragraph>", "^a^c^v^v", "abcdefghiabcdefghi\r\n\r\n", "", 0, 2, true, 0));

            //Regression case - Regression_Bug531
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug531", "", "a{Enter}bcd{Enter}f^a{Enter}", "\r\n\r\n", "", 0, 2, true, 0));

            //Regression case - Regression_Bug532
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug532", "", "a^a", "a\r\n", "a\r\n", 1, 1, true, 0));

            //Regression case - Regression_Bug533
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug533", "<Paragraph><Span FontStyle=\"Italic\"><Run>abc</Run><Run FontWeight=\"Bold\">d</Run><Run>efg</Run></Span></Paragraph>", "^{HOME}{RIGHT 3}{DELETE}", "abcefg\r\n", "", 0, 1, true, 0));

            // Regression case - Regression_Bug534
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug534", "", "abc{enter}defg{UP}{LEFT 3}{DOWN}+{RIGHT}", "abc\r\ndefg\r\n", "d", 0, 2, true, 0));

            //  Regression case - Regression_Bug535
            TestDataArayList.Add(new RichEditingData(" Regression case - Regression_Bug535", "", "Line1{ENTER}Line2{ENTER}Line3^{END}+{UP 3}", "Line1\r\nLine2\r\nLine3\r\n", "Line1\r\nLine2\r\nLine3", 3, 3, true, 0));

            // Regression case - Regression_Bug152
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug152", "", "abc{ENTER}def{ENTER}ghi^a{UP}+{RIGHT}", "abc\r\ndef\r\nghi\r\n", "a", 0, 3, true, 0));

            // Regression case - Regression_Bug536
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug536", "", "abc^a{delete}", "", "", 0, 0, true, 0));

            // Regression case - Regression_Bug537
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug537", "<Paragraph>p1</Paragraph><Paragraph>p2</Paragraph>", "^{HOME}{END}{DOWN}+{LEFT}", "p1\r\np2\r\n", "2", 0, 2, true, 0));

            // Regression case - Regression_Bug538
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug538", "", "a{LEFT}{ENTER}{BACKSPACE}", "a\r\n", "", 0, 1, true, 0));

            // Regression case - Regression_Bug539
            //No paragraph should be in the xaml too.
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug539", "", "a+{ENTER}", "a\r\n\r\n", "", 0, 1, true, 0));

            //Regression case - Regression_Bug540
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug540", "", "a^bb^bc^z^z", "a\r\n", "", 0, 1, true, 0));
            
            //Regression case - Regression_Bug541
            TestDataArayList.Add(new RichEditingData("Regression case - Regression_Bug541", "", "a^a^]^]^]^]^]^]^]^]{RIGHT}{ENTER}", "a\r\n\r\n", "", 0, 2, true, 0));

            StartCasesRunning();
        }

        /// <summary>P1 case</summary>
        [TestCase(LocalCaseStatus.UnderDevelopment, CasePriority.p1, "P1 cases for paragraph Editing")]
        public void BasicP1_Actions()
        {
            EnterFunction("BasicP1_Actions");
            
            if (TestDataArayList == null)
                TestDataArayList = new ArrayList();
            
            TestDataArayList.Clear();
            
            #region shift+{BackSpace}
            
            // shift-backspace with selection cross paragraph
            TestDataArayList.Add(new RichEditingData("shift-backspace with selection cross paragraph", "", "ab{enter}cd{left}+{left 3}+{backspace}", "ad\r\n", "", 0, 1, true, 1));

            //shift-backspace to delete empty paragraph at the end of the document
            TestDataArayList.Add(new RichEditingData("shift-backspace to delete empty paragraph at the end of the document", "", "ab{enter}+{backspace}", "ab\r\n", "", 0, 1, true, 1));

            //shift-backspace at the beginning of a paragraph to merge to the previous paragraph
            TestDataArayList.Add(new RichEditingData("shift-backspace at the beginning of a paragraph to merge to the previous paragraph", "", "{enter}ab{Left 2}+{backspace}", "ab\r\n", "", 0, 1, true, 1));

            //Shift-backspace to delete the paragraph in the middle of a document.
            TestDataArayList.Add(new RichEditingData("Shift-backspace to delete the paragraph in the middle of a document.", "", "{ENTER 2}{LEFT}+{BACKSPACE}", "\r\n\r\n", "", 0, 2, true, 1));
            
            #endregion

            #region Ctrl+{backspace}
            // ctrl-backspace with selection cross paragraph
            TestDataArayList.Add(new RichEditingData("ctrl-backspace with selection cross paragraph", "", "ab{enter}cd{left}+{left 3}^{backspace}", "ad\r\n", "", 0, 1, true, 1));

            //ctrl-backspace to delete empty paragraph at the end of the document
            TestDataArayList.Add(new RichEditingData("ctrl-backspace to delete empty paragraph at the end of the document", "", "ab{enter}^{backspace}", "ab\r\n", "", 0, 1, true, 1));

            //ctrl-backspace at the beginning of a paragraph to merge to the previous paragraph
            TestDataArayList.Add(new RichEditingData("ctrl-backspace at the beginning of a paragraph to merge to the previous paragraph", "", "{enter}ab{Left 2}^{backspace}", "ab\r\n", "", 0, 1, true, 1));

            //ctrl-backspace to delete the paragraph in the middle of a document.
            TestDataArayList.Add(new RichEditingData("ctrl-backspace to delete the paragraph in the middle of a document.", "", "{ENTER 2}{LEFT}^{BACKSPACE}", "\r\n\r\n", "", 0, 2, true, 1));
            #endregion
            
            #region shift+{delete}
            
            //shift-delete with selection cross paragraph
            TestDataArayList.Add(new RichEditingData("shift-delete with selection cross paragraph", "", "ab{enter}cd{left}+{left 3}+{delete}", "ad\r\n", "", 0, 1, true, 1));

            //shift-delete to delete empty paragraph at the end of the document
            //note shift delete won't delet anything if there is nothing selected.
            TestDataArayList.Add(new RichEditingData("shift-delete to delete empty paragraph at the end of the document", "", "ab{enter}{Left}+{delete}", "ab\r\n\r\n", "", 0, 2, true, 1));

            //shift-delete at the beginning of a paragraph to merge to the previous paragraph
            //note shift delete won't delet anything if there is nothing selected.
            TestDataArayList.Add(new RichEditingData("shift-delete at the beginning of a paragraph to merge to the previous paragraph", "", "{enter}ab+{delete}", "\r\nab\r\n", "", 0, 2, true, 1));

            //shift-delete to delete the paragraph in the middle of a document.
            //note shift delete won't delet anything if there is nothing selected.
            TestDataArayList.Add(new RichEditingData("shift-delete to delete the paragraph in the middle of a document.", "", "{ENTER 2}{LEFT}+{delete}", "\r\n\r\n\r\n", "", 0, 3, true, 1));
            
            #endregion
            
            #region Ctrl-Delete
            
            //Ctrl-delete with selection cross paragraph
            TestDataArayList.Add(new RichEditingData("Ctrl-delete with selection cross paragraph", "", "ab{enter}cd{left}+{left 3}^{delete}", "ad\r\n", "", 0, 1, true, 1));

            //Ctrl-delete to delete empty paragraph at the end of the document
            TestDataArayList.Add(new RichEditingData("Ctrl-delete to delete empty paragraph at the end of the document", "", "ab{enter}{left}^{delete}", "ab\r\n", "", 0, 1, true, 1));

            //Ctrl-delete at the beginning of a paragraph to merge to the previous paragraph
            TestDataArayList.Add(new RichEditingData("Ctrl-delete at the beginning of a paragraph to merge to the previous paragraph", "", "{enter}ab{left 3}^{delete}", "ab\r\n", "", 0, 1, true, 1));

            //Ctrl-delete to delete the paragraph in the middle of a document.
            TestDataArayList.Add(new RichEditingData("Ctrl-delete to delete the paragraph in the middle of a document.", "", "{ENTER 2}{LEFT}^{delete}", "\r\n\r\n", "", 0, 2, true, 1));
            
            #endregion
            
            StartCasesRunning();
            EndFunction();
        }
    }

    /// <summary>This class contains boundary cases </summary>
    [TestOwner("Microsoft"), TestBugs(""), TestTactics("693"), TestWorkItem("147, 148")]
    [Test(1, "RichEditing", "ParagraphEditngBoundary", MethodParameters = "/TestCaseType=ParagraphEditngBoundary /Case=BoundaryCases /Priority:1")]
    public class ParagraphEditngBoundary : RichEditingBase
    {
        /// <summary>
        /// BoundaryCases. This cases should be run indivitually since it take longer time. So I don't set the attribute to the method.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "Boundary cases for paragraph editing")]
        public void BoundaryCases()
        {
            //This case need to be run individually since it takes long time. No TestCase Attribute is needed.

            //create 1001 empty paragraph.
            TestDataArayList = new System.Collections.ArrayList();
            TestDataArayList.Add(new RichEditingData("Ctrl-delete to delete the paragraph in the middle of a document.", "", RepeatString("{ENTER}", 1001), RepeatString("\r\n", 1002), "", 0, 1002, true, 1));

            //create a paragraph contains 10k
            TestDataArayList.Add(new RichEditingData("Ctrl-delete to delete the paragraph in the middle of a document.", "<Paragraph>" + RepeatString("aaaaaaaaa", 1024) + "</Paragraph>", "", RepeatString("aaaaaaaaa", 1024) + "\r\n", "", 0, 1, true, 1));
            StartCasesRunning();
        }
    }
}
