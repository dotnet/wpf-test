// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace RegressionIssue132
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify that locally defined component's event handlers that have signature mismatch are caught at compile time
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void ClickHandler(object sender, RoutedEventArgs args)
        {
        }
    }

    public class MyPanel : StackPanel
    {
    }
}
