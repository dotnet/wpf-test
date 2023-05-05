// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Serialization.CustomElements;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Test.Xaml.Utilities;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.CompatReader
{
    /// <summary/>
    public static class CompatReaderTest2_Other_Verify
    {
        /// <summary>
        /// Verification routine for CompatReaderTest2.xaml, 
        /// which tests different aspects of AlternateContent, a 
        /// Markup compatibility tag.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            CustomStackPanel customstackpanel0 = (CustomStackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "CustomStackPanel0");
            bool             result            = true;
            string           transControlsNS   = "Com.ControlStore.";
            string           v2                = "ParserTestControlsV2SubsumingV1";

            // The only child should be v2:TransButton with a Red background
            // and Content="This is a v2 button"
            FrameworkElement transButton = XamlTestHelper.VerifyChildType(customstackpanel0, 1, v2, transControlsNS + "TransButton", ref result);

            SolidColorBrush background = transButton.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            if (!Color.Equals(background.Color, Colors.Red))
            {
                GlobalLog.LogEvidence("!Color.Equals(background.Color, Colors.Red)");
                result = false;
            }

            string content = (string) transButton.GetValue(ContentControl.ContentProperty);
            if ("This is a v2 button" != content)
            {
                GlobalLog.LogEvidence("This is a v2 button != content");
                result = false;
            }
            // No more children
            XamlTestHelper.VerifyNoChild(customstackpanel0, 2, ref result);
            return result;
        }
    }
}
