// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace RegressionIssue65_DefaultStyleInMD_OneLevel
{
    /// <summary>
    /// When resource dictionaries contain default style (style with type keys) they are marked with a flag as an optimization.
    /// When default styles are defined in merged dictionaries, this flag was not propagated to the top level RD. This is the bug.
    /// So default styles defined in merged dictionaries were not found.
    /// This test case checks that default styles defined in merged dictionaries are found.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if ((string)button.Content == "StyleValue")
            {
                Application.Current.Shutdown(0);
            }
            else
            {
                Console.WriteLine("button.Content value. Expected=StyleValue. Got=" + button.Content.ToString());
                Application.Current.Shutdown(1);
            }
        }
    }
}
