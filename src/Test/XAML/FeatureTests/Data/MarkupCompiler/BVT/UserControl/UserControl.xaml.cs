// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test.ComponentModel // Namespace must be the same as what you set in project file
{

  using System;
  using System.Windows;
  using System.Windows.Controls;

  public partial class MyUserControl : UserControl
  {
      public MyUserControl()
      {
          InitializeComponent();
      }

      void HandleClick(object sender, RoutedEventArgs e)
      {
          Console.WriteLine("Verified click handler works for clicking on a UserControl.");
          System.Windows.Application.Current.Shutdown(0);
      }
  }

}
