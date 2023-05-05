// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Layout
{
    /// <summary/>
    public static class LayoutStackPanel_Verify
    {
        /// <summary>
        /// LayoutStackPanel_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool       result  = true;
            StackPanel myPanel = rootElement as StackPanel;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be StackPanel");
                result = false;
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 2)
            {
                GlobalLog.LogEvidence("myChildren.Count != 2");
                result = false;
            }

            StackPanel stackPanel1 = (StackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "StackPanel1");
            StackPanel stackPanel2 = (StackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "StackPanel2");
            StackPanel stackPanel3 = (StackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "StackPanel3");
            StackPanel stackPanel4 = (StackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "StackPanel4");

            if (null == stackPanel1 || null == stackPanel2 || null == stackPanel3 || null == stackPanel4)
            {
                GlobalLog.LogEvidence("Some child StackPanels are missing.");
                result = false;
            }
            if (stackPanel1.Children.Count != 1)
            {
                GlobalLog.LogEvidence("stackPanel1.Children.Count!= 1");
                result = false;
            }
            if (stackPanel3.Children.Count != 1)
            {
                GlobalLog.LogEvidence("stackPanel3.Children.Count!= 1");
                result = false;
            }
            //verify size
            //size  for children

            if (((FrameworkElement) stackPanel1).Height != 100)
            {
                GlobalLog.LogEvidence("((FrameworkElement)stackPanel1).Height!= 100");
                result = false;
            }
            if (((FrameworkElement) stackPanel1).Width != 100)
            {
                GlobalLog.LogEvidence("((FrameworkElement)stackPanel1).Width!= 100");
                result = false;
            }

            if (((FrameworkElement) stackPanel2).Height != 50)
            {
                GlobalLog.LogEvidence("((FrameworkElement)stackPanel2).Height!= 50");
                result = false;
            }
            if (((FrameworkElement) stackPanel2).Width != 50)
            {
                GlobalLog.LogEvidence("((FrameworkElement)stackPanel2).Width!= 50");
                result = false;
            }

            if (((FrameworkElement) stackPanel3).Height != 100)
            {
                GlobalLog.LogEvidence("((FrameworkElement)stackPanel3).Height!= 100");
                result = false;
            }
            if (((FrameworkElement) stackPanel3).Width != 100)
            {
                GlobalLog.LogEvidence("((FrameworkElement)stackPanel3).Width!= 100");
                result = false;
            }

            if (((FrameworkElement) stackPanel4).Height != 50)
            {
                GlobalLog.LogEvidence("((FrameworkElement)stackPanel4).Height!= 50");
                result = false;
            }
            if (((FrameworkElement) stackPanel4).Width != 50)
            {
                GlobalLog.LogEvidence("((FrameworkElement)stackPanel4).Width!= 50");
                result = false;
            }
            //background
            if (!Color.Equals(((SolidColorBrush)(((Panel) myPanel).Background)).Color, Colors.Crimson))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)myPanel).Background)).Color, Colors.Crimson)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(((Panel) stackPanel1).Background)).Color, Colors.LawnGreen))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)stackPanel1).Background)).Color, Colors.LawnGreen)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(((Panel) stackPanel2).Background)).Color, Colors.RoyalBlue))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)stackPanel2).Background)).Color, Colors.RoyalBlue)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(((Panel) stackPanel3).Background)).Color, Colors.Orange))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)stackPanel3).Background)).Color, Colors.Orange)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(((Panel) stackPanel4).Background)).Color, Colors.RoyalBlue))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)stackPanel4).Background)).Color, Colors.RoyalBlue)");
                result = false;
            }
            //alignments
            if ((int)(stackPanel1.HorizontalAlignment) != (int)(HorizontalAlignment.Center))
            {
                GlobalLog.LogEvidence("(int)(stackPanel1.HorizontalAlignment)!= (int)(System.Windows.HorizontalAlignment.Center)");
                result = false;
            }
            if ((int)(stackPanel3.VerticalAlignment) != (int)(VerticalAlignment.Center))
            {
                GlobalLog.LogEvidence("(int)(stackPanel3.VerticalAlignment) != (int)(VerticalAlignment.Center)");
                result = false;
            }
            return result;
        }
    }
}
