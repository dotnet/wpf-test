// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 21 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/TableEditing/ParagraphEditingWithMouse.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.ComponentModel;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// This class will test using mouse to make selection and set caret etc.
    /// </summary>
    [Test(0, "TableEditing", "MouseSelectionInTableCell", MethodParameters = "/TestCaseType=MouseSelectionInTableCell")]
    [TestOwner("Microsoft"), TestBugs(""), TestTactics("659"), TestWorkItem("128")]
    public class MouseSelectionInTableCell : RichEditingBase
    {
        /// <summary>Override to add table </summary>
        /// <param name="xamlstr"></param>
        protected override void SetInitValue(string xamlstr)
        {
            string OpenTags =  "<Table><TableRowGroup><TableRow><TableCell>";
            string CloseTags = "</TableCell></TableRow></TableRowGroup></Table>";
            base.SetInitValue(OpenTags + xamlstr + CloseTags);
        }

        /// <summary>use mouse to set caret, make selection, de-selection etc in diferent justfications</summary>
        [TestCase(LocalCaseStatus.Ready, "MouseToSetCaretAndSelectText")]
        public void MouseToSetCaretAndSelectText()
        {
            EnterFuction("MouseToSetCaretAndSelectText");
            base.IsEditingInsideTable = true;

            SetInitValue("<Paragraph>ab</Paragraph><Paragraph>c</Paragraph>");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(MouseClickAtTheEndOfFirstLine));
        }

        void MouseClickAtTheEndOfFirstLine()
        {
            EnterFuction("MouseClickAtTheEndOfFirstLine");
            Sleep();

            TextPointer start = TextControlWraper.Start;
            Rect rec = TextControlWraper.GetGlobalCharacterRect(PointerFromIndex(TextControlWraper, 3),LogicalDirection.Forward);
            MouseInput.MouseClick((int)rec.Right, (int)((rec.Top + rec.Bottom) / 2));
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(MakeAKeyboardSelectionForB));
        }

        void MakeAKeyboardSelectionForB()
        {
            EnterFuction("MakeAKeyboardSelectionForB");
            KeyboardInput.TypeString("+{left}");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(IsCaretAtTheEndofFirstLine));
        }

        void IsCaretAtTheEndofFirstLine()
        {
            EnterFuction("IsCaretAtTheEndofFirstLine");
            Sleep();
            CheckRichedEditingResults("ab\r\nc\r\n", "b", 0, 2, TextControlWraper);

            Rect rec = TextControlWraper.GetGlobalCharacterRect(PointerFromIndex(TextControlWraper, 5), 0);

            MouseInput.MouseClick((int)rec.Right, (int)((rec.Top + rec.Bottom) / 2));
            EndFunction();
            if (pass)
            {
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(MakeAKeyboardSelectionForC));
            }
            else
            {
                QueueDelegate(EndTest);
            }
        }

        void MakeAKeyboardSelectionForC()
        {
            EnterFuction("MakeAKeyboardSelectionForC");
            Sleep();
            KeyboardInput.TypeString("+{left}");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(IsCaretAtTheEndofSecondLine));
        }

        void IsCaretAtTheEndofSecondLine()
        {
            EnterFuction("IsCaretAtTheEndofSecondLine");
            Sleep();
            CheckRichedEditingResults("ab\r\nc\r\n", "c", 0, 2, TextControlWraper);

            Rect rec = TextControlWraper.GetGlobalCharacterRect(PointerFromIndex(TextControlWraper, 2), 0);

            MouseInput.MouseClick((int)rec.Left, (int)((rec.Top + rec.Bottom) / 2));
            EndFunction();
            if (pass)
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(MakeAKeyboardSelectionForA));
            else
                QueueDelegate(EndTest);
        }

        void MakeAKeyboardSelectionForA()
        {
            EnterFuction("MakeAKeyboardSelectionForA");
            Sleep();
            KeyboardInput.TypeString("+{RIGHT}");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(IsCaretAtTheFrontOfFirstLine));
        }

        void IsCaretAtTheFrontOfFirstLine()
        {
            EnterFuction("IsCaretAtTheFrontOfFirstLine");
            Sleep();
            CheckRichedEditingResults("ab\r\nc\r\n", "a", 0, 2, TextControlWraper);

            Rect rec = TextControlWraper.GetGlobalCharacterRect(PointerFromIndex(TextControlWraper, 5), 0);

            MouseInput.MouseClick((int)rec.Left, (int)((rec.Top + rec.Bottom) / 2));
            EndFunction();
            if (pass)
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(MakeAKeyboardSelectionForCFromLeft));
            else
                QueueDelegate(EndTest);
        }

        void MakeAKeyboardSelectionForCFromLeft()
        {
            EnterFuction("MakeAKeyboardSelectionForCFromLeft");
            Sleep();
            KeyboardInput.TypeString("+{RIGHT}");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(IsCaretAtTheFrontOfSecondLine));
        }

        void IsCaretAtTheFrontOfSecondLine()
        {
            EnterFuction("IsCaretAtTheFrontOfSecondLine");
            Sleep();
            CheckRichedEditingResults("ab\r\nc\r\n", "c", 0, 2, TextControlWraper);
            Rect rec1 = TextControlWraper.GetGlobalCharacterRect(PointerFromIndex(TextControlWraper, 2), 0);
            Rect rec2 = TextControlWraper.GetGlobalCharacterRect(PointerFromIndex(TextControlWraper, 5), 0);

            MouseInput.MouseDragPressed(new Point(rec1.Left, rec1.Top), new Point(rec2.Right - 2, rec2.Bottom-2));
            EndFunction();
            if (pass)
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(IsAllTextSelectedByMouse));
            else
                QueueDelegate(EndTest);
        }

        void IsAllTextSelectedByMouse()
        {
            EnterFuction("IsAllTextSelectedByMouse");
            Sleep();
           
            CheckRichedEditingResults("ab\r\nc\r\n", "ab\r\nc", 2, 2, TextControlWraper);
            MouseInput.MouseClick((UIElement)MainWindow.Content);
            EndFunction();
            if (pass)
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifierNoTextSelected));
            else
                QueueDelegate(EndTest);
        }

        void VerifierNoTextSelected()
        {
            EnterFuction("VerifierNoTextSelected");

            CheckRichedEditingResults("ab\r\nc\r\n", "", 0, 2, TextControlWraper);

            Rect rec1 = TextControlWraper.GetGlobalCharacterRect(PointerFromIndex(TextControlWraper, 2), 0);
            Rect rec2 = TextControlWraper.GetGlobalCharacterRect(PointerFromIndex(TextControlWraper, 3), 0);

            MouseInput.MouseDragPressed(new Point(rec1.Left, rec1.Top), new Point(rec2.Right, (rec2.Bottom + rec2.Top) / 2));
            EndFunction();
            if (pass)
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(IsabSelectedByMouse));
            else
                QueueDelegate(EndTest);
        }

        void IsabSelectedByMouse()
        {
            EnterFuction("IsabSelectedByMouse");
            Sleep();
            CheckRichedEditingResults("ab\r\nc\r\n", "ab", 0, 2, TextControlWraper);

            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(EndTest);

            EndFunction();
        }
    }
}
