// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Few tests with TextOptions.TextFormattingMode set to Ideal and Display

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;     
    using System.Globalization;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Input;
    using Microsoft.Test.Threading;

    using Test.Uis.Data;    
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;   
    
    #endregion Namespaces.

    /// <summary>
    /// Verifies that the TextBox renders differently when different
    /// values are applied to the following properties:
    /// FontSize, FontStyle, FontWeight.
    /// </summary>
    [Test(0, "TextBox", "TextBoxFontPropertiesInDisplayMode", MethodParameters = "/TestCaseType=TextBoxFontPropertiesTest /Text=abcd /TextFormattingmode=Display", SupportFiles = @"FeatureTests\Editing\TextBox.xaml", Keywords = "TextFormattingModeTests")]
    [Test(0, "TextBox", "TextBoxFontPropertiesInIdealMode", MethodParameters = "/TestCaseType=TextBoxFontPropertiesTest /Text=abcd /TextFormattingmode=Ideal", SupportFiles = @"FeatureTests\Editing\TextBox.xaml", Keywords = "TextFormattingModeTests")]
    public class TextBoxFontPropertiesTest : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetTextBoxProperties(TestTextBox);
            Log("Using text: " + Text);
            TestTextBox.FontSize = 12f * (96.0 / 72.0);
            TestTextBox.FontStyle = FontStyles.Normal;
            TestTextBox.FontWeight = FontWeights.Normal;
            TestTextBox.Text = Text;
            switch (ConfigurationSettings.Current.GetArgument("TextFormattingmode"))
            {
                case "Display": TextOptions.SetTextFormattingMode(TestTextBox, TextFormattingMode.Display);
                    break;
                case "Ideal": TextOptions.SetTextFormattingMode(TestTextBox, TextFormattingMode.Ideal);
                    break;
            }
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CaptureInitial));
        }

        private void CaptureInitial()
        {
            if (TestTextBox.Text != Text)
            {
                Log("TextBox.Text=[" + TestTextBox.Text + "]");
                throw new Exception("Text has not been set.");
            }
            Log("Capturing initial rendering...");
            _lastBitmap = BitmapCapture.CreateBitmapFromElement(TestControl);
            TestTextBox.FontSize = 24f * (96.0 / 72.0);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckFontSizeChanged));
        }

        private void CheckFontSizeChanged()
        {
            Log("Verifying that FontSize produces a change...");
            _lastBitmap = CheckChanged();
            TestTextBox.FontStyle = FontStyles.Italic;
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckFontStyleChanged));
        }

        private void CheckFontStyleChanged()
        {
            Log("Verifying that FontStyle produces a change...");
            _lastBitmap = CheckChanged();
            TestTextBox.FontWeight = FontWeights.Bold;
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckFontWeightChanged));
        }

        private void CheckFontWeightChanged()
        {
            Log("Verifying that FontStyle produces a change...");
            CheckChanged();

            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("   ");

            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckEdited));
        }

        private void CheckEdited()
        {
            Verifier.Verify(TestTextBox.Text != Text, "Text has changed", true);
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        /// <summary>
        /// Verifies that the bitmap has changed.
        /// </summary>
        /// <returns>A new bitmap for the currently rendered control.</returns>
        private System.Drawing.Bitmap CheckChanged()
        {
            System.Drawing.Bitmap current = BitmapCapture.CreateBitmapFromElement(TestControl);
            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_lastBitmap, current, null, true))
            {
                throw new Exception("There have been no changes in the control rendering.");
            }
            return current;
        }

        #endregion Verifications.

        #region Private fields.

        private System.Drawing.Bitmap _lastBitmap;

        #endregion Private fields.
    }   
}