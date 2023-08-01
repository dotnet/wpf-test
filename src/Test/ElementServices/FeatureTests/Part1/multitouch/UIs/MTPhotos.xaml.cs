// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using System.Windows.Threading;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Interaction logic for MTGesture.xaml - custom control 
    /// </summary>
    public partial class MTPhotos : Window
    {
        public MTPhotos()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;

            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var photoDirInfo = new DirectoryInfo(System.IO.Path.Combine(currentDir, "photos"));

            var random = new Random();
            int zOrder = 0;

            foreach (var file in photoDirInfo.GetFiles("*.jpg"))
            {
                var photo = new PhotoControl() { ImagePath = new Uri(file.FullName) };

                photo.Loaded += new RoutedEventHandler(delegate(object s, RoutedEventArgs args)
                {
                    var w = photo.ActualWidth;
                    var h = photo.ActualHeight;
                    Canvas.SetLeft(photo, (ActualWidth - w) * random.NextDouble());
                    Canvas.SetTop(photo, (ActualHeight - h) * random.NextDouble());

                }
                );
                Canvas.SetZIndex(photo, zOrder++);

                _Canvas.Children.Add(photo);
            }
        }
    }
}
