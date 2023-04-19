// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.Test.WPF.AppModel.Deployment
{
    /// <summary>
    /// Interaction logic for NestedToolbar.xaml
    /// </summary>
    public partial class NestedToolbar : UserControl
    {
        private static int s_hitCount = 0;

        public NestedToolbar()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            s_hitCount++;
            nestedTextBox.Text = ((Button)sender).Name + " Accelerator Received " + s_hitCount;
            ((BasicFTAppContent)Application.Current.MainWindow.Content).transAccelIndicator.Text = ((Button)sender).Name + " Accelerator Received " + s_hitCount.ToString();
        }
    }
}
