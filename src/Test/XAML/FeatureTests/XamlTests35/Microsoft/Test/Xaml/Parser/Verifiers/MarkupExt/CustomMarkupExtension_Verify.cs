// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary/>
    public static class CustomMarkupExtension_Verify
    {
        /// <summary>
        /// CustomMarkupExtensionVerify
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool   result   = true;
            Button button0  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button0");
            Button button1  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button1");
            Button button2  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button2");
            Button button3  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button3");
            Button button4  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button4");
            Button button5  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button5");
            Button button6  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button6");
            Button button7  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button7");
            Button button8  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button8");
            Button button9  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button9");
            Button button10 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button10");
            Button button11 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button11");
            Button button12 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button12");
            Button button13 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button13");

            // Buttons 0 thru 7 (except Button3) should have a background of Red+Blue
            SolidColorBrush background = (SolidColorBrush) button0.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Red, Colors.Blue })))");
            }

            background = (SolidColorBrush) button1.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Red, Colors.Blue })))(1)");
            }

            background = (SolidColorBrush) button2.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Red, Colors.Blue })))(2)");
            }

            // Button3 should have a background of Maroon+Navy+Aqua+Chocolate+Gold.
            background = (SolidColorBrush) button3.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Maroon, Colors.Navy, Colors.Aqua, Colors.Chocolate, Colors.Gold}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Maroon, Colors.Navy, Colors.Aqua, Colors.Chocolate, Colors.Gold })))");
            }

            background = (SolidColorBrush) button4.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Red, Colors.Blue })))(3)");
            }

            background = (SolidColorBrush) button5.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Red, Colors.Blue })))(4)");
            }

            background = (SolidColorBrush) button6.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Red, Colors.Blue })))(5)");
            }

            background = (SolidColorBrush) button7.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Red, Colors.Blue })))(6)");
            }

            // Buttons 8 and 9 should have background Black + Blue
            background = (SolidColorBrush) button8.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Black, Colors.Blue}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Black, Colors.Blue })))");
            }

            background = (SolidColorBrush) button9.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Black, Colors.Blue}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Black, Colors.Blue })))(1)");
            }

            // Buttons 10 and 11 should have Maroon + Navy + Green background
            background = (SolidColorBrush) button10.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Maroon, Colors.Navy, Colors.Green}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Maroon, Colors.Navy, Colors.Green })))");
            }

            background = (SolidColorBrush) button11.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Maroon, Colors.Navy, Colors.Green}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Maroon, Colors.Navy, Colors.Green })))(1)");
            }

            // Button 12 should have Maroon + Navy + Aqua + Chocolate + Gold + Indigo
            SolidColorBrush brushContent = null;
            brushContent = (SolidColorBrush) button12.Content;
            if (!(Color.Equals(brushContent.Color, MixColors(new Color[] {Colors.Maroon, Colors.Navy, Colors.Aqua, Colors.Chocolate, Colors.Gold, Colors.Indigo}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(brushContent.Color, MixColors(new Color[] { Colors.Maroon, Colors.Navy, Colors.Aqua, Colors.Chocolate, Colors.Gold, Colors.Indigo })))");
            }

            // Button 13 should have Maroon + Navy + Green
            brushContent = (SolidColorBrush) button13.Content;
            if (!(Color.Equals(brushContent.Color, MixColors(new Color[] {Colors.Maroon, Colors.Navy, Colors.Green}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(brushContent.Color, MixColors(new Color[] { Colors.Maroon, Colors.Navy, Colors.Green })))");
            }
            return result;
        }

        private static Color MixColors(Color[] colors)
        {
            Color result = colors[0];
            for (int i = 1; i < colors.Length; i++)
            {
                result = result + colors[i];
            }

            if (result.ScA > 1.0f)
            {
                result.ScA = 1.0f;
            }

            return result;
        }
    }
}
