// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Serialization.CustomElements;
using System.Windows;
using Microsoft.Test.Xaml.Utilities;
using Microsoft.Test.Logging;
using System.Windows.Controls;

namespace Microsoft.Test.Xaml.Parser.Verifiers.CompatReader
{
    /// <summary/>
    public static class CompatReaderTest2_V1_Verify
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
            string           v1                = "ParserTestControlsV1";

            // First child should be a v1:TransButton, with Content="Hello"
            // and no children except "Hello"
            FrameworkElement transButton = XamlTestHelper.VerifyChildType(customstackpanel0, 1, v1, transControlsNS + "TransButton", ref result);
            string           content     = (string) transButton.GetValue(ContentControl.ContentProperty);
            if ("Hello" != content)
            {
                GlobalLog.LogEvidence("Hello != content");
                result = false;
            }

            // "Hello" is also the first logical child. There shouldn't be any 
            // more logical children.
            XamlTestHelper.VerifyNoChild(transButton, 2, ref result);

            // There should be no more children
            XamlTestHelper.VerifyNoChild(customstackpanel0, 2, ref result);
            return result;
        }
    }
}
