// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace RegressionIssue63_SubclassingControlTemplate
{
    /// <summary>
    /// Regression test
    /// Verifies that ControlTemplate derived types can be used in place of ControlTemplate
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

    public class CT : ControlTemplate
    {

    }
}
