// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Layout
{
    /// <summary/>
    public static class LayoutBorder_Verify
    {
        /// <summary>
        /// LayoutBorder_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Inside LayoutXamlVerifiers.BorderVerify()...");

            StackPanel myPanel = rootElement as StackPanel;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be StackPanel");
                result = false;
            }

            UIElementCollection myChildren = myPanel.Children;
            GlobalLog.LogStatus(myChildren.ToString());
            GlobalLog.LogStatus(myPanel.Children.Count.ToString());

            //Border myBorder = IDedObjects["border"] as Border;
            //  if (null == myBorder)
            //  {
            //      GlobalLog.LogEvidence("Should Have a border");
            //  }
            //  VerifyElement.VerifyDouble(((FrameworkElement)myBorder).Height, 100);
            //  VerifyElement.VerifyDouble(((FrameworkElement)myBorder).Width, 100);
            //  VerifyElement.VerifyThickness(((FrameworkElement)myBorder).Margin, new Thickness(10));
            //  VerifyElement.VerifyColor(((SolidColorBrush)(myBorder.Background)).Color, Colors.Orange);
            //  VerifyElement.VerifyColor(((SolidColorBrush)(myBorder.BorderBrush)).Color, Colors.RoyalBlue);
            //  VerifyElement.VerifyThickness(myBorder.BorderThickness, new Thickness(10, 5, 25, 11));
            return result;
        }
    }
}
