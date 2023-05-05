// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Microsoft.Test.Serialization.CustomElements;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary/>
    public static class BindExtension_Verify
    {
        /// <summary>
        /// BindExtensionVerify
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool      result      = true;
            String    matchString = "Wak-a-doo!";
            TextBlock text0       = (TextBlock) LogicalTreeHelper.FindLogicalNode(rootElement, "Text0");
            TextBlock text1       = (TextBlock) LogicalTreeHelper.FindLogicalNode(rootElement, "Text1");

            CustomDockPanel    customdockpanel0 = (CustomDockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "CustomDockPanel0");
            ResourceDictionary resourcescustom0 = customdockpanel0.Resources;

            TextBlock textblock2 = (TextBlock) LogicalTreeHelper.FindLogicalNode(rootElement, "TextBlock2");
            ComboBox  combobox1  = (ComboBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ComboBox1");

            Button  button1  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button1");
            TextBox textbox1 = (TextBox) LogicalTreeHelper.FindLogicalNode(rootElement, "TextBox1");
            ListBox listbox1 = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox1");

            if (!((string) button1.Content).Equals(matchString))
            {
                result = false;
                GlobalLog.LogEvidence("!((string)Button1.Content).Equals(matchString)");
            }

            // Verify the Foreground, FontSize and Text properties of Text0.
            // They are all databound
            SolidColorBrush foreground = (SolidColorBrush) text0.GetValue(TextBlock.ForegroundProperty);
            if (!Color.Equals(foreground.Color, Colors.Red))
            {
                result = false;
                GlobalLog.LogEvidence("!Color.Equals(foreground.Color, Colors.Red)");
            }
            foreground = (SolidColorBrush) textblock2.GetValue(TextBlock.ForegroundProperty);
            if (!Color.Equals(foreground.Color, Colors.Red))
            {
                result = false;
                GlobalLog.LogEvidence("!Color.Equals(foreground.Color, Colors.Red)(1)");
            }

            double fontsize = (double) text0.GetValue(TextBlock.FontSizeProperty);
            if (fontsize != 200.0)
            {
                result = false;
                GlobalLog.LogEvidence("fontsize != 200.0");
            }
            fontsize = (double) textblock2.GetValue(TextBlock.FontSizeProperty);
            if (fontsize != 200.0)
            {
                result = false;
                GlobalLog.LogEvidence("fontsize != 200.0(1)");
            }


            string textcontent = (string) text0.GetValue(TextBlock.TextProperty);
            if ("Hello World" != textcontent)
            {
                result = false;
                GlobalLog.LogEvidence("Hello World != textcontent");
            }
            textcontent = (string) textblock2.GetValue(TextBlock.TextProperty);
            if ("Hello World" != textcontent)
            {
                result = false;
                GlobalLog.LogEvidence("Hello World != textcontent(1)");
            }
            textcontent = (string) textbox1.GetValue(TextBox.TextProperty);
            if (matchString != textcontent)
            {
                result = false;
                GlobalLog.LogEvidence("matchString != textcontent");
            }
            textcontent = (string)(((ComboBoxItem) combobox1.Items[0]).Content);
            if (matchString != textcontent)
            {
                result = false;
                GlobalLog.LogEvidence("matchString != textcontent(1)");
            }
            textcontent = (string)(((ListBoxItem) listbox1.Items[0]).Content);
            if (matchString != textcontent)
            {
                result = false;
                GlobalLog.LogEvidence("matchString != textcontent(2)");
            }


            // Verify Foreground of Text1. It's databound to Null XPath, 
            // thus should have default value
            foreground = (SolidColorBrush) text1.GetValue(TextBlock.ForegroundProperty);
            if (!Color.Equals(foreground.Color, Colors.Black))
            {
                result = false;
                GlobalLog.LogEvidence("!Color.Equals(foreground.Color, Colors.Black)");
            }
            // Verify FontSize of Text1
            fontsize = (double) text1.GetValue(TextBlock.FontSizeProperty);
            if (fontsize != 200.0)
            {
                result = false;
                GlobalLog.LogEvidence("fontsize != 200.0(2)");
            }

            return result;
        }
    }
}
