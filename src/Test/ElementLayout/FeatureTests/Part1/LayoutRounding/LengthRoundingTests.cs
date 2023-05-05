// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.IO;
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Shapes;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.ElementLayout.FeatureTests.Part1
{
    /// <summary>   
    /// Test LayoutRounding in UIElements by verifying ActualWidth and ActualHeight.
    /// If LayoutRounding == true, ActualWidth and ActualHeight of any UIElement should be whole.
    /// </summary>       
    [Test(0, "Part1.LayoutRounding", "LengthRoundingTest Panels", MethodName = "Run", TestParameters = "content=LR_Panels.xaml", Disabled = true )]
    [Test(0, "Part1.LayoutRounding", "LengthRoundingTest Controls", MethodName = "Run", TestParameters = "content=LR_Controls.xaml", Disabled = true )]
    [Test(0, "Part1.LayoutRounding", "LengthRoundingTest Decorators", MethodName = "Run", TestParameters = "content=LR_Decorators.xaml", Disabled = true )]    
    [Test(0, "Part1.LayoutRounding", "LengthRoundingTest Viewers", MethodName = "Run", TestParameters = "content=LR_Viewers.xaml", Disabled = true )]
    [Test(0, "Part1.LayoutRounding", "LengthRoundingTest TextBlock_Inlines", MethodName = "Run", TestParameters = "content=LR_TextBlock_Inlines.xaml", Disabled = true )]
    [Test(0, "Part1.LayoutRounding", "LengthRoundingTest Images", MethodName = "Run", TestParameters = "content=LR_Images.xaml", Disabled = true )]
    [Test(0, "Part1.LayoutRounding", "LengthRoundingTest ExplicitLength", MethodName = "Run", TestParameters = "content=LR_ExplicitLength.xaml", Disabled = true )]
    [Test(0, "Part1.LayoutRounding", "LengthRoundingTest Shapes", MethodName = "Run", TestParameters = "content=LR_Shapes.xaml", Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "LengthRoundingTest SnapsToDevicePixels Panels", MethodName = "Run", TestParameters = "content=LR_Panels_SnapsToDevicePixels.xaml", Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "LengthRoundingTest SnapsToDevicePixels Controls", MethodName = "Run", TestParameters = "content=LR_Controls_SnapsToDevicePixels.xaml", Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "LengthRoundingTest SnapsToDevicePixels Decorators", MethodName = "Run", TestParameters = "content=LR_Decorators_SnapsToDevicePixels.xaml", Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "LengthRoundingTest Transforms Layout", MethodName = "Run", TestParameters = "content=LR_Transforms_Layout.xaml", Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "LengthRoundingTest Transforms Render", MethodName = "Run", TestParameters = "content=LR_Transforms_Render.xaml", Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "LengthRoundingTest Transforms Layout Parent Verification", MethodName = "Run", TestParameters = "content=LR_Transforms_Layout_ParentVerification.xaml", Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "LengthRoundingTest Transforms Render Parent Verification", MethodName = "Run", TestParameters = "content=LR_Transforms_Render_ParentVerification.xaml", Disabled = true )] 
    public class LengthRoundingTest : AvalonTest
    {
        private Window _testWin;
        private static readonly double s_reduceWindowWidthBy = 17; // This value was selected because as an low prime we get a lot of odd sizes to test, but not so many that the test takes too long.
        private static readonly double s_reduceWindowHeightBy = 13; // Value selection similar to above, but using a different low prime to get some different sizes to test.
        private static readonly double s_minWindowLength = 101; // This min Window size should be sufficient to get enough layout variations.

        public LengthRoundingTest()
            : base()
        {            
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);                    
        }       
        
        /// <summary>
        /// Initialize Window content.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _testWin = new Window();
            if (DriverState.DriverParameters["content"] != null)
            {
                _testWin.Content = (UIElement)XamlReader.Load(File.OpenRead(DriverState.DriverParameters["content"].ToLowerInvariant()));
            }
            else
            {
                TestLog.Current.LogEvidence("Could not find content to load!");
                return TestResult.Fail;
            }  
          
            _testWin.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;                
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// Finds the root test element and sends for processing.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTest()
        {          
            Border windowContent = _testWin.Content as Border;
            UIElement testElement = windowContent.FindName("testElement") as UIElement;
            
            do
            {
                ProcessUIElement(testElement);
            }
            while (ReduceWindowSize(s_reduceWindowWidthBy, s_reduceWindowHeightBy));
            
            return TestResult.Pass;
        }

        /// <summary>
        /// Reduces Window size if the Window is larger than a predetermined minimum size.
        /// </summary>
        /// <returns>bool</returns>
        private bool ReduceWindowSize(double reduceWidthBy, double reduceHeightBy)
        {                    
            if (_testWin.ActualWidth > s_minWindowLength && _testWin.ActualHeight > s_minWindowLength)
            {
                _testWin.Width = System.Math.Max(s_minWindowLength, _testWin.ActualWidth - reduceWidthBy);
                _testWin.Height = System.Math.Max(s_minWindowLength, _testWin.ActualHeight - reduceHeightBy);

                WaitForPriority(DispatcherPriority.ApplicationIdle);
                return true;
            }
            else
            {                
                return false;
            }            
        }

        /// <summary>
        /// Sends UIElement for verification and then processes the UIElement.
        /// </summary>       
        private void ProcessUIElement(UIElement uiElement)
        {
            if (uiElement != null)
            {
                VerifyUIElementDimensions(uiElement);
               
                if (uiElement is Panel)
                {
                    ProcessPanel(uiElement as Panel);
                }
                else if (uiElement is Decorator)
                {
                    ProcessDecorator(uiElement as Decorator);                    
                }
                else if (uiElement is ContentControl)
                {
                    ProcessContentControl(uiElement as ContentControl);
                }
                else if (uiElement is Selector)
                {
                    ProcessSelector(uiElement as Selector);
                }
                else if (uiElement is ItemsControl)
                {
                    ProcessItemsControl(uiElement as ItemsControl);
                }               
                else if (uiElement is TextBlock)
                {
                    ProcessInlineCollection(((TextBlock)uiElement).Inlines);
                }               
                else if (uiElement is FlowDocumentReader)
                {
                    ProcessFlowDocument(((FlowDocumentReader)uiElement).Document);
                }
                else if (uiElement is FlowDocumentPageViewer)
                {
                    ProcessFlowDocument(((FlowDocumentPageViewer)uiElement).Document as FlowDocument);
                }
                else if (uiElement is FlowDocumentScrollViewer)
                {
                    ProcessFlowDocument(((FlowDocumentScrollViewer)uiElement).Document);
                }               
            }
        }

        /// <summary>
        /// Processes a UIElementCollection.
        /// </summary>        
        private void ProcessUIElementCollection(UIElementCollection uiElementCollection)
        {
            if (uiElementCollection != null)
            {
                if (uiElementCollection.Count > 0)
                {
                    foreach (UIElement uiElement in uiElementCollection)
                    {
                        ProcessUIElement(uiElement);
                    }
                }
            }
        }

        /// <summary>
        /// Processes an Inline. 
        /// </summary>       
        private void ProcessInline(Inline inline)
        {
            if (inline != null)
            {
                if (inline is InlineUIContainer)
                {
                    ProcessUIElement(((InlineUIContainer)inline).Child);
                }
                else if (inline is Span)
                {
                    ProcessInlineCollection(((Span)inline).Inlines);
                }
            }
        }

        /// <summary>
        /// Processes an InlineCollection. 
        /// </summary>       
        private void ProcessInlineCollection(InlineCollection inlineCollection)
        {
            if (inlineCollection != null)
            {
                if (inlineCollection.Count > 0)
                {
                    foreach (Inline inline in inlineCollection)
                    {
                        ProcessInline(inline);
                    }
                }
            }
        }

        /// <summary>
        /// Processes a Block. 
        /// </summary>       
        private void ProcessBlock(Block block)
        {
            if (block != null)
            {
                if (block is Paragraph)
                {
                    ProcessInlineCollection(((Paragraph)block).Inlines);
                }
                else if (block is Section)
                {
                    ProcessBlockCollection(((Section)block).Blocks);
                }
                else if (block is BlockUIContainer)
                {
                    ProcessUIElement(((BlockUIContainer)block).Child);                    
                }                       
            }
        }

        /// <summary>
        /// Processes a BlockCollection.
        /// </summary>      
        private void ProcessBlockCollection(BlockCollection blockCollection)
        {
            if (blockCollection != null)
            {
                if (blockCollection.Count > 0)
                {
                    foreach (Block block in blockCollection)
                    {
                        ProcessBlock(block);
                    }
                }
            }
        }

        /// <summary>
        /// Processes a Panel. 
        /// </summary>       
        private void ProcessPanel(Panel panel)
        {
            if (panel != null)
            {
                ProcessUIElementCollection(panel.Children);
            }
        }

        /// <summary>
        /// Processes a Decorator. 
        /// </summary>       
        private void ProcessDecorator(Decorator decorator)
        {
            if (decorator != null)
            {
                ProcessUIElement(decorator.Child);
            }
        }

        /// <summary>
        /// Processes a ContentControl. 
        /// </summary>      
        private void ProcessContentControl(ContentControl contentControl)
        {
            if (contentControl != null)
            {
                if (contentControl.Content is Inline)
                {
                    ProcessInline(contentControl.Content as Inline);
                }
                else
                {
                    ProcessUIElement(contentControl.Content as UIElement);
                }
            }
        }

        /// <summary>
        /// Processes a Selector.
        /// </summary>       
        private void ProcessSelector(Selector selector)
        {
            if (selector != null)
            {
                ProcessItemCollection(selector.Items);               
            }
        }

        /// <summary>
        /// Processes an ItemsControl.
        /// </summary>       
        private void ProcessItemsControl(ItemsControl itemsControl)
        {
            if (itemsControl != null)
            {
                ProcessItemCollection(itemsControl.Items);        
            }
        }        

        /// <summary>
        /// Processes an ItemCollection.
        /// </summary>       
        private void ProcessItemCollection(ItemCollection items)
        {
            if (items != null)
            {
                foreach (object item in items)
                {
                    ProcessUIElement(item as UIElement);
                }
            }
        }  
        
        /// <summary>
        /// Processes a FlowDocument.
        /// </summary>       
        private void ProcessFlowDocument(FlowDocument flowDoc)
        {
            if (flowDoc != null)
            {
                ProcessBlockCollection(flowDoc.Blocks);
            }
        }

        /// <summary>
        /// Verifies that layout rounded UIElements have ActualWidth and ActualHeight values that are whole 
        /// </summary>       
        private void VerifyUIElementDimensions(UIElement uiElement)
        {            
            if (uiElement is Shape)
            {
                // Only Rectangle or Ellipse shapes have a true Width/Height
                if (uiElement is Rectangle || uiElement is Ellipse)
                {}
                else
                {
                    return;
                }
            }
            
            double actualWidth = 1.1;
            double actualHeight = 1.1;
           
            if (uiElement is FrameworkElement)
            {
                actualWidth = ((FrameworkElement)uiElement).ActualWidth;
                actualHeight = ((FrameworkElement)uiElement).ActualHeight;
            }
            else
            {
                TestLog.Current.LogEvidence(string.Format("Could not identify UIElement ({0}) type!", uiElement.ToString()));
                TestLog.Current.Result = TestResult.Fail;
                return;
            }
                                 
            if (!LayoutRoundingCommon.IsLengthRounded(actualWidth, true) || !LayoutRoundingCommon.IsLengthRounded(actualHeight, false))
            {
                TestLog.Current.LogEvidence("Encountered a non rounded width or height!");
                TestLog.Current.LogEvidence(string.Format("{0}.UseLayoutRounding = {1}", uiElement.GetType().ToString(), ((FrameworkElement)uiElement).UseLayoutRounding));
                TestLog.Current.LogEvidence(string.Format("ActualWidth: {0}, ActualHeight: {1}", actualWidth, actualHeight));
                TestLog.Current.Result = TestResult.Fail;
            }           
        }       
    }
}
