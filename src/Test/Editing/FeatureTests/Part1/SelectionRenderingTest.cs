// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies rendering of selection in Editing controls with various
//  values of SelectionBrush and SelectionOpacity
//  Also covers API testing for SelectionBrush and SelectionOpacity

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
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
    [Test(2, "Selection", "SelectionRenderingTest_Editable_Controls", MethodParameters = "/TestCaseType:SelectionRenderingTest /Pri=0", Timeout = 200)]
    [Test(1, "Selection", "SelectionRenderingTest_FlowDocument_Viewers", MethodParameters = "/TestCaseType:SelectionRenderingTest /Pri=1", Timeout = 270)]
    // This test verifies rendering of SelectionBrush in controls where it is supported: 
    // TextBox, RichTextBox, PasswordBox, FlowDocumentReader, FlowDocumentScrollViewer, FlowDocumentPageViewer    
    public class SelectionRenderingTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _rectangle = new System.Windows.Shapes.Rectangle();
            _rectangle.Width = 100;
            _rectangle.Height = 25;

            _selectedTextRectangle = new System.Windows.Shapes.Rectangle();
            _selectedTextRectangle.Width = 100;
            _selectedTextRectangle.Height = 25;

            CreateTestControlByType();

            _control.Height = 230;
            _control.Width = 300; 
            _control.FontSize = 24;                                  
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
            panel.Children.Add(_selectedTextRectangle);
            panel.Children.Add(_control);
            MainWindow.Content = panel;             

            QueueDelegate(DoVisualVerification);
        }

        private void SetTestValues()
        {
            // Set SelectionBrush
            _testSelectionBrush = GetTestSelectionBrush();
            _rectangle.Fill = _testSelectionBrush;
            // Set the value on the test control when not testing for default value.
            if (_testSelectionBrushColorValue != "Default")
            {
                if (_control is TextBoxBase)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((TextBoxBase)_control).SetValue(TextBoxBase.SelectionBrushProperty, _testSelectionBrush);
                    }
                    else
                    {
                        ((TextBoxBase)_control).SelectionBrush = _testSelectionBrush;
                    }
                }
                else if (_control is PasswordBox)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((PasswordBox)_control).SetValue(TextBoxBase.SelectionBrushProperty, _testSelectionBrush);
                    }
                    else
                    {
                        ((PasswordBox)_control).SelectionBrush = _testSelectionBrush;
                    }
                }
                else if (_control is FlowDocumentReader)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((FlowDocumentReader)_control).SetValue(FlowDocumentReader.SelectionBrushProperty, _testSelectionBrush);
                    }
                    else
                    {
                        ((FlowDocumentReader)_control).SelectionBrush = _testSelectionBrush;
                    }
                }
                else if (_control is FlowDocumentPageViewer)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((FlowDocumentPageViewer)_control).SetValue(FlowDocumentPageViewer.SelectionBrushProperty, _testSelectionBrush);
                    }
                    else
                    {
                        ((FlowDocumentPageViewer)_control).SelectionBrush = _testSelectionBrush;
                    }
                }
                else if (_control is FlowDocumentScrollViewer)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((FlowDocumentScrollViewer)_control).SetValue(FlowDocumentScrollViewer.SelectionBrushProperty, _testSelectionBrush);
                    }
                    else
                    {
                        ((FlowDocumentScrollViewer)_control).SelectionBrush = _testSelectionBrush;
                    }
                }
            }

            // We only use this when it's set from the test combinations
            if (!string.IsNullOrEmpty(_testSelectionTextBrushColorValue))
            {
                // Set SelectionTextBrush
                _testSelectionTextBrush = GetTestSelectionTextBrush();

                _selectedTextRectangle.Fill = _testSelectionTextBrush;

                // Set the value on the test control when not testing for default value.
                if (_testSelectionTextBrushColorValue != "Default")
                {
                    if (_control is TextBoxBase)
                    {
                        if (_useDependencyPropertyGetSetMethods)
                        {
                            ((TextBoxBase)_control).SetValue(TextBoxBase.SelectionTextBrushProperty, _testSelectionTextBrush);
                        }
                        else
                        {
                            ((TextBoxBase)_control).SelectionTextBrush = _testSelectionTextBrush;
                        }
                    }
                    else if (_control is PasswordBox)
                    {
                        if (_useDependencyPropertyGetSetMethods)
                        {
                            ((PasswordBox)_control).SetValue(TextBoxBase.SelectionTextBrushProperty, _testSelectionTextBrush);
                        }
                        else
                        {
                            ((PasswordBox)_control).SelectionTextBrush = _testSelectionTextBrush;
                        }
                    }
                }
            }

            // Set SelectionOpacity
            _testSelectionOpacity = GetTestSelectionOpacity(_control);
            _rectangle.Opacity = _testSelectionOpacity;
            // Set the value on the test control when not testing for default value.
            if (_testSelectionOpacityValue != "Default")
            {
                if (_control is TextBoxBase)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((TextBoxBase)_control).SetValue(TextBoxBase.SelectionOpacityProperty, _testSelectionOpacity);
                    }
                    else
                    {
                        ((TextBoxBase)_control).SelectionOpacity = _testSelectionOpacity;
                    }
                }
                else if (_control is PasswordBox)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((PasswordBox)_control).SetValue(TextBoxBase.SelectionOpacityProperty, _testSelectionOpacity);
                    }
                    else
                    {
                        ((PasswordBox)_control).SelectionOpacity = _testSelectionOpacity;
                    }
                }
                else if (_control is FlowDocumentReader)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((FlowDocumentReader)_control).SetValue(FlowDocumentReader.SelectionOpacityProperty, _testSelectionOpacity);
                    }
                    else
                    {
                        ((FlowDocumentReader)_control).SelectionOpacity = _testSelectionOpacity;
                    }
                }
                else if (_control is FlowDocumentPageViewer)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((FlowDocumentPageViewer)_control).SetValue(FlowDocumentPageViewer.SelectionOpacityProperty, _testSelectionOpacity);
                    }
                    else
                    {
                        ((FlowDocumentPageViewer)_control).SelectionOpacity = _testSelectionOpacity;
                    }
                }
                else if (_control is FlowDocumentScrollViewer)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((FlowDocumentScrollViewer)_control).SetValue(FlowDocumentScrollViewer.SelectionOpacityProperty, _testSelectionOpacity);
                    }
                    else
                    {
                        ((FlowDocumentScrollViewer)_control).SelectionOpacity = _testSelectionOpacity;
                    }
                }
            }    
        }

        private void DoVisualVerification()
        {
            // Do simple property value verification before visual verification
            VerifyPropertyValuesAfterSet();

            _control.Focus();

            if (_control is TextBoxBase || _control is PasswordBox)
            {
                _wrapper.SelectAll();
            }
            else
            {
                // Ctrl+A to Select all content in the FlowDocument viewers. 
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, true);
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.A, true);
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.A, false);
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, false);
            }

            // Wait for the selection to appear
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            _expectedSelectionRenderColor = GetExpectedSelectionRenderColor();
            _expectedSelectedTextRenderColor = GetExpectedSelectedTextRenderColor();

            Bitmap selectionSnapshot = GetSelectionScreenShot(_control);

            VerifySelectionColor(selectionSnapshot, _expectedSelectionRenderColor);
        }

        private void VerifyPropertyValuesAfterSet()
        {
            // Verify value for SelectionBrush
            Media.Brush currentSelectionBrush = null;
            if (_control is TextBoxBase)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentSelectionBrush = (Media.Brush)((TextBoxBase)_control).GetValue(TextBoxBase.SelectionBrushProperty);
                }
                else
                {
                    currentSelectionBrush = ((TextBoxBase)_control).SelectionBrush;
                }
            }
            else if (_control is PasswordBox)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentSelectionBrush = (Media.Brush)((PasswordBox)_control).GetValue(TextBoxBase.SelectionBrushProperty);
                }
                else
                {
                    currentSelectionBrush = ((PasswordBox)_control).SelectionBrush;
                }
            }
            else if (_control is FlowDocumentReader)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentSelectionBrush = (Media.Brush)((FlowDocumentReader)_control).GetValue(FlowDocumentReader.SelectionBrushProperty);
                }
                else
                {
                    currentSelectionBrush = ((FlowDocumentReader)_control).SelectionBrush;
                }
            }
            else if (_control is FlowDocumentPageViewer)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentSelectionBrush = (Media.Brush)((FlowDocumentPageViewer)_control).GetValue(FlowDocumentPageViewer.SelectionBrushProperty);
                }
                else
                {
                    currentSelectionBrush = ((FlowDocumentPageViewer)_control).SelectionBrush;
                }
            }
            else if (_control is FlowDocumentScrollViewer)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentSelectionBrush = (Media.Brush)((FlowDocumentScrollViewer)_control).GetValue(FlowDocumentScrollViewer.SelectionBrushProperty);
                }
                else
                {
                    currentSelectionBrush = ((FlowDocumentScrollViewer)_control).SelectionBrush;
                }
            }

            if ((_testSelectionBrush != null) && (currentSelectionBrush != null))
            {
                Log("Current SelectionBrush value [" + currentSelectionBrush.ToString() + "]");
                Log("Expected SelectionBrush value [" + _testSelectionBrush.ToString() + "]");
            }

            // Regression_Bug76
            // We have to compare the color here.  Comparing references like was done previously results
            // in a failure when UseAdornerForTextboxSelectionRendering is enabled since theme changes
            // will override the default set in code with the appropriate default from SystemColors via the theme itself.
            // This results in a different reference than was initially created.
            Verifier.Verify((currentSelectionBrush as Media.SolidColorBrush)?.Color == _testSelectionBrush?.Color,
                    "Verifying SelectionBrush value after property is set", true);            

            // Verify value for SelectionOpacity
            double currentSelectionOpacity = -1;
            if (_control is TextBoxBase)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentSelectionOpacity = (double)((TextBoxBase)_control).GetValue(TextBoxBase.SelectionOpacityProperty);
                }
                else
                {
                    currentSelectionOpacity = ((TextBoxBase)_control).SelectionOpacity;
                }
            }
            else if (_control is PasswordBox)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentSelectionOpacity = (double)((PasswordBox)_control).GetValue(TextBoxBase.SelectionOpacityProperty);
                }
                else
                {
                    currentSelectionOpacity = ((PasswordBox)_control).SelectionOpacity;
                }
            }
            else if (_control is FlowDocumentReader)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentSelectionOpacity = (double)((FlowDocumentReader)_control).GetValue(FlowDocumentReader.SelectionOpacityProperty);
                }
                else
                {
                    currentSelectionOpacity = ((FlowDocumentReader)_control).SelectionOpacity;
                }
            }
            else if (_control is FlowDocumentPageViewer)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentSelectionOpacity = (double)((FlowDocumentPageViewer)_control).GetValue(FlowDocumentPageViewer.SelectionOpacityProperty);
                }
                else
                {
                    currentSelectionOpacity = ((FlowDocumentPageViewer)_control).SelectionOpacity;
                }
            }
            else if (_control is FlowDocumentScrollViewer)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentSelectionOpacity = (double)((FlowDocumentScrollViewer)_control).GetValue(FlowDocumentScrollViewer.SelectionOpacityProperty);
                }
                else
                {
                    currentSelectionOpacity = ((FlowDocumentScrollViewer)_control).SelectionOpacity;
                }
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

        private Media.Color GetExpectedSelectedTextRenderColor()
        {
            Bitmap rectangleSnapshot = BitmapCapture.CreateBitmapFromElement(_selectedTextRectangle);
            System.Drawing.Color color = rectangleSnapshot.GetPixel(rectangleSnapshot.Width / 2, rectangleSnapshot.Height / 2);
            return Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Queries the current state of UseAdornerForTextboxSelectionRendering.
        /// </summary>
        /// <returns>The current state of the adorner selection compat flag or true if not set.</returns>
        private bool GetAdornerCompatFlag()
        {
            // Default is use adorner.
            bool adornerSwitch = true;

            if (!AppContext.TryGetSwitch("Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering", out adornerSwitch))
            {
                // If the switch was not found we don't get the default value returned, so we have to reset it.
                adornerSwitch = true;
            }

            Log("Adorner Switch: " + adornerSwitch);

            return adornerSwitch;
        }

        /// <summary>
        /// Retrieves the text from the specified control.
        /// </summary>
        /// <param name="control">The control to retrieve text from.</param>
        /// <returns>The text of the control or null if not retrievable.</returns>
        private string GetControlText(Control control)
        {
            // Grab control text
            string text = null;

            if (control is PasswordBox)
            {
                object passwordBoxSelection = ReflectionUtils.GetProperty(control, "Selection");
                text = ReflectionUtils.GetInterfaceProperty(passwordBoxSelection, "ITextRange", "Text") as string;
            }
            else if (control is TextBoxBase)
            {
                text = _wrapper.SelectionInstance?.Text;
            }
            else
            {
                text = GetTextSelectionFromFlowDocumentViewer()?.Text;
            }

            return text;
        }

        /// <summary>
        /// For some selection scenarios we will only see the selection color.  This function determines if
        /// this is considered a success scenario or not.
        /// </summary>
        /// <remarks>
        /// At 100% selection opacity
        ///     If adorner is enabled, all controls should have only selection color.
        ///     If adorner is disabled, controls utilizing the new rendered (TextBoxBase/PasswordBox) should never have only selection color.
        ///         Special case here for when selection color is the same as the selected text color.
        /// At other selection opacities
        ///     Any control with non-whitespace text should not have only selection color.
        ///         Adorner layer would show this text due to opacity.
        ///         Non-adorner would never overlay selection on top of text.
        ///     Whitespace selections are always pure selection color.
        /// </remarks>
        /// <param name="control">The control with the selection</param>
        /// <returns>True if the only color can be the selection color, false otherwise</returns>
        private bool ExpectedSelectionColorCanBeOnlyColor(Control control)
        {
            bool result = false;

            // Default is use adorner.
            bool adornerSwitch = GetAdornerCompatFlag();

            // The default value of the opacity should allow for other colors unless adorner is off
            // and the control is a TextBox or PasswordBox.
            bool isDefaultOpacityUsed = _testSelectionOpacityValue == "Default";

            bool isControlTextOrPasswordBox = control is PasswordBox || control is TextBox;

            bool isTextNullOrWhitespace = string.IsNullOrWhiteSpace(GetControlText(control));

            double opacity = GetTestSelectionOpacity(control);

            if (!adornerSwitch && isControlTextOrPasswordBox)
            {
                result = isTextNullOrWhitespace || _testSelectionTextBrush == null || GetExpectedSelectionRenderColor() == _testSelectionTextBrush.Color;
            }
            else
            {
                result = (!isDefaultOpacityUsed && opacity == 1) || isTextNullOrWhitespace;
            }

            return result;
        }

        /// <summary>
        /// Determines if we should check that SelectionTextBrush is in the rendered selection rectangle.
        /// </summary>
        /// <param name="control">The control being selected</param>
        /// <returns>True if we should account for SelectionTextBrush, false otherwise.</returns>
        private bool ShouldVerifySelectionTextBrush(Control control)
        {
            // Default is use adorner.
            bool adornerSwitch = GetAdornerCompatFlag();

            bool isTextNullOrWhitespace = string.IsNullOrWhiteSpace(GetControlText(control));

            // Criteria is we're not using the adorner and we're working with Password/TextBoxes.
            // If we have null or whitespace text, the whole thing is selection color so SelectionTextBrush won't matter.
            return !adornerSwitch && (control is PasswordBox || control is TextBox) && !isTextNullOrWhitespace && _testSelectionTextBrush != null;
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

            Log("Expected selection render color: " + _expectedSelectionRenderColor.ToString());
            Log("Total number of pixels in the captured selection area: " + totalSelectionPixels);
            Log("Number of pixels that matched expected color: " + coloredPixels);
            Log("Number of pixels that did not match expected color: " + (totalSelectionPixels - coloredPixels));

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

            // If we shouldn't have just the selection color, then verify some pixels are not the expected selection color.
            if (!ExpectedSelectionColorCanBeOnlyColor(_control))
            {
                if(coloredPixels == totalSelectionPixels)
                {
                    Logger.Current.LogImage(selectionSnapshot, "selectionSnapshot");
                    Logger.Current.LogImage(BitmapCapture.CreateBitmapFromElement(_control), "controlSnapshot");
                }

                Verifier.Verify(coloredPixels != totalSelectionPixels, "Verifying that at least some pixels are not SelectionColor.", true);
            }

            // For PasswordBox and TextBoxBase in the non-adorner case, we should check that the SelectionTextBrush has worked.
            if (ShouldVerifySelectionTextBrush(_control))
            {
                int coloredTextPixels = BitmapUtils.CountColoredPixels(selectionSnapshot, _expectedSelectedTextRenderColor);

                Log("Number of pixels that matched expected text color: " + coloredTextPixels);
                Log("Expected SelectionTextColor: " + _expectedSelectedTextRenderColor.ToString());

                if (coloredTextPixels == 0)
                {
                    Logger.Current.LogImage(selectionSnapshot, "selectionSnapshot");
                    Logger.Current.LogImage(BitmapCapture.CreateBitmapFromElement(_control), "controlSnapshot");
                }

                Verifier.Verify(coloredTextPixels > 0, "Verifying that at least some pixels are SelectionTextColor.", true);
            }

            NextCombination();
        }

        private Media.SolidColorBrush GetTestSelectionBrush()
        {
            switch (_testSelectionBrushColorValue)
            {
                case "Default":                    
                    return (Media.SolidColorBrush)TextBoxBase.SelectionBrushProperty.DefaultMetadata.DefaultValue;                    
                case "Null":
                    return null;
                case "Red":
                    return Media.Brushes.Red;                    
                case "AlphaRed":
                    return new Media.SolidColorBrush(Media.Color.FromArgb(128, 255, 0, 0));                    
                default:
                    throw new ApplicationException("Invalid value for testSelectionBrushColorValue [" + _testSelectionBrushColorValue + "]");                    
            }
        }

        private Media.SolidColorBrush GetTestSelectionTextBrush()
        {
            switch (_testSelectionTextBrushColorValue)
            {
                case "Default":
                    return (Media.SolidColorBrush)TextBoxBase.SelectionTextBrushProperty.DefaultMetadata.DefaultValue;
                case "Null":
                    return null;
                case "Yellow":
                    return Media.Brushes.Yellow;
                case "Green":
                    return Media.Brushes.Green;
                default:
                    throw new ApplicationException("Invalid value for testSelectionBrushColorValue [" + _testSelectionBrushColorValue + "]");
            }
        }

        private double GetTestSelectionOpacity(Control c)
        {
            switch (_testSelectionOpacityValue)
            {
                case "Default":
                    // Regression_Bug76
                    // Default values should be queried through the control that we are testing.
                    // This allows us to pickup any overridden defaults.
                    return (double)TextBoxBase.SelectionOpacityProperty.GetMetadata(c.GetType()).DefaultValue;
                case "0":
                    return 0;                    
                case "0.6":
                    return 0.6;                    
                case "1":
                    return 1;                    
                default:
                    throw new ApplicationException("Invalid value for testSelectionOpacityValue [" + _testSelectionOpacityValue + "]");                    
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
        private bool _useDependencyPropertyGetSetMethods = false;
        private string _testSelectionBrushColorValue = string.Empty;
        private string _testSelectionTextBrushColorValue = string.Empty;
        private string _testSelectionOpacityValue = string.Empty;
        private string _testContent = string.Empty;              

        private Control _control = null;
        private UIElementWrapper _wrapper = null;
        // Rectangle object (configured with test values for SelectionBrush and SelectionOpacity)
        // is used to get the expected selection rendering color during rendering verification.
        private System.Windows.Shapes.Rectangle _rectangle = null;
        private System.Windows.Shapes.Rectangle _selectedTextRectangle = null;
        private Media.SolidColorBrush _testSelectionBrush = null;
        private Media.SolidColorBrush _testSelectionTextBrush = null;
        private double _testSelectionOpacity = -1;
        private Media.Color _expectedSelectionRenderColor = Media.Colors.Black;
        private Media.Color _expectedSelectedTextRenderColor = Media.Colors.Black;

        // Used to remove few pixels of the boundaries from the snapshot 
        // to avoid any aliasing at the borders.
        private const int removeBorderThickness = 2;

        // 1000 is just a heuristic number which works across all variations 
        // to make sure the selection snapshot is not empty.
        private const int minimumSelectionPixels = 1000;

        #endregion
    }
}