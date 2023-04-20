// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test.ComponentModel // Namespace must be the same as what you set in project file
{

  using System;
  using System.Windows;
  using System.Windows.Controls;
  using Microsoft.Test.Logging;

  public partial class GlobalizationClass
  {

    Microsoft.Test.Logging.TestLog _log = new Microsoft.Test.Logging.TestLog("Globalization");

    void OnLoaded (object sender, EventArgs e)
    {

	bool hadFailure = false;

	_log.LogStatus("Able to compile xaml and code with foreign characters"); 

	if (GlobalText.Content as string == "Text using masculin, 按钮, ボタン, 단추, кнопка, κουμπί")
	{
	    _log.LogStatus("Able to verify foreign character contents of a control"); 
	}
	else
	{
	    _log.LogStatus("Unable to verify foreign character contents of a control");
	    hadFailure = true;
	}

	if (masculin.Content as string == "Language1")
	{
	    _log.LogStatus("Able to verify foreign character Name of a control"); 
	}
	else
	{
	    _log.LogStatus("Unable to verify foreign character Name of a control");
	    hadFailure = true;
	}

	if (按钮.Content as string == "Language2")
	{
	    _log.LogStatus("Able to verify foreign character Name of a control"); 
	}
	else
	{
	    _log.LogStatus("Unable to verify foreign character Name of a control");
	    hadFailure = true;
	}

	if (ボタン.Content as string == "Language3")
	{
	    _log.LogStatus("Able to verify foreign character Name of a control"); 
	}
	else
	{
	    _log.LogStatus("Unable to verify foreign character Name of a control");
	    hadFailure = true;
	}

	if (단추.Content as string == "Language4")
	{
	    _log.LogStatus("Able to verify foreign character Name of a control"); 
	}
	else
	{
	    _log.LogStatus("Unable to verify foreign character Name of a control");
	    hadFailure = true;
	}

	if (кнопка.Content as string == "Language5")
	{
	    _log.LogStatus("Able to verify foreign character Name of a control"); 
	}
	else
	{
	    _log.LogStatus("Unable to verify foreign character Name of a control");
	    hadFailure = true;
	}

	if (κουμπί.Content as string == "Language6")
	{
	    _log.LogStatus("Able to verify foreign character Name of a control"); 
	}
	else
	{
	    _log.LogStatus("Unable to verify foreign character Name of a control");
	    hadFailure = true;
	}

	if (hadFailure)
	{
	    _log.Result = TestResult.Fail;
	}
	else
	{
	    _log.Result = TestResult.Pass;
	}
	_log.Close();

	System.Windows.Application.Current.Shutdown();

    }

  }
}
