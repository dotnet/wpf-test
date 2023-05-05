// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Animation
{
    /// <summary/>
    public static class Animation_PointDoubleShape_Verify
    {
        /// <summary>
        /// Animation_PointDoubleShape_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            //former dockpanelverify
            DockPanel myPanel = rootElement as DockPanel;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be DockPanel");
                result = false;
            }

            if (!Color.Equals(((SolidColorBrush)(myPanel.Background)).Color, Colors.White))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(myPanel.Background)).Color, Colors.White)");
                result = false;
            }

            Canvas myCanvas = (Canvas) LogicalTreeHelper.FindLogicalNode(rootElement, "Canvas");

            if (null == myCanvas)
            {
                GlobalLog.LogEvidence("Should be Canvas");
                result = false;
            }
            if (myCanvas.Height != 500)
            {
                GlobalLog.LogEvidence("myCanvas.Height != 500");
                result = false;
            }
            if (myCanvas.Width != 500)
            {
                GlobalLog.LogEvidence("myCanvas.Width != 500");
                result = false;
            }
            //end
            GlobalLog.LogStatus("Verifying PointDoubleShape ...");

            Rectangle rectangle = (Rectangle) LogicalTreeHelper.FindLogicalNode(rootElement, "Rectangle");

            if (rectangle == null)
            {
                GlobalLog.LogEvidence("rectangle == null");
                result = false;
            }
            //Verify storyboards, blocked
            Storyboard storyboard1 = LogicalTreeHelper.FindLogicalNode(rootElement, "storyboard1") as Storyboard;
            if (null == storyboard1)
            {
                GlobalLog.LogEvidence("null == storyboard1");
                result = false;
            }
            Storyboard storyboard2 = LogicalTreeHelper.FindLogicalNode(rootElement, "storyboard2") as Storyboard;
            if (null == storyboard2)
            {
                GlobalLog.LogEvidence("null == storyboard2");
                result = false;
            }
            return result;
        }
    }
}
