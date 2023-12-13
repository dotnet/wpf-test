// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Threading;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// This class tests copy/paste of rich content inside RichTextBox in a partial trust environment
    /// </summary>
    // DISABLEDUNSTABLETEST:
    // TestName:RichTextCutCopyPasteInPartialTrust
    // Area: Editing SubArea: PartialTrust
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: findstr /snip DISABLEDUNSTABLETEST
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "RichTextCutCopyPasteInPartialTrust", MethodParameters = "/TestCaseType=RichTextCutCopyPasteInPartialTrust /InputMonitorEnabled:False /XbapName=EditingTestDeploy", Timeout = 200,Disabled = true)]
    public class RichTextCutCopyPasteInPartialTrust : ManagedCombinatorialTestCase
    {
        #region Main flow

        protected override void DoRunCombination()
        {
            _rtb1 = new RichTextBox();
            _rtb1.Height = 300;
            _rtb1.FontSize = 24;
            _wrapper1 = new UIElementWrapper(_rtb1);

            _rtb2 = new RichTextBox();
            _rtb2.Height = 300;
            _rtb2.FontSize = 24;
            _wrapper2 = new UIElementWrapper(_rtb2);

            _currentIndex = 0;

            StackPanel panel = new StackPanel();
            panel.Children.Add(_rtb1);
            panel.Children.Add(_rtb2);

            TestElement = panel;
            QueueDelegate(AssignContent);
        }

        private void AssignContent()
        {
            Log("*** Assigning content: " + _xamlContents[_currentIndex]);
            _wrapper1.XamlText = _xamlContents[_currentIndex];
            _rtb2.Document.Blocks.Clear();

            _rtb1.Focus();
            _rtb1.SelectAll();
            DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(EditingCommandData.GetInputGestureStringForCommand(_testCommand));

            if (_testCommand == ApplicationCommands.Cut)
            {
                // While testing cut, we would like to test undo after cut
                QueueDelegate(PerformUndo);
            }
            else
            {
                QueueDelegate(PerformPaste);
            }
        }

        private void PerformUndo()
        {
            // Undo is also performed on RTB1 while testing cut.
            KeyboardInput.TypeString(EditingCommandData.GetInputGestureStringForCommand(ApplicationCommands.Undo));
            DispatcherHelper.DoEvents();
            QueueDelegate(PerformPaste);
        }

        private void PerformPaste()
        {
            _rtb2.Focus();
            DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(EditingCommandData.GetInputGestureStringForCommand(ApplicationCommands.Paste));
            DispatcherHelper.DoEvents();
            QueueDelegate(VerifyPaste);
        }

        private void VerifyPaste()
        {
            switch (_currentIndex)
            {
                case 0:
                    VerifyRichTextPaste();
                    break;
                case 1:
                case 2:
                    VerifyImageOrUIElementPaste();
                    break;
                default:
                    throw new ApplicationException("Invalid index");
            }

            if (_currentIndex < _xamlContents.Length - 1)
            {
                _currentIndex++;
                QueueDelegate(AssignContent);
            }
            else
            {
                NextCombination();
            }
        }

        private void VerifyRichTextPaste()
        {
            // Plain rich text should pass through copy/paste in PT
            Paragraph p = (Paragraph)_rtb2.Document.Blocks.FirstBlock;
            Verifier.Verify(p.Inlines.Count == 3, "Verifying inline count after paste for plain rich text: Expected[3] Actual[" + p.Inlines.Count + "]", true);
            Inline middleInline = p.Inlines.FirstInline.NextInline;
            Verifier.Verify(middleInline.FontWeight == FontWeights.Bold, "Verifying that bold inline is pasted", true);

            if (_testCommand == ApplicationCommands.Cut)
            {
                Log("Testing undo after cut on the source RichTextBox with RichText content");
                Paragraph sourcePara = (Paragraph)_rtb1.Document.Blocks.FirstBlock;
                Verifier.Verify(sourcePara.Inlines.Count == 3, "Verifying inline count after cut followed by undo for plain rich text: Expected[3] Actual[" + p.Inlines.Count + "]", true);
                Inline sourceMiddleInline = sourcePara.Inlines.FirstInline.NextInline;
                Verifier.Verify(sourceMiddleInline.FontWeight == FontWeights.Bold, "Verifying that bold inline is brought back after cut followed by undo", true);
            }
        }

        private void VerifyImageOrUIElementPaste()
        {
            // Image/UIElement shouldnt pass through copy/paste in PT
            Paragraph p = (Paragraph)_rtb2.Document.Blocks.FirstBlock;
            foreach (Inline inline in p.Inlines)
            {
                Verifier.Verify(!(inline is InlineUIContainer), "Verifying that InlineUIContainer with Image/UIElement is not pasted", true);
            }

            if (_testCommand == ApplicationCommands.Cut)
            {
                Log("Testing undo after cut on the source RichTextBox with Image/UIElement content");
                Paragraph sourcePara = (Paragraph)_rtb1.Document.Blocks.FirstBlock;
                Verifier.Verify(sourcePara.Inlines.Count == 3, "Verifying inline count after cut followed by undo for Image/UIElement: Expected[3] Actual[" + p.Inlines.Count + "]", true);
                Inline sourceMiddleInline = sourcePara.Inlines.FirstInline.NextInline;
                Verifier.Verify((sourceMiddleInline is InlineUIContainer) &&
                    (((InlineUIContainer)sourceMiddleInline).Child is Grid),
                    "Verifying that empty Grid is placed in place of Image/UIElement after cut followed by undo", true);
            }
        }

        #endregion

        #region Private fields

        private RichTextBox _rtb1 = null;
        private RichTextBox _rtb2 = null;
        private UIElementWrapper _wrapper1 = null;
        private UIElementWrapper _wrapper2 = null;
        private int _currentIndex = 0;
        private RoutedUICommand _testCommand = null;

        private string[] _xamlContents = new string[] {
            "<Paragraph>abc<Bold>BOLD</Bold>def</Paragraph>",
            "<Paragraph>ghi<Image Height='100' Source='pack://siteoforigin:,,,/colors.png'/>jkl</Paragraph>",
            "<Paragraph>mno<Button>Button></Button>pqr</Paragraph>"
        };

        #endregion
    }
}
