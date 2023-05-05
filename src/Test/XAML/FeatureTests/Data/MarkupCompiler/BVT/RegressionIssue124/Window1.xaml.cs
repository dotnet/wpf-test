// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            MyControl myControl1 = (MyControl)(control1.Template.FindName("myc1", control1));
            MyControl myControl2 = (MyControl)(control2.Template.FindName("myc2", control2));

            ArrayList arrayList1 = myControl1.MyArrayList;
            ArrayList arrayList2 = myControl1.MyArrayList;

            // Check if the two types are the same (shared). They 
            // should be shared.
            if (arrayList1.GetHashCode() != arrayList2.GetHashCode())
            {
                Application.Current.Shutdown(-1);
            }
            else
            {
                Application.Current.Shutdown(0);
            }

        }
    }
}
