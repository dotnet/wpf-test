// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 21 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/ExeTarget/EntryPoint.cs $")]

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
    using Microsoft.Test.Imaging;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// This class will test using mouse for pargraphEditing.
    /// </summary>
    [Test(0, "RichEditing", "ParagraphEditingWithMouse1", MethodParameters = "/TestCaseType=ParagraphEditingWithMouse /Case=MouseToSetCaretAndSelectText")]
    [Test(0, "RichEditing", "ParagraphEditingWithMouse2", MethodParameters = "/TestCaseType=ParagraphEditingWithMouse /Case=RunAllCases", Keywords = "MicroSuite")]
    [TestOwner("Microsoft"), TestTactics("690,691"), TestBugs("320, 828, 306, 315, 336, 337, 338, 108"), TestWorkItem("")]
    public class ParagraphEditingWithMouse : RichEditingBase
    {
        string _alignment = "left";

        #region case - MouseToSetCaretAndSelectText.
        /// <summary>use mouse to set caret, make selection, de-selection etc in diferent justfications</summary>
        [TestCase(LocalCaseStatus.Ready, "MouseToSetCaretAndSelectText")]
            public void MouseToSetCaretAndSelectText()
        {        
            EnterFuction("MouseToSetCaretAndSelectText");

            //we need to type enter for the paragraph to be applied21
            KeyboardInput.TypeString("ab{enter}c");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(MouseClickAtTheEndOfFirstLine));
        }

        void MouseClickAtTheEndOfFirstLine()
        {
            EnterFuction("MouseClickAtTheEndOfFirstLine");
            Sleep();

            TextPointer start = TextControlWraper.Start;
            Rect rec = TextControlWraper.GetGlobalCharacterRect(start, 6);

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
            if (TextControlWraper.SelectionInstance.Text != "b")
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: Selection should be: [b], Actual: [" + TextControlWraper.SelectionInstance.Text + "]");
            }

            TextPointer start = TextControlWraper.End;
            
            start = start.GetInsertionPosition(LogicalDirection.Backward);

            Rect rec = TextControlWraper.GetGlobalCharacterRect(start, 0);

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
            if (TextControlWraper.SelectionInstance.Text != "c")
            {
                MyLogger.Log(CurrentFunction + " - Failed: Selection should be: [c], Actual: [" + TextControlWraper.SelectionInstance.Text + "]");
                pass = false;
            }

            TextPointer start = TextControlWraper.Start;
            TextPointerContext type = start.GetPointerContext(LogicalDirection.Forward);

            if (type != TextPointerContext.Text)
            {
                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }

            Rect rec = TextControlWraper.GetGlobalCharacterRect(start, 1);

            MouseInput.MouseClick((int)rec.Left, (int)((rec.Top + rec.Bottom) / 2));
            EndFunction();
            if (pass)
            {
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(MakeAKeyboardSelectionForA));
            }
            else
            {
                QueueDelegate(EndTest);
            }
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
            if (TextControlWraper.SelectionInstance.Text != "a")
            {
                MyLogger.Log(CurrentFunction + " - Failed: Selection should be: [a], Actual: [" + TextControlWraper.SelectionInstance.Text + "]");
                pass = false;
            }

            TextPointer start = TextControlWraper.Start;

            MyLogger.Log("text: " + start.GetTextInRun(LogicalDirection.Forward));
            Rect rec = TextControlWraper.GetGlobalCharacterRect(start, 8);

            MouseInput.MouseClick((int)rec.Left, (int)((rec.Top + rec.Bottom) / 2));
            EndFunction();
            if (pass)
            {
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(MakeAKeyboardSelectionForCFromLeft));
            }
            else
            {
                QueueDelegate(EndTest);
            }
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
            if (TextControlWraper.SelectionInstance.Text != "c")
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: Selection should be: [c], Actual: [" + TextControlWraper.SelectionInstance.Text + "]");
            }

            TextPointer start = TextControlWraper.Start;
            start = start.GetNextInsertionPosition(LogicalDirection.Forward);
            Rect rec1 = TextControlWraper.GetGlobalCharacterRect(start, 0);
            TextPointer end = TextControlWraper.End;
            Rect rec2 = TextControlWraper.GetGlobalCharacterRect(end, 0);

            MouseInput.MouseDragPressed(new Point(rec1.Left, rec1.Top), new Point(rec2.Right, rec2.Bottom));
            EndFunction();
            if (pass)
            {
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(IsAllTextSelectedByMouse));
            }
            else
            {
                QueueDelegate(EndTest);
            }
        }

        void IsAllTextSelectedByMouse()
        {
            EnterFuction("IsAllTextSelectedByMouse");
            Sleep();
            if (TextControlWraper.SelectionInstance.Text != "ab\r\nc")
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: Selection should be: [ab\r\nc\r\n], Actual: [" + TextControlWraper.SelectionInstance.Text + "]");
            }
            MouseInput.MouseClick((UIElement)MainWindow.Content);
            EndFunction();
            if (pass)
            {
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifierNoTextSelected));
            }
            else
            {
                QueueDelegate(EndTest);
            }
        }

        void VerifierNoTextSelected()
        {
            EnterFuction("VerifierNoTextSelected");
            if (TextControlWraper.SelectionInstance.Text != "")
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: No text should be selected, Actual: [" + TextControlWraper.SelectionInstance.Text + "]");
            }

            TextPointer start = TextControlWraper.Start;
            Rect rec1 = TextControlWraper.GetGlobalCharacterRect(start, 2);
            Rect rec2 = TextControlWraper.GetGlobalCharacterRect(start, 4);

            MouseInput.MouseDragPressed(new Point(rec1.Left, rec1.Top), new Point(rec2.Right, (rec2.Bottom + rec2.Top)/2));
            EndFunction();
            if (pass)
            {
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(IsabSelectedByMouse));
            }
            else
            {
                QueueDelegate(EndTest);
            }
        }

        void IsabSelectedByMouse()
        {
            EnterFuction("IsabSelectedByMouse");
            Sleep();
            if (TextControlWraper.SelectionInstance.Text != "ab")
            {
                MyLogger.Log(CurrentFunction + " - Failed: Selection should be: [ab], Actual: [" + TextControlWraper.SelectionInstance.Text + "]");
                pass = false;
                QueueDelegate(EndTest);
                return;
            }
            switch (_alignment)
            {
                case "left":
                    ((RichTextBox)MainWindow.Content).Document.TextAlignment = System.Windows.TextAlignment.Center;
                    _alignment = "center";
                    break;

                case "center":
                    ((RichTextBox)MainWindow.Content).Document.TextAlignment = System.Windows.TextAlignment.Right;
                    _alignment = "right";
                    break;

                case "right":
                    _alignment = "end";
                    break;
            }
            if (_alignment != "end")
            {
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(MouseClickAtTheEndOfFirstLine);
            }
            else
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(EndTest);

            EndFunction();
        }
        #endregion
        
        #region Regression case - Regression_Bug320
        /// <summary>Regression_Bug320</summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug320")]
        public void Regression_Bug320()
        {
            EnterFunction("Regression_Bug320");
            ((Control)MainWindow.Content).HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            SetInitValue("<Paragraph FontSize=\"48px\">abc</Paragraph>");
            EndFunction();
            QueueDelegate(Regression_Bug319_DoMouseClick);
        }

        void Regression_Bug319_DoMouseClick()
        {
            EnterFunction("Regression_Bug319_DoMouseClick");
            TextPointer start = TextControlWraper.SelectionInstance.Start;
            start = start.GetNextInsertionPosition(LogicalDirection.Forward);
            Rect rec1 = TextControlWraper.GetGlobalCharacterRect(start, 0);
            MouseInput.MouseClick((int)rec1.Left, (int)(rec1.Y + rec1.Height / 2));
            EndFunction();
            QueueDelegate(Regression_Bug319_Done);
        }

        void Regression_Bug319_Done()
        {
            EnterFunction("Regression_Bug319_Done");
            CheckRichedEditingResults("abc\r\n", "", 0, TextControlWraper);
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion
        
        #region Regression case - Regression_Bug306
        /// <summary></summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug306")]
        public void Regression_Bug306()
        {
            EnterFunction("Regression_Bug306_CaseStart");
            ((Control)MainWindow.Content).HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            SetInitValue("");
            EndFunction();
            QueueDelegate(Regression_Bug306_TypeContent);
        }

        void Regression_Bug306_TypeContent()
        {
            EnterFunction("Regression_Bug306_TypeContent");
            KeyboardInput.TypeString("{home}This is first line{Enter} This is second line");
            EndFunction();
            QueueDelegate(Regression_Bug306_MakeASelection);
        }

        void Regression_Bug306_MakeASelection()
        {
            EnterFunction("Regression_Bug306_MakeASelection");
            CheckRichedEditingResults("This is first line\r\n This is second line\r\n", "", 0, TextControlWraper);
            TextPointer start = TextControlWraper.Start;
            Rect rect = TextControlWraper.GetGlobalCharacterRect(start, 35);

            MouseInput.MouseClick((int)rect.Left, (int)(rect.Top + rect.Height / 2));
            MouseInput.MouseClick((int)rect.Left, (int)(rect.Top + rect.Height / 2));
            QueueDelegate(Regression_Bug306_DoDragDrop);
            EndFunction();
        }

        void Regression_Bug306_DoDragDrop()
        {
            EnterFunction("Regression_Bug306_DoDragDrop");
            //turn off the inputminitor
            Test.Uis.Utils.InputMonitorManager.Current.IsEnabled = false;
            CheckRichedEditingResults("This is first line\r\n This is second line\r\n", "second ", 0, TextControlWraper);
            TextPointer start = TextControlWraper.Start;
            //start.MoveToNextInsertionPosition(LogicalDirection.Forward);
            start = start.GetPositionAtOffset(1);
            
            Rect rect1 = TextControlWraper.GetGlobalCharacterRect(start, 0);
            Rect rect2 = TextControlWraper.GetGlobalCharacterRect(start, 35);

            //drag the selection to the front of the second line. note: the end point is calculated from the first character of the first line.
            MouseInput.MouseDragPressed(new Point((int)rect2.Left, (int)(rect2.Top + rect2.Height / 2)), new Point((int)rect1.Left - 3, (int)(rect1.Top + (4 * rect1.Height) / 2)));
            EndFunction();
            QueueDelegate(Regression_Bug306_Done);
        }

        void Regression_Bug306_Done()
        {
            EnterFunction("Regression_Bug306_Done");
            //turn on the input manager.
            Test.Uis.Utils.InputMonitorManager.Current.IsEnabled = true;

            CheckRichedEditingResults("This is first line\r\nsecond  This is line\r\n", "second ", 0, TextControlWraper);
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion
        
        #region Regression case - Regression_Bug315
        /// <summary>Regression_Bug315</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1,  "Regression Test for Regression_Bug315")]
        public void Regression_Bug315()
        {
            EnterFunction("Regression_Bug315");
            ((Control)MainWindow.Content).HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            SetInitValue("<Paragraph>abc</Paragraph>");
            EndFunction();
            QueueDelegate(Regression_Bug315_Done);
        }

        void Regression_Bug315_Done()
        {
            TextPointer start = TextControlWraper.Start;
            Rect rect1 = TextControlWraper.GetGlobalCharacterRect(start, 1);
            MouseInput.MouseMove((int)rect1.Left, (int)rect1.Top);
            Point p = ElementUtils.GetScreenRelativePoint(TextControlWraper.Element, new Point(0, 0));
            //this evaluation may not be 100% correct. need to be fixed as the pargraph feautre is fully implemented for editing.
            if (rect1.Left != 7 + (int)p.X + (int)((Control)(TextControlWraper.Element)).Margin.Left + (int)((Control)(TextControlWraper.Element)).BorderThickness.Left)
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - Failed: the default of Paragraph indent may be changed!!! Please double check it.");
            }
            QueueDelegate(EndTest);
        }
        #endregion

        #region Regression case - Regression_Bug316
        /// <summary>Regression_Bug316</summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug316")]
        public void Regression_Bug316()
        {
            EnterFunction("Regression_Bug316");
            SetInitValue("<Paragraph></Paragraph>");
            EndFunction();
            QueueDelegate(Regression_Bug316_ApplyFormat);
        }

        void Regression_Bug316_ApplyFormat()
        {
            EnterFunction("Regression_Bug316_ApplyFormat");
            KeyboardInput.TypeString("^U^I^Babc def+{LEFT 3}");
            EndFunction();
            QueueDelegate(Regression_Bug316_DoDragdrop);
        }

        void Regression_Bug316_DoDragdrop()
        {
            EnterFunction("Regression_Bug316_DoDragdrop");
            TextPointer start = TextControlWraper.SelectionInstance.Start;            
            start = start.GetInsertionPosition(LogicalDirection.Forward);

            Rect rect1 = TextControlWraper.GetGlobalCharacterRect(start, 5);
            Rect rect2 = TextControlWraper.GetGlobalCharacterRect(start, 1);
            Test.Uis.Utils.InputMonitorManager.Current.IsEnabled = false;
            MouseInput.MouseDragPressed(new Point((int)rect1.Left, (int)(rect1.Top + rect1.Height / 2)), new Point((int)rect2.Left, (int)(rect1.Top + rect1.Height / 2)));
            QueueDelegate(Regression_Bug316_Done);
            EndFunction();
        }

        void Regression_Bug316_Done()
        {
            EnterFunction("Regression_Bug316_Done");
            Test.Uis.Utils.InputMonitorManager.Current.IsEnabled = true;
            //no crash, then pass
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion

        #region Regression case - Regression_Bug317
        /// <summary>Regression_Bug317</summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug317")]
        public void Regression_Bug317()
        {
            EnterFunction("Regression_Bug317");
            ((FrameworkElement)TextControlWraper.Element).Width = 200;
            ((FrameworkElement)TextControlWraper.Element).Height = 100;
            SetInitValue("");
            EndFunction();
            QueueDelegate(Regression_Bug317_TypeTwoLines);
        }

        void Regression_Bug317_TypeTwoLines()
        {
            EnterFunction("Regression_Bug317_TypeTwoLines");
            EnterFunction("Regression_Bug317_TypeTwoLines");
            KeyboardInput.TypeString("Abc{ENTER}cefghijklmnopqrstuvwxyz");
            EndFunction();
            QueueDelegate(Regression_Bug317_MouseClick);
        }

        void Regression_Bug317_MouseClick()
        {
            TextPointer start;
            Rect characterRect;
            
            EnterFunction("Regression_Bug316_DoDragdrop");
            start = TextControlWraper.Start;       
            characterRect = TextControlWraper.GetGlobalCharacterRect(start, 31);
            MouseInput.MouseClick((int)characterRect.Left,
                (int)(characterRect.Top + characterRect.Height/2));
            EndFunction();
            QueueDelegate(Regression_Bug317_MakeSelection);
        }

        void Regression_Bug317_MakeSelection()
        {
            EnterFuction("Regression_Bug317_MakeSelection");
            KeyboardInput.TypeString("+{RIGHT}");
            EndFunction();
            QueueDelegate(Regression_Bug317_Done);
        }

        void Regression_Bug317_Done()
        {
            EnterFunction("Regression_Bug317_Done");
            //reset the size
            //((FrameworkElement)TextControlWraper.Element).Width = new Length(100, UnitType.Percent);
            //((FrameworkElement)TextControlWraper.Element).Height = new Length(100, UnitType.Percent);
            CheckRichedEditingResults("Abc\r\ncefghijklmnopqrstuvwxyz\r\n", "z", 0, TextControlWraper);
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion

        #region Regression case - Regression_Bug318
        /// <summary>
        /// Regression_Bug318
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug318")]
        public void Regression_Bug318()
        {
            EnterFuction("Regression_Bug318");
            Control richTextBox = Test.Uis.Utils.ReflectionUtils.CreateInstanceOfType("TextBox", null) as Control;
            MainWindow.Content = richTextBox;
            //richTextBox.Width = new Length(100, UnitType.Percent);
            //richTextBox.Height = new Length(100, UnitType.Percent);
            richTextBox.SetValue(TextElement.FontFamilyProperty, new FontFamily("Palatino Linotype"));
            richTextBox.SetValue(TextElement.FontSizeProperty, 30.0);
            TextControlWraper = new Test.Uis.Wrappers.UIElementWrapper(richTextBox);
            Test.Uis.Utils.ReflectionUtils.SetProperty(richTextBox, "AcceptsReturn", true);

            richTextBox.Focus();
            QueueDelegate(Regression_Bug318_DoActions);
            EndFunction();
        }

        void Regression_Bug318_DoActions()
        {
            EnterFunction("Regression_Bug318_DoActions");
            MouseInput.MouseClick(TextControlWraper.Element);
            KeyboardInput.TypeString("a lot of junks");
            QueueDelegate(Regression_Bug318_Done);
        }

        void Regression_Bug318_Done()
        {
            base.Init();
            QueueDelegate(EndTest);
        }
        #endregion

        #region regression case - Regression_Bug108
        /// <summary>Regression_Bug108</summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug108")]
        public void Regression_Bug108()
        {
            EnterFunction("Regression_Bug108");
            SetInitValue("<Paragraph>abc</Paragraph>");
            QueueDelegate(Regression_Bug108_SetCaret);
            EndFunction();
        }

        void Regression_Bug108_SetCaret()
        {
            EnterFunction("Regression_Bug108_SetCaret");
            KeyboardInput.TypeString("{End}{Left 2}");
            QueueDelegate(Regression_Bug108_DoMouseSelection);
            EndFunction();
        }

        void Regression_Bug108_DoMouseSelection()
        {
            EnterFunction("Regression_Bug108_DoMouseSelection");
            CheckRichedEditingResults("abc\r\n", "", 0, 1, TextControlWraper);
            Rect rec = TextControlWraper.GetGlobalCharacterRect(TextControlWraper.SelectionInstance.Start, LogicalDirection.Forward);
            MouseInput.MouseDragPressed(new Point(rec.Left, (rec.Top + rec.Bottom) / 2), new Point(rec.Right, (rec.Top + rec.Bottom) / 2));
            QueueDelegate(Regression_Bug108_Done);
            EndFunction();
        }

        void Regression_Bug108_Done()
        {
            EnterFunction("Regression_Bug108_Done");
            CheckRichedEditingResults("abc\r\n", "b", 0, 1, TextControlWraper);
            EndTest();
            EndFunction();
        }
        #endregion 
    }
}
