// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication3
{
    class MyControl : Button
    {
        public static readonly DependencyProperty MyArrayListProperty =
        DependencyProperty.Register("MyArrayList",
        typeof(ArrayList), typeof(MyControl), null);

        public ArrayList MyArrayList
        {
            get { return (ArrayList)GetValue(MyArrayListProperty); }
            set { SetValue(MyArrayListProperty, value); }
        }
    }
}
