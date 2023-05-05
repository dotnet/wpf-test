// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.Test.DataServices;


namespace Microsoft.Test.DataServices
{
    public class ListBoxItemList : ObservableCollection<ListBoxItem>
    {
        public ListBoxItemList()
        {
            ListBoxItem lbi1 = new ListBoxItem();
            lbi1.Content = "item 1";
            ListBoxItem lbi2 = new ListBoxItem();
            lbi2.Content = "item 2";
            lbi2.IsSelected = true;
            ListBoxItem lbi3 = new ListBoxItem();
            lbi3.Content = "item 3";
            ListBoxItem lbi4 = new ListBoxItem();
            lbi4.Content = "item 4";
            lbi4.IsSelected = true;
            ListBoxItem lbi5 = new ListBoxItem();
            lbi5.Content = "item 5";

            Add(lbi1);
            Add(lbi2);
            Add(lbi3);
            Add(lbi4);
            Add(lbi5);
        }
    }
}
