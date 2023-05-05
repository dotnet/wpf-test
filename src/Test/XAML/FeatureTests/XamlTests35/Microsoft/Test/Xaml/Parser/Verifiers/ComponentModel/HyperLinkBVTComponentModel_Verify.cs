// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;
using System.Windows.Documents;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.ComponentModel
{
    /// <summary/>
    public static class HyperLinkBVTComponentModel_Verify
    {
        /// <summary>
        /// HyperLinkBVTComponentModel_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool       result  = true;
            StackPanel myPanel = rootElement as StackPanel;
            VerifyElement.VerifyBool(null == myPanel, false, ref result);

            GlobalLog.LogStatus("Verifying Hyperlink ...");

            Hyperlink myHyperlink = (Hyperlink) LogicalTreeHelper.FindLogicalNode(rootElement, "Hyperlink");

            VerifyElement.VerifyBool(null == myHyperlink, false, ref result);
            VerifyElement.VerifyString((string)((Run)(myHyperlink.Inlines.FirstInline)).Text, "Hyperlink", ref result);

            return result;
        }
    }
}
