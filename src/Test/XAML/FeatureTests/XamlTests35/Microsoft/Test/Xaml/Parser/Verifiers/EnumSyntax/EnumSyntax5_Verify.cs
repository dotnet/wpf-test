// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.EnumSyntax
{
    /// <summary/>
    public static class EnumSyntax5_Verify
    {
        /// <summary>
        /// Verifies Avalon Parser's for public static members in property values.  For example:
        /// &lt;Button cc:CustomEnumControl.CustomAttachedBool="*cc:CustomEnumControl.PublicBoolClrProperty" /&gt;
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            //
            // Verify first Button's CustomAttachedBool property.
            //
            GlobalLog.LogStatus("Getting 'TargetElement1'. Should be Button...");

            Button button = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "TargetElement1");

            GlobalLog.LogStatus("Verifying first Button's CustomAttachedBool property is false...");

            bool boolVal = (bool) button.GetValue(CustomEnumControl.CustomAttachedBoolProperty);

            if (boolVal != false)
            {
                GlobalLog.LogEvidence("First Button's CustomAttachedBool property != false.");
                result = false;
            }

            //
            // Verify second Button's CustomAttachedBool property.
            //
            GlobalLog.LogStatus("Getting 'TargetElement2'. Should be Button...");

            button = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "TargetElement2");

            GlobalLog.LogStatus("Verifying second Button's CustomAttachedBool property is false...");

            boolVal = (bool) button.GetValue(CustomEnumControl.CustomAttachedBoolProperty);

            if (boolVal != false)
            {
                GlobalLog.LogEvidence("Second Button's CustomAttachedBool property != false.");
                result = false;
            }
            return result;
        }
    }
}
