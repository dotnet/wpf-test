// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Styles.BasedOn
{
    /// <summary/>
    public static class StyleOverride04_Verify
    {
        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Verify application structure.");

            GlobalLog.LogStatus("1. Root element is a StackPanel.");
            StackPanel sp = rootElement as StackPanel;
            if (sp == null)
            {
                GlobalLog.LogEvidence("sp == null");
                result = false;
            }

            GlobalLog.LogStatus("2. 2 child elements present.");
            if (sp.Children.Count != 2)
            {
                GlobalLog.LogEvidence("sp.Children.Count != 2");
                result = false;
            }

            GlobalLog.LogStatus("3. All child elements are buttons.");
            Button btnCtrl = sp.Children[0] as Button;
            if (btnCtrl == null)
            {
                GlobalLog.LogEvidence("btnCtrl == null");
                result = false;
            }
            Button btnExp = sp.Children[1] as Button;
            if (btnExp == null)
            {
                GlobalLog.LogEvidence("btnExp == null");
                result = false;
            }

            CV04ValidateState(btnCtrl, btnExp, ref result);
            return result;
        }

        private static void CV04ValidateState(Button btnCtrl, Button btnExp, ref bool result)
        {
            GlobalLog.LogStatus("Verify control button state.");
            GlobalLog.LogStatus("1. Red button background.");
            if (((SolidColorBrush) btnCtrl.Background).Color != Colors.Red)
            {
                GlobalLog.LogEvidence("((SolidColorBrush)btnCtrl.Background).Color != Colors.Red");
                result = false;
            }

            GlobalLog.LogStatus("Verify experimental button state.");
            GlobalLog.LogStatus("1. Red button background.");
            if (((SolidColorBrush) btnExp.Background).Color != Colors.Red)
            {
                GlobalLog.LogEvidence("((SolidColorBrush)btnExp.Background).Color != Colors.Red");
                result = false;
            }
            GlobalLog.LogStatus("2. 100px button width.");
            if (btnExp.Width != 100)
            {
                GlobalLog.LogEvidence("btnExp.Width != 100");
                result = false;
            }
        }
    }
}
