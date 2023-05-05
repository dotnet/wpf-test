// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace RegressionIssue66_AttachedPropertyBinding
{
    /// <summary>
    /// Test case for XamlTypeName resolution failure in Binding.PropertyPath failed silently in v3, throws now
    /// Binding to attached property on another types would fail silently in 3.5, in 4.0 it would throw
    /// The app will crash if there is a regression
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
