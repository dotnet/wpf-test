// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyUserControl
{
    public class UserControl3 : UserControl
    {
        public System.Windows.Controls.Button bt1 = new Button();
        public System.Windows.Controls.Button bt2 = new Button();
        public System.Windows.Controls.Button bt3 = new Button();
        System.Windows.Controls.Canvas _cv = new Canvas();

        public UserControl3()
        {
            _cv.Background = Brushes.Orange;
            this.Loaded += new RoutedEventHandler(UserControlLoaded);
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            bt1.Content = "Button 1";
            bt1.Name = "AVButton1";
            Canvas.SetTop(bt1, 0);
            _cv.Children.Add(bt1);
            bt2.Content = "Button 2";
            bt2.Name = "AVButton2";
            Canvas.SetTop(bt2, 25);
            _cv.Children.Add(bt2);
            bt3.Content = "Button 3";
            bt3.Name = "AVButton3";
            Canvas.SetTop(bt3, 50);
            _cv.Children.Add(bt3);
            this.AddChild(_cv);
            bt1.TabIndex = 0;
            bt2.TabIndex = 1;
            bt3.TabIndex = 2;
        }
    }
}