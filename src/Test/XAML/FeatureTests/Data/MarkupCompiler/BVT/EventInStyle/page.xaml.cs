// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Something
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Shapes;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using System.Windows.Data;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;
    using System.Windows.Media.Imaging;
    using System.Windows.Input;
    using System.Windows.Media.TextFormatting;

    public partial class MyClass : StackPanel
    {
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Add code for checking firing of events and such
            // For now just verifying compile and load, so log success
            
            System.Windows.Application.Current.Shutdown(0);
        }

        private void _On_Loaded(object sender, RoutedEventArgs e)
        {
            string id = (sender as FrameworkElement).Name;
            Text1.Text += id + " Loaded.";
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            string id = (sender as FrameworkElement).Name;
            Text1.Text += id + " Clicked.";
        }
        private void _OnStyleTarget_Loaded(object sender, RoutedEventArgs e)
        {
            string id = (sender as FrameworkElement).Name;
            Text1.Text += id + " StyleLoaded.";
        }
        private void _OnPreviewMouseDown(object sender, MouseEventArgs e)
        {
            string id = (sender as FrameworkElement).Name;
            Text1.Text += id + " MouseDown.";
        }

        private void OnCheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            string id = (sender as FrameworkElement).Name;
            Text1.Text += id + " Checked.";
        }
    }
}
