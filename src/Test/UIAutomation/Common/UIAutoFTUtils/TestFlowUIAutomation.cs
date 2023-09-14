// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using MTI = Microsoft.Test.Input;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Test.Logging;
using System.Windows.Automation.Text;
using Microsoft.Test.Input;
using System.Windows.Threading;
using Microsoft.Test.Hosting;
using Microsoft.Test.UIAutomaion;
using System.Globalization;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing InvokePattern for Controls Below
    /// Hyperlink
    /// </summary>
    [Serializable]
    public class UiaHyperlinkInvokeTest : UiaSimpleTestcase
    {
        bool _invoked = false;

        public override void Init(object target)
        {
            // check for Hyperlink
            FlowDocument fd = target as FlowDocument;
            if (fd != null)
            {
                foreach (Paragraph p in fd.Blocks)
                {
                    foreach (Inline inline in p.Inlines)
                    {
                        if (inline is Hyperlink)
                        {
                            ((Hyperlink)inline).Click += new RoutedEventHandler(target_Click);
                        }
                    }
                }
            }
            else
            {
                Hyperlink hyp = target as Hyperlink;
                if (hyp != null)
                {
                    ((Hyperlink)target).Click += new RoutedEventHandler(target_Click);
                }
            }
        }

        void target_Click(object sender, RoutedEventArgs e)
        {
            _invoked = true;
            TestLog.Current.LogEvidence("The click event was raised");
        }

        public override void DoTest(AutomationElement target)
        {
            // check for Hyperlink
            InvokePattern ip = null;
            if (target.Current.IsContentElement)
            {
                // get Text Pattern for Content Elements
                try
                {
                    TextPattern tp = target.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                    AutomationElement[] AEs = tp.DocumentRange.GetChildren();
                    foreach (AutomationElement AE in AEs)
                    {
                        ip = AE.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    }
                }
                catch
                {
                    ip = target.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                }
            }
            else
            {
                ip = target.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            }
            ip.Invoke();
        }

        public override void Validate(object target)
        {
            if (_invoked)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogEvidence("The click event was not raised");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }

    /// <summary>
    /// Testing TextPattern for Controls Below
    /// FlowDocument
    /// </summary>
    [Serializable]
    public class UiaGetTextTest : UiaSimpleTestcase
    {
        string _paraText;
        string _fullText;

        public string ParaText
        {
            get { return _paraText; }
            set { _paraText = value; }
        }

        public string FullText
        {
            get { return _fullText; }
            set { _fullText = value; }
        }

        public override void Init(object target)
        {
            FlowDocument fd = target as FlowDocument;
            FlowDocumentScrollViewer fdsv = fd.Parent as FlowDocumentScrollViewer;
            Paragraph p = fd.Blocks.FirstBlock as Paragraph;
            if (_paraText != null)
            {
                p.Inlines.Clear();
                p.Inlines.Add(new Run(ParaText));
            }
            if (_fullText != null)
            {
                p.Inlines.Clear();
                p.Inlines.Add(new Run(FullText));
            }
            TextRange tr = new TextRange(p.ContentStart, p.ContentEnd);
            tr.Select(p.ContentStart, p.ContentEnd);

            DragMouseToSelectText(fdsv, 0, 0, 420, 0);
        }

        public override void DoTest(AutomationElement target)
        {
            TextPattern tp = target.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            TextPatternRange findRange = tp.DocumentRange.FindText("Hello", false, true);
            TextPatternRange NotfindRange = tp.DocumentRange.FindText("Invisible", false, true);

            if (findRange != null)
                SharedState["findRange"] = true;
            else
                SharedState["findRange"] = false;

            if (NotfindRange == null)
                SharedState["NotfindRange"] = true;
            else
                SharedState["NotfindRange"] = false;

            // wait a certain time before getting selection to avoid race
            WaitTillTimeout(new TimeSpan(0, 0, 2));

            TextPatternRange[] tpRange = tp.GetSelection();
            SharedState["selectedText"] = tpRange[0].GetText(1000).Trim();

            AutomationElement AE = tpRange[0].GetEnclosingElement();
            if (AE.Current.AutomationId == target.Current.AutomationId)
                SharedState["EnclosingElementFound"] = true;
            else
                SharedState["EnclosingElementFound"] = false;            
        }

        public override void Validate(object target)
        {
            if (_paraText != null)
            {
                if ((string)SharedState["selectedText"] == _paraText)
                    TestLog.Current.Result = TestResult.Pass;
                else
                {
                    TestLog.Current.LogEvidence("Selected Text Obtained from Automation is different");
                    TestLog.Current.LogEvidence("Expected Text: " + _paraText);
                    TestLog.Current.LogEvidence("Observed Text: " + (string)SharedState["selectedText"]);
                    TestLog.Current.Result = TestResult.Fail;
                }
            }

            if (_fullText != null)
            {
                if ((string)SharedState["selectedText"] != _fullText)
                    TestLog.Current.Result = TestResult.Pass;
                else
                {
                    TestLog.Current.LogEvidence("Selected Text Obtained from Automation is same as Full Text");
                    TestLog.Current.LogEvidence("Full Text: " + _fullText);
                    TestLog.Current.LogEvidence("Observed Selected Text: " + (string)SharedState["selectedText"]);
                    TestLog.Current.Result = TestResult.Fail;
                }
            }

            if ((bool)SharedState["findRange"] == true)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("Not found word 'Hello' in selected Text");
                TestLog.Current.Result = TestResult.Fail;
            }

            if ((bool)SharedState["NotfindRange"] == true)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("Found word 'Invisible' which was not present");
                TestLog.Current.Result = TestResult.Fail;
            }

            if ((bool)SharedState["EnclosingElementFound"] == true)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("Found enclosing Element for a Range Different from What it was supposed to be");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        private void WaitTillTimeout(TimeSpan timeout)
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            // Set a timer to terminate our loop in the frame after the
            // timeout has expired.
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);
            timer.Interval = timeout;
            timer.Tick += delegate(object sender, EventArgs e)
            {
                ((DispatcherTimer)sender).IsEnabled = false;
                frame.Continue = false;
            };
            timer.Start();

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        public void DragMouseToSelectText(FlowDocumentScrollViewer fdsv, int startX, int startY, int endX, int endY)
        {
            //frm.Status("Moving mouse to the start of the Selection...");
            UserInput.MouseButton(fdsv, startX, startY, "Move");

            //frm.Status("MouseLeftButtonDown...");
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.LeftDown);

            //frm.Status("Move Mouse within the Anchored Block...");
            UserInput.MouseButton(fdsv, endX, endY, "Move");

            //frm.Status("MouseLeftButtonUp...");
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.LeftUp);
        }
    }

    /// <summary>
    /// Testing GridPattern for Controls Below
    /// Table
    /// </summary>
    [Serializable]
    public class UiaTableTest : UiaSimpleTestcase
    {
        int _rowCount = 0;
        int _colCount = 0;

        public override void Init(object target)
        {
            FlowDocument fd = target as FlowDocument;
            Table table = fd.Blocks.FirstBlock as Table;
            foreach (TableRowGroup tblrg in table.RowGroups)
            {
                _rowCount += tblrg.Rows.Count;
            }
            _colCount = table.Columns.Count;
        }

        public override void DoTest(AutomationElement target)
        {
            TextPattern tp = target.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            AutomationElement[] AEs = tp.DocumentRange.GetChildren();
            GridPattern gp = null;

            foreach (AutomationElement AE in AEs)
            {
                if (AE.Current.AutomationId == "TABLE")
                {
                    gp = AE.GetCurrentPattern(GridPattern.Pattern) as GridPattern;
                    SharedState["rowCount"] = gp.Current.RowCount;
                    SharedState["colCount"] = gp.Current.ColumnCount;

                    AutomationElement tableCell00Element = AE.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "TABLECELL00"));
                    AutomationElement tableCell11Element = AE.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "TABLECELL11"));
                    GridItemPattern gip00 = tableCell00Element.GetCurrentPattern(GridItemPattern.Pattern) as GridItemPattern;
                    GridItemPattern gip11 = tableCell11Element.GetCurrentPattern(GridItemPattern.Pattern) as GridItemPattern;
                    if ((gip00.Current.Column == 0) && (gip11.Current.Column == 1))
                        SharedState["foundColumns"] = true;
                    else
                        SharedState["foundColumns"] = false;

                    if ((gip00.Current.Row == 0) && (gip11.Current.Row == 1))
                        SharedState["foundRows"] = true;
                    else
                        SharedState["foundRows"] = false;

                    if (gip00.Current.ContainingGrid.Current.AutomationId == AE.Current.AutomationId)
                    {
                        SharedState["validParent"] = true;                        
                    }
                }
            }            
        }

        public override void Validate(object target)
        {
            if ((int)SharedState["rowCount"] == _rowCount)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("The RowCount Reported by Grid Pattern is wrong.");
                TestLog.Current.LogEvidence("Expected RowCount = " + _rowCount);
                TestLog.Current.LogEvidence("Observed RowCount = " + SharedState["rowCount"]);
                TestLog.Current.Result = TestResult.Fail;
            }

            if ((int)SharedState["colCount"] == _colCount)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("The ColumnCount Reported by Grid Pattern is wrong.");
                TestLog.Current.LogEvidence("Expected ColumnCount = " + _colCount);
                TestLog.Current.LogEvidence("Observed ColumnCount = " + SharedState["colCount"]);
                TestLog.Current.Result = TestResult.Fail;
            }

            if ((bool)SharedState["foundRows"] == true)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("The Row Reported by GridItem Pattern is wrong.");
                TestLog.Current.Result = TestResult.Fail;
            }

            if ((bool)SharedState["foundColumns"] == true)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("The Row Reported by GridItem Pattern is wrong.");
                TestLog.Current.Result = TestResult.Fail;
            }

            if ((bool)SharedState["validParent"] == true)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("The GridItem Pattern Parent is incorrect.");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }

    /// <summary>
    /// This class is used to test all the non basic properties that the Pattern exposes
    /// for FlowDocument.
    /// </summary>
    [Serializable]
    public class UiaDocPatternTest : UiaSimpleTestcase
    {
        string _paraText;
        string _fullText;

        public string ParaText
        {
            get { return _paraText; }
            set { _paraText = value; }
        }

        public string FullText
        {
            get { return _fullText; }
            set { _fullText = value; }
        }

        public override void Init(object target)
        {
            FlowDocument fd = target as FlowDocument;
            FlowDocumentScrollViewer fdsv = fd.Parent as FlowDocumentScrollViewer;
            Paragraph p = fd.Blocks.FirstBlock as Paragraph;
            if (_paraText != null)
            {
                p.Inlines.Clear();
                p.Inlines.Add(new Run(ParaText));
            }
            if (_fullText != null)
            {
                p.Inlines.Clear();
                p.Inlines.Add(new Run(FullText));
            }
            TextRange tr = new TextRange(p.ContentStart, p.ContentEnd);
            tr.Select(p.ContentStart, p.ContentEnd);

            DragMouseToSelectText(fdsv, 0, 0, 420, 0);
        }

        public override void DoTest(AutomationElement target)
        {
            TextPattern tp = target.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            // wait a certain time before getting selection to avoid race
            WaitTillTimeout(new TimeSpan(0, 0, 2));

            TextPatternRange[] tpRange = tp.GetSelection();
            SharedState["selectedText"] = tpRange[0].GetText(1000).Trim();
            SharedState["SupportedSelection"] = tp.SupportedTextSelection;
            SharedState["IsKeyboardFocusableProperty"] = target.GetCurrentPropertyValue(AutomationElement.IsKeyboardFocusableProperty, true);
            SharedState["ControlType"] =  target.Current.ControlType.ProgrammaticName;
            SharedState["IsContentElement"] = target.Current.IsContentElement;
            SharedState["IsControlElement"] = target.Current.IsControlElement;

            SharedState["AnimationStyleAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.AnimationStyleAttribute);
            SharedState["BackgroundColorAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.BackgroundColorAttribute);
            SharedState["BulletStyleAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.BulletStyleAttribute);
            SharedState["CapStyleAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.CapStyleAttribute);
            SharedState["CultureAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.CultureAttribute);
            SharedState["FontNameAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.FontNameAttribute);
            SharedState["FontSizeAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.FontSizeAttribute);
            SharedState["FontWeightAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.FontWeightAttribute);
            SharedState["HorizontalTextAlignmentAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.HorizontalTextAlignmentAttribute);
            SharedState["IndentationFirstLineAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.IndentationFirstLineAttribute);
            SharedState["IndentationLeadingAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.IndentationLeadingAttribute);
            SharedState["IndentationTrailingAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.IndentationTrailingAttribute);
            SharedState["IsHiddenAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.IsHiddenAttribute);
            SharedState["IsItalicAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.IsItalicAttribute);
            SharedState["IsReadOnlyAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.IsReadOnlyAttribute);
            SharedState["IsSubscriptAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.IsSubscriptAttribute);
            SharedState["IsSuperscriptAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.IsSuperscriptAttribute);
            SharedState["MarginBottomAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.MarginBottomAttribute);
            SharedState["MarginLeadingAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.MarginLeadingAttribute);
            SharedState["MarginTopAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.MarginTopAttribute);
            SharedState["MarginTrailingAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.MarginTrailingAttribute);
            SharedState["OutlineStylesAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.OutlineStylesAttribute);
            SharedState["OverlineColorAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.OverlineColorAttribute);
            SharedState["StrikethroughColorAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.StrikethroughColorAttribute);
            SharedState["TextFlowDirectionsAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.TextFlowDirectionsAttribute);
            SharedState["UnderlineColorAttribute"] = tp.DocumentRange.GetAttributeValue(TextPattern.UnderlineColorAttribute);            
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            if ((SupportedTextSelection)SharedState["SupportedSelection"] != SupportedTextSelection.Single)
            {
                TestLog.Current.LogEvidence("SupportedTextSelection should be Single");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((bool)SharedState["IsKeyboardFocusableProperty"] != true)
            {
                TestLog.Current.LogEvidence("IsKeyboardFocusableProperty must be TRUE for FlowDocuments");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((string)SharedState["ControlType"] != "ControlType.Document")
            {
                TestLog.Current.LogEvidence("ControlType must be Document for FlowDocuments");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((bool)SharedState["IsContentElement"] != true)
            {
                TestLog.Current.LogEvidence("IsContentElement must be TRUE for FlowDocuments");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((bool)SharedState["IsControlElement"] != true)
            {
                TestLog.Current.LogEvidence("IsControlElement must be TRUE for FlowDocuments");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((AnimationStyle)SharedState["AnimationStyleAttribute"] != AnimationStyle.None)
            {
                TestLog.Current.LogEvidence("There is no AnimationStyle for this FlowDocument: [" + (AnimationStyle)SharedState["AnimationStyle"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((int)SharedState["BackgroundColorAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("There is no BackgroundColor set for this FlowDocument: [" + SharedState["BackgroundColorAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((BulletStyle)SharedState["BulletStyleAttribute"] != BulletStyle.FilledRoundBullet)
            {
                TestLog.Current.LogEvidence("Invalid BulletStyle for this FlowDocument: [" + (BulletStyle)SharedState["BulletStyleAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((CapStyle)SharedState["CapStyleAttribute"] != CapStyle.None)
            {
                TestLog.Current.LogEvidence("There is no CapStyle for this FlowDocument: [" + (CapStyle)SharedState["CapStyleAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            if (!((CultureInfo)(SharedState["CultureAttribute"])).Equals(CultureInfo.GetCultureInfo("en-US")))
                
            {
                TestLog.Current.LogEvidence("Invalid Culture for this FlowDocument: [" + (CultureInfo)SharedState["CultureAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((string)SharedState["FontNameAttribute"] != "Georgia")
            {
                TestLog.Current.LogEvidence("Invalid FontName for this FlowDocument: [" + SharedState["FontNameAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((double)SharedState["FontSizeAttribute"] != 12)
            {
                TestLog.Current.LogEvidence("Invalid FontSize for this FlowDocument: [" + SharedState["FontSizeAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((Int32)SharedState["FontWeightAttribute"] != 400)
            {
                TestLog.Current.LogEvidence("Invalid FontWeight for this FlowDocument: [" + SharedState["FontWeightAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((HorizontalTextAlignment)SharedState["HorizontalTextAlignmentAttribute"] != HorizontalTextAlignment.Justified)
            {
                TestLog.Current.LogEvidence("Invalid HorizontalTextAlignment for this FlowDocument: [" + (HorizontalTextAlignment)SharedState["HorizontalTextAlignmentAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((double)SharedState["IndentationFirstLineAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("Invalid IndentationFirstLine for this FlowDocument: [" + SharedState["IndentationFirstLineAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((double)SharedState["IndentationLeadingAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("Invalid IndentationLeading for this FlowDocument: [" + SharedState["IndentationLeadingAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((double)SharedState["IndentationTrailingAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("Invalid IndentationTrailing for this FlowDocument: [" + SharedState["IndentationTrailingAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((bool)SharedState["IsHiddenAttribute"] != false)
            {
                TestLog.Current.LogEvidence("Invalid IsHidden attribute for this FlowDocument: [" + SharedState["IsHiddenAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((bool)SharedState["IsItalicAttribute"] != false)
            {
                TestLog.Current.LogEvidence("Invalid IsItalic attribute for this FlowDocument: [" + SharedState["IsItalicAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((bool)SharedState["IsReadOnlyAttribute"] != true)
            {
                TestLog.Current.LogEvidence("Invalid IsReadOnly attribute for this FlowDocument: [" + SharedState["IsReadOnlyAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((bool)SharedState["IsSubscriptAttribute"] != false)
            {
                TestLog.Current.LogEvidence("Invalid IsSubscript attribute for this FlowDocument: [" + SharedState["IsSubscriptAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((bool)SharedState["IsSuperscriptAttribute"] != false)
            {
                TestLog.Current.LogEvidence("Invalid IsSuperscript attribute for this FlowDocument: [" + SharedState["IsSuperscriptAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((double)SharedState["MarginBottomAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("Invalid MarginBottom for this FlowDocument: [" + SharedState["MarginBottomAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((double)SharedState["MarginLeadingAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("Invalid MarginLeading for this FlowDocument: [" + SharedState["MarginLeadingAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((double)SharedState["MarginTopAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("Invalid MarginTop for this FlowDocument: [" + SharedState["MarginTopAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((double)SharedState["MarginTrailingAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("Invalid MarginTrailing for this FlowDocument: [" + SharedState["MarginTrailingAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((OutlineStyles)SharedState["OutlineStylesAttribute"] != OutlineStyles.None)
            {
                TestLog.Current.LogEvidence("There is no OutlineStyles for this FlowDocument: [" + (OutlineStyles)SharedState["OutlineStylesAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((Int32)SharedState["OverlineColorAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("Invalid OverlineColor for this FlowDocument: [" + SharedState["OverlineColorAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((Int32)SharedState["StrikethroughColorAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("Invalid StrikethroughColor for this FlowDocument: [" + SharedState["StrikethroughColorAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((FlowDirections)SharedState["TextFlowDirectionsAttribute"] != FlowDirections.Default)
            {
                TestLog.Current.LogEvidence("Invalid TextFlowDirections for this FlowDocument: [" + (FlowDirections)SharedState["TextFlowDirectionsAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if ((Int32)SharedState["UnderlineColorAttribute"] != 0)
            {
                TestLog.Current.LogEvidence("Invalid UnderlineColor for this FlowDocument: [" + SharedState["UnderlineColorAttribute"] + "]");
                TestLog.Current.Result = TestResult.Fail;
            }
            
        }

        private void WaitTillTimeout(TimeSpan timeout)
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            // Set a timer to terminate our loop in the frame after the
            // timeout has expired.
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);
            timer.Interval = timeout;
            timer.Tick += delegate(object sender, EventArgs e)
            {
                ((DispatcherTimer)sender).IsEnabled = false;
                frame.Continue = false;
            };
            timer.Start();

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        public void DragMouseToSelectText(FlowDocumentScrollViewer fdsv, int startX, int startY, int endX, int endY)
        {
            //frm.Status("Moving mouse to the start of the Selection...");
            UserInput.MouseButton(fdsv, startX, startY, "Move");

            //frm.Status("MouseLeftButtonDown...");
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.LeftDown);

            //frm.Status("Move Mouse within the Anchored Block...");
            UserInput.MouseButton(fdsv, endX, endY, "Move");

            //frm.Status("MouseLeftButtonUp...");
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.LeftUp);
        }
    }
}
