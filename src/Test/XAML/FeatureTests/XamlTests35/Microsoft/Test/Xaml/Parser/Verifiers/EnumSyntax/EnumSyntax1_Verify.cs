// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.EnumSyntax
{
    /// <summary/>
    public static class EnumSyntax1_Verify
    {
        /// <summary>
        /// Verifies Avalon Parser's Enum support for:
        /// &lt;Button DockPanel.Dock="*Dock.Right" /&gt;
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            //
            // Verify first Button's Dock property.
            //
            GlobalLog.LogStatus("Getting 'TargetElement1'. Should be Button...");

            Button button = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "TargetElement1");

            Dock myDock = DockPanel.GetDock(button);

            GlobalLog.LogStatus("Verifying first Button's Dock property is Right...");
            if (myDock != Dock.Right)
            {
                GlobalLog.LogEvidence("First Button's Dock property != Right.");
                result = false;
            }

            //
            // Verify second Button's Dock property.
            //
            GlobalLog.LogStatus("Getting 'TargetElement2'. Should be Button...");
            button = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "TargetElement2");

            myDock = DockPanel.GetDock(button);

            GlobalLog.LogStatus("Verifying second Button's Dock property is Bottom...");
            if (myDock != Dock.Bottom)
            {
                GlobalLog.LogEvidence("Second Button's Dock property != Bottom.");
                result = false;
            }
            return result;
        }
    }
}
