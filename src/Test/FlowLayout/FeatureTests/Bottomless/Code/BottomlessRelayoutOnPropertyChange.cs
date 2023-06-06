// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Testing regression test.   
    /// </summary>
    [Test(3, "Bottomless", "BottomlessRelayoutOnPropertyChange", Timeout=480)]
    class BottomlessRelayoutOnPropertyChange : UIRelayoutTestCase
    {
        private string _content;
        private Window _testWin;

        [Variation("BottomlessRelayoutOnPropertyChange_Figure.xaml")]
        [Variation("BottomlessRelayoutOnPropertyChange_Floater.xaml")]
        [Variation("BottomlessRelayoutOnPropertyChange_List.xaml")]
        [Variation("BottomlessRelayoutOnPropertyChange_Paragraph.xaml")]
        [Variation("BottomlessRelayoutOnPropertyChange_Section.xaml")]
        [Variation("BottomlessRelayoutOnPropertyChange_Table.xaml")]
        public BottomlessRelayoutOnPropertyChange(string content)            
        {
            CreateLog = false;
            this._content = content;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
        }
        
        /// <summary>
        /// Initialize: setup tests
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {            
            _testWin = (Window)TestWin.Launch(typeof(Window), 600, 600, 0, 0, _content, true, "LayoutTestWindow");
            _testWin.Show();

            FlowDocumentScrollViewer containingFDSV = _testWin.Content as FlowDocumentScrollViewer;

            List<TextElement> textElements = new List<TextElement>();
            GetTextElementsRecursively(_testWin, textElements);

            foreach (TextElement textElement in textElements)
            {
                foreach (TestStep step in CreateContentRelayoutTestSteps(containingFDSV, textElement))
                {
                    RunSteps += step;
                }
            }

            CommonFunctionality.FlushDispatcher(containingFDSV.Dispatcher); ; //allow defered changes to take effect
            CommonFunctionality.FlushDispatcher(containingFDSV.Dispatcher); ; //allow defered changes to take effect
            
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }
        
        private IEnumerable<TestStep> CreateContentRelayoutTestSteps(FlowDocumentScrollViewer viewer, TextElement te)
        {
            if (viewer == null)
            {
                yield return new TestStep(
                    delegate
                    {
                        GlobalLog.LogEvidence("The test document must be a FlowDocumentScrollViewer");
                        return TestResult.Fail;
                    }
                );
            }

            foreach(TestStep changeContentStep in CreateChangeContentSteps(te)) 
            {
                CreateListenForRelayoutPair listenForRelayoutPair = new CreateListenForRelayoutPair(this, viewer, ExpectedFlags.Expected);

                yield return listenForRelayoutPair.StartListeningStep;
                yield return changeContentStep;
                yield return delegate { 
                    CommonFunctionality.FlushDispatcher(viewer.Dispatcher);
                    CommonFunctionality.FlushDispatcher(viewer.Dispatcher);
                    return TestResult.Pass; 
                };
                yield return listenForRelayoutPair.StopListeningStep;
            }
        }

        private IEnumerable<TestStep> CreateChangeContentSteps(ContentElement ce)
        {
            TestLog log = new TestLog("Step for " + ce.GetType().ToString());

            TextElement te = ce as TextElement;
            if (te != null)
            {
                string fontFamilyName1 = "Lucida Console";
                string fontFamilyName2 = "Arial";

                yield return delegate
                {
                    bool useFontFamily2 = 0 == string.Compare(te.FontFamily.Source, fontFamilyName1, true, System.Globalization.CultureInfo.InvariantCulture);
                    te.FontFamily = new FontFamily(useFontFamily2 ? fontFamilyName2 : fontFamilyName1);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.FontSize += 10;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.FontStretch = (te.FontStretch == FontStretches.Condensed) ? FontStretches.Normal : FontStretches.Condensed;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.FontStyle = (te.FontStyle == FontStyles.Italic) ? FontStyles.Normal : FontStyles.Italic;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.FontWeight = (te.FontWeight == FontWeights.Black) ? FontWeights.Normal : FontWeights.Black;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.Typography.Capitals = (te.Typography.Capitals == FontCapitals.AllSmallCaps) ? FontCapitals.Normal : FontCapitals.AllSmallCaps;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.Typography.StandardLigatures = !te.Typography.StandardLigatures;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.Typography.CapitalSpacing = !te.Typography.CapitalSpacing;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.Typography.CapitalSpacing = !te.Typography.CapitalSpacing;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.Typography.CapitalSpacing = !te.Typography.CapitalSpacing;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.Typography.ContextualAlternates = !te.Typography.ContextualAlternates;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    te.Typography.Kerning = !te.Typography.Kerning;
                    return TestResult.Pass;
                };
            }

            TableColumn tcole = ce as TableColumn;
            if (tcole != null)
            {
                yield return delegate
                {
                    tcole.Width = new GridLength(tcole.Width.Value + 1.0, tcole.Width.GridUnitType);
                    return TestResult.Pass;
                };
            }

            Block be = ce as Block;
            if (be != null)
            {
                yield return delegate
                {
                    be.BorderThickness = new Thickness(12, 15, 10, 5);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    be.BreakColumnBefore = !be.BreakColumnBefore;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    be.BreakPageBefore = !be.BreakPageBefore;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    be.ClearFloaters = (be.ClearFloaters == WrapDirection.Left) ? WrapDirection.Both : WrapDirection.Left;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    be.FlowDirection = (be.FlowDirection == FlowDirection.LeftToRight) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    be.LineHeight += 3;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    be.Margin = new Thickness(2, 8, 4, 6);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    be.Padding = new Thickness(8, 10, 14, 6);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    be.TextAlignment = (be.TextAlignment == TextAlignment.Left) ? TextAlignment.Right : TextAlignment.Left;
                    return TestResult.Pass;
                };
            }

            List list = ce as List;
            if (list != null)
            {
                yield return delegate
                {
                    list.MarkerOffset += 8;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    list.MarkerStyle = (list.MarkerStyle == TextMarkerStyle.Box) ? TextMarkerStyle.Circle : TextMarkerStyle.Box;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    list.StartIndex += 1;
                    return TestResult.Pass;
                };
            }

            ListItem lie = ce as ListItem;
            if (lie != null)
            {
                yield return delegate
                {
                    lie.BorderThickness = new Thickness(12, 15, 10, 5);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    lie.FlowDirection = (lie.FlowDirection == FlowDirection.LeftToRight) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    lie.LineHeight += 1.8;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    lie.Margin = new Thickness(2, 8, 4, 6);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    lie.Padding = new Thickness(8, 10, 14, 6);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    lie.TextAlignment = (lie.TextAlignment == TextAlignment.Left) ? TextAlignment.Right : TextAlignment.Left;
                    return TestResult.Pass;
                };
            }

            Table table = ce as Table;
            if (table != null)
            {
                yield return delegate
                {
                    table.CellSpacing += 0.47;
                    return TestResult.Pass;
                };
            }

            TableCell tce = ce as TableCell;
            if (tce != null)
            {
                yield return delegate
                {
                    tce.BorderThickness = new Thickness(12, 15, 10, 5);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    tce.ColumnSpan += 1;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    tce.FlowDirection = (tce.FlowDirection == FlowDirection.LeftToRight) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    tce.LineHeight += 1.8;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    tce.Padding = new Thickness(8, 10, 14, 6);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    tce.RowSpan += 1;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    tce.TextAlignment = (tce.TextAlignment == TextAlignment.Left) ? TextAlignment.Right : TextAlignment.Left;
                    return TestResult.Pass;
                };
            }

            TableRow tre = ce as TableRow;

            Paragraph para = ce as Paragraph;
            if (para != null)
            {
                yield return delegate
                {
                    para.KeepTogether = !para.KeepTogether;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    para.KeepWithNext = !para.KeepWithNext;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    para.MinOrphanLines += 1;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    para.MinWidowLines += 1;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    if (para.TextDecorations == null)
                    {
                        para.TextDecorations = TextDecorations.Underline;
                    }
                    else
                    {
                        if (para.TextDecorations.Count < 1)
                        {
                            para.TextDecorations = TextDecorations.Underline;
                        }
                        else
                        {
                            para.TextDecorations = null;
                        }
                    }
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    para.TextIndent += 3.2;
                    return TestResult.Pass;
                };
            }

            Inline inline = ce as Inline;
            if (inline != null)
            {
                yield return delegate
                {
                    inline.BaselineAlignment = (inline.BaselineAlignment == BaselineAlignment.Center) ? BaselineAlignment.Top : BaselineAlignment.Center;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    if (inline.TextDecorations == null)
                    {
                        inline.TextDecorations = TextDecorations.Underline;
                    }
                    else
                    {
                        if (inline.TextDecorations.Count > 0)
                        {
                            inline.TextDecorations = null;
                        }
                        else
                        {
                            inline.TextDecorations = TextDecorations.Underline;
                        }
                    }
                    return TestResult.Pass;
                };
            }

            Run re = ce as Run;
            if (re != null)
            {
                yield return delegate
                {
                    re.Text += "Some new content";
                    return TestResult.Pass;
                };
            }

            AnchoredBlock anchoredBlock = ce as AnchoredBlock;
            if (anchoredBlock != null)
            {
                yield return delegate
                {
                    anchoredBlock.BorderThickness = new Thickness(12, 15, 10, 5);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    anchoredBlock.FlowDirection = (anchoredBlock.FlowDirection == FlowDirection.LeftToRight) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    anchoredBlock.LineHeight += 3.01;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    anchoredBlock.Margin = new Thickness(2, 8, 4, 6);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    anchoredBlock.Padding = new Thickness(8, 10, 14, 6);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    anchoredBlock.TextAlignment = (anchoredBlock.TextAlignment == TextAlignment.Left) ? TextAlignment.Right : TextAlignment.Left;
                    return TestResult.Pass;
                };
            }

            Figure figure = ce as Figure;
            if (figure != null)
            {
                yield return delegate
                {
                    figure.CanDelayPlacement = !figure.CanDelayPlacement;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    figure.Height = new FigureLength(figure.Height.Value + 5.0, figure.Height.FigureUnitType);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    figure.HorizontalAnchor = (figure.HorizontalAnchor == FigureHorizontalAnchor.PageRight) ? FigureHorizontalAnchor.PageLeft : FigureHorizontalAnchor.PageRight;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    figure.HorizontalOffset += 12;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    figure.VerticalAnchor = (figure.VerticalAnchor == FigureVerticalAnchor.PageTop) ? FigureVerticalAnchor.PageBottom : FigureVerticalAnchor.PageTop;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    figure.VerticalOffset += 8;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    figure.Width = new FigureLength(figure.Width.Value + 3.0, figure.Width.FigureUnitType);
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    figure.WrapDirection = (figure.WrapDirection == WrapDirection.Left) ? WrapDirection.Both : WrapDirection.Left;
                    return TestResult.Pass;
                };
            }

            Floater floater = ce as Floater;
            if (floater != null)
            {
                yield return delegate
                {
                    floater.HorizontalAlignment = (floater.HorizontalAlignment == HorizontalAlignment.Left) ? HorizontalAlignment.Stretch : HorizontalAlignment.Right;
                    return TestResult.Pass;
                };

                yield return delegate
                {
                    floater.Width += 12;
                    return TestResult.Pass;
                };
            }
             log.Result = TestResult.Pass;
             log.Close();
        }

        private void GetTextElementsRecursively(DependencyObject dObj, List<TextElement> results)
        {
            if (dObj == null)
            {
                return;
            }

            TextElement textElement = dObj as TextElement;
            if (textElement != null)
            {
                results.Add(textElement);
            }

            foreach (object o in LogicalTreeHelper.GetChildren(dObj))
            {
                GetTextElementsRecursively(o as DependencyObject, results);
            }
        }

        private class CreateListenForRelayoutPair
        {
            TestStep _start = null;
            TestStep _stop = null;

            public CreateListenForRelayoutPair(UIRelayoutTestCase tc, FlowDocumentScrollViewer viewer, ExpectedFlags flags)
            {
                _start = delegate
                {
                    RelayoutListener listener = tc.BeginRelayoutTest(viewer, flags);

                    _stop = delegate {
                        TestResult result = listener.GetResult();
                        ((IDisposable)listener).Dispose();
                        return result;
                    };

                    return TestResult.Pass;
                };
            }

            public TestStep StartListeningStep {
                get { return _start; }
            }

            public TestStep StopListeningStep
            {
                get { return _stop; }
            }
        }
    }
}
