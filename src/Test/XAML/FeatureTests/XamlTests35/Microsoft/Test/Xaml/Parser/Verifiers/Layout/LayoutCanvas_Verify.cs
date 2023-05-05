// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Serialization.CustomElements;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Logging;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Layout
{
    /// <summary/>
    public static class LayoutCanvas_Verify
    {
        /// <summary>
        /// LayoutCanvas_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            CustomCanvas myPanel = rootElement as CustomCanvas;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be CustomCanvas");
                result = false;
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 2)
            {
                GlobalLog.LogEvidence("Should has 2 children");
                result = false;
            }

            Canvas canvas1 = (Canvas) LogicalTreeHelper.FindLogicalNode(rootElement, "Canvas1");
            Canvas canvas2 = (Canvas) LogicalTreeHelper.FindLogicalNode(rootElement, "Canvas2");

            if (null == canvas1 || null == canvas2)
            {
                GlobalLog.LogEvidence("Child canvas is missed");
                result = false;
            }

            // location
            if (Canvas.GetTop(canvas1) != 25)
            {
                GlobalLog.LogEvidence("Canvas.GetTop(canvas1) != 25");
                result = false;
            }
            if (Canvas.GetRight(canvas1) != 25 * 96)
            {
                GlobalLog.LogEvidence("Canvas.GetRight(canvas1) != 25 * 96");
                result = false;
            }
            if (Canvas.GetLeft(canvas2) != 25)
            {
                GlobalLog.LogEvidence("Canvas.GetLeft(canvas2) != 25");
                result = false;
            }
            if (Canvas.GetBottom(canvas2) != 25)
            {
                GlobalLog.LogEvidence("Canvas.GetBottom(canvas2) != 25");
                result = false;
            }

            //backgroud
            if (!Color.Equals(((SolidColorBrush)(((Panel) canvas1).Background)).Color, Colors.Gray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)canvas1).Background)).Color, Colors.Gray)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(((Panel) canvas2).Background)).Color, Colors.RoyalBlue))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)canvas2).Background)).Color, Colors.RoyalBlue)");
                result = false;
            }

            //size
            if (((FrameworkElement) canvas1).Height != 100)
            {
                GlobalLog.LogEvidence("((FrameworkElement)canvas1).Height != 100");
                result = false;
            }
            if (((FrameworkElement) canvas1).Width != 100)
            {
                GlobalLog.LogEvidence("((FrameworkElement)canvas1).Width != 100");
                result = false;
            }
            if (((FrameworkElement) canvas2).Height != 100)
            {
                GlobalLog.LogEvidence("((FrameworkElement)canvas2).Height != 100");
                result = false;
            }
            if (((FrameworkElement) canvas2).Width != 100)
            {
                GlobalLog.LogEvidence("((FrameworkElement)canvas2).Width != 100");
                result = false;
            }
            return result;
        }
    }
}
