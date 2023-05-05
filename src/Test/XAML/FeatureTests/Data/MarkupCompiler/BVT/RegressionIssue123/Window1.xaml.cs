// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace RegressionIssue123
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

        // the Uid on the border should NOT be set
        private void Border_Initialized(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(((UIElement)sender).Uid))
            {
                Application.Current.Shutdown(0);
            }
            else
            {
                Application.Current.Shutdown(-1);
            }
        }
    }
}
