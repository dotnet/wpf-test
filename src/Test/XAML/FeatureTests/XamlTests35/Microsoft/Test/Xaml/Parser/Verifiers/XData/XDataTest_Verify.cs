// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;
using System.Xml;

namespace Microsoft.Test.Xaml.Parser.Verifiers.XData
{
    /// <summary/>
    public static class XDataTest_Verify
    {
        /// <summary>
        /// Verifies routine for XDataTest.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            FrameworkElement fe = rootElement as FrameworkElement;

            TextBlock textblock1 = fe.FindName("textblock1") as TextBlock;
            if(null == textblock1)
            {
                GlobalLog.LogEvidence("null == textblock1");
                result = false;
            }
            if (0 != String.Compare(textblock1.Text.Trim(), "Test1", true))
            {
                GlobalLog.LogEvidence("Text.Text is >>" + textblock1.Text.Trim() + "<<, should be Test");
                result = false;
            }
            else
            {
                VerifyBindings(textblock1, ref result);
            }

            TextBlock textblock2 = fe.FindName("textblock2") as TextBlock;
            if (null == textblock2)
            {
                GlobalLog.LogEvidence("null == textblock2");
                result = false;
            }
            else
            {
                VerifyBindings(textblock2, ref result);
            }

            TextBlock textblock3 = fe.FindName("textblock3") as TextBlock;
            if (null == textblock3)
            {
                GlobalLog.LogEvidence("null == textblock3");
                result = false;
            }
            else
            {
                VerifyBrushBinding(textblock3, ref result);
            }

            TextBlock textblock4 = fe.FindName("textblock4") as TextBlock;
            if (null == textblock4)
            {
                GlobalLog.LogEvidence("null == textblock4");
                result = false;
            }
            else
            {
                VerifyBrushBinding(textblock4, ref result);
            }

            //Verify Custom Xdata host            
            CustomXDataHost host = (CustomXDataHost)fe.FindResource("DSO3");
            VerifyCustomXDataHost(host, ref result);

            //Verify Custom Xdata host
            host = (CustomXDataHost)fe.FindResource("DSO4");
            VerifyCustomXDataHost(host, ref result);

            host = (CustomXDataHost)fe.FindResource("DSO5");
            VerifyCustomXDataHost(host, ref result);

            host = (CustomXDataHost)fe.FindResource("DSO6");
            VerifyCustomXDataHost(host, ref result);

            Button button = fe.FindName("button1") as Button;
            if(button == null)
            {
                GlobalLog.LogEvidence("null == button1");
                result = false;
            }
            host = (CustomXDataHost)button.Content;
            if (host == null)
            {
                GlobalLog.LogEvidence("null == host");
                result = false;
            }
            else
            {
                VerifyCustomXDataHost(host, ref result);
            }

            button = fe.FindName("button2") as Button;
            if(button == null)
            {
                GlobalLog.LogEvidence("button2 == null");
                result = false;
            }
            host = (CustomXDataHost)button.Content;
            if (host == null)
            {
                GlobalLog.LogEvidence("null == host");
                result = false;
            }
            else
            {
                VerifyCustomXDataHost(host, ref result);
            }

            button = fe.FindName("button3") as Button;
            if (button == null)
            {
                GlobalLog.LogEvidence("button3 == null");
                result = false;
            }
            host = (CustomXDataHost)button.Content;
            if (host == null)
            {
                GlobalLog.LogEvidence("null == host");
                result = false;
            }
            else
            {
                VerifyCustomXDataHost(host, ref result);
            }

            button = fe.FindName("button4") as Button;
            if (button == null)
            {
                GlobalLog.LogEvidence("button4 == null");
                result = false;
            }
            host = (CustomXDataHost)button.Content;
            if (host == null)
            {
                GlobalLog.LogEvidence("null == host");
                result = false;
            }
            else
            {
                VerifyCustomXDataHost(host, ref result);
            }
            return result;
        }

        private static void VerifyCustomXDataHost(CustomXDataHost host, ref bool result)
        {
            if(host == null)
            {
                GlobalLog.LogEvidence("host == null");
                result = false;
            }
            XmlElement rootElement = host.Document.DocumentElement;
            if(rootElement == null)
            {
                GlobalLog.LogEvidence("rootElement == null");
                result = false;
            }
            XmlNodeList children = rootElement.ChildNodes;
            if (children.Count != 3)
            {
                GlobalLog.LogEvidence("children.Count != 3");
                result = false;
            }
        }

        private static void VerifyBindings(TextBlock textblock, ref bool result)
        {
            if (textblock.FontSize != 200)
            {
                GlobalLog.LogEvidence("Text.FontSize should be 200");
                result = false;
            }
            if (textblock.FontStyle != System.Windows.FontStyles.Italic)
            {
                GlobalLog.LogEvidence("Text.FontStyle is" + textblock.FontStyle + ", should be 2: Italic");
                result = false;
            }
            VerifyBrushBinding(textblock, ref result);
        }

        private static void VerifyBrushBinding(TextBlock textblock, ref bool result)
        {
            SolidColorBrush brush = textblock.Foreground as SolidColorBrush;
            if (!Color.Equals(brush.Color, Colors.Red))
            {
                GlobalLog.LogEvidence("!Color.Equals(brush.Color, Colors.Red)");
                result = false;
            }
        }
    }
}
