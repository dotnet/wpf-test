// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.IncludeTag
{
    /// <summary/>
    public static class IncludeTagVerification_Verify
    {
        /// <summary>
        /// Verifies x:Include tag is parsed and tree is created correctly.
        /// </summary>
        public static bool Verify(System.Windows.UIElement rootElement)
        {
            bool result = true;
            StackPanel stackPanel = (StackPanel)LogicalTreeHelper.FindLogicalNode(rootElement, "Panel1");
            if (null == stackPanel)
            {
                GlobalLog.LogStatus("not found StackPanel.");
                result = false;
            }
            else
                GlobalLog.LogStatus("found StackPanel.");

            //
            // Verify StackPanel's Background is a resource.
            //
            FrameworkElement fe = (FrameworkElement)rootElement;
            if (null == fe)
            {
                GlobalLog.LogStatus("Root is not a FrameworkElement.");
                result = false;
            }
            else
                GlobalLog.LogStatus("Root is a FrameworkElement.");

            Brush res = (Brush)fe.FindResource("GreenBrush");
            if (null == res)
            {
                GlobalLog.LogStatus("GreenBrush not found.");
                result = false;
            }
            else
                GlobalLog.LogStatus("GreenBrush found.");

            SolidColorBrush brush = (SolidColorBrush)stackPanel.Background;

            if (brush != res)
            {
                GlobalLog.LogEvidence("Resource brush != stackPanel.Background brush.");
                result = false;
            }

            //
            // Verify StackPanel's Background is green and half-transparent.
            //
            Color color = brush.Color;

            if (color.A != 50)
            {
                GlobalLog.LogEvidence("SolidColorBrush.Color.A != 50");
                result = false;
            }
            else if (color.G != 219)
            {
                GlobalLog.LogEvidence("SolidColorBrush.Color.G != 219");
                result = false;
            }
            else if (color.B != 0)
            {
                GlobalLog.LogEvidence("SolidColorBrush.Color.B != 0");
                result = false;
            }
            else if (color.R != 0)
            {
                GlobalLog.LogEvidence("SolidColorBrush.Color.R != 0");
                result = false;
            }
            return result;
        }
    }
}
