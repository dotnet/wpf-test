// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies rendering of selection in Editing controls with SelectionBrush
//  and SelectionOpacity defined in Styles and triggers.

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Media = System.Windows.Media;


using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Imaging;
using MTI = Microsoft.Test.Input;

namespace Microsoft.Test.Editing
{
    [Test(2, "Selection", "SelectionRenderingStyleAndTriggerTest", MethodParameters = "/TestCaseType:SelectionRenderingStyleAndTriggerTest", Timeout = 120)]    
    // This test verifies rendering of SelectionBrush in controls where it is supported by setting these values in a style: 
    // TextBox, RichTextBox, PasswordBox, FlowDocumentReader, FlowDocumentScrollViewer, FlowDocumentPageViewer    
    public class SelectionRenderingStyleAndTriggerTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _rectangle = new System.Windows.Shapes.Rectangle();
            _rectangle.Width = 100;
            _rectangle.Height = 25;
          
            CreateTestControlByType();

            _control.FontSize = 24;            
            _control.Height = 230;
            _control.Width = 300;           
            _control.FontFamily = new Media.FontFamily("Tahoma");

            SetTestValues();
            
            if (_control is TextBoxBase || _control is PasswordBox)
            {
                _wrapper = new UIElementWrapper(_control);
                _wrapper.Text = _testContent;
            }
            else
            {
                SetFlowDocumentViewerControlText();
            }
                
            StackPanel panel = new StackPanel();
            panel.Children.Add(_rectangle);
            panel.Children.Add(_control);
            MainWindow.Content = panel;             

            QueueDelegate(DoVisualVerification);
        }

        private void SetTestValues()
        {            
            _testSelectionBrush = GetTestSelectionBrush();
            _rectangle.Fill = _testSelectionBrush;
            _rectangle.Opacity = _testSelectionOpacity;

            // Set SelectionBrush and SelectionOpacity through style and create a visual trigger for which it will change when mouseover == true
            _control.Style = GetStyleForTest();                
        }

        private void DoVisualVerification()
        {
            // Make sure the mouse is not over the control
            MTI.Input.MoveTo(new System.Windows.Point(0, 0));

            // Wait for the mouse to move out of the way
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            if (_control.IsMouseOver)
            {
                Log("This test may fail because the mouse is still over the control!");
            }

            // Do simple property value verification before visual verification
            VerifyPropertyValuesAfterRender();

            _control.Focus();

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
           
            _expectedSelectionRenderColor = GetExpectedSelectionRenderColor();

            Bitmap selectionSnapshot = GetSelectionScreenShot(_control);

            // Verify selection color from styled control
            VerifySelectionColor(selectionSnapshot, _expectedSelectionRenderColor, false);

            // Reset the rectangle for comparison after trigger is set off
            _rectangle.Fill = _triggerBrush;
            _rectangle.Opacity = _triggerOpacity;

            // Move the mouse over the control to set off the trigger
            MTI.Input.MoveTo(_control);

            // Wait for the mouse to move and the selection to change color
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            _expectedSelectionRenderColor = GetExpectedSelectionRenderColor();

            selectionSnapshot = GetSelectionScreenShot(_control);

            // Verify selection color after the property has been changed via a trigger
            VerifySelectionColor(selectionSnapshot, _expectedSelectionRenderColor, true);
        }

        private void VerifyPropertyValuesAfterRender()
        {
            // Verify value for SelectionBrush
            Media.Brush currentSelectionBrush = null;
            if (_control is TextBoxBase)
            {
                currentSelectionBrush = ((TextBoxBase)_control).SelectionBrush;
            }
            else if (_control is PasswordBox)
            {
                currentSelectionBrush = ((PasswordBox)_control).SelectionBrush;
            }
            else if (_control is FlowDocumentReader)
            {
                currentSelectionBrush = ((FlowDocumentReader)_control).SelectionBrush;
            }
            else if (_control is FlowDocumentPageViewer)
            {
                currentSelectionBrush = ((FlowDocumentPageViewer)_control).SelectionBrush;
            }
            else if (_control is FlowDocumentScrollViewer)
            {
                currentSelectionBrush = ((FlowDocumentScrollViewer)_control).SelectionBrush;
            }

            if ((_testSelectionBrush != null) && (currentSelectionBrush != null))
            {
                Log("Current SelectionBrush value [" + currentSelectionBrush.ToString() + "]");
                Log("Expected SelectionBrush value [" + _testSelectionBrush.ToString() + "]");
            }
            Verifier.Verify(currentSelectionBrush == _testSelectionBrush,
                    "Verifying SelectionBrush value after property is set", true);

            // Verify value for SelectionOpacity
            double currentSelectionOpacity = -1;
            if (_control is TextBoxBase)
            {
                currentSelectionOpacity = ((TextBoxBase)_control).SelectionOpacity;
            }
            else if (_control is PasswordBox)
            {
                currentSelectionOpacity = ((PasswordBox)_control).SelectionOpacity;
            }
            else if (_control is FlowDocumentReader)
            {
                currentSelectionOpacity = ((FlowDocumentReader)_control).SelectionOpacity;
            }
            else if (_control is FlowDocumentPageViewer)
            {

                currentSelectionOpacity = ((FlowDocumentPageViewer)_control).SelectionOpacity;
            }
            else if (_control is FlowDocumentScrollViewer)
            {
                currentSelectionOpacity = ((FlowDocumentScrollViewer)_control).SelectionOpacity;
            }

            Verifier.Verify(currentSelectionOpacity == _testSelectionOpacity,
                    "Verifying SelectionOpacity value after property is set. Current value [" +
                    currentSelectionOpacity + "] Expected value [" + _testSelectionOpacity + "]", true);
        }

        private Media.Color GetExpectedSelectionRenderColor()
        {
            Bitmap rectangleSnapshot = BitmapCapture.CreateBitmapFromElement(_rectangle);
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

        private void VerifySelectionColor(Bitmap selectionSnapshot, Media.Color expectedColor, bool canRunNextCombination)
        {            
            int coloredPixels;
            coloredPixels = BitmapUtils.CountColoredPixels(selectionSnapshot, expectedColor);            

            int totalSelectionPixels = selectionSnapshot.Size.Width * selectionSnapshot.Size.Height;

            Log("Expected selection render color: " + _expectedSelectionRenderColor.ToString());
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

            if (canRunNextCombination)
            {
                NextCombination();
            }
        }

        // Create a style that sets values for the Selection brush and opacity and a trigger that will 
        // update these properties when the control is moused over
        private Style GetStyleForTest()
        {            
            Style controlStyle = new Style(_testControlType);
            
            // Setters for SelectionBrush and SelectionOpacity
            Setter selectionBrushSetter = new Setter();
            Setter selectionOpacitySetter = new Setter();
            
            if (_control is TextBoxBase)
            {
                selectionBrushSetter.Property = TextBoxBase.SelectionBrushProperty;                
                selectionOpacitySetter.Property = TextBoxBase.SelectionOpacityProperty;               
            }
            else if (_control is PasswordBox)
            {
                selectionBrushSetter.Property = PasswordBox.SelectionBrushProperty;               
                selectionOpacitySetter.Property = PasswordBox.SelectionOpacityProperty;                
            }
            else if (_control is FlowDocumentReader)
            {
                selectionBrushSetter.Property = FlowDocumentReader.SelectionBrushProperty;
                selectionOpacitySetter.Property = FlowDocumentReader.SelectionOpacityProperty; 
            }
            else if (_control is FlowDocumentPageViewer)
            {
                selectionBrushSetter.Property = FlowDocumentPageViewer.SelectionBrushProperty;
                selectionOpacitySetter.Property = FlowDocumentPageViewer.SelectionOpacityProperty; 
            }
            else if (_control is FlowDocumentScrollViewer)
            {
                selectionBrushSetter.Property = FlowDocumentPageViewer.SelectionBrushProperty;
                selectionOpacitySetter.Property = FlowDocumentPageViewer.SelectionOpacityProperty; 
            }

            selectionBrushSetter.Value = _testSelectionBrush;
            selectionOpacitySetter.Value = _testSelectionOpacity;

            controlStyle.Setters.Add(selectionBrushSetter);
            controlStyle.Setters.Add(selectionOpacitySetter);

            // Trigger that will change SelectionBrush and SelectioOpacity when the control is moused over
            Trigger trigger = new Trigger { Property = Control.IsMouseOverProperty, Value = true };            

            Setter selectionBrushTriggerSetter = new Setter();
            Setter selectionOpacityTriggerSetter = new Setter();

            if (_control is TextBoxBase)
            {
                selectionBrushTriggerSetter.Property = TextBoxBase.SelectionBrushProperty;
                selectionOpacityTriggerSetter.Property = TextBoxBase.SelectionOpacityProperty;
            }
            else if (_control is PasswordBox)
            {
                selectionBrushTriggerSetter.Property = PasswordBox.SelectionBrushProperty;
                selectionOpacityTriggerSetter.Property = PasswordBox.SelectionOpacityProperty;
            }
            else if (_control is FlowDocumentReader)
            {
                selectionBrushTriggerSetter.Property = FlowDocumentReader.SelectionBrushProperty;
                selectionOpacityTriggerSetter.Property = FlowDocumentReader.SelectionOpacityProperty;
            }
            else if (_control is FlowDocumentPageViewer)
            {
                selectionBrushTriggerSetter.Property = FlowDocumentPageViewer.SelectionBrushProperty;
                selectionOpacityTriggerSetter.Property = FlowDocumentPageViewer.SelectionOpacityProperty;
            }
            else if (_control is FlowDocumentScrollViewer)
            {
                selectionBrushTriggerSetter.Property = FlowDocumentPageViewer.SelectionBrushProperty;
                selectionOpacityTriggerSetter.Property = FlowDocumentPageViewer.SelectionOpacityProperty;
            }

            selectionBrushTriggerSetter.Value = _triggerBrush;
            selectionOpacityTriggerSetter.Value = _triggerOpacity;

            trigger.Setters.Add(selectionBrushTriggerSetter);
            trigger.Setters.Add(selectionOpacityTriggerSetter);

            controlStyle.Triggers.Add(trigger);

            return controlStyle;
        }

        private Media.SolidColorBrush GetTestSelectionBrush()
        {
            switch (_testSelectionBrushColorValue)
            {                
                case "Red":
                    return Media.Brushes.Red;
                case "AlphaRed":
                    return new Media.SolidColorBrush(Media.Color.FromArgb(128, 255, 0, 0));
                default:
                    throw new ApplicationException("Invalid value for testSelectionBrushColorValue [" + _testSelectionBrushColorValue + "]");
            }
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
        private string _testSelectionBrushColorValue = string.Empty;                   

        private Control _control = null;
        private UIElementWrapper _wrapper = null;
        // Rectangle object (configured with test values for SelectionBrush and SelectionOpacity)
        // is used to get the expected selection rendering color during rendering verification.
        private System.Windows.Shapes.Rectangle _rectangle = null;
        private Media.SolidColorBrush _testSelectionBrush = null;       
        private double _testSelectionOpacity = 0.8;
        private Media.Color _expectedSelectionRenderColor = Media.Colors.Black;
        private Media.Brush _triggerBrush = Media.Brushes.Green;
        private double _triggerOpacity = 0.4;
        private string _testContent = "Some test content";              

        // Used to remove few pixels of the boundaries from the snapshot 
        // to avoid any aliasing at the borders.
        private const int removeBorderThickness = 2;

        // 1000 is just a heuristic number which works across all variations 
        // to make sure the selection snapshot is not empty.
        private const int minimumSelectionPixels = 1000;

        #endregion
    }
}