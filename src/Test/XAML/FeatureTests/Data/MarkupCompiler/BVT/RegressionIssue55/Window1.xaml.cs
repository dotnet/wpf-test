// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace RegressionIssue55
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify if binding on a custom attached property works if 
    /// the Binding(Path) constructor is used
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (textBlock1.Text == "1" && textBlock2.Text == "2")
            {
                Application.Current.Shutdown(0);
            }
            else
            {
                GlobalLog.LogEvidence("TextBlocks have unexpected values");
                Application.Current.Shutdown(-1);
            }
        }
    }

    public class DemoControl : Control
    {
        public static int GetDemoProperty(DependencyObject obj)
        {
            return (int)obj.GetValue(DemoPropertyProperty);
        }

        public static void SetDemoProperty(DependencyObject obj, int value)
        {
            obj.SetValue(DemoPropertyProperty, value);
        }

        public static readonly DependencyProperty DemoPropertyProperty =
            DependencyProperty.RegisterAttached("DemoProperty", typeof(int), typeof(DemoControl) );
    }
}
