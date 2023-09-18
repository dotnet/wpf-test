// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides regression coverage for TextOM related regressions

using System;
using System.Windows.Documents;
using System.Windows.Media;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Loggers;
using Test.Uis.TestTypes;

namespace Test.Editing.Regressions
{
    /// <summary>
    /// This test case tests for regressions in TextOM.     
    /// </summary>
    [Test(1, "TextOM", "TextOMRegressions", MethodParameters = "/TestCaseType:TextOMRegressions")]    
    public class TextOMRegressions : CustomTestCase
    {
        /// <summary>
        /// Entry point to test case
        /// </summary>
        public override void RunTestCase()
        {
            // Repro for Part1 Regression_Bug78
            RegressPart1Regression_Bug78();

            // Repro for Part1 Regression_Bug79
            RegressPart1Regression_Bug79();

            // Repro for Part1 Regression_Bug80
            RegressPart1Regression_Bug80();

            Logger.Current.ReportSuccess();
        }

        private void RegressPart1Regression_Bug78()
        {            
            List list = new List();
            //TestWindow.Content = list;
            Section section = new Section();
            section.Blocks.Add(list);

            // With the bug the below statement fires an invariant assert.
            list.SiblingBlocks.Clear();
        }

        private void RegressPart1Regression_Bug79()
        {
            Run run = new Run("This is a test");
            TextRange tr = new TextRange(run.ContentStart, run.ContentEnd);

            // With the bug the below statement throws a null reference exception
            tr.ApplyPropertyValue(Run.ForegroundProperty, Brushes.Blue);

            Verifier.Verify(run.Foreground == Brushes.Blue, "Verifying that property is applied on un-parented Run", true);
        }

        private void RegressPart1Regression_Bug80()
        {
            string content = "This is a test";
            Run run = new Run(content);

            // Place a textpointer somewhere in the middle of the Run
            int tpOffset = 5;
            TextPointer tp = run.ContentStart.GetPositionAtOffset(tpOffset); 

            // With the bug the below statement throws a null reference exception because TextPointer.TextContainer.Parent is null
            tp.InsertParagraphBreak();

            // Verify the contents of the paragraphs after the InsertParagraphBreak operation
            Paragraph p1 = (Paragraph)run.Parent;
            Verifier.Verify(content.Contains(((Run)p1.Inlines.FirstInline).Text), "Verifying the content inside Paragraph1: [" + ((Run)p1.Inlines.FirstInline).Text + "]", true);
            Paragraph p2 = (Paragraph)p1.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
            Verifier.Verify(content.Contains(((Run)p2.Inlines.FirstInline).Text), "Verifying the content inside Paragraph2: [" + ((Run)p2.Inlines.FirstInline).Text + "]", true);
        }
    }
}
