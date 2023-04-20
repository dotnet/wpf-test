// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Demos
{
    /// <summary/>
    public static class PropIEnumerable_Verify
    {
        /// <summary>
        /// Verify that elements and text under an IEnumerable property 
        /// are added to the parent as children.
        /// See Petzold01_Verification.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool                result     = true;
            DockPanel           dockpanel0 = (DockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "DockPanel0");
            UIElementCollection children   = dockpanel0.Children;

            // We cannot verify addition of text, because DockPanel.AddText(string) does nothing.
            if (((children[1] as Button).Content as string) != "dear")
            {
                GlobalLog.LogEvidence("((children[1] as Button).Content as string) != dear");
                result = false;
            }
            return result;
        }
    }
}
