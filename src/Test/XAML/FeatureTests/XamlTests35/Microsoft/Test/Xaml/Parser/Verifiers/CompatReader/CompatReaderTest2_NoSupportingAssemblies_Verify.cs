// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Serialization.CustomElements;
using System.Windows;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.Verifiers.CompatReader
{
    /// <summary/>
    public static class CompatReaderTest2_NoSupportingAssemblies_Verify
    {
        /// <summary>
        /// Verification routine for CompatReaderTest2.xaml, 
        /// which tests different aspects of AlternateContent, a 
        /// Markup compatibility tag.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool             result            = true;
            CustomStackPanel customstackpanel0 = (CustomStackPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "CustomStackPanel0");
            XamlTestHelper.VerifyNoChildren(customstackpanel0, ref result);
            return result;
        }
    }
}
