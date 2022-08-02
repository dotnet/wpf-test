// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections;

namespace DrtIcon {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window {
        public Window1() {
            //InitializeComponent();
        }

        // To use Loaded event put Loaded="WindowLoaded" attribute in root element of .xaml file.
        // private void WindowLoaded(object sender, RoutedEventArgs e) {}

        private void ContentRenderedHandler(object sender, EventArgs e)
        {
            DrtIconApp myApp = Application.Current as DrtIconApp;
            
            if (myApp.Hold)
            {
                return;
            }
            
            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback) delegate (object notused)
                {
                    this.Icon = BitmapFrame.Create(new Uri(@"DrtFiles\Icon\heart.ico", UriKind.RelativeOrAbsolute));
                    return null;
                },
                null);

            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback)delegate(object notused)
                {
                    var bmp = BitmapFrame.Create(new Uri(@"DrtFiles\Icon\heart.ico", UriKind.RelativeOrAbsolute));
                    var newBmp = BitmapFrame.Create(bmp);
                    this.Icon = newBmp;
                    return null;
                },
                null);

            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback) delegate (object notused)
                {
                    this.Icon = null;
                    return null;
                },
                null);

            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback) delegate (object notused)
                {
                    this.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/cart.ico", UriKind.RelativeOrAbsolute));
                    return null;
                },
                null);

            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback)delegate(object notused)
                {
                    this.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/arp16.png", UriKind.RelativeOrAbsolute));
                    return null;
                },
                null);

            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback) delegate (object notused)
                {
                    this.Close();
                    return null;
                },
                null);
        }

        // this is used for manual testing.
        private void ClickHandler(object sender, RoutedEventArgs e) 
        {
            _clickCount++;

            // test setting in code
            if (_clickCount == 1)
            {
                this.Icon = BitmapFrame.Create(new Uri(@"DrtFiles\Icon\heart.ico", UriKind.RelativeOrAbsolute));
            }
            else if (_clickCount == 2)
            {
                // test setting to null
                this.Icon = null;
            }
            else if (_clickCount == 3)
            {
                this.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/cart.ico", UriKind.RelativeOrAbsolute));
            }
            else if (_clickCount == 4)
            {
                this.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/arp16.png", UriKind.RelativeOrAbsolute));
                _clickCount = 0;
            }
        }

        private int _clickCount;
    }
}
