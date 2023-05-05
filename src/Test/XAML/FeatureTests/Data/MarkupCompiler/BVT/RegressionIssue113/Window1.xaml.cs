// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;

namespace RegressionIssue113
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify that UserControl event handler works fine if set inside a template
    /// </summary>
    public partial class Window1 : Window
    {
        private bool _hitInitEvent = false;

        public Window1()
        {
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            _hitInitEvent = true;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Test.Logging.TestLog log = TestLog.Current;
            if (_hitInitEvent == true)
            {
                log.Result = TestResult.Pass;
                Application.Current.Shutdown(0);
            }
            else
            {
                GlobalLog.LogEvidence("Window.Initialized event registered by UserControl is not hit");
                log.Result = TestResult.Fail;
                Application.Current.Shutdown(-1);
            }
        }
    }
}
