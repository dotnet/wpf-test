// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Misc
{
    /// <summary/>
    public static class ClrProperty_Verify
    {
        /// <summary>
        /// Verify the properties of the button.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            Button button1 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button1");

            // Verify the foreground (Red), 
            if (!Color.Equals((button1.Foreground as SolidColorBrush).Color, Colors.Red))
            {
                GlobalLog.LogEvidence("!Color.Equals((Button1.Foreground as SolidColorBrush).Color, Colors.Red)");
                result = false;
            }

            // background (Blue)    
            if (!Color.Equals((button1.Background as SolidColorBrush).Color, Colors.Blue))
            {
                GlobalLog.LogEvidence("!Color.Equals((Button1.Background as SolidColorBrush).Color, Colors.Blue)");
                result = false;
            }
            // and borderbrush (Green) properties.
            if (!Color.Equals((button1.BorderBrush as SolidColorBrush).Color, Colors.Green))
            {
                GlobalLog.LogEvidence("!Color.Equals((Button1.BorderBrush as SolidColorBrush).Color, Colors.Green)");
                result = false;
            }
            return result;
        }
    }
}
