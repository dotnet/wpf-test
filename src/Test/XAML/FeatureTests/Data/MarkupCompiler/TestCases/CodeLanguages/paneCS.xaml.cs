// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test.ComponentModel // Namespace must be the same as what you set in project file
{

  using System;
  using System.Windows;
  using System.Windows.Controls;
  using Microsoft.Test.Logging;
  using Microsoft.Test.Input;

  public partial class CodeLanguagesCSClass
  {

    Microsoft.Test.Logging.TestLog _log = new Microsoft.Test.Logging.TestLog("CodeLanguageCS");

    void OnLoaded (object sender, EventArgs e)
    {
	_log.LogStatus("About to click on the button.");
        UserInput.MouseLeftClickCenter (testButton);
	_log.LogStatus("Just clicked on the button.");
    }

    void HandleClick(object sender, RoutedEventArgs e)
    {
	_log.LogStatus("Just entered click event handler.");
	_log.LogStatus("Verified click handler works when written in C#.");

	if (sender == testButton)
	{
		_log.LogStatus("Click event was sent by the correct button.");
		_log.Result = TestResult.Pass;
	}
	else
	{
		_log.LogStatus("Click event was sent by some source other than the clicked button.");
		_log.Result = TestResult.Fail;
	}
	_log.Close();

	System.Windows.Application.Current.Shutdown();
    }
  }
}
