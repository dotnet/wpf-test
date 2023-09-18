// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies rendering of SelectionBrush and CaretBrush in Editing controls
//  with SelectionBrush, SelectionOpacity and CaretBrush defined in Bindings

using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Data;
using Media = System.Windows.Media;

using Microsoft.Test.Discovery;
using Microsoft.Test.Imaging;
using MTI = Microsoft.Test.Input;

using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Microsoft.Test.Editing
{
    [Test(2, "SelectionAndCaret", "BindingSelectionAndCaretBrushTest", MethodParameters = "/TestCaseType:BindingSelectionAndCaretBrushTest", Timeout = 120)]    
    // This test verifies rendering of OpacityBrush, SelectionBrush, and SelectionOpacity in controls where it is supported.  
    // The values for these properties for this test are controlled through DataBinding.           
    public class BindingSelectionAndCaretBrushTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _caretBrushRectangle = new System.Windows.Shapes.Rectangle();
            _caretBrushRectangle.Width = 100;
            _caretBrushRectangle.Height = 25;            

            _selectionBrushRectangle = new System.Windows.Shapes.Rectangle();
            _selectionBrushRectangle.Width = 100;
            _selectionBrushRectangle.Height = 25;
            _selectionBrushRectangle.Margin = new Thickness(10, 0, 10, 0);

            _selectionOpacityTextBox = new TextBox();
            _selectionOpacityTextBox.Width = 100;
            _selectionOpacityTextBox.Height = 25;

            StackPanel rectangleContainer = new StackPanel();
            rectangleContainer.Orientation = Orientation.Horizontal;
            rectangleContainer.Height = 30;
            rectangleContainer.Children.Add(_caretBrushRectangle);
            rectangleContainer.Children.Add(_selectionBrushRectangle);
            rectangleContainer.Children.Add(_selectionOpacityTextBox);
          
            CreateTestControlByType();

            _control.FontSize = 24;            
            _control.Height = 230;
            _control.Width = 300;           
            _control.FontFamily = new Media.FontFamily("Tahoma");
                        
            if (_control is TextBoxBase || _control is PasswordBox)
            {
                _isCaretTest = true;
                _caretVerifier = new CaretVerifier(_control);
                _wrapper = new UIElementWrapper(_control);
                _wrapper.Text = _testContent;
            }
            else
            {
                _isCaretTest = false;
                SetFlowDocumentViewerControlText();
            }

            SetTestValues();
                
            StackPanel panel = new StackPanel();
            panel.Children.Add(rectangleContainer);
            panel.Children.Add(_control);
            MainWindow.Content = panel;             

            QueueDelegate(DoVerification);
        }

        private void SetTestValues()
        {                       
            // Create Bindings             
            Binding caretBrushBinding = new Binding("Fill");
            caretBrushBinding.Source = _caretBrushRectangle;

            Binding selectionBrushBinding = new Binding("Fill");
            selectionBrushBinding.Source = _selectionBrushRectangle;

            Binding selectionOpacityBinding = new Binding("Text");
            selectionOpacityBinding.Source = _selectionOpacityTextBox;            

            //Set Bindings
            if (_control is TextBoxBase)
            {
                _control.SetBinding(TextBoxBase.SelectionBrushProperty, selectionBrushBinding);
                _control.SetBinding(TextBoxBase.SelectionOpacityProperty, selectionOpacityBinding);
                _control.SetBinding(TextBoxBase.CaretBrushProperty, caretBrushBinding);
            }
            else if (_control is PasswordBox)
            {
                _control.SetBinding(PasswordBox.SelectionBrushProperty, selectionBrushBinding);
                _control.SetBinding(PasswordBox.SelectionOpacityProperty, selectionOpacityBinding);
                _control.SetBinding(PasswordBox.CaretBrushProperty, caretBrushBinding);
            }
            else if (_control is FlowDocumentReader)
            {
                _control.SetBinding(FlowDocumentReader.SelectionBrushProperty, selectionBrushBinding);
                _control.SetBinding(FlowDocumentReader.SelectionOpacityProperty, selectionOpacityBinding);
            }
            else if (_control is FlowDocumentPageViewer)
            {
                _control.SetBinding(FlowDocumentPageViewer.SelectionBrushProperty, selectionBrushBinding);
                _control.SetBinding(FlowDocumentPageViewer.SelectionOpacityProperty, selectionOpacityBinding);
            }
            else if (_control is FlowDocumentScrollViewer)
            {
                _control.SetBinding(FlowDocumentScrollViewer.SelectionBrushProperty, selectionBrushBinding);
                _control.SetBinding(FlowDocumentScrollViewer.SelectionOpacityProperty, selectionOpacityBinding);
            }
        }

        private void DoVerification()
        {
            // Set properties on Binding targets            
            _caretBrushRectangle.Fill = _testCaretBrush;
            _selectionBrushRectangle.Fill = _testSelectionBrush;
            _selectionBrushRectangle.Opacity = _testSelectionOpacity;
            _selectionOpacityTextBox.Text = _testSelectionOpacity.ToString(CultureInfo.InvariantCulture);

            // Wait for binding            
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
           
            // Do simple property value verification before visual verification            
            VerifyInitialPropertyValues();
            
            _control.Focus();

            if (_isCaretTest)
            {                
                VisuallyVerifyCaretRendering();
            }

            VisuallyVerifySelectionRendering();

            // Set properties on Binding targets to test property update           
            _caretBrushRectangle.Fill = _testCaretBrush = Media.Brushes.Purple;
            _selectionBrushRectangle.Fill = _testSelectionBrush = Media.Brushes.Pink;
            _selectionBrushRectangle.Opacity = _testSelectionOpacity = 0.3;
            _selectionOpacityTextBox.Text = _testSelectionOpacity.ToString(CultureInfo.InvariantCulture);

            // Wait for binding            
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            if (_isCaretTest)
            {
                // Clear selection before verifying caret
                _wrapper.Select(0, 0);
                VisuallyVerifyCaretRendering();
            }

            VisuallyVerifySelectionRendering();

            NextCombination();                        
        }

        private void VerifyInitialPropertyValues()
        {
            // Verify value for CaretBrush, SelectionBrush, and SelectionOpacity
            double currentSelectionOpacity = -1;
            Media.Brush currentSelectionBrush = null;
            Media.Brush currentCaretBrush = null;

            if (_control is TextBoxBase)
            {
                if (_isCaretTest)
                {
                    currentCaretBrush = ((TextBoxBase)_control).CaretBrush;
                }
                currentSelectionBrush = ((TextBoxBase)_control).SelectionBrush;
                currentSelectionOpacity = ((TextBoxBase)_control).SelectionOpacity;
            }
            else if (_control is PasswordBox)
            {
                if (_isCaretTest)
                {
                    currentCaretBrush = ((PasswordBox)_control).CaretBrush;
                }
                currentSelectionBrush = ((PasswordBox)_control).SelectionBrush;
                currentSelectionOpacity = ((PasswordBox)_control).SelectionOpacity;
            }
            else if (_control is FlowDocumentReader)
            {                
                currentSelectionBrush = ((FlowDocumentReader)_control).SelectionBrush;
                currentSelectionOpacity = ((FlowDocumentReader)_control).SelectionOpacity;
            }
            else if (_control is FlowDocumentPageViewer)
            {               
                currentSelectionBrush = ((FlowDocumentPageViewer)_control).SelectionBrush;
                currentSelectionOpacity = ((FlowDocumentPageViewer)_control).SelectionOpacity;
            }
            else if (_control is FlowDocumentScrollViewer)
            {               
                currentSelectionBrush = ((FlowDocumentScrollViewer)_control).SelectionBrush;
                currentSelectionOpacity = ((FlowDocumentScrollViewer)_control).SelectionOpacity; 
            }

            if (_isCaretTest)
            {                
                Verifier.Verify(currentCaretBrush == _testCaretBrush,
                        "Verifying initial CaretBrush value. Current value [" +
                        currentCaretBrush.ToString() + "] Expected value [" + _testCaretBrush.ToString() + "]", true);
            }
            
            Verifier.Verify(currentSelectionBrush == _testSelectionBrush,
                    "Verifying initial SelectionBrush value. Current value [" +
                    currentSelectionBrush.ToString() + "] Expected value [" + _testSelectionBrush.ToString() + "]", true);
            
            Verifier.Verify(currentSelectionOpacity == _testSelectionOpacity,
                    "Verifying initial SelectionOpacity value. Current value [" +
                    currentSelectionOpacity + "] Expected value [" + _testSelectionOpacity + "]", true);           
        }

        private void VisuallyVerifyCaretRendering()
        {
            // Global state of caret is not affected by this operation.
            _caretVerifier.StopCaretBlinking();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            Bitmap controlSnapshot = BitmapCapture.CreateBitmapFromElement(_caretVerifier.Element);

            // Global state of caret is not affected by this operation.
            _caretVerifier.StartCaretBlinking();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            Rect caretRect = _caretVerifier.GetExpectedCaretRect();
            // Scale rect to Bitmap's DPI
            caretRect = BitmapUtils.AdjustBitmapSubAreaForDpi(controlSnapshot, caretRect);
            Bitmap caretSnapshot = BitmapUtils.CreateSubBitmap(controlSnapshot, caretRect);

            int colorPixelsInCaretSnapshot;
            Media.Color expectedCaretRenderColor = GetExpectedRenderColor(_caretBrushRectangle);
            colorPixelsInCaretSnapshot = BitmapUtils.CountColoredPixels(caretSnapshot, expectedCaretRenderColor);
            Log("Number of color pixels in the caret snapshot: " + colorPixelsInCaretSnapshot);

            // Number of color pixels in the caret snapshot is expected to be atleast of FontSize height
            if (colorPixelsInCaretSnapshot < testFontSize)
            {
                Logger.Current.LogImage(caretSnapshot, "caretSnapShot");
                Logger.Current.LogImage(controlSnapshot, "controlSnapShot");
                Verifier.Verify(false, "Verifying that we have expected red pixels in the caretSnapshot", true);
            }
        }

        private void VisuallyVerifySelectionRendering()
        {
            if (_control is TextBoxBase || _control is PasswordBox)
            {
                _wrapper.SelectAll();
            }
            else
            {
                // Ctrl+A to Select all content in the control. 
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, true);
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.A, true);
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.A, false);
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, false);
            }

            // Wait for the selection to appear
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            Media.Color expectedSelectionRenderColor = GetExpectedRenderColor(_selectionBrushRectangle);

            Bitmap selectionSnapshot = GetSelectionScreenShot(_control);

            VerifySelectionColor(selectionSnapshot, expectedSelectionRenderColor);
        }

        private Media.Color GetExpectedRenderColor(System.Windows.Shapes.Rectangle rectangle)
        {
            Bitmap rectangleSnapshot = BitmapCapture.CreateBitmapFromElement(rectangle);
            System.Drawing.Color color = rectangleSnapshot.GetPixel(rectangleSnapshot.Width / 2, rectangleSnapshot.Height / 2);
            return Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private Bitmap GetSelectionScreenShot(Control control)
        {            
            Bitmap controlSnapshot = BitmapCapture.CreateBitmapFromElement(control);
            Rect leadingRect, trailingRect;

            // Get bounding rectangle of selection area
            if (control is PasswordBox)
            {
                object passwordBoxSelection = ReflectionUtils.GetProperty(control, "Selection");
                object startPointer = ReflectionUtils.GetInterfaceProperty(passwordBoxSelection, "ITextRange", "Start");
                object endPointer = ReflectionUtils.GetInterfaceProperty(passwordBoxSelection, "ITextRange", "End");
                
                leadingRect = (Rect)ReflectionUtils.InvokeInterfaceMethod(startPointer, "ITextPointer", "GetCharacterRect", new object[] { LogicalDirection.Forward });
                trailingRect = (Rect)ReflectionUtils.InvokeInterfaceMethod(endPointer, "ITextPointer", "GetCharacterRect", new object[] { LogicalDirection.Backward });
            }
            else if (control is TextBoxBase)
            {               
                leadingRect = _wrapper.SelectionInstance.Start.GetCharacterRect(LogicalDirection.Forward);
                trailingRect = _wrapper.SelectionInstance.End.GetCharacterRect(LogicalDirection.Backward);
            }
            else 
            {
                TextSelection selection = GetTextSelectionFromFlowDocumentViewer();               
                leadingRect = selection.Start.GetCharacterRect(LogicalDirection.Forward);
                trailingRect = selection.End.GetCharacterRect(LogicalDirection.Backward);                
            }
           
            Rect selectionBoundingRect = new Rect(leadingRect.TopLeft, trailingRect.BottomRight);
         
            Bitmap selectionSnapshot = BitmapUtils.CreateSubBitmap(controlSnapshot, selectionBoundingRect);            
            selectionSnapshot = BitmapUtils.CreateBorderlessBitmap(selectionSnapshot, removeBorderThickness);                        

            return selectionSnapshot;
        }

        private void VerifySelectionColor(Bitmap selectionSnapshot, Media.Color expectedColor)
        {            
            int coloredPixels;
            coloredPixels = BitmapUtils.CountColoredPixels(selectionSnapshot, expectedColor);            

            int totalSelectionPixels = selectionSnapshot.Size.Width * selectionSnapshot.Size.Height;

            Log("Expected selection render color: " + expectedColor.ToString());
            Log("Total number of pixels in the captured selection area: " + totalSelectionPixels);
            Log("Number of pixels that matched expected color: " + coloredPixels);            

            // Log the snapshot if verification will fail for analysis.
            // Specially we check against two things:
            // 1. Verify that selection has the min # of pixels in it.
            // 2. Number of pixels with expected pixel color are atleast 50% of the selection area being observed. This 
            // is to take into account the text and selection overlapped pixels (which will not have the expected pixel color).

            // Log the snapshots only if the verification fails
            if ((totalSelectionPixels < minimumSelectionPixels) || coloredPixels < (totalSelectionPixels / 2))
            {
                Logger.Current.LogImage(selectionSnapshot, "selectionSnapshot");
                Logger.Current.LogImage(BitmapCapture.CreateBitmapFromElement(_control), "controlSnapshot");
            }
            Verifier.Verify(totalSelectionPixels > minimumSelectionPixels, 
                "Verify that selection has the min # of pixels in it", true);            
            // Expectations are that atleast 50% of the pixels should have pure selection brush color without overlapping of text color
            Verifier.Verify(coloredPixels > (totalSelectionPixels / 2), 
                "Verifying that number of pure selection brush color pixels is more than half the total pixels from the snapshot", true);
        }
       
        private void CreateTestControlByType()
        {
            if (_testControlType == typeof(TextBox))
            {
                _control = new TextBox();
            }
            else if (_testControlType == typeof(RichTextBox))
            {
                _control = new RichTextBox();
            }
            else if (_testControlType == typeof(PasswordBox))
            {
                _control = new PasswordBox();
            }
            else if (_testControlType == typeof(FlowDocumentReader))
            {
                _control = new FlowDocumentReader();
            }
            else if (_testControlType == typeof(FlowDocumentPageViewer))
            {
                _control = new FlowDocumentPageViewer();
            }
            else if (_testControlType == typeof(FlowDocumentScrollViewer))
            {
                _control = new FlowDocumentScrollViewer();
            }
        }

        private void SetFlowDocumentViewerControlText()
        {            
            FlowDocument flowDoc = new FlowDocument(new Paragraph(new Run(_testContent)));
            flowDoc.FontSize = 24;
            
            if (_control is FlowDocumentReader)
            {
                ((FlowDocumentReader)_control).Document = flowDoc;
            }
            else if (_control is FlowDocumentPageViewer)
            {
                ((FlowDocumentPageViewer)_control).Document = flowDoc;
            }
            else if (_control is FlowDocumentScrollViewer)
            {
                ((FlowDocumentScrollViewer)_control).Document = flowDoc;
            }
        }

        private TextSelection GetTextSelectionFromFlowDocumentViewer()
        {
            if (_control is FlowDocumentReader)
            {
                return ((FlowDocumentReader)_control).Selection;
            }
            else if (_control is FlowDocumentPageViewer)
            {
                return ((FlowDocumentPageViewer)_control).Selection;
            }
            else
            {
                return ((FlowDocumentScrollViewer)_control).Selection;
            }            
        }
       
        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values                
        private Type _testControlType = null;                         

        private Control _control = null;
        private UIElementWrapper _wrapper = null;
        // Rectangle object (configured with test values for SelectionBrush and SelectionOpacity)
        // is used to get the expected selection rendering color during rendering verification.
        private System.Windows.Shapes.Rectangle _caretBrushRectangle = null;
        private System.Windows.Shapes.Rectangle _selectionBrushRectangle = null;
        private TextBox _selectionOpacityTextBox = null;
        private Media.Brush _testSelectionBrush = Media.Brushes.LightGreen;
        private Media.Brush _testCaretBrush = Media.Brushes.Red;         
        private double _testSelectionOpacity = 0.8;        
        private CaretVerifier _caretVerifier = null;      
        private string _testContent = "Some test content";
        private bool _isCaretTest = false;
        private const double testFontSize = 24;

        // Used to remove few pixels of the boundaries from the snapshot 
        // to avoid any aliasing at the borders.
        private const int removeBorderThickness = 2;

        // 1000 is just a heuristic number which works across all variations 
        // to make sure the selection snapshot is not empty.
        private const int minimumSelectionPixels = 1000;

        #endregion
    }
}