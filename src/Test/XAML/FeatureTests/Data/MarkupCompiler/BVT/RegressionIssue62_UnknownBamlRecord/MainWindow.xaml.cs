// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace RegressionIssue62_UnknownBamlRecord
{
    /// <summary>
    /// Regression test
    /// This test case verifies that TypeSerializerInfo BAML record is read correctly by the BAML reader
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    /// <summary>
    /// A user defined FrameworkTemplate forces the markup compiler to emit TypeSerializerInfo BAML record
    /// </summary>
    public class CustomFrameworkTemplate : ControlTemplate
    {
    }

    /// <summary>
    /// A user defined Style forces the markup compiler to emit TypeSerializerInfo BAML record
    /// </summary>
    public class CustomStyle : Style
    {
    }
}
