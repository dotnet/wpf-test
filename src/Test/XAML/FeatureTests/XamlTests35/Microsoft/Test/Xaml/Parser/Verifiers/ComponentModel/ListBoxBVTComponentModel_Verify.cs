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
    public static class ListBoxBVTComponentModel_Verify
    {
        /// <summary>
        /// ListBoxBVTComponentModel_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool       result  = true;
            StackPanel myPanel = rootElement as StackPanel;

            VerifyElement.VerifyBool(null == myPanel, false, ref result);

            GlobalLog.LogStatus("Verifying Button ...");

            ListBox myListBox = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox");

            VerifyElement.VerifyBool(null == myListBox, false, ref result);

            ItemCollection myItems = myListBox.Items as ItemCollection;

            VerifyElement.VerifyBool(null == myItems, false, ref result);
            VerifyElement.VerifyInt(myItems.Count, 6, ref result);

            ListBoxItem item = myItems[0] as ListBoxItem;

            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "ListItem0", ref result);

            item = myItems[1] as ListBoxItem;
            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "ListItem1", ref result);

            item = myItems[2] as ListBoxItem;
            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "ListItem2", ref result);

            item = myItems[3] as ListBoxItem;
            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "ListItem3", ref result);

            item = myItems[4] as ListBoxItem;
            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "ListItem4", ref result);

            item = myItems[5] as ListBoxItem;
            VerifyElement.VerifyBool(null == item, false, ref result);
            VerifyElement.VerifyString((string) item.Content, "ListItem5", ref result);

            return result;
        }
    }
}
