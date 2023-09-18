// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    #region Using directives

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

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;

    #endregion

    /// <summary>this class contains code for testing TextChanged event.</summary>
    [Test(0, "RichTextBox", "RichTextBoxEventsTest", MethodParameters = "/TestCaseType=RichTextBoxEventsTest Priority=0")]
    [TestOwner("Microsoft"), TestBugs("670"), TestTactics("686"), TestWorkItem("145")]
    class RichTextBoxEventsTest : RichEditingBase
    {
        /// <summary>Type to trigger Textchanged event</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "Type to RichTextBox to trigger the TextChanged Event")]
        public void TypeChar()
        {
            EnterFunction("TypeChar");
            ((RichTextBox)(TextControlWraper.Element)).TextChanged += new TextChangedEventHandler(RichBox_TextChanged);
            KeyboardInput.TypeString("abc");
            EndFunction();
        }
        /// <summary>Set text to trigger Textchange event</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "set text to RichTextBox to trigger the TextChanged Event")]
        public void ChangeText()
        {
            EnterFunction("ChangeText");
            ((RichTextBox)(TextControlWraper.Element)).TextChanged += new TextChangedEventHandler(RichBox_TextChanged);
            TextControlWraper.Text="a";
            EndFunction();
        }

        void RichBox_TextChanged(object o, TextChangedEventArgs args)
        {
            EnterFunction("RichBox_TextChanged");
            ((RichTextBox)(TextControlWraper.Element)).TextChanged -= new TextChangedEventHandler(RichBox_TextChanged);
            FailedIf(!(o is RichTextBox), CurrentFunction + " - Failed: TextChanged event is not fired from RichTextBox!!!. It is fired form " + o.GetType().ToString());
            FailedIf(!(args.UndoAction >=UndoAction.None && args.UndoAction <= UndoAction.Create), "");
            QueueDelegate(EndTest);
            EndFunction();
        }

        /// <summary>Test event arg class</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "Type to RichTextBox to trigger the TextChanged Event")]
        public void TextChangedEventArgsTest()
        {
            EnterFunction("TextChangedEventArgsTest");

            ((RichTextBox)(TextControlWraper.Element)).TextChanged += new TextChangedEventHandler(Test_EventArg);
            try
            {
                TextControlWraper.Element.RaiseEvent(new TextChangedEventArgs(null, UndoAction.None));
                pass = false;
            }
            catch (ArgumentNullException e)
            {
                if (e.Message.Contains("id"))
                    pass = true;


            }
            for (UndoAction action = UndoAction.None; action <= UndoAction.Create; action++)
            {
                TextChangedEventArgs args = new TextChangedEventArgs(TextBox.TextChangedEvent, action);
                TextControlWraper.Element.RaiseEvent(args);
            }
            EndFunction();
        }

        void Test_EventArg(object o, TextChangedEventArgs args)
        {
            EnterFunction("Test_EventArg");
            FailedIf(!(o is RichTextBox), CurrentFunction + " - Failed: TextChanged event is not fired from RichTextBox!!!. It is fired form " + o.GetType().ToString());
            FailedIf(!(args.UndoAction >= UndoAction.None && args.UndoAction <= UndoAction.Create), "");
            if (args.UndoAction == UndoAction.Create)
            {
                ((RichTextBox)(TextControlWraper.Element)).TextChanged -= new TextChangedEventHandler(Test_EventArg);
                EndTest();
            }
            EndFunction();
        }
    }
}
