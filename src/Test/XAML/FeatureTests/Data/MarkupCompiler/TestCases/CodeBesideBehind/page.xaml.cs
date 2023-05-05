// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test.ComponentModel // Namespace must be the same as what you set in project file
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Test;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Input;

    public partial class CodeBesideBehindClass
    {
        Microsoft.Test.Logging.TestLog _log;

        void OnLoaded(object sender, EventArgs e)
        {
            LogManager.BeginTest(DriverState.TestName);
            _log = new TestLog("Compiled Exe");
            _log.LogStatus("About to click on the button with handler defined in Code-Beside.");
            UserInput.MouseLeftClickCenter(Button1);
            _log.LogStatus("Just clicked on the button.");
        }

        void HandleClickCodeBehind(object sender, RoutedEventArgs e)
        {
            _log.LogStatus("Verified click handler defined in Code-Behind works.");
            _log.Result = TestResult.Pass;
            _log.Close();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
