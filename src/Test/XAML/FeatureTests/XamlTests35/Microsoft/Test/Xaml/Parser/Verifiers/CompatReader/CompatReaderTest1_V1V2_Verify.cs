// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Xaml.Utilities;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.CompatReader
{
    /// <summary/>
    public static class CompatReaderTest1_V1V2_Verify
    {
        /// <summary>
        /// Verification routine for CompatReaderTest1.xaml,
        /// which tests different aspects of Markup-compatibility attributes, such
        /// as mc:Ignorable and mc:ProcessContent
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            StackPanel stackpanel0     = (StackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "StackPanel0");
            StackPanel stackpanel1     = (StackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "StackPanel1");
            StackPanel stackpanel2     = (StackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "StackPanel2");
            bool       result          = true;
            string     transControlsNS = "Com.ControlStore.";
            string     v1              = "ParserTestControlsV1";
            string     v2              = "ParserTestControlsV2";

            //
            // Verify the tree for StackPanel0
            //
            // First child is a v1:TransButton, which has a v2:TransButton child
            FrameworkElement transButton = XamlTestHelper.VerifyChildType(stackpanel0, 1, v1, transControlsNS + "TransButton", ref result);
            XamlTestHelper.VerifyChildType(transButton, 1, v2, transControlsNS + "TransButton", ref result);

            // Second child should be a v2:TransButton, which has a v1:TransButton child
            transButton = XamlTestHelper.VerifyChildType(stackpanel0, 2, v2, transControlsNS + "TransButton", ref result);
            SolidColorBrush background = transButton.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            if (!Color.Equals(background.Color, Colors.Blue))
            {
                GlobalLog.LogEvidence("!Color.Equals(background.Color, Colors.Blue)");
                result = false;
            }
            XamlTestHelper.VerifyChildType(transButton, 1, v1, transControlsNS + "TransButton", ref result);

            // Third child is a v2:TransListBox, with v1:TransListBoxItem children
            FrameworkElement transListBox = XamlTestHelper.VerifyChildType(stackpanel0, 3, v2, transControlsNS + "TransListBox", ref result);
            XamlTestHelper.VerifyChildType(transListBox, 1, v1, transControlsNS + "TransListBoxItem", ref result);
            XamlTestHelper.VerifyChildType(transListBox, 2, v1, transControlsNS + "TransListBoxItem", ref result);
            XamlTestHelper.VerifyChildType(transListBox, 3, v1, transControlsNS + "TransListBoxItem", ref result);

            //
            // Verify the tree for StackPanel1
            //
            // First child should be a v2:TransButton, which has a v1:TransButton child
            transButton = XamlTestHelper.VerifyChildType(stackpanel1, 1, v2, transControlsNS + "TransButton", ref result);
            XamlTestHelper.VerifyChildType(transButton, 1, v1, transControlsNS + "TransButton", ref result);

            // Second child is a v2:TransListBox, with a v1:TransListBoxItem child
            transListBox = XamlTestHelper.VerifyChildType(stackpanel1, 2, v2, transControlsNS + "TransListBox", ref result);
            XamlTestHelper.VerifyChildType(transListBox, 1, v1, transControlsNS + "TransListBoxItem", ref result);

            //
            // Verify the tree for StackPanel2
            //
            // First child should be a v2:TransButton, which has a v1:TransButton child
            transButton = XamlTestHelper.VerifyChildType(stackpanel2, 1, v2, transControlsNS + "TransButton", ref result);
            XamlTestHelper.VerifyChildType(transButton, 1, v1, transControlsNS + "TransButton", ref result);

            // Second child is a v2:TransListBox, with a v1:TransListBoxItem child
            transListBox = XamlTestHelper.VerifyChildType(stackpanel2, 2, v2, transControlsNS + "TransListBox", ref result);
            XamlTestHelper.VerifyChildType(transListBox, 1, v1, transControlsNS + "TransListBoxItem", ref result);
            return result;
        }
    }
}
