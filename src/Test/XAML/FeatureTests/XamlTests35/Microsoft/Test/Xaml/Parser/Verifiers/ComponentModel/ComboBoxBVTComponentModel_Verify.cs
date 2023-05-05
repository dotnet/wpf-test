// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.Verifiers.ComponentModel
{
    /// <summary/>
    public static class ComboBoxBVTComponentModel_Verify
    {
        /// <summary>
        /// ComboBoxBVTComponentModel_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool       result  = true;
            StackPanel myPanel = rootElement as StackPanel;
            VerifyElement.VerifyBool(null == myPanel, false, ref result);

            GlobalLog.LogStatus("Verifying Combobox ...");

            ComboBox myComboBox = (ComboBox) LogicalTreeHelper.FindLogicalNode(rootElement, "CostCenterCombo");

            VerifyElement.VerifyBool(null == myComboBox, false, ref result);
            VerifyElement.VerifyDouble(myComboBox.Width, 200, ref result);
            ItemCollection myItems = myComboBox.Items as ItemCollection;
            VerifyElement.VerifyBool(null == myItems, false, ref result);
            VerifyElement.VerifyInt(myComboBox.Items.Count, 3, ref result);

            ComboBoxItem item = myComboBox.Items[1] as ComboBoxItem;

            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "Item2", ref result);

            item = myItems[0] as ComboBoxItem;
            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "Item1", ref result);

            item = myItems[2] as ComboBoxItem;

            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "Item3", ref result);
            return result;
        }
    }
}
