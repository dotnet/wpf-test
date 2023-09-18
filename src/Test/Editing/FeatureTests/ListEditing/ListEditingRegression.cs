// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 22 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/Text/BVT/RichText/ParagraphEditingTestWithKeyboard.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using AvalonTools.FrameworkUtils;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Markup;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Collections;
    using System.Windows;
    using System.Threading; using System.Windows.Threading;


    #endregion Namespaces.

    /// <summary>Contains cases for List editing</summary>
    [TestOwner("Microsoft"), TestBugs(""), TestTactics("")]
    public class ListEditingRegression : RichEditingBase
    {
        #region regression case - Regression_Bug90
        
        /// <summary>Regression_Bug90</summary>
        [TestCase(AvalonEditingTest.LocalCaseStatus.Broken, "Regression Test for Regression_Bug90")]
        public void Regression_Bug90()
        {
            EnterFunction("Regression_Bug90");
            SetInitValue("<List Marker=\"Circle\"><ListElementItem FontSize=\"30\">First Line</ListElementItem><ListElementItem FontSize=\"30\">Second Line <List Marker=\"Disc\"><ListElementItem FontSize=\"30\">Third Line</ListElementItem><ListElementItem FontSize=\"30\">Forth Line</ListElementItem></List></ListElementItem><ListElementItem FontSize=\"30\">Fifth Line</ListElementItem></List>");
            EndFunction();
            QueueDelegate(Regression_Bug90_SetCaret);
        }

        void Regression_Bug90_SetCaret()
        {
            EnterFunction("Regression_Bug90_SetCaret");
            TextPointer na = TextControlWraper.Start;
            CheckRichedEditingResults("First Line\r\nSecond LineThird Line\r\nForth Line\r\nFifth Line\r\n", "First Line\r\nSecond LineThird Line\r\nForth Line\r\nFifth Line\r\n", 0, TextControlWraper);
            na = na.GetNextInsertionPosition(LogicalDirection.Forward);
            Rect rect = TextControlWraper.GetGlobalCharacterRect(na, 2);
            MouseInput.MouseClick((int)rect.X, (int)rect.Y);
            EndFunction();
            QueueDelegate(Regression_Bug90_DoCtrl_Down);
        }

        void Regression_Bug90_DoCtrl_Down()
        {
            EnterFunction("Regression_Bug90_DoCtrl_Down");
            CheckRichedEditingResults("First Line\r\nSecond LineThird Line\r\nForth Line\r\nFifth Line\r\n", "", 0, TextControlWraper);
            KeyboardInput.TypeString("^{down 6}{End}+{LEFT 10}");
            EndFunction();
            QueueDelegate(Regression_Bug90_Done);
        }

        void Regression_Bug90_Done()
        {
            EnterFunction("Regression_Bug90_Done");
            CheckRichedEditingResults("First Line\r\nSecond LineThird Line\r\nForth Line\r\nFifth Line\r\n", "Fifth Line", 0, TextControlWraper);
            //QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion

        #region Regression case - Regression_Bug89

        /// <summary>Regression_Bug89</summary>
        [TestCase(AvalonEditingTest.LocalCaseStatus.Broken, "Regression Test for Regression_Bug89")]
        public void Regression_Bug89()
        {
            EnterFunction("Regression_Bug89");
            SetInitValue("<List><ListElementItem>Item1</ListElementItem><List><ListElementItem>Item2</ListElementItem></List></List>");
            EndFunction();
            QueueDelegate(Regression_Bug430_DoSelectAll);

        }

        void Regression_Bug430_DoSelectAll()
        {
            EnterFunction("Regression_Bug430_DoSelectAll");
            CheckRichedEditingResults("Item1\r\nItem2\r\n", "Item1\r\nItem2\r\n", 0, TextControlWraper);
            KeyboardInput.TypeString("^a{Delete}");
            EndFunction();
            QueueDelegate(Regression_Bug89_Done);
        }

        void Regression_Bug89_Done()
        {
            EnterFunction("Regression_Bug89_Done");
            CheckRichedEditingResults("\r\n", "", 0, TextControlWraper);
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion
        
        #region Regression case - Regression_Bug87
        /// <summary>Regression_Bug87</summary>
        [TestCase(AvalonEditingTest.LocalCaseStatus.Broken, "Regression Test for Regression_Bug87")]
        public void Regression_Bug87()
        {
            EnterFunction("Regression_Bug87");
            SetInitValue("<List><ListElementItem>Quickly the fox jumped over the fence.<List><ListElementItem>List item 1.</ListElementItem><ListElementItem><Bold>List item 1.</Bold></ListElementItem></List></ListElementItem></List>");
            EndFunction();
            QueueDelegate(Regression_Bug87_DoMouseclick);
        }

        void Regression_Bug87_DoMouseclick()
        {
            EnterFunction("Regression_Bug87_DoMouseclick");
            CheckRichedEditingResults("Quickly the fox jumped over the fence.List item 1.\r\nList item 1.\r\n", "Quickly the fox jumped over the fence.List item 1.\r\nList item 1.\r\n", 0, TextControlWraper);
            TextPointer na = TextControlWraper.SelectionInstance.Start;
            Rect rect = TextControlWraper.GetGlobalCharacterRect(na, 48);
            MouseInput.MouseClick((int)rect.X, (int)(rect.Y + rect.Height / 2));
            EndFunction();
            QueueDelegate(Regression_Bug87_Do_Shift_End);
        }
        
        void Regression_Bug87_Do_Shift_End()
        {
            EnterFunction("Regression_Bug87_Do_Shift_End");
            CheckRichedEditingResults("Quickly the fox jumped over the fence.List item 1.\r\nList item 1.\r\n", "", 0, TextControlWraper);
            KeyboardInput.TypeString("+{End}");
            EndFunction();
            QueueDelegate(Regression_Bug87_Done);
        }

        void Regression_Bug87_Done()
        {
            EnterFunction("Regression_Bug87_Done");
            CheckRichedEditingResults("Quickly the fox jumped over the fence.List item 1.\r\nList item 1.\r\n", "item 1.\r\n", 0, TextControlWraper);
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion

        #region  Regression case - Regression_Bug88
        /// <summary>Regression_Bug88</summary>
        [TestCase(AvalonEditingTest.LocalCaseStatus.Broken, "Regression Test for Regression_Bug88")]
        public void Regression_Bug88()
        {
            EnterFunction("Regression_Bug88");

            SetInitValue("<List><ListElementItem>Item1</ListElementItem><ListElementItem>Item2</ListElementItem><ListElementItem>Item3</ListElementItem></List>");
            EndFunction();
            QueueDelegate(Regression_Bug88_DoSelection);
        }
        
        void Regression_Bug88_DoSelection()
        {
            EnterFunction("Regression_Bug88_DoSelection");
            CheckRichedEditingResults("Item1\r\nItem2\r\nItem3\r\n", "Item1\r\nItem2\r\nItem3\r\n", 0, TextControlWraper);
            KeyboardInput.TypeString("^{END}+{UP 2}+{RIGHT}");
            QueueDelegate(Regression_Bug88_DoCtrl_M);
            EndFunction();
        }
        
        void Regression_Bug88_DoCtrl_M()
        {
            CheckRichedEditingResults("Item1\r\nItem2\r\nItem3\r\n", "Item2\r\nItem3", 0, TextControlWraper);
            KeyboardInput.TypeString("^M");
            QueueDelegate(Regression_Bug88_Done);
        }

        void Regression_Bug88_Done()
        {
            EnterFunction("Regression_Bug88_Done");
            CheckRichedEditingResults("Item1Item2\r\nItem3\r\n", "Item2\r\nItem3", 0, TextControlWraper);
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion
    }
}
