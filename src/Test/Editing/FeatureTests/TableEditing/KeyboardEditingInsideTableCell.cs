// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 22 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/Text/BVT/RichText/ParagraphEditingTestWithKeyboard.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>This class contains case for keyboard editing inside a tableCell</summary>
    [Test(0, "TableEditing", "KeyBoardEditingInsideTableCell", MethodParameters = "/TestCaseType=KeyBoardEditingInsideTableCell")]
    [TestOwner("Microsoft"), TestBugs("555, 807, 808"), TestTactics("660"), TestWorkItem("127")]
    public class KeyBoardEditingInsideTableCell : RichEditingBase
    {
        /// <summary>Override to add table </summary>
        /// <param name="xamlstr"></param>
        protected override void SetInitValue(string xamlstr)
        {
            string OpenTags = "<Table><TableRowGroup><TableRow><TableCell>";
            string CloseTags = "</TableCell></TableRow></TableRowGroup></Table>";
            if ((xamlstr == string.Empty) || (xamlstr == null))
            {
                base.SetInitValue(OpenTags + "<Paragraph></Paragraph>" + CloseTags);
            }
            else
            {
                base.SetInitValue(OpenTags + xamlstr + CloseTags);
            }
        }

        /// <summary>Keyboard BVT cases </summary>
        [TestCase(LocalCaseStatus.Ready, "TypingBVTCases")]
        public void TypingInTableCellBVTCases()
        {
            base.IsEditingInsideTable = true;
            TestDataArayList = new ArrayList();
            BackSpaceData();
            DeleteData();
            Regression();
            StartCasesRunning();
            EndFunction();
        }

        private void DeleteData()
        {
            //Delete at the end of a paragraph will delete the followed paragraph.
            //Undo, Redo
            TestDataArayList.Add(new RichEditingData("DeleteKey_Actions", "", "{RIGHT}a{enter}{Left}{Delete}", "a\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("DeleteKey_Actions", "", "^z", "a\r\n\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("DeleteKey_Actions", "", "^y", "a\r\n", "", 0, 1, true, 0));//Redo

            //Delete works for empty paragraph
            //Undo, Redo
            TestDataArayList.Add(new RichEditingData("Delete an empty line", "", "{RIGHT}{enter}{Left}{Delete}", "\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("Delete an empty line", "", "^z", "\r\n\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("Delete an empty line", "", "^y", "\r\n", "", 0, 1, true, 0));//Redo

            //Do Delete should not delete the only pargraph in the doucment.
            TestDataArayList.Add(new RichEditingData("Delete key", "", "{RIGHT}a{enter}{Left}{DELETE}", "a\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("Delete key", "", "{Home}{DELETE 2}", "\r\n", "", 0, 1, true, 0));

            //Delete merge paragraphs
            //Undo/Redo
            TestDataArayList.Add(new RichEditingData("Delete merge paragraphs", "", "{RIGHT}ab{left}{enter}{LEFT}{DELETE}", "ab\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("Delete merge paragraphs", "", "^z", "a\r\nb\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("Delete merge paragraphs", "", "^y", "ab\r\n", "", 0, 1, true, 0));//Redo

            //Delete from the begining of the document.
            TestDataArayList.Add(new RichEditingData("Delete from the begining of the document", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^{RIGHT}{HOME}{DELETE}", "\r\nb\r\n", "", 0, 2, false, 0));
            TestDataArayList.Add(new RichEditingData("Delete from the begining of the document", "", "{DELETE}", "b\r\n", "", 0, 1, true, 0));

            //Delete on Selection containing a paragraph
            TestDataArayList.Add(new RichEditingData("Delete on Selection containing a paragraph", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^{Home}{RIGHT}+{Right 2}{Delete}", "b\r\n", "", 0, 1, true, 0));

            //Delete Selection that crosses Paragraph
            TestDataArayList.Add(new RichEditingData("//Delete Selection that crosses Paragraph", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph><Paragraph>c</Paragraph>", "^{Home}{RIGHT}+{Right 4}{Delete}", "c\r\n", "", 0, 1, true, 0));
        }
        private void BackSpaceData()
        {
            //Undo/redo for enter to creating paragraph
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "{RIGHT}a{enter}{backspace}", "a\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^z", "a\r\n\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^y", "a\r\n", "", 0, 1, true, 0));//Redo

            //Undo/redo, backspace to delete a paragraph
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "{RIGHT}{enter}{backspace}", "\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^z", "\r\n\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^y", "\r\n", "", 0, 1, true, 0));//Redo

            //Do backspace should not delete the only pargraph in the doucment.
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "{RIGHT}a{enter}{backspace}{HOME}{backspace 3}", "a\r\n", "", 0, 1, true, 0));

            //Bakck space merge two paragraph
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "{RIGHT}ab{left}{enter}{backspace}", "ab\r\n", "", 0, 1, false, 0));
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^z", "a\r\nb\r\n", "", 0, 2, false, 0));//Undo
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "^y", "ab\r\n", "", 0, 1, true, 0));//Redo

            //Backspace on Selection containing a paragraph.
            TestDataArayList.Add(new RichEditingData("Backspace on Selection containing a paragraph.", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>", "^{Home}{Right}+{Right 2}{backspace}", "b\r\n", "", 0, 1, true, 0));

            //Backspace on selection that cross paragraphs
            TestDataArayList.Add(new RichEditingData("Backspace on selection that cross paragraphs", "<Paragraph>a</Paragraph><Paragraph>b</Paragraph><Paragraph>c</Paragraph>", "^{Home}{RIGHT}+{Right 4}{backspace}", "c\r\n", "", 0, 1, true, 0));
        }
        private void Regression()
        {
            //Regression_Bug555
            TestDataArayList.Add(new RichEditingData("BackSpace key", "", "{RIGHT}a{backspace 3}", "\r\n", "", 0, 1, false, 0));
        }
    }
}
