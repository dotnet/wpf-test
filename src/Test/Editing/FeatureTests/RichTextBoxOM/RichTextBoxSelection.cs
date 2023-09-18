// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;    
    using System.Windows.Markup;    
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// This class intend to test the RichTest OM.
    /// </summary>
    [Test(0, "RichTextBox", "RichTextBoxSelection", MethodParameters = "/TestCaseType=RichTextBoxSelection", Timeout = 200)]
    [TestOwner("Microsoft"), TestBugs("179"), TestTactics("676"), TestWorkItem("137")]
    public class RichTextBoxSelection : RichEditingBase
    {
        /// <summary>
        /// Test the Selection property.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, "Test case for RichTextBox.Selection")]
        public void RichTextBoxAPI_Selection()
        {
            FailedIf(null == TextControlWraper.Element as RichTextBox, CurrentFunction + " - Failed: RichTextBox can't be found");
            
            //append text. When Selection is empty, Inserted text will not be contained in the selection. 
           // TextControlWraper.SelectionInstance.End.GetInsertionPosition(LogicalDirection.Backward).InsertText("Text");
            //FailedIf(TextControlWraper.GetSelectedText(false, false) != "", CurrentFunction + " - Failed: Selection contains wrong text: expected[], Actual[" + TextControlWraper.GetSelectedText(false, false) + "]");
            
            //append text with new lines.
            //TextControlWraper.SelectionInstance.End.InsertText("First Line\r\nSecond line");
            //FailedIf(TextControlWraper.GetSelectedText(false, false) != "", CurrentFunction + " - Failed: Selection contains wrong text: expected[], Actual[" + TextControlWraper.GetSelectedText(false, false) + "]");
            
            //clean text.
            TextControlWraper.SelectionInstance.Text = "";
            FailedIf(TextControlWraper.GetSelectedText(false, false) != "", CurrentFunction + " - Failed: Selection contains wrong text: expected[], Actual[" + TextControlWraper.GetSelectedText(false, false) + "]");
     
            //set text
            TextControlWraper.SelectionInstance.Text = "abc";
            FailedIf(TextControlWraper.GetSelectedText(false, false) != "abc", CurrentFunction + " - Failed: Selection contains wrong text: expected[abc], Actual[" + TextControlWraper.GetSelectedText(false, false) + "]");

            //Append Text when Selection contains some text
            //TextControlWraper.SelectionInstance.End.InsertText("Text");
            //FailedIf(TextControlWraper.GetSelectedText(false, false) != "abcText", CurrentFunction + " - Failed: Selection contains wrong text: expected[abcText], Actual[" + TextControlWraper.GetSelectedText(false, false) + "]");

            XamlUtils.TextRange_SetXml(TextControlWraper.SelectionInstance, "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\"><Paragraph>def</Paragraph></Section>");
            FailedIf(TextControlWraper.GetSelectedText(false, false) != "def\r\n", CurrentFunction + " - Failed: Selection contains wrong text: expected[def], Actual[" + TextControlWraper.GetSelectedText(false, false) + "]");
            //type over selection
            KeyboardInput.TypeString("a{enter}b{enter}^a");
            QueueDelegate(Selection_ContainsParagraph);
        }
        void Selection_ContainsParagraph()
        {
            FailedIf(Occurency(XamlUtils.TextRange_GetXml(TextControlWraper.SelectionInstance), "Paragraph") == 3, CurrentFunction + " - Failed: Expected 3 paragraph tags!!! Please examine the xaml[" + XamlUtils.TextRange_GetXml(TextControlWraper.SelectionInstance) + "]");
            QueueDelegate(EndTest);
        }
        /// <summary>
        /// Test the Start and End properties of the RichTextbox.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, "Test case for RichTextBox.Document.ContentStart and End properties")]
        public void RichTextBoxAPI_Start_End()
        {
            TextRange range = new TextRange(((RichTextBox)TextControlWraper.Element).Document.ContentStart, ((RichTextBox)TextControlWraper.Element).Document.ContentEnd);
            range.Text = "abc";
            string str = ((RichTextBox)TextControlWraper.Element).Document.ContentStart.GetTextInRun(LogicalDirection.Forward);
            //FailedIf(((RichTextBox)TextControlWraper.Element).ContentStart.GetTextInRun(LogicalDirection.Forward) != "abc", CurrentFunction + " - Failed: Selection contains wrong text: expected[abc], Actual[" + ((RichTextBox)TextControlWraper.Element).ContentStart.GetTextInRun(LogicalDirection.Forward) + "]");
            //FailedIf(((RichTextBox)TextControlWraper.Element).ContentStart.GetTextInRun(LogicalDirection.Backward) != "", CurrentFunction + " - Failed: Selection contains wrong text: expected[], Actual[" + ((RichTextBox)TextControlWraper.Element).ContentStart.GetTextInRun(LogicalDirection.Backward) + "]");
            //FailedIf(((RichTextBox)TextControlWraper.Element).ContentEnd.GetTextInRun(LogicalDirection.Forward) != "", CurrentFunction + " - Failed: Selection contains wrong text: expected[], Actual[" + ((RichTextBox)TextControlWraper.Element).ContentEnd.GetTextInRun(LogicalDirection.Forward) + "]");
            //FailedIf(((RichTextBox)TextControlWraper.Element).ContentEnd.GetTextInRun(LogicalDirection.Backward) != "abc", CurrentFunction + " - Failed: Selection contains wrong text: expected[abc], Actual[" + ((RichTextBox)TextControlWraper.Element).ContentEnd.GetTextInRun(LogicalDirection.Backward) + "]");
            range.Text = "";
            KeyboardInput.TypeString("def");
            QueueDelegate(Property_Start_End);
        }
        
        void Property_Start_End()
        {
            TextRange range = new TextRange(((RichTextBox)TextControlWraper.Element).Document.ContentStart, ((RichTextBox)TextControlWraper.Element).Document.ContentEnd);
            range.Text = "def";
            //FailedIf(((RichTextBox)TextControlWraper.Element).ContentStart.GetTextInRun(LogicalDirection.Forward) != "def", CurrentFunction + " - Failed: Selection contains wrong text: expected[def], Actual[" + ((RichTextBox)TextControlWraper.Element).ContentStart.GetTextInRun(LogicalDirection.Forward) + "]");
            //FailedIf(((RichTextBox)TextControlWraper.Element).ContentStart.GetTextInRun(LogicalDirection.Backward) != "", CurrentFunction + " - Failed: Selection contains wrong text: expected[], Actual[" + ((RichTextBox)TextControlWraper.Element).ContentStart.GetTextInRun(LogicalDirection.Backward) + "]");
            //FailedIf(((RichTextBox)TextControlWraper.Element).ContentEnd.GetTextInRun(LogicalDirection.Forward) != "", CurrentFunction + " - Failed: Selection contains wrong text: expected[], Actual[" + ((RichTextBox)TextControlWraper.Element).ContentEnd.GetTextInRun(LogicalDirection.Forward) + "]");
            //FailedIf(((RichTextBox)TextControlWraper.Element).ContentEnd.GetTextInRun(LogicalDirection.Backward) != "def", CurrentFunction + " - Failed: Selection contains wrong text: expected[def], Actual[" + ((RichTextBox)TextControlWraper.Element).ContentEnd.GetTextInRun(LogicalDirection.Backward) + "]");
            //Regression test For Regression_Bug179, If no exception, case will pass.
            range.ApplyPropertyValue(Block.TextAlignmentProperty, System.Windows.TextAlignment.Center);

            QueueDelegate(EndTest);
        }
    }

    /// <summary>Test The default selection</summary>
    [Test(0, "RichTextBox", "DefaultSelection", MethodParameters = "/TestCaseType=DefaultSelection Priority=0", Timeout=200)]
    [TestOwner("Microsoft"), TestBugs("674"), TestTactics("677"), TestWorkItem("137")]
    public class DefaultSelection : RichEditingBase
    {
        #region regression case - Regression_Bug674. no default text selected in RichTextBox.
        /// <summary>Default text in RichTextBox should not be selected</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "Default Selection should not contain any texted")]
        public void DefaultTextSelection()
        {
            string xaml = "<RichTextBox xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><FlowDocument><Paragraph>abcdefg</Paragraph></FlowDocument></RichTextBox>";

            MainWindow.Content = XamlUtils.ParseToObject(xaml);
            CheckRichedEditingResults("abcdefg\r\n", "", 0, 1, new Test.Uis.Wrappers.UIElementWrapper(MainWindow.Content as UIElement));
            QueueDelegate(EndTest);
        }
        #endregion
    }
}
