// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Reflection;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            string exePath = Assembly.GetExecutingAssembly().Location;
            string controlsDllFile = Path.Combine(exePath.Substring(0, exePath.LastIndexOf("\\")), @"..\..\Controls\bin\Release\Controls.dll");

            var controlAssembly = Assembly.LoadFrom(controlsDllFile);
            var controlType = controlAssembly.GetType("Controls.UserControl1");
            var control = (UIElement)Activator.CreateInstance(controlType); 
            grid.Children.Add(control);
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
