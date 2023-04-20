// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace RegressionIssue67_xArrayInsideRD
{
    /// <summary>
    /// Regression test: Baml to Objects: x:Array inside RD won't work unless the first item inside the RD is an x:Array
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
