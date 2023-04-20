// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.Verifiers.CompatReader
{
    /// <summary/>
    public static class CompatReaderTest1_NoSupportingAssemblies_Verify
    {
        /// <summary>
        /// Verification routine for CompatReaderTest1.xaml,
        /// which tests different aspects of Markup-compatibility attributes, such
        /// as mc:Ignorable and mc:ProcessContent
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            StackPanel stackpanel0 = (StackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "StackPanel0");
            StackPanel stackpanel1 = (StackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "StackPanel1");
            StackPanel stackpanel2 = (StackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "StackPanel2");

            // Verify that all the StackPanels have no children
            XamlTestHelper.VerifyNoChildren(stackpanel0, ref result);
            XamlTestHelper.VerifyNoChildren(stackpanel1, ref result);
            XamlTestHelper.VerifyNoChildren(stackpanel2, ref result);

            return result;
        }
    }
}
