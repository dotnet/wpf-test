// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using Microsoft.Test.Xaml.Utilities;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.BamlizedTheme
{
    /// <summary/>
    public static class BamlizedThemeTest_Verify
    {
        /// <summary>
        /// Cell resources verification
        /// Need to add more strict verification applied to every theme. 
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Inside LayoutXamlVerifiers.BorderVerify()...");

            DockPanel myPanel = rootElement as DockPanel;

            if (null == myPanel)
            {
                throw new TestValidationException("Should be DockPanel");
            }
            VerifyElement.VerifyBool(null == myPanel, false, ref result);

            //GlobalLog.LogStatus("Changing theme.");
            //string originalTheme = DisplayConfiguration.GetTheme();

            Color.FromArgb(0xff, 0x00, 0x00, 0x00);
            SolidColorBrush myFG = null;


            //Button
            Button button1 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "button1");

            VerifyElement.VerifyBool(null == button1, false, ref result);
            myFG = ((Control) button1).Foreground as SolidColorBrush;
            //VerifyElement.VerifyBool(null == myFG, false);
            //myColor = myFG.Color;
            //VerifyElement.VerifyColor(myColor, lunaBlue);
            GlobalLog.LogStatus("Button1 OK");

            Button button2 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "button2");

            VerifyElement.VerifyBool(null == button2, false, ref result);
            //myFG = ((Control)button2).Foreground as SolidColorBrush;
            //VerifyElement.VerifyBool(null == myFG, false);
            //myColor = myFG.Color;
            //VerifyElement.VerifyColor(myColor, Colors.Black);
            GlobalLog.LogStatus("Button2 OK");

            //CheckBox
            CheckBox checkBox1 = (CheckBox) LogicalTreeHelper.FindLogicalNode(rootElement, "checkbox1");

            VerifyElement.VerifyBool(null == checkBox1, false, ref result);
            //myFG = ((Control)checkBox1).Foreground as SolidColorBrush;
            //VerifyElement.VerifyBool(null == myFG, false);
            //myColor = myFG.Color;
            //VerifyElement.VerifyColor(myColor, lunaBlue);
            GlobalLog.LogStatus("checkBox1 OK");

            CheckBox checkBox2 = (CheckBox) LogicalTreeHelper.FindLogicalNode(rootElement, "checkbox2");

            VerifyElement.VerifyBool(null == checkBox2, false, ref result);
            //myFG = ((Control)checkBox2).Foreground as SolidColorBrush;
            //VerifyElement.VerifyBool(null == myFG, false);
            //myColor = myFG.Color;
            //VerifyElement.VerifyColor(myColor, Colors.Black);
            GlobalLog.LogStatus("checkBox1 OK");

            return result;
        }
    }
}
