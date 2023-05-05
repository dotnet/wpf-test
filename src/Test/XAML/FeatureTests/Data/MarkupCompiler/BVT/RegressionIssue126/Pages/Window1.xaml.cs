// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;

namespace RegressionIssue126
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : NavigationWindow
    {
        public Window1()
        {
            InitializeComponent();
            
        }

        private void NavigationWindow_Loaded(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
        }
        
    }

}
