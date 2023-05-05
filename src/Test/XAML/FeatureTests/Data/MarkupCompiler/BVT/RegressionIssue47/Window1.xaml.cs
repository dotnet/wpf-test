// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RegressionIssue47
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            this.InitializeComponent();
        }

        private static BitmapImage s_img = null;
        public static BitmapImage Img
        {
            get
            {
                if (s_img == null)
                    s_img = new BitmapImage(new Uri(@"Puppies.jpg", UriKind.Relative));
                return s_img;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
