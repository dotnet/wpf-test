// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies rendering of caret in Editing controls with CaretBrush
//  defined in Styles and Triggers.

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Media = System.Windows.Media;

using Microsoft.Test.Discovery;
using Microsoft.Test.Imaging;
using MTI = Microsoft.Test.Input;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Microsoft.Test.Editing
{
    [Test(2, "Caret", "CaretBrushRenderingStyleAndTriggerTest", MethodParameters = "/TestCaseType:CaretBrushRenderingStyleAndTriggerTest", Timeout = 120)]
    // This test verifies rendering of CaretBrush in controls where it is supported by setting this value in a style.
    public class CaretBrushRenderingStyleAndTriggerTest : ManagedCombinatorialTestCase
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
            _rectangle.Fill = _testCaretBrush;

            _control.Style = GetStyleForTest();
        }

        // Create a style that sets values for the Selection brush and opacity and a trigger that will 
        // update these properties when the control is moused over
        private Style GetStyleForTest()
        {
            Style controlStyle;

            if (_control is TextBoxBase)
            {
                controlStyle = new Style(typeof(TextBoxBase));
            }
            else
            {
                controlStyle = new Style(typeof(PasswordBox));
            }

            // Setter for CaretBrush
            Setter caretBrushSetter = new Setter();

            if (_control is TextBoxBase)
            {
                caretBrushSetter.Property = TextBoxBase.CaretBrushProperty;
            }
            else
            {
                caretBrushSetter.Property = PasswordBox.CaretBrushProperty;
            }

            caretBrushSetter.Value = _testCaretBrush;

            controlStyle.Setters.Add(caretBrushSetter);

            // Trigger that will change CaretBrush when the control is moused over
            Trigger trigger = new Trigger { Property = Control.IsMouseOverProperty, Value = true };

            Setter caretBrushTriggerSetter = new Setter();

            if (_control is TextBoxBase)
            {
                caretBrushTriggerSetter.Property = TextBoxBase.CaretBrushProperty;
            }
            else
            {
                caretBrushTriggerSetter.Property = PasswordBox.CaretBrushProperty;
            }

            caretBrushTriggerSetter.Value = _triggerBrush;

            trigger.Setters.Add(caretBrushTriggerSetter);

            controlStyle.Triggers.Add(trigger);

            return controlStyle;
        }

        private void VerifyCaretRendering()
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
            VerifyPropertyValueAfterRender();

            // Wait for the caret to appear
            _control.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // Verify caret color after being set via a Style
            VisuallyVerifyCaretRendering();

            // Reset the rectangle for comparison after trigger is set off
            _rectangle.Fill = _triggerBrush;

            // Move the mouse over the control to set off the trigger
            MTI.Input.MoveTo(_control);

            // Wait for the mouse to move and the selection to change color
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // Verify caret color after being changed via a trigger
            VisuallyVerifyCaretRendering();

            NextCombination();
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
            _expectedCaretRenderColor = GetExpectedCaretRenderColor();
            colorPixelsInCaretSnapshot = BitmapUtils.CountColoredPixels(caretSnapshot, _expectedCaretRenderColor);
            Log("Number of color pixels in the caret snapshot: " + colorPixelsInCaretSnapshot);

            // Number of color pixels in the caret snapshot is expected to be atleast of 3/4th of FontSize height
            if (colorPixelsInCaretSnapshot < (3*testFontSize)/4)
            {
                Logger.Current.LogImage(caretSnapshot, "caretSnapShot");
                Logger.Current.LogImage(controlSnapshot, "controlSnapShot");
                Verifier.Verify(false, "Verifying that we have expected red pixels in the caretSnapshot", true);
            }
        }

        private void VerifyPropertyValueAfterRender()
        {
            // Verify value for CaretBrush
            Media.Brush currentCaretBrush = null;
            if (_control is TextBoxBase)
            {
                currentCaretBrush = ((TextBoxBase)_control).CaretBrush;
            }
            else if (_control is PasswordBox)
            {
                currentCaretBrush = ((PasswordBox)_control).CaretBrush;
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
                case "Red":
                    return Media.Brushes.Red;
                case "AlphaRed":
                    return new Media.SolidColorBrush(Media.Color.FromArgb(128, 255, 0, 0));
                default:
                    throw new ApplicationException("Invalid value for testCaretBrushColorValue [" + _testCaretBrushColorValue + "]");
            }
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
        private string _testCaretBrushColorValue = string.Empty;

        private Control _control = null;
        private Media.SolidColorBrush _testCaretBrush = null;
        private UIElementWrapper _wrapper = null;
        // Rectangle object (configured with test values for CaretBrush) is used to get the expected 
        // caret rendering color during rendering verification.
        private System.Windows.Shapes.Rectangle _rectangle = null;
        private CaretVerifier _caretVerifier = null;
        private Media.Color _expectedCaretRenderColor = Media.Colors.White;
        private Media.Brush _triggerBrush = Media.Brushes.Green;
        private string _testContent = "Some test content";

        private const double testFontSize = 20;

        #endregion
    }
}