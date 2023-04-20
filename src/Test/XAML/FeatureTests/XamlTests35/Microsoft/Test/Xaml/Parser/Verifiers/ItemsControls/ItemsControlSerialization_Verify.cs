// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.Verifiers.ItemsControls
{
    /// <summary/>
    public static class ItemsControlSerialization_Verify
    {
        /// <summary>
        /// Verify routine for ItemsControlTest.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Inside ParserVerifier.ItemsControlTestVerify()...");

            GlobalLog.LogStatus("Verify that ItemsControl can be found.");
            ItemsControl control = (ItemsControl) LogicalTreeHelper.FindLogicalNode(rootElement, "ItemsControl");
            VerifyElement.VerifyBool(null != control, true, ref result);

            GlobalLog.LogStatus("Verify that ItemsControl has 4 items.");
            ItemCollection items = control.Items;
            VerifyElement.VerifyInt(items.Count, 4, ref result);
            return result;
        }
    }
}
