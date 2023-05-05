// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace RegressionIssue43
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // This is a markup compiler test. We pass if the project compiled file.
            // Verifies correct attributes are added to the generated, compilation will
            // fail if not present since they will generate warnings and warnings are 
            // treated as errors.
            Application.Current.Shutdown(0);
        }
    }
}
