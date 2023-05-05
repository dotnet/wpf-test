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

  public partial class UserControlClass : StackPanel
  {
    void OnLoaded (object sender, EventArgs e)
    {
	Console.WriteLine("About to click on the button.");
        UserInput.MouseLeftClickCenter(Button1);
	Console.WriteLine("Just clicked on the button.");
    }
  }

}
