// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary/>
    public static class MarkupExtInStyle_Verify
    {
        /// <summary>
        /// Verification routine for MarkupExtInStyle.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool   result  = true;
            Button button0 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button0");
            Button button1 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button1");
            Button button2 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button2");
            Button button3 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button3");

            // Verify properties for Button0
            SolidColorBrush background = (SolidColorBrush) button0.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Black, Colors.Red}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Black, Colors.Red })))");
            }

            Double fontsize = (Double) button0.GetValue(Control.FontSizeProperty);
            if (fontsize != 48.0)
            {
                result = false;
                GlobalLog.LogEvidence("fontsize != 48.0");
            }

            Dock dock = (Dock) button0.GetValue(DockPanel.DockProperty);
            if (dock != Dock.Left)
            {
                result = false;
                GlobalLog.LogEvidence("dock != Dock.Left");
            }

            FontStyle fontstyle = (FontStyle) button0.GetValue(Control.FontStyleProperty);
            if (fontstyle != FontStyles.Italic)
            {
                result = false;
                GlobalLog.LogEvidence("fontstyle != FontStyles.Italic");
            }

            String content = (String) button0.GetValue(ContentControl.ContentProperty);
            if (content != "{Hello}")
            {
                result = false;
                GlobalLog.LogEvidence("content != {Hello}");
            }

            // Verify properties for Button1
            ControlTemplate template = (ControlTemplate) button1.GetValue(Control.TemplateProperty);
            Canvas          canvas0  = (Canvas) template.FindName("Canvas0", button1);
            background = (SolidColorBrush) canvas0.GetValue(Panel.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Black, Colors.Red}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Black, Colors.Red })))");
            }

            ContentPresenter cp0 = (ContentPresenter) template.FindName("CP0", button1);
            content = (String) cp0.GetValue(ContentPresenter.ContentProperty);
            if (content != "{Hello}")
            {
                result = false;
                GlobalLog.LogEvidence("content != {Hello}(1)");
            }

            // Verify properties for Button2
            SolidColorBrush foreground = (SolidColorBrush) button2.GetValue(Control.ForegroundProperty);
            if (!(Color.Equals(foreground.Color, Colors.Blue)))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(foreground.Color, Colors.Blue))");
            }

            TextBlock[] contentArray = (TextBlock[]) button2.GetValue(ContentControl.ContentProperty);
            if ("Text 0" != contentArray[0].Text)
            {
                result = false;
                GlobalLog.LogEvidence("Text 0 != contentArray[0].Text");
            }
            if ("Text 1" != contentArray[1].Text)
            {
                result = false;
                GlobalLog.LogEvidence("Text 1 != contentArray[1].Text");
            }
            if ("Text 2" != contentArray[2].Text)
            {
                result = false;
                GlobalLog.LogEvidence("Text 2 != contentArray[2].Text");
            }

            // Verify properties for Button3
            background = (SolidColorBrush) button3.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, MixColors(new Color[] {Colors.Maroon, Colors.Navy, Colors.Green, Colors.Indigo, Colors.Blue}))))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, MixColors(new Color[] { Colors.Maroon, Colors.Navy, Colors.Green, Colors.Indigo, Colors.Blue })))");
            }

            dock = (Dock) button3.GetValue(DockPanel.DockProperty);
            if (dock != Dock.Left)
            {
                result = false;
                GlobalLog.LogEvidence("dock != Dock.Left(1)");
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
