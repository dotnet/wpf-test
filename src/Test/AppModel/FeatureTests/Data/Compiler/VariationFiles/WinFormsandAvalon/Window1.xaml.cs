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


namespace WindowsApplication2
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>

  public partial class Window1 : Window
  {
    public Window1()
    {
      InitializeComponent();
    }

    // To use Loaded event put Loaded="WindowLoaded" attribute in root element of .xaml file.
    // private void WindowLoaded(object sender, EventArgs e) {}

    // Sample event handler:  
    // private void ButtonClick(object sender, RoutedEventArgs e) {}

  }
}
