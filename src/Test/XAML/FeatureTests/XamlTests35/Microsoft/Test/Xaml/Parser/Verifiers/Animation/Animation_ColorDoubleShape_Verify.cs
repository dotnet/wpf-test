// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Animation
{
    /// <summary/>
    public static class Animation_ColorDoubleShape_Verify
    {
        /// <summary>
        /// Animation_ColorDoubleShape_Verify
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

            GlobalLog.LogStatus("Verifying color Ellipse ...");

            Ellipse ball1 = (Ellipse) LogicalTreeHelper.FindLogicalNode(rootElement, "ball1");

            if (ball1.Width != 80)
            {
                GlobalLog.LogEvidence("ball1.Width != 80");
                result = false;
            }
            if (ball1.Height != 80)
            {
                GlobalLog.LogEvidence("ball1.Height != 80");
                result = false;
            }
            if ((double) ball1.GetValue(Canvas.LeftProperty) != 10)
            {
                GlobalLog.LogEvidence("(double)ball1.GetValue(Canvas.LeftProperty) != 10");
                result = false;
            }
            if ((double) ball1.GetValue(Canvas.TopProperty) != 60)
            {
                GlobalLog.LogEvidence("(double)ball1.GetValue(Canvas.TopProperty) != 60");
                result = false;
            }

            GlobalLog.LogStatus("Verifying solid color brush ...");
            SolidColorBrush myBrush = ((Shape) ball1).Fill as SolidColorBrush;
            if (!Color.Equals(myBrush.Color, Colors.Red))
            {
                GlobalLog.LogEvidence("!Color.Equals(myBrush.Color, Colors.Red)");
                result = false;
            }
            return result;
        }
    }
}
