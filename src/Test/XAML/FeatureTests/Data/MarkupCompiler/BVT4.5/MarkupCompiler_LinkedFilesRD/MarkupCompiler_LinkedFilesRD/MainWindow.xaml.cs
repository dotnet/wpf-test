// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;

namespace MarkupCompiler_LinkedFilesRD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SolidColorBrush scb = (SolidColorBrush)this.Background;
            if (scb.Color != Colors.Green)
            {
                Console.WriteLine("Window background is not green. It is " + this.Background.ToString());
                Application.Current.Shutdown(1);
            }
            else
            {
                Console.WriteLine("Window background is green.");
                Application.Current.Shutdown(0);
            }
        }
    }
}
