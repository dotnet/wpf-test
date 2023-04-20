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

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary/>
    public static class TypeExtension_Verify
    {
        /// <summary>
        /// TypeExtensionVerify
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool    result   = true;
            Button  button0  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button0");
            ListBox listbox0 = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox0");

            // Verify Background, Foreground, MyClass.MyColor and Content properties for Button0
            SolidColorBrush background = (SolidColorBrush) button0.GetValue(Control.BackgroundProperty);
            if (!Color.Equals(background.Color, Colors.Yellow))
            {
                result = false;
                GlobalLog.LogEvidence("Button0's background color did not match");
            }
            SolidColorBrush foreground = (SolidColorBrush) button0.GetValue(Control.ForegroundProperty);
            if (!Color.Equals(foreground.Color, Colors.Red))
            {
                result = false;
                GlobalLog.LogEvidence("Foreground color did not match");
            }


            String[] content = (String[]) button0.GetValue(ContentControl.ContentProperty);
            if ("Hello" != content[0])
            {
                result = false;
                GlobalLog.LogEvidence("content[0] did not match");
            }
            if ("World" != content[1])
            {
                result = false;
                GlobalLog.LogEvidence("content[1] did not match");
            }

            // Verify Background for ListBox0. The SelectionMode property cannot be verified, 
            // since it't not assigned any particular value.
            background = listbox0.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            if (!Color.Equals(background.Color, Colors.Blue))
            {
                result = false;
                GlobalLog.LogEvidence("ListBox0's background color did not match");
            }
            return result;
        }
    }
}
