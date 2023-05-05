// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Microsoft.Test.Xaml.Utilities;
using Microsoft.Test.Logging;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.ComponentModel
{
    /// <summary/>
    public static class ButtonBVTComponentModel_Verify
    {
        /// <summary>
        /// ButtonBVTComponentModel_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool       result  = true;
            StackPanel myPanel = rootElement as StackPanel;
            VerifyElement.VerifyBool(null == myPanel, false, ref result);

            GlobalLog.LogStatus("Verifying Button ...");

            Button button = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button");

            VerifyElement.VerifyBool(null == button, false, ref result);

            VerifyElement.VerifyString((string) button.Content, "Button", ref result);

            return result;
        }
    }
}
