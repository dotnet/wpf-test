// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Animation
{
    /// <summary/>
    public static class Animation_DoubleControlKeyspline_Verify
    {
        /// <summary>
        /// Animation_DoubleControlKeyspline_Verify
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

            GlobalLog.LogStatus("Verifying DoubleControlKeyspline ...");

            TextBlock text = (TextBlock) LogicalTreeHelper.FindLogicalNode(rootElement, "myText");

            if (text == null)
            {
                GlobalLog.LogEvidence("text == null");
                result = false;
            }
            if (text.Opacity != 1.0)
            {
                GlobalLog.LogEvidence("text.Opacity != 1.0");
                result = false;
            }
            //Verify storyboards, blocked
            //Storyboard storyboard1 = IdTestBaseCase.FindElementWithId(rootElement, "storyboard1") as Storyboard;
            //VerifyElement.VerifyBool(null == storyboard1, false);
            //Storyboard storyboard2 = IdTestBaseCase.FindElementWithId(rootElement, "storyboard2") as Storyboard;
            //VerifyElement.VerifyBool(null == storyboard2, false);
            return result;
        }
    }
}
