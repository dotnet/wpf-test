// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides test cases for FlowDirection on RichTextBox. 

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;            
    using System.Windows.Input;
    using System.Windows.Markup;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    
    using Test.Uis.Data;    
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;            

    #endregion Namespace.
    
    /// <summary>
    /// Test FlowDirection switch for a simple case with LTR (english) content and RTL (arabic) content,
    /// where there is a single run in paragraph and a case where there are multiple runs in a paragraph.
    /// Verification is done by verifying value of FlowDirection at paragraph/span/run level.
    /// </summary>
    [Test(0, "RichTextBox", "FDSwitchTest", MethodParameters = "/TestCaseType=FDSwitchTest")]
    [TestOwner("Microsoft"), TestTactics("673"), TestBugs(""), TestLastUpdatedOn("May 19, 2006")]
    public class FDSwitchTest : ManagedCombinatorialTestCase
    {
        #region Private fields

        private bool _testWithMultipleParagraphs=false;
        private bool _testWithMultipleRuns=false;
        private bool _testWithRTLContent=false;

        private RichTextBox _rtb;
        private UIElementWrapper _wrapper;

        private string _ltrTestString = "avalon ";
        private string _rtlTestString = "\x062e\x0635\x0645"; //Taken from TextScriptData.cs
        private FlowDirection _expectedTextFlowDirection;

        #endregion Private fields

        #region Main flow

        /// <summary>Runs the combination</summary>
        protected override void DoRunCombination()
        {
            _rtb = new RichTextBox();
            _rtb.Document.Blocks.Clear();

            if (_testWithRTLContent)
            {
                _rtb.Document.FlowDirection = FlowDirection.RightToLeft;
                _expectedTextFlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                _expectedTextFlowDirection = FlowDirection.LeftToRight;
            }

            Paragraph p = new Paragraph();
            AddRunElements(p);
            _rtb.Document.Blocks.Add(p);

            if (_testWithMultipleParagraphs)
            {
                Paragraph p1 = new Paragraph();
                AddRunElements(p1);
                _rtb.Document.Blocks.Add(p1);

                Paragraph p2 = new Paragraph();
                AddRunElements(p2);
                _rtb.Document.Blocks.Add(p2);
            }

            TestElement = _rtb;
            _wrapper = new UIElementWrapper(_rtb);

            QueueDelegate(AddRTLKeyboardLayout);
        }

        private void AddRTLKeyboardLayout()
        {
            _rtb.Focus();
            KeyboardInput.AddInputLocale(InputLocaleData.ArabicSaudiArabia.Identifier);

            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(1), new SimpleHandler(VerifyRTLKeyboardLayout), null);
        }

        private void VerifyRTLKeyboardLayout()
        {
            if (!KeyboardInput.IsBidiInputLanguageInstalled())
            {
                Verifier.Verify(false, "Bidi Input Language is not installed");
            }

            if (_testWithMultipleParagraphs)
            {
                //SelectAll to apply FlowDirection on all paragraphs
                _rtb.SelectAll();
            }

            QueueDelegate(DoControlShiftFlip);
        }

        private void DoControlShiftFlip()
        {
            Log("Performing first ControlShift flip");
            if (_testWithRTLContent)
            {
                ApplyFlowDirectionOnSelection(FlowDirection.LeftToRight);
            }
            else
            {
                ApplyFlowDirectionOnSelection(FlowDirection.RightToLeft);
            }
                        
            QueueDelegate(TestControlShiftFlip);
        }

        private void TestControlShiftFlip()
        {
            if (_testWithRTLContent)
            {
                VerifyControlShiftLeft();
            }
            else
            {
                VerifyControlShiftRight();
            }

            Log("Performing ControlShift flip back");
            if (_testWithRTLContent)
            {
                ApplyFlowDirectionOnSelection(FlowDirection.RightToLeft);
            }
            else
            {
                ApplyFlowDirectionOnSelection(FlowDirection.LeftToRight);
            }            

            QueueDelegate(TestControlShiftFlipBack);
        }

        private void TestControlShiftFlipBack()
        {
            if (_testWithRTLContent)
            {
                VerifyControlShiftRight();
            }
            else
            {
                VerifyControlShiftLeft();                
            }
            
            Log("Performing ControlShift flip again");
            if (_testWithRTLContent)
            {
                ApplyFlowDirectionOnSelection(FlowDirection.LeftToRight);
            }
            else
            {
                ApplyFlowDirectionOnSelection(FlowDirection.RightToLeft);
            }            

            QueueDelegate(TestControlShiftFlipAgain);
        }

        private void TestControlShiftFlipAgain()
        {
            if (_testWithRTLContent)
            {
                VerifyControlShiftLeft();
            }
            else
            {
                VerifyControlShiftRight();                
            }            

            QueueDelegate(NextCombination);
        }        

        #endregion Main flow  
      
        #region Helpers

        private void VerifyControlShiftRight()
        {
            foreach (Paragraph p in _rtb.Document.Blocks)
            {
                Verifier.Verify(p.FlowDirection == FlowDirection.RightToLeft,
                    "Verify that Paragraph has a FlowDirection of RightToLeft", true);

                if (_testWithMultipleRuns)
                {
                    Verifier.Verify(p.Inlines.Count == 1, "Paragraph should contain single Inline", false);
                    Verifier.Verify(p.Inlines.FirstInline is Span, "Paragraph should contain single Span", false);
                    Span span = (Span)p.Inlines.FirstInline;
                    Verifier.Verify(span.FlowDirection == _expectedTextFlowDirection,
                        "Verify that inner Span has the right FlowDirection: " + _expectedTextFlowDirection, true);
                    foreach (Run r in span.Inlines)
                    {
                        Verifier.Verify(r.FlowDirection == _expectedTextFlowDirection,
                            "Verify that Run's inside the Span also have the right FlowDirection: " + _expectedTextFlowDirection, true);
                    }
                }
                else
                {
                    Verifier.Verify(p.Inlines.Count == 1, "Paragraph should contain single Run", false);
                    Verifier.Verify(p.Inlines.FirstInline.FlowDirection == _expectedTextFlowDirection,
                        "Verify that inner run has the right FlowDirection: " + _expectedTextFlowDirection, true);
                }
            }
        }

        private void VerifyControlShiftLeft()
        {
            foreach (Paragraph p in _rtb.Document.Blocks)
            {
                Verifier.Verify(p.FlowDirection == FlowDirection.LeftToRight,
                    "Verify that Paragraph has a FlowDirection of LeftToRight", true);

                if (_testWithMultipleRuns)
                {
                    Verifier.Verify(p.Inlines.Count == 1, "Paragraph should contain single Inline", false);
                    Verifier.Verify(p.Inlines.FirstInline is Span, "Paragraph should contain single Span", false);
                    Span span = (Span)p.Inlines.FirstInline;
                    Verifier.Verify(span.FlowDirection == _expectedTextFlowDirection,
                        "Verify that inner Span has the right FlowDirection: " + _expectedTextFlowDirection, true);
                    foreach (Run r in span.Inlines)
                    {
                        Verifier.Verify(r.FlowDirection == _expectedTextFlowDirection,
                            "Verify that Run's inside the Span also have the right FlowDirection: " + _expectedTextFlowDirection, true);
                    }
                }
                else
                {
                    Verifier.Verify(p.Inlines.Count == 1, "Paragraph should contain single Run", false);
                    Verifier.Verify(p.Inlines.FirstInline.FlowDirection == _expectedTextFlowDirection,
                        "Verify that inner run has the right FlowDirection: " + _expectedTextFlowDirection, true);
                }
            }
        }

        private void ApplyFlowDirectionOnSelection(FlowDirection fd)
        {
            Log("Applying FlowDirection[" + fd + "] on RichTextBox selection");
            _rtb.Selection.ApplyPropertyValue(FrameworkElement.FlowDirectionProperty, fd);
        }

        private void AddRunElements(Paragraph p)
        {
            string testString;

            if (_testWithRTLContent)
            {
                testString = _rtlTestString;
            }
            else
            {
                testString = _ltrTestString;
            }

            p.Inlines.Add(new Run(testString));

            if (_testWithMultipleRuns)
            {
                Run r1, r2;

                r1 = new Run(testString);
                r1.FontWeight = FontWeights.Bold;
                p.Inlines.Add(r1);

                r2 = new Run(testString);
                r2.FontStyle = FontStyles.Italic;
                p.Inlines.Add(r2);
            }

            //Assign the Language property to ar-sa for RTL content.
            if (_testWithRTLContent)
            {
                foreach (Run r in p.Inlines)
                {
                    r.Language = XmlLanguage.GetLanguage("ar-sa");
                }
            }
        }

        #endregion Helpers
    }
    
    /// <summary>
    /// Verifies that when RTL content is deleted between two LTR Runs in RTL Paragraph, 
    /// the two LTR Runs get merged. The following two scenarios are covered in this test case
    /// Scenario1: [p][r1]One [/r1][r2 xml:lang='ar-sa' FlowDirection='RTL']xxx[/r2][r3] Two[/r3][/p]
    /// Scenario2: [p][r1 FontWeight='Bold']One [/r1][r2 xml:lang='ar-sa' FlowDirection='RTL']xxx[/r2][r3] Two[/r3][/p]
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("27"), TestBugs("180"), TestLastUpdatedOn("May 19, 2006")]
    public class DeleteRTLRunBtwnLTRRuns : CombinedTestCase
    {
        #region Private fields

        RichTextBox _rtb;
        Run _run1,_run2,_run3;
        Paragraph _para1;

        private string _run1Text = "One ";
        private string _run3Text = " Two";
        private string _rtlContent = "\x062e\x0635\x0645"; //Taken from TextScriptData.cs

        #endregion Private fields

        #region Main flow

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 200;
            _rtb.FontSize = 20;

            Log("----------Scenario1: [p][r1]One [/r1][r2 xml:lang='ar-sa' FlowDirection='RTL']xxx[/r2][r3] Two[/r3][/p]");
            SetRTBContents();

            MainWindow.Content = _rtb;

            QueueDelegate(ApplyRTLFlowDirectionScenario1);
        }

        private void ApplyRTLFlowDirectionScenario1()
        {
            _rtb.Focus();
            Verifier.Verify(_para1.Inlines.Count == 3, "Verify that paragraph has 3 run elements after adding to RTB - Scenario1", false);

            //With empty selection, FD is applied on Paragraph.
            _rtb.Selection.Select(_run1.ContentStart, _run1.ContentStart);
            _rtb.Selection.ApplyPropertyValue(FrameworkElement.FlowDirectionProperty, FlowDirection.RightToLeft);

            QueueDelegate(ClickLeftToContents);
        }

        /// <summary>Verified Regression_Bug180 - Hit test with Bidi content sometimes doesnt work properly</summary>
        private void ClickLeftToContents()
        {
            //Get the point to the left of the contents and Mouse click
            Rect clickRect = _run3.ContentStart.GetCharacterRect(LogicalDirection.Forward);
            //clickRect.X - 30.0 is to make sure we click to the left of the contents
            Point clickPoint = ElementUtils.GetScreenRelativePoint(_rtb, new Point(clickRect.X - 30.0, clickRect.Y + (clickRect.Height/2) ));
            MouseInput.MouseClick(clickPoint);

            QueueDelegate(VerifyContentsAfterClick);
        }

        private void VerifyContentsAfterClick()
        {
            Verifier.Verify(_rtb.Selection.Start.GetTextInRun(LogicalDirection.Forward) == _run3Text,
                "Verifying the contents in forward direction after clicking", true);
            Verifier.Verify(_rtb.Selection.Start.GetTextInRun(LogicalDirection.Backward) == string.Empty,
                "Verifying the contents in backward direction after clicking", true);

            QueueDelegate(DeleteRTLRunScenario1);
        }

        private void DeleteRTLRunScenario1()
        {
            Verifier.Verify(_para1.FlowDirection == FlowDirection.RightToLeft, "Verifying paragraph FD changed to RTL - Scenario1", true);
            _rtb.Selection.Select(_run2.ContentStart, _run2.ContentEnd);
            _rtb.Cut();

            QueueDelegate(VerifyRunsMerged);
        }

        private void VerifyRunsMerged()
        {            
            Verifier.Verify(_para1.Inlines.Count == 1, "Run1 and Run3 should be merged", true);            
            Verifier.Verify(((Run)_para1.Inlines.FirstInline).Text == (_run1Text + _run3Text),
                "Verifying the contents after run1 and run3 contents are merged", true);

            Log("----------Scenario2: [p][r1 FontWeight='Bold']One [/r1][r2 xml:lang='ar-sa' FlowDirection='RTL']xxx[/r2][r3] Two[/r3][/p]");
            SetRTBContents();
            _run1.FontWeight = FontWeights.Bold;

            QueueDelegate(ApplyRTLFlowDirectionScenario2);
        }

        private void ApplyRTLFlowDirectionScenario2()
        {            
            Verifier.Verify(_para1.Inlines.Count == 3, "Verify that paragraph has 3 run elements after adding to RTB - Scenario2", false);

            //With empty selection, FD is applied on Paragraph.
            _rtb.Selection.Select(_run1.ContentStart, _run1.ContentStart);
            _rtb.Selection.ApplyPropertyValue(FrameworkElement.FlowDirectionProperty, FlowDirection.RightToLeft);

            QueueDelegate(DeleteRTLRunScenario2);
        }

        private void DeleteRTLRunScenario2()
        {
            Verifier.Verify(_para1.FlowDirection == FlowDirection.RightToLeft, "Verifying paragraph FD changed to RTL - Scenario2", true);
            _rtb.Selection.Select(_run2.ContentStart, _run2.ContentEnd);
            _rtb.Cut();

            QueueDelegate(VerifySpanCreated);
        }

        private void VerifySpanCreated()
        {
            Verifier.Verify((_para1.Inlines.Count == 1) && (_para1.Inlines.FirstInline is Span), 
                "Verifying that Paragraph has only one Inline which should be Span", true);
            Verifier.Verify(((Span)_para1.Inlines.FirstInline).FlowDirection == FlowDirection.LeftToRight,
                "Verifying that Span has a FlowDirection of LeftToRight", true);
            Verifier.Verify(((Span)_para1.Inlines.FirstInline).Inlines.Count == 2,
                "Verifying that Span contains the two run elements run1, run3", true);
            Verifier.Verify((_run1.Text == _run1Text) && (_run3.Text == _run3Text),
                "Verifying the contents after Span is created", true);

            EndTest();
        }

        #endregion Main flow

        private void SetRTBContents()
        {
            _rtb.Document.Blocks.Clear();

            _run1 = new Run(_run1Text);

            _run2 = new Run(_rtlContent);
            _run2.Language = XmlLanguage.GetLanguage("ar-sa");
            _run2.FlowDirection = FlowDirection.RightToLeft;

            _run3 = new Run(_run3Text);

            _para1 = new Paragraph();
            _para1.Inlines.Add(_run1);
            _para1.Inlines.Add(_run2);
            _para1.Inlines.Add(_run3);

            _rtb.Document.Blocks.Add(_para1);
        }
    }

    /// <summary>
    /// Verified Regression_Bug181 - Scenario where multiple runs are present inside a paragraph with one run
    /// element in the middle which is RTL content. When FlowDirection is switch, there should be two
    /// spans created, one for the first set of runs before the RTL run and one for the second set of runs
    /// after the RTL run
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("27"), TestBugs("181"), TestLastUpdatedOn("June 16, 2006")]
    public class FDSwitchWithBidiContent : CombinedTestCase
    {
        #region Private fields

        private RichTextBox _rtb;
        private Run _run1,_run2,_run3,_run4,_run5,_run6,_run7;
        private Paragraph _para1;
        private string _rtlContent = "\x062e\x0635\x0645"; //Taken from TextScriptData.cs

        #endregion Private fields

        #region Main flow

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 200;
            _rtb.FontSize = 20;

            Log("Scenario: [p][r1]One [/r1][r2 Bold]Two [/r2][r3]Three [/r3][r4 xml:lang='ar-sa' FD='RTL']xxx[/r4][r5] Five [/r5][r6 Bold]Six [/r6][r7]Seven [/r7][/p]");
            SetRTBContents();

            MainWindow.Content = _rtb;

            QueueDelegate(ApplyRTLFlowDirection);
        }

        private void ApplyRTLFlowDirection()
        {
            _rtb.Focus();
            Verifier.Verify(_para1.Inlines.Count == 7, "Verify that paragraph has 7 run elements after adding to RTB", true);

            //With empty selection, FD is applied on Paragraph.
            _rtb.Selection.Select(_run1.ContentStart, _run1.ContentStart);
            _rtb.Selection.ApplyPropertyValue(FrameworkElement.FlowDirectionProperty, FlowDirection.RightToLeft);

            QueueDelegate(VerifySpanCreation);
        }

        private void VerifySpanCreation()
        {
            Verifier.Verify(_para1.Inlines.Count == 3, "Verifying that paragraph has 3 run elements after switching flow direction", true);
            Verifier.Verify(_para1.Inlines.FirstInline is Span, "Verifying that first inline is Span", true);
            Span span1 = (Span)_para1.Inlines.FirstInline;
            Verifier.Verify(span1.Inlines.Count == 3, "Verifying that first span has 3 Runs", true);

            Verifier.Verify(_para1.Inlines.LastInline is Span, "Verifying that last inline is Span", true);
            Span span2 = (Span)_para1.Inlines.LastInline;
            Verifier.Verify(span2.Inlines.Count == 3, "Verifying that last span has 3 Runs", true);

            Verifier.Verify(_run4.Parent==_para1, "Verifying the RTL Run is not spaned", true);
            Verifier.Verify(_run4.FlowDirection == FlowDirection.RightToLeft, "Verifying FlowDirection on RTL Run", true);            
            
            EndTest();
        }

        private void SetRTBContents()
        {
            _rtb.Document.Blocks.Clear();

            _run1 = new Run("One ");
            _run2 = new Run("Two ");
            _run2.FontWeight = FontWeights.Bold;
            _run3 = new Run("Three ");

            _run4 = new Run(_rtlContent);
            _run4.Language = XmlLanguage.GetLanguage("ar-sa");
            _run4.FlowDirection = FlowDirection.RightToLeft;
            
            _run5 = new Run(" Five ");
            _run6 = new Run("Six ");
            _run6.FontWeight = FontWeights.Bold;
            _run7 = new Run("Seven ");            
            
            _para1 = new Paragraph();
            _para1.Inlines.Add(_run1);
            _para1.Inlines.Add(_run2);
            _para1.Inlines.Add(_run3);
            _para1.Inlines.Add(_run4);
            _para1.Inlines.Add(_run5);
            _para1.Inlines.Add(_run6);
            _para1.Inlines.Add(_run7);

            _rtb.Document.Blocks.Add(_para1);
        }

        #endregion Main flow
    }
}
