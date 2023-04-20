// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test.ComponentModel
{
  using System;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Controls;
  using Avalon.Test.ComponentModel;
  using Microsoft.Test;
  using Microsoft.Test.Input;

  public partial class PropertyTriggerClass : StackPanel
  {
    void OnLoaded (object sender, EventArgs e)
    {
        int exitCode = 0;

	Console.WriteLine("About to mouseover the button.");
        UserInput.MouseMove(Button1, 1, 1);
	QueueHelper.WaitTillQueueItemsProcessed();
	Console.WriteLine("Just mouseovered the button.");

        BrushConverter scbc = new BrushConverter();

	if (Button1.Foreground.ToString() != (scbc.ConvertFromInvariantString("Green")).ToString())
	{
		Console.WriteLine("Button foreground should be green but was " + Button1.Foreground.ToString());
		exitCode = 1;
	}
	else if (Button1.Background.ToString() != (scbc.ConvertFromInvariantString("Red")).ToString())
	{
		Console.WriteLine("Button background should be red but was " + Button1.Background.ToString());
		exitCode = 1;
	}
	else
        {
		Console.WriteLine("Button colors are as expected after mouseover.");
		exitCode = 0;
        }

	Application.Current.Shutdown(exitCode);
    }
  }
}
