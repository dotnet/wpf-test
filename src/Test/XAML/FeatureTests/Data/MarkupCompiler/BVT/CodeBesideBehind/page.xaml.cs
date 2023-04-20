// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test.ComponentModel // Namespace must be the same as what you set in project file
{

  using System;
  using System.Windows;
  using System.Windows.Controls;
  using Microsoft.Test;
  using Microsoft.Test.Input;

  public partial class CodeBesideBehindClass
  {
    // Right now the verification structure is:
    // Start in OnLoaded handler and call to a method in Code-Beside which calls back to method in Code-Behind.
    // Then click on button with handler defined in Code-Beside which clicks on button with handler defined in
    // Code-Behind. If a step along the way fails, won't follow path and will fail on timeout. So if you are
    // analyzing for a failure, look at the last stage it did get to.
    void OnLoaded (object sender, RoutedEventArgs e)
    {
        MethodInCodeBeside();
        Console.WriteLine("About to click on the button with handler defined in Code-Beside.");
        UserInput.MouseLeftClickCenter (Button1);
        Console.WriteLine("Just clicked on the button.");
    }

    void HandleClickCodeBehind(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("We verified click handler defined in Code-Behind works.");

        // Made it through sequence, so close.
        System.Windows.Application.Current.Shutdown(0);
    }

    void MethodInCodeBehind()
    {
        Console.WriteLine("We verified a method defined in Code-Behind can be called from Code-Beside.");
    }
  }
}
