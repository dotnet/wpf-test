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
    public static class CompatReaderTest1_V1_Verify
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

            //
            // Verify the tree for StackPanel0
            //
            // First child is a v1:TransButton with Background="Blue"
            // This child doesn't have any children
            FrameworkElement transButton1 = XamlTestHelper.VerifyChildType(stackpanel0, 1, v1, transControlsNS + "TransButton", ref result);
            SolidColorBrush  background   = transButton1.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            if (!Color.Equals(background.Color, Colors.Blue))
            {
                GlobalLog.LogEvidence("!Color.Equals(background.Color, Colors.Blue)");
                result = false;
            }
            XamlTestHelper.VerifyNoChildren(transButton1, ref result);

            // Second child should be a v1:TransButton, with no children
            FrameworkElement transButton2 = XamlTestHelper.VerifyChildType(stackpanel0, 2, v1, transControlsNS + "TransButton", ref result);
            XamlTestHelper.VerifyNoChildren(transButton2, ref result);

            // Third, fourth and fifth children should be v1:TransListBoxItems
            XamlTestHelper.VerifyChildType(stackpanel0, 3, v1, transControlsNS + "TransListBoxItem", ref result);
            XamlTestHelper.VerifyChildType(stackpanel0, 4, v1, transControlsNS + "TransListBoxItem", ref result);
            XamlTestHelper.VerifyChildType(stackpanel0, 5, v1, transControlsNS + "TransListBoxItem", ref result);

            // There should be no more children
            XamlTestHelper.VerifyNoChild(stackpanel0, 6, ref result);

            //
            // Verify the tree for StackPanel1
            //
            // First child should be a v1:TransButton with no children
            FrameworkElement transButton = XamlTestHelper.VerifyChildType(stackpanel1, 1, v1, transControlsNS + "TransButton", ref result);
            XamlTestHelper.VerifyNoChildren(transButton, ref result);

            // Second child should be a v1:TransListBoxItem
            FrameworkElement transListBoxItem = XamlTestHelper.VerifyChildType(stackpanel1, 2, v1, transControlsNS + "TransListBoxItem", ref result);

            // No More children
            XamlTestHelper.VerifyNoChild(stackpanel1, 3, ref result);

            //
            // Vefiry the tree for StackPanel2
            //
            // First child should be a v1:TransButton
            transButton = XamlTestHelper.VerifyChildType(stackpanel2, 1, v1, transControlsNS + "TransButton", ref result);

            // No more children
            XamlTestHelper.VerifyNoChild(stackpanel2, 2, ref result);
            return result;
        }
    }
}
