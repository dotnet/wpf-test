// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies rendering of CaretBrush in Editing controls

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Media = System.Windows.Media;

using Microsoft.Test.Discovery;
using Microsoft.Test.Imaging;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Microsoft.Test.Editing
{
    [Test(2, "Caret", "CaretBrushRenderingTest", MethodParameters = "/TestCaseType:CaretBrushRenderingTest")]
    // This test verifies rendering of CaretBrush in controls where it is supported.
    public class CaretBrushRenderingTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _rectangle = new System.Windows.Shapes.Rectangle();
            _rectangle.Width = 100;
            _rectangle.Height = 25;

            _control = (Control)_testControlType.CreateInstance();
            _control.Height = 100;
            _control.Width = 300;                        
            _control.FontSize = testFontSize;
            _control.FontFamily = new Media.FontFamily("Tahoma");
            _control.SnapsToDevicePixels = true;

            // Wait for the control to render
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(500);
            SetTestValues(); 

            _wrapper = new UIElementWrapper(_control);
            _wrapper.Text = _testContent;
            _caretVerifier = new CaretVerifier(_control);                       

            StackPanel panel = new StackPanel();
            panel.Children.Add(_rectangle);
            panel.Children.Add(_control);
            MainWindow.Content = panel;            

            QueueDelegate(VerifyCaretRendering);            
        }

        private void SetTestValues()
        {
            // Set CaretBrush
            _testCaretBrush = GetTestCaretBrush();
            if (_testCaretBrushColorValue == null)
            {
                _rectangle.Fill = GetDefaultCaretBrush();
            }
            else
            {
                _rectangle.Fill = _testCaretBrush;
            }
            
            // Set the value on the test control when not testing for default value.
            if (_testCaretBrushColorValue != "Default")
            {
                if (_control is TextBoxBase)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((TextBoxBase)_control).SetValue(TextBoxBase.CaretBrushProperty, _testCaretBrush);
                    }
                    else
                    {
                        ((TextBoxBase)_control).CaretBrush = _testCaretBrush;
                    }
                }
                else if (_control is PasswordBox)
                {
                    if (_useDependencyPropertyGetSetMethods)
                    {
                        ((PasswordBox)_control).SetValue(TextBoxBase.CaretBrushProperty, _testCaretBrush);
                    }
                    else
                    {
                        ((PasswordBox)_control).CaretBrush = _testCaretBrush;
                    }
                }
            }
        }

        private void VerifyCaretRendering()
        {
            // Do simple property value verification before visual verification
            VerifyPropertyValueAfterSet();

            // Wait for the caret to appear
            _control.Focus();            
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

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
            _expectedCaretRenderColor = GetExpectedCaretRenderColor();
            colorPixelsInCaretSnapshot = BitmapUtils.CountColoredPixels(caretSnapshot, _expectedCaretRenderColor);            
            Log("Number of color pixels in the caret snapshot: " + colorPixelsInCaretSnapshot);

            // Number of color pixels in the caret snapshot is expected to be atleast of FontSize height
            if (colorPixelsInCaretSnapshot < testFontSize)
            {
                Logger.Current.LogImage(caretSnapshot, "caretSnapShot");
                Logger.Current.LogImage(controlSnapshot, "controlSnapShot");
                Verifier.Verify(false, "Verifying that we have expected red pixels in the caretSnapshot", true);
            }            

            NextCombination();
        }

        private void VerifyPropertyValueAfterSet()
        {
            // Verify value for CaretBrush
            Media.Brush currentCaretBrush = null;
            if (_control is TextBoxBase)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentCaretBrush = (Media.Brush)((TextBoxBase)_control).GetValue(TextBoxBase.CaretBrushProperty);
                }
                else
                {
                    currentCaretBrush = ((TextBoxBase)_control).CaretBrush;                    
                }
            }
            else if (_control is PasswordBox)
            {
                if (_useDependencyPropertyGetSetMethods)
                {
                    currentCaretBrush = (Media.Brush)((PasswordBox)_control).GetValue(TextBoxBase.CaretBrushProperty);
                }
                else
                {
                    currentCaretBrush = ((PasswordBox)_control).CaretBrush;
                }
            }            

            if ((_testCaretBrushColorValue == "Default") || (_testCaretBrushColorValue == "Null"))
            {
                Verifier.Verify(currentCaretBrush == null,
                    "Verifying CaretBrush value after property is set", true);
            }
            else            
            {
                Log("Current CaretBrush value [" + currentCaretBrush.ToString() + "]");
                Log("Expected CaretBrush value [" + _testCaretBrush.ToString() + "]");
                Verifier.Verify(currentCaretBrush == _testCaretBrush,
                    "Verifying CaretBrush value after property is set", true);
            }            
        }
        
        private Media.SolidColorBrush GetTestCaretBrush()
        {
            switch (_testCaretBrushColorValue)
            {
                case "Default":
                    return GetDefaultCaretBrush();
                case "Null":
                    return null;
                case "Red":
                    return Media.Brushes.Red;
                case "AlphaRed":
                    return new Media.SolidColorBrush(Media.Color.FromArgb(128, 255, 0, 0));
                default:
                    throw new ApplicationException("Invalid value for testCaretBrushColorValue [" + _testCaretBrushColorValue + "]");
            }
        }

        // Note: The value returned by this function will only work in this test case when no explicit background 
        // color is set on the controls and no background color is set on the TextRange (applicable only RTB)
        private Media.SolidColorBrush GetDefaultCaretBrush()
        {
            Media.Color windowColor = System.Windows.SystemColors.WindowColor;

            // Invert the color to get the caret color from the system window color.
            byte r = (byte)~(windowColor.R);
            byte g = (byte)~(windowColor.G);
            byte b = (byte)~(windowColor.B);

            return new Media.SolidColorBrush(Media.Color.FromRgb(r, g, b));
        }

        private Media.Color GetExpectedCaretRenderColor()
        {
            Bitmap rectangleSnapshot = BitmapCapture.CreateBitmapFromElement(_rectangle);
            System.Drawing.Color color = rectangleSnapshot.GetPixel(rectangleSnapshot.Width / 2, rectangleSnapshot.Height / 2);
            return Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values        
        private TextEditableType _testControlType = null;
        private bool _useDependencyPropertyGetSetMethods = false;
        private string _testCaretBrushColorValue = string.Empty;
        private string _testContent = string.Empty;

        private Control _control = null;
        private Media.SolidColorBrush _testCaretBrush = null;
        private UIElementWrapper _wrapper = null;
        // Rectangle object (configured with test values for CaretBrush) is used to get the expected 
        // caret rendering color during rendering verification.
        private System.Windows.Shapes.Rectangle _rectangle = null;        
        private CaretVerifier _caretVerifier = null;
        private Media.Color _expectedCaretRenderColor = Media.Colors.White;

        private const double testFontSize = 24;

        #endregion
    }
}