// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Misc
{
    /// <summary/>
    public static class PropertyIList_Verify
    {
        /// <summary>
        /// Verify IList properties.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            ListBox        listbox0 = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox0");
            ItemCollection items    = listbox0.Items;

            if (items.Count != 4)
            {
                GlobalLog.LogEvidence("Items.Count != 4");
                result = false;
            }

            if (!(items[0] as String).Equals("Hello 0 "))
            {
                GlobalLog.LogEvidence("!(Items[0] as String).Equals(Hello 0 )");
                result = false;
            }
            if (!((items[1] as ContentControl).Content as String).Equals("Hello 1"))
            {
                GlobalLog.LogEvidence("!((Items[1] as ContentControl).Content as String).Equals(Hello 1)");
                result = false;
            }
            if (!((items[2] as ContentControl).Content as String).Equals("Hello 2"))
            {
                GlobalLog.LogEvidence("!((Items[2] as ContentControl).Content as String).Equals(Hello 2)");
                result = false;
            }
            if (!((items[3] as ContentControl).Content as String).Equals("Hello 3"))
            {
                GlobalLog.LogEvidence("!((Items[3] as ContentControl).Content as String).Equals(Hello 3)");
                result = false;
            }
            return result;
        }
    }
}
