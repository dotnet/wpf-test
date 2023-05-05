// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MarkupCompiler.RegressionIssue42
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify if internal types can be used in XAML
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }

    internal class Person
    {
        private string _first = null;
        public string First
        {
            get
            {
                return _first;
            }
            set
            {
                _first = value;
            }
        }

    }
}
